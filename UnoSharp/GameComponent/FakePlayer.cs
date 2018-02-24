using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnoSharp.GameComponent
{
    public class FakePlayer : Player
    {
        private static readonly Random Rng = new Random("NERvGear".GetHashCode());
        public FakePlayer(string nick, Desk desk) : base(Rng.Next(300000).ToString(), desk)
        {
            Nick = nick;
        }

        public override string Nick { get; }
        public override bool AutoSubmitCard { get; internal set; } = true;

        public override string ToAtCode()
        {
            return Nick;
        }

        public override string AtCode => Nick;

        public override void AddMessage(string msg)
        {
        }
        
    }
}
