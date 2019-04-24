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
              , this.m_path["up6"]
              , this.m_path["down2"]
              , this.m_path["root"]+"/filemgr/data/index.js"
              , this.m_path["res"]+"layer.window.js"
              ) %>
</head>
<body>
    <div class="container-fluid">
        <div class="m-t-md clearfix">
            <button class="btn btn-default btn-sm pull-left m-r-xs" role="button" id="btn-up"><span class="glyphicon glyphicon-arrow-up" aria-hidden="true"></span> 上传文件</button>
            <button class="btn btn-default btn-sm pull-left m-r-xs" role="button" id="btn-mk-folder"><span class="glyphicon glyphicon-arrow-up" aria-hidden="true"></span> 创建文件夹</button>
            <button class="btn btn-default btn-sm pull-left hide m-r-xs" role="button" id="btn-down"><span class="glyphicon glyphicon-arrow-up" aria-hidden="true"></span> 下载</button>
            <button class="btn btn-default btn-sm pull-left hide" role="button" id="btn-del"><span class="glyphicon glyphicon-remove" aria-hidden="true"></span> 删除</button>
            <span class="pull-right form-inline">
                <input type="text" class="form-control input-sm pull-left m-r-xs" id="search-key" placeholder="" />
                <button class="btn btn-default btn-sm pull-left" role="button" id="btn-search"><span class="glyphicon glyphicon-search" aria-hidden="true"></span> 搜索</button>
            </span>
        </div>
        <div id="http-up6">
            <div class="dialog-web-uploader clearfix" id="up-panel" style="display:none;">
                <div class="uploader-list-wrapper">
                    <div class="uploader-list-header">
                        <div class="file-name ">文件(夹)名</div>
                        <div class="file-size">大小</div>
                        <div class="file-path">上传目录</div>
                        <div class="file-status">状态</div>
                        <div class="file-operate"></div>
                    </div>
                    <div class="uploader-list">
                        <div class="cont" name="list">
                            <div class="file-list" name="file" style="display:none;">
                                <div class="process" style="width: 0%;" name="process"></div>
                                <div class="info ">
                                    <div class="file-name clearfix p-l-sm">
                                        <img name="file" src="res/imgs/24/file.png" /><img name="folder" src="res/imgs/24/folder.png" class="hide"/><span name="name">文件</span>
                                    </div>
                                    <div class="file-size" name="size">10MB</div>
                                    <div class="file-path" name="path">路径</div>
                                    <div class="file-status" name="state"><img src="<%=this.m_path["res"]+"imgs/16/ok.png" %>" style="display:none;color:green;" name="ico-ok"/><span name="msg"></span></div>
                                    <div class="file-operate">
                                        <i class="layui-icon layui-icon-play link" style="display:none;" name="post"></i>
                                        <i class="layui-icon layui-icon-pause link" style="display:none;" name="stop"></i>
                                        <i class="layui-icon layui-icon-close link" style="display:none;" name="del"></i>
                                        <i class="layui-icon layui-icon-close link" style="display:none;" name="cancel"></i>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <span id="down2-panel" class="hide"></span>
        <!--路径导航-->
        <ol class="breadcrumb  m-t-xs" style="margin-bottom:-10px;" id="path">
            <li v-for="p in folders">
                <a class="link" v-on:click="open_folder(p.f_id)">{{p.f_nameLoc}}</a>
            </li>
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
                    , limit: 20//每页显示的条数，默认10
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
                        , { field: 'f_time', title: '上传时间', width:150, templet: function (d) { return moment(d.f_time).format('YYYY-MM-DD HH:mm:ss') } }
                        , { title: '编辑', templet: function (d) { return '<a class="m-r-sm layui-table-link link" lay-event="rename"><span class="glyphicon glyphicon-search" aria-hidden="true"></span> 更名</a><a class="layui-table-link link" lay-event="delete"><span class="glyphicon glyphicon-remove" aria-hidden="true"></span> 删除</a>';} }
                    ]],
                    done: function (res, curr, count) {}
                });

                table.on('tool(files)', function (obj) { pl.attr.event.table_tool_click(obj, table);});

                //工具栏
                table.on('toolbar(files)', function (obj) { pl.attr.event.table_tool_click(obj, table); });

                //复选框
                table.on('checkbox(files)', function (obj) { pl.attr.event.table_check_change(obj, table); });
                table.on('edit(files)', function (obj) { pl.attr.event.table_edit(obj); });
                pl.attr.ui.table = table;
            });
        </script>
    </div>
</body>
</html>
