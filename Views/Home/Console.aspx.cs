using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MicroAuthHelper;
using MicroDBHelper;
using MicroPublicHelper;
using MicroUserHelper;

public partial class Views_home_Console : System.Web.UI.Page
{
    public string ModuleID = "0",
        AllQty = "0",
        YearQty = "0",
        MonthQty = "0",
        WeekQty = "0",
        DayQty = "0",
        MyQty = "0",

        MyApply = "0",  //我的申请
        MyApplyWaitApproval = "0", //我的申请等待审批
        MyApplyWaitAccept = "0",  //我的申请等待处理
        MyApplyAccepting = "0",  //我的申请等待处理
        MyApplyFinish = "0", //我的申请完成
        MyApplyReject = "0", //我的申请驳回
        MyApplyWithdrawal="0", //我的申请撤回

        PendingMyApproval = "0",  //等待我审批
        PendingMyAccept = "0",  //等待我处理
        Accepting = "0",  //处理中
        Finish = "0", //完成
        RunTime = "0",

        Disabled = "",

        CalendarPlanColor = string.Empty,
        CalendarActualColor = string.Empty,
        CalendarOvertimeColor = string.Empty,
        CalendarBusinessTripColor = string.Empty,
        CalendarHolidayColor = string.Empty,
        CalendarAbnormalColor = string.Empty;



    public Boolean AdminRole = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        ModuleID = MicroPublic.GetFriendlyUrlParm(0);
        txtMID.Value = ModuleID;
        string UID = MicroUserInfo.GetUserInfo("UID");
        //DateTime beforeDT = DateTime.Now;

        //允许设置日历，休息日或法定日
        if (MicroAuth.CheckPermit(ModuleID, "3"))
        {
            ckSetCalendarDays.Visible = true;
            ckSetStatutoryDays.Visible = true;
        }


        //判断是否为管理员角色被日历Title的班次是否显示调用（由于开发中而临时使用）
        AdminRole = MicroUserInfo.CheckUserRole("Administrators");
        txtAdminRole.Value = AdminRole.ToString();
        //AdminRole = false;

        //日历语言切换
        string UserName = MicroUserInfo.GetUserInfo("UserName");
        if (!string.IsNullOrEmpty(UserName))
        {
            UserName = UserName.ToLower();
            if (UserName.Substring(0, 2) == "j0")
                txtLocale.Value = "ja";
        }

        //安全事故计算
        string SafetyDay = MicroPublic.GetMicroInfo("SafetyDay");
        SafetyDay = string.IsNullOrEmpty(SafetyDay) ? "2019-11-19" : SafetyDay;
        DateTime LastAccidentDay = SafetyDay.toDateTime(); //; DateTime.Parse("2019-11-19");   //上次事故发生日
        DayQty = (DateTime.Now - LastAccidentDay).Days.ToString(); //计算上次事故距今持续天数


        #region 任务
        //我的申请 Update 20201211
        string _sql3 = "" +
        //一阶 关联相关表
        "select a.*,b.FormName,b.ShortTableName,c.EMail,c.WorkTel,c.WorkMobilePhone,c.AdDepartment from FormApprovalRecords a left join Forms b on a.FormID=b.FormID left join UserInfo c on a.UID=c.UID " +
        //二阶 得到没有审批完成或没有对应完成或没有结案时的 min(FARID)
        "where ((FARID in (select min(FARID) from FormApprovalRecords where Invalid = 0 and Del = 0 and StateCode <= 0 and UID = " + UID + " group by FormID,FormsID)) " +
        //三阶 或者得到已经结案了的max(FARID),同时排除二阶出现过的记录
        "or (FARID in (select max(FARID) from FormApprovalRecords where Invalid = 0 and Del = 0 and StateCode > 0 and UID = " + UID + " " +
        //三阶接续 排除二阶出现过的记录
        "and FormNumber not in (select FormNumber from FormApprovalRecords where FARID in (select min(FARID) from FormApprovalRecords where Invalid = 0 and Del = 0 and StateCode <= 0 and UID = " + UID + " group by FormID,FormsID) ) group by FormID,FormsID) )) " +
        "and a.Invalid=0 and a.Del=0 and b.Invalid = 0 and b.Del = 0 and a.UID = " + UID + " order by FARID desc";
        DataTable _dt3 = MsSQLDbHelper.Query(_sql3).Tables[0];
        MyApply = _dt3.Rows.Count.ToString();
        if (_dt3.Rows.Count > 0)
            aMyApply.Attributes.Add("lay-href", "/Views/Forms/MicroPublicFormList/View/MyApply/" + ModuleID);

        //我的申请等待审批 Update 20201211
        string _sql4 = "" +
        //一阶 关联相关表
        " select a.*,b.FormName,b.ShortTableName,c.EMail,c.WorkTel,c.WorkMobilePhone,c.AdDepartment from FormApprovalRecords a left join Forms b on a.FormID=b.FormID left join UserInfo c on a.UID=c.UID " +
        //二阶 得到在审批阶段已经审批或未审批的max(FARID)作为单条记录
        " where FARID in (select min(FARID) from FormApprovalRecords where WorkFlowID in (select WFID from WorkFlow where FlowCode = 'Approval' and Invalid=0 and Del=0 ) and Invalid = 0 and Del = 0 and StateCode = 0 and UID = " + UID + " group by FormID,FormsID) " +
        //三阶 去除在任意审批阶段被驳回的记录 （注：申请、受理、结案不算审批阶段）
        " and FormNumber not in(select FormNumber from FormApprovalRecords where FARID in (select max(FARID) from FormApprovalRecords where Invalid = 0 and Del = 0 and StateCode < 0 and UID = " + UID + " group by FormID,FormsID) and Invalid=0 and Del=0 and UID = " + UID + ") " +
        //四阶 去除在审批阶段最后一位已经审批通过的记录
        " and FARID not in (select FARID from FormApprovalRecords where WorkFlowID in(select max(WFID) from WorkFlow where FlowCode = 'Approval' and Invalid=0 and Del=0 group by FormID) and Invalid=0 and Del=0 and StateCode=1 and UID = " + UID + ") " +
        " and a.Invalid=0 and a.Del=0 and b.Invalid = 0 and b.Del = 0 and a.UID = " + UID + " order by FARID desc";
        DataTable _dt4 = MsSQLDbHelper.Query(_sql4).Tables[0];
        MyApplyWaitApproval = _dt4.Rows.Count.ToString();
        if (_dt4.Rows.Count > 0)
            aMyApplyWaitApproval.Attributes.Add("lay-href", "/Views/Forms/MicroPublicFormList/View/MyApplyWaitApproval/" + ModuleID);

        //我的申请等待处理 Update 20201214
        string _sql5 = "" +
        //一阶 关联相关表
        "select a.*,b.FormName,b.ShortTableName,c.EMail,c.WorkTel,c.WorkMobilePhone,c.AdDepartment from FormApprovalRecords a left join Forms b on a.FormID=b.FormID left join UserInfo c on a.UID=c.UID " +
        //二阶 得到在审批阶段最大的记录已经经过审批的（即max(FARID) 且StateCode>0）
        " where a.Invalid = 0 and a.Del = 0 and b.Invalid = 0 and b.Del = 0 and (FormNumber in( " +
        " select FormNumber from FormApprovalRecords where FARID in (select max(FARID) from FormApprovalRecords where WorkFlowID in (select WFID from WorkFlow where Invalid=0 and Del=0 and FlowCode = 'Approval') and Invalid=0 and Del=0 group by FormID,FormsID) " +
        " and StateCode>0 and StateCode<>15) " +
        //三阶 并且在受理阶段未受理的
        " and WorkFlowID in (select WFID from WorkFlow where Invalid=0 and Del=0 and FlowCode = 'Accept') and StateCode = 0 " +
        //四阶 去除在任意审批阶段被驳回的记录 （注：申请、受理、结案不算审批阶段）
        " and FormNumber not in(select FormNumber from FormApprovalRecords where FARID in (select max(FARID) from FormApprovalRecords where StateCode < 0 and Invalid = 0 and Del = 0 group by FormID,FormsID)) " +
        //五阶 Invalid=0 and Del=0并且需是我申请的
        " and a.Invalid = 0 and a.Del = 0 and a.UID=" + UID + " ) " +
        //六阶 或者(或运算)得到不需要审批的表单并且在受理阶段的记录、并且是我申请的 （注意：FormID=0）
        " or (FARID in (select FARID from FormApprovalRecords where Invalid=0 and Del=0 and  WorkFlowID in (select WFID from WorkFlow where Invalid=0 and Del=0 and FormID=0 and FlowCode='Accept') and StateCode = 0 " +
        " and UID=" + UID + " )) order by FARID desc";
        DataTable _dt5 = MsSQLDbHelper.Query(_sql5).Tables[0];
        MyApplyWaitAccept = _dt5.Rows.Count.ToString();
        if (_dt5.Rows.Count > 0)
            aMyApplyWaitAccept.Attributes.Add("lay-href", "/Views/Forms/MicroPublicFormList/View/MyApplyWaitAccept/" + ModuleID);

        //我的申请处理中 Update 20201211
        string _sql6 = "" +
        //一阶 关联相关表
        "select a.*,b.FormName,b.ShortTableName,c.EMail,c.WorkTel,c.WorkMobilePhone,c.AdDepartment from FormApprovalRecords a left join Forms b on a.FormID=b.FormID left join UserInfo c on a.UID=c.UID " +
        //二阶 得到在受理阶段已受理的记录
        "where FARID in (select FARID from FormApprovalRecords where WorkFlowID in (select WFID from WorkFlow where Invalid=0 and Del=0 and FlowCode = 'Accept') and Invalid=0 and Del=0 and StateCode > 0 and UID = " + UID + " ) " +
        //三阶 去除在任意审批阶段被驳回的记录 
        "and FormNumber not in(select FormNumber from FormApprovalRecords where FARID in (select max(FARID) from FormApprovalRecords where StateCode < 0 and Invalid = 0 and Del = 0 group by FormID,FormsID))  " +
        //四阶 排除已完成的记录
        "and FormNumber not in (select FormNumber from FormApprovalRecords where WorkFlowID in (select WFID from WorkFlow where Invalid=0 and Del=0 and FlowCode = 'Finish') and Invalid=0 and Del=0 and StateCode > 0 and UID = " + UID + " ) " +
        "and a.Invalid = 0 and a.Del = 0 and b.Invalid = 0 and b.Del = 0 and a.UID = " + UID + " order by FARID desc";
        DataTable _dt6 = MsSQLDbHelper.Query(_sql6).Tables[0];
        MyApplyAccepting = _dt6.Rows.Count.ToString();
        if (_dt6.Rows.Count > 0)
            aMyApplyAccepting.Attributes.Add("lay-href", "/Views/Forms/MicroPublicFormList/View/MyApplyAccepting/" + ModuleID);

        //我的申请完成
        string _sql7 = "select a.*,b.FormName,b.ShortTableName,c.EMail,c.WorkTel,c.WorkMobilePhone,c.AdDepartment from FormApprovalRecords a left join Forms b on a.FormID=b.FormID left join UserInfo c on a.UID=c.UID where WorkFlowID in (select WFID from WorkFlow where Invalid=0 and Del=0 and FlowCode='Finish') and StateCode>0 and a.UID=" + UID + " and a.Invalid=0 and a.Del=0 and b.Invalid = 0 and b.Del = 0 order by FARID desc";
        DataTable _dt7 = MsSQLDbHelper.Query(_sql7).Tables[0];
        MyApplyFinish = _dt7.Rows.Count.ToString();
        if (_dt7.Rows.Count > 0)
            aMyApplyFinish.Attributes.Add("lay-href", "/Views/Forms/MicroPublicFormList/View/MyApplyFinish/" + ModuleID);

        //我的申请驳回
        string _sql8 = "select a.*,b.FormName,b.ShortTableName,c.EMail,c.WorkTel,c.WorkMobilePhone,c.AdDepartment from FormApprovalRecords a left join Forms b on a.FormID=b.FormID left join UserInfo c on a.UID=c.UID where FARID in (select max(FARID) from FormApprovalRecords where StateCode = -1 and Invalid = 0 and Del = 0 group by FormID,FormsID) and a.UID = " + UID + " and a.Invalid=0 and a.Del=0 and b.Invalid = 0 and b.Del = 0 order by FARID desc";
        DataTable _dt8 = MsSQLDbHelper.Query(_sql8).Tables[0];
        MyApplyReject = _dt8.Rows.Count.ToString();
        if (_dt8.Rows.Count > 0)
            aMyApplyReject.Attributes.Add("lay-href", "/Views/Forms/MicroPublicFormList/View/MyApplyReject/" + ModuleID);

        //我的申请撤回
        string _sql9 = "select a.*,b.FormName,b.ShortTableName,c.EMail,c.WorkTel,c.WorkMobilePhone,c.AdDepartment from FormApprovalRecords a left join Forms b on a.FormID=b.FormID left join UserInfo c on a.UID=c.UID where FARID in (select max(FARID) from FormApprovalRecords where (StateCode = -4 or StateCode = 15) and Invalid = 0 and Del = 0 group by FormID,FormsID) and a.UID = " + UID + " and a.Invalid=0 and a.Del=0 and b.Invalid = 0 and b.Del = 0 order by FARID desc";
        DataTable _dt9 = MsSQLDbHelper.Query(_sql9).Tables[0];
        MyApplyWithdrawal = _dt9.Rows.Count.ToString();
        if (_dt9.Rows.Count > 0)
            aMyApplyWithdrawal.Attributes.Add("lay-href", "/Views/Forms/MicroPublicFormList/View/MyApplyWithdrawal/" + ModuleID);



        //********************任务*********************
        //等待我审批 Update 20201231
        string _sql11 = "" +
            //一阶 关联相关表
            "select a.*,b.FormName,b.ShortTableName,c.EMail,c.WorkTel,c.WorkMobilePhone,c.AdDepartment from FormApprovalRecords a left join Forms b on a.FormID=b.FormID left join UserInfo c on a.UID=c.UID " +
            //二阶 得到在审批阶段最小的需要审批的记录
            "where FARID in (select min(FARID) from FormApprovalRecords where WorkFlowID in (select WFID from WorkFlow where Invalid=0 and Del=0 and FlowCode='Approval') and Invalid=0 and Del=0 and StateCode=0 group by FormID,FormsID) " +
            //三阶 去除在任意审批阶段被驳回的记录 （注：申请、受理、结案不算审批阶段）
            "and FormNumber not in(select FormNumber from FormApprovalRecords where FARID in (select max(FARID) from FormApprovalRecords where StateCode < 0 and Invalid = 0 and Del = 0 group by FormID,FormsID)) " +
            //四阶 与我相关的
            "and CHARINDEX(',' + convert(varchar, " + UID + ") + ',',',' + CanApprovalUID + ',')> 0 and a.Invalid = 0 and a.Del = 0 and b.Invalid = 0 and b.Del = 0 order by FARID desc";
        DataTable _dt11 = MsSQLDbHelper.Query(_sql11).Tables[0];
        PendingMyApproval = _dt11.Rows.Count.ToString();
        if (_dt11.Rows.Count > 0)
        {
            //aPendingMyApproval.Attributes.Add("lay-href", "/Views/Forms/MicroPublicFormList/View/PendingMyApproval/" + ModuleID);
            aPendingMyApproval.Attributes.Add("data-type", "GetPendingMyApproval");
            aPendingMyApproval.Attributes.Add("href", "javascript:;");
        }

        //待我处理 Update 20201214
        string _sql12 = "" +
        //一阶 关联相关表
        "select a.*,b.FormName,b.ShortTableName,c.EMail,c.WorkTel,c.WorkMobilePhone,c.AdDepartment from FormApprovalRecords a left join Forms b on a.FormID = b.FormID left join UserInfo c on a.UID = c.UID " +
        //二阶 得到在审批阶段最大的记录已经经过审批的（即max(FARID) 且StateCode > 0）
        "where a.Invalid = 0 and a.Del = 0 and b.Invalid = 0 and b.Del = 0 and (FormNumber in( " +
        "select FormNumber from FormApprovalRecords where FARID in (select max(FARID) from FormApprovalRecords where WorkFlowID in (select WFID from WorkFlow where Invalid = 0 and Del = 0 and FlowCode = 'Approval') and Invalid = 0 and Del = 0 group by FormID,FormsID) " +
        "and StateCode> 0) " +
        //三阶 并且在受理阶段未受理的
        "and WorkFlowID in (select WFID from WorkFlow where Invalid = 0 and Del = 0 and FlowCode = 'Accept') and StateCode = 0 " +
        //四阶 去除在任意审批阶段被驳回的记录 （注：申请、受理、结案不算审批阶段）
        "and FormNumber not in(select FormNumber from FormApprovalRecords where FARID in (select max(FARID) from FormApprovalRecords where StateCode < 0 and Invalid = 0 and Del = 0 group by FormID,FormsID)) " +
        //五阶 Invalid = 0 and Del = 1并且需要我进行处理的 （即 CHARINDEX(',' + convert(varchar, 362) + ',',',' + CanApprovalUID + ',')> 0 ）
        "and a.Invalid = 0 and a.Del = 0 and CHARINDEX(',' + convert(varchar, " + UID + ") + ',',',' + CanApprovalUID + ',')> 0 ) " +
        //六阶 或者(或运算)得到不需要审批的表单在受理阶段的记录并且需要我进行处理的 （注意：FormID = 0）
        "or (FARID in (select FARID from FormApprovalRecords where Invalid = 0 and Del = 0 and WorkFlowID in (select WFID from WorkFlow where Invalid = 0 and Del = 0 and FormID = 0 and FlowCode = 'Accept') and StateCode = 0 " +
        "and CHARINDEX(',' + convert(varchar, " + UID + ") + ',',',' + CanApprovalUID + ',')> 0)) order by FARID desc";
        DataTable _dt12 = MsSQLDbHelper.Query(_sql12).Tables[0];
        PendingMyAccept = _dt12.Rows.Count.ToString();
        if (_dt12.Rows.Count > 0)
            aPendingMyAccept.Attributes.Add("lay-href", "/Views/Forms/MicroPublicFormList/View/PendingMyAccept/" + ModuleID);

        //我处理中 Update 20201214
        string _sql13 = "" +
        //一阶 关联相关表
        "select a.*,b.FormName,b.ShortTableName,c.EMail,c.WorkTel,c.WorkMobilePhone,c.AdDepartment from FormApprovalRecords a left join Forms b on a.FormID=b.FormID left join UserInfo c on a.UID=c.UID " +
        //二阶 得到在受理阶段已经经过受理的记录
        "where FormNumber in( " +
        "select FormNumber from FormApprovalRecords where FARID in (select max(FARID) from FormApprovalRecords where WorkFlowID in (select WFID from WorkFlow where Invalid=0 and Del=0 and FlowCode = 'Accept') and Invalid=0 and Del=0 group by FormID,FormsID) " +
        "and StateCode>0 and ApprovalUID=" + UID + ") " +
        //三阶 并且在结案阶段但未结案的
        "and WorkFlowID in (select WFID from WorkFlow where Invalid=0 and Del=0 and FlowCode = 'Finish') and StateCode = 0 " +
        //四阶 去除在任意审批阶段被驳回的记录
        "and FormNumber not in(select FormNumber from FormApprovalRecords where FARID in (select max(FARID) from FormApprovalRecords where StateCode < 0 and Invalid = 0 and Del = 0 group by FormID,FormsID)) " +
        "and CHARINDEX(',' + convert(varchar, " + UID + ") + ',',',' + CanApprovalUID + ',')> 0 and a.Invalid=0 and a.Del=0 and b.Invalid = 0 and b.Del = 0 order by FARID desc";
        DataTable _dt13 = MsSQLDbHelper.Query(_sql13).Tables[0];
        Accepting = _dt13.Rows.Count.ToString();
        if (_dt13.Rows.Count > 0)
            aAccepting.Attributes.Add("lay-href", "/Views/Forms/MicroPublicFormList/View/Accepting/" + ModuleID);

        //我处完成 Update 20201214
        string _sql14 = "select a.*,b.FormName,b.ShortTableName,c.EMail,c.WorkTel,c.WorkMobilePhone,c.AdDepartment from FormApprovalRecords a left join Forms b on a.FormID=b.FormID left join UserInfo c on a.UID=c.UID where WorkFlowID in (select WFID from WorkFlow where FlowCode='Finish') and StateCode>0 and ApprovalUID= " + UID + " and CHARINDEX(',' + convert(varchar,  " + UID + ") + ',', ',' + CanApprovalUID + ',') > 0 and a.Invalid = 0 and a.Del = 0 and b.Invalid = 0 and b.Del = 0 order by FARID desc";
        DataTable _dt14 = MsSQLDbHelper.Query(_sql14).Tables[0];
        Finish = _dt14.Rows.Count.ToString();
        if (_dt14.Rows.Count > 0)
            aFinish.Attributes.Add("lay-href", "/Views/Forms/MicroPublicFormList/View/Finish/" + ModuleID);

        #endregion

        //计算运行时间
        //DateTime afterDT = System.DateTime.Now;
        //TimeSpan ts = afterDT.Subtract(beforeDT);
        //RunTime = ts.TotalMilliseconds.ToString() + " ms";

        txtCalendarFirstDay.Value = MicroPublic.GetMicroInfo("CalendarFirstDay");

        if (MicroPublic.GetMicroInfo("CalendarPlanEvent").toBoolean())
        {
            divPlanLegend.Visible = true;
            spanPlanLegend.Visible = true;
        }

        if (MicroPublic.GetMicroInfo("CalendarActualEvent").toBoolean())
        {
            divActualLegend.Visible = true;
            spanActualLegend.Visible = true;
        }

        if (MicroPublic.GetMicroInfo("CalendarOvertimeEvent").toBoolean())
        {
            divOvertimeLegend.Visible = true;
            spanOvertimeLegend.Visible = true;
        }

        if (MicroPublic.GetMicroInfo("CalendarBusinessTripEvent").toBoolean())
        {
            divBusinessTripLegend.Visible = true;
            spanBusinessTripLegend.Visible = true;
        }

        if (MicroPublic.GetMicroInfo("CalendarHolidayEvent").toBoolean())
        {
            divHolidayLegend.Visible = true;
            spanHolidayLegend.Visible = true;
        }

        if (MicroPublic.GetMicroInfo("CalendarAbnormalEvent").toBoolean())
        {
            divAbnormalLegend.Visible = true;
            spanAbnormalLegend.Visible = true;
        }

        CalendarPlanColor = MicroPublic.GetMicroInfo("CalendarPlanColor");
        CalendarActualColor = MicroPublic.GetMicroInfo("CalendarActualColor");
        CalendarOvertimeColor = MicroPublic.GetMicroInfo("CalendarOvertimeColor");
        CalendarBusinessTripColor = MicroPublic.GetMicroInfo("CalendarBusinessTripColor");
        CalendarHolidayColor = MicroPublic.GetMicroInfo("CalendarHolidayColor");
        CalendarAbnormalColor = MicroPublic.GetMicroInfo("CalendarAbnormalColor");
      
    }

}