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
    public class CEPGUserData
    {
        public SerializableDictionary<string, List<string>> ChannelsByGroup = new SerializableDictionary<string, List<string>>();
        public List<CScheduledntry> ScheduledProgramms = new List<CScheduledntry>();
        public SerializableDictionary<string, EProgramTag> TagedProgramms = new SerializableDictionary<string, EProgramTag>();

        [XmlIgnore]
        public bool HasChanged = false;

        public EProgramTag GetProgramTag(string title)
        {
            EProgramTag tag;
            if (!TagedProgramms.TryGetValue(title, out tag)) return EProgramTag.None;
            return tag;
        }

        public void SetProgramTag(string title, EProgramTag tag)
        {
            if (tag == EProgramTag.None)
            {
                TagedProgramms.Remove(title);
                HasChanged = true;
                return;
            }
            if (GetProgramTag(title) == tag) return;
            TagedProgramms[title] = tag;
            HasChanged = true;
        }

        public void DoAutoSchedule()
        {
            if (TagedProgramms.Count == 0) return;
            foreach (CChannelData chd in TopManager.St.EPGData.ChannelData)
            {
                foreach (CProgrammData pd in chd.ProgrammData)
                {
                    if (pd.Start.Date >= DateTime.Today
                        && GetProgramTag(pd.Title) == EProgramTag.AutoSchedule)
                    {
                        AddToSchedule(pd.ChId, pd.Start);
                    }
                }
            }
        }

        public void ClearSchedule()
        {
            ScheduledProgramms.Clear();
            HasChanged = true;
        }

        public bool AddToSchedule(string chid, DateTime start)
        {
            if (ScheduledProgramms.FindIndex(
                se => se.ChId == chid && se.Start == start) > -1)
                return false;
            ScheduledProgramms.Add(new CScheduledntry(chid, start));
            HasChanged = true;
            return true;
        }

        public bool RemoveFromSchedule(string chid, DateTime start)
        {
            int k = ScheduledProgramms.FindIndex(
                se => se.ChId == chid && se.Start == start);
            if (k == -1) return false;
            ScheduledProgramms.RemoveAt(k);
            HasChanged = true;
            return true;
        }

        public void RemoveOldScheduleEntries()
        {
            int ct = ScheduledProgramms.Count;
            ScheduledProgramms = new List<CScheduledntry>(
                from se in ScheduledProgramms
                where se.Start.Date >= DateTime.Today
                select se);
            if (ct != ScheduledProgramms.Count) HasChanged = true;
        }

        public void RemoveDateFromSchedule(DateTime date)
        {
            ScheduledProgramms = new List<CScheduledntry>(
                from se in ScheduledProgramms
                where se.Start.Date != date
                select se);
            HasChanged = true;
        }

        public bool AddGroup(string name)
        {
            if (name == "") return false;
            if (ChannelsByGroup.ContainsKey(name)) return false;
            ChannelsByGroup[name] = new List<string>();
            HasChanged = true;
            return true;
        }

        public bool DeleteGroup(string name)
        {
            if (name == "") return false;
            if (!ChannelsByGroup.ContainsKey(name)) return false;
            ChannelsByGroup.Remove(name);
            HasChanged = true;
            return true;
        }

        public bool RenameGroup(string name, string newname)
        {
            if (name == "" || newname == "") return false;
            if (!ChannelsByGroup.ContainsKey(name)) return false;
            if (ChannelsByGroup.ContainsKey(newname)) return false;
            var cl = ChannelsByGroup[name];
            ChannelsByGroup.Remove(name);
            ChannelsByGroup[newname] = cl;
            HasChanged = true;
            return true;
        }
    }

}
