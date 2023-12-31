﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace MacWallpaper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Forms.NotifyIcon _notifyIcon;
        Settings _settings = Settings.Load();
        WallpaperAsset _lastSelectedAsset;
        List<WallpaperAsset> _assets;

        public MainWindow()
        {
            InitializeComponent();
            InitNotifyIcon();
            toggleSwitch1.IsOn = _settings.AutoPlay;
            toggleSwitch2.IsOn = Helper.CheckStartOnBoot();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            FolderHelper.CreateFolder();
            LoadData();
            DownloadImages(_assets);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();

            if (_settings.FirstRun)
            {
                _notifyIcon.ShowBalloonTip(1000, "", I18nWpf.GetString("LHideTip"), System.Windows.Forms.ToolTipIcon.None);
                _settings.FirstRun = false;
            }

            Settings.Save();
        }

        void ShowWindow()
        {
            this.WindowState = WindowState.Normal;
            this.Show();
            this.Activate();
        }

        void InitNotifyIcon()
        {
            var toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem1.Text = I18nWpf.GetString("LMenuShow");
            toolStripMenuItem1.Click += delegate { ShowWindow(); };

            var toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem2.Text = I18nWpf.GetString("LMenuExit");
            toolStripMenuItem2.Click += toolStripMenuItem2_Click;

            var contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip();
            contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            toolStripMenuItem1,
            toolStripMenuItem2});

            _notifyIcon = new System.Windows.Forms.NotifyIcon();
            _notifyIcon.ContextMenuStrip = contextMenuStrip1;
            _notifyIcon.Text = Constants.ProjectName;
            _notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            _notifyIcon.DoubleClick += delegate { ShowWindow(); };
            _notifyIcon.Visible = true;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Settings.Save();

            List<WallpaperAsset> assets = _assets.Where(x => x.downloadState == DownloadState.downloading).ToList();
            if (assets.Count == 0)
            {
                _notifyIcon.Dispose();
                Environment.Exit(0);
                return;
            }

            ShowWindow();

            if (MessageBox.Show(I18nWpf.GetString("LCancelDownloadConfirmTip"), Constants.ProjectName, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                foreach (WallpaperAsset asset in assets)
                {
                    asset.CancelDownload();
                }

                _notifyIcon.Dispose();
                Environment.Exit(0);
            }
        }

        void LoadData()
        {
            string langJson = File.ReadAllText($@"data\{_settings.Language}.json");
            var langDict = TinyJson.JSONParser.FromJson<Dictionary<string, string>>(langJson);

            string GetString(string key)
            {
                if (langDict.TryGetValue(key, out string val))
                    return val;
                return "";
            }

            string entriesJson = File.ReadAllText(@"data\entries.json");
            var model = TinyJson.JSONParser.FromJson<EntriesObject>(entriesJson);

            List<WallpaperAsset> assets = new List<WallpaperAsset>();
            List<WallpaperCategory> categories = new List<WallpaperCategory>();

            foreach (var item in model.categories)
            {
                WallpaperCategory category = new WallpaperCategory();
                category.title = GetString(item.localizedNameKey);
                category.description = GetString(item.localizedDescriptionKey);
                category.assets = new List<WallpaperAsset>();
                foreach (var item2 in model.assets.Where(x => x.categories.Contains(item.id)))
                {
                    string filePath = FolderHelper.GetFilePathForURL(item2.url4KSDR240FPS, FolderHelper.DownloadPath);
                    WallpaperAsset asset = new WallpaperAsset
                    {
                        id = item2.id,
                        name = GetString(item2.localizedNameKey),
                        previewImage = item2.previewImage,
                        downloadURL = item2.url4KSDR240FPS,
                    };
                    if (File.Exists(filePath))
                    {
                        asset._downloadState = DownloadState.downloaded;
                        asset.filePath = filePath;
                    }
                    assets.Add(asset);
                    category.assets.Add(asset);
                }
                categories.Add(category);
            }

            _assets = assets;
            listBox.ItemsSource = categories;
            listBox.SelectedIndex = _settings.SelectedIndex;

            WallpaperAsset selectedAsset = null;
            string id = _settings.SelectedId;
            if (!string.IsNullOrEmpty(id))
            {
                selectedAsset = _assets.Where(x => x.id == id).FirstOrDefault();
            }

            if (selectedAsset == null)
                selectedAsset = _assets[0];

            selectedAsset.isSelected = true;
            headerGrid.DataContext = selectedAsset;
            _lastSelectedAsset = selectedAsset;
        }

        static async void DownloadImages(List<WallpaperAsset> assets)
        {
            WebClient webClient = new WebClient();

            foreach (var asset in assets)
            {
                string imagePath = FolderHelper.GetFilePathForURL(asset.previewImage, FolderHelper.ImageCachePath);
                if (!File.Exists(imagePath))
                {
                    await webClient.DownloadFileTaskAsync(asset.previewImage, imagePath);
                }
                asset.previewImage = imagePath;
            }
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gridView.ItemsSource = ((WallpaperCategory)listBox.SelectedItem).assets;
            if (gridView.Items.Count > 0)
                gridView.ScrollIntoView(gridView.Items[0]);
        }

        private void gridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WallpaperAsset selectedItem = (WallpaperAsset)gridView.SelectedItem;
            if (selectedItem != null)
            {
                if (_lastSelectedAsset != null)
                    _lastSelectedAsset.isSelected = false;

                selectedItem.isSelected = true;
                headerGrid.DataContext = selectedItem;
                _lastSelectedAsset = selectedItem;

                _settings.SelectedIndex = listBox.SelectedIndex;
                _settings.SelectedId = selectedItem.id;
            }
        }

        private void ToggleLanguage(object sender, RoutedEventArgs e)
        {
            if (_settings.Language == "en")
                _settings.Language = "zh_CN";
            else
                _settings.Language = "en";
        }

        private void ToggleAutoPlay(object sender, RoutedEventArgs e)
        {
            _settings.AutoPlay = toggleSwitch1.IsOn;
        }

        private void ToggleStartOnBoot(object sender, RoutedEventArgs e)
        {
            if (toggleSwitch2.IsOn)
                Helper.SetStartOnBoot();
            else
                Helper.RemoveStartOnBoot();
        }

        private void Download(object sender, RoutedEventArgs e)
        {
            _lastSelectedAsset.Download();
        }

        private void CancelDownload(object sender, RoutedEventArgs e)
        {
            _lastSelectedAsset.CancelDownload();
        }

        private void OpenFolder(object sender, RoutedEventArgs e)
        {
            _lastSelectedAsset.OpenFolder();
        }

        private void Preview(object sender, RoutedEventArgs e)
        {
            _lastSelectedAsset.Preview();
        }
    }
}
