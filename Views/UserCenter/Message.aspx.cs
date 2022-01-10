using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using MicroUserHelper;
using MicroDBHelper;

public partial class Views_UserCenter_Message : System.Web.UI.Page
{
    public string Notice = string.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {

        string UID = MicroUserInfo.GetUserInfo("UID");
        txtPrimaryKeyName.Value = UID;
        string _sql = "select * from Notice where Invalid=0 and Del=0 and IsRead=0 and UID=@UID";

        SqlParameter[] _sp = { new SqlParameter("@UID", SqlDbType.Int) };
        _sp[0].Value = UID.toInt();

        DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

        if (_dt.Rows.Count > 0)
            Notice = "<span class=\"layui-badge\">" + _dt.Rows.Count + "</span>";
    }
}