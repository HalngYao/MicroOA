using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MicroPublicHelper;

public partial class Views_Message : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected string Msg()
    {
        string flag = string.Empty, MsgType = MicroPublic.GetFriendlyUrlParm(0);

        if (!string.IsNullOrEmpty(MsgType))
            MsgType = MsgType.ToLower();

        string MsgContent = MicroPublic.GetMsg("DenyURLError");

        switch (MsgType)
        {
            case "denyaccess":  //根据系统设定IP访问列表内容进行提示
                MsgContent = MicroPublic.GetMicroInfo("DenyAccessSiteTips");
                break;
            
        }

        flag = MicroPublic.GetFieldSet("系统提示 / System prompt", MsgContent);

        return flag;
    }
}