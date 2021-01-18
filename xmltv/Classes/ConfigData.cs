using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SerializableDictionary;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace xmltv
{
    
    public class CConfigData
    {
        public List<CSourceConfigData> SourceConfigData = new List<CSourceConfigData>();

        public string CheckConfigData()
        {
            int i;
            string s;
            CSourceConfigData sourceConfig;
            CSourceConfigData sourceConfig2;
            Dictionary<string, CSourceConfigData> byName = new Dictionary<string, CSourceConfigData>();
            for (i = 0; i < SourceConfigData.Count; i++)
            {
                sourceConfig = SourceConfigData[i];
                if (byName.TryGetValue(sourceConfig.Name, out sourceConfig2))
                {
                    return "Duplicate source name:" + sourceConfig.Name;
                }
                s = sourceConfig.CheckConfigData();
                if (s != "OK")
                {
                    return s;
                }
                sourceConfig.ConfigOk = true;
            }
            return "OK";
        }
    }


    public class CSourceConfigData
    {
        public string Name = "";
        public string URL = "";
        public string Prefix = "";
        public string Group = "";
        public int AddHours = 0;

        public List<string> KeepChannelsIds = new List<string>();
        public SerializableDictionary<string, string> RenameChannelsIds = new SerializableDictionary<string, string>();

        [XmlIgnore] public bool ConfigOk = false;

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
    }

    public class CGZSourceURL
    {
        public string[] url;
    }

    public class CGZSourceChannel
    {
        [XmlAttribute]
        public string name = "";
        [XmlElement]
        public string[] url;
    }

    public class CGZSourceSource
    {
        public string description = "";
        [XmlElement]
        public string[] url;
    }

    [XmlRoot(ElementName = "sources")]
    public class CGZSource
    {
        [XmlElement(ElementName = "channel")] 
        public CGZSourceChannel[] channel;

        [XmlElement(ElementName = "source")]
        public CGZSourceSource[] source;
    }

    public class CSettings
    {
        private string colorThemeId = "system";
        private MyColorTheme colorTheme = null;

        [XmlIgnore]
        public MyColorTheme ColorTheme
        {
            get
            {
                if (colorTheme == null)
                {
                    colorTheme = ColorThemeHelper.ColorTheme_System;
                }
                return colorTheme;
            }
        }

        public string ColorThemeId
        {
            get { return colorThemeId; }
            set
            {
                if (colorThemeId == value) return;
                colorThemeId = value;
                colorTheme = null;
                switch (value)
                {
                    case "system":
                        colorTheme = ColorThemeHelper.ColorTheme_System;
                        break;
                    case "dark1":
                        colorTheme = ColorThemeHelper.ColorTheme_Dark1;
                        break;
                    case "green":
                        colorTheme = ColorThemeHelper.ColorTheme_Green;
                        break;
                    case "blackonwhite":
                        colorTheme = ColorThemeHelper.ColorTheme_BlackOnWhite;
                        break;

                }
                if (colorTheme == null)
                {
                    colorTheme = ColorThemeHelper.ColorTheme_System;
                    colorThemeId = "system";

                }
                HasChanged = true;
            }
        }

        public float FontSize = 12;
        public int TimePlusHours = 0;
        public bool CheckMisingDataOnOpen = false;
        public int MainFormWidth = -1;
        public int MainFormHight = -1;
        public string SelectedSources = "";

        [XmlIgnore] public bool HasChanged = false;
    }
}
