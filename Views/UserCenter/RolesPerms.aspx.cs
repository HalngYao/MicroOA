using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using MicroPublicHelper;
using MicroAuthHelper;

public partial class Views_UserCenter_RolesPerms : System.Web.UI.Page
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

        //检查是否已经登录和页面唯一识别是否一致（ShortTableName）
        MicroAuth.CheckAuth(ModuleID, ShortTableName);

        //检查是否有页面浏览权限
        MicroAuth.CheckBrowse(ModuleID);

        if (!MicroAuth.CheckPermit(ModuleID, "3"))
        {
            btnSave.Disabled = true;
            btnSave.Attributes.Add("class", "layui-btn layui-btn-sm layui-btn-disabled");

            btnOpenLink.Disabled = true;
            btnOpenLink.Attributes.Add("class", "layui-btn layui-btn-sm layui-btn-disabled");

        }
        else
        {
            txtEdit.Value = "True";
        }

    }

    protected string GetLeftNav()
    {
        string flag = string.Empty;
        flag = GetRolesNav() + GetJobTitleNav();

        return flag;
    }

    protected string GetRolesNav()
    {
        string flag = string.Empty, _li = string.Empty;
        string ModuleID = MicroPublic.GetFriendlyUrlParm(2);

        Boolean ViewPermit = MicroAuth.CheckPermit(ModuleID, "1");
        Boolean AddPermit = MicroAuth.CheckPermit(ModuleID, "2");
        Boolean EditPermit = MicroAuth.CheckPermit(ModuleID, "3");

        try
        {
            DataTable _dt = MicroDTHelper.MicroDataTable.GetDataTable("Role");

            _li += "<li class=\"layui-nav-item layui-nav-itemed\">";
            _li += "<a href=\"javascript:; \">系统角色</a>";

            if (ViewPermit)
                _li += "<span class=\"micro-click\" micro-text=\"角色管理\" micro-stn=\"Role\" micro-pagename=\"Roles\" data-type=\"GetMgr\">管理</span>";

            _li += "<dl class=\"layui-nav-child\">";

            if (_dt != null && _dt.Rows.Count > 0)
            {
                DataRow[] _rows = _dt.Select("");

                foreach (DataRow _dr in _rows)
                {
                    string Name = _dr["RoleName"].toStringTrim();
                    string ID = _dr["RID"].toStringTrim();

                    _li += "<dd><a href=\"javascript:;\" class=\"micro-click\" micro-text=\"" + Name + "\" micro-pagename=\"Role\" micro-id=\"" + ID + "\" data-type=\"GetPerms\">" + Name + "</a>";

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
            DataTable _dt = MicroDTHelper.MicroDataTable.GetDataTable("JTitle");

            _li += "<li class=\"layui-nav-item layui-nav-itemed\">";
            _li += "<a href=\"javascript:; \">职位职称</a>";

            if (ViewPermit)
                _li += "<span class=\"micro-click\" micro-text=\"职位、职称管理\" micro-stn=\"JTitle\" micro-pagename=\"JobTitle\" data-type=\"GetMgr\">管理</span>";

            _li += "<dl class=\"layui-nav-child\">";

            if (_dt != null && _dt.Rows.Count > 0)
            {
                DataRow[] _rows = _dt.Select("");

                foreach (DataRow _dr in _rows)
                {
                    string Name = _dr["JobTitleName"].toStringTrim();
                    string ID = _dr["JTID"].toStringTrim();

                    _li += "<dd><a href=\"javascript:;\" class=\"micro-click\" micro-text=\"" + Name + "\" micro-pagename=\"JobTitle\" micro-id=\"" + ID + "\" data-type=\"GetPerms\">" + Name + "</a>";

                    if (EditPermit)
                        _li += "<span class=\"micro-click\" micro-text=\"" + Name.Replace("├", "").Replace("└ ", "") + "\" micro-form=\"JobTitleForm\" micro-stn=\"JTitle\" micro-id=\"" + ID + "\" data-type=\"Modify\">修改</span>";

                    _li += "</dd>";
                }
            }

            if (AddPermit)
                _li += "<dd><a href=\"javascript:;\" class=\"micro-click\" micro-text=\"添加职位、职称\" micro-form=\"JobTitleForm\" micro-stn=\"JTitle\" data-type=\"Add\" style=\"color:#009688 !important; font-weight:bold;\"><i class=\"layui-icon layui-icon-add-1\"></i> 添加</a></dd>";

            _li += "</dl>";
            _li += "</li>";

            flag = _li;
        }
        catch (Exception ex){ flag = ex.ToString(); }
        return flag;

    }
}