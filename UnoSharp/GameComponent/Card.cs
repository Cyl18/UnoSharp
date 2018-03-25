using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnoSharp.GameComponent;

namespace UnoSharp
{
    public class Card : IEquatable<Card>, IComparable<Card>
    {
        public Card(CardValue value, CardColor color)
        {
            Value = value;
            Color = color;
            Type = ToType(value);
            DrawColor = ToColor(color);
        }

        private static readonly Color Blue = System.Drawing.Color.FromArgb(0x19, 0x76, 0xD2);
        private static readonly Color Yellow = System.Drawing.Color.FromArgb(0xFF, 0xEB, 0x3B);
        private static readonly Color Green = System.Drawing.Color.FromArgb(0x4C, 0xAF, 0x50);
        private static readonly Color Red = System.Drawing.Color.FromArgb(0xF4, 0x43, 0x36);
        private static readonly Color Wild = System.Drawing.Color.FromArgb(163, 154, 141);

        public CardColor Color { get; internal set; }
        public Color DrawColor { get; internal set; }
        public CardValue Value { get; }
        public CardType Type { get; }
        public int ValueNumber => (int)Value;

        public static readonly SpecialCard[] SpecialCards =
        {
            new Card84(),
            new CardA125(),
            new CardCJ(),
            new CardLG(),
            new CardShp(),
            new CardCY(),
            new CardToma(),
            new CardMetel(),
            new CardShiyu(),
            new CardJ10(),
            new CardXaro(),
            new CardLGGen2Point0(),
        };

        public static Card[] CardsPool { get; } = GenerateDefaultCards();

        public static Queue<Card> CardsQueue { get; } = new Queue<Card>();

        public static Image MainCardImage { get; } = GetMainCard();
        public static Image GoldenCardImage { get; } = GetGoldenCard();

        public static Card Generate()
        {
            return CardsQueue.Any() ? CardsQueue.Dequeue() : CardsPool.PickOne();
        }

        public static Card Generate(Predicate<Card> predicate)
        {
            while (true)
            {
                var card = CardsPool.PickOne();
                if (predicate(card))
                    return card;
            }
        }

        public static IEnumerable<Card> Generate(Predicate<Card> predicate, int count)
        {
            for (var i = 0; i < count; i++)
            {
                yield return Generate(predicate);
            }
        }

        public static IEnumerable<Card> Generate(int num)
        {
            for (var i = 0; i < num; i++)
            {
                yield return Generate();
            }
        }

        private static Card[] GenerateDefaultCards()
        {
            var list = new List<Card>();
            for (var i = 0; i < 10 + 5; i++)
            {
                var value = (CardValue)i;
                var chance = GetChance(value);
                for (var j = 0; j < chance; j++)
                {
                    list.AddRange(GetDefaultColors(value).Select(color => new Card(value, color)));
                }
            }

            foreach (var card in SpecialCards)
            {
                for (var i = 0; i < card.Chance; i++)
                {
                    list.Add(card);
                }
            }
            return list.ToArray();
        }

        public static int GetChance(CardValue value)
        {
            switch (value)
            {
                case CardValue.Zero:
                    return 2;

                default: // non zero number
                    return 4;
            }
        }

        public static IEnumerable<CardColor> GetDefaultColors(CardValue value)
        {
            switch (value)
            {
                case CardValue.DrawFour:
                case CardValue.Wild:
                    yield return CardColor.Wild;
                    yield return CardColor.Wild;
                    break;

                case CardValue.Special:
                    break;

                default:
                    yield return CardColor.Red;
                    yield return CardColor.Blue;
                    yield return CardColor.Yellow;
                    yield return CardColor.Green;
                    break;
            }
        }

        public static CardType ToType(CardValue value)
        {
            switch (value)
            {
                case CardValue.Reverse: return CardType.Reverse;
                case CardValue.Skip: return CardType.Skip;
                case CardValue.DrawTwo: return CardType.DrawTwo;
                case CardValue.DrawFour: return CardType.DrawFour;
                case CardValue.Wild: return CardType.Wild;
                case CardValue.Special: return CardType.Special;
                default: return CardType.Number;
            }
        }

        public static Color ToColor(CardColor color)
        {
            switch (color)
            {
                case CardColor.Red:
                    return Red;

                case CardColor.Yellow:
                    return Yellow;

                case CardColor.Green:
                    return Green;

                case CardColor.Blue:
                    return Blue;

                case CardColor.Wild:
                case CardColor.Special:
                    return Wild;

                default:
                    throw new ArgumentOutOfRangeException(nameof(color), color, null);
            }
        }

        public int CompareTo(Card other)
        {
            return ((int)this).CompareTo(((int)other));
        }

        public override string ToString()
        {
            switch (Type)
            {
                case CardType.Number:
                    return $"{Color} {(int)Value}";

                default:
                    return $"{Color} {Value}";
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Card);
        }

        public bool Equals(Card other)
        {
            return other != null &&
                   Color == other.Color &&
                   Value == other.Value;
        }

        public static bool operator ==(Card card1, Card card2)
        {
            return EqualityComparer<Card>.Default.Equals(card1, card2);
        }

        public static bool operator !=(Card card1, Card card2)
        {
            return !(card1 == card2);
        }

        public static explicit operator int(Card card)
        {
            return 15 * (int)card.Color + card.ValueNumber;
        }

        public static Image GetMainCard()
        {
            var path = $"UnoSharp.Resources.Cards.MainCard.png";
            var content = EmbedResourceReader.GetStream(path);
            return new Bitmap(content);
        }

        private static Image GetGoldenCard()
        {
            var path = $"UnoSharp.Resources.Cards.GoldenCard.png";
            var content = EmbedResourceReader.GetStream(path);
            return new Bitmap(content);
        }

        public virtual Bitmap ToImage()
        {
            var path = $"UnoSharp.Resources.Cards.{this.Color}.{this.Value}.png";
            var content = EmbedResourceReader.GetStream(path);
            if (content == null) return (Bitmap)Card.GoldenCardImage;
            return new Bitmap(content);
        }
    }

    public static class CardExtensions
    {
        public static string ToShortString(this Card card)
        {
            var sb = new StringBuilder();
            switch (card.Color)
            {
                case CardColor.Wild:
                    break;

                case CardColor.Red:
                    sb.Append("R");
                    break;

                case CardColor.Yellow:
                    sb.Append("Y");
                    break;

                case CardColor.Green:
                    sb.Append("G");
                    break;

                case CardColor.Blue:
                    sb.Append("B");
                    break;

                case CardColor.Special:
                    return ((ISpecialCard)card).ShortName;
            }
            switch (card.Type)
            {
                case CardType.Wild:
                    sb.Append("W");
                    break;

                case CardType.DrawTwo:
                    sb.Append("+2");
                    break;

                case CardType.Number:
                    sb.Append(card.ValueNumber);
                    break;

                case CardType.Reverse:
                    sb.Append("R");
                    break;

                case CardType.Skip:
                    sb.Append("S");
                    break;

                case CardType.DrawFour:
                    sb.Append("+4");
                    break;
            }

            return sb.ToString();
        }

        public static bool ContainsType(this IEnumerable<Card> cards, CardType type)
        {
            return cards.Any(card => card.Type == type);
        }

        public static bool ContainsColor(this IEnumerable<Card> cards, CardColor color)
        {
            return cards.Any(card => card.Color == color);
        }

        public static bool ContainsValue(this IEnumerable<Card> cards, CardValue value)
        {
            return cards.Any(card => card.Value == value);
        }

        public static bool IsValidForPlayerAndRemove(this Card card, Player player)
        {
            for (var index = 0; index < player.Cards.Count; index++)
            {
                var playerCard = player.Cards[index];
                switch (card.Type)
                {
                    case CardType.Number:
                    case CardType.Skip:
                    case CardType.Reverse:
                    case CardType.DrawTwo:
                        if (playerCard.Value == card.Value && playerCard.Color == card.Color)
                        {
                            player.Cards.Remove(playerCard);
                            return true;
                        }
                        continue;
                    case CardType.DrawFour:
                    case CardType.Wild:
                        if (playerCard.Value == card.Value)
                        {
                            player.Cards.Remove(playerCard);
                            return true;
                        }
                        continue;
                    case CardType.Special: // 假装不知道如何重写运算符
                        if (!(playerCard is ISpecialCard)) continue;
                        if (((ISpecialCard)card).ShortName == ((ISpecialCard)playerCard).ShortName)
                        {
                            player.Cards.RemoveAt(index);
                            return true;
                        }
                        continue;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return false;
        }
    }

    public enum CardColor
    {
        Red,
        Yellow,
        Green,
        Blue,

        Wild,

        Special
    }

    public enum CardValue
    {
        Zero,

        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,

        Reverse,
        Skip,
        DrawTwo,

        Wild,
        DrawFour,

        Special
    }

    public enum CardType
    {
        Number,

        Reverse,
        Skip,
        DrawTwo,

        Wild,
        DrawFour,

        Special
    }
}