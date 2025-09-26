using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Text;

namespace rMap.Editors
{
    class TilePosEditorObj : Control
    {
        public event EventHandler Moved;
        public bool CanBeSelected;
        private Point Loc;
        public bool CanBeMoved;

        bool objMoving = false;
        Point movingStart;

        public TilePosEditorObj()
        {
            base.MouseDown += new MouseEventHandler(TilePosEditorObj_MouseDown);
            base.MouseUp += new MouseEventHandler(TilePosEditorObj_MouseUp);
            base.MouseMove += new MouseEventHandler(TilePosEditorObj_MouseMove);
        }

        void TilePosEditorObj_MouseMove(object sender, MouseEventArgs e)
        {
            if (objMoving == true)
            {
                Point ne = new Point(RealLoc.X + e.Location.X - movingStart.X, RealLoc.Y + e.Location.Y - movingStart.Y);

                if (ne.X >= 0 && ne.Y >= 0 && ne.X <= ParentSize.Width && ne.Y <= ParentSize.Height)
                {
                    RealLoc = ne;
                }
            }
        }

        void TilePosEditorObj_MouseUp(object sender, MouseEventArgs e)
        {
            if (objMoving)
            {
                objMoving = false;

                Point newLoc = RealToObj(RealLoc);

                if (newLoc != Loc)
                {
                    Loc = newLoc;
                    if (Moved != null)
                        Moved(this, null);
                }

                RealLoc = ObjToReal(Loc);
            }
        }

        void TilePosEditorObj_MouseDown(object sender, MouseEventArgs e)
        {
            if (CanBeMoved)
            {
                objMoving = true;
                movingStart = e.Location;
            }
        }

        public Size ParentSize;

        public Color Color
        {
            get
            {
                return base.BackColor;
            }

            set
            {
                base.BackColor = value;
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(5, 5);
            }
        }

        public Point ObjToReal(Point pos)
        {
            return new Point((int)((double)pos.X * (double)ParentSize.Width / 65d), (int)((double)(65 - pos.Y) * (double)ParentSize.Height / 65d));
        }

        public Point RealToObj(Point pos)
        {
            return new Point((int)Math.Round((double)pos.X * 65d / (double)ParentSize.Width), 65 - (int)Math.Round((double)pos.Y * 65d / (double)ParentSize.Height));
        }

        public Point RealLoc
        {
            get
            {
                return base.Location;
            }
            set
            {
                base.Location = value;
            }
        }

        public new Point Location
        {
            get
            {
                return Loc;
            }
            set
            {
                Point r = ObjToReal(value);
                Loc = value;
                base.Location = new Point(r.X - (Size.Width - 1) / 2, r.Y - (Size.Height - 1) / 2);
            }
        }
    }
}
