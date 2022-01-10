using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MicroPublicHelper;

public partial class Views_Msg : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected string Msg()
    {
        string flag = string.Empty, MsgType = MicroPublic.GetFriendlyUrlParm(0);

        //默认第一个URL参数为空时
        if (string.IsNullOrEmpty(MsgType))
            MsgType = "DenyURLError";

        flag = MicroPublic.GetFieldSet("系统提示 / System prompt", MicroPublic.GetMsg(MsgType));

        return flag;
    }
}