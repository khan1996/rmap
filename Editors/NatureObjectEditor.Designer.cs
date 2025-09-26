namespace rMap.Editors
{
    partial class NatureObjectEditor
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbInfo = new System.Windows.Forms.Label();
            this.nmCustomType = new System.Windows.Forms.NumericUpDown();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tilePosEditor1 = new rMap.Editors.TilePosEditor();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmCustomType)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.tilePosEditor1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(610, 750);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lbInfo);
            this.panel1.Controls.Add(this.nmCustomType);
            this.panel1.Controls.Add(this.cmbType);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(604, 114);
            this.panel1.TabIndex = 0;
            // 
            // lbInfo
            // 
            this.lbInfo.AutoSize = true;
            this.lbInfo.Location = new System.Drawing.Point(16, 48);
            this.lbInfo.Name = "lbInfo";
            this.lbInfo.Size = new System.Drawing.Size(97, 39);
            this.lbInfo.TabIndex = 3;
            this.lbInfo.Text = "Tile ?tx:?ty\r\nPos ?px:?py\r\nPos in game ?ix:?iy";
            // 
            // nmCustomType
            // 
            this.nmCustomType.Location = new System.Drawing.Point(180, 12);
            this.nmCustomType.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nmCustomType.Name = "nmCustomType";
            this.nmCustomType.Size = new System.Drawing.Size(62, 20);
            this.nmCustomType.TabIndex = 2;
            this.nmCustomType.ValueChanged += new System.EventHandler(this.nmCustomType_ValueChanged);
            // 
            // cmbType
            // 
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Location = new System.Drawing.Point(53, 11);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(121, 21);
            this.cmbType.TabIndex = 1;
            this.cmbType.SelectedIndexChanged += new System.EventHandler(this.cmbType_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Type";
            // 
            // tilePosEditor1
            // 
            this.tilePosEditor1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tilePosEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tilePosEditor1.Location = new System.Drawing.Point(3, 123);
            this.tilePosEditor1.MaximumSize = new System.Drawing.Size(585, 558);
            this.tilePosEditor1.MinimumSize = new System.Drawing.Size(585, 585);
            this.tilePosEditor1.Name = "tilePosEditor1";
            this.tilePosEditor1.Size = new System.Drawing.Size(585, 585);
            this.tilePosEditor1.TabIndex = 2;
            this.tilePosEditor1.CanMoveTo += new rMap.Editors.TilePosEditor.CanMoveToDel(this.tilePosEditor1_CanMoveTo);
            this.tilePosEditor1.MovingTo += new rMap.Editors.TilePosEditor.CanMoveToDel(this.tilePosEditor1_MovingTo);
            this.tilePosEditor1.ObjectMoving += new rMap.Editors.TilePosEditor.CanMoveToDel(this.tilePosEditor1_ObjectMoving);
            this.tilePosEditor1.ObjectSelected += new rMap.Editors.TilePosEditor.CanMoveToDel(this.tilePosEditor1_ObjectSelected);
            // 
            // NatureObjectEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "NatureObjectEditor";
            this.Size = new System.Drawing.Size(610, 750);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmCustomType)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.NumericUpDown nmCustomType;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbInfo;
        private TilePosEditor tilePosEditor1;
    }
}
