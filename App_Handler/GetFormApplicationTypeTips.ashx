<%@ WebHandler Language="C#" Class="GetFormApplicationTypeTips" %>

using System;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;

public class GetFormApplicationTypeTips : IHttpHandler, System.Web.SessionState.IRequiresSessionState {

    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/plain";
        string flag = string.Empty,
                Action = context.Server.UrlDecode(context.Request.Form["action"]),
                ID = context.Server.UrlDecode(context.Request.Form["id"]);

        //Action = "view";
        //ID = "22";

        if (!string.IsNullOrEmpty(Action) && !string.IsNullOrEmpty(ID))
            flag = GetTips(ID);

        context.Response.Write(flag);
    }

    private string GetTips(string FATID)
    {
        string flag = string.Empty;

        string _sql = "select Description from FormApplicationType where Invalid=0 and Del=0 and FATID=@FATID";
        SqlParameter[] _sp = { new SqlParameter("@FATID", SqlDbType.Int) };

        _sp[0].Value = FATID.toInt();

        DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];
        if (_dt != null && _dt.Rows.Count > 0)
            flag = _dt.Rows[0]["Description"].toStringTrim();

        return flag;
    }


    public bool IsReusable {
        get {
            return false;
        }
    }

}