<%@ WebHandler Language="C#" Class="UserPublicInfoChange" %>

using System;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroDTHelper;
using MicroPublicHelper;
using MicroUserHelper;

public class UserPublicInfoChange : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        string flag = MicroPublic.GetMsg("DenyURLError"),
             Action = context.Server.UrlDecode(context.Request.Form["action"]),
             Type = context.Server.UrlDecode(context.Request.Form["type"]),
             MID = context.Server.UrlDecode(context.Request.Form["mid"]),
             UID = context.Server.UrlDecode(context.Request.Form["uid"]),
             Fields = context.Server.UrlDecode(context.Request.Form["fields"]),
             Date = context.Server.UrlDecode(context.Request.Form["date"]);

        //Action = string.IsNullOrEmpty(Action) == true ? "" : Action.ToLower();

        try
        {
            if (!string.IsNullOrEmpty(Type) && !string.IsNullOrEmpty(UID) && !string.IsNullOrEmpty(Fields) && UID.toInt()!=0)
            {
                Type = Type.ToLower();

                if (Type == "udepts")
                    flag = ModifyUserDepts(UID, Fields);

                if (Type == "ujtitle")
                    flag = ModifyUserJobTitle(UID, Fields);

                if (Type == "uroles")
                    flag = ModifyUserRoles(UID, Fields);

                if (Type == "uworkhoursystem")
                    flag = ModifyUserWorkHourSystem(UID, Fields, Date);

                if (Type == "annualleave")
                    flag = CtrlUserLeave("1", UID, Fields);

            }
        }
        catch (Exception ex)
        {
            flag = ex.ToString();
        }

        context.Response.Write(flag);
    }

    /// <summary>
    /// 修改用户所属部门
    /// </summary>
    /// <param name="UID"></param>
    /// <param name="Fields"></param>
    /// <returns></returns>
    private string ModifyUserDepts(string UID, string Fields)
    {

        string flag = MicroPublic.GetMsg("ModifyFailed");
        try
        {
            string[] Arr = Fields.Split(',');
            if (Arr.Length > 0)
            {
                //先删除数据库里已有而当前未选中的记录
                string _sqlDel = "delete UserDepts where UID=@UID and DeptID not in (" + Fields + ") ";
                SqlParameter[] _spDel = { new SqlParameter("@UID", SqlDbType.Int) };
                _spDel[0].Value = UID.toInt();
                MsSQLDbHelper.ExecuteSql(_sqlDel, _spDel);

                //读取数据库记录
                string _sql = "select * from UserDepts where UID=@UID ";
                SqlParameter[] _sp = { new SqlParameter("@UID", SqlDbType.Int) };
                _sp[0].Value = UID.toInt();
                DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

                //新建DataTable，用于存放已选中而数据库没有的记录
                DataTable _dt2 = new DataTable();
                _dt2.Columns.Add("UID", typeof(int));
                _dt2.Columns.Add("DeptID", typeof(int));

                //得到已选中而数据库没有的记录写入DataTable
                for (int i = 0; i < Arr.Length; i++)
                {
                    int ID = Arr[i].toInt();
                    DataRow[] _rows = _dt.Select("DeptID=" + ID);
                    if (_rows.Length == 0)
                    {
                        DataRow _dr2 = _dt2.NewRow();
                        _dr2["UID"] = UID.toInt();
                        _dr2["DeptID"] = ID;
                        _dt2.Rows.Add(_dr2);
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
    /// <param name="UID"></param>
    /// <param name="Fields"></param>
    /// <returns></returns>
    private string ModifyUserJobTitle(string UID, string Fields)
    {

        string flag = MicroPublic.GetMsg("ModifyFailed");
        try
        {
            string[] Arr = Fields.Split(',');
            if (Arr.Length > 0)
            {
                //先删除数据库里已有而当前未选中的记录
                string _sqlDel = "delete UserJobTitle where UID=@UID and JTID not in (" + Fields + ") ";
                SqlParameter[] _spDel = { new SqlParameter("@UID", SqlDbType.Int) };
                _spDel[0].Value = UID.toInt();
                MsSQLDbHelper.ExecuteSql(_sqlDel, _spDel);

                //读取数据库记录
                string _sql = "select * from UserJobTitle where UID=@UID ";
                SqlParameter[] _sp = { new SqlParameter("@UID", SqlDbType.Int) };
                _sp[0].Value = UID.toInt();
                DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

                //新建DataTable，用于存放已选中而数据库没有的记录
                DataTable _dt2 = new DataTable();
                _dt2.Columns.Add("UID", typeof(int));
                _dt2.Columns.Add("JTID", typeof(int));

                //得到已选中而数据库没有的记录写入DataTable
                for (int i = 0; i < Arr.Length; i++)
                {
                    int ID = Arr[i].toInt();
                    DataRow[] _rows = _dt.Select("JTID=" + ID);
                    if (_rows.Length == 0)
                    {
                        DataRow _dr2 = _dt2.NewRow();
                        _dr2["UID"] = UID.toInt();
                        _dr2["JTID"] = ID;
                        _dt2.Rows.Add(_dr2);
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
    /// <param name="UID"></param>
    /// <param name="Fields"></param>
    /// <returns></returns>
    private string ModifyUserRoles(string UID, string Fields)
    {

        string flag = MicroPublic.GetMsg("ModifyFailed");
        try
        {
            string[] Arr = Fields.Split(',');
            if (Arr.Length > 0)
            {
                //先删除数据库里已有而当前未选中的记录
                string _sqlDel = "delete UserRoles where UID=@UID and RID not in (" + Fields + ") ";
                SqlParameter[] _spDel = { new SqlParameter("@UID", SqlDbType.Int) };
                _spDel[0].Value = UID.toInt();
                MsSQLDbHelper.ExecuteSql(_sqlDel, _spDel);

                //读取数据库记录
                string _sql = "select * from UserRoles where UID=@UID ";
                SqlParameter[] _sp = { new SqlParameter("@UID", SqlDbType.Int) };
                _sp[0].Value = UID.toInt();
                DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

                //新建DataTable，用于存放已选中而数据库没有的记录
                DataTable _dt2 = new DataTable();
                _dt2.Columns.Add("UID", typeof(int));
                _dt2.Columns.Add("RID", typeof(int));

                //得到已选中而数据库没有的记录写入DataTable
                for (int i = 0; i < Arr.Length; i++)
                {
                    int ID = Arr[i].toInt();
                    DataRow[] _rows = _dt.Select("RID=" + ID);
                    if (_rows.Length == 0)
                    {
                        DataRow _dr2 = _dt2.NewRow();
                        _dr2["UID"] = UID.toInt();
                        _dr2["RID"] = ID;
                        _dt2.Rows.Add(_dr2);
                    }
                }

                //把记录写进数据库
                if (_dt2.Rows.Count > 0)
                    MsSQLDbHelper.SqlBulkCopyInsert(_dt2, "UserRoles");

                flag = MicroPublic.GetMsg("Modify");
            }
        }
        catch (Exception ex) { flag = ex.ToString(); }

        return flag;
    }


    /// <summary>
    /// 修改用户工时制
    /// </summary>
    /// <param name="UID"></param>
    /// <param name="Fields"></param>
    /// <param name="WorkHourSysDate"></param>
    /// <returns></returns>
    private string ModifyUserWorkHourSystem(string UID, string Fields, string WorkHourSysDate)
    {

        string flag = MicroPublic.GetMsg("ModifyFailed");
        try
        {
            string[] Arr = Fields.Split(',');
            if (Arr.Length > 0)
            {

                //读取数据库记录
                string _sql = "select * from HRUserWorkHourSystem where UID=@UID ";
                SqlParameter[] _sp = { new SqlParameter("@UID", SqlDbType.Int) };
                _sp[0].Value = UID.toInt();
                DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

                //新建DataTable，用于存放已选中而数据库没有的记录
                DataTable _dt2 = new DataTable();
                _dt2.Columns.Add("UID", typeof(int));
                _dt2.Columns.Add("WorkHourSysID", typeof(int));
                _dt2.Columns.Add("WorkHourSysDate", typeof(DateTime));

                //得到已选中而数据库没有的记录写入DataTable
                for (int i = 0; i < Arr.Length; i++)
                {
                    int ID = Arr[i].toInt();
                    DataRow[] _rows = _dt.Select("WorkHourSysID=" + ID + " and WorkHourSysDate='" + WorkHourSysDate.toDateFormat() + "'");
                    if (_rows.Length == 0)
                    {
                        DataRow _dr2 = _dt2.NewRow();
                        _dr2["UID"] = UID.toInt();
                        _dr2["WorkHourSysID"] = ID;
                        _dr2["WorkHourSysDate"] = WorkHourSysDate.toDateTime();
                        _dt2.Rows.Add(_dr2);
                    }
                    else
                    {
                        string _sql2 = "update HRUserWorkHourSystem set DateModified=@DateModified where UID=@UID and WorkHourSysID=@WorkHourSysID and  WorkHourSysDate=@WorkHourSysDate";
                        SqlParameter[] _sp2 = { new SqlParameter("@DateModified",SqlDbType.DateTime),
                            new SqlParameter("@UID",SqlDbType.Int),
                            new SqlParameter("@WorkHourSysID",SqlDbType.Int),
                            new SqlParameter("@WorkHourSysDate",SqlDbType.DateTime),
                            };

                        _sp2[0].Value = DateTime.Now;
                        _sp2[1].Value = UID.toInt();
                        _sp2[2].Value = ID;
                        _sp2[3].Value = WorkHourSysDate.toDateTime("yyyy-MM-dd");

                        MsSQLDbHelper.ExecuteSql(_sql2, _sp2);
                    }
                }

                //把记录写进数据库
                if (_dt2.Rows.Count > 0)
                    MsSQLDbHelper.SqlBulkCopyInsert(_dt2, "HRUserWorkHourSystem");

                flag = MicroPublic.GetMsg("Modify");
            }
        }
        catch (Exception ex) { flag = ex.ToString(); }

        return flag;
    }

    /// <summary>
    /// 修改用户假期（存放在HRPersonalHoliday表的假期，如年休），有记录时修改，没有记录里新增
    /// </summary>
    /// <param name="HolidayTypeID"></param>
    /// <param name="UID"></param>
    /// <param name="Fields"></param>
    /// <returns></returns>
    private string CtrlUserLeave(string HolidayTypeID, string UID, string Fields)
    {
        string flag = MicroPublic.GetMsg("ModifyFailed");

        string _sql = "select * from HRPersonalHoliday where Invalid=0 and Del=0 and HolidayTypeID=@HolidayTypeID and UID=@UID";

        SqlParameter[] _sp = { new SqlParameter("@HolidayTypeID", SqlDbType.Int),
                               new SqlParameter("@UID", SqlDbType.Int)};

        _sp[0].Value = HolidayTypeID.toInt();
        _sp[1].Value = UID.toInt();

        DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

        //如果有记录则更新
        if (_dt.Rows.Count > 0)
        {
            string _sql2 = "update HRPersonalHoliday set Days=@Days, DateModified=@DateModified where Invalid=0 and Del=0 and HolidayTypeID=@HolidayTypeID and UID=@UID";
            SqlParameter[] _sp2 = { new SqlParameter("@Days", SqlDbType.Decimal,6),
                                new SqlParameter("@DateModified", SqlDbType.DateTime),
                                new SqlParameter("@HolidayTypeID", SqlDbType.Int),
                                new SqlParameter("@UID", SqlDbType.Int)
                };

            _sp2[0].Value = Fields.toDecimal();
            _sp2[1].Value = DateTime.Now;
            _sp2[2].Value = HolidayTypeID.toInt();
            _sp2[3].Value = UID.toInt();

            if (MsSQLDbHelper.ExecuteSql(_sql2, _sp2) > 0)
                flag = MicroPublic.GetMsg("Modify");
        }
        //如果没有记录则插入
        else
        {
            string _sql3 = "insert into HRPersonalHoliday (HolidayTypeID, Days, HolidayOfDate, UID, CreateUID, CreateDisplayName) values (@HolidayTypeID, @Days, @HolidayOfDate, @UID, @CreateUID, @CreateDisplayName)";

            SqlParameter[] _sp3 = {
                    new SqlParameter("@HolidayTypeID", SqlDbType.Int),
                    new SqlParameter("@Days", SqlDbType.Decimal,6),
                    new SqlParameter("@HolidayOfDate", SqlDbType.DateTime),
                    new SqlParameter("@UID", SqlDbType.Int),
                    new SqlParameter("@CreateUID", SqlDbType.Int),
                    new SqlParameter("@CreateDisplayName", SqlDbType.NVarChar,200),

                };

            _sp3[0].Value = HolidayTypeID.toInt();
            _sp3[1].Value = Fields.toDecimal();
            _sp3[2].Value = DateTime.Now;
            _sp3[3].Value = UID.toInt();
            _sp3[4].Value = MicroUserInfo.GetUserInfo("UID").toInt();
            _sp3[5].Value = MicroUserInfo.GetUserInfo("DisplayName");

            if (MsSQLDbHelper.ExecuteSql(_sql3, _sp3) > 0)
                flag = MicroPublic.GetMsg("Modify");

        }


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