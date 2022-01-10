<%@ WebHandler Language="C#" Class="GetLeaveDateTime" %>

using System;
using System.Web;
using MicroHRHelper;

public class GetLeaveDateTime : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        string StartDateTime = context.Server.UrlDecode(context.Request.QueryString["StartDateTime"]),
         LeaveDays = context.Server.UrlDecode(context.Request.QueryString["LeaveDays"]),
         LeaveHour = context.Server.UrlDecode(context.Request.QueryString["LeaveHour"]),
         LeaveUID = context.Server.UrlDecode(context.Request.QueryString["LeaveUID"]);

        //StartDateTime = "2021-02-05";
        //LeaveDays = "1.5";
        //LeaveHour = "0";
        //LeaveUID = "1";

        context.Response.Write(GetEndDateTime(StartDateTime, LeaveDays, LeaveHour, LeaveUID));
    }


    /// <summary>
    /// 根据开始时间、持续天数、持续小时数计算出结束时间
    /// </summary>
    /// <param name="StartDateTime"></param>
    /// <param name="LeaveDays"></param>
    /// <param name="LeaveHour"></param>
    /// <returns></returns>
    private string GetEndDateTime(string StartDateTime, string LeaveDays, string LeaveHour, string UID = "")
    {
        string flag = "0", strTemp = "\"StartTime\":\"{0}\",\"EndDateTime\":\"{1}\",\"EndDate\":\"{2}\",\"EndTime\":\"{3}\"";
        if (string.IsNullOrEmpty(StartDateTime))
            StartDateTime = DateTime.Now.toDateFormat();

        string StartDate = StartDateTime.toDateFormat(),
               StartTime = StartDateTime.toDateFormat("HH:mm"),
               EndDateTime = StartDateTime,
               EndDate = EndDateTime.toDateFormat(),
               EndTime = EndDateTime.toDateFormat("HH:mm");

        try
        {
            //获取休息时间，默认休息时间45分钟
            int RestTime = 45;
            //如果用户有排班则获取排班的RestTime数据
            if (!string.IsNullOrEmpty(UID))
            {
                var getUserOnDuty = MicroHR.GetUserOnDuty(UID, StartDateTime);
                RestTime = getUserOnDuty.RestTime.toInt();  //班次休息时间
                StartTime = !string.IsNullOrEmpty(getUserOnDuty.StartTime) ? getUserOnDuty.StartTime : StartTime;  //班次开始时间
                int StandardOvertime = getUserOnDuty.StandardOvertime.toInt(); //班次标准上班时间 单位min

                //默认先根据开始时间得到结束时间（在新增时用于根据用户班次开始时间计算出结束时间填充给控件），后续若LeaveDays>0或者LeaveHour>0时再重新计算
                EndDateTime = DateTime.Parse(StartDate + " " + StartTime).AddMinutes(480 + RestTime).ToString("yyyy-MM-dd HH:mm:ss");
                EndDate = EndDateTime.toDateFormat();
                EndTime = EndDateTime.toDateFormat("HH:mm");
            }

            if (LeaveDays.toDecimal() > 0 || LeaveHour.toDecimal() > 0)
            {
                int Days = ((LeaveDays.toDecimal() / 1).toDecimalInt32()).toInt();
                decimal DaysHour = LeaveDays.toDecimal() % 1;  //天数取余*8 h，如0.5天*8转为小时数 (工作以8小时为1天所以*8)

                decimal Hour = LeaveHour.toDecimal() / 1;  //小时数
                int HourMin = (((LeaveHour.toDecimal() % 1) * 60).toDecimalInt32()).toInt();  //小时数取余*60 min，如0.5小时 * 60分钟转为分钟数

                //如果Days>=0 且DaysHour==0则补休息时间（先补时因为后面Days需要变化）
                if (Days >= 1 && DaysHour == 0)
                    HourMin = HourMin + RestTime;

                //如果是半天则*8小时(%1后的值)
                if (DaysHour > 0)
                    DaysHour = DaysHour * 8;

                //如果天数大于1且取余数等于0
                if (Days >= 1 && DaysHour == 0)
                    DaysHour = DaysHour * 8 + 8;

                //如果天数>=2则减1，因为上面已经加了8小时
                if (LeaveDays.toDecimal() >= 2)
                    Days = Days - 1;
                else if (LeaveDays.toDecimal() <= 1)  //否则如果<=1则为0 （如果是LeaveDays=1.5则不改变）
                    Days = 0;

                //重新计算结束时间
                EndDateTime = DateTime.Parse(StartDateTime).AddDays(Days).AddHours((DaysHour + Hour).toDecimalInt32().toInt()).AddMinutes(HourMin).ToString("yyyy-MM-dd HH:mm:ss");
                EndDate = EndDateTime.toDateFormat();
                EndTime = EndDateTime.toDateFormat("HH:mm");
            }
        }
        catch { }

        flag = string.Format(strTemp, StartTime, EndDateTime, EndDate, EndTime);
        flag = "[{" + flag + "}]";

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