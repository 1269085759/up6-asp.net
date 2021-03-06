﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ligerui.aspx.cs" Inherits="up6.down2.ligerui" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>down2演示页面</title>
    <%=this.paramPage() %>
    <%= this.require( this.m_path["ligerui"] ) %>
    <%= this.require( this.m_path["down2"] ) %>
    <script type="text/javascript">

        $.ligerDefaults.Grid.formatters['format_control'] = function (num, column) {
            return '下载';
        }
        $.ligerDefaults.Grid.formatters['format_type'] = function (num, column) {
            return num ? '<img src="js/folder1.png" title="文件夹"/>':'<img src="js/file1.png" title="文件"/>';
        }

        function PageApp(){
            var _this = this;
            this.data={
                downCur:null,
                sels:[],
                downer:null
            };
            this.ui={
                table:null,
                btn:{
                    downs:null/*批量下载按钮*/
                },
                down:{
                    panel:null/*下载面板*/,
                    dlg:null/*弹出窗口*/
                }
            };

            //methods
            this.toolbar_downs_click = function(){
                if (_this.data.downer.Config["Folder"] == "") { _this.data.downer.openConfig(); return; }

                $.each(_this.data.sels,function(i,n){
                    _this.data.downer.addTask(n);
                });
                _this.data.sels.length=0;
            };
            this.toolbar_open_click = function(){
                this.open_down_dlg();
            };
            this.toolbar_click = function(it){
                if(it.text=="批量下载") _this.toolbar_downs_click();
                if(it.text=="打开面板") _this.toolbar_open_click();
            };
            this.down_folderSel = function(){
                setTimeout(function(){
                    if(_this.data.downCur!=null) _this.data.downer.addTask(_this.data.downCur);
                    _this.data.downCur = null;

                    $.each(_this.data.sels,function(i,n){
                        _this.data.downer.addTask(n);
                    });
                    _this.data.sels.length=0;
                },100);
            };
            this.down_fileAppend = function(f){
                this.open_down_dlg();
                setTimeout(function(){
                    _this.data.downer.start_queue();
                },100);
            };
            this.load_files_cmp = function(fs){
                //初始化表格
                app.ui.table = $("#filegrid").ligerGrid({
                    checkbox: true,//复选框支持
                    toolbar:{items:[
                            {text:"批量下载",click:app.toolbar_click,img:'/down2/js/down.png'},
                            {text:"打开面板",click:app.toolbar_click,img:'/down2/js/show.png'}
                        ]},
                    columns: [
                        { display: '类型', name: 'fdTask', align: 'left', width: 40, type:'format_type' },
                        { display: '名称', name: 'nameLoc', align: 'left' },
                        { display: '文件大小', name: 'sizeSvr', align: 'right',width: 50 }
                    ],
                    width:600,
                    pageSize:30 ,
                    rownumbers:true,
                    data:{Rows:fs},
                    onCheckAllRow:function (ck){
                        _this.data.sels.length = 0;
                        if(ck)
                        {
                            var rows = _this.ui.table.getCheckedRows();
                            $.each(rows,function(i,n){
                                _this.data.sels.push(n);
                            });
                        }
                    },
                    onCheckRow: function (checked, data, rowindex, rowobj)
                    {
                        _this.data.sels.length = 0;
                        var rows = _this.ui.table.getCheckedRows();
                        $.each(rows,function(i,n){
                            _this.data.sels.push(n);
                        });
                    }
                });
            };
            this.load_files = function(){
                $.ajax({
                    type: "GET"
                    , dataType: 'jsonp'
                    , jsonp: "callback" //自定义的jsonp回调函数名称，默认为jQuery自动生成的随机函数名
                    , url: this.data.downer.Config["UrlListCmp"]
                    , data: { uid: this.data.downer.Config.Fields["uid"], time: new Date().getTime() }
                    , success: function (msg)
                    {
                        if (msg.value == null)
                        {
                            $("#msg_load").hide();
                            return;
                        }

                        var files = JSON.parse( decodeURIComponent(msg.value) );
                        app.load_files_cmp(files);//
                    }
                    , error: function (req, txt, err) { alert("加载上传数据失败！" + req.responseText); }
                    , complete: function (req, sta) { req = null; }
                });
            };
            this.init_downer=function(){
                this.data.downer = new DownloaderMgr(
                    {
                        Folder:"",
                        event:
                            {
                                ready:function (){
                                    _this.load_files();//加载未完成列表
                                },
                                folderSel:function (){
                                    _this.down_folderSel();
                                },//自动下载
                                fileAppend:function (f){
                                    _this.down_fileAppend(f);
                                }
                            }
                    }
                );
            };
            this.open_down_dlg = function (){
                this.ui.down.panel.show();
                if(this.ui.down.dlg == null)
                {
                    this.ui.down.dlg = $.ligerDialog.open({
                        width:462,
                        height:522,
                        target:this.ui.down.panel,
                        showMax:false,
                        showMin:false,
                        slide:false,
                        modal:false,
                        isResize:true
                    });
                }
                else{
                    this.ui.down.dlg.show();
                }
            };

            this.ready = function(){
                this.init_downer();
                this.data.downer.loadTo("downDiv");
                this.ui.down.panel = $("#downDiv");
                this.ui.down.panel.hide();

                ko.applyBindings(_this);
            };
        }
        var app = new PageApp();

        $(function(){
            app.ready();
        });
    </script>
</head>
<body>
<p>此页为下载控件演示页面，与HttpUploader6配合使用。可以下载HttpUploader6数据库中的文件和文件夹</p>
<ul>
    <li><p><a target="_blank" href="/up6/index">打开上传页面</a></p></li>
    <li><p><a target="_blank" href="index.aspx">打开普通下载示例</a></p></li>
    <li><p><a target="_blank" href="/up6/clear">清空上传数据库</a></p></li>
    <li><p><a target="_blank" href="/down2/clear">清空下载数据库</a></p></li>
</ul>
<div id="filegrid"></div>
<div id="downDiv"></div>
<div id="msg"></div>
</body>
</html>