using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroDTHelper;
using MicroPublicHelper;
using MicroAuthHelper;
using Newtonsoft.Json.Linq;

public partial class Views_UserCenter_ModulePerms : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //动作Action 可选值Add、Modify、View
        string Action = MicroPublic.GetFriendlyUrlParm(0);
        txtAction.Value = Action;
        string ShortTableName = MicroPublic.GetFriendlyUrlParm(1);
        txtShortTableName.Value = ShortTableName;
        string ModuleID = MicroPublic.GetFriendlyUrlParm(2);
        txtMID.Value = ModuleID;

        //检查是否已经登录和页面唯一识别是否一致（ShortTableName）
        MicroAuth.CheckAuth(ModuleID, ShortTableName);

        //检查是否有页面浏览权限
        MicroAuth.CheckBrowse(ModuleID);

        if (!MicroAuth.CheckPermit(ModuleID, "3"))
        {
            btnOpenLink.Disabled = true;
            btnOpenLink.Attributes.Add("class", "layui-btn layui-btn-sm layui-btn-disabled");

            btnSave.Disabled = true;
            btnSave.Attributes.Add("class", "layui-btn layui-btn-disabled");

            btnSetDefault.Disabled = true;
            btnSetDefault.Attributes.Add("class", "layui-btn layui-btn-disabled");

            btnRecovery.Disabled = true;
            btnRecovery.Attributes.Add("class", "layui-btn layui-btn-disabled");
        }

    }

    protected string GetModulePerm()
    {
        
        DataTable ModuleDT = MicroDataTable.GetDataTable("Mod");
        DataTable PermDT = MicroDataTable.GetDataTable("Perm");
        DataTable ModulePermDT = MicroDataTable.GetDataTable("MPerm");

        string Str = string.Empty;
        try
        {
            //*****标题部分*****
            Str += "<div class=\"layui-form-item ws-font-bold\" style=\"background-color:#f2f2f2;\" >";
            Str += "<div class=\"layui-inline ws-width100\" style=\"left:10px;\">";
            Str += "<input type=\"checkbox\" name=\"\" title=\"模块/权限\" lay-skin=\"primary\" id=\"cbAll\" lay-filter=\"cbAll\" />";
            Str += "</div>";

            if (PermDT != null && PermDT.Rows.Count > 0)
            {
                DataRow[] _rows = PermDT.Select("");
                foreach (DataRow _dr in _rows)
                {
                    Str += "<div class=\"layui-inline ws-width60\">";
                    Str += "<label class=\"layui-form-label ws-width70\">" + _dr["PermissionName"].ToString() + "</label>";
                    Str += "</div>";
                }
            }
            Str += "</div>";

            //*****显示模块部分*****
            Str += " <div class=\"ws-scrollbar\"  style=\"padding-left:20px; overflow:auto !important; padding-bottom:10px; height:620px; \">";
            if (ModuleDT != null && ModuleDT.Rows.Count > 0)
            {
                DataRow[] ModuleRows = ModuleDT.Select("");
                foreach (DataRow ModuleDR in ModuleRows)
                {
                    string MID = ModuleDR["MID"].ToString();
                    string MainSub = ModuleDR["MainSub"].ToString() == "Main" ? "" : ModuleDR["MainSub"].ToString() + " ";
                    string FontBold = ModuleDR["ParentID"].ToString() == "0" ? " ws-font-bold" : "";
                    string Left = ModuleDR["ParentID"].ToString() == "0" ? " 【" : "";
                    string Right = ModuleDR["ParentID"].ToString() == "0" ? " 】" : "";

                    Str += "<div class=\"layui-form-item\" style=\"border-bottom: solid 1px #f2f2f2;\">";

                    Str += "<div class=\"layui-inline" + FontBold + "\" style=\"width:120px; padding-bottom:5px;\">";
                    Str += "<input type =\"checkbox\" id=\"cbModuleCheckAll" + MID + "\" name=\"\" lay-skin=\"primary\" lay-filter=\"cbModuleCheckAll\" title=\"" + Left + MainSub + ModuleDR["ModuleName"].ToString() + Right + "\" value=\"divPerm" + MID + "\">";
                    Str += "</div>";

                    Str += "<div id =\"divPerm" + MID + "\" class=\"layui-inline\" style=\"padding-left:5px;\">";

                    if (PermDT != null && PermDT.Rows.Count > 0)
                    {
                        DataRow[] PermRows = PermDT.Select("");
                        foreach (DataRow PermDR in PermRows)
                        {
                            string PID = PermDR["PID"].ToString();
                            string Value = MID + "," + PID;

                            //*****读取已存在的记录让CheckBox选中*****
                            string Checked = string.Empty;
                            string Disabled = string.Empty;
                            string Skin = "primary";
                            DataRow[] ModulePermDRows = ModulePermDT.Select("MID=" + MID.toInt() + " and PID=" + PID.toInt() + "");
                            if (ModulePermDRows.Length > 0)
                            {
                                Checked = " checked=\"checked\"";

                                if (ModulePermDRows[0]["DefaultPerm"].ToString() == "True")
                                {
                                    Disabled = " disabled=\"disabled\"";
                                    Skin = "gray";
                                }
                            }
                            //*****读取已存在的记录让CheckBox选中*****

                            Str += "<div class=\"layui-inline\" style=\"width: 60px; text-align:center; \">";
                            Str += "<input type =\"checkbox\" id=\"ckPerm" + Value + "\" name=\"ckPerm" + Value + "\" lay-skin=\"" + Skin + "\" value=\"" + Value + "\"" + Checked + Disabled + " style=\"color:#f2f2f2;\">";
                            Str += "</div>";
                        }
                    }
                    Str += "</div>";

                    Str += "</div>";
                }
            }
            Str += "</div>";
        }
        catch (Exception ex) { Response.Write(ex.ToString()); }

        return Str;
    }
}