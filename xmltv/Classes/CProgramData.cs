using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace xmltv
{
    public class CProgrammData
    {
        [XmlAttribute]
        public string ChId = "";
        [XmlAttribute]
        public DateTime Start = DateTime.MinValue;
        [XmlAttribute]
        public DateTime Stop = DateTime.MinValue;
        public string Title = "";
        public string TitleLang = "";
        public string SubTitle = "";
        public string SubTitleLang = "";
        public string Desc = "";
        public string DescLang = "";

        [XmlAttribute]
        public int TimePlusHours
        {
            get { return TimePlus.Hours; }
            set { TimePlus = new TimeSpan(value, 0, 0); }
        }

        [XmlIgnore]
        public DateTime StartA = DateTime.MinValue;
        [XmlIgnore]
        public DateTime StopA = DateTime.MinValue;

        [XmlIgnore]
        public TimeSpan TimePlus = new TimeSpan(0, 0, 0);
        [XmlIgnore]
        public CChannelData ChannelData = null;

        public static TimeSpan MyTimePlus = TimeSpan.FromHours(2);

        void LogError(string msg)
        {
            TopManager.St.LogManager.Add(ELogEntryType.Error, "programmdata", msg);
        }

        private void DoError(string s)
        {
            LogError(s);
            throw new MyException(s);
        }

        public bool ContainsText(string text)
        {
            if (text == "") return false;
            if (Title.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                return true;
            }
            if (SubTitle.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                return true;
            }
            if (Desc.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                return true;
            }
            return false;
        }

        bool ParseTimeString(string ts, out DateTime dt, out TimeSpan plustime)
        {
            string s, s1, s2;
            int min, hr, yr, mt, day;
            dt = DateTime.MinValue;
            plustime = new TimeSpan(0, 0, 0);
            if (ts.Length == 20)
            {
                s = ts.Substring(15, 1);
                if (!int.TryParse(ts.Substring(16, 2), out hr)) return false;
                if (!int.TryParse(ts.Substring(18, 2), out min)) return false;
                if (s == "+")
                {
                    plustime = new TimeSpan(hr, min, 0);
                }
                else
                {
                    plustime = new TimeSpan(-hr, -min, 0);
                }
                ts = ts.Substring(0, 14);
            }
            if (ts.Length != 14) return false;
            if (!int.TryParse(ts.Substring(0, 4), out yr)) return false;
            if (!int.TryParse(ts.Substring(4, 2), out mt)) return false;
            if (!int.TryParse(ts.Substring(6, 2), out day)) return false;
            if (!int.TryParse(ts.Substring(8, 2), out hr)) return false;
            if (!int.TryParse(ts.Substring(10, 2), out min)) return false;
            try
            {
                dt = new DateTime(yr, mt, day, hr, min, 0);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public string GetTimeString(DateTime dt, int plusthours)
        {
            string s = dt.ToString("yyyyMMddHHmm00");
            if (plusthours == 0) return s;
            s += plusthours > 0 ? " +" : " -";
            s += plusthours.ToString("D2") + "00";
            return s;
        }

        public bool ReadXML(XmlReader xmlReader)
        {
            string s;
            if (xmlReader.NodeType != XmlNodeType.Element)
            {
                return false;
            }
            if (xmlReader.Name != "programme")
            {
                return false;
            }

            s = xmlReader.GetAttribute("channel");
            if (s == null || s == "")
            {
                DoError("XML programm has no channel id");
                return false;
            }
            ChId = s;

            s = xmlReader.GetAttribute("start");
            if (s == null)
            {
                DoError("XML no programm start");
                return false;
            }
            if (!ParseTimeString(s, out StartA, out TimePlus))
            {
                DoError("XML error in programm start");
                return false;
            }
            Start = StartA + MyTimePlus - TimePlus;

            s = xmlReader.GetAttribute("stop");
            if (s == null)
            {
                DoError("XML no programm stop");
                return false;
            }
            if (!ParseTimeString(s, out StopA, out TimePlus))
            {
                DoError("XML error in programm stop");
                return false;
            }
            Stop = StopA + MyTimePlus - TimePlus;

            bool stopflag = true;
            while (xmlReader.Read() && stopflag)
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (xmlReader.Name)
                        {
                            case "title":
                                s = xmlReader.GetAttribute("lang");
                                if (s != null) TitleLang = s;
                                s = xmlReader.ReadString();
                                if (s != null) Title = s;
                                break;
                            case "sub-title":
                                s = xmlReader.GetAttribute("lang");
                                if (s != null) SubTitleLang = s;
                                s = xmlReader.ReadString();
                                if (s != null) SubTitle = s;
                                break;
                            case "desc":
                                s = xmlReader.GetAttribute("lang");
                                if (s != null) DescLang = s;
                                s = xmlReader.ReadString();
                                if (s != null) Desc = s;
                                break;
                        }
                        break;

                    case XmlNodeType.EndElement:
                        switch (xmlReader.Name)
                        {
                            case "programme":
                                stopflag = false;
                                break;
                        }
                        break;
                }
            }

            return true;
        }


        public bool WriteXML(XmlWriter xmlWriter)
        {
            string s;
            xmlWriter.WriteStartElement("programme");
            xmlWriter.WriteAttributeString("start", GetTimeString(StartA, TimePlusHours));
            xmlWriter.WriteAttributeString("stop", GetTimeString(StopA, TimePlusHours));
            xmlWriter.WriteAttributeString("channel", ChId);

            xmlWriter.WriteStartElement("title");
            if (TitleLang != "")
                xmlWriter.WriteAttributeString("lang", TitleLang);
            xmlWriter.WriteString(Title);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("sub-title");
            if (SubTitleLang != "")
                xmlWriter.WriteAttributeString("lang", SubTitleLang);
            xmlWriter.WriteString(SubTitle);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("desc");
            if (DescLang != "")
                xmlWriter.WriteAttributeString("lang", DescLang);
            xmlWriter.WriteString(Desc);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
            return true;
        }

    }

}
