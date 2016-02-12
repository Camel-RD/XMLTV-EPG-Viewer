using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xmltv
{
    public partial class UCEditGroups : UserControl, IEPGView
    {
        private TopManager _TopManager = TopManager.St;
        private List<string> SelectedGroup = null;
        private string SelectedGroupName = "";
        private bool IgnoreClick = false;

        public UCEditGroups()
        {
            InitializeComponent();
        }

        private void UCEditGroups_Load(object sender, EventArgs e)
        {
            RefreshData();
        }

        void IEPGView.RefreshData()
        {
            RefreshData();
        }

        void IEPGView.ClearForm()
        {
            ClearForm();
        }

        void ClearForm()
        {
            tbName.Text = "";
            lbChannels.Items.Clear();
            lbSelected.Items.Clear();
            lbNames.Items.Clear();
        }

        void RefreshData()
        {
            SelectedGroup = null;
            SelectedGroupName = "";
            tbName.Text = "";
            lbSelected.Items.Clear();

            lbNames.BeginUpdate();
            lbNames.Items.Clear();
            foreach (string s in _TopManager.ChannelsByGroup.Keys)
            {
                lbNames.Items.Add(s);
            }
            lbNames.EndUpdate();
            
            lbChannels.BeginUpdate();
            lbChannels.Items.Clear();
            foreach (CChannelData ch in _TopManager.EPGData.ChannelData)
            {
                lbChannels.Items.Add(ch.DisplayNameR);
            }
            lbChannels.EndUpdate();
        }

        void SelectGroup(string name)
        {
            if (SelectedGroupName == name) return;
            SelectedGroup = _TopManager.ChannelsByGroup[name];
            if (SelectedGroup == null)
            {
                SelectedGroup = new List<string>();
                _TopManager.ChannelsByGroup[name] = SelectedGroup;
            }
            int k = lbNames.FindString(name);
            if (k != -1 && k != lbNames.SelectedIndex)
            {
                IgnoreClick = true;
                lbNames.SelectedIndex = k;
                IgnoreClick = false;
            }
            SelectedGroupName = name;
            tbName.Text = name;
            lbSelected.BeginUpdate();
            lbSelected.Items.Clear();
            CChannelData ch;
            foreach (string chid in SelectedGroup)
            {
                if (_TopManager.EPGData.ChannelDataById.TryGetValue(chid, out ch))
                {
                    ch = _TopManager.EPGData.ChannelDataById[chid];
                    lbSelected.Items.Add(ch.DisplayNameR);
                }
            }
            lbSelected.EndUpdate();
        }

        bool AddGroup(string name)
        {
            if (name == "") return false;
            if (!_TopManager.EPGUserData.AddGroup(name)) return false;
            lbNames.Items.Add(name);
            SelectGroup(name);
            return true;
        }

        void DeleteGroup(int listnr)
        {
            if (listnr == -1) return;
            SelectGroup("");
            string name = (string)lbNames.Items[listnr];
            lbNames.Items.RemoveAt(listnr);
            _TopManager.EPGUserData.DeleteGroup(name);
        }

        bool RenameGroup(int listnr, string newname)
        {
            if (listnr == -1) return false;
            string name = (string)lbNames.Items[listnr];
            if (!_TopManager.EPGUserData.RenameGroup(name, newname)) return false;
            IgnoreClick = true;
            lbNames.Items[listnr] = newname;
            IgnoreClick = false;
            return true;
        }

        void AddChannel(int chnr)
        {
            if (SelectedGroup == null) return;
            CChannelData ch = _TopManager.EPGData.ChannelData[chnr];
            string chid = ch.Id;
            if (SelectedGroup.Contains(chid)) return;
            SelectedGroup.Add(chid);
            lbSelected.Items.Add(ch.DisplayNameR);
            _TopManager.EPGUserData.HasChanged = true;
        }

        void RemoveChannel(int chnr)
        {
            if (SelectedGroup == null) return;
            SelectedGroup.RemoveAt(chnr);
            _TopManager.EPGUserData.HasChanged = true;
            lbSelected.Items.RemoveAt(chnr);
            _TopManager.EPGUserData.HasChanged = true;
        }

        private void btAdd_Click(object sender, EventArgs e)
        {
            if (tbName.Text == "") return;
            if (tbName.Text.Length > 12)
                tbName.Text = tbName.Text.Substring(0, 12);
            AddGroup(tbName.Text);
        }

        private void btChange_Click(object sender, EventArgs e)
        {
            if (SelectedGroup == null) return;
            int k = lbNames.SelectedIndex;
            if (k == -1) return;
            if (tbName.Text == "") return;
            if (tbName.Text.Length > 12)
                tbName.Text = tbName.Text.Substring(0, 12);
            RenameGroup(k, tbName.Text);
        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            if (SelectedGroup == null) return;
            int k = lbNames.SelectedIndex;
            if (k == -1) return;
            DeleteGroup(k);
        }

        private void lbNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IgnoreClick) return;
            int k = lbNames.SelectedIndex;
            if (k == -1) return;
            string name = lbNames.Text;
            SelectGroup(name);
        }

        private void lbChannels_DoubleClick(object sender, EventArgs e)
        {
            int k = lbChannels.SelectedIndex;
            if (k == -1) return;
            AddChannel(k);
        }

        private void lbSelected_DoubleClick(object sender, EventArgs e)
        {
            int k = lbSelected.SelectedIndex;
            if (k == -1) return;
            RemoveChannel(k);
        }

    }
}
