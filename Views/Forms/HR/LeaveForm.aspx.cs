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

public partial class Views_Forms_HR_LeaveForm : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            //动作Action 可选值Add、Modify、View
            string Action = MicroPublic.GetFriendlyUrlParm(0);
            microForm.Action = Action;


            string ShortTableName = MicroPublic.GetFriendlyUrlParm(1);
            microForm.ShortTableName = ShortTableName;
            txtShortTableName.Value = ShortTableName;

            string ModuleID = MicroPublic.GetFriendlyUrlParm(2);
            microForm.ModuleID = ModuleID;
            txtMID.Value = ModuleID;

            string FormID = MicroPublic.GetFriendlyUrlParm(3);
            microForm.FormID = FormID;
            txtFormID.Value = FormID;

            if (!string.IsNullOrEmpty(Action))
            {
                Action = Action.ToLower();
                txtAction.Value = Action;

                if (Action == "modify" || Action == "view" || Action == "close")
                {
                    string FormsID = MicroPublic.GetFriendlyUrlParm(4);
                    microForm.PrimaryKeyValue = FormsID;
                    txtFormsID.Value = FormsID;

                    //当为修改状态时为xmSelect控件赋值（因为该表单的控件涉及js代码是手动编写的所以需要）
                    if (Action == "modify")
                    {
                        string _sql = "select * from HRLeave where Invalid=0 and Del=0 and LeaveID=@LeaveID";
                        SqlParameter[] _sp = { new SqlParameter("@LeaveID", SqlDbType.Int) };

                        _sp[0].Value = FormsID.toInt();

                        DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];
                        if (_dt != null && _dt.Rows.Count > 0)
                        {
                            xmByUserDefVal.Value = _dt.Rows[0]["LeaveUID"].toStringTrim();
                            xmByHolidayTypeDefVal.Value = _dt.Rows[0]["HolidayTypeID"].toStringTrim();
                            xmByLaveDaysDefval.Value = _dt.Rows[0]["LeaveDays"].toStringTrim();
                            xmByLaveHourDefval.Value = _dt.Rows[0]["LeaveHour"].toStringTrim();
                        }
                    }
                }

                if (Action == "add")
                    xmByUserDefVal.Value = MicroUserHelper.MicroUserInfo.GetUserInfo("UID");


            }

            var getUserWorkHourSystem = MicroHRHelper.MicroHR.GetUserWorkHourSystem(MicroUserHelper.MicroUserInfo.GetUserInfo("UID").toInt(), DateTime.Now.toDateFormat());
            string WorkHourSysID = getUserWorkHourSystem.WorkHourSysID;

            if (WorkHourSysID == "1")
            {
                txtMinOvertimeDate.Value = DateTime.Now.toDateMFirstDay();
                txtMaxOvertimeDate.Value = DateTime.Now.toDateMLastDay();
            }
            else if (WorkHourSysID == "2")
            {
                txtMinOvertimeDate.Value = DateTime.Now.toDateQFirstDay();
                txtMaxOvertimeDate.Value = DateTime.Now.toDateQLastDay();
            }
            else
            {
                txtMinOvertimeDate.Value = DateTime.Now.ToString("yyyy") + "-01-01";
                txtMaxOvertimeDate.Value = DateTime.Now.ToString("yyyy") + "-12-31";
            }



        }
        catch { }
    }
}