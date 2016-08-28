using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRateAnalyser.Utils
{
    public static class Logger
    {
        const string messageFormat = "Time = [TIME]\r\n Message = [COMMENT]\r\n Exception: \r\n [EXCEPTION]";

        public static bool Log(string message, Exception exception)
        {
            try
            {
                string result = messageFormat.Replace("[TIME]", DateTime.Now.ToString("yyyy.MM.dd hh:mm:ss"))
                                .Replace("[COMMENT]", message);
                if (exception != null)
                {
                    result = result.Replace("[EXCEPTION]", exception.ToString());
                }

                var logFilePath = System.Configuration.ConfigurationManager.AppSettings["LogFilePath"];

                FileInfo fi = new FileInfo(logFilePath);
                StreamWriter fs;
                if (fi.Exists)
                    fs = fi.AppendText();
                else
                    fs = fi.CreateText();
                fs.WriteLine(result);
                fs.WriteLine();
                fs.Flush();
                fs.Close();
                return true;
            }
            catch { return false; }
        }
    }
}
