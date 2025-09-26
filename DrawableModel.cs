using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rMap.Asset.Animation;
using Viewer;

namespace rMap.Asset
{
    public class DrawableModel : DrawableGameComponent
    {
        public short? Marker1;
        public short? Marker2;
        public int? PartId;

        public Matrix World = Matrix.Identity;
        public List<ModelPart> Parts = new List<ModelPart>();
        public string TexturesFolder;
        private bool octreeGenerated = false;

        BasicEffect effect;

        public bool DrawSkeleton = true;
        public bool DrawSkeletonLinksToVertexes = false;
        public bool DrawMesh = true;
        public FileTypes.OctreeScene Octree = new FileTypes.OctreeScene();

        public DrawableModel() : base(null) { }
        public DrawableModel(Microsoft.Xna.Framework.Game game) : base(game) { }

        protected override void LoadContent()
        {
            if (effect != null)
                effect.Dispose();

            effect = new BasicEffect(GraphicsDevice);

            foreach (ModelPart part in Parts)
                part.Init(GraphicsDevice);

            ReloadTextures();
        }

        public void ReloadTextures()
        {
            foreach (ModelPart part in Parts)
            {
                try
                {
                    part.LoadTexture(TexturesFolder, GraphicsDevice);
                }
                catch { } // errors happen
            }
        }

        protected override void UnloadContent()
        {
            if (effect != null)
            {
                effect.Dispose();
                effect = null;

                foreach (ModelPart p in Parts)
                {
                    if (p.IndexBuffer != null)
                        p.IndexBuffer.Dispose();

                    if (p.VertexBuffer != null)
                        p.VertexBuffer.Dispose();

                    p.IndexBuffer = null;
                    p.VertexBuffer = null;

                    if (p.LoadedTexture != null)
                        p.LoadedTexture.Dispose();
                }
            }
        }

        public BoundingBox Edges { get; private set; }

        public void CalcEdges()
        {
            foreach (ModelPart part in Parts)
                part.CalcEdges(World);

            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float minZ = float.MaxValue;

            float maxX = float.MinValue;
            float maxY = float.MinValue;
            float maxZ = float.MinValue;

            foreach (ModelPart part in Parts)
            {
                BoundingBox box = part.Edges;

                if (box.Max.X > maxX)
                    maxX = box.Max.X;
                if (box.Min.X < minX)
                    minX = box.Min.X;

                if (box.Max.Y > maxY)
                    maxY = box.Max.Y;
                if (box.Min.Y < minY)
                    minY = box.Min.Y;

                if (box.Max.Z > maxZ)
                    maxZ = box.Max.Z;
                if (box.Min.Z < minZ)
                    minZ = box.Min.Z;
            }

            Edges = new BoundingBox(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));

            //Octree.ApplyFromModel(this, 3, 0);
            octreeGenerated = true;
        }

        public float? Intersects(Ray ray)
        {
            if (!octreeGenerated)
                return null;

            //return Octree.Intersects(ray);
            return ray.Intersects(Edges);
        }

        public override void Draw(GameTime gameTime)
        {
            effect.View = Camera.DefaultCamera.View;
            effect.Projection = Camera.DefaultCamera.Projection;

            if (DrawMesh)
            {
                RasterizerState wireRaster = new RasterizerState() { CullMode = CullMode.CullCounterClockwiseFace, FillMode = FillMode.WireFrame, MultiSampleAntiAlias = true };
                RasterizerState rast = GraphicsDevice.RasterizerState;
                foreach (ModelPart part in Parts)
                {
                    effect.Texture = part.LoadedTexture;
                    effect.TextureEnabled = part.LoadedTexture != null;
                    effect.VertexColorEnabled = part.UsesVertexColor();
                    effect.World = part.World * World;

                    if (part.WireMode)
                        GraphicsDevice.RasterizerState = wireRaster;

                    GraphicsDevice.Indices = part.IndexBuffer;
                    GraphicsDevice.SetVertexBuffer(part.VertexBuffer);

                    if (part.LoadedTexture != null)
                    {
                        if (!part.TextureIsDDS)
                            GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicClamp;
                        else
                            GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicWrap;
                    }

                    PrimitiveType type = part.LineList ? PrimitiveType.LineList : PrimitiveType.TriangleList;
                    int primitives = part.LineList ? part.Indices.Count / 2 : part.Indices.Count / 3;

                    try
                    {
                        foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                        {
                            pass.Apply();
                            GraphicsDevice.DrawIndexedPrimitives(type, 0, 0, part.VertexBuffer.VertexCount, 0, primitives);
                        }
                    }
                    catch (Exception) { }

                    if (part.WireMode)
                        GraphicsDevice.RasterizerState = rast;
                }

                wireRaster.Dispose();
            }
        }
    }
}
