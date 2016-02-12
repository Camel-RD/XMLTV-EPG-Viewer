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
    public partial class UCTaskMonitor : UserControl
    {
        List<string> SourceNamesInList = new List<string>();
        List<string> SourceTextInList = new List<string>();


        public UCTaskMonitor()
        {
            InitializeComponent();
        }


        private void TaskMonitor_Load(object sender, EventArgs e)
        {
            AddAllLog();
            UpdateData();
            RefreshSourceList();
            ResizeColumn1();
            ResizeColumn2();
            TopManager.St.SourceEventListener += OnSourceEvent;
            TopManager.St.LogManager.SourceEventListener += OnAddToLog;
        }

        void OnAddToLog(object sender, CLogEventArgs e)
        {
            if (!Visible) return;
            if (InvokeRequired)
            {
                Invoke(new Action<CLogEntry>(AddLogEntry), new object[] { e.LogEntry });
                return;
            }
            AddLogEntry(e.LogEntry);
        }

        void AddLogEntry(CLogEntry logentry)
        {
            ListViewItem lvi;
            int k = 0;
            lvi = lvEvents.Items.Add(logentry.SourceName);
            lvi.SubItems.Add(logentry.Messsage);
            if (logentry.LogEntryType == ELogEntryType.Error) k = 1;
            lvi.ImageIndex = k;
        }

        void AddAllLog()
        {
            lvEvents.BeginUpdate();
            lvEvents.Items.Clear();
            List<CLogEntry> les = TopManager.St.LogManager.GetAll();
            foreach (CLogEntry logentry in les)
            {
                AddLogEntry(logentry);
            }
            lvEvents.EndUpdate();
        }

        void RefreshSourceList()
        {
            int i;
            ListViewItem lvi;
            if (lvSources.Items.Count != SourceNamesInList.Count)
            {
                lvSources.Items.Clear();
                for (i = 0; i < SourceNamesInList.Count; i++)
                {
                    lvi = lvSources.Items.Add(SourceNamesInList[i]);
                    lvi.SubItems.Add(SourceTextInList[i]);
                }
                return;
            }
            for (i = 0; i < SourceNamesInList.Count; i++)
            {
                if (SourceNamesInList[i] != lvSources.Items[i].Text)
                {
                    lvSources.Items[i].Text = SourceNamesInList[i];
                }
                if (SourceTextInList[i] != lvSources.Items[i].SubItems[1].Text)
                {
                    lvSources.Items[i].SubItems[1].Text = SourceTextInList[i];
                }
            }
        }

        void UpdateData()
        {
            int i;
            CSource source;
            ListViewItem lvi;
            if (TopManager.St.SourcesByName.Count != SourceNamesInList.Count)
            {
                SourceNamesInList.Clear();
                SourceTextInList.Clear();
                for (i = 0; i < TopManager.St.SourcesByName.Count; i++)
                {
                    source = TopManager.St.Sources[i];
                    SourceNamesInList.Add(source.Name);
                    SourceTextInList.Add("");
                }
                return;
            }
            for (i = 0; i < TopManager.St.SourcesByName.Count; i++)
            {
                source = TopManager.St.Sources[i];
                SourceNamesInList[i] = source.Name;
                SourceTextInList[i] = "";
            }
        }

        private void UpdateData(CSource source, string msg)
        {
            int i;
            if (TopManager.St.SourcesByName.Count != SourceNamesInList.Count)
                UpdateData();
                //throw new Exception("wrong state");

            SourceTextInList[source.GetSourceNr()] = msg;
        }

        private void OnSourceEvent(object sender, CSourceEventArgs e)
        {
            CSource source = sender as CSource;
            if (source == null) return;
            string s = source.GetStateString();
            UpdateData(source, s);
            if (!Visible) return;
            Invoke(new Action(RefreshSourceList));
        }

        private void TaskMonitor_FormClosing(object sender, FormClosingEventArgs e)
        {
            TopManager.St.SourceEventListener -= OnSourceEvent;
            TopManager.St.LogManager.SourceEventListener -= OnAddToLog;
        }

        private void TaskMonitor_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void cancelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lvSources.SelectedIndices.Count == 0) return;
            string sourcename = lvSources.SelectedItems[0].Text;
            TopManager.St.CancelTask(sourcename);

        }

        private void cancelAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TopManager.St.CancelAllTask();
        }

        void ResizeColumn1()
        {
            lvSources.Columns[1].Width = -2;
        }
        void ResizeColumn2()
        {
            lvEvents.Columns[1].Width = -2;
        }

        private void lvSources_Resize(object sender, EventArgs e)
        {
            ResizeColumn1();
        }

        private void lvEvents_Resize(object sender, EventArgs e)
        {
            ResizeColumn2();
        }

        private void lvEvents_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lvEvents.SelectedIndices.Count == 0) return;
            string s = lvEvents.SelectedItems[0].Text;
            s += "\n" + lvEvents.SelectedItems[0].SubItems[1].Text;
            MessageBox.Show(this, s, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

    }
}
