using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using MicroAuthHelper;
using MicroPublicHelper;

public partial class Resource_MasterPage_Admin : System.Web.UI.MasterPage
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
        //设置日志
        MicroPublic.SetSysLog();

        //设置水印
        Boolean WaterMarkForOA = MicroPublic.GetMicroInfo("WaterMarkForOA").toBoolean();
        if (WaterMarkForOA)
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
                    WaterMarkText = MicroUserHelper.MicroUserInfo.GetUserInfo("DisplayName");

                    if (IsWaterMarkDateTime)
                        WaterMarkText += "\n" + DateTime.Now.toDateFormat("yyyy-MM-dd HH:mm:ss");
                }
            }

            txtIsWaterMark.Visible = true;
            txtIsWaterMark.Value = WaterMarkForOA.ToString();

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

    protected string GetMicroInfo(string Type)
    {
        return MicroPublic.GetMicroInfo(Type);
    }
}
