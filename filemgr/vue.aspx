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
              , this.m_path["moment"]
              , this.m_path["vue"]
              , this.m_path["fm-up6"]
              , this.m_path["fm-down2"]
              , this.m_path["root"]+"/filemgr/data/vue-up6.js"
              , this.m_path["root"]+"/filemgr/data/vue-down2.js"
              , this.m_path["root"]+"/filemgr/data/vue-index.js"
              ) %>
</head>
<body>
    <div class="container">
        <div class="row">
            <div class="col-md-12">说明：使用了vue,boostrap,layer组件，支持浏览器：ie9+,firefox,chrome</div>
            </div>
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
                            <img :src="ico.folder1" />
                            新建文件夹</button>
                        <button class="btn btn-default btn-sm m-r-xs" role="button" @click="openUp_click">
                            <img :src="ico.btnPnlUp" />
                            打开上传面板</button>
                        <button class="btn btn-default btn-sm m-r-xs" role="button" @click="openDown_click">
                            <img :src="ico.btnPnlDown" />
                            打开下载面板</button>
                        <button class="btn btn-default btn-sm m-r-xs" role="button" @click="btnDowns_click" v-show="idSels.length>0">
                            <img :src="ico.btnDown" />
                            批量下载</button>
                        <button class="btn btn-default btn-sm hide" role="button" @click="">
                            <img :src="ico.btnDel" />
                            删除</button>
                        <div class="form-inline pull-right">
                            <input type="text" class="form-control input-sm" placeholder="请输入关键字" v-model="search.key" @input="searchKey_changed" @keyup.enter="btnSearch_click"/>
                            <button class="btn btn-default btn-sm m-r-xs" role="button" @click="btnSearch_click">
                                <span class="glyphicon glyphicon-search" aria-hidden="true"></span>
                                搜索</button>
                        </div>
                    </div>
                </div>
                <!--上传面板-->
                <up6 id="pnl-up" ref="up6" style="display: none;" 
                    :fd_create="url.fd_create"
                    :f_create="url.f_create"
                    :fields="fields"
                    :cookie="cookie"
                    @load_complete="up6_loadComplete"
                    @item_selected="up6_itemSelected"
                    @file_append="up6_fileAppend"
                    @file_complete="up6_fileComplete"
                    @folder_complete="up6_folderComplete"
                    @unsetup="up6_unsetup"></up6>
                <!--下载面板-->
                <down2 id="pnl-down" ref="down" style="display: none;"
                    :fd_data="url.fd_data"
                    :fields="fields"
                    @load_complete="down_loadComplete"
                    @same_file_exist="down_sameFileExist"
                    @unsetup="down_unsetup"
                    @folder_sel=down_folderSel></down2>                
                <!--路径导航-->
                <ol class="breadcrumb  m-t-xs" style="margin-bottom: -5px;">
                    <template v-for="p in pathNav">
                        <li>
                           <a class="link" @click="nav_click(p)">{{p.f_nameLoc}}</a>
                        </li>
                    </template>
                </ol>
                <!--文件列表-->
                <table class="table table-hover table-condensed">
                    <thead>
                        <tr>
                            <th style="width:20px"></th>
                            <th style="width:50%;"><input type="checkbox" @change="selAll_click" v-model="idSelAll" />名称</th>
                            <th>编辑</th>
                            <th>大小</th>
                            <th>上传时间</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-show="folderMker.edit">
                            <td></td>
                            <td>
                                <input class="form-control input-sm" style="width:80%;float:left;" v-model="folderMker.name" ref="tbFdName" @keyup.enter="btnMkFdOk_click"/>
                                <a class="btn btn-default btn-sm m-l-xs" @click="btnMkFdOk_click"><img :src="ico.ok"/></a>
                                <a class="btn btn-default btn-sm" @click="btnMkFdCancel_click"><img :src="ico.cancel"/></a>
                                    </td>
                            <td></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr v-for="(f,index) in items">
                            <td>{{index+1}}</td>
                            <td>
                                <div :name="'v'+index">
                                <input type="checkbox" :value="f.f_id" v-model="idSels" :name="'ckb'+index"/>
                                <img :src="ico.file" v-show="!f.f_fdTask" :name="'name'+index"/>
                                <img :src="ico.folder1" v-show="f.f_fdTask"/>
                                <a @click="open_folder(f)" class="link m-l-xs" :name="'name'+index">{{f.f_nameLoc}}</a>
                                    </div>
                                <div :name="'edit'+index" style="display:none;">
                                <input class="form-control input-sm" style="width:80%;float:left;" :value="f.f_nameLoc" :name="'name'+index" @keyup.enter="btnRename_ok(f,index)"/>
                                <a class="btn btn-default btn-sm m-l-xs" @click="btnRename_ok(f,index)"><img :src="ico.ok"/></a>
                                <a class="btn btn-default btn-sm"  @click="btnRename_cancel(f,index)"><img :src="ico.cancel"/></a>
                                    </div>
                            </td>
                            <td>
                                <a class="m-r-md link" @click="itemRename_click(f,index)"><img :src="ico.btnEdit"/>重命名</a>
                                <a class="m-r-md link" @click="itemDown_click(f)"><img :src="ico.btnDown"/>下载</a>
                                <a class=" link" @click="btnDel_click(f)"><img :src="ico.btnDel"/>删除</a>
                            </td>
                            <td>{{f.f_fdTask?"":f.f_sizeLoc}}</td>
                            <td>{{tm_format(f.f_time)}}</td>
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
                    var v_app = null;
                    var svrCookie = 'ASP.NET_SessionId=<%=Session.SessionID%>';
                    layui.use(['layer'], function () {
                        window.layer = layui.layer;
                    });

                    window.onbeforeunload = function (event) {
                        if (!v_app.taskEmpty()) {
                            event.returnValue = "您还有程序正在运行，确定关闭？";
                        }
                    }
                    window.unload = function (event) {
                        v_app.taskEnd();
                    };
                </script>
                </div>
        </div>
    </div>
</body>
</html>