using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    if (thisCard.Type == CardType.DrawFour || thisCard.Type == CardType.Wild)
                        return true;
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
        private static readonly Random Rng = new Random("11223344sblg".GetHashCode());


        public static string ExtractCommand(List<Card> cards, Card lastCard, GamingState state)
        {
            switch (state)
            {
                case GamingState.Gaming:
                case GamingState.WaitingDrawTwoOverlay:
                case GamingState.WaitingDrawFourOverlay:
                    var card = ExtractCard(cards, lastCard);
                    return card == null ? "draw" : card.ToShortString();
                case GamingState.Doubting:
                    return Rng.Next(8) > 5 ? "质疑" : "不质疑";
            }
        }

        public static Card ExtractCard(List<Card> cards, Card lastCard)
        {
            
        }
    }
}
