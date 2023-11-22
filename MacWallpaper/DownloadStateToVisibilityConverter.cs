using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MacWallpaper
{
    public class DownloadStateToVisibilityConverter : IValueConverter
    {
        public DownloadState State { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is DownloadState downloadState)
            {
                return State==downloadState?Visibility.Visible:Visibility.Collapsed;
            }
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
