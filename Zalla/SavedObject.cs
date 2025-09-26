using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace rMap.Zalla
{
    /// <summary>
    /// name+70b
    /// </summary>
    [Serializable]
    public class SavedObject : WorldMatrixObject
    {
        public string Mesh;
        public uint ObjectID;
        public bool IsAlpha;
        public bool IsLight;

        public override string ToString()
        {
            return Mesh;
        }

        public static SavedObject LoadFrom(BinaryReader br)
        {
            SavedObject h = new SavedObject();

            h.Mesh = br.ReadString(256);
            h.ObjectID = br.ReadUInt32();
            h.WorldM = br.ReadMatrix();
            h.IsAlpha = br.ReadBoolean();
            h.IsLight = br.ReadBoolean();

            return h;
        }

        public void SaveTo(BinaryWriter bw)
        {
            bw.Write(Mesh, 256);
            bw.Write((uint)ObjectID);
            bw.Write(WorldM);
            bw.Write(IsAlpha);
            bw.Write(IsLight);
        }
    }
}
