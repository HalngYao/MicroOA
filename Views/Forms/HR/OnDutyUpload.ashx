<%@ WebHandler Language="C#" Class="OnDutyUpload" %>

using System;
using System.IO;
using System.Web;
using System.Data;
using MicroAuthHelper;
using MicroDBHelper;
using MicroDTHelper;
using MicroPublicHelper;
using MicroUserHelper;


public class OnDutyUpload : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();


        string flag = string.Empty;
        string strTpl = "\"code\":{0},\"msg\":\"{1}\",\"filepath\":\"{2}\",\"title\":\"{3}\",\"cols\":\"{4}\" ,\"data\":{5}";
        int Code = 0;

        //这里只能用<input type="file" />才能有效果,因为服务器控件是HttpInputFile类型
        if (((MicroUserInfo)context.Session["userInfo"]).GetIsLogin())
        {
            try
            {
                HttpFileCollection files = context.Request.Files;
                if (files.Count > 0)
                {
                    string UserName = MicroUserInfo.GetUserInfo("UserName");
                    //设置文件名
                    string FileName = Path.GetFileName(files[0].FileName);
                    string FileNewName = UserName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + FileName;

                    //保存到临时文件夹
                    string FilePath = "/Resource/UploadFiles/Forms/Temp/" + FileNewName;
                    string fullFilePath = context.Server.MapPath(FilePath);
                    files[0].SaveAs(fullFilePath);

                    flag = MicroDataTable.GetOnDutyDataTableList(FileNewName, FilePath);

                }
                else
                    flag = "{" + string.Format(strTpl, Code, "导入失败，文件上传失败", "", "", "", "") + "}";

            }
            catch (Exception ex)
            {
                flag = "{" + string.Format(strTpl, Code, "导入失败，详细错误：" + ex.ToString(), "", "", "", "") + "}";
            }
        }
        else
            flag = "{" + string.Format(strTpl, Code, "导入失败，用户未登录", "", "", "", "") + "}";

        context.Response.Write(flag);
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}