using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsServiceTest1.Service
{
    class Watcher : IDisposable
    {
        private FileSystemWatcher _watcher;
        private static readonly Watcher instance = new Watcher();
        private EventLog eventLog = new EventLog();
        private FileSystemWatcher watcher
        {
            get => _watcher;
            set => _watcher = value;
        }

        private Watcher(FileSystemWatcher watcher) => this._watcher = watcher;

        private Watcher()
        {
        }
        static Watcher()
        {
        }

        public static Watcher Instance
        {
            get
            {

                return instance;
            }
        }

        internal void LogEventStart()
        {
            try
            {

                if (!EventLog.SourceExists("MultiSys"))
                {
                    EventLog.CreateEventSource("MultiSys", "");
                }

                eventLog.Source = "MultiSys";

                //eventLog.Log = "MultiSysServiceLog";


                eventLog.WriteEntry("Start Multisys.", EventLogEntryType.SuccessAudit, 100);
            }
            catch (Exception e)
            {

                WriteToFile(e.Message);
            }
            WriteToFile("Afetr Write an entry to the event log");

        }

        internal void LogEventSTop()
        {
            eventLog.Source = "MultiSys";

            eventLog.WriteEntry("Stop Multisys.", EventLogEntryType.SuccessAudit, 100);
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            eventLog.WriteEntry("OnChanged", EventLogEntryType.Information, 100);
            eventLog.WriteEntry("e.file " + e.FullPath, EventLogEntryType.Information, 100);
            string dest = Path.Combine(@"C:\Users\ASUS\MultiSys\dist", e.Name);
            try
            {
                FileInfo fileInfo = new FileInfo(dest);
                if (File.Exists(dest))
                {
                    File.Delete(e.FullPath);
                }
                else
                {

                    File.Move(e.FullPath, dest);

                }
            }
            catch (Exception ex) { WriteToFile(ex.Message); }


        }
        public Watcher GetWatcherForDirectory(String path)
        {
            watcher = new FileSystemWatcher
            {
                Path = path,
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                   | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.CreationTime | NotifyFilters.Size,
                Filter = "*.*"
            };
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;

            //Watcher temp= new Watcher(watcher);
            //temp.eventLog = new EventLog();
            //WriteToFile(temp.eventLog.ToString());
            return new Watcher(watcher);
        }

        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }

        public void Dispose()
        {

        }

        //public override string ToString()
        //{
        //    return $"{{{nameof(Instance)}={Instance}}}";
        //}
        //bool IsFileLocked(FileInfo file)
        //{
        //    FileStream stream = null;

        //    try
        //    {
        //        stream = file.Open(FileMode.Open,
        //                 FileAccess.ReadWrite, FileShare.None);
        //    }
        //    catch (IOException)
        //    {
        //        //the file is unavailable because it is:
        //        //still being written to
        //        //or being processed by another thread
        //        //or does not exist (has already been processed)
        //        return true;
        //    }
        //    finally
        //    {
        //        if (stream != null)
        //            stream.Close();
        //    }

        //    //file is not locked
        //    return false;
        //}

    }
}
