# 文件备份系统

- 主要用来将文件、或者目录打包后上传到指定地址进行备份，目前只实现了FTP备份，后期会考虑oss、微软网盘等。  目前来说FTP备份对我来说已经足够了。

- 目前只能按照每天来备份，你可以自定义每天的几点几分进行文件的备份，时间格式为24小时制。

- 可以设置最大备份数，超过该天数的文件会被自动删除，请注意，是按照你上传的文件夹中的文件来计算，而这个文件夹名则是你的任务名，所以最后不要在该文件夹内存放其他不必要的文件，防止数据被误删。

- 程序使用.net6进行开发，不需要安装环境，无论windows还是linux下都可以直接运行(当然是我打包好的带独立环境的生成包)。

#### 界面

- ![img1.png](https://img.haokaikai.cn/2022/10/24/312f949dd0981.png)

- ![img2.png](https://img.haokaikai.cn/2022/10/24/1234e338e4586.png)

- ![img3.png](https://img.haokaikai.cn/2022/10/24/56df4f1ab061d.png)

- linux下同样可以使用，注意linux下使用screen，而不要使用nohup，否则会报找不到路径的错误。

- ![img4.jpg](https://img.haokaikai.cn/2022/10/24/c48bdeefe80ea.jpg)

# 

开源地址：

https://github.com/kaixin1995/FileBackup

下载地址奉上：

https://wwt.lanzoul.com/b09zoc5cd

密码:c0i2