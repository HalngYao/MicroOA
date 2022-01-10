<%@ WebHandler Language="C#" Class="GetLeaveOvertimeDate" %>

using System;
using System.Web;
using MicroAuthHelper;
using MicroHRHelper;

public class GetLeaveOvertimeDate : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string Action = context.Server.UrlDecode(context.Request.QueryString["action"]);
        string ModuleID = context.Server.UrlDecode(context.Request.QueryString["mid"]);
        string FormID = context.Server.UrlDecode(context.Request.QueryString["formid"]);
        string FormsID = context.Server.UrlDecode(context.Request.QueryString["formsid"]);
        string LeaveUID = context.Server.UrlDecode(context.Request.QueryString["uid"]);

        //FormID = "5";
        //LeaveUID = "1";

        context.Response.Write(LeaveOvertimeDate(LeaveUID));
    }


    private string LeaveOvertimeDate(string UID)
    {
        string flag = string.Empty,
               WorkHourSysID = "1",
               MinOvertimeDate = DateTime.Now.toDateMFirstDay(),
               MaxOvertimeDate = DateTime.Now.toDateMLastDay();

        if (UID.toInt() > 0)
        {
            var getUserWorkHourSystem = MicroHR.GetUserWorkHourSystem(UID.toInt(), DateTime.Now.toDateFormat());
            WorkHourSysID = getUserWorkHourSystem.WorkHourSysID;

            if (WorkHourSysID == "1")
            {
                MinOvertimeDate = DateTime.Now.toDateMFirstDay();
                MaxOvertimeDate = DateTime.Now.toDateMLastDay();
            }
            else if (WorkHourSysID == "2")
            {
                MinOvertimeDate = DateTime.Now.toDateQFirstDay();
                MaxOvertimeDate = DateTime.Now.toDateQLastDay();
            }
            else
            {
                MinOvertimeDate = DateTime.Now.ToString("yyyy") + "-01-01";
                MaxOvertimeDate = DateTime.Now.ToString("yyyy") + "-12-31";
            }

        }
        flag = string.Format(GetStrTpl(), "True", WorkHourSysID, MinOvertimeDate, MaxOvertimeDate);

        return "{" + flag + "}";

    }

    private string GetStrTpl()
    {
        string strTpl = "\"State\":\"{0}\", \"WorkHourSysID\":\"{1}\", \"MinOvertimeDate\":\"{2}\", \"MaxOvertimeDate\":\"{3}\"";
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