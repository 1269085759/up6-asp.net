<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index-single.aspx.cs" Inherits="up6.index_single" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
	<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>up6单文件上传样式演示页面</title>
    <%=this.paramPage() %>
    <script type="text/javascript" src="js/jquery-1.4.min.js"></script>
    <script type="text/javascript" src="js/json2.min.js" charset="utf-8"></script>
    <%= this.require( this.m_path["up6-single"] ) %>
    <script language="javascript" type="text/javascript">
        var fileMgr = new HttpUploaderMgr();
        fileMgr.event.md5Complete = function (obj, md5) { /*alert(md5);*/ };
        fileMgr.event.fileComplete = function (obj) { /*alert(obj.fileSvr.pathSvr);*/ };

    	$(function()
    	{
    	    fileMgr.loadTo("upPnl");

    		$("#btnSel").click(function()
            {
    		    fileMgr.postAuto(); //
    		});

    		$("#btnPostLoc").click(function ()
    		{
    		    fileMgr.postLoc("D:\\Soft\\QQ2015.exe"); //
    		});

    	});
    </script>
</head>
<body>
    <p>up6此页面演示单个文件上传样式</p>
    <p><a href="db/clear.aspx" target="_blank">清空数据库</a></p>
    <div id="upPnl"></div>
    <a id="btnSetup">安装控件</a>
	<input id="btnSel" type="button" value="浏览" />
    <input id="btnPostLoc" type="button" value="上传本地文件" />
</body>
</html>