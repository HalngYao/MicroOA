<%@ WebHandler Language="C#" Class="WebSiteTestMail" %>

using System;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;
using MicroDBHelper;
using MicroPublicHelper;
using MicroAuthHelper;
using Newtonsoft.Json.Linq;


public class WebSiteTestMail : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        string flag = "测试发送失败。<br/>The test sent failed.",
                   Action = context.Server.UrlDecode(context.Request.Form["action"]),
                   MID = context.Server.UrlDecode(context.Request.Form["mid"]),
                   Fields = context.Server.UrlDecode(context.Request.Form["fields"]);

        if (!string.IsNullOrEmpty(Fields))
            flag = TestSendMail(Fields);

        context.Response.Write(flag);
    }


    private string TestSendMail(string Fields)
    {
        string flag = "测试发送失败。<br/>The test sent failed.", ToAddress = string.Empty, Subject = string.Empty, Content = string.Empty;
        dynamic json = JToken.Parse(Fields) as dynamic;
        ToAddress = json["txtMailAddress"];
        Subject = json["txtSubject"];
        Content = json["txtContent"];
        try
        {
            if (MicroPublic.SendMail(ToAddress, "", Subject, Content))
                flag = "True发送成功。<br/>The send was successfully.";
        }
        catch (Exception ex)
        {
            flag = ex.ToString();
        }

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