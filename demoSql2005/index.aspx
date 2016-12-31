<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="up6.demoSql2005.index" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
	<meta http-equiv="Content-Type" content="text/html; charset=gb2312" />
    <title>up6.2-SQL2005演示页面</title>
    <link href="js/up6.css" type="text/css" rel="Stylesheet" charset="gb2312"/>
    <script type="text/javascript" src="js/jquery-1.4.min.js"></script>
    <script type="text/javascript" src="js/json2.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="js/up6.config.js" charset="utf-8"></script>
    <script type="text/javascript" src="js/up6.file.js" charset="utf-8"></script>
    <script type="text/javascript" src="js/up6.folder.js" charset="utf-8"></script>
    <script type="text/javascript" src="js/up6.js" charset="utf-8"></script>
    <script language="javascript" type="text/javascript">
        var cbMgr = new HttpUploaderMgr();
        cbMgr.event.md5Complete = function (obj, md5) { /*alert(md5);*/ };
        cbMgr.event.fileComplete = function (obj) { /*alert(obj.pathSvr);*/ };
        cbMgr.event.queueComplete = function () { console.log("队列完成"); }
        cbMgr.Config["Cookie"] = 'ASP.NET_SessionId=<%=Session.SessionID%>';
        //使用不同项目配置
        //cbMgr.set_config(up6_config.qq);
        //cbMgr.set_config(up6_config.qq_mail);
        //cbMgr.set_config(up6_config.qq_zone);
        //cbMgr.set_config(up6_config.erp);
        //cbMgr.set_config(up6_config.oa);
        //cbMgr.set_config(up6_config.share_point);
        //cbMgr.set_config(up6_config.vm);

        $(document).ready(function ()
        {
            cbMgr.load_to("FilePanel");
        });
    </script>
</head>
<body>
    <p>up6.2多文件上传演示页面</p>
    <p><a href="db/clear.aspx" target="_blank">清空数据库</a></p>
    <p><a href="down2/index.htm" target="_blank">打开下载页面</a></p>
	<div id="FilePanel"></div>
    <div id="msg"></div>
</body>
</html>

