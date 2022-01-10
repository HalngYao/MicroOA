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
using MicroWorkFlowHelper;

public partial class Views_Forms_HR_OvertimeForm : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            //动作Action 可选值Add、Modify、View
            string Action = MicroPublic.GetFriendlyUrlParm(0);
            microForm.Action = Action;
            txtAction.Value = Action;

            string ShortTableName = MicroPublic.GetFriendlyUrlParm(1);
            microForm.ShortTableName = ShortTableName;
            txtShortTableName.Value = ShortTableName;

            string ModuleID = MicroPublic.GetFriendlyUrlParm(2);
            microForm.ModuleID = ModuleID;
            txtMID.Value = ModuleID;

            string FormID = MicroPublic.GetFriendlyUrlParm(3);
            microForm.FormID = FormID;
            txtFormID.Value = FormID;

            string FormsID = MicroPublic.GetFriendlyUrlParm(4);
            if (!string.IsNullOrEmpty(FormsID))
                txtFormsID.Value = FormsID;
            
            string OvertimeIDs = MicroPublic.GetFriendlyUrlParm(5);
            if (!string.IsNullOrEmpty(OvertimeIDs))
            {
                microForm.PrimaryKeyValue = OvertimeIDs;
                txtOvertimeIDs.Value = OvertimeIDs;

                //在修改表单时绑定xmSelect，根据传递过来的OvertimeID读取数据库取得OvertimeUID（再执行js代码）
                string _sql = "select OvertimeUID,OTHour,OTMin from HROvertime where Del=0 and Invalid=0 and OvertimeID in(" + OvertimeIDs + ")";
                DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

                if (_dt.Rows.Count > 0)
                {
                    string OvertimeUIDs = "0";
                    for (int i = 0; i < _dt.Rows.Count; i++)
                    {
                        //在修改表单时让OTHour和OTMin的值选中，只以第一条记录为准（因为批量修改无法判断所有的持续是时是否为一致）
                        if (i == 0)
                        {
                            txtOTHour.Value = _dt.Rows[i]["OTHour"].toDecimalInt32().ToString();
                            txtOTMin.Value = _dt.Rows[i]["OTMin"].ToString();
                        }

                        //在修改表单时让xmSelect控件值选中
                        OvertimeUIDs += "," + _dt.Rows[i]["OvertimeUID"].ToString();
                    }
                    xmByUserDefVal.Value = OvertimeUIDs;
                }
                else
                    xmByUserDefVal.Value = MicroUserHelper.MicroUserInfo.GetUserInfo("UID");
            }
            else
                xmByUserDefVal.Value = MicroUserHelper.MicroUserInfo.GetUserInfo("UID");


            if (FormID.toInt() > 0)
            {
                string _sql = "select OvertimeTypeCode from HROvertimeType where Invalid=0 and Del=0 and FormID=@FormID";
                SqlParameter[] _sp = {new SqlParameter("@FormID",SqlDbType.Int) };
                _sp[0].Value = FormID.toInt();

                DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];
                if (_dt != null && _dt.Rows.Count > 0)
                    txtOvertimeTypeCode.Value = _dt.Rows[0]["OvertimeTypeCode"].toStringTrim();
            }

        }
        catch { }

    }
}