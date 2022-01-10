<%@ WebHandler Language="C#" Class="JobTitlePerms" %>

using System;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroDTHelper;
using MicroPublicHelper;
using Newtonsoft.Json.Linq;
using MicroJsonHelper;
using System.Collections.Generic;
using MicroAuthHelper;

public class JobTitlePerms : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = MicroPublic.GetMsg("DenyUrlParaError"),
             Action = context.Server.UrlDecode(context.Request.Form["action"]),
             MID = context.Server.UrlDecode(context.Request.Form["mid"]),
             JTID = context.Server.UrlDecode(context.Request.Form["id"]),
             Fields = context.Server.UrlDecode(context.Request.Form["fields"]);

        Action = string.IsNullOrEmpty(Action) == true ? "" : Action.ToLower();

        //Action = "getroleperms";
        //Fields = "%5B%7B%22id%22:23,%22title%22:%22MainNav%20--%20%E3%80%90%E6%9D%83%E9%99%90%E3%80%91%22,%22spread%22:true,%22field%22:%22%22,%22children%22:%5B%7B%22id%22:23,%22title%22:%22%E6%B5%8F%E8%A7%88%22,%22spread%22:true,%22field%22:%22113%22%7D%5D%7D%5D";
        //RID = "1";   

        try
        {
            if (!string.IsNullOrEmpty(Action))
            {
                Action = Action.ToLower();

                if (Action == "getjobtitleperms" && !string.IsNullOrEmpty(JTID))
                    flag = GetMainMenu(JTID);

                if (Action == "modify" && !string.IsNullOrEmpty(JTID) && !string.IsNullOrEmpty(Fields))
                    flag = ModifyJobTitlePermissions(JTID, Fields);

                if (Action == "modifyalias" && !string.IsNullOrEmpty(Fields))
                    flag = ModifyAlias(Fields);
            }
        }
        catch (Exception ex)
        {
            flag = ex.ToString();
        }

        context.Response.Write(flag);
    }


    /// <summary>
    /// 显示主模块和可操作权限
    /// </summary>
    /// <param name="JTID">职位/职称ID</param>
    /// <returns></returns>
    private string GetMainMenu(string JTID)
    {
        string flag = string.Empty, _str = string.Empty;

        //查询模块
        string _sql = "select * from Modules where Invalid=0 and Del=0 order by ParentID,Sort";
        DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

        //查询模块拥有哪些可操作的权限
        string _sql2 = "select a.MPID,a.MID,a.PID,a.Alias,b.Sort,b.PermissionName,b.PermissionCode from ModulePermissions a left join Permissions b on a.PID=b.PID where a.Invalid=0 and a.Del=0 and b.Invalid=0 and b.Del=0 order by b.Sort";
        DataTable _dt2 = MsSQLDbHelper.Query(_sql2).Tables[0];

        string _sql3 = "select MPID, JTID from JobTitlePermissions ";
        DataTable _dt3 = MsSQLDbHelper.Query(_sql3).Tables[0];

        if (_dt != null && _dt.Rows.Count > 0)
        {
            int i = 0;  //ID流水号，用作tree生成树 id

            //得到主模块
            DataRow[] _rows = _dt.Select("ParentID=0", "ParentID,Sort");

            //有记录时加上中括号[]
            if (_rows.Length > 0)
                _str += "[";

            foreach (DataRow _dr in _rows)
            {
                i = i + 1;
                string ModuleID = _dr["MID"].ToString();

                //判断为最后模块时加上“ -- 【权限】”突出显示
                //string ModuleName = _dr["ModuleName"].ToString() + (GetSubModuleCount(ModuleID, _dt) == 0 ? " -- 【模块】" : "");
                string ModuleName = "<b class=\"ws-font-red\">" + _dr["ModuleName"].ToString() + "</b>";
                _str += "{";
                _str += "id: 100" + ModuleID + ", title: '" + ModuleName + "' , spread: true, field:''";
                _str += GetSubMenu(ModuleID, _dt, _dt2, _dt3, JTID); //查询子模块
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
    /// 显示子模块和可操作权限
    /// </summary>
    /// <param name="MID">ModuleID</param>
    /// <param name="_dt">Modules</param>
    /// <param name="_dt2">ModulePermissions left join Permissions</param>
    /// <param name="_dt3">JobTitlePermissions</param>
    /// <param name="JTID">职位/职称ID</param>
    /// <returns></returns>
    private string GetSubMenu(string MID, DataTable _dt, DataTable _dt2, DataTable _dt3, string JTID)
    {
        string flag = string.Empty, _str = string.Empty;

        //根据主模块ID查询得到子模块
        DataRow[] _rows = _dt.Select("ParentID=" + MID, "ParentID,Sort");

        //如果子模块记录>0且拥有可操作权限>0时，显示children代码块
        if (_rows.Length > 0 && GetModulePermsCount(MID, _dt2) > 0)
            _str += ", children: [";

        //得到可操作权限，如果没有可操作权限返回空
        _str += GetModulePerms(MID, _dt, _dt2, _dt3, JTID);

        if (_rows.Length > 0)
        {
            int i = 0;

            //如果有子模块记录并且没有可操作权限时，在这里显示children代码块
            if (GetModulePermsCount(MID, _dt2) == 0)
                _str += ", children: [";

            foreach (DataRow _dr in _rows)
            {
                i = i + 1;
                string ModuleID = _dr["MID"].ToString();

                //判断为最后模块时加上“ -- 【权限】”突出显示
                //string ModuleName = _dr["ModuleName"].ToString() + (GetSubModuleCount(ModuleID, _dt) == 0 ? " -- 【模块】" : "");
                string ModuleName = "<b class=\"ws-font-red\">" + _dr["ModuleName"].ToString() + "</b>";
                _str += "{";
                _str += "id: 200" + ModuleID + ", title: '" + ModuleName + "', spread: true, field:''";
                _str += GetSubMenu(ModuleID, _dt, _dt2, _dt3, JTID); //递归
                _str += "}";

                if (_rows.Length != i)
                    _str += ",";

            }
            if (GetModulePermsCount(MID, _dt2) == 0)
                _str += "]";

        }
        if (GetModulePermsCount(MID, _dt2) > 0 && _rows.Length > 0)
            _str += "]";

        flag = _str;
        return flag;
    }


    /// <summary>
    /// 显示模块可操作权限
    /// </summary>
    /// <param name="MID">ModuleID</param>
    /// <param name="_dt">Modules</param>
    /// <param name="_dt2">ModulePermissions left join Permissions</param>
    /// <param name="_dt3">JobTitlePermissions</param>
    /// <param name="JTID">职位/职称ID</param>
    /// <returns></returns>
    private string GetModulePerms(string MID, DataTable _dt, DataTable _dt2, DataTable _dt3, string JTID)
    {
        string flag = string.Empty, _str = string.Empty;
        DataRow[] _rows = _dt.Select("ParentID=" + MID, "ParentID,Sort");
        DataRow[] _rows2 = _dt2.Select("MID=" + MID, "Sort");

        if (_rows2.Length > 0)
        {
            int i = 0;

            if (_rows.Length == 0)
                _str += ", children: [";

            foreach (DataRow _dr2 in _rows2)
            {

                i = i + 1;
                string MPID = _dr2["MPID"].ToString();
                string ModuleID = _dr2["MID"].ToString();
                string PID = _dr2["PID"].ToString();
                string PermissionName = string.IsNullOrEmpty(_dr2["Alias"].ToString()) == true ? _dr2["PermissionName"].ToString() : _dr2["Alias"].ToString();

                string Checked = string.Empty;
                DataRow[] _rows3 = _dt3.Select("MPID = " + MPID + " and JTID= " + JTID);
                if (_rows3.Length > 0)
                    Checked = ",checked:true";

                _str += "{";
                _str += "id:300" + MPID + ", title: '" + PermissionName + "', spread: true, field:'" + MPID + "' " + Checked + "";
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

    /// <summary>
    /// 传入模块可操作权限表判断是否有记录，并返回结果
    /// </summary>
    /// <param name="MID"></param>
    /// <param name="_dt2"></param>
    /// <returns></returns>
    private int GetModulePermsCount(string MID, DataTable _dt2)
    {
        int Count = 0;

        DataRow[] _rows2 = _dt2.Select("MID=" + MID);
        Count = _rows2.Length;

        return Count;
    }

    /// <summary>
    /// 传入模块表判断是否有子模块记录，并返回结果
    /// </summary>
    /// <param name="MID"></param>
    /// <param name="_dt"></param>
    /// <returns></returns>
    private int GetSubModuleCount(string MID, DataTable _dt)
    {
        int i = 0;
        DataRow[] _rows = _dt.Select("ParentID=" + MID, "ParentID,Sort");
        i = _rows.Length;
        return i;
    }


    /// <summary>
    /// 执行GetMainCheckedData方法
    /// </summary>
    /// <param name="Fields"></param>
    /// <param name="JTID"></param>
    /// <returns></returns>
    private string ModifyJobTitlePermissions(string JTID, string Fields)
    {
        string flag = string.Empty;

        flag = GetMainCheckedData(Fields, JTID);

        return flag;
    }


    /// <summary>
    /// 获取主节点选中的数据
    /// </summary>
    /// <param name="Str">选中数据组成的字符串</param>
    /// <param name="JTID">角色ID</param>
    /// <returns></returns>
    private string GetMainCheckedData(string Fields, string JTID)
    {

        string flag = MicroPublic.GetMsg("SaveFailed");

        //对传递过来的数据进行数组转换
        JArray iArray = JArray.Parse(Fields);

        //创建DataTable用于存放读取出来的值
        DataTable _dt = new DataTable();
        _dt.Columns.Add("MPID", typeof(int));
        _dt.Columns.Add("JTID", typeof(int));

        //查询角色权限表，与当前选中权限作匹配，已有的权限不作改变，没有的权限取出来，最后插入数据库
        string _sql2 = "select MPID, JTID from JobTitlePermissions ";
        DataTable _dt2 = MsSQLDbHelper.Query(_sql2).Tables[0];

        //同时创建sb用于存放已选中的权限的MPID,构造成逗号分隔的字符串，最后用 MPID not in进行删除数据数已有的记录而选中的权限不包含的记录 
        StringBuilder SB = new StringBuilder();

        int Num = 0;
        //通过for循环进行读取数据
        for (int i = 0; i < iArray.Count; i++)
        {

            if (!string.IsNullOrEmpty(iArray[i]["field"].ToString()))
            {
                int MPID = iArray[i]["field"].toInt();

                //当数据库没有记录时再作插入操作
                DataRow[] _rows2 = _dt2.Select("MPID=" + MPID + " and JTID=" + JTID.toInt());
                if (_rows2.Length == 0)
                {
                    DataRow _dr = _dt.NewRow();
                    _dr["MPID"] = MPID;
                    _dr["JTID"] = JTID.toInt();
                    _dt.Rows.Add(_dr);
                }

                SB.Append(MPID + ",");
            }

            //子节点不为空时调用子节点方法
            if (iArray[i]["children"] != null)
            {
                JArray jArray = JArray.Parse(iArray[i]["children"].ToString());
                if (jArray.Count > 0)
                    GetSubCheckedData(jArray, _dt, _dt2, JTID, SB);
            }
            Num = Num + 1;
        }

        //通过Num判断是否有读取到记录，有记录的话写进数据库
        if (Num > 0)
        {
            //最终判断生成的_dt不为null时写入数据库
            if (_dt != null && _dt.Rows.Count > 0)
                MsSQLDbHelper.SqlBulkCopyInsert(_dt, "JobTitlePermissions");

            //构造好的MPID进行 not in删除数据数已有的记录而选中的权限不包含的记录 
            string MPID = SB.ToString();
            if (MPID.Length > 0)
            {
                MPID = MPID.Substring(0, MPID.Length - 1);

                string _sqlDel = "delete JobTitlePermissions where JTID=@JTID and MPID not in (" + MPID + ")";
                SqlParameter[] _spDel = { new SqlParameter("@JTID", SqlDbType.Int) };
                _spDel[0].Value = JTID.toInt();

                //执行删除操作
                MsSQLDbHelper.ExecuteSql(_sqlDel, _spDel);
            }

            flag = MicroPublic.GetMsg("Save");

        }

        return flag;

    }

    /// <summary>
    /// 获取子节点选中的数据
    /// </summary>
    /// <param name="jArray"></param>
    /// <param name="_dt"></param>
    /// <param name="JTID"></param>
    private void GetSubCheckedData(JArray jArray, DataTable _dt, DataTable _dt2, string JTID, StringBuilder SB)
    {

        for (int j = 0; j < jArray.Count; j++)
        {
            if (!string.IsNullOrEmpty(jArray[j]["field"].ToString()))
            {
                int MPID = jArray[j]["field"].toInt();

                //当数据库没有记录时再作插入操作
                DataRow[] _rows2 = _dt2.Select("MPID=" + MPID + " and JTID=" + JTID.toInt());
                if (_rows2.Length == 0)
                {
                    DataRow _dr = _dt.NewRow();
                    _dr["MPID"] = MPID;
                    _dr["JTID"] = JTID.toInt();
                    _dt.Rows.Add(_dr);
                }

                SB.Append(MPID + ",");
            }
            //递归判断是否有子节点
            if (jArray[j]["children"] != null)
            {
                JArray jArray2 = JArray.Parse(jArray[j]["children"].ToString());
                if (jArray2.Count > 0)
                    GetSubCheckedData(jArray2, _dt, _dt2, JTID, SB);
            }
        }

    }

    /// <summary>
    /// 修改权限名称别名
    /// </summary>
    /// <param name="Fields"></param>
    /// <returns></returns>
    private string ModifyAlias(string Fields)
    {
        string flag = MicroPublic.GetMsg("ModifyFailed"), MPID = string.Empty, Alias = string.Empty;
        dynamic json = JToken.Parse(Fields) as dynamic;
        MPID = json["field"];
        Alias = json["title"];
        if (!string.IsNullOrEmpty(MPID) && !string.IsNullOrEmpty(Alias))
        {
            string _sql = "update ModulePermissions set Alias = @Alias where MPID = @MPID";
            SqlParameter[] _sp = {
             new SqlParameter("@Alias", SqlDbType.NVarChar,50),
             new SqlParameter("@MPID",SqlDbType.Int)
            };

            _sp[0].Value = Alias.toStringTrim();
            _sp[1].Value = MPID.toInt();

            if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
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