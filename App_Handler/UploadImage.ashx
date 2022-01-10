<%@ WebHandler Language="C#" Class="UploadImage" %>

using System;
using System.Web;
using MicroUserHelper;
using MicroAuthHelper;

public class UploadImage : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string Dir = context.Server.UrlDecode(context.Request.QueryString["dir"]);
        Dir = string.IsNullOrEmpty(Dir) == true ? "UploadImages" : Dir;
        string flag = string.Empty;

        //这里只能用<input type="file" />才能有效果,因为服务器控件是HttpInputFile类型
        if (((MicroUserInfo)context.Session["userInfo"]).GetIsLogin())
        {
            try
            {
                HttpFileCollection files = context.Request.Files;
                if (files.Count > 0)
                {
                    //服务器路径
                    string filePath = "/Resource/UploadFiles/Images/" + Dir + "/";
                    //设置新文件名
                    string fileNewName = DateTime.Now.ToString("yyyyMMddHHmmssff") + "_" + System.IO.Path.GetFileName(files[0].FileName);
                    string fileFullPath = filePath + fileNewName;

                    //保存文件
                    files[0].SaveAs(context.Server.MapPath(fileFullPath));

                    flag = "{\"code\": " + files.Count + ",\"msg\": \"文件上传成功！\",\"data\": { \"src\": \"" + fileFullPath + "\"}}";
                }
                else
                {
                   string error = "文件上传失败！";
                    flag = "{ \"error\":" + error + "}";
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