using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Zalla3dScene
{
    [Serializable]
    public class FieldObject : WorldMatrixObject
    {
        public uint SceneID;

        public string Mesh;
        public bool IsAlpha;
        public bool IsLight;

        public override string ToString()
        {
            return Mesh;
        }


        public static FieldObject LoadFrom(BinaryReader br)
        {
            FieldObject h = new FieldObject();

            h.SceneID = br.ReadUInt32();
            h.Mesh = br.ReadString(256);
            h.WorldM = WMatrix.Load(br);
            h.IsAlpha = br.ReadBoolean();
            h.IsLight = br.ReadBoolean();

            return h;
        }

        public void SaveTo(BinaryWriter bw)
        {
            bw.Write((uint)SceneID);
            bw.Write(Mesh, 256);
            WorldM.Save(bw);
            bw.Write(IsAlpha);
            bw.Write(IsLight);
        }
    }
}
