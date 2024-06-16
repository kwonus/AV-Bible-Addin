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

namespace AVX
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Settings : System.Windows.Window
    {
        public static Settings SettingsForm { get; private set; } = new Settings();
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
    }
}
