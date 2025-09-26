using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace rMap.HeightmapConverters
{
    class ColorBitmap : Base
    {
        public ColorBitmap()
        {
            Name = "Color bitmap (0.003)";
            Extension = "bmp";
            UsesLowAndStep = true;
            Iterations = 0xffffff;
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

                            int blue = c & 0xff;
                            int green = (c >> 8) & 0xff;
                            int red = (c >> 16) & 0xff;

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
                        int rgb = bmp.GetPixel(x, y).ToArgb() & 0xffffff;
                        // 0xAARRGGBB
                        map[x, y] = low + rgb * step;
                    }
                }
                return map;
            }
        }
    }
}
