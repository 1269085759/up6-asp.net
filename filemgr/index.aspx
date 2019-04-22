<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="up6.filemgr.index" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>文件管理器</title>
    <%=this.paramPage() %>
    <%= this.require(
              this.m_path["jquery"]
              , this.m_path["res"]+"filemgr.css"
              , this.m_path["bootstrap"]
              , this.m_path["layerui"]
              , this.m_path["moment"]
              , this.m_path["vue"]
              , this.m_path["root"]+"/filemgr/data/index.js"
              ) %>
</head>
<body>
    <div class="container-fluid">
        <div class="m-t-md"></div>
        <button class="btn btn-default btn-sm" role="button" id="btn-up"><span class="glyphicon glyphicon-arrow-up" aria-hidden="true"></span> 上传文件</button>
        <span id="up6-panel"></span>
        <!--路径导航-->
        <ol class="breadcrumb  m-t-xs" style="margin-bottom:-10px;" id="path">
            <li v-for="p in folders"><a class="link">{{p.f_nameLoc}}</a></li>
        </ol>
        <table class="layui-hide" lay-size="sm" id="files" lay-filter="files" lay-skin="nob"></table>
        <script type="text/javascript">
            //JavaScript代码区域
            layui.use(['element', 'table','laytpl'], function () {
                var element = layui.element, table = layui.table,laytpl = layui.laytpl;

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
                        , {
                            field: 'f_nameLoc', title: '名称', width: 500, sort: false, templet: function (d) {
                                var tmp = '<img src="{{d.img}}"/> <a class="link" lay-event="file">{{d.f_nameLoc}}</a>';
                                var par = $.extend(d, { img: page.path.res + "imgs/16/file.png" });
                                if (d.f_fdTask) par.img = page.path.res + "imgs/16/folder.png";
                                var str = laytpl(tmp).render(par);
                                return str;
                            }
                        }
                        , { field: 'f_sizeLoc', title: '大小', width: 80, sort: false, }
                        , { field: 'f_time', title: '上传时间', templet: function (d) { return moment(d.f_time).format('YYYY-MM-DD HH:mm:ss') } }
                    ]],
                    done: function (res, curr, count) {}
                });

                table.on('tool(files)', function (obj) { pl.attr.event.table_tool_click(obj, table);});

                //工具栏
                table.on('toolbar(files)', function (obj) { pl.attr.event.table_tool_click(obj, table); });

                //复选框
                table.on('checkbox(files)', function (obj) { alert("test"); });
                table.on('edit(files)', function (obj) { pl.attr.event.table_edit(obj); });

            });
        </script>
    </div>
</body>
</html>
