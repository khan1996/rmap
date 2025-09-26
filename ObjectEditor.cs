using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
using rMap.Zalla;
using Microsoft.Xna.Framework;
using Color = System.Drawing.Color;

namespace rMap
{
    class ObjectEditor : Panel
    {
        public object Current { get; private set; }
        public event EventHandler ObjectChanged;

        private static Dictionary<Type, Type> ClassMap = new Dictionary<Type, Type>()
        {
            { typeof(Scene), typeof(Editors.ScenePanel) },
            { typeof(HeightTable), typeof(HeightMapPanel) },
            { typeof(House), typeof(FieldPanel) },
            { typeof(FieldObject), typeof(FieldPanel) },
            { typeof(Effect), typeof(FieldPanel) },
            { typeof(LandscapeEffect), typeof(FieldPanel) },
            { typeof(SectorLight), typeof(FieldPanel) },
            { typeof(NatureObject), typeof(Editors.NatureObjectEditor) },
            { typeof(ObjectGroup), typeof(FieldPanel) },
            { typeof(SavedObject), typeof(FieldPanel) },
            { typeof(SavedLight), typeof(FieldPanel) },
            { typeof(WaterData), typeof(FieldPanel) },
            { typeof(TileTexture), typeof(FieldPanel) },
            { typeof(FallMap), typeof(FieldPanel) },
            { typeof(FallmapObj), typeof(FieldPanel) }
        };

        public ObjectEditor()
            : base()
        {

        }

        public static IEnumerable<Type> GetSupportedTypes()
        {
            return ClassMap.Keys;
        }

        public void Load(object obj)
        {
            if (Current != null)
                Close();

            Current = obj;

            try
            {
                Populate(Current);
            }
            catch (Exception ex)
            {
                this.Controls.Add(new Label() { Text = "Error loading the object.\r\n\r\n" + ex.ToString(), AutoSize = true, Dock = DockStyle.Fill });
            }
        }

        public void Close()
        {
            Current = null;
            this.Controls.Clear();
        }

        private void Populate(object obj)
        {
            Type editorContType = ClassMap[obj.GetType()];

            ScrollableControl p = Activator.CreateInstance(editorContType, obj, new EventHandler(object_Changed)) as ScrollableControl;
            p.Dock = DockStyle.Fill;
            p.AutoScroll = true;
            p.AutoScrollMargin = new System.Drawing.Size(5, 5);
            Controls.Add(p);
        }

        private void object_Changed(object sender, EventArgs args)
        {
            if (ObjectChanged != null)
                ObjectChanged(sender, args);
        }
    }

    class NatureObjectPanel : TableLayoutPanel
    {
        protected NatureObject TargetObject;
        protected event EventHandler Changed;

        public NatureObjectPanel(object val, EventHandler eventChanged)
        {
            Changed += eventChanged;
            TargetObject = val as NatureObject;

            RowCount = 2;
            ColumnCount = 1;
            RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120));
            RowStyles.Add(new System.Windows.Forms.RowStyle());

            FieldPanel fp = new FieldPanel(val, eventChanged);
            fp.Dock = DockStyle.Fill;
            fp.AutoScroll = true;
            fp.Changed += new EventHandler(fp_Changed);

            PictureBox pic = new PictureBox()
            {
                Image = GetImage(),
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            Controls.Add(fp, 0, 0);
            Controls.Add(pic, 0, 1);
        }

        void fp_Changed(object sender, EventArgs e)
        {
            UpdatePic();
        }

        private Bitmap GetImage()
        {
            Bitmap bmp = TargetObject.Scene.GetTileTexture(rMap.Properties.Settings.Default.RYLFolder, TargetObject.TileX, TargetObject.TileY);

            int posx = TargetObject.PosX * bmp.Size.Width / 65;
            int posy = TargetObject.PosY * bmp.Size.Height / 65;
            Color c = Color.Red;

            bmp.SetPixel(posx - 1, posy - 1, c);
            bmp.SetPixel(posx, posy - 1, c);
            bmp.SetPixel(posx + 1, posy - 1, c);

            bmp.SetPixel(posx - 1, posy, c);
            bmp.SetPixel(posx, posy, c);
            bmp.SetPixel(posx + 1, posy, c);

            bmp.SetPixel(posx - 1, posy + 1, c);
            bmp.SetPixel(posx, posy + 1, c);
            bmp.SetPixel(posx + 1, posy + 1, c);

            return bmp;
        }

        private void UpdatePic()
        {
            PictureBox pb = Controls.OfType<PictureBox>().First();

            if (pb.Image != null)
                pb.Image.Dispose();

            pb.Image = GetImage();
        }
    }

    class HeightMapPanel : TableLayoutPanel
    {
        protected HeightTable TargetObject;
        protected event EventHandler Changed;

        public HeightMapPanel(object val, EventHandler eventChanged)
        {
            Changed += eventChanged;
            TargetObject = val as HeightTable;

            RowCount = 2;
            ColumnCount = 1;
            RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80));
            RowStyles.Add(new System.Windows.Forms.RowStyle());

            FlowLayoutPanel flow = new FlowLayoutPanel();
            flow.Dock = DockStyle.Fill;
            flow.AutoScroll = true;

            LabeledTextField tilex = new LabeledTextField("Tile x", TargetObject.X, 30);
            LabeledTextField tiley = new LabeledTextField("Tile y", TargetObject.Y, 30);
            tilex.Changed += new LabeledTextField.TextChangeDel(tilex_Changed);
            tiley.Changed += new LabeledTextField.TextChangeDel(tiley_Changed);

            LabeledTextField minHeight = new LabeledTextField("Min height", TargetObject.GetLowest(), 70);
            LabeledTextField maxHeight = new LabeledTextField("Max height", TargetObject.GetHighest(), 70);

            minHeight.tb.Enabled = false;
            maxHeight.tb.Enabled = false;

            flow.Controls.Add(tilex);
            flow.Controls.Add(tiley);
            flow.Controls.Add(minHeight);
            flow.Controls.Add(maxHeight);

            PictureBox pic = new PictureBox()
            {
                Image = TargetObject.GetImage(),
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            Controls.Add(flow, 0, 0);
            Controls.Add(pic, 0, 1);
        }

        void tiley_Changed(LabeledTextField sender, string newValue)
        {
            try
            {
                TargetObject.Y = (int)Convert.ChangeType(newValue, typeof(int));
                sender.BackColor = System.Drawing.SystemColors.Window;

                Changed(TargetObject, null);
            }
            catch
            {
                sender.BackColor = System.Drawing.Color.Red;
            } 
        }

        void tilex_Changed(LabeledTextField sender, string newValue)
        {
            try
            {
                TargetObject.X = (int)Convert.ChangeType(newValue, typeof(int));
                sender.BackColor = System.Drawing.SystemColors.Window;

                Changed(TargetObject, null);
            }
            catch
            {
                sender.BackColor = System.Drawing.Color.Red;
            } 
        }
    }

    class LabeledTextField : Panel
    {
        public delegate void TextChangeDel(LabeledTextField sender, string newValue);
        public event TextChangeDel Changed;
        public TextBox tb;

        public LabeledTextField(string label, object val, int txtSize)
        {
            Label l = new Label()
            {
                Text = label,
                Location = new System.Drawing.Point(2, 2),
                Size = new System.Drawing.Size(60, 20),
                AutoSize = false,
                 AutoEllipsis = true
            };

            tb = new TextBox()
            {
                Location = new System.Drawing.Point(60, 1),
                Size = new System.Drawing.Size(txtSize, 20),
                Text = val != null ? val.ToString() : ""
            };
            tb.TextChanged += new EventHandler(tb_TextChanged);

            Controls.Add(tb);
            Controls.Add(l);

            Size = new System.Drawing.Size(60 + txtSize + 2, 24);
        }

        private void tb_TextChanged(object sender, EventArgs e)
        {
            if (Changed != null)
                Changed(this, (sender as TextBox).Text);
        }
    }

    class FieldPanel : FlowLayoutPanel
    {
        public event EventHandler Changed; 
        protected object TargetObject;
        public delegate void FieldChangedDelegate(FieldInfo field, object newValue);

        public FieldPanel(object obj, EventHandler eventChanged)
        {
            TargetObject = obj;
            Changed += eventChanged;

            FieldInfo[] fields = obj.GetType().GetFields();

            foreach (FieldInfo fi in fields)
            {
                PrivateAttribute pa = fi.GetCustomAttributes(typeof(PrivateAttribute), false).SingleOrDefault() as PrivateAttribute;

                if (pa != null)
                    continue;

                FieldPanelRow row = new FieldPanelRow(fi, fi.FieldType, fi.GetValue(obj));
                row.FieldChanged += new FieldChangedDelegate(row_FieldChanged);
                Controls.Add(row);
            }
        }

        private void row_FieldChanged(FieldInfo field, object newValue)
        {
            field.SetValue(TargetObject, newValue);
            Changed(TargetObject, null);
        }
    }

    class FieldPanelRow : Panel
    {
        private FieldInfo Field;
        private Type TargetType;
        private object OldValue;

        public event FieldPanel.FieldChangedDelegate FieldChanged;

        public FieldPanelRow(FieldInfo field, Type t, object val)
        {
            Field = field;
            TargetType = t;
            OldValue = val;

            int height = 24;
            int width = 262;
            string name = field.Name;

            Label l = new Label()
            {
                Text = name,
                Location = new System.Drawing.Point(2, 2),
                Size = new System.Drawing.Size(58, 20)
            };

            if (t == typeof(Vector3))
            {
                Vector3 vf = (Vector3)val;

                TextBox tb1 = new TextBox()
                {
                    Location = new System.Drawing.Point(60, 1),
                    Size = new System.Drawing.Size(60, 20), // 60 * 3 + 10 * 2 = 180+20
                    Text = vf.X.ToString(),
                    Tag = 'X'
                };
                tb1.TextChanged += new EventHandler(vector_TextChanged);

                TextBox tb2 = new TextBox()
                {
                    Location = new System.Drawing.Point(60 + 60 + 10, 1),
                    Size = new System.Drawing.Size(60, 20), // 60 * 3 + 10 * 2 = 180+20
                    Text = vf.Y.ToString(),
                    Tag = 'Y'
                };
                tb2.TextChanged += new EventHandler(vector_TextChanged);

                TextBox tb3 = new TextBox()
                {
                    Location = new System.Drawing.Point(60 + (60 + 10) * 2, 1),
                    Size = new System.Drawing.Size(60, 20), // 60 * 3 + 10 * 2 = 180+20
                    Text = vf.Z.ToString(),
                    Tag = 'Z'
                };
                tb3.TextChanged += new EventHandler(vector_TextChanged);

                Controls.Add(tb1);
                Controls.Add(tb2);
                Controls.Add(tb3);
            }
            else if (t == typeof(Matrix))
            {
                Editors.MatrixEditor medit = new rMap.Editors.MatrixEditor()
                {
                    Location = new System.Drawing.Point(60, 0),
                    Matrix = (Matrix)val
                };
                medit.ObjectChanged += new EventHandler(medit_ObjectChanged);

                Controls.Add(medit);
                height = medit.Height;
                width = 60 + medit.Width;
            }
            else if (t.IsArray)
            {
                Array ar = val as Array;

                for (int i = 0; i < ar.Length; i++)
                {
                    object ar_val = ar.GetValue(i);
                    TextBox tb = new TextBox()
                    {
                        Location = new System.Drawing.Point(60, 1 + i * 22),
                        Size = new System.Drawing.Size(200, 20),
                        Text = ar_val != null ? ar_val.ToString() : "",
                        Tag = i
                    };
                    tb.TextChanged += new EventHandler(array_TextChanged);
                    Controls.Add(tb);
                }
                height = 2 + ar.Length * 22;
            }
            else if (t == typeof(bool))
            {
                CheckBox tb = new CheckBox()
                {
                    Location = new System.Drawing.Point(60, -1),
                    Checked = (bool)val
                };
                tb.CheckedChanged += new EventHandler(tb_CheckedChanged);
                Controls.Add(tb);
            }
            else
            {
                if (Field.Name.ToLower().Contains("color"))
                    val = "0x" + Convert.ToString((uint)val, 16);

                TextBox tb = new TextBox()
                {
                    Location = new System.Drawing.Point(60, 1),
                    Size = new System.Drawing.Size(200, 20),
                    Text = val != null ? val.ToString() : ""
                };
                tb.TextChanged += new EventHandler(single_TextChanged);
                Controls.Add(tb);
            }
            Controls.Add(l);

            Size = new System.Drawing.Size(width, height);
        }

        private void tb_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox send = sender as CheckBox;
            FieldChanged(Field, send.Checked);
        }

        private void medit_ObjectChanged(object sender, EventArgs e)
        {
            Editors.MatrixEditor ed = sender as Editors.MatrixEditor;
            FieldChanged(Field, ed.Matrix);
        }

        private void single_TextChanged(object sender, EventArgs e)
        {
            TextBox send = sender as TextBox;
            try
            {
                object ob = TargetType == typeof(int) ?
                    send.Text.ToInt() :
                    (TargetType == typeof(uint) ? send.Text.ToUInt() :
                    Convert.ChangeType(send.Text, TargetType));

                send.BackColor = System.Drawing.SystemColors.Window;

                FieldChanged(Field, ob);
            }
            catch
            {
                send.BackColor = System.Drawing.Color.Red;
            } 
        }

        private void array_TextChanged(object sender, EventArgs e)
        {
            TextBox send = sender as TextBox;
            int index = (int)send.Tag;
            Array arr = OldValue as Array;

            try
            {
                object ob = Convert.ChangeType(send.Text, TargetType.GetElementType());
                send.BackColor = System.Drawing.SystemColors.Window;

                arr.SetValue(ob, index);
                FieldChanged(Field, arr);
            }
            catch
            {
                send.BackColor = System.Drawing.Color.Red;
            }
        }

        private void vector_TextChanged(object sender, EventArgs e)
        {
            TextBox send = sender as TextBox;
            char index = (char)send.Tag;
            Vector3 old = (Vector3)OldValue;

            try
            {
                float ob = (float)Convert.ChangeType(send.Text, typeof(float));
                send.BackColor = System.Drawing.SystemColors.Window;

                if (index == 'X')
                    old.X = ob;
                else if (index == 'Z')
                    old.Z = ob;
                else if (index == 'Y')
                    old.Y = ob;

                OldValue = old;
                FieldChanged(Field, old);
            }
            catch
            {
                send.BackColor = System.Drawing.Color.Red;
            }
        }
    }
}
