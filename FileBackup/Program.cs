using FileBackup.Implements;
using FileBackup.Interfaces;
using FileBackup.Tools;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

ServiceCollection services = new ServiceCollection();

services.AddLogging(logBuilder =>
 {
     logBuilder.AddConsole();
     logBuilder.SetMinimumLevel(LogLevel.Information);
     logBuilder.AddNLog();
 });

//日志
services.AddSingleton<ILog, TLog>();


//注入任务类
services.AddSingleton<ITask, NewTasks>();

//总操作
services.AddSingleton<IOperationData, OperationData>();


using (ServiceProvider serviceProvider = services.BuildServiceProvider())
{
    //不存在返回为null
    ITask itaks = serviceProvider.GetService<ITask>();
    
    //不存在直接抛出异常
    var operationData = serviceProvider.GetRequiredService<IOperationData>();

    var Log = serviceProvider.GetService<ILog>();


    operationData.InitThe();
    itaks.Run();

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
        string value = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(value))
        {
            Console.WriteLine("输入值错误，请按照序号进行正确输入！\n");
            continue;

        }
        Console.WriteLine();
        switch (value.Trim())
        {
            case "1":
                itaks.ViewTheTask();
                break;
            case "2":
                operationData.Add();
                break;
            case "3":
                Console.WriteLine("懒得写修改了，删除后重加把~  就这样了~");
                break;
            case "4":
                operationData.Delete();
                break;
            case "5":
                operationData.Empty();
                break;
            case "6":
                new FTPHelper(Log).UpdateConfig();
                break;
            case "7":
                return;
        }

    }

}