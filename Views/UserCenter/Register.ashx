<%@ WebHandler Language="C#" Class="Register" %>

using System;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroPublicHelper;
using Newtonsoft.Json.Linq;

public class Register : IHttpHandler, System.Web.SessionState.IRequiresSessionState {

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        string flag = "注册失败，请检查输入的内容是否正确，如需协助请联系管理员。 <br/> Registration failed, please check whether the input is correct, please contact the administrator for assistance.";
        try
        {
            string Fields = context.Server.UrlDecode(context.Request.Form["fields"]);
            //测试数据
            //Fields = "{\"txtUserName\":\"test04\",\"txtUserPassword\":\"abcd1234\",\"txtConfirmPassword\":\"abcd1234\",\"txtEMail\":\"test04@163.com\",\"txtValidateCode\":\"osfa\",\"agreement\":\"on\"}";

            if (!string.IsNullOrEmpty(Fields))
                flag = SetRegister(Fields);
        }
        catch (Exception ex) { flag = ex.ToString(); }

        context.Response.Write(flag);
    }

    private string SetRegister(string Fields)
    {
        string flag = "注册失败，请检查输入的内容是否正确，如需协助请联系管理员。 <br/> Registration failed, please check whether the input is correct, please contact the administrator for assistance.";

        string vCode = HttpContext.Current.Session["vCode"].ToString();
        string UserName = string.Empty, UserPassword = string.Empty, EMail = string.Empty, ValidateCode = string.Empty, RegIP = string.Empty;
        DateTime timeRegTime = DateTime.Now;

        try
        {
            dynamic json = JToken.Parse(Fields) as dynamic;

            UserName = json["txtUserName"];
            if (!string.IsNullOrEmpty(UserName))
                UserName = UserName.Trim();

            UserPassword = json["txtUserPassword"];
            if (!string.IsNullOrEmpty(UserPassword))
                UserPassword = UserPassword.Trim();

            EMail = json["txtEMail"];
            if (!string.IsNullOrEmpty(EMail))
                EMail = EMail.Trim();

            ValidateCode = json["txtValidateCode"];
            if (!string.IsNullOrEmpty(ValidateCode))
                ValidateCode = ValidateCode.Trim().ToLower();

            RegIP = MicroPublic.GetClientIP();

            if (ValidateCode == vCode)
            {
                //避免恶意注册绕过JS的判断，在这里再次确认用户名和邮箱地址是否已经被注册
                if (CheckUserName(UserName) || CheckEMail(EMail))
                    flag = "用户名或邮箱地址已经被注册，请确认。<br/>User name or email address has been registered, please confirm.";

                else //只有用户名和邮箱不被注册过才能进行注册
                {
                    string _sql = "insert into UserInfo (UserName, UserPassword, EMail, RegIP, RegTime ) values ( @UserName, @UserPassword, @EMail, @RegIP, @RegTime )";

                    SqlParameter[] _sp = { new SqlParameter("@UserName", SqlDbType.NVarChar,50),
                                new SqlParameter("@UserPassword", SqlDbType.VarChar,50),
                                new SqlParameter("@EMail", SqlDbType.VarChar,100),
                                new SqlParameter("@RegIP", SqlDbType.VarChar,50),
                                new SqlParameter("@RegTime", SqlDbType.DateTime),
                                };
                    _sp[0].Value = UserName;
                    _sp[1].Value = UserPassword;
                    _sp[2].Value = EMail;
                    _sp[3].Value = RegIP;
                    _sp[4].Value = timeRegTime;

                    if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
                        flag = "True注册成功，即将跳转到登录页面... <br/> Successful registration, about to jump to the login page...";
                }
            }
            else
                flag = "验证码不正确<br/> Incorrect verification code.";
        }
        catch (Exception e) { flag = e.ToString(); }

        return flag;

    }

    private Boolean CheckUserName(string Fields)
    {
        Boolean flag = false;
        try
        {
            string _sql = "select UID from UserInfo where UserName=@UserName";
            SqlParameter[] _sp ={
                                new SqlParameter("@UserName",SqlDbType.VarChar,50),
                           };
            _sp[0].Value = Fields;

            if (MsSQLDbHelper.Exists(_sql, _sp))
                flag = true;
        }
        catch { }

        return flag;
    }

    private Boolean CheckEMail(string Fields)
    {
        Boolean flag = false;
        try
        {
            string _sql = "select UID from UserInfo where EMail=@EMail";
            SqlParameter[] _sp ={
                                new SqlParameter("@EMail",SqlDbType.VarChar,100),
                           };
            _sp[0].Value = Fields;

            if (MsSQLDbHelper.Exists(_sql, _sp))
                flag = true;
        }
        catch { }

        return flag;
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}