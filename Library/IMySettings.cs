using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public interface IMySettings
    {
        bool SetValue(string[] key, string[] value, string fileName);
        string GetValue(string key, string fileName);
        string GetSetCache(string key, string value);
        object GetSetCache(string key, object value);
        T GetSetCache<T>(string key, T value);
    }
}
