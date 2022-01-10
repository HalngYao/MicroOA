<%@ WebHandler Language="C#" Class="UserState" %>

using System;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroPublicHelper;
using MicroUserHelper;
using MicroAuthHelper;

public class UserState : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = MicroPublic.GetMsg("StateFailed"),
                Type = context.Server.UrlDecode(context.Request.Form["type"]),
                    StateCode = context.Server.UrlDecode(context.Request.Form["statecode"]),
                    StateName = context.Server.UrlDecode(context.Request.Form["statename"]),
                    DurationTime = context.Server.UrlDecode(context.Request.Form["durationtime"]),
                    CustomDurationTime = context.Server.UrlDecode(context.Request.Form["customdurationtime"]);


        ////测试数据
        //Type = "fixed";
        //StateCode = "100";
        //StateName = "有空";

        if (!string.IsNullOrEmpty(Type) && !string.IsNullOrEmpty(StateCode) && !string.IsNullOrEmpty(StateName))
            flag = SetUserState(Type, StateCode, StateName, DurationTime, CustomDurationTime);

        context.Response.Write(flag);
    }


    private string SetUserState(string Type, string StateCode, string StateName, string DurationTime, string CustomDurationTime)
    {
        string flag = MicroPublic.GetMsg("StateFailed"),
                UID = MicroUserInfo.GetUserInfo("UID");

        Type = Type.ToLower();

        //即固定的（有空、会议等），非自定义的，当前时间加上90天
        if (Type == "fixed")
            CustomDurationTime = DateTime.Now.AddDays(90).toDateFormat("yyyy-MM-dd HH:mm:ss");
        else
        {
            if (DurationTime == "30")
                CustomDurationTime = DateTime.Now.AddMinutes(30).toDateFormat("yyyy-MM-dd HH:mm:ss");
            else if (DurationTime == "1")
                CustomDurationTime = DateTime.Now.AddHours(1).toDateFormat("yyyy-MM-dd HH:mm:ss");
            else if (DurationTime == "2")
                CustomDurationTime = DateTime.Now.AddHours(2).toDateFormat("yyyy-MM-dd HH:mm:ss");
            else if (DurationTime == "3")
                CustomDurationTime = DateTime.Now.AddHours(3).toDateFormat("yyyy-MM-dd HH:mm:ss");
            else if (DurationTime == "4")
                CustomDurationTime = DateTime.Now.AddHours(4).toDateFormat("yyyy-MM-dd HH:mm:ss");
            else if (DurationTime == "ToDay")
                CustomDurationTime = DateTime.Now.toDateFormat("yyyy-MM-dd") + " 23:59:59.998";
            //else if (DurationTime == "Custom")
            //保持不变
        }


        string _sql = "select * from UserState where Invalid=0 and Del=0 and UID=" + UID;
        DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

        if (_dt != null && _dt.Rows.Count > 0)
        {
            string _sql2 = "update UserState set StateCode=@StateCode, StateName=@StateName, DurationTime=@DurationTime, DateModified=@DateModified where Invalid=0 and Del=0 and UID=" + UID;
            SqlParameter[] _sp2 = { new SqlParameter("@StateCode", SqlDbType.Int),
                    new SqlParameter("@StateName", SqlDbType.NVarChar,50),
                    new SqlParameter("@DurationTime", SqlDbType.DateTime),
                    new SqlParameter("@DateModified", SqlDbType.DateTime)};

            _sp2[0].Value = StateCode.toInt();
            _sp2[1].Value = StateName.toStringTrim();
            _sp2[2].Value = CustomDurationTime.toDateTime("yyyy-MM-dd HH:mm:ss");
            _sp2[3].Value = DateTime.Now;

            if (MsSQLDbHelper.ExecuteSql(_sql2, _sp2) > 0)
                flag = "True";
        }
        else
        {
            string _sql2 = "insert into UserState (UID, StateCode, StateName, DurationTime, DateModified) values (@UID, @StateCode, @StateName, @DurationTime, @DateModified)";
            SqlParameter[] _sp2 = { new SqlParameter("@UID", SqlDbType.Int),
                    new SqlParameter("@StateCode", SqlDbType.Int),
                    new SqlParameter("@StateName", SqlDbType.NVarChar,50),
                    new SqlParameter("@DurationTime", SqlDbType.DateTime),
                    new SqlParameter("@DateModified", SqlDbType.DateTime)};

            _sp2[0].Value = UID.toInt();
            _sp2[1].Value = StateCode.toInt();
            _sp2[2].Value = StateName.toStringTrim();
            _sp2[3].Value = CustomDurationTime.toDateTime("yyyy-MM-dd HH:mm:ss");
            _sp2[4].Value = DateTime.Now;

            if (MsSQLDbHelper.ExecuteSql(_sql2, _sp2) > 0)
                flag = "True";
        }

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