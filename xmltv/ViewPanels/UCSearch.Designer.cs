namespace xmltv
{
    partial class UCSearch
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
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToScheduleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.txDescr = new System.Windows.Forms.TextBox();
            this.cbDates = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tbSearchText = new System.Windows.Forms.TextBox();
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
            this.lvProgramm.Size = new System.Drawing.Size(441, 237);
            this.lvProgramm.TabIndex = 7;
            this.lvProgramm.UseCompatibleStateImageBehavior = false;
            this.lvProgramm.View = System.Windows.Forms.View.Details;
            this.lvProgramm.SelectedIndexChanged += new System.EventHandler(this.lvProgramm_SelectedIndexChanged);
            this.lvProgramm.Resize += new System.EventHandler(this.lvProgramm_Resize);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Srart";
            this.columnHeader1.Width = 130;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Channel";
            this.columnHeader4.Width = 142;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Title";
            this.columnHeader3.Width = 275;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToScheduleToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(230, 34);
            // 
            // addToScheduleToolStripMenuItem
            // 
            this.addToScheduleToolStripMenuItem.Name = "addToScheduleToolStripMenuItem";
            this.addToScheduleToolStripMenuItem.Size = new System.Drawing.Size(229, 30);
            this.addToScheduleToolStripMenuItem.Text = "Add to Schedule";
            this.addToScheduleToolStripMenuItem.Click += new System.EventHandler(this.addToScheduleToolStripMenuItem_Click);
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
            this.splitContainer1.Panel2MinSize = 100;
            this.splitContainer1.Size = new System.Drawing.Size(441, 395);
            this.splitContainer1.SplitterDistance = 237;
            this.splitContainer1.SplitterIncrement = 5;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 22;
            // 
            // txDescr
            // 
            this.txDescr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txDescr.Location = new System.Drawing.Point(0, 0);
            this.txDescr.Margin = new System.Windows.Forms.Padding(2);
            this.txDescr.Multiline = true;
            this.txDescr.Name = "txDescr";
            this.txDescr.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txDescr.Size = new System.Drawing.Size(441, 155);
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
            // panel1
            // 
            this.panel1.Controls.Add(this.tbSearchText);
            this.panel1.Controls.Add(this.cbDates);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(441, 24);
            this.panel1.TabIndex = 23;
            // 
            // tbSearchText
            // 
            this.tbSearchText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSearchText.Location = new System.Drawing.Point(96, 1);
            this.tbSearchText.Margin = new System.Windows.Forms.Padding(2);
            this.tbSearchText.Name = "tbSearchText";
            this.tbSearchText.Size = new System.Drawing.Size(344, 22);
            this.tbSearchText.TabIndex = 16;
            this.tbSearchText.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbSearchText_KeyPress);
            // 
            // UCSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Name = "UCSearch";
            this.Size = new System.Drawing.Size(441, 419);
            this.Load += new System.EventHandler(this.UCForm_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvProgramm;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem addToScheduleToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox txDescr;
        private System.Windows.Forms.ComboBox cbDates;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox tbSearchText;
    }
}
