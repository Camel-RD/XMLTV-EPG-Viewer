using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xmltv
{

    public enum ELogEntryType
    {
        Info,
        Error
    };

    public class CLogEventArgs : EventArgs
    {
        public CLogEntry LogEntry { get; set; }
    }

    public class CLogEntry
    {
        public ELogEntryType LogEntryType = ELogEntryType.Info;
        public string SourceName = "";
        public string Messsage = "";
        public DateTime Time = DateTime.Now;

        public CLogEntry(ELogEntryType logentrytype, string sourcename, string messsage)
        {
            LogEntryType = logentrytype;
            SourceName = sourcename;
            Messsage = messsage;
        }
    }

    public class CLogManager
    {
        private List<CLogEntry> LogEntries = new List<CLogEntry>();

        public event EventHandler<CLogEventArgs> SourceEventListener;

        public int Count
        {
            get
            {
                lock (this)
                {
                    return LogEntries.Count;
                }
            }
        }

        public void Add(ELogEntryType logentrytype, string sourcename, string messsage)
        {
            CLogEntry logentry;
            lock (this)
            {
                logentry = new CLogEntry(logentrytype, sourcename, messsage);
                LogEntries.Add(logentry);
            }
            OnAddToLog(logentry);
        }

        public List<CLogEntry> GetAll()
        {
            lock (this)
            {
                return new List<CLogEntry>(LogEntries);
            }
        }

        public List<CLogEntry> GetInterval(int fromnr, int tonr)
        {
            lock (this)
            {
                if (fromnr < 0 || tonr < 0 || fromnr > tonr 
                    || fromnr >= LogEntries.Count || tonr >= LogEntries.Count)
                    throw new ArgumentOutOfRangeException();
                List<CLogEntry> les = new List<CLogEntry>(tonr-fromnr+1);
                for (int i = fromnr; i < tonr; i++)
                {
                    les.Add(LogEntries[i]);
                }
                return les;
            }
        }

        private void OnAddToLog(CLogEntry logentry)
        {
            CLogEventArgs e = new CLogEventArgs();
            e.LogEntry = logentry;
            EventHandler<CLogEventArgs> handler = SourceEventListener;
            if (handler != null)
            {
                handler(this, e);
            }
        }

    }
}
