namespace xmltv
{
    partial class UCSourceEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCSourceEditor));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label1 = new System.Windows.Forms.Label();
            this.lbAllChanels = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btRename = new System.Windows.Forms.Button();
            this.lbKeepChannels = new System.Windows.Forms.ListBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.lbTvSources = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cbURL = new xmltv.FlatComboBox();
            this.tbAddHours = new xmltv.MyTextBox();
            this.txPrefix = new xmltv.MyTextBox();
            this.tbName = new xmltv.MyTextBox();
            this.tbRName = new xmltv.MyTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 170);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.lbAllChanels);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.label4);
            this.splitContainer1.Panel2.Controls.Add(this.btRename);
            this.splitContainer1.Panel2.Controls.Add(this.tbRName);
            this.splitContainer1.Panel2.Controls.Add(this.lbKeepChannels);
            this.splitContainer1.Size = new System.Drawing.Size(599, 338);
            this.splitContainer1.SplitterDistance = 211;
            this.splitContainer1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(153, 29);
            this.label1.TabIndex = 20;
            this.label1.Text = "All Channels:";
            // 
            // lbAllChanels
            // 
            this.lbAllChanels.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbAllChanels.FormattingEnabled = true;
            this.lbAllChanels.ItemHeight = 29;
            this.lbAllChanels.Location = new System.Drawing.Point(0, 37);
            this.lbAllChanels.Name = "lbAllChanels";
            this.lbAllChanels.Size = new System.Drawing.Size(208, 265);
            this.lbAllChanels.TabIndex = 19;
            this.lbAllChanels.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbAllChanels_MouseDoubleClick);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(184, 29);
            this.label4.TabIndex = 23;
            this.label4.Text = "Keep Channels:";
            // 
            // btRename
            // 
            this.btRename.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btRename.Location = new System.Drawing.Point(231, 37);
            this.btRename.Name = "btRename";
            this.btRename.Size = new System.Drawing.Size(141, 44);
            this.btRename.TabIndex = 21;
            this.btRename.Text = "Rename Ch.";
            this.btRename.UseVisualStyleBackColor = true;
            this.btRename.Click += new System.EventHandler(this.btRename_Click);
            // 
            // lbKeepChannels
            // 
            this.lbKeepChannels.AllowDrop = true;
            this.lbKeepChannels.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbKeepChannels.FormattingEnabled = true;
            this.lbKeepChannels.ItemHeight = 29;
            this.lbKeepChannels.Location = new System.Drawing.Point(0, 93);
            this.lbKeepChannels.Name = "lbKeepChannels";
            this.lbKeepChannels.Size = new System.Drawing.Size(379, 207);
            this.lbKeepChannels.TabIndex = 22;
            this.lbKeepChannels.SelectedIndexChanged += new System.EventHandler(this.lbKeepChannels_SelectedIndexChanged);
            this.lbKeepChannels.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbKeepChannels_MouseDoubleClick);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(21, 21);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripButton3,
            this.toolStripButton4});
            this.toolStrip1.Location = new System.Drawing.Point(262, 1);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(334, 39);
            this.toolStrip1.TabIndex = 30;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(57, 34);
            this.toolStripButton1.Text = "Add";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(73, 34);
            this.toolStripButton2.Text = "Apply";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(80, 34);
            this.toolStripButton3.Text = "Delete";
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(106, 34);
            this.toolStripButton4.Text = "GetAllCh.";
            this.toolStripButton4.Click += new System.EventHandler(this.toolStripButton4_Click);
            // 
            // lbTvSources
            // 
            this.lbTvSources.FormattingEnabled = true;
            this.lbTvSources.ItemHeight = 29;
            this.lbTvSources.Location = new System.Drawing.Point(0, 0);
            this.lbTvSources.Name = "lbTvSources";
            this.lbTvSources.Size = new System.Drawing.Size(259, 120);
            this.lbTvSources.TabIndex = 26;
            this.lbTvSources.SelectedIndexChanged += new System.EventHandler(this.lbTvSources_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(505, 60);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 29);
            this.label2.TabIndex = 32;
            this.label2.Text = "+";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(405, 60);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 29);
            this.label3.TabIndex = 32;
            this.label3.Text = "pr";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(260, 60);
            this.label5.Margin = new System.Windows.Forms.Padding(0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 29);
            this.label5.TabIndex = 32;
            this.label5.Text = "N";
            // 
            // cbURL
            // 
            this.cbURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbURL.BorderColor = System.Drawing.SystemColors.ControlText;
            this.cbURL.DropDownHeight = 300;
            this.cbURL.DropDownWidth = 800;
            this.cbURL.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbURL.FormattingEnabled = true;
            this.cbURL.IntegralHeight = false;
            this.cbURL.Location = new System.Drawing.Point(265, 112);
            this.cbURL.Name = "cbURL";
            this.cbURL.Size = new System.Drawing.Size(329, 37);
            this.cbURL.TabIndex = 31;
            this.cbURL.Leave += new System.EventHandler(this.cbURL_Leave);
            // 
            // tbAddHours
            // 
            this.tbAddHours.BorderColor = System.Drawing.SystemColors.ControlText;
            this.tbAddHours.Location = new System.Drawing.Point(526, 57);
            this.tbAddHours.Name = "tbAddHours";
            this.tbAddHours.Size = new System.Drawing.Size(53, 35);
            this.tbAddHours.TabIndex = 24;
            // 
            // txPrefix
            // 
            this.txPrefix.BorderColor = System.Drawing.SystemColors.ControlText;
            this.txPrefix.Location = new System.Drawing.Point(435, 57);
            this.txPrefix.Name = "txPrefix";
            this.txPrefix.Size = new System.Drawing.Size(61, 35);
            this.txPrefix.TabIndex = 24;
            // 
            // tbName
            // 
            this.tbName.BorderColor = System.Drawing.SystemColors.ControlText;
            this.tbName.Location = new System.Drawing.Point(286, 57);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(116, 35);
            this.tbName.TabIndex = 23;
            // 
            // tbRName
            // 
            this.tbRName.BorderColor = System.Drawing.SystemColors.ControlText;
            this.tbRName.Location = new System.Drawing.Point(8, 44);
            this.tbRName.Name = "tbRName";
            this.tbRName.Size = new System.Drawing.Size(212, 35);
            this.tbRName.TabIndex = 20;
            // 
            // UCSourceEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbURL);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.tbAddHours);
            this.Controls.Add(this.txPrefix);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.lbTvSources);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "UCSourceEditor";
            this.Size = new System.Drawing.Size(600, 513);
            this.Load += new System.EventHandler(this.UCSourceEditor_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lbAllChanels;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btRename;
        private MyTextBox tbRName;
        private System.Windows.Forms.ListBox lbKeepChannels;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private MyTextBox txPrefix;
        private MyTextBox tbName;
        private System.Windows.Forms.ListBox lbTvSources;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private FlatComboBox cbURL;
        private MyTextBox tbAddHours;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
    }
}
