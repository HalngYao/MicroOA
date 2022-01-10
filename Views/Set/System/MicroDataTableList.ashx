<%@ WebHandler Language="C#" Class="MicroDataTableList" %>

using System;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;
using MicroDBHelper;
using MicroPublicHelper;
using MicroAuthHelper;
using Newtonsoft.Json.Linq;


public class MicroDataTableList : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        string flag = "False",
                Action = context.Server.UrlDecode(context.Request.Form["action"]),
                ModuleID = context.Server.UrlDecode(context.Request.Form["mid"]);
        
        //固定值
        Action = "view";
        ModuleID = "13";

        if (!string.IsNullOrEmpty(Action) && !string.IsNullOrEmpty(ModuleID))
        {
            int EditTablePermit = MicroAuth.CheckPermit(ModuleID, "11") == true ? 1 : 0;
            flag = GetMain(EditTablePermit);

            flag = JToken.Parse(flag).ToString();
        }

        context.Response.Write(flag);
    }

    /// <summary>
    /// 获取存放在MicroTables表的表名
    /// </summary>
    /// <returns></returns>
    protected string GetMain(int EditTablePermit)
    {
        string flag = "False";
        string strTemp = GetStrTpl();

        StringBuilder sb = new StringBuilder();
        try
        {
            string _sql = "select * from MicroTables where Invalid=0 and Del=0 order by ParentID,Sort ";
            DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

            if (_dt != null && _dt.Rows.Count > 0)
            {  //if Start
                DataRow[] _rows = _dt.Select("ParentID=0", "ParentID,Sort");

                //var test = _dt.AsEnumerable();
                ////_dt.Select().take(20);
                //test.Skip(200).Take(100);

                foreach (DataRow _dr in _rows)
                {
                    string Str = string.Format(strTemp, "Main", _dr["TID"].toJsonTrim(), _dr["Sort"].toJsonTrim(), _dr["ParentID"].toJsonTrim(), _dr["TabColName"].toJsonTrim(), _dr["ShortTableName"].toJsonTrim(), _dr["Title"].toJsonTrim(), _dr["TitleTipsIcon"].toJsonTrim(), _dr["DataType"].toJsonTrim(), _dr["FieldLength"].toJsonTrim(), _dr["DefaultValue"].toJsonTrim(), _dr["IntSort"].toJsonTrim(), _dr["AscDesc"].toJsonTrim(), _dr["AllowNull"].toJsonTrim(), _dr["InJoinSql"].toJsonTrim(), _dr["PrimaryKey"].toJsonTrim(), _dr["Description"].toJsonTrim(), _dr["Invalid"].toJsonTrim(), _dr["Del"].toJsonTrim(), _dr["tbElem"].toJsonTrim(), _dr["tbURL"].toJsonTrim(), _dr["tbData"].toJsonTrim(), _dr["tbToolbar"].toJsonTrim(), _dr["tbDefaultToolbar"].toJsonTrim(), _dr["tbTitle"].toJsonTrim(), _dr["tbText"].toJsonTrim(), _dr["tbInitSort"].toJsonTrim(), _dr["tbID"].toJsonTrim(), _dr["tbSkin"].toJsonTrim(), _dr["tbEven"].toJsonTrim(), _dr["tbSize"].toJsonTrim(), _dr["tbWidth"].toJsonTrim(), _dr["tbHeight"].toJsonTrim(), _dr["tbCellMinWidth"].toJsonTrim(), _dr["tbPage"].toJsonTrim(), _dr["tbLimit"].toJsonTrim(), _dr["tbLimits"].toJsonTrim(), _dr["tbTotalRow"].toJsonTrim(), _dr["tbLoading"].toJsonTrim(), _dr["tbAutoSort"].toJsonTrim(), _dr["tbDone"].toJsonTrim(), _dr["tbMainSub"].toJsonTrim(), _dr["colCustomSort"].toJsonTrim(), _dr["colTitle"].toJsonTrim(), _dr["colCustomField"].toJsonTrim(), _dr["colWidth"].toJsonTrim(), _dr["colMinWidth"].toJsonTrim(), _dr["colFixed"].toJsonTrim(), _dr["colType"].toJsonTrim(), _dr["colAlign"].toJsonTrim(), _dr["colEdit"].toJsonTrim(), _dr["colEvent"].toJsonTrim(), _dr["colStyle"].toJsonTrim(), _dr["colCheckedAll"].toJsonTrim(), _dr["colSort"].toJsonTrim(), _dr["colHide"].toJsonTrim(), _dr["colUnReSize"].toJsonTrim(), _dr["colTotalRow"].toJsonTrim(), _dr["colTotalRowText"].toJsonTrim(), _dr["colTemplet"].toJsonTrim(), _dr["colToolbar"].toJsonTrim(), _dr["JoinTableColumn"].toJsonTrim(), _dr["ctlPrimaryKey"].toJsonTrim(), _dr["ctlFormStyle"].toJsonTrim(), _dr["ctlDisplayAsterisk"].toJsonTrim(), _dr["ctlAddDisplay"].toJsonTrim(), _dr["ctlAddDisplayButton"].toJsonTrim(), _dr["ctlSaveDraftButton"].toJsonTrim(), _dr["ctlModifyDisplay"].toJsonTrim(), _dr["ctlModifyDisplayButton"].toJsonTrim(), _dr["ctlViewDisplay"].toJsonTrim(), _dr["ctlViewDisplayLabel"].toJsonTrim(), _dr["ctlAfterDisplay"].toJsonTrim(), _dr["ctlTitle"].toJsonTrim(), _dr["ctlTitleStyle"].toJsonTrim(), _dr["ctlTypes"].toJsonTrim(), _dr["ctlPrefix"].toJsonTrim(), _dr["ctlStyle"].toJsonTrim(), _dr["ctlPlaceholder"].toJsonTrim(), _dr["ctlCheckboxSkin"].toJsonTrim(), _dr["ctlSwitchText"].toJsonTrim(), _dr["ctlDescription"].toJsonTrim(), _dr["ctlDescriptionDisplayPosition"].toJsonTrim(), _dr["ctlDescriptionDisplayMode"].toJsonTrim(), _dr["ctlDescriptionStyle"].toJsonTrim(), _dr["ctlValue"].toJsonTrim(), _dr["ctlDefaultValue"].toJsonTrim(), _dr["ctlExtraFunction"].toJsonTrim(), _dr["ctlReceiveName"].toJsonTrim(), _dr["ctlSourceTable"].toJsonTrim(), _dr["ctlTextName"].toJsonTrim(), _dr["ctlTextValue"].toJsonTrim(), _dr["ctlExtendJSCode"].toJsonTrim(), _dr["ctlCharLength"].toJsonTrim(), _dr["ctlFilter"].toJsonTrim(), _dr["ctlVerify"].toJsonTrim(), _dr["ctlVerify2"].toJsonTrim(), _dr["ctlVerifyCustomFunction"].toJsonTrim(), _dr["ctlGroup"].toJsonTrim(), _dr["ctlGroupDescription"].toJsonTrim(), _dr["ctlFormItemStyle"].toJsonTrim(), _dr["ctlInlineCss"].toJsonTrim(), _dr["ctlInlineStyle"].toJsonTrim(), _dr["ctlColSpace"].toJsonTrim(), _dr["ctlOffset"].toJsonTrim(), _dr["ctlXS"].toJsonTrim(), _dr["ctlSM"].toJsonTrim(), _dr["ctlMD"].toJsonTrim(), _dr["ctlLG"].toJsonTrim(), _dr["ctlInputInline"].toJsonTrim(), _dr["ctlInputInlineStyle"].toJsonTrim(), _dr["CustomHtmlCode"].toJsonTrim(), _dr["querySort"].toJsonTrim(), _dr["queryTitle"].toJsonTrim(), _dr["queryAsBaseField"].toJsonTrim(), _dr["queryAsAdvancedField"].toJsonTrim(), _dr["queryAsKeywordField"].toJsonTrim(),_dr["queryCtlTypes"].toJsonTrim(), EditTablePermit);

                    sb.Append("{" + Str + "},");
                    sb.Append(GetSub(_dr["TID"].ToString(), _dt, EditTablePermit));
                }
                string json = sb.ToString();

                flag = json.Substring(0, json.Length - 1);
                flag = "{\"code\": 0,\"msg\": \"\",\"count\": " + _dt.Rows.Count + ",\"data\":  [" + flag + "] }";
            }
        }
        catch { }

        return flag;

    }

    /// <summary>
    /// 获取该表的字段名
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="_dt"></param>
    /// <returns></returns>
    private string GetSub(string ID, DataTable _dt, int EditTablePermit)
    {
        string strTemp = GetStrTpl();

        StringBuilder sb = new StringBuilder();
        DataRow[] _rows = _dt.Select("ParentID=" + ID, "ParentID,Sort");

        foreach (DataRow _dr in _rows)
        {
            string Level = string.Empty;
            Level += "  ∟";

            string str1 = string.Format(strTemp, Level, _dr["TID"].toJsonTrim(), _dr["Sort"].toJsonTrim(), _dr["ParentID"].toJsonTrim(), _dr["TabColName"].toJsonTrim(), _dr["ShortTableName"].toJsonTrim(), _dr["Title"].toJsonTrim(), _dr["TitleTipsIcon"].toJsonTrim(), _dr["DataType"].toJsonTrim(), _dr["FieldLength"].toJsonTrim(), _dr["DefaultValue"].toJsonTrim(), _dr["IntSort"].toJsonTrim(), _dr["AscDesc"].toJsonTrim(), _dr["AllowNull"].toJsonTrim(), _dr["InJoinSql"].toJsonTrim(), _dr["PrimaryKey"].toJsonTrim(), _dr["Description"].toJsonTrim(), _dr["Invalid"].toJsonTrim(), _dr["Del"].toJsonTrim(), _dr["tbElem"].toJsonTrim(), _dr["tbURL"].toJsonTrim(), _dr["tbData"].toJsonTrim(), _dr["tbToolbar"].toJsonTrim(), _dr["tbDefaultToolbar"].toJsonTrim(), _dr["tbTitle"].toJsonTrim(), _dr["tbText"].toJsonTrim(), _dr["tbInitSort"].toJsonTrim(), _dr["tbID"].toJsonTrim(), _dr["tbSkin"].toJsonTrim(), _dr["tbEven"].toJsonTrim(), _dr["tbSize"].toJsonTrim(), _dr["tbWidth"].toJsonTrim(), _dr["tbHeight"].toJsonTrim(), _dr["tbCellMinWidth"].toJsonTrim(), _dr["tbPage"].toJsonTrim(), _dr["tbLimit"].toJsonTrim(), _dr["tbLimits"].toJsonTrim(), _dr["tbTotalRow"].toJsonTrim(), _dr["tbLoading"].toJsonTrim(), _dr["tbAutoSort"].toJsonTrim(), _dr["tbDone"].toJsonTrim(), _dr["tbMainSub"].toJsonTrim(), _dr["colCustomSort"].toJsonTrim(), _dr["colTitle"].toJsonTrim(), _dr["colCustomField"].toJsonTrim(), _dr["colWidth"].toJsonTrim(), _dr["colMinWidth"].toJsonTrim(), _dr["colFixed"].toJsonTrim(), _dr["colType"].toJsonTrim(), _dr["colAlign"].toJsonTrim(), _dr["colEdit"].toJsonTrim(), _dr["colEvent"].toJsonTrim(), _dr["colStyle"].toJsonTrim(), _dr["colCheckedAll"].toJsonTrim(), _dr["colSort"].toJsonTrim(), _dr["colHide"].toJsonTrim(), _dr["colUnReSize"].toJsonTrim(), _dr["colTotalRow"].toJsonTrim(), _dr["colTotalRowText"].toJsonTrim(), _dr["colTemplet"].toJsonTrim(), _dr["colToolbar"].toJsonTrim(), _dr["JoinTableColumn"].toJsonTrim(), _dr["ctlPrimaryKey"].toJsonTrim(), _dr["ctlFormStyle"].toJsonTrim(), _dr["ctlDisplayAsterisk"].toJsonTrim(), _dr["ctlAddDisplay"].toJsonTrim(), _dr["ctlAddDisplayButton"].toJsonTrim(), _dr["ctlSaveDraftButton"].toJsonTrim(), _dr["ctlModifyDisplay"].toJsonTrim(), _dr["ctlModifyDisplayButton"].toJsonTrim(), _dr["ctlViewDisplay"].toJsonTrim(), _dr["ctlViewDisplayLabel"].toJsonTrim(), _dr["ctlAfterDisplay"].toJsonTrim(), _dr["ctlTitle"].toJsonTrim(), _dr["ctlTitleStyle"].toJsonTrim(), _dr["ctlTypes"].toJsonTrim(), _dr["ctlPrefix"].toJsonTrim(), _dr["ctlStyle"].toJsonTrim(), _dr["ctlPlaceholder"].toJsonTrim(), _dr["ctlCheckboxSkin"].toJsonTrim(), _dr["ctlSwitchText"].toJsonTrim(), _dr["ctlDescription"].toJsonTrim(), _dr["ctlDescriptionDisplayPosition"].toJsonTrim(), _dr["ctlDescriptionDisplayMode"].toJsonTrim(), _dr["ctlDescriptionStyle"].toJsonTrim(), _dr["ctlValue"].toJsonTrim(), _dr["ctlDefaultValue"].toJsonTrim(), _dr["ctlExtraFunction"].toJsonTrim(), _dr["ctlReceiveName"].toJsonTrim(), _dr["ctlSourceTable"].toJsonTrim(), _dr["ctlTextName"].toJsonTrim(), _dr["ctlTextValue"].toJsonTrim(), _dr["ctlExtendJSCode"].toJsonTrim(), _dr["ctlCharLength"].toJsonTrim(), _dr["ctlFilter"].toJsonTrim(), _dr["ctlVerify"].toJsonTrim(), _dr["ctlVerify2"].toJsonTrim(), _dr["ctlVerifyCustomFunction"].toJsonTrim(), _dr["ctlGroup"].toJsonTrim(), _dr["ctlGroupDescription"].toJsonTrim(), _dr["ctlFormItemStyle"].toJsonTrim(), _dr["ctlInlineCss"].toJsonTrim(), _dr["ctlInlineStyle"].toJsonTrim(), _dr["ctlColSpace"].toJsonTrim(), _dr["ctlOffset"].toJsonTrim(), _dr["ctlXS"].toJsonTrim(), _dr["ctlSM"].toJsonTrim(), _dr["ctlMD"].toJsonTrim(), _dr["ctlLG"].toJsonTrim(), _dr["ctlInputInline"].toJsonTrim(), _dr["ctlInputInlineStyle"].toJsonTrim(), _dr["CustomHtmlCode"].toJsonTrim(), _dr["querySort"].toJsonTrim(), _dr["queryTitle"].toJsonTrim(), _dr["queryAsBaseField"].toJsonTrim(), _dr["queryAsAdvancedField"].toJsonTrim(), _dr["queryAsKeywordField"].toJsonTrim(), _dr["queryCtlTypes"].toJsonTrim(), EditTablePermit);

            sb.Append("{" + str1 + "},");
        }

        return sb.ToString();
    }

    private string GetStrTpl()
    {
        //\"EditTablePermit\":\"{111}\" 是手动添加的，主要返回前端判断是否有权限编辑（如前端的Switch）
        string strTpl = "\"MainSub\":\"{0}\", \"TID\":\"{1}\", \"Sort\":\"{2}\", \"ParentID\":\"{3}\", \"TabColName\":\"{4}\", \"ShortTableName\":\"{5}\", \"Title\":\"{6}\", \"TitleTipsIcon\":\"{7}\", \"DataType\":\"{8}\", \"FieldLength\":\"{9}\", \"DefaultValue\":\"{10}\", \"IntSort\":\"{11}\", \"AscDesc\":\"{12}\", \"AllowNull\":\"{13}\", \"InJoinSql\":\"{14}\", \"PrimaryKey\":\"{15}\", \"Description\":\"{16}\", \"Invalid\":\"{17}\", \"Del\":\"{18}\", \"tbElem\":\"{19}\", \"tbURL\":\"{20}\", \"tbData\":\"{21}\", \"tbToolbar\":\"{22}\", \"tbDefaultToolbar\":\"{23}\", \"tbTitle\":\"{24}\", \"tbText\":\"{25}\", \"tbInitSort\":\"{26}\", \"tbID\":\"{27}\", \"tbSkin\":\"{28}\", \"tbEven\":\"{29}\", \"tbSize\":\"{30}\", \"tbWidth\":\"{31}\", \"tbHeight\":\"{32}\", \"tbCellMinWidth\":\"{33}\", \"tbPage\":\"{34}\", \"tbLimit\":\"{35}\", \"tbLimits\":\"{36}\", \"tbTotalRow\":\"{37}\", \"tbLoading\":\"{38}\", \"tbAutoSort\":\"{39}\", \"tbDone\":\"{40}\", \"tbMainSub\":\"{41}\", \"colCustomSort\":\"{42}\", \"colTitle\":\"{43}\", \"colCustomField\":\"{44}\", \"colWidth\":\"{45}\", \"colMinWidth\":\"{46}\", \"colFixed\":\"{47}\", \"colType\":\"{48}\", \"colAlign\":\"{49}\", \"colEdit\":\"{50}\", \"colEvent\":\"{51}\", \"colStyle\":\"{52}\", \"colCheckedAll\":\"{53}\", \"colSort\":\"{54}\", \"colHide\":\"{55}\", \"colUnReSize\":\"{56}\", \"colTotalRow\":\"{57}\", \"colTotalRowText\":\"{58}\", \"colTemplet\":\"{59}\", \"colToolbar\":\"{60}\", \"JoinTableColumn\":\"{61}\", \"ctlPrimaryKey\":\"{62}\", \"ctlFormStyle\":\"{63}\", \"ctlDisplayAsterisk\":\"{64}\", \"ctlAddDisplay\":\"{65}\", \"ctlAddDisplayButton\":\"{66}\", \"ctlSaveDraftButton\":\"{67}\", \"ctlModifyDisplay\":\"{68}\", \"ctlModifyDisplayButton\":\"{69}\", \"ctlViewDisplay\":\"{70}\", \"ctlViewDisplayLabel\":\"{71}\", \"ctlAfterDisplay\":\"{72}\", \"ctlTitle\":\"{73}\", \"ctlTitleStyle\":\"{74}\", \"ctlTypes\":\"{75}\", \"ctlPrefix\":\"{76}\", \"ctlStyle\":\"{77}\", \"ctlPlaceholder\":\"{78}\", \"ctlCheckboxSkin\":\"{79}\", \"ctlSwitchText\":\"{80}\", \"ctlDescription\":\"{81}\", \"ctlDescriptionDisplayPosition\":\"{82}\", \"ctlDescriptionDisplayMode\":\"{83}\", \"ctlDescriptionStyle\":\"{84}\", \"ctlValue\":\"{85}\", \"ctlDefaultValue\":\"{86}\", \"ctlExtraFunction\":\"{87}\", \"ctlReceiveName\":\"{88}\", \"ctlSourceTable\":\"{89}\", \"ctlTextName\":\"{90}\", \"ctlTextValue\":\"{91}\", \"ctlExtendJSCode\":\"{92}\", \"ctlCharLength\":\"{93}\", \"ctlFilter\":\"{94}\", \"ctlVerify\":\"{95}\", \"ctlVerify2\":\"{96}\", \"ctlVerifyCustomFunction\":\"{97}\", \"ctlGroup\":\"{98}\", \"ctlGroupDescription\":\"{99}\", \"ctlFormItemStyle\":\"{100}\", \"ctlInlineCss\":\"{101}\", \"ctlInlineStyle\":\"{102}\", \"ctlColSpace\":\"{103}\", \"ctlOffset\":\"{104}\", \"ctlXS\":\"{105}\", \"ctlSM\":\"{106}\", \"ctlMD\":\"{107}\", \"ctlLG\":\"{108}\", \"ctlInputInline\":\"{109}\", \"ctlInputInlineStyle\":\"{110}\", \"CustomHtmlCode\":\"{111}\", \"querySort\":\"{112}\", \"queryTitle\":\"{113}\", \"queryAsBaseField\":\"{114}\", \"queryAsAdvancedField\":\"{115}\", \"queryAsKeywordField\":\"{116}\", \"queryCtlTypes\":\"{117}\", \"EditTablePermit\":\"{118}\"";

        return strTpl;
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}