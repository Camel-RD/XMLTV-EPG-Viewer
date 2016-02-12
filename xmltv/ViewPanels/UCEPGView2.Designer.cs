namespace xmltv
{
    partial class UCEPGView2
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
            this.components = new System.ComponentModel.Container();
            this.lvProgramm = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToScheduleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.seenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ignoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoScheduleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.txDescr = new System.Windows.Forms.TextBox();
            this.cbDates = new System.Windows.Forms.ComboBox();
            this.cbChannels = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvProgramm
            // 
            this.lvProgramm.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader4,
            this.columnHeader3});
            this.lvProgramm.ContextMenuStrip = this.contextMenuStrip1;
            this.lvProgramm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvProgramm.FullRowSelect = true;
            this.lvProgramm.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvProgramm.LabelWrap = false;
            this.lvProgramm.Location = new System.Drawing.Point(0, 0);
            this.lvProgramm.Margin = new System.Windows.Forms.Padding(2);
            this.lvProgramm.MultiSelect = false;
            this.lvProgramm.Name = "lvProgramm";
            this.lvProgramm.Size = new System.Drawing.Size(455, 201);
            this.lvProgramm.TabIndex = 7;
            this.lvProgramm.UseCompatibleStateImageBehavior = false;
            this.lvProgramm.View = System.Windows.Forms.View.Details;
            this.lvProgramm.SelectedIndexChanged += new System.EventHandler(this.lvProgramm_SelectedIndexChanged);
            this.lvProgramm.Resize += new System.EventHandler(this.lvProgramm_Resize);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Srart";
            this.columnHeader1.Width = 70;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "End";
            this.columnHeader2.Width = 70;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Channel";
            this.columnHeader4.Width = 200;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Title";
            this.columnHeader3.Width = 148;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToScheduleToolStripMenuItem,
            this.tagToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(230, 64);
            // 
            // addToScheduleToolStripMenuItem
            // 
            this.addToScheduleToolStripMenuItem.Name = "addToScheduleToolStripMenuItem";
            this.addToScheduleToolStripMenuItem.Size = new System.Drawing.Size(229, 30);
            this.addToScheduleToolStripMenuItem.Text = "Add to Schedule";
            this.addToScheduleToolStripMenuItem.Click += new System.EventHandler(this.addToScheduleToolStripMenuItem_Click);
            // 
            // tagToolStripMenuItem
            // 
            this.tagToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.seenToolStripMenuItem,
            this.ignoreToolStripMenuItem,
            this.autoScheduleToolStripMenuItem});
            this.tagToolStripMenuItem.Name = "tagToolStripMenuItem";
            this.tagToolStripMenuItem.Size = new System.Drawing.Size(229, 30);
            this.tagToolStripMenuItem.Text = "Tag";
            this.tagToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tagToolStripMenuItem_DropDownItemClicked);
            // 
            // seenToolStripMenuItem
            // 
            this.seenToolStripMenuItem.Name = "seenToolStripMenuItem";
            this.seenToolStripMenuItem.Size = new System.Drawing.Size(213, 30);
            this.seenToolStripMenuItem.Text = "Seen";
            // 
            // ignoreToolStripMenuItem
            // 
            this.ignoreToolStripMenuItem.Name = "ignoreToolStripMenuItem";
            this.ignoreToolStripMenuItem.Size = new System.Drawing.Size(213, 30);
            this.ignoreToolStripMenuItem.Text = "Ignore";
            // 
            // autoScheduleToolStripMenuItem
            // 
            this.autoScheduleToolStripMenuItem.Name = "autoScheduleToolStripMenuItem";
            this.autoScheduleToolStripMenuItem.Size = new System.Drawing.Size(213, 30);
            this.autoScheduleToolStripMenuItem.Text = "Auto Schedule";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lvProgramm);
            this.splitContainer1.Panel1MinSize = 100;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.txDescr);
            this.splitContainer1.Panel2MinSize = 30;
            this.splitContainer1.Size = new System.Drawing.Size(455, 296);
            this.splitContainer1.SplitterDistance = 201;
            this.splitContainer1.SplitterIncrement = 5;
            this.splitContainer1.TabIndex = 20;
            // 
            // txDescr
            // 
            this.txDescr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txDescr.Location = new System.Drawing.Point(0, 0);
            this.txDescr.Margin = new System.Windows.Forms.Padding(2);
            this.txDescr.Multiline = true;
            this.txDescr.Name = "txDescr";
            this.txDescr.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txDescr.Size = new System.Drawing.Size(455, 91);
            this.txDescr.TabIndex = 9;
            // 
            // cbDates
            // 
            this.cbDates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDates.FormattingEnabled = true;
            this.cbDates.Location = new System.Drawing.Point(2, 1);
            this.cbDates.Margin = new System.Windows.Forms.Padding(2);
            this.cbDates.Name = "cbDates";
            this.cbDates.Size = new System.Drawing.Size(91, 24);
            this.cbDates.TabIndex = 15;
            this.cbDates.SelectedIndexChanged += new System.EventHandler(this.cbDates_SelectedIndexChanged);
            // 
            // cbChannels
            // 
            this.cbChannels.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbChannels.DropDownHeight = 350;
            this.cbChannels.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbChannels.FormattingEnabled = true;
            this.cbChannels.IntegralHeight = false;
            this.cbChannels.Location = new System.Drawing.Point(96, 1);
            this.cbChannels.Margin = new System.Windows.Forms.Padding(2);
            this.cbChannels.Name = "cbChannels";
            this.cbChannels.Size = new System.Drawing.Size(360, 24);
            this.cbChannels.TabIndex = 16;
            this.cbChannels.SelectedIndexChanged += new System.EventHandler(this.cbChannels_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cbDates);
            this.panel1.Controls.Add(this.cbChannels);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(455, 24);
            this.panel1.TabIndex = 21;
            // 
            // UCEPGView2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Name = "UCEPGView2";
            this.Size = new System.Drawing.Size(455, 320);
            this.Load += new System.EventHandler(this.UCForm_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvProgramm;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox txDescr;
        private System.Windows.Forms.ComboBox cbDates;
        private System.Windows.Forms.ComboBox cbChannels;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem addToScheduleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tagToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem seenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ignoreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoScheduleToolStripMenuItem;
    }
}
