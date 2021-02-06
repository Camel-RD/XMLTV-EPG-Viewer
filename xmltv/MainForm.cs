using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using xmltv.Properties;

namespace xmltv
{
    public interface IEPGView
    {
        void ClearForm();
        void RefreshData();
    }

    internal enum EFormView
    {
        None,
        EPG,
        EPGGrouped,
        Scheduled,
        EditGroups,
        EditSources,
        Search,
        TagedTitles
    }

    public partial class MainForm : Form
    {

        TopManager _topManager = new TopManager();

        private IEPGView CurrentEPGView = null;
        private UserControl CurrentUCView = null;
        EFormView CurrentFoemView = EFormView.None;

        private UCTaskMonitor TaskMonitorUC = null;

        
        void LogError(string msg)
        {
            TopManager.St.LogManager.Add(ELogEntryType.Error, "app", msg);
        }

        void DoMsg(string s)
        {
            MessageBox.Show(this, s, "MyEPG", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        public MainForm()
        {
            InitializeComponent();
            loadFromWebSelectedtoolStripMenuItem.DropDown.Closing +=
                loadFromWebSelectedtoolStripMenuItem_DropDown_Closing;
            //menuStrip1.Renderer.RenderItemCheck += Renderer_RenderItemCheck;
        }

        private void ScalePoint(ref Point p, float fx, float fy, int dx = 0, int dy = 0)
        {
            p.X = (int)((float)p.X * fx) + dx;
            p.Y = (int)((float)p.Y * fy) + dy;
        }

        private void Renderer_RenderItemCheck(object sender, ToolStripItemImageRenderEventArgs e)
        {
            Rectangle rc = new Rectangle(e.ImageRectangle.Left - 2, 1, e.ImageRectangle.Width + 4, e.Item.Height - 2);
            var pen = new Pen(ForeColor, 2f);
            var p1 = new Point(5, 6);
            var p2 = new Point(7, 9);
            var p3 = new Point(11, 4);
            float fx = rc.Width / 16.0f;
            float fy = rc.Height / 16.0f;
            ScalePoint(ref p1, fx, fy, rc.Left, rc.Top);
            ScalePoint(ref p2, fx, fy, rc.Left, rc.Top);
            ScalePoint(ref p3, fx, fy, rc.Left, rc.Top);
            var ps = new[] { p1, p2, p3 };
            e.Graphics.DrawLines(pen, ps);

            pen.Dispose();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadSettings();
            _topManager.LoadSources();
            ShowEPGView();
            LoadEPG();
        }

        private void CheckMisiingData()
        {
            List<string> names = _topManager.EPGData.GetNoDataChannels(DateTime.Today, 20);
            string s = "";
            if (names.Count == 0)
            {
                DoMsg("No missing data.");
                return;
            }
            foreach (string s1 in names)
            {
                if (s == "")
                {
                    s = s1;
                }
                else
                {
                    s += ", " + s1;
                }
            }
            DoMsg("Missing data:\n" + s);


        }

        void LoadSettings()
        {
            _topManager.LoadSettings();
            CProgrammData.MyTimePlus = new TimeSpan(_topManager.Settings.TimePlusHours, 0, 0);
            SetFontSize(_topManager.Settings.FontSize);
            SetColorTheme(_topManager.Settings.ColorThemeId, true);
            checkMissingDataOnOpenToolStripMenuItem.Checked = _topManager.Settings.CheckMisingDataOnOpen;
            if (_topManager.Settings.MainFormWidth > -1)
            {
                this.Width = _topManager.Settings.MainFormWidth;
                this.Height = _topManager.Settings.MainFormHight;
            }
            if (_topManager.Settings.MainFormX > -1)
            {
                this.Top = _topManager.Settings.MainFormY;
                this.Left = _topManager.Settings.MainFormX;
            }
        }

        void SaveSettings()
        {
            _topManager.Settings.FontSize = this.Font.Size;
            _topManager.Settings.TimePlusHours = CProgrammData.MyTimePlus.Hours;
            _topManager.SaveSettings();
        }

        void LoadEPG()
        {
            ClearView();
            this.Enabled = false;
            try
            {
                _topManager.LoadXMLTvFile();
                _topManager.LoadUserData();
                _topManager.EPGUserData.RemoveOldScheduleEntries();
                _topManager.EPGUserData.DoAutoSchedule();
            }
            catch (Exception e)
            {
                DoMsg("Filed to load data.");
                LogError(e.Message);
            }
            this.Enabled = true;
            RefreshView();
        }

        void RemoveEmtyChannels()
        {
            ClearView();
            this.Enabled = false;
            _topManager.EPGData.RemoveEmptyChannels();
            this.Enabled = true;
            RefreshView();
        }

        void ClearView()
        {
            if (CurrentEPGView == null) return;
            CurrentEPGView.ClearForm();
            Application.DoEvents();
        }

        void RefreshView()
        {
            if (CurrentEPGView == null) return;
            CurrentEPGView.RefreshData();
            Application.DoEvents();
        }


        private void ChangeView(EFormView newformview)
        {
            if (CurrentFoemView == newformview) return;
            if (CurrentFoemView != EFormView.None)
            {
                if (CurrentUCView != null)
                    Controls.Remove(CurrentUCView);
            }

            UserControl newuc = null;
            IEPGView newepgview = null;

            switch (newformview)
            {
                case EFormView.None:
                    CurrentFoemView = EFormView.None;
                    return;

                case EFormView.EPG:
                    newuc = new UCEPGView();
                    newepgview = newuc as IEPGView;
                    break;

                case EFormView.EPGGrouped:
                    newuc = new UCEPGView2();
                    newepgview = newuc as IEPGView;
                    break;

                case EFormView.Scheduled:
                    newuc = new UCEPGView3();
                    newepgview = newuc as IEPGView;
                    break;

                case EFormView.EditGroups:
                    newuc = new UCEditGroups();
                    newepgview = newuc as IEPGView;
                    break;

                case EFormView.EditSources:
                    newuc = new UCSourceEditor();
                    newepgview = newuc as IEPGView;
                    break;

                case EFormView.Search:
                    newuc = new UCSearch();
                    newepgview = newuc as IEPGView;
                    break;
                case EFormView.TagedTitles:
                    newuc = new UCTagedTitles();
                    newepgview = newuc as IEPGView;
                    break;
            }

            CurrentUCView = newuc;
            CurrentEPGView = newepgview;
            Controls.Add(CurrentUCView);
            CurrentUCView.Dock = DockStyle.Fill;
            CurrentUCView.BringToFront();
            CurrentEPGView.RefreshData();
            ApplyColorThemeA(newuc);
        }

        void ShowEPGView()
        {
            ChangeView(EFormView.EPG);
        }

        void ShowEPGGroupedView()
        {
            ChangeView(EFormView.EPGGrouped);
        }

        void ShowEPGScheduleView()
        {
            ChangeView(EFormView.Scheduled);
        }

        void ShowEditSourcesView()
        {
            ChangeView(EFormView.EditSources);
        }

        void ShowEditGroupsView()
        {
            ChangeView(EFormView.EditGroups);
        }

        void ShowSearchView()
        {
            ChangeView(EFormView.Search);
        }

        void ShowTagedTitlesView()
        {
            ChangeView(EFormView.TagedTitles);
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowEditSourcesView();
        }

        public void ShowTaskMonitorX()
        {
            if (CurrentUCView != null)
            {
                CurrentUCView.Enabled = false;
                CurrentUCView.Visible = false;
            }
            if (TaskMonitorUC == null)
            {
                TaskMonitorUC = new UCTaskMonitor();
                Controls.Add(TaskMonitorUC);
                TaskMonitorUC.Dock = DockStyle.Fill;
            }
            menuStrip1.Enabled = false;
            menuStrip1.Visible = false;
            menuStrip2.Enabled = true;
            menuStrip2.Visible = true;
            TaskMonitorUC.Visible = true;
            TaskMonitorUC.Enabled = true;
            TaskMonitorUC.BringToFront();
            ApplyColorThemeA(TaskMonitorUC);
        }

        public void HideTaskMonitorX()
        {
            if (TaskMonitorUC != null && TaskMonitorUC.Visible)
            {
                TaskMonitorUC.Enabled = false;
                TaskMonitorUC.Visible = false;
            }
            if (CurrentUCView != null)
            {
                CurrentUCView.Enabled = true;
                CurrentUCView.Visible = true;
                CurrentUCView.BringToFront();
            }
            menuStrip2.Enabled = false;
            menuStrip2.Visible = false;
            menuStrip1.Enabled = true;
            menuStrip1.Visible = true;
        }

        bool ChechConfig()
        {
            string s = _topManager.CheckConfigData();
            if (s == "OK") return true;
            DoMsg("Error in config data:\n" + s);
            return false;
        }

        private void reloadConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearView();
            _topManager.LoadSources();
            ChechConfig();
            if(CurrentUCView is UCSourceEditor)
                RefreshView();
        }

        void OnUpdateAllEvent(ESimpleEvent taskevent)
        {
            Invoke(new Action<ESimpleEvent>(OnUpdateAllEventA)
                , new object[] { taskevent });
        }

        void OnUpdateAllEventA(ESimpleEvent taskevent)
        {
            switch (taskevent)
            {
                case ESimpleEvent.started:
                    closeToolStripMenuItem.Enabled = false;
                    break;
                case ESimpleEvent.finished:
                    DoMsg("Done!");
                    closeToolStripMenuItem.Enabled = true;
                    HideTaskMonitorX();
                    RefreshView();
                    break;
                case ESimpleEvent.canceled:
                case ESimpleEvent.failed:
                    closeToolStripMenuItem.Enabled = true;
                    DoMsg("Failed!");
                    break;
            }
        }

        private void downloadAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ChechConfig()) return;
            ShowTaskMonitorX();
            _topManager.StartUpdateAllTask(OnUpdateAllEvent);
        }

        private void updateAllFromGzToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ChechConfig()) return;
            ShowTaskMonitorX();
            _topManager.StartUpdateAllFromGzTask(OnUpdateAllEvent);
        }

        private void saveEPGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                _topManager.SaveXMLTvFile();
                _topManager.SaveUserData();
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                DoMsg("Filed to save data.");
            }
        }

        private void loadEPGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadEPG();
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearView();
            _topManager.ClearEPG();
        }

        private void taskMonitorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowTaskMonitorX();
        }

        private void ePGToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ShowEPGView();
        }

        private void groupEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowEditGroupsView();
        }

        private void groupedEPGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _topManager.FilterWithTags = false;
            ShowEPGGroupedView();
        }

        private void ePGGroupedFilteredToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _topManager.FilterWithTags = true;
            ShowEPGGroupedView();
        }

        private void scheduledProgrammsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowEPGScheduleView();
        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSearchView();
        }

        private void updateFromWebToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            updateFromWebToolStripMenuItem.DropDownItems.Clear();
            foreach (CSource sr in _topManager.Sources)
            {
                updateFromWebToolStripMenuItem.DropDownItems.Add(sr.Name);
            }
            ApplyColorThemeA(updateFromWebToolStripMenuItem);
        }

        private void updateFromGzToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            updateFromGzToolStripMenuItem.DropDownItems.Clear();
            foreach (CSource sr in _topManager.Sources)
            {
                updateFromGzToolStripMenuItem.DropDownItems.Add(sr.Name);
            }
            ApplyColorThemeA(updateFromGzToolStripMenuItem);
        }

        private void updateFromGzToolStripMenuItem_DropDownItemClicked(object sender, EventArgs e)
        {
            dataSourcesToolStripMenuItem.HideDropDown();
            ToolStripItemClickedEventArgs ee = e as ToolStripItemClickedEventArgs;
            string sourcename = ee.ClickedItem.Text;
            ClearView();
            ShowTaskMonitorX();
            TopManager.St.StartUpdateSourceFromGzTask(OnUpdateAllEvent, sourcename);
            RefreshView();
        }

        private void updateFromWebToolStripMenuItem_DropDownItemClicked(object sender, EventArgs e)
        {
            dataSourcesToolStripMenuItem.HideDropDown();
            ToolStripItemClickedEventArgs ee = e as ToolStripItemClickedEventArgs;
            string sourcename = ee.ClickedItem.Text;
            ClearView();
            ShowTaskMonitorX();
            TopManager.St.StartUpdateSourceFromWebTask(OnUpdateAllEvent, sourcename);
            RefreshView();
        }

        private void loadConfigFromToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.CheckFileExists = true;
            od.Multiselect = false;
            od.Filter = "*-config.xml|*-config.xml";
            od.InitialDirectory = TopManager.DataFolder;
            if (od.ShowDialog() != DialogResult.OK) return;
            string fnm = od.FileName;

            ClearView();
            TopManager.St.LoadSources(fnm);
            RefreshView();

        }

        private void saveConfigAdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog od = new SaveFileDialog();
            od.Filter = "*.config.xml|*.config.xml";
            od.InitialDirectory = TopManager.DataFolder;
            if (od.ShowDialog() != DialogResult.OK) return;
            string fnm = od.FileName;
            TopManager.St.SaveSources(fnm);

        }

        private void loadFromToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.CheckFileExists = true;
            od.Multiselect = false;
            od.Filter = "*.epg.xml|*.epg.xml|*.xml|*.xml";
            od.InitialDirectory = TopManager.DataFolder;
            if (od.ShowDialog() != DialogResult.OK) return;
            string fnm = od.FileName;
            string fnm2 = Utils.RemoveExt(fnm) + ".userdata.xml";

            ClearView();
            //TopManager.St.LoadEpg(fnm);
            try
            {
                _topManager.LoadXMLTvFile(fnm);
                _topManager.LoadUserData(fnm2);
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                DoMsg("Load from file failed.");
            }
            RefreshView();

        }

        private void saveToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog od = new SaveFileDialog();
            od.Filter = "*.epg.xml|*.epg.xml|*.xml|*.xml";
            od.InitialDirectory = TopManager.DataFolder;
            if (od.ShowDialog() != DialogResult.OK) return;
            string fnm = od.FileName;
            string fnm2 = Utils.RemoveExt(fnm) + ".userdata.xml";
            //TopManager.St.SaveEPGData(fnm);
            try
            {
                _topManager.SaveXMLTvFile(fnm);
                _topManager.SaveUserData(fnm2);
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                DoMsg("Save to file failed.");
            }
        }

        private void saveConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TopManager.St.SaveSources();
        }

        void ClearTemp()
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(TopManager.TempFolder);
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo f in files)
                {
                    try
                    {
                        f.Delete();
                    }
                    catch (Exception){}
                }
            }
            catch (Exception){}
        }

        void SaveChangedData()
        {
            try
            {
                if(_topManager.EPGData.HasChanged) _topManager.SaveXMLTvFile();
                if (_topManager.EPGUserData.HasChanged) _topManager.SaveUserData();
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                DoMsg("Filed to save data.");
            }
            
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.Width != _topManager.Settings.MainFormWidth || 
                this.Height != _topManager.Settings.MainFormHight ||
                this.Top != _topManager.Settings.MainFormY ||
                this.Left != _topManager.Settings.MainFormX)
            {
                _topManager.Settings.MainFormWidth = this.Width;
                _topManager.Settings.MainFormHight = this.Height;
                _topManager.Settings.MainFormX = this.Left;
                _topManager.Settings.MainFormY = this.Top;
                _topManager.Settings.HasChanged = true;
            }
            if(_topManager.Settings.HasChanged) SaveSettings();
            ClearTemp();
            if (_topManager.ConfigHasChenged)
            {
                DialogResult dr = MessageBox.Show(this, "Config data has changed.\nDo you want to save data?", "MyEPG"
                    , MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (dr)
                {
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        return;
                    case DialogResult.Yes:
                        _topManager.SaveSources();
                        break;
                }
            }

            if (_topManager.EPGData.HasChanged || _topManager.EPGUserData.HasChanged)
            {
                DialogResult dr = MessageBox.Show(this, "EPG data has changed.\nDo you want to save data?", "MyEPG"
                    , MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (dr)
                {
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        return;
                    case DialogResult.Yes:
                        SaveChangedData();
                        break;
                }
            }

        }

        private void cancelAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _topManager.CancelAllTask();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HideTaskMonitorX();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        void RefreshFontSizeCombo()
        {
            float sz = this.Font.Size;
            toolStripComboBox1.Enabled = false;
            toolStripComboBox1.SelectedIndex = (int)(sz) - 7;
            toolStripComboBox1.Enabled = true;
        }

        void RefreshTimeZoneCombo()
        {
            int k = CProgrammData.MyTimePlus.Hours + 12;
            toolStripComboBox2.Enabled = false;
            toolStripComboBox2.SelectedIndex = k;
            toolStripComboBox2.Enabled = true;
        }

        void RefreshColorThemeCombo()
        {
            string themeid = _topManager.Settings.ColorThemeId;
            int k = 0;
            for(int i = 0; i < micbColorTheme.Items.Count; i++)
            {
                if((string)micbColorTheme.Items[i] == themeid)
                {
                    k = i;
                    break;
                }
            }
            micbColorTheme.Enabled = false;
            micbColorTheme.SelectedIndex = k;
            micbColorTheme.Enabled = true;
        }

        private void settingsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            RefreshColorThemeCombo();
            RefreshFontSizeCombo();
            RefreshTimeZoneCombo();
        }

        private void SetColorTheme(string themeid, bool force = false)
        {
            if (_topManager.Settings.ColorThemeId == themeid && !force) return;
            _topManager.Settings.ColorThemeId = themeid;
            ColorThemeHelper.ApplyToForm(this, _topManager.Settings.ColorTheme);
            CheckMenuColorTheme();
            ApplyColorThemeA(micbColorTheme);
        }

        private void SetFontSize(float sz)
        {
            if (this.Font.Size == sz) return;
            this.Font = new Font(this.Font.Name, sz, this.Font.Style);

            foreach (Control c in this.Controls)
            {
                if (c is ToolStrip || c is MenuStrip)
                {
                    c.Font = this.Font;
                    foreach (var ti in (c as ToolStrip).Items)
                    {
                        if (ti is ToolStripComboBox)
                            (ti as ToolStripComboBox).Font = this.Font;
                    }
                }
                else
                {
                    if (!c.Font.Equals(this.Font))
                    {
                        c.Font = new Font(
                            this.Font.FontFamily,
                            this.Font.SizeInPoints,
                            c.Font.Style);
                    }
                }
            }

            _topManager.Settings.FontSize = sz;
            _topManager.Settings.HasChanged = true;
        }

        private void SetTimeZone(int tz)
        {
            CProgrammData.MyTimePlus = new TimeSpan(tz,0,0);
            if (_topManager.Settings.TimePlusHours == tz) return;
            _topManager.Settings.TimePlusHours = tz;
            _topManager.Settings.HasChanged = true;
        }

        public void SetupMenuRenderer()
        {
            if (this.MainMenuStrip != null)
            {
                MainMenuStrip.ForeColor = Color.White;
                var colortheme = _topManager.Settings.ColorTheme;
                MainMenuStrip.Renderer = new MyToolStripRenderer(colortheme);
            }
        }

        public void CheckMenuColorTheme()
        {
            if (this.MainMenuStrip != null)
            {
                var rend = MainMenuStrip.Renderer as MyToolStripRenderer;
                if (rend == null)
                {
                    SetupMenuRenderer();
                    rend = MainMenuStrip.Renderer as MyToolStripRenderer;
                    if (rend == null) return;
                }
                var colortheme = _topManager.Settings.ColorTheme;
                rend.SetColorTheme(colortheme);
                MainMenuStrip.Refresh();
            }
        }

        private void micbColorTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            colorThemeToolStripMenuItem.HideDropDown();
            int k = micbColorTheme.SelectedIndex;
            if (k == -1) return;
            string s = micbColorTheme.Items[k] as string;
            SetColorTheme(s);

        }
        private void toolStripComboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            settingsToolStripMenuItem.HideDropDown();
            int k = toolStripComboBox1.SelectedIndex;
            if (k == -1) return;
            string s = toolStripComboBox1.Items[k] as string;
            float sz = 0.0f;
            if (!float.TryParse(s, out sz)) return;
            SetFontSize(sz);
        }

        private void toolStripComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            settingsToolStripMenuItem.HideDropDown();
            int k = toolStripComboBox2.SelectedIndex;
            if (k == -1) return;
            SetTimeZone(k-12);
        }

        private void tagedTitlesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowTagedTitlesView();
        }

        private void checkMissingDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckMisiingData();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RemoveEmtyChannels();
        }

        private void checkMissingDataOnOpenToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (_topManager.Settings.CheckMisingDataOnOpen == checkMissingDataOnOpenToolStripMenuItem.Checked) return;
            _topManager.Settings.CheckMisingDataOnOpen = checkMissingDataOnOpenToolStripMenuItem.Checked;
            _topManager.Settings.HasChanged = true;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (_topManager.Settings.CheckMisingDataOnOpen)
                CheckMisiingData();
        }

        private void loadFromWebSelectedtoolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            loadFromWebSelectedtoolStripMenuItem.DropDownItems.Clear();

            ToolStripMenuItem ti;
            ti = loadFromWebSelectedtoolStripMenuItem.DropDownItems.Add("Do It") as ToolStripMenuItem;
            ti.Click += new System.EventHandler(this.toolStripDoItMenuItem_Click);
            ti.Tag = "Do It";
            loadFromWebSelectedtoolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());

            string selectedSources = _topManager.Settings.SelectedSources;
            string[] sources = selectedSources.Split(";".ToCharArray());
            foreach (CSource sr in _topManager.Sources)
            {
                ti = loadFromWebSelectedtoolStripMenuItem.DropDownItems.Add(sr.Name) as ToolStripMenuItem;
                ti.CheckOnClick = true;
                ti.Checked = sources.Contains(sr.Name);
            }
            ApplyColorThemeA(loadFromWebSelectedtoolStripMenuItem);
        }

        private void ApplyColorThemeA(object c0)
        {
            var theme = _topManager.Settings.ColorTheme;
            ColorThemeHelper.ApplyToControlA(c0, theme);
        }

        void SaveSelectedSourcesSeting()
        {
            string s = "";
            foreach (var mi in loadFromWebSelectedtoolStripMenuItem.DropDownItems)
            {
                if (mi is ToolStripMenuItem && (mi as ToolStripMenuItem).Checked)
                {
                    if (s != "") s += ";";
                    s += (mi as ToolStripMenuItem).Text;
                }
            }
            if(_topManager.Settings.SelectedSources != s)
            {
                _topManager.Settings.SelectedSources = s;
                _topManager.Settings.HasChanged = true;
            }
        }

        private void toolStripDoItMenuItem_Click(object sender, EventArgs e)
        {
            SaveSelectedSourcesSeting();
            dataSourcesToolStripMenuItem.HideDropDown();
            ClearView();
            ShowTaskMonitorX();
            TopManager.St.StartUpdateSelectedSourceFromWebTask(OnUpdateAllEvent);
            RefreshView();
        }

        void loadFromWebSelectedtoolStripMenuItem_DropDown_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
            {
                e.Cancel = true;
            }
        }

        private void loadFromWebSelectedtoolStripMenuItem_DropDownClosed(object sender, EventArgs e)
        {
            SaveSelectedSourcesSeting();
        }


    }
}
