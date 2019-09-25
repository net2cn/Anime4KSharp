using System;
using System.Drawing;

namespace Anime4KSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            //if (args.Length < 2)
            //{
            //    Console.WriteLine("Error: Please specify input and output png files");
            //    return;
            //}

            //string inputFile = args[0];
            //string outputFile = args[1];

            string inputFile = "D:\\Video Materials\\TWEWY_Copy\\f113.png";
            string outputFile = "D:\\Video Materials\\TWEWY_Copy\\f113_Ref.png";

            Bitmap img = new Bitmap(inputFile);
            img = copyType(img);

            float scale = 2f;

            if (args.Length >= 3)
            {
                scale = float.Parse(args[2]);
            }

            float pushStrength = 0.3f;
            float pushGradStrength = 1f;

            if (args.Length >= 4)
            {
                pushGradStrength = float.Parse(args[3]);
            }

            if (args.Length >= 5)
            {
                pushStrength = float.Parse(args[4]);
            }

            img = upscale(img, (int)(img.Width * scale), (int)(img.Height * scale));

            img.Save("D:\\Video Materials\\TWEWY_Copy\\Bilinear.png", System.Drawing.Imaging.ImageFormat.Png);

            // Compute Luminance and store it to alpha channel.
            ImageProcess.ComputeLuminance(ref img);

            // Push (Notice that the alpha channel is pushed with rgb channels).
            ImageProcess.PushColor(ref img, clamp((int)(pushStrength * 255), 0, 0xFFFF));

            // Compute Gradient of Luminance and store it to alpha channel.
            ImageProcess.ComputeGradient(ref img);

            // Push Gradient
            ImageProcess.PushGradient(ref img, clamp((int)(pushGradStrength * 255), 0, 0xFFFF));

            img.Save(outputFile, System.Drawing.Imaging.ImageFormat.Png);
        }

        static Bitmap copyType(Bitmap bm)
        {
            Rectangle rect = new Rectangle(0, 0, bm.Width, bm.Height);
            Bitmap clone = bm.Clone(rect, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            return clone;
        }

        static Bitmap upscale(Bitmap bm, int width, int height)
        {
            Bitmap newImage = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
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
