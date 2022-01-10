using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MicroPublicHelper;

public partial class Views_Set_NavigationForm : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
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
            Action = Action.ToLower();
            if (Action == "modify" || Action == "view")
                microForm.PrimaryKeyValue = MicroPublic.GetFriendlyUrlParm(3);
        }

    }
}