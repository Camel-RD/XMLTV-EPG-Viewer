using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using SerializableDictionary;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace xmltv
{

    public enum ESimpleEvent {none, started, canceled, failed, finished }
    public delegate void DSimpleEventListener(ESimpleEvent taskevent);

    public class TopManager
    {
        public static string DataFolder = Utils.GetMyFolder() + "\\data";
        public static string ConfigSourcesFileName = DataFolder + "\\Sources.config.xml";
        public static string EPGDataFileName = DataFolder + "\\TvGuide.xml";
        public static string EPGUserDataFileName = DataFolder + "\\TvGuide.userdata.xml";
        public static string TempFolder = Utils.GetMyFolder() + "\\temp";
        public static string DownloadFolder = Utils.GetMyFolder() + "\\in";
        public static string SettingsFileName = Utils.GetMyFolder() + "\\Settings.xml";

        public static TopManager St { get; private set; }

        public List<CSource> Sources = new List<CSource>();
        public Dictionary<string, CSource> SourcesByName = new Dictionary<string, CSource>();
        public CEPGData EPGData = new CEPGData();
        public CEPGUserData EPGUserData = new CEPGUserData();
        public CDownloaderManager DownloaderManager = new CDownloaderManager();
        public List<DateTime> UsedDates = new List<DateTime>();
        public CLogManager LogManager = new CLogManager();
        public CSettings Settings = new CSettings();

        private DSimpleEventListener TaskEventListener = null;
        private ESimpleEvent TaskState = ESimpleEvent.none;

        public event EventHandler<CSourceEventArgs> SourceEventListener;


        public SerializableDictionary<string, List<string>> ChannelsByGroup { get { return EPGUserData.ChannelsByGroup; } }
        public List<CScheduledntry> ScheduledProgramms { get { return EPGUserData.ScheduledProgramms; } }

        public bool ConfigHasChenged = false;

        public bool FilterWithTags = false;

        public TopManager()
        {
            if (St != null)
            {
                DoError("TopManager allready initialized");
                return;
            }
            St = this;
        }

        void LogError(string msg)
        {
            LogManager.Add(ELogEntryType.Error, "app", msg);
        }

        private void DoError(string s)
        {
            throw new MyException(s);
        }

        public void StartUpdateAllTask(DSimpleEventListener taskeventlistener)
        {
            lock (this)
            {
                if (TaskState == ESimpleEvent.started)
                {
                    DoError("Busy");
                    return;
                }
                ClearEPG();
                TaskEventListener = taskeventlistener;
                TaskState = ESimpleEvent.started;
                OnTaskEvent(ESimpleEvent.started);

                foreach (CSource sr in Sources)
                    sr.ReadDataFromWeb(OnUpdateAllEvent);
            }
        }

        public void StartUpdateAllFromGzTask(DSimpleEventListener taskeventlistener)
        {
            lock (this)
            {
                if (TaskState == ESimpleEvent.started)
                {
                    DoError("Busy");
                    return;
                }
                ClearEPG();
                TaskEventListener = taskeventlistener;
                TaskState = ESimpleEvent.started;
                OnTaskEvent(ESimpleEvent.started);

                foreach (CSource sr in Sources)
                    sr.ReadDataFromGz(OnUpdateAllEvent);
            }
        }

        public void StartUpdateSourceFromGzTask(DSimpleEventListener taskeventlistener, string sourcename)
        {
            lock (this)
            {
                if (TaskState == ESimpleEvent.started)
                {
                    DoError("Busy");
                    return;
                }
                if (!SourcesByName.ContainsKey(sourcename))
                {
                    DoError("Wrong source name");
                    return;
                }
                TaskEventListener = taskeventlistener;
                TaskState = ESimpleEvent.started;
                OnTaskEvent(ESimpleEvent.started);

                foreach (CSource sr in Sources)
                {
                    sr.ClearState();
                }
                SourcesByName[sourcename].ReadDataFromGz(OnUpdateSourceEvent);
            }
        }

        public void StartUpdateSelectedSourceFromWebTask(DSimpleEventListener taskeventlistener)
        {
            lock (this)
            {
                if (TaskState == ESimpleEvent.started)
                {
                    DoError("Busy");
                    return;
                }
                
                string[] sources = Settings.SelectedSources.Split(";".ToCharArray());
                if (sources == null || sources.Length == 0) return;

                int srct = 0;

                foreach (var s in sources)
                {
                    if (SourcesByName.ContainsKey(s))
                    {
                        srct++;
                    }
                    
                }
                if (srct == 0) return;

                TaskEventListener = taskeventlistener;
                TaskState = ESimpleEvent.started;
                OnTaskEvent(ESimpleEvent.started);

                foreach (CSource sr in Sources)
                {
                    sr.ClearState();
                }
                foreach (var s in sources)
                {
                    if (SourcesByName.ContainsKey(s))
                    {
                        SourcesByName[s].ReadDataFromWeb(OnUpdateAllEvent);
                    }
                }
            }
        }

        public void StartUpdateSourceFromWebTask(DSimpleEventListener taskeventlistener, string sourcename)
        {
            lock (this)
            {
                if (TaskState == ESimpleEvent.started)
                {
                    DoError("Busy");
                    return;
                }
                if (!SourcesByName.ContainsKey(sourcename))
                {
                    DoError("Wrong source name");
                    return;
                }
                TaskEventListener = taskeventlistener;
                TaskState = ESimpleEvent.started;
                OnTaskEvent(ESimpleEvent.started);

                foreach (CSource sr in Sources)
                {
                    sr.ClearState();
                }
                SourcesByName[sourcename].ReadDataFromWeb(OnUpdateSourceEvent);
            }
        }


        public void StartGetAllChannels(DSimpleEventListener taskeventlistener, string sourcename)
        {
            lock (this)
            {
                if (TaskState == ESimpleEvent.started)
                {
                    DoError("Busy");
                    return;
                }
                if (!SourcesByName.ContainsKey(sourcename))
                {
                    DoError("Wrong source name");
                    return;
                }
                TaskEventListener = taskeventlistener;
                TaskState = ESimpleEvent.started;
                OnTaskEvent(ESimpleEvent.started);

                foreach (CSource sr in Sources)
                {
                    sr.ClearState();
                }
                SourcesByName[sourcename].ReadAllChannels(OnUpdateSourceEvent);
            }
        }

        public void CancelTask(string sourcename)
        {
            if (TaskState != ESimpleEvent.started) return;
            if (!SourcesByName.ContainsKey(sourcename)) return;
            SourcesByName[sourcename].Cancel();
        }

        public void CancelAllTask()
        {
            if (TaskState != ESimpleEvent.started) return;
            foreach (CSource sr in Sources)
            {
                sr.Cancel();
            }
        }

        ESimpleEvent CheckScheduledTasks()
        {
            ESimpleEvent ret = ESimpleEvent.finished;
            foreach (CSource sr in Sources)
            {
                switch (sr.TaskState)
                {
                    case ESimpleEvent.started:
                        return ESimpleEvent.started;
                        break;
                    case ESimpleEvent.failed:
                    case ESimpleEvent.canceled:
                        ret = ESimpleEvent.failed;
                        break;
                }
            }
            return ret;
        }

        void OnTaskEvent(ESimpleEvent taskevent)
        {
            if (TaskEventListener != null)
                TaskEventListener(taskevent);
        }

        void AfterUpdateAll(bool succeeded)
        {
            if (!succeeded)
            {
                ClearEPG();
                OnTaskEvent(ESimpleEvent.failed);
                return;
            }

            EPGData.Sort();
            EPGData.SplitByDate();
            GetUsedDates();
            
            EPGData.HasChanged = true;
            
            EPGUserData.DoAutoSchedule();

            OnTaskEvent(ESimpleEvent.finished);
        }

        void OnUpdateAllEvent(CSource sender, ESimpleEvent taskevent)
        {
            lock (this)
            {
                TaskState = CheckScheduledTasks();
                if (TaskState == ESimpleEvent.finished)
                {
                    AfterUpdateAll(true);
                }
                if (TaskState == ESimpleEvent.failed)
                {
                    AfterUpdateAll(false);
                }
            }
        }

        void OnUpdateSourceEvent(CSource sender, ESimpleEvent taskevent)
        {
            OnUpdateAllEvent(sender, taskevent);
        }

        public void ClearSources()
        {
            Sources.Clear();
            SourcesByName.Clear();
        }

        public int GetSourceNr(CSource source)
        {
            lock (this)
            {
                return Sources.FindIndex(sr => source == sr);
            }
        }

        public void ClearEPG()
        {
            EPGData.ChannelData.Clear();
            EPGData.ChannelDataById.Clear();
            UsedDates.Clear();
            
        }

        public void AddSource(CSourceConfigData sourceConfigData)
        {
            lock (this)
            {
                CSource source;
                if (SourcesByName.TryGetValue(sourceConfigData.Name, out source))
                {
                    DoError("Duplicate source names");
                    return;
                }
                source = new CSource();
                source.SetConfigData(sourceConfigData);
                Sources.Add(source);
                SourcesByName[sourceConfigData.Name] = source;
                source.SourceEventListener += OnSourceEvent;

                ConfigHasChenged = true;
            }
        }

        public CSource CreateSource(string name)
        {
            lock (this)
            {
                CSource source;
                if (SourcesByName.TryGetValue(name, out source))
                {
                    DoError("Duplicate source names");
                    return null;
                }
                source = new CSource();
                CSourceConfigData sourceConfigData = new CSourceConfigData();
                sourceConfigData.Name = name;
                source.SetConfigData(sourceConfigData);
                Sources.Add(source);
                SourcesByName[sourceConfigData.Name] = source;
                source.SourceEventListener += OnSourceEvent;

                ConfigHasChenged = true;

                return source;
            }
        }

        public bool Rename(CSource source, string name)
        {
            lock (this)
            {
                if (name == source.Name) return true;
                if (SourcesByName.ContainsKey(name)) return false;
                SourcesByName.Remove(source.Name);
                source.Name = name;
                SourcesByName[name] = source;

                ConfigHasChenged = true;
            }
            return true;
        }


        public void DeleteSource(string name)
        {
            lock (this)
            {
                CSource source;
                if (!SourcesByName.TryGetValue(name, out source))
                {
                    DoError("Source not found");
                    return;
                }
                Sources.Remove(source);
                SourcesByName.Remove(name);

                ConfigHasChenged = true;
            }
        }

        public void GetUsedDates()
        {
            foreach (var ch in EPGData.ChannelData)
            {
                foreach (var dt in ch.DatesUsed)
                {
                    if (!UsedDates.Contains(dt))
                    {
                        UsedDates.Add(dt);
                    }
                }
                
            }
            UsedDates.Sort();
        }

        void OnSourceEvent(object sender, CSourceEventArgs e)
        {
            CSourceEventArgs e1 = new CSourceEventArgs();
            e1.state = e.state;
            EventHandler<CSourceEventArgs> handler = SourceEventListener;
            if (handler != null)
            {
                handler(sender, e1);
            }
        }

        public string CheckConfigData()
        {
            int i;
            string s;
            CSource source;
            Dictionary<string, CSource> byName = new Dictionary<string, CSource>();
            for (i = 0; i < Sources.Count; i++)
            {
                source = Sources[i];
                if (byName.ContainsKey(source.Name))
                {
                    return "Duplicate source name:" + source.Name;
                }
                s = source.CheckConfigData();
                if (s != "OK") return s;
                byName[source.Name] = source;
            }
            return "OK";
        }

        CConfigData GetConfigData()
        {
            CConfigData configdata = new CConfigData();
            CSourceConfigData sourceconfigdata;
            foreach (CSource sr in Sources)
            {
                sourceconfigdata = sr.GetConfigData();
                configdata.SourceConfigData.Add(sourceconfigdata);
            }
            return configdata;
        }

        public void LoadSources()
        {
            LoadSources(ConfigSourcesFileName);
        }

        public void LoadSources(string filename)
        {
            if (TaskState == ESimpleEvent.started)
            {
                DoError("Busy");
                return;
            }
            TaskState = ESimpleEvent.started;
            LoadSourcesA(filename);
            TaskState = ESimpleEvent.finished;
        }

        private void LoadSourcesA(string filename)
        {
            ClearSources();
            CConfigData configdata = GetConfigData();
            if (!File.Exists(filename)) return;
            XmlSerializer xs = null;
            FileStream fs = null;
            try
            {
                xs = new XmlSerializer(typeof(CConfigData));
                fs = new FileStream(filename, FileMode.Open);
                configdata = (CConfigData)xs.Deserialize(fs);
            }
            catch (Exception e)
            {
            }
            finally
            {
                if (fs != null) fs.Close();
            }
            int i;
            for (i = 0; i < configdata.SourceConfigData.Count; i++)
            {
                AddSource(configdata.SourceConfigData[i]);
            }
            ConfigHasChenged = false;
        }

        public void SaveSources()
        {
            SaveSources(ConfigSourcesFileName);
        }

        public void SaveSources(string filename)
        {
            if (TaskState == ESimpleEvent.started)
            {
                DoError("Busy");
                return;
            }
            TaskState = ESimpleEvent.started;
            SaveSourcesA(filename);
            TaskState = ESimpleEvent.finished;
        }

        private void SaveSourcesA(string filename)
        {
            CConfigData configdata = GetConfigData();
            XmlSerializer xs = new XmlSerializer(typeof(CConfigData));
            TextWriter wr = null;
            try
            {
                wr = new StreamWriter(filename);
                xs.Serialize(wr, configdata);
            }
            finally
            {
                if (wr != null) wr.Close();
            }
            ConfigHasChenged = false;
        }

        public void LoadEpg()
        {
            LoadEpg(EPGDataFileName);
        }

        public void LoadEpg(string filename)
        {
            if (TaskState == ESimpleEvent.started)
            {
                DoError("Busy");
                return;
            }
            TaskState = ESimpleEvent.started;
            LoadEpgA(filename);
            TaskState = ESimpleEvent.finished;
        }

        private void LoadEpgA(string filename)
        {
            EPGData = new CEPGData();
            if (!File.Exists(filename)) return;
            XmlSerializer xs = null;
            FileStream fs = null;
            try
            {
                xs = new XmlSerializer(typeof(CEPGData));
                fs = new FileStream(filename, FileMode.Open);
                EPGData = (CEPGData)xs.Deserialize(fs);
            }
            catch (Exception e)
            {
                LogError(e.Message);
                EPGData = new CEPGData();
            }
            finally
            {
                if (fs != null) fs.Close();
            }
            EPGData.MakeDictionary();
            EPGData.CheckReferences();
            EPGData.Sort();
            EPGData.SplitByDate();
            GetUsedDates();
            EPGData.HasChanged = false;

        }

        public void SaveEPGData()
        {
            SaveEPGData(EPGDataFileName);
        }

        public void SaveEPGData(string filename)
        {
            if (TaskState == ESimpleEvent.started)
            {
                DoError("Busy");
                return;
            }
            TaskState = ESimpleEvent.started;
            SaveEPGDataA(filename);
            TaskState = ESimpleEvent.finished;
        }

        private void SaveEPGDataA(string filename)
        {
            XmlSerializer xs = new XmlSerializer(typeof(CEPGData));
            TextWriter wr = null;
            try
            {
                wr = new StreamWriter(filename);
                xs.Serialize(wr, EPGData);
                EPGData.HasChanged = false;
            }
            catch (Exception e)
            {
                LogError(e.Message);
            }
            finally
            {
                if (wr != null) wr.Close();
            }
        }

        public void LoadXMLTvFile()
        {
            LoadXMLTvFile(EPGDataFileName);
            EPGData.HasChanged = false;
        }

        public void LoadXMLTvFile(string filename)
        {
            if (TaskState == ESimpleEvent.started)
            {
                DoError("Busy");
                return;
            }
            TaskState = ESimpleEvent.started;
            LoadXMLTvFileA(filename);
            TaskState = ESimpleEvent.finished;
        }

        private void LoadXMLTvFileA(string filename)
        {
            EPGData = new CEPGData();
            if (!File.Exists(filename)) return;
            try
            {
                EPGData.ReadData(filename);
            }
            catch (Exception e)
            {
                LogError(e.Message);
                EPGData = new CEPGData();
            }

            EPGData.MakeDictionary();
            EPGData.CheckReferences();
            EPGData.Sort();
            EPGData.SplitByDate();
            GetUsedDates();
            EPGData.HasChanged = true;
        }

        public void SaveXMLTvFile()
        {
            SaveXMLTvFile(EPGDataFileName);
            EPGData.HasChanged = false;
        }

        public void SaveXMLTvFile(string filename)
        {
            if (TaskState == ESimpleEvent.started)
            {
                DoError("Busy");
                return;
            }
            TaskState = ESimpleEvent.started;
            SaveXMLTvFileA(filename);
            TaskState = ESimpleEvent.finished;
        }

        private void SaveXMLTvFileA(string filename)
        {
            if (File.Exists(filename)) File.Delete(filename);
            EPGData.WriteData(filename);
        }



        public void SaveUserData()
        {
            SaveUserData(EPGUserDataFileName);
            EPGUserData.HasChanged = false;
        }

        public void SaveUserData(string filename)
        {
            if (TaskState == ESimpleEvent.started)
            {
                DoError("Busy");
                return;
            }
            TaskState = ESimpleEvent.started;
            SaveUserDataA(filename);
            TaskState = ESimpleEvent.finished;
        }

        private void SaveUserDataA(string filename)
        {
            XmlSerializer xs = new XmlSerializer(typeof(CEPGUserData));
            TextWriter wr = null;
            try
            {
                wr = new StreamWriter(filename);
                xs.Serialize(wr, EPGUserData);
            }
            catch (Exception e)
            {
                LogError(e.Message);
            }
            finally
            {
                if (wr != null) wr.Close();
            }
        }

        public void LoadUserData()
        {
            LoadUserData(EPGUserDataFileName);
            EPGUserData.HasChanged = false;
        }

        public void LoadUserData(string filename)
        {
            if (TaskState == ESimpleEvent.started)
            {
                DoError("Busy");
                return;
            }
            TaskState = ESimpleEvent.started;
            LoadUserDataA(filename);
            TaskState = ESimpleEvent.finished;
        }

        private void LoadUserDataA(string filename)
        {
            EPGUserData = new CEPGUserData();
            if (!File.Exists(filename)) return;
            XmlSerializer xs = null;
            FileStream fs = null;
            try
            {
                xs = new XmlSerializer(typeof(CEPGUserData));
                fs = new FileStream(filename, FileMode.Open);
                EPGUserData = (CEPGUserData)xs.Deserialize(fs);
            }
            catch (Exception e)
            {
                LogError(e.Message);
                EPGUserData = new CEPGUserData();
            }
            finally
            {
                if (fs != null) fs.Close();
            }
            EPGUserData.HasChanged = true;
        }

        public CXZSource LoadXZSources(string filename)
        {
            CXZSource xzsource;
            if (!File.Exists(filename)) return null;
            XmlSerializer xs = null;
            FileStream fs = null;
            try
            {
                xs = new XmlSerializer(typeof(CXZSource));
                TextReader reader = new StreamReader(filename, Encoding.Default);
                xzsource = (CXZSource)xs.Deserialize(reader);
            }
            catch (Exception e)
            {
                LogError(e.Message);
                return null;
            }
            finally
            {
                if (fs != null) fs.Close();
            }

            return xzsource;
        }

        public void LoadSettings()
        {
            Settings = new CSettings();  
            if (!File.Exists(SettingsFileName)) return;
            XmlSerializer xs = null;
            FileStream fs = null;
            try
            {
                xs = new XmlSerializer(typeof(CSettings));
                fs = new FileStream(SettingsFileName, FileMode.Open);
                Settings = (CSettings)xs.Deserialize(fs);
            }
            catch (Exception e)
            {
                LogError(e.Message);
                Settings = new CSettings();
            }
            finally
            {
                if (fs != null) fs.Close();
            }
        }

        public void SaveSettings()
        {
            XmlSerializer xs = new XmlSerializer(typeof(CSettings));
            TextWriter wr = null;
            try
            {
                wr = new StreamWriter(SettingsFileName);
                xs.Serialize(wr, Settings);
            }
            catch (Exception e)
            {
                LogError(e.Message);
            }
            finally
            {
                if (wr != null) wr.Close();
            }
        }

    }
}
