<%@ WebHandler Language="C#" Class="CtrlPublicTableField" %>

using System;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using Newtonsoft.Json.Linq;
using MicroPublicHelper;
using MicroAuthHelper;

public class CtrlPublicTableField : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = MicroPublic.GetMsg("SubmitFailed"),
                        Action = context.Server.UrlDecode(context.Request.Form["action"]),
                        MID = context.Server.UrlDecode(context.Request.Form["mid"]),
                        Fields = context.Server.UrlDecode(context.Request.Form["fields"]);

        //Action = "modify";
        //MID = "4";
        //Fields = context.Server.UrlDecode("%7B%22STN%22:%22Overtime%22,%22IDName%22:%22OvertimeID%22,%22IDValue%22:%2228577%22,%22FieldName%22:%22ExceptDaiXiu%22,%22FieldValue%22:%225%22%7D");

        if (!string.IsNullOrEmpty(Action) && !string.IsNullOrEmpty(MID) && !string.IsNullOrEmpty(Fields))
        {
            if (Action.Trim().ToLower() == "modify")
                flag = CtrlModify(MID.Trim(), Fields.Trim());
        }

        context.Response.Write(flag);
    }

    /// <summary>
    /// 修改单个字段值
    /// </summary>
    /// <param name="mid"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    private string CtrlModify(string MID, string Fields)
    {
        string flag = MicroPublic.GetMsg("SaveFailed"), ShortTableName = string.Empty, TableName = string.Empty, IDName = string.Empty, IDValue = string.Empty, FieldName = string.Empty, FieldValue = string.Empty;

        try
        {
            dynamic json = JToken.Parse(Fields) as dynamic;
            ShortTableName = json["STN"];
            IDName = json["IDName"];
            IDValue = json["IDValue"];
            FieldName = json["FieldName"];
            FieldValue = json["FieldValue"];
            if (!string.IsNullOrEmpty(FieldValue))
                FieldValue = FieldValue.Trim();

            TableName = MicroPublic.GetTableName(ShortTableName);


            //获取字段的类型和长度生成动态的SqlParameter
            string _sql = "select * from MicroTables where ParentID in (select TID from MicroTables where TabColName=@TableName and ParentID=0) and TabColName = @FieldName and Del=0";
            SqlParameter[] _sp = { new SqlParameter ("@TableName", SqlDbType.VarChar,100),
                                new SqlParameter ("@FieldName", SqlDbType.VarChar,100)
                             };
            _sp[0].Value = TableName;
            _sp[1].Value = FieldName;
            DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

            if (_dt != null && _dt.Rows.Count > 0)
            {
                string DataType = _dt.Rows[0]["DataType"].ToString(),
                    FieldLength = _dt.Rows[0]["FieldLength"].ToString();

                int intFieldLength = FieldLength.toInt();

                string _sql2 = "update " + TableName + " set " + FieldName + "=@" + FieldName + " where " + IDName + "=@" + IDName + " ";
                SqlParameter[] _sp2 = new SqlParameter[2];

                switch (DataType)
                {
                    case "bit":
                        _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                        _sp2[0].Value = FieldValue.toBoolean();
                        break;
                    case "char":
                        _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Char, intFieldLength);
                        _sp2[0].Value = FieldValue;
                        break;
                    case "datetime":
                        _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.DateTime);

                        if (string.IsNullOrEmpty(FieldValue))
                            _sp2[0].Value = null;
                        else
                            _sp2[0].Value = DateTime.Parse(FieldValue);

                        break;
                    case "decimal":
                        _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Decimal);

                        if (string.IsNullOrEmpty(FieldValue))
                            _sp2[0].Value = null;
                        else
                            _sp2[0].Value = decimal.Parse(FieldValue);

                        break;
                    case "float":
                        _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Float);

                        if (string.IsNullOrEmpty(FieldValue))
                            _sp2[0].Value = null;
                        else
                            _sp2[0].Value = float.Parse(FieldValue);

                        break;
                    case "int":
                        _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Int);

                        if (string.IsNullOrEmpty(FieldValue))
                            _sp2[0].Value = null;
                        else
                            _sp2[0].Value = int.Parse(FieldValue);

                        break;
                    case "nchar":
                        _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.NChar, intFieldLength);
                        _sp2[0].Value = FieldValue;
                        break;
                    case "ntext":
                        _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.NText);
                        _sp2[0].Value = FieldValue;
                        break;
                    case "nvarchar":
                        _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.NVarChar, intFieldLength);
                        _sp2[0].Value = FieldValue;
                        break;
                    case "text":
                        _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Text);
                        _sp2[0].Value = FieldValue;
                        break;
                    case "varchar":
                        _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, intFieldLength);
                        _sp2[0].Value = FieldValue;
                        break;
                }

                _sp2[1] = new SqlParameter("@" + IDName, SqlDbType.Int);
                _sp2[1].Value = IDValue;

                if (MsSQLDbHelper.ExecuteSql(_sql2, _sp2) > 0)
                    flag = MicroPublic.GetMsg("Modify");

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