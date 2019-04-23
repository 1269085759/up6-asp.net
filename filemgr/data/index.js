﻿function PageLogic() {
    var _this = this;
    this.downer = null;
    this.down_files = [];

    this.attr = {
        ui: { table:null,btnUp: "#btn-up",btnDown:"#btn-down", key:"#search-key"}
        , nav_path: null
        , ui_ents: [
            {
                id: "#btn-up", e: "click", n: function () {
                    _this.attr.event.btn_up_click();
                }
            },
            {
                id: "#btn-down", e: "click", n: function () {
                    _this.attr.event.btn_down_click();
                }
            },
            {
                id: "#btn-search", e: "click", n: function () {
                    _this.attr.event.btn_search_click();
                }
            }
        ]
        , app: null
        , event: {
              file_post_complete: function () {
                layui.use(['table'], function () {
                    var table = layui.table;
                    table.reload('files', {
                        url: 'index.aspx?op=data' //数据接口
                        , page: { curr: 1 }//第一页
                    });
                });
            }
            , btn_up_click: function () {
                layer.open({
                    type: 2
                    , title: '上传文件'
                    , btn: ['确定', '取消']
                    , content: 'up.aspx'
                    , area: ['454px', '592px']
                    , success: function (layero, index) {
                        var body = layer.getChildFrame('body', index);
                        var ifm = window[layero.find('iframe')[0]['name']]; //得到iframe页的窗口对象，执行iframe页的方法：iframeWin.method();
                        //ifm.method();//调用子页面方法
                        ifm.ent_post_complete = _this.attr.event.file_post_complete;
                    }
                    , btn1: function (index, layero) {
                        var ifm = window[layero.find('iframe')[0]['name']];
                        var ret = ifm.toObj();//
                        layer.close(index);//关闭窗口
                    }
                    , btn2: function (index, layero) { }
                });
            }
            , btn_down_click: function () {
                var pnl = $("#down2-panel");
                pnl.removeClass("hide");
                layer.open({
                    type: 1
                    , title: "下载"
                    , btn: ['确定', '取消']
                    , content: $("#down2-panel")
                    , closeBtn: 0
                    , area: ['439px', '528px']
                    , success: function (layero, index) {

                        if (_this.downer.Config["Folder"] == "") { _this.downer.app.openFolder(); return; }
                        $.each(_this.down_files, function (i, f) {
                            //文件夹
                            if (f.f_fdTask) {
                                _this.downer.app.addFolder(f);
                            }
                            else {
                                //下载数据转换：lenSvr,pathSvr,nameLoc,fileUrl
                                var dt = { f_id:f.f_id,lenSvr: f.f_lenLoc, pathSvr: f.f_pathSvr, nameLoc: f.f_nameLoc, fileUrl: _this.downer.Config["UrlDown"] };
                                _this.downer.app.addFile(dt);
                            }
                        });
                    }
                    , btn1: function (index, layero) {
                        layer.close(index);//
                        pnl.addClass("hide");
                    }
                    , btn2: function (index, layero) {
                        pnl.addClass("hide");
                    }
                });

            }
            , btn_search_click: function () {
                layui.use(['table'], function () {
                    var key = $(_this.attr.ui.key).val();
                    var table = layui.table;
                    table.reload('files', {
                        url: 'index.aspx?op=data&key=' + key //
                        , page: { curr: 1 }//第一页
                    });

                    //_this.attr.event.path_changed(data);
                });  
            }
            , table_tool_click: function (obj, table) {
                _this.attr.table_events[obj.event](obj, table);
            }
            , table_check_change: function (obj, table) {
                var cs = table.checkStatus('files');
                //未选中
                if (cs.data.length < 1) {
                    $(_this.attr.ui.btnDown).addClass("hide");
                }
                else {
                    $(_this.attr.ui.btnDown).removeClass("hide");
                    _this.down_files = cs.data;
                }
            }
            , table_edit: function (obj,table) {

                var param = jQuery.extend({}, obj.data,{ f_nameLoc: obj.value });
                $.ajax({
                    type: "GET"
                    , dataType: "json"
                    , url: "index.aspx?op=rename"
                    , data: { data: JSON.stringify(param) }
                    , success: function (res) {
                        obj.update({f_nameLoc: obj.value});
                    }
                    , error: function (req, txt, err) { }
                    , complete: function (req, sta) { req = null; }
                });
            }
            , table_rename: function (obj, table) {
                new LayerWindow({
                    title: "重命名"
                    , w: "589px"
                    , h: "167px"
                    , url: "app/form.aspx"
                    ,btn_ok:"确定"
                    , load_complete: function (ifm) {
                        ifm.initUI({
                            ui: [{ id: "f_nameLoc", txt: "文件名称" }]
                            , data: obj.data
                        });
                    }
                    , btn_ok_click: function (ifm) {
                        var newData = ifm.toObj();
                        var data = $.extend({}, obj.data, newData);

                        var param = { data: encodeURIComponent(JSON.stringify(data)) };
                        $.ajax({
                            type: "GET"
                            , dataType: "json"
                            , url: "index.aspx?op=rename"
                            , data: param
                            , success: function (res) {
                                obj.update({ "f_nameLoc": newData.f_nameLoc });
                            }
                            , error: function (req, txt, err) { }
                            , complete: function (req, sta) { req = null; }
                        });

                    }
                });
            }
            , table_del: function (obj, table) {
                var msg = "确定要删除文件：" + obj.data.f_nameLoc + " ？";
                if (obj.data.f_fdTask) msg = "确定要删除文件夹：" + obj.data.f_nameLoc + " ？";

                layer.msg(msg, {
                    time: 0 //不自动关闭
                    ,icon:3
                    , btn: ['确定', '取消']
                    , yes: function (index) {
                        layer.close(index);

                        var param = { data: encodeURIComponent(JSON.stringify(obj.data) ) };
                        $.ajax({
                            type: "GET"
                            , dataType: "json"
                            , url: "index.aspx?op=del"
                            , data: param
                            , success: function (res) {
                                obj.del();
                            }
                            , error: function (req, txt, err) { }
                            , complete: function (req, sta) { req = null; }
                        });

                    }
                });

            }
            , table_file_click: function (obj, table) {
                if (obj.data.f_fdTask) _this.attr.open_folder(obj.data, table);
            }
            , path_changed: function (data) {
                $.ajax({
                    type: "GET"
                    , dataType: "json"
                    , url: "index.aspx?op=path"
                    , data: { data: encodeURIComponent(JSON.stringify(data) ) }
                    , success: function (res) {
                        _this.attr.nav_path.folders = res;
                        _this.attr.nav_path.folderCur = data.f_id;
                    }
                    , error: function (req, txt, err) { }
                    , complete: function (req, sta) { req = null; }
                });
            }
        }
        , table_events: {
            "up": function (obj, table) {
            }
            , "mkFolder": function (obj, table) { _this.attr.event.table_file_click(obj, table);}
            , "delete": function (obj, table) { _this.attr.event.table_del(obj, table);}
            , "rename": function (obj, table) { _this.attr.event.table_rename(obj, table);}
            , "file": function (obj, table) { _this.attr.event.table_file_click(obj, table); }
        }
        , data: {}
        , open_folder: function (data, table) {
            layui.use(['table'], function () {
                var table = layui.table;
                table.reload('files', {
                    url: 'index.aspx?op=data&pid=' + data.f_id //
                    , page: { curr: 1 }//第一页
                });

                _this.attr.event.path_changed(data);
            });            
        }
        , search: function (sql) {
            layui.use(['table'], function () {
                var table = layui.table;
                table.reload('files', {
                    url: 'index.aspx?op=search&where=' + encodeURIComponent(sql)
                    , page: { curr: 1 }//
                });
            });
        }
    };

    this.init = function () {
        $.each(_this.attr.ui_ents, function (i, n) {
            $(n.id).bind(n.e, n.n);
        });

        this.attr.nav_path = new Vue({
            el: '#path',
            data: {
                folders: [{ f_id: "", f_nameLoc: "根目录", f_pid: "",f_pidRoot:"" }]
                ,folderCur:""
            }, methods: {
                open_folder: function (id) {
                    var fd = $.grep(this.folders, function (n, i) {
                        return n.f_id == id;
                    });
                    _this.attr.open_folder(fd[0]);
                }
            }
        });
    };
    //
}

var pl = new PageLogic();
$(function () {
    pl.init();
    pl.downer = new DownloaderMgr();
    pl.downer.Config["Folder"] = "";
    pl.downer.loadTo("down2-panel");
});