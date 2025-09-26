using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace rMap.Asset.FileTypes
{
    class NMesh
    {
        public DrawableModel Model { get; set; }
        public bool IsCollision { get; set; }
        public bool IsLight { get; set; }

        public void Load(string filename)
        {
            IsCollision = filename.Contains("collision");

            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(filename)))
            {
                Model = new DrawableModel();
                BinaryReader br = new BinaryReader(ms);

                int nObject = br.ReadInt32();
                int nMat = br.ReadInt32();
                int ObjectMethod = br.ReadInt32();
                IsLight = ObjectMethod != 0;

                List<string> textures = new List<string>(nMat);
                for (int i = 0; i < nMat; i++)
                {
                    string tex = br.ReadString(256);

                    if (!string.IsNullOrEmpty(tex))
                        textures.Add(tex.Substring(0, tex.Length - 3) + "dds");
                }


                for (int i = 0; i < nObject; i++)
                {
                    string objName = br.ReadString(256);
                    int matRef = br.ReadInt32();
                    int nVertexes = br.ReadInt32();
                    int nPolygons = br.ReadInt32();

                    ModelPart part = new ModelPart();
                    part.Texture = textures[matRef];

                    for (int j = 0; j < nVertexes; j++)
                    {
                        if (ObjectMethod == 0)
                        {
                            Vector3 pos = br.ReadVector3();
                            Color diff = br.ReadColor();
                            Color spec = br.ReadColor();
                            Vector2 tex1 = br.ReadVector2();
                            Vector2 tex2 = br.ReadVector2();

                            part.Vertexes.Add(new VertexPositionNormalTexture(pos, Vector3.Zero, tex1));
                        }
                        else
                        {
                            Vector3 pos = br.ReadVector3();
                            Vector3 normal = br.ReadVector3();
                            Color spec = br.ReadColor();
                            Vector2 tex1 = br.ReadVector2();

                            part.Vertexes.Add(new VertexPositionNormalTexture(pos, normal, tex1));
                        }
                    }

                    for (int j = 0; j < nPolygons * 3; j++)
                        part.Indices.Add(br.ReadUInt16());

                    Model.Parts.Add(part);
                }
            }
        }
    }
}
