using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
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

            DownloadButtonCommand command = new DownloadButtonCommand();

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
                    string v = Helper.GetUrlFilePath(item2.url4KSDR240FPS, Helper._downloadPath);
                    Ass ass = new Ass { 
                        str1 = item2.str1,
                        previewImage=item2.previewImage,
                        downloadurl=item2.url4KSDR240FPS,
                        DownloadCommand = command,
                    };
                    if (File.Exists(v))
                    {
                        ass.isDownload1 = true;
                        ass.filepath = v;
                    }
                    asses.Add(ass);
                    cate.assets.Add(ass);
                }
                cates.Add(cate);
            }


            listBox.ItemsSource = cates;
            listBox.SelectedIndex = 0;

            foreach (var asset in asses)
            {
                string v2 = Helper.GetUrlFilePath(asset.previewImage,imgsPath);
                if (!File.Exists(v2))
                {
                    await webClient.DownloadFileTaskAsync(asset.previewImage, v2);
                }
                asset.previewImage = v2;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Helper.CreateFolder();
            Button_Click2(null, null);
        }
    }

    public class Helper
    {
        public static string _downloadPath = @"C:\Users\admin\Documents\4kwallpaper";

        public static void CreateFolder()
        {
            if (!Directory.Exists(_downloadPath))
            {
                Directory.CreateDirectory(_downloadPath);
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

    public class Cate
    {
        public string str1 { get; set; }
        public string des { get; set; }
        public List<Ass> assets { get; set; }
    }
    public class Ass:INotifyPropertyChanged
    {
        private double progress1;
        internal bool isDownload1;

        public string str1 { get; set; }
        public string previewImage { get; set; }
        public string downloadurl { get; set; }

        public string filepath { get; set; }
        public bool isDownload
        {
            get => isDownload1; 
            set
            {
                isDownload1 = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }
        public double progress
        {
            get => progress1; 
            set
            {
                progress1 = value;
                OnPropertyChanged();
            }
        }

        public ICommand DownloadCommand { get; set; }


        public async void Download4kWallpaper()
        {
            Ass ass = this;
            string v = Helper.GetUrlFilePath(ass.downloadurl, Helper._downloadPath);
            WebClient webClient = new WebClient();
            webClient.DownloadFileCompleted += (object sender, System.ComponentModel.AsyncCompletedEventArgs e) => {
                ass.isDownload = true;
                ass.filepath = v;
            };
            webClient.DownloadProgressChanged += (object sender, DownloadProgressChangedEventArgs e) => {
                ass.progress = e.BytesReceived / (double)e.TotalBytesToReceive * 100;
            };
            await webClient.DownloadFileTaskAsync(ass.downloadurl, v);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class DownloadButtonCommand : ICommand
    {
        //public event EventHandler CanExecuteChanged;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if(parameter is Ass ass)
                return !ass.isDownload;
            return false;
        }

        public void Execute(object parameter)
        {
            if(parameter is Ass ass) 
                ass.Download4kWallpaper();
        }
    }
}
