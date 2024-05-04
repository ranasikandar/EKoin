using Microsoft.Extensions.Caching.Memory;
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
        private readonly IMemoryCache memoryCache;
        public MySettings(IMemoryCache _memoryCache)
        {
            memoryCache = _memoryCache;
        }
        
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

                    for (int i = 0; i < key.Length; i++)
                    {
                        GetSetCache(key[i], value[i]);
                    }
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
                if (!memoryCache.TryGetValue(key, out string value))
                {
                    string appSettingsJsonFilePath = System.IO.Path.Combine(System.AppContext.BaseDirectory, fileName);

                    var json = File.ReadAllText(appSettingsJsonFilePath);
                    dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(json);

                    value = jsonObj[key].Value;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        AbsoluteExpiration = null,
                        Priority = CacheItemPriority.High
                    };

                    memoryCache.Set(key, value,cacheEntryOptions);
                }
                return value;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading my app settings | {ex.Message}", ex);
            }

        }

        public string GetSetCache(string key, string value)
        {
            if (!memoryCache.TryGetValue(key, out string _value))
            {
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = null,
                    Priority = CacheItemPriority.High
                };

                memoryCache.Set(key, value, cacheEntryOptions);
            }
            return _value;
        }

        public object GetSetCache(string key, object value)
        {
            if (!memoryCache.TryGetValue(key, out object _value))
            {
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = null,
                    Priority = CacheItemPriority.High
                };

                memoryCache.Set(key, value, cacheEntryOptions);
            }
            return _value;
        }

        public T GetSetCache<T>(string key, T value)
        {
            if (!memoryCache.TryGetValue(key, out T _value))
            {
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = null,
                    Priority = CacheItemPriority.High
                };

                memoryCache.Set(key, value, cacheEntryOptions);
            }
            return _value;
        }

    }

}
