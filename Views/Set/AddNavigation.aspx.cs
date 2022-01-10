using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MicroPublicHelper;

public partial class Views_Set_AddNavigation : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        txtMID.Value = MicroPublic.GetFriendlyUrlParm(0);
    }
}