using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBackup.Interfaces
{
    public interface ILog
    {
        public void LogWriter(LogLevel level, string msg);

        public void Info(string msg);

        public void Error(string msg);

        public void Debug(string msg);
    }
}
