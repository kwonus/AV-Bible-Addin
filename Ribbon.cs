using AVX.Properties;
using Microsoft.Office.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Office = Microsoft.Office.Core;
using System.Drawing;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;
using System.Windows.Resources;
using System.Windows.Interop;

// TODO:  Follow these steps to enable the Ribbon (XML) item:

// 1: Copy the following code block into the ThisAddin, ThisWorkbook, or ThisDocument class.

//  protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
//  {
//      return new Ribbon();
//  }

// 2. Create callback methods in the "Ribbon Callbacks" region of this class to handle user
//    actions, such as clicking a button. Note: if you have exported this Ribbon from the Ribbon designer,
//    move your code from the event handlers to the callback methods and modify the code to work with the
//    Ribbon extensibility (RibbonX) programming model.

// 3. Assign attributes to the control tags in the Ribbon XML file to identify the appropriate callback methods in your code.  

// For more information, see the Ribbon XML documentation in the Visual Studio Tools for Office Help.


namespace AVX
{
    [ComVisible(true)]
    public class Ribbon : Office.IRibbonExtensibility
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        public static void BringToTop(System.Windows.Window form)
        {
            form.Show();
            form.BringIntoView();

            IntPtr hwnd = new WindowInteropHelper(form).Handle;
            SetForegroundWindow(hwnd);

        }
        public FindVerses SearchForm;
        public SelectVerse BrowseForm;

        private string CommonFolder;
        private Office.IRibbonUI ribbon;
        public (UInt32 idx, byte ccnt)[] bkIdx { get; private set; }
        public Dictionary<byte, Dictionary<byte, (UInt32 writIdx, byte vcnt)>> chIdx { get; private set; }
        public (UInt16 word, byte punc, byte tx)[] writ { get; private set; }

        public Dictionary<UInt16, string> Search;
        public Dictionary<UInt16, string> Display;
        public Dictionary<UInt16, string> Modern;

        private ThisAddIn avx;
        public static Ribbon RIBBON { get; private set; } = null;
        public static ThisAddIn AVX { get; private set; } = null;
        public Ribbon(ThisAddIn avx)
        {
            Ribbon.RIBBON = this;
            this.avx = avx;
            Ribbon.AVX = this.avx;

            Assembly assembly = Assembly.GetExecutingAssembly();

            this.CommonFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86), "Digital-AV");
            var path = Path.Combine(this.CommonFolder, "AV-Writ-32.dx");
            var file = File.OpenRead(path);

            var len = file.Length;
            var cnt = (UInt32) (len / 4);
            if (len != (0xC0C93) * 4)
                MessageBox.Show(null, "AVX Addin for Microsoft Word", "Possible File corruption error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            this.writ = new (UInt16 word, byte punc, byte tx)[cnt];
            this.bkIdx = new (UInt32 idx, byte ccnt)[1 + 66];  // waste element-zero for a one-based index
            this.chIdx = new Dictionary<byte, Dictionary<byte, (UInt32 writIdx, byte vcnt)>>();
            for (byte num = 1; num <= 66; num++)
                this.chIdx[num] = new Dictionary<byte, (UInt32 writIdx, byte vcnt)>();

            byte b = 0;
            byte c = 0;
            byte v = 0;

            var reader = new BinaryReader(file);
            UInt32 cidx = 0;
            UInt32 vidx = 0;
            (UInt16 word, byte punc, byte tx) record;
            for (UInt32 i = 0; i < cnt; i++)
            {
                record.word = reader.ReadUInt16();
                record.punc = reader.ReadByte();
                record.tx   = reader.ReadByte();
                this.writ[i] = record;

                switch (record.tx & 0xF0)
                {
                    case TX.BoV:
                        v++;
                        break;
                    case TX.BoC:
                        v = 1;
                        c++;
                        cidx = i;
                        break;
                    case TX.BoB:
                        v = 1;
                        c = 1;
                        b++;
                        cidx = i;
                        break;
                    case TX.EoC:
                        this.chIdx[b][c] = (cidx, v);
                        break;
                    case TX.EoB:
                        this.chIdx[b][c] = (cidx, v);
                        this.bkIdx[b] = (vidx, c);
                        break;
                }
            }
            reader.Close();
            file.Close();

            path = Path.Combine(this.CommonFolder, "AV-Lexicon.dxi");
            this.Search = new Dictionary<UInt16, string>();
            this.Display = new Dictionary<UInt16, string>();
            this.Modern = new Dictionary<UInt16, string>();

            file = File.OpenRead(path);
            reader = new BinaryReader(file);
            char[] word = new char[24];

            for (UInt16 key = 1; key <= 12567; key++)
            {
                reader.ReadUInt16();
                var size = reader.ReadUInt16();
                if (size == 12567)
                    break;
                UInt32 ignore;
                for (int s = 0; s < size; s++)
                    ignore = reader.ReadUInt32();
                for (int w = 1; w <= 3; w++)
                {
                    for (int i = 0; i < word.Length; i++)
                    {
                        word[i] = (char)file.ReadByte();
                        if (word[i] == (char) 0)
                        {
                            if (i > 0)
                            {
                                var text = new string(word).Substring(0,i);
                                switch (w)
                                {
                                    case 1: Search[key]  = text; break;
                                    case 2: Display[key] = text; break;
                                    case 3: Modern[key]  = text; break;
                                }
                            }
                            break;
                        }
                    }
                }
            }
            reader.Close();
            file.Close();
        }

        #region IRibbonExtensibility Members

        public string GetCustomUI(string ribbonID)
        {
            var xml = GetResourceText("AVX.Ribbon.xml");
            return xml;
        }
        private Bitmap BitmapAVX = null;
        public Bitmap GetImageAVX(IRibbonControl control)
        {
            var image = Path.Combine(this.CommonFolder, "avx64.png");
            return (BitmapAVX != null) ? BitmapAVX : BitmapAVX = new Bitmap(image);
        }
        public void clickButtonAVX(Office.IRibbonControl control)
        {
            try
            {
                try
                {
                    if (this.BrowseForm != null)
                    {
                        Ribbon.BringToTop(this.BrowseForm);
                        return;
                    }
                }
                catch
                {
                    ;
                }
                this.BrowseForm = new SelectVerse();
                this.BrowseForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(null, "Please open a document prior to inserting verses", "Cannot insert text with a current document", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void clickButtonSearch(Office.IRibbonControl control)
        {
            try
            {
                try
                {
                    if (this.SearchForm != null)
                    {
                        Ribbon.BringToTop(this.SearchForm);
                        return;
                    }
                }
                catch
                {
                    ;
                }
                this.SearchForm = new FindVerses();
                this.SearchForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(null, "Please open a document prior to searching", "Cannot insert text with a current document", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void clickBookAVX(Office.IRibbonControl control)
        {
            try
            {
                var num = control.Id.Substring(2);
                var bk = UInt16.Parse(num);
//              MessageBox.Show(null, "Book-" + num + " = " + control.Tag, "Book Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                var popup = new SelectVerse((byte)bk);
                popup.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(null, "Please open a document prior to inserting verses", "Cannot insert text with a current document", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Ribbon Callbacks
        //Create callback methods here. For more information about adding callback methods, visit https://go.microsoft.com/fwlink/?LinkID=271226

        public void Ribbon_Load(Office.IRibbonUI ribbonUI)
        {
            this.ribbon = ribbonUI;
        }

        #endregion

        #region Helpers

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        #endregion
    }
}
