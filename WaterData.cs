using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Zalla3dScene
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
    }
}
