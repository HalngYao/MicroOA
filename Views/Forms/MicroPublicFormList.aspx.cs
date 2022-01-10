using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using MicroDTHelper;
using MicroPublicHelper;

public partial class Views_Forms_MicroPublicFormList : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string Action = MicroPublic.GetFriendlyUrlParm(0);
        txtAction.Value = Action;
        string Type = MicroPublic.GetFriendlyUrlParm(1);
        txtType.Value = Type;
        string ModuleID = MicroPublic.GetFriendlyUrlParm(2);
        txtMID.Value = ModuleID;
    }
}