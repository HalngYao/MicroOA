using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MicroPublicHelper;
using MicroAuthHelper;

public partial class Views_Forms_ChangeApproval : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string Action = MicroPublic.GetFriendlyUrlParm(0),
            ShortTableName = MicroPublic.GetFriendlyUrlParm(1),
            ModuleID = MicroPublic.GetFriendlyUrlParm(2),
            WFID = MicroPublic.GetFriendlyUrlParm(3),
            ApprovalType = MicroPublic.GetFriendlyUrlParm(4),
            FieldName = MicroPublic.GetFriendlyUrlParm(5),
            DefaultValue = MicroPublic.GetFriendlyUrlParm(6);

        txtMID.Value = ModuleID;
        txtWFID.Value = WFID;
        txtType.Value = ApprovalType;
        txtFieldName.Value = FieldName;
        txtDefaultValue.Value = DefaultValue;


        if (!MicroAuth.CheckPermit(ModuleID, "3"))
        {
            btnModify.Disabled = true;
            btnModify.Attributes.Add("class", "layui-btn layui-btn-disabled");
        }

    }
}