using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnoSharp.GameComponent
{
    public class FakePlayer : Player
    {
        private static readonly Random Rng = new Random(142857143);
        public FakePlayer(string nick, Desk desk) : base(Rng.Next(300000).ToString(), desk)
        {

        }
        
        public override void AddMessage(string msg)
        {
        }

        
    }
}
