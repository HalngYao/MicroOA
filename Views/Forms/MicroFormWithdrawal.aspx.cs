using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using MicroPublicHelper;
using MicroUserHelper;
using MicroWorkFlowHelper;
using MicroApprovalHelper;


public partial class Views_Forms_MicroFormWithdrawal : System.Web.UI.Page
{

    protected string GetHtmlCode = string.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {
        //动作Action 可选值Add、Modify、View
        string Action = MicroPublic.GetFriendlyUrlParm(0);
        txtAction.Value = Action;

        string ShortTableName = MicroPublic.GetFriendlyUrlParm(1);
        txtShortTableName.Value = ShortTableName;

        string ModuleID = MicroPublic.GetFriendlyUrlParm(2);
        txtMID.Value = ModuleID;

        string FormID = MicroPublic.GetFriendlyUrlParm(3);
        txtFormID.Value = FormID;

        string PrimaryKeyValue = MicroPublic.GetFriendlyUrlParm(4);
        txtPrimaryKeyValue.Value = PrimaryKeyValue;
        string FormsID = PrimaryKeyValue;

        string UID = MicroUserInfo.GetUserInfo("UID").toStringTrim();

        string htmlCode = string.Empty;

        if (!string.IsNullOrEmpty(Action) && !string.IsNullOrEmpty(ShortTableName) && !string.IsNullOrEmpty(ModuleID) && !string.IsNullOrEmpty(FormID) && !string.IsNullOrEmpty(PrimaryKeyValue))
        {
            string layuiDisabled = string.Empty, ctlDisabled = string.Empty, Tips = string.Empty, layVerify = string.Empty, layReqText = string.Empty, WithdrawalAction = "撤回表单。", WithdrawalAction2 = "フォームをキャンセル.", WithdrawalAction3 = "Withdraw form";

             //var GetFormAttr = MicroForm.GetFormAttr(ShortTableName, FormID);  //将来是否需要在表单表Forms上追加允许撤回开关
             //Boolean IsApproval = GetFormAttr.IsApproval;

             var getApproval = MicroApproval.GetApproval(Action, ShortTableName, ModuleID, FormID, PrimaryKeyValue);
            Boolean isWithdrawal = getApproval.Withdrawal;

            if (!isWithdrawal)
            {
                layuiDisabled = "layui-disabled";
                ctlDisabled = "disabled=\"\"";
                Tips = MicroPublic.GetMsg("TipWithdrawal");  //系统提示：该表单当前不在允许撤回阶段，不能撤回操作。<br/>現在、フォームは撤回を許可する段階になく、操作を撤回することはできません。<br/>Tip: The form is currently not in the stage of allowing withdrawal, and the operation cannot be withdrawn.
            }
            else
            {

                var GetFormApprovalRecordsAttr = MicroWorkFlow.GetFormApprovalRecordsAttr(FormID, FormsID);
                DataTable _dt = GetFormApprovalRecordsAttr.SourceDT;
                DataRow[] _rows = _dt.Select("CanApprovalUID<>'' and ApprovalUID<>0", "Sort desc");  // and StateCode=0 and FixedNode=0
                string PreUID = "0",
                    FlowCode = string.Empty;

                if(_rows.Length>0)
                {
                    PreUID = _rows[0]["UID"].toStringTrim();
                    FlowCode = _rows[0]["FlowCode"].toStringTrim();
                }


                if (FlowCode == "Apply")
                {
                    WithdrawalAction = "撤回申请。";
                    WithdrawalAction2 = "申請を取り下げる。";
                    WithdrawalAction3 = "Withdrawal of application.";
                }
                else if (FlowCode == "Approval")
                {
                    WithdrawalAction = "撤回审批。";
                    WithdrawalAction2 = "承認を撤回する。";
                    WithdrawalAction3 = "Withdrawal of approval.";
                }
                else if (FlowCode == "Accept")
                {
                    WithdrawalAction = "撤回受理。";
                    WithdrawalAction2 = "受理を撤回する。";
                    WithdrawalAction3 = "Withdrawal of acceptance.";
                }
                else if (FlowCode == "Finish")
                {
                    WithdrawalAction = "撤回已结案的表单。";
                    WithdrawalAction2 = "処理済みの書類を撤回する。";
                    WithdrawalAction3 = "Withdraw closed forms.";
                }


                //如果是申请者自己撤回，否则是审批者或对应者等撤回
                if (UID.toInt() != 0 && UID.toInt() == PreUID.toInt())
                    Tips = "温馨提示：撤回表单，表单将回到申请状态，如有已审批完毕的结果将失效。" +
                        "<br/>注意：フォームを取り消すと、フォームは申請状態に戻り、承認の結果は無効になります。" +
                        "<br/>Tips: if you withdraw the form, the form will return to the application status, and the approved results will be invalid.";
                else
                    Tips = "温馨提示：撤回表单，表单将回到上一流程节点状态，如有已审批完毕的结果将失效。" +
                        "<br/>注意：フォームを撤回すると、フォームは前のプロセスノードの状態に戻り、承認された結果は無効になります。" +
                        "<br/>Tips: if you withdraw the form, the form will return to the previous process node status, and the approved results will be invalid.";

            }


           
            if (Action.ToLower() == "withdrawal")
            {
                layVerify = "lay-verify=\"required\"";
                layReqText = "lay-reqtext=\"请注明撤回原因。<br/>Please indicate the reason for withdrawal\"";
            }

            //开始生成HTML代码
            htmlCode += "<div class=\"layui-card\">";
            htmlCode += "<div class=\"layui-card-body layui-form\">";


            htmlCode += "<blockquote class=\"layui-elem-quote\">动作：<span class=\"ws-font-red\">" + WithdrawalAction + "</span> &nbsp;&nbsp;動作：<span class=\"ws-font-red\">" + WithdrawalAction2 + "</span> &nbsp;&nbsp; Action: <span class=\"ws-font-red\">" + WithdrawalAction3 + "</span> </blockquote>";

            htmlCode += "<div class=\"layui-form-item layui-row layui-col-space5 ws-margin-top20\">";
            htmlCode += "<div class=\"layui-inline layui-col-xs12 layui-col-sm12 layui-col-md12 layui-col-lg12\">";
            htmlCode += "<label class=\"layui-form-label\"><i class=\"ws-want-asterisk\">&#42;</i> 备注/備考</label>";
            htmlCode += "<div class=\"layui-input-block\">";
            htmlCode += "<textarea id=\"txtNote\" name=\"txtNote\" placeholder=\"请输入内容 / 内容を入力してください\" " + layVerify + " " + layReqText + " class=\"layui-textarea " + layuiDisabled + "\" " + ctlDisabled + "></textarea>";
            htmlCode += "</div>";
            htmlCode += "</div>";
            htmlCode += "</div>";
            htmlCode += "<div class=\"layui-form-item\">";
            htmlCode += "<blockquote class=\"layui-elem-quote\" style=\"border-left-color:#FF5722;\"><span class=\"ws-font-red\">" + Tips + "</span></blockquote>";
            htmlCode += "</div>";
            htmlCode += "<div class=\"layui-form-item\">";
            htmlCode += "<div class=\"layui-input-block layui-hide\">";
            htmlCode += "<button type=\"button\" id=\"btnSave\" class=\"layui-btn " + layuiDisabled + "\" lay-submit=\"\" lay-filter=\"btnSave\" " + ctlDisabled + ">立即提交</button>";
            htmlCode += "</div>";
            htmlCode += "</div>";

            htmlCode += "</div>";
            htmlCode += "</div>";

        }
        else
            htmlCode = MicroPublic.GetFieldSet("错误提示 Error prompt", MicroPublic.GetMsg("DenyURLError"));

        GetHtmlCode = htmlCode;
    }


}