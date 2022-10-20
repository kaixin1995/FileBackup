using FileBackup.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBackup.Interfaces
{
    public interface ITask
    {
        List<TaskModel> TaskModels { get; set; }

        /// <summary>
        /// 执行任务
        /// </summary>
        public void Run();


        /// <summary>
        /// 查看单个任务
        /// </summary>
        /// <param name="taskModel"></param>
        /// <returns></returns>
        public string ViewTheTask(TaskModel taskModel);


        /// <summary>
        /// 转换json文本
        /// </summary>
        /// <param name="taskModel"></param>
        /// <returns></returns>
        public string ToJson(TaskModel taskModel);


        /// <summary>
        /// 查看总任务
        /// </summary>
        /// <returns></returns>
        public string ViewTheTask();


    }
}
