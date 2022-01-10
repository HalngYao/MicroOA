<%@ WebHandler Language="C#" Class="General" %>

using System;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroDTHelper;

public class General : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        string flag = "This tips.",
                Action = context.Server.UrlDecode(context.Request.QueryString["action"]),
                MID = context.Server.UrlDecode(context.Request.QueryString["mid"]),
                Type = context.Server.UrlDecode(context.Request.QueryString["type"]),
                StartDate = context.Server.UrlDecode(context.Request.QueryString["startdate"]),
                EndDate = context.Server.UrlDecode(context.Request.QueryString["enddate"]);

        //Type = "Test";
        //StartDate = "2021-10-13"; //DateTime.Now.toDateFormat("yyyy-MM-dd");
        //EndDate = "2021-10-13"; //DateTime.Now.ToString("yyyy-MM-dd");

        if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            flag = GetOvertimeMeal(Type, StartDate, EndDate);

        context.Response.Write(flag);
    }


    private string GetOvertimeMeal(string Location, string StartDate, string EndDate)
    {
        string flag = string.Empty,
                TotalData = string.Empty,
                MealData = string.Empty,
                DutyData = string.Empty,
               QueryFieldsStr = string.Empty;

        int Meal0 = 0,
                Meal1 = 0,
                Meal2 = 0,
                Meal3 = 0,
                Meal4 = 0;

        if (!string.IsNullOrEmpty(Location))
        {
            Location = Location.toStringTrim();
            QueryFieldsStr = " and Location='" + Location + "'";
        }

        //*****************统计加班就餐Start********************
        DataTable _dt = MicroDataTable.GetDataTable("OvertimeMeal");
        if (_dt != null && _dt.Rows.Count > 0)
        {
            for (int i = 0; i < _dt.Rows.Count; i++)
            {
                string OvertimeMealID = _dt.Rows[i]["OvertimeMealID"].toStringTrim(),
                OvertimeMealName = _dt.Rows[i]["OvertimeMealName"].toStringTrim();

                string _sql2 = " select OvertimeID from HROvertime where Invalid=0 and Del=0 and ParentID<>0 and StateCode>=0 and CHARINDEX(','+convert(varchar," + OvertimeMealID.toInt() + ")+',',','+OvertimeMealID+',')>0 and OvertimeDate between @StartDate and @EndDate" + QueryFieldsStr;

                SqlParameter[] _sp2 = { new SqlParameter("@Location",SqlDbType.NVarChar,1000),
                                    new SqlParameter("@StartDate",SqlDbType.DateTime),
                                    new SqlParameter("@EndDate",SqlDbType.DateTime),
                                };
                _sp2[0].Value = Location;
                _sp2[1].Value = StartDate.toDateTime();
                _sp2[2].Value = EndDate.toDateTime();

                DataTable _dt2 = MsSQLDbHelper.Query(_sql2, _sp2).Tables[0];

                int Count = _dt2.Rows.Count;


                if (i == 0)
                    Meal0 += Count;

                if (i == 1)
                    Meal1 += Count;

                if (i == 2)
                    Meal2 += Count;

                if (i == 3)
                    Meal3 += Count;

                if (i == 4)
                    Meal4 += Count;

                MealData += "{\"OvertimeMealName\":\"" + OvertimeMealName + "\", \"MealCount\":" + Count + "},";
            }

            if (!string.IsNullOrEmpty(MealData))
                MealData = MealData.Substring(0, MealData.Length - 1);
        }
        //*****************统计加班就餐End********************

        //*****************统计排班Start********************
        DataTable _dt3 = MicroDataTable.GetDataTable("ShiftType");
        if (_dt3 != null && _dt3.Rows.Count > 0)
        {
            //获取排班数据
            string _sql4 = "select * from HROnDutyForm a where ParentID<>0 and Invalid=0 and Del=0 and StateCode>=0 and DateCreated in (select max(b.DateCreated) from HROnDutyForm b where a.DutyUID=b.DutyUID and a.DutyDate=b.DutyDate) and DutyDate between  @StartDate and @EndDate" + QueryFieldsStr;

            SqlParameter[] _sp4 = { new SqlParameter("@Location",SqlDbType.NVarChar,1000),
                                    new SqlParameter("@StartDate",SqlDbType.DateTime),
                                    new SqlParameter("@EndDate",SqlDbType.DateTime),
                                };
            _sp4[0].Value = Location.toStringTrim();
            _sp4[1].Value = StartDate.toDateTime();
            _sp4[2].Value = EndDate.toDateTime();

            DataTable _dt4 = MsSQLDbHelper.Query(_sql4, _sp4).Tables[0];

            for (int i = 0; i < _dt3.Rows.Count; i++)
            {
                string ShiftTypeID = _dt3.Rows[i]["ShiftTypeID"].toStringTrim(),
                        ShiftName = _dt3.Rows[i]["Alias"].toStringTrim();
                int Count = _dt4.Compute("count(DutyUID)", "ShiftTypeID=" + ShiftTypeID).toInt();  //获取排班数据每个班次的总和

                //获得GZ-平，和WH-平的数据
                if (ShiftTypeID.toInt() == 2 || ShiftTypeID.toInt() == 6)
                {
                    string Where = string.Empty;

                    if (ShiftTypeID.toInt() == 2)  //获得GZ-平
                        Where = " and DeptID<>8 ";  //DeptID<>8 武汉以外的

                    else if (ShiftTypeID.toInt() == 6)  //WH-平的数据
                        Where = " and DeptID=8 ";  //DeptID=8 武汉的

                    string _sql5 = "select * from UserInfo where Invalid=0 and Del=0 and (UserName like 'RL%' or UserName like 'J0%')" +  // like 'RL%' or 'J0%' 员工

                    //排除已写了排班申请的 但 不是GZ-平或WH-平 且不是取消班次的
                    "and UID not in (select DutyUID from HROnDutyForm a where ParentID<>0 and Invalid = 0 and Del = 0 and StateCode>= 0 and ShiftTypeID<>" + ShiftTypeID.toInt() + " and ShiftName<>'-' and DateCreated in (select max(b.DateCreated) from HROnDutyForm b where a.DutyUID = b.DutyUID and a.DutyDate = b.DutyDate) and DutyDate between @StartDate and @EndDate) " +

                    //排除已休假的
                    "and UID not in (select LeaveUID from HRLeave where Invalid = 0 and Del = 0 and StateCode> 0 and StartDate<= @EndDate and EndDate>= @StartDate)" +

                    //剩下需要上班的而又未写排班申请的
                    //DeptID=8 武汉的，DeptID<>8 武汉以外的
                    "and UID in (select UID from UserDepts where Invalid = 0 and Del = 0 and DeptID in(select DeptID from Department where Invalid = 0 and Del = 0 " + Where + "))";

                    SqlParameter[] _sp5 = {
                                    new SqlParameter("@StartDate",SqlDbType.DateTime),
                                    new SqlParameter("@EndDate",SqlDbType.DateTime),
                                };
                    _sp5[0].Value = StartDate.toDateTime();
                    _sp5[1].Value = EndDate.toDateTime();

                    DataTable _dt5 = MsSQLDbHelper.Query(_sql5, _sp5).Tables[0];

                    //得到平时班的数量
                    Count = _dt5.Rows.Count;
                }

                DutyData += "{\"ShiftName\":\"" + ShiftName + "\", \"DutyCount\":" + Count + "},";

                //GZ-1S
                if (ShiftTypeID.toInt() == 1)
                    Meal1 += Count;  //午餐 = 加班午餐 + GZ1S

                //GZ-2S
                if (ShiftTypeID.toInt() == 3)
                {
                    Meal2 += Count;   //晩餐 = 加班晚餐 + GZ2S
                    Meal3 += Count;  //宵夜 = 加班宵夜 + GZ2S + GZ3S
                }

                //GZ-3S
                if (ShiftTypeID.toInt() == 4)
                {
                    Meal3 += Count;  //宵夜 = 加班宵夜 + GZ2S + GZ3S
                    Meal0 += Count; //早餐 = 加班早餐 + GZ3S                 
                }

            }

            if (!string.IsNullOrEmpty(DutyData))
                DutyData = DutyData.Substring(0, DutyData.Length - 1);

        }
        //*****************统计排班End********************

        TotalData += "{\"TotalName\":\"Meal0\", \"TotalCount\":" + Meal0 + "},";
        TotalData += "{\"TotalName\":\"Meal1\", \"TotalCount\":" + Meal1 + "},";
        TotalData += "{\"TotalName\":\"Meal2\", \"TotalCount\":" + Meal2 + "},";
        TotalData += "{\"TotalName\":\"Meal3\", \"TotalCount\":" + Meal3 + "},";
        TotalData += "{\"TotalName\":\"Meal4\", \"TotalCount\":" + Meal4 + "}";

        flag = "{\"totalData\": [" + TotalData + "], \"mealData\": [" + MealData + "], \"dutyData\": [" + DutyData + "]}";


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