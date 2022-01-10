<%@ WebHandler Language="C#" Class="WorkFlow" %>

using System;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroPublicHelper;
using Newtonsoft.Json.Linq;
using MicroAuthHelper;

public class WorkFlow : IHttpHandler, System.Web.SessionState.IRequiresSessionState
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

        string flag = MicroPublic.GetMsg("ModifyFailed"),
                    Action = context.Server.UrlDecode(context.Request.Form["action"]),
                    MID = context.Server.UrlDecode(context.Request.Form["mid"]),
                    WFID = context.Server.UrlDecode(context.Request.Form["wfid"]),
                    Fields = context.Server.UrlDecode(context.Request.Form["fields"]);

        Action = Action.ToLower();

        if (Action == "modify" && !string.IsNullOrEmpty(Fields))
            flag = CtrlModify(Fields);

        if (Action == "del" && !string.IsNullOrEmpty(WFID))
            flag = DelWorkFlow(WFID);

        context.Response.Write(flag);
    }

    /// <summary>
    /// 修改字段
    /// </summary>
    /// <param name="MID"></param>
    /// <param name="Fields"></param>
    /// <returns></returns>
    private string CtrlModify(string Fields)
    {
        string flag = MicroPublic.GetMsg("ModifyFailed"), TableName = string.Empty, IDName = string.Empty, IDValue = string.Empty, FieldName = string.Empty, FieldValue = string.Empty, FormID = string.Empty;

        try
        {
            dynamic json = JToken.Parse(Fields) as dynamic;
            IDName = "WFID";
            IDValue = json["IDValue"];

            FieldName = json["FieldName"];
            FieldValue = json["FieldValue"];
            FieldValue = FieldValue.toStringTrim();
            FormID = json["FormID"];

            TableName = "WorkFlow";

            //更新的字段为默认流程字段时，先把该表单的所有流程设置为false，再去更新选中的值为true
            //*20210121注*暂不需要恢复所有值为false,因为流程存在的方式能是按生效范围进行区分，用Checkbox代替Radio,如按全公司生效可以设置一个默认流程，如按：职位职称、角色、部门也可以设置一个默认流程
            //if (FieldName == "DefaultFlow")
            //{
            //    string _sql = "update WorkFlow set DefaultFlow=@DefaultFlow, DateModified=@DateModified where ParentID=0 and FormID=(select FormID from WorkFlow where WFID=@WFID)";
            //    SqlParameter[] _sp = {new SqlParameter("@DefaultFlow", SqlDbType.Bit),
            //                          new SqlParameter("@DateModified",SqlDbType.DateTime),
            //                        new SqlParameter("@WFID",SqlDbType.Int)
            //                        };

            //    _sp[0].Value = false;
            //    _sp[1].Value = DateTime.Now;
            //    _sp[2].Value = IDValue.toInt();

            //    MsSQLDbHelper.ExecuteSql(_sql, _sp);
            //}

            string _sql2 = "update " + TableName + " set " + FieldName + "=@" + FieldName + " , DateModified=@DateModified where " + IDName + "=@" + IDName + " ";
            SqlParameter[] _sp2 = new SqlParameter[3];
            //获取字段的类型和长度生成动态的SqlParameter
            switch (FieldName)
            {
                case "DefaultFlow":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.toBoolean();
                    break;
                case "Sort":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Int);
                    _sp2[0].Value = FieldValue.toInt();
                    break;
                case "FlowName":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.NVarChar, 100);
                    _sp2[0].Value = FieldValue;
                    break;
                case "IsAccept":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.toBoolean();
                    break;
                case "IsSync":
                    _sp2[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp2[0].Value = FieldValue.toBoolean();
                    break;

            }
            _sp2[1] = new SqlParameter("@DateModified", SqlDbType.DateTime);
            _sp2[1].Value = DateTime.Now;

            _sp2[2] = new SqlParameter("@" + IDName, SqlDbType.Int);
            _sp2[2].Value = IDValue;

            if (MsSQLDbHelper.ExecuteSql(_sql2, _sp2) > 0)
                flag = MicroPublic.GetMsg("Modify");
        }
        catch { }

        return flag;

    }

    private string DelWorkFlow(string WFID)
    {
        string flag = MicroPublic.GetMsg("DelFailed");

        string _sql = "update WorkFlow set Del=@Del where WFID=@WFID";
        SqlParameter[] _sp = { new SqlParameter("@Del",SqlDbType.Bit),
                            new SqlParameter("@WFID",SqlDbType.Int)
                            };
        _sp[0].Value = true;
        _sp[1].Value = WFID.toInt();

        if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
            flag = MicroPublic.GetMsg("Del");

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