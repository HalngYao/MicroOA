<%@ WebHandler Language="C#" Class="Calendar" %>

using System;
using System.Web;
using System.Text;
using System.Data;
using MicroDBHelper;
using MicroUserHelper;
using MicroHRHelper;
using MicroPublicHelper;

public class Calendar : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        string flag = "This tips.",
                Action = context.Server.UrlDecode(context.Request.QueryString["action"]),
                Type = context.Server.UrlDecode(context.Request.QueryString["type"]),
                MID = context.Server.UrlDecode(context.Request.QueryString["mid"]),
                StartDay = context.Server.UrlDecode(context.Request.QueryString["start"]),
                EndDay = context.Server.UrlDecode(context.Request.QueryString["end"]),
                Value = context.Server.UrlDecode(context.Request.QueryString["val"]);

        ////测试数据
        //Action = "View";
        //Type = "GetCalendarEvents";
        //MID = "2";
        //StartDay = "2021-03-01";
        //EndDay = "2021-03-31";
        //Value = "2021年3月";

        //string d = "[{title: '实际："+StartDay+"',start: '2020-11-02T09:30:00',end: '2020-11-02T17:30:00',color: '#1E9FFF'},{title: '实际：09:00-17:00',start: '2020-12-20T09:30:00',end: '2020-12-20T17:30:00',color: '#1E9FFF'}]";

        //JToken.Parse(d)

        //    {"action":"View","type":"GetCalendarDays","start":"2020-11-01","end":"2020-12-05"}

        if (!string.IsNullOrEmpty(StartDay) && !string.IsNullOrEmpty(EndDay))
        {
            StartDay = MicroPublic.GetSplitFirstStr(StartDay, 'T');
            EndDay = MicroPublic.GetSplitFirstStr(EndDay, 'T');

            //获取个人日历事件（含排班、加班、休假）
            if (Action == "View" && Type == "GetCalendarEvents")
                flag = GetCalendarEvents(StartDay, EndDay);

            if (Action == "View" && Type == "GetCalendarDays")
                flag = GetCalendarDays(StartDay, EndDay);

            if (Action == "Modify" && (Type == "CtrlCalendarDays" || Type == "CtrlStatutoryDays"))
                flag = CtrlCalendarDays(Type, StartDay, EndDay);

            if (Action == "View" && Type == "GetPersonalAttendanceData" && !string.IsNullOrEmpty(Value))
                flag = GetPersonalAttendanceData(Value);

        }

        context.Response.Write(flag);
        //context.Response.Write(d);
    }

    /// <summary>
    /// 获取相关日历事件，写入前端日历
    /// </summary>
    /// <param name="StartDay"></param>
    /// <param name="EndDay"></param>
    /// <returns></returns>
    private string GetCalendarEvents(string StartDay, string EndDay)
    {
        string flag = string.Empty;
        int UID = MicroUserInfo.GetUserInfo("UID").toInt();

        string json = string.Empty;

        //获取个人排班事件(即计划事件)
        if (MicroPublic.GetMicroInfo("CalendarPlanEvent").toBoolean())
            json += MicroHR.GetPersonalOnDutyCalendarEvents(StartDay, EndDay, UID);

        json += MicroHR.GetCalendarEvents(StartDay, EndDay, UID);  //获取日历其它事件(暂时用不上)

        //获取个人加班事件
        if (MicroPublic.GetMicroInfo("CalendarOvertimeEvent").toBoolean())
            json += MicroHR.GetOvertimeCalendarEvents(StartDay, EndDay, UID);

        //获取个人休假事件
        if (MicroPublic.GetMicroInfo("CalendarHolidayEvent").toBoolean())
            json += MicroHR.GetLeaevCalendarEvents(StartDay, EndDay, UID);

        if (json.Length > 0)
            flag = "[" + json.Substring(0, json.Length - 1) + "]";
        else
            flag = "[]";

        return flag;
    }

    /// <summary>
    /// 获取假期日期，返回日期组成的字符串（返回前端让日历背景变成灰色）
    /// </summary>
    /// <param name="StartDay"></param>
    /// <param name="EndDay"></param>
    /// <returns></returns>
    private string GetCalendarDays(string StartDay, string EndDay)
    {

        string flag = "{\"code\": 0, \"msg\": \"\", \"count\": 0, \"data\": [] }";
        string strTpl = "\"DayDate\": \"{0}\", \"DaysType\": \"{1}\"";
        StringBuilder sb = new StringBuilder();

        try
        {
            string _sql = " select * from CalendarDays where Invalid=0 and Del=0 and DayDate between '" + StartDay.toDateFormat() + "' and '" + EndDay.toDateFormat() + "'";
            DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

            if (_dt.Rows.Count > 0)
            {
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    string Str = string.Format(strTpl, _dt.Rows[i]["DayDate"].toDateFormat(), _dt.Rows[i]["DaysType"].toStringTrim());
                    sb.Append("{" + Str + "},");
                }
                string json = sb.ToString();
                flag = json.Substring(0, json.Length - 1);

                flag = "{\"code\": 0,\"msg\": \"\",\"count\": " + _dt.Rows.Count + ",\"data\":  [" + flag + "] }";
            }
        }
        catch { }

        return flag;

    }


    /// <summary>
    /// 设置日历假期(休息日、法定日等)
    /// </summary>
    /// <param name="HolidayType"></param>
    /// <param name="StartDay"></param>
    /// <param name="EndDay"></param>
    /// <returns></returns>
    private string CtrlCalendarDays(string HolidayType, string StartDay, string EndDay)
    {
        string flag = MicroPublic.GetMsg("SetFailed");
        string TrueIDs = "0", FalseIDs = "0";

        try
        {
            if (HolidayType == "CtrlCalendarDays")
                HolidayType = "OffDay";  //设置为休息日

            if (HolidayType == "CtrlStatutoryDays")
                HolidayType = "Statutory";   //设置为法定日

            string _sql = "select * from CalendarDays";
            DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

            //要插入数据的字段
            DataTable _dtInsert = new DataTable();
            _dtInsert.Columns.Add("DaysType", typeof(string));
            _dtInsert.Columns.Add("DayDate", typeof(DateTime));

            DateTime StartDate = DateTime.Parse(StartDay);
            DateTime EndDate = DateTime.Parse(EndDay);
            int day = EndDate.Subtract(StartDate).Days;

            for (int i = 0; i < day; i++)
            {
                string CurrDate = StartDate.AddDays(i).toDateFormat();
                DataRow[] _rows = _dt.Select("(DaysType='OffDay' or DaysType='Statutory') and DayDate='" + CurrDate + "'");

                if (_rows.Length > 0)
                {
                    if (_rows[0]["Invalid"].toBoolean())
                        TrueIDs += "," + _rows[0]["CalDaysID"].ToString();  //得到的Invalid=true的CalDaysIDs
                    else
                        FalseIDs += "," + _rows[0]["CalDaysID"].ToString(); //得到的Invalid=false的CalDaysIDs
                }
                else
                {
                    DataRow _dr = _dtInsert.NewRow();
                    _dr["DaysType"] = HolidayType;
                    _dr["DayDate"] = DateTime.Parse(CurrDate);
                    _dtInsert.Rows.Add(_dr);
                }

            }

            string _sql2 = "update CalendarDays set Invalid=0, DaysType='" + HolidayType + "'  where CalDaysID in (" + TrueIDs + ")";
            MsSQLDbHelper.ExecuteSql(_sql2);

            string _sql3 = "update CalendarDays set Invalid=1, DaysType='" + HolidayType + "' where CalDaysID in (" + FalseIDs + ")";
            MsSQLDbHelper.ExecuteSql(_sql3);

            MsSQLDbHelper.SqlBulkCopyInsert(_dtInsert, "CalendarDays");

            flag = MicroPublic.GetMsg("Set");

        }
        catch { }

        return flag;

    }


    /// <summary>
    /// 获取个人勤怠数据，构成JSON格式字符串（主要首页展示用）
    /// </summary>
    /// <param name="AnyDate"></param>
    /// <returns></returns>
    private string GetPersonalAttendanceData(string AnyDate)
    {
        AnyDate = DateTime.Parse(AnyDate).ToString("yyyy-MM-dd");
        int UID = MicroUserInfo.GetUserInfo("UID").toInt();
        return MicroHR.GetPersonalAttendanceData(AnyDate, UID, "Finish");
    }


    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}