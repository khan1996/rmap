using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace rMap.HeightmapConverters
{
    class FloatArray : Base
    {
        public FloatArray()
        {
            Name = "Float array";
            Extension = "bin";
        }

        public override byte[] Export(float[,] map)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);

                for (int y = 0; y <= map.GetUpperBound(1); y++)
                {
                    for (int x = 0; x <= map.GetUpperBound(0); x++)
                    {
                        bw.Write(map[x, y]);
                    }
                }

                return ms.ToArray();
            }
        }

        public override float[,] Import(byte[] data, int rowLen, float low, float step)
        {
            if (data.Length % 4 > 0)
                throw new Exception("Data length not dividable by 4");

            int floats = data.Length / 4;

            int rowHeight = floats / rowLen;

            float[,] map = new float[rowLen, rowHeight];

            using (MemoryStream ms = new MemoryStream(data))
            {
                BinaryReader bw = new BinaryReader(ms);

                for (int y = 0; y < rowHeight; y++)
                {
                    for (int x = 0; x < rowLen; x++)
                    {
                        map[x, y] = bw.ReadSingle();
                    }
                }
            }

            return map;
        }
    }
}
