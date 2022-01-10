<%@ WebHandler Language="C#" Class="GetDiffTwoDate" %>

using System;
using System.Web;
using MicroPublicHelper;

public class GetDiffTwoDate : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
    //计算两个日期之间的差异天数和小时数返回(原计划用于休假申请计算开始时间和结束时间返回休假的天数)
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        string flag = "\"Days\": \"{0}\", \"Hour\": \"{1}\", \"DaysTotal\": \"{2}\", \"HourTotal\": \"{3}\"";
        string HolidayType = context.Server.UrlDecode(context.Request.QueryString["holidayType"]);
        string AllDayEvent = context.Server.UrlDecode(context.Request.QueryString["allDayEvent"]);
        string StartDateTime = context.Server.UrlDecode(context.Request.QueryString["startDateTime"]);
        string EndDateTime = context.Server.UrlDecode(context.Request.QueryString["endDateTime"]);


        //测试数据
        //AllDayEvent = "True";
        //StartDateTime = "2021-03-13 00:00";
        //EndDateTime = "2021-03-13 00:00";

        string Days = "0", Hour = "0", DaysTotal="0", HourTotal="0";

        if (!string.IsNullOrEmpty(StartDateTime) && !string.IsNullOrEmpty(EndDateTime))
        {


            string d1 = StartDateTime.toDateFormat("yyyy-MM-dd HH:mm"),
                    d2 = EndDateTime.toDateFormat("yyyy-MM-dd HH:mm");

            var getDateTimeDiff = MicroPublic.GetDateTimeDiff(d1, d2);
            Days = getDateTimeDiff.Days;
            Hour = getDateTimeDiff.Hours;
            DaysTotal = getDateTimeDiff.TotalDays;
            HourTotal = getDateTimeDiff.TotalHours;

            if (AllDayEvent.toBoolean())
            {
                d1 = d1.toDateFormat("yyyy-MM-dd");
                d2 = d2.toDateFormat("yyyy-MM-dd");

                getDateTimeDiff = MicroPublic.GetDateTimeDiff(d1, d2);
                Days = (getDateTimeDiff.Days.toDecimal() + 1).ToString();
                Hour = "0";

            }
        }

        flag = string.Format(flag, Days, Hour, DaysTotal, HourTotal);
        flag = "{" + flag + "}";

        context.Response.Write(flag);
    }


    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}