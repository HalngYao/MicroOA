<%@ WebHandler Language="C#" Class="UserPassword" %>

using System;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroPublicHelper;
using MicroUserHelper;
using Newtonsoft.Json.Linq;
using MicroAuthHelper;

public class UserPassword : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = MicroPublic.GetMsg("ModifyFailed"),
                    Action = context.Server.UrlDecode(context.Request.Form["action"]),
                    MID = context.Server.UrlDecode(context.Request.Form["mid"]),
                    Fields = context.Server.UrlDecode(context.Request.Form["fields"]);


        if (Action == "modify" && !string.IsNullOrEmpty(Fields))
            flag = ModifyUserPassword(Fields);


        context.Response.Write(flag);
    }

    private string ModifyUserPassword(string Fields)
    {
        string flag = string.Empty;
        try
        {

            string UserPassword = string.Empty; string NewUserPassword = string.Empty;

            int UID = ((MicroUserInfo)HttpContext.Current.Session["UserInfo"]).UID;

            dynamic json = JToken.Parse(Fields) as dynamic;

            UserPassword = json["txtUserPassword"];
            if (!string.IsNullOrEmpty(UserPassword))
                UserPassword = UserPassword.toStringTrim();

            NewUserPassword = json["txtNewUserPassword"];
            if (!string.IsNullOrEmpty(NewUserPassword))
                NewUserPassword = NewUserPassword.toStringTrim();

            if (!string.IsNullOrEmpty(NewUserPassword))
            {
                string _sql = "select UserName,UserPassword from UserInfo where UID=@UID ";
                SqlParameter[] _sp = { new SqlParameter("@UID", SqlDbType.Int) };

                _sp[0].Value = UID;

                DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

                if (_dt != null && _dt.Rows.Count > 0)
                {
                    //如果是第一次登录则免旧密码，同时更新第一次登录FirstLogin为False值
                    if (MicroUserInfo.GetUserInfo("FirstLogin").toBoolean())
                    {
                        string _sql2 = "update UserInfo set UserPassword = @UserPassword, FirstLogin=@FirstLogin where UID=@UID";
                        SqlParameter[] _sp2 = { new SqlParameter("@UserPassword", SqlDbType.VarChar,50),
                                            new SqlParameter("@FirstLogin", SqlDbType.Bit),
                                            new SqlParameter("@UID", SqlDbType.Int) };

                        _sp2[0].Value = NewUserPassword;
                        _sp2[1].Value = false;
                        _sp2[2].Value = UID;

                        if (MsSQLDbHelper.ExecuteSql(_sql2, _sp2) > 0)
                        {
                            //更新密码成功时把Session的FirstLogin设置为false;
                            ((MicroUserInfo)HttpContext.Current.Session["UserInfo"]).FirstLogin = false;
                            flag = MicroPublic.GetMsg("Modify");
                        }
                    }
                    else  //否则不是第一次登录时
                    {
                        //先判断旧密码不能为空
                        if (!string.IsNullOrEmpty(UserPassword))
                        {
                            //再判断输入的旧密码是否和数据库保存的密码是否相等
                            if (UserPassword == _dt.Rows[0]["UserPassword"].toStringTrim())
                            {
                                string _sql2 = "update UserInfo set UserPassword = @UserPassword, FirstLogin=@FirstLogin where UID=@UID";
                                SqlParameter[] _sp2 = { new SqlParameter("@UserPassword", SqlDbType.VarChar,50),
                                            new SqlParameter("@FirstLogin", SqlDbType.Bit),
                                            new SqlParameter("@UID", SqlDbType.Int) };

                                _sp2[0].Value = NewUserPassword;
                                _sp2[1].Value = false;
                                _sp2[2].Value = UID;

                                if (MsSQLDbHelper.ExecuteSql(_sql2, _sp2) > 0)
                                {
                                    //更新密码成功时把Session的FirstLogin设置为false;
                                    ((MicroUserInfo)HttpContext.Current.Session["UserInfo"]).FirstLogin = false;
                                    flag = MicroPublic.GetMsg("Modify");
                                }

                            }
                            else
                                flag = "当前密码不正确<br/>The current password is incorrect";

                        }
                        else
                            flag = "当前密码不能为空<br/>The current password cannot be empty";
                    }
                }
                else
                    flag = MicroPublic.GetMsg("DenyURLError");  //拒绝访问，非法的URL参数<br/>Denied access, illegal URL parameter
            }

        }
        catch (Exception ex) { flag = ex.ToString(); }

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