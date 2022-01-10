<%@ WebHandler Language="C#" Class="GetPublicTableList" %>

using System;
using System.Web;
using MicroPublicHelper;
using MicroDTHelper;
using MicroAuthHelper;

public class GetPublicTableList : IHttpHandler, System.Web.SessionState.IRequiresSessionState
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
                    FormID = context.Server.UrlDecode(context.Request.QueryString["FormID"]),
                    DataType = context.Server.UrlDecode(context.Request.QueryString["DataType"]),
                    QueryType = context.Server.UrlDecode(context.Request.QueryString["QueryType"]),
                    Keyword = context.Server.UrlDecode(context.Request.QueryString["Keyword"]),
                    StartDate = context.Server.UrlDecode(context.Request.QueryString["StartDate"]),
                    EndDate = context.Server.UrlDecode(context.Request.QueryString["EndDate"]),
                    Page = context.Server.UrlDecode(context.Request.QueryString["page"]),
                    Limit = context.Server.UrlDecode(context.Request.QueryString["limit"]);

        //测试数据
        //Action = "View";
        //ShortTableName = "OnDutyForm";
        //ModuleID = "4";
        //FormID = "13";
        //DataType = "GetOnDutyFormListByMeal";
        //StartDate = "2021-10-14";
        //EndDate = "2021-10-14";
        //QueryType = "DutyDate";
        //Keyword = "Location:AAAA,Keyword:GZ-3S";

        Page = string.IsNullOrEmpty(Page) == true ? "1" : Page;
        Limit = string.IsNullOrEmpty(Limit) == true ? "10" : Limit;

        if (!string.IsNullOrEmpty(Action) && !string.IsNullOrEmpty(ShortTableName))
        {
            int UID = MicroUserHelper.MicroUserInfo.GetUserInfo("UID").toInt();

            if (!string.IsNullOrEmpty(DataType))
                DataType = DataType.ToLower();

            switch (DataType)
            {
                //加班记录
                case "getovertimelist":
                    flag = MicroDataTable.GetLayuiDataTableList(ShortTableName, Page, Limit, true, QueryType, Keyword, "Sub", MicroDataTable.GetOvertimeList(ShortTableName, ModuleID, FormID, QueryType, StartDate, EndDate, Keyword));
                    break;

                //个人加班草稿箱(还是需要MainSub而不能仅用Sub。场景：当用户填写申请但删除了明细仅留下父记录，且同时关闭了窗口下次进来时无法显示父记录则提示存在草稿)
                case "getpersonaldraftlist":
                    flag = MicroDataTable.GetLayuiDataTableList(ShortTableName, Page, Limit, true, QueryType, Keyword, "MainSub", MicroDataTable.GetPersonalDraft(ShortTableName, ModuleID, FormID));
                    break;

                //个人加班汇总（根据时间范围，用度或季度等）
                case "getpersonalovertime":
                    flag = MicroDataTable.GetLayuiDataTableList(ShortTableName, Page, Limit, true, QueryType, Keyword, "Sub", MicroDataTable.GetPersonalAttendance(ShortTableName, ModuleID, FormID, DataType, UID, StartDate, EndDate, Keyword));
                    break;

                //个人月度延时加班
                case "getworkovertime":
                    flag = MicroDataTable.GetLayuiDataTableList(ShortTableName, Page, Limit, true, QueryType, Keyword, "Sub", MicroDataTable.GetPersonalAttendance(ShortTableName, ModuleID, FormID, DataType, UID, StartDate, EndDate, Keyword));
                    break;

                //个人月度休息日加班
                case "getoffdayovertime":
                    flag = MicroDataTable.GetLayuiDataTableList(ShortTableName, Page, Limit, true, QueryType, Keyword, "Sub", MicroDataTable.GetPersonalAttendance(ShortTableName, ModuleID, FormID, DataType, UID, StartDate, EndDate, Keyword));
                    break;

                //个人月度法定加班
                case "getstatutory":
                    flag = MicroDataTable.GetLayuiDataTableList(ShortTableName, Page, Limit, true, QueryType, Keyword, "Sub", MicroDataTable.GetPersonalAttendance(ShortTableName, ModuleID, FormID, DataType, UID, StartDate, EndDate, Keyword));
                    break;

                //个人需代休
                case "getneeddaixiu":
                    //flag = MicroDataTable.GetLayuiDataTableList(ShortTableName, Page, Limit, true, QueryType, Keyword, "Sub", MicroDataTable.GetPersonalOvertime(ShortTableName, DataType, UID, StartDate, EndDate));
                    flag = "";
                    break;

                //个人已代休
                case "getalreadydaixiu":
                    flag = MicroDataTable.GetLayuiDataTableList(ShortTableName, Page, Limit, true, QueryType, Keyword, "", MicroDataTable.GetPersonalAttendance(ShortTableName, ModuleID, FormID, DataType, UID, StartDate, EndDate, Keyword));
                    break;

                //个人休假(首页个人休假明细)
                case "getleave":
                    flag = MicroDataTable.GetLayuiDataTableList(ShortTableName, Page, Limit, true, QueryType, Keyword, "Sub", MicroDataTable.GetPersonalAttendance(ShortTableName, ModuleID, FormID, DataType, UID, StartDate, EndDate, Keyword));
                    break;

                //个人休假(申请记录明细)
                case "getleavelist":
                    flag = MicroDataTable.GetLayuiDataTableList(ShortTableName, Page, Limit, true, QueryType, Keyword, "Sub", MicroDataTable.GetLeaveList(ShortTableName, ModuleID, FormID, QueryType, StartDate, EndDate, Keyword));
                    break;

                //表单记录获取通用方法(表单申请记录明细)
                case "getgeneralformlist":
                    flag = MicroDataTable.GetLayuiDataTableList(ShortTableName, Page, Limit, true, QueryType, Keyword, "", MicroDataTable.GetGeneralFormList(ShortTableName, ModuleID, FormID, QueryType, StartDate, EndDate, Keyword));
                    break;

                //获取待我审批的记录（把同一个表的单据展示在一起来满足批量审批，应用在首页待我审批入口）
                case "getpendingmyapprovallist":

                    string MainSub = "Sub";
                    if (ShortTableName.ToLower() == "ondutyform")
                        MainSub = "Main";  //如果是值班（排班）表则取父记录，如果后续情况多的话可在MicroTables表增加字段控制

                    flag = MicroDataTable.GetLayuiDataTableList(ShortTableName, Page, Limit, true, QueryType, Keyword, MainSub, MicroDataTable.GetPendingMyApprovalList(ShortTableName, ModuleID, FormID, QueryType, StartDate, EndDate, Keyword));

                    break;

                //排班申请记录
                case "getondutyformlist":
                    flag = MicroDataTable.GetLayuiDataTableList(ShortTableName, Page, Limit, true, QueryType, Keyword, "Main", MicroDataTable.GetOnDutyFormList(ShortTableName, ModuleID, FormID, QueryType, StartDate, EndDate, Keyword));
                    break;

                //排班申请记录(就餐统计查询用)
                case "getondutyformlistbymeal":
                    flag = MicroDataTable.GetLayuiDataTableList(ShortTableName, Page, Limit, true, QueryType, Keyword, "Sub", MicroDataTable.GetOnDutyFormListByMeal(ShortTableName, ModuleID, FormID, QueryType, StartDate, EndDate, Keyword));
                    break;

                default:
                    flag = MicroDataTable.GetLayuiDataTableList(ShortTableName, Page, Limit, true, QueryType, Keyword);
                    break;

            }
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