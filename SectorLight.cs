using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Zalla3dScene
{
    [Serializable]
    public class SectorLight : TileObject
    {
        public VectorF Color;
        public float Range;
        public int ShadowSamples;
        public int LightSamples;
        public float ShadowFactors;
        public int Ambient;
        public string Name;
        public float Expose;
        public VectorF Position;

        public override VectorF GetPosition()
        {
            return Position;
        }

        public override string ToString()
        {
            return Name;
        }

        public static SectorLight LoadFrom(BinaryReader br)
        {
            SectorLight obj = new SectorLight();

            obj.Position = new VectorF() { X = br.ReadSingle(), Z = br.ReadSingle(), Y = br.ReadSingle() };
            obj.Color = new VectorF() { X = br.ReadSingle(), Z = br.ReadSingle(), Y = br.ReadSingle() };
            obj.Range = br.ReadSingle();
            obj.ShadowSamples = br.ReadInt32();
            obj.LightSamples = br.ReadInt32();
            obj.ShadowFactors = br.ReadSingle();
            obj.Ambient = br.ReadInt32();
            obj.Name = br.ReadString(256);
            obj.Expose = br.ReadSingle();

            return obj;
        }

        public void SaveTo(BinaryWriter bw)
        {
            bw.Write(Position.X);
            bw.Write(Position.Z);
            bw.Write(Position.Y);

            bw.Write(Color.X);
            bw.Write(Color.Z);
            bw.Write(Color.Y);

            bw.Write(Range);
            bw.Write(ShadowSamples);
            bw.Write(LightSamples);
            bw.Write(ShadowFactors);
            bw.Write(Ambient);

            bw.Write(Name, 256);

            bw.Write(Expose);
        }
    }
}
