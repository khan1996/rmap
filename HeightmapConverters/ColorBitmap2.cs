using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace rMap.HeightmapConverters
{
    class ColorBitmap2 : Base
    {
        public ColorBitmap2()
        {
            Name = "Color bitmap2 (30)";
            Extension = "bmp";
            UsesLowAndStep = true;
            Iterations = 7 * 0xff;
            /* 
             * b -> g =>B0 -> B255 -> B255G255 ->G255 | 3*255
             * g -> r = G255 -> G255R255 -> R255 -> R255B255 -> R255B255G255 | 4*255
             */
        }

        public override byte[] Export(float[,] map)
        {
            float low = GetLow(map);
            float step = GetStep(map, Iterations);

            using (Bitmap bmp = new Bitmap(map.GetUpperBound(0) + 1, map.GetUpperBound(1) + 1))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    for (int y = 0; y <= map.GetUpperBound(1); y++)
                    {
                        for (int x = 0; x <= map.GetUpperBound(0); x++)
                        {
                            int c = (int)Math.Round((map[x, y] - low) / step);

                            int blue = 0;
                            int green = 0;
                            int red = 0;

                            if (c <= 255)
                            {
                                blue = c;
                            }
                            else if (c <= 2 * 255)
                            {
                                blue = 255;
                                green = c - 255;
                            }
                            else if (c <= 3 * 255)
                            {
                                green = 255;
                                blue = 3 * 255 - c;
                            }
                            else if (c <= 4 * 255)
                            {
                                green = 255;
                                red = c - 3 * 255;
                            }
                            else if (c <= 5 * 255)
                            {
                                red = 255;
                                green = 5 * 255 - c;
                            }
                            else if (c <= 6 * 255)
                            {
                                red = 255;
                                blue = c - 5 * 255;
                            }
                            else
                            {
                                red = 255;
                                blue = 255;
                                green = c - 6 * 255;
                            }

                            bmp.SetPixel(x, y, Color.FromArgb(red, green, blue));
                        }
                    }

                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

                    return ms.ToArray();
                }
            }
        }

        public override float[,] Import(byte[] data, int rowLen, float low, float step)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                Bitmap bmp = new Bitmap(ms);
                float[,] map = new float[bmp.Width, bmp.Height];

                for (int y = 0; y < bmp.Height; y++)
                {
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        int rgb = 0;
                        Color color = bmp.GetPixel(x, y);
                        byte r = color.R;
                        byte g = color.G;
                        byte b = color.B;

                        if (r == 0 && g == 0)
                            rgb = b;
                        else if (r == 0 && b == 255)
                            rgb = 255 + g;
                        else if (r == 0 && g == 255)
                            rgb = 3 * 255 - b;
                        else if (b == 0 && g == 255)
                            rgb = 3 * 255 + r;
                        else if (b == 0 && r == 255)
                            rgb = 5 * 255 - g;
                        else if (g == 0 && r == 255)
                            rgb = 5 * 255 + b;
                        else // r & b = 255
                            rgb = 6 * 255 + g;

                        // 0xAARRGGBB
                        map[x, y] = low + rgb * step;
                    }
                }
                return map;
            }
        }
    }
}
