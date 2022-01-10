using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MicroPublicHelper;
using MicroUserHelper;
using MicroAuthHelper;

public partial class Views_UserCenter_Login : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        divValidateCode.Visible = MicroPublic.GetMicroInfo("EnabledVerifyCode").toBoolean();
        divAutoLogin.Visible = MicroPublic.GetMicroInfo("EnabledAutoLogin").toBoolean();
        hlRegister.Visible = MicroPublic.GetMicroInfo("EnabledRegister").toBoolean();
        hlWinLogin.Visible = MicroPublic.GetMicroInfo("DisplayDomainAccountLogin").toBoolean();
        hidCheckBrowser.Value = MicroPublic.CheckBrowser().ToString();
        hidUnsupportedBrowserTips.Value = Server.UrlEncode(MicroPublic.GetMicroInfo("UnsupportedBrowserTips"));
    }

    protected string GetMicroInfo(string Type)
    {
        return MicroPublic.GetMicroInfo(Type);   
    }
}