using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SerializableDictionary;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace xmltv
{
    public class MyException : Exception
    {
        public MyException(string msg) : base(msg)
        {
            
        }
    }

    public enum ESourceState
    {
        None, BadConfig, DownloadInit, DownloadStarted, Downloading, DownloadFailed, DownloadFinished,
        GzExtractStarted, GzFailed, GzFinished, ReadXMLStarted, ReadXMLFailed,
        ReadXMLFinished, AllDone, Canceled
    }

    public delegate void DSourceEventListener(CSource sender, ESourceState state);
    public delegate void DTaskEndEventListener(CSource source, ESimpleEvent endEvent);
    public delegate void DXMLReadEventListener(ESimpleEvent xmlReadEvent);

    public enum EProgramTag
    {
        None,
        Seen,
        Ignore,
        AutoSchedule
    }

    public class CSourceEventArgs : EventArgs
    {
        public ESourceState state { get; set; }
    }

    public class CScheduledntry
    {
        public string ChId = "";
        public DateTime Start = DateTime.MinValue;

        public CScheduledntry()
        {
            
        }

        public override int GetHashCode()
        {
            return HashThis(ChId, Start);
        }

        public static int HashThis(string chId, DateTime start)
        {
            int hash1 = start.GetHashCode();
            int finalHash = chId.GetHashCode() ^ hash1;
            return finalHash != 0 ? finalHash : hash1;            
        }

        public CScheduledntry(CScheduledntry en)
        {
            ChId = en.ChId;
            Start = en.Start;
        }

        public CScheduledntry(string chId, DateTime start)
        {
            ChId = chId;
            Start = start;
        }
    }

    public class CEPGData
    {
        public List<CChannelData> ChannelData = new List<CChannelData>();
        
        [XmlIgnore] 
        public Dictionary<string, CChannelData> ChannelDataById = new Dictionary<string, CChannelData>();

        [XmlIgnore] public bool HasChanged = false;


        void LogError(string msg)
        {
            TopManager.St.LogManager.Add(ELogEntryType.Error, "epgdata: ", msg);
        }

        private void DoError(string s)
        {
            LogError(s);
            throw new MyException(s);
        }

        public void MakeDictionary()
        {
            int i;
            CChannelData cd;
            for (i = 0; i < ChannelData.Count; i++)
            {
                cd = ChannelData[i];
                cd.MakeDictionary();
                if (!ChannelDataById.ContainsKey(cd.Id))
                {
                    ChannelDataById[cd.Id] = cd;
                }
            }
        }

        public CProgrammData GetByChIdAndStar(string chid, DateTime start)
        {
            CChannelData cd;
            CProgrammData pd = new CProgrammData();
            if (!ChannelDataById.TryGetValue(chid, out cd)) return null;
            if(!cd.ProgrammDataByStartTime.TryGetValue(start, out pd)) return null;
            return pd;
        }

        public void RemoveEmptyChannels()
        {
            int i;
            CChannelData cd;
            List<CChannelData> cds = new List<CChannelData>();
            for (i = 0; i < ChannelData.Count; i++)
            {
                cd = ChannelData[i];
                if(cd.ProgrammData.Count == 0) 
                    cds.Add(cd);
            }
            if (cds.Count > 0) HasChanged = true;
            for (i = 0; i < cds.Count; i++)
            {
                cd = cds[i];
                ChannelData.Remove(cd);
                if (!ChannelDataById.ContainsKey(cd.Id))
                {
                    ChannelDataById.Remove(cd.Id);
                }
            }
        }


        public List<CProgrammData> SearchForText(string text)
        {
            List<CProgrammData> progrmms = new List<CProgrammData>();
            List<CProgrammData> ps;
            if (text == "") return progrmms;
            foreach (CChannelData ch in ChannelData)
            {
                ps = ch.SearchForText(text);
                progrmms.AddRange(ps);
            }
            if (progrmms.Count == 0) return progrmms;
            progrmms.Sort(
                (pd1, pd2) =>
                {
                    return DateTime.Compare(pd1.Start, pd2.Start);
                });
            return progrmms;
        }


        public void SplitByDate()
        {
            foreach (CChannelData ch in ChannelData)
            {
                ch.SplitByDate();
            }
        }
        public void CheckReferences()
        {
            foreach (CChannelData ch in ChannelData)
            {
                ch.CheckReferences();
            }
        }

        public void Sort()
        {
            ChannelData.Sort(
                (ch1, ch2) =>
                {
                    return ch1.DisplayNameR.CompareTo(ch2.DisplayNameR);
                }
                );
        }

        public List<CProgrammData> GetByDateAndGroup(DateTime date, string groupname)
        {
            List<CProgrammData> programms = new List<CProgrammData>();
            List<string> chidsforgroup;
            if (!TopManager.St.ChannelsByGroup.TryGetValue(groupname, out chidsforgroup))
            {
                return programms;
            }
            List<CProgrammData> pd;
            CChannelData cd;
            foreach (string chid in chidsforgroup)
            {
                if (!ChannelDataById.TryGetValue(chid, out cd)) continue;
                cd = ChannelDataById[chid];
                if (cd.ProgrammDataByDate.TryGetValue(date, out pd))
                    programms.AddRange(pd);
            }
            programms.Sort(
                (pd1, pd2) =>
                {
                    return DateTime.Compare(pd1.Start, pd2.Start);
                });
            return programms;
        }

        public List<CProgrammData> GetByDateAndGroupCheckTag(DateTime date, string groupname)
        {
            List<CProgrammData> programms = new List<CProgrammData>();
            List<string> chidsforgroup;
            if (!TopManager.St.ChannelsByGroup.TryGetValue(groupname, out chidsforgroup))
            {
                return programms;
            }
            List<CProgrammData> pdl;
            CChannelData cd;
            EProgramTag programTag = EProgramTag.None;
            foreach (string chid in chidsforgroup)
            {
                if (!ChannelDataById.TryGetValue(chid, out cd)) continue;
                if (cd.ProgrammDataByDate.TryGetValue(date, out pdl))
                {
                    foreach (CProgrammData pd in pdl)
                    {
                        programTag = TopManager.St.EPGUserData.GetProgramTag(pd.Title);
                        if (programTag != EProgramTag.Ignore && programTag != EProgramTag.Seen)
                        {
                            programms.Add(pd);
                        }
                    }
                }
            }
            programms.Sort(
                (pd1, pd2) =>
                {
                    return DateTime.Compare(pd1.Start, pd2.Start);
                });
            return programms;
        }

        public List<string> GetNoDataChannels(DateTime date, int maxcount)
        {
            List<string> names = new List<string>();
            foreach (CChannelData ch in ChannelData)
            {
                if (!ch.ProgrammDataByDate.ContainsKey(date))
                {
                    names.Add(ch.DisplayNameR);
                    if(names.Count >= maxcount)
                        return names;
                }
            }
            return names;
        }

        public void ReadData(string filename)
        {
            using(XmlReader xmlReader = XmlReader.Create(filename))
            {
                ReadData(xmlReader);
            }
        }

        public void ReadData(XmlReader xmlReader)
        {
            string s;

            ChannelData.Clear();
            ChannelDataById.Clear();

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
                ChannelData.Add(ch);
                ChannelDataById[ch.Id] = ch;
                while (xmlReader.NodeType != XmlNodeType.Element)
                {
                    if (!xmlReader.Read()) break;
                }
            } while (true);


            while (xmlReader.NodeType != XmlNodeType.Element && xmlReader.Read())
            {}

            CProgrammData pd;
            do
            {
                pd = new CProgrammData();
                if (!pd.ReadXML(xmlReader)) break;
                if (ChannelDataById.TryGetValue(pd.ChId, out ch))
                {
                    ch.AddProgramm(pd);
                }
                while (xmlReader.NodeType != XmlNodeType.Element && xmlReader.Read())
                {}
            } while (true);
        }


        public void WriteData(string filename)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            using(XmlWriter xmlWriter = XmlWriter.Create(filename, settings))
            {
                WriteData(xmlWriter);
            }
        }

        public void WriteData(XmlWriter xmlWriter)
        {
            string s;
            
            xmlWriter.WriteStartElement("tv");
            foreach (CChannelData cd in ChannelData)
            {
                cd.WriteXML(xmlWriter);
            }

            foreach (CChannelData cd in ChannelData)
            {
                foreach (CProgrammData pd in cd.ProgrammData)
                {
                    pd.WriteXML(xmlWriter);
                }
            }

            xmlWriter.WriteEndElement();
            xmlWriter.Close();
        }

    }

}
