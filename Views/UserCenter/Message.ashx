<%@ WebHandler Language="C#" Class="Message" %>

using System;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroPublicHelper;
using MicroUserHelper;
using Newtonsoft.Json.Linq;
using MicroAuthHelper;

public class Message : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = MicroPublic.GetMsg("SaveURLError"),
            Action = context.Server.UrlDecode(context.Request.Form["action"]),
            ShortTableName = context.Server.UrlDecode(context.Request.Form["stn"]),
            MID = context.Server.UrlDecode(context.Request.Form["mid"]),
            NoticeID = context.Server.UrlDecode(context.Request.Form["noticid"]),
            PWD = context.Server.UrlDecode(context.Request.Form["v"]),
            Fields = context.Server.UrlDecode(context.Request.Form["fields"]);

        //测试数据
        //Action = "view";
        //Fields = "%5B%7B%22NoticeID%22:%222%22,%22Sort%22:%22%22,%22ShortTableName%22:%22ITGenForm%22,%22FormID%22:%221%22,%22FormsID%22:%221%22,%22FormNumber%22:%22IT2020080001%22,%22NoticeType%22:%22Msg%22,%22Title%22:%22IT%E9%80%9A%E7%94%A8%E7%94%B3%E8%AF%B7%E8%A1%A8%22,%22Content%22:%22Admin%20(%E7%AE%A1%E7%90%86%E5%91%98)%EF%BC%8C%E6%8F%90%E4%BA%A4%E4%BA%86%E3%80%8AIT%E9%80%9A%E7%94%A8%E7%94%B3%E8%AF%B7%E8%A1%A8%E3%80%8B%E7%94%B3%E8%AF%B7%EF%BC%8C%E7%BC%96%E5%8F%B7%EF%BC%9AIT2020080001%EF%BC%8C%E6%8B%9C%E6%89%98%E6%82%A8%E8%BF%9B%E8%A1%8C%E5%AE%A1%E6%89%B9%E3%80%82%22,%22IsRead%22:%22False%22,%22UID%22:%2215%22,%22SaveDraft%22:%22False%22,%22DateCreated%22:%222020/8/26%209:14:04%22,%22DateModified%22:%222020/8/26%209:14:04%22,%22Invalid%22:%22False%22,%22Del%22:%22False%22%7D%5D";

        if (!string.IsNullOrEmpty(Action))
        {
            Action = Action.ToLower();
            if (Action == "single" && !string.IsNullOrEmpty(NoticeID))
                SetSingleMsgRead(NoticeID);

            if (Action == "ready" && !string.IsNullOrEmpty(Fields))
                flag = SetMsgRead(context.Server.UrlDecode(Fields));

            if (Action == "unready" && !string.IsNullOrEmpty(Fields))
                flag = SetMsgUnread(context.Server.UrlDecode(Fields));

            if (Action == "readyall")
                flag = SetMsgReadAll();

            if (Action == "unreadytotal")
                flag = GetUnreadTotal();

            if (!string.IsNullOrEmpty(PWD) && !string.IsNullOrEmpty(Fields))
                flag = SetMsgDel(Action, PWD, Fields);

        }

        context.Response.Write(flag);
    }


    /// <summary>
    /// 点击单条消息把消息设置为已读
    /// </summary>
    /// <param name="NoticeID"></param>
    /// <returns></returns>
    private Boolean SetSingleMsgRead(string NoticeID)
    {
        Boolean flag = false;
        string _sql = "update Notice set IsRead=@IsRead where NoticeID=@NoticeID and UID=@UID";
        SqlParameter[] _sp = { new SqlParameter("@IsRead",SqlDbType.Bit),
                                new SqlParameter("@NoticeID",SqlDbType.Int),
                                new SqlParameter("@UID",SqlDbType.Int)
            };

        _sp[0].Value = true;
        _sp[1].Value = NoticeID.toInt();
        _sp[2].Value = MicroUserInfo.GetUserInfo("UID");

        if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
            flag = true;

        return flag;
    }

    /// <summary>
    /// 标记已选中的消息记录为已读
    /// </summary>
    /// <param name="Fields"></param>
    /// <returns></returns>
    private string SetMsgRead(string Fields)
    {
        string flag = string.Empty;
        try
        {
            string NoticeIDs = string.Empty;
            JArray Array = JArray.Parse(Fields);

            for (var i = 0; i < Array.Count; i++)
            {
                NoticeIDs += Array[i]["NoticeID"].ToString() + ",";
            }

            if (!string.IsNullOrEmpty(NoticeIDs))
            {
                NoticeIDs = NoticeIDs.Substring(0, NoticeIDs.Length - 1);

                string _sql = "update Notice set IsRead=@IsRead where NoticeID in (" + NoticeIDs + ") and UID=@UID";
                SqlParameter[] _sp = {
                                        new SqlParameter("@IsRead",SqlDbType.Bit),
                                        new SqlParameter("@UID",SqlDbType.Int)
                                    };

                _sp[0].Value = true;
                _sp[1].Value = MicroUserInfo.GetUserInfo("UID");

                if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
                    flag = "True标记成功。<br/>マーク成功。<br/>Marked successfully.";
            }
        }
        catch (Exception ex) { flag = ex.ToString(); }

        return flag;
    }

    /// <summary>
    /// 标记选中消息为未读
    /// </summary>
    /// <param name="Fields"></param>
    /// <returns></returns>
    private string SetMsgUnread(string Fields)
    {
        string flag = string.Empty;
        try
        {
            string NoticeIDs = string.Empty;
            JArray Array = JArray.Parse(Fields);

            for (var i = 0; i < Array.Count; i++)
            {
                NoticeIDs += Array[i]["NoticeID"].ToString() + ",";
            }

            if (!string.IsNullOrEmpty(NoticeIDs))
            {
                NoticeIDs = NoticeIDs.Substring(0, NoticeIDs.Length - 1);

                string _sql = "update Notice set IsRead=@IsRead where NoticeID in (" + NoticeIDs + ") and UID=@UID";
                SqlParameter[] _sp = {
                                        new SqlParameter("@IsRead",SqlDbType.Bit),
                                        new SqlParameter("@UID",SqlDbType.Int)
                                    };

                _sp[0].Value = false;
                _sp[1].Value = MicroUserInfo.GetUserInfo("UID");

                if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
                    flag = "True标记成功。<br/>マーク成功。<br/>Marked successfully.";
            }
        }
        catch (Exception ex) { flag = ex.ToString(); }

        return flag;
    }

    /// <summary>
    /// 标记所有消息为已读
    /// </summary>
    /// <param name="Fields"></param>
    /// <returns></returns>
    private string SetMsgReadAll()
    {
        string flag = string.Empty;
        try
        {
            string _sql = "update Notice set IsRead=@IsRead where UID=@UID";
            SqlParameter[] _sp = {
                                        new SqlParameter("@IsRead",SqlDbType.Bit),
                                        new SqlParameter("@UID",SqlDbType.Int)
                                    };

            _sp[0].Value = true;
            _sp[1].Value = MicroUserInfo.GetUserInfo("UID");

            if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
                flag = "True标记成功。<br/>マーク成功。<br/>Marked successfully.";
            else
                flag = "没有可读消息。<br/>過去の通知はありません。<br/>There are no readable messages.";

        }
        catch (Exception ex) { flag = ex.ToString(); }

        return flag;
    }


    /// <summary>
    /// 删除选中消息
    /// </summary>
    /// <param name="PWD"></param>
    /// <param name="Fields"></param>
    /// <returns></returns>
    private string SetMsgDel(string Action, string PWD, string Fields)
    {
        string flag = MicroPublic.GetMsg("DelFailed");

        try
        {
            string UID = MicroUserInfo.GetUserInfo("UID");
            switch (Action)
            {
                //删除选中
                case "delselected":
                    //在检查密码是否正确后，才判断是否有权限删除
                    if (MicroAuth.VerifyPassword(PWD))
                    {
                        string NoticeIDs = string.Empty;
                        JArray Array = JArray.Parse(Fields);

                        for (var i = 0; i < Array.Count; i++)
                        {
                            NoticeIDs += Array[i]["NoticeID"].ToString() + ",";
                        }

                        if (!string.IsNullOrEmpty(NoticeIDs))
                        {

                            NoticeIDs = NoticeIDs.Substring(0, NoticeIDs.Length - 1);

                            string _sql = "update Notice set Del=1 where NoticeID in (" + NoticeIDs + ") and UID=@UID";
                            SqlParameter[] _sp = { new SqlParameter("@UID", SqlDbType.Int) };

                            _sp[0].Value = UID.toInt();

                            if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
                                flag = MicroPublic.GetMsg("Del"); //"True删除成功。<br/>Delete successfully.";
                        }
                    }
                    else
                        flag = MicroPublic.GetMsg("PWDWrong");//"密码不正确。<br/>The password is incorrect.";

                    break;

                //删除已读
                case "delready":
                    //在检查密码是否正确后，才判断是否有权限删除
                    if (MicroAuth.VerifyPassword(PWD))
                    {
                        string _sql2 = "update Notice set Del=1 where IsRead=1 and UID=@UID";
                        SqlParameter[] _sp2 = { new SqlParameter("@UID", SqlDbType.Int) };
                        _sp2[0].Value = UID.toInt();

                        if (MsSQLDbHelper.ExecuteSql(_sql2, _sp2) > 0)
                            flag = MicroPublic.GetMsg("Del");
                    }
                    else
                        flag = MicroPublic.GetMsg("PWDWrong");

                    break;

                //删除所有
                case "delall":
                    //在检查密码是否正确后，才判断是否有权限删除
                    if (MicroAuth.VerifyPassword(PWD))
                    {
                        string _sql3 = "update Notice set Del=1 where UID=@UID";
                        SqlParameter[] _sp3 = { new SqlParameter("@UID", SqlDbType.Int) };
                        _sp3[0].Value = UID.toInt();

                        if (MsSQLDbHelper.ExecuteSql(_sql3, _sp3) > 0)
                            flag = MicroPublic.GetMsg("Del");
                    }
                    else
                        flag = MicroPublic.GetMsg("PWDWrong");

                    break;
            }
        }
        catch (Exception ex) { flag = ex.ToString(); }

        return flag;
    }


    private string GetUnreadTotal()
    {
        string flag = string.Empty;

        string UID = MicroUserInfo.GetUserInfo("UID");
        string _sql = "select * from Notice where Invalid=0 and Del=0 and IsRead=0 and UID=@UID";

        SqlParameter[] _sp = { new SqlParameter("@UID", SqlDbType.Int) };
        _sp[0].Value = UID.toInt();

        DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];
        flag = _dt.Rows.Count.ToString();

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