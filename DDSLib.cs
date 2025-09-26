//for compatibility with the The Nvidia Photoshop DDS Plugin as it can't read correctly ABGR textures.
//coment this if you want to save color textures as ABGR.
#define COLOR_SAVE_TO_ARGB

#region File Description + License
//-----------------------------------------------------------------------------
// DDSFromFile.cs
// Read/Write dds files from/to files or from streams.
// Version 1.86
// Popescu Alexandru Cristian(kiki_karon@yahoo.com)
// Copyright (C) Popescu Alexandru Cristian All rights reserved.
//-----------------------------------------------------------------------------

//The MIT License

//Copyright (c) 2010 Popescu Alexandru Cristian

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.
#endregion

using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace rMap.Asset
{
    /// <summary>
    /// Read/Write dds files from/to files or from streams.
    /// </summary>
    public static class DDSLib
    {
        private const int DDSD_CAPS = 0x1;                      //Required in every .dds file.	
        private const int DDSD_HEIGHT = 0x2;                    //Required in every .dds file.
        private const int DDSD_WIDTH = 0x4;                     //Required in every .dds file.
        private const int DDSD_PITCH = 0x8;                     //Required when pitch is provided for an uncompressed texture.
        private const int DDSD_PIXELFORMAT = 0x1000;            //Required in every .dds file.
        private const int DDSD_MIPMAPCOUNT = 0x20000;           //Required in a mipmapped texture.
        private const int DDSD_LINEARSIZE = 0x80000;            //Required when pitch is provided for a compressed texture.
        private const int DDSD_DEPTH = 0x800000;                //Required in a depth texture.

        private const int DDPF_ALPHAPIXELS = 0x1;               //Texture contains alpha data; dwRGBAlphaBitMask contains valid data.	
        private const int DDPF_ALPHA = 0x2;	                    //Used in some older DDS files for alpha channel only uncompressed data (dwRGBBitCount contains the alpha channel bitcount; dwABitMask contains valid data)	
        private const int DDPF_FOURCC = 0x4;	                //Texture contains compressed RGB data; dwFourCC contains valid data.	
        private const int DDPF_RGB = 0x40;	                    //Texture contains uncompressed RGB data; dwRGBBitCount and the RGB masks (dwRBitMask, dwRBitMask, dwRBitMask) contain valid data.	
        private const int DDPF_YUV = 0x200;	                    //Used in some older DDS files for YUV uncompressed data (dwRGBBitCount contains the YUV bit count; dwRBitMask contains the Y mask, dwGBitMask contains the U mask, dwBBitMask contains the V mask)	
        private const int DDPF_LUMINANCE = 0x2000;	            //Used in some older DDS files for single channel color uncompressed data (dwRGBBitCount contains the luminance channel bit count; dwRBitMask contains the channel mask). Can be combined with DDPF_ALPHAPIXELS for a two channel DDS file.	
        private const int DDPF_Q8W8V8U8 = 0x00080000;           //Used by Microsoft tools when a Q8W8V8U8 is present, this is not a documeneted flag.

        private const int DDSCAPS_COMPLEX = 0x8;	            //Optional; must be used on any file that contains more than one surface (a mipmap, a cubic environment map, or mipmapped volume texture).	
        private const int DDSCAPS_MIPMAP = 0x400000;            //Optional; should be used for a mipmap.	
        private const int DDSCAPS_TEXTURE = 0x1000;	            //Required	

        private const int DDSCAPS2_CUBEMAP = 0x200;             //Required for a cube map.	
        private const int DDSCAPS2_CUBEMAP_POSITIVEX = 0x400;	//Required when these surfaces are stored in a cube map.	
        private const int DDSCAPS2_CUBEMAP_NEGATIVEX = 0x800;	//Required when these surfaces are stored in a cube map.	
        private const int DDSCAPS2_CUBEMAP_POSITIVEY = 0x1000;	//Required when these surfaces are stored in a cube map.	
        private const int DDSCAPS2_CUBEMAP_NEGATIVEY = 0x2000;	//Required when these surfaces are stored in a cube map.	
        private const int DDSCAPS2_CUBEMAP_POSITIVEZ = 0x4000;	//Required when these surfaces are stored in a cube map.	
        private const int DDSCAPS2_CUBEMAP_NEGATIVEZ = 0x8000;	//Required when these surfaces are stored in a cube map.	
        private const int DDSCAPS2_VOLUME = 0x200000;           //Required for a volume texture.

        private const uint DDS_MAGIC = 0x20534444; // "DDS "

        //Compression formats.
        [Flags()]
        private enum FourCC : uint
        {
            D3DFMT_DXT1 = 0x31545844,
            D3DFMT_DXT2 = 0x32545844,
            D3DFMT_DXT3 = 0x33545844,
            D3DFMT_DXT4 = 0x34545844,
            D3DFMT_DXT5 = 0x35545844,
            DX10 = 0x30315844,
            DXGI_FORMAT_BC4_UNORM = 0x55344342,
            DXGI_FORMAT_BC4_SNORM = 0x53344342,
            DXGI_FORMAT_BC5_UNORM = 0x32495441,
            DXGI_FORMAT_BC5_SNORM = 0x53354342,

            //DXGI_FORMAT_R8G8_B8G8_UNORM
            D3DFMT_R8G8_B8G8 = 0x47424752,

            //DXGI_FORMAT_G8R8_G8B8_UNORM
            D3DFMT_G8R8_G8B8 = 0x42475247,

            //DXGI_FORMAT_R16G16B16A16_UNORM
            D3DFMT_A16B16G16R16 = 36,

            //DXGI_FORMAT_R16G16B16A16_SNORM
            D3DFMT_Q16W16V16U16 = 110,

            //DXGI_FORMAT_R16_FLOAT
            D3DFMT_R16F = 111,

            //DXGI_FORMAT_R16G16_FLOAT
            D3DFMT_G16R16F = 112,

            //DXGI_FORMAT_R16G16B16A16_FLOAT
            D3DFMT_A16B16G16R16F = 113,

            //DXGI_FORMAT_R32_FLOAT
            D3DFMT_R32F = 114,

            //DXGI_FORMAT_R32G32_FLOAT
            D3DFMT_G32R32F = 115,

            //DXGI_FORMAT_R32G32B32A32_FLOAT
            D3DFMT_A32B32G32R32F = 116,

            D3DFMT_UYVY = 0x59565955,
            D3DFMT_YUY2 = 0x32595559,
            D3DFMT_CxV8U8 = 117,

            //This is set only by the nvidia exporter, it is not set by the dx texture tool
            //,it is ignored by the dx texture tool but it returns the ability to be opened in photoshop so I decided to keep it.
            D3DFMT_Q8W8V8U8 = 63,
        }

        // Indicates whether this texture is cube map.
        private static bool IsCubemapTest(int ddsCaps1, int ddsCaps2)
        {
            return ((ddsCaps1 & DDSCAPS_COMPLEX) != 0) && ((ddsCaps2 & DDSCAPS2_CUBEMAP) != 0);
        }

        // Indicates whether this texture is volume map. 
        private static bool IsVolumeTextureTest(int ddsCaps1, int ddsCaps2)
        {
            //return ((ddsCaps1 & DDSCAPS_COMPLEX) != 0) && ((ddsCaps2 & DDSCAPS2_VOLUME) != 0);
            return ((ddsCaps2 & DDSCAPS2_VOLUME) != 0);
        }


        //Test if the texture is using any compression.
        private static bool IsCompressedTest(uint pfFlags)
        {
            return ((pfFlags & DDPF_FOURCC) != 0);
        }

        private static bool HasAlphaTest(uint pfFlags)
        {
            return ((pfFlags & DDPF_ALPHAPIXELS) != 0);
        }

        //We need the the mip size, we shift until we get there but the smallest mip must be at least of 1 pixel.
        private static int MipMapSize(int map, int size)
        {
            for (int i = 0; i < map; i++)
                size >>= 1;
            if (size <= 0)
                return 1;
            return size;
        }

        //Surface formats that we can load from a dds
        //I am not using the XNA SurfaceFormat as that one is missing a few formats.
        private enum LoadSurfaceFormat
        {
            Unknown,
            Dxt1,
            Dxt3,
            Dxt5,
            R8G8B8,
            B8G8R8,
            Bgra5551,
            Bgra4444,
            Bgr565,
            Alpha8,
            X8R8G8B8,
            A8R8G8B8,
            A8B8G8R8,
            X8B8G8R8,
            RGB555,
            R32F,
            R16F,
            A32B32G32R32F,
            A16B16G16R16F,
            Q8W8V8U8,
            CxV8U8,
            G16R16F,
            G32R32F,
            G16R16,
            A2B10G10R10,
            A16B16G16R16,
        }

        //Get pixel format from hader.
        private static LoadSurfaceFormat GetLoadSurfaceFormat(uint pixelFlags, uint pixelFourCC, int bitCount, uint rBitMask, uint gBitMask, uint bBitMask, uint aBitMask, FourCC compressionFormat)
        {
            FourCC givenFourCC = (FourCC)pixelFourCC;

            if (givenFourCC == FourCC.D3DFMT_A16B16G16R16)
            {
                return LoadSurfaceFormat.A16B16G16R16;
            }

            if (givenFourCC == FourCC.D3DFMT_G32R32F)
            {
                return LoadSurfaceFormat.G32R32F;
            }

            if (givenFourCC == FourCC.D3DFMT_G16R16F)
            {
                return LoadSurfaceFormat.G16R16F;
            }

            if (givenFourCC == FourCC.D3DFMT_Q8W8V8U8)
            {
                //This is true if the file was generated with the nvidia tools.
                return LoadSurfaceFormat.Q8W8V8U8;
            }

            if (givenFourCC == FourCC.D3DFMT_CxV8U8)
            {
                return LoadSurfaceFormat.CxV8U8;
            }

            if (givenFourCC == FourCC.D3DFMT_A16B16G16R16F)
            {
                return LoadSurfaceFormat.A16B16G16R16F;
            }

            if (givenFourCC == FourCC.D3DFMT_A32B32G32R32F)
            {
                return LoadSurfaceFormat.A32B32G32R32F;
            }

            if (givenFourCC == FourCC.D3DFMT_R32F)
            {
                return LoadSurfaceFormat.R32F;
            }

            if (givenFourCC == FourCC.D3DFMT_R16F)
            {
                return LoadSurfaceFormat.R16F;
            }

            if ((pixelFlags & DDPF_FOURCC) != 0)
            {
                //The texture is compressed(Dxt1,Dxt3/Dxt2,Dxt5/Dxt4).
                if (pixelFourCC == 0x31545844)
                {
                    return LoadSurfaceFormat.Dxt1;
                }
                if (pixelFourCC == 0x33545844 || pixelFourCC == 0x32545844)
                {
                    return LoadSurfaceFormat.Dxt3;
                }
                if (pixelFourCC == 0x35545844 || pixelFourCC == 0x34545844)
                {
                    return LoadSurfaceFormat.Dxt5;
                }
            }

            if ((pixelFlags & DDPF_RGB) != 0)
            {
                if (pixelFlags == 0x40 &&
                    bitCount == 0x00000010 &&
                    pixelFourCC == 0 &&
                    rBitMask == 0x00007c00 &&
                    gBitMask == 0x000003e0 &&
                    bBitMask == 0x0000001f &&
                    aBitMask == 0x00000000)
                {
                    return LoadSurfaceFormat.RGB555;
                }

                if (pixelFlags == 0x41 &&
                    bitCount == 0x20 &&
                    pixelFourCC == 0 &&
                    rBitMask == 0xff0000 &&
                    gBitMask == 0xff00 &&
                    bBitMask == 0xff &&
                    aBitMask == 0xff000000)
                {
                    return LoadSurfaceFormat.A8R8G8B8;
                }

                if (pixelFlags == 0x40 &&
                    bitCount == 0x20 &&
                    pixelFourCC == 0 &&
                    rBitMask == 0xff0000 &&
                    gBitMask == 0xff00 &&
                    bBitMask == 0xff &&
                    aBitMask == 0)
                {
                    //DDS_FORMAT_X8R8G8B8
                    return LoadSurfaceFormat.X8R8G8B8;
                }

                if (pixelFlags == 0x41 &&
                    bitCount == 0x20 &&
                    pixelFourCC == 0 &&
                    rBitMask == 0xff &&
                    gBitMask == 0xff00 &&
                    bBitMask == 0xff0000 &&
                    aBitMask == 0xff000000)
                {
                    //DDS_FORMAT_A8B8G8R8
                    return LoadSurfaceFormat.A8B8G8R8;
                }

                if (pixelFlags == 0x40 &&
                    bitCount == 0x20 &&
                    pixelFourCC == 0 &&
                    rBitMask == 0xff &&
                    gBitMask == 0xff00 &&
                    bBitMask == 0xff0000 &&
                    aBitMask == 0)
                {
                    //DDS_FORMAT_X8B8G8R8
                    return LoadSurfaceFormat.X8B8G8R8;
                }

                if (pixelFlags == 0x41 &&
                    bitCount == 0x10 &&
                    pixelFourCC == 0 &&
                    rBitMask == 0x7c00 &&
                    gBitMask == 0x3e0 &&
                    bBitMask == 0x1f &&
                    aBitMask == 0x8000)
                {
                    return LoadSurfaceFormat.Bgra5551;
                }

                if (pixelFlags == 0x41 &&
                    bitCount == 0x10 &&
                    pixelFourCC == 0 &&
                    rBitMask == 0xf00 &&
                    gBitMask == 240 &&
                    bBitMask == 15 &&
                    aBitMask == 0xf000)
                {
                    return LoadSurfaceFormat.Bgra4444;
                }

                if (pixelFlags == 0x40 &&
                    bitCount == 0x18 &&
                    pixelFourCC == 0 &&
                    rBitMask == 0xff0000 &&
                    gBitMask == 0xff00 &&
                    bBitMask == 0xff &&
                    aBitMask == 0)
                {
                    //DDS_FORMAT_R8G8B8
                    return LoadSurfaceFormat.R8G8B8;
                }

                if (pixelFlags == 0x40 &&
                    bitCount == 0x10 &&
                    pixelFourCC == 0 &&
                    rBitMask == 0xf800 &&
                    gBitMask == 0x7e0 &&
                    bBitMask == 0x1f &&
                    aBitMask == 0)
                {
                    return LoadSurfaceFormat.Bgr565;
                }

                if (pixelFlags == 2 &&
                    bitCount == 8 &&
                    pixelFourCC == 0 &&
                    rBitMask == 0 &&
                    gBitMask == 0 &&
                    bBitMask == 0 &&
                    aBitMask == 255)
                {
                    return LoadSurfaceFormat.Alpha8;
                }

                if (pixelFlags == 0x40 &&
                    bitCount == 32 &&
                    pixelFourCC == 0 &&
                    rBitMask == 0x0000ffff &&
                    gBitMask == 0xffff0000 &&
                    bBitMask == 0 &&
                    aBitMask == 0)
                {
                    return LoadSurfaceFormat.G16R16;
                }

                if (pixelFlags == 0x41 &&
                    bitCount == 32 &&
                    pixelFourCC == 0 &&
                    rBitMask == 0x3ff00000 &&
                    gBitMask == 0x000ffc00 &&
                    bBitMask == 0x000003ff &&
                    aBitMask == 0xc0000000)
                {
                    return LoadSurfaceFormat.A2B10G10R10;
                }
            }

            //We consider the standard dx pixelFourCC + pixelFourCC == 63(nvidia tools generated dds)
            if (pixelFlags == 0x00080000 &&
                bitCount == 32 &&
                (pixelFourCC == 0 || pixelFourCC == 63) &&
                rBitMask == 0x000000ff &&
                gBitMask == 0x0000ff00 &&
                bBitMask == 0x00ff0000 &&
                aBitMask == 0xff000000)
            {
                return LoadSurfaceFormat.Q8W8V8U8;
            }

            return LoadSurfaceFormat.Unknown;
        }

        //Get compression format.
        private static FourCC GetCompressionFormat(uint pixelFlags, uint pixelFourCC)
        {
            if ((pixelFlags & DDPF_FOURCC) != 0)
                return (FourCC)pixelFourCC;
            else return 0;
        }

        //Get the size in bytes for a mip-map level.
        private static int MipMapSizeInBytes(int map, int width, int height, bool isCompressed, FourCC compressionFormat, int depth)
        {
            width = MipMapSize(map, width);
            height = MipMapSize(map, height);

            //We hardcoded some compression formats as some flags are not set by all the tools for them,
            //as a result for this formats we must hardcode the outcome.
            if (compressionFormat == FourCC.D3DFMT_R32F)
            {
                return width * height * 4;
            }
            if (compressionFormat == FourCC.D3DFMT_R16F)
            {
                return width * height * 2;
            }
            if (compressionFormat == FourCC.D3DFMT_A32B32G32R32F)
            {
                return width * height * 16;
            }
            if (compressionFormat == FourCC.D3DFMT_A16B16G16R16F)
            {
                return width * height * 8;
            }
            if (compressionFormat == FourCC.D3DFMT_CxV8U8)
            {
                return width * height * 2;
            }
            if (compressionFormat == FourCC.D3DFMT_Q8W8V8U8)
            {
                return width * height * 4;
            }
            if (compressionFormat == FourCC.D3DFMT_G16R16F)
            {
                return width * height * 4;
            }
            if (compressionFormat == FourCC.D3DFMT_G32R32F)
            {
                return width * height * 8;
            }
            if (compressionFormat == FourCC.D3DFMT_A16B16G16R16)
            {
                return width * height * 8;
            }

            if (isCompressed)
            {
                int blockSize = (compressionFormat == FourCC.D3DFMT_DXT1 ? 8 : 16);
                return ((width + 3) / 4) * ((height + 3) / 4) * blockSize;
            }
            else
            {
                return width * height * (depth / 8);
            }
        }

        //Get the byte data from a mip-map level.
        private static void GetMipMaps(int offsetInStream, int map, bool hasMipMaps, int width, int height, bool isCompressed, FourCC compressionFormat, int rgbBitCount, bool partOfCubeMap, BinaryReader reader, LoadSurfaceFormat loadSurfaceFormat, ref byte[] data, out int numBytes)
        {
            int seek = 128 + offsetInStream;

            for (int i = 0; i < map; i++)
            {
                seek += MipMapSizeInBytes(i, width, height, isCompressed, compressionFormat, rgbBitCount);
            }

            reader.BaseStream.Seek(seek, SeekOrigin.Begin);

            numBytes = MipMapSizeInBytes(map, width, height, isCompressed, compressionFormat, rgbBitCount);

            if (isCompressed == false && rgbBitCount == 24)
            {
                numBytes += (numBytes / 3);
            }

            if (data == null || data.Length < numBytes)
            {
                data = new byte[numBytes];
            }

            if (isCompressed == false && loadSurfaceFormat == LoadSurfaceFormat.R8G8B8)
            {
                for (int i = 0; i < numBytes; i += 4)
                {
                    data[i] = reader.ReadByte();
                    data[i + 1] = reader.ReadByte();
                    data[i + 2] = reader.ReadByte();
                    data[i + 3] = 255;
                }
            }
            else
            {
                reader.Read(data, 0, numBytes);
            }

            if (loadSurfaceFormat == LoadSurfaceFormat.X8R8G8B8 || loadSurfaceFormat == LoadSurfaceFormat.X8B8G8R8)
            {
                for (int i = 0; i < numBytes; i += 4)
                {
                    data[i + 3] = 255;
                }
            }

            if (loadSurfaceFormat == LoadSurfaceFormat.A8R8G8B8 ||
                loadSurfaceFormat == LoadSurfaceFormat.X8R8G8B8 ||
                loadSurfaceFormat == LoadSurfaceFormat.R8G8B8)
            {
                int bytesPerPixel = (rgbBitCount == 32 || rgbBitCount == 24) ? 4 : 3;

                byte g, b;
                if (bytesPerPixel == 3)
                {
                    for (int i = 0; i < numBytes - 2; i += 3)
                    {
                        g = data[i];
                        b = data[i + 2];
                        data[i] = b;
                        data[i + 2] = g;
                    }
                }
                else
                {
                    for (int i = 0; i < numBytes - 3; i += 4)
                    {
                        g = data[i];
                        b = data[i + 2];
                        data[i] = b;
                        data[i + 2] = g;
                    }
                }
            }
        }

        //Xna only supporst mip-map on textures with full chains == last-mip is 1x1
        private static bool CheckFullMipChain(int width, int height, int numMip)
        {
            int max = Math.Max(width, height);
            int imaginariMipMax = 0;
            while (max > 1)
            {
                max /= 2;
                imaginariMipMax++;
            }

            if (imaginariMipMax <= numMip)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Open a dds from file.
        /// (Supported formats : Dxt1,Dxt2,Dxt3,Dxt4,Dxt5,A8R8G8B8/Color,X8R8G8B8,R8G8B8,A4R4G4B4,A1R5G5B5,R5G6B5,A8,
        /// FP32/Single,FP16/HalfSingle,FP32x4/Vector4,FP16x4/HalfVector4,CxV8U8/NormalizedByte2/CxVU,Q8VW8V8U8/NormalizedByte4/8888QWVU
        /// ,HalfVector2/G16R16F/16.16fGR,Vector2/G32R32F,G16R16/RG32/1616GB,B8G8R8,X8B8G8R8,A8B8G8R8/Color,L8,A2B10G10R10/Rgba1010102,A16B16G16R16/Rgba64)
        /// </summary>
        /// <param name="fileName">File containing the data.</param>
        /// <param name="device">Graphic device where you want the texture to be loaded.</param>
        /// <param name="texture">The reference to the loaded texture.</param>
        /// <param name="streamOffset">Offset in the stream to where the DDS is located.</param>
        /// <param name="loadMipMap">If true it will load the mip-map chain for this texture.</param>
        public static void DDSFromFile(string fileName, GraphicsDevice device, bool loadMipMap, out Texture2D texture)
        {
            Stream stream = File.OpenRead(fileName);
            Texture tex;
            InternalDDSFromStream(stream, device, 0, loadMipMap, out tex);
            stream.Close();

            texture = tex as Texture2D;
            if (texture == null)
            {
                throw new Exception("The data in the stream contains a TextureCube not Texture2D");
            }
        }

        /// <summary>
        /// Open a dds from file.
        /// (Supported formats : Dxt1,Dxt2,Dxt3,Dxt4,Dxt5,A8R8G8B8/Color,X8R8G8B8,R8G8B8,A4R4G4B4,A1R5G5B5,R5G6B5,A8,
        /// FP32/Single,FP16/HalfSingle,FP32x4/Vector4,FP16x4/HalfVector4,CxV8U8/NormalizedByte2/CxVU,Q8VW8V8U8/NormalizedByte4/8888QWVU
        /// ,HalfVector2/G16R16F/16.16fGR,Vector2/G32R32F,G16R16/RG32/1616GB,B8G8R8,X8B8G8R8,A8B8G8R8/Color,L8,A2B10G10R10/Rgba1010102,A16B16G16R16/Rgba64)
        /// </summary>
        /// <param name="fileName">File containing the data.</param>
        /// <param name="device">Graphic device where you want the texture to be loaded.</param>
        /// <param name="texture">The reference to the loaded texture.</param>
        /// <param name="streamOffset">Offset in the stream to where the DDS is located.</param>
        /// <param name="loadMipMap">If true it will load the mip-map chain for this texture.</param>
        public static void DDSFromFile(string fileName, GraphicsDevice device, bool loadMipMap, out TextureCube texture)
        {
            Stream stream = File.OpenRead(fileName);
            Texture tex;
            InternalDDSFromStream(stream, device, 0, loadMipMap, out tex);
            stream.Close();

            texture = tex as TextureCube;
            if (texture == null)
            {
                throw new Exception("The data in the stream contains a Texture2D not TextureCube");
            }
        }

        /// <summary>
        /// Open a dds from file.
        /// (Supported formats : Dxt1,Dxt2,Dxt3,Dxt4,Dxt5,A8R8G8B8/Color,X8R8G8B8,R8G8B8,A4R4G4B4,A1R5G5B5,R5G6B5,A8,
        /// FP32/Single,FP16/HalfSingle,FP32x4/Vector4,FP16x4/HalfVector4,CxV8U8/NormalizedByte2/CxVU,Q8VW8V8U8/NormalizedByte4/8888QWVU
        /// ,HalfVector2/G16R16F/16.16fGR,Vector2/G32R32F,G16R16/RG32/1616GB,B8G8R8,X8B8G8R8,A8B8G8R8/Color,L8,A2B10G10R10/Rgba1010102,A16B16G16R16/Rgba64)
        /// </summary>
        /// <param name="fileName">File containing the data.</param>
        /// <param name="device">Graphic device where you want the texture to be loaded.</param>
        /// <param name="texture">The reference to the loaded texture.</param>
        /// <param name="streamOffset">Offset in the stream to where the DDS is located.</param>
        /// <param name="loadMipMap">If true it will load the mip-map chain for this texture.</param>
        public static void DDSFromFile(string fileName, GraphicsDevice device, bool loadMipMap, out Texture3D texture)
        {
            Stream stream = File.OpenRead(fileName);
            Texture tex;
            InternalDDSFromStream(stream, device, 0, loadMipMap, out tex);
            stream.Close();

            texture = tex as Texture3D;
            if (texture == null)
            {
                throw new Exception("The data in the stream contains a Texture2D not TextureCube");
            }
        }

        /// <summary>
        /// Open a dds from a stream.
        /// (Supported formats : Dxt1,Dxt2,Dxt3,Dxt4,Dxt5,A8R8G8B8/Color,X8R8G8B8,R8G8B8,A4R4G4B4,A1R5G5B5,R5G6B5,A8,
        /// FP32/Single,FP16/HalfSingle,FP32x4/Vector4,FP16x4/HalfVector4,CxV8U8/NormalizedByte2/CxVU,Q8VW8V8U8/NormalizedByte4/8888QWVU
        /// ,HalfVector2/G16R16F/16.16fGR,Vector2/G32R32F,G16R16/RG32/1616GB,B8G8R8,X8B8G8R8,A8B8G8R8/Color,L8,A2B10G10R10/Rgba1010102,A16B16G16R16/Rgba64)
        /// </summary>
        /// <param name="stream">Stream containing the data.</param>
        /// <param name="device">Graphic device where you want the texture to be loaded.</param>
        /// <param name="texture">The reference to the loaded texture.</param>
        /// <param name="streamOffset">Offset in the stream to where the DDS is located.</param>
        /// <param name="loadMipMap">If true it will load the mip-map chain for this texture.</param>
        public static void DDSFromStream(Stream stream, GraphicsDevice device, int streamOffset, bool loadMipMap, out Texture2D texture)
        {
            Texture tex;
            InternalDDSFromStream(stream, device, streamOffset, loadMipMap, out tex);
            texture = tex as Texture2D;
            if (texture == null)
            {
                throw new Exception("The data in the stream contains a TextureCube not Texture2D");
            }
        }

        /// <summary>
        /// Open a dds from a stream.
        /// (Supported formats : Dxt1,Dxt2,Dxt3,Dxt4,Dxt5,A8R8G8B8/Color,X8R8G8B8,R8G8B8,A4R4G4B4,A1R5G5B5,R5G6B5,A8,
        /// FP32/Single,FP16/HalfSingle,FP32x4/Vector4,FP16x4/HalfVector4,CxV8U8/NormalizedByte2/CxVU,Q8VW8V8U8/NormalizedByte4/8888QWVU
        /// ,HalfVector2/G16R16F/16.16fGR,Vector2/G32R32F,G16R16/RG32/1616GB,B8G8R8,X8B8G8R8,A8B8G8R8/Color,L8,A2B10G10R10/Rgba1010102,A16B16G16R16/Rgba64)
        /// </summary>
        /// <param name="stream">Stream containing the data.</param>
        /// <param name="device">Graphic device where you want the texture to be loaded.</param>
        /// <param name="texture">The reference to the loaded texture.</param>
        /// <param name="streamOffset">Offset in the stream to where the DDS is located.</param>
        /// <param name="loadMipMap">If true it will load the mip-map chain for this texture.</param>
        public static void DDSFromStream(Stream stream, GraphicsDevice device, int streamOffset, bool loadMipMap, out TextureCube texture)
        {
            Texture tex;
            InternalDDSFromStream(stream, device, streamOffset, loadMipMap, out tex);

            texture = tex as TextureCube;
            if (texture == null)
            {
                throw new Exception("The data in the stream contains a Texture2D not TextureCube");
            }
        }

        /// <summary>
        /// Open a dds from a stream.
        /// (Supported formats : Dxt1,Dxt2,Dxt3,Dxt4,Dxt5,A8R8G8B8/Color,X8R8G8B8,R8G8B8,A4R4G4B4,A1R5G5B5,R5G6B5,A8,
        /// FP32/Single,FP16/HalfSingle,FP32x4/Vector4,FP16x4/HalfVector4,CxV8U8/NormalizedByte2/CxVU,Q8VW8V8U8/NormalizedByte4/8888QWVU
        /// ,HalfVector2/G16R16F/16.16fGR,Vector2/G32R32F,G16R16/RG32/1616GB,B8G8R8,X8B8G8R8,A8B8G8R8/Color,L8,A2B10G10R10/Rgba1010102,A16B16G16R16/Rgba64)
        /// </summary>
        /// <param name="stream">Stream containing the data.</param>
        /// <param name="device">Graphic device where you want the texture to be loaded.</param>
        /// <param name="texture">The reference to the loaded texture.</param>
        /// <param name="streamOffset">Offset in the stream to where the DDS is located.</param>
        /// <param name="loadMipMap">If true it will load the mip-map chain for this texture.</param>
        public static void DDSFromStream(Stream stream, GraphicsDevice device, int streamOffset, bool loadMipMap, out Texture3D texture)
        {
            Texture tex;
            InternalDDSFromStream(stream, device, streamOffset, loadMipMap, out tex);

            texture = tex as Texture3D;
            if (texture == null)
            {
                throw new Exception("The data in the stream contains a Texture2D not TextureCube");
            }
        }

#if WINDOWS
        [ThreadStatic]
        private static byte[] mipData;
#else

        static DDSLib()
        {
            mipDataSet = new Dictionary<int, byte[]>();
        }

        private static Dictionary<int, byte[]> mipDataSet;
        private static byte[] mipData
        {
            get
            {
                int localThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
                if (mipDataSet.ContainsKey(localThreadID))
                {
                    return mipDataSet[localThreadID];
                }
                else
                {
                    lock (mipDataSet)
                    {
                        mipDataSet.Add(localThreadID, new byte[1]);
                    }
                    return mipDataSet[localThreadID];
                }
            }
            set
            {
                lock (mipDataSet)
                {
                    int localThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
                    mipDataSet[localThreadID] = value;
                }
            }
        }
#endif

        //try to evaluate the xna compatible surface for the present data
        private static SurfaceFormat SurfaceFormatFromLoadFormat(LoadSurfaceFormat loadSurfaceFormat, FourCC compressionFormat, uint pixelFlags, int rgbBitCount)
        {
            if (loadSurfaceFormat == LoadSurfaceFormat.Unknown)
            {
                switch (compressionFormat)
                {
                    case FourCC.D3DFMT_DXT1:
                        return SurfaceFormat.Dxt1;
                    case FourCC.D3DFMT_DXT3:
                        return SurfaceFormat.Dxt3;
                    case FourCC.D3DFMT_DXT5:
                        return SurfaceFormat.Dxt5;
                    case 0:
                        if (rgbBitCount == 8)
                        {
                            return SurfaceFormat.Alpha8;
                        }
                        if (rgbBitCount == 16)
                        {
                            if (HasAlphaTest(pixelFlags))
                            {
                                return SurfaceFormat.Bgr565;
                            }
                            else
                            {
                                return SurfaceFormat.Bgra4444;
                            }
                        }
                        if (rgbBitCount == 32 || rgbBitCount == 24)
                        {
                            return SurfaceFormat.Color;
                        }
                        break;
                    default:
                        throw new Exception("Unsuported format");
                }
            }
            else
            {
                switch (loadSurfaceFormat)
                {
                    case LoadSurfaceFormat.Alpha8:
                        return SurfaceFormat.Alpha8;
                    case LoadSurfaceFormat.Bgr565:
                        return SurfaceFormat.Bgr565;
                    case LoadSurfaceFormat.Bgra4444:
                        return SurfaceFormat.Bgra4444;
                    case LoadSurfaceFormat.Bgra5551:
                        return SurfaceFormat.Bgra5551;
                    case LoadSurfaceFormat.A8R8G8B8:
                        return SurfaceFormat.Color;
                    case LoadSurfaceFormat.Dxt1:
                        return SurfaceFormat.Dxt1;
                    case LoadSurfaceFormat.Dxt3:
                        return SurfaceFormat.Dxt3;
                    case LoadSurfaceFormat.Dxt5:
                        return SurfaceFormat.Dxt5;
                    //Updated at load time to X8R8B8B8
                    case LoadSurfaceFormat.R8G8B8:
                        return SurfaceFormat.Color;
                    case LoadSurfaceFormat.X8B8G8R8:
                        return SurfaceFormat.Color;
                    case LoadSurfaceFormat.X8R8G8B8:
                        return SurfaceFormat.Color;
                    case LoadSurfaceFormat.A8B8G8R8:
                        return SurfaceFormat.Color;
                    case LoadSurfaceFormat.R32F:
                        return SurfaceFormat.Single;
                    case LoadSurfaceFormat.A32B32G32R32F:
                        return SurfaceFormat.Vector4;
                    case LoadSurfaceFormat.G32R32F:
                        return SurfaceFormat.Vector2;
                    case LoadSurfaceFormat.R16F:
                        return SurfaceFormat.HalfSingle;
                    case LoadSurfaceFormat.G16R16F:
                        return SurfaceFormat.HalfVector2;
                    case LoadSurfaceFormat.A16B16G16R16F:
                        return SurfaceFormat.HalfVector4;
                    case LoadSurfaceFormat.CxV8U8:
                        return SurfaceFormat.NormalizedByte2;
                    case LoadSurfaceFormat.Q8W8V8U8:
                        return SurfaceFormat.NormalizedByte4;
                    case LoadSurfaceFormat.G16R16:
                        return SurfaceFormat.Rg32;
                    case LoadSurfaceFormat.A2B10G10R10:
                        return SurfaceFormat.Rgba1010102;
                    case LoadSurfaceFormat.A16B16G16R16:
                        return SurfaceFormat.Rgba64;
                    default:
                        throw new Exception(loadSurfaceFormat.ToString() + " is an unsuported format");
                }
            }

            throw new Exception("Unsuported format");
        }

        //new cube-map texture
        private static TextureCube GenerateNewCubeTexture(LoadSurfaceFormat loadSurfaceFormat, FourCC compressionFormat, GraphicsDevice device, int width, bool hasMipMaps, uint pixelFlags, int rgbBitCount)
        {
            SurfaceFormat surfaceFormat = SurfaceFormatFromLoadFormat(loadSurfaceFormat, compressionFormat, pixelFlags, rgbBitCount);

            TextureCube tx = new TextureCube(device, width, hasMipMaps, surfaceFormat);

            if (tx.Format != surfaceFormat)
            {
                throw new Exception("Can't generate a " + surfaceFormat.ToString() + " surface.");
            }

            return tx;
        }

        //new 2d-map texture
        private static Texture2D GenerateNewTexture2D(LoadSurfaceFormat loadSurfaceFormat, FourCC compressionFormat, GraphicsDevice device, int width, int height, bool hasMipMaps, uint pixelFlags, int rgbBitCount)
        {
            SurfaceFormat surfaceFormat = SurfaceFormatFromLoadFormat(loadSurfaceFormat, compressionFormat, pixelFlags, rgbBitCount);

            Texture2D tx = new Texture2D(device, width, height, hasMipMaps, surfaceFormat);

            if (tx.Format != surfaceFormat)
            {
                throw new Exception("Can't generate a " + surfaceFormat.ToString() + " surface.");
            }

            return tx;
        }

        //new 3d-map texture
        private static Texture3D GenerateNewTexture3D(LoadSurfaceFormat loadSurfaceFormat, FourCC compressionFormat, GraphicsDevice device, int width, int height, int depth, bool hasMipMaps, uint pixelFlags, int rgbBitCount)
        {
            SurfaceFormat surfaceFormat = SurfaceFormatFromLoadFormat(loadSurfaceFormat, compressionFormat, pixelFlags, rgbBitCount);

            Texture3D tx = new Texture3D(device, width, height, depth, hasMipMaps, surfaceFormat);

            if (tx.Format != surfaceFormat)
            {
                throw new Exception("Can't generate a " + surfaceFormat.ToString() + " surface.");
            }

            return tx;
        }

        //loads the data from a stream in to a texture object.
        private static void InternalDDSFromStream(Stream stream, GraphicsDevice device, int streamOffset, bool loadMipMap, out Texture texture)
        {
            if (stream == null)
            {
                throw new Exception("Can't read from a null stream");
            }

            BinaryReader reader = new BinaryReader(stream);

            if (streamOffset > reader.BaseStream.Length)
            {
                throw new Exception("The stream you offered is smaller then the offset you are proposing for it.");
            }

            reader.BaseStream.Seek(streamOffset, SeekOrigin.Begin);

            //First element of a dds file is a "magic-number" a system to identify that the file is a dds if translated as asci chars the first 4 charachters should be 'DDS '

            bool isDDS = reader.ReadUInt32() == DDS_MAGIC;
            //bool isDDS = (reader.ReadChar() == 'D' && reader.ReadChar() == 'D' && reader.ReadChar() == 'S');
            //empty char space.
            //reader.ReadChar();

            if (!isDDS)
            {
                throw new Exception("Can't open non DDS data.");
            }

            // size of the DDSURFACEDESC.
            //reader.ReadInt32();

            // validation flags.
            //reader.ReadInt32();

            reader.BaseStream.Position += 8;

            //size in pixels for the texture.
            int height = reader.ReadInt32();
            int width = reader.ReadInt32();

            //linear size.
            //reader.ReadInt32();

            reader.BaseStream.Position += 4;

            //depth
            int depth = reader.ReadInt32();

            //number of mip-maps.
            int numMips = reader.ReadInt32();

            //alpha bit depth.
            //reader.ReadInt32();

            //empty space.
            //reader.ReadInt32();

            //pointer to associated surface.
            //reader.ReadInt32();

            //cubemap not present bitmaps colors.
            //colorSpaceLowValue
            //reader.ReadInt32();
            //colorSpaceHighValue
            //reader.ReadInt32();
            //destBltColorSpaceLowValue
            //reader.ReadInt32();
            //destBltColorSpaceHighValue
            //reader.ReadInt32();
            //srcOverlayColorSpaceLowValue
            //reader.ReadInt32();
            //srcOverlayColorSpaceHighValue
            //reader.ReadInt32();
            //srcBltColorSpaceLowValue
            //reader.ReadInt32();
            //srcBltColorSpaceHighValue
            //reader.ReadInt32();

            // size of DDPIXELFORMAT structure
            //reader.ReadInt32();

            reader.BaseStream.Position += 4 * 12;

            //pixel format flags
            uint pixelFlags = reader.ReadUInt32();

            // (FOURCC code)
            uint pixelFourCC = reader.ReadUInt32();

            //color bit depth
            int rgbBitCount = reader.ReadInt32();

            //mask for red.
            uint rBitMask = reader.ReadUInt32();

            //mask for green.
            uint gBitMask = reader.ReadUInt32();

            //mask for blue.
            uint bBitMask = reader.ReadUInt32();


            //mask for alpha.
            uint aBitMask = reader.ReadUInt32();

            //reader.BaseStream.Position += 16;

            //texture + mip-map flags.
            int ddsCaps1 = reader.ReadInt32();

            //extra info flags.
            int ddsCaps2 = reader.ReadInt32();
            //ddsCaps3
            //reader.ReadInt32();
            //ddsCaps4
            //reader.ReadInt32();

            //reader.ReadInt32();

            reader.BaseStream.Position += 12;

            bool isCubeMap = IsCubemapTest(ddsCaps1, ddsCaps2);

            bool isVolumeTexture = IsVolumeTextureTest(ddsCaps1, ddsCaps2);

            FourCC compressionFormat = GetCompressionFormat(pixelFlags, pixelFourCC);

            if (compressionFormat == FourCC.DX10)
            {
                throw new Exception("The Dxt 10 header reader is not implemented");
            }

            LoadSurfaceFormat loadSurfaceFormat = GetLoadSurfaceFormat(pixelFlags, pixelFourCC, rgbBitCount, rBitMask, gBitMask, bBitMask, aBitMask, compressionFormat);

            bool isCompressed = IsCompressedTest(pixelFlags);

            bool hasMipMaps = CheckFullMipChain(width, height, numMips);

            bool hasAnyMipmaps = numMips > 0;

            hasMipMaps &= loadMipMap;

            if (isCubeMap)
            {
                TextureCube tex = GenerateNewCubeTexture(loadSurfaceFormat, compressionFormat, device, width, hasMipMaps, pixelFlags, rgbBitCount);

                int byteAcumulator = 0;

                if (numMips == 0)
                {
                    numMips = 1;
                }

                if (!hasMipMaps)
                {
                    for (int j = 0; j < numMips; j++)
                    {
                        byteAcumulator += MipMapSizeInBytes(j, width, height, isCompressed, compressionFormat, rgbBitCount);
                    }
                }

                for (int j = 0; j < numMips; j++)
                {
                    int numBytes = 0;

                    byte[] localMipData = mipData;
                    GetMipMaps(streamOffset, j, hasAnyMipmaps, width, height, isCompressed, compressionFormat, rgbBitCount, isCubeMap, reader, loadSurfaceFormat, ref localMipData, out numBytes);
                    mipData = localMipData;

                    if (hasMipMaps)
                    {
                        byteAcumulator += numBytes;
                    }

                    if (j == 0 || hasMipMaps)
                    {
                        tex.SetData<byte>(CubeMapFace.PositiveX, j, null, localMipData, 0, numBytes);
                    }
                    else
                    {
                        break;
                    }
                }

                for (int j = 0; j < numMips; j++)
                {
                    int numBytes = 0;

                    byte[] localMipData = mipData;
                    GetMipMaps(byteAcumulator + streamOffset, j, hasAnyMipmaps, width, height, isCompressed, compressionFormat, rgbBitCount, isCubeMap, reader, loadSurfaceFormat, ref localMipData, out numBytes);
                    mipData = localMipData;

                    if (j == 0 || hasMipMaps)
                    {
                        tex.SetData<byte>(CubeMapFace.NegativeX, j, null, localMipData, 0, numBytes);
                    }
                    else
                    {
                        break;
                    }
                }

                for (int j = 0; j < numMips; j++)
                {
                    int numBytes = 0;

                    byte[] localMipData = mipData;
                    GetMipMaps((byteAcumulator * 2) + streamOffset, j, hasAnyMipmaps, width, height, isCompressed, compressionFormat, rgbBitCount, isCubeMap, reader, loadSurfaceFormat, ref localMipData, out numBytes);
                    mipData = localMipData;

                    if (j == 0 || hasMipMaps)
                    {
                        tex.SetData<byte>(CubeMapFace.PositiveY, j, null, localMipData, 0, numBytes);
                    }
                    else
                    {
                        break;
                    }
                }

                for (int j = 0; j < numMips; j++)
                {
                    int numBytes = 0;

                    byte[] localMipData = mipData;
                    GetMipMaps((byteAcumulator * 3) + streamOffset, j, hasAnyMipmaps, width, height, isCompressed, compressionFormat, rgbBitCount, isCubeMap, reader, loadSurfaceFormat, ref localMipData, out numBytes);
                    mipData = localMipData;

                    if (j == 0 || hasMipMaps)
                    {
                        tex.SetData<byte>(CubeMapFace.NegativeY, j, null, localMipData, 0, numBytes);
                    }
                    else
                    {
                        break;
                    }
                }

                for (int j = 0; j < numMips; j++)
                {
                    int numBytes = 0;

                    byte[] localMipData = mipData;
                    GetMipMaps((byteAcumulator * 4) + streamOffset, j, hasAnyMipmaps, width, height, isCompressed, compressionFormat, rgbBitCount, isCubeMap, reader, loadSurfaceFormat, ref localMipData, out numBytes);
                    mipData = localMipData;

                    if (j == 0 || hasMipMaps)
                    {
                        tex.SetData<byte>(CubeMapFace.PositiveZ, j, null, localMipData, 0, numBytes);
                    }
                    else
                    {
                        break;
                    }
                }

                for (int j = 0; j < numMips; j++)
                {
                    int numBytes = 0;

                    byte[] localMipData = mipData;
                    GetMipMaps((byteAcumulator * 5) + streamOffset, j, hasAnyMipmaps, width, height, isCompressed, compressionFormat, rgbBitCount, isCubeMap, reader, loadSurfaceFormat, ref localMipData, out numBytes);
                    mipData = localMipData;

                    if (j == 0 || hasMipMaps)
                    {
                        tex.SetData<byte>(CubeMapFace.NegativeZ, j, null, localMipData, 0, numBytes);
                    }
                    else
                    {
                        break;
                    }
                }

                texture = tex;
            }
            else if (isVolumeTexture)
            {
                Texture3D tex = GenerateNewTexture3D(loadSurfaceFormat, compressionFormat, device, width, height, depth, hasMipMaps, pixelFlags, rgbBitCount);

                int localStreamOffset = streamOffset;
                for (int i = 0; i < tex.LevelCount; i++)
                {
                    int localWidth = MipMapSize(i, width);
                    int localHeight = MipMapSize(i, height);
                    int localDepth = MipMapSize(i, depth);
                    for (int j = 0; j < localDepth; j++)
                    {
                        int numBytes = 0;

                        byte[] localMipData = mipData;
                        GetMipMaps(localStreamOffset, 0, hasAnyMipmaps, localWidth, localHeight, isCompressed, compressionFormat, rgbBitCount, isCubeMap, reader, loadSurfaceFormat, ref localMipData, out numBytes);
                        localStreamOffset += numBytes;
                        mipData = localMipData;

                        tex.SetData<byte>(i, 0, 0, localWidth, localHeight, j, j + 1, localMipData, 0, numBytes);
                    }

                }

                texture = tex;
            }
            else
            {
                Texture2D tex = GenerateNewTexture2D(loadSurfaceFormat, compressionFormat, device, width, height, hasMipMaps, pixelFlags, rgbBitCount);

                for (int i = 0; i < tex.LevelCount; i++)
                {
                    int numBytes = 0;
                    byte[] localMipData = mipData;
                    GetMipMaps(streamOffset, i, hasAnyMipmaps, width, height, isCompressed, compressionFormat, rgbBitCount, isCubeMap, reader, loadSurfaceFormat, ref localMipData, out numBytes);
                    mipData = localMipData;

                    tex.SetData<byte>(i, null, localMipData, 0, numBytes);
                }


                texture = tex;
            }

        }

        //detect if a texture is using a compressed format.
        private static bool IsXNATextureCompressed(Texture texture)
        {
            if (texture.Format == SurfaceFormat.Dxt1 ||
                texture.Format == SurfaceFormat.Dxt3 ||
                texture.Format == SurfaceFormat.Dxt5)
            {
                return true;
            }

            return false;
        }

        //compression for given texture expressed as FourCC code.
        private static FourCC XNATextureFourCC(Texture texture)
        {
            if (texture.Format == SurfaceFormat.Rgba64)
            {
                return FourCC.D3DFMT_A16B16G16R16;
            }

            if (texture.Format == SurfaceFormat.Vector4)
            {
                return FourCC.D3DFMT_A32B32G32R32F;
            }

            if (texture.Format == SurfaceFormat.Vector2)
            {
                return FourCC.D3DFMT_G32R32F;
            }

            if (texture.Format == SurfaceFormat.HalfVector2)
            {
                return FourCC.D3DFMT_G16R16F;
            }

            if (texture.Format == SurfaceFormat.NormalizedByte4)
            {
                return FourCC.D3DFMT_Q8W8V8U8;
            }

            if (texture.Format == SurfaceFormat.NormalizedByte2)
            {
                return FourCC.D3DFMT_CxV8U8;
            }

            if (texture.Format == SurfaceFormat.HalfVector4)
            {
                return FourCC.D3DFMT_A16B16G16R16F;
            }

            if (texture.Format == SurfaceFormat.Single)
            {
                return FourCC.D3DFMT_R32F;
            }

            if (texture.Format == SurfaceFormat.HalfSingle)
            {
                return FourCC.D3DFMT_R16F;
            }

            if (texture.Format == SurfaceFormat.Dxt1)
            {
                return FourCC.D3DFMT_DXT1;
            }
            if (texture.Format == SurfaceFormat.Dxt3)
            {
                return FourCC.D3DFMT_DXT3;
            }
            if (texture.Format == SurfaceFormat.Dxt5)
            {
                return FourCC.D3DFMT_DXT5;
            }
            return 0;
        }

        //color depth for the given texture.
        private static int XNATextureColorDepth(Texture texture)
        {
            return XNATextureNumBytesPerPixel(texture) * 8;
        }

        //color depth for the given texture in bytes.
        private static int XNATextureNumBytesPerPixel(Texture texture)
        {
            int pixelWidth = 0;
            switch (texture.Format)
            {
                case SurfaceFormat.Dxt1:
                case SurfaceFormat.Dxt3:
                case SurfaceFormat.Dxt5:
                    pixelWidth = 0;
                    break;

                case SurfaceFormat.Vector4:
                    pixelWidth = 16;
                    break;

                case SurfaceFormat.Rgba64:
                case SurfaceFormat.HalfVector4:
                case SurfaceFormat.Vector2:
                    pixelWidth = 8;
                    break;

                case SurfaceFormat.Rg32:
                case SurfaceFormat.Rgba1010102:
                case SurfaceFormat.NormalizedByte4:
                case SurfaceFormat.HalfVector2:
                case SurfaceFormat.Single:
                case SurfaceFormat.Color:
                    pixelWidth = 4;
                    break;

                case SurfaceFormat.NormalizedByte2:
                case SurfaceFormat.HalfSingle:
                case SurfaceFormat.Bgra5551:
                case SurfaceFormat.Bgra4444:
                case SurfaceFormat.Bgr565:
                    pixelWidth = 2;
                    break;

                case SurfaceFormat.Alpha8:
                    pixelWidth = 1;
                    break;
                default:
                    throw new Exception(texture.Format + " has no save as DDS support.");
            }
            return pixelWidth;
        }

        //Generate the data for the DDS pixel flags structure.
        private static void GenerateDdspf(SurfaceFormat fileFormat, out uint flags, out uint rgbBitCount, out uint rBitMask, out uint gBitMask, out uint bBitMask, out uint aBitMask, out uint fourCC)
        {
            switch (fileFormat)
            {
                case SurfaceFormat.Dxt1:
                case SurfaceFormat.Dxt3:
                case SurfaceFormat.Dxt5:
                    flags = 4;
                    rgbBitCount = 0;
                    rBitMask = 0;
                    gBitMask = 0;
                    bBitMask = 0;
                    aBitMask = 0;
                    fourCC = 0;
                    if (fileFormat == SurfaceFormat.Dxt1)
                    {
                        fourCC = 0x31545844;
                    }
                    if (fileFormat == SurfaceFormat.Dxt3)
                    {
                        fourCC = 0x33545844;
                    }
                    if (fileFormat == SurfaceFormat.Dxt5)
                    {
                        fourCC = 0x35545844;
                    }
                    return;
#if COLOR_SAVE_TO_ARGB
                case SurfaceFormat.Color:
                    flags = 0x41;
                    rgbBitCount = 32;
                    fourCC = 0;
                    rBitMask = 0xff0000;
                    gBitMask = 0xff00;
                    bBitMask = 0xff;
                    aBitMask = 0xff000000;
                    return;
#else
                case SurfaceFormat.Color:
                    flags = 0x41;
                    rgbBitCount = 32;
                    fourCC = 0;
                    rBitMask = 0xff;
                    gBitMask = 0xff00;
                    bBitMask = 0xff0000;
                    aBitMask = 0xff000000;
                    return;
#endif

                //case DDS_FORMAT_X8R8G8B8:
                //    flags = 0x40;
                //    rgbBitCount = 0x20;
                //    fourCC = 0;
                //    rBitMask = 0xff0000;
                //    gBitMask = 0xff00;
                //    bBitMask = 0xff;
                //    aBitMask = 0;
                //    return;

                //case DDS_FORMAT_A8B8G8R8:
                //    flags = 0x41;
                //    rgbBitCount = 0x20;
                //    fourCC = 0;
                //    rBitMask = 0xff;
                //    gBitMask = 0xff00;
                //    bBitMask = 0xff0000;
                //    aBitMask = 0xff000000;
                //    return;

                //case DDS_FORMAT_X8B8G8R8:
                //    flags = 0x40;
                //    rgbBitCount = 0x20;
                //    fourCC = 0;
                //    rBitMask = 0xff;
                //    gBitMask = 0xff00;
                //    bBitMask = 0xff0000;
                //    aBitMask = 0;
                //    return;

                case SurfaceFormat.Bgra5551:
                    flags = 0x41;
                    rgbBitCount = 0x10;
                    fourCC = 0;
                    rBitMask = 0x7c00;
                    gBitMask = 0x3e0;
                    bBitMask = 0x1f;
                    aBitMask = 0x8000;
                    return;

                case SurfaceFormat.Bgra4444:
                    flags = 0x41;
                    rgbBitCount = 0x10;
                    fourCC = 0;
                    rBitMask = 0xf00;
                    gBitMask = 240;
                    bBitMask = 15;
                    aBitMask = 0xf000;
                    return;

                //case DDS_FORMAT_R8G8B8:
                //    flags = 0x40;
                //    fourCC = 0;
                //    rgbBitCount = 0x18;
                //    rBitMask = 0xff0000;
                //    gBitMask = 0xff00;
                //    bBitMask = 0xff;
                //    aBitMask = 0;
                //    return;

                case SurfaceFormat.Bgr565:
                    flags = 0x40;
                    fourCC = 0;
                    rgbBitCount = 0x10;
                    rBitMask = 0xf800;
                    gBitMask = 0x7e0;
                    bBitMask = 0x1f;
                    aBitMask = 0;
                    break;

                case SurfaceFormat.Alpha8:
                    flags = 2;
                    fourCC = 0;
                    rgbBitCount = 8;
                    rBitMask = 0;
                    gBitMask = 0;
                    bBitMask = 0;
                    aBitMask = 255;
                    break;

                case SurfaceFormat.Single:
                    flags = 4;
                    fourCC = 114;
                    rgbBitCount = 0;
                    rBitMask = 0;
                    gBitMask = 0;
                    bBitMask = 0;
                    aBitMask = 0;
                    break;

                case SurfaceFormat.HalfSingle:
                    flags = 4;
                    fourCC = 111;
                    rgbBitCount = 0;
                    rBitMask = 0;
                    gBitMask = 0;
                    bBitMask = 0;
                    aBitMask = 0;
                    break;

                case SurfaceFormat.Vector2:
                    flags = 4;
                    fourCC = 115;
                    rgbBitCount = 0;
                    rBitMask = 0;
                    gBitMask = 0;
                    bBitMask = 0;
                    aBitMask = 0;
                    break;

                case SurfaceFormat.Vector4:
                    flags = 4;
                    fourCC = 116;
                    rgbBitCount = 0;
                    rBitMask = 0;
                    gBitMask = 0;
                    bBitMask = 0;
                    aBitMask = 0;
                    break;

                case SurfaceFormat.HalfVector4:
                    flags = 4;
                    fourCC = 113;
                    rgbBitCount = 0;
                    rBitMask = 0;
                    gBitMask = 0;
                    bBitMask = 0;
                    aBitMask = 0;
                    break;

                case SurfaceFormat.HalfVector2:
                    flags = 4;
                    fourCC = 112;
                    rgbBitCount = 0;
                    rBitMask = 0;
                    gBitMask = 0;
                    bBitMask = 0;
                    aBitMask = 0;
                    break;

                case SurfaceFormat.NormalizedByte2:
                    flags = 4;
                    fourCC = 117;
                    rgbBitCount = 16;
                    rBitMask = 0x000000ff;
                    gBitMask = 0x0000ff00;
                    bBitMask = 0;
                    aBitMask = 0;
                    break;

                case SurfaceFormat.NormalizedByte4:
                    flags = 0x00080000;
                    //This is set because of compatibility problems with the nvidia plugin
                    fourCC = 63;
                    rgbBitCount = 32;
                    rBitMask = 0x000000ff;
                    gBitMask = 0x0000ff00;
                    bBitMask = 0x00ff0000;
                    aBitMask = 0xff000000;
                    break;

                case SurfaceFormat.Rg32:
                    flags = 0x40;
                    fourCC = 0;
                    rgbBitCount = 32;
                    rBitMask = 0x0000ffff;
                    gBitMask = 0xffff0000;
                    bBitMask = 0;
                    aBitMask = 0;
                    break;

                case SurfaceFormat.Rgba1010102:
                    flags = 0x41;
                    fourCC = 0;
                    rgbBitCount = 32;
                    rBitMask = 1072693248;
                    gBitMask = 1047552;
                    bBitMask = 1023;
                    aBitMask = 3221225472;
                    break;


                case SurfaceFormat.Rgba64:
                    flags = 4;
                    fourCC = 36;
                    rgbBitCount = 64;
                    rBitMask = 0;
                    gBitMask = 0;
                    bBitMask = 0;
                    aBitMask = 0;
                    break;


                default:
                    throw new Exception("Unsuported format");
            }
        }

        //Write texture data to stream if the texture is a 2d texture the face is ignored.
        private static void WriteTexture(BinaryWriter writer, CubeMapFace face, Texture texture, bool saveMipMaps, int width, int height, bool isCompressed, FourCC fourCC, int rgbBitCount)
        {
            int numMip = texture.LevelCount;
            numMip = saveMipMaps ? numMip : 1;

            for (int i = 0; i < numMip; i++)
            {
                int size = MipMapSizeInBytes(i, width, height, isCompressed, fourCC, rgbBitCount);
                byte[] data = mipData;
                if (data == null || data.Length < size)
                {
                    data = new byte[size];
                }

                if (texture is TextureCube)
                {
                    (texture as TextureCube).GetData<byte>(face, i, null, data, 0, size);
                }
                if (texture is Texture2D)
                {
                    (texture as Texture2D).GetData<byte>(i, null, data, 0, size);
                }


#if COLOR_SAVE_TO_ARGB
                if (texture.Format == SurfaceFormat.Color)
                {
                    byte g, b;
                    for (int k = 0; k < size - 3; k += 4)
                    {
                        g = data[k];
                        b = data[k + 2];
                        data[k] = b;
                        data[k + 2] = g;
                    }
                }
#endif

                writer.Write(data, 0, size);
                //for (int j = 0; j < size; j++)
                //{
                //    writer.Write(data[j]);
                //}
                mipData = data;
            }

        }

        //Write texture data to stream if the texture is a 2d texture the face is ignored.
        private static void WriteTexture(BinaryWriter writer, CubeMapFace face, Texture texture, int mipLevel, int depth, int width, int height, bool isCompressed, FourCC fourCC, int rgbBitCount)
        {
            int size = MipMapSizeInBytes(mipLevel, width, height, isCompressed, fourCC, rgbBitCount);
            byte[] data = mipData;
            if (data == null || data.Length < size)
            {
                data = new byte[size];
            }

            if (texture is TextureCube)
            {
                (texture as TextureCube).GetData<byte>(face, mipLevel, null, data, 0, size);
            }
            if (texture is Texture2D)
            {
                (texture as Texture2D).GetData<byte>(mipLevel, null, data, 0, size);
            }
            if (texture is Texture3D)
            {
                Texture3D tex = (texture as Texture3D);
                int localWidth = MipMapSize(mipLevel, width);
                int localHeight = MipMapSize(mipLevel, height);

                tex.GetData<byte>(mipLevel, 0, 0, localWidth, localHeight, depth, depth + 1, data, 0, size);
            }


#if COLOR_SAVE_TO_ARGB
            if (texture.Format == SurfaceFormat.Color)
            {
                byte g, b;
                for (int k = 0; k < size - 3; k += 4)
                {
                    g = data[k];
                    b = data[k + 2];
                    data[k] = b;
                    data[k + 2] = g;
                }
            }
#endif

            writer.Write(data, 0, size);
            //for (int j = 0; j < size; j++)
            //{
            //    writer.Write(data[j]);
            //}
            mipData = data;


        }

        /// <summary>
        /// Save a texture from memory to a stream.
        /// (Supported formats : Dxt1,Dxt3,Dxt5,A8R8G8B8/Color,A4R4G4B4,A1R5G5B5,R5G6B5,A8,
        /// FP32/Single,FP16/HalfSingle,FP32x4/Vector4,FP16x4/HalfVector4,CxV8U8/NormalizedByte2/CxVU,Q8VW8V8U8/NormalizedByte4/8888QWVU
        /// ,HalfVector2/G16R16F/16.16fGR,Vector2/G32R32F,G16R16/RG32/1616GB,A8B8G8R8,A2B10G10R10/Rgba1010102,A16B16G16R16/Rgba64)
        /// </summary>
        /// <param name="stream">The stream where you want to save the texture.</param>
        /// <param name="streamOffset">Offset in stream where you want to save the texture.</param>
        /// <param name="saveMipMaps">Save the complete mip-map chain ?</param>
        /// <param name="texture">The texture that you want to save.</param>
        public static void DDSToStream(Stream stream, int streamOffset, bool saveMipMaps, Texture texture)
        {
            if (stream == null)
            {
                throw new Exception("Can't write to a null stream");
            }

            if (texture == null || texture.IsDisposed)
            {
                throw new Exception("Can't read from a null texture.");
            }

            Texture2D textureAs2D = texture as Texture2D;
            Texture3D textureAs3D = texture as Texture3D;
            TextureCube textureAsCube = texture as TextureCube;

            BinaryWriter writer = new BinaryWriter(stream);

            writer.BaseStream.Seek(streamOffset, SeekOrigin.Begin);

            //Magic number
            //writer.Write('D');
            //writer.Write('D');
            //writer.Write('S');
            //writer.Write(' ');
            writer.Write(DDS_MAGIC);

            //Size of heder
            writer.Write(124);

            //dwHeaderFlags
            int dwHeaderFlags = DDSD_CAPS | DDSD_HEIGHT | DDSD_WIDTH | DDSD_PIXELFORMAT;

            bool isCompressed = IsXNATextureCompressed(texture);

            if (!isCompressed)
            {
                dwHeaderFlags |= DDSD_PITCH;
            }
            else
            {
                dwHeaderFlags |= DDSD_LINEARSIZE;
            }

            if (texture.LevelCount > 1 && saveMipMaps)
            {
                dwHeaderFlags |= DDSD_MIPMAPCOUNT;
            }

            if (textureAs3D != null)
            {
                dwHeaderFlags |= DDSD_DEPTH;
            }

            writer.Write(dwHeaderFlags);

            int Width = 1;
            int Height = 1;

            if (textureAs2D != null)
            {
                Width = textureAs2D.Width;
                Height = textureAs2D.Height;
            }
            if (textureAs3D != null)
            {
                Width = textureAs3D.Width;
                Height = textureAs3D.Height;
            }
            if (textureAsCube != null)
            {
                Width = textureAsCube.Size;
                Height = textureAsCube.Size;
            }


            //dwHeight
            writer.Write(Height);

            //dwWidth
            writer.Write(Width);

            //dwPitchOrLinearSize
            uint dwPitchOrLinearSize = 0;

            if (isCompressed)
            {
                int blockCount = ((Width + 3) / 4) * ((Height + 3) / 4);
                int blockSize = (texture.Format != SurfaceFormat.Dxt1) ? 8 : 0x10;
                dwPitchOrLinearSize = (uint)(blockCount * blockSize);
            }
            else
            {
                dwPitchOrLinearSize = (uint)(Width * XNATextureNumBytesPerPixel(texture));
            }

            writer.Write(dwPitchOrLinearSize);

            //dwDepth
            if (textureAs3D != null)
            {
                writer.Write(textureAs3D.Depth);
            }
            else
            {
                writer.Write(0);
            }

            int dwMipMapCount = texture.LevelCount == 1 ? 0 : (texture.LevelCount);
            if (!saveMipMaps)
            {
                dwMipMapCount = 0;
            }
            writer.Write(dwMipMapCount);

            //dwReserved1[11]
            for (int i = 0; i < 11; i++)
            {
                writer.Write(0);
            }

            uint flags;
            uint fourCC;
            uint rgbBitCount;
            uint rBitMask;
            uint gBitMask;
            uint bBitMask;
            uint aBitMask;

            GenerateDdspf(texture.Format, out flags, out rgbBitCount, out rBitMask, out gBitMask, out bBitMask, out aBitMask, out fourCC);

            //ddspf
            //dwSize
            writer.Write(32);
            //dwFlags
            writer.Write(flags);
            //dwFourCC
            writer.Write(fourCC);
            //dwRGBBitCount
            writer.Write(rgbBitCount);
            //dwRBitMask;
            writer.Write(rBitMask);
            //dwGBitMask;
            writer.Write(gBitMask);
            //dwBBitMask;
            writer.Write(bBitMask);
            //dwABitMask;
            writer.Write(aBitMask);
            //ddspf end

            uint dwSurfaceFlags = DDSCAPS_TEXTURE;
            if ((texture.LevelCount > 1 && saveMipMaps))
            {
                dwSurfaceFlags |= DDSCAPS_MIPMAP;
                dwSurfaceFlags |= DDSCAPS_COMPLEX;
            }
            if (textureAsCube != null || textureAs3D != null)
            {
                dwSurfaceFlags |= DDSCAPS_COMPLEX;
            }
            writer.Write(dwSurfaceFlags);


            uint dwCubemapFlags = 0;
            if (textureAsCube != null)
            {
                dwCubemapFlags |= DDSCAPS2_CUBEMAP;
                dwCubemapFlags |= DDSCAPS2_CUBEMAP_NEGATIVEX;
                dwCubemapFlags |= DDSCAPS2_CUBEMAP_NEGATIVEY;
                dwCubemapFlags |= DDSCAPS2_CUBEMAP_NEGATIVEZ;
                dwCubemapFlags |= DDSCAPS2_CUBEMAP_POSITIVEX;
                dwCubemapFlags |= DDSCAPS2_CUBEMAP_POSITIVEY;
                dwCubemapFlags |= DDSCAPS2_CUBEMAP_POSITIVEZ;
            }

            if (textureAs3D != null)
            {
                dwCubemapFlags |= DDSCAPS2_VOLUME;
            }

            writer.Write(dwCubemapFlags);

            //dwReserved2[3]
            for (int i = 0; i < 3; i++)
            {
                writer.Write(0);
            }
            //standard texture
            if (textureAs2D != null)
            {
                WriteTexture(writer, CubeMapFace.PositiveX, texture, saveMipMaps, Width, Height, isCompressed, (FourCC)fourCC, (int)rgbBitCount);
            }
            //cube texture
            if (textureAsCube != null)
            {
                WriteTexture(writer, CubeMapFace.PositiveX, texture, saveMipMaps, Width, Height, isCompressed, (FourCC)fourCC, (int)rgbBitCount);
                WriteTexture(writer, CubeMapFace.NegativeX, texture, saveMipMaps, Width, Height, isCompressed, (FourCC)fourCC, (int)rgbBitCount);
                WriteTexture(writer, CubeMapFace.PositiveY, texture, saveMipMaps, Width, Height, isCompressed, (FourCC)fourCC, (int)rgbBitCount);
                WriteTexture(writer, CubeMapFace.NegativeY, texture, saveMipMaps, Width, Height, isCompressed, (FourCC)fourCC, (int)rgbBitCount);
                WriteTexture(writer, CubeMapFace.PositiveZ, texture, saveMipMaps, Width, Height, isCompressed, (FourCC)fourCC, (int)rgbBitCount);
                WriteTexture(writer, CubeMapFace.NegativeZ, texture, saveMipMaps, Width, Height, isCompressed, (FourCC)fourCC, (int)rgbBitCount);
            }

            //volume texture
            if (textureAs3D != null)
            {
                for (int i = 0; i < textureAs3D.LevelCount; i++)
                {
                    int availableDepth = MipMapSize(i, textureAs3D.Depth);
                    for (int j = 0; j < availableDepth; j++)
                    {
                        WriteTexture(writer, CubeMapFace.PositiveX, texture, i, j, Width, Height, isCompressed, (FourCC)fourCC, (int)rgbBitCount);
                    }
                }
            }

        }

        /// <summary>
        /// Save a texture from memory to a file.
        /// (Supported formats : Dxt1,Dxt3,Dxt5,A8R8G8B8/Color,A4R4G4B4,A1R5G5B5,R5G6B5,A8,
        /// FP32/Single,FP16/HalfSingle,FP32x4/Vector4,FP16x4/HalfVector4,CxV8U8/NormalizedByte2/CxVU,Q8VW8V8U8/NormalizedByte4/8888QWVU
        /// ,HalfVector2/G16R16F/16.16fGR,Vector2/G32R32F,G16R16/RG32/1616GB,A8B8G8R8,A2B10G10R10/Rgba1010102,A16B16G16R16/Rgba64)
        /// </summary>
        /// <param name="fileName">The name of the file where you want to save the texture.</param>
        /// <param name="saveMipMaps">Save the complete mip-map chain ?</param>
        /// <param name="texture">The texture that you want to save.</param>
        /// <param name="throwExceptionIfFileExist">Throw an exception if the file exists ?</param>
        public static void DDSToFile(string fileName, bool saveMipMaps, Texture texture, bool throwExceptionIfFileExist)
        {
            if (throwExceptionIfFileExist && File.Exists(fileName))
            {
                throw new Exception("The file allready exists and \"throwExceptionIfFileExist\" is true");
            }

            Stream fileStream = null;
            try
            {
                fileStream = File.Create(fileName);
                DDSToStream(fileStream, 0, saveMipMaps, texture);

            }
            catch (Exception x)
            {
                throw x;
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                    fileStream = null;
                }
            }

        }

        /// <summary>
        /// Get the size of the byte array that you should use if you want to get the entier mip-map level using the GetData() function.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="mipMapLevel">The mip-map level.</param>
        public static int GetDataByteSize(Texture2D texture, int mipMapLevel)
        {
            if (!(texture.LevelCount > mipMapLevel))
            {
                return -1;
            }
            return MipMapSizeInBytes(mipMapLevel, texture.Width, texture.Height, IsXNATextureCompressed(texture), XNATextureFourCC(texture), XNATextureColorDepth(texture));
        }

        /// <summary>
        /// Get the size of the byte array that you should use if you want to get the entier mip-map level using the GetData() function.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="mipMapLevel">The mip-map level.</param>
        public static int GetDataByteSize(TextureCube texture, int mipMapLevel)
        {
            if (!(texture.LevelCount > mipMapLevel))
            {
                return -1;
            }
            return MipMapSizeInBytes(mipMapLevel, texture.Size, texture.Size, IsXNATextureCompressed(texture), XNATextureFourCC(texture), XNATextureColorDepth(texture));
        }
    }

	/// <summary>
	/// The brand-spankingly new and revamped DDSReader library.
    /// 
    /// Now with 100% native .NET goodness.
    /// 
    /// This loads a DDS image into an object.  Not much more than that.  At the moment,
    /// it only supports DXT1 compressed images.  It doesn't support uncompressed
    /// images yet.
	/// </summary>
	public class DDSImage
	{
        /*
         * This class is based on parts of DevIL.net, specifically;
         * /DevIL-1.6.8/src-IL/src/il_dds.c
         *
         * All ported to c#/.net.
         * 
         * http://msdn.microsoft.com/library/default.asp?url=/library/en-us/directx9_c/Opaque_and_1_Bit_Alpha_Textures.asp
         */
         
        /// <summary>
        /// A space-seperated list of supported image encoders.
        /// </summary>
        public const String SUPPORTED_ENCODERS = "DXT1 DXT3";

        private static byte[] DDS_HEADER = Convert.FromBase64String("RERTIA=="); // "DDS "
		
		// fourccs
		private const uint FOURCC_DXT1 = 827611204;
		private const uint FOURCC_DXT2 = 844388420;
		private const uint FOURCC_DXT3 = 861165636;
		private const uint FOURCC_DXT4 = 877942852;
		private const uint FOURCC_DXT5 = 894720068;
		private const uint FOURCC_ATI1 = 826889281;
		private const uint FOURCC_ATI2 = 843666497;
		private const uint FOURCC_RXGB = 1111971922;
		private const uint FOURCC_DOLLARNULL = 36;
		private const uint FOURCC_oNULL = 111;
		private const uint FOURCC_pNULL = 112;
		private const uint FOURCC_qNULL = 113;
		private const uint FOURCC_rNULL = 114;
		private const uint FOURCC_sNULL = 115;
		private const uint FOURCC_tNULL = 116;
		
		
		// other defines
		private const uint DDS_LINEARSIZE = 524288;
		private const uint DDS_PITCH = 8;
		private const uint DDS_FOURCC = 4;
		private const uint DDS_LUMINANCE = 131072;
		private const uint DDS_ALPHAPIXELS = 1;
		
		// headers 
		// DDSURFACEDESC2 structure
		private byte[] signature;
		private uint size1;
		private uint flags1;
		private uint height;
		private uint width;
		private uint linearsize;
		private uint depth;
		private uint mipmapcount;
		private uint alphabitdepth;
			// DDPIXELFORMAT structure
			private uint size2;
			private uint flags2;
			private uint fourcc;
			private uint rgbbitcount;
			private uint rbitmask;
			private uint bbitmask;
			private uint gbitmask;
			private uint alphabitmask;
		
			// DDCAPS2 structure
			private uint ddscaps1;
			private uint ddscaps2;
			private uint ddscaps3;
			private uint ddscaps4;
			// end DDCAPS2 structure
		private uint texturestage;
		// end DDSURFACEDESC2 structure
		
		private PixelFormat CompFormat;
		private uint blocksize;
		
		private uint bpp;
		private uint bps;
		private uint sizeofplane;
		private uint compsize;
		private byte[] compdata;
		private byte[] rawidata;
		private BinaryReader br;
		private Bitmap img;

        /// <summary>
        /// Returns a System.Imaging.Bitmap containing the DDS image.
        /// </summary>
        public Bitmap BitmapImage { get { return this.img; } }
		
        /// <summary>
        /// Constructs a new DDSImage object using the given byte array, which
        /// contains the raw DDS file.
        /// </summary>
        /// <param name="ddsimage">A byte[] containing the DDS file.</param>
		public DDSImage(byte[] ddsimage)
		{
			// creates a new DDSImage from a byte[] containing a DDS Image.
			MemoryStream ms = new MemoryStream(ddsimage.Length);
			ms.Write(ddsimage, 0, ddsimage.Length);
			ms.Seek(0, SeekOrigin.Begin);

			br = new BinaryReader(ms);
			this.signature = br.ReadBytes(4);

            if (!IsByteArrayEqual(this.signature, DDS_HEADER))
                throw new Exception("Not a dds image");
			
			//System.Console.WriteLine("Got dds header okay");
			
			// now read in the rest
			this.size1 = br.ReadUInt32();
			this.flags1 = br.ReadUInt32();
			this.height = br.ReadUInt32();
			this.width = br.ReadUInt32();
			this.linearsize = br.ReadUInt32();
			this.depth = br.ReadUInt32();
			this.mipmapcount = br.ReadUInt32();
			this.alphabitdepth = br.ReadUInt32();
			
			// skip next 10 uints
			for (int x=0; x<10; x++) {
				br.ReadUInt32();
			}
			
			this.size2 = br.ReadUInt32();
			this.flags2 = br.ReadUInt32();
			this.fourcc = br.ReadUInt32();
			this.rgbbitcount = br.ReadUInt32();
			this.rbitmask = br.ReadUInt32();
			this.gbitmask = br.ReadUInt32();
            this.bbitmask = br.ReadUInt32();
			this.alphabitmask = br.ReadUInt32();
			this.ddscaps1 = br.ReadUInt32();
			this.ddscaps2 = br.ReadUInt32();
			this.ddscaps3 = br.ReadUInt32();
			this.ddscaps4 = br.ReadUInt32();
			this.texturestage = br.ReadUInt32();
            
			
			// patches for stuff
			if (this.depth == 0) {
				this.depth = 1;
			}

			if ((this.flags2 & DDS_FOURCC) > 0) {
				blocksize = ((this.width+3)/4) * ((this.height+3)/4) * this.depth;
				
				switch (this.fourcc) {
					case FOURCC_DXT1:
						CompFormat = PixelFormat.DXT1;
						blocksize *= 8;
						break;
					
					case FOURCC_DXT2:
						CompFormat = PixelFormat.DXT2;
						blocksize *= 16;
						break;
					
					case FOURCC_DXT3:
						CompFormat = PixelFormat.DXT3;
						blocksize *= 16;
						break;
						
					case FOURCC_DXT4:
						CompFormat = PixelFormat.DXT4;
						blocksize *= 16;
						break;
					
					case FOURCC_DXT5:
						CompFormat = PixelFormat.DTX5;
						blocksize *= 16;
						break;
					
					case FOURCC_ATI1:
						CompFormat = PixelFormat.ATI1N;
						blocksize *= 8;
						break;
					
					case FOURCC_ATI2:
						CompFormat = PixelFormat.THREEDC;
						blocksize *= 16;
						break;
					
					case FOURCC_RXGB:
						CompFormat = PixelFormat.RXGB;
						blocksize *= 16;
						break;
					
					case FOURCC_DOLLARNULL:
						CompFormat = PixelFormat.A16B16G16R16;
						blocksize = this.width * this.height * this.depth * 8;
						break;
					
					case FOURCC_oNULL:
						CompFormat = PixelFormat.R16F;
						blocksize = this.width * this.height * this.depth * 2;
						break;
					
					case FOURCC_pNULL:
						CompFormat = PixelFormat.G16R16F;
						blocksize = this.width * this.height * this.depth * 4;
						break;
						
					case FOURCC_qNULL:
						CompFormat = PixelFormat.A16B16G16R16F;
						blocksize = this.width * this.height * this.depth * 8;
						break;
					
					case FOURCC_rNULL:
						CompFormat = PixelFormat.R32F;
						blocksize = this.width * this.height * this.depth * 4;
						break;
					
					case FOURCC_sNULL:
						CompFormat = PixelFormat.G32R32F;
						blocksize = this.width * this.height * this.depth * 8;
						break;
					
					case FOURCC_tNULL:
						CompFormat = PixelFormat.A32B32G32R32F;
						blocksize = this.width * this.height * this.depth * 16;
						break;
						
					default:
						CompFormat = PixelFormat.UNKNOWN;
						blocksize *= 16;
						break;
				} // switch
			} else {
				// uncompressed image
				if ((this.flags2 & DDS_LUMINANCE) > 0) {
					if ((this.flags2 & DDS_ALPHAPIXELS) > 0) {
						CompFormat = PixelFormat.LUMINANCE_ALPHA;
					} else {
						CompFormat = PixelFormat.LUMINANCE;
					}
				} else {
					if ((this.flags2 & DDS_ALPHAPIXELS) > 0) {
						CompFormat = PixelFormat.ARGB;
					} else {
						CompFormat = PixelFormat.RGB;
					}
				}
				
				blocksize = (this.width * this.height * this.depth * (this.rgbbitcount >> 3));
			}			
			
			if (CompFormat == PixelFormat.UNKNOWN) {
                throw new Exception("Invalid header");
			}
			
			if ((this.flags1 & (DDS_LINEARSIZE | DDS_PITCH))==0
				|| this.linearsize == 0) {
				this.flags1 |= DDS_LINEARSIZE;
				this.linearsize = blocksize;
			}
			
			
			//System.Console.WriteLine(String.Format("Image Size: {0}x{1}, Pixel Format: {2}, Blocksize: {3}", this.width, this.height, this.CompFormat, this.blocksize));
			
			// get image data
			this.ReadData();
			
			//System.Console.WriteLine(String.Format("Compressed data size: {0}/{1} bytes", this.compsize, this.compdata.Length));
			
			// allocate bitmap
			this.bpp = this.PixelFormatToBpp(this.CompFormat);
			this.bps = this.width * this.bpp * this.PixelFormatToBpc(this.CompFormat);
			this.sizeofplane = this.bps * this.height;
			this.rawidata = new byte[this.depth * this.sizeofplane + this.height * this.bps + this.width * this.bpp];
			
			// decompress
			switch (this.CompFormat) {
				case PixelFormat.ARGB:
				case PixelFormat.RGB:
				case PixelFormat.LUMINANCE:
				case PixelFormat.LUMINANCE_ALPHA:
					this.DecompressARGB();
					break;
				
				case PixelFormat.DXT1:
					this.DecompressDXT1();
					break;
					
				case PixelFormat.DXT3:
					this.DecompressDXT3();
					break;
				
				default:
                    throw new Exception("Unknown format");
			}
			
			this.img = new Bitmap((int)this.width, (int)this.height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

#if SAFE_MODE
            // now fill bitmap with raw image datas.  this is really slow.
			// but only on windows/microsoft's .net clr.  it's fast in mono.
            // should find a better way to do this.

            for (int y=0; y<this.height; y++) {
                for (int x=0; x<this.width; x++) {
                    // draw
                    ulong pos = (ulong)(((y*this.width)+x)*4);
                    this.img.SetPixel(x, y, Color.FromArgb(this.rawidata[pos+3], this.rawidata[pos], this.rawidata[pos+1], this.rawidata[pos+2]));
                }
            }
#else
            // new optimised Bitmap creation routine.  Based on Bitmap->Gdk.Pixbuf conversion code
            // thanks to bratsche (Cody Russell) on #mono for the help!

            BitmapData data = this.img.LockBits(new Rectangle(0, 0,
                this.img.Width,
                this.img.Height),
                ImageLockMode.WriteOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            IntPtr scan = data.Scan0;
            int size = this.img.Width * this.img.Height * 4;
            //byte[] bdata = new byte[size];
            
            unsafe
            {
                byte* p = (byte*)scan;
                for (int i = 0; i < size; i += 4)
                {
                    // iterate through bytes.
                    // Bitmap stores it's data in RGBA order.
                    // DDS stores it's data in BGRA order.
                    p[i] = this.rawidata[i + 2]; // blue
                    p[i + 1] = this.rawidata[i + 1]; // green
                    p[i + 2] = this.rawidata[i];   // red
                    p[i + 3] = this.rawidata[i + 3]; // alpha
                }
            }

            this.img.UnlockBits(data);
#endif


            // cleanup
			this.rawidata = null;
			this.compdata = null;
			//this.img.Save("/home/michael/idata.bmp");
			
			
		}
		
		private static bool IsByteArrayEqual(byte[] arg0, byte[] arg1) {
			if (arg0.Length != arg1.Length) {
				return false;
			}
			
			for (int x=0; x<arg0.Length; x++) {
				if (arg0[x] != arg1[x]) {
					return false;
				}
			}
			
			return true;
		}
		
		// iCompFormatToBpp
		private uint PixelFormatToBpp(PixelFormat pf) {
			switch (pf) {
				case PixelFormat.LUMINANCE:
				case PixelFormat.LUMINANCE_ALPHA:
				case PixelFormat.ARGB:
					return this.rgbbitcount/8;
				
				case PixelFormat.RGB:
				case PixelFormat.THREEDC:
				case PixelFormat.RXGB:
					return 3;
				
				case PixelFormat.ATI1N:
					return 1;
				
				case PixelFormat.R16F:
					return 2;
				
				case PixelFormat.A16B16G16R16:
				case PixelFormat.A16B16G16R16F:
				case PixelFormat.G32R32F:
					return 8;
				
				case PixelFormat.A32B32G32R32F:
					return 16;
				
				default:
					return 4;
			}
		}
		
		// iCompFormatToBpc
		private uint PixelFormatToBpc(PixelFormat pf) {
			switch (pf) {
				case PixelFormat.R16F:
				case PixelFormat.G16R16F:
				case PixelFormat.A16B16G16R16F:
					return 4;
				
				case PixelFormat.R32F:
				case PixelFormat.G32R32F:
				case PixelFormat.A32B32G32R32F:
					return 4;
				
				case PixelFormat.A16B16G16R16:
					return 2;
				
				default:
					return 1;
			}
		}
		
		// iCompFormatToChannelCount
		private uint PixelFormatToChannelCount(PixelFormat pf) {
			switch (pf) {
				case PixelFormat.RGB:
				case PixelFormat.THREEDC:
				case PixelFormat.RXGB:
					return 3;
					
				case PixelFormat.LUMINANCE:
				case PixelFormat.R16F:
				case PixelFormat.R32F:
				case PixelFormat.ATI1N:
					return 1;
				
				case PixelFormat.LUMINANCE_ALPHA:
				case PixelFormat.G16R16F:
				case PixelFormat.G32R32F:
					return 2;
				
				default:
					return 4;
			}
		}
		
		private void ReadData() {
			this.compdata = null;
			
			if ((this.flags1 & DDS_LINEARSIZE) > 1) {
				this.compdata = this.br.ReadBytes((int)this.linearsize);
				this.compsize = (uint)this.compdata.Length;
			} else {
				uint bps = this.width * this.rgbbitcount / 8;
				this.compsize = bps * this.height * this.depth;
				this.compdata = new byte[this.compsize];
				
				MemoryStream mem = new MemoryStream((int)this.compsize);
				
				
				byte[] temp;
				for (int z=0; z<this.depth; z++) {
					for (int y=0; y<this.height; y++) {
						temp = this.br.ReadBytes((int)this.bps);
						mem.Write(temp, 0, temp.Length);
					}
				}
				mem.Seek(0, SeekOrigin.Begin);
				
				mem.Read(this.compdata, 0, this.compdata.Length);
				mem.Close();
			}
		}
		
		private void DecompressARGB() {
			// not done
            throw new Exception("Unknown format");
		}
		
		private void DecompressDXT1() {
			// DXT1 decompressor
			Colour8888[] colours = new Colour8888[4];
			
			
			ushort colour0, colour1;
			uint bitmask, offset;
			int i, j, k, x, y, z, Select;
			
			MemoryStream mem = new MemoryStream(this.compdata.Length);
			mem.Write(this.compdata, 0, this.compdata.Length);
			mem.Seek(0, SeekOrigin.Begin);
			BinaryReader r = new BinaryReader(mem);
			
			colours[0].a = 255;
			colours[1].a = 255;
			colours[2].a = 255;
			
			for (z=0; z<this.depth; z++) {
				for (y=0; y<this.height; y+=4) {
					for (x=0; x<this.width; x+=4) {
						colour0 = r.ReadUInt16();
						colour1 = r.ReadUInt16();
						
						this.ReadColour(colour0, ref colours[0]);
						this.ReadColour(colour1, ref colours[1]);
						
						bitmask = r.ReadUInt32();
						
						if (colour0 > colour1) {
							// Four-color block: derive the other two colors.
							// 00 = color_0, 01 = color_1, 10 = color_2, 11 = color_3
							// These 2-bit codes correspond to the 2-bit fields
							// stored in the 64-bit block.
							colours[2].b = (byte)((2 * colours[0].b + colours[1].b + 1) / 3);
							colours[2].g = (byte)((2 * colours[0].g + colours[1].g + 1) / 3);
							colours[2].r = (byte)((2 * colours[0].r + colours[1].r + 1) / 3);
							
							colours[3].b = (byte)((colours[0].b + 2 * colours[1].b + 1) / 3);
							colours[3].g = (byte)((colours[0].g + 2 * colours[1].g + 1) / 3);
							colours[3].r = (byte)((colours[0].r + 2 * colours[1].r + 1) / 3);
							colours[3].a = 0xFF;
						} else {
							// Three-color block: derive the other color.
							// 00 = color_0,  01 = color_1,  10 = color_2,
							// 11 = transparent.
							// These 2-bit codes correspond to the 2-bit fields 
							// stored in the 64-bit block. 
							colours[2].b = (byte)((colours[0].b + colours[1].b) / 2);
							colours[2].g = (byte)((colours[0].g + colours[1].g) / 2);
							colours[2].r = (byte)((colours[0].r + colours[1].r) / 2);

                            colours[3].b = 0;
                            colours[3].g = 0;
                            colours[3].r = 0;
							colours[3].a = 0;
						}
						
                        
						for (j = 0, k = 0; j < 4; j++) {
							for (i = 0; i < 4; i++, k++) {
								Select = (int)((bitmask & (0x03 << k*2)) >> k*2);
								if (((x + i) < this.width) && ((y + j) < this.height)) {
									offset = (uint)(z * this.sizeofplane + (y + j) * this.bps + (x + i) * this.bpp);
									this.rawidata[offset] = (byte)colours[Select].r;
									this.rawidata[offset + 1] = (byte)colours[Select].g;
									this.rawidata[offset + 2] = (byte)colours[Select].b;
									this.rawidata[offset + 3] = (byte)colours[Select].a;
								}
							}
						}

                      
						
					}
				}
			}
			
		}
		
		private void DecompressDXT3() {
			Colour8888[] colours = new Colour8888[4];
			uint bitmask, offset;
			int i, j, k, x, y, z, Select;
			ushort word, colour0, colour1;
			byte[] alpha; //temp;
			
			MemoryStream mem = new MemoryStream(this.compdata.Length);
			mem.Write(this.compdata, 0, this.compdata.Length);
			mem.Seek(0, SeekOrigin.Begin);
			BinaryReader r = new BinaryReader(mem);
			
			for (z=0; z<this.depth; z++) {
				for (y=0; y<this.height; y+=4) {
					for (x=0; x<this.width; x+=4) {
						alpha = r.ReadBytes(8);
						
						colour0 = r.ReadUInt16();
						colour1 = r.ReadUInt16();
						this.ReadColour(colour0, ref colours[0]);
						this.ReadColour(colour1, ref colours[1]);
						
						bitmask = r.ReadUInt32();
						
						colours[2].b = (byte)((2 * colours[0].b + colours[1].b + 1) / 3);
						colours[2].g = (byte)((2 * colours[0].g + colours[1].g + 1) / 3);
						colours[2].r = (byte)((2 * colours[0].r + colours[1].r + 1) / 3);
							
						colours[3].b = (byte)((colours[0].b + 2 * colours[1].b + 1) / 3);
						colours[3].g = (byte)((colours[0].g + 2 * colours[1].g + 1) / 3);
						colours[3].r = (byte)((colours[0].r + 2 * colours[1].r + 1) / 3);
						
						for (j = 0, k = 0; j < 4; j++) {
							for (i = 0; i < 4; k++, i++) {
								Select = (int)((bitmask & (0x03 << k*2)) >> k*2);
								
								if (((x + i) < this.width) && ((y + j) < this.height)) {
									offset = (uint)(z * this.sizeofplane + (y + j) * this.bps + (x + i) * this.bpp);
									this.rawidata[offset] = (byte)colours[Select].r;
									this.rawidata[offset + 1] = (byte)colours[Select].g;
									this.rawidata[offset + 2] = (byte)colours[Select].b;
								}
							}
						}
						
						for (j = 0; j < 4; j++) {
							word = (ushort)(alpha[2*j] + 256*alpha[2*j+1]);
							for (i = 0; i < 4; i++) {
								if (((x + i) < this.width) && ((y + j) < this.height)) {
									offset = (uint)(z * this.sizeofplane + (y + j) * this.bps + (x + i) * this.bpp + 3);
									this.rawidata[offset] = (byte)(word & 0x0F);
									this.rawidata[offset] = (byte)(this.rawidata[offset] | (this.rawidata[offset] << 4));
								}
								word >>= 4;
							}
						}		
					}
				}
			}					
		}
		
		private void ReadColour(ushort Data, ref Colour8888 op)
		{
			byte r, g, b;

			b = (byte)(Data & 0x1f);
			g = (byte)((Data & 0x7E0) >> 5);
			r = (byte)((Data & 0xF800) >> 11);

            op.r = (byte)(r * 255 / 31);
            op.g = (byte)(g * 255 / 63);
            op.b = (byte)(b * 255 / 31);
		}

        struct Colour8888
        {
            public byte r;
            public byte g;
            public byte b;
            public byte a;
        }

        enum PixelFormat
        {
            /// <summary>
            /// 32-bit image, with 8-bit red, green, blue and alpha.
            /// </summary>
            ARGB,
            /// <summary>
            /// 24-bit image with 8-bit red, green, blue.
            /// </summary>
            RGB,
            /// <summary>
            /// 16-bit DXT-1 compression, 1-bit alpha.
            /// </summary>
            DXT1,
            /// <summary>
            /// DXT-2 Compression
            /// </summary>
            DXT2,
            /// <summary>
            /// DXT-3 Compression
            /// </summary>
            DXT3,
            /// <summary>
            /// DXT-4 Compression
            /// </summary>
            DXT4,
            /// <summary>
            /// DXT-5 Compression
            /// </summary>
            DTX5,
            /// <summary>
            /// 3DC Compression
            /// </summary>
            THREEDC,
            /// <summary>
            /// ATI1n Compression
            /// </summary>
            ATI1N,
            LUMINANCE,
            LUMINANCE_ALPHA,
            RXGB,
            A16B16G16R16,
            R16F,
            G16R16F,
            A16B16G16R16F,
            R32F,
            G32R32F,
            A32B32G32R32F,
            /// <summary>
            /// Unknown pixel format.
            /// </summary>
            UNKNOWN
        }

	}
}
