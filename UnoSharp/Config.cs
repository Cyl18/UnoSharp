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

        private Dictionary<string, List<string>> Commands { get; set; }

        public Dictionary<string, List<string>> GetCommands()
        {
            if (Commands == null)
            {
                var messagesJson = EmbedResourceReader.Read("UnoSharp.Resources.messages.json"); // 默认信息JSON
                Commands = messagesJson.JsonDeserialize<Dictionary<string, List<string>>>();
            }

            return Commands;
        }

        public Config()
        {
            Commands = GetCommands();
            if (!File.Exists(ConfigPath))
                Save();
        }

        public static Config Get()
        {
            return File.Exists(ConfigPath)
                ? File.ReadAllText(ConfigPath).JsonDeserialize<Config>()
                : new Config();
        }

        public static bool IdAdmin(string playerid) => playerid == "775942303";
    }
}