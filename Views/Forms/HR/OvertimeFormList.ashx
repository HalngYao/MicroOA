<%@ WebHandler Language="C#" Class="OvertimeFormList" %>

using System;
using System.Web;
using System.Text;
using System.Data;
using MicroDBHelper;
using MicroPublicHelper;
using MicroDTHelper;
using MicroAuthHelper;

public class OvertimeFormList : IHttpHandler, System.Web.SessionState.IRequiresSessionState
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

        string flag = "{\"code\": 0, \"msg\": \"\", \"count\": 0, \"data\":  [] }",
                    Action = context.Server.UrlDecode(context.Request.QueryString["action"]),
                    ModuleID = context.Server.UrlDecode(context.Request.QueryString["mid"]),
                    ShortTableName = context.Server.UrlDecode(context.Request.QueryString["stn"]),
                    FormID = context.Server.UrlDecode(context.Request.QueryString["formid"]),
                    Page = context.Server.UrlDecode(context.Request.QueryString["page"]),
                    Limit = context.Server.UrlDecode(context.Request.QueryString["limit"]),
                    QueryType = context.Server.UrlDecode(context.Request.QueryString["QueryType"]),
                    Keyword = context.Server.UrlDecode(context.Request.QueryString["Keyword"]),
                    MainOrSub = context.Server.UrlDecode(context.Request.QueryString["Mainsub"]);

        ////测试数据
        //ShortTableName = "overtime";
        ////ModuleID =
        //FormID = "6";
        //QueryType = "OvertimeID:int";
        //Keyword = "1";
        //MainOrSub = "Sub";

        Page = string.IsNullOrEmpty(Page) == true ? "1" : Page;
        Limit = string.IsNullOrEmpty(Limit) == true ? "10" : Limit;

        if (!string.IsNullOrEmpty(ShortTableName) && !string.IsNullOrEmpty(FormID) && !string.IsNullOrEmpty(Keyword))
        {
            string TableName = MicroPublic.GetTableName(ShortTableName);
            string _sql = "select * from " + TableName + " where Invalid=0 and Del=0 " +
                "and OvertimeTypeID = (select OvertimeTypeID from HROvertimeType where Invalid=0 and Del=0 and FormID=" + FormID.toInt() + ") " +
                "and (OvertimeID=" + Keyword + " or ParentID=" + Keyword + ") order by OvertimeDate,StartTime ";
            DataTable CustomDataTable = MsSQLDbHelper.Query(_sql).Tables[0];

            if (CustomDataTable != null && CustomDataTable.Rows.Count > 0)
                flag = MicroDataTable.GetLayuiDataTableList(ShortTableName, Page, Limit, false, QueryType, Keyword, MainOrSub, CustomDataTable, "OvertimeDate, StartTime");
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