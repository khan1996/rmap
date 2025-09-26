using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using rMap.Zalla;
using Microsoft.Xna.Framework;
using Rectangle = System.Drawing.Rectangle;
using Point = System.Drawing.Point;
using Color = System.Drawing.Color;

namespace rMap
{
    public delegate void MapObjEvent(object obj);

    public partial class ObjectSelector : Form
    {
        public int OpenZone { get; private set; }
        public string RylDir { get; private set; }
        public event MapObjEvent ShowObject;

        private List<MapTile> Tiles = new List<MapTile>();
        private List<MapObj> Objects = new List<MapObj>();
        private Bitmap bigMap = null;
        private Rectangle TilesRec = new Rectangle();
        private Point? MouseOverTile = null;
        private Point? ZoomedTile = null;
        private Scene OpenScene = null;

        public ObjectSelector()
        {
            InitializeComponent();

            objLimit = objectLimitBar.Value;
            if (objectLimitBar.Value == objectLimitBar.Maximum)
                objLimit = null;
            UpdateHiddenCount();
        }

        public void LoadScene(Scene scene, string rylFolder)
        {
            Unload();

            OpenZone = scene.TextureZone;
            RylDir = rylFolder;
            OpenScene = scene;

            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + " big start");
            Bitmap big = scene.ExportFullTextureMap(scene.GetTexFolder(rylFolder));

            if (big == null)
                return;

            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + " tiles start");
            CreateTiles(scene, big);

            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + " resize start");
            int maxX = scene.Textures.Max(t => t.X);
            int maxY = scene.Textures.Max(t => t.Y);
            bigMap = new Bitmap(big, new Size(maxX * 128, maxY * 128)); // 2 times smaller
            big.Dispose();
            TilesRec = new Rectangle(1, 1, maxX, maxY);
            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + " done");

            selectPanel.BackgroundImage = bigMap;
        }

        private void CreateTiles(Scene scene, Bitmap big)
        {
            // tile 1:1 is the left bottom one in size 256x256
            int maxX = scene.Textures.Max(t => t.X);
            int maxY = scene.Textures.Max(t => t.Y);

            BitmapData bigdata = big.LockBits(new Rectangle(0, 0, big.Width, big.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            try
            {
                foreach (TileTexture tex in scene.Textures)
                {
                    if (tex.X < 1 || tex.Y < 1)
                        continue;

                    Bitmap tilebmp = new Bitmap(256 + 128 * 2, 256 + 128 * 2);
                    BitmapData tiledata = tilebmp.LockBits(new Rectangle(0, 0, tilebmp.Width, tilebmp.Height),
                        ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

                    Rectangle from = new Rectangle();
                    Point to = new Point(0, 0);

                    from.X = (tex.X - 1) * 256 - 128;
                    from.Y = (maxY - tex.Y) * 256 - 128;
                    from.Width = 256 + 128 * 2;
                    from.Height = 256 + 128 * 2;

                    if (from.X < 0)
                    {
                        from.Width += from.X;
                        to.X -= from.X;
                        from.X = 0;
                    }
                    if (from.Y < 0)
                    {
                        from.Height += from.Y;
                        to.Y -= from.Y;
                        from.Y = 0;
                    }
                    if (from.X + from.Width > big.Width)
                        from.Width -= (from.X + from.Width) - big.Width;
                    if (from.Y + from.Height > big.Height)
                        from.Height -= (from.Y + from.Height) - big.Height;

                    try
                    {
                        unsafe
                        {
                            byte* bigstart = (byte*)(void*)bigdata.Scan0;
                            byte* tilestart = (byte*)(void*)tiledata.Scan0;

                            if (bigdata.Stride != big.Width * 3 || tiledata.Stride != tilebmp.Width * 3)
                                throw new Exception("Pixel width isnt 3 bytes");

                            int bigstride = bigdata.Stride;
                            int tilestride = tiledata.Stride;

                            int torow = to.Y;
                            int tocol = to.X;
                            for (int py = from.Y; py < from.Height + from.Y; py++)
                            {
                                for (int px = from.X; px < from.Width + from.X; px++)
                                {
                                    int frompos = py * bigstride + px * 3;
                                    int topos = torow * tilestride + tocol++ * 3;

                                    tilestart[topos++] = bigstart[frompos++]; // b
                                    tilestart[topos++] = bigstart[frompos++]; // g
                                    tilestart[topos++] = bigstart[frompos++]; // r
                                }
                                tocol = to.X;
                                torow++;
                            }
                        }
                    }
                    finally
                    {
                        tilebmp.UnlockBits(tiledata);
                    }

                    Tiles.Add(new MapTile() { X = tex.X, Y = tex.Y, Image = tilebmp });
                }
            }
            finally
            {
                big.UnlockBits(bigdata);
            }
        }

        private Rectangle GetBackgroundImageRect()
        {
            if (selectPanel.BackgroundImage == null)
                throw new Exception();

            Rectangle e = new Rectangle(0, 0, selectPanel.Width, selectPanel.Height);

            double aspectRate = (double)selectPanel.BackgroundImage.Width / selectPanel.BackgroundImage.Height;
            double rate = (double)e.Width / e.Height;

            bool isHeightSmaller = rate > aspectRate;

            Point start = new Point(0, 0);
            if (isHeightSmaller)
                start.X = (int)((e.Width - aspectRate * e.Height) / 2);
            else
                start.Y = (int)((e.Height - (double)e.Width / aspectRate) / 2);

            int w = e.Width - start.X * 2;
            int h = e.Height - start.Y * 2;

            return new Rectangle(start, new Size(w, h));
        }

        private void selectPanel_Paint(object sender, PaintEventArgs e)
        {
            if (selectPanel.BackgroundImage != null)
            {
                Rectangle bgrec = GetBackgroundImageRect();

                if (!ZoomedTile.HasValue)
                {
                    int w = bgrec.Width / (TilesRec.Width - TilesRec.X + 1);
                    int h = bgrec.Height / (TilesRec.Height - TilesRec.Y + 1);

                    int y = TilesRec.Height - TilesRec.Y;
                    for (int ty = TilesRec.Y; ty <= TilesRec.Height; ty++, y--)
                    {
                        int x = 0;
                        for (int tx = TilesRec.X; tx <= TilesRec.Width; tx++, x++)
                        {
                            Rectangle rec = new Rectangle(
                                bgrec.X + x * w,
                                bgrec.Y + y * h,
                                w - (tx == TilesRec.Width ? 1 : 0),
                                h - (ty == TilesRec.Height ? 1 : 0));

                            if (MouseOverTile.HasValue && MouseOverTile.Value.X == tx && MouseOverTile.Value.Y == ty)
                                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(127, Color.Blue)), rec);

                            else
                                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(127, Color.Blue)), rec);
                        }
                    }
                }
                else
                {
                    Rectangle rec = new Rectangle(
                        bgrec.X + bgrec.Width / 4,
                        bgrec.Y + bgrec.Height / 4,
                        bgrec.Width / 2,
                        bgrec.Height / 2);

                    e.Graphics.DrawRectangle(new Pen(Color.FromArgb(150, Color.Blue)), rec); 
                }

                e.Graphics.Flush();
            }
        }

        public void Unload()
        {
            if (ZoomedTile.HasValue)
                ZoomOut();

            BackgroundImage = null;
            OpenScene = null;

            if (bigMap != null)
                bigMap.Dispose();

            foreach (MapTile tile in Tiles)
                tile.Dispose();

            Tiles.Clear();
        }

        public void ObjectChanged(object obj)
        {
        }

        private void ZoomIn(Point tilep)
        {
            MapTile tile = Tiles.SingleOrDefault(mt => mt.X == tilep.X && mt.Y == tilep.Y);
            if (tile == null)
                return;

            ZoomedTile = tilep;
            MouseOverTile = null;

            selectPanel.BackgroundImage = tile.Image;

            Rectangle rec = GetBackgroundImageRect();
            RectangleF maprec = new RectangleF(
                (ZoomedTile.Value.X - 1) * TileObject.TileSizes.X + TileObject.TileSizes.X / 2,
                (ZoomedTile.Value.Y - 1) * TileObject.TileSizes.Y + TileObject.TileSizes.Y / 2,
                TileObject.TileSizes.X * 2,
                TileObject.TileSizes.Y * 2);

            this.SuspendLayout();

            if (chkHouse.Checked)
                AddHouses(rec, maprec);
            if (chkNature.Checked)
                AddNature(rec, maprec);
            if (chkObj.Checked)
                AddObj(rec, maprec);
            if (chkEff.Checked)
                AddEff(rec, maprec);

            this.ResumeLayout(true);

            UpdateHiddenCount();
        }

        private void AddEff(Rectangle clientRec, RectangleF mapRec)
        {
            foreach (Effect ob in OpenScene.Effects.Where(h => GotRoomForObj() && mapRec.Contains(h.GetPosition().X, h.GetPosition().Z)))
            {
                MapObj ob2 = new MapObj(ob);
                Objects.Add(ob2);
                selectPanel.Controls.Add(ob2);
                ob2.UpdatePos(clientRec, mapRec);
                ob2.MouseClick += new MouseEventHandler(ob_MouseClick);
            }

            foreach (SectorLight ob in OpenScene.SectorLights.Where(h => GotRoomForObj() && mapRec.Contains(h.GetPosition().X, h.GetPosition().Z)))
            {
                MapObj ob2 = new MapObj(ob);
                Objects.Add(ob2);
                selectPanel.Controls.Add(ob2);
                ob2.UpdatePos(clientRec, mapRec);
                ob2.MouseClick += new MouseEventHandler(ob_MouseClick);
            }
        }

        private void AddObj(Rectangle clientRec, RectangleF mapRec)
        {
            foreach (FieldObject ob in OpenScene.FieldObjects.Where(h => GotRoomForObj() && mapRec.Contains(h.GetPosition().X, h.GetPosition().Z)))
            {
                MapObj ob2 = new MapObj(ob);
                Objects.Add(ob2);
                selectPanel.Controls.Add(ob2);
                ob2.UpdatePos(clientRec, mapRec);
                ob2.MouseClick += new MouseEventHandler(ob_MouseClick);
            }
        }

        private void AddHouses(Rectangle clientRec, RectangleF mapRec)
        {
            foreach (House ob in OpenScene.Houses.Where(h => GotRoomForObj() && mapRec.Contains(h.GetPosition().X, h.GetPosition().Z)))
            {
                MapObj ob2 = new MapObj(ob);
                Objects.Add(ob2);
                selectPanel.Controls.Add(ob2);
                ob2.UpdatePos(clientRec, mapRec);
                ob2.MouseClick += new MouseEventHandler(ob_MouseClick);
            }
        }

        private void AddNature(Rectangle clientRec, RectangleF mapRec)
        {
            foreach (NatureObject ob in OpenScene.NatureObjects.Where(h => GotRoomForObj() && mapRec.Contains(h.MapPosX, h.MapPosY)))
            {
                MapObj ob2 = new MapObj(ob);
                Objects.Add(ob2);
                selectPanel.Controls.Add(ob2);
                ob2.UpdatePos(clientRec, mapRec);
                ob2.MouseClick += new MouseEventHandler(ob_MouseClick);
            }
        }

        private bool GotRoomForObj()
        {
            return objLimit.HasValue ? Objects.Count < objLimit.Value : true;
        }

        private void ob_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (ShowObject != null)
                    ShowObject((sender as MapObj).Object);
            }
        }

        private void ZoomOut()
        {
            if (!ZoomedTile.HasValue)
                return;

            selectPanel.BackgroundImage = bigMap;
            ZoomedTile = null;

            foreach (MapObj ob in Objects)
            {
                selectPanel.Controls.Remove(ob);
                ob.Dispose();
            }
            Objects.Clear();
        }

        private class MapTile : IDisposable
        {
            public int X;
            public int Y;
            public Bitmap Image;

            #region IDisposable Members

            public void Dispose()
            {
                if (Image != null)
                    Image.Dispose();
            }

            #endregion
        }

        private class MapObj : Control
        {
            public object Object;

            public MapObj(object obj)
            {
                Object = obj;
                this.Size = new Size(5, 5);
            }

            public void UpdatePos(Rectangle rec, RectangleF maprec)
            {
                Vector3 pos = 
                    Object is TileObject ? (Object as TileObject).GetPosition() :
                    (Object is NatureObject ? 
                    new Vector3()
                    { 
                        X = (Object as NatureObject).MapPosX,
                        Z = (Object as NatureObject).MapPosY
                    } : new Vector3());

                float x = pos.X - maprec.X;
                float y = pos.Z - maprec.Y;

                x = rec.X + x * rec.Width / maprec.Width;
                y = rec.Height - (rec.Y + y * rec.Height / maprec.Height);

                this.Location = new Point((int)x, (int)y);
            }

            public override Color BackColor
            {
                get
                {
                    return Object is NatureObject ? Color.Green : 
                        (Object is FieldObject ? Color.Red :
                        (Object is House ? Color.Blue : Color.Orange));
                }
                set
                {
                }
            }
        }

        private class SelectPanelObj : Panel
        {
            public SelectPanelObj()
            {
                SetStyle(
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.UserPaint |
                    ControlStyles.DoubleBuffer, true);
            }
        }

        private void ObjectSelector_FormClosing(object sender, FormClosingEventArgs e)
        {
            Unload();
        }

        private void selectPanel_MouseMove(object sender, MouseEventArgs arg)
        {
            if (selectPanel.BackgroundImage != null && !ZoomedTile.HasValue)
            {
                Rectangle bgrec = GetBackgroundImageRect();

                int w = bgrec.Width / (TilesRec.Width - TilesRec.X + 1);
                int h = bgrec.Height / (TilesRec.Height - TilesRec.Y + 1);

                int y = TilesRec.Height - TilesRec.Y;
                for (int ty = TilesRec.Y; ty <= TilesRec.Height; ty++, y--)
                {
                    int x = 0;
                    for (int tx = TilesRec.X; tx <= TilesRec.Width; tx++, x++)
                    {
                        Rectangle rec = new Rectangle(
                            bgrec.X + x * w,
                            bgrec.Y + y * h,
                            w,
                            h);

                        if (rec.Contains(arg.X, arg.Y))
                        {
                            if (!MouseOverTile.HasValue || (MouseOverTile.Value != new Point(tx, ty)))
                            {
                                MouseOverTile = new Point(tx, ty);
                                Refresh();
                                return;
                            }
                            else
                                return;
                        }
                    }
                }

                MouseOverTile = null;
                Refresh();
            }
        }

        private void selectPanel_MouseEnter(object sender, EventArgs e)
        {
            selectPanel_MouseMove(sender, new MouseEventArgs(MouseButtons.None, 0, MousePosition.X, MousePosition.Y, 0));
        }

        private void selectPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (MouseOverTile.HasValue && !ZoomedTile.HasValue)
                ZoomIn(MouseOverTile.Value);
            else if (ZoomedTile.HasValue && e.Button == MouseButtons.Right)
                ZoomOut();
        }

        private void selectPanel_MouseLeave(object sender, EventArgs e)
        {
            MouseOverTile = null;
        }

        private void selectPanel_Resize(object sender, EventArgs e)
        {
            if (ZoomedTile.HasValue)
            {
                Rectangle rec = GetBackgroundImageRect();
                RectangleF maprec = new RectangleF(
                    (ZoomedTile.Value.X - 1) * TileObject.TileSizes.X + TileObject.TileSizes.X / 2,
                    (ZoomedTile.Value.Y - 1) * TileObject.TileSizes.Y + TileObject.TileSizes.Y / 2,
                    TileObject.TileSizes.X * 2,
                    TileObject.TileSizes.Y * 2);

                foreach (MapObj ob in Objects)
                {
                    ob.UpdatePos(rec, maprec);
                }
            }
        }

        #region Object limit
        bool objLimitBarChanging = false;
        int? objLimit = null;

        private void objectLimitBar_MouseDown(object sender, MouseEventArgs e)
        {
            objLimitBarChanging = true;
        }

        private void objectLimitBar_MouseUp(object sender, MouseEventArgs e)
        {
            ChangeObjectLimit();
        }

        private void objectLimitBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (objLimitBarChanging)
            {
                double pos = (double)e.X / (double)objectLimitBar.Width;
                int val = (int)Math.Floor(objectLimitBar.Minimum + pos * (objectLimitBar.Maximum - objectLimitBar.Minimum));
                
                if (val > objectLimitBar.Maximum)
                    val = objectLimitBar.Maximum; // this can happen if the move event is called out with a ui lag.
                else if (val < objectLimitBar.Minimum)
                    val = objectLimitBar.Minimum;

                objectLimitBar.Value = val;
                string limit = val == objectLimitBar.Maximum ? "unlimited" : val.ToString();
                objectLimitTip.Show("Limit: " + limit, objectLimitBar);
            }
        }

        private void objectLimitBar_MouseLeave(object sender, EventArgs e)
        {
            ChangeObjectLimit();
            objectLimitTip.Hide(objectLimitBar);
        }

        private void ChangeObjectLimit()
        {
            objLimitBarChanging = false;

            if (objectLimitBar.Value == objectLimitBar.Maximum)
                objLimit = null;
            else
                objLimit = objectLimitBar.Value;
        }

        private void objectLimitBar_MouseEnter(object sender, EventArgs e)
        {
            string limit = objectLimitBar.Value == objectLimitBar.Maximum ? "unlimited" : objectLimitBar.Value.ToString();
            objectLimitTip.Show("Limit: " + limit, objectLimitBar);
        }
        #endregion

        private void UpdateHiddenCount()
        {
            int over = 0;

            if (objLimit.HasValue && OpenScene != null && ZoomedTile.HasValue)
            {
                RectangleF maprec = new RectangleF(
                    (ZoomedTile.Value.X - 1) * TileObject.TileSizes.X + TileObject.TileSizes.X / 2,
                    (ZoomedTile.Value.Y - 1) * TileObject.TileSizes.Y + TileObject.TileSizes.Y / 2,
                    TileObject.TileSizes.X * 2,
                    TileObject.TileSizes.Y * 2);

                if (chkHouse.Checked)
                    over += OpenScene.Houses.Where(h => maprec.Contains(h.GetPosition().X, h.GetPosition().Y)).Count();
                if (chkNature.Checked)
                    over += OpenScene.NatureObjects.Where(h => maprec.Contains(h.MapPosX, h.MapPosY)).Count();
                if (chkObj.Checked)
                    over += OpenScene.FieldObjects.Where(h => maprec.Contains(h.GetPosition().X, h.GetPosition().Y)).Count();
                if (chkEff.Checked)
                    over += OpenScene.Effects.Where(h => maprec.Contains(h.GetPosition().X, h.GetPosition().Y)).Count();
                if (chkEff.Checked)
                    over += OpenScene.SectorLights.Where(h => maprec.Contains(h.GetPosition().X, h.GetPosition().Y)).Count();

                over -= Objects.Count;
            }

            lblLimit.Text = Objects.Count + "/" + (over + Objects.Count).ToString();
        }

        private void ToggleMapObjects(Type type, bool visible)
        {
            if (ZoomedTile.HasValue && OpenScene != null)
            {
                this.SuspendLayout();
                if (visible)
                {
                    Rectangle rec = GetBackgroundImageRect();
                    RectangleF maprec = new RectangleF(
                        (ZoomedTile.Value.X - 1) * TileObject.TileSizes.X + TileObject.TileSizes.X / 2,
                        (ZoomedTile.Value.Y - 1) * TileObject.TileSizes.Y + TileObject.TileSizes.Y / 2,
                        TileObject.TileSizes.X * 2,
                        TileObject.TileSizes.Y * 2);


                    if (type == typeof(House))
                        AddHouses(rec, maprec);
                    else if (type == typeof(NatureObject))
                        AddNature(rec, maprec);
                    else if (type == typeof(FieldObject))
                        AddObj(rec, maprec);
                    else if (type == typeof(Effect))
                        AddEff(rec, maprec);
                }
                else
                {
                    foreach (MapObj mo in Objects.Where(mp => mp.Object.GetType() == type).ToArray())
                    {
                        selectPanel.Controls.Remove(mo);
                        Objects.Remove(mo);
                        mo.Dispose();
                    }
                }
                this.ResumeLayout(true);

                UpdateHiddenCount();
            }
        }

        private void chkHouse_CheckedChanged(object sender, EventArgs e)
        {
            ToggleMapObjects(typeof(House), chkHouse.Checked);
        }

        private void chkNature_CheckedChanged(object sender, EventArgs e)
        {
            ToggleMapObjects(typeof(NatureObject), chkNature.Checked);
        }

        private void chkObj_CheckedChanged(object sender, EventArgs e)
        {
            ToggleMapObjects(typeof(FieldObject), chkObj.Checked);
        }

        private void chkEff_CheckedChanged(object sender, EventArgs e)
        {
            ToggleMapObjects(typeof(Effect), chkEff.Checked);
        }
    }
}
