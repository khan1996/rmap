using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace r3s_to_3ds.Converters
{
    /// <summary>
    /// http://www.spacesimulator.net/wiki/index.php/Tutorials:3ds_Loader#THE_3DS_FILE_STRUCTURE
    /// 
    /// Problems: faces to indices doesn't consider face normals.
    /// Texture names are max 8.3 (dos) in length, after realizing that i'm putting this thing on hold.
    /// </summary>
    class _3dsTor3s : IConverter
    {
        BinaryReader br;
        BinaryWriter bw;

        List<rylModel.ObjMesh> meshes;
        rylModel.ObjMesh ongoingMesh;
        List<rylModel.MultiFVF> verts;
        List<ushort> indices;

        #region IConverter Members

        public void SetInputFile(string file) { }

        public IDictionary<string, string> SupportedFormats()
        {
            return new Dictionary<string, string>()
            {
                {"3ds", "r3s"}
            };
        }

        public void Convert(Stream fileIn, Stream fileOut, string type)
        {
            br = new BinaryReader(fileIn);
            bw = new BinaryWriter(fileOut);

            Console.WriteLine("File start");
            meshes = new List<rylModel.ObjMesh>();
            ReadTree(br.BaseStream.Length);
            Write();

            br = null;
            bw = null;
        }

        #endregion

        private void ReadTree(long len)
        {
            long cursor = br.BaseStream.Position;

            while (br.BaseStream.Position < cursor + len)
            {
                long contStart = br.BaseStream.Position;
                ushort sector = br.ReadUInt16();
                uint seclen = br.ReadUInt32();

                SectorStarts(sector);
                ReadContent(sector, seclen - (br.BaseStream.Position - contStart));
                ReadTree(seclen - (br.BaseStream.Position - contStart));
                SectorEnds(sector);
            }
        }

        private void ReadContent(ushort sector, long inLen)
        {
            switch (sector)
            {
                case 0x4d4d: // main chunk
                    break;
                case 0x3d3d: // 3d editor block
                    break;
                case 0x4000: // object block
                    ongoingMesh.Name = ReadString(br); // object name
                    Console.WriteLine("\tObject start: " + ongoingMesh.Name);
                    break;
                case 0x4100: // triangles
                    Console.WriteLine("\t\tTriangle start");
                    break;
                case 0x4110: // vertices
                    {
                        ushort count = br.ReadUInt16();
                        Console.WriteLine("\t\tVert count: " + count);
                        for (ushort i = 0; i < count; i++)
                        {
                            rylModel.MultiFVF v = new rylModel.MultiFVF()
                            {
                                X = br.ReadSingle(),
                                Y = br.ReadSingle(),
                                Z = br.ReadSingle(),
                                diff = 0xffffffff,
                                spec = 0,
                                tu = 0,
                                tv = 0,
                                tu1 = 0,
                                tv1 = 0
                            };

                            verts.Add(v);
                            //Console.WriteLine("\t\t\tVert: " + v);
                        }

                    }
                    break;
                case 0x4120: // faces
                    {
                        ushort count = br.ReadUInt16();
                        Console.WriteLine("\t\tPoly count: " + count);
                        for (ushort i = 0; i < count; i++)
                        {
                            ushort p1 = br.ReadUInt16();
                            ushort p2 = br.ReadUInt16();
                            ushort p3 = br.ReadUInt16();

                            indices.Add(p1);
                            indices.Add(p2);
                            indices.Add(p3);
                        }
                    }
                    break;
                case 0x4140:
                    {
                        ushort count = br.ReadUInt16();
                        Console.WriteLine("\t\ttex cord count: " + count);
                        for (ushort i = 0; i < count; i++)
                        {
                            rylModel.MultiFVF v = verts[i];

                            v.tu = br.ReadSingle();
                            v.tv = br.ReadSingle();

                            verts[i] = v;
                        }
                    }
                    break;
                //case 0xafff:
                //    break;
                default:
                    br.BaseStream.Seek(inLen, SeekOrigin.Current);
                    break;
            }
        }

        private void SectorStarts(ushort sector)
        {
            if (sector == 0x4000)
            {
                verts = new List<rylModel.MultiFVF>();
                ongoingMesh = new rylModel.ObjMesh();
                indices = new List<ushort>();
            }
        }

        private void SectorEnds(ushort sector)
        {
            if (sector == 0x4000)
            {
                ongoingMesh.Vertexes = verts.ToArray();
                ongoingMesh.Indices = indices.ToArray();
                meshes.Add(ongoingMesh);
            }
        }

        private static string ReadString(BinaryReader br)
        {
            string ret = "";

            while (true)
            {
                char c = br.ReadChar();

                if ((byte)c == 0)
                    return ret;
                else
                    ret += c;
            }
        }

        private static void WriteString(BinaryWriter bw, string write, int len)
        {
            for (int i = 0; i < len; i++)
            {
                if (string.IsNullOrEmpty(write) || i >= write.Length)
                    bw.Write((char)0);
                else
                    bw.Write(write[i]);
            }
        }

        private void Write()
        {
            bw.Write(meshes.Count); // meshes
            bw.Write(meshes.Count); // textures
            bw.Write(0); // method, 0 = multi tex

            // textures
            foreach (rylModel.ObjMesh mesh in meshes)
            {
                WriteString(bw, "mameta1l_hc_6000.bmp", 256);
                //WriteString(bw, mesh.Texture, 256);
            }

            int i = 0;
            foreach (rylModel.ObjMesh mesh in meshes)
            {
                WriteString(bw, mesh.Name, 256);
                bw.Write(i++); // texture index
                bw.Write((uint)mesh.Vertexes.Length);
                bw.Write((uint)(mesh.Indices.Length / 3));

                for (int vind = 0; vind < mesh.Vertexes.Length; vind++)
                {
                    rylModel.MultiFVF v = mesh.Vertexes[vind];

                    bw.Write((float)v.X);
                    bw.Write((float)v.Y);
                    bw.Write((float)v.Z);

                    bw.Write((uint)v.diff);
                    bw.Write((uint)v.spec);
                    bw.Write((float)v.tu);
                    bw.Write((float)v.tv);
                    bw.Write((float)v.tu1);
                    bw.Write((float)v.tv1);
                }

                for (int iind = 0; iind < mesh.Indices.Length; iind++)
                {
                    bw.Write((ushort)mesh.Indices[iind]);
                }
            }
        }
    }
}
