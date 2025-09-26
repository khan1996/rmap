using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace rylModel
{
    public class R3S
    {
        public enum Filter
        {
            Normal = 0,
            Bump = 1,

        }
        public U1Mesh[] Meshes;
        public Filter Method;


        public R3S() { }
        public R3S(byte[] data) { Load(data); }

        public void Load(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                BinaryReader br = new BinaryReader(ms);
                int meshcount = br.ReadInt32();
                int texcount = br.ReadInt32();
                Method = (Filter)br.ReadInt32();

                string[] Textures = new string[texcount];
                Meshes = new U1Mesh[meshcount];

                for (int i = 0; i < texcount; i++)
                    Textures[i] = br.ReadString(256);

                // 864 bytes = 216 floats, highest indice = 23, 216/24 = 9

                for (int mesh = 0; mesh < meshcount; mesh++)
                {
                    Meshes[mesh].Name = br.ReadString(256);

                    int textindex = br.ReadInt32();
                    int vcount = br.ReadInt32();
                    int icount = br.ReadInt32() * 3;

                    Meshes[mesh].Texture = Textures[textindex];

                    if (Method == Filter.Normal)
                        Meshes[mesh].Vertexes = new MultiFVF[vcount];
                    else
                        Meshes[mesh].Vertexes2 = new BumpVertex[vcount];

                    Meshes[mesh].Indices = new short[icount];

                    for (int vind = 0; vind < vcount; vind++)
                    {
                        if (Method == Filter.Normal)
                        {
                            MultiFVF vert = new MultiFVF();

                            vert.X = br.ReadSingle();
                            vert.Z = br.ReadSingle();
                            vert.Y = br.ReadSingle();

                            vert.diff = br.ReadUInt32();
                            vert.spec = br.ReadUInt32();
                            vert.tu = br.ReadSingle();
                            vert.tv = br.ReadSingle();
                            vert.tu1 = br.ReadSingle();
                            vert.tv1 = br.ReadSingle();

                            Meshes[mesh].Vertexes[vind] = vert;
                        }
                        else
                        {
                            BumpVertex vert = new BumpVertex();

                            vert.X = br.ReadSingle();
                            vert.Z = br.ReadSingle();
                            vert.Y = br.ReadSingle();

                            vert.nX = br.ReadSingle();
                            vert.nZ = br.ReadSingle();
                            vert.nY = br.ReadSingle();

                            vert.tu = br.ReadSingle();
                            vert.tv = br.ReadSingle();
                            vert.tu1 = br.ReadSingle();
                            vert.tv1 = br.ReadSingle();

                            vert.uX = br.ReadSingle();
                            vert.uZ = br.ReadSingle();
                            vert.uY = br.ReadSingle();

                            Meshes[mesh].Vertexes2[vind] = vert;
                        }
                    }

                    for (int iind = 0; iind < icount; iind++)
                    {
                        Meshes[mesh].Indices[iind] = br.ReadInt16();
                    }
                }
            }
        }
    }
}
