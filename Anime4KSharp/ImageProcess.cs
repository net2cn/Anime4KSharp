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
        public static Bitmap ComputeLuminance(Bitmap bm)
        {
            Bitmap luminanceMap = new Bitmap(bm.Width, bm.Height);
            for(int x = 0; x < bm.Width - 1; x++)
            {
                for(int y = 0; y < bm.Height - 1; y++)
                {
                    var pixel = bm.GetPixel(x, y);
                    float lum = pixel.GetBrightness();
                    int castedLum = clamp(Convert.ToByte(lum * 255), 0, 0xFF);
                    luminanceMap.SetPixel(x, y, Color.FromArgb(castedLum,castedLum,castedLum));
                }
            }

            luminanceMap.Save("D:\\Video Materials\\TWEWY_Copy\\Luminance.png", ImageFormat.Png);
            return luminanceMap;
        }

        public static Bitmap Unblur(Bitmap bm, Bitmap lm, int strength)
        {
            strength = clamp(strength, 0, 0xFF);

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

                    //Get coordinates for adjacent pixels.
                    //WARNING! SHITTY HARD-CODED TRANSFROM AHEAD!
                    //Top pixel.
                    int tx = x + xn;
                    int ty = y;

                    //Top-left pixel
                    int tlx = tx;
                    int tly = ty + yn;

                    //Top-right pixel ("_try" to avoid reserved word "try")
                    int trx = tx;
                    int _try = ty + yp;

                    //Left pixel
                    int lx = x;
                    int ly = y + yn;

                    //Right pixel
                    int rx = x;
                    int ry = y + yp;

                    //Bottom pixel
                    int bx = x + xp;
                    int by = y;

                    //Bottom-left pixel
                    int blx = bx;
                    int bly = by + yn;

                    //Bottom-right pixel
                    int brx = bx;
                    int bry = by + yp;

                    //Temp pixel
                    int d0x = x;
                    int d0y = y;

                    int d1x = x;
                    int d1y = y;

                    int d2x = x;
                    int d2y = y;

                    int l0x = x;
                    int l0y = y;

                    int l1x = x;
                    int l1y = y;

                    int l2x = x;
                    int l2y = y;

                    int l3x = x;
                    int l3y = y;

                    bool l4 = false;

                    var lightestColor = bm.GetPixel(x, y);
                    var lightestLum = lm.GetPixel(x, y);

                    for (int kernelType = 0; kernelType < 8; kernelType++)
                    {
                        // Careful here! Bug may occur any where!
                        if (kernelType == 0)
                        {
                            d0x = tlx;
                            d0y = tly;

                            d1x = tx;
                            d1y = ty;

                            d2x = trx;
                            d2y = _try;

                            l0x = x;
                            l0y = y;


                            l1x = blx;
                            l1y = bly;

                            l2x = bx;
                            l2y = by;

                            l3x = brx;
                            l3y = bry;

                            l4 = true;

                        }
                        else if (kernelType == 1)
                        {
                            d0x = tx;
                            d0y = ty;

                            d1x = trx;
                            d1y = _try;

                            d2x = rx;
                            d2y = ry;


                            l0x = x;
                            l0y = y;

                            l1x = lx;
                            l1y = ly;

                            l2x = bx;
                            l2y = by;

                            l4 = false;
                        }
                        else if (kernelType == 2)
                        {
                            d0x = trx;
                            d0y = _try;

                            d1x = rx;
                            d1y = ry;

                            d2x = brx;
                            d2y = bry;


                            l0x = x;
                            l0y = y;

                            l1x = tlx;
                            l1y = tly;

                            l2x = lx;
                            l2y = ly;

                            l3x = blx;
                            l3y = bly;

                            l4 = true;
                        }
                        else if (kernelType == 3)
                        {
                            d0x = rx;
                            d0y = ry;

                            d1x = brx;
                            d1y = bry;

                            d2x = bx;
                            d2y = by;


                            l0x = x;
                            l0y = y;

                            l1x = tx;
                            l1y = ty;

                            l2x = lx;
                            l2y = ly;

                            l4 = false;
                        }
                        else if (kernelType == 4)
                        {
                            d0x = blx;
                            d0y = bly;

                            d1x = bx;
                            d1y = by;

                            d2x = brx;
                            d2y = bry;


                            l0x = x;
                            l0y = y;

                            l1x = tlx;
                            l1y = tly;

                            l2x = tx;
                            l2y = ty;

                            l3x = trx;
                            l3y = _try;

                            l4 = true;
                        }
                        else if (kernelType == 5)
                        {
                            d0x = lx;
                            d0y = ly;

                            d1x = blx;
                            d1y = bly;

                            d2x = bx;
                            d2y = by;


                            l0x = x;
                            l0y = y;

                            l1x = tx;
                            l1y = ty;

                            l2x = rx;
                            l2y = ry;

                            l4 = false;
                        }
                        else if (kernelType == 6)
                        {
                            d0x = tlx;
                            d0y = tly;

                            d1x = lx;
                            d1y = ly;

                            d2x = blx;
                            d2y = bly;


                            l0x = x;
                            l0y = y;

                            l1x = trx;
                            l1y = _try;

                            l2x = rx;
                            l2y = ry;

                            l3x = brx;
                            l3y = bry;

                            l4 = true;
                        }
                        else if (kernelType == 7)
                        {
                            d0x = tlx;
                            d0y = tly;

                            d1x = tx;
                            d1y = ty;

                            d2x = lx;
                            d2y = ly;


                            l0x = x;
                            l0y = y;

                            l1x = bx;
                            l1y = by;

                            l2x = rx;
                            l2y = ry;

                            l4 = false;
                        }
                        else
                        {
                            throw new OverflowException();
                        }

                        if (l4)
                        {
                            int d0 = lm.GetPixel(d0x, d0y).R;
                            int d1 = lm.GetPixel(d1x, d1y).R;
                            int d2 = lm.GetPixel(d2x, d2y).R;

                            int l0 = lm.GetPixel(l0x, l0y).R;
                            int l1 = lm.GetPixel(l1x, l1y).R;
                            int l2 = lm.GetPixel(l2x, l2y).R;
                            int l3 = lm.GetPixel(l3x, l3y).R;

                            if (!compareLuminance4(d0, d1, d2, l0, l1, l2, l3))
                            {
                                var c0 = bm.GetPixel(d0x,d0y);
                                var c1 = bm.GetPixel(d1x, d1y);
                                var c2 = bm.GetPixel(d2x, d2y);

                                var newColor = weightedAverageRGB(bm.GetPixel(x,y), averageRGB(c0, c1, c2), strength);
                                var newLum = getLuminance(getRed(newColor), getGreen(newColor), getBlue(newColor));

                                if (newLum > lightestLum)
                                {
                                    lightestLum = newLum;
                                    lightestColor = newColor;
                                }

                            }
                        }
                    }
                }
            }

            return bm;
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

        private static bool compareLuminance4(int dark0, int dark1, int dark2, int light0, int light1, int light2, int light3)
        {
            return (dark0 < light0 && dark0 < light1 && dark0 < light2 && dark0 < light3 && dark1 < light0 && dark1 < light1 && dark1 < light2 && dark1 < light3 && dark2 < light0 && dark2 < light1 && dark2 < light2 && dark2 < light3);
        }
    }
}
