using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MicroPublicHelper;

public partial class Views_UserCenter_Register : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //是否启用新用户注册，没有则跳转到登录页面
        Boolean EnabledRegister = MicroPublic.GetMicroInfo("EnabledRegister").toBoolean();
        if (!EnabledRegister)
            Response.Redirect("~/Views/UserCenter/Login");

        divValidateCode.Visible = MicroPublic.GetMicroInfo("EnabledVerifyCode").toBoolean();
        //hlDomainAccountRegister.Visible = MicroPublic.GetMicroInfo("EnabledRegister").toBoolean();
    }

    protected string GetMicroInfo(string Type)
    {
        return MicroPublic.GetMicroInfo(Type);
    }
}