# Calcitem File System Monitor

# Prepare

## Installation requirements

## Internet Software

Installation of Calcitem File System Monitor requires Microsoft Internet Explorer 6.0 SP1 or higher, as required for the Calcitem browser. A minimal installation of Internet Explorer is sufficient, and Internet Explorer does not have to be the default browser.

## .NET Framework

Installing Calcitem File System Monitor requires Microsoft® .NET Framework 2.0 or higher.

Calcitem File System Monitor does not install .NET Framework 2.0. Before installing Calcitem File System Monitor, you must download the [Download Microsoft .NET Framework 2.0 Service Pack 1 (x64) from Official Microsoft Download Center](https://www.microsoft.com/en-us/download/details.aspx?id=6041) to download and install .NET Framework 2.0.

## Operating System

Calcitem File System Monitor runs on Windows 2000 or later.

Note: In Windows XP (before Service Pack 1) or Windows 2000 SP2 or earlier, if monitoring the same path is started multiple times, only one monitor will detect the event. All monitoring will see the appropriate event on computers running Windows XP SP1 and later, Windows 2000 SP3 or later, or Windows Server 2003.

# Introduction

Calcitem File System Monitor monitors local hard drives and sends information about file system changes to the Windows Management Service.

## User Interface Reference

Run Calcitem.FileSystemWatcher.exe. Select a directory in the directory list on the left to add the directory to the list on the right.

Select the directory in the list, and then click the "Start" button on the toolbar, the selected directory enters the monitored state, and the directory name changes from black to brown.

Before starting monitoring, you can specify the file type in the toolbar and specify whether to monitor subdirectories and the scope of monitoring in the program menu, including modification, creation, deletion, and renaming of files or directories. Note: Once you have specified a monitoring type, these options cannot be modified during monitoring.

Click the "Stop" button in the toolbar, the program will stop monitoring all the selected directories in the list on the right, and the directory's name will turn black.

If the directory is in the monitored state, all changes in the directory that meet the specified conditions will be captured. The event list on the right shows the most recently changed events, and the frequency of updating events is determined by the "Update Frequency" option in the menu. "High", "Medium", and "Low" mean refresh once every 1, 5, and 10 seconds, respectively.

Click the "Enable" button in the "Log" toolbar of the main window, then if there are directories in the monitoring state, the change events of these directories that meet the conditions will be written to the system log. You can right-click the "My Computer" icon on the desktop. , and then click "Management" -> "Event Viewer" -> "File System", the recent events are listed on the right. Double-click these events to view details.

By right-clicking "System Files", and then click Properties, you can set log-related options.

Run the "Log Viewer" from the "Refresh Log" button in the log toolbar in the program's toolbar. You can set the upper limit for the viewer to display log items on the right side of the toolbar.

Click the "Refresh" button. The program will read the event items and display them in chronological order. The log filter bar on the toolbar determines that only events that contain the text in the filter bar will be displayed.

Specify the text in the event to be marked in the marking column, and then click the "Mark" button then the relevant item will be kept in blue.

You can clear the log. Once the log is removed, the "File System" log entry in the System Event Viewer will be deleted.

You can specify the refresh method in the menu in the log viewer. If you select "Auto-refresh", the latest events will be inserted into the list in real-time.

Double-click the selected item in the list on the right side of the program window to view the files in this directory. The tool for browsing files is "Clacitem Browser".

There is a navigation bar below the address bar of "Clacitem Browser", which can be reversed to any directory level.

You can also use the "Clacitem Browser" to browse the web, and the navigation bar can also bring you convenience.

# Supplementary explanation

Standard file system operations may raise several events. For example, when a file is moved from one directory to another, several "change" and some "create" and "delete" events may be raised. Moving a file is a complex operation consisting of several simple functions and thus raises several events. Also, some applications, such as anti-virus software, can cause additional file system events detected by File System Monitor.

The file system monitor can monitor disks if they are not switched or removed. Because the timestamps and attributes of CDs and DVDs cannot be changed, the File System Monitor does not raise events for CDs and DVDs.
