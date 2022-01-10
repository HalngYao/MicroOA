using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MicroUserHelper;


public partial class Views_UserCenter_UserPassword : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        if (MicroUserInfo.GetUserInfo("FirstLogin").toBoolean())
        {
            divUserPassword.Visible = false;
            txtUserPassword.Visible = false;
        }
    }


}