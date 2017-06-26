<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="sql.aspx.cs" Inherits="up6.demoSql2005.db.sql" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form action="sql.aspx" method="post">
        数据库连接信息：<br/>
        User Id:        <input type="text" name="uid" value="<%=this.m_dbUser %>" /><br/>
        Password:       <input type="text" name="pwd" value="<%=this.m_dbPass %>" /><br/>
        Initial Catalog:<input type="text" name="dbname" value="<%=this.m_dbName %>" /><br/>
        <input type="submit" value="提交" />
    </form>
</body>
</html>
