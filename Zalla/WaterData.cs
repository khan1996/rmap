using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using rMap.Asset;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = System.Drawing.Color;

namespace rMap.Zalla
{
    [Serializable]
    public class WaterData : SceneObject
    {
        public int TileX;
        public int TileY;

        public float PosX;
        public float PosY;

        public float SizeX;
        public float SizeY;

        public float Height;

        public bool Reflection;
        public uint Color;

        public override string ToString()
        {
            return TileX + ":" + TileY + " settings";
        }

        public static WaterData LoadFrom(BinaryReader br)
        {
            WaterData tile = new WaterData();

            tile.TileX = br.ReadInt32();
            tile.TileY = br.ReadInt32();

            tile.PosX = br.ReadSingle();
            tile.PosY = br.ReadSingle();

            tile.SizeX = br.ReadSingle();
            tile.SizeY = br.ReadSingle();

            tile.Height = br.ReadSingle();
            tile.Reflection = br.ReadBoolean();
            tile.Color = br.ReadUInt32();

            return tile;
        }

        public void SaveTo(BinaryWriter bw)
        {
            bw.Write((int)TileX);
            bw.Write((int)TileY);

            bw.Write((float)PosX);
            bw.Write((float)PosY);
            bw.Write((float)SizeX);
            bw.Write((float)SizeY);

            bw.Write((float)Height);
            bw.Write(Reflection);
            bw.Write((uint)Color);
        }

        public Vector3 GetPosition()
        {
            return new Vector3(TileX * 31507, Height, TileY * 31507);
        }

        public BoundingBox GetBox()
        {
            Vector3 pos = GetPosition();
            return new BoundingBox(new Vector3(pos.X, float.MinValue, pos.Z), new Vector3(pos.X + 31507, float.MaxValue, pos.Z + 31507));
        }

        public override Asset.DrawableModel GetModel(Microsoft.Xna.Framework.Game game)
        {
            DrawableModel model = new DrawableModel(game) { TexturesFolder = Path.Combine(rMap.Properties.Settings.Default.RYLFolder, @"Texture\Widetexture\Zone" + Scene.TextureZone) };
            ModelPart part = GenerateHeightmap(new float[,] { { 0, 0 }, { 0, 0 } });
            model.Parts.Add(part);

            return model;
        }

        private ModelPart GenerateHeightmap(float[,] heights)
        {
            ModelPart<VertexPositionColor> part = new ModelPart<VertexPositionColor>() { ColladaWriteNormals = false };
            part.World = Matrix.CreateScale(new Vector3(31507f, 1, 31507f));

            int height = heights.GetUpperBound(1) + 1;
            int width = heights.GetUpperBound(0) + 1;


            VertexPositionColor[] verts = new VertexPositionColor[width * height];
            int[] indices = new int[(width - 1) * (height - 1) * 6];

            for (int z = 0; z < height; z++)
                for (int x = 0; x < width; x++)
                {
                    Vector3 tempPos = new Vector3((float)x, heights[x, height - z - 1], (float)z);
                    Vector2 tempTexCo = new Vector2((float)(x) / (width - 1), (float)(z) / (height - 1));

                    VertexPositionColor temp = new VertexPositionColor(tempPos, Microsoft.Xna.Framework.Color.Blue);
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

            part.Vertexes = new List<VertexPositionColor>(verts);
            part.Indices = new List<int>(indices);

            return part;
        }
    }
}
