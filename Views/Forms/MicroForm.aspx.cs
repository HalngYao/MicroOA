using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MicroPublicHelper;

public partial class Views_Forms_MicroForm : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
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

            string FormID = MicroPublic.GetFriendlyUrlParm(3);
            microForm.FormID = FormID;
            txtFormID.Value = FormID;

            if (!string.IsNullOrEmpty(Action))
            {
                Action = Action.ToLower();

                if (Action == "modify" || Action == "view" || Action == "close")
                    microForm.PrimaryKeyValue = MicroPublic.GetFriendlyUrlParm(4);
            }

        }
        catch { }

    }
}