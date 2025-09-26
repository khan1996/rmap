using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace rMap.Zalla
{
    [Serializable]
    public class TileTexture : SceneObject
    {
        public int X;
        public int Y;

        public string Texture1;
        public string DetailBlend1;

        public string Texture2;
        public string DetailBlend2;

        public string Texture3;
        public string DetailBlend3;

        public override string ToString()
        {
            return X + ":" + Y + " " + Texture1;
        }

        public static TileTexture LoadFrom(BinaryReader br)
        {
            TileTexture tile = new TileTexture();

            tile.X = br.ReadInt32();
            tile.Y = br.ReadInt32();

            tile.Texture1 = br.ReadString(256);
            tile.DetailBlend1 = br.ReadString(256);

            tile.Texture2 = br.ReadString(256);
            tile.DetailBlend2 = br.ReadString(256);

            tile.Texture3 = br.ReadString(256);
            tile.DetailBlend3 = br.ReadString(256);

            if (tile.X < 0 || tile.X > 256 || tile.Y < 0 || tile.Y > 256 || string.IsNullOrEmpty(tile.Texture1))
                tile = null;

            return tile;
        }

        public void SaveTo(BinaryWriter bw)
        {
            bw.Write((int)X);
            bw.Write((int)Y);

            bw.Write(Texture1, 256);
            bw.Write(DetailBlend1, 256);

            bw.Write(Texture2, 256);
            bw.Write(DetailBlend2, 256);

            bw.Write(Texture3, 256);
            bw.Write(DetailBlend3, 256);
        }
    }
}
