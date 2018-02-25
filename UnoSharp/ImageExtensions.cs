using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UnoSharp
{
    public static class ImageExtensions
    {
        

        public static Image ToImage(this ICollection<Card> cards)
        {
            if (cards.Count == 0) return new Bitmap(1, 1);
            if (cards.Count == 1) return cards.First().ToImage();
            var cardlist = cards.ToArray();
            var images = cards.Select(card => card.ToImage()).ToList();
            var first = images.First();
            var eachWidth = (int) (first.Width / 5.0 * 2.0);
            var width = first.Width + eachWidth * (cards.Count - 1);
            var height = first.Height;

            var bitmap = new Bitmap(width, height + 55);
            var result = Graphics.FromImage(bitmap);
            using (result)
            {
                var point = new Point(0, 0);
                var textpoint = new Point(first.Width/2-80, height + 5);
                var font = new Font("Microsoft YaHei", 32);

                for (var index = 0; index < images.Count; index++)
                {
                    var image = images[index];
                    var card = cardlist[index];

                    result.DrawImage(image, point);
                    result.DrawString(card.ToShortString(), font, new SolidBrush(card.DrawColor), textpoint);
                    point.X += eachWidth;
                    textpoint.X += eachWidth;
                }
            
                return bitmap;
            }
        }
        //private static readonly Random Rng = new Random("Chtholly Nota Seniorious".GetHashCode());
        public static string ToImageCodeAndDispose(this Image image, bool forceOverwrite = false)
        {
            string filename = image.Resize(image.Width / 3, image.Height / 3).SaveToData(forceOverwrite);
            image.Dispose();

            return $"[CQ:image,file={filename}.jpg]";
        }

        public static Bitmap Resize(this Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage)) {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes()) {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public const string ImagePath = "data\\image";
        private static string SaveToData(this Bitmap image, bool overwrite)
        {
            byte[] bytes;
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Jpeg);
                bytes = ms.ToArray();
            }
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("x2").ToLower());
            }
            var filename = $@"{ImagePath}\{sb}.jpg";
            if (!File.Exists(filename) || overwrite)
            {
                if (!Directory.Exists(ImagePath)) Directory.CreateDirectory(ImagePath);
                File.WriteAllBytes(filename, bytes);
            }
            return filename;
        }
    }
}
