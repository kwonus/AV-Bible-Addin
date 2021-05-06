using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
    public partial class SelectVerse : Window
    {
        private void WriteVerseSpec()
        {
            ComboBoxItem item = (ComboBoxItem)this.comboBoxBook.SelectedItem;
            var bk = item.Tag.ToString().Substring(2);
            var b = (byte)UInt16.Parse(bk);
            var book = Ribbon.RIBBON.bkIdx[b];
            string spec = item.Content.ToString() + " " + this.textBoxChaterAndVerse.Text.Trim().Replace(" ", "").Replace(",", ", ") + "\n";
            dynamic rng = Ribbon.AVX.Application.ActiveDocument.Range();
            rng.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
            rng.Bold = 1;
            rng.Text = spec;
            foreach (Word.Range w in rng.Words)
            {
                w.Font.Bold = 1;
            }
        }
        private void WriteVerse(byte b, byte c, byte v, bool modern, bool contiguous)
        {
            byte prevPunc = 0;
            var chapter = Ribbon.RIBBON.chIdx[b][c];
            var records = Ribbon.RIBBON.writ;
            UInt32 r = chapter.writIdx;
            for (int i = 1; i < v; /**/)
            {
                r++;
                if ((records[r].tx & 0x70) == 0x20) // BoV
                    i++;
                if ((records[r].tx & 0x70) == 0x70) // EoC or EoB
                    return;
            }
            var keepr = r;
            var first = true;
            var verse = new StringBuilder();
            if (!contiguous)
                verse.Append("\n");
            do
            {
                string word = null;
                UInt16 key = (UInt16)(0x7FFF & records[r].word);
                if (modern && Ribbon.RIBBON.Modern.ContainsKey(key))
                    word = Ribbon.RIBBON.Modern[key];
                if (word == null && Ribbon.RIBBON.Display.ContainsKey(key))
                    word = Ribbon.RIBBON.Display[key];
                if (word == null && Ribbon.RIBBON.Search.ContainsKey(key))
                    word = Ribbon.RIBBON.Search[key];

                if (first)
                    first = false;
                else
                    verse.Append(' ');

                var postfix = PUNC.PostfixPunctuation(word, records[r].punc);
                var prefix = PUNC.PrefixPunctuation(records[r].punc, prevPunc);
                prevPunc = records[r].punc;

                if (prefix.Length > 0)
                    verse.Append(prefix);
                verse.Append(CAPS.Captitalize(records[r].word, word));
                if (postfix.Length > 0)
                    verse.Append(postfix);

            }   while ((records[r++].tx & 0x30) != 0x30); // EoV or EoC or EoB

            verse.Append("  ");

            dynamic rng = Ribbon.AVX.Application.ActiveDocument.Range();
            rng.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
            rng.Text = verse.ToString();

            r = keepr;
            foreach (Word.Range w in rng.Words)
            {
                var text = w.Text.Trim();
                if (text.Length >= 1 && char.IsLetter(text[0]))
                {
                    var italics = PUNC.IsItalisized(records[r].punc);
                    if (italics)
                        w.Font.Italic = 1;
                    r++;
                }
            }
        }
       public SelectVerse(byte bkNum)
        {
            InitializeComponent();
            if (bkNum >= 1 && bkNum <= 66)
                this.comboBoxBook.SelectedItem = this.comboBoxBook.Items.GetItemAt(bkNum - 1);

        }
        public SelectVerse()
        {
            InitializeComponent();
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.comboBoxBook.SelectedItem != null)
            {
                ComboBoxItem item = (ComboBoxItem) this.comboBoxBook.SelectedItem;
                var bk = item.Tag.ToString().Substring(2);
                var b = (byte) UInt16.Parse(bk);
                var book = Ribbon.RIBBON.bkIdx[b];
                string bname = item.Content.ToString();
                this.info.Text = bname + " has " + ((uint)book.ccnt).ToString() + " chapters";
            }
        }

        private void textBoxChaterAndVerse_KeyUp(object sender, KeyEventArgs e)
        {
            ComboBoxItem item = (ComboBoxItem)this.comboBoxBook.SelectedItem;
            var bk = item.Tag.ToString().Substring(2);
            var b = (byte)UInt16.Parse(bk);
            var book = Ribbon.RIBBON.bkIdx[b];
            string bname = item.Content.ToString();
            this.info.Text = bname + " has " + ((uint)book.ccnt).ToString() + " chapters";

            if (this.textBoxChaterAndVerse.Text.Length > 0)
            {
                var text = this.textBoxChaterAndVerse.Text.Trim();
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
                if (ch <= 255)
                {
                    byte c = (byte)ch;
                    if (Ribbon.RIBBON.chIdx[b].ContainsKey(c))
                    {
                        var chapter = Ribbon.RIBBON.chIdx[b][c];
                        this.info.Text += ("\nChapter " + ch.ToString() + " has " + ((uint)chapter.vcnt).ToString() + " verses");
                    }
                }
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem item = (ComboBoxItem)this.comboBoxBook.SelectedItem;
            if (item != null)
            {
                string bad = null;
                string error = null;

                var list = new List<byte>();

                var bk = item.Tag.ToString().Substring(2);
                var b = (byte)UInt16.Parse(bk);
                var book = Ribbon.RIBBON.bkIdx[b];
                string bname = item.Content.ToString();
                byte c = 0;

                if (this.textBoxChaterAndVerse.Text.Length > 0)
                {
                    var text = this.textBoxChaterAndVerse.Text.Trim();
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
                    if (ch <= 255 && Ribbon.RIBBON.chIdx[b].ContainsKey((byte)ch))
                    {
                        c = (byte)ch;
                        var chapter = Ribbon.RIBBON.chIdx[b][c];
                        this.info.Text += ("\nChapter " + ch.ToString() + " has " + ((uint)chapter.vcnt).ToString() + " verses");

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
                                    if (UInt16.TryParse(bad, out val) && val <= chapter.vcnt && val >= 1)
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
                                        if (UInt16.TryParse(bad, out val) && val <= chapter.vcnt && val >= 1)
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
                            for (byte v = 1; v <= chapter.vcnt; v++)
                                list.Add(v);
                        }
                    }
                    else
                    {
                        this.info.Text = "You must specify a chapter between 1 and " + ((uint)book.ccnt).ToString() + " (inclusive)";
                    }
                }
                if (bad == null && error == null)
                {
                    this.WriteVerseSpec();

                    bool first = true;
                    int prev = 0;
                    foreach (var v in list)
                    {
                        this.WriteVerse(b, c, v, true, first || (v == ++prev));
                        prev = v;
                        first = false;
                    }
                    dynamic rng = Ribbon.AVX.Application.ActiveDocument.Range();
                    rng.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
                    rng.Text = "\n";

                    this.Close();
                }
            }
            else
            {
                this.info.Text = "You must first select a book from the list prior to attempting to inserting verses.";
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.KeyUp += new KeyEventHandler(MainWindow_KeyUp);
        }

        void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                button_Click(null, null);
            }
        }
    }
}
