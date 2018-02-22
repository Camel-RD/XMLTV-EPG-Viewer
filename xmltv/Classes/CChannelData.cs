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
    public class CChannelData
    {
        [XmlAttribute]
        public string Id = "";
        [XmlAttribute]
        public string DisplayName = "";
        [XmlAttribute]
        public string DisplayNameLang = "";
        [XmlAttribute]
        public string DisplayName2 = "";
        [XmlAttribute]
        public string Prefix = "";
        public List<CProgrammData> ProgrammData = new List<CProgrammData>();

        [XmlIgnore]
        public CSource Source = null;

        [XmlIgnore]
        public Dictionary<DateTime, CProgrammData> ProgrammDataByStartTime =
            new Dictionary<DateTime, CProgrammData>();

        [XmlIgnore]
        public Dictionary<DateTime, List<CProgrammData>> ProgrammDataByDate =
            new Dictionary<DateTime, List<CProgrammData>>();

        [XmlIgnore]
        public List<DateTime> DatesUsed = new List<DateTime>();

        private void LogError(string msg)
        {
            TopManager.St.LogManager.Add(ELogEntryType.Error, "channeldata", msg);
        }

        private void DoError(string s)
        {
            LogError(s);
            throw new MyException(s);
        }

        public string DisplayNameA
        {
            get
            {
                if (DisplayName != "") return DisplayName;
                return Id;
            }
        }

        public string DisplayNameR
        {
            get
            {
                if (DisplayName2 != "") return Prefix + DisplayName2;
                if (DisplayName != "") return Prefix + DisplayName;
                return Prefix + Id;
            }
        }

        public bool AddProgramm(CProgrammData pr)
        {
            if (ProgrammDataByStartTime.ContainsKey(pr.Start)) return false;
            if (pr.Start.Date < DateTime.Today) return false;
            pr.SubTitle = pr.SubTitle.Trim();
            ProgrammData.Add(pr);
            ProgrammDataByStartTime[pr.Start] = pr;
            pr.ChannelData = this;
            return true;
        }

        public void AddProgrammsFrom(CChannelData ch)
        {
            foreach (var pr in ch.ProgrammData)
            {
                AddProgramm(pr);
            }
        }

        public void CheckReferences()
        {
            for (int i = 0; i < ProgrammData.Count; i++)
            {
                ProgrammData[i].ChannelData = this;
            }
        }

        public void MakeDictionary()
        {
            int i;
            CProgrammData pr;
            for (i = 0; i < ProgrammData.Count; i++)
            {
                pr = ProgrammData[i];
                if (!ProgrammDataByStartTime.ContainsKey(pr.Start))
                {
                    ProgrammDataByStartTime[pr.Start] = pr;
                }
            }
        }

        public void SplitByDate()
        {
            int i;
            CProgrammData pr;
            DateTime dt;
            List<CProgrammData> lpr;

            SortData();
            ProgrammDataByDate = new Dictionary<DateTime, List<CProgrammData>>();
            DatesUsed.Clear();

            for (i = 0; i < ProgrammData.Count; i++)
            {
                pr = ProgrammData[i];
                dt = pr.Start;
                dt = new DateTime(dt.Year, dt.Month, dt.Day);

                if (!ProgrammDataByDate.TryGetValue(dt, out lpr))
                {
                    lpr = new List<CProgrammData>();
                    ProgrammDataByDate[dt] = lpr;
                    DatesUsed.Add(dt);
                }
                lpr.Add(pr);
            }
        }

        public void SortData()
        {
            ProgrammData.Sort(
                (pr1, pr2) =>
                {
                    return DateTime.Compare(pr1.Start, pr2.Start);
                });
        }

        public List<CProgrammData> SearchForText(string text)
        {
            List<CProgrammData> progrmms = new List<CProgrammData>();
            if (text == "") return progrmms;
            foreach (CProgrammData pd in ProgrammData)
            {
                if (pd.ContainsText(text))
                    progrmms.Add(pd);
            }
            return progrmms;
        }

        public bool ReadXML(XmlReader xmlReader)
        {
            string s;
            if (xmlReader.NodeType != XmlNodeType.Element)
            {
                return false;
            }
            if (xmlReader.Name != "channel")
            {
                return false;
            }

            s = xmlReader.GetAttribute("id");
            if (s == null)
            {
                DoError("XML no channel id");
                return false;
            }

            Id = s;
            if (!xmlReader.ReadToDescendant("display-name"))
            {
                //DoError("XML no display-name");
                //return false;
                DisplayName = Id;
            }
            else
            {
                s = xmlReader.GetAttribute("lang");
                if (s == null) s = "";
                DisplayNameLang = s;
                DisplayName = xmlReader.ReadString();
            }

            while (!(xmlReader.NodeType == XmlNodeType.EndElement
                     && xmlReader.Name == "channel") && xmlReader.Read())
            {
            }

            return true;
        }

        public bool WriteXML(XmlWriter xmlWriter)
        {
            string s;
            xmlWriter.WriteStartElement("channel");
            xmlWriter.WriteAttributeString("id", Id);
            xmlWriter.WriteStartElement("display-name");
            if (DisplayNameLang != "")
                xmlWriter.WriteAttributeString("lang", DisplayNameLang);
            xmlWriter.WriteString(DisplayNameR);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
            return true;
        }
    }

}
