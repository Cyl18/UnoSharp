using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnoSharp
{
    public class UnoRule
    {
        public static bool IsValid(Card thisCard, Card lastCard, GamingState state)
        {
            switch (state)
            {
                case GamingState.Gaming:
                    switch (thisCard.Type)
                    {
                        case CardType.DrawFour:
                        case CardType.Wild:
                        case CardType.Special:
                            return true;
                    }
                    return thisCard.Color == lastCard.Color || thisCard.ValueNumber == lastCard.ValueNumber;
                case GamingState.WaitingDrawTwoOverlay:
                    return thisCard.Type == CardType.DrawTwo;
                case GamingState.WaitingDrawFourOverlay:
                    return thisCard.Type == CardType.DrawFour;
                case GamingState.Doubting:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, "Unknown state!");
            }
        }
        private static readonly Random Rng = new Random("YuukiAsuna-Kantinatron-Neutron-KirigayaKazuto-Cryptoshop-Nephren Ruq Insania".GetHashCode());

        public static bool IsValidForFollowCard(Card thisCard, Card lastCard, GamingState state)
        {
            switch (state)
            {
                case GamingState.Gaming:
                    return thisCard.Type == CardType.Number && thisCard.Color == lastCard.Color && thisCard.Value == lastCard.Value;
                case GamingState.WaitingDrawTwoOverlay:
                case GamingState.WaitingDrawFourOverlay:
                case GamingState.Doubting:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
        public static string ExtractCommand(List<Card> cards, Card lastCard, GamingState state)
        {
            switch (state)
            {
                case GamingState.Gaming:
                case GamingState.WaitingDrawTwoOverlay:
                case GamingState.WaitingDrawFourOverlay:
                    var card = ExtractCard(cards, lastCard, state);
                    if (card?.Color == CardColor.Wild) card.Color = ToWildColor(cards);
                    return card == null ? "摸了" : card.ToShortString();
                case GamingState.Doubting:
                    return Rng.Next(8) > 5 ? "喵喵喵?" : "不质疑";
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
        
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
        public static CardColor ToWildColor(List<Card> cards)
        {
            var gcards = cards.Where(card => card.Color != CardColor.Wild && card.Color != CardColor.Special).ToList();
            if (gcards.Count == 0) return PickColor();
            var dic = new Dictionary<CardColor, int>
            {
                {CardColor.Red, 0},
                {CardColor.Green, 0},
                {CardColor.Blue, 0},
                {CardColor.Yellow, 0}
            };
            foreach (var gcard in gcards)
            {
                dic[gcard.Color]++;
            }

            return dic.OrderBy(item => item.Value).First().Key;
        }

        public static Card ExtractCard(List<Card> cards, Card lastCard, GamingState state)
        {
            var valids = new List<Card>();
            foreach (var card in cards)
            {
                if (IsValid(card, lastCard, state))
                {
                    valids.Add(card);
                }
            }

            if (valids.Count == 0) return null;
            
            var valueWithCards = (from valid in valids select new {ValidCard = valid, Value = GetValue(valid, lastCard, valids, cards)}).ToArray();
            var maxs = valueWithCards.Where(i => i.Value == valueWithCards.Max(item => item.Value)).ToList();
            return maxs.PickOne().ValidCard;
        }

        private static int GetValue(Card valid, Card lastCard, List<Card> valids, List<Card> allCards)
        {
            var value = 0;
            if (valid.Color == CardColor.Special) value += -50;
            if (valid.Color == CardColor.Wild) value += -80;
            if (valid.Type == CardType.Number) value += 1;
            if (valid.Type == CardType.Reverse) value += 10; // 功能牌优先
            if (valid.Type == CardType.Skip) value += 10;
            if (valid.Type == CardType.DrawTwo) value += 10;
            if (allCards.Count(card => card.ValueNumber == valid.ValueNumber) > 1) value += 5;
            return value;
        }
    }
}
