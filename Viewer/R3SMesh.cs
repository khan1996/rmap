using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Arcane.Xna.Engine.Terrain;
using System.ComponentModel;
using System.Globalization;
using rylModel;

namespace Viewer
{
    class R3SMesh : DrawableGameComponent
    {
        R3S r3s;
        public Part[] Parts;
        public string TextureFolder;

        public R3SMesh(Game game, R3S mesh, string texFolder)
            : base(game)
        {
            r3s = mesh;
            Parts = new Part[r3s.Meshes.Length];
            TextureFolder = texFolder;
        }

        public void RotateZ(float angle)
        {
            foreach (Part p in Parts)
            {
                p.effect.World *= Matrix.CreateRotationY(angle);
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            Matrix world = Matrix.Identity; //new Matrix(0.9210608f, 0, 0.3894183f, 0, 0, 1, 0, 0, -0.3894183f, 0, 0.9210608f, 0, 0, 0, 0, 1);

            int index = 0;
            foreach (U1Mesh m in r3s.Meshes)
            {
                Parts[index] = new Part()
                {
                    vertDec = new VertexDeclaration(GraphicsDevice, VertexPositionTexture.VertexElements),
                    vertexes = new VertexPositionTexture[r3s.Method == R3S.Filter.Normal ? m.Vertexes.Length : m.Vertexes2.Length],
                    indicles = m.Indices,
                    texName = m.Texture,
                    effect = new BasicEffect(GraphicsDevice, null)
                        {
                            TextureEnabled = true,
                            World = world
                        }
                };

                for (int i = 0; i < Parts[index].vertexes.Length; i++)
                {
                    VertexPositionTexture vert = new VertexPositionTexture();

                    if (r3s.Method == R3S.Filter.Normal)
                    {
                        vert.TextureCoordinate = new Vector2(m.Vertexes[i].tu, m.Vertexes[i].tv);
                        vert.Position = new Vector3(m.Vertexes[i].X, m.Vertexes[i].Z, -m.Vertexes[i].Y);
                    }
                    else
                    {
                        vert.TextureCoordinate = new Vector2(m.Vertexes2[i].tu, m.Vertexes2[i].tv);
                        vert.Position = new Vector3(m.Vertexes2[i].X, m.Vertexes2[i].Z, -m.Vertexes2[i].Y);
                    }

                    Parts[index].vertexes[i] = vert;
                }

                index++;
            }

            foreach (Part p in Parts)
            {
                if (!string.IsNullOrEmpty(p.texName))
                {
                    byte[] da = System.IO.File.ReadAllBytes(System.IO.Path.Combine(TextureFolder, System.IO.Path.GetFileNameWithoutExtension(p.texName) + ".dds"));
                    da[0] = (byte)'D';
                    da[1] = (byte)'D';
                    da[2] = (byte)'S';
                    da[3] = (byte)' ';
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream(da))
                    {
                        p.texture = Texture2D.FromFile(GraphicsDevice, ms);
                        p.effect.Texture = p.texture;
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (Part p in Parts)
            {
                p.effect.View = Camera.DefaultCamera.View;
                p.effect.Projection = Camera.DefaultCamera.Projection;

                p.effect.Begin();
                foreach (EffectPass pass in p.effect.CurrentTechnique.Passes)
                {
                    pass.Begin();
                    
                    GraphicsDevice.VertexDeclaration = p.vertDec;
                    GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList,
                        p.vertexes, 0, p.vertexes.Length, p.indicles, 0, p.indicles.Length / 3);
                    pass.End();
                }
                p.effect.End();

            }

            base.Draw(gameTime);
        }

        public class Part
        {
            public string texName;
            public BasicEffect effect;
            public Texture2D texture;
            public VertexDeclaration vertDec;
            public VertexPositionTexture[] vertexes;
            public short[] indicles;
        }
    }
}
