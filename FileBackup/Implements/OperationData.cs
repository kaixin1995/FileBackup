using FileBackup.Interfaces;
using FileBackup.Models;
using FileBackup.Tools;
using Newtonsoft.Json;

namespace FileBackup.Implements
{
    public class OperationData: IOperationData
    {

        private readonly ITask _newTasks;
        private readonly ILog _log;
        public OperationData(ITask _newTasks,ILog log)
        { 
            this._newTasks = _newTasks;
            this._log = log;
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
        /// 配置更新时将会变更此方法
        /// </summary>
        public void Update()
        {
            try
            {
                string jsonStr = JsonConvert.SerializeObject(_newTasks.TaskModels);
                //写入
                StreamWriter writer = new StreamWriter(DataPath);
                writer.Write(jsonStr);
                writer.Close();
                writer.Dispose();
            }
            catch (Exception ex)
            {
                _log.Error($"配置更新时将会变更此方法:{ex.Message}");
            }
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Empty()
        {
            _newTasks.TaskModels = new List<TaskModel>();
            Update();
            _log.Info($"清空任务");
        }


        /// <summary>
        /// 初始化配置
        /// </summary>
        public void InitThe()
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
                _newTasks.TaskModels = JsonConvert.DeserializeObject<List<TaskModel>>(content);
                _newTasks.TaskModels.OrderBy(d => d.ID);
            }
            catch (Exception ex)
            {
                _log.Error($"初始化配置:{ex.Message}");
            }
        }


        /// <summary>
        /// 删除任务
        /// </summary>
        /// <returns></returns>
        public bool Delete()
        {
            try
            {
                _newTasks.ViewTheTask();
                Console.WriteLine("请输入您要删除的任务ID");
                string id = Console.ReadLine();
                if (!int.TryParse(id, out int i))
                {
                    return false;
                }
                TaskModel taskModel = _newTasks.TaskModels.FirstOrDefault(p => p.ID == Convert.ToInt32(id));
                _newTasks.TaskModels.Remove(taskModel);
                Update();
                _log.Info($"删除任务成功:{_newTasks.ToJson(taskModel)}");
                return true;
            }
            catch (Exception ex)
            {
                _log.Error($"删除任务:{ex.Message}");
                return false;
            }

        }

        /// <summary>
        /// 增加任务
        /// </summary>
        /// <returns></returns>
        public  bool Add()
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

                if (string.IsNullOrWhiteSpace(Name) || Count == 0 || string.IsNullOrWhiteSpace(Path))
                {
                    return false;
                }

                if (!File.Exists(Path))
                {
                    return false;
                }

                int _count = 0;
                if (_newTasks.TaskModels != null && _newTasks.TaskModels.Any())
                {
                    _count = _newTasks.TaskModels.LastOrDefault().ID + 1;
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
                _newTasks.TaskModels.Add(taskModel);
                Update();
                _log.Info($"增加任务成功:{_newTasks.ToJson(taskModel)}");

                return true;
            }
            catch (Exception ex)
            {
                _log.Error($"增加任务:{ex.Message}");
                return false;
            }
        }


    }
}
