using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Schema;
using UnoSharp.GameComponent;

namespace UnoSharp
{
    public static class DeskRenderer
    {
        private static readonly float Opacity = 0.3f;
        private static readonly int Padding = 20;

        private static int GetEachLength(int baseLength) => (int)(baseLength / 5.0 * 2.0);
        private static int GetLength(int baseLength, int eachLength, int count) =>
            baseLength + eachLength * (count - 1);

        public static void RenderImageWithShadow(this Graphics graphics, Image image, Point point, int radius, float opacity)
        {
            using (Image shadow = GaussianHelper.CreateShadow(image, radius, opacity))
                graphics.DrawImage(shadow, point);
            graphics.DrawImage(image, point);
        }

        public static Image RenderBlankCards(int count, int goldenCards)
        {
            if (count == 0)
                return new Bitmap(1, 1);
            if (count == 1)
                return Card.MainCardImage;

            var baseLength = Card.MainCardImage.Width;
            var eachWidth = GetEachLength(baseLength);
            var width = GetLength(baseLength, eachWidth, count);
            var point = new Point(0, 0);
            var bitmap = new Bitmap(width + Padding, Card.MainCardImage.Height + Padding); // 增加一个阴影的padding
            var grap = Graphics.FromImage(bitmap);
            using (grap)
            {
                for (var i = 0; i < count - goldenCards; i++)
                {
                    grap.RenderImageWithShadow(Card.MainCardImage, point, 5, Opacity);
                    point.X += eachWidth;
                }

                for (var i = 0; i < goldenCards; i++)
                {
                    grap.RenderImageWithShadow(Card.GoldenCardImage, point, 5, Opacity);
                    point.X += eachWidth;
                }
            }

            return bitmap;
        }

        public static Image RenderPlayers(this List<Player> players)
        {
            // init
            var font = new Font("Microsoft YaHei", 52);
            var blankCards = from player in players
                select player.PublicCard
                    ? player.Cards.ToImage()
                    : RenderBlankCards(player.Cards.Count, player.Cards.Count(card => card is ISpecialCard));
            var enumerable = blankCards.ToArray();
            var nicks = players.Select(player => player.Nick);

            // text
            var longest = nicks.Max(nick => nick.Length);

            const int margin = 120;
            var maxTextSize = TextRenderer.MeasureText(new string('珂', longest), font);
            var textWidth = maxTextSize.Width;
            var textHeight = maxTextSize.Height;
            var textCenterWidth = textWidth / 2;
            var textCenter = textCenterWidth + margin;
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

                    grap.RenderImageWithShadow(blankCard, blankCardPoint, 5, Opacity);
                    textPoint.X = margin + textCenterWidth - TextRenderer.MeasureText(player.Nick, font).Width / 2;
                    TextRenderer.DrawText(grap, player.Nick, font, textPoint, player.Uno ? Color.Red : (player.AutoSubmitCard ? Color.BlueViolet : Color.Gray));
                    if (player.IsCurrentPlayer())
                    {
                        DrawTextRect(grap, player.Nick, font, textPoint);
                        if (player.Desk.Reversed)
                        {
                            if (index == 0)
                            {
                                var pen = new Pen(Color.Cyan, 15);
                                var upLineY = textPoint.Y + (textHeight + 15) / 2;
                                var leftLineX = margin - 10;
                                var anglePoint1 = new Point(leftLineX, upLineY);
                                grap.DrawLine(pen, new Point(textPoint.X - 10, upLineY), anglePoint1); // 本玩家拉出来的线
                                var downLineY = textPoint.Y + eachHeight * players.Count -
                                                (eachHeight - textHeight) - 40;
                                var anglePoint2 = new Point(leftLineX, downLineY);
                                anglePoint1.Y -= 5;
                                grap.DrawLine(pen, anglePoint1, anglePoint2);// 竖线
                                anglePoint2.X -= 5;
                                grap.DrawArraw(anglePoint2, new Point(margin + textCenterWidth - TextRenderer.MeasureText(players.Last().Nick, font).Width / 2 - 10, downLineY)); // 到目标玩家的箭头
                            }
                            else
                                DrawArraw(grap, new Point(textCenter, textPoint.Y - 20), new Point(textCenter, textPoint.Y - 20 - 50));
                        }
                        else
                        { //TODO bug
                            if (index == enumerable.Length - 1)
                            {
                                var pen = new Pen(Color.Cyan, 15);
                                var upLineY = textPoint.Y + (textHeight + 15) / 2;
                                var leftLineX = margin - 10;
                                var anglePoint1 = new Point(leftLineX, upLineY);
                                grap.DrawLine(pen, new Point(textPoint.X - 10, upLineY), anglePoint1);
                                var downLineY = textPoint.Y - eachHeight * players.Count +
                                                (eachHeight - textHeight) + 130;
                                var anglePoint2 = new Point(leftLineX, downLineY);
                                anglePoint1.Y += 5;
                                grap.DrawLine(pen, anglePoint1, anglePoint2); // 竖线
                                anglePoint2.X -= 5;
                                grap.DrawArraw(anglePoint2, new Point(margin + textCenterWidth - TextRenderer.MeasureText(players.First().Nick, font).Width / 2 - 10, downLineY));
                            }
                            else
                                DrawArraw(grap, new Point(textCenter, textPoint.Y + 80 + 20), new Point(textCenter, textPoint.Y + 80 + 20 + 50));
                        }
                    }
                    blankCardPoint.Y += eachHeight;
                    textPoint.Y += eachHeight;

                }

            foreach (var image in enumerable)
            {
                if (!image.IsCachedImage())
                    image.Dispose();
            }
            font.Dispose();
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
            var width = size.Width + margin * 2; // margin*2/2
            var height = size.Height + margin * 2;
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
                grap.RenderImageWithShadow(lastCard, cardPoint, 5, Opacity);
                TextRenderer.DrawText(grap, text, font, textPoint, Color.Gray);
            }

            if (!lastCard.IsCachedImage())
                lastCard.Dispose();

            return bitmap;
        }

        public static Image RenderDesk(this Desk desk)
        {
            Bitmap bitmap;
            using (Image 
                playersImage = desk.PlayerList.RenderPlayers(), 
                lastCardImage = desk.LastCard.ToImage().RenderLastCard())
            {
                const int margin = 40;
                var width = Math.Max(playersImage.Width, playersImage.Width) + margin;
                var height = margin + playersImage.Height + margin + lastCardImage.Height + margin;
                bitmap = new Bitmap(width, height);
                for (var i = 0; i < width; i++)
                    for (var j = 0; j < height; j++)
                        bitmap.SetPixel(i, j, Color.White);

                var grap = Graphics.FromImage(bitmap);
                var point = new Point();
                using (grap)
                {
                    point.Y += margin;
                    grap.RenderImageWithShadow(playersImage, point, 5, Opacity);
                    point.Y += playersImage.Height;
                    point.Y += margin;
                    grap.RenderImageWithShadow(lastCardImage, point, 5, Opacity);
                    point.Y += margin;
                }
            }
            

            return bitmap;
        }
    }
}
