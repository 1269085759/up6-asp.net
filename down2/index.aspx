<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="up6.down2.index" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>down2-演示页面</title>
    <%=this.paramPage() %>
    <script type="text/javascript" src="js/json2.min.js"></script>
    <script type="text/javascript" src="js/jquery-1.4.min.js"></script>
    <%= this.require( this.m_path["down2"] ) %>
    <script language="javascript" type="text/javascript">
        var downer = new DownloaderMgr();
        downer.Config["Folder"] = "";
        downer.event.ready = function () {
            load_files();
        };
        var svrFiles = new Object();

        function load_files()
        {
            $.ajax({
                type: "GET"
                , dataType: 'jsonp'
                , jsonp: "callback" //自定义的jsonp回调函数名称，默认为jQuery自动生成的随机函数名
                , url: downer.Config["UrlListCmp"]
                , data: { uid: downer.Config.Fields["uid"], time: new Date().getTime() }
                , success: function (msg)
                {
                    if (msg.value == null)
                    {
                        $("#msg_load").hide();
                        return;
                    }

                    var files = JSON.parse( decodeURIComponent(msg.value) );
                    var tb = $("#tbCmp");
                    tb.find('input[name="btnSelAll"]').click(function () {
                        var ck = $(this).attr("checked");
                        $("input[name='cbSel']").each(function (i,n) {
                            $(n).attr("checked", ck);
                        });
                    });

                    $.each(files, function (i, item)
                    {
                        var tmp = $("#tbHead").clone();
                        var tdSel = tmp.find('td[name="sel"]').html('<input type="checkbox" name="cbSel" />');
                        tdSel.find("input").attr("fid", item.id);
                        var tdType = tmp.find('td[name="type"]');
                        var tdName = tmp.find('td[name="name"]');
                        var tdSize = tmp.find('td[name="size"]');
                        var tdOp = tmp.find('td[name="op"]');
                        var f = item;
                        f.fileUrl = downer.Config.UrlDown;
                        if (f.fdTask ) { tdType.text("文件夹"); }
                        else { tdType.text("文件"); }
                        tdName.text(f.nameLoc);
                        tdSize.text(f.sizeSvr);
                        tdOp.text("下载").css("cursor", "pointer").click(function ()
                        {
                            if (downer.Config["Folder"] == "") { downer.app.openFolder(); return; }
                            //文件夹
                            if (f.fdTask)
                            {
                                downer.addFolder(f);
                            }
                            else
                            {
                                downer.addFile( f);
                            }                            
                        });
                        tb.append(tmp);
                        svrFiles[f.id] = f;//
                    });
                    tb.show();
                    $("#msg_load").hide();
                }
                , error: function (req, txt, err) { alert("加载上传数据失败！" + req.responseText); }
                , complete: function (req, sta) { req = null; }
            });
        }

    	$(function ()
        {
            $("#tbCmp").hide().after("<p id='msg_load'>正在加载数据<p>");
    	    downer.loadTo("downDiv");

            $("#btnDownSel").click(function () {
                if (downer.Config["Folder"] == "") { downer.openConfig(); return; }
                var count = 0;
                $("input[name='cbSel']").each(function (i, n) {
                    if ($(n).attr("checked")) {
                        var f = svrFiles[$(n).attr("fid")];
                        if (f.fdTask) downer.addFolder(f);
                        else downer.addFile(f);
                        count++;
                    }
                });
                if (count) 
                setTimeout(function () { downer.down_next();}, 500);
            });
    	});
    </script>
</head>
<body>
    <p>此页为下载控件演示页面，与up6配合使用。可以下载up6数据库中的文件和文件夹</p>
    <ul>
        <li><p><a target="_blank" href="../index.htm">打开上传页面</a></p></li>
        <li><p><a target="_blank" href="../db/clear.aspx">清空上传数据库</a></p></li>
        <li><p><a target="_blank" href="db/clear.aspx">清空下载数据库</a></p></li>
    </ul>    
    <table id="tbCmp" cellpadding="0" cellspacing="0" border="1" class="files-svr">
        <tr id="tbHead">
            <td name="sel" align="center"><input type="checkbox" name="btnSelAll" /></td>
            <td name="type">类型</td>
            <td name="name">名称</td>
            <td name="size">文件大小</td>
            <td name="op">操作</td>
        </tr>
        <tfoot>
            <tr>
                <td colspan="5">
                    <a id="btnDownSel">批量下载</a>
                </td>
            </tr>
        </tfoot>
    </table>
    <div id="downDiv"></div>
    <div id="msg"></div>
</body>
</html>