using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Views_UserCenter_SyncDomainUsers : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        txtMID.Value = MicroPublicHelper.MicroPublic.GetFriendlyUrlParm(0);
    }
}