using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnoSharp.GameComponent
{
    public class CardLG : SpecialCard
    {
        public override string Description { get; } = "所有牌随机转换颜色";
        public override string ShortName { get; } = "LG";
        public override int Chance { get; } = 5;

        public override void Behave(Desk desk)
        {
            foreach (var player in desk.PlayerList)
            {
                foreach (var card in player.Cards.Where(c => c.Color != CardColor.Special && c.Color != CardColor.Wild))
                {
                    card.Color = PickColor();
                    card.DrawColor = Card.ToColor(card.Color);
                }
                player.AddMessage("诶嘿嘿, 你的卡牌颜色全部已经被交换.");
                player.Cards.Sort();
                player.SendCardsMessage();
            }

        }
        private static readonly Random Rng = new Random();
        private static CardColor PickColor()
        {
            switch (Rng.Next(4))
            {
                case 0:
                    return CardColor.Red;
                case 1:
                    return CardColor.Green;
                case 2:
                    return CardColor.Blue;
                case 3:
                    return CardColor.Yellow;
                default:
                    throw new Exception("WTF??");
            }
        }
    }

    public class Card84 : SpecialCard
    {
        public override string Description { get; } = "场上玩家随机更换牌";
        public override string ShortName { get; } = "84";
        public override int Chance { get; } = 3;

        public override void Behave(Desk desk)
        {
            var cards = desk.Players.Select(player => player.Cards).ToList();
            cards.Shuffle();
            for (var i = 0; i < desk.PlayerList.Count; i++)
            {
                desk.PlayerList[i].Cards = cards[i];
            }
            desk.PlayerList.ForEach(player => player.SendCardsMessage());
        }
    }

    public class CardCJ : SpecialCard
    {
        public override string Description { get; } = "将场上所有人的牌数量扣或加到7";
        public override string ShortName { get; } = "CJ";
        public override int Chance { get; } = 3;

        public override void Behave(Desk desk)
        {
            foreach (var player in desk.Players)
            {
                var count = player.Cards.Count;
                var px = count - 7;
                if (px > 0)
                {
                    for (var i = 0; i < px; i++)
                    {
                        player.Cards.Remove(player.Cards.PickOne());
                        player.SendCardsMessage();
                    }
                }
                else if (px < 0)
                {
                    player.AddCardsAndSort(-px);
                }
            }
        }
    }

    public class CardA125 : SpecialCard
    {
        public override string Description { get; } = "后面摸到的5张牌都是蓝色";
        public override string ShortName { get; } = "A125";
        public override int Chance { get; } = 3;
        public override void Behave(Desk desk)
        {
            foreach (var blue in Generate(card => card.Color == CardColor.Blue, 5))
                CardsQueue.Enqueue(blue);
        }
    }

    public class CardShp : SpecialCard
    {
        public override string Description { get; } = "所有玩家向后面交换手牌";
        public override string ShortName { get; } = "SHP";
        public override int Chance { get; } = 3;
        public override void Behave(Desk desk)
        {
            var cards = desk.Players.Select(player => player.Cards).ToList();
            cards.EachToNext();
            for (var i = 0; i < desk.PlayerList.Count; i++)
            {
                desk.PlayerList[i].Cards = cards[i];
            }
            desk.PlayerList.ForEach(player => player.SendCardsMessage());
        }
    }

    public class CardCY : SpecialCard
    {
        public override string Description { get; } = "你永远猜不到这张卡有什么功能";
        public override string ShortName { get; } = "CY";
        public override int Chance { get; } = 2;
        public override void Behave(Desk desk)
        {
            var specialCard = SpecialCards.PickOne();
            if (!(specialCard is CardCY))
            {
                desk.AddMessageLine($"Cy牌: 表达出{specialCard.ShortName}: {specialCard.Description}");
            }
            specialCard.Behave(desk);
        }
    }

    public class CardToma : SpecialCard
    {
        public override string Description { get; } = "随机打乱玩家顺序";
        public override string ShortName { get; } = "SM";
        public override int Chance { get; } = 7;
        public override void Behave(Desk desk)
        {
            desk.RandomizePlayers();
            desk.PlayerList.ForEach(player => desk.AddMessageLine(player.AtCode));
            desk.PlayerList.ForEach(player => player.SendCardsMessage());
        }
    }

    public class CardMetel : SpecialCard
    {
        public override string Description { get; } = "弃掉全场功能牌";
        public override string ShortName { get; } = "MT";
        public override int Chance { get; } = 3;
        public override void Behave(Desk desk)
        {
            desk.PlayerList.ForEach(player => player.Cards.RemoveAll(card => card.Type != CardType.Number));
            desk.PlayerList.ForEach(player => player.SendCardsMessage());
        }
    }

}
