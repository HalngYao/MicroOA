<%@ WebHandler Language="C#" Class="CheckLogin" %>

using System;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroPublicHelper;
using MicroUserHelper;
using Newtonsoft.Json.Linq;

public class CheckLogin : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        string flag = "请您先登录。<br/> Please log in first.",
                Action = context.Server.UrlDecode(context.Request.Form["action"]),
                Fields = context.Server.UrlDecode(context.Request.Form["fields"]);

        //测试数据
        //Action = "login";
        //Fields = context.Server.UrlDecode("%7B%22txtUserName%22:%22admin%22,%22txtUserPassword%22:%22admin%22%7D");

        if (!string.IsNullOrEmpty(Fields.toStringTrim()))
        {
            if (Action == "login")
                flag = GetCheckLogin(Fields);

            if (Action == "winlogin")
                flag = GetCheckWindowsLogin(Fields);
        }
        context.Response.Write(flag);
    }

    private string GetCheckLogin(string Fields)
    {
        string flag = "请您先登录。<br/> Please log in first.";

        string vCode = string.Empty;
        string UserName = string.Empty, UserPassword = string.Empty, ValidateCode = string.Empty, AutoLogin = "False";
        dynamic json = JToken.Parse(Fields) as dynamic;

        UserName = json["txtUserName"];
        if (!string.IsNullOrEmpty(UserName))
            UserName = UserName.Trim();

        UserPassword = json["txtUserPassword"];
        if (!string.IsNullOrEmpty(UserPassword))
            UserPassword = UserPassword.Trim();

        ValidateCode = json["txtValidateCode"];
        if (!string.IsNullOrEmpty(ValidateCode))
            ValidateCode = ValidateCode.Trim().ToLower();

        AutoLogin = json["cbAutoLogin"];

        Boolean EnabledVerifyCode=MicroPublic.GetMicroInfo("EnabledVerifyCode").toBoolean();
        if (!EnabledVerifyCode)
        {
            ValidateCode = "1";
            vCode = "1";
        }
        else
            vCode = HttpContext.Current.Session["vCode"].ToString();

        if (ValidateCode == vCode)
        {
            if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(UserPassword))
            {
                MicroUserInfo UserInfo = (MicroUserInfo)HttpContext.Current.Session["UserInfo"];
                UserInfo.UserName = UserName;
                UserInfo.UserPassword = UserPassword;
                UserInfo.AutoLogin = AutoLogin.toBoolean();

                if (UserInfo.TryLogin())
                {
                    if (UserInfo.AutoLogin)
                        UserInfo.SetCookies();

                    flag = UserInfo.LoginMsg;
                }
                else
                    flag = MicroPublic.GetLoginMsg(UserInfo.LoginStatus);

            }
            else
                flag = MicroPublic.GetLoginMsg(1);
        }
        else
            flag = MicroPublic.GetLoginMsg(3);

        return flag;
    }


    private string GetCheckWindowsLogin(string Fields)
    {
        string flag = "请您先登录。<br/> Please log in first.";

        string UserName = string.Empty, DomainName = string.Empty, DomainPrefix = string.Empty, AutoLogin = "False";

        Boolean EnabledLDAPAuth = MicroPublic.GetMicroInfo("EnabledLDAPAuth").toBoolean();

        //是否启用LDAP(域账号)认证
        if (EnabledLDAPAuth)
        {
            string WindowsAccount = HttpContext.Current.User.Identity.Name;   //取得当前windows用户 domain\user
            dynamic json = JToken.Parse(Fields) as dynamic;

            AutoLogin = json["cbAutoLogin"];

            if (!string.IsNullOrEmpty(WindowsAccount))
            {
                DomainPrefix = MicroPublic.GetMicroInfo("DomainPrefix"); //取得系统设定的域名前缀
                DomainPrefix = !string.IsNullOrEmpty(DomainPrefix) == true ? DomainPrefix.ToLower() : "";

                if (WindowsAccount.Split('\\').Length > 0)
                {
                    DomainName = MicroPublic.GetSplitStr(WindowsAccount.ToLower(), '\\', 0);  //取得domain
                    UserName = MicroPublic.GetSplitStr(WindowsAccount.ToLower(), '\\', 1);   //取得user
                }

                //以当前登录的Windows用户的域与系统设定的域对比，当前域用户是否合法
                if (!string.IsNullOrEmpty(DomainPrefix) && DomainPrefix == DomainName)
                {
                    if (!string.IsNullOrEmpty(UserName))
                    {
                        MicroUserInfo UserInfo = (MicroUserInfo)HttpContext.Current.Session["UserInfo"];
                        UserInfo.UserName = UserName;
                        UserInfo.AutoLogin = AutoLogin == "on" ? true : false;

                        if (UserInfo.TryWindowsLogin())
                        {
                            if (UserInfo.AutoLogin)
                                UserInfo.SetCookies();

                            flag = UserInfo.LoginMsg;
                        }
                        else
                            flag = MicroPublic.GetLoginMsg(UserInfo.LoginStatus);
                    }
                    else  //帐号或密码不正确
                        flag = MicroPublic.GetLoginMsg(1);
                }
                else  //当前Windows登录域用户非设定的域
                    flag = MicroPublic.GetLoginMsg(6);
            }
            else   //非域用户登录Windows
                flag = MicroPublic.GetLoginMsg(5);


        }
        else  //没有启用域帐号认证
            flag = MicroPublic.GetLoginMsg(7);

        return flag;
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}