<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Message.aspx.cs" Inherits="Views_Message" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link href="<%: ResolveUrl("~/layuiadmin/layui/css/layui.css")%>" rel="stylesheet" media="all" />
    <link href="<%: ResolveUrl("~/Resource/css/micro.css")%>" rel="stylesheet" media="all" />
    <style type="text/css">
        .mydiv{margin:0 auto;width:50%;height:200px; margin-top:30px;}

    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="mydiv">
            <%= Msg()%>
        </div>
    </form>
</body>
</html>
