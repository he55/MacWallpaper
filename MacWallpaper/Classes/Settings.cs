﻿using System.IO;
using System.Xml.Serialization;

namespace DreamScene2
{
    public class Settings
    {
        static string s_filePath = Helper.GetPathForUserAppDataFolder("settings.xml");
        static Settings s_settings;

        private Settings() { }

        public bool FirstRun { get; set; } = true;
        public bool AutoPlay { get; set; } = true;
        public int SelectedIdx { get; set; }
        public string SelectedId { get; set; }

        public static Settings Load()
        {
            if (s_settings == null)
            {
                if (File.Exists(s_filePath))
                {
                    using (FileStream fileStream = File.OpenRead(s_filePath))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));
                        s_settings = (Settings)xmlSerializer.Deserialize(fileStream);
                    }
                }
                else
                {
                    s_settings = new Settings();
                }
            }
            return s_settings;
        }

        public static void Save()
        {
            using (FileStream fileStream = File.Create(s_filePath))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));
                xmlSerializer.Serialize(fileStream, s_settings);
            }
        }
    }
}
