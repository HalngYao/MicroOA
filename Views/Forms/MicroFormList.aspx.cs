using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroFormHelper;
using MicroPublicHelper;
using MicroAuthHelper;
using MicroDTHelper;

public partial class Views_Forms_MicroFormList : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        MicroAuth.CheckLogin();

        // 动作Action 可选值Add、Modify、View、Draft
        string Action = MicroPublic.GetFriendlyUrlParm(0);
        txtAction.Value = Action;

        string ShortTableName = MicroPublic.GetFriendlyUrlParm(1);
        txtShortTableName.Value = ShortTableName;

        string ModuleID = MicroPublic.GetFriendlyUrlParm(2);
        txtMID.Value = ModuleID;

        //检查是否有页面浏览权限
        MicroAuth.CheckBrowse(ModuleID);

        string FormID = MicroPublic.GetFriendlyUrlParm(3);
        txtFormID.Value = FormID;

        //主要用于点击外部超链接（通常通过外部超链接进入的是MicroFormList页面）时传入主键值，弹出该条数据的详细内容
        string FormsID = MicroPublic.GetFriendlyUrlParm(4);
        txtPrimaryKeyValue.Value = FormsID;

        string FormNumber = MicroPublic.GetFriendlyUrlParm(5);
        txtFormNumber.Value = FormNumber;

        string DataType = MicroPublic.GetFriendlyUrlParm(6);
        txtDataType.Value = DataType;

        //显示批量审批按钮
        if (DataType == "GetPendingMyApprovalList")
        {
            btnBatchAgree.Visible = true;
            btnBatchReturn.Visible = true;
        }

        string StartDate = MicroPublic.GetFriendlyUrlParm(7);
        if (string.IsNullOrEmpty(Server.UrlDecode(StartDate)))
            StartDate = DateTime.Now.toDateFormat("yyyy-MM-dd");

        txtStartDate.Value = StartDate;

        string EndDate = MicroPublic.GetFriendlyUrlParm(8);
        if (string.IsNullOrEmpty(Server.UrlDecode(EndDate)))
            EndDate = DateTime.Now.ToString("yyyy-MM-dd");

        txtEndDate.Value = EndDate;

        string Keyword = MicroPublic.GetFriendlyUrlParm(9);
        txtKeyword.Value = Keyword.Replace('_', ':');

        //传递过来的第10个参数，用于selQueryItem的默认选中
        //string SelectItem = MicroPublic.GetFriendlyUrlParm(10);

        //得到主键名称，主要用于点击LayuiDataTabel行时根据主键名称返回主键值
        var GetTableAttr = MicroDataTable.GetTableAttr(MicroPublic.GetTableName(ShortTableName));
        txtPrimaryKeyName.Value = GetTableAttr.PrimaryKeyName;

        divScript.InnerHtml = MicroForm.GetLayCheckBoxTpl(ShortTableName, ModuleID);

        //这里的OpenLink是申请，所以权限是“8”
        if (!MicroAuth.CheckPermit(ModuleID, "8"))
        {
            btnAddOpenLink.Disabled = true;
            btnAddOpenLink.Attributes.Add("class", "layui-btn layui-btn-disabled");
        }

        if (!MicroAuth.CheckPermit(ModuleID, "16"))
        {
            btnDel.Disabled = true;
            btnDel.Attributes.Add("class", "layui-btn layui-btn-disabled");
        }

        DataTable _dtForms = MicroDataTable.GetDataTable("Forms");
        string LinkAddress = _dtForms.Select("FormID=" + FormID.toInt())[0]["LinkAddress"].toStringTrim();
        if (!string.IsNullOrEmpty(LinkAddress))
            txtLinkAddress.Value = LinkAddress;


        //显示基础查询描述提示
        if (FormID.toInt() > 0)
        {
            string _sql = "select QueryBaseDescription from Forms where Invalid=0 and Del=0 and FormID=@FormID";

            SqlParameter[] _sp = { new SqlParameter("@FormID", SqlDbType.Int) };
            _sp[0].Value = FormID.toInt();

            DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

            if (_dt != null && _dt.Rows.Count > 0)
            {
                string QueryBaseDescription = _dt.Rows[0]["QueryBaseDescription"].toStringTrim();
                if (!string.IsNullOrEmpty(QueryBaseDescription))
                {
                    iconDescription.Visible = true;
                    iconDescription.Attributes.Add("lay-tips", QueryBaseDescription);
                }
            }

        }

    }

    /// <summary>
    /// 查询项目下拉菜单值
    /// </summary>
    /// <returns></returns>
    protected string QueryItem()
    {
        string QueryItem = string.Empty,
            ShortTableName = MicroPublic.GetFriendlyUrlParm(1);

        //传递过来的第10个参数，用于selQueryItem的默认选中
        string SelectItem = MicroPublic.GetFriendlyUrlParm(10);

        if (!string.IsNullOrEmpty(ShortTableName))
        {
            string _sql = "select TID, TabColName, Title, queryTitle from MicroTables where Invalid=0 and Del=0 and QueryAsBaseField=1 and ParentID = (select TID from MicroTables where ShortTableName=@ShortTableName) order by QuerySort";

            SqlParameter[] _sp = { new SqlParameter("@ShortTableName", SqlDbType.VarChar, 100) };
            _sp[0].Value = ShortTableName.toStringTrim();

            DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

            if (_dt != null && _dt.Rows.Count > 0)
            {
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    string TabColName = _dt.Rows[i]["TabColName"].toStringTrim(),
                        Title = _dt.Rows[i]["Title"].toStringTrim(),
                        queryTitle = _dt.Rows[i]["queryTitle"].toStringTrim(),
                        selected = string.Empty;

                    if (string.IsNullOrEmpty(queryTitle))
                        queryTitle = Title;

                    if (!string.IsNullOrEmpty(SelectItem))
                    {
                        if (SelectItem.ToLower() == TabColName.ToLower())
                            selected = "selected = \"selected\"";
                    }

                    QueryItem += "<option value=\"" + TabColName + "\" " + selected + ">" + queryTitle + "</option>";
                }
            }

        }
        return QueryItem;

    }
}