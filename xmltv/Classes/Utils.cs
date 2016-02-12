using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xmltv
{
    public static class Utils
    {
        public static string GetMyFolder()
        {
            string s = Application.ExecutablePath;
            int i = s.Length - 1;
            while (s[i] != '\\' && s[i] != '/') i--;
            s = s.Substring(0, i);
            return s;
        }

        public static string GetFolder(string filename)
        {
            int i, k = filename.Length - 1;
            char c;
            for (i = k; i >= 0; i--)
            {
                c = filename[i];
                if (c == '\\' || c == '/') return filename.Substring(0, i);
            }
            return "";
        }

        public static string GetFileNameFromURL(string url)
        {
            int i, k = url.Length - 1;
            for (i = k; i >= 0; i--)
                if (url[i] == '\\' || url[i] == '/')
                    return url.Substring(i + 1);
            return url;
        }

        public static string RemoveExt(string filename)
        {
            int i, k = filename.Length - 1;
            char c;
            for (i = k; i >= 0; i--)
            {
                c = filename[i];
                if (c == '.') return filename.Substring(0, i);
                if (c == '\\' || c == '/') return "";
            }
            return filename;
        }

        public static string GetExt(string filename)
        {
            int i, k = filename.Length - 1;
            char c;
            for (i = k; i >= 0; i--)
            {
                c = filename[i];
                if (c == '.') return filename.Substring(i + 1, k - i);
                if (c == '\\' || c == '/') return "";
            }
            return "";
        }

        public static string GetNextFile(string folder, string prefix, string ext)
        {
            string fnm = folder + "\\" + prefix;
            ext = "." + ext;
            int i = 1;
            while (File.Exists(fnm + i + ext)) i++;
            return fnm + i + ext;
        }

        public static long GetFileSize(string filename)
        {
            try
            {
                FileInfo fileinfo = new FileInfo(filename);
                return fileinfo.Length;
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}
