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

public partial class Views_Forms_Flow : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //动作Action 可选值Add、Modify、View
        string Action = MicroPublic.GetFriendlyUrlParm(0);

        string ShortTableName = MicroPublic.GetFriendlyUrlParm(1);
        txtSTN.Value = ShortTableName;

        string ModuleID = MicroPublic.GetFriendlyUrlParm(2);
        txtMID.Value = ModuleID;

        string FormID= MicroPublic.GetFriendlyUrlParm(3);
        txtFormID.Value = FormID;

        string WFID = MicroPublicHelper.MicroPublic.GetFriendlyUrlParm(4);
        txtWFID.Value = WFID;

        if (!MicroAuth.CheckPermit(ModuleID, "2"))
        {
            btnAddNode.Disabled = true;
            btnAddNode.Attributes.Add("class", "layui-btn layui-btn-sm layui-btn-disabled");
        }

        if (!MicroAuth.CheckPermit(ModuleID, "3"))
        {
            btnModify.Disabled = true;
            btnModify.Attributes.Add("class", "layui-btn layui-btn-sm layui-btn-disabled");
        }

        if (!MicroAuth.CheckPermit(ModuleID, "16"))
        {
            linkDel.Attributes.Add("class", "layui-btn layui-btn-xs layui-btn-disabled");
            linkDel.Attributes.Remove("lay-event");
        }

        if (MicroAuth.CheckPermit(ModuleID, "11"))
            txtEditTablePermit.Value = "1";

        if (string.IsNullOrEmpty(WFID))
        {
            if (!string.IsNullOrEmpty(FormID))
            {
                int intMaxSort = MsSQLDbHelper.GetMaxNumber("Sort", "WorkFlow", "where FormID=" + FormID + " and ParentID=0 and Invalid=0 and Del=0");
                int Sort = intMaxSort + 1;
                txtFlowName.Value = "FlowName" + Sort;
            }
        }
        else
        {
            string _sql = "select * from WorkFlow where WFID=@WFID and Invalid=0 and Del=0";
            SqlParameter[] _sp = { new SqlParameter("@WFID", SqlDbType.Int) };
            _sp[0].Value = WFID.toInt();
            DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

            if (_dt != null && _dt.Rows.Count > 0)
            {
                txtFlowName.Value = _dt.Rows[0]["FlowName"].toStringTrim();
                ckIsSync.Checked = _dt.Rows[0]["IsSync"].toStringTrim() == "True" ? true : false;
                selEffectiveType.Value = _dt.Rows[0]["EffectiveType"].toStringTrim();
                hidEffectiveIDStr.Value = _dt.Rows[0]["EffectiveIDStr"].toStringTrim();
            }

        }

    }
}