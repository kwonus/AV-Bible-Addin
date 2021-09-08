using System;
using System.Collections.Generic;
using System.Text;
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
        public FindVerses()
        {
            InitializeComponent();
        }
        private void search_Click(object sender, RoutedEventArgs e)
        {
        }
        private void insert_Click(object sender, RoutedEventArgs e)
        {
            /*
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

                        if (colon > 0 && colon + 1 < len)
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
                        this.WriteVerse(b, c, v, this.modernize.IsChecked == true, first || (v == ++prev));
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
            */
        }
    }
}
