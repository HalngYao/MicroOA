<%@ WebHandler Language="C#" Class="GetPublicValue" %>

using System;
using System.Web;
using System.Text;
using System.Data;
using MicroDBHelper;
using Newtonsoft.Json.Linq;
using MicroPublicHelper;
using MicroAuthHelper;

public class GetPublicValue : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        //获取某表某个指定的值，返回结果
        //如发布信息列表，根据选择的信息类型自动返回图片路径后赋值给配图

        string flag = "",
                Action = context.Server.UrlDecode(context.Request.Form["action"]),
                MID = context.Server.UrlDecode(context.Request.Form["mid"]),
                Fields = context.Server.UrlDecode(context.Request.Form["fields"]);

        if (!string.IsNullOrEmpty(Action) && !string.IsNullOrEmpty(Fields))
            flag = GetValue(Fields);

        context.Response.Write(flag);

    }

    private string GetValue(string Fields)
    {
        string flag = "";
        try
        {
            string TableName = string.Empty, ShortTableName = string.Empty, TabColName = string.Empty, PrimaryKeyName = string.Empty, PrimaryKeyValue = string.Empty, Where = string.Empty, OrderBy = string.Empty;

            dynamic json = JToken.Parse(Fields) as dynamic;

            ShortTableName = json["stn"]; TabColName = json["tcn"]; PrimaryKeyName = json["pkn"]; PrimaryKeyValue = json["pkv"]; OrderBy = json["ob"];
            TableName = MicroPublic.GetTableName(ShortTableName);  //得到完整表名


            if (!string.IsNullOrEmpty(TableName) && !string.IsNullOrEmpty(TabColName) && !string.IsNullOrEmpty(PrimaryKeyName) && !string.IsNullOrEmpty(PrimaryKeyValue))
            {
                Where = " where " + PrimaryKeyName + " = " + PrimaryKeyValue;
                string _sql = "select " + TabColName + " from " + TableName + Where + OrderBy;
                DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

                if (_dt != null && _dt.Rows.Count > 0)
                    flag = _dt.Rows[0][TabColName].ToString();

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