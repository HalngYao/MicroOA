<%@ WebHandler Language="C#" Class="CtrlPublicBatchOperationKey" %>

using System;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroUserHelper;
using MicroAuthHelper;
using MicroPublicHelper;

public class CtrlPublicBatchOperationKey : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = MicroPublic.GetMsg("BatchOperationFailed"),
                Action = context.Server.UrlDecode(context.Request.Form["action"]),
                ShortTableName = context.Server.UrlDecode(context.Request.Form["stn"]),
                ModuleID = context.Server.UrlDecode(context.Request.Form["mid"]),
                IDs = context.Server.UrlDecode(context.Request.Form["ids"]);


        if (!string.IsNullOrEmpty(Action) && !string.IsNullOrEmpty(ShortTableName) && !string.IsNullOrEmpty(ModuleID) && !string.IsNullOrEmpty(IDs))
            flag = SetBatchOperationKey(Action, ShortTableName, ModuleID, IDs);

        context.Response.Write(flag);
    }


    private string SetBatchOperationKey(string Action, string ShortTableName, string ModuleID, string IDs)
    {
        string flag = MicroPublic.GetMsg("BatchOperationFailed");
        string _sql = "insert into BatchOperationKey (BatAction, ShortTableName, ModuleID, IDs, UID, DisplayName) values (@BatAction, @ShortTableName, @ModuleID, @IDs, @UID, @DisplayName)";

        SqlParameter[] _sp = { new SqlParameter("@BatAction", SqlDbType.VarChar, 50),
            new SqlParameter("@ShortTableName",SqlDbType.VarChar,100),
            new SqlParameter("@ModuleID",SqlDbType.Int),
            new SqlParameter("@IDs",SqlDbType.VarChar,8000),
            new SqlParameter("@UID",SqlDbType.Int),
            new SqlParameter("@DisplayName",SqlDbType.NVarChar,200),
            };

        _sp[0].Value = Action.toStringTrim();
        _sp[1].Value = ShortTableName.toStringTrim();
        _sp[2].Value = ModuleID.toInt();
        _sp[3].Value = IDs.toStringTrim();
        _sp[4].Value = MicroUserInfo.GetUserInfo("UID").toInt();
        _sp[5].Value = MicroUserInfo.GetUserInfo("DisplayName");

        if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
            flag = MicroPublic.GetMsg("BatchOperation");

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