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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DreamScene2;

namespace MacWallpaper
{
    /// <summary>
    /// Interaction logic for MyHeaderControl.xaml
    /// </summary>
    public partial class MyHeaderControl : UserControl
    {
        Settings _settings=Settings.Load();

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
