using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using DreamScene2;

namespace MacWallpaper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        Settings settings=Settings.Load();
        protected override void OnStartup(StartupEventArgs e)
        {
            if (settings.Lang == "zh_CN")
                Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("") });

            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        }
    }
}
