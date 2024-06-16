namespace AVX
{
    using AVX.Properties;
    using System;
    using System.IO;
    using System.Text;
    using System.Windows;

    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
		public static HelpWindow HelpForm { get; private set; } = new HelpWindow();
        public static bool ForceClose;
        public HelpWindow()
        {
            InitializeComponent();
            HelpWindow.ForceClose = false;

            using (var overview = new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.overview)))
                this.OverviewHelp.Selection.Load(overview, DataFormats.Rtf);

            using (var settings = new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.settings)))
                this.SettingsHelp.Selection.Load(settings, DataFormats.Rtf);

            using (var searching = new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.searching)))
                this.SearchingHelp.Selection.Load(searching, DataFormats.Rtf);

            using (var language = new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.language)))
                this.LanguageHelp.Selection.Load(language, DataFormats.Rtf);
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
