using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnoSharp
{
    public static class GaussianHelper
    {
        const int CHANNELS = 4;

        public static Bitmap CreateShadow(Image bitmap, int radius, float opacity)
        {
            // Alpha mask with opacity
            var matrix = new ColorMatrix(new float[][]
            {
                new float[] {0F, 0F, 0F, 0F, 0F},
                new float[] {0F, 0F, 0F, 0F, 0F},
                new float[] {0F, 0F, 0F, 0F, 0F},
                new float[] {-1F, -1F, -1F, opacity, 0F},
                new float[] {1F, 1F, 1F, 0F, 1F}
            });

            var imageAttributes = new ImageAttributes();
            imageAttributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            var shadow = new Bitmap(bitmap.Width + 4 * radius, bitmap.Height + 4 * radius);
            using (var graphics = Graphics.FromImage(shadow))
                graphics.DrawImage(bitmap, new Rectangle(2 * radius, 2 * radius, bitmap.Width, bitmap.Height), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, imageAttributes);

            // Gaussian blur
            var clone = shadow.Clone() as Bitmap;
            var shadowData = shadow.LockBits(new Rectangle(0, 0, shadow.Width, shadow.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            var cloneData = clone.LockBits(new Rectangle(0, 0, clone.Width, clone.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            var boxes = DetermineBoxes(radius, 3);
            BoxBlur(shadowData, cloneData, shadow.Width, shadow.Height, (boxes[0] - 1) / 2);
            BoxBlur(shadowData, cloneData, shadow.Width, shadow.Height, (boxes[1] - 1) / 2);
            BoxBlur(shadowData, cloneData, shadow.Width, shadow.Height, (boxes[2] - 1) / 2);

            shadow.UnlockBits(shadowData);
            clone.UnlockBits(cloneData);
            return shadow;
        }

        private static unsafe void BoxBlur(BitmapData data1, BitmapData data2, int width, int height, int radius)
        {
            byte* p1 = (byte*)(void*)data1.Scan0;
            byte* p2 = (byte*)(void*)data2.Scan0;

            int radius2 = 2 * radius + 1;
            int[] sum = new int[CHANNELS];
            int[] FirstValue = new int[CHANNELS];
            int[] LastValue = new int[CHANNELS];

            // Horizontal
            int stride = data1.Stride;
            for (var row = 0; row < height; row++)
            {
                int start = row * stride;
                int left = start;
                int right = start + radius * CHANNELS;

                for (int channel = 0; channel < CHANNELS; channel++)
                {
                    FirstValue[channel] = p1[start + channel];
                    LastValue[channel] = p1[start + (width - 1) * CHANNELS + channel];
                    sum[channel] = (radius + 1) * FirstValue[channel];
                }
                for (var column = 0; column < radius; column++)
                    for (int channel = 0; channel < CHANNELS; channel++)
                        sum[channel] += p1[start + column * CHANNELS + channel];
                for (var column = 0; column <= radius; column++, right += CHANNELS, start += CHANNELS)
                    for (int channel = 0; channel < CHANNELS; channel++)
                    {
                        sum[channel] += p1[right + channel] - FirstValue[channel];
                        p2[start + channel] = (byte)(sum[channel] / radius2);
                    }
                for (var column = radius + 1; column < width - radius; column++, left += CHANNELS, right += CHANNELS, start += CHANNELS)
                    for (int channel = 0; channel < CHANNELS; channel++)
                    {
                        sum[channel] += p1[right + channel] - p1[left + channel];
                        p2[start + channel] = (byte)(sum[channel] / radius2);
                    }
                for (var column = width - radius; column < width; column++, left += CHANNELS, start += CHANNELS)
                    for (int channel = 0; channel < CHANNELS; channel++)
                    {
                        sum[channel] += LastValue[channel] - p1[left + channel];
                        p2[start + channel] = (byte)(sum[channel] / radius2);
                    }
            }

            // Vertical
            stride = data2.Stride;
            for (int column = 0; column < width; column++)
            {
                int start = column * CHANNELS;
                int top = start;
                int bottom = start + radius * stride;

                for (int channel = 0; channel < CHANNELS; channel++)
                {
                    FirstValue[channel] = p2[start + channel];
                    LastValue[channel] = p2[start + (height - 1) * stride + channel];
                    sum[channel] = (radius + 1) * FirstValue[channel];
                }
                for (int row = 0; row < radius; row++)
                    for (int channel = 0; channel < CHANNELS; channel++)
                        sum[channel] += p2[start + row * stride + channel];
                for (int row = 0; row <= radius; row++, bottom += stride, start += stride)
                    for (int channel = 0; channel < CHANNELS; channel++)
                    {
                        sum[channel] += p2[bottom + channel] - FirstValue[channel];
                        p1[start + channel] = (byte)(sum[channel] / radius2);
                    }
                for (int row = radius + 1; row < height - radius; row++, top += stride, bottom += stride, start += stride)
                    for (int channel = 0; channel < CHANNELS; channel++)
                    {
                        sum[channel] += p2[bottom + channel] - p2[top + channel];
                        p1[start + channel] = (byte)(sum[channel] / radius2);
                    }
                for (int row = height - radius; row < height; row++, top += stride, start += stride)
                    for (int channel = 0; channel < CHANNELS; channel++)
                    {
                        sum[channel] += LastValue[channel] - p2[top + channel];
                        p1[start + channel] = (byte)(sum[channel] / radius2);
                    }
            }
        }

        private static int[] DetermineBoxes(double Sigma, int BoxCount)
        {
            double IdealWidth = Math.Sqrt((12 * Sigma * Sigma / BoxCount) + 1);
            int Lower = (int)Math.Floor(IdealWidth);
            if (Lower % 2 == 0)
                Lower--;
            int Upper = Lower + 2;

            double MedianWidth = (12 * Sigma * Sigma - BoxCount * Lower * Lower - 4 * BoxCount * Lower - 3 * BoxCount) / (-4 * Lower - 4);
            int Median = (int)Math.Round(MedianWidth);

            int[] BoxSizes = new int[BoxCount];
            for (int i = 0; i < BoxCount; i++)
                BoxSizes[i] = (i < Median) ? Lower : Upper;
            return BoxSizes;
        }
    }
}
