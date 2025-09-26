namespace rMap.Editors
{
    partial class TilePosEditor
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
            this.aRight = new System.Windows.Forms.PictureBox();
            this.pBox = new System.Windows.Forms.PictureBox();
            this.aUp = new System.Windows.Forms.PictureBox();
            this.aLeft = new System.Windows.Forms.PictureBox();
            this.aDown = new System.Windows.Forms.PictureBox();
            this.chkGrid = new System.Windows.Forms.CheckBox();
            this.lnkTileSelect = new System.Windows.Forms.LinkLabel();
            this.tileSelect = new rMap.Editors.TileSelector();
            ((System.ComponentModel.ISupportInitialize)(this.aRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.aUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.aLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.aDown)).BeginInit();
            this.SuspendLayout();
            // 
            // aRight
            // 
            this.aRight.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.aRight.Cursor = System.Windows.Forms.Cursors.PanEast;
            this.aRight.Image = global::rMap.Properties.Resources.arrow_blue_rounded_right;
            this.aRight.Location = new System.Drawing.Point(555, 277);
            this.aRight.Name = "aRight";
            this.aRight.Size = new System.Drawing.Size(30, 30);
            this.aRight.TabIndex = 1;
            this.aRight.TabStop = false;
            this.aRight.Click += new System.EventHandler(this.aRight_Click);
            // 
            // pBox
            // 
            this.pBox.Location = new System.Drawing.Point(36, 36);
            this.pBox.Name = "pBox";
            this.pBox.Size = new System.Drawing.Size(512, 512);
            this.pBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pBox.TabIndex = 0;
            this.pBox.TabStop = false;
            // 
            // aUp
            // 
            this.aUp.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.aUp.Cursor = System.Windows.Forms.Cursors.PanNorth;
            this.aUp.Image = global::rMap.Properties.Resources.arrow_blue_rounded_up;
            this.aUp.Location = new System.Drawing.Point(277, 0);
            this.aUp.Name = "aUp";
            this.aUp.Size = new System.Drawing.Size(30, 30);
            this.aUp.TabIndex = 2;
            this.aUp.TabStop = false;
            this.aUp.Click += new System.EventHandler(this.aUp_Click);
            // 
            // aLeft
            // 
            this.aLeft.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.aLeft.Cursor = System.Windows.Forms.Cursors.PanWest;
            this.aLeft.Image = global::rMap.Properties.Resources.arrow_blue_rounded_left;
            this.aLeft.Location = new System.Drawing.Point(0, 277);
            this.aLeft.Name = "aLeft";
            this.aLeft.Size = new System.Drawing.Size(30, 30);
            this.aLeft.TabIndex = 3;
            this.aLeft.TabStop = false;
            this.aLeft.Click += new System.EventHandler(this.aLeft_Click);
            // 
            // aDown
            // 
            this.aDown.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.aDown.Cursor = System.Windows.Forms.Cursors.PanSouth;
            this.aDown.Image = global::rMap.Properties.Resources.arrow_blue_rounded_down;
            this.aDown.Location = new System.Drawing.Point(277, 555);
            this.aDown.Name = "aDown";
            this.aDown.Size = new System.Drawing.Size(30, 30);
            this.aDown.TabIndex = 4;
            this.aDown.TabStop = false;
            this.aDown.Click += new System.EventHandler(this.aDown_Click);
            // 
            // chkGrid
            // 
            this.chkGrid.AutoSize = true;
            this.chkGrid.Location = new System.Drawing.Point(503, 13);
            this.chkGrid.Name = "chkGrid";
            this.chkGrid.Size = new System.Drawing.Size(45, 17);
            this.chkGrid.TabIndex = 5;
            this.chkGrid.Text = "Grid";
            this.chkGrid.UseVisualStyleBackColor = true;
            this.chkGrid.CheckedChanged += new System.EventHandler(this.chkGrid_CheckedChanged);
            // 
            // lnkTileSelect
            // 
            this.lnkTileSelect.AutoSize = true;
            this.lnkTileSelect.Location = new System.Drawing.Point(416, 14);
            this.lnkTileSelect.Name = "lnkTileSelect";
            this.lnkTileSelect.Size = new System.Drawing.Size(53, 13);
            this.lnkTileSelect.TabIndex = 6;
            this.lnkTileSelect.TabStop = true;
            this.lnkTileSelect.Text = "Select tile";
            this.lnkTileSelect.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkTileSelect_LinkClicked);
            // 
            // tileSelect
            // 
            this.tileSelect.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tileSelect.Location = new System.Drawing.Point(347, 30);
            this.tileSelect.Name = "tileSelect";
            this.tileSelect.Size = new System.Drawing.Size(213, 189);
            this.tileSelect.TabIndex = 7;
            this.tileSelect.Visible = false;
            this.tileSelect.Selected += new rMap.Editors.PosChange(this.tileSelect_Selected);
            // 
            // TilePosEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tileSelect);
            this.Controls.Add(this.lnkTileSelect);
            this.Controls.Add(this.chkGrid);
            this.Controls.Add(this.aDown);
            this.Controls.Add(this.aLeft);
            this.Controls.Add(this.aUp);
            this.Controls.Add(this.aRight);
            this.Controls.Add(this.pBox);
            this.DoubleBuffered = true;
            this.MaximumSize = new System.Drawing.Size(585, 558);
            this.MinimumSize = new System.Drawing.Size(585, 585);
            this.Name = "TilePosEditor";
            this.Size = new System.Drawing.Size(585, 585);
            ((System.ComponentModel.ISupportInitialize)(this.aRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.aUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.aLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.aDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pBox;
        private System.Windows.Forms.PictureBox aRight;
        private System.Windows.Forms.PictureBox aUp;
        private System.Windows.Forms.PictureBox aLeft;
        private System.Windows.Forms.PictureBox aDown;
        private System.Windows.Forms.CheckBox chkGrid;
        private System.Windows.Forms.LinkLabel lnkTileSelect;
        private TileSelector tileSelect;
    }
}
