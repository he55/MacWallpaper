using ModernWpf.Controls;
using System;
using System.IO;
using System.Linq;

namespace MacWallpaper
{
    public class Helper
    {
        public static string _downloadPath;
        public static string imgsPath;

        public static void CreateFolder()
        {
            string v = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            _downloadPath = System.IO.Path.Combine(v, "4kwallpaper");
            string v1 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            imgsPath = System.IO.Path.Combine(v1, @"4kwallpaper\images");

            if (!Directory.Exists(_downloadPath))
            {
                Directory.CreateDirectory(_downloadPath);
            }

            if (!Directory.Exists(imgsPath))
            {
                Directory.CreateDirectory(imgsPath);
            }
        }

        public static string GetUrlFilePath(string url, string path)
        {
            string v = new Uri(url).Segments.Last();
            string v1 = System.IO.Path.Combine(path, v);
            string v2 = System.IO.Path.GetFullPath(v1);
            return v2;
        }
    }

}
