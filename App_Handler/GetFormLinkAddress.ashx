<%@ WebHandler Language="C#" Class="GetFormLinkAddress" %>

using System;
using System.Web;
using System.Data;
using MicroAuthHelper;
using MicroDTHelper;


public class GetFormLinkAddress : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = string.Format(GetStrTpl(), "True", "/Views/Forms/MicroForm"),
                Action = context.Server.UrlDecode(context.Request.Form["action"]),
                ShortTableName = context.Server.UrlDecode(context.Request.Form["stn"]),
                ModuleID = context.Server.UrlDecode(context.Request.Form["mid"]),
                FormID = context.Server.UrlDecode(context.Request.Form["formid"]),
                FormsID = context.Server.UrlDecode(context.Request.Form["pkv"]);

        //测试数据
        //FormID = "5";

        if (!string.IsNullOrEmpty(FormID))
            flag = GetLinkAddress(FormID);

        flag = "{" + flag + "}";

        context.Response.Write(flag);
    }


    private string GetLinkAddress(string FormID)
    {
        string flag = string.Format(GetStrTpl(), "True", "/Views/Forms/MicroForm"),
              LinkAddress = "/Views/Forms/MicroForm";

        DataTable _dtForms = MicroDataTable.GetDataTable("Forms");

        if (_dtForms != null && _dtForms.Rows.Count > 0)
        {
            string linkAddress = _dtForms.Select("FormID=" + FormID.toInt())[0]["LinkAddress"].toStringTrim();
            if (!string.IsNullOrEmpty(linkAddress))
                LinkAddress = linkAddress.toStringTrim();
        }

        flag = string.Format(GetStrTpl(), "True", LinkAddress);

        return flag;
    }

    private string GetStrTpl()
    {
        string flag = "\"State\":\"{0}\",\"LinkAddress\":\"{1}\"";
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