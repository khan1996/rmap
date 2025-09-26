using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace rMap.Asset.FileTypes
{
    class LODMesh
    {
        public DrawableModel Model { get; set; }

        public static short GetMeshType(string path)
        {
            if (File.Exists(path))
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryReader br = new BinaryReader(fs);
                    return br.ReadInt16();
                }

            return 0;
        }

        public void Load(string filename)
        {
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(filename)))
            {
                Model = new DrawableModel();
                BinaryReader br = new BinaryReader(ms);

                Model.PartId = br.ReadInt16();

                int nVertexes = br.ReadInt32();
                int nIndices = br.ReadInt32(); // lod 0
                br.ReadInt32(); // lod 1
                br.ReadInt32(); // lod 2
                br.ReadInt32(); // lod 3

                ModelPart part = new ModelPart();

                for (int j = 0; j < nVertexes; j++)
                {
                    Vector3 pos = br.ReadVector3();
                    float blendfactor = br.ReadSingle();
                    byte matrix_id_1 = br.ReadByte();
                    byte matrix_id_2 = br.ReadByte();
                    br.ReadBytes(2); // memory padding 2 bytes
                    Vector3 normal = br.ReadVector3();
                    Vector3 tex1 = br.ReadVector3();
                    Vector2 tex2 = br.ReadVector2();
                    byte lod_level = br.ReadByte();
                    br.ReadBytes(3); // memory padding 3 bytes

                    VertexExtraInfo e = new VertexExtraInfo() { Bone1 = matrix_id_1, Bone2 = matrix_id_2, Bone1Weight = blendfactor };
                    part.Vertexes.Add(new VertexPositionNormalTexture(pos, normal, tex2));
                    part.VertexExtraInfo.Add(e);
                }

                for (int j = 0; j < nIndices; j++)
                    part.Indices.Add(br.ReadUInt16());

                // skip the rest indices

                Model.Parts.Add(part);
            }
        }
    }
}
