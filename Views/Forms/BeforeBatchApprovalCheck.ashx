<%@ WebHandler Language="C#" Class="BeforeBatchApprovalCheck" %>

using System;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroUserHelper;
using MicroAuthHelper;
using MicroPublicHelper;

public class BeforeBatchApprovalCheck : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = MicroPublic.GetMsg("BatchApprovalFailed"),
                Action = context.Server.UrlDecode(context.Request.Form["action"]),
                ShortTableName = context.Server.UrlDecode(context.Request.Form["stn"]),
                ModuleID = context.Server.UrlDecode(context.Request.Form["mid"]),
                FormID = context.Server.UrlDecode(context.Request.Form["formid"]),
                FormsIDs = context.Server.UrlDecode(context.Request.Form["formsids"]);


        if (!string.IsNullOrEmpty(Action) && !string.IsNullOrEmpty(ShortTableName) && !string.IsNullOrEmpty(ModuleID) && !string.IsNullOrEmpty(FormID) && !string.IsNullOrEmpty(FormsIDs))
            flag = SetBatchApprovalKey(Action, ShortTableName, ModuleID, FormID, FormsIDs);

        context.Response.Write(flag);

    }


    /// <summary>
    /// 批量审批前把需要审批的主键值插入表中FormBatchApprovalKey，在批量审批页面调用
    /// </summary>
    /// <param name="Action"></param>
    /// <param name="ShortTableName"></param>
    /// <param name="ModuleID"></param>
    /// <param name="FormID"></param>
    /// <param name="FormsIDs"></param>
    /// <returns></returns>
    private string SetBatchApprovalKey(string Action, string ShortTableName, string ModuleID, string FormID, string FormsIDs)
    {
        string flag = MicroPublic.GetMsg("BatchApprovalFailed");
        string _sql = "insert into FormBatchApprovalKey (ApprovalAction, ShortTableName, FormID, FormsIDs, UID, DisplayName) values (@ApprovalAction, @ShortTableName, @FormID, @FormsIDs, @UID, @DisplayName)";

        SqlParameter[] _sp = { new SqlParameter("@ApprovalAction", SqlDbType.VarChar, 50),
            new SqlParameter("@ShortTableName",SqlDbType.VarChar,100),
            new SqlParameter("@FormID",SqlDbType.Int),
            new SqlParameter("@FormsIDs",SqlDbType.VarChar,8000),
            new SqlParameter("@UID",SqlDbType.Int),
            new SqlParameter("@DisplayName",SqlDbType.NVarChar,200),
            };

        _sp[0].Value = Action.toStringTrim();
        _sp[1].Value = ShortTableName.toStringTrim();
        _sp[2].Value = FormID.toInt();
        _sp[3].Value = FormsIDs.toStringTrim();
        _sp[4].Value = MicroUserInfo.GetUserInfo("UID").toInt();
        _sp[5].Value = MicroUserInfo.GetUserInfo("DisplayName");

        if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
            flag = MicroPublic.GetMsg("BatchApproval");

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