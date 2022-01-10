<%@ WebHandler Language="C#" Class="Users" %>

using System;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MicroDBHelper;
using MicroPublicHelper;
using MicroDTHelper;
using MicroAuthHelper;
using MicroUserHelper;
using MicroHRHelper;

public class Users : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    //****************
    //说明
    //根据表名返回表的内容
    //如父项和子项，主要菜单和子菜单等
    //****************

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = MicroPublic.GetMsg("SubmitFailed"),
                    ModuleID = context.Server.UrlDecode(context.Request.QueryString["ModuleID"]),
                    Action = context.Server.UrlDecode(context.Request.QueryString["Action"]),
                    ShortTableName = context.Server.UrlDecode(context.Request.QueryString["STN"]),
                    DataType = context.Server.UrlDecode(context.Request.QueryString["DataType"]),
                    QueryType = context.Server.UrlDecode(context.Request.QueryString["QueryType"]),
                    Keyword = context.Server.UrlDecode(context.Request.QueryString["Keyword"]),
                    StartDate = context.Server.UrlDecode(context.Request.QueryString["StartDate"]),
                    EndDate = context.Server.UrlDecode(context.Request.QueryString["EndDate"]),
                    Page = context.Server.UrlDecode(context.Request.QueryString["page"]),
                    Limit = context.Server.UrlDecode(context.Request.QueryString["limit"]);

        ////测试数据
        //Action = "View";
        //ShortTableName = "Overtime";
        //ModuleID = "37";
        //DataType = "GetPersonalOvertime";
        //StartDate = "2021-02-01";
        //EndDate = "2021-02-28";
        //QueryType = "Dept";
        //Keyword = "82";

        Page = string.IsNullOrEmpty(Page) == true ? "1" : Page;
        Limit = string.IsNullOrEmpty(Limit) == true ? "10" : Limit;

        if (!string.IsNullOrEmpty(ModuleID) && !string.IsNullOrEmpty(ShortTableName))
            flag = GetUserOvertime(ModuleID, ShortTableName, DataType, QueryType, Keyword, StartDate, EndDate, Page, Limit);

        context.Response.Write(flag);
    }


    private string GetUserOvertime(string ModuleID, string ShortTableName, string DataType = "", string QueryType = "Dept", string Keyword = "All", string StartDate = "", string EndDate = "", string Page = "1", string Limit = "10")
    {
        string flag = string.Empty,
                UID = MicroUserInfo.GetUserInfo("UID"),
                DeptIDs = "0";
        int TotalCount = 0;

        //如果开始时间和结束时间为空时，默认取当前日期的第一天和最后一天
        if (string.IsNullOrEmpty(StartDate) && string.IsNullOrEmpty(EndDate))
        {
            StartDate = DateTime.Now.ToString().toDateMFirstDay()+ " 00:00:00.000";
            EndDate = DateTime.Now.ToString().toDateMLastDay() + " 23:59:59.998";
        }
        else
        {
            StartDate = StartDate.toDateFormat() + " 00:00:00.000";
            EndDate = EndDate.toDateFormat() + " 23:59:59.998";
        }

        //获取用户部门
        DataTable _dtUserDepts = MicroDataTable.GetAllUserAttrDataTable("UserDepts");

        //默认只能查看自己
        string _sql = "select * from UserInfo where Invalid=0 and Del=0 and UID in (" + UID.toInt() + ")";
        DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

        string Where = string.Empty;  //只能查看自己时带上条件

        if (MicroAuth.CheckPermit(ModuleID, "6")) //查看所有       
            DeptIDs = MicroUserInfo.GetUserInfo("AllDeptsID");
        else if (MicroAuth.CheckPermit(ModuleID, "19"))  //查看自部门及自部门所有的父部门及所有父部门包含的子部门      
            DeptIDs = MicroUserInfo.GetUserInfo("ParentSubDeptsID");
        else if (MicroAuth.CheckPermit(ModuleID, "20"))  //查看科内，科内及科内所有子部门      
            DeptIDs = MicroUserInfo.GetUserInfo("SectionDeptsID");
        else if (MicroAuth.CheckPermit(ModuleID, "17"))  //查看自部门含子部门      
            DeptIDs = MicroUserInfo.GetUserInfo("SubDeptsID");
        else
        {
            DeptIDs = MicroUserInfo.GetUserInfo("SubDeptsID");
            Where = " and UID in (" + UID.toInt() + ")";  //只能查看自己时带上条件
        }

        //如果按部门（QueryType == "Dept"）进行查询时以下条件符合则显示（通常发生在默认打开页面时）
        if (QueryType == "Dept" && Keyword == "All" && DeptIDs != "0")
        {
            _sql = "select * from UserInfo where Invalid=0 and Del=0 and UID in (select UID from UserDepts where Invalid=0 and Del=0 and DeptID in(" + DeptIDs + ")) " + Where;
            _dt = MsSQLDbHelper.Query(_sql).Tables[0];
        }

        else if (QueryType == "Dept" && Keyword.toInt() > 0) //如果点选部门进行查询（此时Keyword取到的是DeptID的值），先判断是否有权限查询该部门
        {
            if (MicroAuth.CheckStrofStrs(Keyword, DeptIDs))  //判断是否有权限查询该部门，如果有权限查询，则把Keyword赋值给DeptIDs
            {
                string SourceStrs = MicroUserInfo.GetAllDeptsID("AllSubDepts", "DeptID", Keyword);
                DeptIDs = MicroPublic.GetSourcesOfTargets(SourceStrs, DeptIDs);
                if (string.IsNullOrEmpty(DeptIDs))
                    DeptIDs = "0";
            }
            else   //如果没有权限查询，则把0赋值给DeptIDs
                DeptIDs = "0";

            _sql = "select * from UserInfo where Invalid=0 and Del=0 and UID in (select UID from UserDepts where Invalid=0 and Del=0 and DeptID in(" + DeptIDs + ")) " + Where;
            _dt = MsSQLDbHelper.Query(_sql).Tables[0];
        }

        else if (QueryType == "Search" && string.IsNullOrEmpty(Keyword))  //如果点按钮进行查询并且Keyword为空时（即QueryType == "Search")且Keyword为空时。（通常会查询UserInfo表的UserName、ChineseName、EnglishName、AdDisplayName等）
        {
            //步骤：先进行模糊查询得到User信息，再根据DetpIDs查询UserDepts表得到的UID in运算
            _sql = "select * from UserInfo where Invalid=0 and Del=0 and UID in (select UID from UserDepts where Invalid=0 and Del=0 and DeptID in(" + DeptIDs + ")) " + Where;
            _dt = MsSQLDbHelper.Query(_sql).Tables[0];
        }

        else if (QueryType == "Search" && !string.IsNullOrEmpty(Keyword))  //如果点按钮进行查询并且Keyword不为空时（即QueryType == "Search")且Keyword不为空时。（通常会查询UserInfo表的UserName、ChineseName、EnglishName、AdDisplayName等）
        {
            //步骤：先进行模糊查询得到User信息，再根据DetpIDs查询UserDepts表得到的UID in运算

            _sql = "select * from UserInfo where Invalid=0 and Del=0 and (UserName like @Keyword or ChineseName like @Keyword or EnglishName like @Keyword or EMail like @Keyword or WorkTel like @Keyword or WorkMobilePhone like @Keyword or WeChat like @Keyword or WorkWeChat like @Keyword or QQ like @Keyword or WorkQQ like @Keyword or AdDisplayName like @Keyword or AdDescription like @Keyword or AdDepartment like @Keyword or Description like @Keyword ) and UID in (select UID from UserDepts where Invalid=0 and Del=0 and DeptID in(" + DeptIDs + ")) " + Where;
            SqlParameter[] _sp = { new SqlParameter("@Keyword", SqlDbType.VarChar) };
            _sp[0].Value = "%" + Keyword.toJsonTrim() + "%";

            _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];
        }


        if (_dt != null && _dt.Rows.Count > 0)
        {
            string strTemp = GetStrTpl();
            TotalCount = _dt.Rows.Count;  //如果有记录先取得总记录数，避免取到分页后的数值

            StringBuilder sb = new StringBuilder();

            //返回相关数据表
            DataTable _dtUserJobTitle = MicroDataTable.GetAllUserAttrDataTable("UserJobTitle"),
                    //_dtUserRoles = MicroDataTable.GetAllUserAttrDataTable("UserRoles"),  //暂时不获取用户角色数据
                    _dtUserWorkHourSystem = MicroDataTable.GetAllUserAttrDataTable("UserWorkHourSystem", StartDate, EndDate),
                    _dtUserOvertime = MicroDataTable.GetAllUserAttrDataTable("UserOvertime", StartDate, EndDate, " and StateCode=100"),
                    _dtUserLeave = MicroDataTable.GetAllUserAttrDataTable("UserLeave", StartDate, EndDate, " and StateCode=100"),
                    _dtUserAnnualLeave = MicroDataTable.GetAllUserAttrDataTable("UserRemainingLeave", StartDate, EndDate, ""),
                    _dtCalendarDays = MicroDataTable.GetCalendarDays(StartDate, EndDate);

            string Offdays = "'" + StartDate.toDateTime().AddDays(-1).ToString("yyyy-MM-dd") + "'",
                    Statutorys = "'" + StartDate.toDateTime().AddDays(-1).ToString("yyyy-MM-dd") + "'";

            //得到休日日期
            DataRow[] _rowsOffdays = _dtCalendarDays.Select("(DaysType = 'OffDay')");
            if (_rowsOffdays.Length > 0)
            {
                foreach (DataRow _dr in _rowsOffdays)
                {
                    Offdays += ",'" + _dr["DayDate"].toStringTrim() + "'";
                }
            }

            //得到法定日期
            DataRow[] _rowsStatutorys = _dtCalendarDays.Select("(DaysType = 'Statutory')");
            if (_rowsStatutorys.Length > 0)
            {
                foreach (DataRow _dr in _rowsStatutorys)
                {
                    Statutorys += ",'" + _dr["DayDate"].toStringTrim() + "'";
                }
            }


            //实现分页
            _dt = _dt.AsEnumerable().Skip((Page.toInt() - 1) * Limit.toInt()).Take(Limit.toInt()).CopyToDataTable<DataRow>();

            for (int i = 0; i < _dt.Rows.Count; i++)
            {
                string UserID = string.Empty, DeptName = string.Empty, UserName = string.Empty, DisplayName = string.Empty, ChineseName = string.Empty, EnglishName = string.Empty, AdDisplayName = string.Empty, EMail = string.Empty, WorkTel = string.Empty, WorkMobilePhone = string.Empty, JobTitle = string.Empty, WorkHourSysName = string.Empty, ShiftName = string.Empty, WorkOvertime = "0", OffdayOvertime = "0", StatutoryOvertime = "0", OvertimeTotal = "0", AlreadyDaiXiu = "0", ExceptDaiXiu = "0", WarningValue = "0", LeaveTotal = "0", RemainingAnnualLeave = "0";

                UserID = _dt.Rows[i]["UID"].toStringTrim();

                UserName = _dt.Rows[i]["UserName"].toStringTrim();
                ChineseName = _dt.Rows[i]["ChineseName"].toStringTrim();
                EnglishName = _dt.Rows[i]["EnglishName"].toStringTrim();
                AdDisplayName = _dt.Rows[i]["AdDisplayName"].toStringTrim();
                DisplayName = MicroUserInfo.GetUserInfo(UserName, ChineseName, EnglishName, AdDisplayName);

                EMail = _dt.Rows[i]["EMail"].toStringTrim();
                WorkTel = _dt.Rows[i]["WorkTel"].toStringTrim();
                WorkMobilePhone = _dt.Rows[i]["WorkMobilePhone"].toStringTrim();

                DataRow[] _rowsUserDepts = _dtUserDepts.Select("UID=" + UserID.toInt(), "Sort desc"); //降序
                DataRow[] _rowsUserJobTitle = _dtUserJobTitle.Select("UID=" + UserID.toInt(), "Sort asc"); //升序

                // DataRow[] _rowsUserRoles = _dtUserRoles.Select("UID=" + UserID.toInt());
                DataRow[] _rowsUserWorkHourSystem = _dtUserWorkHourSystem.Select("UID=" + UserID.toInt() + " and WorkHourSysDate>='" + StartDate + "' and WorkHourSysDate<='" + EndDate + "'", "WorkHourSysDate desc, DateModified desc");
                DataRow[] _rowsUserOvertime = _dtUserOvertime.Select("OvertimeUID=" + UserID.toInt(), "");
                DataRow[] _rowsUserLeave = _dtUserLeave.Select("LeaveUID=" + UserID.toInt(), "");  //共休假，任何假期类别
                DataRow[] _rowsRemainingAnnualLeave = _dtUserAnnualLeave.Select("UID=" + UserID.toInt() + " and HolidayTypeID=1", ""); //剩余年休假 HolidayTypeID=1

                if (_rowsUserDepts.Length > 0)
                    DeptName = _rowsUserDepts[0]["DeptName"].toStringTrim();


                if (_rowsUserJobTitle.Length > 0)
                {
                    foreach (DataRow _dr in _rowsUserJobTitle)
                    {
                        JobTitle += _dr["JobTitleName"].toStringTrim() + "/";
                    }

                    JobTitle = JobTitle.Substring(0, JobTitle.Length - 1);
                }


                if (_rowsUserWorkHourSystem.Length > 0)
                {
                    WorkHourSysName = _rowsUserWorkHourSystem[0]["WorkHourSysName"].toStringTrim();
                    WarningValue = _rowsUserWorkHourSystem[0]["WarningValue"].toDecimal().toStringTrim();
                }
                else
                {
                    _rowsUserWorkHourSystem = _dtUserWorkHourSystem.Select("UID=" + UserID.toInt(), "WorkHourSysDate desc, DateModified desc");
                    if (_rowsUserWorkHourSystem.Length > 0)
                    {
                        WorkHourSysName = _rowsUserWorkHourSystem[0]["WorkHourSysName"].toStringTrim();
                        WarningValue = _rowsUserWorkHourSystem[0]["WarningValue"].toDecimal().toStringTrim();
                    }
                    else
                    {
                        WorkHourSysName = "未设置";
                        WarningValue = "28.0";
                    }
                }


                if (_rowsUserOvertime.Length > 0)
                {
                    DataTable _dtUserOvertime2 = _rowsUserOvertime.CopyToDataTable();  //把DataRow Copy到DataTable，方便使用Compute命令
                    OvertimeTotal = _dtUserOvertime2.Compute("sum(OvertimeHour)", "").toDecimal().ToString();

                    //得到延时加班总小时数（采用加班总记录排除休日和法定日的方法）
                    WorkOvertime = _dtUserOvertime2.Compute("sum(OvertimeHour)", "OvertimeDate not in (" + Offdays + "," + Statutorys + ")").toDecimal().ToString();

                    //得到休日加班总小时数
                    OffdayOvertime = _dtUserOvertime2.Compute("sum(OvertimeHour)", "OvertimeDate in (" + Offdays + ")").toDecimal().ToString();

                    //得到法定日加班总小时数
                    StatutoryOvertime = _dtUserOvertime2.Compute("sum(OvertimeHour)", "OvertimeDate in (" + Statutorys + ")").toDecimal().ToString();
                }

                //计算用户已代休和剔除代休后的加班小时数
                DataRow[] _rowsUserDaiXiu = _dtUserLeave.Select("LeaveUID=" + UserID.toInt() + " and HolidayTypeID=2", "");  //得到用户代休假的天数HolidayTypeID=2
                //代休记录不为空
                if (_rowsUserDaiXiu.Length > 0)
                {
                    DataTable _dtUserDaiXiu = _rowsUserDaiXiu.CopyToDataTable();  //把DataRow Copy到DataTable
                    string LeaveDays = _dtUserDaiXiu.Compute("sum(LeaveDays)", "").toDecimal().toStringTrim();  //得到用户代休假的天数汇总
                    AlreadyDaiXiu = (LeaveDays.toDecimal() * 8).toDecimal().toStringTrim();  //得到已代休的天数 *8 转为小时数  
                }

                //加班小时数大于0时
                if (OvertimeTotal.toDecimal() > 0)
                    ExceptDaiXiu = (OvertimeTotal.toDecimal() - AlreadyDaiXiu.toDecimal()).toDecimal().toStringTrim(); //加班总时间减去已代休时间得到剔除代休后的加班时间

                if (_rowsUserLeave.Length > 0)
                {
                    DataTable _dtUserLeave2 = _rowsUserLeave.CopyToDataTable();  //把DataRow Copy到DataTable
                    string LeaveDays = _dtUserLeave2.Compute("sum(LeaveDays)", "").toDecimal().ToString(),
                            IntLeaveDays = LeaveDays.toDecimal().toDecimalInt32().ToString(),  //取整
                            RemLeaveDays = ((LeaveDays.toDecimal() % 1) * 8).ToString(),  //取余
                            LeaveHour = (_dtUserLeave2.Compute("sum(LeaveHour)", "").toDecimal()+ RemLeaveDays.toDecimal()).toDecimalInt32().ToString();

                    LeaveTotal = IntLeaveDays + " 天，" + LeaveHour + " 小时";
                }

                if (_rowsRemainingAnnualLeave.Length > 0)
                    RemainingAnnualLeave = _rowsRemainingAnnualLeave[0]["Days"].toDecimal().toStringTrim();

                string Str = string.Format(strTemp, UserID, UserName, EMail, DeptName, DisplayName, JobTitle, WorkTel, WorkMobilePhone, WorkHourSysName, ShiftName, WorkOvertime, OffdayOvertime, StatutoryOvertime, OvertimeTotal, AlreadyDaiXiu, ExceptDaiXiu, WarningValue, RemainingAnnualLeave, LeaveTotal);
                sb.Append("{" + Str + "},");

            }

            string json = sb.ToString();
            flag = json.Substring(0, json.Length - 1);

        }
        flag = "{\"code\": 0,\"msg\": \"\",\"count\": " + TotalCount + ",\"data\":  [" + flag + "] }";

        return flag;
    }



    private string GetStrTpl()
    {
        string strTpl = "\"UID\":\"{0}\", \"UserName\":\"{1}\",\"EMail\":\"{2}\",\"DeptName\":\"{3}\", \"DisplayName\":\"{4}\", \"JobTitle\":\"{5}\", \"WorkTel\":\"{6}\", \"WorkMobilePhone\":\"{7}\", \"WorkHourSysName\":\"{8}\", \"ShiftName\":\"{9}\", \"WorkOvertime\":\"{10}\", \"OffdayOvertime\":\"{11}\", \"StatutoryOvertime\":\"{12}\", \"OvertimeTotal\":\"{13}\", \"AlreadyDaiXiu\":\"{14}\", \"ExceptDaiXiu\":\"{15}\", \"WarningValue\":\"{16}\", \"RemainingAnnualLeave\":\"{17}\", \"LeaveTotal\":\"{18}\"";

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