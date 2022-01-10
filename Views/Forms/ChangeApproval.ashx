<%@ WebHandler Language="C#" Class="ChangeApproval" %>

using System;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroPublicHelper;
using MicroAuthHelper;

public class ChangeApproval : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = MicroPublic.GetMsg("DenyUrlParaError"),
             Action = context.Server.UrlDecode(context.Request.Form["action"]),
             MID = context.Server.UrlDecode(context.Request.Form["mid"]),
             WFID = context.Server.UrlDecode(context.Request.Form["wfid"]),
             FieldName = context.Server.UrlDecode(context.Request.Form["fieldname"]),
             Fields = context.Server.UrlDecode(context.Request.Form["fields"]);

        //测试数据
        //mid:4,wfid:7,fieldname:ApprovalByIDStr,fields:3,4
        //Action = "modify";
        //WFID = "7";
        //FieldName = "ApprovalByIDStr";
        //Fields = "3,4";

        if (Action == "modify" && !string.IsNullOrEmpty(WFID) && !string.IsNullOrEmpty(FieldName) && !string.IsNullOrEmpty(Fields))
            flag = ModifyApproval(WFID, FieldName, Fields);


        context.Response.Write(flag);
    }

    private string ModifyApproval(string WFID, string FieldName, string Fields)
    {

        string flag = MicroPublic.GetMsg("ModifyFailed");
        try
        {
            string _sql = "update WorkFlow set " + FieldName + "=@" + FieldName + " where WFID=@WFID";
            SqlParameter[] _sp = {new SqlParameter("@"+FieldName,SqlDbType.VarChar,200),
                                  new SqlParameter("@WFID", SqlDbType.Int)
                                    };

            _sp[0].Value = Fields.toStringTrim();
            _sp[1].Value = WFID.toInt();

            if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
                flag = MicroPublic.GetMsg("Modify");

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