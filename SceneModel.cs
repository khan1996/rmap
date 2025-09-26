using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using rMap.Zalla;

namespace rMap.Asset
{
    class SceneModel
    {
        private Scene scene;
        private string rylFolder;

        public SceneModel(Scene pScene, string pRylFolder)
        {
            scene = pScene;
            rylFolder = pRylFolder;
        }

        public void Export(string folder, string plainFilename, Dictionary<string, bool> settings)
        {
            FileTypes.AutodeskCollada collada = new FileTypes.AutodeskCollada();
            collada.Model = new DrawableModel();

            if (settings["groundtex"])
            {
                Bitmap bmp = scene.ExportFullTextureMap(scene.GetTexFolder(rylFolder), true);
                bmp.Save(Path.Combine(folder, plainFilename + ".png"), ImageFormat.Png);
                bmp.Dispose();
            }

            if (settings["heightmap"])
            {
                /*
                ModelPart part = GenerateFull();
                part.Name = "heightmap";
                part.LastTextureFullPath = Path.Combine(folder, plainFilename + ".png");
                collada.Model.Parts.Add(part);*/
                collada.Model.Parts.AddRange(GenerateHeightmap());
            }

            if (settings["object"] && !Security.Checkin.IsTrial)
            {
                foreach (FieldObject obj in scene.FieldObjects)
                {
                    FileTypes.NMesh mFile = new FileTypes.NMesh();
                    mFile.Load(Path.Combine(rylFolder, @"objects\object", obj.Mesh));

                    int counter = 0;
                    foreach (ModelPart p in mFile.Model.Parts)
                    {
                        if (!string.IsNullOrEmpty(p.Texture))
                            p.LastTextureFullPath = Path.Combine(rylFolder, @"texture\object", p.Texture);

                        p.Name = "obj." + Path.GetFileNameWithoutExtension(obj.Mesh) + "." + counter++.ToString();
                        p.World = obj.WorldM * Matrix.CreateScale(1, 1, -1);
                        collada.Model.Parts.Add(p);
                    }
                }
            }

            if (settings["house"] && !Security.Checkin.IsTrial)
            {
                foreach (House obj in scene.Houses)
                {
                    int mType = -1;
                    foreach (string mesh in obj.Meshes)
                    {
                        mType++;
                        if (string.IsNullOrEmpty(mesh))
                            continue;

                        FileTypes.NMesh mFile = new FileTypes.NMesh();
                        mFile.Load(Path.Combine(rylFolder, @"objects\house", mesh));

                        int counter = 0;
                        foreach (ModelPart p in mFile.Model.Parts)
                        {
                            if (!string.IsNullOrEmpty(p.Texture))
                                p.LastTextureFullPath = Path.Combine(rylFolder, @"texture\object", p.Texture);

                            p.Name = "house." + Path.GetFileNameWithoutExtension(mesh) + "." + mType + "." + counter++.ToString();
                            p.World = obj.WorldM * Matrix.CreateScale(1, 1, -1);
                            collada.Model.Parts.Add(p);
                        }
                    }
                }
            }

            if (settings["nature"] && !Security.Checkin.IsTrial)
            {
                foreach (NatureObject obj in scene.NatureObjects.Where(x => x.Type != 0))
                {
                    FileTypes.NMesh mFile = new FileTypes.NMesh();
                    mFile.Load(Path.Combine(rylFolder, @"objects\natureobject\zone" + scene.TextureZone, "normaltree" + (obj.Type + 1) + ".r3s"));

                    int counter = 0;
                    foreach (ModelPart p in mFile.Model.Parts)
                    {
                        if (!string.IsNullOrEmpty(p.Texture))
                            p.LastTextureFullPath = Path.Combine(rylFolder, @"texture\object", p.Texture);

                        p.Name = "nature." + obj.Type + "." + counter++.ToString();
                        p.World = Matrix.CreateTranslation(obj.GetPosition()) * Matrix.CreateScale(1, 1, -1);
                        collada.Model.Parts.Add(p);
                    }
                }
            }

            if (settings["water"])
            {
                foreach (WaterData water in scene.WaterDatas)
                {
                    collada.Model.Parts.Add(GenerateWater(water));
                }
            }

            collada.Save(Path.Combine(folder, plainFilename + ".dae"));
        }

        public void Import(string path, Dictionary<string, bool> settings)
        {
            FileTypes.AutodeskCollada collada = new FileTypes.AutodeskCollada();
            collada.LoadOnlyMeshes = settings["heightmap"] ? new[] { "heightmap" } : new string[] { };
            collada.Load(path);

            if (settings["heightmap"])
            {
                scene.HeightTables.Clear();
                foreach (ModelPart mesh in collada.Model.Parts.Where(x => x.Name.StartsWith("heightmap.")))
                {
                    LoadHeightmapSplit(mesh);
                }
                /*
                ModelPart part = collada.Model.Parts.SingleOrDefault(x => x.Name == "heightmap");

                if (part == null)
                    throw new Exception("Heightmap not found in collada");

                LoadHeightmap(part);*/
            }

            if (settings["groundtex"])
            {
                string filename = Path.GetFileNameWithoutExtension(path) + ".png";

                GC.Collect();
                using (Bitmap bmp = (Bitmap)Image.FromFile(filename))
                {
                    GenerateGroundTextures(bmp);
                }
            }

            uint scene_id = 1;

            if (settings["object"])
            {
                scene.FieldObjects.Clear();

                foreach (ModelPart mesh in collada.Model.Parts.Where(x => x.Name.StartsWith("obj.")))
                {
                    LoadObjects(mesh, ref scene_id);
                }
            }
            if (settings["house"])
            {
                scene.Houses.Clear();
                List<int> done_instances = new List<int>();

                foreach (ModelPart mesh in collada.Model.Parts.Where(x => x.Name.StartsWith("house.")))
                {
                    LoadHouses(mesh, ref scene_id);
                }
            }
            if (settings["nature"])
            {
                scene.NatureObjects.Clear();
                List<int> done_instances = new List<int>();

                foreach (ModelPart mesh in collada.Model.Parts.Where(x => x.Name.StartsWith("nature.")))
                {
                    LoadNatures(mesh, ref scene_id);
                }
            }
            if (settings["water"])
            {
                WaterData[] waters = scene.WaterDatas.ToArray();
                scene.WaterDatas.Clear();

                foreach (ModelPart mesh in collada.Model.Parts.Where(x => x.Name.StartsWith("water.")))
                {
                    LoadWaterHeights(mesh, waters);
                }
            }
        }

        private void LoadNatures(ModelPart mesh, ref uint scene_id)
        {
            string[] nameSplits = mesh.Name.Split('.');
            byte type = byte.Parse(nameSplits[1], Program.Number);
            int meshPart = int.Parse(nameSplits[2], Program.Number);

            if (meshPart != 0)
                return;

            Vector3 pos = (mesh.World * Matrix.CreateScale(1, 1, -1)).Translation;

            // TileY * TileSizeY + PosY * TileSizeY / 65;

            int tileX = (int)Math.Floor((double)pos.X / TileObject.TileSizes.X);
            int tileY = (int)Math.Floor((double)pos.Z / TileObject.TileSizes.Y);

            byte posX = (byte)Math.Round((pos.X - tileX * TileObject.TileSizes.X) * 65 / TileObject.TileSizes.X);
            byte posY = (byte)Math.Round((pos.Z - tileY * TileObject.TileSizes.Y) * 65 / TileObject.TileSizes.Y);

            NatureObject obj = new NatureObject()
            {
                Scene = scene,
                TileX = tileX,
                TileY = tileY,
                PosX = posX,
                PosY = posY,
                Type = type
            };

            scene.NatureObjects.Add(obj);
        }

        private void LoadHouses(ModelPart mesh, ref uint scene_id)
        {
            string[] nameSplits = mesh.Name.Split('.');
            string r3s = nameSplits[1];
            int partType = int.Parse(nameSplits[2], Program.Number);
            int meshPart = int.Parse(nameSplits[3], Program.Number);

            if (meshPart != 0)
                return;

            Matrix world = mesh.World * Matrix.CreateScale(1, 1, -1);
            House house = scene.Houses.FirstOrDefault(x => x.WorldM == world);

            if (house == null)
            {
                house = new House()
                {
                    Scene = scene,
                    SceneID = (int)scene_id++,
                    WorldM = world
                };

                scene.Houses.Add(house);
            }
            house.Meshes[partType] = r3s;
        }

        private void LoadObjects(ModelPart mesh, ref uint scene_id)
        {
            string[] nameSplits = mesh.Name.Split('.');
            string r3s = nameSplits[1];
            int meshPart = int.Parse(nameSplits[2], Program.Number);

            if (meshPart != 0)
                return;

            FieldObject obj = new FieldObject()
            {
                Scene = scene,
                Mesh = r3s,
                SceneID = scene_id++,
                WorldM = mesh.World * Matrix.CreateScale(1, 1, -1)
            };

            scene.FieldObjects.Add(obj);
        }

        private void LoadWaterHeights(ModelPart mesh, WaterData[] existing)
        {
            string[] nameSplits = mesh.Name.Split('.');
            int x = int.Parse(nameSplits[1], Program.Number);
            int y = int.Parse(nameSplits[2], Program.Number);

            WaterData water = existing.SingleOrDefault(a => a.TileX == x && a.TileY == y);

            if (water == null)
                water = new WaterData()
                {
                    TileX = x,
                    TileY = y,
                    Color = 0x5f000000,
                    PosX = 0,
                    PosY = 0,
                    SizeX = TileObject.TileSizes.X,
                    SizeY = TileObject.TileSizes.Y,
                    Reflection = false,
                    Scene = scene
                };

            water.Height = mesh.World.Translation.Y;
            scene.WaterDatas.Add(water);
        }

        private void LoadHeightmapSplit(ModelPart mesh)
        {
            string[] nameSplits = mesh.Name.Split('.');
            int tileX = int.Parse(nameSplits[1], Program.Number);
            int tileY = int.Parse(nameSplits[2], Program.Number);

            Matrix world = mesh.World * Matrix.CreateTranslation(new Vector3(-tileX * 31507, 0, tileY * 31507)) * Matrix.CreateScale(new Vector3(64f / 31507f, 1, -64f / 31507f));
            var vertexes = mesh.Vertexes.Select(x => Vector3.Transform(x.Position, world));

            int minX = (int)vertexes.Min(x => x.X);
            int minZ = (int)vertexes.Min(x => x.Z);
            int maxX = (int)vertexes.Max(x => x.X);
            int maxZ = (int)vertexes.Max(x => x.Z);

            int height = maxZ - minZ + 1;
            float[,] heights = new float[maxX - minX + 1, height];

            foreach (Vector3 vec in vertexes)
            {
                heights[(int)vec.X - minX, height - ((int)vec.Z - minZ) - 1] = vec.Y;
            }

            HeightTable table = new HeightTable()
            {
                Scene = scene,
                X = tileX,
                Y = tileY,
                Table = heights
            };

            scene.HeightTables.Add(table);
        }
        private void LoadHeightmap(ModelPart mesh)
        {
            Matrix world = mesh.World * Matrix.CreateScale(new Vector3(64f / 31507f, 1, -64f / 31507f));
            var vertexes = mesh.Vertexes.Select(x => Vector3.Transform(x.Position, world));

            int minX = (int)vertexes.Min(x => x.X);
            int minZ = (int)vertexes.Min(x => x.Z);
            int maxX = (int)vertexes.Max(x => x.X);
            int maxZ = (int)vertexes.Max(x => x.Z);

            int height = maxZ - minZ + 1;
            float[,] heights = new float[maxX - minX + 1, height];

            foreach (Vector3 vec in vertexes)
            {
                heights[(int)vec.X - minX, height - ((int)vec.Z - minZ) - 1] = vec.Y;
            }

            int tilesMinX = (int)Math.Floor((double)minX / 64); // 0->64 is one tile named 0, -1 -> -65 is one tile named -1
            int tilesMinY = (int)Math.Floor((double)minZ / 64);
            int tilesMaxX = (int)Math.Floor((double)maxX / 64) - 1;
            int tilesMaxY = (int)Math.Floor((double)maxZ / 64) - 1;

            scene.SetHeights(heights, tilesMinX, tilesMinY, tilesMaxX, tilesMaxY);
        }

        private void GenerateGroundTextures(Bitmap big)
        {
            string folder = Path.Combine(rylFolder, @"texture\widetexture\zone" + scene.TextureZone);

            foreach (TileTexture tex in scene.Textures)
            {
                if (!string.IsNullOrEmpty(tex.Texture1))
                    File.Delete(Path.Combine(folder, tex.Texture1));
            }
            TileTexture[] textures = scene.Textures.ToArray();

            scene.Textures.Clear();
            scene.TextureThumbnail = null;

            if (scene.HeightTables.Count < 1)
                return;

            int ax, ay, bx, by;
            scene.GetHeightMapTileBoundarys(out ax, out ay, out bx, out by);

            int nx = bx - ax + 1;
            int ny = by - ay + 1;

            if (big.Width != nx * scene.TextureSize)
                throw new Exception("Ground texture width isn't correct: Is " + big.Width + ", has to be " + (nx * scene.TextureSize));
            if (big.Height != ny * scene.TextureSize)
                throw new Exception("Ground texture height isn't correct: Is " + big.Height + ", has to be " + (ny * scene.TextureSize));

            for (int x = ax; x <= bx; x++)
            {
                for (int y = ay; y <= by; y++)
                {
                    TileTexture existing = textures.SingleOrDefault(z => z.X == x && z.Y == y);

                    int startX = (x - ax) * scene.TextureSize;
                    int startY = (by - y - ay - 1) * scene.TextureSize;

                    System.Drawing.Imaging.BitmapData bmpData = big.LockBits(new System.Drawing.Rectangle(startX, startY, scene.TextureSize, scene.TextureSize),
                        System.Drawing.Imaging.ImageLockMode.ReadWrite,
                        System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    if (GotColors(bmpData))
                    {
                        string name = existing == null ? string.Format("{0}_{1}_0_Land.dds", x, y) : existing.Texture1;

                        Tao.DDSLoader.SaveBitmapToDDS(Path.Combine(folder, name), bmpData);

                        if (existing == null)
                            existing = new TileTexture() { X = x, Y = y, Texture1 = name, DetailBlend1 = "landdetail.dds", Scene = scene };

                        scene.Textures.Add(existing);
                    }
                    big.UnlockBits(bmpData);
                }
            }
        }

        private unsafe bool GotColors(System.Drawing.Imaging.BitmapData bmpData)
        {
            int pixels = bmpData.Height * bmpData.Width;
            byte* p = (byte*)(void*)bmpData.Scan0;

            for (int i = 0; i < pixels; i++)
            {
                if (*p++ > 0 || *p++ > 0 || *p++ > 0) // BGRA
                    return true;

                p++;
            }
            return false;
        }

        private ModelPart GenerateHeightmap(float[,] heights)
        {
            ModelPart part = new ModelPart() { ColladaWriteNormals = false };
            part.World = Matrix.CreateScale(new Vector3(31507f / 64f, 1, -31507f / 64f));

            int height = heights.GetUpperBound(1) + 1;
            int width = heights.GetUpperBound(0) + 1;

            VertexPositionNormalTexture[] verts = new VertexPositionNormalTexture[width * height];
            int[] indices = new int[(width - 1) * (height - 1) * 6];

            for (int z = 0; z < height; z++)
                for (int x = 0; x < width; x++)
                {
                    Vector3 tempPos = new Vector3((float)x, heights[x, height - z - 1], (float)z);
                    Vector2 tempTexCo = new Vector2((float)(x) / (width - 1), (float)(z) / (height - 1));

                    VertexPositionNormalTexture temp = new VertexPositionNormalTexture(tempPos, Vector3.Zero, tempTexCo);
                    verts[x + z * width] = temp;
                }

            for (int x = 0; x < width - 1; x++)
                for (int y = 0; y < height - 1; y++)
                {
                    indices[(x + y * (width - 1)) * 6] = (x + 1) + (y + 1) * width;
                    indices[(x + y * (width - 1)) * 6 + 1] = (x + 1) + y * width;
                    indices[(x + y * (width - 1)) * 6 + 2] = x + y * width;

                    indices[(x + y * (width - 1)) * 6 + 3] = (x + 1) + (y + 1) * width;
                    indices[(x + y * (width - 1)) * 6 + 4] = x + y * width;
                    indices[(x + y * (width - 1)) * 6 + 5] = x + (y + 1) * width;
                }
            /*
            for (int i = 0; i < indices.Length / 3; i++)
            {
                int index1 = indices[i * 3];
                int index2 = indices[i * 3 + 1];
                int index3 = indices[i * 3 + 2];

                Vector3 side1 = verts[index2].Position - verts[index1].Position;
                Vector3 side2 = verts[index1].Position - verts[index3].Position;
                Vector3 normal = Vector3.Cross(side1, side2);
                normal.Normalize();

                verts[index1].Normal += normal;
                verts[index2].Normal += normal;
                verts[index3].Normal += normal;
            }

            for (int i = 0; i < verts.Length; i++)
                verts[i].Normal.Normalize();*/

            part.Vertexes = new List<VertexPositionNormalTexture>(verts);
            part.Indices = new List<int>(indices.Select(x => (int)x));

            return part;
        }

        private IEnumerable<ModelPart> GenerateHeightmap()
        {
            if (scene.HeightTables.Count > 0)
            {
                int minX, minY, maxX, maxY;
                scene.GetHeightMapTileBoundarys(out minX, out minY, out maxX, out maxY);

                for (int tx = minX; tx <= maxX; tx++)
                {
                    for (int ty = minY; ty <= maxY; ty++)
                    {
                        float[,] heights = scene.GetHeights(tx, ty, tx, ty);
                        Zalla.TileTexture tex = scene.Textures.SingleOrDefault(x => x.X == tx && x.Y == ty);

                        ModelPart part = GenerateHeightmap(heights);
                        part.Name = "heightmap." + tx + "." + ty;
                        part.World *= Matrix.CreateTranslation(new Vector3(tx * 31507, 0, -ty * 31507));
                        part.LastTextureFullPath = tex == null ? null : System.IO.Path.Combine(rylFolder, @"Texture\Widetexture\Zone" + scene.TextureZone, tex.Texture1);

                        yield return part;
                    }
                }
            }
        }

        private ModelPart GenerateFull()
        {
            if (scene.HeightTables.Count < 1)
                return new ModelPart();

            int minX, minY, maxX, maxY;
            scene.GetHeightMapTileBoundarys(out minX, out minY, out maxX, out maxY);

            float[,] heights = scene.GetHeights(minX, minY, maxX, maxY);

            ModelPart part = GenerateHeightmap(heights);
            part.World *= Matrix.CreateTranslation(new Vector3(minX * 31507, 0, -minY * 31507));

            return part;
        }

        private ModelPart GenerateWater(WaterData water)
        {
            ModelPart part = GenerateHeightmap(new float[,] { { 0, 0 }, { 0, 0 } });
            part.World *= Matrix.CreateScale(64f, 1f, 64f) * Matrix.CreateTranslation(new Vector3(water.TileX * 31507, water.Height, -water.TileY * 31507));
            part.Color = new Microsoft.Xna.Framework.Color(0, 12, 255, 255);
            part.Transparency = 0.5f;
            part.Name = "water." + water.TileX + "." + water.TileY;

            return part;
        }
    }
}
