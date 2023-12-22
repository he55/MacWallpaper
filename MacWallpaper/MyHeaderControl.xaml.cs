using DreamScene2;
using System.Windows;
using System.Windows.Controls;

namespace MacWallpaper
{
    /// <summary>
    /// Interaction logic for MyHeaderControl.xaml
    /// </summary>
    public partial class MyHeaderControl : UserControl
    {
        Settings _settings = Settings.Load();

        public MyHeaderControl()
        {
            InitializeComponent();
            toggleSwitch1.IsOn = _settings.AutoPlay;
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            _settings.AutoPlay = toggleSwitch1.IsOn;
        }
    }
}
