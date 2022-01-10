<%@ WebHandler Language="C#" Class="OnDutyTpl" %>

using System;
using System.Web;
using MicroUserHelper;
using MicroAuthHelper;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroDTHelper;
using MicroPublicHelper;

public class OnDutyTpl : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string DeptIDs = context.Server.UrlDecode(context.Request.QueryString["DeptIDs"]),
               StartDate = context.Server.UrlDecode(context.Request.QueryString["StartDate"]),
               EndDate = context.Server.UrlDecode(context.Request.QueryString["EndDate"]),
               UserObj= context.Server.UrlDecode(context.Request.QueryString["UserObj"]);

        //DeptIDs = "85";
        //StartDate = "2021-08-30";
        //EndDate = "2021-09-05";

        if (!string.IsNullOrEmpty(DeptIDs) && !string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            GetOnDutyTpl(DeptIDs, StartDate, EndDate, UserObj);

    }


    private void GetOnDutyTpl(string DeptIDs, string StartDate, string EndDate, string UserObj)
    {
        var getDateTimeDiff = MicroPublic.GetDateTimeDiff(StartDate, EndDate);
        int TotalDays = getDateTimeDiff.TotalDays.toInt();
        Boolean IsFormalEmployee = UserObj.toBoolean();  //判断是否仅下载正式员工

        DataTable _dtInsert = new DataTable();
        _dtInsert.Columns.Add("部门", typeof(string));
        _dtInsert.Columns.Add("电脑ID", typeof(string));
        _dtInsert.Columns.Add("姓名", typeof(string));
        _dtInsert.Columns.Add("地点", typeof(string));

        if (TotalDays > 0)
        {
            //*****（全局）得到节假日 日期Start*****
            string _sql4 = "select * from CalendarDays where Invalid=0 and Del=0 and DayDate between @StartDay and @EndDay ";
            SqlParameter[] _sp4 = {
                new SqlParameter("@StartDay", SqlDbType.VarChar, 50),
                new SqlParameter("@EndDay", SqlDbType.VarChar, 50),
            };

            _sp4[0].Value = StartDate.toDateFormat();
            _sp4[1].Value = EndDate.toDateFormat();

            DataTable _dt4 = MsSQLDbHelper.Query(_sql4, _sp4).Tables[0];
            //*****（全局）得到假期日期End*****

            for (int i = 0; i <= TotalDays; i++)
            {
                string ColName = string.Empty,
                        OffDay = string.Empty,
                        CurrDate = StartDate.toDateTime().AddDays(i).ToString("yyyy-MM-dd"),
                        Week = MicroPublic.GetWeek(CurrDate, "WW");

                DataRow[] _rows4 = _dt4.Select("DayDate= '" + CurrDate + "'");
                if (_rows4.Length > 0)
                {
                    string DaysType = _rows4[0]["DaysType"].ToString();
                    if (DaysType == "OffDay")
                        OffDay = " 休";
                    else if (DaysType == "Statutory")
                        OffDay = " 法";
                }
                else
                {
                    if (Week == "周六" || Week == "周日")
                        OffDay = " 班";
                }

                ColName = CurrDate + "（" + Week + OffDay + "）";  //如：2021-08-07 （周六 班）

                _dtInsert.Columns.Add(ColName, typeof(string));
            }

            DeptIDs = MicroUserInfo.GetAllDeptsID("AllSubDepts", "DeptID", DeptIDs);

            string Where = string.Empty;

            //判断是否仅为正式员工
            if (IsFormalEmployee)
                Where = " and a.UserName like 'RL0%' ";

            string _sql = " select a.UID,a.UserName,a.ChineseName,c.LevelCode from UserInfo a left join UserDepts b on a.UID=b.UID left join Department c on b.DeptID=c.DeptID where a.Invalid=0 and a.Del=0 " + Where + " and a.UID in (select UID from UserDepts where Invalid=0 and Del=0 and DeptID in (" + DeptIDs + ")) order by c.LevelCode, a.UserName";
            DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

            if (_dt != null && _dt.Rows.Count > 0)
            {
                string _sql2 = "select * from UserDepts where Invalid=0 and Del=0 order by DeptID";
                DataTable _dt2 = MsSQLDbHelper.Query(_sql2).Tables[0];

                DataTable _dtDept = MicroDataTable.GetDataTable("Dept");

                //**************插入第一行例子Start****************
                DataRow _drInsert = _dtInsert.NewRow();
                _drInsert["部门"] = "1例：（该行请不要删除）";
                _drInsert["电脑ID"] = "RL000001";
                _drInsert["姓名"] = "张三";
                _drInsert["地点"] = "Test";

                for (int j = 0; j <= TotalDays; j++)
                {
                    //string ColName = StartDate.toDateTime().AddDays(j).ToString("yyyy-MM-dd");
                    //ColName = ColName + "（" + MicroPublic.GetWeek(ColName, "WW") + "）";

                    string ColName = string.Empty,
                            OffDay = string.Empty,
                            CurrDate = StartDate.toDateTime().AddDays(j).ToString("yyyy-MM-dd"),
                            Week = MicroPublic.GetWeek(CurrDate, "WW");

                    DataRow[] _rows4 = _dt4.Select("DayDate= '" + CurrDate + "'");
                    if (_rows4.Length > 0)
                    {
                        string DaysType = _rows4[0]["DaysType"].ToString();
                        if (DaysType == "OffDay")
                            OffDay = " 休";
                        else if (DaysType == "Statutory")
                            OffDay = " 法";
                    }
                    else
                    {
                        if (Week == "周六" || Week == "周日")
                            OffDay = " 班";
                    }

                    ColName = CurrDate + "（" + Week + OffDay + "）";  //如：2021-08-07 （周六 班）

                    if (j == 0)
                        _drInsert[ColName] = "GZ-平";
                    else if (j == 1)
                        _drInsert[ColName] = "GZ-1S";
                    else if (j == 2)
                        _drInsert[ColName] = "GZ-2S";
                    else if (j == 3)
                        _drInsert[ColName] = "GZ-3S";
                    else if (j == 4)
                        _drInsert[ColName] = "WH-平";
                    else if (j == 5)
                        _drInsert[ColName] = "WH-1S";
                    else if (j == 6)
                        _drInsert[ColName] = "WH-2S";
                    else if (j == 7)
                        _drInsert[ColName] = "WH-3S";
                    else
                        _drInsert[ColName] = "GZ-平";

                }
                _dtInsert.Rows.Add(_drInsert);
                //**************插入第一行例子Start****************

                //循环填充人员信息
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    int UID = _dt.Rows[i]["UID"].toInt();
                    string UserName = _dt.Rows[i]["UserName"].toStringTrim(),
                            ChineseName = _dt.Rows[i]["ChineseName"].toStringTrim();

                    int DeptID = _dt2.Select("UID=" + UID)[0]["DeptID"].toInt();

                    string DeptName = _dtDept.Select("DeptID=" + DeptID)[0]["DeptName"].toStringTrim();

                    DataRow _drInsert2 = _dtInsert.NewRow();

                    _drInsert2["部门"] = DeptName;
                    _drInsert2["电脑ID"] = UserName;
                    _drInsert2["姓名"] = ChineseName;
                    _drInsert2["地点"] = "Test";

                    _dtInsert.Rows.Add(_drInsert2);

                }

                //生成Excel并导出
                if (_dtInsert != null && _dtInsert.Rows.Count > 0)
                {
                    //按部门重新排序
                    //DataRow[] _rowsInsert = _dtInsert.Select("", "部门");
                    //通过DefaultView再次转为DataTable
                    //DataTable _dtFinal = _rowsInsert.CopyToDataTable().DefaultView.ToTable();

                    //导出到Excel
                    string FileName = "OnDuty_" + DateTime.Now.ToString("yyyyMMdd");
                    MicroDataTable.ExportExcel(_dtInsert, FileName);

                }

            }
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}