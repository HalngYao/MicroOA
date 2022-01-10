<%@ WebHandler Language="C#" Class="GetUserPublicInfoForXmSelect" %>

using System;
using System.Web;
using System.Data;
using MicroDBHelper;
using MicroPublicHelper;
using MicroAuthHelper;

public class GetUserPublicInfoForXmSelect : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = MicroPublic.GetMsg("DenyUrlParaError"),
             Action = context.Server.UrlDecode(context.Request.Form["action"]),
             Type = context.Server.UrlDecode(context.Request.Form["type"]),
             MID = context.Server.UrlDecode(context.Request.Form["mid"]),
             DefaultValue = context.Server.UrlDecode(context.Request.Form["DefaultValue"]),
             Fields = context.Server.UrlDecode(context.Request.Form["fields"]);

        Action = string.IsNullOrEmpty(Action) == true ? "" : Action.ToLower();

        //测试数据
        //Action = "get";
        //Type = "form";
        //DefaultValue = "Admin";

        try
        {
            if (Action == "get")
            {
                Type = string.IsNullOrEmpty(Type) == true ? "" : Type.ToLower();

                if (Type == "dept" || Type == "depts" || Type == "department" || Type == "udepts")
                    flag = GetMainDepts(DefaultValue);

                if (Type == "jtitle" || Type == "jobtitle" || Type == "ujtitle" || Type == "setaujtitle")
                    flag = GetJobTitle(DefaultValue);

                if (Type == "role" || Type == "roles" || Type == "uroles" || Type == "setaurole")
                    flag = GetRoles(DefaultValue);

                if (Type == "uworkhoursystem")
                    flag = GetWorkHourSystem(DefaultValue);

                if (Type == "use" || Type == "userinfo")
                    flag = GetUsers(DefaultValue);

                if (Type == "form")
                    flag = GetForms(DefaultValue);

                if (Type == "deptusers")
                    flag = GetMainDeptUsers(DefaultValue);
            }

        }
        catch (Exception ex)
        {
            flag = ex.ToString();
        }

        context.Response.Write(flag);
    }

    /// <summary>
    /// 获取主部门生成XmSelect需要的数据
    /// </summary>
    /// <param name="UID">传入UID，读出该用户所属的部门，让所属的部门选中</param>
    /// <returns></returns>
    private string GetMainDepts(string DefaultValue = "")
    {
        string flag = string.Empty, _str = string.Empty;

        //查询主部门
        string _sql = "select * from Department where Invalid=0 and Del=0 order by ParentID,Sort";
        DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

        if (_dt != null && _dt.Rows.Count > 0)
        {
            int i = 0;

            //得到顶级部门
            DataRow[] _rows = _dt.Select("ParentID=0", "ParentID,Sort");

            //有记录时加上中括号[]
            if (_rows.Length > 0)
                _str += "[";

            foreach (DataRow _dr in _rows)
            {

                i = i + 1;
                string ID = _dr["DeptID"].ToString();
                string Name = _dr["DeptName"].ToString();

                Boolean Selected = false;
                if (!string.IsNullOrEmpty(DefaultValue))
                {
                    string[] Arr = DefaultValue.Split(',');
                    for (int j = 0; j < Arr.Length; j++)
                    {
                        if (Arr[j].toInt() == ID.toInt())
                            Selected = true;
                    }
                }

                _str += "{";
                _str += "name:'" + Name + "', value: " + ID + ", selected: " + Selected.ToString().ToLower() + "";
                _str += GetSubDept(ID, _dt, DefaultValue); //查询子部门
                _str += "}";

                //如果不是最后记录里要加上逗号
                if (_rows.Length != i)
                    _str += ",";
            }

            //有记录时加上中括号[]
            if (_rows.Length > 0)
                _str += "]";

        }
        flag = _str;
        return flag;

    }

    /// <summary>
    /// 获取子部门生成XmSelect需要的数据
    /// </summary>
    /// <param name="ParentID"></param>
    /// <param name="_dt"></param>
    /// <param name="_dt2"></param>
    /// <param name="UID"></param>
    /// <returns></returns>
    private string GetSubDept(string ParentID, DataTable _dt, string DefaultValue)
    {
        string flag = string.Empty, _str = string.Empty;

        DataRow[] _rows = _dt.Select("ParentID=" + ParentID, "ParentID,Sort");

        if (_rows.Length > 0)
        {
            int i = 0;

            _str += ", children: [";

            foreach (DataRow _dr in _rows)
            {
                i = i + 1;
                string ID = _dr["DeptID"].ToString();
                string Name = _dr["DeptName"].ToString();

                Boolean Selected = false;
                if (!string.IsNullOrEmpty(DefaultValue))
                {
                    string[] Arr = DefaultValue.Split(',');
                    for (int j = 0; j < Arr.Length; j++)
                    {
                        if (Arr[j].toInt() == ID.toInt())
                            Selected = true;
                    }
                }

                _str += "{";
                _str += "name:'" + Name + "', value: " + ID + ", selected: " + Selected.ToString().ToLower() + "";
                _str += GetSubDept(ID, _dt, DefaultValue);
                _str += "}";

                if (_rows.Length != i)
                    _str += ",";

            }
            _str += "]";

        }

        flag = _str;
        return flag;
    }

    /// <summary>
    /// 获取职位职称表生成XmSelect需要的数据
    /// </summary>
    /// <param name="DefaultValue">传入UID，读出该用户所属的职称，让所属的职称选中</param>
    /// <returns></returns>
    private string GetJobTitle(string DefaultValue = "")
    {
        string flag = string.Empty, _str = string.Empty;

        //查询职位职称表
        string _sql = "select * from JobTitle where Invalid=0 and Del=0 order by Sort";
        DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

        if (_dt != null && _dt.Rows.Count > 0)
        {
            int i = 0;

            DataRow[] _rows = _dt.Select("", "Sort");

            foreach (DataRow _dr in _rows)
            {
                i = i + 1;
                string ID = _dr["JTID"].ToString();
                string Name = _dr["JobTitleName"].ToString();

                Boolean Selected = false;
                if (!string.IsNullOrEmpty(DefaultValue))
                {
                    string[] Arr = DefaultValue.Split(',');
                    for (int j = 0; j < Arr.Length; j++)
                    {
                        if (Arr[j].toInt() == ID.toInt())
                            Selected = true;
                    }
                }

                _str += "{";
                _str += "name:'" + Name + "', value: " + ID + ", selected: " + Selected.ToString().ToLower() + "";
                _str += "}";

                //如果不是最后记录里要加上逗号
                if (_rows.Length != i)
                    _str += ",";
            }
        }
        flag = "[" + _str + "]";
        return flag;
    }


    /// <summary>
    /// 获取角色表生成XmSelect需要的数据
    /// </summary>
    /// <param name="DefaultValue">传入DefaultValue，让对应项默认选中</param>
    /// <returns></returns>
    private string GetRoles(string DefaultValue = "")
    {
        string flag = string.Empty, _str = string.Empty;

        //查询角色表
        string _sql = "select * from Roles where Invalid=0 and Del=0 order by Sort";
        DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

        if (_dt != null && _dt.Rows.Count > 0)
        {
            int i = 0;  //ID流水号，用作tree生成树 id

            //得到主模块
            DataRow[] _rows = _dt.Select("", "Sort");

            foreach (DataRow _dr in _rows)
            {

                i = i + 1;
                string ID = _dr["RID"].ToString();
                string Name = _dr["RoleName"].ToString();

                Boolean Selected = false;
                if (!string.IsNullOrEmpty(DefaultValue))
                {
                    string[] Arr = DefaultValue.Split(',');
                    for (int j = 0; j < Arr.Length; j++)
                    {
                        if (Arr[j].toInt() == ID.toInt())
                            Selected = true;
                    }
                }

                _str += "{";
                _str += "name:'" + Name + "', value: " + ID + ", selected: " + Selected.ToString().ToLower() + "";
                _str += "}";

                //如果不是最后记录里要加上逗号
                if (_rows.Length != i)
                    _str += ",";
            }
        }
        flag = "[" + _str + "]";
        return flag;

    }


    /// <summary>
    /// 工时制表
    /// </summary>
    /// <param name="DefaultValue"></param>
    /// <returns></returns>
    private string GetWorkHourSystem(string DefaultValue = "")
    {
        string flag = string.Empty, _str = string.Empty;

        //查询工时制表
        string _sql = "select * from HRWorkHourSystem where Invalid=0 and Del=0 order by Sort";
        DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

        if (_dt != null && _dt.Rows.Count > 0)
        {
            int i = 0;

            DataRow[] _rows = _dt.Select("", "Sort");

            foreach (DataRow _dr in _rows)
            {
                i = i + 1;
                string ID = _dr["WorkHourSysID"].ToString();
                string Name = _dr["WorkHourSysName"].ToString();

                Boolean Selected = false;
                if (!string.IsNullOrEmpty(DefaultValue))
                {
                    string[] Arr = DefaultValue.Split(',');
                    for (int j = 0; j < Arr.Length; j++)
                    {
                        if (Arr[j].toInt() == ID.toInt())
                            Selected = true;
                    }
                }

                _str += "{";
                _str += "name:'" + Name + "', value: " + ID + ", selected: " + Selected.ToString().ToLower() + "";
                _str += "}";

                //如果不是最后记录里要加上逗号
                if (_rows.Length != i)
                    _str += ",";
            }
        }
        flag = "[" + _str + "]";
        return flag;
    }

    /// <summary>
    /// 获取用户表生成XmSelect需要的数据
    /// </summary>
    /// <param name="DefaultValue">传入UserNameArr，让对应的用户选中，如：User01,User02</param>
    /// <returns></returns>
    private string GetUsers(string DefaultValue = "")
    {
        string flag = string.Empty, _str = string.Empty;

        //查询角色表
        string _sql = "select * from UserInfo where Invalid=0 and Del=0 order by UserName";
        DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

        if (_dt != null && _dt.Rows.Count > 0)
        {
            int i = 0;  //ID流水号，用作tree生成树 id

            //得到主模块
            DataRow[] _rows = _dt.Select("", "UserName");

            foreach (DataRow _dr in _rows)
            {

                i = i + 1;
                string ID = _dr["UID"].ToString();
                string Name = string.Empty;
                string UserName = _dr["UserName"].ToString();
                string ChineaseName = _dr["ChineseName"].ToString();
                string EnglishName = _dr["EnglishName"].ToString();
                string AdDisplayName = _dr["AdDisplayName"].ToString();

                if (!string.IsNullOrEmpty(ChineaseName) && !string.IsNullOrEmpty(EnglishName))
                    Name = EnglishName + "(" + ChineaseName + ")";
                else if (!string.IsNullOrEmpty(ChineaseName) && string.IsNullOrEmpty(EnglishName))
                    Name = ChineaseName;
                else if (string.IsNullOrEmpty(ChineaseName) && !string.IsNullOrEmpty(EnglishName))
                    Name = EnglishName;
                else if (!string.IsNullOrEmpty(AdDisplayName))
                    Name = AdDisplayName;
                else
                    Name = UserName;

                Boolean Selected = false;
                //string v = string.Empty;
                if (!string.IsNullOrEmpty(DefaultValue))
                {
                    string[] Arr = DefaultValue.Split(',');
                    for (int j = 0; j < Arr.Length; j++)
                    {
                        //为数字类型时与ID作比较，否则与UserName作比较
                        if (MicroPublic.IsNumeric(Arr[j].toStringTrim()))
                        {
                            if (Arr[j].toInt() == ID.toInt())
                                Selected = true;
                        }
                        else
                        {
                            if (Arr[j].toStringTrim().ToLower() == UserName.ToLower())
                                Selected = true;
                        }
                    }
                }


                _str += "{";
                _str += "name:'" + Name + "', value: " + ID + ", selected: " + Selected.ToString().ToLower() + "";
                _str += "}";

                //如果不是最后记录里要加上逗号
                if (_rows.Length != i)
                    _str += ",";
            }
        }
        flag = "[" + _str + "]";
        return flag;

    }

    /// <summary>
    /// 获取Forms表生成XmSelect需要的数据
    /// </summary>
    /// <param name="DefaultValue">默认值传入FormID，让其选中</param>
    /// <returns></returns>
    private string GetForms(string DefaultValue = "")
    {
        string flag = string.Empty, _str = string.Empty;

        string _sql = "select * from Forms order by Sort";
        DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

        if (_dt != null && _dt.Rows.Count > 0)
        {
            int i = 0;

            DataRow[] _rows = _dt.Select("", "Sort");

            foreach (DataRow _dr in _rows)
            {
                i = i + 1;
                string ID = _dr["FormID"].ToString();
                string Name = _dr["FormName"].ToString();
                string ShortTableName = _dr["ShortTableName"].ToString();

                Boolean Selected = false;
                if (!string.IsNullOrEmpty(DefaultValue))
                {
                    string[] Arr = DefaultValue.Split(',');
                    for (int j = 0; j < Arr.Length; j++)
                    {
                        if (Arr[j].toInt() == ID.toInt())
                            Selected = true;
                    }
                }

                _str += "{";
                _str += "name:'" + Name + "', value: " + ID + ", selected: " + Selected.ToString().ToLower() + ",stn:'" + ShortTableName + "'";
                _str += "}";

                //如果不是最后记录里要加上逗号
                if (_rows.Length != i)
                    _str += ",";
            }
        }
        flag = "[" + _str + "]";
        return flag;
    }

    #region 通过部门查询用户生成XmSelect需要的数据

    private string GetMainDeptUsers(string DefaultValue = "")
    {
        string flag = string.Empty, _str = string.Empty;

        //查询部门
        string _sql = "select ParentID,Sort,DeptID,DeptName from Department where Invalid=0 and Del=0 order by ParentID,Sort";
        DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

        //查询用户所属那些部门
        string _sql2 = "select a.DeptID,b.UID,b.UserName,b.ChineseName,b.EnglishName,b.AdDisplayName from UserDepts a left join UserInfo b on a.UID=b.UID where b.Invalid=0 and b.Del=0 ";
        DataTable _dt2 = MsSQLDbHelper.Query(_sql2).Tables[0];

        if (_dt != null && _dt.Rows.Count > 0)
        {
            int i = 0;

            //得到主部门
            DataRow[] _rows = _dt.Select("ParentID=0", "ParentID,Sort");

            //有记录时加上中括号[]
            if (_rows.Length > 0)
                _str += "[";

            foreach (DataRow _dr in _rows)
            {
                i = i + 1;
                string ID = _dr["DeptID"].ToString();
                string Name = _dr["DeptName"].ToString();

                _str += "{";
                _str += "name:'" + Name + "', value: 80000" + ID + "";
                _str += GetSubDeptUsers(ID, _dt, _dt2, DefaultValue); //查询子部门
                _str += "}";

                //如果不是最后记录里要加上逗号
                if (_rows.Length != i)
                    _str += ",";
            }

            //有记录时加上中括号[]
            if (_rows.Length > 0)
                _str += "]";

        }
        flag = _str;

        return flag;

    }

    private string GetSubDeptUsers(string DeptID, DataTable _dt, DataTable _dt2, string DefaultValue)
    {
        string flag = string.Empty, _str = string.Empty;

        //根据主部门ID查询得到子部门
        DataRow[] _rows = _dt.Select("ParentID=" + DeptID, "ParentID,Sort");

        //如果子部门记录>0且部门下有用户>0时，显示children代码块
        if (_rows.Length > 0 && GetDeptUsersCount(DeptID, _dt2) > 0)
            _str += ", children: [";

        //得到用户所属部门，如果没有则返回空
        _str += GetDeptUsers(DeptID, _dt, _dt2, DefaultValue);

        if (_rows.Length > 0)
        {
            int i = 0;

            //如果没有子部门时则在这里显示children代码块
            if (GetDeptUsersCount(DeptID, _dt2) == 0)
                _str += ", children: [";

            foreach (DataRow _dr in _rows)
            {
                i = i + 1;
                string ID = _dr["DeptID"].ToString();
                string Name = _dr["DeptName"].ToString();

                _str += "{";
                _str += "name:'" + Name + "', value: 80000" + ID + "";
                _str += GetSubDeptUsers(ID, _dt, _dt2, DefaultValue); //查询子部门
                _str += "}";

                if (_rows.Length != i)
                    _str += ",";

            }
            if (GetDeptUsersCount(DeptID, _dt2) == 0)
                _str += "]";

        }
        if (GetDeptUsersCount(DeptID, _dt2) > 0 && _rows.Length > 0)
            _str += "]";

        flag = _str;
        return flag;
    }

    private string GetDeptUsers(string DeptID, DataTable _dt, DataTable _dt2, string DefaultValue)
    {
        string flag = string.Empty, _str = string.Empty;
        DataRow[] _rows = _dt.Select("ParentID=" + DeptID, "ParentID,Sort");
        DataRow[] _rows2 = _dt2.Select("DeptID=" + DeptID);

        if (_rows2.Length > 0)
        {
            int i = 0;

            if (_rows.Length == 0)
                _str += ", children: [";

            foreach (DataRow _dr2 in _rows2)
            {

                i = i + 1;

                string ID = _dr2["UID"].ToString();
                string Name = string.Empty;
                string UserName = _dr2["UserName"].ToString();
                string ChineaseName=_dr2["ChineseName"].ToString();
                string EnglishName=_dr2["EnglishName"].ToString();
                string AdDisplayName=_dr2["AdDisplayName"].ToString();

                if (!string.IsNullOrEmpty(ChineaseName) && !string.IsNullOrEmpty(EnglishName))
                    Name = EnglishName + "(" + ChineaseName + ")";
                else if (!string.IsNullOrEmpty(ChineaseName) && string.IsNullOrEmpty(EnglishName))
                    Name = ChineaseName;
                else if (string.IsNullOrEmpty(ChineaseName) && !string.IsNullOrEmpty(EnglishName))
                    Name = EnglishName;
                else if (!string.IsNullOrEmpty(AdDisplayName))
                    Name = AdDisplayName;
                else
                    Name = UserName;

                Boolean Selected = false;
                if (!string.IsNullOrEmpty(DefaultValue))
                {
                    string[] Arr = DefaultValue.Split(',');
                    for (int j = 0; j < Arr.Length; j++)
                    {
                        if (Arr[j].toInt() == ID.toInt())
                            Selected = true;
                    }
                }

                _str += "{";
                _str += "name:'" + Name + "', value: " + ID + ", selected: " + Selected.ToString().ToLower() + "";
                _str += "}";

                if (_rows2.Length != i || _rows.Length > 0)
                    _str += ",";

            }
            if (_rows.Length == 0)
                _str += "]";
        }
        flag = _str;
        return flag;
    }




    private int GetDeptUsersCount(string DeptID, DataTable _dt2)
    {
        int Count = 0;

        DataRow[] _rows2 = _dt2.Select("DeptID=" + DeptID);
        Count = _rows2.Length;

        return Count;
    }

    #endregion



    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}