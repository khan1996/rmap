using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;

namespace rMap.Zalla
{
    [Serializable]
    public class SectorLight : TileObject
    {
        public Vector3 Color;
        public float Range;
        public int ShadowSamples;
        public int LightSamples;
        public float ShadowFactors;
        public int Ambient;
        public string Name;
        public float Expose;
        public Vector3 Position;

        public override Vector3 GetPosition()
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

            obj.Position = br.ReadVector3();
            obj.Color = br.ReadVector3();
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
            bw.Write(Position);
            bw.Write(Color);

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
