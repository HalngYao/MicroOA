using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroDTHelper;
using MicroUserHelper;
using MicroPublicHelper;

public partial class Views_Forms_HR_OnDutyTpl : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        UserDepts.Value = MicroUserInfo.GetUserInfo("UserDepts");
        txtStartDate.Value = MicroPublic.GetYearMonthDay("CurrWeekFirstDay", DateTime.Now.AddDays(7).ToString("yyyy-MM-dd"));
        txtEndDate.Value = MicroPublic.GetYearMonthDay("CurrWeekLastDay", DateTime.Now.AddDays(7).ToString("yyyy-MM-dd")).toDateFormat("yyyy-MM-dd");

    }

}