using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnoSharp.GameComponent
{
    public interface ISpecialCard
    {
        string Description { get; }
        string ShortName { get; }
        int Chance { get; }
        void Behave(Desk desk);

    }
}
