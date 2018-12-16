using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        static StreamWriter w = File.AppendText("log.txt");
        public static void LogException(Exception ex)
        {
         
            w.Write("\r\nLog Entry : ");
            w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString());
            w.WriteLine("  :{0}", ex.Message);
            w.WriteLine("-------------------------------");
            // TODO: Create log file named log.txt to log exception details in it
            // for each exception write its details associated with datetime 
        }
    }
}
