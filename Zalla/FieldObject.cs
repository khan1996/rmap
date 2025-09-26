using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace rMap.Zalla
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
            h.WorldM = br.ReadMatrix();
            h.IsAlpha = br.ReadBoolean();
            h.IsLight = br.ReadBoolean();

            return h;
        }

        public void SaveTo(BinaryWriter bw)
        {
            bw.Write((uint)SceneID);
            bw.Write(Mesh, 256);
            bw.Write(WorldM);
            bw.Write(IsAlpha);
            bw.Write(IsLight);
        }

        public override Asset.DrawableModel GetModel(Microsoft.Xna.Framework.Game game)
        {
            Asset.DrawableModel model = new Asset.DrawableModel(game) { TexturesFolder = Path.Combine(rMap.Properties.Settings.Default.RYLFolder, @"texture\object") };

            Asset.FileTypes.NMesh mesh = new Asset.FileTypes.NMesh();
            mesh.Load(Path.Combine(rMap.Properties.Settings.Default.RYLFolder, @"objects\object", Mesh));

            model.Parts.AddRange(mesh.Model.Parts);
            return model;
        }
    }
}
