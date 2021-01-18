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
    public partial class UCEPGView3 : UserControl, IEPGView
    {
        private TopManager _topManager = TopManager.St;
        List<CProgrammData> ProgrammList = null;
        private bool IgnoreClicks = false;
        List<DateTime> DatesUsed = new List<DateTime>();

        public UCEPGView3()
        {
            InitializeComponent();
            ColorListViewHeader.colorListViewHeader(lvProgramm);
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


        void FirstSelect()
        {
            DateTime dt = DateTime.Now;
            dt = new DateTime(dt.Year, dt.Month, dt.Day);
            int k = DatesUsed.FindIndex(d => d == dt);
            if (k == -1) return;
            if (_topManager.EPGData.ChannelData.Count == 0) return;
            cbDates.SelectedIndex = k;
        }

        void ClearForm()
        {
            Enabled = false;
            cbDates.Items.Clear();
            lvProgramm.Items.Clear();
            txDescr.Text = "";
            Enabled = true;
        }


        void RefreshData()
        {
            int i;
            string s;
            DateTime dt,dt2;

            Enabled = false;

            lvProgramm.Items.Clear();
            txDescr.Text = "";

            cbDates.BeginUpdate();
            cbDates.Items.Clear();

            _topManager.ScheduledProgramms.Sort(
                (se1, se2) =>
                {
                    return DateTime.Compare(se1.Start, se2.Start);
                });

            DatesUsed = new List<DateTime>();
            foreach (CScheduledntry se in _topManager.ScheduledProgramms)
            {
                dt2 = se.Start.Date;
                if (!DatesUsed.Contains(dt2))
                    DatesUsed.Add(dt2);
            }
            DatesUsed.Sort();

            for (i = 0; i < DatesUsed.Count; i++)
            {
                dt = DatesUsed[i];
                s = dt.ToString("MMM-dd ddd");
                cbDates.Items.Add(s);
            }
            cbDates.EndUpdate();

            Enabled = true;
            FirstSelect();

        }

        private void RefreshProgrammList()
        {
            int dtnr = cbDates.SelectedIndex;
            RefreshProgrammList(dtnr);
        }

        void RefreshProgrammList(int dtnr)
        {

            txDescr.Text = "";
            lvProgramm.Items.Clear();
            if (dtnr == -1) return;
            DateTime dt = DatesUsed[dtnr];

            ProgrammList = new List<CProgrammData>();

            CProgrammData pd;

            foreach (CScheduledntry se in _topManager.ScheduledProgramms)
            {
                if(se.Start.Date == dt)
                {
                    pd = _topManager.EPGData.GetByChIdAndStar(se.ChId, se.Start);
                    if(pd!=null) ProgrammList.Add(pd);
                }
            }

            int i;
            string sstart, sstop;
            ListViewItem lvi;

            lvProgramm.BeginUpdate();

            for (i = 0; i < ProgrammList.Count; i++)
            {
                pd = ProgrammList[i];
                sstart = pd.Start.ToString("HH:mm");
                sstop = pd.Stop.ToString("HH:mm");
                lvi = lvProgramm.Items.Add(sstart);
                lvi.SubItems.Add(sstop);
                lvi.SubItems.Add(pd.ChannelData.DisplayNameR);
                lvi.SubItems.Add(pd.Title);
            }

            lvProgramm.EndUpdate();

            if (ProgrammList.Count > 0)
            {
                lvProgramm.Items[0].Selected = true;
            }
        }


        private void cbDates_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IgnoreClicks) return;
            RefreshProgrammList();
        }

        private void lvProgramm_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IgnoreClicks) return;
            txDescr.Text = "";
            if (lvProgramm.SelectedIndices.Count != 1) return;
            int k = lvProgramm.SelectedIndices[0];
            if (k == -1) return;
            CProgrammData pd = ProgrammList[k];
            if (pd.SubTitle != "")
            {
                txDescr.Text = pd.SubTitle + "\r\n" + pd.Desc;
            }
            else
            {
                txDescr.Text = pd.Desc;
            }
        }

        void ResizeColumn()
        {
            lvProgramm.Columns[3].Width = -2;
        }

        private void lvProgramm_Resize(object sender, EventArgs e)
        {
            ResizeColumn();
        }

        void RemoveSelected()
        {
            int count = lvProgramm.SelectedIndices.Count;
            if (count == 0) return;
            ListViewItem[] items = new ListViewItem[count];
            int[] indices = new int[count];
            CProgrammData[] pdd = new CProgrammData[count];
            int i;
            for (i = 0; i < count; i++)
            {
                items[i] = lvProgramm.SelectedItems[i];
                indices[i] = lvProgramm.SelectedIndices[i];
                pdd[i] = ProgrammList[indices[i]];
            }
            CProgrammData pd;
            IgnoreClicks = true;
            txDescr.Text = "";
            for (i = 0; i < count; i++)
            {
                pd = pdd[i];
                _topManager.EPGUserData.RemoveFromSchedule(pd.ChId, pd.Start);
                lvProgramm.Items.Remove(items[i]);
                ProgrammList.Remove(pd);
            }
            IgnoreClicks = false;
            if (indices.Length > 0 && lvProgramm.Items.Count > 0)
            {
                i = indices[indices.Length - 1];
                i = i - indices.Length + 1;
                if (i >= lvProgramm.Items.Count)
                {
                    i = lvProgramm.Items.Count;
                }
                lvProgramm.Items[i].Selected = true;
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveSelected();
        }

        private void removeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _topManager.EPGUserData.ClearSchedule();
            RefreshData();
        }

        private void removeDateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int k = cbDates.SelectedIndex;
            if (k == -1 || k >= DatesUsed.Count) return;
            DateTime dt = DatesUsed[k];

            ClearForm();
            _topManager.EPGUserData.RemoveDateFromSchedule(dt);
            RefreshData();
        }

        private void lvProgramm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                RemoveSelected();
            }

        }

        private void tagToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            contextMenuStrip1.Hide();
            if (lvProgramm.SelectedIndices.Count != 1) return;
            int k = lvProgramm.SelectedIndices[0];
            if (k == -1) return;
            CProgrammData pd = ProgrammList[k];
            string op = e.ClickedItem.Text;
            EProgramTag tag = EProgramTag.None;
            switch (op)
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
            _topManager.EPGUserData.SetProgramTag(pd.Title, tag);

        }

    }
}
