<%@ WebHandler Language="C#" Class="ControlList" %>

using System;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroPublicHelper;
using Newtonsoft.Json.Linq;

public class ControlList : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        string flag = "False",
               Action = context.Server.UrlDecode(context.Request.Form["action"]),
               MID = context.Server.UrlDecode(context.Request.Form["mid"]),
               Fields = context.Server.UrlDecode(context.Request.Form["fields"]);

        if (Action.toStringTrim().ToLower() == "get")
            flag = GetSingleList(Fields.toStringTrim());

        context.Response.Write(flag);
    }

    private string GetStrTpl()
    {
        string strTpl = "\"MainSub\":\"{0}\", \"TID\":\"{1}\", \"Sort\":\"{2}\", \"ParentID\":\"{3}\", \"TabColName\":\"{4}\", \"ShortTableName\":\"{5}\", \"Title\":\"{6}\", \"TitleTipsIcon\":\"{7}\", \"DataType\":\"{8}\", \"FieldLength\":\"{9}\", \"DefaultValue\":\"{10}\", \"IntSort\":\"{11}\", \"AscDesc\":\"{12}\", \"AllowNull\":\"{13}\", \"InJoinSql\":\"{14}\", \"PrimaryKey\":\"{15}\", \"Description\":\"{16}\", \"Invalid\":\"{17}\", \"Del\":\"{18}\", \"tbElem\":\"{19}\", \"tbURL\":\"{20}\", \"tbData\":\"{21}\", \"tbToolbar\":\"{22}\", \"tbDefaultToolbar\":\"{23}\", \"tbTitle\":\"{24}\", \"tbText\":\"{25}\", \"tbInitSort\":\"{26}\", \"tbID\":\"{27}\", \"tbSkin\":\"{28}\", \"tbEven\":\"{29}\", \"tbSize\":\"{30}\", \"tbWidth\":\"{31}\", \"tbHeight\":\"{32}\", \"tbCellMinWidth\":\"{33}\", \"tbPage\":\"{34}\", \"tbLimit\":\"{35}\", \"tbLimits\":\"{36}\", \"tbTotalRow\":\"{37}\", \"tbLoading\":\"{38}\", \"tbAutoSort\":\"{39}\", \"tbDone\":\"{40}\", \"tbMainSub\":\"{41}\", \"colCustomSort\":\"{42}\", \"colTitle\":\"{43}\", \"colCustomField\":\"{44}\", \"colWidth\":\"{45}\", \"colMinWidth\":\"{46}\", \"colFixed\":\"{47}\", \"colType\":\"{48}\", \"colAlign\":\"{49}\", \"colEdit\":\"{50}\", \"colEvent\":\"{51}\", \"colStyle\":\"{52}\", \"colCheckedAll\":\"{53}\", \"colSort\":\"{54}\", \"colHide\":\"{55}\", \"colUnReSize\":\"{56}\", \"colTotalRow\":\"{57}\", \"colTotalRowText\":\"{58}\", \"colTemplet\":\"{59}\", \"colToolbar\":\"{60}\", \"JoinTableColumn\":\"{61}\", \"ctlPrimaryKey\":\"{62}\", \"ctlFormStyle\":\"{63}\", \"ctlDisplayAsterisk\":\"{64}\", \"ctlAddDisplay\":\"{65}\", \"ctlAddDisplayButton\":\"{66}\", \"ctlSaveDraftButton\":\"{67}\", \"ctlModifyDisplay\":\"{68}\", \"ctlModifyDisplayButton\":\"{69}\", \"ctlViewDisplay\":\"{70}\", \"ctlViewDisplayLabel\":\"{71}\", \"ctlAfterDisplay\":\"{72}\", \"ctlTitle\":\"{73}\", \"ctlTitleStyle\":\"{74}\", \"ctlTypes\":\"{75}\", \"ctlPrefix\":\"{76}\", \"ctlStyle\":\"{77}\", \"ctlPlaceholder\":\"{78}\", \"ctlCheckboxSkin\":\"{79}\", \"ctlSwitchText\":\"{80}\", \"ctlDescription\":\"{81}\", \"ctlDescriptionDisplayPosition\":\"{82}\", \"ctlDescriptionDisplayMode\":\"{83}\", \"ctlDescriptionStyle\":\"{84}\", \"ctlValue\":\"{85}\", \"ctlDefaultValue\":\"{86}\", \"ctlExtraFunction\":\"{87}\", \"ctlReceiveName\":\"{88}\", \"ctlSourceTable\":\"{89}\", \"ctlTextName\":\"{90}\", \"ctlTextValue\":\"{91}\", \"ctlExtendJSCode\":\"{92}\", \"ctlCharLength\":\"{93}\", \"ctlFilter\":\"{94}\", \"ctlVerify\":\"{95}\", \"ctlVerify2\":\"{96}\", \"ctlVerifyCustomFunction\":\"{97}\", \"ctlGroup\":\"{98}\", \"ctlGroupDescription\":\"{99}\", \"ctlFormItemStyle\":\"{100}\", \"ctlInlineCss\":\"{101}\", \"ctlInlineStyle\":\"{102}\", \"ctlColSpace\":\"{103}\", \"ctlOffset\":\"{104}\", \"ctlXS\":\"{105}\", \"ctlSM\":\"{106}\", \"ctlMD\":\"{107}\", \"ctlLG\":\"{108}\", \"ctlInputInline\":\"{109}\", \"ctlInputInlineStyle\":\"{110}\", \"CustomHtmlCode\":\"{111}\"";

        return strTpl;
    }

    private string GetSingleList(string Fields)
    {
        string flag = "False", TID = string.Empty, strTpl = GetStrTpl();
        dynamic json = JToken.Parse(Fields) as dynamic;
        TID = json["TID"];
        //TID = "1";
        StringBuilder sb = new StringBuilder();

        string _sql = "select * from MicroTables where Invalid=0 and Del=0 and (TID=@TID or TID = (select ParentID from MicroTables where TID=@TID) ) order by ParentID,Sort";
        SqlParameter[] _sp = new SqlParameter[1];
        _sp[0] = new SqlParameter("@TID", SqlDbType.Int);
        _sp[0].Value = TID.toInt();

        DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

        if (_dt != null && _dt.Rows.Count > 0)
        {  //if Start
            string CustomHtmlCode = _dt.Select("ParentID=0")[0]["CustomHtmlCode"].toJsonTrim();

            DataRow[] _rows = _dt.Select("ParentID<>0");
            foreach (DataRow _dr in _rows)
            {
                string str1 = string.Format(strTpl, "Main", _dr["TID"].toJsonTrim(), _dr["Sort"].toJsonTrim(), _dr["ParentID"].toJsonTrim(), _dr["TabColName"].toJsonTrim(), _dr["ShortTableName"].toJsonTrim(), _dr["Title"].toJsonTrim(), _dr["TitleTipsIcon"].toJsonTrim(), _dr["DataType"].toJsonTrim(), _dr["FieldLength"].toJsonTrim(), _dr["DefaultValue"].toJsonTrim(), _dr["IntSort"].toJsonTrim(), _dr["AscDesc"].toJsonTrim(), _dr["AllowNull"].toJsonTrim(), _dr["InJoinSql"].toJsonTrim(), _dr["PrimaryKey"].toJsonTrim(), _dr["Description"].toJsonTrim(), _dr["Invalid"].toJsonTrim(), _dr["Del"].toJsonTrim(), _dr["tbElem"].toJsonTrim(), _dr["tbURL"].toJsonTrim(), _dr["tbData"].toJsonTrim(), _dr["tbToolbar"].toJsonTrim(), _dr["tbDefaultToolbar"].toJsonTrim(), _dr["tbTitle"].toJsonTrim(), _dr["tbText"].toJsonTrim(), _dr["tbInitSort"].toJsonTrim(), _dr["tbID"].toJsonTrim(), _dr["tbSkin"].toJsonTrim(), _dr["tbEven"].toJsonTrim(), _dr["tbSize"].toJsonTrim(), _dr["tbWidth"].toJsonTrim(), _dr["tbHeight"].toJsonTrim(), _dr["tbCellMinWidth"].toJsonTrim(), _dr["tbPage"].toJsonTrim(), _dr["tbLimit"].toJsonTrim(), _dr["tbLimits"].toJsonTrim(), _dr["tbTotalRow"].toJsonTrim(), _dr["tbLoading"].toJsonTrim(), _dr["tbAutoSort"].toJsonTrim(), _dr["tbDone"].toJsonTrim(), _dr["tbMainSub"].toJsonTrim(), _dr["colCustomSort"].toJsonTrim(), _dr["colTitle"].toJsonTrim(), _dr["colCustomField"].toJsonTrim(), _dr["colWidth"].toJsonTrim(), _dr["colMinWidth"].toJsonTrim(), _dr["colFixed"].toJsonTrim(), _dr["colType"].toJsonTrim(), _dr["colAlign"].toJsonTrim(), _dr["colEdit"].toJsonTrim(), _dr["colEvent"].toJsonTrim(), _dr["colStyle"].toJsonTrim(), _dr["colCheckedAll"].toJsonTrim(), _dr["colSort"].toJsonTrim(), _dr["colHide"].toJsonTrim(), _dr["colUnReSize"].toJsonTrim(), _dr["colTotalRow"].toJsonTrim(), _dr["colTotalRowText"].toJsonTrim(), _dr["colTemplet"].toJsonTrim(), _dr["colToolbar"].toJsonTrim(), _dr["JoinTableColumn"].toJsonTrim(), _dr["ctlPrimaryKey"].toJsonTrim(), _dr["ctlFormStyle"].toJsonTrim(), _dr["ctlDisplayAsterisk"].toJsonTrim(), _dr["ctlAddDisplay"].toJsonTrim(), _dr["ctlAddDisplayButton"].toJsonTrim(), _dr["ctlSaveDraftButton"].toJsonTrim(), _dr["ctlModifyDisplay"].toJsonTrim(), _dr["ctlModifyDisplayButton"].toJsonTrim(), _dr["ctlViewDisplay"].toJsonTrim(), _dr["ctlViewDisplayLabel"].toJsonTrim(), _dr["ctlAfterDisplay"].toJsonTrim(), _dr["ctlTitle"].toJsonTrim(), _dr["ctlTitleStyle"].toJsonTrim(), _dr["ctlTypes"].toJsonTrim(), _dr["ctlPrefix"].toJsonTrim(), _dr["ctlStyle"].toJsonTrim(), _dr["ctlPlaceholder"].toJsonTrim(), _dr["ctlCheckboxSkin"].toJsonTrim(), _dr["ctlSwitchText"].toJsonTrim(), _dr["ctlDescription"].toJsonTrim(), _dr["ctlDescriptionDisplayPosition"].toJsonTrim(), _dr["ctlDescriptionDisplayMode"].toJsonTrim(), _dr["ctlDescriptionStyle"].toJsonTrim(), _dr["ctlValue"].toJsonTrim(), _dr["ctlDefaultValue"].toJsonTrim(), _dr["ctlExtraFunction"].toJsonTrim(), _dr["ctlReceiveName"].toJsonTrim(), _dr["ctlSourceTable"].toJsonTrim(), _dr["ctlTextName"].toJsonTrim(), _dr["ctlTextValue"].toJsonTrim(), _dr["ctlExtendJSCode"].toJsonTrim(), _dr["ctlCharLength"].toJsonTrim(), _dr["ctlFilter"].toJsonTrim(), _dr["ctlVerify"].toJsonTrim(), _dr["ctlVerify2"].toJsonTrim(), _dr["ctlVerifyCustomFunction"].toJsonTrim(), _dr["ctlGroup"].toJsonTrim(), _dr["ctlGroupDescription"].toJsonTrim(), _dr["ctlFormItemStyle"].toJsonTrim(), _dr["ctlInlineCss"].toJsonTrim(), _dr["ctlInlineStyle"].toJsonTrim(), _dr["ctlColSpace"].toJsonTrim(), _dr["ctlOffset"].toJsonTrim(), _dr["ctlXS"].toJsonTrim(), _dr["ctlSM"].toJsonTrim(), _dr["ctlMD"].toJsonTrim(), _dr["ctlLG"].toJsonTrim(), _dr["ctlInputInline"].toJsonTrim(), _dr["ctlInputInlineStyle"].toJsonTrim(), CustomHtmlCode);

                sb.Append("{" + str1 + "},");
            }
            string json2 = sb.ToString();

            flag = "[" + json2.Substring(0, json2.Length - 1) + "]";
            flag = "{\"code\": 0,\"msg\": \"\",\"count\": " + _dt.Rows.Count + ",\"data\":  " + flag + " }";
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