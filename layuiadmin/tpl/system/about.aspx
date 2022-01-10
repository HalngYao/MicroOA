<%@ Page Language="C#" AutoEventWireup="true" CodeFile="about.aspx.cs" Inherits="layuiadmin_tpl_system_about" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div class="layui-card-header">关于 <%=GetTitle() %></div>
            <div class="layui-card-body layui-text layadmin-about">
                <p><%=GetFoot() %></p>
<%--              <script type="text/html" template>
                <p><%=GetFoot() %></p>
              </script>--%>

            </div>

        </div>
    </form>
</body>
</html>
