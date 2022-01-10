<%@ WebHandler Language="C#" Class="GetParSubTextValue" %>

using System;
using System.Web;
using System.Text;
using System.Data;
using MicroDBHelper;
using Newtonsoft.Json.Linq;
using MicroAuthHelper;

public class GetParSubTextValue : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = "False",
           Action = context.Server.UrlDecode(context.Request.Form["action"]),
           MID = context.Server.UrlDecode(context.Request.Form["mid"]),
           Fields = context.Server.UrlDecode(context.Request.Form["fields"]);

        if (Action.toStringTrim().ToLower() == "get")
            flag = GetMain(Fields.toStringTrim());

        context.Response.Write(flag);
    }

    private string GetStrTpl()
    {
        string strTpl = "\"text\":\"{0}\",\"value\":\"{1}\",\"numLevel\":\"{2}\",\"ParentID\":\"{3}\"";
        return strTpl;
    }

    private string GetMain(string Fields)
    {
        string flag = "false", strTpl = GetStrTpl();
        try
        {
            string Text = string.Empty, Value = string.Empty, Level = string.Empty, TableName = string.Empty, ShortTableName = string.Empty, Where = string.Empty, OrderBy = string.Empty, Note = string.Empty, _Note = string.Empty;
            StringBuilder sb = new StringBuilder();
            dynamic json = JToken.Parse(Fields) as dynamic;

            Text = json["txt"]; Value = json["val"]; Level = json["lv"]; ShortTableName = json["stn"]; OrderBy = json["ob"]; Note = json["note"];

            //测试数据
            // Text = "TabColName"; Value = "TID"; Level = "False"; ShortTableName = "mTabs"; OrderBy = "ParentID,Sort"; Note = "Title";  

            TableName = MicroPublicHelper.MicroPublic.GetTableName(ShortTableName);  //得到完整表名
            Where = " del=0 "; //获得表主键，构造where参数
            if (!string.IsNullOrEmpty(OrderBy))
                OrderBy = " order by " + OrderBy;
            else
                OrderBy = "";

            string __Note = string.IsNullOrEmpty(Note) == true ? Note : "," + Note; //为值加上逗号分隔

            string _sql = "select " + Text + "," + Value + __Note + " ,* " + " from " + TableName + " where " + Where + OrderBy + " ";
            DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

            if (_dt != null && _dt.Rows.Count > 0)
            {
                int numLevel = 1;
                DataRow[] _rows = _dt.Select("ParentID=0"); //=0父项,所有>0为子项
                foreach (DataRow _dr in _rows)
                {
                    if (!string.IsNullOrEmpty(Note))
                        _Note = " - " + _dr["" + Note + ""].ToString();

                    string str1 = string.Format(strTpl, _dr["" + Text + ""].ToString() + _Note, _dr["" + Value + ""].ToString(), numLevel, _dr["ParentID"].ToString());
                    sb.Append("{" + str1 + "},");
                    sb.Append(GetSub(_dr["" + Value + ""].ToString(), _dt, Text, Value, Level, numLevel, Note));
                }
                string json2 = sb.ToString();
                flag = "[" + json2.Substring(0, json2.Length - 1) + "]"; //去除最后一个逗号
            }

        }
        catch (Exception ex) { flag = ex.ToString(); }

        return flag;
    }

    //制表符号
    //┌─┬─┬─┬─┐
    //│  │  │  │  │
    //├─┼─┼─┼─┤
    //│  │  │  │  │
    //├─┼─┼─┼─┤
    //│  │  │  │  │
    //└─┴─┴─┴─┘

    private string GetSub(string ID, DataTable _dt, string Text, string Value, string Level, int numLevel, string Note)
    {
        string strTpl = GetStrTpl(), _Note = string.Empty;
        StringBuilder sb = new StringBuilder();
        DataRow[] _rows = _dt.Select("ParentID=" + ID);
        if (_rows.Length > 0)
            numLevel = numLevel + 1;
        int count = 0;
        //string __Note = string.IsNullOrEmpty(Note) == true ? Note : "," + Note; //为值加上逗号分隔

        foreach (DataRow _dr in _rows)
        {
            count++;
            int _intLevel = 1;
            if (Level == "True")  //有些表需是父子关系，但没有Level的，所以这里加了一个Level进行判断
                _intLevel = _dr["Level"].toInt(); //显示当前层级

            string _Level = string.Empty;
            _Level += "&nbsp; ";
            for (int i = 0; i < _intLevel; i++)
            {
                _Level += "&nbsp; ";
            }
            if (count == _rows.Length)
                _Level += " └ ";
            else
                _Level += " ├ ";

            if (!string.IsNullOrEmpty(Note))
                _Note = " - " + _dr["" + Note + ""].ToString();

            string str1 = string.Format(strTpl, _Level + _dr[Text].ToString() + _Note, _dr[Value].ToString(), numLevel, _dr["ParentID"].ToString());
            sb.Append("{" + str1 + "},");
            sb.Append(GetSub(_dr[Value].ToString(), _dt, Text, Value, Level, numLevel, Note));  //递归   
        }

        return sb.ToString();
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}