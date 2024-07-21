using Microsoft.Office.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Office = Microsoft.Office.Core;
using System.Drawing;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;
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
        private Office.IRibbonUI ribbon;
 
        private ThisAddIn avx;
        public static Ribbon RIBBON { get; private set; } = null;
        public static ThisAddIn AVX { get; private set; } = null;
        public Ribbon(ThisAddIn avx)
        {
            Ribbon.RIBBON = this;
            this.avx = avx;
            Ribbon.AVX = this.avx;

            Assembly assembly = Assembly.GetExecutingAssembly();

            //            this.CommonFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86), "Digital-AV");
            //            var path = Path.Combine(this.CommonFolder, "AV-Writ-32.dx");
            //            var file = File.OpenRead(path);

            var len = 0;// file.Length;
            var cnt = (UInt32) (len / 4);
//            if (len != (0xC0C93) * 4)
//                MessageBox.Show(null, "AV-Bible Addin for Microsoft Word", "Possible File corruption error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #region IRibbonExtensibility Members

        public string GetCustomUI(string ribbonID)
        {
            var xml = GetResourceText("AVX.Ribbon.xml");
            return xml;
        }
        public Bitmap GetImage(IRibbonControl control)
        {
            if (control.Tag == "OT" || control.Tag == "NT")
                return ThisAddIn.BIBLE;
            if (control.Tag == "FIND")
                return ThisAddIn.FIND;
            if (control.Tag == "HELP")
                return ThisAddIn.HELP;
            if (control.Tag == "ABOUT")
                return ThisAddIn.INFO;
            if (control.Tag == "CFG")
                return ThisAddIn.CFG;

            return ThisAddIn.BOOK;
        }
        public void clickRef(Office.IRibbonControl control)
        {
            if (control.Tag == "NT")
            {
                InsertVerses.ShowForm(InsertVerses.InsertNT);
            }
            else if (control.Tag == "OT")
            {
                InsertVerses.ShowForm(InsertVerses.InsertOT);
            }
            else
            {
                InsertVerses.ShowForm(InsertVerses.InsertAny);
            }
        }
        public void clickAbout(Office.IRibbonControl control)
        {
            AboutInfo.AboutForm.Show();
            try
            {
                Ribbon.BringToTop(AboutInfo.AboutForm);
            }
            catch
            {
                ;
            }
        }
        public void clickCfg(Office.IRibbonControl control)
        {
            Settings settings = new Settings();
            settings.Show();
            try
            {
                Ribbon.BringToTop(settings);
            }
            catch
            {
                ;
            }
        }
        public void clickFind(Office.IRibbonControl control)
        {
            FindVerses.SearchForm.Show();
            try
            {
                Ribbon.BringToTop(FindVerses.SearchForm);
            }
            catch
            {
                ;
            }
        }
        public void clickHelp(Office.IRibbonControl control)
        {
            HelpWindow.HelpForm.Show();
            try
            {
                Ribbon.BringToTop(HelpWindow.HelpForm);
            }
            catch
            {
                ;
            }
        }
        public void clickBook(Office.IRibbonControl control)
        {
            try
            {
                var num = control.Id.Substring(2);
                var bk = UInt16.Parse(num);
//              MessageBox.Show(null, "Book-" + num + " = " + control.Tag, "Book Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                InsertVerses.ShowForm((byte)bk);
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
