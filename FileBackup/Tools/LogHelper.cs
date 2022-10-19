using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;

namespace FileBackup.Tools
{
    public static class LogHelper
    {

        static ServiceCollection _service { get; set; }
        static LogHelper()
        {
            SetupStaticLogger();
        }

        private static void SetupStaticLogger()
        {
            _service = new ServiceCollection();
            _service.AddLogging(logBuilder =>
            {
                logBuilder.AddConsole();
                logBuilder.SetMinimumLevel(LogLevel.Information);
                logBuilder.AddNLog();
            });
            _service.AddScoped<TLog>();
        }


        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="level"></param>
        /// <param name="msg"></param>
        private static void LogWriter(LogLevel level, string msg)
        {
            ////调用者类名和方法名
            //string logMSourceInfo = "";
            //StackTrace trace = new StackTrace();
            //string invokerType = trace.GetFrame(2).GetMethod().DeclaringType.Name;
            //string invokerMethod = trace.GetFrame(2).GetMethod().Name;
            //logMSourceInfo = $"{invokerType}.{invokerMethod}|";
            
            using (var sp = _service.BuildServiceProvider())
            {
                var operatio = sp.GetRequiredService<TLog>();
                switch (level)
                { 
                   case LogLevel.Debug:
                        operatio.Debug(msg);
                        break;
                    case LogLevel.Error:
                        //msg = logMSourceInfo + msg;
                        operatio.Error(msg);
                        break;
                    case LogLevel.Information:
                        operatio.Info(msg);
                        break;
                }
                
            }
        }

        public static void Info(string msg)
        {
            LogWriter(LogLevel.Information,msg);
        }

        public static void Error(string msg)
        {
            LogWriter(LogLevel.Error, msg);
        }

        public static void Debug(string msg)
        {
            LogWriter(LogLevel.Debug, msg);
        }



        public class TLog
        {
            private readonly ILogger<TLog> log;
            public TLog(ILogger<TLog> log)
            {
                this.log = log;
            }

            public  void Info(string msg)
            {
                log.LogInformation(msg);
            }

            public  void Error(string msg)
            {
                log.LogError(msg);
            }

            public  void Debug(string msg)
            {
                log.LogDebug(msg);
            }
        }
    }
}
