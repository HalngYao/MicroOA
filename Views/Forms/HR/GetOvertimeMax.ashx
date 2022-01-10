<%@ WebHandler Language="C#" Class="GetOvertimeMax" %>

using System;
using System.Web;
using MicroAuthHelper;
using MicroHRHelper;

public class GetOvertimeMax : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string Action = context.Server.UrlDecode(context.Request.QueryString["action"]);
        string ShortTableName = context.Server.UrlDecode(context.Request.QueryString["stn"]);
        string ModuleID = context.Server.UrlDecode(context.Request.QueryString["mid"]);
        string FormID = context.Server.UrlDecode(context.Request.QueryString["formid"]);
        string FormsID = context.Server.UrlDecode(context.Request.QueryString["formsid"]);
        string OvertimeDate = context.Server.UrlDecode(context.Request.QueryString["date"]);
        string ShiftTypeID = context.Server.UrlDecode(context.Request.QueryString["typeid"]);
        string OvertimeUID = context.Server.UrlDecode(context.Request.QueryString["val"]);

        //FormID = "5";
        //OvertimeDate = "2021-02-05";
        //ShiftTypeID = "1";
        //OvertimeUID = "1";

        context.Response.Write(GetOvertimeMaxValue(FormID, OvertimeDate, OvertimeUID));
    }


    private string GetOvertimeMaxValue(string FormID, string OvertimeDate, string OvertimeUID)
    {
        string flag = string.Empty;

        if (!string.IsNullOrEmpty(FormID) && !string.IsNullOrEmpty(OvertimeDate))
            flag = MicroHR.GetOvertimeMax(FormID, OvertimeDate, OvertimeUID);

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