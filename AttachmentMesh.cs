using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace rMap.Asset.FileTypes
{
    class AttachmentMesh
    {
        public DrawableModel Model { get; set; }

        public void Load(string filename)
        {
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(filename)))
            {
                Model = new DrawableModel();
                BinaryReader br = new BinaryReader(ms);

                int nVertexes = br.ReadInt16();
                int nIndices = br.ReadInt16();

                ModelPart part = new ModelPart();

                for (int j = 0; j < nVertexes; j++)
                {
                    Vector3 pos = br.ReadVector3();
                    Vector3 normal = br.ReadVector3();
                    Vector2 tex1 = br.ReadVector2();
                    Vector3 box_tex1 = br.ReadVector3();

                    part.Vertexes.Add(new VertexPositionNormalTexture(pos, normal, tex1));
                }

                for (int j = 0; j < nIndices; j++)
                    part.Indices.Add(br.ReadUInt16());

                Model.Marker1 = br.ReadInt16();
                Model.Marker2 = br.ReadInt16();

                Model.Parts.Add(part);
            }
        }
    }
}
