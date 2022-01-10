<%@ WebHandler Language="C#" Class="MicroFormWithdrawal" %>

using System;
using System.Web;
using System.Data;
using MicroPublicHelper;
using MicroWorkFlowHelper;
using MicroUserHelper;
using MicroFormHelper;
using MicroAuthHelper;
using MicroApprovalHelper;
using MicroNoticeHelper;
using MicroPrivateHelper;
using Newtonsoft.Json.Linq;

public class MicroFormWithdrawal : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = MicroPublic.GetMsg("SaveURLError"),
                Action = context.Server.UrlDecode(context.Request.Form["action"]),
                ShortTableName = context.Server.UrlDecode(context.Request.Form["stn"]),
                ModuleID = context.Server.UrlDecode(context.Request.Form["mid"]),
                FormID = context.Server.UrlDecode(context.Request.Form["formid"]),
                FormsID = context.Server.UrlDecode(context.Request.Form["pkv"]),
                Fields = context.Server.UrlDecode(context.Request.Form["fields"]);

        //测试数据
        //Action = "View";
        //ShortTableName = "ITPermsForm";
        //ModuleID = "4";
        //FormID = "3";
        //FormsID = "117";
        //Fields = context.Server.UrlDecode("%7B%22txtNote%22:%22%E5%8F%97%E7%90%86%E6%B5%8B%E8%AF%95%E6%92%A4%E5%9B%9E%22%7D");

        if (!string.IsNullOrEmpty(Action) && !string.IsNullOrEmpty(ShortTableName) && !string.IsNullOrEmpty(ModuleID) && !string.IsNullOrEmpty(FormID) && !string.IsNullOrEmpty(FormsID) && !string.IsNullOrEmpty(Fields))
            flag = SetWithdrawal(Action, ShortTableName, ModuleID, FormID, FormsID, Fields);

        context.Response.Write(flag);

    }


    private string SetWithdrawal(string Action, string ShortTableName, string ModuleID, string FormID, string FormsID, string Fields)
    {
        string flag = MicroPublic.GetMsg("WithdrawalFailed");

        var getApproval = MicroApproval.GetApproval(Action, ShortTableName, ModuleID, FormID, FormsID);
        Boolean isWithdrawal = getApproval.Withdrawal;

        if (isWithdrawal)
        {
            string UID = MicroUserInfo.GetUserInfo("UID"),
                    DisplayName = MicroUserInfo.GetUserInfo("DisplayName");

            dynamic json = JToken.Parse(Fields) as dynamic;
            string Note = json["txtNote"];  //审批备注
            Note = Note.toJsonTrim();

            var GetFormAttr = MicroForm.GetFormAttr(ShortTableName, FormID);
            string FormName = GetFormAttr.FormName;

            //获取上一审批节点（上一审批节点这是相对当前节点且未审批而言的）
            //***参考sql语句***
            //string _sql = "select * from FormApprovalRecords where Invalid=0 and Del=0 and FormID=@FormID and FormsID=@FormsID " +
            //        //得到当前表单已审批的节点的最大值
            //        "and FARID = (select max(FARID) from FormApprovalRecords where Invalid=0 and Del=0 and FormID=@FormID and FormsID=@FormsID and CanApprovalUID<>'' and ApprovalUID<>0) " +
            //        //同时判断审批者是否为当前登录用户
            //        "and ApprovalUID=@ApprovalUID";

            var getPreNodeAttr = MicroApproval.GetPreNodeAttr(FormID, FormsID);
            string PreFARID = getPreNodeAttr.FARID,
                    PreFormNumber = getPreNodeAttr.FormNumber,
                    PreNodeName = getPreNodeAttr.NodeName,
                    PreApprovalUID = getPreNodeAttr.ApprovalUID,
                    PreUID = getPreNodeAttr.UID,
                    PreDisplayName = getPreNodeAttr.DisplayName;

            string ApplicantID = PreUID,
                    Applicant = PreDisplayName,
                    FlowCode = string.Empty;

            var GetFormApprovalRecordsAttr = MicroWorkFlow.GetFormApprovalRecordsAttr(FormID, FormsID);
            DataTable _dt = GetFormApprovalRecordsAttr.SourceDT;

            if (_dt != null && _dt.Rows.Count > 0)
            {
                DataRow[] _rows = _dt.Select("CanApprovalUID<>'' and ApprovalUID<>0 and StateCode<>0", "Sort desc");
                if (_rows.Length > 0)
                    FlowCode = _rows[0]["FlowCode"].toStringTrim();
            }

            //如果是审批者、受理者、对应者撤回，表单回到上一状态
            //先判断当前登录者是不是已审批最大记录的审批者。 且 FlowCode!="Apply"  && FlowCode!="Finish"
            if (UID.toInt() == PreApprovalUID.toInt() && PreApprovalUID.toInt() != 0 && FlowCode != "Apply" && FlowCode != "Finish")
            {
                if (!MicroWorkFlow.SetFormState(ShortTableName, 15, FormID, FormsID))
                    flag = "更新表单状态失败。错误代码：113 <br/>The update of the form status failed.";
                else
                {
                    //通知审批者自己撤回成功
                    //Applicant + "提交的 " + FormName1 + "申请，编号：" + FormNumber + "，已被您撤回成功，已审批完毕的结果将失效，您可以重新审批该表单或驳回表单。" + Href;
                    string Content4 = MicroNotice.GetNoticeContent("ApprovalWithdrawalToApproval", ShortTableName, ModuleID, FormID, FormsID, PreFormNumber, FormName, "", PreNodeName, DisplayName, Applicant, Note);
                    MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, FormsID, PreFormNumber, "Msg", FormName, Content4, false, PreApprovalUID);

                    //通知当前正在等待审批的审批者
                    DataRow[] _rows2 = _dt.Select("CanApprovalUID<>'' and ApprovalUID=0 and StateCode=0", "Sort"); //如果是受理者或对就者时 考虑and FixedNode<>0
                    if (_rows2.Length > 0)
                    {
                        string CanApprovalUID = _rows2[0]["CanApprovalUID"].toStringTrim();  //未审批的取CanApprovalUID

                        //Applicant + "提交的 " + FormName1 + "申请，编号：" + FormNumber + "，已被本人撤回成功，该表单暂时不需要您审批请知悉，谢谢！" + Href;
                        string Content3 = MicroNotice.GetNoticeContent("ApplicantWithdrawalToNextApproval", ShortTableName, ModuleID, FormID, FormsID, PreFormNumber, FormName, "", PreNodeName, DisplayName, Applicant, Note);
                        MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, FormsID, PreFormNumber, "Msg", FormName, Content3, false, CanApprovalUID);

                    }

                    //更新FormApprovalRecords
                    MicroWorkFlow.SetFormApprovalRecords(PreApprovalUID, DisplayName, 15, "Note", PreFARID.toInt());

                    //审批结果更新表成功时，插入logs记录
                    MicroWorkFlow.SetApprovalLogs(FormID, FormsID, PreFormNumber, PreNodeName, UID, DisplayName, 15, Note);

                    flag = MicroPublic.GetMsg("Withdrawal");

                }

            }

            //如果上述不符合的话，则判断是否为申请者撤回，如果是表单回到申请状态
            else if (UID.toInt() == PreUID.toInt() && PreUID.toInt() != 0)
            {
                //检查一下是否已经有撤回的表单，如果有则先处理完再来撤回
                flag = MicroApproval.CheckWithdrawal(Action, ShortTableName, ModuleID, FormID, FormsID);
                if (flag.toBoolean())
                {
                    if (!MicroWorkFlow.SetFormState(ShortTableName, -4, FormID, FormsID))
                        flag = "更新表单状态失败。错误代码：113 <br/>The update of the form status failed.";
                    else
                    {
                        //撤回时返回被扣除的假期 条件(Action == "add" || Action == "modify" || Action == "return" || Action== "withdrawal") && ShortTableName == "Leave"
                        MicroPrivate.SubmitFormAfterExecutePrivate(Action, ShortTableName, ModuleID, FormID, true, Fields, FormsID);

                        //撤回成功通知自己
                        //"您提交的 " + FormName1 + "申请，编号：" + FormNumber + "，已撤回成功，如有已审批完毕的结果将失效，您可以修改表单内容或删除表单，如果需要继续请重新提交表单。" + Href;
                        string Content = MicroNotice.GetNoticeContent("ApplicantWithdrawalToApplicant", ShortTableName, ModuleID, FormID, FormsID, PreFormNumber, FormName, "", PreNodeName, DisplayName, Applicant, Note);
                        MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, FormsID, PreFormNumber, "Msg", FormName, Content, false, ApplicantID);

                        //通知已审批的审批者
                        DataRow[] _rows = _dt.Select("CanApprovalUID<>'' and ApprovalUID<>0 and ApprovalUID<>" + UID + " and StateCode<>0 ", "Sort"); //如果是受理者或对就者时 考虑and FixedNode<>0
                        if (_rows.Length > 0)
                        {
                            string ApprovaledUIDs = "0";
                            foreach (DataRow _dr in _rows)
                            {
                                string _ApprovaledUIDs = _dr["ApprovalUID"].toStringTrim();  //已审批的取ApprovalUID
                                                                                             //没有重复的情况下才追加
                                if (!MicroPublic.CheckSplitExists(ApprovaledUIDs, _ApprovaledUIDs, ','))
                                    ApprovaledUIDs = "," + _ApprovaledUIDs;
                            }
                            //Applicant + "提交的 " + FormName1 + "申请，编号：" + FormNumber + "，已被本人撤回成功，已审批完毕的结果将失效。" + Href;
                            string Content2 = MicroNotice.GetNoticeContent("ApplicantWithdrawalToApproval", ShortTableName, ModuleID, FormID, FormsID, PreFormNumber, FormName, "", PreNodeName, DisplayName, Applicant, Note);
                            MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, FormsID, PreFormNumber, "Msg", FormName, Content2, false, ApprovaledUIDs);
                        }

                        //通知当前正在等待审批的审批者
                        DataRow[] _rows2 = _dt.Select("CanApprovalUID<>'' and ApprovalUID=0 and StateCode=0", "Sort"); //如果是受理者或对就者时 考虑and FixedNode<>0
                        if (_rows2.Length > 0)
                        {
                            string CanApprovalUID = _rows2[0]["CanApprovalUID"].toStringTrim();  //未审批的取CanApprovalUID

                            //Applicant + "提交的 " + FormName1 + "申请，编号：" + FormNumber + "，已被本人撤回成功，该表单暂时不需要您审批请知悉，谢谢！" + Href;
                            string Content3 = MicroNotice.GetNoticeContent("ApplicantWithdrawalToNextApproval", ShortTableName, ModuleID, FormID, FormsID, PreFormNumber, FormName, "", PreNodeName, DisplayName, Applicant, Note);
                            MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, FormsID, PreFormNumber, "Msg", FormName, Content3, false, CanApprovalUID);
                        }

                        //更新FormApprovalRecords
                        MicroWorkFlow.SetFormApprovalRecords(PreUID, PreDisplayName, -4, Note, PreFARID.toInt());

                        //审批结果更新表成功时，插入logs记录
                        MicroWorkFlow.SetApprovalLogs(FormID, FormsID, PreFormNumber, PreNodeName, UID, DisplayName, -4, Note + "【申请者本人操作】");

                        flag = MicroPublic.GetMsg("Withdrawal");
                    }
                }
            }

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