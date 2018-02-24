using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnoSharp.GameComponent;

namespace UnoSharp
{
    public class Player : MessageSenderBase, IEquatable<Player>
    {
        public Player(string playerId, Desk desk)
        {
            PlayerId = playerId;
            Desk = desk;
        }

        public string PlayerId { get; }
        public Desk Desk { get; }
        public int Index { get; internal set; }
        public string Tag => $"P{Index+1}";
        public string AtCode => $"{Nick}-{ToAtCode()}";
        public List<Card> Cards { get; internal set; } = new List<Card>();

        public bool Equals(Player other)
        {
            if (other is null) return false;
            return ReferenceEquals(this, other) || string.Equals(PlayerId, other.PlayerId);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((Player) obj);
        }

        public override int GetHashCode()
        {
            return PlayerId.GetHashCode();
        }

        public static bool operator ==(Player left, Player right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Player left, Player right)
        {
            return !Equals(left, right);
        }

        public bool Uno { get; internal set; }
        public bool PublicCard { get; internal set; }
        public DateTime LastSendTime { get; internal set; }

        public virtual string Nick
        {
            get
            {
                var config = Config.Get();
                if (config.Nicks.ContainsKey(PlayerId))
                {
                    return config.Nicks[PlayerId];
                }
                else
                {
                    return Tag;
                }
            }
        }

        public string ToAtCode()
        {
#if !DEBUG
            return $"[CQ:at,qq={PlayerId}]";
#else
            return $"{PlayerId}";
#endif
        }

        public void SendCardsMessage()
        {
            AddMessage(Cards.ToImage().ToImageCode());
        }

        public void AddCardsAndSort(int count)
        {
            var cards = Card.Generate(count).ToArray();
            if (cards.Any(card =>card is ISpecialCard))
            {
                SendSpecialCardMessage(cards);
            }
            Cards.AddRange(cards);
            Cards.Sort();
            if (Uno) Uno = false;
            SendCardsMessage();
        }

        private void SendSpecialCardMessage(Card[] cards)
        {
            AddMessageLine("诶嘿嘿你手里好像收到了特殊牌哟~ 让我来为你解释一下规则: ");
            foreach (var sp in cards.OfType<ISpecialCard>())
            {
                AddMessageLine($"{sp.ShortName}: {sp.Description}");
            }

        }

        public bool IsCurrentPlayer()
        {
            return Desk.CurrentPlayer.PlayerId == PlayerId;
        }

    }
}
