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
    public partial class UCSearch : UserControl, IEPGView
    {
        private TopManager _topManager = TopManager.St;
        List<CProgrammData> ProgrammList = null;
        Dictionary<DateTime, List<CProgrammData>> ProgrammListByDate = new Dictionary<DateTime, List<CProgrammData>>();

        private bool IgnoreClicks = false;
        List<DateTime> DatesUsed = new List<DateTime>();

        public UCSearch()
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
            ClearForm();
            ResizeColumn();
        }


        void IEPGView.ClearForm()
        {
            ClearForm();
        }

        void IEPGView.RefreshData()
        {
            ClearForm();
        }


        void FirstSelect()
        {
            if (DatesUsed.Count == 0) return;
            if (_topManager.EPGData.ChannelData.Count == 0) return;
            DateTime dt = DateTime.Today;
            int k = DatesUsed.FindIndex(d => d == dt);
            if (k == -1) return;
            cbDates.SelectedIndex = k;
        }

        void ClearForm()
        {
            Enabled = false;
            cbDates.Items.Clear();
            lvProgramm.Items.Clear();
            tbSearchText.Text = "";
            txDescr.Text = "";
            ProgrammList = null;
            ProgrammListByDate.Clear();
            DatesUsed.Clear();
            Enabled = true;
        }

        void SearchForText(string text)
        {
            ClearForm();
            if (text == "") return;
            ProgrammList = _topManager.EPGData.SearchForText(text);
            if (ProgrammList.Count == 0) return;
            int i;
            DateTime dt,lastdt = DateTime.MinValue;
            CProgrammData pd = null;
            List<CProgrammData> pdlist = null;
            for (i = 0; i < ProgrammList.Count; i++)
            {
                pd = ProgrammList[i];
                dt = pd.Start.Date;
                if (dt != lastdt)
                {
                    pdlist = new List<CProgrammData>();
                    ProgrammListByDate[dt] = pdlist;
                    DatesUsed.Add(dt);
                    lastdt = dt;
                }
                pdlist.Add(pd);
            }

            string s;
            cbDates.BeginUpdate();
            for (i = 0; i < DatesUsed.Count; i++)
            {
                dt = DatesUsed[i];
                s = dt.ToString("MMM-dd ddd");
                cbDates.Items.Add(s);
            }
            cbDates.EndUpdate();

            RefreshProgrammList();
        }


        private void SelectDate(int dtnr)
        {
            txDescr.Text = "";
            lvProgramm.Items.Clear();
            if (DatesUsed.Count == 0) return;
            if (dtnr == -1) return;
            DateTime dt = DatesUsed[dtnr];

            ProgrammList = ProgrammListByDate[dt];
            
            RefreshProgrammList();
        }

        void RefreshProgrammList()
        {

            txDescr.Text = "";
            lvProgramm.Items.Clear();
            if (ProgrammList.Count == 0) return;

            int i;
            CProgrammData pd;
            string sstart;
            ListViewItem lvi;

            lvProgramm.BeginUpdate();

            for (i = 0; i < ProgrammList.Count; i++)
            {
                pd = ProgrammList[i];
                sstart = pd.Start.ToString("MM.dd HH:mm");
                lvi = lvProgramm.Items.Add(sstart);
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
            int k = cbDates.SelectedIndex;
            if (k == -1) return;
            SelectDate(k);
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

        private void tbSearchText_KeyPress(object sender, KeyPressEventArgs e)
        {
            string s = tbSearchText.Text;
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (s == "")
                {
                    ClearForm();
                    return;
                }
                tbSearchText.Text = "";
                SearchForText(s);
            }
        }


    }
}
