using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MicroPublicHelper;

public partial class Views_Set_System_Browser : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Write("BrowserType：" + MicroPublic.GetBrowser("BrowserNameVersion") + "<br/>");
        Response.Write("BrowserName：" + MicroPublic.GetBrowser("BrowserName") + "<br/>");
        Response.Write("Version：" + MicroPublic.GetBrowser("Version") + "<br/>");
        Response.Write("Platform：" + MicroPublic.GetBrowser("Platform") + "<br/>");
    }
}