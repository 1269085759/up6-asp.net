<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="vue.aspx.cs" Inherits="up6.vue" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>vue演示</title>
    <link href="../js/up6.css" type="text/css" rel="Stylesheet" charset="utf-8"/>
    <script type="text/javascript" src="../js/jquery-1.4.min.js"></script>
    <script type="text/javascript" src="../js/json2.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="../js/up6.app.js" charset="utf-8"></script>
    <script type="text/javascript" src="../js/up6.edge.js" charset="utf-8"></script>
    <script type="text/javascript" src="../js/up6.config.js" charset="utf-8"></script>
    <script type="text/javascript" src="../js/up6.file.js" charset="utf-8"></script>
    <script type="text/javascript" src="../js/up6.folder.js" charset="utf-8"></script>
    <script type="text/javascript" src="../js/up6.js" charset="utf-8"></script>
    <script type="text/javascript" src="../js/vue.min.js" charset="utf-8"></script>
</head>
<body>
    <p><a href="db/clear.aspx" target="_blank">清空数据库</a></p>
    <p><a href="filemgr/index.aspx" target="_blank">文件管理器演示</a></p>
    <p><a href="index2.aspx" target="_blank">单面板演示</a></p>
    <p><a href="down2/index.htm" target="_blank">打开下载页面</a></p>
    <p><a href="index-single.htm" target="_blank">单文件上传演示</a></p>
    <p><a href="vue.aspx" target="_blank">vue演示</a></p>
    <div id="app">
  {{ message }}
          <up6></up6>
</div>
    <script type="text/javascript">
        // 定义一个名为 button-counter 的新组件
        Vue.component('up6', {
            data() {
                return {
                    app: null
                }
            },
            mounted() {
                //初始化up6
                const _this = this;
                this.app = new HttpUploaderMgr();
                this.app.load_to("up6-div");
            },
            destoryed() {},
            methods: {
                open_file: function () { this.app.openFile(); }
                , open_folder: function () { this.app.openFolder(); }
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