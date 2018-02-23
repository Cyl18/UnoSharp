using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Schema;

namespace UnoSharp
{
    public static class DeskRenderer
    {
        private static int GetEachLength(int baseLength) => (int)(baseLength / 5.0 * 2.0);
        private static int GetLength(int baseLength, int eachLength, int count) =>
            baseLength + eachLength * (count - 1);

        public static Image RenderBlankCards(int count)
        {
            if (count == 0)
                return new Bitmap(1, 1);
            if (count == 1)
                return Card.MainCardImage;

            var baseLength = Card.MainCardImage.Width;
            var eachWidth = GetEachLength(baseLength);
            var width = GetLength(baseLength, eachWidth, count);
            var point = new Point(0, 0);
            var bitmap = new Bitmap(width, Card.MainCardImage.Height);
            var grap = Graphics.FromImage(bitmap);
            using (grap)
            {
                for (var i = 0; i < count; i++)
                {
                    grap.DrawImage(Card.MainCardImage, point);
                    point.X += eachWidth;
                }
            }

            return bitmap;
        }

        public static Image RenderPlayers(this List<Player> players)
        {
            // init
            var font = new Font("Microsoft YaHei", 52);
            var blankCards = players.Select(player => player.PublicCard ? player.Cards.ToImage() : RenderBlankCards(player.Cards.Count));
            var enumerable = blankCards.ToArray();

            // text
            var textWidth = TextRenderer.MeasureText("P0", font).Width;
            const int margin = 120;
            var beforeRenderBlankCardWidth = margin + textWidth + margin;
            var textPoint = new Point(margin, 32);

            // blank card
            var maxWidth = enumerable.Max(image => image.Width);
            var baseHeight = enumerable.First().Height;
            var blankCardPoint = new Point(beforeRenderBlankCardWidth, 0);

            // main image
            var eachHeight = Math.Max(GetEachLength(baseHeight), 100); // prevent zero
            var height = GetLength(baseHeight, eachHeight, players.Count);
            var width = beforeRenderBlankCardWidth + maxWidth;

            var bitmap = new Bitmap(width, height);
            var grap = Graphics.FromImage(bitmap);

            using (grap)
                for (var index = 0; index < enumerable.Length; index++)
                {
                    var blankCard = enumerable[index];
                    var player = players[index];

                    grap.DrawImage(blankCard, blankCardPoint);
                    TextRenderer.DrawText(grap, player.Tag, font, textPoint, player.Uno ? Color.Red : Color.Gray);
                    if (player.IsCurrentPlayer())
                    {
                        DrawTextRect(grap, player.Tag, font, textPoint);
                        if (player.Desk.Reversed)
                        {
                            DrawArraw(grap, new Point(textWidth / 2 + margin, textPoint.Y - 20), new Point(textWidth / 2 + margin, textPoint.Y - 20 - 50));
                        }
                        else
                        {
                            DrawArraw(grap, new Point(textWidth / 2 + margin, textPoint.Y + 80 + 20), new Point(textWidth / 2 + margin, textPoint.Y + 80 + 20 + 50));
                        }
                    }
                    blankCardPoint.Y += eachHeight;
                    textPoint.Y += eachHeight;

                }

            return bitmap;
        }

        public static void DrawTextRect(this Graphics grap, string text, Font font, Point point)
        {
            var rectSize = 10;
            var margin = 10;
            var pen = new Pen(Color.Aquamarine, rectSize);
            var size = TextRenderer.MeasureText(text, font);
            var all = margin;
            var pDraw = new Point(point.X - all, point.Y - all);
            var width = size.Width + margin; // margin*2/2
            var height = size.Height + margin;
            grap.DrawRectangle(pen, pDraw.X, pDraw.Y, width, height);
        }

        public static void DrawArraw(this Graphics grap, Point from, Point to)
        {
            var pen = new Pen(Color.Cyan, 15)
            {
                StartCap = LineCap.NoAnchor,
                EndCap = LineCap.ArrowAnchor
            };
            grap.DrawLine(pen, from, to);
        }

        public static Image RenderLastCard(this Image lastCard)
        {
            // init
            var font = new Font("Microsoft YaHei", 48);

            // text
            const string text = "上一张牌";
            var textWidth = TextRenderer.MeasureText(text, font).Width;
            const int margin = 44;
            var beforeRenderLastCardWidth = margin + textWidth + margin;
            var textPoint = new Point(margin, 40);

            // last card
            var cardWidth = lastCard.Width;
            var width = beforeRenderLastCardWidth + cardWidth;
            var height = lastCard.Height;
            var cardPoint = new Point(beforeRenderLastCardWidth, 0);

            var bitmap = new Bitmap(width, height);
            var grap = Graphics.FromImage(bitmap);

            using (grap)
            {
                grap.DrawImage(lastCard, cardPoint);
                TextRenderer.DrawText(grap, text, font, textPoint, Color.Gray);
            }

            return bitmap;
        }

        public static Image RenderDesk(this Desk desk)
        {
            var playersImage = desk.PlayerList.RenderPlayers();
            var lastCardImage = desk.LastCard.ToImage().RenderLastCard();
            const int margin = 40;
            var width = Math.Max(playersImage.Width, playersImage.Width) + margin;
            var height = margin + playersImage.Height + margin + lastCardImage.Height + margin;
            var bitmap = new Bitmap(width, height);
            var grap = Graphics.FromImage(bitmap);
            var point = new Point();
            using (grap)
            {
                point.Y += margin;
                grap.DrawImage(playersImage, point);
                point.Y += playersImage.Height;
                point.Y += margin;
                grap.DrawImage(lastCardImage, point);
                point.Y += margin;
            }

            return bitmap;
        }
    }
}
