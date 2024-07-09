namespace AVX
{
    using AVX.Properties;
    using System;
    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
		public static HelpWindow HelpForm { get; private set; } = new HelpWindow();
        public static bool ForceClose;

        private void ClearSelectionAndMakeReadOnly(RichTextBox rtf)
        {
            rtf.IsReadOnly = true;
            rtf.Selection.Select(rtf.Document.ContentStart, rtf.Document.ContentStart);
        }

        public HelpWindow()
        {
            InitializeComponent();
            HelpWindow.ForceClose = false;

            using (var quickstart = new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.quickstart)))
                this.QuickStartHelp.Selection.Load(quickstart, DataFormats.Rtf);

            using (var overview = new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.overview)))
                this.OverviewHelp.Selection.Load(overview, DataFormats.Rtf);

            using (var settings = new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.settings)))
                this.SettingsHelp.Selection.Load(settings, DataFormats.Rtf);

            using (var searching = new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.searching)))
                this.SearchingHelp.Selection.Load(searching, DataFormats.Rtf);

            using (var language = new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.language)))
                this.LanguageHelp.Selection.Load(language, DataFormats.Rtf);

            this.ClearSelectionAndMakeReadOnly(this.OverviewHelp);
            this.ClearSelectionAndMakeReadOnly(this.SettingsHelp);
            this.ClearSelectionAndMakeReadOnly(this.SearchingHelp);
            this.ClearSelectionAndMakeReadOnly(this.LanguageHelp);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!HelpWindow.ForceClose)
            {
                e.Cancel = true;
                this.Hide();
            }
        }
    }
}
