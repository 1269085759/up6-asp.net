﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="up6.down2.index" %>
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
        function PageApp() {
            var _this = this;
            this.data = {
                filesSvr:ko.observableArray(),
                downer:new DownloaderMgr({
                    event:{
                        ready:function()
                        {
                            _this.load_files();
                        },
                        folderSel:function(dir){
                            _this.down_folderSel(dir);
                        },
                        fileAppend:function(f){
                            _this.down_fileAppend(f);
                        }
                    }
                }),
                downCur:null,
                selAll:ko.observable(false),
                sels:ko.observableArray()
            };
            this.ui = {
                tip:null
            };
            
            this.data.selAll.subscribe(function(nvalue){
                var s = _this.data.filesSvr();
                _this.data.filesSvr([]);
                for(var i = 0 ;i<s.length;i++)
                {
                    s[i].sel = nvalue;
                }
                _this.data.filesSvr(s);
                _this.data.sels(s);
            });

            //api
            this.load_files = function () {
                $.ajax({
                    type: "GET"
                    , dataType: 'jsonp'
                    , jsonp: "callback" //自定义的jsonp回调函数名称，默认为jQuery自动生成的随机函数名
                    , url: _this.data.downer.Config["UrlListCmp"]
                    , data: { uid: _this.data.downer.Config.Fields["uid"], time: new Date().getTime() }
                    , success: function (msg) {
                        _this.ui.tip.hide();
                        if (msg.value == null) {
                            return;
                        }

                        var files = JSON.parse(decodeURIComponent(msg.value));
                        _this.load_files_cmp(files);
                    }
                    , error: function (req, txt, err) { alert("加载上传数据失败！" + req.responseText); }
                    , complete: function (req, sta) { req = null; }
                });
            };
            this.load_files_cmp = function (files) { 
                _this.data.filesSvr.removeAll();
                $.each(files,function(i,n){
                    var pv = $.extend({},n,{sel:false});
                    _this.data.filesSvr.push(pv);
                });
                this.ui.tip.hide();
            };
            this.down_folderSel = function(dir){
                setTimeout(function(){
                    if(_this.data.downCur!=null)
                    {
                        _this.data.downer.addTask(_this.data.downCur);
                        _this.data.downCur = null;
                    }
                    if(_this.data.sels().length>0)
                    {
                        $.each(_this.data.sels(),function(i,n){
                            _this.data.downer.addTask(n);
                        });
                        _this.data.sels([]);
                    }
                },100);
            };
            this.down_fileAppend = function(f){
                setTimeout(function(){
                    _this.data.downer.start_queue();
                },300);
            };
            //event
            this.mouse_over = function(data,e){
                var o = $(e.srcElement);
                if(!o.is("tr"))
                {
                    o = o.parent();
                    while(!o.is("tr")) o = o.parent();
                }
                o.addClass("bk-hover");
            };
            this.mouse_out = function(data,e){
                var o = $(e.srcElement);
                if(!o.is("tr"))
                {
                    o = o.parent();
                    while(!o.is("tr")) o = o.parent();
                }
                o.removeClass("bk-hover");
            };
            this.btnCbk_click = function(d,e){
                _this.data.sels.push(d);
            };
            this.btnDown_click = function(d,e)
            {
                if (_this.data.downer.Config["Folder"] == "") 
                { 
                    _this.data.downCur = d;
                    _this.data.downer.app.openFolder(); 
                    return; 
                }

                _this.data.downer.addTask(d);
            };
            //批量下载
            this.btnDownBat_click = function(){
                _this.data.sels([]);
                $.each(_this.data.filesSvr(),function(i,n){
                    if(n.sel)
                    {
                        _this.data.sels.push(n);
                    }
                });
                
                if (_this.data.downer.Config["Folder"] == "") 
                { 
                    _this.data.downer.app.openFolder(); 
                    return; 
                }
                _this.down_folderSel();
            };
            //
            this.ready = function(){
                this.ui.tip = $("#tip").show();
                this.data.downer.loadTo("downDiv");
                ko.applyBindings(_this);//
            };
        }

    	$(function ()
        {
            var app = new PageApp();
            app.ready();
    	});
    </script>
</head>
<body>
    <p>此页为下载控件演示页面，与up6配合使用。可以下载up6数据库中的文件和文件夹</p>
    <ul>
        <li><p><a target="_blank" href="ligerui.aspx">打开ligerui示例</a></p></li>
        <li><p><a target="_blank" href="../index.htm">打开上传页面</a></p></li>
        <li><p><a target="_blank" href="../db/clear.aspx">清空上传数据库</a></p></li>
        <li><p><a target="_blank" href="db/clear.aspx">清空下载数据库</a></p></li>
    </ul>
    <p id="tip">正在加载数据</p>
    <table id="tbCmp" cellpadding="0" cellspacing="0" border="1" class="files-svr">
        <thead>
            <tr>
                <td colspan="5">
                    <a data-bind="click:btnDownBat_click"><img src="js/down.png"/>批量下载</a>
                </td>
            </tr>            
        </thead>
        <tr id="tbHead">
            <td name="sel" align="center"><input type="checkbox" data-bind="checked:data.selAll" /></td>
            <td name="type">类型</td>
            <td name="name">名称</td>
            <td name="size">文件大小</td>
            <td name="op">操作</td>
        </tr>
        <tbody data-bind="foreach:data.filesSvr">
            <tr data-bind="event:{ mouseover:$parent.mouse_over,mouseout:$parent.mouse_out}">
                <td name="sel" align="center"><input type="checkbox" data-bind="checked:sel"/></td>
                <td name="type"><img src="js/file1.png" data-bind="visible:!fdTask"/><img src="js/folder1.png" data-bind="visible:fdTask"/></td>
                <td name="name" data-bind="text:nameLoc"></td>
                <td name="size" data-bind="text:sizeSvr"></td>
                <td name="op"><a data-bind="click:$parent.btnDown_click"><img src="js/down.png" title='下载'/></a></td>
            </tr>
        </tbody>
    </table>
    <div id="downDiv"></div>
    <div id="msg"></div>
</body>
</html>