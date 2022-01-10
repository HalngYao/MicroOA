<%@ WebHandler Language="C#" Class="MicroFormCheckApproval" %>

using System;
using System.Web;
using MicroAuthHelper;
using MicroApprovalHelper;

public class MicroFormCheckApproval : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = string.Format(GetStrTpl(), "0", "micro-layer-class4","False"),
                Action = context.Server.UrlDecode(context.Request.Form["action"]),
                ShortTableName = context.Server.UrlDecode(context.Request.Form["stn"]),
                ModuleID = context.Server.UrlDecode(context.Request.Form["mid"]),
                FormID = context.Server.UrlDecode(context.Request.Form["formid"]),
                FormsID = context.Server.UrlDecode(context.Request.Form["pkv"]);

        ////测试数据
        //Action = "View";
        //ShortTableName = "Leave";
        //ModuleID = "4";
        //FormID = "7";
        //FormsID = "4164";

        if (!string.IsNullOrEmpty(Action) && !string.IsNullOrEmpty(ShortTableName) && !string.IsNullOrEmpty(ModuleID) && !string.IsNullOrEmpty(FormID) && !string.IsNullOrEmpty(FormsID))
            flag = GetCheckApproval(Action, ShortTableName, ModuleID, FormID, FormsID);

        flag = "{" + flag + "}";
        context.Response.Write(flag);

    }

    /// <summary>
    /// 检查审批，根据状态显示需要显示的按钮
    /// </summary>
    /// <param name="FormID"></param>
    /// <param name="ShortTableName"></param>
    /// <param name="FormsID"></param>
    /// <param name="Fields"></param>
    /// <returns></returns>
    private string GetCheckApproval(string Action, string ShortTableName,string ModuleID, string FormID,  string FormsID)
    {
        var getApproval = MicroApproval.GetApproval(Action, ShortTableName, ModuleID, FormID, FormsID);
        string flag = string.Format(GetStrTpl(), "0", getApproval.ClassName, getApproval.Withdrawal);

        return flag;

    }

    private string GetStrTpl()
    {
        string flag = "\"Btn\":\"{0}\",\"Class\":\"{1}\",\"Withdrawal\":\"{2}\"";
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