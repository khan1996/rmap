namespace rMap
{
    partial class OptionsForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsForm));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnBrowseRylFolder = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.nmTextureSize = new System.Windows.Forms.NumericUpDown();
            this.txtRylFolder = new System.Windows.Forms.TextBox();
            this.txtCryptoKey = new rMap.CryptoPanel();
            this.nmObjectsLoadingRange = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbDeviceProfile = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nmTextureSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmObjectsLoadingRange)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(442, 159);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(67, 20);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(369, 159);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(67, 20);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "R.Y.L folder";
            // 
            // btnBrowseRylFolder
            // 
            this.btnBrowseRylFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseRylFolder.Location = new System.Drawing.Point(442, 22);
            this.btnBrowseRylFolder.Name = "btnBrowseRylFolder";
            this.btnBrowseRylFolder.Size = new System.Drawing.Size(67, 20);
            this.btnBrowseRylFolder.TabIndex = 4;
            this.btnBrowseRylFolder.Text = "Browse";
            this.btnBrowseRylFolder.UseVisualStyleBackColor = true;
            this.btnBrowseRylFolder.Click += new System.EventHandler(this.btnBrowseRylFolder_Click);
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "RYL Folder location";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Crypto Key";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Default texture size";
            // 
            // nmTextureSize
            // 
            this.nmTextureSize.Location = new System.Drawing.Point(115, 78);
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
            this.nmTextureSize.TabIndex = 8;
            this.nmTextureSize.Value = new decimal(new int[] {
            256,
            0,
            0,
            0});
            // 
            // txtRylFolder
            // 
            this.txtRylFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRylFolder.Location = new System.Drawing.Point(81, 22);
            this.txtRylFolder.Name = "txtRylFolder";
            this.txtRylFolder.Size = new System.Drawing.Size(355, 20);
            this.txtRylFolder.TabIndex = 2;
            // 
            // txtCryptoKey
            // 
            this.txtCryptoKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCryptoKey.Crypto = new byte[] {
        ((byte)(99)),
        ((byte)(106)),
        ((byte)(115)),
        ((byte)(103))};
            this.txtCryptoKey.CryptoSerialized = "0,Y2pzZw==";
            this.txtCryptoKey.CryptoType = 0;
            this.txtCryptoKey.Location = new System.Drawing.Point(81, 48);
            this.txtCryptoKey.Name = "txtCryptoKey";
            this.txtCryptoKey.Size = new System.Drawing.Size(355, 24);
            this.txtCryptoKey.TabIndex = 5;
            // 
            // nmObjectsLoadingRange
            // 
            this.nmObjectsLoadingRange.Location = new System.Drawing.Point(191, 104);
            this.nmObjectsLoadingRange.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.nmObjectsLoadingRange.Name = "nmObjectsLoadingRange";
            this.nmObjectsLoadingRange.Size = new System.Drawing.Size(67, 20);
            this.nmObjectsLoadingRange.TabIndex = 10;
            this.nmObjectsLoadingRange.Value = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 106);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(173, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Object viewer nearby models range";
            // 
            // cmbDeviceProfile
            // 
            this.cmbDeviceProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDeviceProfile.FormattingEnabled = true;
            this.cmbDeviceProfile.Items.AddRange(new object[] {
            "Hi-Def",
            "Reach"});
            this.cmbDeviceProfile.Location = new System.Drawing.Point(137, 130);
            this.cmbDeviceProfile.Name = "cmbDeviceProfile";
            this.cmbDeviceProfile.Size = new System.Drawing.Size(121, 21);
            this.cmbDeviceProfile.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 133);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(98, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "XNA Device Profile";
            // 
            // OptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(521, 191);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbDeviceProfile);
            this.Controls.Add(this.nmObjectsLoadingRange);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.nmTextureSize);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtCryptoKey);
            this.Controls.Add(this.btnBrowseRylFolder);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtRylFolder);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OptionsForm";
            this.Text = "Options";
            ((System.ComponentModel.ISupportInitialize)(this.nmTextureSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmObjectsLoadingRange)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.TextBox txtRylFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnBrowseRylFolder;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label label2;
        private rMap.CryptoPanel txtCryptoKey;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nmTextureSize;
        private System.Windows.Forms.NumericUpDown nmObjectsLoadingRange;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbDeviceProfile;
        private System.Windows.Forms.Label label5;
    }
}