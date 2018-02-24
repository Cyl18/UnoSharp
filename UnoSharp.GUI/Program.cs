using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnoSharp;

namespace UnoSharp.GUI
{
    public class Program
    {
        static void Main(string[] args)
        {
            var s = string.Join(", ", Card.CardsPool.Select(card => card.ToString()));
            Console.WriteLine(s);
            //DeskRenderer.RenderDesk(desk).Save("test5.png");
        }
    }
}
