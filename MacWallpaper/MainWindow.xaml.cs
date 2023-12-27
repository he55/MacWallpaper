using DreamScene2;
using System;
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
        System.Windows.Forms.NotifyIcon notifyIcon1;

        Settings _settings = Settings.Load();
        Ass _lastSelectedItem;
        List<Ass> _asses;

        public MainWindow()
        {
            InitializeComponent();
            InitNotifyIcon();
            toggleSwitch1.IsOn = _settings.AutoPlay;
            toggleSwitch2.IsOn = Helper.CheckStartOnBoot();
        }

        void InitNotifyIcon()
        {
            var toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem1.Text = I18nWpf.GetString("LMenuShow");
            toolStripMenuItem1.Click += toolStripMenuItem1_Click;

            var toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem2.Text = "Exit";
            toolStripMenuItem2.Click += toolStripMenuItem2_Click;

            var contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip();
            contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            toolStripMenuItem1,
            toolStripMenuItem2});


            notifyIcon1 = new System.Windows.Forms.NotifyIcon();
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            notifyIcon1.Text = "4kwallpaper";
            notifyIcon1.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            notifyIcon1.DoubleClick += NotifyIcon1_DoubleClick;
            notifyIcon1.Visible = true;
        }

        private void NotifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.Activate();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Show();
            this.Activate();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Settings.Save();

            List<Ass> asses = _asses.Where(x => x.downloadState == DownloadState.downloading).ToList();
            if (asses.Count == 0)
            {
                notifyIcon1.Dispose();
                Environment.Exit(0);
                return;
            }

            this.Show();
            this.Activate();

            if (MessageBox.Show("正在下载文件，是否确认退出", "4kwallpaper", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                foreach (Ass ass in asses)
                {
                    ass.CancelDownload();
                }

                notifyIcon1.Dispose();
                Environment.Exit(0);
            }
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            _settings.AutoPlay = toggleSwitch1.IsOn;
        }

        private void ToggleSwitch_Toggled2(object sender, RoutedEventArgs e)
        {
            if (toggleSwitch2.IsOn)
                Helper.SetStartOnBoot();
            else
                Helper.RemoveStartOnBoot();
        }

        void LoadData()
        {
            string vvv = File.ReadAllText($@"data\{_settings.Lang}.json");
            var lang = JSONParser.FromJson<Dictionary<string, string>>(vvv);

            string GetString(string key)
            {
                if (lang.TryGetValue(key, out string val))
                    return val;
                return "";
            }

            string v11 = File.ReadAllText(@"data\entries.json");
            var model = JSONParser.FromJson<Rootobject>(v11);

            List<Ass> asses = new List<Ass>();
            List<Cate> cates = new List<Cate>();

            foreach (var item in model.categories)
            {
                Cate cate = new Cate();
                cate.title = GetString(item.localizedNameKey);
                cate.description = GetString(item.localizedDescriptionKey);
                cate.assets = new List<Ass>();
                foreach (var item2 in model.assets.Where(x => x.categories.Contains(item.id)))
                {
                    string v = Helper2.GetUrlFilePath(item2.url4KSDR240FPS, Helper2._downloadPath);
                    Ass ass = new Ass
                    {
                        id = item2.id,
                        name = GetString(item2.localizedNameKey),
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
                string v2 = Helper2.GetUrlFilePath(asset.previewImage, Helper2.imgsPath);
                if (!File.Exists(v2))
                {
                    await webClient.DownloadFileTaskAsync(asset.previewImage, v2);
                }
                asset.previewImage = v2;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Helper2.CreateFolder();
            LoadData();
            DownloadImage(_asses);
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gridView.ItemsSource = ((Cate)listBox.SelectedItem).assets;
            if (gridView.Items.Count > 0)
                gridView.ScrollIntoView(gridView.Items[0]);
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
            e.Cancel = true;
            this.Hide();
            Settings.Save();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_settings.Lang == "en")
                _settings.Lang = "zh_CN";
            else
                _settings.Lang = "en";
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _lastSelectedItem.Download();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            _lastSelectedItem.CancelDownload();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            _lastSelectedItem.OpenFolder();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            _lastSelectedItem.Preview();
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
        public string title { get; set; }
        public string description { get; set; }
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
        public string name { get; set; }
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

                string v = Helper2.GetUrlFilePath(ass.downloadURL, Helper2._downloadPath);
                ass.downloadState = DownloadState.downloaded;
                ass.filePath = v;
                File.Move(tmpfile, v);
            };
            webClient.DownloadProgressChanged += (object sender, DownloadProgressChangedEventArgs e) =>
            {
                ass.progress = e.ProgressPercentage;
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
            UwpVideoWindow videoWindow = new UwpVideoWindow();
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
