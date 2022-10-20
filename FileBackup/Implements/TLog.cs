using FileBackup.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBackup.Implements
{
    public class TLog: ILog
    {
        private readonly ILogger<ILog> _log;
        public TLog(ILogger<ILog> log)
        {
            this._log = log;
        }

        public void Info(string msg)
        {
            _log.LogInformation(msg);
        }

        public void Error(string msg)
        {
            _log.LogError(msg);
        }

        public void Debug(string msg)
        {
            _log.LogDebug(msg);
        }

        public void LogWriter(LogLevel level, string msg)
        {
            ////调用者类名和方法名
            //string logMSourceInfo = "";
            //StackTrace trace = new StackTrace();
            //string invokerType = trace.GetFrame(2).GetMethod().DeclaringType.Name;
            //string invokerMethod = trace.GetFrame(2).GetMethod().Name;
            //logMSourceInfo = $"{invokerType}.{invokerMethod}|";

            switch (level)
            {
                case LogLevel.Debug:
                    Debug(msg);
                    break;
                case LogLevel.Error:
                    //msg = logMSourceInfo + msg;
                    Error(msg);
                    break;
                case LogLevel.Information:
                    Info(msg);
                    break;
            }
        }
    }
}
