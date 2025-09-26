using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using rMap.Zalla;
using System.IO;

namespace rMap.Editors
{
    public partial class ScenePanel : UserControl
    {
        protected event EventHandler Changed;
        Scene scene;
        float[,] map = null;

        public ScenePanel(Scene val, EventHandler eventChanged)
        {
            Changed += eventChanged;
            scene = val;

            InitializeComponent();
            FillConverters();

            txtDef.Text = val.Definition;
            nmTexZone.Value = val.TextureZone;
            nmTextureSize.Value = val.TextureSize;

            if (scene.HeightTables.Count > 0)
            {
                int xMax, xMin, yMax, yMin;
                scene.GetHeightMapTileBoundarys(out xMin, out yMin, out xMax, out yMax);

                numFromX.Value = xMin;
                numFromY.Value = yMin;
                numToX.Value = xMax;
                numToY.Value = yMax;

                map = scene.GetHeights((int)numFromX.Value, (int)numFromY.Value, (int)numToX.Value, (int)numToY.Value);
                txtLow.Text = HeightmapConverters.Base.GetLow(map).ToString();
                txtStep.Text = HeightmapConverters.Base.GetStep(map, 255f).ToString();
            }
            else
            {
                grpHeightmap.Enabled = false;
                chkColladaHeightmap.Enabled = chkColladaHeightmap.Checked = false;
            }

            if (Security.Checkin.IsTrial)
                chkColladaHouse.Enabled = chkColladaHouse.Checked = chkColladaNature.Enabled = chkColladaNature.Checked = chkColladaObject.Enabled = chkColladaObject.Checked = false;
        }

        private void FillConverters()
        {
            cmbConverter.Items.Add(new HeightmapConverters.GreyBitmap());
            cmbConverter.Items.Add(new HeightmapConverters.ColorBitmap());
            cmbConverter.Items.Add(new HeightmapConverters.ColorBitmap2());
            cmbConverter.Items.Add(new HeightmapConverters.FloatArray());

            cmbConverter.SelectedIndex = 0;
        }

        private void btnBrowseHeightmap_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtFile.Text))
            {
                string p = Path.GetDirectoryName(txtFile.Text);
                if (Directory.Exists(p))
                    saveFileDialog1.InitialDirectory = p;
            }

            HeightmapConverters.Base converter = cmbConverter.SelectedItem as HeightmapConverters.Base;

            saveFileDialog1.Filter = converter.Name + " (*." + converter.Extension + ")|*." + converter.Extension + "|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtFile.Text = saveFileDialog1.FileName;
                txtFile.BackColor = SystemColors.Window;
            }
        }

        private void btnExportHeightmaps_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFile.Text))
            {
                MessageBox.Show("Enter the file path");
                txtFile.BackColor = Color.Red;
                return;
            }
            else
                txtFile.BackColor = SystemColors.Window;

            map = scene.GetHeights((int)numFromX.Value, (int)numFromY.Value, (int)numToX.Value, (int)numToY.Value);
            txtLow.Text = HeightmapConverters.Base.GetLow(map).ToString();

            HeightmapConverters.Base converter = cmbConverter.SelectedItem as HeightmapConverters.Base;
            txtStep.Text = HeightmapConverters.Base.GetStep(map, converter.Iterations).ToString();

            byte[] data = (cmbConverter.SelectedItem as HeightmapConverters.Base).Export(map);

            System.IO.File.WriteAllBytes(txtFile.Text, data);
        }

        private void btnImportHeightmaps_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFile.Text))
            {
                MessageBox.Show("Enter the file path");
                txtFile.BackColor = Color.Red;
                return;
            }
            else
                txtFile.BackColor = SystemColors.Window;

            byte[] data = System.IO.File.ReadAllBytes(txtFile.Text);
            float[,] map = (cmbConverter.SelectedItem as HeightmapConverters.Base).Import(data, (int)(numToX.Value - numFromX.Value + 1) * 64 + 1, float.Parse(txtLow.Text), float.Parse(txtStep.Text));
            scene.SetHeights(map, (int)numFromX.Value, (int)numFromY.Value, (int)numToX.Value, (int)numToY.Value);

            Changed(scene, null);
        }

        private void cmbConverter_SelectedIndexChanged(object sender, EventArgs e)
        {
            HeightmapConverters.Base converter = cmbConverter.SelectedItem as HeightmapConverters.Base;
            bool usesLowStep = converter.UsesLowAndStep;

            txtLow.Enabled = txtStep.Enabled = usesLowStep;
        }

        private void nmTexZone_ValueChanged(object sender, EventArgs e)
        {
            scene.TextureZone = (int)nmTexZone.Value;
        }

        private void nmTextureSize_ValueChanged(object sender, EventArgs e)
        {
            scene.TextureSize = (int)nmTextureSize.Value;
        }

        private void btnBrowseColladaFile_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtColladaFile.Text))
            {
                string p = Path.GetDirectoryName(txtColladaFile.Text);
                if (Directory.Exists(p))
                    saveFileDialog1.InitialDirectory = p;
            }

            saveFileDialog1.Filter = "Collada files (*.dae)|*.dae|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtColladaFile.Text = saveFileDialog1.FileName;
                txtColladaFile.BackColor = SystemColors.Window;
            }
        }

        private void btnImportCollada_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtColladaFile.Text))
            {
                MessageBox.Show("Enter the file path");
                txtColladaFile.BackColor = Color.Red;
                return;
            }
            else
                txtColladaFile.BackColor = SystemColors.Window;

            Asset.SceneModel model = new Asset.SceneModel(scene, rMap.Properties.Settings.Default.RYLFolder);

            AsyncLoader.Start(delegate
            {
                model.Import(txtColladaFile.Text, getColladaSettings());
            },
            delegate
            {
                rMapForm.Instance.Reload();
            },
            delegate(Exception ex)
            {
                LogDetailView.Instance.AddInfo("Importing a collada model can in alot of cases cause a out-of-memory error. This doesn't mean that you don't have the free memory but that the operating system cannot find a continuous memory block. If you have a memory defgramentation utility, use it. Otherways you can try to reboot your computer or get more memory.\r\n\r\n" + ex.ToString());
            }, true);
        }

        private void btnExportCollada_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtColladaFile.Text))
            {
                MessageBox.Show("Enter the file path");
                txtColladaFile.BackColor = Color.Red;
                return;
            }
            else
                txtColladaFile.BackColor = SystemColors.Window;

            string folder = Path.GetDirectoryName(txtColladaFile.Text);
            string filename = Path.GetFileNameWithoutExtension(txtColladaFile.Text);

            Asset.SceneModel model = new Asset.SceneModel(scene, rMap.Properties.Settings.Default.RYLFolder);

            AsyncLoader.Start(delegate
            {
                model.Export(folder, filename, getColladaSettings());
            },
            null,
            delegate(Exception ex)
            {
                LogDetailView.Instance.AddInfo(ex.ToString());
            }, true);
        }

        private Dictionary<string, bool> getColladaSettings()
        {
            Dictionary<string, bool> dic = new Dictionary<string, bool>();

            foreach (CheckBox box in grpCollada.Controls.OfType<CheckBox>())
            {
                dic.Add(box.Name.Substring("chkCollada".Length).ToLower(), box.Checked);
            }

            return dic;
        }
    }
}
