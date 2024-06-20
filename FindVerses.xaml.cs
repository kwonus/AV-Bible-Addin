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
            API api = new API();

            var result = api.Find(this.TextCriteria.Text, null);

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
                                cv.Header = c.ToString() + v.ToString();
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

        private void AddVerseToDocument(BookInfo book, byte chapter, byte verse)
        {
            this.WriteVerseSpec(book.Name, chapter + ":" + verse);
            ThisAddIn.WriteVerse(book.Num, chapter, verse, this.modernize.IsChecked == true, false);
        }
        private void AddChapterToDocument(BookInfo book, TreeViewItem chapterNode)
        {
            var c = (byte)chapterNode.Tag;
            //var chapter = api.Chapters[book.chapterIdx + c - 1];
            foreach (var verseNode in chapterNode.Items)
            {
                AddVerseToDocument(book, c, (byte)((TreeViewItem)verseNode).Tag);
            }
        }
        
        TreeViewItem FindNode(string bookName)
        {/*
            foreach (var root in this.FoundTree.Items)
            {
                foreach (var candidate in ((TreeViewItem)root).Items)
                {
                    var book = (TreeViewItem)candidate;
                    if (book.Header.ToString().Equals(bookName, StringComparison.InvariantCultureIgnoreCase))
                        return book;
                }
            }*/
            return null;
        }
        private void insert_book_Click(object sender, RoutedEventArgs e)
        {
            InsertVerses.ShowForm(InsertVerses.InsertAny);

            var book = (TreeViewItem)this.FoundTree.SelectedItem;

            foreach (var node in book.Items)
            {
                var verse = (TreeViewItem)node;
            }

            /*if (book != null)
            {
                InsertVerses.ShowForm((byte)((UInt16)(book.Tag)));
                InsertVerses.InsertAny.textBoxChapterAndVerse.Text = (string)verse.Header;
                InsertVerses.InsertAny.button.IsEnabled = true;
            }

            if (trimmed == ": all matching verses")
            {
                var bookNode = this.FindNode(book.name);
                if (bookNode != null)
                {
                    foreach (var chapterNode in bookNode.Items)
                        AddChapterToDocument(book, (TreeViewItem)chapterNode);
                }
                return;
            }*/
        }
        private void insert_verse_Click(object sender, RoutedEventArgs e)
        {
        }
        private void insert_variant_Click(object sender, RoutedEventArgs e)
        {
            InsertVerses.ShowForm(InsertVerses.InsertAny);

            var verse = (TreeViewItem)this.FoundTree.SelectedItem;
            var book  = (TreeViewItem) ((verse != null) ? verse.Parent : null);

            if (book != null)
            {
                InsertVerses.ShowForm((byte)((UInt16)(book.Tag)));
                InsertVerses.InsertAny.textBoxChapterAndVerse.Text = (string) verse.Header;
                InsertVerses.InsertAny.button.IsEnabled = true;
            }
        }
        private void insert_all_Click(object sender, RoutedEventArgs e)
        {/*
            var trimmed = this.textBoxChaterAndVerse.Text.Trim();
            if (trimmed.Equals("all matching verses", StringComparison.InvariantCultureIgnoreCase))
            {
                foreach (var root in this.FoundTree.Items)
                {
                    foreach (var candidate in ((TreeViewItem)root).Items)
                    {
                        var bookNode = (TreeViewItem)candidate;
                        var book = api.Books[(byte)bookNode.Tag-1];
                        foreach (var chapterNode in bookNode.Items)
                            AddChapterToDocument(book, (TreeViewItem)chapterNode);
                    }
                }
                return;
            }*/
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
