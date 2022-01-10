<%@ WebHandler Language="C#" Class="MicroPublicFormList" %>

using System;
using System.Web;
using System.Text;
using System.Data;
using System.Linq;
using MicroDBHelper;
using MicroAuthHelper;

public class MicroPublicFormList : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = "{\"code\": 0, \"msg\": \"No Data.没有数据。\", \"count\": 0, \"data\": [] }",
        Action = context.Server.UrlDecode(context.Request.QueryString["action"]),
        Type = context.Server.UrlDecode(context.Request.QueryString["type"]),
        ModuleID = context.Server.UrlDecode(context.Request.QueryString["mid"]),
        Page = context.Server.UrlDecode(context.Request.QueryString["page"]),
        Limit = context.Server.UrlDecode(context.Request.QueryString["limit"]);

        Page = string.IsNullOrEmpty(Page) == true ? "1" : Page;
        Limit = string.IsNullOrEmpty(Limit) == true ? "10" : Limit;
        //Type = "MyApply";
        //ModuleID = "2";

        if (!string.IsNullOrEmpty(Type) && !string.IsNullOrEmpty(ModuleID))
            flag = GetPuhlicFormList(Type, Page, Limit);

        context.Response.Write(flag);
    }

    private string GetPuhlicFormList(string Type, string Page, string Limit)
    {
        string flag = "{\"code\": 0, \"msg\": \"No Data.没有数据。\", \"count\": 0, \"data\": [] }",
                UID = MicroUserHelper.MicroUserInfo.GetUserInfo("UID");
        string strTemp = GetStrTpl();
        string _sql = string.Empty;
        DataTable SourceDT = null, TargetDT = null;
        int IntPage = Page.toInt(), IntLimit = Limit.toInt();
        Type = Type.ToLower();

        switch (Type)
        {
            case "myapply":  //我的申请 Update 20201211
                _sql = "" +
                //一阶 关联相关表
                "select a.*,b.FormName,b.ShortTableName,c.EMail,c.WorkTel,c.WorkMobilePhone,c.AdDepartment from FormApprovalRecords a left join Forms b on a.FormID=b.FormID left join UserInfo c on a.UID=c.UID " +
                //二阶 得到没有审批完成或没有对应完成或没有结案时的 min(FARID)
                "where ((FARID in (select min(FARID) from FormApprovalRecords where Invalid = 0 and Del = 0 and StateCode <= 0 and UID = " + UID + " group by FormID,FormsID)) " +
                //三阶 或者得到已经结案了的max(FARID),同时排除二阶出现过的记录
                "or (FARID in (select max(FARID) from FormApprovalRecords where Invalid = 0 and Del = 0 and StateCode > 0 and UID = " + UID + " " +
                //三阶接续 排除二阶出现过的记录
                "and FormNumber not in (select FormNumber from FormApprovalRecords where FARID in (select min(FARID) from FormApprovalRecords where Invalid = 0 and Del = 0 and StateCode <= 0 and UID = " + UID + " group by FormID,FormsID) ) group by FormID,FormsID) )) " +
                "and a.Invalid=0 and a.Del=0 and b.Invalid = 0 and b.Del = 0 and a.UID = " + UID + " order by FARID desc";
                SourceDT = MsSQLDbHelper.Query(_sql).Tables[0];
                break;

            case "myapplywaitapproval":  //我的申请等待审批  Update 20201211
                _sql = "" +
                 //一阶 关联相关表
                 " select a.*,b.FormName,b.ShortTableName,c.EMail,c.WorkTel,c.WorkMobilePhone,c.AdDepartment from FormApprovalRecords a left join Forms b on a.FormID=b.FormID left join UserInfo c on a.UID=c.UID " +
                //二阶 得到在审批阶段已经审批或未审批的max(FARID)作为单条记录
                "where FARID in (select min(FARID) from FormApprovalRecords where WorkFlowID in (select WFID from WorkFlow where FlowCode = 'Approval' and Invalid=0 and Del=0 ) and Invalid = 0 and Del = 0 and StateCode = 0 and UID = " + UID + " group by FormID,FormsID) " +
                //三阶 去除在任意审批阶段被驳回的记录 （注：申请、受理、结案不算审批阶段）
                "and FormNumber not in(select FormNumber from FormApprovalRecords where FARID in (select max(FARID) from FormApprovalRecords where Invalid = 0 and Del = 0 and StateCode < 0 and UID = " + UID + " group by FormID,FormsID) and Invalid=0 and Del=0 and UID = " + UID + ") " +
                //四阶 去除在审批阶段最后一位已经审批通过的记录
                "and FARID not in (select FARID from FormApprovalRecords where WorkFlowID in(select max(WFID) from WorkFlow where FlowCode = 'Approval' and Invalid=0 and Del=0 group by FormID) and Invalid=0 and Del=0 and StateCode=1 and UID = " + UID + ") " +
                "and a.Invalid=0 and a.Del=0 and b.Invalid = 0 and b.Del = 0 and a.UID = " + UID + " order by FARID desc";
                SourceDT = MsSQLDbHelper.Query(_sql).Tables[0];
                break;

            case "myapplywaitaccept":  //我的申请等待处理 Update 20201214
                _sql = "" +
                //一阶 关联相关表
                "select a.*,b.FormName,b.ShortTableName,c.EMail,c.WorkTel,c.WorkMobilePhone,c.AdDepartment from FormApprovalRecords a left join Forms b on a.FormID=b.FormID left join UserInfo c on a.UID=c.UID " +
                //二阶 得到在审批阶段最大的记录已经经过审批的（即max(FARID) 且StateCode>0）
                "where a.Invalid=0 and a.Del=0 and b.Invalid = 0 and b.Del = 0 and (FormNumber in( " +
                "select FormNumber from FormApprovalRecords where FARID in (select max(FARID) from FormApprovalRecords where WorkFlowID in (select WFID from WorkFlow where Invalid=0 and Del=0 and FlowCode = 'Approval') and Invalid=0 and Del=0 group by FormID,FormsID) " +
                "and StateCode>0 and StateCode<>15) " +
                //三阶 并且在受理阶段未受理的
                "and WorkFlowID in (select WFID from WorkFlow where Invalid=0 and Del=0 and FlowCode = 'Accept') and StateCode = 0 " +
                //四阶 去除在任意审批阶段被驳回的记录 （注：申请、受理、结案不算审批阶段）
                "and FormNumber not in(select FormNumber from FormApprovalRecords where FARID in (select max(FARID) from FormApprovalRecords where StateCode < 0 and Invalid = 0 and Del = 0 group by FormID,FormsID)) " +
                //五阶 Invalid=0 and Del=0并且需是我申请的
                "and a.Invalid = 0 and a.Del = 0 and a.UID=" + UID + " ) " +
                //六阶 或者(或运算)得到不需要审批的表单并且在受理阶段的记录、并且是我申请的 （注意：FormID=0）
                "or (FARID in (select FARID from FormApprovalRecords where Invalid=0 and Del=0 and  WorkFlowID in (select WFID from WorkFlow where Invalid=0 and Del=0 and FormID=0 and FlowCode='Accept') and StateCode = 0 " +
                "and UID=" + UID + " )) order by FARID desc";
                SourceDT = MsSQLDbHelper.Query(_sql).Tables[0];
                break;

            case "myapplyaccepting":  //我的申请处理中 Update 20201231
                _sql = "" +
                //一阶 关联相关表
                "select a.*,b.FormName,b.ShortTableName,c.EMail,c.WorkTel,c.WorkMobilePhone,c.AdDepartment from FormApprovalRecords a left join Forms b on a.FormID=b.FormID left join UserInfo c on a.UID=c.UID " +
                //二阶 得到在受理阶段已受理的记录
                "where FARID in (select FARID from FormApprovalRecords where WorkFlowID in (select WFID from WorkFlow where Invalid=0 and Del=0 and FlowCode = 'Accept') and Invalid=0 and Del=0 and StateCode > 0 and UID = " + UID + " ) " +
                //三阶 去除在任意审批阶段被驳回的记录 
                "and FormNumber not in(select FormNumber from FormApprovalRecords where FARID in (select max(FARID) from FormApprovalRecords where StateCode < 0 and Invalid = 0 and Del = 0 group by FormID,FormsID))  " +
                //四阶 排除已完成的记录
                "and FormNumber not in (select FormNumber from FormApprovalRecords where WorkFlowID in (select WFID from WorkFlow where Invalid=0 and Del=0 and FlowCode = 'Finish') and Invalid=0 and Del=0 and StateCode > 0 and UID = " + UID + " ) " +
                "and a.Invalid = 0 and a.Del = 0 and b.Invalid = 0 and b.Del = 0 and a.UID = " + UID + " order by FARID desc";
                SourceDT = MsSQLDbHelper.Query(_sql).Tables[0];
                break;

            case "myapplyfinish":  //我的申请完成 Update 20201214
                _sql = "select a.*,b.FormName,b.ShortTableName,c.EMail,c.WorkTel,c.WorkMobilePhone,c.AdDepartment from FormApprovalRecords a left join Forms b on a.FormID=b.FormID left join UserInfo c on a.UID=c.UID where WorkFlowID in (select WFID from WorkFlow where Invalid=0 and Del=0 and FlowCode='Finish') and StateCode>0 and a.UID = " + UID + " and a.Invalid=0 and a.Del=0  and b.Invalid = 0 and b.Del = 0 order by FARID desc";
                SourceDT = MsSQLDbHelper.Query(_sql).Tables[0];
                break;

            case "myapplyreject":  //我的申请驳回 Update 20211119
                _sql = "select a.*,b.FormName,b.ShortTableName,c.EMail,c.WorkTel,c.WorkMobilePhone,c.AdDepartment from FormApprovalRecords a left join Forms b on a.FormID=b.FormID left join UserInfo c on a.UID=c.UID where FARID in (select max(FARID) from FormApprovalRecords where StateCode = -1 and Invalid = 0 and Del = 0 group by FormID,FormsID) and a.UID = " + UID + " and a.Invalid=0 and a.Del=0 and b.Invalid = 0 and b.Del = 0 order by FARID desc";
                SourceDT = MsSQLDbHelper.Query(_sql).Tables[0];
                break;

            case "myapplywithdrawal":  //我的申请撤回 Update 20211119
                _sql = "select a.*,b.FormName,b.ShortTableName,c.EMail,c.WorkTel,c.WorkMobilePhone,c.AdDepartment from FormApprovalRecords a left join Forms b on a.FormID=b.FormID left join UserInfo c on a.UID=c.UID where FARID in (select max(FARID) from FormApprovalRecords where (StateCode = -4 or StateCode = 15 ) and Invalid = 0 and Del = 0 group by FormID,FormsID) and a.UID = " + UID + " and a.Invalid=0 and a.Del=0 and b.Invalid = 0 and b.Del = 0 order by FARID desc";
                SourceDT = MsSQLDbHelper.Query(_sql).Tables[0];
                break;



            //********************任务*********************
            case "pendingmyapproval":  //任务-等待我审批 Update 20201214
                _sql = "" +
                //一阶 关联相关表
                "select a.*,b.FormName,b.ShortTableName,c.EMail,c.WorkTel,c.WorkMobilePhone,c.AdDepartment from FormApprovalRecords a left join Forms b on a.FormID=b.FormID left join UserInfo c on a.UID=c.UID " +
                //二阶 得到在审批阶段最小的需要审批的记录
                "where FARID in (select min(FARID) from FormApprovalRecords where WorkFlowID in (select WFID from WorkFlow where Invalid=0 and Del=0 and FlowCode='Approval') and Invalid=0 and Del=0 and StateCode=0 group by FormID,FormsID) " +
                //三阶 去除在任意审批阶段被驳回的记录 （注：申请、受理、结案不算审批阶段）
                "and FormNumber not in(select FormNumber from FormApprovalRecords where FARID in (select max(FARID) from FormApprovalRecords where StateCode < 0 and Invalid = 0 and Del = 0 group by FormID,FormsID)) " +
                //四阶 与我相关的
                "and CHARINDEX(',' + convert(varchar, " + UID + ") + ',',',' + CanApprovalUID + ',')> 0 and a.Invalid = 0 and a.Del = 0 and b.Invalid = 0 and b.Del = 0 order by FARID desc";
                SourceDT = MsSQLDbHelper.Query(_sql).Tables[0];
                break;

            case "pendingmyaccept":  //任务-待我处理 Update 20201214
                _sql = "" +
                //一阶 关联相关表
                "select a.*,b.FormName,b.ShortTableName,c.EMail,c.WorkTel,c.WorkMobilePhone,c.AdDepartment from FormApprovalRecords a left join Forms b on a.FormID = b.FormID left join UserInfo c on a.UID = c.UID " +
                //二阶 得到在审批阶段最大的记录已经经过审批的（即max(FARID) 且StateCode > 0）
                "where a.Invalid=0 and a.Del=0 and b.Invalid = 0 and b.Del = 0 and (FormNumber in( " +
                "select FormNumber from FormApprovalRecords where FARID in (select max(FARID) from FormApprovalRecords where WorkFlowID in (select WFID from WorkFlow where Invalid = 0 and Del = 0 and FlowCode = 'Approval') and Invalid = 0 and Del = 0 group by FormID,FormsID) " +
                "and StateCode> 0) " +
                //三阶 并且在受理阶段未受理的
                "and WorkFlowID in (select WFID from WorkFlow where Invalid = 0 and Del = 0 and FlowCode = 'Accept') and StateCode = 0 " +
                //四阶 去除在任意审批阶段被驳回的记录 （注：申请、受理、结案不算审批阶段）
                "and FormNumber not in(select FormNumber from FormApprovalRecords where FARID in (select max(FARID) from FormApprovalRecords where StateCode < 0 and Invalid = 0 and Del = 0 group by FormID,FormsID)) " +
                //五阶 Invalid = 0 and Del = 1并且需要我进行处理的 （即 CHARINDEX(',' + convert(varchar, 362) + ',',',' + CanApprovalUID + ',')> 0 ）
                "and a.Invalid = 0 and a.Del = 0 and CHARINDEX(',' + convert(varchar, " + UID + ") + ',',',' + CanApprovalUID + ',')> 0 )" +
                //六阶 或者(或运算)得到不需要审批的表单在受理阶段的记录并且需要我进行处理的 （注意：FormID = 0）
                "or (FARID in (select FARID from FormApprovalRecords where Invalid = 0 and Del = 0 and WorkFlowID in (select WFID from WorkFlow where Invalid = 0 and Del = 0 and FormID = 0 and FlowCode = 'Accept') and StateCode = 0 " +
                "and CHARINDEX(',' + convert(varchar, " + UID + ") + ',',',' + CanApprovalUID + ',')> 0))  order by FARID desc";
                SourceDT = MsSQLDbHelper.Query(_sql).Tables[0];
                break;

            case "accepting":  //任务-我处理中 Update 20201214
                _sql = "" +
                //一阶 关联相关表
                "select a.*,b.FormName,b.ShortTableName,c.EMail,c.WorkTel,c.WorkMobilePhone,c.AdDepartment from FormApprovalRecords a left join Forms b on a.FormID=b.FormID left join UserInfo c on a.UID=c.UID " +
                //二阶 得到在受理阶段已经经过受理的记录
                "where FormNumber in( " +
                "select FormNumber from FormApprovalRecords where FARID in (select max(FARID) from FormApprovalRecords where WorkFlowID in (select WFID from WorkFlow where Invalid=0 and Del=0 and FlowCode = 'Accept') and Invalid=0 and Del=0 group by FormID,FormsID) " +
                "and StateCode>0 and ApprovalUID=" + UID + ") " +
                //三阶 并且在结案阶段但未结案的
                "and WorkFlowID in (select WFID from WorkFlow where Invalid=0 and Del=0 and FlowCode = 'Finish') and StateCode = 0 " +
                //四阶 去除在任意审批阶段被驳回的记录
                "and FormNumber not in(select FormNumber from FormApprovalRecords where FARID in (select max(FARID) from FormApprovalRecords where StateCode < 0 and Invalid = 0 and Del = 0 group by FormID,FormsID)) " +
                "and CHARINDEX(',' + convert(varchar, " + UID + ") + ',',',' + CanApprovalUID + ',')> 0 and a.Invalid=0 and a.Del=0 and b.Invalid = 0 and b.Del = 0 order by FARID desc";
                SourceDT = MsSQLDbHelper.Query(_sql).Tables[0];
                break;

            case "finish":  //任务-我处完成 Update 20201214
                _sql = "select a.*,b.FormName,b.ShortTableName,c.EMail,c.WorkTel,c.WorkMobilePhone,c.AdDepartment from FormApprovalRecords a left join Forms b on a.FormID=b.FormID left join UserInfo c on a.UID=c.UID where WorkFlowID in (select WFID from WorkFlow where FlowCode='Finish') and StateCode>0 and ApprovalUID= " + UID + " and CHARINDEX(',' + convert(varchar,  " + UID + ") + ',', ',' + CanApprovalUID + ',') > 0 and a.Invalid = 0 and a.Del = 0 and b.Invalid = 0 and b.Del = 0 order by FARID desc";
                SourceDT = MsSQLDbHelper.Query(_sql).Tables[0];
                break;

        }

        if (SourceDT != null && SourceDT.Rows.Count > 0)
        {
            TargetDT = SourceDT.AsEnumerable().Skip((IntPage - 1) * IntLimit).Take(IntLimit).CopyToDataTable<DataRow>();

            if (TargetDT != null && TargetDT.Rows.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                DataRow[] _rows = TargetDT.Select("", "DateCreated desc");

                foreach (DataRow _dr in _rows)
                {
                    string str1 = string.Format(strTemp, _dr["ModuleID"].toJsonTrim(), _dr["FormID"].toJsonTrim(), _dr["FormsID"].toJsonTrim(), _dr["FormNumber"].toJsonTrim(), _dr["WorkFlowID"].toJsonTrim(), _dr["NodeName"].toJsonTrim(), _dr["CanApprovalUID"].toJsonTrim(), _dr["ApprovalUID"].toJsonTrim(), _dr["ApprovalDisplayName"].toJsonTrim(), _dr["StateCode"].toJsonTrim(), _dr["ApprovalState"].toJsonTrim(), _dr["ApprovalTime"].toDateWeekTime(), _dr["ApprovalIP"].toJsonTrim(), _dr["UID"].toJsonTrim(), _dr["DisplayName"].toJsonTrim(), _dr["Note"].toJsonTrim(), _dr["DateCreated"].toDateWeekTime(), _dr["DateModified"].toJsonTrim(), _dr["Invalid"].toJsonTrim(), _dr["Del"].toJsonTrim(), _dr["FormName"].toJsonTrim(), _dr["ShortTableName"].toJsonTrim(), _dr["EMail"].toJsonTrim(), _dr["WorkTel"].toJsonTrim(), _dr["WorkMobilePhone"].toJsonTrim(), _dr["AdDepartment"].toJsonTrim());

                    sb.Append("{" + str1 + "},");
                }
                string json = sb.ToString();

                flag = json.Substring(0, json.Length - 1);
                flag = "{\"code\": 0, \"msg\": \"\", \"count\": " + SourceDT.Rows.Count + ", \"data\": [" + flag + "] }";
            }
        }

        return flag;
    }

    private string GetStrTpl()
    {
        string strTpl = "\"ModuleID\":\"{0}\", \"FormID\":\"{1}\", \"FormsID\":\"{2}\", \"FormNumber\":\"{3}\", \"WorkFlowID\":\"{4}\", \"NodeName\":\"{5}\", \"CanApprovalUID\":\"{6}\", \"ApprovalUID\":\"{7}\", \"ApprovalDisplayName\":\"{8}\", \"StateCode\":\"{9}\", \"ApprovalState\":\"{10}\", \"ApprovalTime\":\"{11}\", \"ApprovalIP\":\"{12}\", \"UID\":\"{13}\", \"DisplayName\":\"{14}\", \"Note\":\"{15}\", \"DateCreated\":\"{16}\", \"DateModified\":\"{17}\", \"Invalid\":\"{18}\", \"Del\":\"{9}\", \"FormName\":\"{20}\", \"ShortTableName\":\"{21}\", \"EMail\":\"{22}\", \"WorkTel\":\"{23}\", \"WorkMobilePhone\":\"{24}\", \"AdDepartment\":\"{25}\"";

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