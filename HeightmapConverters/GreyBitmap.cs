using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace rMap.HeightmapConverters
{
    class GreyBitmap : Base
    {
        public GreyBitmap()
        {
            Name = "Grey bitmap (209)";
            Extension = "bmp";
            UsesLowAndStep = true;
            Iterations = 255;
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
                            byte c = (byte)Math.Round((map[x, y] - low) / step);
                            bmp.SetPixel(x, y, Color.FromArgb(c, c, c));
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
                        map[x, y] = low + bmp.GetPixel(x, y).R * step;
                    }
                }
                return map;
            }
        }
    }
}
