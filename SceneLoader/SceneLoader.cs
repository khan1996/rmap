using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace rMap.Zalla
{
    public class SceneLoader : MarshalByRefObject, ISceneLoader
    {
        private Scene scene;

        public byte[] Load(byte[] data, int zone)
        {
            scene = new Scene();

            using (MemoryStream ms = new MemoryStream(data))
            {
                BinaryReader br = new BinaryReader(ms);
                scene.Definition = ReadString(br, 256);
                scene.TextureZone = zone;

                // zone 100 - 2 0 0 0, houses = 2, effects = 12
                //                            z8    row z4
                uint c1 = br.ReadUInt32(); // 1248, 221
                uint c2 = br.ReadUInt32(); // 16,   0
                uint c3 = br.ReadUInt32(); // 3,    0
                uint c4 = br.ReadUInt32(); // 748,  1647

                LoadHightables(br, br.ReadUInt32());
                LoadTileProperties(br, br.ReadUInt32());
                LoadTileTextures(br, br.ReadUInt32());
                LoadFallmaps(br, br.ReadUInt32());
                LoadHouses(br, br.ReadUInt32());
                LoadObjectGroups(br, br.ReadUInt32());
                LoadNatureObjects(br, br.ReadUInt32());
                LoadObjects(br, br.ReadUInt32());
                LoadEffects(br, br.ReadUInt32());
                LoadGemDatas(br, br.ReadUInt32());

                if (ms.Position < ms.Length)
                    LoadEffect2s(br, br.ReadUInt32());

                if (ms.Position != ms.Length)
                    throw new Exception("Too much data");
            }

            return scene.SerializeToXML();
        }

        public byte[] Save(byte[] p_scene)
        {
            scene = Scene.DeSerializeFromXML(p_scene);

            using (MemoryStream ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);

                Write(bw, scene.Definition, 256);

                bw.Write((uint)0);
                bw.Write((uint)0);
                bw.Write((uint)0);
                bw.Write((uint)0);

                SaveHeightTables(bw);
                SaveTileProperties(bw);
                SaveTileTextures(bw);
                SaveFallMaps(bw);
                SaveHouses(bw);
                SaveObjectGroups(bw);
                SaveNatureObjects(bw);
                SaveObjects(bw);
                SaveEffects(bw);
                SaveGemDatas(bw);
                SaveEffect2s(bw);

                return ms.ToArray();
            }
        }

        #region Save Parts

        private void SaveHeightTables(BinaryWriter bw)
        {
            bw.Write((uint)scene.HeightTables.Count);
            foreach (HeightTable table in scene.HeightTables)
                table.SaveTo(bw);
        }

        private void SaveTileProperties(BinaryWriter bw)
        {
            bw.Write((uint)scene.WaterDatas.Count);
            foreach (WaterData tile in scene.WaterDatas)
                tile.SaveTo(bw);
        }

        private void SaveTileTextures(BinaryWriter bw)
        {
            bw.Write((uint)scene.Textures.Count);
            foreach (TileTexture tile in scene.Textures)
                tile.SaveTo(bw);
        }

        private void SaveFallMaps(BinaryWriter bw)
        {
            bw.Write((uint)scene.FallMaps.Count);
            foreach (FallMap fm in scene.FallMaps)
                fm.SaveTo(bw);
        }

        private void SaveHouses(BinaryWriter bw)
        {
            IEnumerable<IGrouping<Point, House>> dist = scene.Houses.GroupBy(h => new Point(h.TileX, h.TileY)).OrderBy(ig => ig.Key);

            bw.Write((uint)dist.Count());

            foreach (IGrouping<Point, House> to in dist)
            {
                bw.Write((int)to.Key.X);
                bw.Write((int)to.Key.Y);
                bw.Write((uint)to.Count());

                foreach (House h in to)
                    h.SaveTo(bw);
            }
        }

        private void SaveObjectGroups(BinaryWriter bw)
        {
            bw.Write((uint)scene.ObjectGroups.Count);
            foreach (ObjectGroup group in scene.ObjectGroups)
                group.SaveTo(bw);
        }

        private void SaveNatureObjects(BinaryWriter bw)
        {
            IEnumerable<IGrouping<Point, NatureObject>> dist = scene.NatureObjects.GroupBy(h => new Point(h.TileX, h.TileY)).OrderBy(ig => ig.Key);

            bw.Write((uint)dist.Count());

            foreach (IGrouping<Point, NatureObject> to in dist)
            {
                bw.Write((int)to.Key.X);
                bw.Write((int)to.Key.Y);
                bw.Write((uint)to.Count());

                foreach (NatureObject h in to)
                    h.SaveTo(bw);
            }
        }

        private void SaveObjects(BinaryWriter bw)
        {
            IEnumerable<IGrouping<Point, FieldObject>> dist = scene.FieldObjects.GroupBy(h => new Point(h.TileX, h.TileY)).OrderBy(ig => ig.Key);

            bw.Write((uint)dist.Count());

            foreach (IGrouping<Point, FieldObject> to in dist)
            {
                bw.Write((int)to.Key.X);
                bw.Write((int)to.Key.Y);
                bw.Write((uint)to.Count());

                foreach (FieldObject h in to)
                    h.SaveTo(bw);
            }
        }

        private void SaveEffects(BinaryWriter bw)
        {
            IEnumerable<IGrouping<Point, Effect>> dist = scene.Effects.GroupBy(h => new Point(h.TileX, h.TileY)).OrderBy(ig => ig.Key);

            bw.Write((uint)dist.Count());

            foreach (IGrouping<Point, Effect> to in dist)
            {
                bw.Write((int)to.Key.X);
                bw.Write((int)to.Key.Y);
                bw.Write((uint)to.Count());

                foreach (Effect h in to)
                    h.SaveTo(bw);
            }
        }

        private void SaveGemDatas(BinaryWriter bw)
        {
            IEnumerable<IGrouping<Point, LandscapeEffect>> dist = scene.LandscapeEffects.GroupBy(h => new Point(h.TileX, h.TileY)).OrderBy(ig => ig.Key);

            bw.Write((uint)dist.Count());

            foreach (IGrouping<Point, LandscapeEffect> to in dist)
            {
                bw.Write((int)to.Key.X);
                bw.Write((int)to.Key.Y);
                bw.Write((uint)to.Count());

                foreach (LandscapeEffect h in to)
                    h.SaveTo(bw);
            }
        }

        private void SaveEffect2s(BinaryWriter bw)
        {
            IEnumerable<IGrouping<Point, SectorLight>> dist = scene.SectorLights.GroupBy(h => new Point(h.TileX, h.TileY)).OrderBy(ig => ig.Key);

            bw.Write((uint)dist.Count());

            foreach (IGrouping<Point, SectorLight> to in dist)
            {
                bw.Write((int)to.Key.X);
                bw.Write((int)to.Key.Y);
                bw.Write((uint)to.Count());

                foreach (SectorLight h in to)
                    h.SaveTo(bw);
            }
        }

        #endregion

        #region Load parts

        private void LoadHightables(BinaryReader br, uint count)
        {
            for (int hTable = 0; hTable < count; hTable++)
            {
                HeightTable ht = HeightTable.LoadFrom(br);
                ht.Scene = scene;
                scene.HeightTables.Add(ht);
            }
        }

        private void LoadTileProperties(BinaryReader br, uint count)
        {
            for (int hTable = 0; hTable < count; hTable++)
            {
                WaterData tp = WaterData.LoadFrom(br);
                tp.Scene = scene;
                scene.WaterDatas.Add(tp);
            }
        }

        private void LoadTileTextures(BinaryReader br, uint count)
        {
            for (int hTable = 0; hTable < count; hTable++)
            {
                TileTexture tx = TileTexture.LoadFrom(br);
                if (tx != null)
                {
                    tx.Scene = scene;
                    scene.Textures.Add(tx);
                }
            }
        }

        private void LoadFallmaps(BinaryReader br, uint count)
        {
            for (int hTable = 0; hTable < count; hTable++)
            {
                FallMap fm = FallMap.LoadFrom(br, scene);
                fm.Scene = scene;
                scene.FallMaps.Add(fm);
            }
        }

        private void LoadHouses(BinaryReader br, uint count)
        {
            for (int hTable = 0; hTable < count; hTable++)
            {
                br.ReadInt32(); // tile x
                br.ReadInt32(); // tile y

                uint cObjects = br.ReadUInt32();

                for (int i = 0; i < cObjects; i++)
                {
                    House h = House.LoadFrom(br);
                    h.Scene = scene;
                    scene.Houses.Add(h);
                }
            }
        }

        private void LoadNatureObjects(BinaryReader br, uint count)
        {
            for (int hTable = 0; hTable < count; hTable++)
            {
                int tilex = br.ReadInt32();
                int tiley = br.ReadInt32();

                uint cObjects = br.ReadUInt32();

                for (int i = 0; i < cObjects; i++)
                {
                    NatureObject no = NatureObject.LoadFrom(br, tilex, tiley);
                    no.Scene = scene;
                    scene.NatureObjects.Add(no);
                }
            }
        }

        private void LoadObjects(BinaryReader br, uint count)
        {
            for (int hTable = 0; hTable < count; hTable++)
            {
                br.ReadInt32(); // tile x
                br.ReadInt32(); // tile y

                uint cObjects = br.ReadUInt32();

                for (int i = 0; i < cObjects; i++)
                {
                    FieldObject fo = FieldObject.LoadFrom(br);
                    fo.Scene = scene;
                    scene.FieldObjects.Add(fo);
                }
            }
        }

        private void LoadObjectGroups(BinaryReader br, uint count)
        {
            for (uint sec = 0; sec < count; sec++)
            {
                ObjectGroup og = ObjectGroup.LoadFrom(br, scene);
                og.Scene = scene;
                scene.ObjectGroups.Add(og);
            }
        }

        private void LoadEffects(BinaryReader br, uint count)
        {
            for (int hTable = 0; hTable < count; hTable++)
            {
                br.ReadInt32(); // tile x
                br.ReadInt32(); // tile y

                uint cObjects = br.ReadUInt32();

                for (int i = 0; i < cObjects; i++)
                {
                    Effect e = Effect.LoadFrom(br);
                    e.Scene = scene;
                    scene.Effects.Add(e);
                }
            }
        }

        private void LoadGemDatas(BinaryReader br, uint count)
        {
            for (int hTable = 0; hTable < count; hTable++)
            {
                br.ReadInt32(); // tile x
                br.ReadInt32(); // tile y

                uint cObjects = br.ReadUInt32();

                for (int i = 0; i < cObjects; i++)
                {
                    LandscapeEffect gd = LandscapeEffect.LoadFrom(br);
                    gd.Scene = scene;
                    scene.LandscapeEffects.Add(gd);
                }
            }

        }
        private void LoadEffect2s(BinaryReader br, uint count)
        {
            for (int hTable = 0; hTable < count; hTable++)
            {
                br.ReadInt32(); // tile x
                br.ReadInt32(); // tile y

                uint cObjects = br.ReadUInt32();

                for (int i = 0; i < cObjects; i++)
                {
                    SectorLight ef2 = SectorLight.LoadFrom(br);
                    ef2.Scene = scene;
                    scene.SectorLights.Add(ef2);
                }
            }
        }

        #endregion

        static string ReadString(BinaryReader br, int len)
        {
            byte[] data = br.ReadBytes(len);
            StringBuilder sb = new StringBuilder();

            foreach (byte d in data)
            {
                if (d > 0)
                    sb.Append((char)d);
                else
                    break;
            }

            return sb.ToString();
        }

        static void Write(BinaryWriter bw, string str, int len)
        {
            for (int i = 0; i < len; i++)
            {
                if (i < str.Length)
                    bw.Write((byte)str[i]);
                else
                    bw.Write((byte)0);
            }
        }

        [Serializable]
        struct Point : IComparable<Point>
        {
            public int X;
            public int Y;

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public override string ToString()
            {
                return "Point {" + X + ":" + Y + "}";
            }

            #region IComparable<Point> Members

            public int CompareTo(Point other)
            {
                int ret = Comparer<int>.Default.Compare(X, other.X);

                if (ret != 0)
                    return ret;

                return Comparer<int>.Default.Compare(Y, other.Y);
            }

            #endregion
        }
    }
}
