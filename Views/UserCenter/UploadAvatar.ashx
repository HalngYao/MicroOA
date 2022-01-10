<%@ WebHandler Language="C#" Class="UploadAvatar" %>

using System;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroPublicHelper;
using MicroUserHelper;
using MicroAuthHelper;

public class UploadAvatar : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string msg = MicroPublic.GetMsg("UploadFailed");
        string flag = "{\"code\": 0,\"msg\": \"" + msg + "\",\"data\": { \"src\": \"\"}}";
        string filePath = "~/Resource/UploadFiles/Images/Avatar/";

        if (((MicroUserInfo)context.Session["UserInfo"]).GetIsLogin())
        {
            try
            {
                HttpFileCollection files = context.Request.Files;
                if (files.Count > 0)
                {
                    //设置文件名
                    string fileNewName = DateTime.Now.ToString("yyyyMMddHHmmssff") + "_" + System.IO.Path.GetFileName(files[0].FileName);
                    //保存文件
                    filePath += fileNewName;
                    files[0].SaveAs(context.Server.MapPath(filePath));

                    string _sql = "update UserInfo set Avatar=@Avatar where UID=@UID";
                    SqlParameter[] _sp = { new SqlParameter("@Avatar",SqlDbType.VarChar,200),
                                       new SqlParameter("@UID",SqlDbType.Int)   };
                    _sp[0].Value = filePath;
                    _sp[1].Value = ((MicroUserInfo)context.Session["UserInfo"]).UID;


                    if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
                        ((MicroUserInfo)context.Session["UserInfo"]).Avatar = filePath;

                    msg = MicroPublic.GetMsg("Upload");
                    flag = "{\"code\": " + files.Count + ",\"msg\": \"" + msg + "\",\"data\": { \"src\": \"" + filePath + "\"}}";
                }

            }
            catch { }
            context.Response.Write(flag);
        }
    }
    public bool IsReusable {
        get {
            return false;
        }
    }

}