<%@ WebHandler Language="C#" Class="GetPublicXmSelect" %>

using System;
using System.Web;
using MicroPublicHelper;
using MicroXmSelectHelper;
using MicroAuthHelper;

public class GetPublicXmSelect : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = MicroPublic.GetMsg("DenyUrlParaError"),
         Action = context.Server.UrlDecode(context.Request.Form["action"]),
         MID = context.Server.UrlDecode(context.Request.Form["mid"]),
         FormID = context.Server.UrlDecode(context.Request.Form["formid"]),
         TableName = context.Server.UrlDecode(context.Request.Form["tn"]),
         DisplayNameField = context.Server.UrlDecode(context.Request.Form["txt"]),
         DisplayValueField = context.Server.UrlDecode(context.Request.Form["val"]),
         IsApprovalForm=context.Server.UrlDecode(context.Request.Form["isapprovalform"]),
         DefaultValue = context.Server.UrlDecode(context.Request.Form["defaultvalue"]);


        //测试数据
        //Action = "get";
        //TableName = "HRHolidayType";
        //DisplayNameField = "HolidayName";
        //DisplayValueField = "HolidayTypeID";
        //IsApprovalForm = "True";
        //DefaultValue = "";


        Action = string.IsNullOrEmpty(Action) == true ? "" : Action.ToLower();
        try
        {
            if (Action == "get" && !string.IsNullOrEmpty(TableName) && !string.IsNullOrEmpty(DisplayNameField) && !string.IsNullOrEmpty(DisplayValueField) && !string.IsNullOrEmpty(IsApprovalForm))
                flag = GetXmSelect(TableName, DisplayNameField, DisplayValueField, IsApprovalForm, FormID, DefaultValue);
        }
        catch (Exception ex)
        {
            flag = ex.ToString();
        }

        context.Response.Write(flag);
    }

    /// <summary>
    /// 获取适用于XmSelect控件的数据
    /// </summary>
    /// <param name="Fields"></param>
    /// <returns></returns>
    private string GetXmSelect(string TableName, string DisplayNameField, string DisplayValueField, string IsApprovalForm, string FormID = "", string DefaultValue = "")
    {
        string flag = string.Empty;

        TableName = TableName.toStringTrim();
        DisplayNameField = DisplayNameField.toStringTrim();
        DisplayValueField = DisplayValueField.toStringTrim();
        DefaultValue = DefaultValue.toStringTrim();

        flag = MicroXmSelect.GetXmSelect(TableName, DisplayNameField, DisplayValueField, IsApprovalForm, FormID, DefaultValue);

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