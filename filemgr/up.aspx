<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="up.aspx.cs" Inherits="up6.filemgr.up" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <%= this.require(
              this.m_path["jquery"]
              , this.m_path["layerui"]
              , this.m_path["moment"]
              , this.m_path["up6"]
              ) %>
</head>
<body>
        <div id="up6-panel">
        </div>
    <script>

        function ent_post_complete() { }
        
    var app = new HttpUploaderMgr();
    app.event.md5Complete = function (obj, md5) { /*alert(md5);*/ };
    app.event.fileComplete = function (obj) { ent_post_complete() };
    app.event.queueComplete = function () { }
    app.event.addFdError = function (jv) { alert("本地路径不存在：" + jv.path); };
    app.event.scanComplete = function (obj) { /*alert(obj.folderSvr.pathLoc);*/ };
    app.Config["Cookie"] = 'ASP.NET_SessionId=<%=Session.SessionID%>';
    app.Config.Fields["uid"] = 0;

    app.load_to("up6-panel");
    </script>
</body>
</html>
