using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnoSharp
{
    public class Config
    {
        private const string ConfigPath = "UnoSharp.json";

        public void Save()
        {
            File.WriteAllText(ConfigPath, this.ToJsonString());
        }
        public Dictionary<string, string> Nicks { get; } = new Dictionary<string, string>();
        public static Config Get()
        {
            return File.Exists(ConfigPath)
                ? File.ReadAllText(ConfigPath).JsonDeserialize<Config>()
                : new Config();
        }
    }
}
