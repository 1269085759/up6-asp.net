var layout = {
    ui: { btnUp: "#btn-up" }
    , ui_ents: [{ id: "#btn-up", e: "click", n: function () { layout.event.btn_up_click(); } }]
    , app: null
    , event: {
        btn_up_click: function () {
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
                }
                , btn1: function (index, layero) {
                    var ifm = window[layero.find('iframe')[0]['name']];
                    var ret = ifm.toObj();//
                    layer.close(index);//关闭窗口
                }
                , btn2: function (index, layero) { }
            });
        }
    }
    , data: {}
    , init: function () {
        $.each(layout.ui_ents, function (i, n) {
            $(n.id).bind(n.e, n.n);
        });
    }
};

$(function () {
    layout.init();
});