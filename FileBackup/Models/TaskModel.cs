using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBackup.Models
{
    public class TaskModel
    {

        /// <summary>
        /// 任务名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }


        /// <summary>
        /// 是否是文件，反之则是文件夹
        /// </summary>
        public bool IsFile { get; set; }


        /// <summary>
        /// 总次数
        /// </summary>
        public int Count { get; set; }


        /// <summary>
        /// 运行次数
        /// </summary>
        public int TheNumOf { get; set; }

        /// <summary>
        /// 时
        /// </summary>
        public int Hour { get; set; }


        /// <summary>
        /// 分
        /// </summary>
        public int Minute { get; set; }


        /// <summary>
        /// 备份类型 0默认是另一个目录，1则是FTP
        /// </summary>
        public int BackupType { get; set; } = 1;



        public int ID { get; set; }

    }
}
