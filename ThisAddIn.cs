using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Word = Microsoft.Office.Interop.Word;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Word;
using System.Drawing;
using System.Runtime.Remoting.Messaging;
using AVX.Serialization;

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
        public static Bitmap CFG { get; private set; }

        public static AVX.Serialization.API API { get; private set; } = new AVX.Serialization.API();

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            InsertVerses.ForceClose = false;

            ThisAddIn.BIBLE = Properties.Resources.bible_80;
            ThisAddIn.FIND = Properties.Resources.find_80;
            ThisAddIn.BOOK = Properties.Resources.book_40;
            ThisAddIn.INFO = Properties.Resources.info_80;
            ThisAddIn.HELP = Properties.Resources.help_80;
            ThisAddIn.ICON = Properties.Resources.logo_160;
            ThisAddIn.CFG = Properties.Resources.settings_64;
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            InsertVerses.ForceClose = true;
            FindVerses.ForceClose = true;
            AboutInfo.ForceClose = true;
            HelpWindow.ForceClose = true;
        }
        protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            return new Ribbon(this);
        }
        public static void WriteVerse(byte bookNum, AVX.Serialization.DataStream[] words, int idx, bool modern, bool squelchHighlights = false)
        {
            byte prevPunc = 0;
            var first = true;
            var verse = new StringBuilder();

            bool end = false;
            int i = idx;
            do
            {
                DataStream word = words[i++];

                bool diff = word.Modernized;
                string text = modern ? word.Modern : word.Text;

                if (first)
                    first = false;
                else
                    verse.Append(' ');

                var postfix = PUNC.PostfixPunctuation(text, word.Punctuation);
                var prefix = PUNC.PrefixPunctuation(word.Punctuation, prevPunc);
                prevPunc = word.Punctuation;

                if (prefix.Length > 0)
                    verse.Append(prefix);
                verse.Append(text);
                if (postfix.Length > 0)
                    verse.Append(postfix);

                end = (word.Coordinates.WC == 1);

            }   while (!end);

            verse.Append("\n");

            dynamic rng = Ribbon.AVX.Application.ActiveDocument.Range();
            rng.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
            rng.Text = verse.ToString();

            i = idx;
            foreach (Word.Range w in rng.Words)
            {
                DataStream word = words[i];

                w.Bold = (word != null && word.Triggers != null && word.Triggers.Count > 0 && squelchHighlights == false) ? 1 : 0;
                var text = w.Text.Trim();
                if (text.Length >= 1)
                {
                    if (char.IsLetter(text[0]))
                    {
                        i++;

                        w.Font.Superscript = 0;

                        var italics = PUNC.IsItalisized(word.Punctuation);
                        if (italics)
                            w.Font.Italic = 1;
                    }
                }
                if (word.Coordinates.WC == 1)
                    break; // fail-safety
            }
        }

        public static void WriteInlineVerse(byte bookNum, AVX.Serialization.DataStream[] words, int idx, bool modern, bool first, bool squelchHighlights = false, Word.WdColor label = Word.WdColor.wdColorBlue)
        {
            byte prevPunc = 0;
            var verse = new StringBuilder();

            DataStream word = null;
            int i = idx;
            int end = -1;
            do
            {
                if (!first)
                    verse.Append("  ");

                bool addNum = (word == null) || (word.Coordinates.WC == 1);
                if (word != null)
                    verse.Append(" ");

                word = words[i++];
                if (end == (-1))
                    end = idx + word.Coordinates.WC;

                string text = modern ? word.Modern : word.Text;

                if (addNum)
                {
                    verse.Append(word.Coordinates.V.ToString());
                    addNum = false;
                }
                verse.Append(' ');

                var postfix = PUNC.PostfixPunctuation(text, word.Punctuation);
                var prefix = PUNC.PrefixPunctuation(word.Punctuation, prevPunc);
                prevPunc = word.Punctuation;

                if (prefix.Length > 0)
                    verse.Append(prefix);
                verse.Append(text);
                if (postfix.Length > 0)
                    verse.Append(postfix);

            }   while (i < end);


            dynamic rng = Ribbon.AVX.Application.ActiveDocument.Range();
            rng.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
            rng.Text = verse.ToString();

            Word.WdColor original = label;

            i = idx;
            word = null;
            foreach (Word.Range w in rng.Words)
            {
                if (i == idx)
                {
                    original = w.Font.Color;
                }

                w.Bold = (word != null && word.Triggers != null && word.Triggers.Count > 0 && squelchHighlights == false) ? 1 : 0;
                var text = w.Text.Trim();
                if (text.Length >= 1)
                {
                    if (char.IsLetter(text[0]))
                    {
                        if (i < words.Length)
                        {
                            word = words[i];
                            i++;
                        }
                    }

                    if (char.IsDigit(text[0]))
                    {
                        w.Font.Subscript = 0;
                        w.Font.Superscript = 1;
                        w.Font.Color = label;
                    }
                    else
                    {
                        w.Font.Superscript = 0;
                        w.Font.Color = original;

                        if (char.IsLetter(text[0]))
                        {
                            w.Font.Subscript = 0;

                            var italics = PUNC.IsItalisized(word.Punctuation);
                            if (italics)
                                w.Font.Italic = 1;
                        }
                        else
                        {
                            w.Font.Subscript = 1;
                        }
                    }
                }
            }
        }

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
