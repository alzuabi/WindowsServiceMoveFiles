using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsServiceMoveFiles.Entity;

namespace WindowsServiceMoveFiles
{
    class Watcher : IDisposable
    {
        private FileSystemWatcher _watcher;
        private string _dest;
        private static readonly Watcher instance = new Watcher();
        private readonly EventLog eventLog = new EventLog();
        private string Dest 
        {
            get => _dest;
            set => _dest = value;
        }
        private FileSystemWatcher watcher
        {
            get => _watcher;
            set => _watcher = value;
        }

        private Watcher(FileSystemWatcher watcher, string dist)
        {
            _watcher = watcher;
            _dest = dist;
        }

        private Watcher()
        {
        }
        static Watcher()
        {
        }

        public Watcher(FileSystemWatcher watcher)
        {
           _watcher = watcher;
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
            try
            {
                WriteToFile("OnChanged at " + DateTime.Now);
                eventLog.WriteEntry("OnChanged", EventLogEntryType.Information, 100);
                eventLog.WriteEntry("e.file " + e.FullPath, EventLogEntryType.Information, 100);
                string fileName = Path.GetFileNameWithoutExtension(e.Name);
                string dir = Path.Combine(fileName.Split('-'));
                string dest = Path.Combine(Dest, dir);
                Directory.CreateDirectory(dest);
                dest = Path.Combine(dest, e.Name);

                if (File.Exists(dest))
                {
                    File.Delete(e.FullPath);
                }
                else
                {
                    File.Move(e.FullPath, dest);
                    using (var db = new TestContext())
                    {
                        var ev = new Event()
                        {
                            eventName = "test",
                            eventnDesc = e.Name,
                            eventDate = DateTime.Now
                        };
                        db.Events.Add(ev);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex) { WriteToFile(ex.Message); }

        }
        public Watcher GetWatcherForDirectory(string source, string dist)
        {
            watcher = new FileSystemWatcher
            {
                Path = source,
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                   | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.CreationTime | NotifyFilters.Size,
                Filter = "*.*"
            };
            Dest = dist;
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;
            return new Watcher(watcher);
        }

        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\Files\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
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
