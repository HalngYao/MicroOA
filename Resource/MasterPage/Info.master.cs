using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Security;
using System.Web.UI.WebControls;
using MicroAuthHelper;
using MicroPublicHelper;
using MicroUserHelper;
using MicroDBHelper;

public partial class Resource_MasterPage_Info : System.Web.UI.MasterPage
{

    private const string AntiXsrfTokenKey = "__AntiXsrfToken";
    private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
    private string _antiXsrfTokenValue;


    protected void Page_Init(object sender, EventArgs e)
    {

        // 以下代码可帮助防御 XSRF 攻击
        var requestCookie = Request.Cookies[AntiXsrfTokenKey];
        Guid requestCookieGuidValue;
        if (requestCookie != null && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
        {
            // 使用 Cookie 中的 Anti-XSRF 令牌
            _antiXsrfTokenValue = requestCookie.Value;
            Page.ViewStateUserKey = _antiXsrfTokenValue;
        }
        else
        {
            // 生成新的 Anti-XSRF 令牌并保存到 Cookie
            _antiXsrfTokenValue = Guid.NewGuid().ToString("N");
            Page.ViewStateUserKey = _antiXsrfTokenValue;

            var responseCookie = new HttpCookie(AntiXsrfTokenKey)
            {
                HttpOnly = true,
                Value = _antiXsrfTokenValue
            };
            if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
            {
                responseCookie.Secure = true;
            }
            Response.Cookies.Set(responseCookie);
        }

        Page.PreLoad += master_Page_PreLoad;


        try
        {
            //检查站点访问控制
            MicroPublic.CheckWebSiteAccessControl();

            //if (!((MicroUserInfo)Session["UserInfo"]).GetIsLogin())
            if (!MicroAuth.CheckIsLogin())  //*****确认是否登录*****
            {
                Response.Redirect("~/Views/UserCenter/Login?url=" + Server.UrlEncode(Request.Url.ToString()));  //Server.UrlEncode(Request.Url.ToString())
                Response.End();
            }
            // MicroAuth.CheckLogin();
        }
        catch (Exception ex) { Response.Write(ex.ToString()); }

    }

    void master_Page_PreLoad(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // 设置 Anti-XSRF 令牌
            ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;
            ViewState[AntiXsrfUserNameKey] = Context.User.Identity.Name ?? String.Empty;
        }
        else
        {
            // 验证 Anti-XSRF 令牌
            if ((string)ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue
                || (string)ViewState[AntiXsrfUserNameKey] != (Context.User.Identity.Name ?? String.Empty))
            {
                throw new InvalidOperationException("Anti-XSRF 令牌验证失败。");
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        string Avatar = ((MicroUserInfo)Session["UserInfo"]).Avatar;
        if (!string.IsNullOrEmpty(Avatar))
            imgAvatar.ImageUrl = Avatar;

        //设置日志
        MicroPublic.SetSysLog();

        //设置水印
        Boolean WaterMarkForInfo = MicroPublic.GetMicroInfo("WaterMarkForInfo").toBoolean();
        if (WaterMarkForInfo)
        {
            string WaterMarkText = string.Empty;
            string WaterMarkFixedValue = MicroPublic.GetMicroInfo("WaterMarkFixedValue");
            Boolean IsWaterMarkUserName = MicroPublic.GetMicroInfo("WaterMarkUserName").toBoolean();
            Boolean IsWaterMarkDateTime = MicroPublic.GetMicroInfo("WaterMarkDateTime").toBoolean();

            if (!string.IsNullOrEmpty(WaterMarkFixedValue))
            {
                WaterMarkText = WaterMarkFixedValue;
                if (IsWaterMarkDateTime)
                    WaterMarkText += "\n" + DateTime.Now.toDateFormat("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                if (IsWaterMarkUserName)
                {
                    WaterMarkText = MicroUserInfo.GetUserInfo("DisplayName");

                    if (IsWaterMarkDateTime)
                        WaterMarkText += "\n" + DateTime.Now.toDateFormat("yyyy-MM-dd HH:mm:ss");
                }
            }

            txtIsWaterMark.Visible = true;
            txtIsWaterMark.Value = WaterMarkForInfo.ToString();

            txtWaterMarkText.Visible = true;
            txtWaterMarkText.Value = WaterMarkText;

            txtXSpace.Visible = true;
            txtXSpace.Value = MicroPublic.GetMicroInfo("WaterMarkXSpace");

            txtYSpace.Visible = true;
            txtYSpace.Value = MicroPublic.GetMicroInfo("WaterMarkYSpace");

            txtWaterMarkColor.Visible = true;
            txtWaterMarkColor.Value = MicroPublic.GetMicroInfo("WaterMarkColor");


        }

    }

    /// <summary>
    /// 顶部主要导航菜单
    /// </summary>
    /// <returns></returns>
    protected string TopMenu()
    {
        return MicroPublic.GetTopMenu("Info");
    }

    /// <summary>
    /// 顶部二级子菜单导航
    /// </summary>
    /// <returns></returns>
    protected string GetSubNav()
    {
        //Boolean AdminRole = MicroUserInfo.CheckUserRole("Administrators");

        //string Del = AdminRole == false ? " and Del=0 " : "";

        //string flag = string.Empty;
        //string _sql = "select * from InformationClass where Invalid=0 and ParentID=0 " + Del + " order by Sort";

        //DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

        //if (_dt.Rows.Count > 0)
        //{
        //    for (int i = 0; i < _dt.Rows.Count; i++)
        //    {
        //        flag += "<li><a href=\"/Views/Info/List/"+_dt.Rows[i]["ICID"].ToString()+"\">" + _dt.Rows[i]["InfoClassName"].toStringTrim() + "</a></li>";
        //    }
        //}

        //return flag;

        return MicroPublic.GetMicroMainMenu("InfoClass", "InfoClassName", "ICID", false, "/Views/Info/List/");

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
