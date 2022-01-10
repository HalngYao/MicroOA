<%@ WebHandler Language="C#" Class="Upload" %>

using System;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroUserHelper;
using MicroAuthHelper;

public class Upload : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string msg = string.Empty;
        string error = string.Empty;
        string result = string.Empty;
        string filePath = string.Empty;
        string fileNewName = string.Empty;
        //这里只能用<input type="file" />才能有效果,因为服务器控件是HttpInputFile类型
        if (((MicroUserInfo)context.Session["userInfo"]).GetIsLogin())
        {
            try
            {
                HttpFileCollection files = context.Request.Files;
                if (files.Count > 0)
                {
                    //设置文件名
                    fileNewName = DateTime.Now.ToString("yyyyMMddHHmmssff") + "_" + System.IO.Path.GetFileName(files[0].FileName);
                    //保存文件
                    files[0].SaveAs(context.Server.MapPath("~/User/Upload/" + fileNewName));
                    filePath = "~/User/Upload/" + fileNewName;
                    msg = "文件上传成功！";

                    string _sql = "update UserInfo set Avatar=@Avatar where UserName=@UserName";
                    SqlParameter[] _sp = { new SqlParameter("@Avatar",SqlDbType.VarChar,200),
                                   new SqlParameter("@UserName",SqlDbType.VarChar,50)   };
                    _sp[0].Value = filePath;
                    _sp[1].Value = ((MicroUserInfo)context.Session["userInfo"]).UserName;

                    MsSQLDbHelper.ExecuteSql(_sql, _sp);
                    
                    result = "{\"code\": " + files.Count + ",\"msg\": \"" + msg + "\",\"data\": { \"src\": \"" + filePath + "\"}}";
                }
                else
                {
                    error = "文件上传失败！";
                    result = "{ \"error\":" + error + "}";
                }
            }
            catch { }
            context.Response.Write(result);
        }
    }
    public bool IsReusable {
        get {
            return false;
        }
    }

}