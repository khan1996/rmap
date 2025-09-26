using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Drawing;
using rMap.Asset;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = System.Drawing.Color;

namespace rMap.Zalla
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

        public override Asset.DrawableModel GetModel(Microsoft.Xna.Framework.Game game)
        {
            DrawableModel model = new DrawableModel(game) { TexturesFolder = Path.Combine(rMap.Properties.Settings.Default.RYLFolder, @"Texture\Widetexture\Zone" + Scene.TextureZone) };
            ModelPart part = GenerateHeightmap(Table);
            model.Parts.Add(part);

            TileTexture tex = Scene.Textures.SingleOrDefault(x => x.X == X && x.Y == Y);

            if (tex != null)
                part.Texture = tex.Texture1;

            return model;
        }

        public Vector3 GetPosition()
        {
            return new Vector3(X * 31507, 0, Y * 31507);
        }

        public BoundingBox GetBox()
        {
            Vector3 pos = GetPosition();
            return new BoundingBox(new Vector3(pos.X, float.MinValue, pos.Z), new Vector3(pos.X + 31507, float.MaxValue, pos.Z + 31507)); 
        }

        private ModelPart GenerateHeightmap(float[,] heights)
        {
            ModelPart part = new ModelPart() { ColladaWriteNormals = false };
            part.World = Matrix.CreateScale(new Vector3(31507f / 64f, 1, 31507f / 64f));

            int height = heights.GetUpperBound(1) + 1;
            int width = heights.GetUpperBound(0) + 1;

            VertexPositionNormalTexture[] verts = new VertexPositionNormalTexture[width * height];
            int[] indices = new int[(width - 1) * (height - 1) * 6];

            for (int z = 0; z < height; z++)
                for (int x = 0; x < width; x++)
                {
                    Vector3 tempPos = new Vector3((float)x, heights[x, height - z - 1], (float)z);
                    Vector2 tempTexCo = new Vector2((float)(x) / (width - 1), (float)(z) / (height - 1));

                    VertexPositionNormalTexture temp = new VertexPositionNormalTexture(tempPos, Vector3.Zero, tempTexCo);
                    verts[x + z * width] = temp;
                }

            for (int x = 0; x < width - 1; x++)
                for (int y = 0; y < height - 1; y++)
                {
                    indices[(x + y * (width - 1)) * 6] = (x + 1) + (y + 1) * width;
                    indices[(x + y * (width - 1)) * 6 + 1] = (x + 1) + y * width;
                    indices[(x + y * (width - 1)) * 6 + 2] = x + y * width;

                    indices[(x + y * (width - 1)) * 6 + 3] = (x + 1) + (y + 1) * width;
                    indices[(x + y * (width - 1)) * 6 + 4] = x + y * width;
                    indices[(x + y * (width - 1)) * 6 + 5] = x + (y + 1) * width;
                }
            /*
            for (int i = 0; i < indices.Length / 3; i++)
            {
                int index1 = indices[i * 3];
                int index2 = indices[i * 3 + 1];
                int index3 = indices[i * 3 + 2];

                Vector3 side1 = verts[index2].Position - verts[index1].Position;
                Vector3 side2 = verts[index1].Position - verts[index3].Position;
                Vector3 normal = Vector3.Cross(side1, side2);
                normal.Normalize();

                verts[index1].Normal += normal;
                verts[index2].Normal += normal;
                verts[index3].Normal += normal;
            }

            for (int i = 0; i < verts.Length; i++)
                verts[i].Normal.Normalize();*/

            part.Vertexes = new List<VertexPositionNormalTexture>(verts);
            part.Indices = new List<int>(indices);

            return part;
        }
    }
}
