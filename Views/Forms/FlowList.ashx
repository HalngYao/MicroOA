<%@ WebHandler Language="C#" Class="FlowList" %>

using System;
using System.Web;
using System.Text;
using System.Data;
using MicroPublicHelper;
using MicroDTHelper;
using MicroAuthHelper;

public class FlowList : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = MicroPublic.GetMsg("ModifyFailed"),
                    Action = context.Server.UrlDecode(context.Request.Form["action"]),
                    MID = context.Server.UrlDecode(context.Request.Form["mid"]),
                    WFID= context.Server.UrlDecode(context.Request.Form["wfid"]);

        //测试数据
        //Action = "getflownode";
        //WFID = "52";

        if (Action == "getflownode" && !string.IsNullOrEmpty(WFID))
            flag = GetFlowNode(WFID);

        context.Response.Write(flag);
    }

    /// <summary>
    /// 获取流程节点
    /// </summary>
    /// <param name="WFID"></param>
    /// <returns></returns>
    private string GetFlowNode(string WFID)
    {
        string flag = string.Empty;
        string strTpl = GetStrTpl();
        int Count = 0;
        try
        {
            DataTable _dt = MicroDataTable.GetDataTable("WFlow");

            if (_dt != null && _dt.Rows.Count > 0)
            {

                string FixedNode = " and FixedNode = 0";
                FixedNode = "";

                DataRow[] _rows = _dt.Select("ParentID = " + WFID.toInt() + FixedNode, "Sort");
                if (_rows.Length > 0)
                {
                    Count = _rows.Length;
                    StringBuilder sb = new StringBuilder();

                    foreach (DataRow _dr in _rows)
                    {
                        string ApprovalType = _dr["ApprovalType"].toJsonTrim(),
                                ApprovalIDStr = _dr["ApprovalIDStr"].toJsonTrim(),
                                ApprovalByIDStr = _dr["ApprovalByIDStr"].toJsonTrim();

                        string ShortTableName = string.Empty, NameField = string.Empty, IDField = string.Empty, ApprovalNameStr = string.Empty, ApprovalByNameStr = string.Empty;

                        //根据审批类型，得到对应表的名称和显示字段
                        if (!string.IsNullOrEmpty(ApprovalType))
                        {
                            switch (ApprovalType)
                            {
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
                                case "DeptUsers":
                                    ShortTableName = "Use";
                                    NameField = "ChineseName";
                                    IDField = "UID";
                                    break;
                            }

                            DataTable _dt2 = MicroDataTable.GetDataTable(ShortTableName);
                            if (_dt2 != null && _dt2.Rows.Count > 0)
                            {
                                //得到主要审批者的名称
                                if (!string.IsNullOrEmpty(ApprovalIDStr))
                                {
                                    DataRow[] _rows2 = _dt2.Select(IDField + " in (" + ApprovalIDStr + ")");
                                    foreach (DataRow _dr2 in _rows2)
                                    {
                                        ApprovalNameStr += _dr2[NameField].toJsonTrim() + "、";
                                    }
                                }

                                //得到代理审批者的名称
                                if (!string.IsNullOrEmpty(ApprovalByIDStr))
                                {
                                    DataRow[] _rows3 = _dt2.Select(IDField + " in (" + ApprovalByIDStr + ")");
                                    foreach (DataRow _dr3 in _rows3)
                                    {
                                        ApprovalByNameStr += _dr3[NameField].toJsonTrim() + "、";
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(ApprovalNameStr))
                                ApprovalNameStr = ApprovalNameStr.Substring(0, ApprovalNameStr.Length - 1);

                            if (!string.IsNullOrEmpty(ApprovalByNameStr))
                                ApprovalByNameStr = ApprovalByNameStr.Substring(0, ApprovalByNameStr.Length - 1);
                        }

                        string str1 = string.Format(strTpl, _dr["WFID"].toJsonTrim(), _dr["Sort"].toJsonTrim(), _dr["ParentID"].toJsonTrim(), _dr["FlowName"].toJsonTrim(), _dr["FlowCode"].toJsonTrim(), _dr["IsConditionApproval"].toBoolean(), _dr["OperField"].toJsonTrim(), _dr["Condition"].toJsonTrim(), _dr["OperValue"].toJsonTrim(), ApprovalType, ApprovalIDStr, ApprovalByIDStr, ApprovalNameStr,  _dr["ApprovalIDStrSort"].toBoolean(), ApprovalByNameStr, _dr["ApprovalByIDStrSort"].toBoolean(), _dr["ApproversSelectedByDefault"].toBoolean(), _dr["IsOptionalApproval"].toBoolean(), _dr["IsVerticalDirection"].toBoolean(), _dr["FindRange"].toBoolean(), _dr["FindGMOffice"].toBoolean(), _dr["IsSpecialApproval"].toBoolean());

                        sb.Append("{" + str1 + "},");
                    }
                    string json = sb.ToString();

                    flag = json.Substring(0, json.Length - 1);

                }
            }
            flag = "{\"code\": 0,\"msg\": \"\",\"count\": " + Count + ",\"data\":  [" + flag + "]}";
        }
        catch (Exception ex) { flag = ex.ToString(); }

        return flag;
    }


    private string GetStrTpl()
    {
        string strTpl = "\"WFID\":\"{0}\",\"Sort\":\"{1}\",\"ParentID\":\"{2}\",\"FlowName\":\"{3}\",\"FlowCode\":\"{4}\",\"IsConditionApproval\":\"{5}\",\"OperField\":\"{6}\",\"Condition\":\"{7}\",\"OperValue\":\"{8}\",\"ApprovalType\":\"{9}\",\"ApprovalIDStr\":\"{10}\",\"ApprovalByIDStr\":\"{11}\",\"ApprovalNameStr\":\"{12}\",\"ApprovalIDStrSort\":\"{13}\",\"ApprovalByNameStr\":\"{14}\",\"ApprovalByIDStrSort\":\"{15}\",\"ApproversSelectedByDefault\":\"{16}\",\"IsOptionalApproval\":\"{17}\",\"IsVerticalDirection\":\"{18}\",\"FindRange\":\"{19}\",\"FindGMOffice\":\"{20}\",\"IsSpecialApproval\":\"{21}\"";

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