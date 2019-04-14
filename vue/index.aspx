﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="up6.vue.index" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>up6-vue</title>
    <script src="https://cdn.jsdelivr.net/npm/vue"></script>
    <link href="../js/up6.css" type="text/css" rel="Stylesheet" charset="gb2312"/>
    <script type="text/javascript" src="../js/jquery-1.4.min.js"></script>
    <script type="text/javascript" src="../js/json2.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="../js/up6.app.js" charset="utf-8"></script>
    <script type="text/javascript" src="../js/up6.edge.js" charset="utf-8"></script>
    <script type="text/javascript" src="../js/up6.config.js" charset="utf-8"></script>
    <script type="text/javascript" src="../js/up6.file.js" charset="utf-8"></script>
    <script type="text/javascript" src="../js/up6.folder.js" charset="utf-8"></script>
    <script type="text/javascript" src="../js/up6.js" charset="utf-8"></script>
</head>
<body>
    <div id="app">
  {{ message }}
          <http-uploader6></http-uploader6>
</div>
    <script type="text/javascript">
        // 定义一个名为 button-counter 的新组件
        Vue.component('http-uploader6', {
            data() {
                return {
                    upApp: null
                }
            },
            mounted() {
                //初始化up6
                const _this = this;
                this.upApp = new HttpUploaderMgr();
                this.upApp.load_to("up6-div");
            },
            destoryed() {
                //this.editor.destory();
            },
            methods: {
                open_file: function () { this.upApp.openFile(); }
                , open_folder: function () { this.upApp.openFolder(); }
            },
            template: '<div id="up6-div"></div>'
        });

        var app = new Vue({
            el: '#app',
            data: {
                message: '演示up6如何在vue中使用'
            }
        });
    </script>
</body>
</html>