using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnoSharp.GameStep
{
    public abstract class GameStepBase : Samsara, ICommandParser
    {
        public static Dictionary<string, string> GenericCommandCache { get; } = new Dictionary<string, string>();

        public abstract void Parse(Desk desk, Player player, string command);

        /**
         * 根据messages.json的内容来解析命令.
         */
        public static string ToGenericCommand(string command)
        {
            if (GenericCommandCache.ContainsKey(command))
                return GenericCommandCache[command];

            var genericCommand = "";

            foreach (var entry in Config.Get().GetCommands())
            {
                if (entry.Value.Contains(command))
                {
                    GenericCommandCache.Add(entry.Key, command);
                    genericCommand = entry.Key;
                }
            }

            return genericCommand;
        }
    }
}
