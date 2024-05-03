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

    }
}
