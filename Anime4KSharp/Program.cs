using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Anime4KSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Error: Please specify input and output png files");
                return;
            }

            string inputFile = args[0];
            string outputFile = args[1];

            Bitmap img = new Bitmap(inputFile);
            img = copyType(img);

            float scale = 2f;

            if (args.Length >= 3)
            {
                scale = float.Parse(args[2]);
            }

            float pushStrength = scale / 6f;
            float pushGradStrength = scale / 2f;

            if (args.Length >= 4)
            {
                pushStrength = float.Parse(args[4]);
            }

            if (args.Length >= 5)
            {
                pushGradStrength = float.Parse(args[3]);
            }

            img = upscale(img, (int)(img.Width * scale), (int)(img.Height * scale));
            //img.Save("Bicubic.png", ImageFormat.Png);

            // Push twice to get sharper lines.
            for(int i = 0; i < 2; i++)
            {
                // Compute Luminance and store it to alpha channel.
                ImageProcess.ComputeLuminance(ref img);
                //img.Save("Luminance.png", ImageFormat.Png);

                // Push (Notice that the alpha channel is pushed with rgb channels).
                ImageProcess.PushColor(ref img, clamp((int)(pushStrength * 255), 0, 0xFFFF));
                //img.Save("Push.png", ImageFormat.Png);

                // Compute Gradient of Luminance and store it to alpha channel.
                ImageProcess.ComputeGradient(ref img);
                //img.Save("Grad.png", ImageFormat.Png);

                // Push Gradient
                ImageProcess.PushGradient(ref img, clamp((int)(pushGradStrength * 255), 0, 0xFFFF));
            }
            img.Save(outputFile, ImageFormat.Png);
        }

        static Bitmap copyType(Bitmap bm)
        {
            Rectangle rect = new Rectangle(0, 0, bm.Width, bm.Height);
            Bitmap clone = bm.Clone(rect, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            return clone;
        }

        static Bitmap upscale(Bitmap bm, int width, int height)
        {
            // Upscale image with Bicubic interpolation.
            Bitmap newImage = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
            Graphics g = Graphics.FromImage(newImage);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.DrawImage(bm, 0, 0, width, height);
            return newImage;
        }

        private static int clamp(int i, int min, int max)
        {
            if (i < min)
            {
                i = min;
            }
            else if (i > max)
            {
                i = max;
            }

            return i;
        }
    }
}
