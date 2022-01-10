<%@ WebHandler Language="C#" Class="GetParSubLevel" %>

using System;
using System.Web;
using System.Data;
using MicroDBHelper;
using Newtonsoft.Json.Linq;
using MicroPublicHelper;
using MicroAuthHelper;

/// <summary>
/// 有父和子项关系的表，获取它们的Level和LevelCode返回给前端
/// </summary>
public class GetParSubLevel : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = string.Empty,
                    Action = context.Server.UrlDecode(context.Request.Form["action"]),
                    MID = context.Server.UrlDecode(context.Request.Form["mid"]),
                    Fields = context.Server.UrlDecode(context.Request.Form["fields"]);
        //测试数据
        //Fields = "{ \"idname\": \"DeptID\", \"val\": \"1\", \"stn\": \"Dept\"}";
        if (!string.IsNullOrEmpty(Action) && !string.IsNullOrEmpty(Fields))
            flag = GetLevel(Action, Fields);

        context.Response.Write(flag);
    }

    private string GetLevel(string Action, string Fields)
    {
        string flag = string.Empty, strTpl = GetStrTpl(), ShortTableName = string.Empty, IDName = string.Empty, Value = string.Empty;
        try
        {
            dynamic json = JToken.Parse(Fields) as dynamic;
            ShortTableName = json["stn"]; IDName = json["idname"]; Value = json["val"];

            //测试数据
            //ShortTableName = "Dept"; IDName = "DeptID"; Value = "1";

            string TableName = MicroPublic.GetTableName(ShortTableName);

            //*****获取Level*****
            //根据子节点得到所有父节点的数量，然后+1作为新的Level，返回给前端
            string _sql = "with Temp AS( select * from " + TableName.toStringTrim() + " where " + IDName + "=" + Value.toInt() + " UNION ALL select t.* from Temp," + TableName.toStringTrim() + " t where Temp.ParentID=t." + IDName + ") select count(*) as q from Temp";
            string Level = ((MicroPublic.GetSingleField(_sql, 0)).toInt()).ToString();

            //*****获取LevelCode和Sort*****
            string LevelCode = string.Empty, Sort = string.Empty;
            if (!string.IsNullOrEmpty(Value) || Value != "0") //当Value不为空或不为0时（即有发生Select Change事件），
            {
                //获取当前节点的LevelCode(0101),然后+1作为新的LevelCode返回给前端
                string _sql2 = "select * from " + TableName.toStringTrim() + " where ParentID=" + Value.toInt() + "";
                DataTable _dt2 = MsSQLDbHelper.Query(_sql2).Tables[0];

                if (_dt2 != null && _dt2.Rows.Count > 0)  //当前节点的子节点不为空时
                {
                    //获取LevelCode
                    LevelCode = (_dt2.Compute("Max(LevelCode)", "")).ToString(); //得到最大的LevelCode
                    string LeftLevelCode = LevelCode.Substring(0, LevelCode.Length - 2);  //得到左边字符串（除去右边两位）
                    string RightLevelCode = LevelCode.Substring(LevelCode.Length - 2, 2);  //得到右边两位
                    int intRightLevelCode = RightLevelCode.toInt() + 1;

                    if (intRightLevelCode < 10)  //如 01转为数字类型后，前面的0会被省略，所以小于10时补0
                        RightLevelCode = "0" + intRightLevelCode.ToString();
                    else
                        RightLevelCode = intRightLevelCode.ToString();

                    LevelCode = LeftLevelCode + RightLevelCode;

                    //获取Sort
                    Sort = (_dt2.Compute("Max(Sort)", "")).ToString();
                    Sort = (Sort.toInt() + 1).ToString();
                }
                else //当前节点为的子节点为空时，把where参数变为IDName（ID字段）进行查询，获取LevelCode
                {
                    //获取LevelCode
                    //此时当前节点变作为父节点，它的LeveCode再拼接上01作为当前节点的第一个子节点的LevelCode
                    string _sql3 = "select LevelCode from " + TableName.toStringTrim() + " where " + IDName + "=" + Value.toInt() + "";
                    LevelCode = (MicroPublic.GetSingleField(_sql3, 0)).ToString() + "01";

                    //获取Sort
                    //因为当前节点无任何子节点，所以直接赋值1
                    Sort = "1";
                }
            }
            else  //当Value为空或0时（第一次加载页面传入的ajax），
            {
                string _sql4 = "select * from " + TableName.toStringTrim() + " where ParentID=0";
                DataTable _dt4 = MsSQLDbHelper.Query(_sql4).Tables[0];

                if (_dt4 != null && _dt4.Rows.Count > 0)  //当记录不为空时
                {
                    //获取LevelCode
                    LevelCode = (_dt4.Compute("Max(LevelCode)", "")).ToString(); //得到最大的LevelCode
                    string LeftLevelCode = LevelCode.Substring(0, LevelCode.Length - 2);  //得到左边字符串（除去右边两位）
                    string RightLevelCode = LevelCode.Substring(LevelCode.Length - 2, 2);  //得到右边两位
                    int intRightLevelCode = RightLevelCode.toInt() + 1;

                    if (intRightLevelCode < 10)  //因01转为数字类型后，前面的0会被省略，所以小于10时补0
                        RightLevelCode = "0" + intRightLevelCode.ToString();
                    else
                        RightLevelCode = intRightLevelCode.ToString();

                    LevelCode = LeftLevelCode + RightLevelCode;

                    //获取Sort
                    Sort = (_dt4.Compute("Max(Sort)", "")).ToString(); //得到最大的Sort
                    Sort = (Sort.toInt() + 1).ToString();
                }
                else
                {
                    //获取LevelCode
                    //没有记录时直接赋值01
                    LevelCode = "01";

                    //获取Sort
                    //没有记录时直接赋值1
                    Sort = "1";
                }
            }

            flag = string.Format(strTpl, Level, LevelCode, Sort);
            flag = ("{" + flag + "}");

        }
        catch (Exception ex) { flag = ex.ToString(); }
        return flag;

    }

    private string GetStrTpl()
    {
        //Level第几层=数字，LevelCode层给代码=0101，Sort排序=数字
        string strTpl = "\"Level\":\"{0}\",\"LevelCode\":\"{1}\",\"Sort\":\"{2}\"";
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