using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Zalla3dScene
{
    [Serializable]
    public class FallmapObj : SceneObject
    {
        public float Height;
        public float Left;
        public float Right;
        public float AddX;
        public uint Color;

        public static FallmapObj LoadFrom(BinaryReader br)
        {
            FallmapObj obj = new FallmapObj();

            obj.Height = br.ReadSingle();
            obj.Left = br.ReadSingle();
            obj.Right = br.ReadSingle();
            obj.AddX = br.ReadSingle();
            obj.Color = br.ReadUInt32();

            return obj;
        }

        public void SaveTo(BinaryWriter bw)
        {
            bw.Write(Height);
            bw.Write(Left);
            bw.Write(Right);
            bw.Write(AddX);
            bw.Write(Color);
        }
    }
}
