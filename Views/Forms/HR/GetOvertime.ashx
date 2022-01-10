<%@ WebHandler Language="C#" Class="GetOvertime" %>

using System;
using System.Web;
using Newtonsoft.Json.Linq;
using MicroAuthHelper;
using MicroHRHelper;

public class GetOvertime : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string strTemp = GetStrTpl();
        string flag = string.Format(strTemp, "Flag", "2020-01-01", "00:00:00", "23:59:59");

        string Action = context.Server.UrlDecode(context.Request.Form["action"]);
        string ShortTableName = context.Server.UrlDecode(context.Request.Form["stn"]);
        string ModuleID = context.Server.UrlDecode(context.Request.Form["mid"]);
        string FormID = context.Server.UrlDecode(context.Request.Form["formid"]);
        string FormsID = context.Server.UrlDecode(context.Request.Form["formsid"]);
        string OvertimeDate = context.Server.UrlDecode(context.Request.Form["OvertimeDate"]);
        string ShiftTypeID = context.Server.UrlDecode(context.Request.Form["ShiftTypeID"]);

        //Action = "View";
        //ShortTableName = "Test";
        //ModuleID = "8";
        //FormID = "8";
        //FormsID = "8";
        //OvertimeDate = "2020-01-10";
        //ShiftTypeID = "8";

        if (!string.IsNullOrEmpty(OvertimeDate) && !string.IsNullOrEmpty(ShiftTypeID))
        {
            flag = GetOTStartEndTime(OvertimeDate, ShiftTypeID);
        }

        flag = "{" + flag + "}";
        flag = JToken.Parse(flag).ToString();

        context.Response.Write(flag);
    }


    private string GetOTStartEndTime(string OvertimeDate, string ShiftTypeID)
    {
        string strTemp = GetStrTpl();
        string flag = string.Format(strTemp, "Flag", "2020-01-01", "00:00:00", "23:59:59");

        if (!string.IsNullOrEmpty(OvertimeDate) && !string.IsNullOrEmpty(ShiftTypeID))
        {
            var getOTStartEndTime = MicroHR.GetOTStartEndTime(OvertimeDate, ShiftTypeID);

            flag = string.Format(strTemp, "True", getOTStartEndTime.OvertimeDate, getOTStartEndTime.OTStartTime, getOTStartEndTime.OTEndTime);

        }


        return flag;
    }

    private string GetStrTpl()
    {
        string strTpl = "\"State\":\"{0}\", \"OvertimeDate\":\"{1}\", \"OTStartTime\":\"{2}\",\"OTEndTime\":\"{3}\"";
        return strTpl;
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}