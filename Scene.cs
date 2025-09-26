using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Drawing;
using System.Xml.Serialization;
using System.Drawing.Imaging;

namespace Zalla3dScene
{
    [Serializable]
    public class Scene : MarshalByRefObject
    {
        [Private]
        private const int RowCryptoStart = 256;

        public string Definition;
        public int TextureZone;
        [XmlIgnore]
        public Bitmap TextureThumbnail;

        [Private]
        public List<WaterData> WaterDatas = new List<WaterData>();
        [Private]
        public List<TileTexture> Textures = new List<TileTexture>();
        [Private]
        public List<FallMap> FallMaps = new List<FallMap>();
        [Private]
        public List<House> Houses = new List<House>();
        [Private]
        public List<NatureObject> NatureObjects = new List<NatureObject>();
        [Private]
        public List<FieldObject> FieldObjects = new List<FieldObject>();
        [Private]
        public List<ObjectGroup> ObjectGroups = new List<ObjectGroup>();
        [Private]
        public List<Effect> Effects = new List<Effect>();
        [Private]
        public List<LandscapeEffect> LandscapeEffects = new List<LandscapeEffect>();
        [Private]
        public List<SectorLight> SectorLights = new List<SectorLight>();
        [Private]
        public List<HeightTable> HeightTables = new List<HeightTable>();

        public Multivalue<int, int, int, int> GetHeightMapTileBoundarys()
        {
            int minX = HeightTables.Min(t => t.X);
            int minY = HeightTables.Min(t => t.Y);

            int maxX = HeightTables.Max(t => t.X);
            int maxY = HeightTables.Max(t => t.Y);

            return new Multivalue<int, int, int, int>()
            {
                value1 = minX,
                value2 = minY,
                value3 = maxX,
                value4 = maxY
            };
        }

        public float[,] GetHeights(int minX, int minY, int maxX, int maxY)
        {
            int tilesX = maxX - minX + 1; // 12 - -1 = 13
            int tilesY = maxY - minY + 1;

            float[,] heights = new float[64 * tilesX + 1, 64 * tilesY + 1];

            foreach (HeightTable ht in HeightTables)
            {
                if (ht.X >= minX && ht.X <= maxX && ht.Y >= minY && ht.Y <= maxY)
                {
                    int tx = ht.X - minX;
                    int ty = ht.Y - minY;
                    for (int x = 0; x < 65; x++)
                    {
                        for (int y = 0; y < 65; y++)
                        {
                            // 0 -> [64] -> [128] ->
                            heights[tx * 64 + x, (tilesY - ty - 1) * 64 + y] = ht.Table[x, y];
                        }
                    }
                }
            }

            return heights;
        }

        public void SetHeights(float[,] map, int minX, int minY, int maxX, int maxY)
        {
            int tilesX = maxX - minX + 1; // 12 - -1 = 13
            int tilesY = maxY - minY + 1;

            HeightTables.Clear();

            for (int tiy = minY; tiy <= maxY; tiy++ )
            {
                for (int tix = minX; tix <= maxX; tix++)
                {
                    HeightTable ht = new HeightTable()
                    {
                        X = tix,
                        Y = tiy
                    };

                    int tx = ht.X - minX;
                    int ty = ht.Y - minY;

                    // 0,0 contains info for tile 0:maxy
                    // so the data for tile min,min is at 0,(tilesy - min -1)*64
                    for (int x = 0; x < 65; x++)
                    {
                        for (int y = 0; y < 65; y++)
                        {
                            // 0 -> [64] -> [128] ->
                            ht.Table[x,y] = map[tx * 64 + x, (tilesY - ty - 1) * 64 + y];
                        }
                    }
                    HeightTables.Add(ht);
                }
            }
        }

        public static Scene LoadWithCrypto(byte[] data, int zone, byte[] crypto, ISceneLoader loader)
        {
            int counter = 0;

            for (int pos = 256; pos < data.Length; pos++)
            {
                if (counter >= crypto.Length)
                    counter = 0;

                data[pos] = (byte)((byte)(data[pos] - counter) ^ crypto[counter++]);
            }

            return Load(data, zone, loader);
        }

        public byte[] SaveWithCrypto(byte[] crypto, ISceneLoader loader)
        {
            byte[] data = Save(loader);

            int counter = 0;

            for (int pos = 256; pos < data.Length; pos++)
            {
                if (counter >= crypto.Length)
                    counter = 0;

                data[pos] = (byte)((byte)(data[pos] ^ crypto[counter]) + counter++);
            }

            return data;
        }

        public static Scene Load(byte[] data, int zone, ISceneLoader loader)
        {
            if (loader == null)
                return null;

            Scene s = DeSerializeFromXML(loader.Load(data, zone));
            return s;
        }

        public string GetTexFolder(string rylFolder)
        {
            return Path.Combine(rylFolder, @"texture\widetexture\zone" + TextureZone);
        }

        public Bitmap ExportHeightmapTextured(string texturePath)
        {
            int maxX = Textures.Max(t => t.X);
            int maxY = Textures.Max(t => t.Y);

            float[,] map = GetHeights(0, 0, maxX, maxY);
            float low = GetLow(map);
            float step = GetStep(map);
            Color line = Color.FromArgb(200, 180, 120, 60);

            using (Bitmap bmp = new Bitmap(map.GetUpperBound(0) + 1, map.GetUpperBound(1) + 1))
            {
                for (int y = 0; y <= map.GetUpperBound(1); y++)
                {
                    for (int x = 0; x <= map.GetUpperBound(0); x++)
                    {
                        byte c = (byte)Math.Round((map[x, y] - low) / step);

                        if (c > 0 && c % 5 == 0)
                            bmp.SetPixel(x, y, line);
                        else
                            bmp.SetPixel(x, y, Color.Transparent);
                    }
                }

                Bitmap tex = ExportFullTextureMap(texturePath);
                Graphics gr = Graphics.FromImage(tex);

                gr.DrawImage(bmp, 0, 0, tex.Width, tex.Height);
                gr.Flush();

                return tex;
            }


        }

        private static float GetLow(float[,] map)
        {
            float low = map[0, 0];
            for (int y = 0; y <= map.GetUpperBound(1); y++)
            {
                for (int x = 0; x <= map.GetUpperBound(0); x++)
                {
                    if (map[x, y] < low)
                        low = map[x, y];
                }
            }

            return low;
        }

        private static float GetStep(float[,] map)
        {
            float low = map[0, 0];
            float high = map[0, 0];

            for (int y = 0; y <= map.GetUpperBound(1); y++)
            {
                for (int x = 0; x <= map.GetUpperBound(0); x++)
                {
                    if (map[x, y] > high)
                        high = map[x, y];
                    if (map[x, y] < low)
                        low = map[x, y];
                }
            }

            return (high - low) / 255f;
        }

        public Bitmap GetTileTexture(string rylFolder, int x, int y)
        {
            TileTexture tt = Textures.SingleOrDefault(t => t.X == x && t.Y == y);

            byte[] da = File.ReadAllBytes(Path.Combine(GetTexFolder(rylFolder), tt.Texture1));
            da[0] = (byte)'D';
            da[1] = (byte)'D';
            da[2] = (byte)'S';
            da[3] = (byte)' ';

            Bitmap bmp = Tao.DDSLoader.DDSDataToBMP(da);
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bmp;
        }

        [XmlElement("TextureThumbnail"), System.ComponentModel.Browsable(false)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)] 
        public string TextureThumbnailSerialization
        {
            get
            {
                if (TextureThumbnail == null)
                    return "";

                using (MemoryStream stream = new MemoryStream())
                {
                    Bitmap bm = new Bitmap(TextureThumbnail); // please don't ask... 
                    bm.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                    bm.Dispose();
                    return Convert.ToBase64String(stream.ToArray());
                }

            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(value)))
                {
                    TextureThumbnail = Bitmap.FromStream(stream) as Bitmap;
                }
            }
        }

        /// <summary>
        /// 26 seconds on intel core 2 with zone 1 with old code
        /// 4 seconds with new unsafe code
        /// </summary>
        /// <param name="rylFolder"></param>
        public void SetThumbnail(string rylFolder)
        {
            if (Textures.Count < 1)
                TextureThumbnail = null;
            else
            {
                //System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + " 1");
                Bitmap full = ExportFullTextureMap(GetTexFolder(rylFolder));
                //System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + " 2");
                int X = Textures.Max(t => t.X);
                int Y = Textures.Max(t => t.Y);

                TextureThumbnail = new Bitmap(full, new Size(X * 25, Y * 25));
                full.Dispose();
                //System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + " 3");
            }
        }

        public Bitmap ExportFullTextureMap(string texturePath)
        {
            if (Textures.Count < 1)
                return null;

            int maxX = Textures.Max(t => t.X);
            int maxY = Textures.Max(t => t.Y);

            Bitmap all = new Bitmap(256 * maxX, 256 * maxY, PixelFormat.Format24bppRgb);

            BitmapData alldata = all.LockBits(new Rectangle(0, 0, all.Width, all.Height),
                 ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            try
            {
                for (int y = 1; y <= maxY; y++)
                {
                    for (int x = 1; x <= maxX; x++)
                    {
                        TileTexture tt = Textures.SingleOrDefault(t => t.X == x && t.Y == y);

                        if (tt != null)
                        {
                            if (File.Exists(Path.Combine(texturePath, tt.Texture1)))
                            {
                                byte[] da = File.ReadAllBytes(Path.Combine(texturePath, tt.Texture1));
                                da[0] = (byte)'D';
                                da[1] = (byte)'D';
                                da[2] = (byte)'S';
                                da[3] = (byte)' ';

                                using (Bitmap bmp = Tao.DDSLoader.DDSDataToBMP(da))
                                {
                                    BitmapData bdt = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                                         ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                                    unsafe
                                    {
                                        byte* p = (byte*)(void*)bdt.Scan0;
                                        byte* allp = (byte*)(void*)alldata.Scan0;

                                        if (bdt.Stride != bmp.Width * 3 || alldata.Stride != all.Width * 3)
                                            throw new Exception("Pixel width isnt 3 bytes");

                                        int allWidth = alldata.Stride;
                                        int pWidth = bdt.Stride;

                                        int startX = (x - 1) * 256;
                                        int startY = all.Height - (y - 1) * 256 - 1;

                                        for (int py = 0; py < bmp.Height; py++)
                                        {
                                            for (int px = 0; px < bmp.Height; px++)
                                            {
                                                int allpos = (startY - py) * allWidth + (startX + px) * 3;
                                                allp[allpos++] = p[py * pWidth + px * 3]; // b
                                                allp[allpos++] = p[py * pWidth + px * 3 + 1]; // g
                                                allp[allpos] = p[py * pWidth + px * 3 + 2]; // r

                                                //all.SetPixel((x - 1) * 256 + px, all.Height - ((y - 1) * 256 + py) - 1, bmp.GetPixel(px, py));
                                            }
                                        }
                                    }

                                    bmp.UnlockBits(bdt);
                                }
                            }
                        }
                    }
                }
                all.UnlockBits(alldata);

                return all;
            }
            catch
            {
                all.UnlockBits(alldata);
                if (all != null)
                    all.Dispose();

                throw;
            }
        }

        public byte[] Save(ISceneLoader loader)
        {
            if (loader == null)
                return null;

            return loader.Save(SerializeToXML());
        }

        public byte[] Serialize()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (DeflateStream df = new DeflateStream(ms, CompressionMode.Compress, true))
                {
                    XmlSerializer ser = new XmlSerializer(this.GetType());
                    ser.Serialize(df, this);
                }
                return ms.ToArray();
            }
        }

        public byte[] SerializeToXML()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer ser = new XmlSerializer(this.GetType());
                ser.Serialize(ms, this);

                return ms.ToArray();
            }
        }

        public static Scene DeSerialize(byte[] serData)
        {
            using (MemoryStream ms = new MemoryStream(serData))
            {
                using (DeflateStream df = new DeflateStream(ms, CompressionMode.Decompress))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(Scene));
                    return ser.Deserialize(df) as Scene;
                }
            }
        }

        public static Scene DeSerializeFromXML(byte[] serData)
        {
            using (MemoryStream ms = new MemoryStream(serData))
            {
                XmlSerializer ser = new XmlSerializer(typeof(Scene));
                return ser.Deserialize(ms) as Scene;
            }
        }

        public override string ToString()
        {
            return Definition;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class PrivateAttribute : Attribute
    {
    }
}
