using FileBackup.Models;
using Newtonsoft.Json;

namespace FileBackup.Tools
{
    public static class NewTasks
    {

        static List<TaskModel> _taskModels=new List<TaskModel>();

        static NewTasks()
        {
            InitThe();
            LogHelper.Info($"初始化任务");
        }

        /// <summary>
        /// 配置更新时将会变更此方法
        /// </summary>
        public static void Update()
        {
            try
            {
                string jsonStr = JsonConvert.SerializeObject(_taskModels);
                //写入
                StreamWriter writer = new StreamWriter(DataPath);
                writer.Write(jsonStr);
                writer.Close();
                writer.Dispose();
            }
            catch (Exception ex)
            {
                LogHelper.Error($"配置更新时将会变更此方法:{ex.Message}");
            }
        }

        /// <summary>
        /// 清空
        /// </summary>
        public static void Empty()
        {
            _taskModels = new List<TaskModel>();
            Update();
            LogHelper.Info($"清空任务");
        }




        /// <summary>
        /// 执行任务
        /// </summary>
        public static void Run()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    if (_taskModels != null && _taskModels.Any())
                    {
                        foreach (var taskModel in _taskModels.Where(p=>p.Hour==Convert.ToInt32(DateTime.Now.ToString("HH"))&&p.Minute== Convert.ToInt32(DateTime.Now.ToString("mm"))))
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
        private static async Task PerformATask(TaskModel taskModel)
        {
            string path = string.Empty;
            try
            {
                if (taskModel.IsFile)
                {
                    path = ZipHelper.FileCompression(taskModel.Name, taskModel.Path);
                }
                else
                {
                    path = ZipHelper.DirCompression(taskModel.Name, taskModel.Path);
                }
                LogHelper.Info($"执行任务中：{ToJson(taskModel)}");
                FTPHelper.UploadFile(path, $"FTPData/{taskModel.Name}");
                LogHelper.Info($"删除文件：{path}");
                File.Delete(path);

                //此处加一个删除过往备份
                FTPHelper.DelTheBackup(taskModel.Name, taskModel.Count);
            }
            catch (Exception ex)
            {
                File.Delete(path);
                LogHelper.Error($"执行任务:{ex.Message}");
            }
        }


        /// <summary>
        /// json文件地址
        /// </summary>
        private static string DataPath
        {
            get
            {
                return $"{UniversalTool.RunDir}Data.json";
            }
        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        public static void InitThe()
        {
            try
            {
                if (!File.Exists(DataPath))
                {
                    //创建文件后立马释放文件
                    File.Create(DataPath).Close();
                    return;
                }
                StreamReader sr = new StreamReader(DataPath);
                string content = sr.ReadToEnd();
                sr.Close();
                sr.Dispose();
                if (string.IsNullOrWhiteSpace(content))
                {
                    return;
                }
                _taskModels = JsonConvert.DeserializeObject<List<TaskModel>>(content);
                _taskModels.OrderBy(d => d.ID);
            }
            catch (Exception ex)
            {
                LogHelper.Error($"初始化配置:{ex.Message}");
            }
        }



        /// <summary>
        /// 删除任务
        /// </summary>
        /// <returns></returns>
        public static bool Delete()
        {
            try
            {
                ViewTheTask();
                Console.WriteLine("请输入您要删除的任务ID");
                string id = Console.ReadLine();
                if (!int.TryParse(id, out int i))
                {
                    return false;
                }
                TaskModel taskModel = _taskModels.FirstOrDefault(p => p.ID == Convert.ToInt32(id));
                _taskModels.Remove(taskModel);
                Update();
                LogHelper.Info($"删除任务成功:{ToJson(taskModel)}");
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error($"删除任务:{ex.Message}");
                return false;
            }

        }


        /// <summary>
        /// 增加任务
        /// </summary>
        /// <returns></returns>
        public static bool Add()
        {

            try
            {
                Console.WriteLine("请输入任务名：");
                string Name = Console.ReadLine();

                Console.WriteLine("请输入备份最大保存数：(超过时间将会自动删除过期备份文件)");
                string _tempcount = Console.ReadLine();
                int Count = 0;
                if (int.TryParse(_tempcount, out int i))
                {
                    Count = int.Parse(_tempcount);
                }


                Console.WriteLine("请输入备份时间：(格式为时:分，例如12:15，格式为24小时制)");

                string[] Times = Console.ReadLine().Split(new string[] { ":", "：" }, StringSplitOptions.RemoveEmptyEntries); ;
                int Hour = int.Parse(Times[0]);


                int Minute = int.Parse(Times[1]);

                Console.WriteLine("您备份的任务是否是文件？输入0则是文件，输入1则是文件夹：");
                string _isFile = Console.ReadLine();
                bool IsFile = false;
                if (_isFile == "0")
                {
                    IsFile = true;
                }
                else if (_isFile == "1")
                {
                    IsFile = false;
                }

                Console.WriteLine("您将要备份的文件路径是:(请输入绝对路径)");
                string Path = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(Name) || Count == 0||string.IsNullOrWhiteSpace(Path))
                {
                    return false;
                }

                if (!File.Exists(Path))
                {
                    return false;
                }

                int _count = 0;
                if (_taskModels != null && _taskModels.Any())
                {
                    _count = _taskModels.LastOrDefault().ID + 1;
                }

                TaskModel taskModel = new TaskModel
                {
                    Name = Name,
                    Count = Count,
                    Hour = Hour,
                    Minute = Minute,
                    BackupType = 1,
                    TheNumOf = 0,
                    ID = _count,
                    IsFile = IsFile,
                    Path = Path
                };
                _taskModels.Add(taskModel);
                Update();
                LogHelper.Info($"增加任务成功:{ToJson(taskModel)}");

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error($"增加任务:{ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// 查看单个任务
        /// </summary>
        /// <param name="taskModel"></param>
        /// <returns></returns>
        public static string ViewTheTask(TaskModel taskModel)
        {
            try
            {
                string Content = $"ID:{taskModel.ID}\n任务名:{taskModel.Name}\n保存最大数:{taskModel.Count}\n已经执行次数：{taskModel.TheNumOf}\n执行时间{taskModel.Hour}:{taskModel.Minute}\n\n";
                Console.WriteLine(Content);
                return Content;
            }
            catch (Exception ex)
            {
                LogHelper.Error($"查看单个任务:{ex.Message}");
                return "";
            }
        }


        /// <summary>
        /// 转换json文本
        /// </summary>
        /// <param name="taskModel"></param>
        /// <returns></returns>
        public static string ToJson(TaskModel taskModel)
        {
            try
            {
                string jsonStr = JsonConvert.SerializeObject(taskModel);
                return jsonStr;
            }
            catch (Exception ex)
            {
                LogHelper.Error($"转换json文本:{ex.Message}");
                return "";
            }

        }


        /// <summary>
        /// 查看总任务
        /// </summary>
        /// <returns></returns>
        public static string ViewTheTask()
        {
            try
            {
                string Content = string.Empty;
                if (_taskModels != null && _taskModels.Any())
                {
                    foreach (var item in _taskModels)
                    {
                        Content += ViewTheTask(item);
                    }
                }
                return Content;
            }
            catch (Exception ex)
            {
                LogHelper.Error($"查看总任务:{ex.Message}");
                return "";
            }
        }
    }
}
