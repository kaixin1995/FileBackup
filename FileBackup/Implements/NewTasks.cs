using FileBackup.Interfaces;
using FileBackup.Models;
using FileBackup.Tools;
using Newtonsoft.Json;
using NLog.Fluent;

namespace FileBackup.Implements
{
    public class NewTasks: ITask
    {
        public List<TaskModel> TaskModels { get; set; }

        private readonly ILog _log;
        public NewTasks(ILog log)
        {
            this._log = log;
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        public void Run()
        {
            _log.Info("备份服务已经启动");
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    if (TaskModels != null && TaskModels.Any())
                    {
                        foreach (var taskModel in TaskModels.Where(p => p.Hour == Convert.ToInt32(DateTime.Now.ToString("HH")) && p.Minute == Convert.ToInt32(DateTime.Now.ToString("mm"))))
                        {
                            if (DateTime.Now.Second == 3)
                            {
                                PerformATask(taskModel);
                            }
                        }
                    }

                }
            });

        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="taskModel"></param>
        /// <returns></returns>
        private async Task PerformATask(TaskModel taskModel)
        {
            ZipHelper zip = new ZipHelper(_log);
            FTPHelper ftp = new FTPHelper(_log);
            string path = string.Empty;
            try
            {
                if (taskModel.IsFile)
                {
                    path = zip.FileCompression(taskModel.Name, taskModel.Path);
                }
                else
                {
                    path = zip.DirCompression(taskModel.Name, taskModel.Path);
                }
                _log.Info($"执行任务中：{ToJson(taskModel)}");
                ftp.UploadFile(path, $"FTPData/{taskModel.Name}");
                _log.Info($"删除文件：{path}");
                File.Delete(path);

                //此处加一个删除过往备份
                ftp.DelTheBackup(taskModel.Name, taskModel.Count);
            }
            catch (Exception ex)
            {
                File.Delete(path);
                _log.Error($"执行任务:{ex.Message}");
            }
        }


        /// <summary>
        /// 查看单个任务
        /// </summary>
        /// <param name="taskModel"></param>
        /// <returns></returns>
        public string ViewTheTask(TaskModel taskModel)
        {
            try
            {
                string Content = $"ID:{taskModel.ID}\n任务名:{taskModel.Name}\n保存最大数:{taskModel.Count}\n已经执行次数：{taskModel.TheNumOf}\n执行时间{taskModel.Hour}:{taskModel.Minute}\n\n";
                Console.WriteLine(Content);
                return Content;
            }
            catch (Exception ex)
            {
                _log.Error($"查看单个任务:{ex.Message}");
                return "";
            }
        }


        /// <summary>
        /// 转换json文本
        /// </summary>
        /// <param name="taskModel"></param>
        /// <returns></returns>
        public string ToJson(TaskModel taskModel)
        {
            try
            {
                string jsonStr = JsonConvert.SerializeObject(taskModel);
                return jsonStr;
            }
            catch (Exception ex)
            {
                _log.Error($"转换json文本:{ex.Message}");
                return "";
            }

        }


        /// <summary>
        /// 查看总任务
        /// </summary>
        /// <returns></returns>
        public string ViewTheTask()
        {
            try
            {
                string Content = string.Empty;
                if (TaskModels != null && TaskModels.Any())
                {
                    foreach (var item in TaskModels)
                    {
                        Content += ViewTheTask(item);
                    }
                }
                return Content;
            }
            catch (Exception ex)
            {
                _log.Error($"查看总任务:{ex.Message}");
                return "";
            }
        }
    }
}
