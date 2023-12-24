using DreamScene2;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TinyJson;

namespace MacWallpaper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Settings _settings = Settings.Load();
        Ass _lastSelectedItem;
        List<Ass> _asses;

        public MainWindow()
        {
            InitializeComponent();
        }

        string GetString(string name)
        {
            if (lang.TryGetValue(name, out string value))
                return value;
            return "";
        }

        Dictionary<string, string> lang;
        void LoadData()
        {
            string vvv = File.ReadAllText($@"data\{_settings.Lang}.json");
            lang = JSONParser.FromJson<Dictionary<string, string>>(vvv);

            string v11 = File.ReadAllText(@"data\entries.json");
            var model = JSONParser.FromJson<Rootobject>(v11);

            List<Ass> asses = new List<Ass>();
            List<Cate> cates = new List<Cate>();

            foreach (var item in model.categories)
            {
                Cate cate = new Cate();
                cate.str1 = GetString(item.localizedNameKey);
                cate.des = GetString(item.localizedDescriptionKey);
                cate.assets = new List<Ass>();
                foreach (var item2 in model.assets.Where(x => x.categories.Contains(item.id)))
                {
                    string v = Helper.GetUrlFilePath(item2.url4KSDR240FPS, Helper._downloadPath);
                    Ass ass = new Ass
                    {
                        id = item2.id,
                        str1 = GetString(item2.localizedNameKey),
                        previewImage = item2.previewImage,
                        downloadURL = item2.url4KSDR240FPS,
                    };
                    if (File.Exists(v))
                    {
                        ass.downloadState1 = DownloadState.downloaded;
                        ass.filePath = v;
                    }
                    asses.Add(ass);
                    cate.assets.Add(ass);
                }
                cates.Add(cate);
            }

            _asses = asses;
            listBox.ItemsSource = cates;
            listBox.SelectedIndex = _settings.SelectedIdx;

            Ass ass1 = null;
            string id = _settings.SelectedId;
            if (!string.IsNullOrEmpty(id))
            {
                ass1 = _asses.Where(x => x.id == id).FirstOrDefault();
            }

            if (ass1 == null)
                ass1 = _asses[0];

            ass1.isSelected = true;
            myHeaderControl.DataContext = ass1;
            _lastSelectedItem = ass1;
        }

        static async void DownloadImage(List<Ass> asses)
        {
            WebClient webClient = new WebClient();

            foreach (var asset in asses)
            {
                string v2 = Helper.GetUrlFilePath(asset.previewImage, Helper.imgsPath);
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
            LoadData();
            DownloadImage(_asses);
        }

        private void gridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Ass selectedItem = (Ass)gridView.SelectedItem;
            if (selectedItem != null)
            {
                if (_lastSelectedItem != null)
                    _lastSelectedItem.isSelected = false;

                _lastSelectedItem = selectedItem;
                selectedItem.isSelected = true;
                myHeaderControl.DataContext = selectedItem;

                _settings.SelectedIdx = listBox.SelectedIndex;
                _settings.SelectedId = selectedItem.id;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Settings.Save();

            List<Ass> asses = _asses.Where(x => x.downloadState == DownloadState.downloading).ToList();
            if (asses.Count == 0)
                return;

            if (MessageBox.Show("正在下载文件，是否确认退出", "4kwallpaper", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
            {
                e.Cancel = true;
                return;
            }

            foreach (Ass ass in asses)
            {
                ass.CancelDownload();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_settings.Lang == "en")
                _settings.Lang = "zh_CN";
            else
                _settings.Lang = "en";
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

    public class Ass : INotifyPropertyChanged
    {
        WebClient webClient;
        string tmpfile;

        private double progress1;
        private bool isSelected1;
        internal DownloadState downloadState1;

        public string id { get; set; }
        public string str1 { get; set; }
        public string previewImage { get; set; }
        public string downloadURL { get; set; }

        public string filePath { get; set; }
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

        public async void Download()
        {
            Ass ass = this;
            ass.downloadState = DownloadState.downloading;

            webClient = new WebClient();
            tmpfile = Path.GetTempFileName();
            webClient.DownloadFileCompleted += (object sender, AsyncCompletedEventArgs e) =>
            {
                if (e.Cancelled)
                    return;

                string v = Helper.GetUrlFilePath(ass.downloadURL, Helper._downloadPath);
                ass.downloadState = DownloadState.downloaded;
                ass.filePath = v;
                File.Move(tmpfile, v);
            };
            webClient.DownloadProgressChanged += (object sender, DownloadProgressChangedEventArgs e) =>
            {
                ass.progress = e.BytesReceived / (double)e.TotalBytesToReceive * 100;
            };
            try
            {
                await webClient.DownloadFileTaskAsync(ass.downloadURL, tmpfile);
            }
            catch { }
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

        public void OpenFolder()
        {
            Process.Start("explorer.exe", $"/select, \"{filePath}\"");
        }

        public void Preview()
        {
            VideoWindow videoWindow = new VideoWindow();
            videoWindow.Source = filePath;
            videoWindow.Show();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

}
