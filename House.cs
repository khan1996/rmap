using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Zalla3dScene
{
    [Serializable]
    public class House : WorldMatrixObject
    {
        public int SceneID;

        public string[] Meshes = new string[4];

        public override string ToString()
        {
            return string.Join(", ", Meshes.Where(s => !string.IsNullOrEmpty(s)).ToArray());
        }

        public static House LoadFrom(BinaryReader br)
        {
            House h = new House();

            h.WorldM = WMatrix.Load(br);
            h.SceneID = br.ReadInt32();

            for (int i = 0; i < 4; i++)
                h.Meshes[i] = br.ReadString(256);

            return h;
        }

        public void SaveTo(BinaryWriter bw)
        {
            WorldM.Save(bw);
            bw.Write(SceneID);

            foreach (string m in Meshes)
                bw.Write(m, 256);
        }
    }
}
