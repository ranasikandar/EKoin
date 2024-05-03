using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class MySettings : IMySettings
    {
        public bool SetValue(string[] key, string[] value, string fileName)
        {
            try
            {
                //string appSettingsJsonFilePath = System.IO.Path.Combine(System.AppContext.BaseDirectory, "myAppSettings.json");

                //var json = File.ReadAllText(appSettingsJsonFilePath);
                //dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(json);

                //for (int i = 0; i < key.Length; i++)
                //{
                //    jsonObj[key[i]] = value[i];
                //}

                //string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);

                //File.WriteAllText(appSettingsJsonFilePath, output);


                string appSettingsJsonFilePath = Path.Combine(System.AppContext.BaseDirectory, fileName);
                string json = string.Empty;
                dynamic jsonObj = null;

                if (File.Exists(appSettingsJsonFilePath))
                {

                    json = File.ReadAllText(appSettingsJsonFilePath);
                    jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(json);

                    for (int i = 0; i < key.Length; i++)
                    {
                        jsonObj[key[i]] = value[i];
                    }
                    string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);

                    File.WriteAllText(appSettingsJsonFilePath, output);
                }
                else
                {
                    return false;
                }


                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error writing my app settings | {ex.Message}", ex);
            }

        }

        public string GetValue(string key, string fileName)
        {
            try
            {
                string appSettingsJsonFilePath = System.IO.Path.Combine(System.AppContext.BaseDirectory, fileName);

                var json = File.ReadAllText(appSettingsJsonFilePath);
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(json);

                return jsonObj[key].Value;

            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading my app settings | {ex.Message}", ex);
            }

        }


    }

    //public static class AppSettings
    //{
    //    private static void LoadSettings
    //    {
    //        // This is where you would call the DB and populate the cache
    //    }

    //    public static string AppName
    //    {
    //        get
    //        {
    //            if (Cache["appname"] == null)
    //                LoadSettings();
    //            return Cache["appname"];
    //        }
    //    }
    //}
}
