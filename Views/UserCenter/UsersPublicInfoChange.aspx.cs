using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MicroPublicHelper;
using MicroAuthHelper;

public partial class Views_UserCenter_UsersPublicInfoChange : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        string Action = MicroPublic.GetFriendlyUrlParm(0),
            ShortTableName = MicroPublic.GetFriendlyUrlParm(1),
            ModuleID = MicroPublic.GetFriendlyUrlParm(2),
            UIDS = MicroPublic.GetFriendlyUrlParm(3);

        //因为是按钮点击跳转过来，所以用系统编辑权限来进行判断
        if (!MicroAuth.CheckPermit(ModuleID, "3"))
        {
            btnModify.Disabled = true;
            btnModify.Attributes.Add("class", "layui-btn layui-btn-disabled");
        }

        txtMID.Value = ModuleID;
        txtUIDS.Value = UIDS;
        txtType.Value = ShortTableName;

        txtDate.Value = DateTime.Now.toDateFormat();

        //不是操作设定工时制时，隐藏开始日期TextBox
        if (ShortTableName != "UWorkHourSystem")
            divDate.Visible = false;


    }

}