using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace Zalla3dScene
{
    [Serializable]
    public class Effect : TileObject
    {
        public VectorF Position;
        public VectorF Rotation;
        public string Name;

        public override VectorF GetPosition()
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

            obj.Position = new VectorF() { X = br.ReadSingle(), Z = br.ReadSingle(), Y = br.ReadSingle() };
            obj.Rotation = new VectorF() { X = br.ReadSingle(), Z = br.ReadSingle(), Y = br.ReadSingle() };
            obj.Name = br.ReadString(256);

            return obj;
        }

        public void SaveTo(BinaryWriter bw)
        {
            bw.Write(Position.X);
            bw.Write(Position.Z);
            bw.Write(Position.Y);

            bw.Write(Rotation.X);
            bw.Write(Rotation.Z);
            bw.Write(Rotation.Y);

            bw.Write(Name, 256);
        }
    }
}
