<%@ WebHandler Language="C#" Class="ModulePerms" %>

using System;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroDTHelper;
using Newtonsoft.Json.Linq;
using MicroPublicHelper;
using MicroAuthHelper;

public class ModulePerms : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = MicroPublic.GetMsg("SaveFailedTry"),
                Action = context.Server.UrlDecode(context.Request.Form["action"]),
                MID = context.Server.UrlDecode(context.Request.Form["mid"]),
                Type = context.Server.UrlDecode(context.Request.Form["type"]),
                Fields = context.Server.UrlDecode(context.Request.Form["fields"]);
        //测试数据
        //Fields = "{\"ckPerm11\":\"1,1\",\"ckPerm12\":\"1,2\"}";

        if (Type == "recovery")
            flag = GetDefault();
        else
            flag = SetModify(Type.toStringTrim(), Fields.toStringTrim());

        context.Response.Write(flag);
    }

    protected string SetModify(string Type, string Fields)
    {
        string flag = MicroPublic.GetMsg("SaveFailedTry");
        Boolean DefaultPerm = false;
        if (Type == "default")
            DefaultPerm = true;

        try
        {
            DataTable ModulePermDT = MicroDataTable.GetDataTable("MPerm"); //得到模权限表的记录
            DataTable NewModulePermDT = ModulePermDT.Clone();  //克隆相同结构的新表，把符合条件的记录插入到该表，最后再统一写入数据库

            //创建sb用于存放已选中的权限的MPID,构造成逗号分隔的字符串，最后用 MPID not in进行删除数据数已有而选中的权限不包含的记录 
            StringBuilder SB = new StringBuilder();

            string strMID = string.Empty;
            string strPID = string.Empty;

            JObject Array = JObject.Parse(Fields);

            foreach (var Item in Array)
            {
                int intMID = MicroPublic.GetSplitStr(Item.Value.ToString(), ',', 0).toInt();
                int intPID = MicroPublic.GetSplitStr(Item.Value.ToString(), ',', 1).toInt();


                DataRow[] ModulePermRows = ModulePermDT.Select("MID=" + intMID + " and PID=" + intPID + "");

                //已选中且数据库没有的记录放进DataTable
                if (ModulePermRows.Length == 0)
                {
                    //没有的记录统一暂时写入New DataTable，最后再写入数据库
                    DataRow NewModulePermRow = NewModulePermDT.NewRow();
                    NewModulePermRow["MID"] = intMID;
                    NewModulePermRow["PID"] = intPID;
                    NewModulePermRow["DefaultPerm"] = DefaultPerm;
                    NewModulePermDT.Rows.Add(NewModulePermRow);
                }
                else
                {
                    //已选中且数据库已有的记录进行数据更新
                    string _sql = "update ModulePermissions set DefaultPerm=@DefaultPerm where MID=@MID and PID=@PID and DefaultPerm=0";
                    SqlParameter[] _sp = {
                        new SqlParameter("@DefaultPerm",SqlDbType.Bit),
                        new SqlParameter("@MID",SqlDbType.Int),
                        new SqlParameter("@PID",SqlDbType.Int),
                                 };

                    _sp[0].Value = DefaultPerm;
                    _sp[1].Value = intMID;
                    _sp[2].Value = intPID;

                    MsSQLDbHelper.ExecuteSql(_sql, _sp);

                    SB.Append(ModulePermRows[0]["MPID"].ToString() + ",");
                }
                strMID += intMID.ToString() + ",";
                strPID += intPID.ToString() + ",";
            }

            //删除没有选中且数据库已存在的记录，再插入已选中且数据库没有的记录
            string MPID = SB.ToString();
            if (MPID.Length > 0)
            {
                MPID = MPID.Substring(0, MPID.Length - 1);
                string _sql = " delete ModulePermissions where MPID not in (" + MPID + ") and DefaultPerm=0";
                MsSQLDbHelper.ExecuteSql(_sql);
            }
            else
            {
                //一个都没有选中的时候删除非默认的
                string _sql = " delete ModulePermissions where DefaultPerm=0";
                MsSQLDbHelper.ExecuteSql(_sql);
            }

            //把已选中的（即DataTable）写入记录数据库
            if (MsSQLDbHelper.SqlBulkCopyInsert(NewModulePermDT, "ModulePermissions"))
                flag = MicroPublic.GetMsg("Save");

        }
        catch (Exception ex) { flag = ex.ToString(); }

        return flag;

    }

    /// <summary>
    /// 恢复默认
    /// </summary>
    /// <returns></returns>
    protected string GetDefault()
    {
        string flag = MicroPublic.GetMsg("RecoveryFailed");

        string _sql = "delete ModulePermissions where DefaultPerm=0 ";

        if (MsSQLDbHelper.ExecuteSql(_sql) >= 0)
            flag = MicroPublic.GetMsg("Recovery");

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