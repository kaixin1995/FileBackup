using FileBackup.Interfaces;
using FileBackup.Models;
using FluentFTP;
using Newtonsoft.Json;
using System.Text;

namespace FileBackup.Tools
{
    public class FTPHelper
    {
        public FtpClient _ftpClient {get;set;}

        private readonly ILog _log;
        public FTPHelper(ILog _log)
        {
            this._log = _log;
            InitThe();
        }


        public void InitThe()
        {
            StreamReader sr = new StreamReader($"{UniversalTool.RunDir}Config.json");
            string content = sr.ReadToEnd();
            sr.Close();
            sr.Dispose();
            ConfigModel configModel = JsonConvert.DeserializeObject<ConfigModel>(content);

            //读取配置中的账户进行填充
            /*
              第一个参数是FTP地址，注意要加协议名
              第二个参数是端口，默认21
              第三个参数是FTP用户名
              第四个参数是FTP密码
              Encoding是指定编码
            */
            _ftpClient = new FtpClient($"ftp://{configModel.Host}", configModel.UserName, configModel.PassWord, configModel.Port)
            {
                Encoding = Encoding.UTF8
            };
        }

        /// <summary>
        /// 更新FTP的配置文件
        /// </summary>
        public void UpdateConfig()
        {
            Console.Write("请输入FTP的IP：");
            string ip=Console.ReadLine();

            //Console.WriteLine();

            Console.Write("请输入FTP的端口：");
            string port = Console.ReadLine();

            while (!int.TryParse(port, out int i))
            {
                Console.WriteLine("端口输入错误，请输入正确的端口！");
                Console.Write("请输入FTP的端口：");
                port = Console.ReadLine();
            }

            //Console.WriteLine();

            Console.Write("请输入FTP的账号：");
            string user = Console.ReadLine();

            //Console.WriteLine();

            Console.Write("请输入FTP的密码：");
            string pswd = Console.ReadLine();

            ConfigModel configModel = new ConfigModel {Host=ip,Port=int.Parse(port),UserName=user,PassWord=pswd };

            try
            {
                string jsonStr = JsonConvert.SerializeObject(configModel);
                //写入
                StreamWriter writer = new StreamWriter($"{UniversalTool.RunDir}Config.json");
                writer.Write(jsonStr);
                writer.Close();
                writer.Dispose();
            }
            catch (Exception ex)
            {
                _log.Error($"配置更新时将会变更此方法:{ex.Message}");
            }
            _log.Info($"FTP配置更新成功！");
            InitThe();
        }


        /// <summary>
        /// 登录
        /// </summary>
        public void Login()
        {
            try
            {
                //IsConnected是判断client是否与远程服务建立了连接
                if (!_ftpClient.IsConnected)
                {
                    //发起连接登录
                    _ftpClient.Connect();
                    //启用UTF8传输
                    var result = _ftpClient.Execute("OPTS UTF8 ON");
                    if (!result.Code.Equals("200") && !result.Code.Equals("202"))
                    {
                        _ftpClient.Encoding = Encoding.GetEncoding("ISO-8859-1");
                    }

                    _log.Info($"FTP登录成功");

                }
            }
            catch (Exception ex)
            {
                _log.Error($"FTP登录:{ex.Message}");
            }
        }

        /// <summary>
        /// 上传单个文件
        /// </summary>
        /// <param name="sourcePath">文件源路径</param>
        /// <param name="destPath">上传到指定的ftp文件夹路径</param>
        public void UploadFile(string sourcePath, string destPath)
        {
            try
            {
                Login();
                if (!File.Exists(sourcePath))
                    return;
                var fileInfo = new FileInfo(sourcePath);
                //LogHelper.Info($"destPath:{destPath}       fileInfo.Name:{fileInfo.Name}");
                _ftpClient.UploadFile(sourcePath, $"{destPath}/{fileInfo.Name}", createRemoteDir: true);
                _log.Info($"FTP:{sourcePath}上传到{destPath}/{fileInfo.Name}");
            }
            catch (Exception ex)
            {
                _log.Error($"FTP上传单个文件:{ex.Message}");
            }
        }


        /// <summary>
        /// 删除超时备份
        /// </summary>
        /// <param name="name"></param>
        /// <param name="count"></param>
        public void DelTheBackup(string name,int count)
        {
            Login();
            using (var conn = _ftpClient)
            {
                conn.Connect();

                //不存在该目录就退出
                if (!conn.DirectoryExists($"\\FTPData\\{name}"))
                {
                    return;
                }
                foreach (var s in conn.GetNameListing($"\\FTPData\\{name}"))
                {
                    // load some information about the object
                    // returned from the listing...
                    var isDirectory = conn.DirectoryExists(s);
                    var modify = conn.GetModifiedTime(s);
                    var size = isDirectory ? 0 : conn.GetFileSize(s);
                    if (UniversalTool.DateDiff(modify, DateTime.Now) > count)
                    {
                        conn.DeleteFile(s);
                        _log.Info($"FTP删除文件:{s}");
                    }
                }
            }
        }

    }
}

