using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroUserHelper;
using MicroPublicHelper;

public partial class Views_Info_GlobalTips : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string flag = string.Empty;

        try
        {
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
                string _sql = "select Top 10 a.*,b.InfoClassName from Information a left join InformationClass b on a.InfoClassID=b.ICID where a.Invalid=0 and a.Del=0 and a.GlobalTips=1 and a.GlobalTipsTime >= '" + DateTime.Now.ToString("yyyy-MM-dd") + "' order by DateCreated desc";
                DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

                if (_dt.Rows.Count > 0)
                {
                    string MaxDateCreated = _dt.Compute("max(DateCreated)","").toDateFormat("yyyy-MM-dd");

                    for (int i = 0; i < _dt.Rows.Count; i++)
                    {
                        string NewTips = string.Empty,
                            DateCreated = _dt.Rows[i]["DateCreated"].toStringTrim();

                        if (!string.IsNullOrEmpty(MaxDateCreated) && DateCreated.toDateTime("yyyy-MM-dd") == MaxDateCreated.toDateTime("yyyy-MM-dd") && MicroPublic.GetMicroInfo("InfoNewTips").toBoolean())
                            NewTips = "<span class=\"layui-badge layui-bg-red\" style=\"border:none; margin-left:10px; \">新</span>";

                        flag += "<li><a style=\"border: 1px solid #5FB878; background: none; color: #5FB878; font-size:10px; padding:1px 5px 1px 5px; border-radius:2px; margin-right:8px;\">" + _dt.Rows[i]["InfoClassName"].toStringTrim() + "</a>";
                        flag += "<a href= \"/Views/Info/Detail/27/" + _dt.Rows[i]["InfoID"].toStringTrim() + "\" target= \"_blank\" >" + _dt.Rows[i]["Title"].toStringTrim() + "</a>" + NewTips + "<span style = \"position:absolute; right:20px;\">" + DateCreated.toDateFormat("yyyy-MM-dd HH:mm") + "</span>";
                        flag += "</li>";
                    }

                    ulList.InnerHtml = flag;
                }

            }

        }
        catch { }
    }
}