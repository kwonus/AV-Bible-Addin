using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Word = Microsoft.Office.Interop.Word;

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
        {/*
            this.FoundTree.Items.Clear();

            var command = QuelleCommand(this.TextCriteria.Text);

            var verses = new HashSet<UInt16>();
            if (command.success && command.result != null && command.result.verses != null && command.result.verses.Count > 0)
            {
                var root = new TreeViewItem();
                root.Tag = (byte) 0;
                root.Header = "All Matching Verses";
                this.FoundTree.Items.Add(root);

                TreeViewItem book = null;
                TreeViewItem chapter = null;
                TreeViewItem verse = null;

                byte bk = 0;
                byte ch = 0;
                byte vs = 0;
                byte ignore;

                foreach (UInt16 vidx in from v in command.result.verses orderby v select v)
                {
                    if (!AVXAPI.SELF.XVerse.GetEntry(vidx, out bk, out ch, out vs, out ignore))
                        return; // something unexpected went wrong

                    if (book == null || (byte)book.Tag != bk)
                    {
                        chapter = null;
                        book = new TreeViewItem();
                        book.Tag = bk;
                        book.Header = AVXAPI.SELF.XBook.GetBookByNum(bk).Value.name;
                        root.Items.Add(book);
                    }
                    if (chapter == null || (byte)chapter.Tag != ch)
                    {
                        chapter = new TreeViewItem();
                        chapter.Tag = ch;
                        chapter.Header = "Chapter " + ((uint)ch).ToString();
                        book.Items.Add(chapter);
                    }
                    verse = new TreeViewItem();
                    verse.Tag = vs;
                    verse.Header = ((uint)vs).ToString();
                    chapter.Items.Add(verse);
                }
                if (this.FoundTree.Items.Count == 1)
                {
                    if (root.Items.Count == 1)
                        this.textBoxChaterAndVerse.Text = ((TreeViewItem)root.Items[0]).Header.ToString() + ": all matching verses";
                    else
                        this.textBoxChaterAndVerse.Text = "all matching verses";
                }
                else
                { 
                    this.textBoxChaterAndVerse.Text = "";
                }
            }*/
        }
        /*
        private void AddVerseToDocument(Book book, byte chapter, byte verse)
        {
            this.WriteVerseSpec(book, chapter, verse);
            ThisAddIn.WriteVerse(book, chapter, verse, this.modernize.IsChecked == true, false);
        }
        private void AddChapterToDocument(Book book, TreeViewItem chapterNode)
        {
            var c = (byte)chapterNode.Tag;
            //var chapter = api.Chapters[book.chapterIdx + c - 1];
            foreach (var verseNode in chapterNode.Items)
            {
                AddVerseToDocument(book, c, (byte)((TreeViewItem)verseNode).Tag);
            }
        }
        */
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
        private void insert_Click(object sender, RoutedEventArgs e)
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
            }

            else
            {
                for (byte bk = 1; bk <= 66; bk++)
                {
                    var book = api.Books[bk-1];
                    var len = book.name.Length;

                    if (trimmed.StartsWith(book.name, StringComparison.InvariantCultureIgnoreCase) && len < trimmed.Length)
                    {
                        trimmed = trimmed.Substring(len).Trim();
                        if (trimmed == ": all matching verses")
                        {
                            var bookNode = this.FindNode(book.name);
                            if (bookNode != null)
                            {
                                foreach (var chapterNode in bookNode.Items)
                                    AddChapterToDocument(book, (TreeViewItem)chapterNode);
                            }
                            return;
                        }
                        else
                        {
                            var spec = trimmed.Split(':');
                            if (spec.Length == 2)
                            {
                                var c = byte.Parse(spec[0].Trim());
                                if (c >= 1 && c <= book.chapterCnt)
                                {
                                    // Chapter chapter = api.Chapters[book.chapterIdx + c - 1];

                                    foreach (var verse in spec[1].Split(','))
                                    {
                                        var v = int.Parse(verse.Trim());
                                        if (v >= 1 && v <= 255)
                                            AddVerseToDocument(book, c, (byte)v);
                                    }
                                }
                            }
                        }
                        return;
                    }
                }
                for (byte bk = 1; bk <= 66; bk++)
                {
                    var book = api.Books[bk-1];
                    foreach (var abbr in book.abbreviations)
                    {
                        var len = abbr.Length;
                        if (trimmed.StartsWith(abbr, StringComparison.InvariantCultureIgnoreCase))
                        {
                            for (trimmed = trimmed.Substring(len); trimmed.Length > 0 && !char.IsWhiteSpace(trimmed[0]); trimmed = trimmed.Substring(1)) // handles Gen vs Ge for abbreviation
                                ;
                            trimmed = trimmed.Trim();
                                if (trimmed.Length == 0)
                                    return;

                            if (trimmed == ": all matching verses")
                            {
                                var bookNode = this.FindNode(book.name);
                                if (bookNode != null)
                                {
                                    foreach (var chapterNode in bookNode.Items)
                                        AddChapterToDocument(book, (TreeViewItem)chapterNode);
                                }
                            }
                            else
                            {
                                var spec = trimmed.Split(':');
                                if (spec.Length == 2)
                                {
                                    var c = byte.Parse(spec[0].Trim());
                                    if (c >= 1 && c <= book.chapterCnt)
                                    {
                                        // Chapter chapter = api.Chapters[book.chapterIdx + c - 1];

                                        foreach (var verse in spec[1].Split(','))
                                        {
                                            var v = int.Parse(verse.Trim());
                                            if (v >= 1 && v <= 255)
                                                AddVerseToDocument(book, c, (byte)v);
                                        }
                                    }
                                }
                            }
                            return;
                        }
                    }
                }
            }*/
        }

        private void FoundTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {/*
            var node = (TreeViewItem) this.FoundTree.SelectedItem;

            if (node == null)
            {
                this.textBoxChaterAndVerse.Text = "";
            }
            else if ((byte)node.Tag == 0)   // root
            {
                this.textBoxChaterAndVerse.Text = "all matching verses";
            }
            else
            {
                var parent = (TreeViewItem)node.Parent;

                if ((byte)parent.Tag == 0)        // Book
                {
                    this.textBoxChaterAndVerse.Text = node.Header.ToString() + ": all matching verses";
                }
                else
                {
                    var grandParent = (TreeViewItem)(parent.Parent);

                    if ((byte)grandParent.Tag == 0)        // Chapter
                    {
                        char delimiter = ':';
                        this.textBoxChaterAndVerse.Text = parent.Header.ToString() + ' ' + ((byte)node.Tag).ToString();
                        foreach (var verse in node.Items)
                        {
                            this.textBoxChaterAndVerse.Text += delimiter;
                            this.textBoxChaterAndVerse.Text += ((TreeViewItem)verse).Header.ToString();
                            delimiter = ',';
                        }
                    }
                    else // verse
                    {
                        this.textBoxChaterAndVerse.Text = grandParent.Header.ToString() + ' ' + ((byte)parent.Tag).ToString() + ':' + ((byte)node.Tag).ToString();
                    }
                }
            }*/
        }

        private void OnKeyDownCrieria(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.search_Click(sender, null);
            }
        }
        private void OnKeyDownSpec(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.insert_Click(sender, null);
            }
        }

    }
}
