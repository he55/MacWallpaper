using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

namespace MacWallpaper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            WebClient webClient = new WebClient();
            string imgsPath = "images";
            if(!Directory.Exists(imgsPath))
            {
                Directory.CreateDirectory(imgsPath);
            }

            var model= ModelHelper.Load();
            listBox.ItemsSource=model.categories; 
            listBox.SelectedIndex=0;

            foreach ( Asset asset in model.assets )
            {
                string v = new Uri(asset.previewImage).Segments.Last();
                string v1 = System.IO.Path.Combine(imgsPath, v);
                string v2 = System.IO.Path.GetFullPath(v1);
                if(!File.Exists(v2))
                {
                   await webClient.DownloadFileTaskAsync(asset.previewImage, v2);
                }
                asset.previewImage = v2;
            }
        }

        private async void Button_Click2(object sender, RoutedEventArgs e)
        {
            WebClient webClient = new WebClient();
            string imgsPath = "images";
            if (!Directory.Exists(imgsPath))
            {
                Directory.CreateDirectory(imgsPath);
            }

            var model = ModelHelper.Load();

            List<Ass> asses = new List<Ass>();
            List<Cate> cates = new List<Cate>();
            foreach (var item in model.categories)
            {
                Cate cate = new Cate();
                cate.str1 = item.str1;
                cate.des = item.str2;
                cate.assets = new List<Ass>();
                foreach (var item2 in item.assets)
                {
                    Ass ass = new Ass { 
                        str1 = item2.str1,
                        previewImage=item2.previewImage,
                        downloadurl=item2.url4KSDR240FPS,
                    };
                    asses.Add(ass);
                    cate.assets.Add(ass);
                }
                cates.Add(cate);
            }



            listBox.ItemsSource = cates;
            listBox.SelectedIndex = 0;

            foreach (var asset in asses)
            {
                string v = new Uri(asset.previewImage).Segments.Last();
                string v1 = System.IO.Path.Combine(imgsPath, v);
                string v2 = System.IO.Path.GetFullPath(v1);
                if (!File.Exists(v2))
                {
                    await webClient.DownloadFileTaskAsync(asset.previewImage, v2);
                }
                asset.previewImage = v2;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Button_Click2(null, null);
        }
    }

    public class Cate
    {
        public string str1 { get; set; }
        public string des { get; set; }
        public List<Ass> assets { get; set; }
    }
    public class Ass
    {
        public string str1 { get; set; }
        public string previewImage { get; set; }

        public string downloadurl { get; set; }
        public bool isDownload { get; set; }
        public double progress { get; set; }
    }
}
