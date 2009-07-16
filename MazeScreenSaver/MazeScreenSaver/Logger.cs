using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MazeScreenSaver
{
    class Logger
    {
        private string logfile = @"c:\dungeoneer\log.txt";
        private bool enabled;

        public Logger()
        {
            enabled = true;
        }

        public void enableLogging(bool status)
        {
            enabled = status;
        }

        public void Write(string message)
        {
            this.Write(message, 1);
        }

        public void Write(string message, int importance)
        {
            if (!enabled)
                return;

            if (!Directory.Exists(Directory.GetParent(logfile).ToString()))
                Directory.CreateDirectory(Directory.GetParent(logfile).ToString());

            FileInfo fi = new FileInfo(logfile);
            if (fi.Length > 1024 * 1024 * 10 && importance < 2)
                return;

            StreamWriter file = new StreamWriter(logfile, true);
            file.WriteLine(message);
            file.Close();
        }
    }
}
