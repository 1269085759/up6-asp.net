<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="sql.aspx.cs" Inherits="up6.db.sql" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <p>数据库连接信息：<br/></p>
    <p>User Id：<%=this.m_dbUser %></p>
    <p>Password：<%=this.m_dbPass %></p>
    <p>Initial Catalog：<%=this.m_dbName %></p>
</body>
</html>
