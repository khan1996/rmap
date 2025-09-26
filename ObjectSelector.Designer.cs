namespace rMap
{
    partial class ObjectSelector
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
            this.selectPanel = new rMap.ObjectSelector.SelectPanelObj();
            this.label1 = new System.Windows.Forms.Label();
            this.chkHouse = new System.Windows.Forms.CheckBox();
            this.chkNature = new System.Windows.Forms.CheckBox();
            this.chkObj = new System.Windows.Forms.CheckBox();
            this.chkEff = new System.Windows.Forms.CheckBox();
            this.objLimitLbl = new System.Windows.Forms.Label();
            this.objectLimitBar = new System.Windows.Forms.ProgressBar();
            this.objectLimitTip = new System.Windows.Forms.ToolTip(this.components);
            this.lblLimit = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // selectPanel
            // 
            this.selectPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.selectPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.selectPanel.Location = new System.Drawing.Point(0, 28);
            this.selectPanel.Name = "selectPanel";
            this.selectPanel.Size = new System.Drawing.Size(660, 591);
            this.selectPanel.TabIndex = 0;
            this.selectPanel.MouseLeave += new System.EventHandler(this.selectPanel_MouseLeave);
            this.selectPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.selectPanel_Paint);
            this.selectPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.selectPanel_MouseMove);
            this.selectPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.selectPanel_MouseClick);
            this.selectPanel.Resize += new System.EventHandler(this.selectPanel_Resize);
            this.selectPanel.MouseEnter += new System.EventHandler(this.selectPanel_MouseEnter);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Filter:";
            // 
            // chkHouse
            // 
            this.chkHouse.AutoSize = true;
            this.chkHouse.BackColor = System.Drawing.Color.LightBlue;
            this.chkHouse.Location = new System.Drawing.Point(50, 8);
            this.chkHouse.Name = "chkHouse";
            this.chkHouse.Size = new System.Drawing.Size(57, 17);
            this.chkHouse.TabIndex = 2;
            this.chkHouse.Text = "House";
            this.chkHouse.UseVisualStyleBackColor = false;
            this.chkHouse.CheckedChanged += new System.EventHandler(this.chkHouse_CheckedChanged);
            // 
            // chkNature
            // 
            this.chkNature.AutoSize = true;
            this.chkNature.BackColor = System.Drawing.Color.LightGreen;
            this.chkNature.Location = new System.Drawing.Point(113, 8);
            this.chkNature.Name = "chkNature";
            this.chkNature.Size = new System.Drawing.Size(58, 17);
            this.chkNature.TabIndex = 3;
            this.chkNature.Text = "Nature";
            this.chkNature.UseVisualStyleBackColor = false;
            this.chkNature.CheckedChanged += new System.EventHandler(this.chkNature_CheckedChanged);
            // 
            // chkObj
            // 
            this.chkObj.AutoSize = true;
            this.chkObj.BackColor = System.Drawing.Color.Pink;
            this.chkObj.Location = new System.Drawing.Point(177, 8);
            this.chkObj.Name = "chkObj";
            this.chkObj.Size = new System.Drawing.Size(57, 17);
            this.chkObj.TabIndex = 4;
            this.chkObj.Text = "Object";
            this.chkObj.UseVisualStyleBackColor = false;
            this.chkObj.CheckedChanged += new System.EventHandler(this.chkObj_CheckedChanged);
            // 
            // chkEff
            // 
            this.chkEff.AutoSize = true;
            this.chkEff.BackColor = System.Drawing.Color.Moccasin;
            this.chkEff.Location = new System.Drawing.Point(240, 8);
            this.chkEff.Name = "chkEff";
            this.chkEff.Size = new System.Drawing.Size(54, 17);
            this.chkEff.TabIndex = 5;
            this.chkEff.Text = "Effect";
            this.chkEff.UseVisualStyleBackColor = false;
            this.chkEff.CheckedChanged += new System.EventHandler(this.chkEff_CheckedChanged);
            // 
            // objLimitLbl
            // 
            this.objLimitLbl.AutoSize = true;
            this.objLimitLbl.Location = new System.Drawing.Point(329, 9);
            this.objLimitLbl.Name = "objLimitLbl";
            this.objLimitLbl.Size = new System.Drawing.Size(61, 13);
            this.objLimitLbl.TabIndex = 6;
            this.objLimitLbl.Text = "Object limit:";
            // 
            // objectLimitBar
            // 
            this.objectLimitBar.ForeColor = System.Drawing.Color.Maroon;
            this.objectLimitBar.Location = new System.Drawing.Point(396, 8);
            this.objectLimitBar.Maximum = 1000;
            this.objectLimitBar.Name = "objectLimitBar";
            this.objectLimitBar.Size = new System.Drawing.Size(100, 17);
            this.objectLimitBar.TabIndex = 7;
            this.objectLimitBar.Value = 200;
            this.objectLimitBar.MouseLeave += new System.EventHandler(this.objectLimitBar_MouseLeave);
            this.objectLimitBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.objectLimitBar_MouseMove);
            this.objectLimitBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.objectLimitBar_MouseDown);
            this.objectLimitBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.objectLimitBar_MouseUp);
            this.objectLimitBar.MouseEnter += new System.EventHandler(this.objectLimitBar_MouseEnter);
            // 
            // lblLimit
            // 
            this.lblLimit.AutoSize = true;
            this.lblLimit.Location = new System.Drawing.Point(502, 9);
            this.lblLimit.Name = "lblLimit";
            this.lblLimit.Size = new System.Drawing.Size(60, 13);
            this.lblLimit.TabIndex = 8;
            this.lblLimit.Text = "1000/1000";
            // 
            // ObjectSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(660, 617);
            this.Controls.Add(this.lblLimit);
            this.Controls.Add(this.objectLimitBar);
            this.Controls.Add(this.objLimitLbl);
            this.Controls.Add(this.chkEff);
            this.Controls.Add(this.chkObj);
            this.Controls.Add(this.chkNature);
            this.Controls.Add(this.chkHouse);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.selectPanel);
            this.DoubleBuffered = true;
            this.Name = "ObjectSelector";
            this.Text = "ObjectSelector";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ObjectSelector_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SelectPanelObj selectPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkHouse;
        private System.Windows.Forms.CheckBox chkNature;
        private System.Windows.Forms.CheckBox chkObj;
        private System.Windows.Forms.CheckBox chkEff;
        private System.Windows.Forms.Label objLimitLbl;
        private System.Windows.Forms.ProgressBar objectLimitBar;
        private System.Windows.Forms.ToolTip objectLimitTip;
        private System.Windows.Forms.Label lblLimit;
    }
}