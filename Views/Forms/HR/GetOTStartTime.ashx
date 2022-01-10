<%@ WebHandler Language="C#" Class="GetOTStartTime" %>

using System;
using System.Web;
using MicroAuthHelper;
using MicroHRHelper;

public class GetOTStartTime : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = "17:15";
        string OvertimeDate=context.Server.UrlDecode(context.Request.Form["OvertimeDate"]);
        string ShiftTypeID=context.Server.UrlDecode(context.Request.Form["ShiftTypeID"]);

        //Action = "View";
        //ShortTableName = "Test";
        //ModuleID = "8";
        //FormID = "8";
        //FormsID = "8";
        //OvertimeDate = "2020-01-10";
        //ShiftTypeID = "8";

        if (!string.IsNullOrEmpty(OvertimeDate) && !string.IsNullOrEmpty(ShiftTypeID))
        {
            flag = GetOvertimeStartTime(OvertimeDate,ShiftTypeID);
        }

        context.Response.Write(flag);
    }


    private string GetOvertimeStartTime(string OvertimeDate, string ShiftTypeID)
    {
        return MicroHR.GetOTStartTime(OvertimeDate,ShiftTypeID);
    }


    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}