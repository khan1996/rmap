using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace rMap
{
    public partial class NewObject : Form
    {
        public NewObject()
        {
            InitializeComponent();
        }

        public static Type Get(IEnumerable<KeyValuePair<string, Type>> ofType)
        {
            NewObject obj = new NewObject();
            obj.comboBox1.Items.AddRange(ofType.Select(kv => kv.Key).ToArray());

            DialogResult res = obj.ShowDialog();

            if (res == DialogResult.OK)
            {
                string item = obj.comboBox1.SelectedItem as string;

                return ofType.Single(kv => kv.Key == item).Value;
            }
            else
                return null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select a type", "Not selected", MessageBoxButtons.OK);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
