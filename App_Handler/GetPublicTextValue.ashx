<%@ WebHandler Language="C#" Class="GetPublicTextValue" %>

using System;
using System.Web;
using System.Text;
using System.Data;
using MicroDBHelper;
using Newtonsoft.Json.Linq;
using MicroPublicHelper;
using MicroAuthHelper;

public class GetPublicTextValue : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = MicroPublic.GetMsg("DenyUrlParaError"),
                Action = context.Server.UrlDecode(context.Request.Form["action"]),
                MID = context.Server.UrlDecode(context.Request.Form["mid"]),
                Fields = context.Server.UrlDecode(context.Request.Form["fields"]);

        //测试数据
        //Action = "get";
        //Fields = "{ \"txt\": \"TabColName,Title\", \"val\": \"TabColName\", \"stn\": \"mTabs\", \"tcn\":\"micro\",\"ob\": \"TID\" }";

        if (!string.IsNullOrEmpty(Action) && !string.IsNullOrEmpty(Fields))
            flag = GetTextValue(Action, Fields);

        context.Response.Write(flag);
    }

    private string GetTextValue(string Action, string Fields)
    {
        string flag = string.Empty;
        try
        {
            string Text = string.Empty, Text1 = string.Empty, Text2 = string.Empty, Value = string.Empty, TableName = string.Empty, ShortTableName = string.Empty, TabColName = string.Empty, Where = string.Empty, OrderBy = string.Empty, _sql = string.Empty;
            string strTemp = "\"text\":\"{0}\",\"value\":\"{1}\"";
            StringBuilder sb = new StringBuilder();
            dynamic json = JToken.Parse(Fields) as dynamic;

            Text = json["txt"]; Value = json["val"]; ShortTableName = json["stn"]; TabColName = json["tcn"]; OrderBy = json["ob"];

            TableName = MicroPublic.GetTableName(ShortTableName);  //得到完整表名

            string[] tmpArray = Text.Split(',');
            if (tmpArray.Length > 1)
            {
                Text1 = tmpArray[0].toStringTrim();
                Text2 = tmpArray[1].toStringTrim();
            }

            Where = " Invalid=0 and Del=0 ";

            if (Action.toStringTrim().ToLower() == "getalltn")
                Where = Where + " and ParentID=0 ";

            if (!string.IsNullOrEmpty(OrderBy))
                OrderBy = " order by " + OrderBy;
            else
                OrderBy = "";

            if (string.IsNullOrEmpty(TabColName))
                _sql = "select " + Text + "," + Value + " from " + TableName + " where " + Where + OrderBy + " ";
            else
                _sql = "select " + Text + "," + Value + " from " + TableName + " where " + Where + " and ParentID in (select TID from " + TableName + " where TabColName='" + TabColName + "' or ShortTableName='" + TabColName + "') " + OrderBy + " ";

            DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

            if (_dt != null && _dt.Rows.Count > 0)
            {
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    string str1 = string.Empty;

                    if (tmpArray.Length > 1)
                        str1 = string.Format(strTemp, _dt.Rows[i]["" + Text1 + ""].ToString() + " -【" + _dt.Rows[i]["" + Text2 + ""].ToString() + "】", _dt.Rows[i]["" + Value + ""].ToString());
                    else
                        str1 = string.Format(strTemp, _dt.Rows[i]["" + Text + ""].ToString(), _dt.Rows[i]["" + Value + ""].ToString());

                    sb.Append("{" + str1 + "},");
                }
                string json2 = sb.ToString();
                flag = "[" + json2.Substring(0, json2.Length - 1) + "]"; //去除最后一个逗号
            }
        }
        catch (Exception ex) { flag = ex.ToString(); }

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