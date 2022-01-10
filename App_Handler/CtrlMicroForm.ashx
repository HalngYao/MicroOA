<%@ WebHandler Language="C#" Class="CtrlMicroForm" %>

using System;
using System.Web;
using System.Data;
using MicroFormHelper;
using MicroDTHelper;
using MicroUserHelper;
using MicroPublicHelper;
using MicroWorkFlowHelper;
using Newtonsoft.Json.Linq;
using MicroAuthHelper;
using MicroNoticeHelper;
using MicroPrivateHelper;

public class CtrlMicroForm : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = MicroPublic.GetMsg("SaveURLError");

        string Action = context.Server.UrlDecode(context.Request.Form["action"]);
        string ShortTableName = context.Server.UrlDecode(context.Request.Form["stn"]);
        string ModuleID = context.Server.UrlDecode(context.Request.Form["mid"]);
        string FormID = context.Server.UrlDecode(context.Request.Form["formid"]);
        string IsApprovalForm = context.Server.UrlDecode(context.Request.Form["isapprovalform"]);
        //string PrimaryKeyValue = context.Server.UrlDecode(context.Request.Form["pkv"]);
        string FormFields = context.Server.UrlDecode(context.Request.Form["fields"]);

        IsApprovalForm = string.IsNullOrEmpty(IsApprovalForm) == true ? "False" : IsApprovalForm;
        FormID = IsApprovalForm.toBoolean() == false ? "0" : FormID;

        if (!string.IsNullOrEmpty(Action) && !string.IsNullOrEmpty(ShortTableName) && !string.IsNullOrEmpty(ModuleID) && !string.IsNullOrEmpty(FormID) && !string.IsNullOrEmpty(IsApprovalForm) && !string.IsNullOrEmpty(FormFields))
        {
            if (Action == "add")
            {
                //在提交表单前执行私有方法
                flag = MicroPrivate.SubmitFormBeforeExecutePrivate(Action, ShortTableName, ModuleID, FormID, IsApprovalForm.toBoolean(), FormFields);

                if (flag.Substring(0, 4).toBoolean())
                {
                    flag = MicroForm.SetSubmitForm(Action, ShortTableName, ModuleID, FormID, IsApprovalForm.toBoolean(), FormFields);

                    //******提交后执行私有方法Start******
                    //暂时对休假申请执行，详细内容请看方法SubmitFormAfterExecutePrivate
                    if (flag.Length > 4)
                    {
                        //在提交表单成功后执行私有方法(判断flag前4位是否等于True)
                        if (flag.Substring(0, 4).toBoolean())
                            MicroPrivate.SubmitFormAfterExecutePrivate(Action, ShortTableName, ModuleID, FormID, IsApprovalForm.toBoolean(), FormFields);
                    }
                    //******提交后执行私有方法End******
                }

            }

            if (Action == "modify")
            {
                //表单类型为审批类型表单时
                if (IsApprovalForm.toBoolean())
                {
                    //防止重复提交表单，表单为审批类型的表单，在修改情况下判断一下表单的状态，避免重复修改（原则上表单进入流程阶段或结案了是不允许修改的）  
                    string TableName = MicroPublic.GetTableName(ShortTableName);   //短表名得到长表名（即真实表名）
                    var getTableAttr = MicroDataTable.GetTableAttr(TableName);  //调用方法返回主键属性
                    string PrimaryKeyName = getTableAttr.PrimaryKeyName;  //赋值，主键字段名称
                    string ctlPrefix = getTableAttr.CtlPrefix; //赋值，控件前缀
                    string ctlPrimaryKeyName = ctlPrefix + PrimaryKeyName;  //得到表单页面主键的控件名称

                    dynamic json = JToken.Parse(FormFields) as dynamic;  //json转换
                    string FormsID = json[ctlPrimaryKeyName];  //得到主键的值

                    int StateCode = MicroWorkFlow.GetFormState(ShortTableName, FormID, FormsID);

                    //审批状态：0 = 等待审批[Waiting]、1 = 审批通过[Pass]、-1 = 驳回申请[Return]、-2 = 临时保存[Draft]、11 = 提交申请[Pass]、22 = 等待受理[WaitingForAccept]、23 = 等待对应[WaitingForProcessing]、24 = 等待处理[WaitingForProcessing]、32 = 受理中[Accepting]、33 = 对应中[Processing]、34 = 处理中[Processing]、100 = 完成[Finish]
                    if (StateCode >= 0)
                        flag = MicroPublic.GetMsg("TipApprovalSubmitted");  //"系统提示：该表单已提交过请勿重复提交。<br/>Tip: The form has been submitted and must not be submitted repeatedly."
                    else
                    {
                        //在提交表单前执行私有方法
                        flag = MicroPrivate.SubmitFormBeforeExecutePrivate(Action, ShortTableName, ModuleID, FormID, IsApprovalForm.toBoolean(), FormFields, FormsID);
                        if (flag.Substring(0, 4).toBoolean())
                            flag = MicroForm.SetModifyForm(Action, ShortTableName, ModuleID, FormID, IsApprovalForm.toBoolean(), FormFields);
                    }

                    //******执行私有方法Start******
                    //暂时对休假申请执行，详细内容请看方法SubmitFormAfterExecutePrivate
                    if (flag.Length > 4)
                    {
                        //在提交表单成功后执行私有方法(判断flag前4位是否等于True)
                        if (flag.Substring(0, 4).toBoolean())
                            MicroPrivate.SubmitFormAfterExecutePrivate(Action, ShortTableName, ModuleID, FormID, IsApprovalForm.toBoolean(), FormFields, FormsID);
                    }
                    //******执行私有方法End******

                }
                else
                    flag = MicroForm.SetModifyForm(Action, ShortTableName, ModuleID, FormID, IsApprovalForm.toBoolean(), FormFields);
            }

            if (Action == "close")
            {

                string TableName = MicroPublic.GetTableName(ShortTableName);   //短表名得到长表名（即真实表名）
                var getTableAttr = MicroDataTable.GetTableAttr(TableName);  //调用方法返回主键属性
                string PrimaryKeyName = getTableAttr.PrimaryKeyName;  //赋值，主键字段名称
                string CtlPrefix = getTableAttr.CtlPrefix; //赋值，控件前缀
                string ctlPrimaryKeyName = CtlPrefix + PrimaryKeyName;  //得到表单页面主键的控件名称

                dynamic json = JToken.Parse(FormFields) as dynamic;  //json转换
                string FormsID = json[ctlPrimaryKeyName];  //得到主键的值

                int StateCode = MicroWorkFlow.GetFormState(ShortTableName, FormID, FormsID);

                //审批状态：0 = 等待审批[Waiting]、1 = 审批通过[Pass]、-1 = 驳回申请[Return]、-2 = 临时保存[Draft]、11 = 提交申请[Pass]、22 = 等待受理[WaitingForAccept]、23 = 等待对应[WaitingForProcessing]、24 = 等待处理[WaitingForProcessing]、32 = 受理中[Accepting]、33 = 对应中[Processing]、34 = 处理中[Processing]、100 = 完成[Finish]
                if ((StateCode > 30 && StateCode < 40) || StateCode == 15)
                {
                    flag = MicroForm.SetModifyForm(Action, ShortTableName, ModuleID, FormID, IsApprovalForm.toBoolean(), FormFields);
                    //返回的结果为True时，更新表单状态，审批记录状态，写入日志
                    if (flag.toBoolean())
                    {
                        flag = MicroPublic.GetMsg("Closed");  //保存成功，该申请对应完成已关闭

                        int _StateCode = 100;
                        string ApprovalState = MicroWorkFlow.GetApprovalState(_StateCode);
                        //更新表单状态
                        if (!MicroWorkFlow.SetFormState(ShortTableName, _StateCode, FormID, FormsID))
                            flag = MicroPublic.GetMsg("SetFormStateFailed"); // "更新表单状态失败";
                        else
                        {
                            //更新审批记录表状态
                            var GetFormApprovalRecordsAttr = MicroWorkFlow.GetFormApprovalRecordsAttr(FormID, FormsID);
                            DataRow[] _rows = GetFormApprovalRecordsAttr.SourceRows;
                            string FormNumber = GetFormApprovalRecordsAttr.FormNumber;

                            if (_rows.Length > 0)
                            {
                                //_rows.Length - 1 作为最后一步
                                int FARID = _rows[_rows.Length - 1]["FARID"].toInt();
                                string NodeName = _rows[_rows.Length - 1]["NodeName"].toJsonTrim();
                                string UID = MicroUserInfo.GetUserInfo("UID"),
                                        DisplayName = MicroUserInfo.GetUserInfo("DisplayName");

                                if (!MicroWorkFlow.SetFormApprovalRecords(UID, DisplayName, _StateCode, "", FARID))
                                    flag = MicroPublic.GetMsg("SetFormStateFailed"); // "更新表单审批记录失败";            
                                else
                                {
                                    //插入日志
                                    //审批结果更新表成功时，插入logs记录
                                    if (!MicroWorkFlow.SetApprovalLogs(FormID, FormsID, FormNumber, NodeName, UID, DisplayName, _StateCode, ""))
                                        flag = MicroPublic.GetMsg("SetApprovalLogsFailed"); //更新日志记录失败
                                }

                                //*****结案时发送通知Start*****
                                var GetFormAttr = MicroForm.GetFormAttr(ShortTableName, FormID);
                                Boolean IsApproval = GetFormAttr.IsApproval;
                                Boolean AutoClose = GetFormAttr.AutoClose;
                                string FormName = GetFormAttr.FormName;
                                string _FormName = string.IsNullOrEmpty(FormName) == true ? "" : "《" + FormName + "》";

                                string ApplicantID = _rows[0]["UID"].toJsonTrim();

                                //string Content = "您提交的" + _FormName + "申请，编号：" + FormNumber + "，因为已经对应（处理）完成，所以进行结案。";
                                string Content = MicroNotice.GetNoticeContent("ClosedToApplicant", ShortTableName, ModuleID, FormID, FormsID, FormNumber, FormName, "", "", DisplayName, "", "");
                                MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, FormsID, FormNumber, "Msg", FormName, Content.toJsonTrim(), false, ApplicantID);

                                //*****结案时发送通知End*****
                            }
                        }
                    }
                }
                else
                    flag = MicroPublic.GetMsg("NotInClose"); //不在结案阶段

            }
        }

        context.Response.Write(flag);
    }


    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}