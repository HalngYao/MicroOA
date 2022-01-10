<%@ WebHandler Language="C#" Class="OnDutyFormList" %>

using System;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroPublicHelper;
using MicroDTHelper;
using MicroAuthHelper;

public class OnDutyFormList : IHttpHandler, System.Web.SessionState.IRequiresSessionState
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

        string flag = string.Empty,
                strTpl = "\"code\":{0},\"msg\":\"{1}\",\"filepath\":\"{2}\",\"title\":\"{3}\",\"cols\":\"{4}\" ,\"data\":{5}",
                Action = context.Server.UrlDecode(context.Request.Form["action"]),
                ModuleID = context.Server.UrlDecode(context.Request.Form["mid"]),
                ShortTableName = context.Server.UrlDecode(context.Request.Form["stn"]),
                FormID = context.Server.UrlDecode(context.Request.Form["formid"]),
                FormsID = context.Server.UrlDecode(context.Request.Form["formsid"]);

        flag = "{" + string.Format(strTpl, 0, MicroPublic.GetMsg("DenyURLError"), "", "", "", "[]") + "}";

        ////测试数据
        //Action = "View";
        //ModuleID = "4";
        //ShortTableName = "OnDutyForm";
        //FormID = "13";
        //FormsID = "29";


        if (!string.IsNullOrEmpty(Action) && !string.IsNullOrEmpty(ShortTableName) && !string.IsNullOrEmpty(FormID) && !string.IsNullOrEmpty(FormsID))
        {
            if (Action.ToLower() == "view" || Action.ToLower() == "modify")
                flag = GetOnDutyFormList(ShortTableName, FormID, FormsID);
        }

        context.Response.Write(flag);
    }


    private string GetOnDutyFormList(string ShortTableName, string FormID, string FormsID)
    {
        string flag = string.Empty;
        string strTpl = "\"code\":{0},\"msg\":\"{1}\",\"filepath\":\"{2}\",\"title\":\"{3}\",\"cols\":\"{4}\" ,\"data\":{5}";
        int Code = 0;

        string _sql = "select * from HROnDutyForm where Invalid=0 and Del=0 and ParentID=0 and DutyID=@DutyID";
        SqlParameter[] _sp = { new SqlParameter("@DutyID", SqlDbType.Int) };

        _sp[0].Value = FormsID.toInt();

        DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

        if (_dt != null && _dt.Rows.Count > 0)
        {
            string FilePath = _dt.Rows[0]["FilePath"].toStringTrim();
            if (!string.IsNullOrEmpty(FilePath))
                flag = MicroDataTable.GetOnDutyDataTableList("", FilePath);
            else
                flag = "{" + string.Format(strTpl, Code, "读取Excel失败，不正确的文件路径", "", "", "", "[]") + "}";
        }
        else
            flag = "{" + string.Format(strTpl, Code, "读取Excel失败，未能匹配到有效记录", "", "", "", "[]") + "}";

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