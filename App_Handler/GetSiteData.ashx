<%@ WebHandler Language="C#" Class="GetSiteData" %>

using System;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using Newtonsoft.Json.Linq;


public class GetSiteData : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        string flag = string.Empty,
        Type = context.Server.UrlDecode(context.Request.QueryString["Type"]);

        //测试数据
        //Type = "PageView";

        if (Type == "PageView" &&  MicroUserHelper.MicroUserInfo.CheckUserRole("Administrators"))
        {
            string strTpl = "\"PageViewsByDayData\":{0}, \"PageViewsData\":{1}, \"Browser\":{2}, \"FormTotalByMonth\":{3}";
            flag = string.Format(strTpl, GetPageViewByDay(), GetPageView(), GetBrowser(), GetFormTotalByMonth());
            flag = JToken.Parse("{" + flag + "}").ToString();
        }

        //flag = GetPageViewByDay();

        context.Response.Write(flag);
    }

    /// <summary>
    /// 当天24H流量
    /// </summary>
    /// <returns></returns>
    private string GetPageViewByDay()
    {
        string flag = string.Empty,
            xData = string.Empty,
            pvData = string.Empty,
            uvData = string.Empty,
            ipData = string.Empty;

        string CurrDay = DateTime.Now.ToString("yyyy-MM-dd"),
            StartTime = CurrDay + " 00:00:00.000",
            EndTime = CurrDay + " 00:29:59.998";  //半个小时

        string strTpl = "\"xData\":\"{0}\", \"pvData\":\"{1}\", \"uvData\":\"{2}\", \"ipData\":\"{3}\"";

        string _sql = " select DateCreated,UID,IP from SystemLog where DateCreated between '" + CurrDay + " 00:00:00.000' and '" + CurrDay + " 23:59:59.998'";

        DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];
        if (_dt.Rows.Count > 0)
        {
            for (double i = 0; i < 24; i = i + 0.5)
            {
                string startTime = StartTime.toDateTime("yyyy-MM-dd HH:mm:ss.fff").AddHours(i).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                        endTime = EndTime.toDateTime("yyyy-MM-dd HH:mm:ss.fff").AddHours(i).ToString("yyyy-MM-dd HH:mm:ss.fff");

                xData += startTime.toDateFormat("HH:mm") + ",";

                DataRow[] _rows = _dt.Select("DateCreated>='" + startTime + "' and DateCreated<='" + endTime + "'");
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

            flag = string.Format(strTpl, xData, pvData, uvData, ipData);
            flag = JToken.Parse("{" + flag + "}").ToString();
            //flag = "[{" + flag + "}]";
        }

        return flag;

    }

    /// <summary>
    /// 最近30天流量
    /// </summary>
    /// <returns></returns>
    private string GetPageView()
    {
        string flag = string.Empty,
                xData = string.Empty,
                pvData = string.Empty,
                uvData = string.Empty,
                ipData = string.Empty;

        string strTpl = "\"xData\":\"{0}\", \"pvData\":\"{1}\", \"uvData\":\"{2}\", \"ipData\":\"{3}\"";

        string _sql = " select CONVERT(varchar(100), DateCreated, 23) as PVDate, count (*) as PageViews from SystemLog where " +
            //" DateCreated between '2021-04-01 00:00:00.000' and '2021-06-30 23:59:59:998'"+ //指定区间
            " DATEDIFF(dd,DateCreated,GETDATE())<=30" +  //最近30天
            " group by CONVERT(varchar(100), DateCreated, 23) order by CONVERT(varchar(100), DateCreated, 23) ";

        DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];
        if (_dt.Rows.Count > 0)
        {
            for (int i = 0; i < _dt.Rows.Count; i++)
            {
                xData += _dt.Rows[i]["PVDate"].toStringTrim() + ",";
                pvData += _dt.Rows[i]["PageViews"].toStringTrim() + ",";
                uvData += GetUsersView(_dt.Rows[i]["PVDate"].toStringTrim()) + ",";
                ipData += GetIPView(_dt.Rows[i]["PVDate"].toStringTrim()) + ",";
            }

            xData = xData.Substring(0, xData.Length - 1);
            pvData = pvData.Substring(0, pvData.Length - 1);
            uvData = uvData.Substring(0, uvData.Length - 1);
            ipData = ipData.Substring(0, ipData.Length - 1);

            flag = string.Format(strTpl, xData, pvData, uvData, ipData);
            flag = JToken.Parse("{" + flag + "}").ToString();
            //flag = "[{" + flag + "}]";
        }

        return flag;

    }

    /// <summary>
    /// UsersView
    /// </summary>
    /// <param name="PVDate"></param>
    /// <returns></returns>
    private string GetUsersView(string PVDate)
    {
        string flag = "0";
        string _sql = " select COUNT(distinct UID) as UserViews from SystemLog where CONVERT(varchar(100), DateCreated, 23)=@PVDate";
        SqlParameter[] _sp = { new SqlParameter("@PVDate",SqlDbType.VarChar)};
        _sp[0].Value = PVDate;

        DataTable _dt = MsSQLDbHelper.Query(_sql,_sp).Tables[0];

        if (_dt.Rows.Count > 0)
            flag = _dt.Rows[0][0].toStringTrim();

        return flag;

    }

    /// <summary>
    /// IPView
    /// </summary>
    /// <param name="PVDate"></param>
    /// <returns></returns>
    private string GetIPView(string PVDate)
    {
        string flag = "0";
        string _sql = "  select COUNT(distinct IP) as IPViews from SystemLog where CONVERT(varchar(100), DateCreated, 23)=@PVDate";
        SqlParameter[] _sp = { new SqlParameter("@PVDate",SqlDbType.VarChar)};
        _sp[0].Value = PVDate;

        DataTable _dt = MsSQLDbHelper.Query(_sql,_sp).Tables[0];

        if (_dt.Rows.Count > 0)
            flag = _dt.Rows[0][0].toStringTrim();

        return flag;

    }

    /// <summary>
    /// Browser
    /// </summary>
    /// <returns></returns>
    private string GetBrowser()
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
            xData = xData.Substring(0, xData.Length - 1) ;
            bData = "[" + bData.Substring(0, bData.Length - 1) + "]";

            flag = string.Format(strTpl, xData, bData);
            flag = JToken.Parse("{" + flag + "}").ToString();
            //flag = "{" + flag + "}";

        }

        return flag;
    }

    /// <summary>
    /// 按月获取每张表单的申请数量（取最后12个月的数据）
    /// </summary>
    /// <returns></returns>
    private string GetFormTotalByMonth()
    {
        string flag = string.Empty,
                lData = string.Empty,
                xData = string.Empty,
                series = string.Empty,
                TotalByMonth = string.Empty;

        string strTpl = "\"lData\":\"{0}\", \"xData\":\"{1}\", \"series\":\"{2}\"";

        //获取表单名称
        string _sql = "select FormID, Sort, FormName from Forms where Invalid=0 and Del=0 order by Sort";
        DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];


        //获取最后12个月的月份
        string _sql2 = "select top 12 CONVERT(varchar(7), a.DateCreated,23) as Months from FormApprovalRecords a left join Forms b on a.FormID=b.FormID where b.Invalid=0 and b.Del=0 group by CONVERT(varchar(7), a.DateCreated,23) order by Months desc";
        DataTable _dt2 = MsSQLDbHelper.Query(_sql2).Tables[0];


        //获取每张表单每个月的申请数量
        string _sql3 = "select a.FormID, b.FormName ,CONVERT(varchar(7), a.DateCreated,23) as Months ,count(distinct FormsID) as Total from FormApprovalRecords a left join Forms b on a.FormID=b.FormID where b.Invalid=0 and b.Del=0 group by a.FormID,b.FormName,CONVERT(varchar(7), a.DateCreated,23) order by Months";
        DataTable _dt3 = MsSQLDbHelper.Query(_sql3).Tables[0];

        if (_dt != null && _dt.Rows.Count > 0 && _dt2 != null && _dt2.Rows.Count > 0)
        {
            DataRow[] _rows2 = _dt2.Select("", "Months");
            foreach (DataRow _dr in _rows2)
            {
                string Months = _dr["Months"].toStringTrim();
                xData += Months + ",";  //[7月,8月,9月]

                TotalByMonth += _dt3.Compute("sum(Total)", "Months='" + Months + "'").ToString() + ",";

            }

            if (!string.IsNullOrEmpty(xData))
                xData = xData.Substring(0, xData.Length - 1);


            for (int i = 0; i < _dt.Rows.Count; i++)
            {
                string FormID = _dt.Rows[i]["FormID"].toStringTrim(),
                FormName = _dt.Rows[i]["FormName"].toStringTrim().Split('/')[0].toStringTrim(),
                sData = string.Empty;  //[30,40,50] 值与xData对应（即：[7月,8月,9月]）

                lData += FormName + ","; //['故障修理','申请依赖表']

                DataRow[] __rows2 = _dt2.Select("", "Months");
                foreach (DataRow __dr in __rows2)
                {
                    string Months = __dr["Months"].toStringTrim();

                    DataRow[] _rows3 = _dt3.Select("FormID=" + FormID + " and Months='" + Months + "'");
                    if (_rows3.Length > 0)
                        sData += _rows3[0]["Total"].ToString() + ",";
                    else
                        sData += "0,";
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


            if (!string.IsNullOrEmpty(lData))
            {
                lData = lData.Substring(0, lData.Length - 1); //['故障修理','申请依赖表']

                //加上总量图例
                if (!string.IsNullOrEmpty(TotalByMonth))
                    lData = lData + ",总量";
            }

            if (!string.IsNullOrEmpty(series))
            {
                series = series.Substring(0, series.Length - 1);  // "[" + series.Substring(0, series.Length - 1) + "]";

                //加上总量数据
                if (!string.IsNullOrEmpty(TotalByMonth))
                {
                    TotalByMonth = TotalByMonth.Substring(0, TotalByMonth.Length - 1);

                    series += ",{" +
                    "name: '总量'," +
                    "type: 'bar'," +
                    "barWidth: 10,"+
                    "stack: 'MonthTotal'," +
                    "emphasis: {focus: 'series'}," +
                    "data: [" + TotalByMonth + "]" +
                    "}";
                }

                series = "[" + series + "]";

            }


            flag = string.Format(strTpl, lData, xData, series);
            flag = JToken.Parse("{" + flag + "}").ToString();

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