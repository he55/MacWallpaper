using System.Windows;

namespace MacWallpaper
{
    public static class I18nWpf
    {
        public static string GetString(string key)
        {
            return (string)Application.Current.Resources[key];
        }
    }
}
