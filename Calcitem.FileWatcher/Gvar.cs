/*
 * Copyright (c) 2006 Calcitem Studio
 */

using System;
using System.Windows.Forms;

namespace Calcitem.FileSystemWatch
{
    public class Gfct
    {
        public static int ConvertStringToUInt(string s)
        {
            if (s == null)
            {
                return 0;
            }

            int value = 0;
            int size = s.Length;
            for (int i = 0; i < size; ++i)
            {
                value += (s[i] - 48) * (int)Math.Pow(10, size - i - 1);
            }

            return value;
        }
    }


    public class Gval
    {
        public static int EventId;
        public static bool WriteLog;

        public static string[] MonPath = new string[128];
        public static string[] HisMonPath = new string[256];

        public static void ClearMonPath()
        {
            for (int i = 0; i < MonPath.Length; ++i)
            {
                MonPath[i] = null;
            }
        }

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.DoEvents();

            MainForm mainForm = new MainForm();
            Application.Run(mainForm);
        }

        public struct LatestLogEvent
        {
            public static string Date;
            public static string Time;
            public static string Path;
            public static string LatestEvent;
        }
    }
}
