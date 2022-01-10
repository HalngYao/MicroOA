<%@ WebHandler Language="C#" Class="UsersList" %>

using System;
using System.Web;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroPublicHelper;
using MicroDTHelper;
using MicroAuthHelper;

public class UsersList : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    //****************
    //说明
    //根据表名返回表的内容
    //如父项和子项，主要菜单和子菜单等
    //****************

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = MicroPublic.GetMsg("SubmitFailed"),
                    Action = context.Server.UrlDecode(context.Request.QueryString["action"]),
                    MID = context.Server.UrlDecode(context.Request.QueryString["mid"]),
                    ShortTableName = context.Server.UrlDecode(context.Request.QueryString["stn"]),
                    Type = context.Server.UrlDecode(context.Request.QueryString["type"]),
                    TypeID = context.Server.UrlDecode(context.Request.QueryString["typeid"]),
                    Keyword = context.Server.UrlDecode(context.Request.QueryString["Keyword"]),
                    Page = context.Server.UrlDecode(context.Request.QueryString["page"]),
                    Limit = context.Server.UrlDecode(context.Request.QueryString["limit"]);

        //测试数据
        //ShortTableName = "use";
        //Type = "all";
        //TypeID = "1";

        Page = string.IsNullOrEmpty(Page) == true ? "1" : Page;
        Limit = string.IsNullOrEmpty(Limit) == true ? "10" : Limit;

        if (!string.IsNullOrEmpty(ShortTableName) && !string.IsNullOrEmpty(Type))
        {
            Type = Type.ToLower();

            if (Type == "all")
                flag = GetAllUsersList(ShortTableName, Page.toInt(), Limit.toInt());

            if (Type == "dept")
                flag = GetDeptUsersList(ShortTableName, TypeID.toInt(), Page.toInt(), Limit.toInt());

            if (Type == "jtitle")
                flag = GetJobTitleUsersList(ShortTableName, TypeID.toInt(), Page.toInt(), Limit.toInt());

            if (Type == "role")
                flag = GetRolesUsersList(ShortTableName, TypeID.toInt(), Page.toInt(), Limit.toInt());

            if (Type == "search")
                flag = GetSearchUsersList(ShortTableName, Keyword, Page.toInt(), Limit.toInt());

        }

        context.Response.Write(flag);
    }

    private string GetAllUsersList(string ShortTableName, int IntPage = 1, int IntLimit = 10)
    {
        string flag = string.Empty, strTpl = string.Empty;
        int UsersSourceCount = 0;
        try
        {
            //获取长表名
            string TableName = MicroPublic.GetTableName(ShortTableName);

            if (!string.IsNullOrEmpty(TableName))
            {
                //获取表的字段和相关属性
                var getTableAttr = MicroDataTable.GetTableAttr(TableName);
                DataTable MicroDT = getTableAttr.MicroDT; //获取存放在MicroTables的数据，该表是存放每个表的表名和字段
                string PrimaryKeyName = getTableAttr.PrimaryKeyName;  //获得主键字段名称
                string SortFieldName = getTableAttr.SortFieldName;  //获得排序字段名称
                string OrderBy = getTableAttr.OrderBy;  //获得order by 命令字符串
                Boolean MainSub = getTableAttr.MainSub;  //用于判断是否开启父子项
                Boolean IsPage = getTableAttr.IsPage; //是否开启分页

                //GetUsers
                string _sql = "select * from UserInfo where Invalid=0 and Del=0 " + OrderBy + "";
                DataTable UsersSourceDT = MsSQLDbHelper.Query(_sql).Tables[0];

                //GetDepartment
                string _sql2 = "select a.UID,a.DeptID,b.Sort,b.ParentID,b.DeptName from UserDepts a left join Department b on a.DeptID=b.DeptID order by b.Sort";
                DataTable UserDeptsDT = MsSQLDbHelper.Query(_sql2).Tables[0];

                //GetJobTitle
                string _sql3 = "select a.UID,a.JTID,b.Sort,b.JobTitleName from UserJobTitle a left join JobTitle b on a.JTID=b.JTID order by b.Sort";
                DataTable UserJobTitleDT = MsSQLDbHelper.Query(_sql3).Tables[0];

                //GetRoles
                string _sql4 = "select a.UID,a.RID,b.Sort,b.RoleName from UserRoles a left join Roles b on a.RID=b.RID order by b.Sort";
                DataTable UserRolesDT = MsSQLDbHelper.Query(_sql4).Tables[0];

                //GetWorkHourSystem
                string _sql5 = "select a.UID,a.WorkHourSysID,a.WorkHourSysDate,a.DateModified,b.Sort,b.WorkHourSysName from HRUserWorkHourSystem a left join HRWorkHourSystem b on a.WorkHourSysID=b.WorkHourSysID order by b.Sort";
                DataTable UserWorkHourSystemDT = MsSQLDbHelper.Query(_sql5).Tables[0];

                UsersSourceCount = UsersSourceDT.Rows.Count;

                if (UsersSourceDT != null && UsersSourceDT.Rows.Count > 0)
                {
                    //对源数据进行分页
                    DataTable UsersTargetDT = UsersSourceDT.AsEnumerable().Skip((IntPage - 1) * IntLimit).Take(IntLimit).CopyToDataTable<DataRow>();
                    DataRow[] UsersRows = UsersTargetDT.Select();

                    foreach (DataRow UserDR in UsersRows)
                    {
                        strTpl += "{";

                        DataRow[] MicroRows = MicroDT.Select("ParentID<>0", "Sort");
                        foreach (DataRow MicroDR in MicroRows) //与类同 //for (int i = 0; i < _dt.Rows.Count; i++)
                        {

                            string TabColName = MicroDR["TabColName"].toJsonTrim(),
                                   ctlTypes = MicroDR["ctlTypes"].toJsonTrim(),
                                   ctlPlaceholder = MicroDR["ctlPlaceholder"].toJsonTrim(),
                                   strValue = UserDR["" + TabColName + ""].toJsonTrim();

                            switch (ctlTypes)
                            {
                                case "Date":
                                    strValue = strValue.toDateFormat(ctlPlaceholder);
                                    break;
                                case "Time":
                                    strValue = strValue.toDateFormat(!string.IsNullOrEmpty(ctlPlaceholder) ? ctlPlaceholder : "HH:mm:ss");
                                    break;
                                case "DateTime":
                                    strValue = strValue.toDateFormat(!string.IsNullOrEmpty(ctlPlaceholder) ? ctlPlaceholder : "yyyy-MM-dd HH:mm:ss");
                                    break;
                                case "DateWeek":
                                    strValue = strValue.toDateWeek();
                                    break;
                                case "DateWeekTime":
                                    strValue = strValue.toDateWeekTime("ww", ctlPlaceholder);
                                    break;
                            }


                            //获取用户所属部门，当TabColName=ColUserDepts时，ColUserDepts是用户所属部门，该字段不填写内容，主要用于生成LayuiDataTable时的表头参数
                            if (TabColName == "ColUserDepts")
                            {
                                DataRow[] UserDeptsRows = UserDeptsDT.Select("UID=" + UserDR["UID"].toInt(), "ParentID,Sort");
                                if (UserDeptsRows.Length > 0)
                                {
                                    string UserDepts = string.Empty;
                                    foreach (DataRow UserDeptsDR in UserDeptsRows)
                                    {
                                        UserDepts += UserDeptsDR["DeptName"].toJsonTrim() + " / ";
                                    }
                                    UserDepts = UserDepts.Substring(0, UserDepts.Length - 3);

                                    strTpl += "\"" + TabColName + "\":\"" + UserDepts + "\",";
                                }

                            }
                            //获取用户所属职位，当TabColName=ColUserJobTitle时，ColUserJobTitle是用户所属职位，该字段不填写内容，主要用于生成LayuiDataTable时的表头参数
                            else if (TabColName == "ColUserJobTitle")
                            {
                                DataRow[] UserJobTitleRows = UserJobTitleDT.Select("UID=" + UserDR["UID"].toInt(), "Sort");
                                if (UserJobTitleRows.Length > 0)
                                {
                                    string UserJobTitle = string.Empty;
                                    foreach (DataRow UserJobTitleDR in UserJobTitleRows)
                                    {
                                        UserJobTitle += UserJobTitleDR["JobTitleName"].toJsonTrim() + " / ";
                                    }
                                    UserJobTitle = UserJobTitle.Substring(0, UserJobTitle.Length - 3);

                                    strTpl += "\"" + TabColName + "\":\"" + UserJobTitle + "\",";
                                }
                            }
                            //获取用户所属角色，当TabColName=ColUserRoles时，ColUserRoles是用户所属角色，该字段不填写内容，主要用于生成LayuiDataTable时的表头参数
                            else if (TabColName == "ColUserRoles")
                            {
                                DataRow[] UserRolesRows = UserRolesDT.Select("UID=" + UserDR["UID"].toInt(), "Sort");
                                if (UserRolesRows.Length > 0)
                                {
                                    string UserRoles = string.Empty;
                                    foreach (DataRow UserRolesDR in UserRolesRows)
                                    {
                                        UserRoles += UserRolesDR["RoleName"].toJsonTrim() + " / ";
                                    }
                                    UserRoles = UserRoles.Substring(0, UserRoles.Length - 3);

                                    strTpl += "\"" + TabColName + "\":\"" + UserRoles + "\",";
                                }
                            }
                            //获取用户工时制，当TabColName=ColUserWorkHourSystem，ColUserWorkHourSystem是用户所属工时制，该字段不填写内容，主要用于生成LayuiDataTable时的表头参数
                            else if (TabColName == "ColUserWorkHourSystem")
                            {
                                DataRow[] UserWorkHourSystemRows = UserWorkHourSystemDT.Select("UID=" + UserDR["UID"].toInt(), "WorkHourSysDate desc, DateModified desc");
                                if (UserWorkHourSystemRows.Length > 0)
                                {
                                    string WorkHourSysName = string.Empty;
                                    int i = 0;
                                    foreach (DataRow UserWorkHourSystemDR in UserWorkHourSystemRows)
                                    {
                                        if (i == 0)
                                            WorkHourSysName += UserWorkHourSystemDR["WorkHourSysName"].toJsonTrim() + " / ";

                                        i = i + 1;
                                    }
                                    WorkHourSysName = WorkHourSysName.Substring(0, WorkHourSysName.Length - 3);

                                    strTpl += "\"" + TabColName + "\":\"" + WorkHourSysName + "\",";
                                }
                            }
                            else
                                strTpl += "\"" + TabColName + "\":\"" + strValue + "\",";
                        }

                        strTpl = strTpl.Substring(0, strTpl.Length - 1);
                        strTpl += "},";
                    }

                    strTpl = strTpl.Substring(0, strTpl.Length - 1);

                }

            }
            flag = "{\"code\": 0, \"msg\": \"\", \"count\": " + UsersSourceCount + ", \"data\":  [" + strTpl + "] }";

        }
        catch (Exception ex) { flag = ex.ToString(); }


        return flag;
    }


    private string GetDeptUsersList(string ShortTableName, int DeptID, int IntPage = 1, int IntLimit = 10)
    {
        string flag = string.Empty, strTpl = string.Empty;
        int UsersSourceCount = 0;
        try
        {
            //获取长表名
            string TableName = MicroPublic.GetTableName(ShortTableName);

            if (!string.IsNullOrEmpty(TableName))
            {
                //获取表的字段和相关属性
                var getTableAttr = MicroDataTable.GetTableAttr(TableName);
                DataTable MicroDT = getTableAttr.MicroDT; //获取存放在MicroTables的数据，该表是存放每个表的表名和字段
                string PrimaryKeyName = getTableAttr.PrimaryKeyName;  //获得主键字段名称
                string SortFieldName = getTableAttr.SortFieldName;  //获得排序字段名称
                string OrderBy = getTableAttr.OrderBy;  //获得order by 命令字符串
                Boolean MainSub = getTableAttr.MainSub;  //用于判断是否开启父子项
                Boolean IsPage = getTableAttr.IsPage; //是否开启分页

                //GetUsers
                string _sql = "select * from UserInfo where Invalid=0 and Del=0 and UID in (select UID from UserDepts where DeptID in( select DeptID from Department where LevelCode like (select LevelCode from Department where DeptID=@DeptID)+'%') group by UID)  " + OrderBy + "";
                SqlParameter[] _sp = { new SqlParameter("@DeptID", SqlDbType.Int) };
                _sp[0].Value = DeptID;
                DataTable UsersSourceDT = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

                //GetDepartment
                string _sql2 = "select a.UID,a.DeptID,b.Sort,b.ParentID,b.DeptName from UserDepts a left join Department b on a.DeptID=b.DeptID order by b.Sort";
                DataTable UserDeptsDT = MsSQLDbHelper.Query(_sql2).Tables[0];

                //GetJobTitle
                string _sql3 = "select a.UID,a.JTID,b.Sort,b.JobTitleName from UserJobTitle a left join JobTitle b on a.JTID=b.JTID order by b.Sort";
                DataTable UserJobTitleDT = MsSQLDbHelper.Query(_sql3).Tables[0];

                //GetRoles
                string _sql4 = "select a.UID,a.RID,b.Sort,b.RoleName from UserRoles a left join Roles b on a.RID=b.RID order by b.Sort";
                DataTable UserRolesDT = MsSQLDbHelper.Query(_sql4).Tables[0];

                //GetWorkHourSystem
                string _sql5 = "select a.UID,a.WorkHourSysID,a.WorkHourSysDate,a.DateModified,b.Sort,b.WorkHourSysName from HRUserWorkHourSystem a left join HRWorkHourSystem b on a.WorkHourSysID=b.WorkHourSysID order by b.Sort";
                DataTable UserWorkHourSystemDT = MsSQLDbHelper.Query(_sql5).Tables[0];

                UsersSourceCount = UsersSourceDT.Rows.Count;

                if (UsersSourceDT != null && UsersSourceDT.Rows.Count > 0)
                {
                    //对源数据进行分页
                    DataTable UsersTargetDT = UsersSourceDT.AsEnumerable().Skip((IntPage - 1) * IntLimit).Take(IntLimit).CopyToDataTable<DataRow>();
                    DataRow[] UsersRows = UsersTargetDT.Select();

                    foreach (DataRow UserDR in UsersRows)
                    {
                        strTpl += "{";

                        DataRow[] MicroRows = MicroDT.Select("ParentID<>0", "Sort");
                        foreach (DataRow MicroDR in MicroRows) //与类同 //for (int i = 0; i < _dt.Rows.Count; i++)
                        {

                            string TabColName = MicroDR["TabColName"].toJsonTrim(),
                                ctlTypes = MicroDR["ctlTypes"].toJsonTrim(),
                                   ctlPlaceholder = MicroDR["ctlPlaceholder"].toJsonTrim(),
                                   strValue = UserDR["" + TabColName + ""].toJsonTrim();

                            switch (ctlTypes)
                            {
                                case "Date":
                                    strValue = strValue.toDateFormat(ctlPlaceholder);
                                    break;
                                case "Time":
                                    strValue = strValue.toDateFormat(!string.IsNullOrEmpty(ctlPlaceholder) ? ctlPlaceholder : "HH:mm:ss");
                                    break;
                                case "DateTime":
                                    strValue = strValue.toDateFormat(!string.IsNullOrEmpty(ctlPlaceholder) ? ctlPlaceholder : "yyyy-MM-dd HH:mm:ss");
                                    break;
                                case "DateWeek":
                                    strValue = strValue.toDateWeek();
                                    break;
                                case "DateWeekTime":
                                    strValue = strValue.toDateWeekTime("ww", ctlPlaceholder);
                                    break;
                            }

                            //获取用户所属部门，当TabColName=ColUserDepts时，ColUserDepts是用户所属部门，该字段不填写内容，主要用于生成LayuiDataTable时的表头参数
                            if (TabColName == "ColUserDepts")
                            {
                                DataRow[] UserDeptsRows = UserDeptsDT.Select("UID=" + UserDR["UID"].toInt(), "ParentID,Sort");
                                if (UserDeptsRows.Length > 0)
                                {
                                    string UserDepts = string.Empty;
                                    foreach (DataRow UserDeptsDR in UserDeptsRows)
                                    {
                                        UserDepts += UserDeptsDR["DeptName"].toJsonTrim() + " / ";
                                    }
                                    UserDepts = UserDepts.Substring(0, UserDepts.Length - 3);

                                    strTpl += "\"" + TabColName + "\":\"" + UserDepts + "\",";
                                }

                            }
                            //获取用户所属职位，当TabColName=ColUserJobTitle时，ColUserJobTitle是用户所属职位，该字段不填写内容，主要用于生成LayuiDataTable时的表头参数
                            else if (TabColName == "ColUserJobTitle")
                            {
                                DataRow[] UserJobTitleRows = UserJobTitleDT.Select("UID=" + UserDR["UID"].toInt(), "Sort");
                                if (UserJobTitleRows.Length > 0)
                                {
                                    string UserJobTitle = string.Empty;
                                    foreach (DataRow UserJobTitleDR in UserJobTitleRows)
                                    {
                                        UserJobTitle += UserJobTitleDR["JobTitleName"].toJsonTrim() + " / ";
                                    }
                                    UserJobTitle = UserJobTitle.Substring(0, UserJobTitle.Length - 3);

                                    strTpl += "\"" + TabColName + "\":\"" + UserJobTitle + "\",";
                                }
                            }
                            //获取用户所属角色，当TabColName=ColUserRoles时，ColUserRoles是用户所属角色，该字段不填写内容，主要用于生成LayuiDataTable时的表头参数
                            else if (TabColName == "ColUserRoles")
                            {
                                DataRow[] UserRolesRows = UserRolesDT.Select("UID=" + UserDR["UID"].toInt(), "Sort");
                                if (UserRolesRows.Length > 0)
                                {
                                    string UserRoles = string.Empty;
                                    foreach (DataRow UserRolesDR in UserRolesRows)
                                    {
                                        UserRoles += UserRolesDR["RoleName"].toJsonTrim() + " / ";
                                    }
                                    UserRoles = UserRoles.Substring(0, UserRoles.Length - 3);

                                    strTpl += "\"" + TabColName + "\":\"" + UserRoles + "\",";
                                }
                            }
                            //获取用户工时制，当TabColName=ColUserWorkHourSystem，ColUserWorkHourSystem是用户所属工时制，该字段不填写内容，主要用于生成LayuiDataTable时的表头参数
                            else if (TabColName == "ColUserWorkHourSystem")
                            {
                                DataRow[] UserWorkHourSystemRows = UserWorkHourSystemDT.Select("UID=" + UserDR["UID"].toInt(), "WorkHourSysDate desc, DateModified desc");
                                if (UserWorkHourSystemRows.Length > 0)
                                {
                                    string WorkHourSysName = string.Empty;
                                    int i = 0;
                                    foreach (DataRow UserWorkHourSystemDR in UserWorkHourSystemRows)
                                    {
                                        if (i == 0)
                                            WorkHourSysName += UserWorkHourSystemDR["WorkHourSysName"].toJsonTrim() + " / ";

                                        i = i + 1;
                                    }
                                    WorkHourSysName = WorkHourSysName.Substring(0, WorkHourSysName.Length - 3);

                                    strTpl += "\"" + TabColName + "\":\"" + WorkHourSysName + "\",";
                                }
                            }
                            else
                                strTpl += "\"" + TabColName + "\":\"" + strValue + "\",";
                        }

                        strTpl = strTpl.Substring(0, strTpl.Length - 1);
                        strTpl += "},";
                    }

                    strTpl = strTpl.Substring(0, strTpl.Length - 1);
                }
                flag = "{\"code\": 0, \"msg\": \"\", \"count\": " + UsersSourceCount + ", \"data\":  [" + strTpl + "] }";
            }
        }
        catch (Exception ex) { flag = ex.ToString(); }


        return flag;

    }


    private string GetJobTitleUsersList(string ShortTableName, int JTID, int IntPage = 1, int IntLimit = 10)
    {
        string flag = string.Empty, strTpl = string.Empty;
        int UsersSourceCount = 0;
        try
        {
            //获取长表名
            string TableName = MicroPublic.GetTableName(ShortTableName);

            if (!string.IsNullOrEmpty(TableName))
            {
                //获取表的字段和相关属性
                var getTableAttr = MicroDataTable.GetTableAttr(TableName);
                DataTable MicroDT = getTableAttr.MicroDT; //获取存放在MicroTables的数据，该表是存放每个表的表名和字段
                string PrimaryKeyName = getTableAttr.PrimaryKeyName;  //获得主键字段名称
                string SortFieldName = getTableAttr.SortFieldName;  //获得排序字段名称
                string OrderBy = getTableAttr.OrderBy;  //获得order by 命令字符串
                Boolean MainSub = getTableAttr.MainSub;  //用于判断是否开启父子项
                Boolean IsPage = getTableAttr.IsPage; //是否开启分页

                //GetUsers
                string _sql = "select * from UserInfo where Invalid=0 and Del=0 and UID in (select UID from UserJobTitle where JTID in( select JTID from JobTitle where JTID=@JTID))  " + OrderBy + "";
                SqlParameter[] _sp = { new SqlParameter("@JTID", SqlDbType.Int) };
                _sp[0].Value = JTID;
                DataTable UsersSourceDT = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

                //GetDepartment
                string _sql2 = "select a.UID,a.DeptID,b.Sort,b.ParentID,b.DeptName from UserDepts a left join Department b on a.DeptID=b.DeptID order by b.Sort";
                DataTable UserDeptsDT = MsSQLDbHelper.Query(_sql2).Tables[0];

                //GetJobTitle
                string _sql3 = "select a.UID,a.JTID,b.Sort,b.JobTitleName from UserJobTitle a left join JobTitle b on a.JTID=b.JTID order by b.Sort";
                DataTable UserJobTitleDT = MsSQLDbHelper.Query(_sql3).Tables[0];

                //GetRoles
                string _sql4 = "select a.UID,a.RID,b.Sort,b.RoleName from UserRoles a left join Roles b on a.RID=b.RID order by b.Sort";
                DataTable UserRolesDT = MsSQLDbHelper.Query(_sql4).Tables[0];

                //GetWorkHourSystem
                string _sql5 = "select a.UID,a.WorkHourSysID,a.WorkHourSysDate,a.DateModified,b.Sort,b.WorkHourSysName from HRUserWorkHourSystem a left join HRWorkHourSystem b on a.WorkHourSysID=b.WorkHourSysID order by b.Sort";
                DataTable UserWorkHourSystemDT = MsSQLDbHelper.Query(_sql5).Tables[0];

                UsersSourceCount = UsersSourceDT.Rows.Count;

                if (UsersSourceDT != null && UsersSourceDT.Rows.Count > 0)
                {
                    //对源数据进行分页
                    DataTable UsersTargetDT = UsersSourceDT.AsEnumerable().Skip((IntPage - 1) * IntLimit).Take(IntLimit).CopyToDataTable<DataRow>();
                    DataRow[] UsersRows = UsersTargetDT.Select();

                    foreach (DataRow UserDR in UsersRows)
                    {
                        strTpl += "{";

                        DataRow[] MicroRows = MicroDT.Select("ParentID<>0", "Sort");
                        foreach (DataRow MicroDR in MicroRows) //与类同 //for (int i = 0; i < _dt.Rows.Count; i++)
                        {

                            string TabColName = MicroDR["TabColName"].toJsonTrim(),
                                    ctlTypes = MicroDR["ctlTypes"].toJsonTrim(),
                                   ctlPlaceholder = MicroDR["ctlPlaceholder"].toJsonTrim(),
                                   strValue = UserDR["" + TabColName + ""].toJsonTrim();

                            switch (ctlTypes)
                            {
                                case "Date":
                                    strValue = strValue.toDateFormat(ctlPlaceholder);
                                    break;
                                case "Time":
                                    strValue = strValue.toDateFormat(!string.IsNullOrEmpty(ctlPlaceholder) ? ctlPlaceholder : "HH:mm:ss");
                                    break;
                                case "DateTime":
                                    strValue = strValue.toDateFormat(!string.IsNullOrEmpty(ctlPlaceholder) ? ctlPlaceholder : "yyyy-MM-dd HH:mm:ss");
                                    break;
                                case "DateWeek":
                                    strValue = strValue.toDateWeek();
                                    break;
                                case "DateWeekTime":
                                    strValue = strValue.toDateWeekTime("ww", ctlPlaceholder);
                                    break;
                            }

                            //获取用户所属部门，当TabColName=ColUserDepts时，ColUserDepts是用户所属部门，该字段不填写内容，主要用于生成LayuiDataTable时的表头参数
                            if (TabColName == "ColUserDepts")
                            {
                                DataRow[] UserDeptsRows = UserDeptsDT.Select("UID=" + UserDR["UID"].toInt(), "ParentID,Sort");
                                if (UserDeptsRows.Length > 0)
                                {
                                    string UserDepts = string.Empty;
                                    foreach (DataRow UserDeptsDR in UserDeptsRows)
                                    {
                                        UserDepts += UserDeptsDR["DeptName"].toJsonTrim() + " / ";
                                    }
                                    UserDepts = UserDepts.Substring(0, UserDepts.Length - 3);

                                    strTpl += "\"" + TabColName + "\":\"" + UserDepts + "\",";
                                }

                            }
                            //获取用户所属职位，当TabColName=ColUserJobTitle时，ColUserJobTitle是用户所属职位，该字段不填写内容，主要用于生成LayuiDataTable时的表头参数
                            else if (TabColName == "ColUserJobTitle")
                            {
                                DataRow[] UserJobTitleRows = UserJobTitleDT.Select("UID=" + UserDR["UID"].toInt(), "Sort");
                                if (UserJobTitleRows.Length > 0)
                                {
                                    string UserJobTitle = string.Empty;
                                    foreach (DataRow UserJobTitleDR in UserJobTitleRows)
                                    {
                                        UserJobTitle += UserJobTitleDR["JobTitleName"].toJsonTrim() + " / ";
                                    }
                                    UserJobTitle = UserJobTitle.Substring(0, UserJobTitle.Length - 3);

                                    strTpl += "\"" + TabColName + "\":\"" + UserJobTitle + "\",";
                                }
                            }
                            //获取用户所属角色，当TabColName=ColUserRoles时，ColUserRoles是用户所属角色，该字段不填写内容，主要用于生成LayuiDataTable时的表头参数
                            else if (TabColName == "ColUserRoles")
                            {
                                DataRow[] UserRolesRows = UserRolesDT.Select("UID=" + UserDR["UID"].toInt(), "Sort");
                                if (UserRolesRows.Length > 0)
                                {
                                    string UserRoles = string.Empty;
                                    foreach (DataRow UserRolesDR in UserRolesRows)
                                    {
                                        UserRoles += UserRolesDR["RoleName"].toJsonTrim() + " / ";
                                    }
                                    UserRoles = UserRoles.Substring(0, UserRoles.Length - 3);

                                    strTpl += "\"" + TabColName + "\":\"" + UserRoles + "\",";
                                }
                            }
                            //获取用户工时制，当TabColName=ColUserWorkHourSystem，ColUserWorkHourSystem是用户所属工时制，该字段不填写内容，主要用于生成LayuiDataTable时的表头参数
                            else if (TabColName == "ColUserWorkHourSystem")
                            {
                                DataRow[] UserWorkHourSystemRows = UserWorkHourSystemDT.Select("UID=" + UserDR["UID"].toInt(), "WorkHourSysDate desc, DateModified desc");
                                if (UserWorkHourSystemRows.Length > 0)
                                {
                                    string WorkHourSysName = string.Empty;
                                    int i = 0;
                                    foreach (DataRow UserWorkHourSystemDR in UserWorkHourSystemRows)
                                    {
                                        if (i == 0)
                                            WorkHourSysName += UserWorkHourSystemDR["WorkHourSysName"].toJsonTrim() + " / ";

                                        i = i + 1;
                                    }
                                    WorkHourSysName = WorkHourSysName.Substring(0, WorkHourSysName.Length - 3);

                                    strTpl += "\"" + TabColName + "\":\"" + WorkHourSysName + "\",";
                                }
                            }
                            else
                                strTpl += "\"" + TabColName + "\":\"" + strValue + "\",";
                        }

                        strTpl = strTpl.Substring(0, strTpl.Length - 1);
                        strTpl += "},";
                    }

                    strTpl = strTpl.Substring(0, strTpl.Length - 1);
                }
                flag = "{\"code\": 0, \"msg\": \"\", \"count\": " + UsersSourceCount + ", \"data\":  [" + strTpl + "] }";
            }
        }
        catch (Exception ex) { flag = ex.ToString(); }


        return flag;

    }


    private string GetRolesUsersList(string ShortTableName, int RID, int IntPage = 1, int IntLimit = 10)
    {
        string flag = string.Empty, strTpl = string.Empty;
        int UsersSourceCount = 0;
        try
        {
            //获取长表名
            string TableName = MicroPublic.GetTableName(ShortTableName);

            if (!string.IsNullOrEmpty(TableName))
            {
                //获取表的字段和相关属性
                var getTableAttr = MicroDataTable.GetTableAttr(TableName);
                DataTable MicroDT = getTableAttr.MicroDT; //获取存放在MicroTables的数据，该表是存放每个表的表名和字段
                string PrimaryKeyName = getTableAttr.PrimaryKeyName;  //获得主键字段名称
                string SortFieldName = getTableAttr.SortFieldName;  //获得排序字段名称
                string OrderBy = getTableAttr.OrderBy;  //获得order by 命令字符串
                Boolean MainSub = getTableAttr.MainSub;  //用于判断是否开启父子项
                Boolean IsPage = getTableAttr.IsPage; //是否开启分页

                //GetUsers
                string _sql = "select * from UserInfo where Invalid=0 and Del=0 and UID in (select UID from UserRoles where RID in( select RID from Roles where RID=@RID)) " + OrderBy + "";
                SqlParameter[] _sp = { new SqlParameter("@RID", SqlDbType.Int) };
                _sp[0].Value = RID;
                DataTable UsersSourceDT = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

                //GetDepartment
                string _sql2 = "select a.UID,a.DeptID,b.Sort,b.ParentID,b.DeptName from UserDepts a left join Department b on a.DeptID=b.DeptID order by b.Sort";
                DataTable UserDeptsDT = MsSQLDbHelper.Query(_sql2).Tables[0];

                //GetJobTitle
                string _sql3 = "select a.UID,a.JTID,b.Sort,b.JobTitleName from UserJobTitle a left join JobTitle b on a.JTID=b.JTID order by b.Sort";
                DataTable UserJobTitleDT = MsSQLDbHelper.Query(_sql3).Tables[0];

                //GetRoles
                string _sql4 = "select a.UID,a.RID,b.Sort,b.RoleName from UserRoles a left join Roles b on a.RID=b.RID order by b.Sort";
                DataTable UserRolesDT = MsSQLDbHelper.Query(_sql4).Tables[0];

                //GetWorkHourSystem
                string _sql5 = "select a.UID,a.WorkHourSysID,a.WorkHourSysDate,a.DateModified,b.Sort,b.WorkHourSysName from HRUserWorkHourSystem a left join HRWorkHourSystem b on a.WorkHourSysID=b.WorkHourSysID order by b.Sort";
                DataTable UserWorkHourSystemDT = MsSQLDbHelper.Query(_sql5).Tables[0];

                UsersSourceCount = UsersSourceDT.Rows.Count;

                if (UsersSourceDT != null && UsersSourceDT.Rows.Count > 0)
                {
                    //对源数据进行分页
                    DataTable UsersTargetDT = UsersSourceDT.AsEnumerable().Skip((IntPage - 1) * IntLimit).Take(IntLimit).CopyToDataTable<DataRow>();
                    DataRow[] UsersRows = UsersTargetDT.Select();

                    foreach (DataRow UserDR in UsersRows)
                    {
                        strTpl += "{";

                        DataRow[] MicroRows = MicroDT.Select("ParentID<>0", "Sort");
                        foreach (DataRow MicroDR in MicroRows) //与类同 //for (int i = 0; i < _dt.Rows.Count; i++)
                        {

                            string TabColName = MicroDR["TabColName"].toJsonTrim(),
                                    ctlTypes = MicroDR["ctlTypes"].toJsonTrim(),
                                   ctlPlaceholder = MicroDR["ctlPlaceholder"].toJsonTrim(),
                                   strValue = UserDR["" + TabColName + ""].toJsonTrim();

                            switch (ctlTypes)
                            {
                                case "Date":
                                    strValue = strValue.toDateFormat(ctlPlaceholder);
                                    break;
                                case "Time":
                                    strValue = strValue.toDateFormat(!string.IsNullOrEmpty(ctlPlaceholder) ? ctlPlaceholder : "HH:mm:ss");
                                    break;
                                case "DateTime":
                                    strValue = strValue.toDateFormat(!string.IsNullOrEmpty(ctlPlaceholder) ? ctlPlaceholder : "yyyy-MM-dd HH:mm:ss");
                                    break;
                                case "DateWeek":
                                    strValue = strValue.toDateWeek();
                                    break;
                                case "DateWeekTime":
                                    strValue = strValue.toDateWeekTime("ww", ctlPlaceholder);
                                    break;
                            }

                            //获取用户所属部门，当TabColName=ColUserDepts时，ColUserDepts是用户所属部门，该字段不填写内容，主要用于生成LayuiDataTable时的表头参数
                            if (TabColName == "ColUserDepts")
                            {
                                DataRow[] UserDeptsRows = UserDeptsDT.Select("UID=" + UserDR["UID"].toInt(), "ParentID,Sort");
                                if (UserDeptsRows.Length > 0)
                                {
                                    string UserDepts = string.Empty;
                                    foreach (DataRow UserDeptsDR in UserDeptsRows)
                                    {
                                        UserDepts += UserDeptsDR["DeptName"].toJsonTrim() + " / ";
                                    }
                                    UserDepts = UserDepts.Substring(0, UserDepts.Length - 3);

                                    strTpl += "\"" + TabColName + "\":\"" + UserDepts + "\",";
                                }

                            }
                            //获取用户所属职位，当TabColName=ColUserJobTitle时，ColUserJobTitle是用户所属职位，该字段不填写内容，主要用于生成LayuiDataTable时的表头参数
                            else if (TabColName == "ColUserJobTitle")
                            {
                                DataRow[] UserJobTitleRows = UserJobTitleDT.Select("UID=" + UserDR["UID"].toInt(), "Sort");
                                if (UserJobTitleRows.Length > 0)
                                {
                                    string UserJobTitle = string.Empty;
                                    foreach (DataRow UserJobTitleDR in UserJobTitleRows)
                                    {
                                        UserJobTitle += UserJobTitleDR["JobTitleName"].toJsonTrim() + " / ";
                                    }
                                    UserJobTitle = UserJobTitle.Substring(0, UserJobTitle.Length - 3);

                                    strTpl += "\"" + TabColName + "\":\"" + UserJobTitle + "\",";
                                }
                            }
                            //获取用户所属角色，当TabColName=ColUserRoles时，ColUserRoles是用户所属角色，该字段不填写内容，主要用于生成LayuiDataTable时的表头参数
                            else if (TabColName == "ColUserRoles")
                            {
                                DataRow[] UserRolesRows = UserRolesDT.Select("UID=" + UserDR["UID"].toInt(), "Sort");
                                if (UserRolesRows.Length > 0)
                                {
                                    string UserRoles = string.Empty;
                                    foreach (DataRow UserRolesDR in UserRolesRows)
                                    {
                                        UserRoles += UserRolesDR["RoleName"].toJsonTrim() + " / ";
                                    }
                                    UserRoles = UserRoles.Substring(0, UserRoles.Length - 3);

                                    strTpl += "\"" + TabColName + "\":\"" + UserRoles + "\",";
                                }
                            }
                            //获取用户工时制，当TabColName=ColUserWorkHourSystem，ColUserWorkHourSystem是用户所属工时制，该字段不填写内容，主要用于生成LayuiDataTable时的表头参数
                            else if (TabColName == "ColUserWorkHourSystem")
                            {
                                DataRow[] UserWorkHourSystemRows = UserWorkHourSystemDT.Select("UID=" + UserDR["UID"].toInt(), "WorkHourSysDate desc, DateModified desc");
                                if (UserWorkHourSystemRows.Length > 0)
                                {
                                    string WorkHourSysName = string.Empty;
                                    int i = 0;
                                    foreach (DataRow UserWorkHourSystemDR in UserWorkHourSystemRows)
                                    {
                                        if (i == 0)
                                            WorkHourSysName += UserWorkHourSystemDR["WorkHourSysName"].toJsonTrim() + " / ";

                                        i = i + 1;
                                    }
                                    WorkHourSysName = WorkHourSysName.Substring(0, WorkHourSysName.Length - 3);

                                    strTpl += "\"" + TabColName + "\":\"" + WorkHourSysName + "\",";
                                }
                            }
                            else
                                strTpl += "\"" + TabColName + "\":\"" + strValue + "\",";
                        }

                        strTpl = strTpl.Substring(0, strTpl.Length - 1);
                        strTpl += "},";
                    }

                    strTpl = strTpl.Substring(0, strTpl.Length - 1);
                }
                flag = "{\"code\": 0, \"msg\": \"\", \"count\": " + UsersSourceCount + ", \"data\":  [" + strTpl + "] }";
            }
        }
        catch (Exception ex) { flag = ex.ToString(); }


        return flag;

    }

    private string GetSearchUsersList(string ShortTableName, string Keyword, int IntPage = 1, int IntLimit = 10)
    {
        string flag = string.Empty, strTpl = string.Empty;
        int UsersSourceCount = 0;
        try
        {
            //获取长表名
            string TableName = MicroPublic.GetTableName(ShortTableName);

            if (!string.IsNullOrEmpty(TableName))
            {
                //获取表的字段和相关属性
                var getTableAttr = MicroDataTable.GetTableAttr(TableName);
                DataTable MicroDT = getTableAttr.MicroDT; //获取存放在MicroTables的数据，该表是存放每个表的表名和字段
                string PrimaryKeyName = getTableAttr.PrimaryKeyName;  //获得主键字段名称
                string SortFieldName = getTableAttr.SortFieldName;  //获得排序字段名称
                string OrderBy = getTableAttr.OrderBy;  //获得order by 命令字符串
                Boolean MainSub = getTableAttr.MainSub;  //用于判断是否开启父子项
                Boolean IsPage = getTableAttr.IsPage; //是否开启分页

                //GetUsers
                string _sql = "select * from UserInfo where Invalid=0 and Del=0 and (UserName like @Keyword or ChineseName like @Keyword or EnglishName like @Keyword or EMail like @Keyword or WorkTel like @Keyword or WorkMobilePhone like @Keyword or WeChat like @Keyword or WorkWeChat like @Keyword or QQ like @Keyword or WorkQQ like @Keyword or AdDisplayName like @Keyword or AdDescription like @Keyword or AdDepartment like @Keyword or Description like @Keyword )  " + OrderBy + "";
                SqlParameter[] _sp = { new SqlParameter("@Keyword", SqlDbType.VarChar) };
                _sp[0].Value = "%" + Keyword.toJsonTrim() + "%";

                DataTable UsersSourceDT = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

                //GetDepartment
                string _sql2 = "select a.UID,a.DeptID,b.Sort,b.ParentID,b.DeptName from UserDepts a left join Department b on a.DeptID=b.DeptID order by b.Sort";
                DataTable UserDeptsDT = MsSQLDbHelper.Query(_sql2).Tables[0];

                //GetJobTitle
                string _sql3 = "select a.UID,a.JTID,b.Sort,b.JobTitleName from UserJobTitle a left join JobTitle b on a.JTID=b.JTID order by b.Sort";
                DataTable UserJobTitleDT = MsSQLDbHelper.Query(_sql3).Tables[0];

                //GetRoles
                string _sql4 = "select a.UID,a.RID,b.Sort,b.RoleName from UserRoles a left join Roles b on a.RID=b.RID order by b.Sort";
                DataTable UserRolesDT = MsSQLDbHelper.Query(_sql4).Tables[0];

                //GetWorkHourSystem
                string _sql5 = "select a.UID,a.WorkHourSysID,a.WorkHourSysDate,a.DateModified,b.Sort,b.WorkHourSysName from HRUserWorkHourSystem a left join HRWorkHourSystem b on a.WorkHourSysID=b.WorkHourSysID order by b.Sort";
                DataTable UserWorkHourSystemDT = MsSQLDbHelper.Query(_sql5).Tables[0];

                UsersSourceCount = UsersSourceDT.Rows.Count;

                if (UsersSourceDT != null && UsersSourceDT.Rows.Count > 0)
                {
                    //对源数据进行分页
                    DataTable UsersTargetDT = UsersSourceDT.AsEnumerable().Skip((IntPage - 1) * IntLimit).Take(IntLimit).CopyToDataTable<DataRow>();
                    DataRow[] UsersRows = UsersTargetDT.Select();

                    foreach (DataRow UserDR in UsersRows)
                    {
                        strTpl += "{";

                        DataRow[] MicroRows = MicroDT.Select("ParentID<>0", "Sort");
                        foreach (DataRow MicroDR in MicroRows) //与类同 //for (int i = 0; i < _dt.Rows.Count; i++)
                        {

                            string TabColName = MicroDR["TabColName"].toJsonTrim(),
                                    ctlTypes = MicroDR["ctlTypes"].toJsonTrim(),
                                   ctlPlaceholder = MicroDR["ctlPlaceholder"].toJsonTrim(),
                                   strValue = UserDR["" + TabColName + ""].toJsonTrim();

                            switch (ctlTypes)
                            {
                                case "Date":
                                    strValue = strValue.toDateFormat(ctlPlaceholder);
                                    break;
                                case "Time":
                                    strValue = strValue.toDateFormat(!string.IsNullOrEmpty(ctlPlaceholder) ? ctlPlaceholder : "HH:mm:ss");
                                    break;
                                case "DateTime":
                                    strValue = strValue.toDateFormat(!string.IsNullOrEmpty(ctlPlaceholder) ? ctlPlaceholder : "yyyy-MM-dd HH:mm:ss");
                                    break;
                                case "DateWeek":
                                    strValue = strValue.toDateWeek();
                                    break;
                                case "DateWeekTime":
                                    strValue = strValue.toDateWeekTime("ww", ctlPlaceholder);
                                    break;
                            }

                            //获取用户所属部门，当TabColName=ColUserDepts时，ColUserDepts是用户所属部门，该字段不填写内容，主要用于生成LayuiDataTable时的表头参数
                            if (TabColName == "ColUserDepts")
                            {
                                DataRow[] UserDeptsRows = UserDeptsDT.Select("UID=" + UserDR["UID"].toInt(), "ParentID,Sort");
                                if (UserDeptsRows.Length > 0)
                                {
                                    string UserDepts = string.Empty;
                                    foreach (DataRow UserDeptsDR in UserDeptsRows)
                                    {
                                        UserDepts += UserDeptsDR["DeptName"].toJsonTrim() + " / ";
                                    }
                                    UserDepts = UserDepts.Substring(0, UserDepts.Length - 3);

                                    strTpl += "\"" + TabColName + "\":\"" + UserDepts + "\",";
                                }

                            }
                            //获取用户所属职位，当TabColName=ColUserJobTitle时，ColUserJobTitle是用户所属职位，该字段不填写内容，主要用于生成LayuiDataTable时的表头参数
                            else if (TabColName == "ColUserJobTitle")
                            {
                                DataRow[] UserJobTitleRows = UserJobTitleDT.Select("UID=" + UserDR["UID"].toInt(), "Sort");
                                if (UserJobTitleRows.Length > 0)
                                {
                                    string UserJobTitle = string.Empty;
                                    foreach (DataRow UserJobTitleDR in UserJobTitleRows)
                                    {
                                        UserJobTitle += UserJobTitleDR["JobTitleName"].toJsonTrim() + " / ";
                                    }
                                    UserJobTitle = UserJobTitle.Substring(0, UserJobTitle.Length - 3);

                                    strTpl += "\"" + TabColName + "\":\"" + UserJobTitle + "\",";
                                }
                            }
                            //获取用户所属角色，当TabColName=ColUserRoles时，ColUserRoles是用户所属角色，该字段不填写内容，主要用于生成LayuiDataTable时的表头参数
                            else if (TabColName == "ColUserRoles")
                            {
                                DataRow[] UserRolesRows = UserRolesDT.Select("UID=" + UserDR["UID"].toInt(), "Sort");
                                if (UserRolesRows.Length > 0)
                                {
                                    string UserRoles = string.Empty;
                                    foreach (DataRow UserRolesDR in UserRolesRows)
                                    {
                                        UserRoles += UserRolesDR["RoleName"].toJsonTrim() + " / ";
                                    }
                                    UserRoles = UserRoles.Substring(0, UserRoles.Length - 3);

                                    strTpl += "\"" + TabColName + "\":\"" + UserRoles + "\",";
                                }
                            }
                            //获取用户工时制，当TabColName=ColUserWorkHourSystem，ColUserWorkHourSystem是用户所属工时制，该字段不填写内容，主要用于生成LayuiDataTable时的表头参数
                            else if (TabColName == "ColUserWorkHourSystem")
                            {
                                DataRow[] UserWorkHourSystemRows = UserWorkHourSystemDT.Select("UID=" + UserDR["UID"].toInt(), "WorkHourSysDate desc, DateModified desc");
                                if (UserWorkHourSystemRows.Length > 0)
                                {
                                    string WorkHourSysName = string.Empty;
                                    int i = 0;
                                    foreach (DataRow UserWorkHourSystemDR in UserWorkHourSystemRows)
                                    {
                                        if (i == 0)
                                            WorkHourSysName += UserWorkHourSystemDR["WorkHourSysName"].toJsonTrim() + " / ";

                                        i = i + 1;
                                    }
                                    WorkHourSysName = WorkHourSysName.Substring(0, WorkHourSysName.Length - 3);

                                    strTpl += "\"" + TabColName + "\":\"" + WorkHourSysName + "\",";
                                }
                            }
                            else
                                strTpl += "\"" + TabColName + "\":\"" + strValue + "\",";
                        }

                        strTpl = strTpl.Substring(0, strTpl.Length - 1);
                        strTpl += "},";
                    }

                    strTpl = strTpl.Substring(0, strTpl.Length - 1);
                }
                flag = "{\"code\": 0, \"msg\": \"\", \"count\": " + UsersSourceCount + ", \"data\":  [" + strTpl + "] }";
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