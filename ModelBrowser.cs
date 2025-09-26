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
    public partial class ModelBrowser : Form
    {
        public event Show3D View;

        public ModelBrowser()
        {
            InitializeComponent();

            txtFolder.Text = rMap.Properties.Settings.Default.RYLFolder + "\\objects\\object";
        }

        private void txtFolder_TextChanged(object sender, EventArgs e)
        {
            if (lstItems.Items.Count > 0)
                lstItems.Items.Clear();

            try
            {
                OpenFolder(txtFolder.Text);
            }
            catch (Exception)
            {
            }
        }

        private void OpenFolder(string folder)
        {
            string[] files = System.IO.Directory.GetFiles(folder, "*.r3s", System.IO.SearchOption.TopDirectoryOnly);

            foreach (string file in files)
            {
                lstItems.Items.Add(System.IO.Path.GetFileNameWithoutExtension(file));
            }
        }

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            if (System.IO.Directory.Exists(txtFolder.Text))
                folderBrowserDialog1.SelectedPath = txtFolder.Text;

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void lstItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (View != null)
                View(System.IO.Path.Combine(txtFolder.Text, (string)lstItems.SelectedItem + ".r3s"));
        }
    }
}
