<%@ WebHandler Language="C#" Class="HomePage" %>

using System;
using System.Web;
using MicroBIHelper;
using Newtonsoft.Json.Linq;


public class HomePage : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        string flag = string.Empty,
        DataType = context.Server.UrlDecode(context.Request.Form["DataType"]),
        QueryType = context.Server.UrlDecode(context.Request.Form["QueryType"]),
        CustomDate = context.Server.UrlDecode(context.Request.Form["CustomDate"]);

        CustomDate = string.IsNullOrEmpty(CustomDate) ? DateTime.Now.toDateFormat("yyyy-MM-dd") : CustomDate;

        ////测试数据
        //DataType = "SysInfo";
        //QueryType = "All";
        //CustomDate = "2021-09-15";

        if (!string.IsNullOrEmpty(DataType) && !string.IsNullOrEmpty(QueryType) && !string.IsNullOrEmpty(CustomDate))
        {
            DataType = DataType.ToLower();

            if (DataType == "qty")
                flag = MicroBI.GetApplyByQty(QueryType, CustomDate);

            else if (DataType == "chart")
                flag = MicroBI.GetApplyByChart(QueryType, CustomDate);

            else if (DataType == "piechart")
                flag = MicroBI.GetApplyByPieChart(QueryType, CustomDate);

            else if (DataType == "pageview")
                flag = MicroBI.GetPageView(QueryType, CustomDate);

            else if (DataType == "browser")
                flag = MicroBI.GetBrowser();

            else if (DataType == "sysinfo")
                flag = MicroBI.GetSysInfo();
        }

        context.Response.Write(flag);
    }


    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}