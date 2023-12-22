using Microsoft.Toolkit.Wpf.UI.XamlHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Windows.Media.Core;
using Windows.UI.Xaml.Controls;

namespace MacWallpaper
{
    /// <summary>
    /// Interaction logic for VideoWindow.xaml
    /// </summary>
    public partial class VideoWindow : Window
    {
        MediaPlayerElement mediaPlayerElement;

        public VideoWindow()
        {
            InitializeComponent();
        }

        public string Source { get; set; }

        private void WindowsXamlHost_ChildChanged(object sender, EventArgs e)
        {
            mediaPlayerElement = (MediaPlayerElement)((WindowsXamlHost)sender).Child;
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
