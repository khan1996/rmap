using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace rMap.Zalla
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

            h.WorldM = br.ReadMatrix();
            h.SceneID = br.ReadInt32();

            for (int i = 0; i < 4; i++)
                h.Meshes[i] = br.ReadString(256);

            return h;
        }

        public void SaveTo(BinaryWriter bw)
        {
            bw.Write(WorldM);
            bw.Write(SceneID);

            foreach (string m in Meshes)
                bw.Write(m, 256);
        }

        public override Asset.DrawableModel GetModel(Microsoft.Xna.Framework.Game game)
        {
            Asset.DrawableModel model = new Asset.DrawableModel(game);
            model.TexturesFolder = Path.Combine(rMap.Properties.Settings.Default.RYLFolder, @"texture\object");

            foreach (string file in Meshes)
            {
                if (string.IsNullOrEmpty(file))
                    continue;

                Asset.FileTypes.NMesh mesh = new Asset.FileTypes.NMesh();
                mesh.Load(Path.Combine(rMap.Properties.Settings.Default.RYLFolder, @"objects\house", file));

                model.Parts.AddRange(mesh.Model.Parts);
            }
            return model;
        }
    }
}
