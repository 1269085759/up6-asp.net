数据库：SQL2005
编码：utf-8

---------
更新记录：
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