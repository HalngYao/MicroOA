using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroPublicHelper;
using MicroAuthHelper;

public partial class Views_Forms_WorkFlow : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //string ShortTableName= MicroPublicHelper.MicroPublic.GetFriendlyUrlParm(0);
        //string ModuleID = MicroPublicHelper.MicroPublic.GetFriendlyUrlParm(1);
        //string FormID= MicroPublicHelper.MicroPublic.GetFriendlyUrlParm(2);

        //txtSTN.Value = ShortTableName;
        //txtMID.Value = ModuleID;
        //txtFormID.Value = FormID;


        //动作Action 可选值Add、Modify、View
        string Action = MicroPublic.GetFriendlyUrlParm(0);

        string ShortTableName = MicroPublic.GetFriendlyUrlParm(1);
        txtSTN.Value = ShortTableName;

        string ModuleID = MicroPublic.GetFriendlyUrlParm(2);
        txtMID.Value = ModuleID;

        string FormID = MicroPublic.GetFriendlyUrlParm(3);
        txtFormID.Value = FormID;

        if (!MicroAuth.CheckPermit(ModuleID, "2"))
        {
            btnAddOpenLink.Disabled = true;
            btnAddOpenLink.Attributes.Add("class", "layui-btn layui-btn-sm layui-btn-disabled");
        }

        if (!MicroAuth.CheckPermit(ModuleID, "10"))
        {
            if (!MicroAuth.CheckPermit(ModuleID, "9"))
            {
                linkModify.Attributes.Add("class", "layui-btn layui-btn-xs layui-btn-disabled");
                linkModify.Attributes.Remove("lay-event");
            }
        }


        if (!MicroAuth.CheckPermit(ModuleID, "16"))
        {
            linkDel.Attributes.Add("class", "layui-btn layui-btn-xs layui-btn-disabled");
            linkDel.Attributes.Remove("lay-event");
        }


    }

}