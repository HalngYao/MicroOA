using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MicroPublicHelper;

public partial class Views_UserCenter_Forget : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        hlWinLogin.Visible = MicroPublic.GetMicroInfo("DisplayDomainAccountLogin").toBoolean();
    }

    protected string GetMicroInfo(string Type)
    {
        return MicroPublicHelper.MicroPublic.GetMicroInfo(Type);
    }
}