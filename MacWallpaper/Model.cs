using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TinyJson;

namespace MacWallpaper
{
    public static class ModelHelper
    {
        static Dictionary<string, string> s_dict;
        public static Asset[] s_assets;
        public static string GetString(string name)
        {
            if(s_dict.TryGetValue(name,out string value))
                return value;
            return "";
        }

        public static Rootobject Load()
        {
            string v = File.ReadAllText(@"data\Localizable.json");
            var lang=JSONParser.FromJson<Dictionary<string,string>>(v);
            s_dict = lang;

            string v1 = File.ReadAllText(@"data\entries.json");
            var model=JSONParser.FromJson<Rootobject>(v1);
            s_assets = model.assets;
            return model;
        }
    }

    public class Rootobject
    {
        public int initialAssetCount { get; set; }
        public Category[] categories { get; set; }
        public int version { get; set; }
        public string localizationVersion { get; set; }
        public Asset[] assets { get; set; }
    }

    public class Category
    {
        public string previewImage { get; set; }
        public Category[] subcategories { get; set; }
        public string id { get; set; }
        public int preferredOrder { get; set; }
        public string representativeAssetID { get; set; }
        public string localizedNameKey { get; set; }
        public string localizedDescriptionKey { get; set; }

        public string str1
        {
            get
            {
                return ModelHelper.GetString(localizedNameKey);
            }
        }
        public string str2
        {
            get
            {
                return ModelHelper.GetString(localizedDescriptionKey);
            }
        }
        public Asset[] assets
        {
            get
            {
                return ModelHelper.s_assets.Where(x=>x.categories.Contains(id)).ToArray();
            }
        }
    }

    public class Asset
    {
        public string[] subcategories { get; set; }
        public bool showInTopLevel { get; set; }
        public string shotID { get; set; }
        public string previewImage { get; set; }
        public Dictionary<string,string> pointsOfInterest { get; set; }
        public string localizedNameKey { get; set; }
        public bool includeInShuffle { get; set; }
        public string accessibilityLabel { get; set; }
        [DataMember(Name = "url-4K-SDR-240FPS")]
        public string url4KSDR240FPS { get; set; }
        public string id { get; set; }
        public int preferredOrder { get; set; }
        public string[] categories { get; set; }
        public string group { get; set; }

        public string str1
        {
            get
            {
                return ModelHelper.GetString(localizedNameKey);
            }
        }
    }

}
