using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Zalla3dScene
{
    [Serializable]
    public class NatureObject : SceneObject
    {
        public static Point TileSizes = new Point(31507, 31507);

        public int TileX;
        public int TileY;

        public byte Type;
        public byte PosX;
        public byte PosY;

        public override string ToString()
        {
            return TileX + ":" + TileY + " at " + PosX + ":" + PosY + " - " + Type;
        }

        [System.Xml.Serialization.XmlIgnore]
        public int MapPosX
        {
            get
            {
                return TileX * TileSizes.X + PosX * TileSizes.X / 65;
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        public int MapPosY
        {
            get
            {
                return TileY * TileSizes.Y + PosY * TileSizes.Y / 65;
            }
        }

        public static NatureObject LoadFrom(BinaryReader br, int tilex, int tiley)
        {
            NatureObject obj = new NatureObject();

            obj.TileX = tilex;
            obj.TileY = tiley;

            obj.Type = br.ReadByte();
            obj.PosX = br.ReadByte();
            obj.PosY = br.ReadByte();

            return obj;
        }

        public void SaveTo(BinaryWriter bw)
        {
            bw.Write((byte)Type);
            bw.Write((byte)PosX);
            bw.Write((byte)PosY);
        }
    }
}
