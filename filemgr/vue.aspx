<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="vue.aspx.cs" Inherits="up6.filemgr.vue" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>文件管理器(vue)</title>
    <%=this.paramPage() %>
    <%= this.require(
              this.m_path["jquery"]
              , this.m_path["res"]+"filemgr.css"
              , this.m_path["bootstrap"]
              , this.m_path["layerui"]
              , this.m_path["jstree"]
              , this.m_path["moment"]
              , this.m_path["vue"]
              , this.m_path["up6"]
              , this.m_path["down2"]
              , this.m_path["root"]+"/filemgr/data/vue-up6.js"
              , this.m_path["root"]+"/filemgr/data/vue-down2.js"
              , this.m_path["root"]+"/filemgr/data/vue-index.js"
              , this.m_path["res"]+"layer.window.js"
              ) %>
</head>
<body>
    <div class="container">
        <div class="row" id="app">
            <div class="col-md-12">
                <div class="m-t-md row">
                    <div class="col-md-12">
                    <button class="btn btn-default btn-sm m-r-xs" role="button" @click="btnUp_click">
                        <img :src="ico.file" />
                        上传文件</button>
                    <button class="btn btn-default btn-sm m-r-xs" role="button" @click="btnUpFolder_click">
                        <img :src="ico.btnUpFd" />
                        上传目录</button>
                    <button class="btn btn-default btn-sm m-r-xs" role="button" @click="btnPaste_click">
                        <img :src="ico.btnPaste" />
                        粘贴上传</button>
                    <button class="btn btn-default btn-sm m-r-xs" role="button" @click="btnMkFolder_click">
                        <img :src="ico.btnEdit" />
                        新建文件夹</button>
                    <button class="btn btn-default btn-sm m-r-xs" role="button" @click="openUp_click">
                        <img :src="ico.btnPnlUp" />
                        打开上传面板</button>
                    <button class="btn btn-default btn-sm m-r-xs" role="button" @click="openDown_click">
                        <img :src="ico.btnPnlDown" />
                        打开下载面板</button>
                    <button class="btn btn-default btn-sm hide m-r-xs" role="button" @click="btnUp_click">
                        <img :src="ico.btnDown" />
                        下载</button>
                    <button class="btn btn-default btn-sm hide" role="button" @click="">
                        <img :src="ico.btnDel" />
                        删除</button>
                        </div>
                </div>
                <!--上传面板-->
                <up6 id="pnl-up" ref="up6" style="display: none;" 
                    :fd_create="url.fd_create"
                    :f_create="url.f_create"
                    @load_complete="up6_loadComplete"
                    @item_selected="up6_itemSelected"
                    @file_append="up6_fileAppend"
                    @file_complete="up6_fileComplete"
                    @folder_complete="up6_folderComplete"></up6>
                <!--下载面板-->
                <down2 id="pnl-down" ref="down" style="display: none;"
                    :fd_data="url.fd_data"
                    @load_complete="down_loadComplete"
                    @same_file_exist="down_sameFileExist"></down2>
                <!--路径导航-->
                <ol class="breadcrumb  m-t-xs" style="margin-bottom: -5px;">
                    <template v-for="p in pathNav">
                    <li>
                        <a class="link" @click="nav_click(p)">{{p.f_nameLoc}}</a>
                    </li>
                </template>
                </ol>
                <table class="table table-hover table-condensed">
                    <thead>
                        <tr>
                            <th style="width:20px"></th>
                            <th style="width:50%;"><input type="checkbox" @change="selAll_click" v-model="idSelAll" />名称</th>
                            <th>大小</th>
                            <th>上传时间</th>
                            <th>编辑</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(f,index) in items">
                            <td>{{index+1}}</td>
                            <td><input type="checkbox" :value="f.f_id" v-model="idSels" />
                                <img :src="ico.file" v-show="!f.f_fdTask"/><img :src="ico.folder" v-show="f.f_fdTask"/>&nbsp;<a @click="open_folder(f)" class="link">{{f.f_nameLoc}}</a></td>
                            <td>{{f.f_sizeLoc}}</td>
                            <td>{{tm_format(f.f_time)}}</td>
                            <td>
                                <a class="m-r-md link" @click=""><img :src="ico.btnEdit"/>重命名</a>
                                <a class="m-r-md link" @click="itemDown_click(f)"><img :src="ico.btnDown"/>下载</a>
                                <a class=" link" @click="btnDel_click(f)"><img :src="ico.btnDel"/>删除</a>
                            </td>
                        </tr>
                    </tbody>
                    <tfoot>
                        <tr>
                            <td colspan="5">
                                <div id="pager"></div>
                            </td>
                        </tr>
                    </tfoot>
                </table>
                <script type="text/javascript">
                    layui.use(['layer'], function () {
                        window.layer = layui.layer;
                    });

                    var pageApp = new PageLogic();
                    window.onbeforeunload = function (event) {  }
                    window.unload = function (event) { pageApp.page_close(); };
                </script>
                </div>
        </div>
    </div>
</body>
</html>