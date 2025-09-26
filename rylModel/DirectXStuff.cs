using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rylModel
{
    public enum D3DOptions : uint
    {
        D3DFVF_XYZ = 0x002,
        D3DFVF_XYZRHW = 0x004,
        D3DFVF_NORMAL = 0x010,
        D3DFVF_DIFFUSE = 0x040,
        D3DFVF_SPECULAR = 0x080,
        D3DFVF_TEX1 = 0x100,
        D3DFVF_TEX2 = 0x200,
        D3DFVF_TEX3 = 0x300,
        D3DFVF_TEXTUREFORMAT3_2 = 1 << 20
    }

    public struct Vertex
    {
        public float X;
        public float Z;
        public float Y;
    }

    public struct MultiFVF
    {
        public const D3DOptions Options = D3DOptions.D3DFVF_XYZ | D3DOptions.D3DFVF_DIFFUSE | D3DOptions.D3DFVF_SPECULAR | D3DOptions.D3DFVF_TEX2;

        public float X;
        public float Z;
        public float Y;

        public uint diff; // -1
        public uint spec; // zero
        public float tu; // 4.5, 5.0
        public float tv; // 0.9801, 0.74
        public float tu1; // zero
        public float tv1; // zero
    }

    public struct BumpVertex
    {
        public const D3DOptions Options = D3DOptions.D3DFVF_XYZ | D3DOptions.D3DFVF_NORMAL | D3DOptions.D3DFVF_TEX3 | D3DOptions.D3DFVF_TEXTUREFORMAT3_2;

        public float X;
        public float Z;
        public float Y;

        public float nX;
        public float nZ;
        public float nY;

        public float tu;
        public float tv;
        public float tu1;
        public float tv1;

        public float uX;
        public float uZ;
        public float uY;
    }

    public struct U1Mesh
    {
        public MultiFVF[] Vertexes;
        public BumpVertex[] Vertexes2; // one of these
        public short[] Indices;
        public string Name;
        public string Texture;
    }
}
