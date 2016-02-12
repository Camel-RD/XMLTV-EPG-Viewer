namespace xmltv
{
    partial class UCEditGroups
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
            this.lbNames = new System.Windows.Forms.ListBox();
            this.lbChannels = new System.Windows.Forms.ListBox();
            this.lbSelected = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.btAdd = new System.Windows.Forms.Button();
            this.btDelete = new System.Windows.Forms.Button();
            this.btChange = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbNames
            // 
            this.lbNames.FormattingEnabled = true;
            this.lbNames.ItemHeight = 16;
            this.lbNames.Location = new System.Drawing.Point(0, 0);
            this.lbNames.Name = "lbNames";
            this.lbNames.Size = new System.Drawing.Size(145, 116);
            this.lbNames.TabIndex = 0;
            this.lbNames.SelectedIndexChanged += new System.EventHandler(this.lbNames_SelectedIndexChanged);
            // 
            // lbChannels
            // 
            this.lbChannels.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lbChannels.FormattingEnabled = true;
            this.lbChannels.ItemHeight = 16;
            this.lbChannels.Location = new System.Drawing.Point(0, 122);
            this.lbChannels.Name = "lbChannels";
            this.lbChannels.Size = new System.Drawing.Size(150, 180);
            this.lbChannels.TabIndex = 1;
            this.lbChannels.DoubleClick += new System.EventHandler(this.lbChannels_DoubleClick);
            // 
            // lbSelected
            // 
            this.lbSelected.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lbSelected.FormattingEnabled = true;
            this.lbSelected.ItemHeight = 16;
            this.lbSelected.Location = new System.Drawing.Point(154, 122);
            this.lbSelected.Name = "lbSelected";
            this.lbSelected.Size = new System.Drawing.Size(150, 180);
            this.lbSelected.TabIndex = 2;
            this.lbSelected.DoubleClick += new System.EventHandler(this.lbSelected_DoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(151, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Group Name:";
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(154, 19);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(117, 22);
            this.tbName.TabIndex = 4;
            // 
            // btAdd
            // 
            this.btAdd.Location = new System.Drawing.Point(154, 47);
            this.btAdd.Name = "btAdd";
            this.btAdd.Size = new System.Drawing.Size(44, 24);
            this.btAdd.TabIndex = 5;
            this.btAdd.Text = "Add";
            this.btAdd.UseVisualStyleBackColor = true;
            this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
            // 
            // btDelete
            // 
            this.btDelete.Location = new System.Drawing.Point(154, 77);
            this.btDelete.Name = "btDelete";
            this.btDelete.Size = new System.Drawing.Size(62, 24);
            this.btDelete.TabIndex = 6;
            this.btDelete.Text = "Delete";
            this.btDelete.UseVisualStyleBackColor = true;
            this.btDelete.Click += new System.EventHandler(this.btDelete_Click);
            // 
            // btChange
            // 
            this.btChange.Location = new System.Drawing.Point(204, 47);
            this.btChange.Name = "btChange";
            this.btChange.Size = new System.Drawing.Size(67, 24);
            this.btChange.TabIndex = 7;
            this.btChange.Text = "Change";
            this.btChange.UseVisualStyleBackColor = true;
            this.btChange.Click += new System.EventHandler(this.btChange_Click);
            // 
            // UCEditGroups
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btChange);
            this.Controls.Add(this.btDelete);
            this.Controls.Add(this.btAdd);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbSelected);
            this.Controls.Add(this.lbChannels);
            this.Controls.Add(this.lbNames);
            this.Name = "UCEditGroups";
            this.Size = new System.Drawing.Size(309, 307);
            this.Load += new System.EventHandler(this.UCEditGroups_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbNames;
        private System.Windows.Forms.ListBox lbChannels;
        private System.Windows.Forms.ListBox lbSelected;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Button btAdd;
        private System.Windows.Forms.Button btDelete;
        private System.Windows.Forms.Button btChange;
    }
}
