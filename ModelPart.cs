using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using Microsoft.Xna.Framework;
using System.IO;
using rMap.Asset.Animation;

namespace rMap.Asset
{
    public class ModelPart
    {
        public List<VertexPositionNormalTexture> Vertexes = new List<VertexPositionNormalTexture>();
        public List<VertexExtraInfo> VertexExtraInfo = new List<VertexExtraInfo>();

        public List<int> Indices = new List<int>();
        public string Texture;
        public string BumpTexture;
        public byte[] TextureByte;
        public string LastTextureFullPath;
        public bool TextureIsDDS;
        public string Name;
        public bool ColladaWriteNormals = true;
        public Microsoft.Xna.Framework.Color? Color;
        public float Transparency = 1;

        public VertexBuffer VertexBuffer;
        public IndexBuffer IndexBuffer;
        public Texture2D LoadedTexture;

        public bool LineList;
        public bool WireMode;

        public Matrix World = Matrix.Identity;

        public virtual void Init(GraphicsDevice graphics)
        {
            VertexBuffer = new VertexBuffer(graphics, typeof(VertexPositionNormalTexture), Vertexes.Count, BufferUsage.WriteOnly);
            VertexBuffer.SetData(Vertexes.ToArray());

            IndexBuffer = new IndexBuffer(graphics, IndexElementSize.SixteenBits, Indices.Count, BufferUsage.WriteOnly);
            IndexBuffer.SetData(Indices.Select(x => (ushort)x).ToArray());
        }

        public virtual bool UsesVertexColor() { return false; }

        public void LoadTexture(string path, GraphicsDevice graphics)
        {
            if (TextureByte != null)
            {
                if (TextureIsDDS)
                {
                    using (MemoryStream ms = new MemoryStream(TextureByte))
                    {
                        DDSLib.DDSFromStream(ms, graphics, 0, false, out LoadedTexture);
                    }
                }
                else // Support PNG, JPEG, and GIF formats 
                {
                    using (MemoryStream ms = new MemoryStream(TextureByte))
                    {
                        LoadedTexture = Texture2D.FromStream(graphics, ms);
                    }
                }
                return;
            }

            if (string.IsNullOrEmpty(Texture))
                LoadedTexture = null;
            else
            {
                string file = string.IsNullOrEmpty(path) ? Texture : Path.Combine(path, Texture);
                string filename = Path.GetFileNameWithoutExtension(file);

                if (!string.IsNullOrEmpty(path))
                {
                    string[] files1 = Directory.GetFiles(path, filename + ".dds");

                    if (files1.Length < 1)
                    {
                        string[] files = Directory.GetFiles(path, filename + ".*");
                        if (files.Length > 0)
                        {
                            if (!files.Contains(file, StringComparer.CurrentCultureIgnoreCase))
                                file = files[0];
                        }
                    }
                    else
                        file = files1[0];
                }

                string ext = Path.GetExtension(file).ToLower();
                LastTextureFullPath = file;
                try
                {
                    if (ext == ".dds")
                    {
                        TextureIsDDS = true;

                        byte[] data = File.ReadAllBytes(file);
                        data[0] = (byte)'D';
                        data[1] = (byte)'D';
                        data[2] = (byte)'S';
                        data[3] = (byte)' ';

                        using (MemoryStream ms = new MemoryStream(data))
                        {
                            DDSLib.DDSFromStream(ms, graphics, 0, false, out LoadedTexture);
                        }
                    }
                    else // Support PNG, JPEG, and GIF formats 
                    {
                        TextureIsDDS = false;

                        byte[] data = File.ReadAllBytes(file);
                        using (MemoryStream ms = new MemoryStream(data))
                        {
                            LoadedTexture = Texture2D.FromStream(graphics, ms);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Texture = null;

                    LogDetailView.Instance.AddInfo(ex.ToString());
                }
            }
        }

        public BoundingBox Edges { get; protected set; }

        public virtual void CalcEdges(Matrix world)
        {
            Matrix cWorld = World * world;

            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float minZ = float.MaxValue;

            float maxX = float.MinValue;
            float maxY = float.MinValue;
            float maxZ = float.MinValue;

            for (int i = 0; i < Vertexes.Count; i++)
            {
                Vector3 pos = Vector3.Transform(Vertexes[i].Position, cWorld);

                if (pos.X > maxX)
                    maxX = pos.X;
                if (pos.X < minX)
                    minX = pos.X;

                if (pos.Y > maxY)
                    maxY = pos.Y;
                if (pos.Y < minY)
                    minY = pos.Y;

                if (pos.Z > maxZ)
                    maxZ = pos.Z;
                if (pos.Z < minZ)
                    minZ = pos.Z;
            }

            Edges = new BoundingBox(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
        }
    }

    public class ModelPart<T> : ModelPart where T : struct
    {
        public new List<T> Vertexes = new List<T>();

        public override void Init(GraphicsDevice graphics)
        {
            VertexBuffer = new VertexBuffer(graphics, typeof(T), Vertexes.Count, BufferUsage.WriteOnly);
            VertexBuffer.SetData(Vertexes.ToArray());

            IndexBuffer = new IndexBuffer(graphics, IndexElementSize.SixteenBits, Indices.Count, BufferUsage.WriteOnly);
            IndexBuffer.SetData(Indices.Select(x => (ushort)x).ToArray());
        }

        public override bool UsesVertexColor() { return true; }

        public override void CalcEdges(Matrix world)
        {
            Matrix cWorld = World * world;

            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float minZ = float.MaxValue;

            float maxX = float.MinValue;
            float maxY = float.MinValue;
            float maxZ = float.MinValue;

            for (int i = 0; i < Vertexes.Count; i++)
            {
                Vector3 pos = Vector3.Transform((Vector3)typeof(T).GetField("Position").GetValue(Vertexes[i]), cWorld);

                if (pos.X > maxX)
                    maxX = pos.X;
                if (pos.X < minX)
                    minX = pos.X;

                if (pos.Y > maxY)
                    maxY = pos.Y;
                if (pos.Y < minY)
                    minY = pos.Y;

                if (pos.Z > maxZ)
                    maxZ = pos.Z;
                if (pos.Z < minZ)
                    minZ = pos.Z;
            }

            Edges = new BoundingBox(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
        }
    }

    public class VertexExtraInfo
    {
        public int Bone1;
        public int Bone2;
        public float Bone1Weight;
    }
}
