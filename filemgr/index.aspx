<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="up6.filemgr.index" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>文件管理器</title>
    <%= this.require(
              this.m_path["jquery"]
              , this.m_path["bootstrap"]
              , this.m_path["layerui"]
              , this.m_path["moment"]
              , this.m_path["root"]+"/filemgr/data/index.js"
              ) %>
</head>
<body>
    <div class="container-fluid">
        <a class="btn btn-default" href="#" role="button" id="btn-up">上传文件</a>
        <button class="btn btn-default" type="submit">上传文件夹</button>
        <span id="up6-panel"></span>
        <table class="layui-table" lay-size="sm" id="files" lay-filter="files'" lay-skin="nob"></table>
        <script type="text/javascript">
            //JavaScript代码区域
            layui.use(['element', 'table'], function () {
                var element = layui.element, table = layui.table;

                //js-module
                table.render({
                    elem: '#files'
                    , id: 'files'
                    , defaultToolbar: []//关闭过滤，打印按钮
                    , size: 'sm'
                    , height: 'full'//高度,full, 
                    , url: 'index.aspx?op=data' //数据接口
                    , limit: 30//每页显示的条数，默认10
                    , page: true //开启分页
                    , cols: [[ //表头
                        { width: 50, sort: false, type: 'numbers' }
                        , { field: 'f_id', title: '', width: 50, sort: false, type: 'checkbox' }
                        , { field: 'f_nameLoc', title: '文件名称', width: 500, sort: false, }
                        , { field: 'f_sizeLoc', title: '大小', width: 80, sort: false, }
                        , { field: 'f_time', title: '上传时间', templet: function (d) { return moment(d.f_time).format('YYYY-MM-DD HH:mm:ss') } }
                    ]],
                    done: function (res, curr, count) {
                        //如果是异步请求数据方式，res即为你接口返回的信息。
                        //如果是直接赋值的方式，res即为：{data: [], count: 99} data为当前页数据、count为数据总长度
                        console.log(res);

                        //得到当前页码
                        console.log(curr);

                        //得到数据总量
                        console.log(count);
                    }
                });

                //监听工具条,列模板中的<a>事件也在此监听
                table.on('tool(files)', function (obj) { //注：tool是工具条事件名，test是table原始容器的属性 lay-filter="对应的值"
                    var data = obj.data; //获得当前行数据
                    var layEvent = obj.event; //获得 lay-event 对应的值（也可以是表头的 event 参数对应的值）
                    var tr = obj.tr; //获得当前行 tr 的DOM对象

                    if (layEvent === 'detail') { //查看
                        //do somehing
                    } else if (layEvent === 'del') { //删除
                        layer.confirm('确定删除文章：' + data.title + "？", function (index) {
                            obj.del();
                            layer.close(index);
                            //向服务端发送删除指令
                        });
                    } else if (layEvent === 'edit') { //编辑
                        //do something
                    }
                });

                //工具栏
                table.on('toolbar(docs)', function (obj) { docs_toolbar(obj, table); });

                //复选框
                table.on('checkbox(docs)', function (obj) { docs_toolbar_check(obj); });
            });
        </script>
    </div>
</body>
</html>
