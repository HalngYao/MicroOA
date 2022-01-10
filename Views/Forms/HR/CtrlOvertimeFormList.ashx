<%@ WebHandler Language="C#" Class="CtrlOvertimeFormList" %>

using System;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroPublicHelper;
using MicroWorkFlowHelper;
using MicroAuthHelper;


public class CtrlOvertimeFormList : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = MicroPublic.GetMsg("SubmitURLError");

        string Action = context.Server.UrlDecode(context.Request.Form["action"]);
        string ShortTableName = context.Server.UrlDecode(context.Request.Form["stn"]);
        string ModuleID = context.Server.UrlDecode(context.Request.Form["mid"]);
        string FormID = context.Server.UrlDecode(context.Request.Form["formid"]);
        string FormsID = context.Server.UrlDecode(context.Request.Form["formsid"]);
        string IsApprovalForm = context.Server.UrlDecode(context.Request.Form["isapprovalform"]);
        string Fields = context.Server.UrlDecode(context.Request.Form["fields"]);

        FormID = IsApprovalForm.toBoolean() == false ? "0" : FormID;

        //Action = "Add";
        //ShortTableName = "Overtime";
        //ModuleID = "4";
        //FormID = "5";
        //FormsID = "168";
        //IsApprovalForm = "True";
        //Fields = "%7B%22hidFormID%22:%22%22,%22txtFormNumber%22:%22HROT2021010031%22,%22txtFormState%22:%22%E5%A1%AB%E5%86%99%E7%94%B3%E8%AF%B7%5BFill%20in%5D%22,%22hidStateCode%22:%22GetFormStateCode%22,%22ApprovalNode21%22:%22362%22,%22ApprovalNode24%22:%22362%22,%22ApprovalNode25%22:%22362%22%7D";
        //Fields = context.Server.UrlDecode(Fields);

        if (!string.IsNullOrEmpty(Action) && !string.IsNullOrEmpty(ShortTableName) && !string.IsNullOrEmpty(ModuleID) && !string.IsNullOrEmpty(FormID) && !string.IsNullOrEmpty(IsApprovalForm) && !string.IsNullOrEmpty(Fields))
        {
            Action = Action.ToLower();

            if (Action == "add")
                flag = SetSubmitForm(Action, ShortTableName, ModuleID, FormID, FormsID, IsApprovalForm, Fields);

            if (Action == "modify")
                flag = SetModifyForm(Action, ShortTableName, ModuleID, FormID, FormsID, IsApprovalForm, Fields);
        }

        context.Response.Write(flag);
    }

    /// <summary>
    /// 提交表单
    /// </summary>
    /// <param name="Action"></param>
    /// <param name="ShortTableName"></param>
    /// <param name="ModuleID"></param>
    /// <param name="FormID"></param>
    /// <param name="FormsID"></param>
    /// <param name="IsApprovalForm"></param>
    /// <param name="FormFields"></param>
    /// <returns></returns>
    private static string SetSubmitForm(string Action, string ShortTableName, string ModuleID, string FormID, string FormsID, string IsApprovalForm, string FormFields)
    {
        string flag = MicroPublic.GetMsg("SubmitURLError"),
                TableName = MicroPublic.GetTableName(ShortTableName);
        try
        {
            string _sql = "update " + TableName + " set FormState=@FormState, StateCode=@StateCode where OvertimeID=@OvertimeID or ParentID=@ParentID";
            SqlParameter[] _sp = { new SqlParameter("@FormState",SqlDbType.NVarChar,200),
                         new SqlParameter("@StateCode",SqlDbType.Int),
                         new SqlParameter("@OvertimeID",SqlDbType.Int),
                         new SqlParameter("@ParentID",SqlDbType.Int)
                    };

            _sp[0].Value = MicroWorkFlow.GetApprovalState(0); //0 = 等待审批[Waiting]，-3 = 填写申请[Fill in]
            _sp[1].Value = 0; //0 = 等待审批[Waiting]，-3 = 填写申请[Fill in]
            _sp[2].Value = FormsID.toInt();
            _sp[3].Value = FormsID.toInt();

            if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
            {
                //在提交申请单时再更新加班小时汇总数据（因为在填写申请时加班小时汇总是以申请动作计算的）
                //MicroHR.SetBulkUpdatePersonalRecord(FormsID); //批量更新用户加班小时汇总（在提交表单走审批流程时） (已取消，通过限制草稿箱有记录不能再新操作)

                if (IsApprovalForm.toBoolean())
                {
                    if (MicroWorkFlow.SetWorkFlow(ShortTableName, TableName, ModuleID, FormID, FormsID, FormFields))
                        flag = MicroPublic.GetMsg("Submit");
                    else
                        flag = MicroPublic.GetMsg("SaveWolrkFlowFailed");
                }
            }
        }
        catch (Exception ex) { flag = ex.ToString(); }

        return flag;
    }


    /// <summary>
    /// 修改表单
    /// </summary>
    /// <param name="Action"></param>
    /// <param name="ShortTableName"></param>
    /// <param name="ModuleID"></param>
    /// <param name="FormID"></param>
    /// <param name="FormsID"></param>
    /// <param name="IsApprovalForm"></param>
    /// <param name="FormFields"></param>
    /// <returns></returns>
    private static string SetModifyForm(string Action, string ShortTableName, string ModuleID, string FormID, string FormsID, string IsApprovalForm, string FormFields)
    {
        string flag = MicroPublic.GetMsg("SubmitURLError"),
                TableName = MicroPublic.GetTableName(ShortTableName);
        try
        {
            string _sql = "update " + TableName + " set FormState=@FormState, StateCode=@StateCode where OvertimeID=@OvertimeID or ParentID=@ParentID";
            SqlParameter[] _sp = { new SqlParameter("@FormState",SqlDbType.NVarChar,200),
                         new SqlParameter("@StateCode",SqlDbType.Int),
                         new SqlParameter("@OvertimeID",SqlDbType.Int),
                         new SqlParameter("@ParentID",SqlDbType.Int)
                    };

            _sp[0].Value = MicroWorkFlow.GetApprovalState(0); //0 = 等待审批[Waiting]，-3 = 填写申请[Fill in]
            _sp[1].Value = 0; //0 = 等待审批[Waiting]，-3 = 填写申请[Fill in]
            _sp[2].Value = FormsID.toInt();
            _sp[3].Value = FormsID.toInt();

            if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
            {
                //MicroHR.SetBulkUpdatePersonalRecord(FormsID); //批量更新用户加班小时汇总（在提交表单走审批流程时）

                if (IsApprovalForm.toBoolean())
                {
                    if (MicroWorkFlow.SetModifyWorkFlow(ShortTableName, TableName, ModuleID, FormID, FormsID, FormFields))
                        flag = MicroPublic.GetMsg("Submit");
                    else
                        flag = MicroPublic.GetMsg("SaveWolrkFlowFailed");
                }
            }
        }
        catch (Exception ex) { flag = ex.ToString(); }

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