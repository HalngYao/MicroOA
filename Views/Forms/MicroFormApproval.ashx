<%@ WebHandler Language="C#" Class="MicroFormApproval" %>

using System;
using System.Web;
using MicroPublicHelper;
using Newtonsoft.Json.Linq;
using MicroWorkFlowHelper;
using MicroUserHelper;
using MicroFormHelper;
using MicroAuthHelper;
using MicroApprovalHelper;
using MicroNoticeHelper;
using MicroPrivateHelper;

public class MicroFormApproval : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = MicroPublic.GetMsg("SaveFailed"),
                Action = context.Server.UrlDecode(context.Request.Form["action"]),
                ShortTableName = context.Server.UrlDecode(context.Request.Form["stn"]),
                ModuleID = context.Server.UrlDecode(context.Request.Form["mid"]),
                FormID = context.Server.UrlDecode(context.Request.Form["formid"]),
                FormsID = context.Server.UrlDecode(context.Request.Form["pkv"]),
                Fields = context.Server.UrlDecode(context.Request.Form["fields"]);

        //测试数据
        //Action = "BatchAgree";
        //ShortTableName = "Overtime";
        //ModuleID = "4";
        //FormID = "5";
        //FormsID = "";
        //Fields = context.Server.UrlDecode("%7B%22txtNote%22:%22%22,%22txtFormsIDs%22:%2213041,13043%22,%22ckAllApproval%22:%22on%22%7D");


        if (!string.IsNullOrEmpty(Action))
        {
            Action = Action.toStringTrim().ToLower();
            try
            {
                if (Action == "agree" || Action == "return")
                {
                    if (!string.IsNullOrEmpty(ShortTableName) && !string.IsNullOrEmpty(ModuleID) && !string.IsNullOrEmpty(FormID) && !string.IsNullOrEmpty(FormsID) && !string.IsNullOrEmpty(Fields))
                        flag = SetApproval(Action, ShortTableName, ModuleID, FormID, FormsID, Fields);
                }

                if (Action == "batchagree" || Action == "batchreturn")
                {
                    if (!string.IsNullOrEmpty(ShortTableName) && !string.IsNullOrEmpty(ModuleID) && !string.IsNullOrEmpty(FormID) && !string.IsNullOrEmpty(Fields))
                    {
                        dynamic json = JToken.Parse(Fields) as dynamic;
                        string FormsIDs = json["txtFormsIDs"];
                        FormsIDs = FormsIDs.toStringTrim();
                        if (!string.IsNullOrEmpty(FormsIDs))
                            flag = SetBatchApproval(Action, ShortTableName, ModuleID, FormID, Fields, FormsIDs);
                    }
                }

            }
            catch (Exception ex)
            {
                flag = ex.ToString();
            }

        }

        context.Response.Write(flag);

    }


    /// <summary>
    /// 单个审批
    /// </summary>
    /// <param name="Action"></param>
    /// <param name="ShortTableName"></param>
    /// <param name="ModuleID"></param>
    /// <param name="FormID"></param>
    /// <param name="FormsID"></param>
    /// <param name="Fields"></param>
    /// <returns></returns>
    private string SetApproval(string Action, string ShortTableName, string ModuleID, string FormID, string FormsID, string Fields)
    {
        string flag =  MicroPublic.GetMsg("SaveFailed");

        //得到表单状态
        //审批状态：-40 = 删除[Delete]、-30 = 无效[Invalid]、-4 = 撤回[Withdrawal]、-3 = 填写申请[Fill in]、-2 = 临时保存[Draft]、-1 = 驳回申请[Return]、0 = 等待审批[Waiting]、1 = 审批通过[Pass]、11 = 提交申请[Pass]、15 = 撤回审批[WithdrawalApproval]、18 = 转发[Forward]、22 = 等待受理[WaitingForAccept]、23 = 等待对应[WaitingForProcessing]、24 = 等待处理[WaitingForProcessing]、32 = 受理中[Accepting]、33 = 对应中[Processing]、34 = 处理中[Processing]、100 = 完成[Finish]
        int FormStateCode = MicroWorkFlow.GetFormState(ShortTableName, FormID, FormsID);

        //代表在审批阶段时
        if (FormStateCode >= 0 && FormStateCode < 100)
        {
            if (Action == "agree")
                flag = MicroApproval.SetAgreeForm(ModuleID, ShortTableName, FormID, FormsID, Fields, FormStateCode);

            if (Action == "return")
            {
                flag = MicroApproval.SetReturnForm(ModuleID, ShortTableName, FormID, FormsID, Fields);

                //******执行私有方法Start******
                //暂时对休假申请执行，详细内容请看方法SubmitFormAfterExecutePrivate
                if (flag.Length > 4)
                {
                    //在提交表单成功后执行私有方法(判断flag前4位是否等于True)
                    if (flag.Substring(0, 4).toBoolean())
                        MicroPrivate.SubmitFormAfterExecutePrivate(Action, ShortTableName, ModuleID, FormID, true, Fields, FormsID);
                }
                //******执行私有方法End******

            }
        }
        //代表已驳回时
        else if (FormStateCode == -1)
            flag = MicroPublic.GetMsg("TipApprovalReturn");

        //代表其它情况时
        else
            flag = MicroPublic.GetMsg("TipCannotApproval");

        return flag;
    }


    /// <summary>
    /// 批量审批（这里只是调用了单条审批的逻辑，进行循环而已）
    /// </summary>
    /// <param name="Action"></param>
    /// <param name="ShortTableName"></param>
    /// <param name="ModuleID"></param>
    /// <param name="FormID"></param>
    /// <param name="FormsID"></param>
    /// <param name="Fields"></param>
    /// <param name="FormsIDs"></param>
    /// <returns></returns>
    private string SetBatchApproval(string Action, string ShortTableName, string ModuleID, string FormID, string Fields, string FormsIDs)
    {
        string flag = MicroPublic.GetMsg("SaveFailed"),
                ApprovalFailed = string.Empty,
                ApprovalReturn = string.Empty,  //进入审批介面前还没有被驳回，但在审批按钮保存时发现被驳回了
                NotInApprovalStage = string.Empty;  //进入审批介面前还在审批阶段，但在审批按钮保存时发现不在审批阶段了

        int Num = 0;
        string[] arrFormsIDs = FormsIDs.Split(',');

        if (arrFormsIDs.Length > 0)
        {
            var GetFormAttr = MicroForm.GetFormAttr(ShortTableName, FormID);
            string FormName = GetFormAttr.FormName;

            for (int i = 0; i < arrFormsIDs.Length; i++)
            {
                string FormsID = arrFormsIDs[i].toStringTrim();

                //得到表单状态
                //审批状态：-40 = 删除[Delete]、-30 = 无效[Invalid]、-4 = 撤回[Withdrawal]、-3 = 填写申请[Fill in]、-2 = 临时保存[Draft]、-1 = 驳回申请[Return]、0 = 等待审批[Waiting]、1 = 审批通过[Pass]、11 = 提交申请[Pass]、15 = 撤回审批[WithdrawalApproval]、18 = 转发[Forward]、22 = 等待受理[WaitingForAccept]、23 = 等待对应[WaitingForProcessing]、24 = 等待处理[WaitingForProcessing]、32 = 受理中[Accepting]、33 = 对应中[Processing]、34 = 处理中[Processing]、100 = 完成[Finish]

                var getFormRecordAttr = MicroWorkFlow.GetFormRecordAttr(ShortTableName, FormID, FormsID);
                int FormStateCode = getFormRecordAttr.StateCode; //MicroWorkFlow.GetFormState(ShortTableName, FormID, FormsID);
                string FormNumber = getFormRecordAttr.FormNumber;

                //代表在审批阶段时
                if (FormStateCode >= 0 && FormStateCode < 100)
                {
                    if (Action == "batchagree")
                        flag = MicroApproval.SetAgreeForm(ModuleID, ShortTableName, FormID, FormsID, Fields, FormStateCode);

                    if (Action == "batchreturn")
                    {
                        flag = MicroApproval.SetReturnForm(ModuleID, ShortTableName, FormID, FormsID, Fields);

                        //******执行私有方法Start******
                        //暂时对休假申请执行，详细内容请看方法SubmitFormAfterExecutePrivate
                        if (flag.Length > 4)
                        {
                            //在提交表单成功后执行私有方法(判断flag前4位是否等于True)
                            if (flag.Substring(0, 4).toBoolean())
                                MicroPrivate.SubmitFormAfterExecutePrivate(Action, ShortTableName, ModuleID, FormID, true, Fields, FormsID);
                        }
                        //******执行私有方法End******

                    }

                    //如果每条记录审批成功则+1
                    if (flag.Substring(0, 4).toBoolean())
                        Num = Num + 1;
                    else
                        ApprovalFailed += FormNumber + "、";

                }
                //代表已驳回时
                else if (FormStateCode == -1)
                {
                    flag = MicroPublic.GetMsg("TipApprovalReturn");
                    ApprovalReturn += FormNumber + "、";
                }
                //代表其它情况时
                else
                {
                    flag = MicroPublic.GetMsg("TipCannotApproval");
                    NotInApprovalStage += FormNumber;
                }
            }

            //任意表单有审批异常的情况（需要发送异常通知邮件）
            var Tips = string.Empty;
            if (!string.IsNullOrEmpty(ApprovalFailed) || !string.IsNullOrEmpty(ApprovalReturn) || !string.IsNullOrEmpty(NotInApprovalStage))
            {
                Tips = "*请注意：部分表单存在审批异常 ";
                if (!string.IsNullOrEmpty(ApprovalFailed))
                    Tips += "表单编号：" + ApprovalFailed.Substring(0, ApprovalFailed.Length - 1) + " 审批失败<br/>";

                if (!string.IsNullOrEmpty(ApprovalReturn))
                    Tips += "表单编号：" + ApprovalReturn.Substring(0, ApprovalReturn.Length - 1) + " 在审批前就已经被驳回<br/>";

                if (!string.IsNullOrEmpty(NotInApprovalStage))
                    Tips += "表单编号：" + NotInApprovalStage.Substring(0, NotInApprovalStage.Length - 1) + " 不在审批阶段<br/>";

                //发送审批异常邮件
                MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, "0", "批量审批", "Abnormal", FormName, Tips, false, MicroUserInfo.GetUserInfo("UID"));

            }

            //任意表单有审批成功时
            if (Num > 0)
                flag = MicroPublic.GetMsg("Save") + "<br/><br/>" + Tips;
            else
                flag += "<br/><br/>" + Tips;

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