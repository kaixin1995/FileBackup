using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileBackup.Tools
{
    public static class UniversalTool
    {

        /// <summary>
        /// 获取运行目录，为了兼容
        /// </summary>
        public static string RunDir
        {
            get
            {
                string thisFile = AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/");
                if (!thisFile.EndsWith("/"))
                {
                    thisFile += "/";
                }
                return thisFile;
            }
        }


        /// <summary>
        /// 两个时间相差的天数
        /// </summary>
        /// <param name="dateStart"></param>
        /// <param name="dateEnd"></param>
        /// <returns></returns>
        public static int DateDiff(DateTime dateStart, DateTime dateEnd)
        {
            DateTime start = Convert.ToDateTime(dateStart.ToShortDateString());
            DateTime end = Convert.ToDateTime(dateEnd.ToShortDateString());
            TimeSpan sp = end.Subtract(start);
            return sp.Days;
        }


    }
}
