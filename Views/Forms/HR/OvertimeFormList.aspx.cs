using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using MicroAuthHelper;
using MicroDBHelper;
using MicroDTHelper;
using MicroFormHelper;
using MicroPublicHelper;
using MicroUserHelper;
using MicroWorkFlowHelper;

public partial class Views_Forms_HR_OvertimeFormList : System.Web.UI.Page
{
    public string HtmlCode = "",
        JsXmSelectCode = "",
        divCol2HtmlCode = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            //动作Action 可选值Add、Modify、View
            string Action = MicroPublic.GetFriendlyUrlParm(0);
            txtAction.Value = Action;
            if (!string.IsNullOrEmpty(Action))
                Action = Action.ToLower();

            string ShortTableName = MicroPublic.GetFriendlyUrlParm(1);
            txtShortTableName.Value = ShortTableName;

            string ModuleID = MicroPublic.GetFriendlyUrlParm(2);
            txtMID.Value = ModuleID;

            string FormID = MicroPublic.GetFriendlyUrlParm(3);
            txtFormID.Value = FormID;


            var getFormAttr = MicroForm.GetFormAttr(ShortTableName, FormID);
            spanTitle.InnerHtml = getFormAttr.FormName + getFormAttr.Description;  //表单名称和描述
            spanWorkFlow.Visible = MicroAuth.CheckPermit(ModuleID, "3");  //是否显示修改流程
            string Note = getFormAttr.Note;
            if (!string.IsNullOrEmpty(Note))
            {
                divNote.Visible = true;
                spanNote.InnerHtml = Note;
            }

            string FormsID = MicroPublic.GetFriendlyUrlParm(4);
            if (!string.IsNullOrEmpty(FormsID))
                txtFormsID.Value = FormsID;

            //得到主键名称，主要用于点击LayuiDataTabel行时根据主键名称返回主键值
            var GetTableAttr = MicroDataTable.GetTableAttr(MicroPublic.GetTableName(ShortTableName));
            txtPrimaryKeyName.Value = "data." + GetTableAttr.PrimaryKeyName;

            //string OvertimeType = MicroPublic.GetFriendlyUrlParm(5);
            //txtOvertimeType.Value = OvertimeType;

            //string OperFieldConditionOperValue = string.Empty;
            //if (OvertimeType == "2")
            //    OperFieldConditionOperValue = "OvertimeTypeID:charindex:2;OvertimeTypeID:charindex:3";
            //else if(OvertimeType == "3")
            //    OperFieldConditionOperValue = "OvertimeTypeID:charindex:2;OvertimeTypeID:charindex:3";

            int StateCode = 0;
            Boolean IsRecordExists = false;

            if (!string.IsNullOrEmpty(ShortTableName) && !string.IsNullOrEmpty(FormID) && !string.IsNullOrEmpty(FormsID))
            {
                var getFormRecordAttr = MicroWorkFlow.GetFormRecordAttr(ShortTableName, FormID, FormsID);
                txtFormNumber.Value = getFormRecordAttr.FormNumber;
                txtFormState.Value = getFormRecordAttr.FormState;
                StateCode = getFormRecordAttr.StateCode;
                IsRecordExists = getFormRecordAttr.IsRecordExists;
            }

            //等于view或close时隐藏blockquote标签
            if (Action == "view" || Action == "close")
            {
                bqTitle.Visible = false;
                divClo1.Attributes.Add("class", "layui-col-xs6 layui-col-sm9 layui-col-md10 layui-col-lg10 ws-scrollbar");
                divClo1.Attributes.Add("style", "overflow:auto;");

                if (!string.IsNullOrEmpty(FormID) && !string.IsNullOrEmpty(FormsID))
                    divCol2HtmlCode = MicroForm.GetFormApprovalLogsHtmlCode(FormID, FormsID);

                divHeader.InnerHtml = MicroForm.GetFormWorkFlowStepsHtmlCode(FormID, FormsID);
            }
            else
                divClo1.Attributes.Add("class", "layui-col-xs12 layui-col-sm12 layui-col-md12 layui-col-lg12");


            //控制按钮
            if (Action == "add" || Action == "modify")
            {
                //读取流程
                var getWorkFlow = MicroWorkFlow.GetWorkFlow(Action, FormID, FormsID, ShortTableName);
                HtmlCode = getWorkFlow.HtmlCode;
                JsXmSelectCode = getWorkFlow.JsXmSelectCode;

                //如果非管理员
                if (!MicroUserInfo.CheckUserRole("Admins"))
                {
                    //如果记录存在时，且StateCode大于等于0说明记录已经提交上去，此时禁用按钮
                    if (IsRecordExists && StateCode >= 0)
                    {
                        btnAddOpenLink.Attributes.Add("class", "layui-btn layui-btn-xs layui-btn-disabled");
                        btnAddOpenLink.Disabled = true;

                        btnCopyAddOpenLink.Attributes.Add("class", "layui-btn layui-btn-xs layui-btn-disabled");
                        btnCopyAddOpenLink.Disabled = true;

                        btnModifyOpenLink.Attributes.Add("class", "layui-btn layui-btn-xs layui-btn-disabled");
                        btnModifyOpenLink.Disabled = true;

                        btnDel.Attributes.Add("class", "layui-btn layui-btn-xs layui-btn-disabled");
                        btnDel.Disabled = true;
                    }
                }

                if (Action == "modify")
                {
                    btnAddOpenLink.Attributes.Add("class", "layui-btn layui-btn-xs layui-btn-disabled");
                    btnAddOpenLink.Disabled = true;
                    btnAddOpenLink.Visible = false;

                    btnCopyAddOpenLink.Attributes.Add("class", "layui-btn layui-btn-xs layui-btn-disabled");
                    btnCopyAddOpenLink.Disabled = true;
                    btnCopyAddOpenLink.Visible = false;
                }

            }
            else
            {
                btnAddOpenLink.Attributes.Add("class", "layui-btn layui-btn-xs layui-btn-disabled");
                btnAddOpenLink.Disabled = true;

                btnCopyAddOpenLink.Attributes.Add("class", "layui-btn layui-btn-xs layui-btn-disabled");
                btnCopyAddOpenLink.Disabled = true;

                btnModifyOpenLink.Attributes.Add("class", "layui-btn layui-btn-xs layui-btn-disabled");
                btnModifyOpenLink.Disabled = true;

                btnDel.Attributes.Add("class", "layui-btn layui-btn-xs layui-btn-disabled");
                btnDel.Disabled = true;
            }

            //判断草稿箱是否有记录，若有记录要先处理掉才能进行新增操作
            string UID = MicroUserHelper.MicroUserInfo.GetUserInfo("UID");
            string _sql = "select * from HROvertime where Invalid=0 and Del=0 and StateCode>=-4 and StateCode<=-1 and UID=" + UID.toInt() + " and OvertimeTypeID=(select OvertimeTypeID from HROvertimeType where Invalid=0 and Del=0 and FormID=@FormID)";
            SqlParameter[] _sp = { new SqlParameter("@FormID", SqlDbType.Int) };
            _sp[0].Value = FormID.toInt();

            DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

            if (_dt != null && _dt.Rows.Count > 0)
            {
                if (Action == "add" && string.IsNullOrEmpty(FormsID))
                {
                    btnAddOpenLink.Attributes.Add("class", "layui-btn layui-btn-xs layui-btn-disabled");
                    btnAddOpenLink.Disabled = true;

                    btnCopyAddOpenLink.Attributes.Add("class", "layui-btn layui-btn-xs layui-btn-disabled");
                    btnCopyAddOpenLink.Disabled = true;

                    btnModifyOpenLink.Attributes.Add("class", "layui-btn layui-btn-xs layui-btn-disabled");
                    btnModifyOpenLink.Disabled = true;

                    btnDel.Attributes.Add("class", "layui-btn layui-btn-xs layui-btn-disabled");
                    btnDel.Disabled = true;

                    divTips.Visible = true;
                }

                else if ((Action == "add" || Action == "modify") && !string.IsNullOrEmpty(FormsID))
                {
                    DataRow[] _rows = _dt.Select("ParentID=0 and OvertimeID=" + FormsID.toInt() + " and Invalid=0", "Sort");
                    if (_rows.Length <= 0)
                    {
                        btnAddOpenLink.Attributes.Add("class", "layui-btn layui-btn-xs layui-btn-disabled");
                        btnAddOpenLink.Disabled = true;

                        btnCopyAddOpenLink.Attributes.Add("class", "layui-btn layui-btn-xs layui-btn-disabled");
                        btnCopyAddOpenLink.Disabled = true;

                        btnModifyOpenLink.Attributes.Add("class", "layui-btn layui-btn-xs layui-btn-disabled");
                        btnModifyOpenLink.Disabled = true;

                        btnDel.Attributes.Add("class", "layui-btn layui-btn-xs layui-btn-disabled");
                        btnDel.Disabled = true;

                        divTips.Visible = true;
                    }
                }

            }
        }
        catch { }
    }

}