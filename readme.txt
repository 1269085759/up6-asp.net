数据库：SQL2005
编码：utf-8

NuGet安装依赖
Install-Package Newtonsoft.Json
Install-Package Microsoft.Experimental.IO

---------
更新记录：
2019-04-27
	增加错误码
	  up6.js
	  12,13,14,15
	删除文件夹后清除本地路径
	新增删除文件对象方法，从队列中取消文件时调用。
	  up6.js
	    del_file
	  up6.folder.js
	    remove方法更新，

2019-04-26
	1.添加文件，添加文件夹时检查防止重复添加
		up6.js
		addFileLoc
		addFolderLoc
	2.文件夹增加自动续传功能
		up6.js
		Config.AutoConnect
	3.增加对长路径的处理。
		FileBlockWriter.make
		新增PathTool.cs
	4.新增服务器错误标识
	  11