<%@ WebHandler Language="C#" Class="VerifyPWD" %>

using System;
using System.Web;
using MicroUserHelper;
using MicroAuthHelper;
using MicroPublicHelper;
using MicroLdapHelper;

public class VerifyPWD : IHttpHandler, System.Web.SessionState.IRequiresSessionState {

    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = "密码不正确。<br/>password is incorrect.",
        PWD = context.Server.UrlDecode(context.Request.Form["pwd"]);

        if (!string.IsNullOrEmpty(PWD))
            flag = VerifyPassword(PWD);

        context.Response.Write(flag);
    }

    private string VerifyPassword(string PWD)
    {
        string flag = "密码不正确。<br/>password is incorrect.";

        if (MicroAuth.VerifyPassword(PWD))
            flag = "True";

        return flag;
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}