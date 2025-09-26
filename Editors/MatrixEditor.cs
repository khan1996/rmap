using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;

namespace rMap.Editors
{
    public partial class MatrixEditor : UserControl
    {
        public event EventHandler ObjectChanged;

        public MatrixEditor()
        {
            InitializeComponent();

            p1.ValueChanged += new EventHandler(ValTextChanged);
            p2.ValueChanged += new EventHandler(ValTextChanged);
            p3.ValueChanged += new EventHandler(ValTextChanged);

            s1.ValueChanged += new EventHandler(ValTextChanged);
            s2.ValueChanged += new EventHandler(ValTextChanged);
            s3.ValueChanged += new EventHandler(ValTextChanged);

            r1.ValueChanged += new EventHandler(ValTextChanged);
            r2.ValueChanged += new EventHandler(ValTextChanged);
            r3.ValueChanged += new EventHandler(ValTextChanged);
        }

        public Matrix Matrix
        {
            get
            {
                Vector3 pos = new Vector3()
                {
                    X = (float)p1.Value,
                    Y = (float)p2.Value,
                    Z = (float)p3.Value
                },
                rot = new Vector3()
                {
                    X = DegreeToRad((float)r1.Value),
                    Y = DegreeToRad((float)r2.Value),
                    Z = DegreeToRad((float)r3.Value),
                },
                sca = new Vector3()
                {
                    X = (float)s1.Value,
                    Y = (float)s2.Value,
                    Z = (float)s3.Value
                };

                Matrix m = Matrix.CreateScale(sca) * Matrix.CreateRotationX(rot.X) * Matrix.CreateRotationY(rot.Y) * Matrix.CreateRotationZ(rot.Z) * Matrix.CreateTranslation(pos);

                return m;
            }

            set
            {
                Vector3 pos, sca;
                Quaternion rot;
                value.Decompose(out sca, out rot, out pos);

                p1.Value = (decimal)Math.Round(pos.X, 2);
                p2.Value = (decimal)Math.Round(pos.Y, 2);
                p3.Value = (decimal)Math.Round(pos.Z, 2);

                s1.Value = (decimal)Math.Round(sca.X, 3);
                s2.Value = (decimal)Math.Round(sca.Y, 3);
                s3.Value = (decimal)Math.Round(sca.Z, 3);

                Vector3 r = rot.ToEuler();
                r1.Value = (decimal)Math.Round(RadToDegree(r.X), 3);
                r2.Value = (decimal)Math.Round(RadToDegree(r.Y), 3);
                r3.Value = (decimal)Math.Round(RadToDegree(r.Z), 3);
            }
        }

        private float RadToDegree(float f)
        {
            return (float)(f * 180d / Math.PI);
        }

        private float DegreeToRad(float f)
        {
            return (float)(f * Math.PI / 180d);
        }

        private void ValTextChanged(object sender, EventArgs arg)
        {
            if (ObjectChanged != null)
                ObjectChanged(this, null);
        }
    }
}
