<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="vue.aspx.cs" Inherits="up6.vue" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>vue演示</title>
    <%=this.paramPage() %>
    <script type="text/javascript" src="js/jquery-1.4.min.js"></script>
    <script type="text/javascript" src="js/json2.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="js/vue.min.js" charset="utf-8"></script>
    <%= this.require( this.m_path["up6"] ) %>
</head>
<body>
    <p><a href="db/clear.aspx" target="_blank">清空数据库</a></p>
    <p><a href="filemgr/index.aspx" target="_blank">文件管理器演示</a></p>
    <p><a href="index2.aspx" target="_blank">单面板演示</a></p>
    <p><a href="down2/index.aspx" target="_blank">打开下载页面</a></p>
    <p><a href="index-single.aspx" target="_blank">单文件上传演示</a></p>
    <p><a href="vue.aspx" target="_blank">vue演示</a></p>
    <div id="app">
  {{ message }}
          <up6></up6>
</div>
    <script type="text/javascript">
        Vue.component('up6', {
            data: function(){
                return {
                    app: null
                }
            },
            mounted:function() {
                this.app = new HttpUploaderMgr();
                this.app.load_to("up6-div");
            },
            destoryed:function() {},
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