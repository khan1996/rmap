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

namespace rMap.Zalla
{
    [Serializable]
    public class Scene
    {
        [Private]
        private const int RowCryptoStart = 256;

        public string Definition;
        public int TextureZone;
        [XmlIgnore]
        public Bitmap TextureThumbnail;

        public int TextureSize = 256;

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

        public void GetHeightMapTileBoundarys(out int minX, out int minY, out int maxX, out int maxY)
        {
            minX = HeightTables.Min(t => t.X);
            minY = HeightTables.Min(t => t.Y);

            maxX = HeightTables.Max(t => t.X);
            maxY = HeightTables.Max(t => t.Y);
        }

        public float[,] GetHeights(Rectangle rect)
        {
            return GetHeights(rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
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

        public float GetNatureHeight(NatureObject obj)
        {
            HeightTable heights = HeightTables.SingleOrDefault(x => x.X == obj.TileX && x.Y == obj.TileY);

            if (heights == null)
                return 0;

            return heights.Table[obj.PosX, 65 - obj.PosY - 1];
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
            if (crypto != null && crypto.Length > 0)
                BinaryTools.XorCounter(data, data, RowCryptoStart, RowCryptoStart, data.Length - RowCryptoStart, crypto, 0, crypto.Length);

            return Load(data, zone, loader);
        }

        public byte[] SaveWithCrypto(byte[] crypto, ISceneLoader loader)
        {
            byte[] data = Save(loader);

            if (crypto != null && crypto.Length > 0)
                BinaryTools.XorCounter(data, data, RowCryptoStart, RowCryptoStart, data.Length - RowCryptoStart, crypto, 0, crypto.Length, true);

            return data;
        }

        public static Scene Load(byte[] data, int zone, ISceneLoader loader)
        {
            if (loader == null)
                return null;

            Scene s = DeSerializeFromXML(loader.Load(data, zone));

            s.TextureSize = rMap.Properties.Settings.Default.TextureSize;

            return s;
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

        public byte[] Save(ISceneLoader loader)
        {
            if (loader == null)
                return null;

            return loader.Save(SerializeToXML());
        }

        public override string ToString()
        {
            return Definition;
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

        public string GetTexFolder(string rylFolder)
        {
            return Path.Combine(rylFolder, @"texture\widetexture\zone" + TextureZone);
        }

        public Bitmap ExportFullTextureMap(string texturePath, bool forGround = false)
        {
            if (Textures.Count < 1)
                return null;

            int minX = forGround ? HeightTables.Min(x => x.X) : 1;
            int minY = forGround ? HeightTables.Min(x => x.Y) : 1;
            int maxX = forGround ? HeightTables.Max(x => x.X) : Textures.Max(t => t.X);
            int maxY = forGround ? HeightTables.Max(x => x.Y) : Textures.Max(t => t.Y);

            int nX = maxX - minX + 1;
            int nY = maxY - minY + 1;

            Bitmap all = new Bitmap(TextureSize * nX, TextureSize * nY, PixelFormat.Format24bppRgb);

            BitmapData alldata = all.LockBits(new Rectangle(0, 0, all.Width, all.Height),
                 ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            try
            {
                for (int y = minY; y <= maxY; y++)
                {
                    for (int x = minX; x <= maxX; x++)
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

                                        int startX = (x - minX) * TextureSize;
                                        int startY = all.Height - (y - minY) * TextureSize - 1;

                                        for (int py = 0; py < bmp.Height; py++)
                                        {
                                            for (int px = 0; px < bmp.Height; px++)
                                            {
                                                int allpos = (startY - py) * allWidth + (startX + px) * 3;
                                                allp[allpos++] = p[py * pWidth + px * 3]; // b
                                                allp[allpos++] = p[py * pWidth + px * 3 + 1]; // g
                                                allp[allpos] = p[py * pWidth + px * 3 + 2]; // r
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

        public byte[] SerializeToXML()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer ser = new XmlSerializer(this.GetType());
                ser.Serialize(ms, this);

                return ms.ToArray();
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
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class PrivateAttribute : Attribute
    {
    }
}
