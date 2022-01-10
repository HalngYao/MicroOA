<%@ WebHandler Language="C#" Class="CheckUser" %>

using System;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;

public class CheckUser : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        string flag = "False",
                Action = context.Request.Form["action"],
                Fields = context.Request.Form["fields"];

        if (Action == "CheckUser" && !string.IsNullOrEmpty(Fields.toStringTrim()))
            flag = CheckUserName(Fields.toStringTrim());

        if (Action == "CheckEMail" && !string.IsNullOrEmpty(Fields.toStringTrim()))
            flag = CheckEMail(Fields.toStringTrim());

        context.Response.Write(flag);
    }

    private string CheckUserName(string Fields)
    {
        string flag = "False";
        try
        {
            string _sql = "select UID from UserInfo where UserName=@UserName";
            SqlParameter[] _sp ={
                                new SqlParameter("@UserName",SqlDbType.VarChar,50),
                           };
            _sp[0].Value = Fields;

            if (MsSQLDbHelper.Exists(_sql, _sp))
                flag = "此帐号已经存在。<br/> This account has already existed.";
            else
                flag = "True";
        }
        catch { }

        return flag;
    }

    private string CheckEMail(string Fields)
    {
        string flag = "False";
        try
        {
            string _sql = "select UID from UserInfo where EMail=@EMail";
            SqlParameter[] _sp ={
                                new SqlParameter("@EMail",SqlDbType.VarChar,100),
                           };
            _sp[0].Value = Fields;

            if (MsSQLDbHelper.Exists(_sql, _sp))
                flag = "此邮箱已被注册，如发现不是本人使用此邮进行注册，请立即联系管理员。<br/> This mailbox has been registered. If you find that I do not use this mailbox for registration, please contact the administrator immediately.";
            else
                flag = "True";
        }
        catch { }

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