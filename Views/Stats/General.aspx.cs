using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroDTHelper;
using MicroPublicHelper;
using MicroAuthHelper;

public partial class Views_Stats_General : System.Web.UI.Page
{
    /// <summary>
    /// 早餐
    /// </summary>
    public int Meal0 = 0; 

    /// <summary>
    /// 午餐
    /// </summary>
    public int Meal1 = 0;

    /// <summary>
    /// 晚餐
    /// </summary>
    public int Meal2 = 0;

    /// <summary>
    /// 宵夜
    /// </summary>
    public int Meal3 = 0;

    /// <summary>
    /// 无
    /// </summary>
    public int Meal4 = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        //动作Action 可选值Add、Modify、View
        string Action = MicroPublic.GetFriendlyUrlParm(0);
        txtAction.Value = Action;

        string ShortTableName = MicroPublic.GetFriendlyUrlParm(1);
        txtShortTableName.Value = ShortTableName;

        string ModuleID = MicroPublic.GetFriendlyUrlParm(2);
        txtMID.Value = ModuleID;

        string StartDate = DateTime.Now.toDateFormat("yyyy-MM-dd"),
        EndDate = DateTime.Now.ToString("yyyy-MM-dd");

        txtStartDate.Value = StartDate;
        txtEndDate.Value = EndDate;

        //检查是否已经登录和页面唯一识别是否一致（ShortTableName）
        MicroAuth.CheckAuth(ModuleID, ShortTableName);

        //检查是否有页面浏览权限
        MicroAuth.CheckBrowse(ModuleID);

        GetOvertimeMeal();
        GetOnDuty();

    }

   /// <summary>
   /// 获取加班就餐数据
   /// </summary>
   /// <returns></returns>
    protected string GetOvertimeMeal()
    {
        string flag = string.Empty;

        DataTable _dt = MicroDataTable.GetDataTable("OvertimeMeal");

        if (_dt != null && _dt.Rows.Count > 0)
        {
            string Location = txtLocation.Value,
                StartDate = DateTime.Now.toDateFormat("yyyy-MM-dd"),
                EndDate = DateTime.Now.ToString("yyyy-MM-dd"),
                QueryFieldsStr = string.Empty,
                KeywordStr = string.Empty;

            if (!string.IsNullOrEmpty(Location))
            {
                Location = Location.toStringTrim();
                QueryFieldsStr = " and Location='" + Location + "'";
                KeywordStr = "Location_" + Location + ",Keyword_";  //构成：Location-Test,Keyword-午餐
            }

            for (int i = 0; i < _dt.Rows.Count; i++)
            {
                string OvertimeMealID = _dt.Rows[i]["OvertimeMealID"].toStringTrim(),
                OvertimeMealName = _dt.Rows[i]["OvertimeMealName"].toStringTrim(),
                LayHref = string.Empty;

                string _sql2 = " select OvertimeID,OvertimeMealID from HROvertime where Invalid=0 and Del=0 and ParentID<>0 and StateCode>=0 and CHARINDEX(','+convert(varchar," + OvertimeMealID.toInt() + ")+',',','+OvertimeMealID+',')>0 and OvertimeDate between @StartDate and @EndDate" + QueryFieldsStr;

                SqlParameter[] _sp2 = { new SqlParameter("@Location",SqlDbType.NVarChar,1000),
                                    new SqlParameter("@StartDate",SqlDbType.DateTime),
                                    new SqlParameter("@EndDate",SqlDbType.DateTime),
                                };
                _sp2[0].Value = Location.toStringTrim();
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

                if (Count > 0)
                    LayHref = "lay-href=\"/Views/Forms/MicroFormList/View/Overtime/4/5/1/DefaultNumber/GetOvertimeList/" + Server.UrlEncode(StartDate) + "/" + Server.UrlEncode(EndDate) + "/" + Server.UrlEncode(KeywordStr + OvertimeMealName) + "/OvertimeDate\"";

                //用于调整布局所占宽度
                string Size = i > 1 ? "4" : "6";
               
                flag += "<li class=\"layui-col-xs" + Size + "\">";
                flag += "<a id=\"aMeal2" + i.ToString() + "\" runat=\"server\" lay-text=\"就餐统计：" + OvertimeMealName + "\" class=\"layadmin-backlog-body\" " + LayHref + ">";
                flag += "<h3>" + OvertimeMealName + "</h3>";
                flag += "<p><cite id=\"citeMeal2" + i.ToString() + "\">" + Count + "</cite></p>";
                flag += "</a>";
                flag += "</li>";
            }
        }

        return flag;
    }


    /// <summary>
    /// 获取排班数据
    /// </summary>
    /// <returns></returns>
    protected string GetOnDuty()
    {
        string flag = string.Empty;

        DataTable _dt = MicroDataTable.GetDataTable("ShiftType");

        if (_dt != null && _dt.Rows.Count > 0)
        {
            string Location = txtLocation.Value,
                StartDate = DateTime.Now.toDateFormat("yyyy-MM-dd"),
                EndDate = DateTime.Now.ToString("yyyy-MM-dd"),
                QueryFieldsStr = string.Empty,
                KeywordStr = string.Empty;

            if (!string.IsNullOrEmpty(Location))
            {
                Location = Location.toStringTrim();
                QueryFieldsStr = " and Location='" + Location + "'";
                KeywordStr = "Location_" + Location + ",Keyword_";  //构成：Location-Test,Keyword-午餐
            }


            string _sql2 = "select * from HROnDutyForm a where ParentID<>0 and Invalid=0 and Del=0 and StateCode>=0 and DateCreated in (select max(b.DateCreated) from HROnDutyForm b where a.DutyUID=b.DutyUID and a.DutyDate=b.DutyDate) and DutyDate between  @StartDate and @EndDate" + QueryFieldsStr;

            SqlParameter[] _sp2 = { new SqlParameter("@Location",SqlDbType.NVarChar,1000),
                                    new SqlParameter("@StartDate",SqlDbType.DateTime),
                                    new SqlParameter("@EndDate",SqlDbType.DateTime),
                                };
            _sp2[0].Value = Location.toStringTrim();
            _sp2[1].Value = StartDate.toDateTime();
            _sp2[2].Value = EndDate.toDateTime();

            DataTable _dt2 = MsSQLDbHelper.Query(_sql2, _sp2).Tables[0];


            for (int i = 0; i < _dt.Rows.Count; i++)
            {
                string ShiftTypeID = _dt.Rows[i]["ShiftTypeID"].toStringTrim(),
                ShiftName = _dt.Rows[i]["Alias"].toStringTrim(),
                LayHref = string.Empty;

                int Count = _dt2.Compute("count(DutyUID)", "ShiftTypeID=" + ShiftTypeID).toInt();
                if (Count > 0)
                    LayHref = "lay-href=\"/Views/Forms/MicroFormList/View/OnDutyForm/4/13/1/DefaultNumber/GetOnDutyFormListByMeal/" + Server.UrlEncode(StartDate) + "/" + Server.UrlEncode(EndDate) + "/" + Server.UrlEncode(KeywordStr + ShiftName) + "/DutyDate\"";

                //获得GZ-平，和WH-平的数据
                if (ShiftTypeID.toInt() == 2 || ShiftTypeID.toInt() == 6)
                {
                    string Where = string.Empty;

                    if (ShiftTypeID.toInt() == 2)
                        Where = " and DeptID<>8 ";

                    else if (ShiftTypeID.toInt() == 6)
                        Where = " and DeptID=8 ";

                    string _sql3 = "select * from UserInfo where Invalid=0 and Del=0 and (UserName like 'RL%' or UserName like 'J0%')" +  // like 'RL%' or 'J0%' 员工

                    //排除已写了排班申请的 但 不是GZ-平或WH-平 且不是取消班次的
                    "and UID not in (select DutyUID from HROnDutyForm a where ParentID<>0 and Invalid = 0 and Del = 0 and StateCode>=0 and ShiftTypeID<>" + ShiftTypeID.toInt() + " and ShiftName<>'-' and DateCreated in (select max(b.DateCreated) from HROnDutyForm b where a.DutyUID = b.DutyUID and a.DutyDate = b.DutyDate) and DutyDate between @StartDate and @EndDate) " +

                    //排除已休假的
                    "and UID not in (select LeaveUID from HRLeave where Invalid = 0 and Del = 0 and StateCode> 0 and StartDate<= @EndDate and EndDate>= @StartDate)" +

                    //剩下需要上班的而又未写排班申请的
                    //DeptID=8 武汉的，DeptID<>8 武汉以外的
                    "and UID in (select UID from UserDepts where Invalid = 0 and Del = 0 and DeptID in(select DeptID from Department where Invalid = 0 and Del = 0 " + Where + "))";


                    SqlParameter[] _sp3 = { 
                                    new SqlParameter("@StartDate",SqlDbType.DateTime),
                                    new SqlParameter("@EndDate",SqlDbType.DateTime),
                                };
                    _sp3[0].Value = StartDate.toDateTime();
                    _sp3[1].Value = EndDate.toDateTime();

                    DataTable _dt3 = MsSQLDbHelper.Query(_sql3, _sp3).Tables[0];

                    //得到平时班的数量
                    Count = _dt3.Rows.Count;

                }

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


                //用于调整布局所占宽度
                string Size = "3";

                flag += "<li class=\"layui-col-xs" + Size + "\">";
                flag += "<a id=\"aShiftName3" + i.ToString() + "\" runat=\"server\" lay-text=\"排班统计：" + ShiftName + "\" class=\"layadmin-backlog-body\" " + LayHref + ">";
                flag += "<h3>" + ShiftName + "</h3>";
                flag += "<p><cite id=\"citeShiftName3" + i.ToString() + "\">" + Count + "</cite></p>";
                flag += "</a>";
                flag += "</li>";
            }
        }

        return flag;
    }

}