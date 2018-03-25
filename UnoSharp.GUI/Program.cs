using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnoSharp;
using UnoSharp.GameComponent;
using UnoSharp.GameStep;
using Timer = System.Timers.Timer;

namespace UnoSharp.GUI
{
    public class Program
    {
        private static void Main(string[] args)
        {
            //var s = string.Join(", ", Card.CardsPool.Select(card => card.ToString()));
            //Console.WriteLine(s);

            // To use code below you need to change some access modifiers and comment Line 55 (image.Resize(image.Width / 3, image.Height / 3).Save(filename);) in ImageExtensions.cs

            Desk nd = new Desk("222");
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
            Thread.Sleep(1000);
            var timer = new Timer(100);
            timer.Elapsed += (sender, aargs) =>
            {
                var desk = nd;
                Samsara.DoAutoSubmitCard(desk);
                if (desk.Message != null)
                {
                    var msg = desk.Message;
                    desk.ClearMessage();
                    Console.WriteLine(msg);
                }

                foreach (var player in desk.Players.Where(player => player.Message != null))
                {
                    var msg = player.Message;
                    player.ClearMessage();
                    Console.WriteLine(msg);
                }
            };
            //timer.Start();
            //nd.CurrentParser.Reversed = true;
            //nd.CurrentParser.CurrentIndex = 0;
            nd.CurrentPlayer.AddCardsAndSort(1000);
            Thread.Sleep(1000);
            for (int i = 0; i < 4; i++)
            {
                nd.RenderDesk();
            }
            // nd.RenderDesk();
            //  nd.RenderDesk();
            //   nd.RenderDesk();
            //    nd.RenderDesk();
            //    nd.RenderDesk();
            //    nd.RenderDesk();
            //    nd.RenderDesk();
            //    nd.RenderDesk();
            //    nd.RenderDesk();
            //    nd.RenderDesk();
            //    nd.RenderDesk();
            //    nd.RenderDesk();
            //     nd.RenderDesk();
            //    nd.RenderDesk();
            //    nd.RenderDesk();

            //Card.CardsPool.ToImage().Save("test.png");
            //DeskRenderer.RenderDesk(desk).Save("test5.png");
            //var writer = File.CreateText("test.txt");
            //writer.Write(GameStepBase.ToGenericCommand("摸"));
            //writer.Close();
        }
    }
}