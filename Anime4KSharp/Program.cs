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

            string inputFile = "D:\\Video Materials\\TWEWY_Copy\\ (1).png";
            string outputFile = "D:\\Video Materials\\TWEWY_Copy\\ (1)_Ref.png";

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
                pushGradStrength = float.Parse(args[3]);
            }

            if (args.Length >= 5)
            {
                pushStrength = float.Parse(args[4]);
            }

            img = upscale(img, (int)(img.Width * scale), (int)(img.Height * scale));

            // Compute Luminance
            Bitmap luminanceMap = ImageProcess.ComputeLuminance(img);

            // Push
            Bitmap pushMap = ImageProcess.Unblur(img, luminanceMap, clamp((int)(pushStrength * 255), 0, 0xFFFF));

            // Compute Gradient
            Bitmap GradientMap = ImageProcess.ComputeGradient(pushMap);

            // Push Gradient
            Bitmap pushGradientMap = ImageProcess.PushGradient(img, clamp((int)(pushStrength * 255), 0, 0xFFFF));

            img.Save(outputFile, System.Drawing.Imaging.ImageFormat.Png);
        }

        static Bitmap copyType(Bitmap bm)
        {
            Bitmap newImage = new Bitmap(bm.Width, bm.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            Graphics g = Graphics.FromImage(newImage);
            g.DrawImage(bm, 0, 0);
            return newImage;
        }

        static Bitmap upscale(Bitmap bm, int width, int height)
        {
            Bitmap newImage = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
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
