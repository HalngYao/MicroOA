using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MicroPublicHelper;
using MicroUserHelper;
using MicroAuthHelper;

public partial class Views_UserCenter_UserPublicInfoChange : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        string Action = MicroPublic.GetFriendlyUrlParm(0),
            ShortTableName = MicroPublic.GetFriendlyUrlParm(1),
            ModuleID = MicroPublic.GetFriendlyUrlParm(2),
            UID = MicroPublic.GetFriendlyUrlParm(3);

        txtType.Value = ShortTableName;
        txtMID.Value = ModuleID;
        txtUID.Value = UID;

        //因为是从表格内容点击跳转过来，所以用编辑表格权限来进行判断
        if (!MicroAuth.CheckPermit(ModuleID, "11"))
        {
            btnModify.Disabled = true;
            btnModify.Attributes.Add("class", "layui-btn layui-btn-disabled");
        }


        if (!string.IsNullOrEmpty(ShortTableName) && !string.IsNullOrEmpty(UID))
        {
            string FieldName = string.Empty;
            switch (ShortTableName.ToLower())
            {
                case "udepts":
                    FieldName = "DeptID";
                    break;
                case "ujtitle":
                    FieldName = "JTID";
                    break;
                case "uroles":
                    FieldName = "RID";
                    break;
                case "uworkhoursystem":
                    FieldName = "WorkHourSysID";
                    break;
            }
            txtDefaultValue.Value = MicroUserInfo.GetUserInfo(UID, ShortTableName, FieldName);

            txtDate.Value = DateTime.Now.toDateFormat();

            //不是操作设定工时制时，隐藏开始日期TextBox
            if (ShortTableName != "UWorkHourSystem")
                divDate.Visible = false;
        }

    }

}