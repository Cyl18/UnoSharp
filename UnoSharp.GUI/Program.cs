using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnoSharp;

namespace UnoSharp.GUI
{
    public class Program
    {
        static void Main(string[] args)
        {
            //var s = string.Join(", ", Card.CardsPool.Select(card => card.ToString()));
            //Console.WriteLine(s);


            // To use code below you need to change some access modifiers and comment Line 55 (image.Resize(image.Width / 3, image.Height / 3).Save(filename);) in ImageExtensions.cs
            /*Desk nd = new Desk("222");
            var conf = Config.Get();
            var nks = conf.Nicks;
            nks["LasmGratel"] = "苟枫凌";
            nks["Cyl17"] = "A125";
            nks["CharlieJiang"] = "RED";
            nks["Baka84"] = "膜法少女LG大续命师";
            conf.Save();

            nd.AddPlayer(new Player("LasmGratel", nd));
            nd.AddPlayer(new Player("Cyl17", nd));
            nd.AddPlayer(new Player("Baka84", nd));
            nd.AddPlayer(new Player("CharlieJiang", nd));
            
            nd.StartGame();
            nd.CurrentParser.Reversed = true;
            nd.CurrentParser.CurrentIndex = 0;
            nd.RenderDesk().Save("px.png", ImageFormat.Png);*/

            //DeskRenderer.RenderDesk(desk).Save("test5.png");
        }
    }
}
