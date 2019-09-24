using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

namespace Anime4KSharp
{
    public sealed class ImageProcess
    {
        public static void ComputeLuminance(ref Bitmap bm)
        {
            for(int x = 0; x < bm.Width - 1; x++)
            {
                for(int y = 0; y < bm.Height - 1; y++)
                {
                    var pixel = bm.GetPixel(x, y);
                    float lum = pixel.GetBrightness();
                    int castedLum = clamp(Convert.ToByte(lum * 255), 0, 0xFF);
                    bm.SetPixel(x, y, Color.FromArgb(castedLum, pixel.R, pixel.G, pixel.B));
                }
            }

            //bm.Save("D:\\Video Materials\\TWEWY_Copy\\Luminance.png", ImageFormat.Png);
        }

        public static void PushColor(ref Bitmap bm, int strength)
        {
            for (int x = 0; x < bm.Width-1; x++)
            {
                for (int y = 0; y < bm.Height-1; y++)
                {
                    //Default translation constants
                    int xn = -1;
                    int xp = 1;
                    int yn = -1;
                    int yp = 1;

                    //If x or y is on the border, don't move out of bounds
                    if (x == 0)
                    {
                        xn = 0;
                    }
                    else if (x == bm.Width - 1)
                    {
                        xp = 0;
                    }
                    if (y == 0)
                    {
                        yn = 0;
                    }
                    else if (y == bm.Height - 1)
                    {
                        yp = 0;
                    }

                    var kernel = new List<Point>();
                    //Top column
                    //Point tl = new Point(x + xn, y + yn);
                    //Point tc = new Point(x, y + yn);
                    //Point tr = new Point(x + xp, y + yn);
                    var tl = bm.GetPixel(x + xn, y + yn);
                    var tc = bm.GetPixel(x, y + yn);
                    var tr = bm.GetPixel(x + xp, y + yn);

                    //Middle column
                    //Point ml = new Point(x + xn, y);
                    //Point mc = new Point(x, y);
                    //Point mr = new Point(x + xp, y);
                    var ml = bm.GetPixel(x + xn, y);
                    var mc = bm.GetPixel(x, y);
                    var mr = bm.GetPixel(x + xp, y);

                    //Bottom column
                    //Point bl = new Point(x + xn, y + yp);
                    //Point bc = new Point(x, y + yp);
                    //Point br = new Point(x + xp, y + yp);
                    var bl = bm.GetPixel(x + xn, y + yp);
                    var bc = bm.GetPixel(x, y + yp);
                    var br = bm.GetPixel(x + xp, y + yp);

                    var lightestColor = bm.GetPixel(x, y);

                    //Kernel 0 and 4
                    float maxDark = max3(br, bc, bl);
                    float minLight = min3(tl, tc, tr);

                    if (minLight > mc.A && minLight > maxDark)
                    {
                        lightestColor = getLargest(mc, lightestColor, tl, tc, tr, strength);
                    }
                    else
                    {
                        maxDark = max3(tl, tc, tr);
                        minLight = min3(br, bc, bl);
                        if (minLight > mc.A && minLight > maxDark)
                        {
                            lightestColor = getLargest(mc, lightestColor, br, bc, bl, strength);
                        }
                    }

                    //Kernel 1 and 5
                    maxDark = max3(mc, ml, bc);
                    minLight = min3(mr, tc, tr);

                    if (minLight > maxDark)
                    {
                        lightestColor = getLargest(mc, lightestColor, mr, tc, tr, strength);
                    }
                    else
                    {
                        maxDark = max3(mc, mr, tc);
                        minLight = min3(bl, ml, bc);
                        if (minLight > maxDark)
                        {
                            lightestColor = getLargest(mc, lightestColor, bl, ml, bc, strength);
                        }
                    }

                    //Kernel 2 and 6
                    maxDark = max3(ml, tl, bl);
                    minLight = min3(mr, br, tr);

                    if (minLight > mc.A && minLight > maxDark)
                    {
                        lightestColor = getLargest(mc, lightestColor, mr, br, tr, strength);
                    }
                    else
                    {
                        maxDark = max3(mr, br, tr);
                        minLight = min3(ml, tl, bl);
                        if (minLight > mc.A && minLight > maxDark)
                        {
                            lightestColor = getLargest(mc, lightestColor, ml, tl, bl, strength);
                        }
                    }

                    //Kernel 3 and 7
                    maxDark = max3(mc, ml, tc);
                    minLight = min3(mr, br, bc);

                    if (minLight > maxDark)
                    {
                        lightestColor = getLargest(mc, lightestColor, mr, br, bc, strength);
                    }
                    else
                    {
                        maxDark = max3(mc, mr, bc);
                        minLight = min3(tc, ml, tl);
                        if (minLight > maxDark)
                        {
                            lightestColor = getLargest(mc, lightestColor, tc, ml, tl, strength);
                        }
                    }

                    bm.SetPixel(x, y, lightestColor);
                }
            }

            //bm.Save("D:\\Video Materials\\TWEWY_Copy\\Push.png", ImageFormat.Png);
        }

        public static void ComputeGradient(ref Bitmap bm)
        {
            int[][] sobelx = {new int[] {-1, 0, 1},
                              new int[] {-2, 0, 2},
                              new int[] {-1, 0, 1}};

            int[][] sobely = {new int[] {-1, -2, -1},
                              new int[] { 0, 0, 0},
                              new int[] { 1, 2, 1}};

            for (int i = 1; i < bm.Width - 1; i++)
            {
                for (int j = 1; j < bm.Height - 1; j++)
                {
                    int dx = bm.GetPixel(i - 1, j - 1).R * sobelx[0][0] + bm.GetPixel(i, j - 1).R * sobelx[0][1] + bm.GetPixel(i + 1, j - 1).R * sobelx[0][2]
                              + bm.GetPixel(i - 1, j).R * sobelx[1][0] + bm.GetPixel(i, j).R * sobelx[1][1] + bm.GetPixel(i + 1, j).R * sobelx[1][2]
                              + bm.GetPixel(i - 1, j + 1).R * sobelx[2][0] + bm.GetPixel(i, j + 1).R * sobelx[2][1] + bm.GetPixel(i + 1, j + 1).R * sobelx[2][2];

                    int dy = bm.GetPixel(i - 1, j - 1).R * sobely[0][0] + bm.GetPixel(i, j - 1).R * sobely[0][1] + bm.GetPixel(i + 1, j - 1).R * sobely[0][2]
                           + bm.GetPixel(i - 1, j).R * sobely[1][0] + bm.GetPixel(i, j).R * sobely[1][1] + bm.GetPixel(i + 1, j).R * sobely[1][2]
                           + bm.GetPixel(i - 1, j + 1).R * sobely[2][0] + bm.GetPixel(i, j + 1).R * sobely[2][1] + bm.GetPixel(i + 1, j + 1).R * sobely[2][2];
                    double derivata = Math.Sqrt((dx * dx) + (dy * dy));

                    var pixel = bm.GetPixel(i, j);
                    if (derivata > 255)
                    {
                        bm.SetPixel(i, j, Color.FromArgb(255, pixel.R,pixel.G,pixel.B));
                    }
                    else
                    {
                        bm.SetPixel(i, j, Color.FromArgb((int)derivata, pixel.R, pixel.G, pixel.B));
                    }
                }
            }
        }

        public static void PushGradient(ref Bitmap bm, int strength)
        {
            for (int x = 0; x < bm.Width - 1; x++)
            {
                for (int y = 0; y < bm.Height - 1; y++)
                {
                    //Default translation constants
                    int xn = -1;
                    int xp = 1;
                    int yn = -1;
                    int yp = 1;

                    //If x or y is on the border, don't move out of bounds
                    if (x == 0)
                    {
                        xn = 0;
                    }
                    else if (x == bm.Width - 1)
                    {
                        xp = 0;
                    }
                    if (y == 0)
                    {
                        yn = 0;
                    }
                    else if (y == bm.Height - 1)
                    {
                        yp = 0;
                    }

                    var kernel = new List<Point>();
                    //Top column
                    //Point tl = new Point(x + xn, y + yn);
                    //Point tc = new Point(x, y + yn);
                    //Point tr = new Point(x + xp, y + yn);
                    var tl = bm.GetPixel(x + xn, y + yn);
                    var tc = bm.GetPixel(x, y + yn);
                    var tr = bm.GetPixel(x + xp, y + yn);

                    //Middle column
                    //Point ml = new Point(x + xn, y);
                    //Point mc = new Point(x, y);
                    //Point mr = new Point(x + xp, y);
                    var ml = bm.GetPixel(x + xn, y);
                    var mc = bm.GetPixel(x, y);
                    var mr = bm.GetPixel(x + xp, y);

                    //Bottom column
                    //Point bl = new Point(x + xn, y + yp);
                    //Point bc = new Point(x, y + yp);
                    //Point br = new Point(x + xp, y + yp);
                    var bl = bm.GetPixel(x + xn, y + yp);
                    var bc = bm.GetPixel(x, y + yp);
                    var br = bm.GetPixel(x + xp, y + yp);

                    var lightestColor = bm.GetPixel(x, y);

                    //Kernel 0 and 4
                    float maxDark = max3(br, bc, bl);
                    float minLight = min3(tl, tc, tr);

                    if (minLight > mc.A && minLight > maxDark)
                    {
                        lightestColor = getAverage(mc, tl, tc, tr, strength);
                    }
                    else
                    {
                        maxDark = max3(tl, tc, tr);
                        minLight = min3(br, bc, bl);
                        if (minLight > mc.A && minLight > maxDark)
                        {
                            lightestColor = getAverage(mc, br, bc, bl, strength);
                        }
                    }

                    //Kernel 1 and 5
                    maxDark = max3(mc, ml, bc);
                    minLight = min3(mr, tc, tr);

                    if (minLight > maxDark)
                    {
                        lightestColor = getAverage(mc, mr, tc, tr, strength);
                    }
                    else
                    {
                        maxDark = max3(mc, mr, tc);
                        minLight = min3(bl, ml, bc);
                        if (minLight > maxDark)
                        {
                            lightestColor = getAverage(mc, bl, ml, bc, strength);
                        }
                    }

                    //Kernel 2 and 6
                    maxDark = max3(ml, tl, bl);
                    minLight = min3(mr, br, tr);

                    if (minLight > mc.A && minLight > maxDark)
                    {
                        lightestColor = getAverage(mc, mr, br, tr, strength);
                    }
                    else
                    {
                        maxDark = max3(mr, br, tr);
                        minLight = min3(ml, tl, bl);
                        if (minLight > mc.A && minLight > maxDark)
                        {
                            lightestColor = getAverage(mc, ml, tl, bl, strength);
                        }
                    }

                    //Kernel 3 and 7
                    maxDark = max3(mc, ml, tc);
                    minLight = min3(mr, br, bc);

                    if (minLight > maxDark)
                    {
                        lightestColor = getAverage(mc, mr, br, bc, strength);
                    }
                    else
                    {
                        maxDark = max3(mc, mr, bc);
                        minLight = min3(tc, ml, tl);
                        if (minLight > maxDark)
                        {
                            lightestColor = getAverage(mc, tc, ml, tl, strength);
                        }
                    }

                    bm.SetPixel(x, y, lightestColor);
                }
            }

            //bm.Save("D:\\Video Materials\\TWEWY_Copy\\PushGrad.png", ImageFormat.Png);

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
        
        private static int min3(Color a, Color b, Color c)
        {
            return Math.Min(Math.Min(a.A, b.A), c.A);
        }

        private static int max3(Color a, Color b, Color c)
        {
            return Math.Max(Math.Max(a.A, b.A), c.A);
        }

        private static Color getLargest(Color cc, Color lightestColor, Color a, Color b, Color c, int strength)
        {
            int ra = (cc.R * (0xFF - strength) + ((a.R + b.R + c.R) / 3) * strength) / 0xFF;
            int ga = (cc.G * (0xFF - strength) + ((a.G + b.G + c.G) / 3) * strength) / 0xFF;
            int ba = (cc.B * (0xFF - strength) + ((a.B + b.B + c.B) / 3) * strength) / 0xFF;
            int aa = (cc.A * (0xFF - strength) + ((a.A + b.A + c.A) / 3) * strength) / 0xFF;

            var newColor = Color.FromArgb(aa, ra, ga, ba);

            return newColor.A > lightestColor.A ? newColor : lightestColor;
        }

        private static Color getAverage(Color cc, Color a, Color b, Color c, int strength)
        {
            int ra = (cc.R * (0xFF - strength) + ((a.R + b.R + c.R) / 3) * strength) / 0xFF;
            int ga = (cc.G * (0xFF - strength) + ((a.G + b.G + c.G) / 3) * strength) / 0xFF;
            int ba = (cc.B * (0xFF - strength) + ((a.B + b.B + c.B) / 3) * strength) / 0xFF;
            int aa = (cc.A * (0xFF - strength) + ((a.A + b.A + c.A) / 3) * strength) / 0xFF;

            return Color.FromArgb(aa, ra, ga, ba);

        }
    }
}
