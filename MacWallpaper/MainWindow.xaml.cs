using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
        Ass _lastSelectedItem;
        List<Ass> _asses;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void LoadData()
        {
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
                    string v = Helper.GetUrlFilePath(item2.url4KSDR240FPS, Helper._downloadPath);
                    Ass ass = new Ass { 
                        str1 = item2.str1,
                        previewImage=item2.previewImage,
                        downloadurl=item2.url4KSDR240FPS,
                    };
                    if (File.Exists(v))
                    {
                        ass.downloadState1 = DownloadState.downloaded;
                        ass.filepath = v;
                    }
                    asses.Add(ass);
                    cate.assets.Add(ass);
                }
                cates.Add(cate);
            }

            _asses = asses;
            listBox.ItemsSource = cates;
            listBox.SelectedIndex = 0;


            WebClient webClient = new WebClient();

            foreach (var asset in asses)
            {
                string v2 = Helper.GetUrlFilePath(asset.previewImage,Helper.imgsPath);
                if (!File.Exists(v2))
                {
                    await webClient.DownloadFileTaskAsync(asset.previewImage, v2);
                }
                asset.previewImage = v2;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Helper.InitFolder();
            Helper.CreateFolder();
            LoadData();
        }

        private void gridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Ass selectedItem = (Ass)gridView.SelectedItem;
            if (selectedItem != null)
            {
                if(_lastSelectedItem != null)
                    _lastSelectedItem.isSelected = false;

                _lastSelectedItem = selectedItem;
                selectedItem.isSelected = true;
                myHeaderControl.DataContext = selectedItem;
            }
        }

        private  void Window_Closing(object sender, CancelEventArgs e)
        {
            List<Ass> asses = _asses.Where(x=>x.downloadState== DownloadState.downloading).ToList();
            if(asses.Count > 0)
            {
                if (MessageBox.Show("正在下载文件，是否确认退出", "4kwallpaper", MessageBoxButton.OKCancel)== MessageBoxResult.OK)
                {
                    foreach(Ass ass in asses)
                    {
                        ass.CancelDownload();
                    }
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
    }

    public class Helper
    {
        public static string _downloadPath = @"C:\Users\admin\Documents\4kwallpaper";
       public static string imgsPath = "images";

        public static void InitFolder()
        {
            string v = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            _downloadPath = System.IO.Path.Combine(v, "4kwallpaper");
            string v1 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            imgsPath = System.IO.Path.Combine(v1,@"4kwallpaper\images");
        }
        public static void CreateFolder()
        {
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
    public enum DownloadState
    {
        none,
        downloading,
        downloaded
    }
    public class Cate
    {
        public string str1 { get; set; }
        public string des { get; set; }
        public List<Ass> assets { get; set; }
    }
    public class Ass:INotifyPropertyChanged
    {
        WebClient webClient;
        string tmpfile;

        private double progress1;
        private bool isSelected1;
        internal DownloadState downloadState1;

        public string str1 { get; set; }
        public string previewImage { get; set; }
        public string downloadurl { get; set; }

        public string filepath { get; set; }
        public DownloadState downloadState
        {
            get => downloadState1; 
            set
            {
                downloadState1 = value;
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
        public bool isSelected
        {
            get => isSelected1; 
            set
            {
                isSelected1 = value;
                OnPropertyChanged();
            }
        }


        public async void Download4kWallpaper()
        {
            Ass ass = this;
            ass.downloadState = DownloadState.downloading;

             webClient = new WebClient();
            tmpfile=System.IO.Path.GetTempFileName();
            webClient.DownloadFileCompleted += (object sender, System.ComponentModel.AsyncCompletedEventArgs e) => {
                if(e.Cancelled) 
                    return;

                string v = Helper.GetUrlFilePath(ass.downloadurl, Helper._downloadPath);
                ass.downloadState = DownloadState.downloaded;
                ass.filepath = v;
                File.Move(tmpfile, v);
            };
            webClient.DownloadProgressChanged += (object sender, DownloadProgressChangedEventArgs e) => {
                ass.progress = e.BytesReceived / (double)e.TotalBytesToReceive * 100;
            };
            try
            {
                await webClient.DownloadFileTaskAsync(ass.downloadurl, tmpfile);
            }
            catch (Exception ex)
            {
            }
        }
        public async void CancelDownload()
        {
            webClient.CancelAsync();
            webClient = null;
            downloadState = DownloadState.none;
            progress = 0;

            await Task.Delay(200);
            File.Delete(tmpfile);
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
                return ass.downloadState== DownloadState.none;
            return false;
        }

        public void Execute(object parameter)
        {
            if(parameter is Ass ass) 
                ass.Download4kWallpaper();
        }
    }

    public class CancelDownloadButtonCommand : ICommand
    {
        //public event EventHandler CanExecuteChanged;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if (parameter is Ass ass)
                return ass.downloadState == DownloadState.downloading;
            return false;
        }

        public void Execute(object parameter)
        {
            if (parameter is Ass ass)
                ass.CancelDownload();
        }
    }

}
