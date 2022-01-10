<%@ WebHandler Language="C#" Class="GetLeaveMax" %>

using System;
using System.Web;
using MicroAuthHelper;
using MicroHRHelper;

public class GetLeaveMax : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string UID = context.Server.UrlDecode(context.Request.QueryString["Val"]);
        string HolidayTypeID = context.Server.UrlDecode(context.Request.QueryString["TypeID"]);
        string OvertimeDate = context.Server.UrlDecode(context.Request.QueryString["Date"]);

        //OvertimeDate = "2021-07-20";
        //UID = "1";
        //HolidayTypeID = "6";

        context.Response.Write(GetLeaveMaxValue(UID, HolidayTypeID, OvertimeDate));
    }


    private string GetLeaveMaxValue(string UID, string HolidayTypeID, string OvertimeDate)
    {
        UID = string.IsNullOrEmpty(UID) ? "0" : UID;
        HolidayTypeID = string.IsNullOrEmpty(HolidayTypeID) ? "0" : HolidayTypeID;

        return MicroHR.GetLeaveMax(UID, HolidayTypeID, OvertimeDate, "Finish");

    }


    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}