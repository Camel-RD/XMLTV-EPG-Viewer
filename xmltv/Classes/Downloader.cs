using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Net;
using System.IO;
using System.Timers;

namespace xmltv
{

    public enum EDownloadStatus
    {
        none, started, finished, failed, canceled, timeout
    }

    public enum EDownloadEndStatus
    {
        finished, failed, canceled
    }

    public delegate void DDownloadEventListener(CDownload download, ESimpleEvent downloadEvent, string msg);
    public delegate void DDownloadProgressListener(CDownload download, int progress);

    public class CDownloaderManager
    {
        public IDownloadListener DownloadListener = null;
        public static long TimerLimit = 5 * 60 * 1000;

        private List<CDownload> RunningDownloads = new List<CDownload>();
        private List<CDownload> AllDownloads = new List<CDownload>();
        private Dictionary<string, CDownload> AllDownloadsByFileName = new Dictionary<string, CDownload>();

        public CDownload GetDownloadByFileName(string filename)
        {
            lock (this)
            {
                CDownload download;
                if (!AllDownloadsByFileName.TryGetValue(filename, out download)) return null;
                return download;
            }
        }

        public CDownload AddDownload(DDownloadEventListener downloadEventListener
            , string url, string targetfilename, DDownloadProgressListener progresseventlistener)
        {
            //url = url.ToLower();
            //targetfilename = targetfilename.ToLower();
            lock (this)
            {
                CDownload download = GetDownloadByFileName(targetfilename);
                if (download != null) return null;
                download = new CDownload(this, downloadEventListener, url
                    , targetfilename, progresseventlistener);
                AllDownloads.Add(download);
                AllDownloadsByFileName[targetfilename] = download;
                if (RunningDownloads.Count == 0)
                {
                    download.StartDownload();
                }
                return download;
            }
        }

        public void ProgressChanged(CDownload sender)
        {
            lock (this)
            {
                if (DownloadListener != null)
                    DownloadListener.ProgressChanged(sender);
            }
        }

        public void DownloadStateChanged(CDownload sender, EDownloadStatus state)
        {
            lock (this)
            {
                if (!AllDownloads.Contains(sender)) return;
                switch (state)
                {
                    case EDownloadStatus.started:
                        if (!RunningDownloads.Contains(sender))
                            RunningDownloads.Add(sender);
                        break;
                    case EDownloadStatus.failed:
                    case EDownloadStatus.canceled:
                    case EDownloadStatus.timeout:
                    case EDownloadStatus.finished:
                        RunningDownloads.Remove(sender);
                        AllDownloads.Remove(sender);
                        AllDownloadsByFileName.Remove(sender.FullFileName);
                        StartNextDownload();
                        break;
                }
                if (DownloadListener != null)
                    DownloadListener.DownloadStateChanged(sender, state);
            }
        }

        void StartNextDownload()
        {
            if (RunningDownloads.Count == 0 && AllDownloads.Count > 0)
            {
                AllDownloads[0].StartDownload();
            }
        }

    }

    public interface IDownloadListener
    {
        void DownloadStateChanged(CDownload sender, EDownloadStatus state);
        void ProgressChanged(CDownload sender);
    }

    
    public class CDownload
    {
        public int Progress {get; private set;}
        public CDownloaderManager DownloadManager { get; private set; }
        public string URL { get; private set; }
        public string FileName { get; private set; }
        public string FullFileName { get; private set; }
        public EDownloadStatus DownloadStatus { get; private set; }
        public bool Downloading { get; private set; }

        private WebClient webClient = null;
        private System.Timers.Timer aTimer = null;

        DDownloadEventListener DownloadEventListener = null;

        private DDownloadProgressListener ProgressEventListener = null;
        private DateTime LasProgressEventTime = DateTime.MinValue;

        public TimeSpan TimeBetweenProgressEvents = new TimeSpan(0,0,0,0,500);

        public CDownload(CDownloaderManager manager, DDownloadEventListener downloadEventListener, string url, string targetfilename, DDownloadProgressListener progresseventlistener)
        {
            Progress = 0;
            DownloadManager = manager;
            DownloadEventListener = downloadEventListener;
            ProgressEventListener = progresseventlistener;
            URL = url;
            FullFileName = targetfilename;
            FileName = Utils.GetFileNameFromURL(FullFileName);
            DownloadStatus = EDownloadStatus.none;
            Downloading = false;
        }

        void LogError(string msg)
        {
            TopManager.St.LogManager.Add(ELogEntryType.Error, "downloader", msg);
        }

        void DoError(string msg)
        {
            throw new MyException(msg);
        }

        public bool CanStart()
        {
            CDownload download;
            lock (this)
                lock (DownloadManager)
                {
                    download = DownloadManager.GetDownloadByFileName(FullFileName);
                    if (download == null) return true;
                    if (download.Downloading) return false;
                    return true;
                }
        }

        public void StartDownload()
        {
            lock (this)
            {
                if (Downloading || !CanStart())
                {
                    //DoError("Download allready started");
                    DownloadStatus = EDownloadStatus.failed;
                    DownloadManager.DownloadStateChanged(this, EDownloadStatus.failed);
                    if (DownloadEventListener != null)
                        DownloadEventListener(this, ESimpleEvent.failed, "Download allready started");
                    return;
                }
                Downloading = true;
                DownloadStatus = EDownloadStatus.started;
                DownloadManager.DownloadStateChanged(this, EDownloadStatus.started);
                if (DownloadEventListener != null)
                    DownloadEventListener(this, ESimpleEvent.started, "");
                try
                {
                    StartDownloadA();
                }
                catch(Exception e)
                {
                    LogError("failed to download from: " + URL);
                    LogError(e.Message);
                    DownloadStatus = EDownloadStatus.failed;
                    DownloadManager.DownloadStateChanged(this, EDownloadStatus.failed);
                    if (DownloadEventListener != null)
                        DownloadEventListener(this, ESimpleEvent.failed, "");
                    Downloading = false;
                    webClient = null;
                }
            }
        }

        private void StartDownloadA()
        {
            webClient = new WebClient();
            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
            aTimer = new System.Timers.Timer(CDownloaderManager.TimerLimit);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = false;
            webClient.Proxy = null;
            if (aTimer.Enabled)
            {
                aTimer.Enabled = false;
            }
            aTimer.Enabled = true;

            try
            {
                if (File.Exists(FullFileName)) File.Delete(FullFileName);
            }
            catch (Exception e)
            {
                throw new MyException("Cant delete file: " + FullFileName);
            }
            webClient.DownloadFileAsync(new Uri(URL), FullFileName);
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            lock (this)
            {
                Progress = e.ProgressPercentage;
                if (LasProgressEventTime + TimeBetweenProgressEvents > DateTime.Now) return;
                LasProgressEventTime = DateTime.Now;
                DownloadManager.ProgressChanged(this);
                if (ProgressEventListener != null)
                {
                    ProgressEventListener(this, Progress);
                }
            }
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            lock (this)
            {
                if (!Downloading) return;
                Downloading = false;
                aTimer.Stop();
                if (e.Cancelled) return;
                if (e.Error != null)
                {
                    LogError("failed to download from: " + URL);
                    LogError(e.Error.Message);
                    DownloadStatus = EDownloadStatus.failed;
                    DownloadManager.DownloadStateChanged(this, EDownloadStatus.failed);
                    if (DownloadEventListener != null)
                        DownloadEventListener(this, ESimpleEvent.failed, "");
                    return;
                }
                if (File.Exists(FullFileName) && Utils.GetFileSize(FullFileName) > 0)
                {
                    DownloadStatus = EDownloadStatus.finished;
                    DownloadManager.DownloadStateChanged(this, EDownloadStatus.finished);
                    if (DownloadEventListener != null)
                        DownloadEventListener(this, ESimpleEvent.finished, "");
                }
                else
                {
                    LogError("failed to download from: " + URL);
                    DownloadStatus = EDownloadStatus.failed;
                    DownloadManager.DownloadStateChanged(this, EDownloadStatus.failed);
                    if (DownloadEventListener != null)
                        DownloadEventListener(this, ESimpleEvent.failed, "");
                }
            }
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            lock (this)
            {
                Stop();
                DownloadStatus = EDownloadStatus.failed;
                DownloadManager.DownloadStateChanged(this, EDownloadStatus.timeout);
                if (DownloadEventListener != null)
                    DownloadEventListener(this, ESimpleEvent.failed, "");
            }
        }

        private void Stop()
        {
            if (!Downloading) return;
            aTimer.Enabled = false;
            Downloading = false;
            try
            {
                webClient.CancelAsync();
                if (File.Exists(FullFileName)) File.Delete(FullFileName);
            }
            catch (Exception)
            {
            }
        }

        public void Cancel()
        {
            lock (this)
            {
                Stop();
                DownloadStatus = EDownloadStatus.canceled;
                DownloadManager.DownloadStateChanged(this, EDownloadStatus.canceled);
                if (DownloadEventListener != null)
                    DownloadEventListener(this, ESimpleEvent.canceled, "");
            }
        }
    }


}
