namespace rMap
{
    partial class rMapForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(rMapForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.recentClearSplit1 = new System.Windows.Forms.ToolStripSeparator();
            this.recentClearSplit2 = new System.Windows.Forms.ToolStripSeparator();
            this.recentClear = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.cryptoToolstrip = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modelBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importMapDiag = new System.Windows.Forms.OpenFileDialog();
            this.exportMapDiag = new System.Windows.Forms.SaveFileDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.split = new System.Windows.Forms.SplitContainer();
            this.statusLicenseStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.treeObjects = new rMap.ObjectTree();
            this.editor = new rMap.ObjectEditor();
            this.timerLicenseStatusUpdate = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.split)).BeginInit();
            this.split.Panel1.SuspendLayout();
            this.split.Panel2.SuspendLayout();
            this.split.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.windowToolStripMenuItem,
            this.mapToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(829, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.cryptoToolstrip,
            this.toolStripSeparator4,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem1,
            this.recentClearSplit1,
            this.recentClearSplit2,
            this.recentClear});
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // fileToolStripMenuItem1
            // 
            this.fileToolStripMenuItem1.Name = "fileToolStripMenuItem1";
            this.fileToolStripMenuItem1.Size = new System.Drawing.Size(101, 22);
            this.fileToolStripMenuItem1.Text = "File..";
            this.fileToolStripMenuItem1.Click += new System.EventHandler(this.fileToolStripMenuItem1_Click);
            // 
            // recentClearSplit1
            // 
            this.recentClearSplit1.Name = "recentClearSplit1";
            this.recentClearSplit1.Size = new System.Drawing.Size(98, 6);
            // 
            // recentClearSplit2
            // 
            this.recentClearSplit2.Name = "recentClearSplit2";
            this.recentClearSplit2.Size = new System.Drawing.Size(98, 6);
            // 
            // recentClear
            // 
            this.recentClear.Name = "recentClear";
            this.recentClear.Size = new System.Drawing.Size(101, 22);
            this.recentClear.Text = "Clear";
            this.recentClear.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.saveAsToolStripMenuItem.Text = "Save As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(120, 6);
            // 
            // cryptoToolstrip
            // 
            this.cryptoToolstrip.Checked = global::rMap.Properties.Settings.Default.UseCrypto;
            this.cryptoToolstrip.CheckOnClick = true;
            this.cryptoToolstrip.Name = "cryptoToolstrip";
            this.cryptoToolstrip.Size = new System.Drawing.Size(123, 22);
            this.cryptoToolstrip.Text = "Crypted";
            this.cryptoToolstrip.CheckedChanged += new System.EventHandler(this.cryptoToolstrip_CheckedChanged);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(120, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem,
            this.modelBrowserToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.optionsToolStripMenuItem.Text = "Options";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // modelBrowserToolStripMenuItem
            // 
            this.modelBrowserToolStripMenuItem.Name = "modelBrowserToolStripMenuItem";
            this.modelBrowserToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.modelBrowserToolStripMenuItem.Text = "Model Browser";
            this.modelBrowserToolStripMenuItem.Click += new System.EventHandler(this.modelBrowserToolStripMenuItem_Click);
            // 
            // windowToolStripMenuItem
            // 
            this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
            this.windowToolStripMenuItem.Size = new System.Drawing.Size(80, 20);
            this.windowToolStripMenuItem.Text = "3D Window";
            this.windowToolStripMenuItem.Click += new System.EventHandler(this.windowToolStripMenuItem_Click);
            // 
            // mapToolStripMenuItem
            // 
            this.mapToolStripMenuItem.Name = "mapToolStripMenuItem";
            this.mapToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.mapToolStripMenuItem.Text = "Map";
            this.mapToolStripMenuItem.Click += new System.EventHandler(this.mapToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // importMapDiag
            // 
            this.importMapDiag.Filter = "RYL Map files|*.z3s|All files|*.*";
            this.importMapDiag.Title = "Map to import";
            // 
            // exportMapDiag
            // 
            this.exportMapDiag.Filter = "RYL Map files|*.z3s|All files|*.*";
            this.exportMapDiag.Title = "Map saving location";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusText,
            this.statusProgress,
            this.statusLicenseStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 528);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(829, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusText
            // 
            this.statusText.AutoSize = false;
            this.statusText.Name = "statusText";
            this.statusText.Size = new System.Drawing.Size(200, 17);
            this.statusText.Text = "Ready";
            this.statusText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusProgress
            // 
            this.statusProgress.Name = "statusProgress";
            this.statusProgress.Size = new System.Drawing.Size(300, 16);
            this.statusProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.statusProgress.Visible = false;
            // 
            // split
            // 
            this.split.Dock = System.Windows.Forms.DockStyle.Fill;
            this.split.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.split.Location = new System.Drawing.Point(0, 24);
            this.split.Name = "split";
            // 
            // split.Panel1
            // 
            this.split.Panel1.Controls.Add(this.treeObjects);
            // 
            // split.Panel2
            // 
            this.split.Panel2.Controls.Add(this.editor);
            this.split.Size = new System.Drawing.Size(829, 504);
            this.split.SplitterDistance = 248;
            this.split.TabIndex = 2;
            // 
            // statusLicenseStatus
            // 
            this.statusLicenseStatus.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.statusLicenseStatus.IsLink = true;
            this.statusLicenseStatus.Name = "statusLicenseStatus";
            this.statusLicenseStatus.Size = new System.Drawing.Size(614, 17);
            this.statusLicenseStatus.Spring = true;
            this.statusLicenseStatus.Text = "License status";
            this.statusLicenseStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.statusLicenseStatus.Click += new System.EventHandler(this.statusLicenseStatus_Click);
            // 
            // treeObjects
            // 
            this.treeObjects.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeObjects.HideSelection = false;
            this.treeObjects.Location = new System.Drawing.Point(3, 3);
            this.treeObjects.Name = "treeObjects";
            this.treeObjects.Size = new System.Drawing.Size(242, 498);
            this.treeObjects.Sorted = true;
            this.treeObjects.TabIndex = 0;
            this.treeObjects.OnSelect += new rMap.OnSelectDelegate(this.treeObjects_OnSelect);
            // 
            // editor
            // 
            this.editor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.editor.Location = new System.Drawing.Point(3, 3);
            this.editor.Name = "editor";
            this.editor.Size = new System.Drawing.Size(571, 498);
            this.editor.TabIndex = 0;
            this.editor.ObjectChanged += new System.EventHandler(this.editor_ObjectChanged);
            // 
            // timerLicenseStatusUpdate
            // 
            this.timerLicenseStatusUpdate.Enabled = true;
            this.timerLicenseStatusUpdate.Interval = 15000;
            this.timerLicenseStatusUpdate.Tick += new System.EventHandler(this.timerLicenseStatusUpdate_Tick);
            // 
            // rMapForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(829, 550);
            this.Controls.Add(this.split);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "rMapForm";
            this.Text = "RYL Map Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.rMapForm_FormClosing);
            this.Load += new System.EventHandler(this.rMapForm_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.rMapForm_KeyUp);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.split.Panel1.ResumeLayout(false);
            this.split.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.split)).EndInit();
            this.split.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator recentClearSplit2;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog importMapDiag;
        private System.Windows.Forms.ToolStripSeparator recentClearSplit1;
        private System.Windows.Forms.ToolStripMenuItem recentClear;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.SaveFileDialog exportMapDiag;
        private System.Windows.Forms.SplitContainer split;
        private ObjectEditor editor;
        private System.Windows.Forms.ToolStripMenuItem windowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        public ObjectTree treeObjects;
        private System.Windows.Forms.ToolStripMenuItem cryptoToolstrip;
        private System.Windows.Forms.ToolStripMenuItem modelBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mapToolStripMenuItem;
        public System.Windows.Forms.ToolStripStatusLabel statusText;
        public System.Windows.Forms.ToolStripProgressBar statusProgress;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLicenseStatus;
        private System.Windows.Forms.Timer timerLicenseStatusUpdate;
    }
}

