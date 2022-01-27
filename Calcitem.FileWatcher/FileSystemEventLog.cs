/*
 * Copyright (c) 2006 Calcitem Studio
 */

using System.Diagnostics;

namespace Calcitem.FileSystemWatch
{
    public class FileSystemEventLog
    {
        private const string Source = "Calcitem Studio";
        private const string Item = "File System";

        public static void WriteLog(string log, short categoryNumber)
        {
            if (!EventLog.SourceExists(Source))
            {
                EventLog.CreateEventSource(Source, Item);
            }

            EventLog myLog = new EventLog();
            myLog.Source = Source;
            myLog.Log = Item;
            //myLog.c
            //myLog.WriteEntry("");
            myLog.WriteEntry(log, EventLogEntryType.Information, Gval.EventId, categoryNumber);
            ++Gval.EventId;
            // myLog.Close();
        }
    }
}
