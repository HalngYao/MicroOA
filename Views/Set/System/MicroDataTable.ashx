<%@ WebHandler Language="C#" Class="MicroDataTable" %>

using System;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using MicroPublicHelper;
using MicroDBHelper;
using Newtonsoft.Json.Linq;

public class MicroDataTable : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        string flag = "提交失败，请检查URL参数<br/>The submission failed. Check the URL parameters",
                        Action = context.Server.UrlDecode(context.Request.Form["action"]),
                        MID = context.Server.UrlDecode(context.Request.Form["mid"]),
                        Fields = context.Server.UrlDecode(context.Request.Form["fields"]);

        if (!string.IsNullOrEmpty(Action) && !string.IsNullOrEmpty(MID) && !string.IsNullOrEmpty(Fields))
        {
            if (Action.Trim().ToLower() == "modify")
                flag = CtrlModify(MID.Trim(), Fields.Trim());
        }

        context.Response.Write(flag);

    }

    /// <summary>
    /// 修改字段
    /// </summary>
    /// <param name="MID"></param>
    /// <param name="Fields"></param>
    /// <returns></returns>
    protected string CtrlModify(string MID, string Fields)
    {
        string flag = "保存失败<br/>Save failed", TableName = string.Empty, IDName = string.Empty, IDValue = string.Empty, FieldName = string.Empty, FieldValue = string.Empty, _sql2 = string.Empty;

        try
        {
            dynamic json = JToken.Parse(Fields) as dynamic;
            TableName = json["TableName"];
            IDName = json["IDName"];
            IDValue = json["IDValue"];
            FieldName = json["FieldName"];
            FieldValue = json["FieldValue"];
            FieldValue = FieldValue.toStringTrim();
            TableName = TableName == "mTabs" ? "MicroTables" : "";

            //TableName = "MicroTables";

            _sql2 = "update " + TableName + " set " + FieldName + "=@" + FieldName + " where " + IDName + "=@" + IDName + " ";
            SqlParameter[] _sp2 = new SqlParameter[2];
            //获取字段的类型和长度生成动态的SqlParameter
            switch (FieldName)
            {
                case "Sort":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Int);
                    _sp2[0].Value = FieldValue.toInt();
                    break;
                case "ParentID":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Int);
                    _sp2[0].Value = FieldValue.toInt();
                    break;
                case "TabColName":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 100);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ShortTableName":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 100);
                    _sp2[0].Value = FieldValue;
                    break;
                case "Title":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 100);
                    _sp2[0].Value = FieldValue;
                    break;
                case "TitleTipsIcon":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "DataType":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 100);
                    _sp2[0].Value = FieldValue;
                    break;
                case "FieldLength":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 100);
                    _sp2[0].Value = FieldValue;
                    break;
                case "DefaultValue":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 20);
                    _sp2[0].Value = FieldValue.toInt();
                    break;
                case "IntSort":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Int);
                    _sp2[0].Value = FieldValue.toInt();
                    break;
                case "AscDesc":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 10);
                    _sp2[0].Value = FieldValue;
                    break;
                case "AllowNull":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "InJoinSql":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "PrimaryKey":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "Description":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.NVarChar, 4000);
                    _sp2[0].Value = FieldValue;
                    break;
                case "Invalid":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "Del":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;

                //表格全局属性
                case "tbElem":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 100);
                    _sp2[0].Value = FieldValue;
                    break;
                case "tbURL":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 500);
                    _sp2[0].Value = FieldValue;
                    break;
                case "tbData":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 100);
                    _sp2[0].Value = FieldValue;
                    break;
                case "tbToolbar":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 1000);
                    _sp2[0].Value = FieldValue;
                    break;
                case "tbDefaultToolbar":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 200);
                    _sp2[0].Value = FieldValue;
                    break;
                case "tbTitle":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 200);
                    _sp2[0].Value = FieldValue;
                    break;
                case "tbText":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 200);
                    _sp2[0].Value = FieldValue;
                    break;
                case "tbInitSort":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 100);
                    _sp2[0].Value = FieldValue;
                    break;
                case "tbID":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 50);
                    _sp2[0].Value = FieldValue;
                    break;
                case "tbSkin":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 200);
                    _sp2[0].Value = FieldValue;
                    break;
                case "tbEven":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 200);
                    _sp2[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "tbSize":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 20);
                    _sp2[0].Value = FieldValue;
                    break;
                case "tbWidth":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Int);
                    _sp2[0].Value = FieldValue.toInt();
                    break;
                case "tbHeight":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Int);
                    _sp2[0].Value = FieldValue.toInt();
                    break;
                case "tbCellMinWidth":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Int);
                    _sp2[0].Value = FieldValue.toInt();
                    break;
                case "tbPage":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "tbLimit":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Int);
                    _sp2[0].Value = FieldValue.toInt();
                    break;
                case "tbLimits":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 200);
                    _sp2[0].Value = FieldValue;
                    break;
                case "tbTotalRow":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "tbLoading":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "tbAutoSort":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "tbDone":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 5000);
                    _sp2[0].Value = FieldValue;
                    break;
                case "tbMainSub":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;

                //表头属性
                case "colCustomSort":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Int);
                    _sp2[0].Value = FieldValue;
                    break;
                case "colTitle":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 100);
                    _sp2[0].Value = FieldValue;
                    break;
                case "colCustomField":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 100);
                    _sp2[0].Value = FieldValue;
                    break;
                case "colWidth":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 20);
                    _sp2[0].Value = FieldValue;
                    break;
                case "colMinWidth":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Int);
                    _sp2[0].Value = FieldValue.toInt();
                    break;
                case "colFixed":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 20);
                    _sp2[0].Value = FieldValue;
                    break;
                case "colType":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 20);
                    _sp2[0].Value = FieldValue;
                    break;
                case "colAlign":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 20);
                    _sp2[0].Value = FieldValue;
                    break;
                case "colEdit":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 20);
                    _sp2[0].Value = FieldValue;
                    break;
                case "colEvent":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 100);
                    _sp2[0].Value = FieldValue;
                    break;
                case "colStyle":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 2000);
                    _sp2[0].Value = FieldValue;
                    break;
                case "colCheckedAll":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "colSort":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "colHide":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "colUnReSize":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "colTotalRow":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "colTotalRowText":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 100);
                    _sp2[0].Value = FieldValue;
                    break;
                case "colTemplet":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 100);
                    _sp2[0].Value = FieldValue;
                    break;
                case "colToolbar":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 500);
                    _sp2[0].Value = FieldValue;
                    break;
                case "JoinTableColumn":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;

                //控件属性
                case "ctlPrimaryKey":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "ctlFormStyle":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 50);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlDisplayAsterisk":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "ctlAddDisplay":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "ctlAddDisplayButton":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "ctlSaveDraftButton":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "ctlModifyDisplay":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "ctlModifyDisplayButton":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "ctlViewDisplay":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "ctlViewDisplayLabel":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "ctlAfterDisplay":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "ctlTitle":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 50);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlTitleStyle":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 500);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlTypes":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 200);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlPrefix":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 30);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlStyle":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 500);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlPlaceholder":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 100);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlCheckboxSkin":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 100);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlSwitchText":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 100);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlDescription":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.NVarChar, 4000);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlDescriptionDisplayPosition":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar,10);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlDescriptionDisplayMode":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "ctlDescriptionStyle":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 500);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlValue":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 100);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlDefaultValue":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 100);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlExtraFunction":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 30);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlReceiveName":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 100);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlSourceTable":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 100);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlTextName":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 100);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlTextValue":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 100);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlExtendJSCode":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 5000);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlCharLength":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 30);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlFilter":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 100);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlVerify":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 100);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlVerify2":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 100);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlVerifyCustomFunction":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 2000);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlGroup":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Int);
                    _sp2[0].Value = int.Parse(FieldValue);
                    break;
                case "ctlGroupDescription":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 500);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlFormItemStyle":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 500);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlInlineCss":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 500);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlInlineStyle":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 500);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlColSpace":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 50);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlOffset":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 200);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlXS":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 50);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlSM":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 50);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlMD":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 50);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlLG":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 50);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlInputInline":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 100);
                    _sp2[0].Value = FieldValue;
                    break;
                case "ctlInputInlineStyle":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 500);
                    _sp2[0].Value = FieldValue;
                    break;
                case "CustomHtmlCode":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.NText);
                    _sp2[0].Value = FieldValue;
                    break;

                case "querySort":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Int);
                    _sp2[0].Value = int.Parse(FieldValue);
                    break;
                case "queryTitle":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 50);
                    _sp2[0].Value = FieldValue;
                    break;
                case "queryAsBaseField":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "queryAsAdvancedField":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "queryAsKeywordField":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "queryCtlTypes":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 200);
                    _sp2[0].Value = FieldValue;
                    break;
            }

            _sp2[1] = new SqlParameter("@" + IDName, SqlDbType.Int);
            _sp2[1].Value = IDValue;

            if (MsSQLDbHelper.ExecuteSql(_sql2, _sp2) > 0)
                flag = "True保存成功<br/>Save successfully";
        }
        catch { }

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