using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rMap.HeightmapConverters
{
    abstract class Base
    {
        public string Extension { get; protected set; }
        public int Iterations;
        public string Name { get; protected set; }
        public bool UsesLowAndStep { get; protected set; }

        public abstract byte[] Export(float[,] map);
        public abstract float[,] Import(byte[] data, int rowLen, float low, float step);

        public static float GetLow(float[,] map)
        {
            float low = map[0, 0];
            for (int y = 0; y <= map.GetUpperBound(1); y++)
            {
                for (int x = 0; x <= map.GetUpperBound(0); x++)
                {
                    if (map[x, y] < low)
                        low = map[x, y];
                }
            }

            return low;
        }

        public static float GetStep(float[,] map, float iterations)
        {
            float low = map[0, 0];
            float high = map[0, 0];

            for (int y = 0; y <= map.GetUpperBound(1); y++)
            {
                for (int x = 0; x <= map.GetUpperBound(0); x++)
                {
                    if (map[x, y] > high)
                        high = map[x, y];
                    if (map[x, y] < low)
                        low = map[x, y];
                }
            }

            return (high - low) / iterations;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
