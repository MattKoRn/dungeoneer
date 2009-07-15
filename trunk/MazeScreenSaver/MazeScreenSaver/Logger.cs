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

        public Logger()
        {
        }

        public void Write(string message)
        {
            if (!Directory.Exists(Directory.GetParent(logfile).ToString()))
                Directory.CreateDirectory(Directory.GetParent(logfile).ToString());
            StreamWriter file = new StreamWriter(logfile, true);
            file.WriteLine(message);
            file.Close();
        }
    }
}
