using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using MicroUserHelper;
using MicroPublicHelper;
using MicroDBHelper;

public partial class Views_Default : System.Web.UI.Page
{
    public string Notice = string.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            hidCheckBrowser.Value = MicroPublic.CheckBrowser().ToString();
            hidUnsupportedBrowserTips.Value = HttpUtility.UrlEncode(MicroPublic.GetMicroInfo("UnsupportedBrowserTips"));

            MicroPublic.SetSysLog();

            //*****确认是否登录*****
            if (!((MicroUserInfo)Session["UserInfo"]).GetIsLogin())
            {
                string Url = Request.QueryString["url"].toStringTrim();
                if (string.IsNullOrEmpty(Url))
                    Response.Redirect("~/Views/UserCenter/Login");  //Server.UrlEncode(Request.Url.ToString())
                else
                    Response.Redirect("~/Views/UserCenter/Login?url=" + Server.UrlEncode(Url));  //Server.UrlEncode(Request.Url.ToString())
            }
            else
            {
                string UID = MicroUserInfo.GetUserInfo("UID");
                //通知提示
                string _sql = "select * from Notice where Invalid=0 and Del=0 and IsRead=0 and UID=@UID";

                SqlParameter[] _sp = { new SqlParameter("@UID", SqlDbType.Int) };
                _sp[0].Value = UID.toInt();

                DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

                Boolean IsDot = MicroPublic.GetMicroInfo("IsCenterMsgDot").toBoolean(); //true值显示圆点，false值时显示提示加数字

                if (_dt.Rows.Count > 0)
                {
                    string Record = _dt.Rows.Count > 100 ? "99+" : _dt.Rows.Count.ToString();
                    //提示紧圆点
                    if (IsDot)
                        Notice = "<span class=\"layui-badge-dot\"></span>";
                    else  //提示加数字
                        Notice = "<span class=\"layui-badge\">" + Record + "</span>";
                }

                //在线状态
                string _sql2 = "select * from UserState where Invalid=0 and Del=0 and UID=" + UID;
                DataTable _dt2 = MsSQLDbHelper.Query(_sql2).Tables[0];

                if (_dt2 != null && _dt2.Rows.Count > 0)
                {
                    string StateCode = _dt2.Rows[0]["StateCode"].toStringTrim(),
                        StateName = _dt2.Rows[0]["StateName"].toStringTrim();

                    if (StateCode == "100")
                    {
                        spanSetate.Attributes.Add("class", "layui-badge-dot ws-bg-green");
                        //spanSetate.Attributes.Add("class", "ws-bg-green");
                    }
                    else
                    {
                        spanSetate.Attributes.Remove("class");
                        spanSetate.Attributes.Add("class", "layui-badge-dot");
                    }

                    citeState.InnerText = StateName;
                }

            }

        }
        catch { }
        string Avatar = ((MicroUserInfo)Session["UserInfo"]).Avatar;
        if (!string.IsNullOrEmpty(Avatar))
            imgAvatar.ImageUrl = Avatar;

        //*****全局提示Start*****
        //先判断是否开启了全局提示，再检查是否有提示记录，有的话再弹窗提示
        Boolean GlobalTips = MicroUserInfo.GetUserInfo("IsGlobalTips").toBoolean();
        if (GlobalTips)
        {
            string _sql = "select * from Information where GlobalTips=1 and GlobalTipsTime >= '" + DateTime.Now.ToString("yyyy-MM-dd") + "'";
            DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

            if (_dt.Rows.Count > 0)
                hidGlobalTips.Value = GlobalTips.ToString();
        }
        //*****全局提示End*****
    }

    protected string TopMenu()
    {
        return MicroPublic.GetTopMenu("OA");
    }

    protected string LeftMenu()
    {
        return MicroPublic.GetMainMenu();
    }

    protected string GetDisplayName()
    {
        return ((MicroUserInfo)Session["UserInfo"]).DisplayName;
    }

    protected void lbLogOff_Click(object sender, EventArgs e)
    {
        ((MicroUserInfo)Session["UserInfo"]).ResetCookies();
        ((MicroUserInfo)Session["UserInfo"]).LogOff();
        Response.Redirect("~/Views/UserCenter/Login?url=" + Server.UrlEncode(Request.Url.ToString()));
    }

    protected string GetMicroInfo(string Type)
    {
        return MicroPublic.GetMicroInfo(Type);
    }
}