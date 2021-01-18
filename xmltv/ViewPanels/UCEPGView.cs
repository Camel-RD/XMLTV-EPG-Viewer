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
    public partial class UCEPGView : UserControl, IEPGView
    {
        public UCEPGView()
        {
            InitializeComponent();
            ColorListViewHeader.colorListViewHeader(lvProgramm);
        }

        private TopManager _topManager = TopManager.St;
        List<CProgrammData> ProgrammList = null;
        private bool IgnoreClicks = false;


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
            dt = new DateTime(dt.Year,dt.Month,dt.Day);
            int k =_topManager.UsedDates.FindIndex(d => d == dt);
            if (k == -1) return;
            if (_topManager.EPGData.ChannelData.Count == 0) return;
            cbDates.SelectedIndex = k;
            cbChannels.SelectedIndex = 0;
        }

        void ClearForm()
        {
            Enabled = false;
            cbDates.Items.Clear();
            cbChannels.Items.Clear();
            lvProgramm.Items.Clear();
            txDescr.Text = "";
            Enabled = true;
        }


        void RefreshData()
        {
            int i;
            string s;
            DateTime dt;

            Enabled = false;

            lvProgramm.Items.Clear();
            txDescr.Text = "";

            cbChannels.BeginUpdate();
            cbChannels.Items.Clear();
            for (i = 0; i < _topManager.EPGData.ChannelData.Count; i++)
            {
                s = _topManager.EPGData.ChannelData[i].DisplayNameR;
                cbChannels.Items.Add(s);
            }
            cbChannels.EndUpdate();

            cbDates.BeginUpdate();
            cbDates.Items.Clear();
            for (i = 0; i < _topManager.UsedDates.Count; i++)
            {
                dt = _topManager.UsedDates[i];
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
            int chnr = cbChannels.SelectedIndex;
            RefreshProgrammList(dtnr, chnr);
        }

        void RefreshProgrammList(int dtnr, int chnr)
        {

            txDescr.Text = "";
            lvProgramm.Items.Clear();
            if (dtnr == -1 || chnr == -1) return;
            CChannelData ch = _topManager.EPGData.ChannelData[chnr]; ;
            DateTime dt = _topManager.UsedDates[dtnr];
            ProgrammList = null;
            if (!ch.ProgrammDataByDate.TryGetValue(dt, out ProgrammList)) return;

            int i;
            CProgrammData pd;
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

        private void cbChannels_SelectedIndexChanged(object sender, EventArgs e)
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
                txDescr.Text = pd.SubTitle +"\r\n" + pd.Desc;
            }
            else
            {
                txDescr.Text = pd.Desc;
            }
        }

        void ResizeColumn()
        {
            lvProgramm.Columns[2].Width = -2;
        }

        private void lvProgramm_Resize(object sender, EventArgs e)
        {
            ResizeColumn();
        }

        private void addToScheduleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IgnoreClicks) return;
            if (lvProgramm.SelectedIndices.Count != 1) return;
            int k = lvProgramm.SelectedIndices[0];
            if (k == -1) return;
            CProgrammData pd = ProgrammList[k];
            _topManager.EPGUserData.AddToSchedule(pd.ChId, pd.Start);
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
