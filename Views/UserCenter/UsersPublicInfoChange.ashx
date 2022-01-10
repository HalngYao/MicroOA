<%@ WebHandler Language="C#" Class="UsersPublicInfoChange" %>

using System;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroDTHelper;
using MicroPublicHelper;
using MicroUserHelper;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using MicroAuthHelper;

public class UsersPublicInfoChange : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = MicroPublic.GetMsg("DenyUrlParaError"),
             Action = context.Server.UrlDecode(context.Request.Form["action"]),
             Type = context.Server.UrlDecode(context.Request.Form["type"]),
             MID = context.Server.UrlDecode(context.Request.Form["mid"]),
             UIDS = context.Server.UrlDecode(context.Request.Form["uids"]),
             Fields = context.Server.UrlDecode(context.Request.Form["fields"]),
             Date = context.Server.UrlDecode(context.Request.Form["date"]);


        Action = string.IsNullOrEmpty(Action) == true ? "" : Action.ToLower();

        try
        {
            if (!string.IsNullOrEmpty(Type) && !string.IsNullOrEmpty(Fields))
            {
                Type = Type.ToLower();

                if (Type == "setaujtitle")
                    flag = SetAllUserDefaultJobTitle(Fields);

                if (Type == "setaurole")
                    flag = SetAllUserDefaultRole(Fields);

                if (Type == "udepts")
                    flag = ModifyUserDepts(UIDS, Fields);

                if (Type == "ujtitle")
                    flag = ModifyUserJobTitle(UIDS, Fields);

                if (Type == "uroles")
                    flag = ModifyUserRoles(UIDS, Fields);

                if (Type == "uworkhoursystem")
                    flag = ModifyUserWorkHourSystem(UIDS, Fields, Date);
            }
        }
        catch (Exception ex)
        {
            flag = ex.ToString();
        }

        context.Response.Write(flag);
    }


    /// <summary>
    /// 设置所有用户默认职务为传递过来的职务
    /// </summary>
    /// <param name="Fields">传递过来的JTIDS</param>
    /// <returns></returns>
    private string SetAllUserDefaultJobTitle(string Fields)
    {
        return MicroUserInfo.SetAllUserDefaultJobTitle(Fields);
    }


    /// <summary>
    /// 设置所有用户默认角色为传递过来的角色
    /// </summary>
    /// <param name="Fields">传递过来的RIDS</param>
    /// <returns></returns>
    private string SetAllUserDefaultRole(string Fields)
    {
        return MicroUserInfo.SetAllUserDefaultRole(Fields);
    }


    /// <summary>
    /// 修改用户所属部门
    /// </summary>
    /// <param name="UIDS"></param>
    /// <param name="Fields"></param>
    /// <returns></returns>
    private string ModifyUserDepts(string UIDS, string Fields)
    {
        string flag = MicroPublic.GetMsg("ModifyFailed");
        try
        {
            string[] UIDSArr = UIDS.Split(',');
            string[] DeptsArr = Fields.Split(',');
            if (UIDSArr.Length>0 && DeptsArr.Length > 0)
            {
                //读取数据库记录
                string _sql = "select * from UserDepts";
                DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

                //新建DataTable，用于存放已选中而数据库没有的记录
                DataTable _dt2 = new DataTable();
                _dt2.Columns.Add("UID", typeof(int));
                _dt2.Columns.Add("DeptID", typeof(int));

                for (int j = 0; j < UIDSArr.Length; j++)
                {
                    int UID = UIDSArr[j].toInt();

                    //先删除数据库里已有而当前未选中的记录
                    string _sqlDel = "delete UserDepts where UID=@UID and DeptID not in (" + Fields + ") ";
                    SqlParameter[] _spDel = { new SqlParameter("@UID", SqlDbType.Int) };
                    _spDel[0].Value = UID.toInt();
                    MsSQLDbHelper.ExecuteSql(_sqlDel, _spDel);

                    //得到已选中而数据库没有的记录写入DataTable
                    for (int i = 0; i < DeptsArr.Length; i++)
                    {
                        int DeptID = DeptsArr[i].toInt();
                        DataRow[] _rows = _dt.Select("UID=" + UID + " and DeptID=" + DeptID);
                        if (_rows.Length == 0)
                        {
                            DataRow _dr2 = _dt2.NewRow();
                            _dr2["UID"] = UID;
                            _dr2["DeptID"] = DeptID;
                            _dt2.Rows.Add(_dr2);
                        }
                    }
                }

                //把记录写进数据库
                if (_dt2.Rows.Count > 0)
                    MsSQLDbHelper.SqlBulkCopyInsert(_dt2, "UserDepts");

                flag = MicroPublic.GetMsg("Modify");
            }
        }
        catch (Exception ex) { flag = ex.ToString(); }

        return flag;
    }

    /// <summary>
    /// 修改用户职位职称
    /// </summary>
    /// <param name="UIDS"></param>
    /// <param name="Fields"></param>
    /// <returns></returns>
    private string ModifyUserJobTitle(string UIDS, string Fields)
    {
        string flag = MicroPublic.GetMsg("ModifyFailed");
        try
        {
            string[] UIDSArr = UIDS.Split(',');
            string[] UJTitleArr = Fields.Split(',');
            if (UIDSArr.Length>0 && UJTitleArr.Length > 0)
            {
                //读取数据库记录
                string _sql = "select * from UserJobTitle ";
                DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

                //新建DataTable，用于存放已选中而数据库没有的记录
                DataTable _dt2 = new DataTable();
                _dt2.Columns.Add("UID", typeof(int));
                _dt2.Columns.Add("JTID", typeof(int));

                for (int j = 0; j < UIDSArr.Length; j++)
                {
                    int UID = UIDSArr[j].toInt();
                    //先删除数据库里已有而当前未选中的记录
                    string _sqlDel = "delete UserJobTitle where UID=@UID and JTID not in (" + Fields + ") ";
                    SqlParameter[] _spDel = { new SqlParameter("@UID", SqlDbType.Int) };
                    _spDel[0].Value = UID.toInt();
                    MsSQLDbHelper.ExecuteSql(_sqlDel, _spDel);

                    //得到已选中而数据库没有的记录写入DataTable
                    for (int i = 0; i < UJTitleArr.Length; i++)
                    {
                        int JTID = UJTitleArr[i].toInt();
                        DataRow[] _rows = _dt.Select("UID=" + UID + " and JTID=" + JTID);
                        if (_rows.Length == 0)
                        {
                            DataRow _dr2 = _dt2.NewRow();
                            _dr2["UID"] = UID;
                            _dr2["JTID"] = JTID;
                            _dt2.Rows.Add(_dr2);
                        }
                    }
                }

                //把记录写进数据库
                if (_dt2.Rows.Count > 0)
                    MsSQLDbHelper.SqlBulkCopyInsert(_dt2, "UserJobTitle");

                flag = MicroPublic.GetMsg("Modify");
            }
        }
        catch (Exception ex) { flag = ex.ToString(); }

        return flag;
    }

    /// <summary>
    /// 修改用户角色
    /// </summary>
    /// <param name="UIDS"></param>
    /// <param name="Fields"></param>
    /// <returns></returns>
    private string ModifyUserRoles(string UIDS, string Fields)
    {

        string flag = MicroPublic.GetMsg("ModifyFailed");
        try
        {
            string[] UIDSArr = UIDS.Split(',');
            string[] URolesArr = Fields.Split(',');
            if (UIDSArr.Length>0 && URolesArr.Length > 0)
            {

                //读取数据库记录
                string _sql = "select * from UserRoles";
                DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

                //新建DataTable，用于存放已选中而数据库没有的记录
                DataTable _dt2 = new DataTable();
                _dt2.Columns.Add("UID", typeof(int));
                _dt2.Columns.Add("RID", typeof(int));

                for (int j = 0; j < UIDSArr.Length; j++)
                {
                    int UID = UIDSArr[j].toInt();

                    //先删除数据库里已有而当前未选中的记录
                    string _sqlDel = "delete UserRoles where UID=@UID and RID not in (" + Fields + ") ";
                    SqlParameter[] _spDel = { new SqlParameter("@UID", SqlDbType.Int) };
                    _spDel[0].Value = UID;
                    MsSQLDbHelper.ExecuteSql(_sqlDel, _spDel);

                    //得到已选中而数据库没有的记录写入DataTable
                    for (int i = 0; i < URolesArr.Length; i++)
                    {
                        int RID = URolesArr[i].toInt();
                        DataRow[] _rows = _dt.Select("UID=" + UID + " and RID=" + RID);
                        if (_rows.Length == 0)
                        {
                            DataRow _dr2 = _dt2.NewRow();
                            _dr2["UID"] = UID.toInt();
                            _dr2["RID"] = RID;
                            _dt2.Rows.Add(_dr2);
                        }
                    }

                }

                //把记录写进数据库
                if (_dt2.Rows.Count > 0)
                    MsSQLDbHelper.SqlBulkCopyInsert(_dt2, "UserRoles");

                flag = MicroPublic.GetMsg("Modify");
            }
        }
        catch ( Exception ex){ flag = ex.ToString(); }

        return flag;
    }


    /// <summary>
    /// 修改用户工时制
    /// </summary>
    /// <param name="UIDS"></param>
    /// <param name="Fields"></param>
    /// <param name="WorkHourSysDate"></param>
    /// <returns></returns>
    private string ModifyUserWorkHourSystem(string UIDS, string Fields, string WorkHourSysDate)
    {

        string flag = MicroPublic.GetMsg("ModifyFailed");
        try
        {
            string[] UIDSArr = UIDS.Split(',');
            string[] UWorkHourSystemArr = Fields.Split(',');
            if (UIDSArr.Length>0 && UWorkHourSystemArr.Length > 0)
            {
                //读取数据库记录
                string _sql = "select * from HRUserWorkHourSystem";
                DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

                //新建DataTable，用于存放已选中而数据库没有的记录
                DataTable _dt2 = new DataTable();
                _dt2.Columns.Add("UID", typeof(int));
                _dt2.Columns.Add("WorkHourSysID", typeof(int));
                _dt2.Columns.Add("WorkHourSysDate", typeof(DateTime));

                for (int j = 0; j < UIDSArr.Length; j++)
                {
                    int UID = UIDSArr[j].toInt();
                    for (int i = 0; i < UWorkHourSystemArr.Length; i++)
                    {
                        int WorkHourSysID = UWorkHourSystemArr[i].toInt();
                        DataRow[] _rows = _dt.Select("UID=" + UID + " and WorkHourSysID=" + WorkHourSysID + " and WorkHourSysDate='" + WorkHourSysDate.toDateFormat() + "'");
                        if (_rows.Length == 0)
                        {
                            DataRow _dr2 = _dt2.NewRow();
                            _dr2["UID"] = UID.toInt();
                            _dr2["WorkHourSysID"] = WorkHourSysID;
                            _dr2["WorkHourSysDate"] = WorkHourSysDate.toDateTime();
                            _dt2.Rows.Add(_dr2);
                        }
                    }

                }

                //把记录写进数据库
                if (_dt2.Rows.Count > 0)
                    MsSQLDbHelper.SqlBulkCopyInsert(_dt2, "HRUserWorkHourSystem");

                flag = MicroPublic.GetMsg("Modify");
            }
        }
        catch ( Exception ex){ flag = ex.ToString(); }

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