using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xmltv
{
    public partial class UCSourceEditor : UserControl, IEPGView
    {
        private TopManager _TopManager = TopManager.St;

        private CSource SelectedSource = null;

        private bool IgnorelbSourcesClick = false;
        private bool IgnorelbKeepChannelsClick = false;

        private ListBoxDragDropHelper AllToKeepDragDropHelper;
        private ListBoxDragDropHelper KeepToKeepDragDropHelper;

        //List<string> URLList = new List<string>();

        public UCSourceEditor()
        {
            InitializeComponent();
        }

        private void UCSourceEditor_Load(object sender, EventArgs e)
        {
            AllToKeepDragDropHelper = new ListBoxDragDropHelper(lbAllChanels, lbKeepChannels, OnAllToKeepDragDrop);
            KeepToKeepDragDropHelper = new ListBoxDragDropHelper(lbKeepChannels, lbKeepChannels, OnKeepToKeepDragDrop);
            RefreshSourceList();
            GetURLList();
        }

        void IEPGView.ClearForm()
        {
            ClearForm();
        }

        void IEPGView.RefreshData()
        {
            RefreshSourceList();
        }


        public void LoadData()
        {
            try
            {
                TopManager.St.LoadSources();
            }
            catch (Exception)
            {
                return;
            }
            RefreshSourceList();
        }

        void ClearForm()
        {
            Enabled = false;
            SelectedSource = null;
            lbAllChanels.Items.Clear();
            lbKeepChannels.Items.Clear();
            lbTvSources.Items.Clear();
            txPrefix.Text = "";
            tbName.Text = "";
            cbURL.Text = "";
            tbRName.Text = "";
            tbAddHours.Text = "";
            Enabled = true;
        }

        void RefreshSelectSource()
        {
            if (SelectedSource == null) return;
            lbAllChanels.Items.Clear();
            lbKeepChannels.Items.Clear();

            int k = FindListItem(SelectedSource.Name);
            if (lbTvSources.SelectedIndex != k) lbTvSources.SelectedIndex = k;
            if (SelectedSource == null) return;

            tbName.Text = SelectedSource.Name;
            cbURL.Text = SelectedSource.URL;
            txPrefix.Text = SelectedSource.Prefix;
            int i;
            string chid,s,displnm;
            lbAllChanels.BeginUpdate();
            lbKeepChannels.BeginUpdate();
            for (i = 0; i < SelectedSource.AllChannels.Count; i++)
            {
                lbAllChanels.Items.Add(SelectedSource.AllChannels[i].DisplayNameA);
            }
            for (i = 0; i < SelectedSource.KeepChannelsIds.Count; i++)
            {
                chid = SelectedSource.KeepChannelsIds[i];
                displnm = GetDisplayNameForChId(chid);
                if(SelectedSource.RenameChannelsIds.TryGetValue(chid, out s))
                {
                    s = displnm + " - " + s;
                }else
                {
                    s = displnm;
                }
                lbKeepChannels.Items.Add(s);
            }
            lbAllChanels.EndUpdate();
            lbKeepChannels.EndUpdate();
        }

        void SelectSource(CSource source)
        {
            if (SelectedSource == source) return;
            SelectedSource = source;
            if (source == null)
            {
                lbAllChanels.Items.Clear();
                lbKeepChannels.Items.Clear();
                tbName.Text = "";
                cbURL.Text = "";
                txPrefix.Text = "";
                tbAddHours.Text = "";
                lbAllChanels.Enabled = false;
                lbKeepChannels.Enabled = false;
                lbKeepChannels.Enabled = false;
                //sourceToolStripMenuItem.Enabled = false;
                return;
            }

            lbAllChanels.Enabled = true;
            lbKeepChannels.Enabled = true;
            lbKeepChannels.Enabled = true;
            //sourceToolStripMenuItem.Enabled = true;

            RefreshSelectSource();
        }

        void RefreshSourceList()
        {
            int i;
            SelectSource(null);
            lbTvSources.BeginUpdate();
            lbTvSources.Items.Clear();
            for (i = 0; i < TopManager.St.Sources.Count; i++)
            {
                lbTvSources.Items.Add(TopManager.St.Sources[i].Name);
            }
            lbTvSources.EndUpdate();
        }

        int FindListItem(string sourcename)
        {
            int i;
            for (i = 0; i < lbTvSources.Items.Count; i++)
            {
                if (lbTvSources.Items[i].ToString() == sourcename)
                {
                    return i;
                }
            }
            return -1;
        }

        void SelectSourceByNr(int nr)
        {
            SelectSource(TopManager.St.Sources[nr]);
        }


        void DoMsg(string msg)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(DoMsgA), msg);
            }
            else
            {
                DoMsgA(msg);
            }
        }

        private void DoMsgA(string msg)
        {
            MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        bool CheckNewName(string name)
        {
            if (name == "")
            {
                DoMsg("Enter source name");
                return false;
            }
            if (tbName.Text.Length > 12)
                tbName.Text = tbName.Text.Substring(0, 12);
            if (TopManager.St.SourcesByName.ContainsKey(name))
            {
                DoMsg("This name allready used");
                return false;
            }
            return true;
        }

        bool CheckConfigData()
        {
            if (SelectedSource == null) return false;
            TopManager.St.CheckConfigData();
            return SelectedSource.ConfigOk;
        }

        void ReadAllChannels()
        {
            if (SelectedSource == null)
            {
                DoMsg("First select one sources from list");
                return;
            }
            string s = SelectedSource.CheckConfigData();
            if (s != "OK")
            {
                DoMsg("Config error:\n" + s);
                return;
            }
            MainForm mainform = Parent as MainForm;
            if(mainform != null) mainform.ShowTaskMonitorX();
            _TopManager.StartGetAllChannels(OnGetAllChannelsEvent, SelectedSource.Name);
        }

        void OnGetAllChannelsEvent(ESimpleEvent taskevent)
        {
            Invoke(new Action<ESimpleEvent>(OnGetAllChannelsEventA)
                , new object[] { taskevent });
        }

        void OnGetAllChannelsEventA(ESimpleEvent taskevent)
        {
            switch (taskevent)
            {
                case ESimpleEvent.finished:
                    RefreshSelectSource();
                    Application.DoEvents();
                    DoMsg("Done!");
                    MainForm mainform = Parent as MainForm;
                    if (mainform != null) mainform.HideTaskMonitorX();
                    break;
                case ESimpleEvent.failed:
                    DoMsg("Failed!");
                    break;
            }
        }


        void GetURLList()
        {
            string filename = TopManager.DataFolder + "\\rytec.sources.xml";
            CXZSource xzs;
            try
            {
                xzs = TopManager.St.LoadXZSources(filename);
            }
            catch (Exception e)
            {
                return;
            }
            cbURL.Items.Clear();
            if (xzs == null) return;
            if (xzs.sourcecat == null) return;

            foreach (var cat in xzs.sourcecat)
            {
                foreach (var src in cat.source)
                {
                    cbURL.Items.Add("+" + src.description);
                    if (src.url != null)
                    {
                        foreach (var url in src.url)
                        {
                            cbURL.Items.Add("  " + url);
                        }
                    }
                }
            }
        }

        string GetDisplayNameForChId(string chid)
        {
            if (SelectedSource == null) return "";
            CChannelData ch;
            if(SelectedSource.AllChannelsById.TryGetValue(chid, out ch))
                return ch.DisplayNameA;
            return chid;
        }

        bool AddToKeepChannels(string chid)
        {
            if (SelectedSource == null) return false;
            if (SelectedSource.KeepChannelsIds.Contains(chid)) return false;
            SelectedSource.KeepChannelsIds.Add(chid);
            lbKeepChannels.Items.Add(GetDisplayNameForChId(chid));
            
            TopManager.St.ConfigHasChenged = true;

            return true;
        }

        bool AddToKeepChannels(int fromnr, int tonr)
        {
            if (SelectedSource == null) return false;
            if (fromnr < 0) return false;
            string chid = SelectedSource.AllChannels[fromnr].Id;
            string displnm = GetDisplayNameForChId(chid);
            if (SelectedSource.KeepChannelsIds.Contains(chid)) return false;
            if (tonr == -1)
            {
                SelectedSource.KeepChannelsIds.Add(chid);
                lbKeepChannels.Items.Add(displnm);
            }
            else
            {
                SelectedSource.KeepChannelsIds.Insert(tonr, chid);
                lbKeepChannels.Items.Insert(tonr, displnm);
            }
            TopManager.St.ConfigHasChenged = true;
            return true;
        }

        bool RemoveFromKeepChannels(int nr)
        {
            if (SelectedSource == null) return false;
            string id = SelectedSource.KeepChannelsIds[nr];
            SelectedSource.KeepChannelsIds.RemoveAt(nr);
            SelectedSource.RenameChannelsIds.Remove(id);
            lbKeepChannels.Items.RemoveAt(nr);
            TopManager.St.ConfigHasChenged = true;
            return true;
        }

        bool RenameChannel(int nr, string rname)
        {
            if (SelectedSource == null) return false;
            string id = SelectedSource.KeepChannelsIds[nr];
            string displnm = GetDisplayNameForChId(id);
            if (rname == "")
            {
                SelectedSource.RenameChannelsIds.Remove(id);
            }
            else
            {
                SelectedSource.RenameChannelsIds[id] = rname;
                displnm = displnm + " - " + rname;
            }
            IgnorelbKeepChannelsClick = true;
            lbKeepChannels.Items[nr] = displnm;
            lbKeepChannels.SelectedIndex = nr;
            IgnorelbKeepChannelsClick = false;
            TopManager.St.ConfigHasChenged = true;
            return true;
        }

        void MoveChenel(int fromnr, int tonr)
        {
            if (SelectedSource == null) return;
            int k = lbKeepChannels.Items.Count;
            if (fromnr >= k || fromnr < 0 || tonr >= k || tonr < -1 || fromnr == tonr) return;
            string s1 = SelectedSource.KeepChannelsIds[fromnr];
            SelectedSource.KeepChannelsIds.RemoveAt(fromnr);
            if (tonr == -1)
            {
                SelectedSource.KeepChannelsIds.Add(s1);
                s1 = (string) lbKeepChannels.Items[fromnr];
                lbKeepChannels.BeginUpdate();
                lbKeepChannels.Items.RemoveAt(fromnr);
                lbKeepChannels.Items.Add(s1);
                lbKeepChannels.EndUpdate();
                lbKeepChannels.SelectedIndex = lbKeepChannels.Items.Count - 1;
            }
            else
            {
                SelectedSource.KeepChannelsIds.Insert(tonr, s1);
                s1 = (string)lbKeepChannels.Items[fromnr];
                lbKeepChannels.BeginUpdate();
                if (fromnr > tonr)
                {
                    lbKeepChannels.Items.RemoveAt(fromnr);
                    lbKeepChannels.Items.Insert(tonr, s1);
                }
                else
                {
                    lbKeepChannels.Items.Insert(tonr, s1);
                    lbKeepChannels.Items.RemoveAt(fromnr);
                }
                lbKeepChannels.EndUpdate();
                lbKeepChannels.SelectedIndex = tonr;
            }
            TopManager.St.ConfigHasChenged = true;
        }

        void DoAddSource()
        {
            if (tbName.Text.Length > 12)
                tbName.Text = tbName.Text.Substring(0, 12);
            if (!CheckNewName(tbName.Text)) return;
            CSource source;
            try
            {
                source = TopManager.St.CreateSource(tbName.Text);
            }
            catch (MyException me)
            {
                DoMsg("Can't add source:\n" + me.Message);
                return;
            }
            source.URL = cbURL.Text;
            if (txPrefix.Text.Length > 10) txPrefix.Text = txPrefix.Text.Substring(0, 10);
            source.Prefix = txPrefix.Text;
            RefreshSourceList();
            SelectSource(source);
        }


        void DoDelete()
        {
            if (SelectedSource == null) return;
            CSource sr = SelectedSource;
            SelectSource(null);
            TopManager.St.DeleteSource(sr.Name);
            RefreshSourceList();
        }

        private void lbTvSources_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IgnorelbSourcesClick) return;
            int k = lbTvSources.SelectedIndex;
            if (k == -1)
            {
                SelectSource(null);
            }
            else
            {
                SelectSourceByNr(k);
            }
        }

        void DoApplyChanges()
        {
            if (SelectedSource == null) return;
            if (tbName.Text.Length > 12)
                tbName.Text = tbName.Text.Substring(0, 12);

            if (SelectedSource.Name != tbName.Text)
            {
                if (!CheckNewName(tbName.Text)) return;
                int k = FindListItem(SelectedSource.Name);
                if (!SelectedSource.Rename(tbName.Text))
                {
                    DoMsg("Name allready used");
                    return;
                }
                IgnorelbSourcesClick = true;
                lbTvSources.Items[k] = tbName.Text;
                lbTvSources.SelectedIndex = k;
                IgnorelbSourcesClick = false;
            }

            CleanURL();
            SelectedSource.URL = cbURL.Text;
            if (txPrefix.Text.Length > 10) txPrefix.Text = txPrefix.Text.Substring(0, 10);
            SelectedSource.Prefix = txPrefix.Text;

            TopManager.St.ConfigHasChenged = true;
        }

        private void lbAllChanels_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int k = lbAllChanels.SelectedIndex;
            if (k == -1) return;
            if (SelectedSource == null) return;
            AddToKeepChannels(SelectedSource.AllChannels[k].Id);
        }

        private void lbKeepChannels_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int k = lbKeepChannels.SelectedIndex;
            if (k == -1) return;
            if (SelectedSource == null) return;
            RemoveFromKeepChannels(k);
        }

        private void btRename_Click(object sender, EventArgs e)
        {
            int k = lbKeepChannels.SelectedIndex;
            if (k == -1) return;
            if (SelectedSource == null) return;
            if (tbRName.Text.Length > 12)
                tbRName.Text = tbRName.Text.Substring(1, 12);
            RenameChannel(k, tbRName.Text);
        }

        private void lbKeepChannels_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IgnorelbKeepChannelsClick) return;
            int k = lbKeepChannels.SelectedIndex;
            if (k == -1 || SelectedSource == null)
            {
                tbRName.Text = "";
                return;
            }
            string s, chid = SelectedSource.KeepChannelsIds[k];
            if (!SelectedSource.RenameChannelsIds.TryGetValue(chid, out s))
            {
                s = "";
            }
            tbRName.Text = s;

        }


        private void OnAllToKeepDragDrop(ListBoxDragDropHelper sender, int dragitemindex, int dropitemindex)
        {
            AddToKeepChannels(dragitemindex, dropitemindex);
        }

        private void OnKeepToKeepDragDrop(ListBoxDragDropHelper sender, int dragitemindex, int dropitemindex)
        {
            MoveChenel(dragitemindex, dropitemindex);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            DoAddSource();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            DoApplyChanges();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            DoDelete();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            ReadAllChannels();
        }

        private void cbURL_SelectedValueChanged(object sender, EventArgs e)
        {

        }

        void CleanURL()
        {
            string s = cbURL.Text;
            if (s.Length < 3 || s[0] == '+')
            {
                cbURL.Text = "";
                return;
            }
            cbURL.Text = cbURL.Text.Trim();
        }

        private void cbURL_Leave(object sender, EventArgs e)
        {
            CleanURL();
        }

    }
}
