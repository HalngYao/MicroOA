using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MicroFormHelper;
using MicroPublicHelper;
using MicroAuthHelper;

public partial class Views_Forms_SysFormList : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // 动作Action 可选值Add、Modify、View
        string Action = MicroPublic.GetFriendlyUrlParm(0);

        string ShortTableName = MicroPublic.GetFriendlyUrlParm(1);
        txtShortTableName.Value = ShortTableName;

        string ModuleID = MicroPublic.GetFriendlyUrlParm(2);
        txtMID.Value = ModuleID;

        //得到主键名称
        var GetTableAttr = MicroDTHelper.MicroDataTable.GetTableAttr(MicroPublic.GetTableName(ShortTableName));
        txtPrimaryKeyName.Value = "data." + GetTableAttr.PrimaryKeyName;

        divScript.InnerHtml = MicroForm.GetLayCheckBoxTpl(ShortTableName, ModuleID);  //例FormAppType

        MicroAuth.CheckBrowse(ModuleID);

        if (!MicroAuth.CheckPermit(ModuleID, "2"))
        {
            btnAddOpenLink.Disabled = true;
            btnAddOpenLink.Attributes.Add("class", "layui-btn layui-btn-disabled");
        }

        if (!MicroAuth.CheckPermit(ModuleID, "3"))
        {
            linkModify.Attributes.Add("class", "layui-btn layui-btn-xs layui-btn-disabled");
            linkModify.Attributes.Remove("lay-event");
        }

        if (!MicroAuth.CheckPermit(ModuleID, "4"))
        {
            btnDel.Disabled = true;
            btnDel.Attributes.Add("class", "layui-btn layui-btn-disabled");
        }
    }
}