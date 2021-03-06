﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index2.aspx.cs" Inherits="up6.single_panel" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
	<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>up6-单面板演示页面</title>
    <script type="text/javascript" src="js/jquery-1.4.min.js"></script>
    <script type="text/javascript" src="js/json2.min.js" charset="utf-8"></script>
    <%=this.paramPage() %>
    <%= this.require( this.m_path["up6-panel"] ) %>
    <script language="javascript" type="text/javascript">
        var cbMgr = new HttpUploaderMgr();
        cbMgr.event.md5Complete = function (obj, md5) { /*alert(md5);*/ };
        cbMgr.event.fileComplete = function (obj) { /*alert(obj.fileSvr.pathSvr);*/ };
        cbMgr.event.queueComplete = function () { $(document.body).append("队列完成<br/>"); }
        cbMgr.event.addFdError = function (jv) { alert("本地路径不存在：" + jv.path); };
        cbMgr.event.scanComplete = function (obj) { /*alert(obj.folderSvr.pathLoc);*/ };
        cbMgr.Config["Cookie"] = 'ASP.NET_SessionId=<%=Session.SessionID%>';
        cbMgr.Config.Fields["uid"] = 0;

        $(function ()
        {
            cbMgr.load_to("FilePanel");
            //上传指定文件
            $("#btnUpF").click(function () {
                var path = $("#filePath").val();
                cbMgr.addFile( path );
            });
            //上传指定目录
            $("#btnUpFd").click(function () {
                var path = $("#folderPath").val();
                cbMgr.addFolder( path );
            });
        });
    </script>
</head>
<body>
    <p>up6单面板演示页面</p>
    <p>当打开页面时自动加载上次未上传完的文件列表</p>
    <p><a href="db/clear.aspx" target="_blank">清空数据库</a></p>
    <p><a href="filemgr/vue.aspx" target="_blank">文件管理器演示</a></p>
    <p><a href="index2.aspx" target="_blank">单面板演示</a></p>
    <p><a href="down2/index.htm" target="_blank">打开下载页面</a></p>
    <p><a href="index-single.htm" target="_blank">单文件上传演示</a></p>
    <p><a href="vue.aspx" target="_blank">vue演示</a></p>
    <p>
        文件路径：<input id="filePath" type="text" size="50" value="D:\\360safe-inst.exe" />&nbsp;
        <input id="btnUpF" type="button" value="上传本地文件" />
    </p>
    <p>
        目录路径：<input id="folderPath" type="text" size="50" value="C:\\Users\\Administrator\\Desktop\\test" />&nbsp;
        <input id="btnUpFd" type="button" value="上传本地目录" />
    </p>
	<div id="FilePanel"></div>
    <div id="msg"></div>
</body>
</html>