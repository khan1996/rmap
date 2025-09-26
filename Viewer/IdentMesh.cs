using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.ComponentModel;
using System.Globalization;

namespace Viewer
{
    class IdentMesh : DrawableGameComponent
    {
        public Part[] Parts;

        public IdentMesh(Game game)
            : base(game)
        {
            Parts = new Part[2];
        }

        public override void Initialize()
        {
            base.Initialize();

            for (int index = 0; index < 2; index++ )
            {
                Parts[index] = new Part()
                {
                    effect = new BasicEffect(GraphicsDevice)
                        {
                            VertexColorEnabled = true,
                            World = Matrix.Identity
                        }
                };
            }

            Color c = Color.Red;
            Parts[0].vertexes = new VertexPositionColor[]
            {
                new VertexPositionColor(new Vector3(0,0,25), c),
                new VertexPositionColor(new Vector3(200,0,0), c),
                new VertexPositionColor(new Vector3(0,0,-25), c)
            };
            Parts[0].indicles = new short[] { 0, 1, 2 };

            c = Color.Blue;
            Parts[1].vertexes = new VertexPositionColor[]
            {
                new VertexPositionColor(new Vector3(-25,1,0), c),
                new VertexPositionColor(new Vector3(0,1,200), c),
                new VertexPositionColor(new Vector3(25,1,0), c)
            };
            Parts[1].indicles = new short[] { 0, 1, 2 };
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (Part p in Parts)
            {
                p.effect.View = Camera.DefaultCamera.View;
                p.effect.Projection = Camera.DefaultCamera.Projection;
                foreach (EffectPass pass in p.effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList,
                        p.vertexes, 0, p.vertexes.Length, p.indicles, 0, p.indicles.Length / 3);
                }
            }

            base.Draw(gameTime);
        }

        public class Part
        {
            public BasicEffect effect;
            public VertexPositionColor[] vertexes;
            public short[] indicles;
        }
    }
}
