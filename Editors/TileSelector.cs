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
    public delegate void PosChange(int x, int y);

    public partial class TileSelector : UserControl
    {
        Bitmap origMap;
        int onecord = 25;
        public event PosChange Selected;

        public TileSelector()
        {
            InitializeComponent();
        }

        public void SetMap(Bitmap mapp)
        {
            this.Size = mapp.Size;
            origMap = mapp;

            CreateCopyMap(mapp);
        }

        private void CreateCopyMap(Bitmap from)
        {
            if (map.Image != null)
                map.Image.Dispose();

            Bitmap bmp = new Bitmap(from);

            
            int zy = bmp.Size.Height / onecord;
            int zx = bmp.Size.Width / onecord;
            using (Graphics gr = Graphics.FromImage(bmp))
            {
                for (int y = 0; y < zy; y ++)
                {
                    for (int x = 0; x < zx; x ++)
                    {
                        gr.DrawRectangle(new Pen(Color.FromArgb(100, 90, 90, 90), 1), x * onecord, y * onecord, onecord, onecord);
                    }
                }


                gr.Save();
            }

            map.Image = bmp;
        }

        private void map_MouseClick(object sender, MouseEventArgs e)
        {
            int left = e.Location.X / onecord + 1;
            int down = (map.Image.Size.Height - e.Location.Y) / onecord + 1;

            if (Selected != null)
                Selected(left, down);
        }

        public void Highlight(int tx, int ty)
        {
            int zy = map.Image.Size.Height / onecord;
            int zx = map.Image.Size.Width / onecord;

            if (tx <= 0 || tx > zx || ty <= 0 || ty > zy)
                return;

            ty = zy - ty;
            tx--;

            CreateCopyMap(origMap);
            using (Graphics gr = Graphics.FromImage(map.Image))
            {
                gr.DrawRectangle(new Pen(Color.FromArgb(190, 255, 90, 0), 1), tx * onecord, ty * onecord, onecord, onecord);

                gr.Save();
            }
            map.Refresh();
        }
    }
}
