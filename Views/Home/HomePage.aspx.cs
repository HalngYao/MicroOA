using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MicroPublicHelper;
using MicroAuthHelper;

public partial class Views_Home_HomePage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string ShortTableName = MicroPublic.GetFriendlyUrlParm(0);
        string ModuleID = MicroPublic.GetFriendlyUrlParm(1);

        //检查是否已经登录和页面唯一识别是否一致（ShortTableName）
        MicroAuth.CheckAuth(ModuleID, ShortTableName);

        //检查是否有页面浏览权限
        MicroAuth.CheckBrowse(ModuleID);
    }
}