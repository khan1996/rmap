namespace rMap.Editors
{
    partial class ScenePanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtDef = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnExportHeightmaps = new System.Windows.Forms.Button();
            this.grpHeightmap = new System.Windows.Forms.GroupBox();
            this.lblStep = new System.Windows.Forms.Label();
            this.txtStep = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtLow = new System.Windows.Forms.TextBox();
            this.cmbConverter = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.numToY = new System.Windows.Forms.NumericUpDown();
            this.numToX = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.numFromY = new System.Windows.Forms.NumericUpDown();
            this.numFromX = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnBrowseHeightmap = new System.Windows.Forms.Button();
            this.txtFile = new System.Windows.Forms.TextBox();
            this.btnImportHeightmaps = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.nmTexZone = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.grpCollada = new System.Windows.Forms.GroupBox();
            this.chkColladaGroundTex = new System.Windows.Forms.CheckBox();
            this.chkColladaWater = new System.Windows.Forms.CheckBox();
            this.chkColladaNature = new System.Windows.Forms.CheckBox();
            this.chkColladaHouse = new System.Windows.Forms.CheckBox();
            this.chkColladaObject = new System.Windows.Forms.CheckBox();
            this.chkColladaHeightmap = new System.Windows.Forms.CheckBox();
            this.btnBrowseColladaFile = new System.Windows.Forms.Button();
            this.txtColladaFile = new System.Windows.Forms.TextBox();
            this.btnImportCollada = new System.Windows.Forms.Button();
            this.btnExportCollada = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.nmTextureSize = new System.Windows.Forms.NumericUpDown();
            this.grpHeightmap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numToY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numToX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFromY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFromX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmTexZone)).BeginInit();
            this.grpCollada.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmTextureSize)).BeginInit();
            this.SuspendLayout();
            // 
            // txtDef
            // 
            this.txtDef.Enabled = false;
            this.txtDef.Location = new System.Drawing.Point(60, 9);
            this.txtDef.Name = "txtDef";
            this.txtDef.Size = new System.Drawing.Size(225, 20);
            this.txtDef.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Definition";
            // 
            // btnExportHeightmaps
            // 
            this.btnExportHeightmaps.Location = new System.Drawing.Point(317, 119);
            this.btnExportHeightmaps.Name = "btnExportHeightmaps";
            this.btnExportHeightmaps.Size = new System.Drawing.Size(62, 24);
            this.btnExportHeightmaps.TabIndex = 2;
            this.btnExportHeightmaps.Text = "Export";
            this.btnExportHeightmaps.UseVisualStyleBackColor = true;
            this.btnExportHeightmaps.Click += new System.EventHandler(this.btnExportHeightmaps_Click);
            // 
            // grpHeightmap
            // 
            this.grpHeightmap.Controls.Add(this.lblStep);
            this.grpHeightmap.Controls.Add(this.txtStep);
            this.grpHeightmap.Controls.Add(this.label7);
            this.grpHeightmap.Controls.Add(this.txtLow);
            this.grpHeightmap.Controls.Add(this.cmbConverter);
            this.grpHeightmap.Controls.Add(this.label6);
            this.grpHeightmap.Controls.Add(this.numToY);
            this.grpHeightmap.Controls.Add(this.numToX);
            this.grpHeightmap.Controls.Add(this.label4);
            this.grpHeightmap.Controls.Add(this.label5);
            this.grpHeightmap.Controls.Add(this.numFromY);
            this.grpHeightmap.Controls.Add(this.numFromX);
            this.grpHeightmap.Controls.Add(this.label3);
            this.grpHeightmap.Controls.Add(this.label2);
            this.grpHeightmap.Controls.Add(this.btnBrowseHeightmap);
            this.grpHeightmap.Controls.Add(this.txtFile);
            this.grpHeightmap.Controls.Add(this.btnImportHeightmaps);
            this.grpHeightmap.Controls.Add(this.btnExportHeightmaps);
            this.grpHeightmap.Location = new System.Drawing.Point(7, 90);
            this.grpHeightmap.Name = "grpHeightmap";
            this.grpHeightmap.Size = new System.Drawing.Size(385, 151);
            this.grpHeightmap.TabIndex = 3;
            this.grpHeightmap.TabStop = false;
            this.grpHeightmap.Text = "Heightmap";
            // 
            // lblStep
            // 
            this.lblStep.AutoSize = true;
            this.lblStep.Location = new System.Drawing.Point(106, 125);
            this.lblStep.Name = "lblStep";
            this.lblStep.Size = new System.Drawing.Size(29, 13);
            this.lblStep.TabIndex = 19;
            this.lblStep.Text = "Step";
            // 
            // txtStep
            // 
            this.txtStep.Location = new System.Drawing.Point(141, 122);
            this.txtStep.Name = "txtStep";
            this.txtStep.Size = new System.Drawing.Size(66, 20);
            this.txtStep.TabIndex = 18;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 125);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(27, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "Low";
            // 
            // txtLow
            // 
            this.txtLow.Location = new System.Drawing.Point(39, 122);
            this.txtLow.Name = "txtLow";
            this.txtLow.Size = new System.Drawing.Size(66, 20);
            this.txtLow.TabIndex = 16;
            // 
            // cmbConverter
            // 
            this.cmbConverter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbConverter.FormattingEnabled = true;
            this.cmbConverter.Location = new System.Drawing.Point(239, 25);
            this.cmbConverter.Name = "cmbConverter";
            this.cmbConverter.Size = new System.Drawing.Size(140, 21);
            this.cmbConverter.TabIndex = 15;
            this.cmbConverter.SelectedIndexChanged += new System.EventHandler(this.cmbConverter_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(180, 28);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Converter";
            // 
            // numToY
            // 
            this.numToY.Location = new System.Drawing.Point(134, 52);
            this.numToY.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numToY.Name = "numToY";
            this.numToY.Size = new System.Drawing.Size(40, 20);
            this.numToY.TabIndex = 13;
            // 
            // numToX
            // 
            this.numToX.Location = new System.Drawing.Point(134, 26);
            this.numToX.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numToX.Name = "numToX";
            this.numToX.Size = new System.Drawing.Size(40, 20);
            this.numToX.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(114, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Y";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(98, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(30, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "To X";
            // 
            // numFromY
            // 
            this.numFromY.Location = new System.Drawing.Point(52, 52);
            this.numFromY.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numFromY.Name = "numFromY";
            this.numFromY.Size = new System.Drawing.Size(40, 20);
            this.numFromY.TabIndex = 9;
            // 
            // numFromX
            // 
            this.numFromX.Location = new System.Drawing.Point(52, 26);
            this.numFromX.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numFromX.Name = "numFromX";
            this.numFromX.Size = new System.Drawing.Size(40, 20);
            this.numFromX.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(32, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Y";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "From X";
            // 
            // btnBrowseHeightmap
            // 
            this.btnBrowseHeightmap.Location = new System.Drawing.Point(317, 89);
            this.btnBrowseHeightmap.Name = "btnBrowseHeightmap";
            this.btnBrowseHeightmap.Size = new System.Drawing.Size(62, 24);
            this.btnBrowseHeightmap.TabIndex = 5;
            this.btnBrowseHeightmap.Text = "Browse";
            this.btnBrowseHeightmap.UseVisualStyleBackColor = true;
            this.btnBrowseHeightmap.Click += new System.EventHandler(this.btnBrowseHeightmap_Click);
            // 
            // txtFile
            // 
            this.txtFile.Location = new System.Drawing.Point(6, 92);
            this.txtFile.Name = "txtFile";
            this.txtFile.Size = new System.Drawing.Size(305, 20);
            this.txtFile.TabIndex = 4;
            // 
            // btnImportHeightmaps
            // 
            this.btnImportHeightmaps.Location = new System.Drawing.Point(249, 119);
            this.btnImportHeightmaps.Name = "btnImportHeightmaps";
            this.btnImportHeightmaps.Size = new System.Drawing.Size(62, 24);
            this.btnImportHeightmaps.TabIndex = 3;
            this.btnImportHeightmaps.Text = "Import";
            this.btnImportHeightmaps.UseVisualStyleBackColor = true;
            this.btnImportHeightmaps.Click += new System.EventHandler(this.btnImportHeightmaps_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.OverwritePrompt = false;
            this.saveFileDialog1.Title = "Export/import file";
            // 
            // nmTexZone
            // 
            this.nmTexZone.Location = new System.Drawing.Point(79, 35);
            this.nmTexZone.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nmTexZone.Name = "nmTexZone";
            this.nmTexZone.Size = new System.Drawing.Size(44, 20);
            this.nmTexZone.TabIndex = 4;
            this.nmTexZone.ValueChanged += new System.EventHandler(this.nmTexZone_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(4, 37);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(69, 13);
            this.label8.TabIndex = 5;
            this.label8.Text = "Texture zone";
            // 
            // grpCollada
            // 
            this.grpCollada.Controls.Add(this.chkColladaGroundTex);
            this.grpCollada.Controls.Add(this.chkColladaWater);
            this.grpCollada.Controls.Add(this.chkColladaNature);
            this.grpCollada.Controls.Add(this.chkColladaHouse);
            this.grpCollada.Controls.Add(this.chkColladaObject);
            this.grpCollada.Controls.Add(this.chkColladaHeightmap);
            this.grpCollada.Controls.Add(this.btnBrowseColladaFile);
            this.grpCollada.Controls.Add(this.txtColladaFile);
            this.grpCollada.Controls.Add(this.btnImportCollada);
            this.grpCollada.Controls.Add(this.btnExportCollada);
            this.grpCollada.Location = new System.Drawing.Point(6, 247);
            this.grpCollada.Name = "grpCollada";
            this.grpCollada.Size = new System.Drawing.Size(386, 127);
            this.grpCollada.TabIndex = 6;
            this.grpCollada.TabStop = false;
            this.grpCollada.Text = "Collada (for 3ds max)";
            // 
            // chkColladaGroundTex
            // 
            this.chkColladaGroundTex.AutoSize = true;
            this.chkColladaGroundTex.Location = new System.Drawing.Point(98, 19);
            this.chkColladaGroundTex.Name = "chkColladaGroundTex";
            this.chkColladaGroundTex.Size = new System.Drawing.Size(96, 17);
            this.chkColladaGroundTex.TabIndex = 15;
            this.chkColladaGroundTex.Text = "Ground texture";
            this.chkColladaGroundTex.UseVisualStyleBackColor = true;
            // 
            // chkColladaWater
            // 
            this.chkColladaWater.AutoSize = true;
            this.chkColladaWater.Location = new System.Drawing.Point(200, 19);
            this.chkColladaWater.Name = "chkColladaWater";
            this.chkColladaWater.Size = new System.Drawing.Size(55, 17);
            this.chkColladaWater.TabIndex = 14;
            this.chkColladaWater.Text = "Water";
            this.chkColladaWater.UseVisualStyleBackColor = true;
            // 
            // chkColladaNature
            // 
            this.chkColladaNature.AutoSize = true;
            this.chkColladaNature.Checked = true;
            this.chkColladaNature.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkColladaNature.Location = new System.Drawing.Point(200, 42);
            this.chkColladaNature.Name = "chkColladaNature";
            this.chkColladaNature.Size = new System.Drawing.Size(58, 17);
            this.chkColladaNature.TabIndex = 13;
            this.chkColladaNature.Text = "Nature";
            this.chkColladaNature.UseVisualStyleBackColor = true;
            // 
            // chkColladaHouse
            // 
            this.chkColladaHouse.AutoSize = true;
            this.chkColladaHouse.Checked = true;
            this.chkColladaHouse.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkColladaHouse.Location = new System.Drawing.Point(98, 42);
            this.chkColladaHouse.Name = "chkColladaHouse";
            this.chkColladaHouse.Size = new System.Drawing.Size(57, 17);
            this.chkColladaHouse.TabIndex = 12;
            this.chkColladaHouse.Text = "House";
            this.chkColladaHouse.UseVisualStyleBackColor = true;
            // 
            // chkColladaObject
            // 
            this.chkColladaObject.AutoSize = true;
            this.chkColladaObject.Checked = true;
            this.chkColladaObject.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkColladaObject.Location = new System.Drawing.Point(7, 42);
            this.chkColladaObject.Name = "chkColladaObject";
            this.chkColladaObject.Size = new System.Drawing.Size(85, 17);
            this.chkColladaObject.TabIndex = 11;
            this.chkColladaObject.Text = "Field objects";
            this.chkColladaObject.UseVisualStyleBackColor = true;
            // 
            // chkColladaHeightmap
            // 
            this.chkColladaHeightmap.AutoSize = true;
            this.chkColladaHeightmap.Checked = true;
            this.chkColladaHeightmap.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkColladaHeightmap.Location = new System.Drawing.Point(7, 19);
            this.chkColladaHeightmap.Name = "chkColladaHeightmap";
            this.chkColladaHeightmap.Size = new System.Drawing.Size(77, 17);
            this.chkColladaHeightmap.TabIndex = 10;
            this.chkColladaHeightmap.Text = "Heightmap";
            this.chkColladaHeightmap.UseVisualStyleBackColor = true;
            // 
            // btnBrowseColladaFile
            // 
            this.btnBrowseColladaFile.Location = new System.Drawing.Point(317, 62);
            this.btnBrowseColladaFile.Name = "btnBrowseColladaFile";
            this.btnBrowseColladaFile.Size = new System.Drawing.Size(62, 24);
            this.btnBrowseColladaFile.TabIndex = 9;
            this.btnBrowseColladaFile.Text = "Browse";
            this.btnBrowseColladaFile.UseVisualStyleBackColor = true;
            this.btnBrowseColladaFile.Click += new System.EventHandler(this.btnBrowseColladaFile_Click);
            // 
            // txtColladaFile
            // 
            this.txtColladaFile.Location = new System.Drawing.Point(6, 65);
            this.txtColladaFile.Name = "txtColladaFile";
            this.txtColladaFile.Size = new System.Drawing.Size(305, 20);
            this.txtColladaFile.TabIndex = 8;
            // 
            // btnImportCollada
            // 
            this.btnImportCollada.Location = new System.Drawing.Point(249, 92);
            this.btnImportCollada.Name = "btnImportCollada";
            this.btnImportCollada.Size = new System.Drawing.Size(62, 24);
            this.btnImportCollada.TabIndex = 7;
            this.btnImportCollada.Text = "Import";
            this.btnImportCollada.UseVisualStyleBackColor = true;
            this.btnImportCollada.Click += new System.EventHandler(this.btnImportCollada_Click);
            // 
            // btnExportCollada
            // 
            this.btnExportCollada.Location = new System.Drawing.Point(317, 92);
            this.btnExportCollada.Name = "btnExportCollada";
            this.btnExportCollada.Size = new System.Drawing.Size(62, 24);
            this.btnExportCollada.TabIndex = 6;
            this.btnExportCollada.Text = "Export";
            this.btnExportCollada.UseVisualStyleBackColor = true;
            this.btnExportCollada.Click += new System.EventHandler(this.btnExportCollada_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(4, 63);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(64, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "Texture size";
            // 
            // nmTextureSize
            // 
            this.nmTextureSize.Location = new System.Drawing.Point(79, 61);
            this.nmTextureSize.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.nmTextureSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nmTextureSize.Name = "nmTextureSize";
            this.nmTextureSize.Size = new System.Drawing.Size(44, 20);
            this.nmTextureSize.TabIndex = 7;
            this.nmTextureSize.Value = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.nmTextureSize.ValueChanged += new System.EventHandler(this.nmTextureSize_ValueChanged);
            // 
            // ScenePanel
            // 
            this.Controls.Add(this.label9);
            this.Controls.Add(this.nmTextureSize);
            this.Controls.Add(this.grpCollada);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.nmTexZone);
            this.Controls.Add(this.grpHeightmap);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtDef);
            this.Name = "ScenePanel";
            this.Size = new System.Drawing.Size(413, 426);
            this.grpHeightmap.ResumeLayout(false);
            this.grpHeightmap.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numToY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numToX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFromY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFromX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmTexZone)).EndInit();
            this.grpCollada.ResumeLayout(false);
            this.grpCollada.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmTextureSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtDef;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnExportHeightmaps;
        private System.Windows.Forms.GroupBox grpHeightmap;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnBrowseHeightmap;
        private System.Windows.Forms.TextBox txtFile;
        private System.Windows.Forms.Button btnImportHeightmaps;
        private System.Windows.Forms.ComboBox cmbConverter;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numToY;
        private System.Windows.Forms.NumericUpDown numToX;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numFromY;
        private System.Windows.Forms.NumericUpDown numFromX;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label lblStep;
        private System.Windows.Forms.TextBox txtStep;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtLow;
        private System.Windows.Forms.NumericUpDown nmTexZone;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox grpCollada;
        private System.Windows.Forms.CheckBox chkColladaHouse;
        private System.Windows.Forms.CheckBox chkColladaObject;
        private System.Windows.Forms.CheckBox chkColladaHeightmap;
        private System.Windows.Forms.Button btnBrowseColladaFile;
        private System.Windows.Forms.TextBox txtColladaFile;
        private System.Windows.Forms.Button btnImportCollada;
        private System.Windows.Forms.Button btnExportCollada;
        private System.Windows.Forms.CheckBox chkColladaWater;
        private System.Windows.Forms.CheckBox chkColladaNature;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown nmTextureSize;
        private System.Windows.Forms.CheckBox chkColladaGroundTex;
    }
}
