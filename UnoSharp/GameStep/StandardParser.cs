using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnoSharp.GameStep
{
    public class StandardParser : ICommandParser
    {
        public void Parse(Desk desk, Player player, string command)
        {
            if (command.StartsWith("设置昵称 "))
            {
                var nick = command.Substring("设置昵称 ".Length);
                var config = Config.Get();
                config.Nicks[player.PlayerId] = nick;
                config.Save();
                desk.AddMessage("Done.");
            }

            switch (command)
            {
                case "特殊牌功能":
                    foreach (var specialCard in Card.SpecialCards)
                    {
                        desk.AddMessageLine($"{specialCard.ShortName} {specialCard.Description}");
                        desk.AddMessageLine(specialCard.ToImage().ToImageCode());
                    }
                    break;
            }
        }
    }
}
