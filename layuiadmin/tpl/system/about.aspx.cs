using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class layuiadmin_tpl_system_about : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected string GetTitle()
    {
        return MicroPublicHelper.MicroPublic.GetMicroInfo("Title");
    }

    protected string GetFoot()
    {
        return MicroPublicHelper.MicroPublic.GetMicroInfo("Foot");
    }
}