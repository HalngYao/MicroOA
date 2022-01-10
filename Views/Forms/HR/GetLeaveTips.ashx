<%@ WebHandler Language="C#" Class="GetLeaveTips" %>

using System;
using System.Web;
using MicroAuthHelper;
using MicroHRHelper;
using Newtonsoft.Json.Linq;

public class GetLeaveTips : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string UID = context.Server.UrlDecode(context.Request.QueryString["Val"]);
        string HolidayTypeID = context.Server.UrlDecode(context.Request.QueryString["TypeID"]);
        string OvertimeDate = context.Server.UrlDecode(context.Request.QueryString["Date"]);
        string HolidayTypeName = context.Server.UrlDecode(context.Request.QueryString["TypeName"]);

        //OvertimeDate = "2021-02-05";
        //UID = "1";
        //HolidayTypeID = "1";

        context.Response.Write(GetLeave(UID, HolidayTypeID, OvertimeDate, HolidayTypeName));
    }


    private string GetLeave(string UID, string HolidayTypeID, string OvertimeDate, string TypeName)
    {
        string flag = MicroHR.GetLeaveMax(UID, HolidayTypeID, OvertimeDate, "Finish"), Tips = string.Empty, Days = "0", Hour = "0", HourTips = string.Empty, Description = string.Empty;

        try
        {
            var getUserWorkHourSystem = MicroHR.GetUserWorkHourSystem(UID.toInt(), OvertimeDate);
            int WorkHourSysID = (getUserWorkHourSystem.WorkHourSysID).toInt();

            string TitleOvertimeDate = OvertimeDate.toDateFormat("yyyy年MM月");

            if (WorkHourSysID == 2)
                TitleOvertimeDate = OvertimeDate.toDateQFirstDay().toDateFormat("yyyy年MM月") + "~" + OvertimeDate.toDateQLastDay().toDateFormat("MM月") + " 季度内";

            dynamic json = JToken.Parse(flag) as dynamic;

            Days = json["Days"];
            Hour = json["Hour"];
            Description = json["Description"];

            //if (!string.IsNullOrEmpty(Hour))
            HourTips = "，" + Hour + " 小时。 ";

            if (HolidayTypeID == "1") //年休假时
                Tips = "可用" + TypeName + " " + Days + " 天" + HourTips + Description;

            else if (HolidayTypeID == "2")  //代休假时
            {
                if (!string.IsNullOrEmpty(OvertimeDate))
                    Tips = string.IsNullOrEmpty(OvertimeDate) ? "" : TitleOvertimeDate + " 剔除已代休后的休日加班共 " + Hour + " 小时，可代休 " + Days + " 天。 " + Description;
                else
                    Tips = "请选择加班月份";
            }
            else
                Tips = Description;
        }
        catch { }

        return "{\"Days\": \"" + Days + "\",\"Hour\":  \"" + Hour + "\",\"Tips\": \"" + Tips + "\"}";

    }


    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}