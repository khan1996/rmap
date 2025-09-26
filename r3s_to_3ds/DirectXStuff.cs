using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace rylModel
{
    public struct Vertex
    {
        public float X;
        public float Y;
        public float Z;

        public override string ToString()
        {
            return "{" + X + ", " + Y + ", " + Z + "}";
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(X);
            bw.Write(Y);
            bw.Write(Z);
        }

        public static Vertex Load(BinaryReader br)
        {
            return new Vertex()
            {
                X = br.ReadSingle(),
                Y = br.ReadSingle(),
                Z = br.ReadSingle()
            };
        }
    }

    public class Z3DBlend2Vertex
    {
        public Vertex pos;
        public float bfactor;
        public byte mtrx_id_1;
        public byte mtrx_id_2;
        public Vertex normal;
        public Vertex u;
        public float tu;
        public float tv;
        public byte lodLevel;


        public void Write(BinaryWriter bw)
        {
            pos.Write(bw);
            bw.Write(bfactor);
            bw.Write(mtrx_id_1);
            bw.Write(mtrx_id_2);
            bw.Write(new byte[2]);
            normal.Write(bw);
            u.Write(bw);
            bw.Write(tu);
            bw.Write(tv);
            bw.Write(lodLevel);
            bw.Write(new byte[3]);
        }

        public void Read(BinaryReader br)
        {
            pos = Vertex.Load(br);
            bfactor = br.ReadSingle();
            mtrx_id_1 = br.ReadByte();
            mtrx_id_2 = br.ReadByte();
            br.ReadBytes(2); // some odd atrifacts
            normal = Vertex.Load(br);
            u = Vertex.Load(br);
            tu = br.ReadSingle();
            tv = br.ReadSingle();
            lodLevel = br.ReadByte();
            br.ReadBytes(3);
        }

        public static Z3DBlend2Vertex Load(BinaryReader br)
        {
            Z3DBlend2Vertex fvf = new Z3DBlend2Vertex();
            fvf.Read(br);
            return fvf;
        }
    }

    public class Z3DLODMesh
    {
        public const int LOD_COUNT = 4;

        public ushort ID;
        public List<Z3DBlend2Vertex> Vertexes = new List<Z3DBlend2Vertex>();
        public List<ushort>[] Indices = new List<ushort>[]{
            new List<ushort>(),
            new List<ushort>(), // LOD_COUNT
            new List<ushort>(),
            new List<ushort>()
        };

        public void Write(BinaryWriter bw)
        {
            bw.Write(ID);
            bw.Write((uint)Vertexes.Count);

            for (int i = 0; i < LOD_COUNT; i++)
                bw.Write((uint)Indices[i].Count);

            foreach (Z3DBlend2Vertex fvf in Vertexes)
                fvf.Write(bw);
            for (int i = 0; i < LOD_COUNT; i++)
                foreach (ushort us in Indices[i])
                    bw.Write(us);
        }

        public void Read(BinaryReader br)
        {
            ID = br.ReadUInt16();
            uint vert_count = br.ReadUInt32();

            uint[] indis = new uint[LOD_COUNT];
            for (int i = 0; i < LOD_COUNT; i++)
                indis[i] = br.ReadUInt32();

            for (uint i = 0; i < vert_count; i++)
                Vertexes.Add(Z3DBlend2Vertex.Load(br));

            for (int i = 0; i < LOD_COUNT; i++)
                for (uint j = 0; j < indis[i]; j++)
                    Indices[i].Add(br.ReadUInt16());
        }

        public static Z3DLODMesh Load(BinaryReader br)
        {
            Z3DLODMesh m = new Z3DLODMesh();
            m.Read(br);
            return m;
        }
    }

    public class MultiFVF
    {
        public float X;
        public float Y;
        public float Z;

        public uint diff; // -1
        public uint spec; // zero
        public float tu; // 4.5, 5.0
        public float tv; // 0.9801, 0.74
        public float tu1; // zero
        public float tv1; // zero

        public override string ToString()
        {
            return "{" + X + ", " + Y + ", " + Z + "}";
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(X);
            bw.Write(Y);
            bw.Write(Z);

            bw.Write(diff);
            bw.Write(spec);

            bw.Write(tu);
            bw.Write(tv);

            bw.Write(tu1);
            bw.Write(tv1);
        }

        public void Read(BinaryReader br)
        {
            X = br.ReadSingle();
            Y = br.ReadSingle();
            Z = br.ReadSingle();

            diff = br.ReadUInt32();
            spec = br.ReadUInt32();

            tu = br.ReadSingle();
            tv = br.ReadSingle();
            tu1 = br.ReadSingle();
            tv1 = br.ReadSingle();
        }

        public static MultiFVF Load(BinaryReader br)
        {
            MultiFVF fvf = new MultiFVF();
            fvf.Read(br);
            return fvf;
        }
    }

    public class R3SContainer
    {
        public List<ObjMesh> Objects = new List<ObjMesh>();
        public List<string> Textures = new List<string>();
        public int Method = 0;

        public void Write(BinaryWriter bw)
        {
            bw.Write((uint)Objects.Count);
            bw.Write((uint)Textures.Count);
            bw.Write((uint)Method);

            foreach (string tex in Textures)
                WriteString(bw, tex, 256);

            foreach (ObjMesh m in Objects)
                m.Write(bw);
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
        private static string ReadString(BinaryReader br, int len)
        {
            byte[] cs = br.ReadBytes(len);
            string ret = "";
            for (int i = 0; i < len; i++)
            {
                if (cs[i] == 0)
                    return ret;
                else
                    ret += (char)cs[i];
            }
            return ret;
        }


        public void Read(BinaryReader br)
        {
            uint oc = br.ReadUInt32();
            uint tc = br.ReadUInt32();
            Method = (int)br.ReadUInt32();

            for (uint i = 0; i < tc; i++)
                Textures.Add(ReadString(br, 256));
            for (uint i = 0; i < oc; i++)
                Objects.Add(ObjMesh.Load(br));
        }

        public static R3SContainer Load(BinaryReader br)
        {
            R3SContainer cont = new R3SContainer();
            cont.Read(br);
            return cont;
        }
    }

    public class ObjMesh
    {
        public List<MultiFVF> Vertexes = new List<MultiFVF>();
        public List<ushort> Indices = new List<ushort>();
        public string Name = "Unknown";
        public int Texture;

        public void Write(BinaryWriter bw)
        {
            WriteString(bw, Name, 256);
            bw.Write((uint)Texture);
            bw.Write((uint)Vertexes.Count);
            bw.Write((uint)Indices.Count / 3);

            foreach (MultiFVF fvf in Vertexes)
                fvf.Write(bw);
            foreach (ushort us in Indices)
                bw.Write(us);
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

        private static string ReadString(BinaryReader br, int len)
        {
            byte[] cs = br.ReadBytes(len);
            string ret = "";
            for (int i = 0; i < len; i++)
            {
                if (cs[i] == 0)
                    return ret;
                else
                    ret += (char)cs[i];
            }
            return ret;
        }

        public void Read(BinaryReader br)
        {
            Name = ReadString(br, 256);
            Texture = (int)br.ReadUInt32();
            uint vc = br.ReadUInt32();
            uint ic = br.ReadUInt32() * 3;

            for (uint i = 0; i < vc; i++)
                Vertexes.Add(MultiFVF.Load(br));
            for (uint i = 0; i < ic; i++)
                Indices.Add(br.ReadUInt16());
        }

        public static ObjMesh Load(BinaryReader br)
        {
            ObjMesh m = new ObjMesh();
            m.Read(br);
            return m;
        }
    }
}
