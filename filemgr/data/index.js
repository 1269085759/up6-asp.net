function PageLogic() {
    var _this = this;

    this.attr = {
        ui: { btnUp: "#btn-up" }
        ,nav_path:null
        , ui_ents: [
            {
                id: "#btn-up", e: "click", n: function () {
                    _this.attr.event.btn_up_click();
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
            , table_tool_click: function (obj, table) {
                _this.attr.table_events[obj.event](obj, table);
            }
            , table_edit: function (obj) {

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
            , "mkFolder": function (obj, table) { }
            , "delete": function (obj, table) { }
            , "file": function (obj, table) { _this.attr.event.table_file_click(obj, table); }
        }
        , data: {}
        , open_folder: function (data, table) {
            layui.use(['table'], function () {
                var table = layui.table;
                table.reload('files', {
                    url: 'index.aspx?op=data&pid=' + data.f_id //数据接口
                    , page: { curr: 1 }//第一页
                });

                _this.attr.event.path_changed(data);
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
});