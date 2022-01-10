using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroDTHelper;
using MicroPublicHelper;
using MicroFormHelper;
using MicroWorkFlowHelper;
using MicroUserHelper;
using MicroAuthHelper;

public partial class Views_Forms_MicroFormApproval : System.Web.UI.Page
{

    protected string GetHtmlCode = MicroPublic.GetFieldSet("错误提示 Error prompt", MicroPublic.GetMsg("DenyURLError"));
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

        string htmlCode = MicroPublic.GetFieldSet("错误提示 Error prompt", MicroPublic.GetMsg("DenyURLError"));

        if (!string.IsNullOrEmpty(Action) && !string.IsNullOrEmpty(ShortTableName) && !string.IsNullOrEmpty(ModuleID) && !string.IsNullOrEmpty(FormID))
        {
            if (!string.IsNullOrEmpty(PrimaryKeyValue))
                htmlCode = GetApprovalHtmlCode(Action.Trim(), ShortTableName.Trim(), ModuleID.Trim(), FormID.Trim(), PrimaryKeyValue.Trim());
            else
            {
                if (Action.ToLower() == "batchagree" || Action.ToLower() == "batchreturn")
                    htmlCode = GetBatchApprovalHtmlCode(Action.Trim(), ShortTableName.Trim(), ModuleID.Trim(), FormID.Trim());
            }
        }

        GetHtmlCode = htmlCode;
    }

    /// <summary>
    /// 单个审批
    /// </summary>
    /// <param name="Action"></param>
    /// <param name="ShortTableName"></param>
    /// <param name="ModuleID"></param>
    /// <param name="FormID"></param>
    /// <param name="PrimaryKeyValue"></param>
    /// <returns></returns>
    private string GetApprovalHtmlCode(string Action, string ShortTableName, string ModuleID, string FormID, string PrimaryKeyValue)
    {
        string htmlCode = string.Empty, htmlCodeContinuous = string.Empty,
        layuiDisabled = string.Empty, ctlDisabled = string.Empty, Tips = string.Empty, layVerify = string.Empty, layReqText = string.Empty,
        UID = MicroUserInfo.GetUserInfo("UID");

        var GetFormAttr = MicroForm.GetFormAttr(ShortTableName, FormID);
        Boolean IsApproval = GetFormAttr.IsApproval;

        if (Action.ToLower() == "return")
        {
            layVerify = "lay-verify=\"required\"";
            layReqText = "lay-reqtext=\"驳回申请时请注明原因。<br/>Please indicate the reason when rejecting the application.\"";
        }

        //表单不需要审批时
        if (!IsApproval)
        {
            layuiDisabled = "layui-disabled";
            ctlDisabled = "disabled=\"\"";
            Tips = MicroPublic.GetMsg("TipNoApprovalRequired"); //"系统提示：该表单不需要审批。<br/>Tip: The form does not require approval.";
        }
        else
        {
            //得到表单状态
            //审批状态：-40 = 删除[Delete]、-30 = 无效[Invalid]、-4 = 撤回[Withdrawal]、-3 = 填写申请[Fill in]、-2 = 临时保存[Draft]、-1 = 驳回申请[Return]、0 = 等待审批[Waiting]、1 = 审批通过[Pass]、11 = 提交申请[Pass]、15 = 撤回审批[WithdrawalApproval]、18 = 转发[Forward]、22 = 等待受理[WaitingForAccept]、23 = 等待对应[WaitingForProcessing]、24 = 等待处理[WaitingForProcessing]、32 = 受理中[Accepting]、33 = 对应中[Processing]、34 = 处理中[Processing]、100 = 完成[Finish]
            int FormStateCode = MicroWorkFlow.GetFormState(ShortTableName, FormID, PrimaryKeyValue);
            //代表在审批阶段时
            if (FormStateCode >= 0 && FormStateCode < 100)
            {
                var GetFormApprovalRecordsAttr = MicroWorkFlow.GetFormApprovalRecordsAttr(FormID, PrimaryKeyValue);
                DataTable _dt = GetFormApprovalRecordsAttr.SourceDT;

                DataRow[] _rows = _dt.Select("CanApprovalUID<>'' and ApprovalUID=0 and StateCode=0 and FixedNode=0", "Sort");

                //如果是处于撤回审批的状态（通常由审批者撤回自己的审批时）
                if (FormStateCode == 15)
                {
                    if (MicroAuth.CheckPermit(ModuleID, "12"))
                        _rows = _dt.Select("CanApprovalUID<>'' and StateCode=15 and FixedNode=0", "Sort");

                    else
                        _rows = _dt.Select("CanApprovalUID<>'' and ApprovalUID=" + UID + " and StateCode=15 and FixedNode=0", "Sort");
                }


                //如果有审批记录表有审批记录需要审批时
                if (_rows.Length > 0)
                {
                    //判断是否有权限审批（CanApprovalUID与当前登录用户UID比较）
                    Boolean Perm = false;
                    //判断是否有连续审批
                    Boolean isContinuous = true;
                    //有连续审批时把审批节点插入sb，没有的话插入第一个
                    StringBuilder sbNodeName = new StringBuilder();

                    for (int i = 0; i < _rows.Length; i++)
                    {

                        if (i == 0)
                        {
                            string iCanApprovalUID = _rows[i]["CanApprovalUID"].toJsonTrim();

                            //检查是否有审批权限
                            if (MicroPublic.CheckSplitExists(iCanApprovalUID, UID, ','))
                            {
                                //如果有，把第一条记录插入sb
                                sbNodeName.Append(_rows[i]["NodeName"].toStringTrim() + "、");
                                Perm = true;  //赋予审批权限
                            }
                            else
                                Perm = false;
                        }


                        //审批动作为同意时才判断是否有连续审批，驳回动作时不需要
                        if (Action.ToLower() == "agree")
                        {
                            if (isContinuous)
                            {
                                //*****得到连续审批的节点名称Start*****
                                int j = i + 1;
                                if (j < _rows.Length)
                                {
                                    string jCanApprovalUID = _rows[j]["CanApprovalUID"].toJsonTrim();

                                    //每次都和第一条进行比较，判断是否为连续审批
                                    if (MicroPublic.CheckSplitExists(_rows[0]["CanApprovalUID"].toJsonTrim(), UID, ',') && MicroPublic.CheckSplitExists(jCanApprovalUID, UID, ','))
                                        sbNodeName.Append(_rows[j]["NodeName"].toStringTrim() + "、");  //得到连续审批的节点名称，插入到sb
                                                                                                       //不是连续审批的话中断
                                    else
                                        isContinuous = false;
                                }
                                //*****得到连续审批的节点名称End*****
                            }

                        }
                    }

                    //有审批权限时（即表单提交时选定的审批人员）
                    if (Perm)
                    {
                        string NodeName = sbNodeName.ToString();
                        NodeName = NodeName.Substring(0, NodeName.Length - 1);

                        int ApprovalSum = NodeName.Split('、').Length;
                        //审批个数大于1时提示有连续审批
                        if (ApprovalSum > 1)
                        {
                            Tips = "温馨提示：该表单您有 " + ApprovalSum + " 个连续的审批：" + "【" + NodeName + "】，是否一起审批？<br/>" +
                                "暖かいヒント：こちらのフォームには、貴方が" + ApprovalSum + " つの連続的な承認となります：【" + NodeName + "】、一括に承認行いますか ?<br/>" +
                                "Warm prompt: You have " + ApprovalSum + " approvals for this form, Do you approve together?";

                            //显示一起审批的Checkbox
                            htmlCodeContinuous += "<div class=\"layui-form-item\">";
                            htmlCodeContinuous += "<label class=\"layui-form-label\"></label>";
                            htmlCodeContinuous += "<div class=\"layui-input-block\">";
                            htmlCodeContinuous += "<input type =\"checkbox\" id=\"ckAllApproval\" name=\"ckAllApproval\" checked=\"checked\" lay-skin=\"primary\" title=\"一起审批 / 一括に承認する / Approve together\">";
                            htmlCodeContinuous += "</div>";
                            htmlCodeContinuous += "</div>";
                        }
                    }
                    //没有审批权限时（即不是表单提交时选定的审批人员）
                    else
                    {
                        //没有审批权限时同时也没有设定的审批权限时，禁用审批提交按钮
                        if (!MicroAuth.CheckPermit(ModuleID, "12"))
                        {
                            layuiDisabled = "layui-disabled";
                            ctlDisabled = "disabled=\"\"";
                            Tips = MicroPublic.GetMsg("TipNoPermApproval"); //"系统提示：检测到您没有权限审批该表单。<br/>Tip: You are detected that you do not have permission to approve the form.";
                        }
                    }

                } // if (_rows.Length > 0) //没有审批记录时，直接禁用按钮
                else
                {
                    layuiDisabled = "layui-disabled";
                    ctlDisabled = "disabled=\"\"";

                    if (FormStateCode == 15)
                        Tips = MicroPublic.GetMsg("TipNoPermApproval"); //"系统提示：检测到您没有权限审批该表单。<br/>Tip: You are detected that you do not have permission to approve the form.";
                    else
                        Tips = MicroPublic.GetMsg("TipApproved");  //"系统提示：该表单已审批过或不需要审批。<br/>Tip: The form has been approved or does not require approval.";

                }

            }
            //代表已驳回时
            else if (FormStateCode == -1)
            {
                layuiDisabled = "layui-disabled";
                ctlDisabled = "disabled=\"\"";
                Tips = MicroPublic.GetMsg("TipApprovalReturn");  //"系统提示：该表单已被驳回，不能进行审批操作。<br/>Tip: The form has been rejected and cannot be approved.";
            }
            //代表其它情况时
            else
            {
                layuiDisabled = "layui-disabled";
                ctlDisabled = "disabled=\"\"";
                Tips = MicroPublic.GetMsg("TipCannotApproval");  //"系统提示：该表单不在审批阶段，不能进行审批操作。<br/>Tip: This form has been rejected Do not duplicate approval."
            }
        }


        //开始生成HTML代码
        htmlCode += "<div class=\"layui-card\">";
        htmlCode += "<div class=\"layui-card-body layui-form\">";


        if (Action.ToLower() == "agree")
            htmlCode += "<blockquote class=\"layui-elem-quote\">审批表单：<span class=\"ws-font-green2\">同意申请。</span> &nbsp;&nbsp;承認フォーム：<span class=\"ws-font-green2\">申請に同意します。</span> &nbsp;&nbsp;Approval form: <span class=\"ws-font-green2\">agree application.</span> </blockquote>";
        else
            htmlCode += "<blockquote class=\"layui-elem-quote\">审批表单：<span class=\"ws-font-red\">驳回申请。</span> &nbsp;&nbsp;承認フォーム：<span class=\"ws-font-red\">申請を却下します。</span> &nbsp;&nbsp; Approve form: <span class=\"ws-font-red\">reject application.</span> </blockquote>";

        htmlCode += "<div class=\"layui-form-item layui-row layui-col-space5 ws-margin-top20\">";
        htmlCode += "<div class=\"layui-inline layui-col-xs12 layui-col-sm12 layui-col-md12 layui-col-lg12\">";
        htmlCode += "<label class=\"layui-form-label\"><i class=\"ws-want-asterisk\">&#42;</i> 备注/備考</label>";
        htmlCode += "<div class=\"layui-input-block\">";
        htmlCode += "<textarea id=\"txtNote\" name=\"txtNote\" placeholder=\"请输入内容 / 内容を入力してください\" " + layVerify + " " + layReqText + " class=\"layui-textarea " + layuiDisabled + "\" " + ctlDisabled + "></textarea>";
        htmlCode += "</div>";
        htmlCode += "</div>";
        htmlCode += "</div>";
        htmlCode += htmlCodeContinuous;
        htmlCode += "<div class=\"layui-form-item\">";

        if (!string.IsNullOrEmpty(Tips))
            htmlCode += "<blockquote class=\"layui-elem-quote\" style=\"border-left-color:#FF5722;\"><span class=\"ws-font-red\">" + Tips + "</span></blockquote>";

        htmlCode += "</div>";
        htmlCode += "<div class=\"layui-form-item\">";
        htmlCode += "<div class=\"layui-input-block layui-hide\">";
        htmlCode += "<button type=\"button\" id=\"btnSave\" class=\"layui-btn " + layuiDisabled + "\" lay-submit=\"\" lay-filter=\"btnSave\" " + ctlDisabled + ">立即提交</button>";
        htmlCode += "</div>";
        htmlCode += "</div>";


        htmlCode += "</div>";
        htmlCode += "</div>";


        return htmlCode;

    }


    /// <summary>
    /// 批量审批
    /// </summary>
    /// <param name="Action"></param>
    /// <param name="ShortTableName"></param>
    /// <param name="ModuleID"></param>
    /// <param name="FormID"></param>
    /// <param name="PrimaryKeyValue"></param>
    /// <returns></returns>
    private string GetBatchApprovalHtmlCode(string Action, string ShortTableName, string ModuleID, string FormID)
    {
        string htmlCode = string.Empty, htmlCodeContinuous = string.Empty,
        layuiDisabled = string.Empty, ctlDisabled = string.Empty, Tips = string.Empty, Tips2 = string.Empty, layVerify = string.Empty, layReqText = string.Empty,
        FormsIDs = string.Empty,
        NewFormsIDs = string.Empty,  //通过下面的遍历检查每条记录是否有审批权限，把有审批权限的FormsID取出构造成新的FormsIDs
        UID = MicroUserInfo.GetUserInfo("UID");

        var GetFormAttr = MicroForm.GetFormAttr(ShortTableName, FormID);
        Boolean IsApproval = GetFormAttr.IsApproval;

        //表单不需要审批时
        if (!IsApproval)
        {
            layuiDisabled = "layui-disabled";
            ctlDisabled = "disabled=\"\"";
            Tips = MicroPublic.GetMsg("TipNoApprovalRequired"); //"系统提示：该表单不需要审批。<br/>Tip: The form does not require approval.";
        }
        else
        {
            //获取到批量审批的主键
            string _sqlkey = "select FormsIDs from FormBatchApprovalKey where Invalid=0 and Del=0 and FBAKID=(select max(FBAKID) as FBAKID from FormBatchApprovalKey where Invalid=0 and Del=0 and ApprovalAction=@ApprovalAction and ShortTableName=@ShortTableName and FormID=@FormID and UID=@UID and DateCreated>@DateCreated)";

            SqlParameter[] _spkey ={ new SqlParameter("@ApprovalAction",SqlDbType.VarChar,50),
            new SqlParameter("@ShortTableName",SqlDbType.VarChar,100),
            new SqlParameter("@FormID",SqlDbType.Int),
            new SqlParameter("@UID",SqlDbType.Int),
            new SqlParameter("@DateCreated",SqlDbType.DateTime),
                };

            _spkey[0].Value = Action.toStringTrim();
            _spkey[1].Value = ShortTableName.toStringTrim();
            _spkey[2].Value = FormID.toInt();
            _spkey[3].Value = UID.toInt();
            _spkey[4].Value = DateTime.Now.AddMinutes(-3); //大于Now-1分钟，让差距不太大

            DataTable _dtkey = MsSQLDbHelper.Query(_sqlkey, _spkey).Tables[0];

            if (_dtkey != null && _dtkey.Rows.Count > 0)
                FormsIDs = _dtkey.Rows[0]["FormsIDs"].toStringTrim();

            //批量审批主键表值不为空时
            if (!string.IsNullOrEmpty(FormsIDs))
            {
                if (Action.ToLower() == "batchreturn")
                {
                    layVerify = "lay-verify=\"required\"";
                    layReqText = "lay-reqtext=\"驳回申请时请注明原因。<br/>Please indicate the reason when rejecting the application.\"";
                }

                string TableName = MicroPublic.GetTableName(ShortTableName);
                var GetTableAttr = MicroDataTable.GetTableAttr(TableName);
                string PrimaryKeyName = GetTableAttr.PrimaryKeyName,
                    NoPermFormNumbers = string.Empty,  //没有权限或已审批过的表单
                    ReturnFormNumbers = string.Empty, //已被驳回的表单
                    NotInFormNumbers = string.Empty; //不要审批阶段的表单

                //如果有审批权限则+1，没有则不加，到最后也是等于0的话，说明所有记录都没有权限，需要禁用提交按钮
                int Num = 0;

                string _sql2 = "select * from " + TableName + " where Invalid=0 and Del=0 and FormID = @FormID and " + PrimaryKeyName + " in (" + FormsIDs + ") ";
                SqlParameter[] _sp2 = { new SqlParameter("@FormID", SqlDbType.Int) };
                _sp2[0].Value = FormID.toInt();

                DataTable _dt2 = MsSQLDbHelper.Query(_sql2, _sp2).Tables[0];

                if (_dt2 != null && _dt2.Rows.Count > 0)
                {
                    string _sql3 = "select a.*,b.WFID ,b.FlowName ,b.FlowCode ,b.Alias ,b.EffectiveType ,b.EffectiveIDStr ,b.IsConditionApproval ,b.OperField ,b.Condition ,b.OperValue ,b.CustomConditions ,b.ApprovalType ,b.ApprovalIDStr ,b.ApprovalByIDStr ,b.IsSync ,b.Creator ,b.DefaultFlow ,b.FixedNode ,b.Invalid ,b.Del ,b.IsAccept ,b.ApproversSelectedByDefault ,b.ExtraFunction ,b.IsOptionalApproval ,b.IsSpecialApproval ,b.IsVerticalDirection ,b.Description from FormApprovalRecords a left join WorkFlow b on a.WorkFlowID=b.WFID where a.Invalid=0 and a.Del=0 and a.FormID = @FormID and a.FormsID in (" + FormsIDs + ") order by a.FormsID,a.Sort ";
                    SqlParameter[] _sp3 = { new SqlParameter("@FormID", SqlDbType.Int) };
                    _sp3[0].Value = FormID.toInt();

                    DataTable _dt3 = MsSQLDbHelper.Query(_sql3, _sp3).Tables[0];

                    for (int i = 0; i < _dt2.Rows.Count; i++)
                    {
                        int FormsID = _dt2.Rows[i][PrimaryKeyName].toInt(),
                        FormStateCode = _dt2.Rows[i]["StateCode"].toInt();

                        string FormNumber = _dt2.Rows[i]["FormNumber"].toStringTrim();

                        //代表在审批阶段时
                        if (FormStateCode >= 0 && FormStateCode < 100)
                        {
                            DataRow[] _rows3 = _dt3.Select("FormsID=" + FormsID + " and CanApprovalUID<>'' and ApprovalUID=0 and StateCode=0 and FixedNode=0", "Sort");

                            //如果是处于撤回审批的状态（通常由审批者撤回自己的审批时）
                            if (FormStateCode == 15)
                            {
                                if (MicroAuth.CheckPermit(ModuleID, "12"))
                                    _rows3 = _dt3.Select("FormsID=" + FormsID + " and CanApprovalUID<>'' and StateCode=15 and FixedNode=0", "Sort");

                                else
                                    _rows3 = _dt3.Select("FormsID=" + FormsID + " and CanApprovalUID<>'' and ApprovalUID=" + UID + " and StateCode=15 and FixedNode=0", "Sort");
                            }


                            //如果有审批记录表有审批记录需要审批时
                            if (_rows3.Length > 0)
                            {
                                string iCanApprovalUID = _rows3[0]["CanApprovalUID"].toJsonTrim();
                                //检查是否有审批权限
                                if (MicroPublic.CheckSplitExists(iCanApprovalUID, UID, ',') || MicroAuth.CheckPermit(ModuleID, "12"))
                                {
                                    NewFormsIDs += FormsID.toStringTrim() + ",";  //如果有审批权限则取出FormsID
                                    Num = Num + 1;
                                }
                                else
                                    NoPermFormNumbers += FormNumber + "、";  //否则你没有审批权限则取出表单编号

                            }
                            //否则没有找到需要审批的节点(这种情况通常是你打开了审批页面没有审批，而又开了另一个审批页面进行审批)
                            else
                                NotInFormNumbers += FormNumber + "、"; 

                        }
                        //代表已驳回时
                        else if (FormStateCode == -1)
                            ReturnFormNumbers += FormNumber + "、";  //已被驳回的表单取出表单编号

                        //代表其它情况时
                        else
                            NotInFormNumbers += FormNumber + "、";
                    }

                    if (Num > 0)
                    {
                        Tips = "温馨提示：如果表单存在多个连续的审批节点是否一起审批？<br/>" +
                            "暖かいヒント：フォームに複数の承認ノードが連続して存在する場合、一緒に承認しますか?<br/>" +
                            "Warm prompt: If there are multiple successive approval nodes in the form, do you want to approve them together?";

                        //显示一起审批的Checkbox
                        htmlCodeContinuous += "<div class=\"layui-form-item\">";
                        htmlCodeContinuous += "<label class=\"layui-form-label\"></label>";
                        htmlCodeContinuous += "<div class=\"layui-input-block\">";
                        htmlCodeContinuous += "<input type =\"checkbox\" id=\"ckAllApproval\" name=\"ckAllApproval\" checked=\"checked\" lay-skin=\"primary\" title=\"一起审批 / 一括に承認する / Approve together\">";
                        htmlCodeContinuous += "</div>";
                        htmlCodeContinuous += "</div>";
                    }
                    else
                    {
                        layuiDisabled = "layui-disabled";
                        ctlDisabled = "disabled=\"\"";
                        Tips = MicroPublic.GetMsg("TipAllNoApprovalRequired");
                    }


                    if (!string.IsNullOrEmpty(NoPermFormNumbers) || !string.IsNullOrEmpty(ReturnFormNumbers) || !string.IsNullOrEmpty(NotInFormNumbers))
                    {
                        Tips2 = "*注意：系统检测到";
                        if (!string.IsNullOrEmpty(NoPermFormNumbers))
                            Tips2 += "表单编号：" + NoPermFormNumbers.Substring(0, NoPermFormNumbers.Length - 1) + " 已审批过或您没有审批权限<br/>";

                        if (!string.IsNullOrEmpty(ReturnFormNumbers))
                            Tips2 += "表单编号：" + ReturnFormNumbers.Substring(0, ReturnFormNumbers.Length - 1) + " 已被驳回审批<br/>";

                        if (!string.IsNullOrEmpty(NotInFormNumbers))
                            Tips2 += "表单编号：" + NotInFormNumbers.Substring(0, NotInFormNumbers.Length - 1) + " 不在审批阶段<br/>";

                        if (Num > 0)
                            Tips2 += "如 “继续审批” 系统则自动跳过这些表单，如 “放弃审批” 则关闭此窗口即可，请注意！<br/>";

                        Tips2 += "<br/>";
                    }


                    if (!string.IsNullOrEmpty(NewFormsIDs))
                        NewFormsIDs = NewFormsIDs.Substring(0, NewFormsIDs.Length - 1);
                }
            }

        }
        //开始生成HTML代码
        htmlCode = "<div class=\"layui-card\">";
        htmlCode += "<div class=\"layui-card-body layui-form\">";

        if (Action.ToLower() == "batchagree")
            htmlCode += "<blockquote class=\"layui-elem-quote\">审批表单：<span class=\"ws-font-green2\">批量同意申请。</span> &nbsp;&nbsp;承認フォーム：<span class=\"ws-font-green2\">一括同意申請。</span> &nbsp;&nbsp;Approval form: <span class=\"ws-font-green2\">Batch agree application.</span> </blockquote>";
        else
            htmlCode += "<blockquote class=\"layui-elem-quote\">审批表单：<span class=\"ws-font-red\">批量驳回申请。</span> &nbsp;&nbsp;承認フォーム：<span class=\"ws-font-red\">一括却下申請。</span> &nbsp;&nbsp; Approve form: <span class=\"ws-font-red\">Batch reject application.</span> </blockquote>";

        htmlCode += "<div class=\"layui-form-item layui-row layui-col-space5 ws-margin-top20\">";
        htmlCode += "<div class=\"layui-inline layui-col-xs12 layui-col-sm12 layui-col-md12 layui-col-lg12\">";
        htmlCode += "<label class=\"layui-form-label\"><i class=\"ws-want-asterisk\">&#42;</i> 备注/備考</label>";
        htmlCode += "<div class=\"layui-input-block\">";
        htmlCode += "<textarea id=\"txtNote\" name=\"txtNote\" placeholder=\"请输入内容 / 内容を入力してください\" " + layVerify + " " + layReqText + " class=\"layui-textarea " + layuiDisabled + "\" " + ctlDisabled + "></textarea>";
        htmlCode += "<input type=\"text\" id=\"txtFormsIDs\" name=\"txtFormsIDs\" lay-verify=\"required\" lay-reqtext=\"批量审批识别码为空，禁止提交。<br/>The ID code is empty and cannot be submitted.\" class=\"layui-input layui-hide\" value=\"" + NewFormsIDs + "\">";
        htmlCode += "</div>";
        htmlCode += "</div>";
        htmlCode += "</div>";
        htmlCode += htmlCodeContinuous;
        htmlCode += "<div class=\"layui-form-item\">";

        if (!string.IsNullOrEmpty(Tips) || !string.IsNullOrEmpty(Tips2))
            htmlCode += "<blockquote class=\"layui-elem-quote\" style=\"border-left-color:#FF5722;\"><span class=\"ws-font-red\">" + Tips2 + Tips + "</span></blockquote>";

        htmlCode += "</div>";
        htmlCode += "<div class=\"layui-form-item\">";
        htmlCode += "<div class=\"layui-input-block layui-hide\">";
        htmlCode += "<button type=\"button\" id=\"btnSave\" class=\"layui-btn " + layuiDisabled + "\" lay-submit=\"\" lay-filter=\"btnSave\" " + ctlDisabled + ">立即提交</button>";
        htmlCode += "</div>";
        htmlCode += "</div>";


        htmlCode += "</div>";
        htmlCode += "</div>";


        return htmlCode;

    }


}