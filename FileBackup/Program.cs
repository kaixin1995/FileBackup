// See https://aka.ms/new-console-template for more information
using System.Reflection;
using FileBackup.Models;
using FileBackup.Tools;
using ICSharpCode.SharpZipLib.Zip;

NewTasks.Run();
LogHelper.Info("备份服务已经启动");


//FTPHelper.DelTheBackup("webBck",0);
//return;

//using (ZipFile zip = ZipFile.Create(@"E:\test.zip"))
//{
//    zip.BeginUpdate();
//    zip.Add(@"E:\语言文档.txt");
//    zip.CommitUpdate();
//}


//string path = ZipHelper.FileCompression("webBck", "/data/www/PrivateWebsite/usr/5cf8735ac0cc7.db");

//FTPHelper.UploadFile(path, "FTPData/webBck");
//File.Delete(path);
//return;

while (true)
{
    Console.WriteLine("###############软件备份程序###############");
    Console.WriteLine("1.查看全部任务");
    Console.WriteLine("2.增加任务");
    Console.WriteLine("3.编辑任务");
    Console.WriteLine("4.删除任务");
    Console.WriteLine("5.清空全部任务");
    Console.WriteLine("6.FTP上传配置");
    Console.WriteLine("7.退出");
    Console.WriteLine("##########################################");
    string value=Console.ReadLine();
    if (string.IsNullOrWhiteSpace(value))
    {
        Console.WriteLine("输入值错误，请按照序号进行正确输入！\n");
        continue;

    }
    Console.WriteLine();
    switch (value.Trim())
    {
        case "1":
            NewTasks.ViewTheTask();
            break;
        case "2":
            NewTasks.Add();
            break;
        case "3":
            Console.WriteLine("懒得写修改了，删除后重加把~  就这样了~");
            break;
        case "4":
            NewTasks.Delete();
            break;
        case "5":
            NewTasks.Empty();
            break;
        case "6":
            FTPHelper.UpdateConfig();
            break;
        case "7":
            return;
    }
    
}
