using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using rMap.Zalla;

namespace rMap.Editors
{
    public partial class NatureObjectEditor : UserControl
    {
        protected event EventHandler Changed;
        protected NatureObject obj;
        private string InfoTemplate;
        private int oldType = -1;

        public NatureObjectEditor(NatureObject no, EventHandler eventChanged)
        {
            Changed += eventChanged;
            Changed += new EventHandler(NatureObjectEditor_Changed);
            obj = no;
            InitializeComponent();

            FillCombo();
            InfoTemplate = lbInfo.Text;
            UpdateGeneralInfo();

            NatureObjType nt = cmbType.Items.OfType<NatureObjType>().SingleOrDefault(no2 => no2.Type == obj.Type);

            if (nt != null)
                cmbType.SelectedItem = nt;
            else
            {
                nmCustomType.Value = obj.Type;
                cmbType.SelectedItem = cmbType.Items.OfType<NatureObjType>().First();
            }

            tilePosEditor1.UnderModColor = Color.Red;
            tilePosEditor1.CurrentTileCords = new System.Drawing.Point(no.TileX, no.TileY);
            tilePosEditor1.SetObjectLocation(no.PosX, no.PosY);
            tilePosEditor1.SetMinimap(obj.Scene.TextureThumbnail);
            tilePosEditor1.AllLoaded();
        }

        void NatureObjectEditor_Changed(object sender, EventArgs e)
        {
            UpdateGeneralInfo();
        }

        private void UpdateGeneralInfo()
        {
            lbInfo.Text = InfoTemplate
                .Replace("?tx", obj.TileX.ToString())
                .Replace("?ty", obj.TileY.ToString())
                .Replace("?px", obj.PosX.ToString())
                .Replace("?py", obj.PosY.ToString())
                .Replace("?ix", (obj.MapPosX / 100).ToString())
                .Replace("?iy", (obj.MapPosY / 100).ToString());

            if (obj.Type != oldType)
            {
                oldType = obj.Type;
            }
        }

        private bool tilePosEditor1_CanMoveTo(int x, int y)
        {
            return obj.Scene.Textures.Count(tt => tt.X == x && tt.Y == y && !string.IsNullOrEmpty(tt.Texture1)) > 0;
        }

        private bool tilePosEditor1_MovingTo(int x, int y)
        {
            obj.TileX = x;
            obj.TileY = y;
            Changed(obj, null);

            Bitmap bmp = obj.Scene.GetTileTexture(rMap.Properties.Settings.Default.RYLFolder, x, y);
            tilePosEditor1.LoadMap(bmp);

            tilePosEditor1.UpdateObjects(
                obj.Scene.NatureObjects
                    .Where(no => no.TileX == x && no.TileY == y && no != obj)
                    .Select(no2 => new TilePosEditor.TileEditorObj()
                    {
                        color = Color.Blue,
                        X = no2.PosX,
                        Y = no2.PosY,
                        selectable = true
                    })
                 );

            return true;
        }

        private void FillCombo()
        {
            cmbType.Items.AddRange(new object[]{
                new NatureObjType(-1, "Custom"),
                new NatureObjType(0, "Normal tree 1"),
                new NatureObjType(1, "Normal tree 2"),
                new NatureObjType(2, "Normal tree 3"),
                new NatureObjType(3, "Normal tree 4"),
                new NatureObjType(4, "Normal tree 5"),
                new NatureObjType(5, "Normal tree 6"),

                new NatureObjType(6, "Big tree 1"),
                new NatureObjType(7, "Big tree 2"),
                new NatureObjType(8, "Big tree 3"),
                new NatureObjType(9, "Big tree 4"),
                new NatureObjType(10, "Big tree 5"),
                new NatureObjType(11, "Big tree 6"),

                new NatureObjType(12, "Small tree 1"),
                new NatureObjType(13, "Small tree 2"),
                new NatureObjType(14, "Small tree 3"),
                new NatureObjType(15, "Small tree 4"),
                new NatureObjType(16, "Small tree 5"),

                new NatureObjType(17, "Fluffy tree 1"),
                new NatureObjType(18, "Fluffy tree 2"),
                new NatureObjType(19, "Fluffy tree 3"),

                new NatureObjType(20, "Huge tree 1"),
                new NatureObjType(21, "Huge tree 2"),
                new NatureObjType(22, "Huge tree 3"),

                new NatureObjType(23, "Odd tree 1"),
                new NatureObjType(24, "Odd tree 2"),
                new NatureObjType(25, "Odd tree 3"),

                new NatureObjType(26, "Tiny tree 1"),
                new NatureObjType(27, "Tiny tree 2"),
                new NatureObjType(28, "Tiny tree 3"),
                new NatureObjType(29, "Tiny tree 4")
            });
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            NatureObjType t = cmbType.SelectedItem as NatureObjType;

            if (t.Type < 0 && !nmCustomType.Visible)
                nmCustomType.Visible = true;
            else if (t.Type >= 0 && nmCustomType.Visible)
                nmCustomType.Visible = false;

            if (t.Type >= 0)
            {
                obj.Type = (byte)t.Type;
                nmCustomType.Value = obj.Type;
                Changed(obj, null);
            }
            else
            {
                obj.Type = (byte)nmCustomType.Value;
                Changed(obj, null);
            }
        }

        private void nmCustomType_ValueChanged(object sender, EventArgs e)
        {
            obj.Type = (byte)nmCustomType.Value;
            Changed(obj, null);
        }

        private bool tilePosEditor1_ObjectMoving(int x, int y)
        {
            obj.PosX = (byte)x;
            obj.PosY = (byte)y;

            Changed(obj, null);

            return true;
        }

        private bool tilePosEditor1_ObjectSelected(int x, int y)
        {
            NatureObject no = obj.Scene.NatureObjects.Single(n => n.TileX == obj.TileX && n.TileY == obj.TileY && n.PosX == x && n.PosY == y);
            rMapForm rm = rMapForm.ActiveForm as rMapForm;
            rm.treeObjects.Select(no);
            return true;
        }
    }

    class NatureObjType : IComparable<NatureObjType>, IEquatable<NatureObjType>
    {
        public NatureObjType(int type, string name)
        {
            Type = type;
            Name = name;
        }

        public int Type;
        public string Name;

        public override string ToString()
        {
            return Name + (Type > 0 ? " (" + Type + ")" : "");
        }

        #region IComparable<NatureObjType> Members

        public int CompareTo(NatureObjType other)
        {
            return Type.CompareTo(other.Type);
        }

        #endregion

        #region IEquatable<NatureObjType> Members

        public bool Equals(NatureObjType other)
        {
            return Type == other.Type;
        }

        #endregion
    }
}
