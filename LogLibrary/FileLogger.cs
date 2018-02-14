using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogLibrary
{
    public class FileLogger
    {
        public static void AppendLog(string MasterCustID, LogType lType, string Functionname, string Message)
        {
            try
            {
                Environment.CurrentDirectory = ConfigurationManager.AppSettings["logsLocation"].ToString();
                string LogName = lType == LogType.Access ? "Access" : "Error";
                string FileName = "";

                FileName = MasterCustID + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".log";

                if (!Directory.Exists(Environment.CurrentDirectory + @"\Log"))
                {
                    Directory.CreateDirectory(Environment.CurrentDirectory + @"\Log");
                }

                if (!Directory.Exists(Environment.CurrentDirectory + @"\Log\" + LogName))
                {
                    Directory.CreateDirectory(Environment.CurrentDirectory + @"\Log\" + LogName);
                }

                if (!File.Exists(Environment.CurrentDirectory + @"\Log\" + LogName + @"\" + FileName))
                {
                    FileStream fs = File.Create(Environment.CurrentDirectory + @"\Log\" + LogName + @"\" + FileName);
                    fs.Close();
                }

                StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + @"\Log\" + LogName + @"\" + FileName, true);
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t" + Functionname + " " + Message, true);
                sw.Close();
            }
            catch (Exception ex)
            {
                addToWindowsLogs(Functionname, ex);
            }
        }

        public static void addToWindowsLogs(string source, Exception ex)
        {
            try
            {
                //System.Diagnostics.EventLog.WriteEntry("POWEBAPI", ex.StackTrace + "\n" + source, System.Diagnostics.EventLogEntryType.Warning);
            }
            catch (Exception e) { Console.Write(e.Message); }
        }
    }

    public enum LogType
    {
        Access, Error
    }
}
