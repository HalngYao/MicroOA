using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using MicroDTHelper;
using MicroFormHelper;
using MicroPublicHelper;
using MicroAuthHelper;
using MicroUserHelper;

public partial class Views_Stats_Attendance_UserOvertime : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //动作Action 可选值Add、Modify、View
        string Action = MicroPublic.GetFriendlyUrlParm(0);
        txtAction.Value = Action;

        string ShortTableName = MicroPublic.GetFriendlyUrlParm(1);
        txtShortTableName.Value = ShortTableName;

        string ModuleID = MicroPublic.GetFriendlyUrlParm(2);
        txtMID.Value = ModuleID;

        string CurrDate = DateTime.Now.toDateFormat();
        txtDateRange.Value = CurrDate.toDateMFirstDay() + " ~ " + CurrDate.toDateMLastDay();

        divScript.InnerHtml = MicroForm.GetLayCheckBoxTpl(ShortTableName, ModuleID);

        //检查是否已经登录和页面唯一识别是否一致（ShortTableName）
        MicroAuth.CheckAuth(ModuleID, ShortTableName);

        //检查是否有页面浏览权限
        MicroAuth.CheckBrowse(ModuleID);

        //if (!MicroAuth.CheckPermit(ModuleID, "2"))
        //{
        //    btnAddOpenLink.Disabled = true;
        //    btnAddOpenLink.Attributes.Add("class", "layui-btn layui-btn-disabled");
        //}

        //if (!MicroAuth.CheckPermit(ModuleID, "3"))
        //{
        //    btnMore.Disabled = true;
        //    btnMore.Attributes.Add("class", "layui-btn layui-btn-disabled");
        //}

        //if (!MicroAuth.CheckPermit(ModuleID, "4"))
        //{
        //    btnDel.Disabled = true;
        //    btnDel.Attributes.Add("class", "layui-btn layui-btn-disabled");
        //}

        if (!MicroAuth.CheckPermit(ModuleID, "11"))
        {
            link1ChangeDept.Attributes.Add("class", "");
            link1ChangeDept.Attributes.Remove("lay-event");

            link2ChangeDept.Attributes.Add("class", "");
            link2ChangeDept.Attributes.Remove("lay-event");


            link1ChangeJobTitle.Attributes.Add("class", "");
            link1ChangeJobTitle.Attributes.Remove("lay-event");

            link2ChangeJobTitle.Attributes.Add("class", "");
            link2ChangeJobTitle.Attributes.Remove("lay-event");


            link1ChangeRoles.Attributes.Add("class", "");
            link1ChangeRoles.Attributes.Remove("lay-event");

            link2ChangeRoles.Attributes.Add("class", "");
            link2ChangeRoles.Attributes.Remove("lay-event");
        }

    }

    protected string GetLeftNav()
    {
        string flag = string.Empty;
        flag = GetDeptNav();

        return flag;
    }

    protected string GetDeptNav()
    {
        string flag = string.Empty, _li = string.Empty;

        string ModuleID = MicroPublic.GetFriendlyUrlParm(2),
            UID = MicroUserInfo.GetUserInfo("UID");

        Boolean ViewPermit = MicroAuth.CheckPermit(ModuleID, "1");
        Boolean AddPermit = MicroAuth.CheckPermit(ModuleID, "2");
        Boolean EditPermit = MicroAuth.CheckPermit(ModuleID, "3");


        try
        {
            string DeptIDs = "0";

            if (MicroAuth.CheckPermit(ModuleID, "6")) //查看所有       
                DeptIDs = MicroUserInfo.GetUserInfo("AllDeptsID");
            else if (MicroAuth.CheckPermit(ModuleID, "19"))  //查看部内，自部门及自部门所有的父部门及所有父部门包含的子部门      
                DeptIDs = MicroUserInfo.GetUserInfo("ParentSubDeptsID");
            else if (MicroAuth.CheckPermit(ModuleID, "20"))  //查看科内，科内及科内所有子部门      
                DeptIDs = MicroUserInfo.GetUserInfo("ParentDeptsID") + "," + MicroUserInfo.GetUserDepts(UID, "DeptID", "SectionDepts", true);
            else if (MicroAuth.CheckPermit(ModuleID, "17"))  //查看自部门含子部门        
                DeptIDs = MicroUserInfo.GetUserInfo("ParentDeptsID") + "," + MicroUserInfo.GetUserInfo("SubDeptsID");  //得到自部门的所有父ID和自部门的所有子ID，便于生成菜单
            else  //只能查看自己时也列出自部门的父部门，便于生成菜单 
                DeptIDs = MicroUserInfo.GetUserInfo("ParentDeptsID") + "," + MicroUserInfo.GetUserInfo("SubDeptsID");  //只能查看自部门时也一样。

            string _sql = "select * from Department where Invalid=0 and Del=0 and DeptID in (" + DeptIDs + ")";
            DataTable _dt = MicroDBHelper.MsSQLDbHelper.Query(_sql).Tables[0];  //得到_dt作为自定义DataTable通过MicroDataTable.GetDataTable方法进行查询记录（该方法的特点如是有父记录的表会追加MainSub等相关属性返回）
            if (_dt != null && _dt.Rows.Count > 0)
                _dt = MicroDataTable.GetDataTable("Dept", true, _dt);

            _li += "<li class=\"layui-nav-item layui-nav-itemed\">";
            _li += "<a href=\"javascript:; \">部门</a>";

            if (AddPermit || EditPermit)
                _li += "<span class=\"micro-click\" micro-text=\"部门管理\" micro-stn=\"Dept\" data-type=\"GetMgr\">管理</span>";

            _li += "<dl class=\"layui-nav-child\">";

            _li += "<dd class=\"layui-this\"><a href=\"javascript:;\" class=\"micro-click\" micro-type=\"Dept\" micro-id=\"All\" data-type=\"GetUsers\"><b style=\"font-size:14px;\">全公司</b></a></dd>";

            if (_dt != null && _dt.Rows.Count > 0)
            {
                DataRow[] _rows = _dt.Select("");
                int i = 0;
                foreach (DataRow _dr in _rows)
                {
                    string MainSub = _dr["MainSub"].toStringTrim() == "Main" ? "" : _dr["MainSub"].toStringTrim() + " ",
                     Name = _dr["DeptName"].toStringTrim(),
                     Name2 = Name,
                     AdDepartment = _dr["AdDepartment"].toStringTrim(),
                     ParentID = _dr["ParentID"].toStringTrim(),
                     ID = _dr["DeptID"].toStringTrim();

                    //Name = _dr["MainSub"].toStringTrim() == "Main" ? "<b style=\"font-size:14px;\">" + Name + "</b>" : MainSub + Name;

                    if (ParentID == "0" || ParentID == "1")
                        Name = "<b style=\"font-size:14px;\">" + MainSub + Name + "</b>";
                    else
                        Name = MainSub + Name;

                    _li += "<dd><a href=\"javascript:;\" title=\"" + AdDepartment + "\" class=\"micro-click\" micro-type=\"Dept\" micro-id=\"" + ID + "\" data-type=\"GetUsers\">" + Name + "</a>";

                    if (EditPermit)
                        _li += "<span class=\"micro-click\" micro-text=\"" + Name2.Replace("├", "").Replace("└ ", "") + "\" micro-form=\"DeptsForm\" micro-stn=\"Dept\" micro-id=\"" + ID + "\" data-type=\"Modify\">修改</span>";

                    _li += "</dd>";

                    i = i + 1;
                }
            }

            if (AddPermit)
                _li += "<dd><a href=\"javascript:;\" class=\"micro-click\" micro-text=\"添加部门\" micro-form=\"DeptsForm\" micro-stn=\"Dept\" data-type=\"Add\" style=\"color:#009688 !important; font-weight:bold;\"><i class=\"layui-icon layui-icon-add-1\"></i> 添加</a></dd>";

            _li += "</dl>";
            _li += "</li>";

            flag = _li;
        }
        catch (Exception ex) { flag = ex.ToString(); }

        return flag;

    }


    protected string GetJobTitleNav()
    {
        string flag = string.Empty, _li = string.Empty;
        string ModuleID = MicroPublic.GetFriendlyUrlParm(2);

        Boolean ViewPermit = MicroAuth.CheckPermit(ModuleID, "1");
        Boolean AddPermit = MicroAuth.CheckPermit(ModuleID, "2");
        Boolean EditPermit = MicroAuth.CheckPermit(ModuleID, "3");

        try
        {
            DataTable _dt = MicroDataTable.GetDataTable("JTitle");

            _li += "<li class=\"layui-nav-item layui-nav-itemed\">";
            _li += "<a href=\"javascript:; \">职位职称</a>";

            if (ViewPermit)
                _li += "<span class=\"micro-click\" micro-text=\"职位、职称管理\" micro-stn=\"JTitle\" data-type=\"GetMgr\">管理</span>";

            _li += "<dl class=\"layui-nav-child\">";

            if (_dt != null && _dt.Rows.Count > 0)
            {
                DataRow[] _rows = _dt.Select("");

                foreach (DataRow _dr in _rows)
                {
                    string Name = _dr["JobTitleName"].toStringTrim();
                    string ID = _dr["JTID"].toStringTrim();

                    _li += "<dd><a href=\"javascript:;\" class=\"micro-click\" micro-type=\"JTitle\" micro-id=\"" + ID + "\" data-type=\"GetUsers\">" + Name + "</a>";

                    if (EditPermit)
                        _li += "<span class=\"micro-click\" micro-text=\"" + Name.Replace("├", "").Replace("└ ", "") + "\" micro-form=\"JobTitleForm\" micro-stn=\"JTitle\" micro-id=\"" + ID + "\" data-type=\"Modify\">修改</span>";

                    _li += " </dd>";
                }
            }

            if (AddPermit)
                _li += "<dd><a href=\"javascript:;\" class=\"micro-click\" micro-text=\"添加职位、职称\" micro-form=\"JobTitleForm\" micro-stn=\"JTitle\" data-type=\"Add\" style=\"color:#009688 !important; font-weight:bold;\"><i class=\"layui-icon layui-icon-add-1\"></i> 添加</a></dd>";


            _li += "</dl>";
            _li += "</li>";

            flag = _li;
        }
        catch (Exception ex) { flag = ex.ToString(); }
        return flag;

    }


    protected string GetRolesNav()
    {
        string flag = string.Empty, _li = string.Empty;
        DataTable _dt = MicroDataTable.GetDataTable("Role");
        string ModuleID = MicroPublic.GetFriendlyUrlParm(2);

        Boolean ViewPermit = MicroAuth.CheckPermit(ModuleID, "1");
        Boolean AddPermit = MicroAuth.CheckPermit(ModuleID, "2");
        Boolean EditPermit = MicroAuth.CheckPermit(ModuleID, "3");

        _li += "<li class=\"layui-nav-item layui-nav-itemed\">";
        _li += "<a href=\"javascript:; \">系统角色</a>";

        if (ViewPermit)
            _li += "<span class=\"micro-click\" micro-text=\"角色管理\" micro-stn=\"Role\" data-type=\"GetMgr\">管理</span>";

        _li += "<dl class=\"layui-nav-child\">";

        if (_dt != null && _dt.Rows.Count > 0)
        {
            DataRow[] _rows = _dt.Select("");

            foreach (DataRow _dr in _rows)
            {
                string Name = _dr["RoleName"].toStringTrim();
                string ID = _dr["RID"].toStringTrim();

                _li += "<dd><a href=\"javascript:;\" class=\"micro-click\" micro-type=\"Role\" micro-id=\"" + ID + "\" data-type=\"GetUsers\">" + Name + "</a>";

                if (EditPermit)
                    _li += "<span class=\"micro-click\" micro-text=\"" + Name.Replace("├", "").Replace("└ ", "") + "\" micro-form=\"RolesForm\" micro-stn=\"Role\" micro-id=\"" + ID + "\" data-type=\"Modify\">修改</span>";

                _li += "</dd>";
            }
        }

        if (AddPermit)
            _li += "<dd><a href=\"javascript:;\" class=\"micro-click\" micro-text=\"添加角色\" micro-form=\"RolesForm\" micro-stn=\"Role\" data-type=\"Add\" style=\"color:#009688 !important; font-weight:bold;\"><i class=\"layui-icon layui-icon-add-1\"></i> 添加</a></dd>";

        _li += "</dl>";
        _li += "</li>";

        flag = _li;
        return flag;

    }
}