using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading.Tasks;
using System.IO.Compression;
using XZ.NET;

namespace xmltv
{

    public delegate void GZipExtractEventListener(GZipExtract gzipExtract, ESimpleEvent gzipExtractEvent);

    public class GZipExtract
    {
        private GZipExtractEventListener EventListener = null;
        public string GZipFileName { get; private set; }
        public string TargetFile { get; private set; }
        public bool Started { get; private set; }

        private void LogError(string msg)
        {
            TopManager.St.LogManager.Add(ELogEntryType.Error, "gzextract", msg);
        }

        private void DoError(string s)
        {
            LogError(s);
            throw new MyException(s);
        }
        public static bool Decompress(string source, string target)
        {
            try
            {
                using (FileStream originalFileStream = File.OpenRead(source))
                {
                    using (FileStream decompressedFileStream = File.Create(target))
                    {
                        using (
                            GZipStream decompressionStream = new GZipStream(originalFileStream,
                                CompressionMode.Decompress))
                        {
                            decompressionStream.CopyTo(decompressedFileStream);
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static bool DecompressXZ(string source, string target)
        {
            var inFileStream = new FileStream(source, FileMode.Open);
            var binaryWriter = new BinaryWriter(new FileStream(target, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None));
            try
            {
                using (var xzStream = new XZInputStream(inFileStream))
                {
                    var buf = new byte[2048];
                    while (true)
                    {
                        var count = xzStream.Read(buf, 0, buf.Length);
                        binaryWriter.Write(buf, 0, count);
                        if (count == 0)
                            break;
                    }
                }
                inFileStream.Close();
                binaryWriter.Close();
                return true;
            }
            catch (Exception e)
            {
                inFileStream.Close();
                binaryWriter.Close();
                return false;
            }
        }


        public GZipExtract(string gzipFileName, string targetFile, GZipExtractEventListener eventListener)
        {
            Started = true;
            GZipFileName = gzipFileName;
            TargetFile = targetFile;
            EventListener = eventListener;
            if (!File.Exists(gzipFileName))
                DoError("File not found: " + gzipFileName);
            if (Utils.GetFileSize(gzipFileName) < 1)
                DoError("Empty file: " + gzipFileName);

            Task<bool> t = new Task<bool>(
                (object gzipExtract) =>
                {
                    if (gzipFileName.ToLower().EndsWith(".gz"))
                    {
                        GZipExtract gz = (GZipExtract)gzipExtract;
                        return Decompress(gz.GZipFileName, gz.TargetFile);
                    }
                    else if (gzipFileName.ToLower().EndsWith(".xz"))
                    {
                        return DecompressXZ(gzipFileName, targetFile);
                    }
                    return false;
                    //return ExtractGZipA(gz.GZipFileName, gz.TargetFile);
                }
                , this);

            t.ContinueWith(OnComplete, this);

            if (eventListener != null)
                eventListener(this, ESimpleEvent.started);

            t.Start();
        }

        private static void OnComplete(Task<bool> t1, object gzipExtract)
        {
            GZipExtract gz = (GZipExtract) gzipExtract;
            if (gz.EventListener == null) return;
            switch (t1.Status)
            {
                case TaskStatus.RanToCompletion:
                    if (t1.Result)
                    {
                        gz.EventListener(gz, ESimpleEvent.finished);
                    }
                    else
                    {
                        gz.LogError("failed to extract: " + gz.GZipFileName);
                        gz.EventListener(gz, ESimpleEvent.failed);
                    }
                    break;
                case TaskStatus.Faulted:
                    gz.LogError("failed to extract: " + gz.GZipFileName);
                    gz.LogError(t1.Exception.InnerException.Message);
                    gz.EventListener(gz, ESimpleEvent.failed);
                    break;
            }
        }

        /*
        public bool ExtractGZipA(string gzipFileName, string targetFile)
        {

            // Use a 4K buffer. Any larger is a waste.    
            byte[] dataBuffer = new byte[4096];

            try
            {
                using (Stream fs = new FileStream(gzipFileName, FileMode.Open, FileAccess.Read))
                {
                    using (GZipInputStream gzipStream = new GZipInputStream(fs))
                    {
                        if (File.Exists(targetFile))
                            File.Delete(targetFile);
                        using (FileStream fsOut = File.Create(targetFile))
                        {
                            StreamUtils.Copy(gzipStream, fsOut, dataBuffer);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogError(e.Message);
                return false;
            }
            return true;
        }
        */
    }
}
