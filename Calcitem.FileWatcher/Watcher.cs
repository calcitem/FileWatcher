/*
 * Copyright (c) 2006 Calcitem Studio
 */

using System;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.FileIO;

namespace Calcitem.FileSystemWatch
{
    [Flags]
    public enum MonType
    {
        Changed = 1,
        Created = 2,
        Deleted = 4,
        Renamed = 8
    }

    public class Watcher
    {
        private static FileSystemWatcher s_watcher;

        //[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static int Run(string path, string file, MonType type, bool includeSubdirectories)
        {
            try
            {
                //Gval.mainForm.Text = "fff";
                // Create a new FileSystemWatcher and set its properties.
                s_watcher = new FileSystemWatcher();
                s_watcher.Path = path;
                /* Watch for changes in LastAccess and LastWrite times, and 
                   the renaming of files or directories. */
                s_watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                                                 | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                // Only watch text files.
                s_watcher.Filter = file;
                s_watcher.IncludeSubdirectories = includeSubdirectories;

                // Add event handlers.
                int t = (int)type;
                if (t % 2 == 1)
                {
                    s_watcher.Changed += OnChanged;
                }

                if (t % 4 > 1)
                {
                    s_watcher.Created += OnChanged;
                }

                if (t % 8 > 3)
                {
                    s_watcher.Deleted += OnChanged;
                }

                if (t > 7)
                {
                    s_watcher.Renamed += OnRenamed;
                }


                // Begin watching.
                s_watcher.EnableRaisingEvents = true;
                return 0;
            }
            catch (Exception)
            {
                return 1;
            }
        }

        private static string GetDirFromFullPath(string fullPath) =>
            fullPath.Remove(fullPath.LastIndexOf("\\", StringComparison.Ordinal));

        // Define the event handlers.
        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            //watcher.Changed -= new FileSystemEventHandler(OnChanged);
            string dirOrFile;
            string strChangeType = "";
            // Specify what is done when a file is changed, created, or deleted.

            if (FileSystem.DirectoryExists(e.FullPath))
            {
                dirOrFile = "Folder ";
            }
            else
            {
                dirOrFile = FileSystem.FileExists(e.FullPath) ? "File " : "";
            }

            short categoryNumber = 0;
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Changed:
                    strChangeType = "Modified";
                    categoryNumber = 0;
                    break;
                case WatcherChangeTypes.Created:
                    strChangeType = "Created";
                    categoryNumber = 1;
                    break;
                case WatcherChangeTypes.Deleted:
                    strChangeType = "Deleted";
                    categoryNumber = 2;
                    break;
                case WatcherChangeTypes.Renamed:
                    break;
                case WatcherChangeTypes.All:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            string thisEvent = dirOrFile + e.FullPath + " " + strChangeType;
            Gval.LatestLogEvent.Path = GetDirFromFullPath(e.FullPath);
            Gval.LatestLogEvent.Date =
                DateTime.Now.Date.Year + "-"
                                       + DateTime.Now.Date.Month + "-"
                                       + DateTime.Now.Date.Day;
            string time = DateTime.Now.TimeOfDay.ToString();
            try
            {
                time = time.Remove(time.IndexOf('.'));
            }
            catch (Exception)
            {
                // ignored
            }

            Gval.LatestLogEvent.Time = time;
            Gval.LatestLogEvent.LatestEvent = thisEvent; //////////////

            if (Gval.WriteLog == false)
            {
                return;
            }

            bool bWrite = Gval.MonPath.Any(t => t == GetDirFromFullPath(e.FullPath));

            if (bWrite)
            {
                FileSystemEventLog.WriteLog(thisEvent, categoryNumber);
            }
        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            string dirOrFile;
            if (FileSystem.DirectoryExists(e.FullPath))
            {
                dirOrFile = "Folder ";
            }
            else
            {
                dirOrFile = FileSystem.FileExists(e.FullPath) ? "File " : "";
            }

            string thisEvent = dirOrFile + e.OldFullPath + "was renamed to " + e.FullPath;
            Gval.LatestLogEvent.Path = GetDirFromFullPath(e.FullPath);
            Gval.LatestLogEvent.Date = DateTime.Now.Day.ToString();
            Gval.LatestLogEvent.Time = DateTime.Now.TimeOfDay.ToString();
            Gval.LatestLogEvent.LatestEvent = thisEvent; //////////////
            // Specify what is done when a file is renamed.

            bool bWrite = Gval.MonPath.Any(t => t == GetDirFromFullPath(e.OldFullPath));

            if (bWrite)
            {
                FileSystemEventLog.WriteLog(thisEvent, 3);
            }
        }
    }
}
