﻿using System;
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
            Asset[] assets = model.categories[0].assets;
            foreach ( Asset asset in assets )
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
            gridView.ItemsSource = assets;
        }
    }
}
