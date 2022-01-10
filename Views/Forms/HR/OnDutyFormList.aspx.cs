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
using MicroWorkFlowHelper;

public partial class Views_Forms_HR_OnDutyFormList : System.Web.UI.Page
{
    public string HtmlCode = "",
    JsXmSelectCode = "",
    divCol2HtmlCode = "";

    protected void Page_Load(object sender, EventArgs e)
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

        if (!string.IsNullOrEmpty(ShortTableName) && !string.IsNullOrEmpty(FormID) && !string.IsNullOrEmpty(FormsID))
        {
            var getFormRecordAttr = MicroWorkFlow.GetFormRecordAttr(ShortTableName, FormID, FormsID);
            txtFormNumber.Value = getFormRecordAttr.FormNumber;
            txtFormState.Value = getFormRecordAttr.FormState;
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
        {
            divClo1.Attributes.Add("class", "layui-col-xs12 layui-col-sm12 layui-col-md12 layui-col-lg12");
        }

        if (Action == "add" || Action == "modify")
        {
            var getWorkFlow = MicroWorkFlow.GetWorkFlow(Action, FormID, FormsID, ShortTableName);
            HtmlCode = getWorkFlow.HtmlCode;
            JsXmSelectCode = getWorkFlow.JsXmSelectCode;

            if (Action == "add")
            {
                btnDel.Attributes.Add("class", "layui-btn layui-btn-xs layui-btn-disabled");
                btnDel.Disabled = true;
                divTips.InnerHtml = "温馨提示：您可以通过 “模板下载” 取得导入文件需要的格式";
            }

            if (Action == "modify")
            {
                btnAddOpenLink.Attributes.Add("class", "layui-btn layui-btn-xs layui-btn-disabled");
                btnAddOpenLink.Disabled = true;
                btnAddOpenLink.Visible = false;
                divTips.InnerHtml= "<span class=\"ws-font-red\">温馨提示：在修改状态时通过 “批量导入” 可以覆盖现有内容（注意：是整单覆盖，非并单条记录覆盖）</span>";

            }
        }
        else if (Action == "view")
        {
            btnAddOpenLink.Attributes.Add("class", "layui-btn layui-btn-xs layui-btn-disabled");
            btnAddOpenLink.Disabled = true;

            btnUpload.Attributes.Add("class", "layui-btn layui-btn-xs layui-btn-disabled");
            btnUpload.Disabled = true;

            btnTemplate.Attributes.Add("class", "layui-btn layui-btn-xs layui-btn-disabled");
            btnTemplate.Disabled = true;

            btnDel.Attributes.Add("class", "layui-btn layui-btn-xs layui-btn-disabled");
            btnDel.Disabled = true;
        }


        //当月页面初始化或上传失败时生成空的数据表格参数，用于创建空的数据表格
        string Cols = string.Empty,
            StartDate = DateTime.Now.toDateWFirstDay(),
            EndDate = DateTime.Now.toDateWLastDay();

        var getDateTimeDiff = MicroPublic.GetDateTimeDiff(StartDate, EndDate);
        int TotalDays = getDateTimeDiff.TotalDays.toInt();

        for (int i = 0; i < TotalDays; i++)
        {
            string ColName = StartDate.toDateTime().AddDays(i).ToString("yyyy-MM-dd");
            string Week = "（" + MicroPublic.GetWeek(ColName, "WW") + "）";
            int width = 170;

            Cols += "{field: '" + ColName + "', title: '" + ColName + Week + "', width:" + width + ", align:'center', sort:true},";
        }

        Cols = Cols.Substring(0, Cols.Length - 1);

        txtTabParms.Value = "{\"code\": 0, \"msg\": \"导入失败，页面初始化\", \"filepath\": \"\", \"title\": \"\", \"cols\": \"[{field: '部门', title: '部门', width:170, align:'center', sort:true },{field: '电脑ID', title: '电脑ID', width:120, align:'center', sort:true, hide:true},{field: '姓名', title: '姓名', width:120, align:'center', sort:true },{field: '地点', title: '地点', width:170, align:'center', sort:true }," + Cols + "]\", \"data\": []}";

    }
}