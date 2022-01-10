<%@ WebHandler Language="C#" Class="SyncDomainUsers" %>

using System;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroPublicHelper;
using Newtonsoft.Json.Linq;
using MicroLdapHelper;
using MicroAuthHelper;

public class SyncDomainUsers : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = "测试连接失败。<br/>Test connection failed.",
        Action = context.Server.UrlDecode(context.Request.Form["action"]),
        MID = context.Server.UrlDecode(context.Request.Form["mid"]),
            Fields = context.Server.UrlDecode(context.Request.Form["fields"]);

        if (Action == "test")
            flag = GetTestConn(Fields);

        if (Action == "sync")
            flag = GetDomainUser(Fields);

        if (Action == "userdepts")
            flag = UpdateUserDepts();

        context.Response.Write(flag);
    }


    private string GetTestConn(string Fields)
    {
        string flag = "测试连接失败。错误代码：1";
        try
        {
            string UserName = string.Empty, Password = string.Empty, DomainName = string.Empty, LdapOU = string.Empty;
            dynamic json = JToken.Parse(Fields) as dynamic;

            UserName = json["txtUserName"];
            Password = json["txtPassword"];
            DomainName = json["txtDomainName"];
            LdapOU = json["txtLdapOU"];

            UserName = UserName.toStringTrim(); Password = Password.toStringTrim(); DomainName = DomainName.toStringTrim(); LdapOU = LdapOU.toStringTrim();

            if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(DomainName) && !string.IsNullOrEmpty(LdapOU))
            {
                if (LdapHelper.TestConn(DomainName, LdapOU, UserName, Password))
                    flag = "True测试连接成功。<br/>Test connection is successful.";
                else
                    flag = "测试连接失败。错误代码：3";
            }
            else
                flag = "测试连接失败。错误代码：2";

        }
        catch (Exception ex) { flag = ex.ToString(); }

        return flag;

    }

    private string GetDomainUser(string Fields)
    {
        string flag = "同步失败，请确认填写的域控信息是否正确，错误代码：1";

        try
        {
            string UserName = string.Empty, Password = string.Empty, DomainName = string.Empty, LdapOU = string.Empty;
            dynamic json = JToken.Parse(Fields) as dynamic;

            UserName = json["txtUserName"];
            Password = json["txtPassword"];
            DomainName = json["txtDomainName"];
            LdapOU = json["txtLdapOU"];

            UserName = UserName.Trim(); Password = Password.Trim(); DomainName = DomainName.Trim(); LdapOU = LdapOU.Trim();

            if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(DomainName) && !string.IsNullOrEmpty(LdapOU))
                flag = LdapHelper.SyncDomainUser(DomainName, LdapOU, UserName, Password);
            else
                flag = "同步失败，请确认填写的域控信息是否正确，错误代码：2";
        }
        catch (Exception ex) { flag = ex.ToString(); }

        return flag;

    }

    /// <summary>
    /// 更新用户所属部门，根据AD的长部门名称更新到正规的部门名称
    /// </summary>
    /// <returns></returns>
    private string UpdateUserDepts()
    {
        string flag = MicroPublic.GetMsg("UpdateFailed");

        try
        {
            string _sql = "select UID,AdDepartment,AdDescription from UserInfo where Invalid=0 and Del=0";
            DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

            string _sql2 = "select * from Department";
            DataTable _dt2 = MsSQLDbHelper.Query(_sql2).Tables[0];

            string _sql3 = "select * from UserDepts";
            DataTable _dt3 = MsSQLDbHelper.Query(_sql3).Tables[0];

            if (_dt != null && _dt.Rows.Count > 0 && _dt2 != null && _dt2.Rows.Count > 0)
            {
                DataTable _dtUserDepts = new DataTable();
                _dtUserDepts.Columns.Add("UID", typeof(int));
                _dtUserDepts.Columns.Add("DeptID", typeof(int));

                DataRow[] _rows = _dt.Select("");
                foreach (DataRow _dr in _rows)
                {
                    string AdDepartment = _dr["AdDepartment"].toStringTrim();
                    if (!string.IsNullOrEmpty(AdDepartment))
                    {
                        DataRow[] _rows2 = _dt2.Select("AdDepartment=" + "'" + AdDepartment + "'");
                        if (_rows2.Length > 0)
                        {
                            int UID = _dr["UID"].toInt();
                            int DeptID = _rows2[0]["DeptID"].toInt();

                            //确认UserDepts表记录不存在时才添加
                            DataRow[] _rows3 = _dt3.Select("UID=" + UID + " and DeptID=" + DeptID);
                            if (_rows3.Length <= 0)
                            {
                                DataRow _drInsert = _dtUserDepts.NewRow();
                                _drInsert["UID"] = UID;
                                _drInsert["DeptID"] = DeptID;
                                _dtUserDepts.Rows.Add(_drInsert);
                            }
                        }
                    }
                }

                ////先删除表的数据再插入
                //string _sql4 = "delete UserDepts";
                //MsSQLDbHelper.ExecuteSql(_sql4);

                if (MsSQLDbHelper.SqlBulkCopyInsert(_dtUserDepts, "UserDepts"))
                    flag = MicroPublic.GetMsg("Update");
            }


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