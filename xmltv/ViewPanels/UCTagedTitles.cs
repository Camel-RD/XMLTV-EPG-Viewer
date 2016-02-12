using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xmltv
{
    public partial class UCTagedTitles : UserControl, IEPGView
    {

        private TopManager _topManager = TopManager.St;
        private bool IgnoreClicks = false;


        public UCTagedTitles()
        {
            InitializeComponent();
        }

        void DoMsg(string s)
        {
            MessageBox.Show(this, s, "MyEPG", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UCForm_Load(object sender, EventArgs e)
        {
            RefreshData();
            ResizeColumn();
        }


        void IEPGView.ClearForm()
        {
            ClearForm();
        }

        void IEPGView.RefreshData()
        {
            RefreshData();
        }

        void ClearForm()
        {
            lvTags.Items.Clear();
        }

        string TagString(EProgramTag tag)
        {
            switch (tag)
            {
                case EProgramTag.Seen: return "Seen";
                case EProgramTag.Ignore: return "Ignore";
                case EProgramTag.AutoSchedule: return "Schedule";
           }
            return "";
        }

        void RefreshData()
        {
            string s = "";
            ListViewItem lvi;
            Enabled = false;

            lvTags.BeginUpdate();
            lvTags.Items.Clear();

            foreach (var kv in _topManager.EPGUserData.TagedProgramms)
            {
                s = TagString(kv.Value);
                lvi = lvTags.Items.Add(kv.Key);
                lvi.SubItems.Add(s);
            }
            lvTags.EndUpdate();
            Enabled = true;
        }

        private bool resizinglist = false;

        void ResizeColumn()
        {
            if (resizinglist) return;
            resizinglist = true;
            lvTags.Columns[0].Width = -2;
            resizinglist = false;
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int ct = lvTags.SelectedItems.Count;
            if (ct == 0) return;
            ListViewItem[] items = new ListViewItem[ct];
            lvTags.SelectedItems.CopyTo(items,0);
            foreach (ListViewItem lvi in items)
            {
                _topManager.EPGUserData.SetProgramTag(lvi.SubItems[0].Text, EProgramTag.None);
                lvTags.Items.Remove(lvi);
            }
        }

        private void tagToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            int ct = lvTags.SelectedItems.Count;
            if (ct == 0) return;
            ListViewItem[] items = new ListViewItem[ct];
            lvTags.SelectedItems.CopyTo(items, 0);
            EProgramTag tag = EProgramTag.None;
            switch (e.ClickedItem.Text)
            {
                case "Seen":
                    tag = EProgramTag.Seen;
                    break;
                case "Ignore":
                    tag = EProgramTag.Ignore;
                    break;
                case "Auto Schedule":
                    tag = EProgramTag.AutoSchedule;
                    break;
            }
            foreach (ListViewItem lvi in items)
            {
                _topManager.EPGUserData.SetProgramTag(lvi.SubItems[0].Text, tag);
                lvi.SubItems[1].Text = TagString(tag);
            }
        }

        private void lvTags_Resize(object sender, EventArgs e)
        {
            ResizeColumn();
        }

    }
}
