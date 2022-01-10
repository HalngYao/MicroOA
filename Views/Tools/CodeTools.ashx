<%@ WebHandler Language="C#" Class="CodeTools" %>

using System;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using Newtonsoft.Json.Linq;
using MicroPublicHelper;

public class CodeTools : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        string flag = MicroPublic.GetMsg("DenyUrlParaError"), TableName = string.Empty,
        Action = context.Server.UrlDecode(context.Request.Form["action"]),
                      MID = context.Server.UrlDecode(context.Request.Form["mid"]),
                      Fields = context.Server.UrlDecode(context.Request.Form["fields"]);

        if (!string.IsNullOrEmpty(Action) && !string.IsNullOrEmpty(MID) && !string.IsNullOrEmpty(Fields))
        {
            dynamic json = JToken.Parse(Fields) as dynamic;
            TableName = json["TableName"];

            if (Action.Trim().ToLower() == "insert" && !string.IsNullOrEmpty(TableName))
                flag = GetInsertQuery(TableName.Trim());

            if (Action.Trim().ToLower() == "update" && !string.IsNullOrEmpty(TableName))
                flag = GetUpdateQuery(TableName.Trim());

            if (Action.Trim().ToLower() == "field" && !string.IsNullOrEmpty(TableName))
                flag = GetFieldString(TableName.Trim());
        }
        else
            flag = "出错了请检查URL参数。Check the URL parameters for errors.";

        context.Response.Write(flag);
    }

    /// <summary>
    /// 根据表的字段生成插入语句
    /// </summary>
    /// <param name="TableName"></param>
    /// <returns></returns>
    protected string GetInsertQuery(string TableName)
    {
        string flag = MicroPublic.GetMsg("DenyUrlParaError"), _StringFieldName = string.Empty, _StringPrefixFieldName = string.Empty, _Json = string.Empty, _FieldName = string.Empty, _FieldValue = string.Empty, _SQL = string.Empty, _SP = string.Empty, _SPValue = string.Empty, _dtInsert = string.Empty, _drInsert = string.Empty;

        string _sql = "select * from MicroTables where ParentID in (select TID from MicroTables where TabColName = '" + TableName + "' and ParentID = 0) and InJoinSql=1 order by ParentID,Sort";
        SqlParameter[] _sp = { new SqlParameter("@TableName", SqlDbType.VarChar, 100) };

        DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

        if (_dt != null && _dt.Rows.Count > 0)
        {
            _Json = "dynamic json = JToken.Parse(Fields) as dynamic; <br/><br/>";
            _SQL = "string _sql=\"insert into " + TableName + " (";
            _SP = "SqlParameter[] _sp = { ";
            _dtInsert = "DataTable _dtInsert = new DataTable();<br/><br/>";
            _drInsert = " DataRow _drInsert = _dtInsert.NewRow();<br/><br/>";

            DataRow[] _rows = _dt.Select("PrimaryKey<>1", "ParentID,Sort");  //主键为系统自动标识不作为插入语句的对象，所以需要排除

            for (int i = 0; i < _rows.Length; i++)
            {
                string Prefix = _rows[i]["ctlPrefix"].ToString(), FieldName = _rows[i]["TabColName"].ToString(), DataType = _rows[i]["DataType"].ToString(),
                    FieldLength = _rows[i]["FieldLength"].ToString();

                int intFieldLength = 0;
                if (!string.IsNullOrEmpty(FieldLength))
                    intFieldLength = int.Parse(FieldLength);

                _FieldName += FieldName + ", ";  //组成 (Name1,Name2,Name3,...)
                _FieldValue += "@" + FieldName + ", "; //组成 value (@Name1,@Name2,@Name3,...)

                _StringFieldName += "string " + FieldName + " = string.Empty; ";  //声明变量获取传入过来的值
                _Json += FieldName + " = json[\"" + Prefix + FieldName + "\"];<br/>";
                _Json += FieldName + " = " + FieldName + ".toStringTrim();<br/><br/>";

                switch (DataType)
                {
                    case "bit":
                        _SP += "new SqlParameter(\"@" + FieldName + "\", SqlDbType.Bit),<br/>";
                        _SPValue += "_sp[" + i + "].Value =" + FieldName + ".toBoolean();<br/>";

                        _dtInsert += " _dtInsert.Columns.Add(\"" + FieldName + "\", typeof(Boolean));<br/>";
                        _drInsert += "_drInsert[\"" + FieldName + "\"] = " + FieldName + ".toBoolean();<br/>";
                        break;
                    case "char":
                        _SP += "new SqlParameter(\"@" + FieldName + "\", SqlDbType.Char," + intFieldLength + "),<br/>";
                        _SPValue += "_sp[" + i + "].Value = " + FieldName + ";<br/>";

                        _dtInsert += " _dtInsert.Columns.Add(\"" + FieldName + "\", typeof(string));<br/>";
                        _drInsert += "_drInsert[\"" + FieldName + "\"] = " + FieldName + ";<br/>";
                        break;
                    case "datetime":
                        _SP += "new SqlParameter(\"@" + FieldName + "\", SqlDbType.DateTime),<br/>";
                        _SPValue += "_sp[" + i + "].Value = " + FieldName + ".toDateTime();<br/>";

                        _dtInsert += " _dtInsert.Columns.Add(\"" + FieldName + "\", typeof(DateTime));<br/>";
                        _drInsert += "_drInsert[\"" + FieldName + "\"] =" + FieldName + ".toDateTime();<br/>";
                        break;
                    case "decimal":
                        _SP += "new SqlParameter(\"@" + FieldName + "\", SqlDbType.Decimal),<br/>";
                        _SPValue += "_sp[" + i + "].Value = " + FieldName + ".toDecimal();<br/>";

                        _dtInsert += " _dtInsert.Columns.Add(\"" + FieldName + "\", typeof(decimal));<br/>";
                        _drInsert += "_drInsert[\"" + FieldName + "\"] = " + FieldName + ".toDecimal();<br/>";
                        break;
                    case "float":
                        _SP += "new SqlParameter(\"@" + FieldName + "\", SqlDbType.Float),<br/>";
                        _SPValue += "_sp[" + i + "].Value = " + FieldName + ".toFloat();<br/>";

                        _dtInsert += " _dtInsert.Columns.Add(\"" + FieldName + "\", typeof(float));<br/>";
                        _drInsert += "_drInsert[\"" + FieldName + "\"] = " + FieldName + ".toFloat();<br/>";
                        break;
                    case "int":
                        _SP += "new SqlParameter(\"@" + FieldName + "\", SqlDbType.Int),<br/>";
                        _SPValue += "_sp[" + i + "].Value = " + FieldName + ".toInt();<br/>";

                        _dtInsert += " _dtInsert.Columns.Add(\"" + FieldName + "\", typeof(int));<br/>";
                        _drInsert += "_drInsert[\"" + FieldName + "\"] = " + FieldName + ".toInt();<br/>";
                        break;
                    case "nchar":
                        _SP += "new SqlParameter(\"@" + FieldName + "\", SqlDbType.NChar," + intFieldLength + "),<br/>";
                        _SPValue += "_sp[" + i + "].Value = " + FieldName + ";<br/>";

                        _dtInsert += " _dtInsert.Columns.Add(\"" + FieldName + "\", typeof(string));<br/>";
                        _drInsert += "_drInsert[\"" + FieldName + "\"] = " + FieldName + ";<br/>";
                        break;
                    case "ntext":
                        _SP += "new SqlParameter(\"@" + FieldName + "\", SqlDbType.NText),<br/>";
                        _SPValue += "_sp[" + i + "].Value = " + FieldName + ";<br/>";

                        _dtInsert += " _dtInsert.Columns.Add(\"" + FieldName + "\", typeof(string));<br/>";
                        _drInsert += "_drInsert[\"" + FieldName + "\"] = " + FieldName + ";<br/>";
                        break;
                    case "nvarchar":
                        _SP += "new SqlParameter(\"@" + FieldName + "\", SqlDbType.NVarChar," + intFieldLength + "),<br/>";
                        _SPValue += "_sp[" + i + "].Value = " + FieldName + ";<br/>";

                        _dtInsert += " _dtInsert.Columns.Add(\"" + FieldName + "\", typeof(string));<br/>";
                        _drInsert += "_drInsert[\"" + FieldName + "\"] = " + FieldName + ";<br/>";
                        break;
                    case "text":
                        _SP += "new SqlParameter(\"@" + FieldName + "\", SqlDbType.Text),<br/>";
                        _SPValue += "_sp[" + i + "].Value = " + FieldName + ";<br/>";

                        _dtInsert += " _dtInsert.Columns.Add(\"" + FieldName + "\", typeof(string));<br/>";
                        _drInsert += "_drInsert[\"" + FieldName + "\"] = " + FieldName + ";<br/>";
                        break;
                    case "varchar":
                        _SP += "new SqlParameter(\"@" + FieldName + "\", SqlDbType.VarChar," + intFieldLength + "),<br/>";
                        _SPValue += "_sp[" + i + "].Value = " + FieldName + ";<br/>";

                        _dtInsert += " _dtInsert.Columns.Add(\"" + FieldName + "\", typeof(string));<br/>";
                        _drInsert += "_drInsert[\"" + FieldName + "\"] = " + FieldName + ";<br/>";
                        break;
                }
            }

            _StringFieldName += "<br/>";
            _StringPrefixFieldName += "<br/>";
            _Json += "<br/>";
            _FieldName = _FieldName.Substring(0, _FieldName.Length - 2); //(Filed1,Filed2,Filed2,...)
            _FieldValue = _FieldValue.Substring(0, _FieldValue.Length - 2); // value (@Filed1,@Filed2,@Filed2,...)
            _SQL += _FieldName + " ) values ( " + _FieldValue + " )\";<br/><br/>";
            _SP += "}; <br/>";
            _SPValue += "<br/>";

            flag = _StringFieldName + _StringPrefixFieldName + _Json + _SQL + _SP + _SPValue;
            flag += "MsSQLDbHelper.ExecuteSql(_sql, _sp); <br/><br/>";
            flag += _dtInsert + "<br/><br/>";
            flag += _drInsert;

        }

        return flag;
    }

    /// <summary>
    /// 根据表的字段生成更新语句
    /// </summary>
    /// <param name="TableName"></param>
    /// <returns></returns>
    protected string GetUpdateQuery(string TableName)
    {
        string flag = MicroPublic.GetMsg("DenyUrlParaError"), _StringFieldName = string.Empty, _StringPrefixFieldName = string.Empty, _Json = string.Empty, _FieldName = string.Empty, _FieldValue = string.Empty, _SQL = string.Empty, _SP = string.Empty, _SPValue = string.Empty, _dtInsert = string.Empty, _drInsert = string.Empty;

        string _sql = "select * from MicroTables where ParentID in (select TID from MicroTables where TabColName = '" + TableName + "' and ParentID = 0) and InJoinSql=1 order by ParentID,Sort";
        SqlParameter[] _sp = { new SqlParameter("@TableName", SqlDbType.VarChar, 100) };

        DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];
        if (_dt != null && _dt.Rows.Count > 0)
        {
            int _i = 0;
            _Json = "dynamic json = JToken.Parse(Fields) as dynamic; <br/><br/>";
            _SQL = "string _sql=\"update " + TableName + " set ";
            _SP = "SqlParameter[] _sp = { ";

            DataRow[] _rows = _dt.Select("PrimaryKey<>1", "ParentID,Sort");  //主键不作为更新的对象，所以更新语句需要排除，但主键需要作为更新的where条件

            for (int i = 0; i < _rows.Length; i++)
            {
                _i = i + 1;
                string Prefix = _rows[i]["ctlPrefix"].ToString(), FieldName = _rows[i]["TabColName"].ToString(), DataType = _rows[i]["DataType"].ToString(),
                    FieldLength = _rows[i]["FieldLength"].ToString();

                int intFieldLength = 0;
                if (!string.IsNullOrEmpty(FieldLength))
                    intFieldLength = int.Parse(FieldLength);

                _FieldValue = " = @" + FieldName; //组成 Name1=@Name1,Name2=@Name2,Name2=@Name3,...
                _FieldName += FieldName + _FieldValue + ", ";  //组成 (Name1,Name2,Name3,...)

               _StringFieldName += "string " + FieldName + " = string.Empty; ";  //声明变量获取传入过来的值
                _Json += FieldName + " = json[\"" + Prefix + FieldName + "\"];<br/>";
                _Json += FieldName + " = " + FieldName + ".toStringTrim();<br/><br/>";

                switch (DataType)
                {
                    case "bit":
                        _SP += "new SqlParameter(\"@" + FieldName + "\", SqlDbType.Bit),<br/>";
                        _SPValue += "_sp[" + i + "].Value =" + FieldName + ".toBoolean();<br/>";

                        _dtInsert += " _dtInsert.Columns.Add(\"" + FieldName + "\", typeof(Boolean));<br/>";
                        _drInsert += "_drInsert[\"" + FieldName + "\"] = " + FieldName + ".toBoolean();<br/>";
                        break;
                    case "char":
                        _SP += "new SqlParameter(\"@" + FieldName + "\", SqlDbType.Char," + intFieldLength + "),<br/>";
                        _SPValue += "_sp[" + i + "].Value = " + FieldName + ";<br/>";

                        _dtInsert += " _dtInsert.Columns.Add(\"" + FieldName + "\", typeof(string));<br/>";
                        _drInsert += "_drInsert[\"" + FieldName + "\"] = " + FieldName + ";<br/>";
                        break;
                    case "datetime":
                        _SP += "new SqlParameter(\"@" + FieldName + "\", SqlDbType.DateTime),<br/>";
                        _SPValue += "_sp[" + i + "].Value = " + FieldName + ".toDateTime();<br/>";

                        _dtInsert += " _dtInsert.Columns.Add(\"" + FieldName + "\", typeof(DateTime));<br/>";
                        _drInsert += "_drInsert[\"" + FieldName + "\"] = " + FieldName + ".toDateTime();<br/>";
                        break;
                    case "decimal":
                        _SP += "new SqlParameter(\"@" + FieldName + "\", SqlDbType.Decimal),<br/>";
                        _SPValue += "_sp[" + i + "].Value = " + FieldName + ".toDecimal();<br/>";

                        _dtInsert += " _dtInsert.Columns.Add(\"" + FieldName + "\", typeof(decimal));<br/>";
                        _drInsert += "_drInsert[\"" + FieldName + "\"] = " + FieldName + ".toDecimal();<br/>";
                        break;
                    case "float":
                        _SP += "new SqlParameter(\"@" + FieldName + "\", SqlDbType.Float),<br/>";
                        _SPValue += "_sp[" + i + "].Value = " + FieldName + ".toFloat();<br/>";

                        _dtInsert += " _dtInsert.Columns.Add(\"" + FieldName + "\", typeof(float));<br/>";
                        _drInsert += "_drInsert[\"" + FieldName + "\"] = " + FieldName + ".toFloat();<br/>";
                        break;
                    case "int":
                        _SP += "new SqlParameter(\"@" + FieldName + "\", SqlDbType.Int),<br/>";
                        _SPValue += "_sp[" + i + "].Value = " + FieldName + ".toInt();<br/>";

                        _dtInsert += " _dtInsert.Columns.Add(\"" + FieldName + "\", typeof(int));<br/>";
                        _drInsert += "_drInsert[\"" + FieldName + "\"] = " + FieldName + ".toInt();<br/>";
                        break;
                    case "nchar":
                        _SP += "new SqlParameter(\"@" + FieldName + "\", SqlDbType.NChar," + intFieldLength + "),<br/>";
                        _SPValue += "_sp[" + i + "].Value = " + FieldName + ";<br/>";

                        _dtInsert += " _dtInsert.Columns.Add(\"" + FieldName + "\", typeof(string));<br/>";
                        _drInsert += "_drInsert[\"" + FieldName + "\"] = " + FieldName + ";<br/>";
                        break;
                    case "ntext":
                        _SP += "new SqlParameter(\"@" + FieldName + "\", SqlDbType.NText),<br/>";
                        _SPValue += "_sp[" + i + "].Value = " + FieldName + ";<br/>";

                        _dtInsert += " _dtInsert.Columns.Add(\"" + FieldName + "\", typeof(string));<br/>";
                        _drInsert += "_drInsert[\"" + FieldName + "\"] = " + FieldName + ";<br/>";
                        break;
                    case "nvarchar":
                        _SP += "new SqlParameter(\"@" + FieldName + "\", SqlDbType.NVarChar," + intFieldLength + "),<br/>";
                        _SPValue += "_sp[" + i + "].Value = " + FieldName + ";<br/>";

                        _dtInsert += " _dtInsert.Columns.Add(\"" + FieldName + "\", typeof(string));<br/>";
                        _drInsert += "_drInsert[\"" + FieldName + "\"] = " + FieldName + ";<br/>";
                        break;
                    case "text":
                        _SP += "new SqlParameter(\"@" + FieldName + "\", SqlDbType.Text),<br/>";
                        _SPValue += "_sp[" + i + "].Value = " + FieldName + ";<br/>";

                        _dtInsert += " _dtInsert.Columns.Add(\"" + FieldName + "\", typeof(string));<br/>";
                        _drInsert += "_drInsert[\"" + FieldName + "\"] = " + FieldName + ";<br/>";
                        break;
                    case "varchar":
                        _SP += "new SqlParameter(\"@" + FieldName + "\", SqlDbType.VarChar," + intFieldLength + "),<br/>";
                        _SPValue += "_sp[" + i + "].Value = " + FieldName + ";<br/>";

                        _dtInsert += " _dtInsert.Columns.Add(\"" + FieldName + "\", typeof(string));<br/>";
                        _drInsert += "_drInsert[\"" + FieldName + "\"] = " + FieldName + ";<br/>";
                        break;
                }
            }

            _StringFieldName += "<br/>";
            _StringPrefixFieldName += "<br/>";
            _Json += "<br/>";
            _FieldName = _FieldName.Substring(0, _FieldName.Length - 2);

            DataRow[] _rows2 = _dt.Select("DataType='int' and PrimaryKey=1", "ParentID,Sort");  //主键作为更新语句的where条件，通过它提取表的主键
            string IDName = string.Empty, Where = string.Empty;
            if (_rows2.Length > 0)
            {

                IDName = _rows2[0]["TabColName"].ToString();
                _SP += "new SqlParameter(\"@" + IDName + "\", SqlDbType.Int),<br/>";
                _SPValue += "_sp[" + _i + "].Value = " + IDName + ";<br/>";
                Where = " where " + IDName + "=@" + IDName + "";
            }

            _SQL += _FieldName + Where + ";<br/><br/>";
            _SP += "}; <br/>";
            _SPValue += "<br/>";

            flag = _StringFieldName + _StringPrefixFieldName + _Json + _SQL + _SP + _SPValue;
            flag += "MsSQLDbHelper.ExecuteSql(_sql, _sp); <br/><br/>";
            flag += _dtInsert + "<br/><br/>";
            flag += _drInsert;

        }

        return flag;
    }

    /// <summary>
    /// 根据表字段生成字段占位符模板
    /// </summary>
    /// <param name="TableName"></param>
    /// <returns></returns>
    protected string GetFieldString(string TableName)
    {
        string flag = MicroPublic.GetMsg("DenyUrlParaError"), _StringTemp = string.Empty, _StringTempValue = string.Empty, _StringTempValue2 = string.Empty, _TableFileds = string.Empty;

        string _sql = "select * from MicroTables where ParentID in (select TID from MicroTables where TabColName = '" + TableName + "' and ParentID = 0) and InJoinSql=1 order by ParentID,Sort";
        SqlParameter[] _sp = { new SqlParameter("@TableName", SqlDbType.VarChar, 100) };

        DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];
        if (_dt != null && _dt.Rows.Count > 0)
        {
            _StringTemp += "//StringTpl<br/> string strTpl = \"";
            _StringTempValue += "//StringValue _dt<br/> string strValue = string.Format(strTemp, ";
            _StringTempValue2 += "//StringValue _dr<br/> string strValue = string.Format(strTemp, ";
            _TableFileds += "//TableFiles <br/>    { type: 'checkbox', fixed: 'left' }<br/>";

            for (int i = 0; i < _dt.Rows.Count; i++)
            {
                string FieldName = _dt.Rows[i]["TabColName"].ToString(), Title = _dt.Rows[i]["Title"].ToString();
                _StringTemp += "\\\"" + FieldName + "\\\":\\\"{" + i + "}\\\",";
                _StringTempValue += "_dt.Rows[i][\"" + FieldName + "\"].ToString(), ";
                _StringTempValue2 += "_dr[\"" + FieldName + "\"].ToString(), ";
                _TableFileds += ", { field: '" + FieldName + "', width: 100, title: '" + Title + "', align: 'center'}<br/>";
            }
            _StringTemp = _StringTemp.Substring(0, _StringTemp.Length - 1);
            _StringTemp += "\"; <br/><br/>";

            _StringTempValue = _StringTempValue.Substring(0, _StringTempValue.Length - 2);
            _StringTempValue += ");<br/><br/>";

            _StringTempValue2 = _StringTempValue2.Substring(0, _StringTempValue2.Length - 2);
            _StringTempValue2 += ");<br/><br/>";

            flag = _StringTemp + _StringTempValue + _StringTempValue2 + _TableFileds;

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