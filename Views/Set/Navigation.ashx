<%@ WebHandler Language="C#" Class="Navigation" %>

using System;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using Newtonsoft.Json.Linq;
using MicroPublicHelper;
using MicroAuthHelper;

public class Navigation : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = MicroPublic.GetMsg("SubmitFailed"),
                        Action = context.Server.UrlDecode(context.Request.Form["action"]),
                        MID = context.Server.UrlDecode(context.Request.Form["mid"]),
                        Fields = context.Server.UrlDecode(context.Request.Form["fields"]);

        if (!string.IsNullOrEmpty(Action) && !string.IsNullOrEmpty(MID) && !string.IsNullOrEmpty(Fields))
        {
            if (Action.Trim().ToLower() == "add")
                flag = GetAdd(MID.Trim(), Fields.Trim());
        }


        context.Response.Write(flag);
    }

    private string GetAdd(string MID, string Fields)
    {
        string flag = MicroPublic.GetMsg("AddFailed"), ParentID = string.Empty, Sort = string.Empty, ModuleName = string.Empty, URL = string.Empty, Icon = string.Empty, Show = string.Empty, Expand = string.Empty, Form = string.Empty, Level = string.Empty, LevelCode = string.Empty, Description = string.Empty, Disable = string.Empty;

        int intParentID = 0, intSort = 0, intLevel = 0, intResult = 0;
        Boolean bitShow = false, bitExpand = false, bitForm = false, bitDisable = false;

        try
        {
            dynamic json = JToken.Parse(Fields) as dynamic;

            ParentID = json["selParentID"];
            if (string.IsNullOrEmpty(ParentID))
                ParentID = "0";
            intParentID = int.Parse(ParentID);

            Sort = json["txtSort"];
            intSort = int.Parse(Sort.Trim());

            ModuleName = json["txtModuleName"];
            ModuleName = ModuleName.Trim();

            URL = json["txtURL"];
            URL = URL.Trim();

            Icon = json["txtIcon"];
            Icon = Icon.Trim();

            Show = json["ckShow"];
            bitShow = Show == "on" ? true : false;

            Expand = json["ckExpand"];
            bitExpand = Expand == "on" ? true : false;

            Form = json["ckForm"];
            bitForm = Form == "on" ? true : false;

            Level = json["txtLevel"];
            intLevel = int.Parse(Level.Trim());

            LevelCode = json["txtLevelCode"];
            if (!string.IsNullOrEmpty(LevelCode))
                LevelCode = LevelCode.Trim();

            Description = json["txtDescription"];
            Description = Description.Trim();

            Disable = json["ckDisable"];
            bitDisable = Disable == "on" ? true : false;

            if (!string.IsNullOrEmpty(ModuleName))
            {

                string _sql = "insert into Modules (ParentID, Sort, ModuleName, URL, Icon, Show, Expand, Form, Level, LevelCode, Description, AddDateTime, Invalid) values (@ParentID, @Sort, @ModuleName, @URL, @Icon, @Show, @Expand, @Form, @Level, @LevelCode, @Description, @AddDateTime, @Invalid)";
                SqlParameter[] _sp = { new SqlParameter("@ParentID", SqlDbType.Int),
                                new SqlParameter("@Sort", SqlDbType.Int),
                                new SqlParameter("@ModuleName", SqlDbType.VarChar,50),
                                new SqlParameter("@URL", SqlDbType.VarChar,1000),
                                new SqlParameter("@Icon", SqlDbType.VarChar,200),
                                new SqlParameter("@Show", SqlDbType.Bit),
                                new SqlParameter("@Expand", SqlDbType.Bit),
                                new SqlParameter("@Form", SqlDbType.Bit),
                                new SqlParameter("@Level", SqlDbType.Int),
                                new SqlParameter("@LevelCode", SqlDbType.VarChar,200),
                                new SqlParameter("@Description", SqlDbType.VarChar,1000),
                                new SqlParameter("@AddDateTime", SqlDbType.DateTime),
                                new SqlParameter("@Invalid", SqlDbType.Bit)
                        };
                _sp[0].Value = intParentID;
                _sp[1].Value = intSort;
                _sp[2].Value = ModuleName;
                _sp[3].Value = URL;
                _sp[4].Value = Icon;
                _sp[5].Value = bitShow;
                _sp[6].Value = bitExpand;
                _sp[7].Value = bitForm;
                _sp[8].Value = intLevel;
                _sp[9].Value = LevelCode;
                _sp[10].Value = Description;
                _sp[11].Value = DateTime.Now;
                _sp[12].Value = bitDisable;

                intResult = MsSQLDbHelper.ExecuteSql(_sql, _sp);
                if (intResult > 0)
                    flag = MicroPublic.GetMsg("Save");

            }
        }
        catch (Exception ex) { flag = ex.ToString(); }
        finally { }

        return flag;
    }

    private string GetList()
    {
        string flag = "False";

        string strTpl = "\"TID\":\"{0}\",\"ParentID\":\"{1}\",\"TabColName\":\"{2}\",\"DataType\":\"{3}\",\"FieldLength\":\"{4}\",\"DefaultValue\":\"{5}\",\"AllowNull\":\"{6}\",\"Description\":\"{7}\",\"Invalid\":\"{8}\",\"Del\":\"{9}\",\"MainSub\":\"{10}\"";

        StringBuilder sb = new StringBuilder();
        try
        {
            string _sql = "select * from MicroTables order by TID ";
            DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

            if (_dt != null && _dt.Rows.Count > 0)
            {  //if Start
                DataRow[] _rows = _dt.Select("ParentID=0");
                foreach (DataRow _dr in _rows)
                {
                    string str1 = string.Format(strTpl, _dr["TID"].ToString(), _dr["ParentID"].ToString(), _dr["TabColName"].ToString(), _dr["DataType"].ToString(), _dr["FieldLength"].ToString(), _dr["DefaultValue"].ToString(), _dr["AllowNull"].ToString(), _dr["Description"].ToString(), _dr["Invalid"].ToString(), _dr["Del"].ToString(), "Main");

                    sb.Append("{" + str1 + "},");
                }
                string json = sb.ToString();

                flag = "[" + json.Substring(0, json.Length - 1) + "]";
                flag = "{\"code\": 0,\"msg\": \"\",\"count\": " + _dt.Rows.Count + ",\"data\":  " + flag + " }";
            }
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