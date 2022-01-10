<%@ WebHandler Language="C#" Class="MicroFormAccept" %>

using System;
using System.Web;
using System.Data;
using MicroPublicHelper;
using MicroWorkFlowHelper;
using MicroUserHelper;
using MicroFormHelper;
using MicroAuthHelper;
using MicroNoticeHelper;

public class MicroFormAccept : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = MicroPublic.GetMsg("AcceptAcceptFailed"),
                Action = context.Server.UrlDecode(context.Request.Form["action"]),
                ShortTableName = context.Server.UrlDecode(context.Request.Form["stn"]),
                ModuleID = context.Server.UrlDecode(context.Request.Form["mid"]),
                FormID = context.Server.UrlDecode(context.Request.Form["formid"]),
                FormsID = context.Server.UrlDecode(context.Request.Form["pkv"]);

        //测试数据
        //Action = "view";
        //ShortTableName = "ITGenForm";
        //ModuleID = "4";
        //FormID = "1";
        //FormsID = "16";

        if (!string.IsNullOrEmpty(Action) && !string.IsNullOrEmpty(ShortTableName) && !string.IsNullOrEmpty(ModuleID) && !string.IsNullOrEmpty(FormID) && !string.IsNullOrEmpty(FormsID))
            flag = SetAccept(ModuleID, FormID, ShortTableName, FormsID);

        context.Response.Write(flag);
    }


    private string SetAccept(string ModuleID, string FormID, string ShortTableName, string FormsID)
    {
        string flag = MicroPublic.GetMsg("AcceptAcceptFailed"),
                FormNumber = string.Empty,
                UID = MicroUserInfo.GetUserInfo("UID"),
                DisplayName = MicroUserInfo.GetUserInfo("DisplayName"),
                ApprovalState = MicroWorkFlow.GetApprovalState(1),
                IP = MicroUserInfo.GetUserInfo("IP"),
                Note = string.Empty;

        try
        {

            var GetFormAttr = MicroForm.GetFormAttr(ShortTableName, FormID);

            Boolean IsApproval = GetFormAttr.IsApproval;
            Boolean AutoAccept = GetFormAttr.AutoAccept;
            Boolean AutoClose = GetFormAttr.AutoClose;
            string FormName = GetFormAttr.FormName;

            // 0 = 等待审批[Waiting]、1 = 审批通过[Pass]、-1 = 驳回申请[Return]、-2 = 临时保存[Draft]、11 = 提交申请[Pass]、22 = 等待受理[WaitingForAccept]、23 = 等待对应[WaitingForProcessing]、24 = 等待处理[WaitingForProcessing]、32 = 受理中[Accepting]、33 = 对应中[Processing]、34 = 处理中[Processing]、100 = 完成[Finish]
            int FormState = MicroWorkFlow.GetFormState(ShortTableName, FormID, FormsID);

            //处于受理状态
            if ((FormState > 20 && FormState < 30) || FormState == 15)
            {
                var GetFormApprovalRecordsAttr = MicroWorkFlow.GetFormApprovalRecordsAttr(FormID, FormsID);
                DataTable _dt = GetFormApprovalRecordsAttr.SourceDT;
                FormNumber = GetFormApprovalRecordsAttr.FormNumber;


                //申请者信息
                string Applicant = string.Empty;
                string ApplicantID = string.Empty;
                //受理者信息
                string CanReceiveID = string.Empty;
                string ReceiveID = string.Empty;

                if (_dt.Rows.Count > 0)
                {
                    Boolean Perm = false;

                    //第一条记录是申请者信息
                    Applicant = _dt.Rows[0]["DisplayName"].toJsonTrim();
                    ApplicantID = _dt.Rows[0]["UID"].toJsonTrim();

                    //最后一条记录（_dt.Rows.Count - 1）（审批完成时通知受理者或结案者）
                    CanReceiveID = _dt.Rows[_dt.Rows.Count - 1]["CanApprovalUID"].toStringTrim();
                    ReceiveID = _dt.Rows[_dt.Rows.Count - 1]["ApprovalUID"].toStringTrim();

                    //有受理权限时
                    if (MicroAuth.CheckPermit(ModuleID, "13"))
                        Perm = true;
                    else  //没有受理权限时，判断是否为受理者（FormApprovalRecords表里该表单记录的受理步骤）
                    {
                        //已受理过，但被受理者撤回
                        DataRow[] _rows2 = _dt.Select("CanApprovalUID<>'' and ApprovalUID=" + UID + " and StateCode=15 and FixedNode=1", "Sort");
                        //未受理过，第一次受理
                        if (_rows2.Length == 0)
                            _rows2 = _dt.Select("CanApprovalUID<>'' and ApprovalUID=0 and StateCode=0 and FixedNode=1", "Sort");

                        if (_rows2.Length > 0)
                        {
                            string iCanApprovalUID = _rows2[0]["CanApprovalUID"].toJsonTrim();
                            if (MicroPublic.CheckSplitExists(iCanApprovalUID, UID, ','))
                                Perm = true;
                        }
                    }

                    if (Perm)
                    {
                        //更新FormApprovalRecords状态（倒数第二步，即“受理”）
                        DataRow[] _rows3 = _dt.Select("", "Sort");
                        if (_rows3.Length > 0)
                        {
                            //得到受理步骤的FARID
                            int FARID3 = _rows3[_rows3.Length - 2]["FARID"].toInt();  //_rows3.Length - 2 倒数第二步
                            string NodeName3 = _rows3[_rows3.Length - 2]["NodeName"].toJsonTrim();

                            //更新日志状态（在FormApprovalRecords状态更新成功时)
                            if (MicroWorkFlow.SetFormApprovalRecords(UID, DisplayName, 33, Note, FARID3))
                            {
                                flag = MicroPublic.GetMsg("Accept");

                                //更新表单状态，设置表单状态为33“对应中[Processing]”状态
                                if (!MicroWorkFlow.SetFormState(ShortTableName, 33, FormID, FormsID))
                                    flag = "更新表单状态失败。错误代码：101 <br/>The update of the form status failed.";

                                //if (!SetApprovalLogs(FormID, FormsID, FormNumber, NodeName3, 33, MicroWorkFlow.GetApprovalState(33), Note))
                                if (!MicroWorkFlow.SetApprovalLogs(FormID, FormsID, FormNumber, NodeName3, UID, DisplayName, 33, Note))
                                    flag = "更新日志状态“受理”步骤失败。错误代码：102<br/>The auto accept step of updating the log status failed.";

                                //*****在受理成功后判断是否是自动结案Start*****
                                //如果开启自动结案
                                if (AutoClose)
                                {
                                    //1.更新FormApprovalRecords状态（最后一步，即“完成”）
                                    DataRow[] _rows4 = _dt.Select("", "Sort");
                                    if (_rows4.Length > 0)
                                    {
                                        int FARID4 = _rows4[_rows4.Length - 1]["FARID"].toInt();
                                        string NodeName4 = _rows4[_rows4.Length - 1]["NodeName"].toJsonTrim();

                                        //2.更新日志状态（最后一步，即“完成”在FormApprovalRecords状态更新成功时，再次更新日志状态)
                                        if (MicroWorkFlow.SetFormApprovalRecords(UID, DisplayName, 100, Note, FARID4))
                                        {
                                            //3.更新表单状态，设置表单状态为“完成[Finish]”状态
                                            if (!MicroWorkFlow.SetFormState(ShortTableName, 100, FormID, FormsID))
                                                flag = "更新表单状态失败。错误代码：102 <br/>The update of the form status failed.";

                                            //###2结案了通知（受理者）对应者。例1：xxx提交的《xxx》申请，编号：xxx，已受理并且对应（处理）完成【系统自动结案】。
                                            //string Content4 = "" + Applicant + "提交的" + _FormName + "申请，编号：" + FormNumber + "，已受理并且对应（处理）完成【系统自动结案】。";  //Update 20210112
                                            string Content4 = MicroNotice.GetNoticeContent("ClosedAutoProcess", ShortTableName, ModuleID, FormID, FormsID, FormNumber, FormName, "", "", DisplayName, Applicant, Note);
                                            MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, FormsID, FormNumber, "Msg", FormName, Content4, false, CanReceiveID);

                                            //###3结案了通知申请者。例1：您提交的《xxx》申请，编号：xxx，已对应（处理）完成【系统自动结案】。
                                            //string Content5 = "您提交的" + _FormName + "申请，编号：" + FormNumber + "，已受理和对应（处理）完成【系统自动结案】。";  //Update 20210112
                                            string Content5 = MicroNotice.GetNoticeContent("ClosedAutoApplicant", ShortTableName, ModuleID, FormID, FormsID, FormNumber, FormName, "", "", DisplayName, Applicant, Note);
                                            MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, FormsID, FormNumber, "Msg", FormName, Content5, false, ApplicantID);

                                            //if (!SetApprovalLogs(FormID, FormsID, FormNumber, NodeName4, 100, MicroWorkFlow.GetApprovalState(100), Note))
                                            if (!MicroWorkFlow.SetApprovalLogs(FormID, FormsID, FormNumber, NodeName4, UID, DisplayName, 100, Note))
                                                flag = "更新日志状态“自动结案”步骤失败。错误代码：103<br/>The auto close step of updating the log status failed.";
                                        }
                                        else
                                            flag = "更新审批状态“自动结案”步骤失败。错误代码：104<br/>The auto close step of updating the approval status failed.";
                                    }
                                }
                                //autoClose else.非自动完成结案审批时，更新表单状态（FormApprovalRecords状态和日志状态已在上面更新）
                                else
                                {
                                    if (!MicroWorkFlow.SetFormState(ShortTableName, 33, FormID, FormsID))
                                        flag = "更新表单状态失败。错误代码：105 <br/>The update of the form status failed.";

                                    //###3受理了通知申请者。例1：您提交的《xxx》申请，编号：xxx，已被受理正在对应/处理中，请耐心等待。 
                                    //string Content3 = "您提交的" + _FormName + "申请，编号：" + FormNumber + "，已被受理正在为您安排对应（处理），请耐心等待。";  //Update 20210112
                                    string Content3 = MicroNotice.GetNoticeContent("AcceptToApplicant", ShortTableName, ModuleID, FormID, FormsID, FormNumber, FormName, "", "", DisplayName, Applicant, Note);
                                    MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, FormsID, FormNumber, "Msg", FormName, Content3, false, ApplicantID);

                                    //###4受理完成，通知对应者
                                    //站点通知，通知对应者。例1：xxx提交的《xxx》申请，编号：xxx，已由xxx受理成功，拜托您尽快进行对应（处理）。 例2：xxx提交的《xxx》申请，编号：xxx，受理成功，请您尽快对应（处理）。
                                    //string Content6 = "" + Applicant + "提交的" + _FormName + "申请，编号：" + FormNumber + "，已受理成功，拜托您尽快进行对应（处理）。";  //Update 20210112
                                    string Content6 = MicroNotice.GetNoticeContent("AcceptToProcess", ShortTableName, ModuleID, FormID, FormsID, FormNumber, FormName, "", "", DisplayName, Applicant, Note);
                                    MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, FormsID, FormNumber, "Msg", FormName, Content6, false, CanReceiveID);

                                }
                                //*****在自动受理成功后判断是否是自动结案End*****
                            }
                            else
                                flag = "更新审批状态“受理”步骤失败。错误代码：106<br/>The auto accept step of updating the approval status failed.";
                        }
                        else
                            flag = "受理失败，没有找到审批记录101。<br/>Acceptance failed. No approval record found.";
                    }
                    else
                        flag = "受理失败，您没有权限进行此操作。<br/>Acceptance failed. You do not have permission to do this operation";
                }
                else
                    flag = "受理失败，没有找到审批记录102。<br/>Acceptance failed. No approval record found.";
            }
            else
                flag = "受理失败，表单不在受理阶段。<br/>Acceptance failed. The form is not in the acceptance stage.";

        }
        catch (Exception ex)
        {
            flag = ex.ToString();
        }

        return flag;
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}