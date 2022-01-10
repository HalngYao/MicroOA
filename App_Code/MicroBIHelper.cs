using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using MicroDBHelper;
//using MicroSysInfoHelper;
using MicroPublicHelper;

/// <summary>
/// DataTable数据表
/// </summary>
namespace MicroBIHelper
{
    public class MicroBI
    {
        /// <summary>
        /// 获取申请量数据（首页2数字用）
        /// </summary>
        /// <param name="QueryType"></param>
        /// <param name="CustomDate"></param>
        /// <returns></returns>
        public static string GetApplyByQty(string QueryType, string CustomDate = "")
        {
            string strTpl = "\"currQty\":\"{0}\", \"preQty\":\"{1}\", \"diffQty\":\"{2}\"";

            string flag = string.Empty,
                StartDate = DateTime.Now.toDateFormat("yyyy-MM-dd") + " 00:00:00.000",
                EndDate = DateTime.Now.toDateFormat("yyyy-MM-dd") + " 23:59:59.998",
                PreStartDate = DateTime.Now.AddDays(-1).toDateFormat("yyyy-MM-dd") + " 00:00:00.000", //上一天
                PreEndDate = DateTime.Now.AddDays(-1).toDateFormat("yyyy-MM-dd") + " 23:59:59.998";

            CustomDate = string.IsNullOrEmpty(CustomDate) ? DateTime.Now.toDateFormat("yyyy-MM-dd") : CustomDate;

            int CurrQty = 0,
                PreQty = 0,
                DiffQty = 0;

            switch (QueryType.ToLower())
            {
                case "day":
                    StartDate = CustomDate.toDateFormat("yyyy-MM-dd") + " 00:00:00.000";
                    EndDate = CustomDate.toDateFormat("yyyy-MM-dd") + " 23:59:59.998";
                    PreStartDate = CustomDate.toDateTime().AddDays(-1).toDateFormat("yyyy-MM-dd") + " 00:00:00.000";
                    PreEndDate = CustomDate.toDateTime().AddDays(-1).toDateFormat("yyyy-MM-dd") + " 23:59:59.998";
                    break;

                case "week":
                    StartDate = CustomDate.toDateWFirstDay() + " 00:00:00.000";
                    EndDate = CustomDate.toDateWLastDay() + " 23:59:59.998";
                    PreStartDate = CustomDate.toDateTime().AddDays(-7).toDateWFirstDay() + " 00:00:00.000";
                    PreEndDate = CustomDate.toDateTime().AddDays(-7).toDateWLastDay() + " 23:59:59.998";
                    break;

                case "month":
                    StartDate = CustomDate.toDateMFirstDay() + " 00:00:00.000";
                    EndDate = CustomDate.toDateMLastDay() + " 23:59:59.998";
                    PreStartDate = CustomDate.toDateTime().AddMonths(-1).toDateMFirstDay() + " 00:00:00.000";
                    PreEndDate = CustomDate.toDateTime().AddMonths(-1).toDateMLastDay() + " 23:59:59.998";
                    break;

                case "year":
                    StartDate = CustomDate.toDateFormat("yyyy") + "-01-01 00:00:00.000";
                    EndDate = CustomDate.toDateFormat("yyyy") + "-12-31 23:59:59.998";
                    PreStartDate = CustomDate.toDateTime().AddYears(-1).toDateFormat("yyyy") + "-01-01 00:00:00.000";
                    PreEndDate = CustomDate.toDateTime().AddYears(-1).toDateFormat("yyyy") + "-12-31 23:59:59.998";
                    break;
            }

            //当前日期数据
            string _sql = "select a.FormID, b.FormName ,count(distinct FormsID) as Total from FormApprovalRecords a left join Forms b on a.FormID = b.FormID where a.DateCreated between @StartDate and @EndDate group by a.FormID,b.FormName order by FormID";

            SqlParameter[] _sp = { new SqlParameter("@StartDate",SqlDbType.VarChar,30),
                                    new SqlParameter("@EndDate",SqlDbType.VarChar,30)
                    };

            _sp[0].Value = StartDate;
            _sp[1].Value = EndDate;

            DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];
            if (_dt != null && _dt.Rows.Count > 0)
                CurrQty = _dt.Compute("sum(Total)", "").toInt();


            //上一日期数据
            string _sql2 = "select a.FormID, b.FormName ,count(distinct FormsID) as Total from FormApprovalRecords a left join Forms b on a.FormID = b.FormID where a.DateCreated between @StartDate and @EndDate group by a.FormID,b.FormName order by FormID";

            SqlParameter[] _sp2 = { new SqlParameter("@StartDate",SqlDbType.VarChar,30),
                                    new SqlParameter("@EndDate",SqlDbType.VarChar,30)
                    };

            _sp2[0].Value = PreStartDate;
            _sp2[1].Value = PreEndDate;

            DataTable _dt2 = MsSQLDbHelper.Query(_sql2, _sp2).Tables[0];
            if (_dt2 != null && _dt2.Rows.Count > 0)
                PreQty = _dt2.Compute("sum(Total)", "").toInt();

            //差异数据
            DiffQty = CurrQty - PreQty;


            flag = "{" + string.Format(strTpl, CurrQty, PreQty, DiffQty) + "}";

            return flag;

        }

        /// <summary>
        /// 获取柱状图格式的申请数据（首页2）
        /// </summary>
        /// <param name="QueryType"></param>
        /// <param name="CustomDate"></param>
        /// <returns></returns>
        public static string GetApplyByChart(string QueryType = "Year", string CustomDate = "")
        {
            string flag = string.Empty,
                    lData = string.Empty,
                    xData = string.Empty,
                    series = string.Empty,
                    Total = string.Empty;

            QueryType = QueryType.ToLower();

            string strTpl = "\"lData\":\"{0}\", \"xData\":\"{1}\", \"series\":\"{2}\"";


            string GStartDate = DateTime.Now.toDateFormat("yyyy") + "-01-01 00:00:00.000",
                GEndDate = DateTime.Now.toDateFormat("yyyy") + "-12-31 23:59:59.998",
                StartDate = DateTime.Now.toDateFormat("yyyy-MM-dd") + " 00:00:00.000",
                EndDate = DateTime.Now.toDateFormat("yyyy-MM-dd") + " 23:59:59.998";


            CustomDate = string.IsNullOrEmpty(CustomDate) ? DateTime.Now.toDateFormat("yyyy-MM-dd") : CustomDate;

            int MaxValue = 0; //天=24H，周=7天， 月=30天，年=12个月

            switch (QueryType)
            {
                case "day":
                    GStartDate = CustomDate.toDateFormat("yyyy-MM-dd") + " 00:00:00.000";
                    GEndDate = CustomDate.toDateFormat("yyyy-MM-dd") + " 23:59:59.998";

                    StartDate = GStartDate;
                    EndDate = StartDate.toDateFormat("yyyy-MM-dd") + " 00:59:59.998"; //以一个小时为单位

                    MaxValue = 23;
                    break;

                case "week":
                    GStartDate = CustomDate.toDateWFirstDay() + " 00:00:00.000";
                    GEndDate = CustomDate.toDateWLastDay() + " 23:59:59.998";

                    StartDate = GStartDate;
                    EndDate = StartDate.toDateFormat("yyyy-MM-dd") + " 23:59:59.998";

                    MaxValue = 7;
                    break;

                case "month":
                    GStartDate = CustomDate.toDateMFirstDay() + " 00:00:00.000";
                    GEndDate = CustomDate.toDateMLastDay() + " 23:59:59.998";

                    StartDate = GStartDate;
                    EndDate = StartDate.toDateFormat("yyyy-MM-dd") + " 23:59:59.998";

                    MaxValue = EndDate.toDateMLastDay("dd").toInt();
                    break;

                case "year":
                    GStartDate = CustomDate.toDateFormat("yyyy") + "-01-01 00:00:00.000";
                    GEndDate = CustomDate.toDateFormat("yyyy") + "-12-31 23:59:59.998";

                    StartDate = GStartDate;
                    EndDate = GStartDate.toDateMLastDay() + " 23:59:59.998";

                    MaxValue = 12;
                    break;
            }


            //获取表单名称
            string _sql = "select FormID, Sort, FormName from Forms where Invalid=0 and Del=0 order by Sort";
            DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

            //获取每张表单每个月的申请数量
            string _sql3 = "select a.FormID, b.FormName, count(distinct FormsID) as Total, a.DateCreated from FormApprovalRecords a left join Forms b on a.FormID=b.FormID where b.Invalid=0 and b.Del=0 and a.DateCreated between @GStartDate and @GEndDate group by a.FormID,b.FormName, a.DateCreated";

            SqlParameter[] _sp3 = { new SqlParameter("@GStartDate",SqlDbType.VarChar,30),
                                    new SqlParameter("@GEndDate",SqlDbType.VarChar,30),
                                };

            _sp3[0].Value = GStartDate;
            _sp3[1].Value = GEndDate;

            DataTable _dt3 = MsSQLDbHelper.Query(_sql3, _sp3).Tables[0];

            if (_dt != null && _dt.Rows.Count > 0 && MaxValue > 0)
            {
                for (int i = 0; i < MaxValue; i++)
                {
                    string startDate = StartDate,
                        endDate = EndDate;

                    if (QueryType == "day")
                    {
                        xData += StartDate.toDateTime("yyyy-MM-dd HH:mm:ss.fff").AddHours(i + 1).ToString("HH:mm") + ",";

                        startDate = StartDate.toDateTime("yyyy-MM-dd HH:mm:ss.fff").AddHours(i).ToString("yyyy-MM-dd HH:mm:ss.fff");
                        endDate = EndDate.toDateTime("yyyy-MM-dd HH:mm:ss.fff").AddHours(i).ToString("yyyy-MM-dd HH:mm:ss.fff");
                    }
                    else if (QueryType == "week")
                    {
                        string CurrDate = StartDate.toDateTime("yyyy-MM-dd HH:mm:ss.fff").AddDays(i).ToString("yyyy-MM-dd");
                        xData += CurrDate.toDateWeek() + ",";

                        startDate = StartDate.toDateTime("yyyy-MM-dd HH:mm:ss.fff").AddDays(i).ToString("yyyy-MM-dd HH:mm:ss.fff");
                        endDate = EndDate.toDateTime("yyyy-MM-dd HH:mm:ss.fff").AddDays(i).ToString("yyyy-MM-dd HH:mm:ss.fff");
                    }
                    else if (QueryType == "month")
                    {
                        string CurrDate = StartDate.toDateTime("yyyy-MM-dd HH:mm:ss.fff").AddDays(i).ToString("MM-dd");
                        xData += CurrDate + ",";

                        startDate = StartDate.toDateTime("yyyy-MM-dd HH:mm:ss.fff").AddDays(i).ToString("yyyy-MM-dd HH:mm:ss.fff");
                        endDate = EndDate.toDateTime("yyyy-MM-dd HH:mm:ss.fff").AddDays(i).ToString("yyyy-MM-dd HH:mm:ss.fff");
                    }
                    else if (QueryType == "year")
                    {
                        string CurrDate = StartDate.toDateTime("yyyy-MM-dd HH:mm:ss.fff").AddMonths(i).ToString("yyyy-MM");
                        xData += CurrDate + ",";

                        startDate = StartDate.toDateTime("yyyy-MM-dd HH:mm:ss.fff").AddMonths(i).ToString("yyyy-MM-dd HH:mm:ss.fff");
                        endDate = startDate.toDateMLastDay() + " 23:59:59.998";
                    }

                    Total += _dt3.Compute("sum(Total)", "DateCreated>='" + startDate + "' and DateCreated<='" + endDate + "'").toInt().ToString() + ",";
                }


                if (!string.IsNullOrEmpty(xData))
                    xData = xData.Substring(0, xData.Length - 1);


                DataRow[] _rows = _dt.Select("", "Sort");
                foreach (DataRow _dr in _rows)
                {
                    string FormID = _dr["FormID"].toStringTrim(),
                       FormName = _dr["FormName"].toStringTrim().Split('/')[0].toStringTrim(),
                       sData = string.Empty;  //[30,40,50] 值与xData对应（即：[7月,8月,9月]）

                    lData += FormName + ","; //['故障修理','申请依赖表']

                    for (int i = 0; i < MaxValue; i++)
                    {
                        string startDate = StartDate,
                        endDate = EndDate;

                        if (QueryType == "day")
                        {
                            startDate = StartDate.toDateTime("yyyy-MM-dd HH:mm:ss.fff").AddHours(i).ToString("yyyy-MM-dd HH:mm:ss.fff");
                            endDate = EndDate.toDateTime("yyyy-MM-dd HH:mm:ss.fff").AddHours(i).ToString("yyyy-MM-dd HH:mm:ss.fff");
                        }
                        else if (QueryType == "week")
                        {
                            startDate = StartDate.toDateTime("yyyy-MM-dd HH:mm:ss.fff").AddDays(i).ToString("yyyy-MM-dd HH:mm:ss.fff");
                            endDate = EndDate.toDateTime("yyyy-MM-dd HH:mm:ss.fff").AddDays(i).ToString("yyyy-MM-dd HH:mm:ss.fff");
                        }
                        else if (QueryType == "month")
                        {
                            startDate = StartDate.toDateTime("yyyy-MM-dd HH:mm:ss.fff").AddDays(i).ToString("yyyy-MM-dd HH:mm:ss.fff");
                            endDate = EndDate.toDateTime("yyyy-MM-dd HH:mm:ss.fff").AddDays(i).ToString("yyyy-MM-dd HH:mm:ss.fff");
                        }
                        else if (QueryType == "year")
                        {
                            startDate = StartDate.toDateTime("yyyy-MM-dd HH:mm:ss.fff").AddMonths(i).ToString("yyyy-MM-dd HH:mm:ss.fff");
                            endDate = startDate.toDateMLastDay() + " 23:59:59.998";
                        }

                        sData += _dt3.Compute("sum(Total)", "FormID=" + FormID + " and DateCreated>='" + startDate + "' and DateCreated<='" + endDate + "'").toInt().ToString() + ",";
                    }

                    if (!string.IsNullOrEmpty(sData))
                        sData = sData.Substring(0, sData.Length - 1);

                    series += "{" +
                        "name: '" + FormName + "'," +
                        "type: 'bar'," +
                        "stack: 'Forms'," +
                        "emphasis: {focus: 'series'}," +
                        "data: [" + sData + "]" +
                        "},";
                }

                //lData不为空时加上“总量图例”
                if (!string.IsNullOrEmpty(lData))
                {
                    lData = lData.Substring(0, lData.Length - 1); //['故障修理','申请依赖表','总量']

                    //加上总量图例
                    if (!string.IsNullOrEmpty(Total))
                        lData = lData + ",总量";
                }

                //加上总量的数据
                if (!string.IsNullOrEmpty(series))
                {
                    series = series.Substring(0, series.Length - 1);  // "[" + series.Substring(0, series.Length - 1) + "]";

                    //加上总量数据
                    if (!string.IsNullOrEmpty(Total))
                    {
                        Total = Total.Substring(0, Total.Length - 1);

                        series += ",{" +
                        "name: '总量'," +
                        "type: 'bar'," +
                        "barWidth: 10," +
                        "stack: 'MonthTotal'," +
                        "emphasis: {focus: 'series'}," +
                        "data: [" + Total + "]" +
                        "}";
                    }

                    series = "[" + series + "]";

                }

                flag = "{" + string.Format(strTpl, lData, xData, series) + "}";

            }

            return flag;

        }

        /// <summary>
        /// 获取饼状图格式的申请数据（首页2）
        /// </summary>
        /// <param name="QueryType"></param>
        /// <param name="CustomDate"></param>
        /// <returns></returns>
        public static string GetApplyByPieChart(string QueryType = "Year", string CustomDate = "")
        {
            string flag = string.Empty,
                       lData = string.Empty,
                       sData = string.Empty, //内环
                       sData2 = string.Empty; //外环

            QueryType = QueryType.ToLower();

            string strTpl = "\"lData\":\"{0}\", \"sData\":\"{1}\", \"sData2\":\"{2}\"";


            string GStartDate = DateTime.Now.toDateFormat("yyyy") + "-01-01 00:00:00.000",
                GEndDate = DateTime.Now.toDateFormat("yyyy") + "-12-31 23:59:59.998";


            CustomDate = string.IsNullOrEmpty(CustomDate) ? DateTime.Now.toDateFormat("yyyy-MM-dd") : CustomDate;

            switch (QueryType)
            {
                case "day":
                    GStartDate = CustomDate.toDateFormat("yyyy-MM-dd") + " 00:00:00.000";
                    GEndDate = CustomDate.toDateFormat("yyyy-MM-dd") + " 23:59:59.998";
                    break;

                case "week":
                    GStartDate = CustomDate.toDateWFirstDay() + " 00:00:00.000";
                    GEndDate = CustomDate.toDateWLastDay() + " 23:59:59.998";
                    break;

                case "month":
                    GStartDate = CustomDate.toDateMFirstDay() + " 00:00:00.000";
                    GEndDate = CustomDate.toDateMLastDay() + " 23:59:59.998";
                    break;

                case "year":
                    GStartDate = CustomDate.toDateFormat("yyyy") + "-01-01 00:00:00.000";
                    GEndDate = CustomDate.toDateFormat("yyyy") + "-12-31 23:59:59.998";
                    break;
            }


            //获取表单名称
            string _sql = "select FormID, Sort, FormName from Forms where Invalid=0 and Del=0 order by Sort";
            DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

            //获取表单分类
            string _sql2 = "select * from FormClassification where Invalid=0 and Del=0 and ParentID<>0 order by Sort";
            DataTable _dt2 = MsSQLDbHelper.Query(_sql2).Tables[0];

            //获取每张表单每个月的申请数量
            string _sql3 = "select a.FormID, b.FCID, b.FormName, count(distinct FormsID) as Total, a.DateCreated from FormApprovalRecords a left join Forms b on a.FormID=b.FormID where b.Invalid=0 and b.Del=0 and a.DateCreated between @GStartDate and @GEndDate group by a.FormID, b.FCID, b.FormName, a.DateCreated";

            SqlParameter[] _sp3 = { new SqlParameter("@GStartDate",SqlDbType.VarChar,30),
                                    new SqlParameter("@GEndDate",SqlDbType.VarChar,30),
                                };

            _sp3[0].Value = GStartDate;
            _sp3[1].Value = GEndDate;

            DataTable _dt3 = MsSQLDbHelper.Query(_sql3, _sp3).Tables[0];

            //按表单名称分类得到各表单数据（应用在外环）
            if (_dt != null && _dt.Rows.Count > 0)
            {
                DataRow[] _rows = _dt.Select("", "Sort");
                foreach (DataRow _dr in _rows)
                {
                    string FormID = _dr["FormID"].toStringTrim(),
                       FormName = _dr["FormName"].toStringTrim().Split('/')[0].toStringTrim();

                    lData += FormName + ","; //['故障修理','申请依赖表']

                    //外环数据
                    sData2 += "{value:" + _dt3.Compute("sum(Total)", "FormID=" + FormID).toInt().ToString() + ",name:\\\"" + FormName + "\\\"},";
                }

                if (!string.IsNullOrEmpty(sData2))
                    sData2 = "[" + sData2.Substring(0, sData2.Length - 1) + "]";

            }


            //按表单类别得到各表单数据（应用在内环）
            if (_dt2 != null && _dt2.Rows.Count > 0)
            {
                DataRow[] _rows2 = _dt2.Select("", "Sort");
                foreach (DataRow _dr2 in _rows2)
                {
                    string FCID = _dr2["FCID"].toStringTrim(),
                       ClassName = _dr2["ClassName"].toStringTrim().Split('/')[0].toStringTrim();

                    lData += ClassName + ","; //['IT类','HR类']

                    //内环数据
                    sData += "{value:" + _dt3.Compute("sum(Total)", "FCID=" + FCID).toInt().ToString() + ",name:\\\"" + ClassName + "\\\"},";
                }

                if (!string.IsNullOrEmpty(sData))
                    sData = "[" + sData.Substring(0, sData.Length - 1) + "]";
            }

            //lData“图例”
            if (!string.IsNullOrEmpty(lData))
                lData = lData.Substring(0, lData.Length - 1); //['故障修理','申请依赖表','总量']


            flag = "{" + string.Format(strTpl, lData, sData, sData2) + "}";


            return flag;
        }


        /// <summary>
        /// 获取浏览量
        /// </summary>
        /// <param name="QueryType"></param>
        /// <param name="CustomDate"></param>
        /// <returns></returns>
        public static string GetPageView(string QueryType = "Month", string CustomDate = "")
        {
            string flag = string.Empty,
                          xData = string.Empty,
                        pvData = string.Empty,
                        uvData = string.Empty,
                        ipData = string.Empty;

            QueryType = QueryType.ToLower();

            string strTpl = "\"xData\":\"{0}\", \"pvData\":\"{1}\", \"uvData\":\"{2}\", \"ipData\":\"{3}\"";


            string GStartDate = DateTime.Now.toDateFormat("yyyy") + "-01-01 00:00:00.000",
                GEndDate = DateTime.Now.toDateFormat("yyyy") + "-12-31 23:59:59.998",
                StartDate = DateTime.Now.toDateFormat("yyyy-MM-dd") + " 00:00:00.000",
                EndDate = DateTime.Now.toDateFormat("yyyy-MM-dd") + " 23:59:59.998";


            CustomDate = string.IsNullOrEmpty(CustomDate) ? DateTime.Now.toDateFormat("yyyy-MM-dd") : CustomDate;

            int MaxValue = 0; //天=24H，周=7天， 月=30天，年=12个月

            switch (QueryType)
            {
                case "day":
                    GStartDate = CustomDate.toDateFormat("yyyy-MM-dd") + " 00:00:00.000";
                    GEndDate = CustomDate.toDateFormat("yyyy-MM-dd") + " 23:59:59.998";

                    StartDate = GStartDate;
                    EndDate = StartDate.toDateFormat("yyyy-MM-dd") + " 00:59:59.998"; //以半个小时为单位

                    MaxValue = 24;
                    break;

                case "week":
                    GStartDate = CustomDate.toDateWFirstDay() + " 00:00:00.000";
                    GEndDate = CustomDate.toDateWLastDay() + " 23:59:59.998";

                    StartDate = GStartDate;
                    EndDate = StartDate.toDateFormat("yyyy-MM-dd") + " 23:59:59.998";

                    MaxValue = 7;
                    break;

                case "month":
                    GStartDate = CustomDate.toDateMFirstDay() + " 00:00:00.000";
                    GEndDate = CustomDate.toDateMLastDay() + " 23:59:59.998";

                    StartDate = GStartDate;
                    EndDate = StartDate.toDateFormat("yyyy-MM-dd") + " 23:59:59.998";

                    MaxValue = EndDate.toDateMLastDay("dd").toInt();
                    break;

                case "year":
                    GStartDate = CustomDate.toDateFormat("yyyy") + "-01-01 00:00:00.000";
                    GEndDate = CustomDate.toDateFormat("yyyy") + "-12-31 23:59:59.998";

                    StartDate = GStartDate;
                    EndDate = GStartDate.toDateMLastDay() + " 23:59:59.998";

                    MaxValue = 12;
                    break;
            }


            //获取指定时间范围内浏览量
            string _sql = "select DateCreated,UID,IP from SystemLog where DateCreated between @GStartDate and @GEndDate";

            SqlParameter[] _sp = { new SqlParameter("@GStartDate",SqlDbType.VarChar,30),
                                    new SqlParameter("@GEndDate",SqlDbType.VarChar,30),
                                };

            _sp[0].Value = GStartDate;
            _sp[1].Value = GEndDate;

            DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

            if (_dt != null && _dt.Rows.Count > 0 && MaxValue > 0)
            {
                for (int i = 0; i < MaxValue; i++)
                {
                    string startDate = StartDate,
                        endDate = EndDate;

                    if (QueryType == "day")
                    {
                        xData += StartDate.toDateTime("yyyy-MM-dd HH:mm:ss.fff").AddHours(i).ToString("HH:mm") + ",";

                        startDate = StartDate.toDateTime("yyyy-MM-dd HH:mm:ss.fff").AddHours(i).ToString("yyyy-MM-dd HH:mm:ss.fff");
                        endDate = EndDate.toDateTime("yyyy-MM-dd HH:mm:ss.fff").AddHours(i).ToString("yyyy-MM-dd HH:mm:ss.fff");
                    }
                    else if (QueryType == "week")
                    {
                        string CurrDate = StartDate.toDateTime("yyyy-MM-dd HH:mm:ss.fff").AddDays(i).ToString("yyyy-MM-dd");
                        xData += CurrDate.toDateWeek() + ",";

                        startDate = StartDate.toDateTime("yyyy-MM-dd HH:mm:ss.fff").AddDays(i).ToString("yyyy-MM-dd HH:mm:ss.fff");
                        endDate = EndDate.toDateTime("yyyy-MM-dd HH:mm:ss.fff").AddDays(i).ToString("yyyy-MM-dd HH:mm:ss.fff");
                    }
                    else if (QueryType == "month")
                    {
                        string CurrDate = StartDate.toDateTime("yyyy-MM-dd HH:mm:ss.fff").AddDays(i).ToString("MM-dd");
                        xData += CurrDate + ",";

                        startDate = StartDate.toDateTime("yyyy-MM-dd HH:mm:ss.fff").AddDays(i).ToString("yyyy-MM-dd HH:mm:ss.fff");
                        endDate = EndDate.toDateTime("yyyy-MM-dd HH:mm:ss.fff").AddDays(i).ToString("yyyy-MM-dd HH:mm:ss.fff");
                    }
                    else if (QueryType == "year")
                    {
                        string CurrDate = StartDate.toDateTime("yyyy-MM-dd HH:mm:ss.fff").AddMonths(i).ToString("yyyy-MM");
                        xData += CurrDate + ",";

                        startDate = StartDate.toDateTime("yyyy-MM-dd HH:mm:ss.fff").AddMonths(i).ToString("yyyy-MM-dd HH:mm:ss.fff");
                        endDate = startDate.toDateMLastDay() + " 23:59:59.998";
                    }

                    DataRow[] _rows = _dt.Select("DateCreated>='" + startDate + "' and DateCreated<='" + endDate + "'");
                    if (_rows.Length > 0)
                    {
                        pvData += _rows.Length + ",";

                        //DataRow转换为DataTable
                        DataView _dv = _rows.CopyToDataTable().DefaultView;
                        DataTable _dtUV = _dv.ToTable(true, "UID");  //=distinct UID
                        uvData += +_dtUV.Rows.Count + ",";

                        DataTable _dtIP = _dv.ToTable(true, "IP");  //distinct IP
                        ipData += +_dtIP.Rows.Count + ",";

                    }
                    else
                    {
                        pvData += "0,";
                        uvData += "0,";
                        ipData += "0,";
                    }
                }


                xData = xData.Substring(0, xData.Length - 1);
                pvData = pvData.Substring(0, pvData.Length - 1);
                uvData = uvData.Substring(0, uvData.Length - 1);
                ipData = ipData.Substring(0, ipData.Length - 1);

                flag = "{" + string.Format(strTpl, xData, pvData, uvData, ipData) + "}";

            }

            return flag;
        }


        /// <summary>
        /// 获取访客浏览器版本
        /// </summary>
        /// <returns></returns>
        public static string GetBrowser()
        {
            string flag = string.Empty,
                    xData = string.Empty,
                    bData = string.Empty;  //b=BrowserName

            string strTpl = "\"xData\":\"{0}\", \"bData\":\"{1}\"";

            string _sql = " select BrowserType,count(*) as Num from SystemLog group by BrowserType ";

            DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];
            if (_dt.Rows.Count > 0)
            {
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    string BrowserName = _dt.Rows[i]["BrowserType"].toStringTrim();
                    BrowserName = string.IsNullOrEmpty(BrowserName) ? "NULL" : BrowserName;

                    xData += BrowserName + ",";
                    bData += "{ value:" + _dt.Rows[i]["Num"].toStringTrim() + ",name:'" + BrowserName + "'},";
                }
                xData = xData.Substring(0, xData.Length - 1);
                bData = "[" + bData.Substring(0, bData.Length - 1) + "]";

                flag = "{" + string.Format(strTpl, xData, bData) + "}";
            }

            return flag;
        }


        /// <summary>
        /// 获取CPU、Memory使用率
        /// </summary>
        /// <returns></returns>
        public static string GetSysInfo()
        {
            string flag = string.Empty,
                    CPU = string.Empty,
                    CPUCores = string.Empty,
                    Memory = string.Empty,
                    dwMemoryLoad=string.Empty;

            ////System.Threading.Thread.Sleep(1000);  //毫秒

            //CPU = Math.Ceiling(((MicroInfo)HttpContext.Current.Session["MicroInfo"]).CPUCounter.NextValue()).ToString();  //CPU使用率
            //CPUCores = Environment.ProcessorCount.ToString();  // CPU核心数

            //Memory = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).RAMCounter.NextValue().ToString();  //可用内存 MB
            //Memory = decimal.Round(decimal.Parse(Memory) / 1024, 2).ToString();  //MB->GB

            ////调用MicroSysInfoHelper.cs类的内存方法
            //MicroSysInfo.MEMORY_INFO MemInfo;
            //MemInfo = new MicroSysInfo.MEMORY_INFO();
            //MicroSysInfo.GlobalMemoryStatus(ref MemInfo);
            //dwMemoryLoad = MemInfo.dwMemoryLoad;

            flag = "<div class=\"layuiadmin-card-list\">" +
                  "<p class=\"layuiadmin-normal-font\">服务器实时性能</p>" +
                  "<span>CPU使用率（CPU: " + CPUCores + " Cores）</span>" +
                  "<div class=\"layui-progress layui-progress-big\" lay-showPercent=\"yes\">" +
                  "<div class=\"layui-progress-bar\" lay-percent=\"" + CPU + "%\"></div>" +
                  "</div>" +
                  "</div>" +
                  "<div class=\"layuiadmin-card-list\">" +
                  "<span>内存使用率（Available: " + Memory + " GB）</span>" +
                  "<div class=\"layui-progress layui-progress-big\" lay-showPercent=\"yes\">" +
                  "<div class=\"layui-progress-bar\" lay-percent=\"" + dwMemoryLoad + "%\"></div>" +
                  "</div>" +
                  "</div>";

            return flag;
        }

    }
}