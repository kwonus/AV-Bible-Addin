using AVX.Serialization;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace AVX
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Settings : System.Windows.Window
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

        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);
        public Settings()
        {
            InitializeComponent();

            IntPtr ip = ThisAddIn.ICON.GetHbitmap();
            BitmapSource src = null;
            try
            {
                src = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip,
                   IntPtr.Zero, Int32Rect.Empty,
                   System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(ip);
            }
            if (src != null)
                this.AVIcon.Source = src;

            this.Loaded += new RoutedEventHandler(OnFormShow);
        }

        private void OnFormShow(object sender, RoutedEventArgs e)
        {
            this.HideMinimizeAndMaximizeButtons();

            var settings = ThisAddIn.API.ManageSettings();

            if (settings != null && settings.Count > 0)
            {
                this.Status.Foreground = new SolidColorBrush(Colors.Black);
                this.Status.Text = "";
                this.ButtonUpdate.IsEnabled = true;

                foreach (var key in settings.Keys)
                {
                    switch(key)
                    {
                        case "span":    this.span.Text    = settings[key]; break;
                        case "lexicon": this.lexicon.Text = settings[key]; break;
                        case "word":    this.word.Text    = settings[key]; break;
                        case "lemma":   this.lemma.Text   = settings[key]; break;
                    }
                }
            }
            else
            {
                this.Status.Foreground = new SolidColorBrush(Colors.Maroon);
                this.Status.Text = "AV Data-Manager is not running. See User-Help: 'Getting Started'";
                this.ButtonUpdate.IsEnabled = false;
            }
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            var settings = ThisAddIn.API.ManageSettings();

            if (settings != null)
            {
                foreach (var key in settings.Keys)
                {
                    switch(key)
                    {
                        case "span":    if (!this.span.Text.Trim().Equals(settings[key], StringComparison.InvariantCultureIgnoreCase))
                                            ThisAddIn.API.ManageSettings("span", this.span.Text.Trim());
                                        break;
                        case "lexicon": if (!this.lexicon.Text.Trim().Equals(settings[key], StringComparison.InvariantCultureIgnoreCase))
                                            ThisAddIn.API.ManageSettings("lexicon", this.lexicon.Text.Trim());
                                        break;
                        case "word":    if (!this.word.Text.Trim().Equals(settings[key], StringComparison.InvariantCultureIgnoreCase))
                                            ThisAddIn.API.ManageSettings("word", this.word.Text.Trim());
                                        break;
                        case "lemma":   if (!this.lemma.Text.Trim().Equals(settings[key], StringComparison.InvariantCultureIgnoreCase))
                                            ThisAddIn.API.ManageSettings("lemma", this.lemma.Text.Trim());
                                        break;
                    }
                }
            }
            Close();
        }
    }
}
