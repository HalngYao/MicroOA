using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MicroUserHelper;


public partial class Views_UserCenter_UserState : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        txtDate.Value = DateTime.Now.ToString("yyyy-MM-dd");
        txtTime.Value = DateTime.Now.AddMinutes(30).ToString("HH:mm");
    }


}