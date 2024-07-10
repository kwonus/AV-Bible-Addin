using AVX.Serialization;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Xml.Linq;
using Word = Microsoft.Office.Interop.Word;

namespace AVX
{
    public abstract class CAPS
    {
        public const UInt16 Cap1st  = 0x8000;
        public const UInt16 CapAll  = 0x4000;
        public const UInt16 CapAny  = 0xC000;
        public const UInt16 CapNone = 0x0000;

        public static string Captitalize(UInt16 cap, string input)
        {
            switch (cap & CapAny)
            {
                case Cap1st:  return input.Substring(0,1).ToUpper() + ((input.Length > 0) ? input.Substring(1) : "");
                case CapAll:  return input.ToUpper();
                default:      return input;
            }
        }
    }
    public abstract class TX
    {
        public const UInt16 BoV = 0x20;
        public const UInt16 BoC = 0x60;
        public const UInt16 BoB = 0xE0;

        public const UInt16 EoC = 0x70;
        public const UInt16 EoB = 0xF0;
    }
    public abstract class PUNC
    {
        public const UInt16 CLAUSE = 0xE0;
        public const UInt16 Exclamatory = 0x80;
        public const UInt16 Interrogative = 0xC0;
        public const UInt16 Declarative = 0xE0;
        public const UInt16 Dash = 0xA0;
        public const UInt16 Semicolon = 0x20;
        public const UInt16 Comma = 0x40;
        public const UInt16 Colon = 0x60;
        public const UInt16 Possessive = 0x10;
        public const UInt16 ParenClose = 0x0C;
        public const UInt16 Parenthetical = 0x04;
        public const UInt16 Italics = 0x02;
        public const UInt16 Jesus = 0x01;

        public static string PostfixPunctuation(string word, byte punc)
        {
            string postfix;
            if ((punc & Possessive) == Possessive)
                postfix = word != null && word.EndsWith("s") ? "'" : "'s";
            else
                postfix = "";

            if ((punc & ParenClose) == ParenClose)
                postfix += ")";

            switch (punc & CLAUSE)
            {
                case Exclamatory:   return postfix + "!";
                case Interrogative: return postfix + "?";
                case Declarative:   return postfix + ".";
                case Dash:          return postfix + "--";
                case Semicolon:     return postfix + ";";
                case Comma:         return postfix + ",";
                case Colon:         return postfix + ":";
            }
            return postfix;
        }
        public static string PrefixPunctuation(byte punc, byte prev)
        {
            if ((punc & Parenthetical) == Parenthetical && (prev & Parenthetical) != Parenthetical)
                return "(";
            return "";
        }
        public static bool IsItalisized(byte punc)
        {
            return ((punc & Italics) == Italics);
        }
    }
    /// <summary>
    /// Interaction logic for SelectVerse.xaml
    /// </summary>
    public partial class InsertVerses : System.Windows.Window
    {
        // Constants from winuser.h
        private const int GWL_STYLE = -16;
        private const int WS_MAXIMIZEBOX = 0x10000;
        private const int WS_MINIMIZEBOX = 0x20000;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int value);

        private void HideMinimizeAndMaximizeButtons()
        {
            IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            var currentStyle = GetWindowLong(hwnd, GWL_STYLE);
            SetWindowLong(hwnd, GWL_STYLE, (currentStyle & ~WS_MAXIMIZEBOX & ~WS_MINIMIZEBOX));
        }
        private string SearchSpec;  // this form can be called from SearchForm to allow for custom verse variant

        internal static bool PositionForm(InsertVerses form)
        {
            bool repositioned = false;

            if (double.IsNaN(form.Top))
            {
                form.Top = form.PointToScreen(Mouse.GetPosition(null)).Y;
                repositioned = true;
            }
            if (double.IsNaN(form.Left))
            {
                form.Left = form.PointToScreen(Mouse.GetPosition(null)).X;
                repositioned = true;
            }
//          Screen[] screens = Screen.AllScreens;
            return repositioned;
        }
        private static (double top, double left, double height, double width) Coordinates = (0, 0, 0, 0);

        public static void ShowForm(InsertVerses form)
        {
            if (form != null)
            {
                form.button.IsEnabled = false;
                form.WindowStartupLocation = WindowStartupLocation.Manual;
                form.SearchSpec = null;

                // First Insert* ShowForm()
                //
                if (InsertVerses.Coordinates.height > 0.0 && InsertVerses.Coordinates.width > 0.0)
                {
                    form.Top = Coordinates.top;
                    form.Left = Coordinates.left;
                    form.Height = Coordinates.height;
                    form.Width = Coordinates.width;
                }
                else
                {
                    form.ResetCoordinates();
                }
                // User may not have closed previous form, grab its coordinates and hide previous form
                //
                if (form == InsertVerses.InsertNT)
                {
                    form.ReplaceForm(InsertVerses.InsertOT);
                    form.ReplaceForm(InsertVerses.InsertAny);
                }
                else if (form == InsertVerses.InsertOT)
                {
                    form.ReplaceForm(InsertVerses.InsertNT);
                    form.ReplaceForm(InsertVerses.InsertAny);
                }
                else if (form == InsertVerses.InsertAny)
                {
                    form.ReplaceForm(InsertVerses.InsertOT);
                    form.ReplaceForm(InsertVerses.InsertNT);
                }

                form.Show();
                form.HideMinimizeAndMaximizeButtons();

                if (InsertVerses.PositionForm(form))
                {
                    Coordinates.top = form.Top;
                    Coordinates.left = form.Left;
                }
                Ribbon.BringToTop(form);
                form.textBoxChaterAndVerse_KeyUp(null, null);
            }
        }
        public static void ShowForm(byte bkNum, string searchSpec = null)
        {
            var form = InsertAny;
            form.SearchSpec = searchSpec;

            if (bkNum >= 1 && bkNum <= 66)
                form.comboBoxBook.SelectedItem = form.comboBoxBook.Items.GetItemAt(bkNum - 1);

            InsertVerses.ShowForm(form);
        }
        public static bool ForceClose = false; // Indicate if it is an explicit close request
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            if (ForceClose)
                return;

            this.ResetCoordinates();

            e.Cancel = true;
            this.Hide();
        }
        private void ResetCoordinates()
        {
            Coordinates.top = this.Top;
            Coordinates.left = this.Left;
            Coordinates.height = this.Height;
            Coordinates.width = this.Width;
        }
        public void ReplaceForm(InsertVerses old)
        {
            if (old.Visibility == Visibility.Visible)
            {
                old.ResetCoordinates();
                old.Hide();

                this.Top = Coordinates.top;
                this.Left = Coordinates.left;
                this.Height = Coordinates.height;
                this.Width = Coordinates.width;
            }
        }
        public static InsertVerses InsertAny { get; private set; } = new InsertVerses(ot:true,  nt:true);
        public static InsertVerses InsertNT  { get; private set; } = new InsertVerses(ot:false, nt:true);
        public static InsertVerses InsertOT  { get; private set; } = new InsertVerses(ot:true,  nt:false);

        private void WriteVerseSpec()
        {
            ComboBoxItem item = (ComboBoxItem)this.comboBoxBook.SelectedItem;
            var bk = item.Tag.ToString().Substring(2);
            var b = byte.Parse(bk);
            var info = BookInfo.GetBook(b);
            string spec = info.Name + " " + this.textBoxChapterAndVerse.Text.Trim().Replace(" ", "").Replace(",", ", ");
            if (this.modernize.IsChecked == true)
                spec += "  (AVX)";
            dynamic rng = Ribbon.AVX.Application.ActiveDocument.Range();
            rng.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
            rng.Bold = 1;
            rng.Text = spec + "\n";
            foreach (Word.Range w in rng.Words)
            {
                w.Font.Bold = 1;
            }
        }

        private InsertVerses(bool ot = false, bool nt = false)
        {
            InitializeComponent();
            this.SearchSpec = null;

            if (this.comboBoxBook.Items.Count > 66) // Why?
            {
                for (int i = this.comboBoxBook.Items.Count - 1; i >= 66; i--)
                {
                    this.comboBoxBook.Items.RemoveAt(i);
                }
            }
            if (nt != ot && this.comboBoxBook.Items.Count == 66)
            {
                if (ot)
                {
                    this.Title = this.Title.Replace("Verses", "OT Verses");
                    for (int i = 40; i <= 66; i++)
                    {
                        this.comboBoxBook.Items.RemoveAt(39);
                    }
                }
                else
                {
                    this.Title = this.Title.Replace("Verses", "NT Verses");
                    for (int i = 1; i <= 39; i++)
                    {
                        this.comboBoxBook.Items.RemoveAt(0);
                    }
                }
            }
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.textBoxChapterAndVerse.Text = "";
            if (this.comboBoxBook.SelectedItem != null)
            {
                ComboBoxItem item = (ComboBoxItem) this.comboBoxBook.SelectedItem;
                var bk = item.Tag.ToString().Substring(2);
                var b = byte.Parse(bk);
                var book = BookInfo.GetBook(b);
                string bname = item.Content.ToString();
                this.info.Text = bname + " has " + ((uint)book.ChapterCount).ToString() + " chapters";
            }
        }

        private void textBoxChaterAndVerse_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (this.comboBoxBook.SelectedItem == null)
                return;

            ComboBoxItem item = (ComboBoxItem)this.comboBoxBook.SelectedItem;
            var bk = item.Tag.ToString().Substring(2);
            var b = (byte)UInt16.Parse(bk);
            var book = BookInfo.GetBook(b);
            string bname = item.Content.ToString();
            this.info.Text = bname + " has " + ((uint)book.ChapterCount).ToString() + " chapters";

            this.button.IsEnabled = false;

            if (this.textBoxChapterAndVerse.Text.Length > 0)
            {
                var text = this.textBoxChapterAndVerse.Text.Trim();
                var len = text.Length;

                UInt16 ch = 0;
                for (int i = 0; i < len; i++)
                {
                    if (text[i] >= '0' && text[i] <= '9')
                    {
                        ch *= 10;
                        ch += (byte)((byte)text[i] - (byte)'0');
                    }
                    else break;
                }
                if (ch >= 1 && ch <= book.ChapterCount)
                {
                    this.button.IsEnabled = true;
                    this.info.Text += ("\nChapter " + ch.ToString() + " has " + book.VerseCountsByChapter[ch].ToString() + " verses");
                }
            }
        }
        private static HttpClient AVAPI = new HttpClient()
        {
            BaseAddress = new Uri("https://github.com/kwonus"),
        };
        private void button_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem item = (ComboBoxItem)this.comboBoxBook.SelectedItem;
            if (item != null)
            {
                string bad = null;
                string error = null;

                var list = new List<byte>();

                var bk = item.Tag.ToString().Substring(2);
                var b = byte.Parse(bk);
                var info = BookInfo.GetBook(b);
                string bname = info.Name;
                byte c = 0;

                if (this.textBoxChapterAndVerse.Text.Length > 0)
                {
                    var text = this.textBoxChapterAndVerse.Text.Trim();
                    var len = text.Length;
                    int colon = -1;
                    UInt16 ch = 0;
                    for (int i = 0; i < len; i++)
                    {
                        if (text[i] >= '0' && text[i] <= '9')
                        {
                            ch *= 10;
                            ch += (byte)((byte)text[i] - (byte)'0');
                        }
                        else
                        {
                            if (text[i] == ':')
                                colon = i;
                            break;
                        }
                    }
                    if (ch >= 1 && ch <= info.ChapterCount)
                    {
                        c = (byte)ch;
                        this.info.Text += ("\nChapter " + ch.ToString() + " has " + ((uint)info.VerseCountsByChapter[ch]).ToString() + " verses");

                        if (colon > 0 && colon+1 < len)
                        {
                            UInt16 val;
                            byte[] vals = new byte[2];
                             var sections = text.Substring(colon + 1).Split(',');
                            foreach (var section in sections)
                            {
                                var range = section.Split('-');
                                if (range.Length == 1)
                                {
                                    bad = range[0];
                                    if (UInt16.TryParse(bad, out val) && val <= info.VerseCountsByChapter[ch] && val >= 1)
                                    {
                                        list.Add((byte)val);
                                        bad = null;
                                    }
                                    else break;
                                }
                                else if (range.Length == 2)
                                {
                                    vals[0] = vals[1] = 0;
                                    for (int i = 0; i < 2; i++)
                                    {
                                        bad = range[i];
                                        if (UInt16.TryParse(bad, out val) && val <= info.VerseCountsByChapter[ch] && val >= 1)
                                        {
                                            vals[i] = (byte)val;
                                            bad = null;
                                        }
                                        else break;
                                    }
                                    if (bad != null)
                                        break;
                                    if (vals[0] > vals[1])
                                    {
                                        error = "Invalid verse range specified";
                                        break;
                                    }
                                    for (byte v = vals[0]; v <= vals[1]; v++)
                                    {
                                        list.Add(v);
                                    }
                                }
                            }
                            if (error != null)
                            {
                                this.info.Text = error;
                            }
                            else if (bad != null)
                            {
                                this.info.Text = "'" + bad + "' could not be interpretted as a verse (or it is out of range for "
                                    + item.Content.ToString()
                                    + " Chapter " + ch.ToString();
                            }
                        }
                        else
                        {
                            for (byte v = 1; v <= info.VerseCountsByChapter[ch]; v++)
                                list.Add(v);
                        }
                    }
                    else
                    {
                        this.info.Text = "You must specify a chapter between 1 and " + ((uint)info.ChapterCount).ToString() + " (inclusive)";
                    }
                }
                if (bad != null)
                {
                    this.Status.Text = bad;
                    return;
                }
                if (error != null)
                {
                    this.Status.Text = error;
                    return;
                }
                this.Status.Text = "";

                int idx = 0;
                DataStream[] words = ThisAddIn.API.InsertDetails(new Dictionary<string, string>(), info, c);

                if (words != null)
                {
                    this.Status.Foreground = new SolidColorBrush(Colors.Black);
                    this.Status.Text = "";
                    this.WriteVerseSpec();

                    if (list.Count == 1)
                    {
                        byte v = list[0];
                        do
                        {
                            DataStream word = words[idx];

                            if (word.Coordinates.V == v)
                            {
                                ThisAddIn.WriteVerse(b, words, idx, this.modernize.IsChecked.Value, squelchHighlights: true);
                                break;
                            }
                            idx += word.Coordinates.WC;

                        } while (idx < words.Length);
                        dynamic rng = Ribbon.AVX.Application.ActiveDocument.Range();
                        rng.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                        this.info.Text = "";
                        this.Close();
                    }
                    else if (list.Count > 1)
                    {
                        int cnt = 0;
                        foreach (byte v in list)
                        {
                            DataStream word = words[idx];

                            while (word.Coordinates.V < v && word != null)
                            {
                                idx += word.Coordinates.WC;
                                word = idx < words.Length ? words[idx] : null;
                            }
                            if (word != null && word.Coordinates.V == v)
                            {
                                ThisAddIn.WriteInlineVerse(b, words, idx, this.modernize.IsChecked.Value, (++cnt == 1), squelchHighlights: true);
                                idx += word.Coordinates.WC;
                            }
                            else // something went wrong
                            {
                                break;
                            }
                        }
                        dynamic rng = Ribbon.AVX.Application.ActiveDocument.Range();
                        rng.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

                        this.info.Text = "";
                        this.Close();
                    }
                }
                else
                {
                    string revison = ThisAddIn.API.GetRevision();
                    if (string.IsNullOrWhiteSpace(revison))
                    {
                        this.Status.Foreground = new SolidColorBrush(Colors.Maroon);
                        this.Status.Text = "AV Data-Manager is not running. See User-Help: 'Getting Started'";
                        return;
                    }
                }
            }
            else
            {
                this.Status.Foreground = new SolidColorBrush(Colors.Maroon);
                this.info.Text = "You must first select a book from the list prior to attempting to inserting verses.";
                this.Status.Text = this.info.Text;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.KeyUp += new System.Windows.Input.KeyEventHandler(MainWindow_KeyUp);
        }

        void MainWindow_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                button_Click(null, null);
            }
        }
    }
}
