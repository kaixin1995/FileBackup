using FileBackup.Implements;
using FileBackup.Interfaces;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System.IO;
using System.Xml.Linq;

namespace FileBackup.Tools
{
    /// <summary>
    /// 压缩帮助类
    /// </summary>
    public class ZipHelper
    {

        public ZipHelper(ILog _log)
        {
            this._log = _log;
            CreateAFolder();
        }

        private readonly ILog _log;


        /// <summary>
        /// 多文件压缩备份
        /// </summary>
        /// <param name="name"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public string MulFileCompression(string name,List<string> files)
        {
            try
            {
                string _filepath = $@"{UniversalTool.RunDir}Backup/{name}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.zip";
                using (ZipFile zip = ZipFile.Create(_filepath))
                {
                    zip.BeginUpdate();
                    files.ForEach(file =>
                    {
                        zip.Add(file);
                    });
                    zip.CommitUpdate();
                }
                return _filepath;
            }
            catch (Exception ex)
            {
                _log.Error($"多文件压缩备份:{ex.Message}");
                return "";
            }
        }


        /// <summary>
        /// 目录压缩
        /// </summary>
        /// <param name="name">压缩目录名</param>
        /// <param name="filepath">被压缩的目录</param>
        /// <returns></returns>
        public string DirCompression(string name, string path)
        {
            try
            {
                string _filepath = $@"{UniversalTool.RunDir}Backup/{name}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.zip";
                (new FastZip()).CreateZip(_filepath, path, true, "");
                return _filepath;
            }
            catch (Exception ex)
            {
                _log.Error($"目录压缩:{ex.Message}");
                return "";
            }
        }

        /// <summary>
        /// 创建备份文件夹
        /// </summary>
        private static void CreateAFolder()
        {
            if (!Directory.Exists($@"{UniversalTool.RunDir}Backup"))
            {
                Directory.CreateDirectory($@"{UniversalTool.RunDir}Backup");
            }
        }

        /// <summary>
        /// 文件压缩
        /// </summary>
        /// <param name="name"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public string FileCompression(string name, string filepath)
        {
            try
            {
                string _filepath = $@"{UniversalTool.RunDir}Backup/{name}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.zip";
                
                using (ZipFile zip = ZipFile.Create(_filepath))
                {
                    zip.BeginUpdate();
                    zip.Add(filepath);
                    zip.CommitUpdate();
                }
                _log.Info($"文件压缩完成，地址为:{_filepath}");
                return _filepath;
            }
            catch (Exception ex)
            {
                _log.Error($"文件压缩:{ex.Message}");
                return "";
            }
        }


        /// <summary>
        /// 给压缩包中增加文件
        /// </summary>
        /// <param name="zippath"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public bool ZipAddFile(string zippath,List<string> files)
        {
            try
            {
                using (ZipFile zip = new ZipFile(zippath))
                {
                    zip.BeginUpdate();
                    files.ForEach(file =>
                    {
                        zip.Add(file);
                    });
                    zip.CommitUpdate();
                }
                return true;
            }
            catch (Exception ex)
            {
                _log.Error($"给压缩包中增加文件:{ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// 解压压缩包到指定目录
        /// </summary>
        /// <param name="zippath"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public bool UnzipTheFiles(string zippath, string filepath)
        {
            try
            {
                (new FastZip()).ExtractZip(zippath, filepath, "");
                return true;
            }
            catch (Exception ex)
            {
                _log.Error($"解压压缩包到指定目录:{ex.Message}");
                return false;
            }
        }
    }
}
