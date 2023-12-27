using Microsoft.Toolkit.Wpf.UI.XamlHost;
using System;
using System.Windows;
using Windows.Media.Core;
using Windows.UI.Xaml.Controls;

namespace MacWallpaper
{
    /// <summary>
    /// Interaction logic for UwpVideoWindow.xaml
    /// </summary>
    public partial class UwpVideoWindow : Window
    {
        MediaPlayerElement mediaPlayerElement;

        public UwpVideoWindow()
        {
            InitializeComponent();

            this.Topmost = true;
            this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.NoResize;
            this.Top = 0;
            this.Left = 0;
            this.Width = SystemParameters.PrimaryScreenWidth;
            this.Height = SystemParameters.PrimaryScreenHeight;
        }

        public string Source { get; set; }

        private void WindowsXamlHost_ChildChanged(object sender, EventArgs e)
        {
            mediaPlayerElement = (MediaPlayerElement)((WindowsXamlHost)sender).Child;
            if (mediaPlayerElement == null)
                return;

            mediaPlayerElement.Stretch = Windows.UI.Xaml.Media.Stretch.UniformToFill;
            mediaPlayerElement.AutoPlay = true;
            mediaPlayerElement.MediaPlayer.IsLoopingEnabled = true;
            mediaPlayerElement.PointerReleased += MediaPlayerElement_PointerReleased;
            mediaPlayerElement.Source = MediaSource.CreateFromUri(new Uri(Source));
        }

        private void MediaPlayerElement_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            this.Close();
        }
    }
}
