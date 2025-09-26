using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace rMap.Editors
{
    public partial class TilePosEditor : UserControl
    {
        public delegate bool CanMoveToDel(int x, int y);
        public event CanMoveToDel CanMoveTo;
        public event CanMoveToDel MovingTo;
        public event CanMoveToDel ObjectMoving;
        public event CanMoveToDel ObjectSelected;

        private TilePosEditorObj objUnderMod = null;
        public Color UnderModColor = Color.Red;
        public Point CurrentTileCords;
        private List<TilePosEditorObj> Ghosts = new List<TilePosEditorObj>();
        private static bool _grid = true;
        private Image originalImg;

        public TilePosEditor()
        {
            InitializeComponent();

            aDown.Visible = false;
            aUp.Visible = false;
            aLeft.Visible = false;
            aRight.Visible = false;
            chkGrid.Checked = _grid;
        }

        public void LoadMap(Image img)
        {
            if (originalImg != null)
                originalImg.Dispose();

            originalImg = img;
            UpdateMap();
        }

        private void UpdateMap()
        {
            if (pBox.Image != null)
                pBox.Image.Dispose();

            Bitmap bmp = new Bitmap(originalImg, pBox.Size);

            if (_grid)
            {
                double onecord = (double)pBox.Size.Width / 65d;
                using (Graphics gr = Graphics.FromImage(bmp))
                {
                    for (double y = 0; y < pBox.Size.Height; y += onecord)
                    {
                        for (double x = 0; x < pBox.Size.Width; x += onecord)
                        {
                            gr.DrawRectangle(new Pen(Color.FromArgb(100, 90, 90, 90), 1), (float)x, (float)y, (float)onecord, (float)onecord);
                        }
                    }


                    gr.Save();
                }
            }

            pBox.Image = bmp;
        }

        public void AllLoaded()
        {
            MoveTile(0);
        }

        public void SetMinimap(Bitmap bmp)
        {
            tileSelect.SetMap(bmp);
        }

        protected bool Grid
        {
            get
            {
                return _grid;
            }

            set
            {
                _grid = value;

                if (originalImg != null)
                    UpdateMap();
            }
        }

        public void SetObjectLocation(int x, int y)
        {
            if (objUnderMod == null)
            {
                objUnderMod = new TilePosEditorObj()
                {
                     Color = UnderModColor,
                     ParentSize = pBox.Size,
                     CanBeMoved = true
                };
                pBox.Controls.Add(objUnderMod);
                objUnderMod.Moved += new EventHandler(objUnderMod_Moved);
            }

            objUnderMod.Location = new Point(x, y);
        }

        void objUnderMod_Moved(object sender, EventArgs e)
        {
            if (ObjectMoving != null)
                ObjectMoving(objUnderMod.Location.X, objUnderMod.Location.Y);
        }

        /// <param name="dir">0 - none, 1 - up, 2 - right, 3- down, 4 - left</param>
        private void MoveTile(int dir)
        {
            if (dir == 1)
                CurrentTileCords.Y++;
            else if (dir == 2)
                CurrentTileCords.X++;
            else if (dir == 3)
                CurrentTileCords.Y--;
            else if (dir == 4)
                CurrentTileCords.X--;

            if (MovingTo != null)
                MovingTo(CurrentTileCords.X, CurrentTileCords.Y);

            if (CanMoveTo != null)
            {
                aDown.Visible = CanMoveTo(CurrentTileCords.X, CurrentTileCords.Y - 1);
                aUp.Visible = CanMoveTo(CurrentTileCords.X, CurrentTileCords.Y + 1);
                aLeft.Visible = CanMoveTo(CurrentTileCords.X - 1, CurrentTileCords.Y);
                aRight.Visible = CanMoveTo(CurrentTileCords.X + 1, CurrentTileCords.Y);
            }

            tileSelect.Highlight(CurrentTileCords.X, CurrentTileCords.Y);
        }

        public void UpdateObjects(IEnumerable<TileEditorObj> objects)
        {
            foreach (TilePosEditorObj old in Ghosts)
            {
                pBox.Controls.Remove(old);
            }
            Ghosts.Clear();

            foreach (TileEditorObj to in objects)
            {
                TilePosEditorObj te = new TilePosEditorObj()
                {
                    Color = to.color,
                    ParentSize = pBox.Size,
                    CanBeSelected = to.selectable
                };
                te.Location = new Point(to.X, to.Y);
                te.Click += new EventHandler(te_Click);

                pBox.Controls.Add(te);
                Ghosts.Add(te);
            }

            if (objUnderMod != null)
                objUnderMod.BringToFront();
        }

        void te_Click(object sender, EventArgs e)
        {
            TilePosEditorObj send = sender as TilePosEditorObj;

            if (send.CanBeSelected && ObjectSelected != null)
                ObjectSelected(send.Location.X, send.Location.Y);
        }

        public class TileEditorObj
        {
            public int X;
            public int Y;
            public Color color;
            public bool selectable;
        }

        private void aUp_Click(object sender, EventArgs e)
        {
            MoveTile(1);
        }

        private void aLeft_Click(object sender, EventArgs e)
        {
            MoveTile(4);
        }

        private void aDown_Click(object sender, EventArgs e)
        {
            MoveTile(3);
        }

        private void aRight_Click(object sender, EventArgs e)
        {
            MoveTile(2);
        }

        private void chkGrid_CheckedChanged(object sender, EventArgs e)
        {
            Grid = chkGrid.Checked;
        }

        private void lnkTileSelect_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (tileSelect.Visible)
                tileSelect.Hide();
            else
                tileSelect.Show();
        }

        private void tileSelect_Selected(int x, int y)
        {
            CurrentTileCords = new Point(x, y);
            MoveTile(0);
        }
    }
}
