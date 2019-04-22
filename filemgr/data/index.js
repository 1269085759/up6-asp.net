function PageLogic() {
    var _this = this;
    this.attr = {
        ui: { btnUp: "#btn-up" }
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
            ,btn_up_click: function () {
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
        }
        , table_events: {
            "up": function (obj, table) {
            }
            , "mkFolder": function (obj, table) { }
            , "delete": function (obj, table) { }
        }
        , data: {}
    };

    this.init = function () {
        $.each(_this.attr.ui_ents, function (i, n) {
            $(n.id).bind(n.e, n.n);
        });
    };
    //
}

var pl = new PageLogic();
$(function () {
    pl.init();
});