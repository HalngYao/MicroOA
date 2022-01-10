<%@ WebHandler Language="C#" Class="GetMicroFormCode" %>

using System;
using System.Web;
using MicroFormHelper;
using MicroAuthHelper;

public class GetMicroFormCode : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
    //***说明***
    //主用是获取动态生成表单验证、SelectChange事件、提交表单等JS代码
    //暂时用不上，目前都写在控件上了（MicroForm.ascx）

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = string.Empty;

        string Action = context.Server.UrlDecode(context.Request.Form["action"]);
        string ShortTableName = context.Server.UrlDecode(context.Request.Form["stn"]);
        string FormCode = context.Server.UrlDecode(context.Request.Form["fc"]);
        string MID = context.Server.UrlDecode(context.Request.Form["mid"]);
        string FormID = context.Server.UrlDecode(context.Request.Form["formid"]);
        string PrimaryKeyValue = context.Server.UrlDecode(context.Request.Form["pkv"]);
        string FormType = context.Server.UrlDecode(context.Request.Form["formtype"]);
        string IsApprovalForm = context.Server.UrlDecode(context.Request.Form["isapprovalform"]);
        string ShowHeader = context.Server.UrlDecode(context.Request.Form["showheader"]);  //是否显示头部
        

        switch (FormCode)
        {
            //返回所有JS代码，如下三种
            case "all":
                flag = GetJsFormCode(Action, ShortTableName,MID, FormID,  PrimaryKeyValue, FormType, IsApprovalForm.toBoolean(), ShowHeader.toBoolean());
                break;

            //返回表单验证JS代码，如不允许为空值检测等
            case "vcode":
                flag = GetJsFormVerifyCode(Action, ShortTableName,MID, FormID,  PrimaryKeyValue, FormType, IsApprovalForm.toBoolean(), ShowHeader.toBoolean());
                break;

            //返回控件Change事件JS代码，如Select Change事件，CheckBox Click事件等
            case "ccode":
                flag = GetJsFormCtrlChangeCode(Action, ShortTableName,MID, FormID,  PrimaryKeyValue, FormType, IsApprovalForm.toBoolean(), ShowHeader.toBoolean());
                break;

            //返回表单提交按钮JS代码submit
            case "scode":
                flag = GetJsFormSubmitCode(Action, ShortTableName,MID, FormID,  PrimaryKeyValue, FormType, IsApprovalForm.toBoolean(), ShowHeader.toBoolean());
                break;
        }

        context.Response.Write(flag);
    }

    /// <summary>
    /// 紧返回JsFormVerifyCode
    /// </summary>
    /// <param name="ShortTableName">短表名</param>
    /// <returns></returns>
    private string GetJsFormVerifyCode(string Action, string ShortTableName, string MID, string FormID, string PrimaryKeyValue, string FormType, Boolean IsApprovalForm, Boolean ShowHeader)
    {
        string flag = string.Empty;
        flag = MicroForm.GetHtmlCode(Action, ShortTableName, MID, FormID, PrimaryKeyValue, FormType, IsApprovalForm, ShowHeader).JsFormVerifyCode;
        return flag;
    }

    /// <summary>
    /// 紧返回JsFormCtrlChangeCode
    /// </summary>
    /// <param name="ShortTableName"></param>
    /// <returns></returns>
    private string GetJsFormCtrlChangeCode(string Action, string ShortTableName, string MID, string FormID, string PrimaryKeyValue, string FormType, Boolean IsApprovalForm, Boolean ShowHeader)
    {
        string flag = string.Empty;
        flag = MicroForm.GetHtmlCode(Action, ShortTableName, MID, FormID, PrimaryKeyValue, FormType, IsApprovalForm, ShowHeader).JsFormCtrlChangeCode;
        return flag;
    }

    /// <summary>
    /// 紧返回JsFormSubmitCode
    /// </summary>
    /// <param name="ShortTableName"></param>
    /// <returns></returns>
    private string GetJsFormSubmitCode(string Action, string ShortTableName, string MID, string FormID, string PrimaryKeyValue, string FormType, Boolean IsApprovalForm, Boolean ShowHeader)
    {
        string flag = string.Empty;
        flag = MicroForm.GetHtmlCode(Action, ShortTableName, MID, FormID, PrimaryKeyValue, FormType, IsApprovalForm, ShowHeader).JsFormSubmitCode;
        return flag;
    }

    /// <summary>
    /// 返回所有JSFormCode，包含：JsFormVerifyCode、JsFormCtrlChangeCode、JsFormSubmitCode
    /// </summary>
    /// <param name="ShortTableName"></param>
    /// <returns></returns>
    private string GetJsFormCode(string Action, string ShortTableName, string MID, string FormID, string PrimaryKeyValue, string FormType, Boolean IsApprovalForm, Boolean ShowHeader)
    {
        string flag = string.Empty;
        flag = MicroForm.GetHtmlCode(Action, ShortTableName, MID, FormID, PrimaryKeyValue, FormType, IsApprovalForm, ShowHeader).JsFormCode;
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