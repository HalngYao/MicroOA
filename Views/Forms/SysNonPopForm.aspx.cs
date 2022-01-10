using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MicroPublicHelper;
using MicroAuthHelper;

public partial class Views_Forms_SysNonPopForm : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //该表单：不是由页面点击弹窗出来的表单，直接在选项卡上显示的
        try
        {
            //动作Action 可选值Add、Modify、View
            string Action = MicroPublic.GetFriendlyUrlParm(0);
            microForm.Action = Action;

            string ShortTableName = MicroPublic.GetFriendlyUrlParm(1);
            microForm.ShortTableName = ShortTableName;

            string ModuleID = MicroPublic.GetFriendlyUrlParm(2);
            microForm.ModuleID = ModuleID;
            txtMID.Value = ModuleID;

            if (!string.IsNullOrEmpty(Action))
            {
                if (Action.ToLower() == "modify" || Action.ToLower() == "view")
                    microForm.PrimaryKeyValue = MicroPublic.GetFriendlyUrlParm(3);
            }

            //根据传入的ShortTableName重新获取一下ModuleID，因为默认传入的MoluleID有可能从其它页面传递过来（如在InfoList页面点击编辑过来）
            ModuleID = MicroPublic.GetModuleID(ShortTableName);

            //检查是否有页面浏览权限
            MicroAuth.CheckBrowse(ModuleID);

            //检查是否已经登录和页面唯一识别是否一致（ShortTableName）
            MicroAuth.CheckAuth(ModuleID, ShortTableName);

            
        }
        catch { }
    }
}