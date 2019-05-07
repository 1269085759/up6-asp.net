<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="single-panel.aspx.cs" Inherits="up6.single_panel" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
	<meta http-equiv="Content-Type" content="text/html; charset=gb2312" />
    <title>up6.2-SQL2005��ʾҳ��</title>
    <link href="js/single-panel/up6.css" type="text/css" rel="Stylesheet" charset="gb2312"/>
    <script type="text/javascript" src="js/jquery-1.4.min.js"></script>
    <script type="text/javascript" src="js/json2.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="js/up6.config.js" charset="utf-8"></script>
    <script type="text/javascript" src="js/up6.app.js" charset="utf-8"></script>
    <script type="text/javascript" src="js/up6.edge.js" charset="utf-8"></script>
    <script type="text/javascript" src="js/single-panel/up6.file.js" charset="utf-8"></script>
    <script type="text/javascript" src="js/single-panel/up6.folder.js" charset="utf-8"></script>
    <script type="text/javascript" src="js/single-panel/up6.js" charset="utf-8"></script>
    <script language="javascript" type="text/javascript">
        var cbMgr = new HttpUploaderMgr();
        cbMgr.event.md5Complete = function (obj, md5) { /*alert(md5);*/ };
        cbMgr.event.fileComplete = function (obj) { /*alert(obj.fileSvr.pathSvr);*/ };
        cbMgr.event.queueComplete = function () { $(document.body).append("�������<br/>"); }
        cbMgr.event.addFdError = function (jv) { alert("����·�������ڣ�" + jv.path); };
        cbMgr.event.scanComplete = function (obj) { /*alert(obj.folderSvr.pathLoc);*/ };
        cbMgr.Config["Cookie"] = 'ASP.NET_SessionId=<%=Session.SessionID%>';
        cbMgr.Config.Fields["uid"] = 0;

        $(function ()
        {
            cbMgr.load_to("FilePanel");
            //�ϴ�ָ���ļ�
            $("#btnUpF").click(function () {
                var path = $("#filePath").val();
                cbMgr.app.addFile({ pathLoc: path });
            });
            //�ϴ�ָ��Ŀ¼
            $("#btnUpFd").click(function () {
                var path = $("#folderPath").val();
                cbMgr.app.addFolder({ pathLoc: path });
            });
        });
    </script>
</head>
<body>
    <p>up6�������ʾҳ��</p>
    <p>����ҳ��ʱ�Զ������ϴ�δ�ϴ�����ļ��б�</p>
    <p><a href="db/clear.aspx" target="_blank">������ݿ�</a></p>
    <p><a href="down2/index.htm" target="_blank">������ҳ��</a></p>
    <p><a href="index-single.htm" target="_blank">���ļ��ϴ���ʾ</a></p>
    <p>
        �ļ�·����<input id="filePath" type="text" size="50" value="D:\\360safe-inst.exe" />&nbsp;
        <input id="btnUpF" type="button" value="�ϴ������ļ�" />
    </p>
    <p>
        Ŀ¼·����<input id="folderPath" type="text" size="50" value="C:\\Users\\Administrator\\Desktop\\test" />&nbsp;
        <input id="btnUpFd" type="button" value="�ϴ�����Ŀ¼" />
    </p>
	<div id="FilePanel"></div>
    <div id="msg"></div>
</body>
</html>