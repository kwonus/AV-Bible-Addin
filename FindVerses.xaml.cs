using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using Word = Microsoft.Office.Interop.Word;
using AVX.Serialization;

namespace AVX
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class FindVerses : Window
    {
        public static FindVerses SearchForm { get; private set; } = new FindVerses();

        public FindVerses()
        {
            InitializeComponent();
        }
        public static bool ForceClose = false; // Indicate if it is an explicit close request
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            if (ForceClose)
                return;

            e.Cancel = true;
            this.Hide();
        }
        private void search_Click(object sender, RoutedEventArgs e)
        {
            // API (to get matching verse references)
            // app.MapGet("/debug/find/{spec}", (string spec) => API.api.engine.Debug_Find(spec, out message, quoted: false).ToString());
            // app.MapGet("/debug/find-quoted/{spec}", (string spec) => API.api.engine.Debug_Find(spec, out message, quoted: true).ToString());

            var result = ThisAddIn.API.Find(this.TextCriteria.Text, null);

            this.FoundTree.Items.Clear();

            this.button_insert_all.IsEnabled = (result.Count > 0);

            if (this.button_insert_all.IsEnabled)
            {
                TreeViewItem book = null;
                TreeViewItem cv = null;

                byte b = 0;
                byte c = 0;
                byte v = 0;

                foreach (BookResult br in result)
                {
                    if (br.B != b)
                    {
                        b = br.B;
                        book = new TreeViewItem();
                        book.Tag = (UInt16) b;
                        book.Header = br.Info.Name;
                        this.FoundTree.Items.Add(book);
                        c = 0;
                        v = 0;
                    }
                    foreach (ChapterResult cr in br)
                    {
                        if (cr.C != c)
                        {
                            c = cr.C;
                            v = 0;
                        }
                        foreach (VerseResult vr in cr)
                        {
                            if (vr.V != v)
                            {
                                v = vr.V;
                                cv = new TreeViewItem();
                                cv.Tag = (UInt16)((((UInt16)c) << 8) | (UInt16)v);
                                cv.Header = c.ToString() + ":" + v.ToString();
                                book.Items.Add(cv);
                            }
                        }
                    }
                }
            }
        }
        private void WriteVerseSpec(string book, string spec)
        {
            dynamic rng = Ribbon.AVX.Application.ActiveDocument.Range();
            rng.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
            rng.Bold = 1;
            string insert = book + " " + spec;
            if (this.modernize.IsChecked.Value)
                insert += "  (AVX)";
            insert += "\n";
            rng.Text = insert;
            foreach (Word.Range w in rng.Words)
            {
                w.Font.Bold = 1;
            }
        }

        private void AddVerseToDocument(BookInfo book, DataStream[] words, int idx)
        {
            this.WriteVerseSpec(book.Name, words[idx].Coordinates.C + ":" + words[idx].Coordinates.V);
            ThisAddIn.WriteVerse(book.Num, words, idx, this.modernize.IsChecked == true, false);
        }
        
        private void insert_book_Click(object sender, RoutedEventArgs e)
        {
            bool inserted = false;
            var node = (TreeViewItem)this.FoundTree.SelectedItem;
            BookInfo book = BookInfo.GetBook((byte)((UInt16)node.Tag));

            DataStream[] words = null;
            byte C = 0;
            foreach (var cvnode in node.Items)
            {
                UInt16 cv = (UInt16)((TreeViewItem)cvnode).Tag;
                byte c = (byte)(cv >> 8);
                byte v = (byte)(cv & 0xff);

                if (c >= 1 && v >= 1 && c <= book.ChapterCount && v <= book.VerseCountsByChapter[c])
                {
                    if (C != c)
                    {
                        words = ThisAddIn.API.FindWithDetails(this.TextCriteria.Text, new Dictionary<string, string>(), book, c);
                        C = c;
                    }
                    if (words != null)
                    {
                        int i = 0;
                        foreach (var word in words)
                        {
                            if (word.Coordinates.V == v)
                            {
                                AddVerseToDocument(book, words, i);
                                inserted = true;
                                break;
                            }
                            i++;
                        }
                    }
                }
            }
            if (inserted)
                this.Close();
        }
        private void insert_verse_Click(object sender, RoutedEventArgs e)
        {
            var node = (TreeViewItem)this.FoundTree.SelectedItem;
            BookInfo book = BookInfo.GetBook((byte)((UInt16)((TreeViewItem)node.Parent).Tag));

            UInt16 cv = (UInt16)node.Tag;
            byte c = (byte)(cv >> 8);
            byte v = (byte)(cv & 0xff);
            if (c >= 1 && v >= 1 && c <= book.ChapterCount && v <= book.VerseCountsByChapter[c])
            {
                DataStream[] words = ThisAddIn.API.FindWithDetails(this.TextCriteria.Text, new Dictionary<string, string>(), book, c);

                if (words != null)
                {
                    int i = 0;
                    foreach (var word in words)
                    {
                        if (word.Coordinates.V == v)
                        {
                            AddVerseToDocument(book, words, i);
                            this.Close();
                            break;
                        }
                        i++;
                    }
                }
            }
        }
        private void insert_variant_Click(object sender, RoutedEventArgs e)
        {
            var verse = (TreeViewItem)this.FoundTree.SelectedItem;
            var book  = (TreeViewItem) ((verse != null) ? verse.Parent : null);

            if (book != null)
            {
                byte bk = (byte)((UInt16)(book.Tag));
                string spec = (string) verse.Header;
                InsertVerses.ShowForm(bk, spec);
                InsertVerses.InsertAny.textBoxChapterAndVerse.Text = spec;
                InsertVerses.InsertAny.button.IsEnabled = true;
                this.Close();
            }
        }
        private void insert_all_Click(object sender, RoutedEventArgs e)
        {
            bool inserted = false;

            foreach (var node in this.FoundTree.Items)
            {
                var bk = (TreeViewItem)node;
                BookInfo book = BookInfo.GetBook((byte)((UInt16)bk.Tag));

                DataStream[] words = null;
                byte C = 0;
                foreach (var cvnode in bk.Items)
                {
                    UInt16 cv = (UInt16)((TreeViewItem)cvnode).Tag;
                    byte c = (byte)(cv >> 8);
                    byte v = (byte)(cv & 0xff);
                    if (c >= 1 && v >= 1 && c <= book.ChapterCount && v <= book.VerseCountsByChapter[c])
                    {
                        if (C != c)
                        {
                            words = ThisAddIn.API.FindWithDetails(this.TextCriteria.Text, new Dictionary<string, string>(), book, c);
                            C = c;
                        }
                        if (words != null)
                        {
                            int i = 0;
                            foreach (var word in words)
                            {
                                if (word.Coordinates.V == v)
                                {
                                    AddVerseToDocument(book, words, i);
                                    inserted = true;
                                    break;
                                }
                                i++;
                            }
                        }
                    }
                }
                if (inserted)
                    this.Close();
            }
        }

        private void FoundTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var node = (TreeViewItem) this.FoundTree.SelectedItem;

            if (node != null)
            {
                UInt16 tag = (UInt16)node.Tag;

                if (tag == 0)
                {
                    this.button_insert_book.IsEnabled = false;
                    this.button_insert_verse.IsEnabled = false;
                    this.button_insert_variant.IsEnabled = false;
                }
                else if (tag >= 1 && tag <= 66)
                {
                    this.button_insert_book.IsEnabled = true;
                    this.button_insert_verse.IsEnabled = false;
                    this.button_insert_variant.IsEnabled = false;
                }
                else if (tag >= 0x100)
                {
                    this.button_insert_book.IsEnabled = false;
                    this.button_insert_verse.IsEnabled = true;
                    this.button_insert_variant.IsEnabled = true;
                }
            }
            else
            {
                this.button_insert_book.IsEnabled = false;
                this.button_insert_book.IsEnabled = false;
                this.button_insert_variant.IsEnabled = false;
            }
        }

        private void OnKeyDownCrieria(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.search_Click(sender, null);
            }
        }
        private void OnKeyDownSpec(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.insert_all_Click(sender, null);
            }
        }

    }
}
