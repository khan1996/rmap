using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Drawing;

namespace Zalla3dScene
{
    [Serializable]
    public class HeightTable : SceneObject
    {
        public int X;
        public int Y;

        [XmlIgnore]
        public float[,] Table = new float[65, 65];

        [XmlElement("Table"), Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)] 
        public string TableDto
        {
            get
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryWriter bw = new BinaryWriter(ms);

                    for (int row = 0; row < 65; row++)
                        for (int col = 0; col < 65; col++)
                            bw.Write((float)Table[row, col]);

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
            set
            {
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(value)))
                {
                    BinaryReader br = new BinaryReader(ms);

                    for (int row = 0; row < 65; row++)
                        for (int col = 0; col < 65; col++)
                            Table[row, col] = br.ReadSingle();
                }
            }
        }

        public static HeightTable LoadFrom(BinaryReader br)
        {
            HeightTable table = new HeightTable();

            table.X = br.ReadInt32();
            table.Y = br.ReadInt32();

            for (int col = 64; col >= 0; col--)
                for (int row = 0; row < 65; row++)
                    table.Table[row, col] = br.ReadSingle();

            return table;
        }

        public void SaveTo(BinaryWriter bw)
        {
            bw.Write((int)X);
            bw.Write((int)Y);

            for (int col = 64; col >= 0; col--)
                for (int row = 0; row < 65; row++)
                    bw.Write((float)Table[row, col]);
        }

        public float GetLowest()
        {
            float l = Table[0,0];
            for (int x = 0; x < 65; x++)
            {
                for (int y = 0; y < 65; y++)
                {
                    if (Table[x, y] < l)
                        l = Table[x, y];
                }
            }

            return l;
        }

        public float GetHighest()
        {
            float l = Table[0, 0];
            for (int x = 0; x < 65; x++)
            {
                for (int y = 0; y < 65; y++)
                {
                    if (Table[x, y] > l)
                        l = Table[x, y];
                }
            }

            return l;
        }

        public Bitmap GetImage()
        {
            float lowest = GetLowest();
            float highest = GetHighest();
            float step = (highest - lowest) / 255f;

            Bitmap bmp = new Bitmap(65, 65);

            for (int x = 0; x < 65; x++)
            {
                for (int y = 0; y < 65; y++)
                {
                    float h = Table[x, y] - lowest;
                    byte color = (byte)Math.Round(h / step);

                    bmp.SetPixel(x, y, Color.FromArgb(color, color, color));
                }
            }

            return bmp;
        }

        public override string ToString()
        {
            return X + ":" + Y + " heights";
        }
    }
}
