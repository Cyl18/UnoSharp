using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnoSharp.GameComponent;

namespace UnoSharp.GameStep
{
    public class WaitingParser : GameStepBase
    {
        public override void Parse(Desk desk, Player player, string command)
        {
            var genericCommand = ToGenericCommand(command);
            switch (genericCommand)
            {
                case "join":
                    desk.AddPlayer(player);
                    break;
                case "leave":
                    desk.RemovePlayer(player);
                    break;
                case "addRobot":
                    desk.AddMessage("请使用 添加机器人 [昵称]");
                    break;
                case "start":
                    desk.StartGame();
                    break;
            }

            if (command.StartsWith("添加机器人 "))
            {
                var nick = command.Substring(6);
                desk.AddPlayer(new FakePlayer(nick, desk));
            }

            if (command.StartsWith("移除机器人 "))
            {
                var nick = command.Substring(6);
                var players = desk.PlayerList.OfType<FakePlayer>().Where(p => p.Nick == nick).ToArray();
                if (players.Length == 0) return;
                desk.RemovePlayer(players.First());
            }
        }
    }
}
