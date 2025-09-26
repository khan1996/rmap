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
    public partial class OptionsForm : Form
    {
        public OptionsForm()
        {
            InitializeComponent();

            nmTextureSize.Value = rMap.Properties.Settings.Default.TextureSize;
            txtRylFolder.Text = rMap.Properties.Settings.Default.RYLFolder;
            txtCryptoKey.CryptoSerialized = rMap.Properties.Settings.Default.CryptoKey;
            nmObjectsLoadingRange.Value = rMap.Properties.Settings.Default.ObjectsViewerLoadingRange;
            cmbDeviceProfile.SelectedIndex = rMap.Properties.Settings.Default.DeviceProfileReach ? 1 : 0;
        }

        private void btnBrowseRylFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtRylFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtRylFolder.Text))
                MessageBox.Show("RYL Folder cannot be empty");
            else
            {
                rMap.Properties.Settings.Default.RYLFolder = txtRylFolder.Text;
                rMap.Properties.Settings.Default.CryptoKey = txtCryptoKey.CryptoSerialized;
                rMap.Properties.Settings.Default.TextureSize = (int)nmTextureSize.Value;
                rMap.Properties.Settings.Default.ObjectsViewerLoadingRange = (int)nmObjectsLoadingRange.Value;
                rMap.Properties.Settings.Default.DeviceProfileReach = cmbDeviceProfile.SelectedIndex == 1;

                if (rMapForm.Instance.MeshWindow != null && rMapForm.Instance.MeshWindow.Game != null)
                    rMapForm.Instance.MeshWindow.Game.SelectReachProfile(rMap.Properties.Settings.Default.DeviceProfileReach);

                rMap.Properties.Settings.Default.Save();
                Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
