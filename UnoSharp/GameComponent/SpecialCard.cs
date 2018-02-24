using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnoSharp.GameComponent
{
    public abstract class SpecialCard : Card, ISpecialCard, IEquatable<SpecialCard>
    {
        protected SpecialCard() : base(CardValue.Special, CardColor.Special)
        {

        }

        public abstract string Description { get; }
        public abstract string ShortName { get; }
        public abstract int Chance { get; }
        public abstract void Behave(Desk desk);

        public override string ToString()
        {
            return ShortName;
        }

        public override Bitmap ToImage()
        {
            var path = $"UnoSharp.Resources.Cards.{this.Color}.{ShortName}.png";
            var content = EmbedResourceReader.GetStream(path);
            if (content == null) return (Bitmap)Card.GoldenCardImage;
            return new Bitmap(content);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SpecialCard);
        }

        public bool Equals(SpecialCard other)
        {
            return other != null &&
                   base.Equals(other) &&
                   ShortName == other.ShortName;
        }

        public override int GetHashCode()
        {
            return 1404916966 + EqualityComparer<string>.Default.GetHashCode(ShortName);
        }

        public static bool operator ==(SpecialCard card1, SpecialCard card2)
        {
            return EqualityComparer<SpecialCard>.Default.Equals(card1, card2);
        }

        public static bool operator !=(SpecialCard card1, SpecialCard card2)
        {
            return !(card1 == card2);
        }
    }
}
