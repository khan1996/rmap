using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Zalla3dScene
{
    /// <summary>
    /// name+76b
    /// </summary>
    [Serializable]
    public class SavedLight : WorldMatrixObject
    {
        public string Mesh;
        public uint LightID;
        public float LightRange;
        public uint Color;

        public override string ToString()
        {
            return Mesh;
        }

        public static SavedLight LoadFrom(BinaryReader br)
        {
            SavedLight h = new SavedLight();

            h.Mesh = br.ReadString(256);
            h.LightID = br.ReadUInt32();
            h.WorldM = WMatrix.Load(br);
            h.LightRange = br.ReadSingle();
            h.Color = br.ReadUInt32();

            return h;
        }

        public void SaveTo(BinaryWriter bw)
        {
            bw.Write(Mesh, 256);
            bw.Write((uint)LightID);
            WorldM.Save(bw);
            bw.Write(LightRange);
            bw.Write(Color);
        }
    }
}