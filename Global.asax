<%@ Application Language="C#" %>
<%@ Import Namespace="MicroOA" %>
<%@ Import Namespace="MicroUserHelper" %>
<%@ Import Namespace="MicroPublicHelper" %>
<%@ Import Namespace="System.Web.Optimization" %>
<%@ Import Namespace="System.Web.Routing" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e)
    {
        RouteConfig.RegisterRoutes(RouteTable.Routes);
        //BundleConfig.RegisterBundles(BundleTable.Bundles);
        Application["OnlineUser"] = 0;
    }

    void Application_End(object sender, EventArgs e)
    {
        //  在应用程序关闭时运行的代码

    }

    void Application_Error(object sender, EventArgs e)
    {
        // 在出现未处理的错误时运行的代码

    }

    void Session_Start(object sender, EventArgs e)
    {
        ////在新会话启动时运行的代码
        Session["MicroInfo"] = new MicroInfo(); //系统信息、站点信息
        Session["UserInfo"] = new MicroUserInfo();  //用户登录       

        Application.Lock();
        Application["OnlineUser"] = (int)Application["OnlineUser"] + 1;
        Application.UnLock();
    }

    void Session_End(object sender, EventArgs e)
    {
        //在会话结束时运行的代码。 
        // 注意: 只有在 Web.config 文件中的 sessionstate 模式设置为
        // InProc 时，才会引发 Session_End 事件。如果会话模式 
        //设置为 StateServer 或 SQLServer，则不会引发该事件。

        Application.Lock();
        Application["OnlineUser"] = (int)Application["OnlineUser"] - 1;
        Application.UnLock();

    }
</script>
