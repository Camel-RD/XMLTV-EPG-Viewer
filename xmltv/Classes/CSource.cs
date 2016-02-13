using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SerializableDictionary;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace xmltv
{
    public class CSource
    {
        public ESourceState State { get; private set; }
        public ESimpleEvent TaskState { get; private set; }
        public ESimpleEvent SubTaskState { get; private set; }

        public string Name = "";
        public string URL = "";
        public string Prefix = "";
        public string Group = "";

        public List<string> KeepChannelsIds = new List<string>();
        public Dictionary<string, string> RenameChannelsIds = new Dictionary<string, string>();

        private Dictionary<string, CChannelData> KeepChannelsById = new Dictionary<string, CChannelData>();


        public List<CChannelData> Channels = new List<CChannelData>();
        public Dictionary<string, CChannelData> ChannelsById = new Dictionary<string, CChannelData>();

        public List<CChannelData> AllChannels = new List<CChannelData>();
        public Dictionary<string, CChannelData> AllChannelsById = new Dictionary<string, CChannelData>();


        private CDownload Downloader = null;
        private GZipExtract GzipExtract = null;

        //private DSourceEventListener SourceEventListener = null;
        public event EventHandler<CSourceEventArgs> SourceEventListener;

        private List<Action> ScheduledTasks = new List<Action>();
        private DTaskEndEventListener TaskEventListener;

        public bool ConfigOk = false;
        public int Progress { get; private set; }

        private bool CancelToken = false;

        public CSource()
        {
            State = ESourceState.None;
            TaskState = ESimpleEvent.none;
            SubTaskState = ESimpleEvent.none;
        }

        void LogError(string msg)
        {
            TopManager.St.LogManager.Add(ELogEntryType.Error, "source: " + Name, msg);
        }

        private void DoError(string s)
        {
            LogError(s);
            throw new MyException(s);
        }

        public void SetConfigData(CSourceConfigData configdata)
        {
            Name = configdata.Name;
            URL = configdata.URL;
            Prefix = configdata.Prefix;
            KeepChannelsIds.Clear();
            KeepChannelsIds.AddRange(configdata.KeepChannelsIds);
            RenameChannelsIds.Clear();
            foreach (var kv in configdata.RenameChannelsIds)
                RenameChannelsIds.Add(kv.Key, kv.Value);
            FillKeepChannelsById();
            CheckConfigData();
        }

        public CSourceConfigData GetConfigData()
        {
            CSourceConfigData configdata = new CSourceConfigData();
            configdata.Name = Name;
            configdata.URL = URL;
            configdata.Prefix = Prefix;
            configdata.KeepChannelsIds.AddRange(KeepChannelsIds);
            foreach (var kv in RenameChannelsIds)
                configdata.RenameChannelsIds.Add(kv.Key, kv.Value);
            return configdata;
        }

        public string CheckConfigData()
        {
            ConfigOk = false;
            if (Name == "")
            {
                return "Empty source name";
            }
            if (URL == "")
            {
                return "Empty URL";
            }
            ConfigOk = true;
            return "OK";
        }

        void FillKeepChannelsById()
        {
            int i;
            KeepChannelsById.Clear();
            for (i = 0; i < KeepChannelsIds.Count; i++)
            {
                KeepChannelsById[KeepChannelsIds[i]] = null;
            }
        }

        public int GetSourceNr()
        {
            return TopManager.St.GetSourceNr(this);
        }

        public bool Rename(string name)
        {
            lock (this)
            {
                if (name == Name) return true;
                return TopManager.St.Rename(this, name);
            }
        }

        public void ClearState()
        {
            lock (this)
            {
                if (TaskState == ESimpleEvent.started)
                {
                    DoError("busy");
                }
                ScheduledTasks.Clear();
                CancelToken = false;
                State = ESourceState.None;
                TaskState = ESimpleEvent.none;
                SubTaskState = ESimpleEvent.none;
                Downloader = null;
                GzipExtract = null;
                Progress = 0;
            }
        }

        public void ClearAllChannels()
        {
            AllChannels.Clear();
            AllChannelsById.Clear();
        }

        public string GetStateString()
        {
            if (TaskState == ESimpleEvent.canceled)
                return "Canceled";

            switch (State)
            {
                case ESourceState.None:
                    return "";
                case ESourceState.BadConfig:
                    return "bad config";
                case ESourceState.DownloadInit:
                    return "download scheduled";
                case ESourceState.DownloadStarted:
                    return "downloading:";
                case ESourceState.Downloading:
                    return "downloading: " + Progress + "%";
                case ESourceState.DownloadFailed:
                    return "download failed";
                case ESourceState.DownloadFinished:
                    return "download finished";
                case ESourceState.GzExtractStarted:
                    return "extracting";
                case ESourceState.GzFailed:
                    return "failed .gz extract";
                case ESourceState.GzFinished:
                    return "extracted .gz";
                case ESourceState.ReadXMLStarted:
                    return "read XML started";
                case ESourceState.ReadXMLFailed:
                    return "read XML failed";
                case ESourceState.ReadXMLFinished:
                    return "read XML finished";
                case ESourceState.AllDone:
                    return "OK";
                case ESourceState.Canceled:
                    return "canceled";
            }
            return "";
        }

        public void Cancel()
        {
            if (TaskState != ESimpleEvent.started) return;
            if (State == ESourceState.DownloadStarted
                || State == ESourceState.Downloading)
            {
                if (Downloader == null) return;
                try
                {
                    Downloader.Cancel();
                }
                catch (Exception e)
                {

                }
                return;
            }
            CancelToken = true;

        }

        public bool WillStartNewTask()
        {
            if (TaskState == ESimpleEvent.started)
            {
                DoError("Work in progress");
                return false;
            }
            ClearState();
            return true;
        }

        public void DownloadFromWeb(DTaskEndEventListener taskEndListener)
        {
            lock (this)
            {
                if (!WillStartNewTask()) return;
                TaskEventListener = taskEndListener;
                ScheduledTasks.Clear();
                ScheduledTasks.Add(StartDownload);
                StartScheduledTasks();
            }
        }

        public void ReadDataFromWeb(DTaskEndEventListener taskEndListener)
        {
            lock (this)
            {
                if (!WillStartNewTask()) return;
                TaskEventListener = taskEndListener;
                ScheduledTasks.Clear();
                ScheduledTasks.Add(StartDownload);
                ScheduledTasks.Add(StartExtract);
                ScheduledTasks.Add(ReadData);
                StartScheduledTasks();
            }
        }

        public void ReadDataFromGz(DTaskEndEventListener taskEndListener)
        {
            lock (this)
            {
                if (!WillStartNewTask()) return;
                TaskEventListener = taskEndListener;
                ScheduledTasks.Clear();
                ScheduledTasks.Add(StartExtract);
                ScheduledTasks.Add(ReadData);
                StartScheduledTasks();
            }
        }

        public void ReadAllChannels(DTaskEndEventListener taskEndListener)
        {
            lock (this)
            {
                if (!WillStartNewTask()) return;
                TaskEventListener = taskEndListener;
                ScheduledTasks.Clear();
                string GzFileName = Utils.GetFileNameFromURL(URL);
                string GzFullFileName = TopManager.DownloadFolder + "\\" + GzFileName;
                if (!File.Exists(GzFullFileName) || Utils.GetFileSize(GzFullFileName) == 0)
                {
                    ScheduledTasks.Add(StartDownload);
                }
                ScheduledTasks.Add(StartExtract);
                ScheduledTasks.Add(ReadAllChannels);
                StartScheduledTasks();
            }
        }

        void StartScheduledTasks()
        {
            if (ScheduledTasks.Count == 0)
            {
                DoError("state error");
                return;
            }
            State = ESourceState.None;
            if (CancelToken)
            {
                TaskState = ESimpleEvent.canceled;
                OnStateChanged(ESourceState.Canceled);
                return;
            }
            TaskState = ESimpleEvent.started;
            OnTaskEvent(ESimpleEvent.started);
            StartNextScheduledTasks();
        }

        void StartNextScheduledTasks()
        {
            lock (this)
            {
                if (ScheduledTasks.Count == 0 || SubTaskState == ESimpleEvent.started
                    || TaskState != ESimpleEvent.started)
                {
                    DoError("state error");
                    return;
                }
                if (CancelToken)
                {
                    ClearState();
                    TaskState = ESimpleEvent.canceled;
                    OnTaskEvent(ESimpleEvent.canceled);
                    return;
                }
                SubTaskState = ESimpleEvent.started;
                ScheduledTasks[0]();
            }
        }

        void ReadAllChannels()
        {
            lock (this)
            {
                OnStateChanged(ESourceState.ReadXMLStarted);
                OnXMLReadEvent(ESimpleEvent.started);
                try
                {
                    ReadAllChannelsA();
                }
                catch (Exception e)
                {
                    if (!(e is MyException)) LogError(e.Message);
                    OnStateChanged(ESourceState.ReadXMLFailed);
                    OnXMLReadEvent(ESimpleEvent.failed);
                    return;
                }
                OnStateChanged(ESourceState.ReadXMLFinished);
                OnXMLReadEvent(ESimpleEvent.finished);
            }

        }

        void ReadData()
        {
            lock (this)
            {
                OnStateChanged(ESourceState.ReadXMLStarted);
                OnXMLReadEvent(ESimpleEvent.started);
                try
                {
                    ReadDataA();
                    SendEpgData();
                }
                catch (Exception e)
                {
                    if (!(e is MyException)) LogError(e.Message);
                    OnStateChanged(ESourceState.ReadXMLFailed);
                    OnXMLReadEvent(ESimpleEvent.failed);
                    return;
                }
                OnStateChanged(ESourceState.ReadXMLFinished);
                OnXMLReadEvent(ESimpleEvent.finished);

            }

        }

        void StartDownload()
        {
            lock (this)
            {
                OnStateChanged(ESourceState.DownloadInit);
                Progress = 0;
                try
                {
                    StartDownloadA();
                }
                catch (Exception e)
                {
                    if (!(e is MyException)) LogError(e.Message);
                    OnStateChanged(ESourceState.DownloadFailed);
                    OnDownloadEvent(Downloader, ESimpleEvent.failed, "");
                }
            }
        }


        void StartExtract()
        {
            lock (this)
            {
                OnStateChanged(ESourceState.GzExtractStarted);
                try
                {
                    StartExtractA();
                }
                catch (Exception e)
                {
                    if (!(e is MyException)) LogError(e.Message);
                    OnStateChanged(ESourceState.GzFailed);
                    OnGZipExtractEvent(GzipExtract, ESimpleEvent.failed);
                }
            }
        }

        bool NeedGoodGonfigData()
        {
            if (!ConfigOk)
            {
                DoError("Bad source config data");
                return false;
            }
            return true;
        }

        void ReadAllChannelsA()
        {
            NeedGoodGonfigData();

            string GzFileName = "";
            string XMLFileName = "";
            string XMLFullFileName = "";
            string url = URL;
            GzFileName = Utils.GetFileNameFromURL(url);
            XMLFileName = Utils.RemoveExt(GzFileName) + ".xml";
            XMLFullFileName = TopManager.TempFolder + "\\" + XMLFileName;
            var st = new XmlReaderSettings();
            st.DtdProcessing = DtdProcessing.Ignore;
            using (XmlReader xmlReader = XmlReader.Create(XMLFullFileName, st))
            {
                ReadAllChannels(xmlReader);
                xmlReader.Close();
            }
            AllChannels.Sort(
                (ch1, ch2) =>
                {
                    return ch1.DisplayNameA.CompareTo(ch2.DisplayNameA);
                });
        }

        void ReadDataA()
        {
            NeedGoodGonfigData();

            if (KeepChannelsIds.Count == 0)
            {
                return;
            }
            string GzFileName = "";
            string XMLFileName = "";
            string XMLFullFileName = "";
            string url = URL;
            GzFileName = Utils.GetFileNameFromURL(url);
            XMLFileName = Utils.RemoveExt(GzFileName) + ".xml";
            XMLFullFileName = TopManager.TempFolder + "\\" + XMLFileName;
            FillKeepChannelsById();
            var st = new XmlReaderSettings();
            st.DtdProcessing = DtdProcessing.Ignore;
            using (XmlReader xmlReader = XmlReader.Create(XMLFullFileName, st))
            {
                ReadData(xmlReader);
                xmlReader.Close();
            }

        }


        private void ReadAllChannels(XmlReader xmlReader)
        {
            string s;

            AllChannels.Clear();
            AllChannelsById.Clear();

            if (xmlReader.ReadState == ReadState.Initial) xmlReader.Read();

            if (!xmlReader.ReadToNextSibling("tv"))
            {
                DoError("XML no tv node");
                return;
            }
            if (!xmlReader.ReadToDescendant("channel"))
            {
                DoError("XML no channels");
                return;
            }
            if (xmlReader.NodeType != XmlNodeType.Element)
            {
                DoError("XML no Element node");
                return;
            }

            s = "";
            CChannelData ch = null;

            do
            {
                ch = new CChannelData();
                if (!ch.ReadXML(xmlReader)) break;
                AllChannels.Add(ch);
                AllChannelsById[ch.Id] = ch;
                while (xmlReader.NodeType != XmlNodeType.Element)
                {
                    if (!xmlReader.Read()) break;
                }
            } while (true);

        }

        private void ReadData(XmlReader xmlReader)
        {
            string s;

            Channels.Clear();
            ChannelsById.Clear();

            if (xmlReader.ReadState == ReadState.Initial) xmlReader.Read();

            if (!xmlReader.ReadToNextSibling("tv"))
            {
                DoError("No tv node");
                return;
            }
            if (!xmlReader.ReadToDescendant("channel"))
            {
                DoError("XML no channels");
                return;
            }
            if (xmlReader.NodeType != XmlNodeType.Element)
            {
                DoError("XML no Element node");
                return;
            }

            s = "";
            CChannelData ch = null;

            do
            {
                ch = new CChannelData();
                if (!ch.ReadXML(xmlReader)) break;
                if (KeepChannelsById.ContainsKey(ch.Id))
                {
                    ch.Prefix = Prefix;
                    if (!RenameChannelsIds.TryGetValue(ch.Id, out ch.DisplayName2))
                    {
                        ch.DisplayName2 = "";
                    }
                    Channels.Add(ch);
                    ChannelsById[ch.Id] = ch;
                }
                while (xmlReader.NodeType != XmlNodeType.Element)
                {
                    if (!xmlReader.Read()) break;
                }
            } while (true);


            while (xmlReader.NodeType != XmlNodeType.Element && xmlReader.Read()) { }

            CProgrammData pd;
            do
            {
                pd = new CProgrammData();
                if (!pd.ReadXML(xmlReader)) break;
                if (KeepChannelsById.ContainsKey(pd.ChId))
                {
                    if (ChannelsById.TryGetValue(pd.ChId, out ch))
                    {
                        ch.AddProgramm(pd);
                    }
                }
                while (xmlReader.NodeType != XmlNodeType.Element && xmlReader.Read()) { }
            } while (true);
        }

        public void SendEpgData()
        {
            int i;
            CChannelData cd, cd1;
            for (i = 0; i < Channels.Count; i++)
            {
                cd = Channels[i];
                if (TopManager.St.EPGData.ChannelDataById.TryGetValue(cd.Id, out cd1))
                {
                    cd1.AddProgrammsFrom(cd);
                }
                else
                {
                    TopManager.St.EPGData.ChannelData.Add(cd);
                    TopManager.St.EPGData.ChannelDataById[cd.Id] = cd;
                }
            }
        }

        private void StartDownloadA()
        {
            NeedGoodGonfigData();

            string url = URL;
            string GzFileName = "";
            string GzFullFileName = "";
            GzFileName = Utils.GetFileNameFromURL(url);
            if (GzFileName == url)
            {
                OnDownloadEvent(null, ESimpleEvent.finished, "");
                return;
            }
            GzFullFileName = TopManager.DownloadFolder + "\\" + GzFileName;
            if (File.Exists(GzFullFileName))
            {
                File.Delete(GzFullFileName);
            }
            Downloader = TopManager.St.DownloaderManager.AddDownload(OnDownloadEvent, url, GzFullFileName, OnDownloadProgress);
            if (Downloader == null)
            {
                DoError("Allready downloading the file");
                return;
            }
            return;
        }

        private void StartExtractA()
        {
            NeedGoodGonfigData();

            string url = URL;
            string GzFileName = "";
            string GzFullFileName = "";
            string XMLFileName = "";
            string XMLFullFileName = "";
            GzFileName = Utils.GetFileNameFromURL(url);
            GzFullFileName = TopManager.DownloadFolder + "\\" + GzFileName;
            XMLFileName = Utils.RemoveExt(GzFileName) + ".xml";
            XMLFullFileName = TopManager.TempFolder + "\\" + XMLFileName;
            if (!File.Exists(GzFullFileName))
            {
                DoError("File [" + GzFileName + "] not found");
                return;
            }
            if (Utils.GetExt(GzFileName).ToLower() == "xml")
            {
                File.Copy(GzFullFileName, XMLFullFileName, true);
                OnGZipExtractEvent(null, ESimpleEvent.finished);
                return;
            }
            GzipExtract = new GZipExtract(GzFullFileName, XMLFullFileName, OnGZipExtractEvent);
        }

        private void OnStateChanged(ESourceState state)
        {
            State = state;
            OnStateChanged();
        }

        private void OnStateChanged()
        {
            CSourceEventArgs e = new CSourceEventArgs();
            e.state = State;
            EventHandler<CSourceEventArgs> handler = SourceEventListener;
            if (handler != null)
            {
                handler(this, e);
            }
            //if (SourceEventListener != null) SourceEventListener(this, State);
        }

        private void OnTaskEvent(ESimpleEvent endEvent)
        {
            TaskState = endEvent;
            if (endEvent == ESimpleEvent.canceled)
                OnStateChanged(ESourceState.Canceled);
            if (endEvent == ESimpleEvent.finished)
                OnStateChanged(ESourceState.AllDone);
            if (TaskEventListener != null)
            {
                TaskEventListener(this, endEvent);
            }
        }

        private void OnSubTaskEvent(ESimpleEvent endEvent)
        {
            lock (this)
            {
                SubTaskState = endEvent;
                switch (endEvent)
                {
                    case ESimpleEvent.failed:
                        if (ScheduledTasks.Count == 0)
                        {
                            DoError("Task state error");
                            return;
                        }
                        ScheduledTasks.Clear();
                        OnTaskEvent(ESimpleEvent.failed);

                        break;

                    case ESimpleEvent.canceled:
                        if (ScheduledTasks.Count == 0)
                        {
                            DoError("Task state error");
                            return;
                        }
                        ScheduledTasks.Clear();
                        OnTaskEvent(ESimpleEvent.canceled);

                        break;

                    case ESimpleEvent.finished:
                        if (ScheduledTasks.Count == 0)
                        {
                            DoError("Task state error");
                            return;
                        }
                        ScheduledTasks.RemoveAt(0);
                        if (ScheduledTasks.Count == 0)
                        {
                            OnTaskEvent(ESimpleEvent.finished);
                        }
                        else
                        {
                            StartNextScheduledTasks();
                        }
                        break;
                }
            }
        }

        void OnDownloadProgress(CDownload downloader, int progress)
        {
            if (Downloader != downloader) return;
            Progress = progress;
            OnStateChanged(ESourceState.Downloading);
        }

        void OnDownloadEvent(CDownload downloader, ESimpleEvent downloadEvent, string msg)
        {
            lock (this)
            {
                switch (downloadEvent)
                {
                    case ESimpleEvent.started:
                        OnStateChanged(ESourceState.DownloadStarted);
                        break;
                    case ESimpleEvent.canceled:
                        LogError("download canceled");
                        OnStateChanged(ESourceState.DownloadFailed);
                        break;
                    case ESimpleEvent.failed:
                        OnStateChanged(ESourceState.DownloadFailed);
                        break;

                    case ESimpleEvent.finished:
                        OnStateChanged(ESourceState.DownloadFinished);
                        break;
                }
            }
            OnSubTaskEvent(downloadEvent);
        }

        void OnGZipExtractEvent(GZipExtract gzipExtract, ESimpleEvent gzipExtractEvent)
        {
            lock (this)
            {
                switch (gzipExtractEvent)
                {
                    case ESimpleEvent.started:
                        OnStateChanged(ESourceState.GzExtractStarted);
                        break;
                    case ESimpleEvent.failed:
                    case ESimpleEvent.canceled:
                        OnStateChanged(ESourceState.GzFailed);
                        break;

                    case ESimpleEvent.finished:
                        OnStateChanged(ESourceState.GzFinished);
                        break;
                }
            }
            OnSubTaskEvent(gzipExtractEvent);
        }

        void OnXMLReadEvent(ESimpleEvent xmlReadEvent)
        {
            lock (this)
            {
                switch (xmlReadEvent)
                {
                    case ESimpleEvent.started:
                        OnStateChanged(ESourceState.ReadXMLStarted);
                        break;
                    case ESimpleEvent.failed:
                    case ESimpleEvent.canceled:
                        OnStateChanged(ESourceState.ReadXMLFailed);
                        break;

                    case ESimpleEvent.finished:
                        OnStateChanged(ESourceState.ReadXMLFinished);
                        break;
                }
            }
            OnSubTaskEvent(xmlReadEvent);
        }

    }

}
