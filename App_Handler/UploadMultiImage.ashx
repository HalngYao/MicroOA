<%@ WebHandler Language="C#" Class="UploadMultiImage" %>

using System;
using System.Web;
using MicroUserHelper;
using MicroAuthHelper;

public class UploadMultiImage : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string Dir = context.Server.UrlDecode(context.Request.QueryString["dir"]);
        Dir = string.IsNullOrEmpty(Dir) == true ? "UploadImages" : Dir;

        string flag = "{\"errno\":0, \"data\": [\"图片1地址.jpg\", \"图片2地址.jpg\" ]}";

        if (((MicroUserInfo)context.Session["UserInfo"]).GetIsLogin())
        {
            try
            {
                HttpFileCollection files = context.Request.Files;
                if (files.Count > 0)
                {
                    string filePath = "/Resource/UploadFiles/Images/" + Dir + "/";
                    string src = string.Empty;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //设置文件名
                        string fileNewName = DateTime.Now.ToString("yyyyMMddHHmmssff") + "_img" + i.ToString() + "_" + System.IO.Path.GetFileName(files[i].FileName);
                        //保存文件
                        string fileFullPath = filePath + fileNewName;

                        files[i].SaveAs(context.Server.MapPath(fileFullPath));
                        src += "\"" + fileFullPath + "\",";
                    }
                    src = src.Substring(0, src.Length - 1);
                    flag = "{\"errno\": 0, \"count\":" + files.Count + ", \"data\": [" + src + "]}";
                }
            }
            catch (Exception ex) { flag = ex.ToString(); }
            context.Response.Write(flag);
        }
    }
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}