<%@ WebHandler Language="C#" Class="WorkFlowList" %>

using System;
using System.Web;
using System.Text;
using System.Data;
using MicroPublicHelper;
using MicroDTHelper;
using MicroAuthHelper;

public class WorkFlowList : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    //****************
    //说明
    //根据表名返回表的内容
    //如父项和子项，主要菜单和子菜单等
    //****************

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = MicroPublic.GetMsg("DenyUrlParaError"),
                    Action = context.Server.UrlDecode(context.Request.Form["action"]),
                    MID = context.Server.UrlDecode(context.Request.Form["mid"]);

        //测试数据
        //Action = "getworkflow";
        //MID = "25";

        if (Action == "getworkflow" && !string.IsNullOrEmpty(MID))
            flag = GetWorkFlow(MID);


        context.Response.Write(flag);
    }


    private string GetWorkFlow(string MID)
    {
        string flag = string.Empty;
        string strTemp = GetStrTpl();
        int Count = 0;
        try
        {
            DataTable _dt = MicroDataTable.GetDataTable("WFlow");

            if (_dt != null && _dt.Rows.Count > 0)
            {
                DataRow[] _rows = _dt.Select("ParentID = 0 and FormID=" + MID.toInt(), "Sort");
                if (_rows.Length > 0)
                {
                    Count = _rows.Length;
                    StringBuilder sb = new StringBuilder();

                    foreach (DataRow _dr in _rows)
                    {
                        string EffectiveType = _dr["EffectiveType"].toJsonTrim(),
                                EffectiveIDStr = _dr["EffectiveIDStr"].toJsonTrim();

                        string ShortTableName = string.Empty, NameField = string.Empty, IDField = string.Empty, EffectiveNameStr = string.Empty;

                        //根据审批类型，得到对应表的名称和显示字段
                        if (!string.IsNullOrEmpty(EffectiveType) && !string.IsNullOrEmpty(EffectiveIDStr))
                        {
                            switch (EffectiveType)
                            {
                                case "Dept":
                                    ShortTableName = "Dept";
                                    NameField = "DeptName";
                                    IDField = "DeptID";
                                    break;
                                case "JTitle":
                                    ShortTableName = "JTitle";
                                    NameField = "JobTitleName";
                                    IDField = "JTID";
                                    break;
                                case "Role":
                                    ShortTableName = "Role";
                                    NameField = "RoleName";
                                    IDField = "RID";
                                    break;
                                case "Use":
                                    ShortTableName = "Use";
                                    NameField = "ChineseName";
                                    IDField = "UID";
                                    break;
                            }

                            DataTable _dt2 = MicroDataTable.GetDataTable(ShortTableName);
                            if (_dt2 != null && _dt2.Rows.Count > 0)
                            {
                                //得到主要审批者的名称
                                DataRow[] _rows2 = _dt2.Select(IDField + " in (" + EffectiveIDStr + ")");
                                foreach (DataRow _dr2 in _rows2)
                                {
                                    EffectiveNameStr += _dr2[NameField].toJsonTrim() + "、";
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(EffectiveNameStr))
                            EffectiveNameStr = EffectiveNameStr.Substring(0, EffectiveNameStr.Length - 1);
                        else
                            EffectiveNameStr = "全公司生效";

                        string strValue = string.Format(strTemp, _dr["WFID"].ToString(), _dr["Sort"].ToString(), _dr["FormID"].ToString(), _dr["ParentID"].ToString(), _dr["FlowName"].ToString(), _dr["FlowCode"].ToString(), _dr["Alias"].ToString(), _dr["EffectiveType"].ToString(), _dr["EffectiveIDStr"].ToString(), _dr["IsAccept"].ToString(), _dr["IsSync"].ToString(), _dr["Creator"].ToString(), _dr["DefaultFlow"].ToString(), _dr["DateCreated"].ToString(), _dr["DateModified"].ToString(), _dr["Del"].ToString(), EffectiveNameStr);

                        sb.Append("{" + strValue + "},");
                    }
                    string json = sb.ToString();

                    if (!string.IsNullOrEmpty(json))
                        flag = json.Substring(0, json.Length - 1);

                }
            }
            flag = "{\"code\": 0,\"msg\": \"\",\"count\": " + _dt.Rows.Count + ",\"data\":  [" + flag + "]}";
        }
        catch (Exception ex) { flag = ex.ToString(); }

        return flag;
    }


    private string GetStrTpl()
    {
        string strTpl = "\"WFID\":\"{0}\",\"Sort\":\"{1}\",\"FormID\":\"{2}\",\"ParentID\":\"{3}\",\"FlowName\":\"{4}\",\"FlowCode\":\"{5}\",\"Alias\":\"{6}\",\"EffectiveType\":\"{7}\",\"EffectiveIDStr\":\"{8}\",\"IsAccept\":\"{9}\",\"IsSync\":\"{10}\",\"Creator\":\"{11}\",\"DefaultFlow\":\"{12}\",\"DateCreated\":\"{13}\",\"DateModified\":\"{14}\",\"Del\":\"{15}\", \"EffectiveNameStr\":\"{16}\"";

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