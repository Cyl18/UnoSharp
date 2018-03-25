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
                if (nick.Length > 40)
                {
                    desk.AddMessage("请不要设置 太长的昵称 。");
                    return;
                }
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
                        desk.AddMessageLine(specialCard.ToImage().ToImageCodeAndDispose());
                    }
                    break;
            }

            if (Config.IsAdmin(player.PlayerId))
            {
                if (command.StartsWith("印卡"))
                {
                    var split = command.Split(' ');
                    var target = split[1];
                    var num = int.Parse(split[2]);
                    desk.AddMessage("[印卡场] 快速印卡正在执行...");
                    if (num > 200)
                    {
                        desk.GetPlayer(target).FastCreateCards(num);
                    }
                    else
                    {
                        desk.GetPlayer(target).AddCardsAndSort(num);
                    }
                    desk.AddMessage("[印卡场] Done.");
                    desk.SendLastCardMessage();
                }
            }
        }
    }
}