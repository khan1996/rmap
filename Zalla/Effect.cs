using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;

namespace rMap.Zalla
{
    [Serializable]
    public class Effect : TileObject
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public string Name;

        public override Vector3 GetPosition()
        {
            return Position;
        }

        public override string ToString()
        {
            return Name;
        }

        public static Effect LoadFrom(BinaryReader br)
        {
            Effect obj = new Effect();

            obj.Position = br.ReadVector3();
            obj.Rotation = br.ReadVector3();
            obj.Name = br.ReadString(256);

            return obj;
        }

        public void SaveTo(BinaryWriter bw)
        {
            bw.Write(Position);
            bw.Write(Rotation);

            bw.Write(Name, 256);
        }
    }
}
