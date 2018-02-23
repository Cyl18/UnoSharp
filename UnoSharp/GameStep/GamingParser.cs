using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnoSharp.GameStep
{
    public class GamingParser : GameStepBase
    {
        public override void Parse(Desk desk, Player player, string command)
        {
            switch (command)
            {
                case "上一张牌":
                case "全场信息":
                    desk.SendLastCardMessage();
                    return;
            }
            if (!desk.Players.Contains(player))
                return;
            if (ParseUnoCommand(desk, player, command))
                return;
            switch (command)
            {
                case "明牌":
                    player.PublicCard = true;
                    desk.AddMessage("明牌成功.");
                    return;
                case "我的回合":
                case "我的回合!":
                case "我的回合！":
                    desk.AddMessage("是是, 我们都知道是你的回合");
                    return;
            }

            if (!IsValidPlayer(desk, player))
                return;


            // uno draw
            switch (command)
            {
                case "摸":
                case "摸牌":
                case "mo":
                case "draw":
                case "画画":
                case "抽卡！": //为了84！
                case "抽卡":
                    switch (desk.State)
                    {
                        case GamingState.Gaming:
                            player.AddCardsAndSort(1);
                            desk.AddMessageLine("已为你摸牌.");
                            break;
                        case GamingState.WaitingDrawTwoOverlay:
                        case GamingState.WaitingDrawFourOverlay:
                            desk.FinishDraw(desk.CurrentPlayer);
                            break;
                        case GamingState.Doubting:
                            desk.AddMessage("我在问你要不要质疑! 不是问你摸不摸!");
                            return;
                    }

                    MoveNext(desk);
                    desk.SendLastCardMessage();
                    return;
            }

            // uno doubt
            if (desk.State == GamingState.Doubting)
            {
                switch (command)
                {
                    case "质疑":
                        FinishDoubt(desk, player, true);
                        return;
                    case "不质疑":
                        FinishDoubt(desk, player, false);
                        return;
                    default:
                        desk.AddMessage("不是一个标准的质疑命令.");
                        return;
                }
            }

            // uno submit card
            var card = command.ToCard();
            if (card == null)
            {
                desk.AddMessage("无法匹配你想出的牌.");
                return;
            }

            if (!UnoRule.IsValid(card, desk.LastCard, desk.State))
            {
                desk.AddMessage("你想出的牌并不匹配 UNO 规则.");
                return;
            }

            if (!card.IsValidForPlayerAndRemove(player))
            {
                desk.AddMessage("你的手里并没有这些牌.");
                return;
            }

            desk.LastCard = card;
            if (card.Type != CardType.DrawFour)
            {
                desk.LastNonDrawFourCard = card;
            }

            if (player.Cards.Count == 0)
            {
                desk.FinishGame(player);
                return;
            }

            player.LastSendTime = DateTime.Now;
            desk.LastSendPlayer = player;
            Behave(desk, player, card);
            desk.SendLastCardMessage();
        }

        private void FinishDoubt(Desk desk, Player player, bool doubt)
        {
            if (doubt)
            {
                if (desk.LastNonDrawFourCard == null)
                {
                    desk.AddMessage("无法质疑: 没有上一张牌.");
                    desk.FinishDraw(player);
                    return;
                }
                var valid = _firstSubmitDrawFourPlayer.Cards.ContainsColor(desk.LastNonDrawFourCard.Color) ||
                            _firstSubmitDrawFourPlayer.Cards.ContainsValue(desk.LastNonDrawFourCard.Value);
                if (valid) // doubt is valid
                {
                    desk.FinishDraw(_firstSubmitDrawFourPlayer);
                }
                else
                {
                    desk.OverlayCardNum += 2;
                    desk.FinishDraw(player);
                }
            }
            else
            {
                desk.FinishDraw(player);
            }
        }

        private bool ParseUnoCommand(Desk desk, Player player, string command)
        {
            switch (command.ToUpper())
            {
                case "UNO":
                case "UNO!":
                case "UNO！":
                    if ((desk.CurrentPlayer == player && player.Cards.Count == 2) ||
                        (desk.LastSendPlayer == player && player.Cards.Count == 1))
                    {
                        player.Uno = true;
                        desk.AddMessage("UNO!");
                    }
                    else
                    {
                        desk.AddMessage("你还不能说 UNO!");
                    }
                    return true;
                case "质疑UNO":
                case "UNO PLZ":
                    if (desk.LastSendPlayer.Cards.Count == 1 && !desk.LastSendPlayer.Uno)
                    {
                        desk.AddMessage($"{desk.LastSendPlayer.AtCode}没有说 UNO, 被罚两张!");
                        desk.LastSendPlayer.AddCardsAndSort(2);
                        desk.SendLastCardMessage();
                    }
                    return true;
            }

            return false;
        }

        private void Behave(Desk desk, Player player, Card card)
        {
            player.SendCardsMessage();
            MoveNext(desk);
            var nextPlayer = desk.CurrentPlayer;
            switch (card.Type)
            {
                case CardType.Number:
                    // ignored
                    break;
                case CardType.Reverse:
                    desk.AddMessage($"方向反转.");
                    Reverse();
                    MoveNext(desk);
                    MoveNext(desk);
                    break;
                case CardType.Skip:
                    desk.AddMessage($"{nextPlayer.AtCode}被跳过.");
                    MoveNext(desk);
                    nextPlayer = desk.CurrentPlayer;
                    break;
                case CardType.DrawTwo:
                    desk.State = GamingState.WaitingDrawTwoOverlay;
                    desk.OverlayCardNum += 2;
                    BehaveDrawTwo(nextPlayer, desk);
                    break;
                case CardType.Wild:
                    desk.AddMessage($"变色");
                    break;
                case CardType.DrawFour:
                    if (desk.State == GamingState.Gaming)
                    {
                        _firstSubmitDrawFourPlayer = Previous(desk);
                        desk.State = GamingState.WaitingDrawFourOverlay;
                    }
                    desk.OverlayCardNum += 4;
                    BehaveDrawFour(nextPlayer, desk);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private Player _firstSubmitDrawFourPlayer;
        private void BehaveDrawFour(Player nextPlayer, Desk desk)
        {
            if (nextPlayer.Cards.ContainsType(CardType.DrawFour))
            {
                nextPlayer.AddMessage("你现在可以选择 叠加+4 或者 摸牌来让前面叠加的+2加到你身上");
            }
            else
            {
                if (desk.OverlayCardNum > 4)
                {
                    desk.FinishDraw(nextPlayer);
                }
                else
                {
                    desk.State = GamingState.Doubting;
                    desk.AddMessage($"{nextPlayer.AtCode}你要质疑吗?");
                }
            }
        }

        private void BehaveDrawTwo(Player nextPlayer, Desk desk)
        {
            if (nextPlayer.Cards.ContainsType(CardType.DrawTwo))
            {
                nextPlayer.AddMessage("你现在可以选择 叠加+2 或者 摸牌来让前面叠加的+2加到你身上");
            }
            else
            {
                desk.FinishDraw(desk.CurrentPlayer);
                desk.State = GamingState.Gaming;
            }
        }
    }
}
