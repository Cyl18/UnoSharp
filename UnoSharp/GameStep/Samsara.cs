using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnoSharp.GameComponent;
using UnoSharp.TimerEvents;

namespace UnoSharp.GameStep
{
    public class Samsara
    {
        public int CurrentIndex { get; internal set; }
        public bool Reversed { get; protected set; }

        protected bool IsValidPlayer(Desk desk, Player player)
        {
            if (!desk.Players.Contains(player))
                return false;
            return desk.Players.ToList().FindIndex(p => p == player) == CurrentIndex;
        }



        public void Reverse()
        {
            Reversed = !Reversed;
        }

        internal virtual void MoveNext(Desk desk)
        {
            desk.Step++;
            if (Reversed)
            {
                CurrentIndex--;
                if (CurrentIndex == -1)
                {
                    CurrentIndex = desk.Players.Count() - 1;
                }
            }
            else
            {
                CurrentIndex = (CurrentIndex + 1) % desk.Players.Count();
            }

            
        }

        public static void DoAutoSubmitCard(Desk desk)
        {
            var validstr = UnoRule.ExtractCommand(desk.CurrentPlayer.Cards, desk.LastCard, desk.State);
            desk.AddMessage($"{(desk.CurrentPlayer is FakePlayer ? "机器人" : "托管玩家")}命令: {validstr}");
            Thread.Sleep(100);
            var cp = desk.CurrentPlayer.PlayerId;
            if (desk.CurrentPlayer.Cards.Count == 2)
            {
                desk.Events.Add(new TimerEvent(() => desk.ParseMessage(cp, "uno"), 1, -1));
            }
            desk.ParseMessage(desk.CurrentPlayer.PlayerId, validstr);
            
        }

        public Player Next(Desk desk)
        {
            var current = CurrentIndex;
            MoveNext(desk);
            var cp = desk.PlayerList[CurrentIndex];
            CurrentIndex = current;
            return cp;
        }

        public Player Previous(Desk desk)
        {
            var current = CurrentIndex;
            Reverse();
            MoveNext(desk);
            var cp = desk.PlayerList[CurrentIndex];
            Reverse();
            CurrentIndex = current;
            return cp;
        }

    }
}
