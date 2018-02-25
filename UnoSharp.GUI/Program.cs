#define GUI
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnoSharp;
using UnoSharp.GameStep;

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
            nks["LasmGratel"] = "LasmGratel";
            nks["Cyl17"] = "Cyl17";
            nks["CharlieJiang"] = "CharlieJiang";
            nks["Baka84"] = "Baka84";
            conf.Save();

            nd.AddPlayer(new Player("LasmGratel", nd));
            nd.AddPlayer(new Player("Cyl17", nd));
            nd.AddPlayer(new Player("CharlieJiang", nd));
            nd.AddPlayer(new Player("Baka84", nd));
            nd.AddPlayer(new Player("LasmG2ratel", nd));
            nd.AddPlayer(new Player("Cyl127", nd));
            nd.AddPlayer(new Player("Charl2ieJiang", nd));
            nd.AddPlayer(new Player("Bak2a84", nd));
            nd.StartGame();
            //nd.CurrentParser.Reversed = true;
            nd.CurrentParser.CurrentIndex = 5;
            nd.RenderDesk().Save("px.png", ImageFormat.Png);*/

            //DeskRenderer.RenderDesk(desk).Save("test5.png");
            //var writer = File.CreateText("test.txt");
            //writer.Write(GameStepBase.ToGenericCommand("摸"));
            //writer.Close();
        }
    }
}
