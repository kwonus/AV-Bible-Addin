using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Word = Microsoft.Office.Interop.Word;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Word;
using System.Drawing;

namespace AVX
{
    public partial class ThisAddIn
    {
        public static Bitmap BIBLE { get; private set; }
        public static Bitmap FIND { get; private set; }
        public static Bitmap BOOK { get; private set; }
        public static Bitmap HELP { get; private set; }
        public static Bitmap INFO { get; private set; }
        public static Bitmap ICON { get; private set; }
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            InsertVerses.ForceClose = false;

            ThisAddIn.BIBLE = Properties.Resources.bible_80;
            ThisAddIn.FIND = Properties.Resources.find_80;
            ThisAddIn.BOOK = Properties.Resources.book_40;
            ThisAddIn.INFO = Properties.Resources.info_80;
            ThisAddIn.HELP = Properties.Resources.help_80;
            ThisAddIn.ICON = Properties.Resources.logo_160;
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            InsertVerses.ForceClose = true;
            FindVerses.ForceClose = true;
            AboutInfo.ForceClose = true;
        }
        protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            return new Ribbon(this);
        }
        /*
        public static void WriteVerse(Book book, byte c, byte v, bool modern, bool contiguous)
        {
            ThisAddIn.WriteVerse(book.num, c, v, modern, contiguous);
        }

        public static void WriteVerse(byte b, byte c, byte v, bool modern, bool contiguous, Word.WdColor label = Word.WdColor.wdColorBlue)
        {
            byte prevPunc = 0;
            var chapter = Ribbon.RIBBON.chIdx[b][c];
            var records = Ribbon.RIBBON.writ;
            UInt32 r = chapter.writIdx;
            for (int i = 1; i < v; //)
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

            do
            {
                if (contiguous && ((records[r].tx & 0x70) == 0x20)) // BoV
                    verse.Append(v.ToString() + (char)0x200B // zero-width-space //);

                string word = null;
                UInt16 key = (UInt16)(0x7FFF & records[r].word);
                bool diff = modern && Ribbon.RIBBON.Modern.ContainsKey(key);
                if (diff)
                {
                    word = Ribbon.RIBBON.Modern[key];
                    ThisAddIn.api.XWrit.SetCursor(r);
                    byte pn = (byte) (ThisAddIn.api.XWrit.WClass >> 12);
                    bool plural = (pn & 0xC) == 0x8;
                    bool singular = (pn & 0xC) == 0x4;
                    byte p = (byte) (pn & 0x3);

                    if ((p == 2) && singular)
                    {
                        word += (char)0x200B; // zero-width-space //
                        word += '†';
                    }
                    else
                    {
                        var orig = Ribbon.RIBBON.Search[key];
                        if (orig.StartsWith("th", StringComparison.InvariantCultureIgnoreCase) && word.StartsWith("you", StringComparison.InvariantCultureIgnoreCase))
                            word += '†';
                        else if (orig.EndsWith("st", StringComparison.InvariantCultureIgnoreCase) && !word.EndsWith("st", StringComparison.InvariantCultureIgnoreCase))
                            word += '†';
                    }

                }
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

            } while ((records[r++].tx & 0x30) != 0x30); // EoV or EoC or EoB

            if (contiguous)
                verse.Append("  ");
            else
                verse.Append("\n");

            dynamic rng = Ribbon.AVX.Application.ActiveDocument.Range();
            rng.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
            rng.Text = verse.ToString();

            Word.WdColor original = label;
            r = keepr;
            foreach (Word.Range w in rng.Words)
            {
                if (contiguous && (original == label))
                    original = w.Font.Color;

                w.Bold = 0;
                var text = w.Text.Trim();
                if (text.Length >= 1)
                {
                    if (char.IsDigit(text[0]))
                    {
                        w.Font.Subscript = 0;
                        w.Font.Superscript = 1;
                        w.Font.Color = label;
                    }
                    else
                    {
                        w.Font.Superscript = 0;
                        if (contiguous)
                            w.Font.Color = original;

                        if (char.IsLetter(text[0]))
                        {
                            w.Font.Subscript = 0;

                            var italics = PUNC.IsItalisized(records[r].punc);
                            if (italics)
                                w.Font.Italic = 1;
                            r++;
                        }
                        else
                        {
                            w.Font.Subscript = 1;
                        }
                    }
                }
            }
        }*/


        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
