using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using MicroFormHelper;
using MicroPublicHelper;
using MicroAuthHelper;


public partial class Views_Forms_Forms : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //动作Action 可选值Add、Modify、View
        string Action = MicroPublic.GetFriendlyUrlParm(0);

        string ShortTableName = MicroPublic.GetFriendlyUrlParm(1);
        string ModuleID = MicroPublic.GetFriendlyUrlParm(2);
        txtMID.Value = ModuleID;
        divScript.InnerHtml = MicroForm.GetLayCheckBoxTpl(ShortTableName, ModuleID);

        //检查是否已经登录和页面唯一识别是否一致（ShortTableName）
        MicroAuth.CheckAuth(ModuleID, ShortTableName);
        
        //检查是否有页面浏览权限
        MicroAuth.CheckBrowse(ModuleID);

        if (!MicroAuth.CheckPermit(ModuleID, "2"))
        {
            btnAddOpenLink.Disabled = true;
            btnAddOpenLink.Attributes.Add("class", "layui-btn layui-btn-disabled");
        }

        if (!MicroAuth.CheckPermit(ModuleID, "3"))
        {
            linkModify.Visible = false;
            linkModify.Attributes.Add("class", "layui-btn layui-btn-xs layui-btn-disabled");
            linkModify.Attributes.Remove("lay-event");
        }

        if (!MicroAuth.CheckPermit(ModuleID, "4"))
        {
            btnDel.Disabled = true;
            btnDel.Attributes.Add("class", "layui-btn layui-btn-disabled");
        }

        if (!MicroAuth.CheckPermit(ModuleID, "5"))
        {
            linkView.Attributes.Add("class", "layui-btn layui-btn-xs layui-btn-disabled");
            linkView.Attributes.Remove("lay-event");
        }

        if (!MicroAuth.CheckPermit(ModuleID, "8"))
        {
            linkAdd.Attributes.Add("class", "layui-btn layui-btn-xs layui-btn-disabled");
            linkAdd.Attributes.Remove("lay-event");
        }

    }

    protected string GetLeftNav()
    {
        string flag = string.Empty;
        flag = GetFormClassNav(); // + GetDeptNav(); //按部门的申请暂时不需要，注释掉

        return flag;
    }


    protected string GetFormClassNav()
    {
        string flag = string.Empty, _li = string.Empty;
        DataTable _dt = MicroDTHelper.MicroDataTable.GetDataTable("FormClass");
        string ModuleID = MicroPublic.GetFriendlyUrlParm(2);

        Boolean ViewPermit = MicroAuth.CheckPermit(ModuleID, "1");
        Boolean AddPermit = MicroAuth.CheckPermit(ModuleID, "2");
        Boolean EditPermit = MicroAuth.CheckPermit(ModuleID, "3");

        _li += "<li class=\"layui-nav-item layui-nav-itemed\">";
        _li += "<a href=\"javascript:; \">按分类 / <span class=\"ws-font-size8\">分類</span></a>";

        if (AddPermit || EditPermit)
            _li += "<span class=\"micro-click\" micro-text=\"表单分类管理\" micro-stn=\"FormClass\" data-type=\"GetMgr\">管理</span>";

        _li += "<dl class=\"layui-nav-child\">";
        //_li += "<dd class=\"layui-this\"><a href=\"javascript:;\" class=\"micro-click\" micro-type=\"All\" micro-data=\"\" data-type=\"GetForms\"><b>全部</b></a></dd>";
        if (_dt != null && _dt.Rows.Count > 0)
        {
            DataRow[] _rows = _dt.Select("");

            int i = 0;
            foreach (DataRow _dr in _rows)
            {
                string MainSub = _dr["MainSub"].toStringTrim() == "Main" ? "" : _dr["MainSub"].toStringTrim() + " ";
                string Name = _dr["ClassName"].toMenuSplit();
                Name = _dr["MainSub"].toStringTrim() == "Main" ? "<b>" + Name + "</b>" : MainSub + Name;
                string ID = _dr["FCID"].toStringTrim();

                string LayuiThis = "ws-margin-left20";
                string MicroType = "FCID:Int";

                if (i == 0)
                {
                    ID = "";
                    LayuiThis = "layui-this";
                    MicroType = "All";
                }

                _li += "<dd class=\"" + LayuiThis + "\">";
                _li += "<a href=\"javascript:;\" class=\"micro-click\" micro-type=\"" + MicroType + "\" micro-data=\"" + ID + "\" data-type=\"GetForms\">" + Name + "</a>";

                if (EditPermit)
                    _li += "<span class=\"micro-click\" micro-text=\"" + _dr["ClassName"].toStringTrim().Replace("├", "").Replace("└ ", "") + "\" micro-stn=\"FormClass\" micro-id=\"" + _dr["FCID"].toStringTrim() + "\" data-type=\"Modify\">修改</span>";

                _li += "</dd>";

                i = i + 1;
            }
        }

        if (AddPermit)
            _li += "<dd><a href=\"javascript:;\" class=\"micro-click\" micro-text=\"添加分类\" micro-stn=\"FormClass\" data-type=\"Add\" style=\"color:#009688 !important; font-weight:bold;\"><i class=\"layui-icon layui-icon-add-1\"></i> 添加</a></dd>";

        _li += "</dl>";
        _li += "</li>";

        flag = _li;
        return flag;

    }


    protected string GetDeptNav()
    {
        string flag = string.Empty, _li = string.Empty;
        DataTable _dt = MicroDTHelper.MicroDataTable.GetDataTable("Dept");
        string ModuleID = MicroPublic.GetFriendlyUrlParm(2);

        Boolean ViewPermit = MicroAuth.CheckPermit(ModuleID, "1");
        Boolean AddPermit = MicroAuth.CheckPermit(ModuleID, "2");
        Boolean EditPermit = MicroAuth.CheckPermit(ModuleID, "3");

        _li += "<li class=\"layui-nav-item layui-nav-itemed\">";
        _li += "<a href=\"javascript:; \">按部门</a>";

        if (ViewPermit)
            _li += "<span class=\"micro-click\" micro-text=\"部门管理\" micro-stn=\"Dept\" data-type=\"GetMgr\">管理</span>";

        _li += "<dl class=\"layui-nav-child\">";
        _li += "<dd class=\"layui-this\"><a href=\"javascript:;\" class=\"micro-click\" micro-type=\"All\" micro-data=\"\" data-type=\"GetForms\"><b>全公司</b></a></dd>";
        if (_dt != null && _dt.Rows.Count > 0)
        {
            DataRow[] _rows = _dt.Select("");

            foreach (DataRow _dr in _rows)
            {
                string MainSub = _dr["MainSub"].toStringTrim() == "Main" ? "" : _dr["MainSub"].toStringTrim() + " ";
                string Name = _dr["DeptName"].toStringTrim();
                Name = _dr["MainSub"].toStringTrim() == "Main" ? "<b>" + Name + "</b>" : MainSub + Name;
                string ID = _dr["DeptID"].toStringTrim();

                _li += "<dd>";
                _li += "<a href=\"javascript:;\" class=\"micro-click\" micro-type=\"DeptID:Int\" micro-data=\"" + ID + "\" data-type=\"GetForms\">" + Name + "</a>";

                if (EditPermit)
                    _li += "<span class=\"micro-click\" micro-text=\"" + Name.Replace("├", "").Replace("└ ", "") + "\" micro-stn=\"Dept\" micro-id=\"" + ID + "\" data-type=\"Modify\">修改</span>";

                _li += "</dd>";
            }
        }

        if (AddPermit)
            _li += "<dd><a href=\"javascript:;\" class=\"micro-click\" micro-text=\"添加部门\" micro-stn=\"Dept\" data-type=\"Add\" style=\"color:#009688 !important; font-weight:bold;\"><i class=\"layui-icon layui-icon-add-1\"></i> 添加</a></dd>";

        _li += "</dl>";
        _li += "</li>";

        flag = _li;
        return flag;

    }

}