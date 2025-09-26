using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Threading;

namespace Viewer
{
    public class Window : IDisposable
    {
        public event Action<Ray> OnMouseClick;

        Game cont = null;
        Thread runner;

        public void Run(bool reachProfile)
        {
            runner = new Thread(new ParameterizedThreadStart(ThreadStartP));
            runner.Start(reachProfile);
        }

        private void ThreadStartP(object reachProfile)
        {
            using (cont = new Game())
            {
                cont.SelectReachProfile((bool)reachProfile);
                cont.OnMouseClick += new Action<Ray>(cont_OnMouseClick);
                cont.Run();
            }
            cont = null;
        }

        void cont_OnMouseClick(Ray obj)
        {
            if (OnMouseClick != null)
                OnMouseClick(obj);
        }

        public Game Game { get { return cont; } }

        public bool IsActive
        {
            get
            {
                return cont != null;
            }
        }

        public void SetCamera(BoundingBox box)
        {
            if (cont != null)
                cont.SetCamera(box);
        }

        public void SyncObjects(IEnumerable<DrawableGameComponent> objs)
        {
            if (cont != null)
                cont.SyncObjects(objs);
        }

        public void Show(IEnumerable<DrawableGameComponent> objs)
        {
            if (cont != null)
            {
                cont.Show(objs);
            }
        }

        public void Clear()
        {
            if (cont != null)
                cont.Clear();
        }

        #region IDisposable Members

        public void Dispose()
        {
            cont.Exit();
        }

        #endregion
    }

    class HeightMap
    {
        public List<VertexPositionColor> verts = new List<VertexPositionColor>();

        public HeightMap(IEnumerable<float[,]> planes)
        {
            verts.Add(new VertexPositionColor(new Vector3(0, 1, 0), Color.Blue));
            verts.Add(new VertexPositionColor(new Vector3(1, -1, 0), Color.Yellow));
            verts.Add(new VertexPositionColor(new Vector3(-1, -1, 0), Color.Red));
            verts.Add(new VertexPositionColor(new Vector3(1, 1, 0), Color.Red));
        }
    }
}
