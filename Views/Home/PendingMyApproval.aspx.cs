using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MicroDBHelper;
using MicroDTHelper;
using MicroPublicHelper;
using MicroUserHelper;

public partial class Views_home_PendingMyApproval : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        string ModuleID = MicroPublic.GetFriendlyUrlParm(0);
        txtMID.Value = ModuleID;
    }


    protected string GetPendingMyApproval()
    {
        string flag = string.Empty;

        string _sql = "  select FCID,ClassName from FormClassification where Invalid=0 and Del=0 and ParentID<>0 order by Sort";

        DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

        if (_dt != null && _dt.Rows.Count > 0)
        {
            string str = string.Empty,
                UID = MicroUserInfo.GetUserInfo("UID");

            for (int i = 0; i < _dt.Rows.Count; i++)
            {
                string FCID = _dt.Rows[i]["FCID"].toStringTrim(),
                    ClassName = _dt.Rows[i]["ClassName"].toStringTrim();

                str += "<div class=\"layui-col-md6\">";
                str += "<div class=\"layui-card\" > ";
                str += "<div class=\"layui-card-header layui-elip\" > " + ClassName + "</div>";
                str += "<div class=\"layui-card-body\">";

                str += "<div class=\"layui-carousel layadmin-carousel layadmin-backlog\">";
                str += "<div carousel-item>";

                str += "<ul class=\"layui-row layui-col-space10\">";
                str += GetPendingMyApprovalList(FCID);
                str += "</ul>";

                str += "</div>";
                str += "</div>";
                str += "</div>";
                str += "</div>";
                str += "</div>";

            }
            flag = str;

        }
        return flag;

    }


    protected string GetPendingMyApprovalList(string FCID)
    {
        string flag = string.Empty;
        string _sql = "  select FormID,FormName,ShortTableName from Forms where Invalid=0 and Del=0 and FCID=" + FCID.toInt() + " order by Sort";
        DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

        if (_dt != null && _dt.Rows.Count > 0)
        {
            string str = string.Empty,
                UID = MicroUserInfo.GetUserInfo("UID");

            string ColNum = "6";

            if (_dt.Rows.Count > 4 && _dt.Rows.Count <= 6)
                ColNum = "4";

            else if (_dt.Rows.Count > 6 && _dt.Rows.Count <= 8)
                ColNum = "3";

            for (int i = 0; i < _dt.Rows.Count; i++)
            {
                string FormID = _dt.Rows[i]["FormID"].toStringTrim(),
                    FormName = _dt.Rows[i]["FormName"].toStringTrim(),
                    ShortTableName = _dt.Rows[i]["ShortTableName"].toStringTrim(),
                    FontRed = "ws-font-gray3",
                    layhref = string.Empty;

                string _sql2 = "" +
                    //一阶 关联相关表
                    "select a.*,b.FormName,b.ShortTableName,c.EMail,c.WorkTel,c.WorkMobilePhone,c.AdDepartment from FormApprovalRecords a left join Forms b on a.FormID=b.FormID left join UserInfo c on a.UID=c.UID " +
                    //二阶 得到在审批阶段最小的需要审批的记录
                    "where FARID in (select min(FARID) from FormApprovalRecords where WorkFlowID in (select WFID from WorkFlow where Invalid=0 and Del=0 and FlowCode='Approval') and Invalid=0 and Del=0 and StateCode=0 group by FormID,FormsID) " +
                    //三阶 去除在任意审批阶段被驳回的记录 （注：申请、受理、结案不算审批阶段）
                    "and FormNumber not in(select FormNumber from FormApprovalRecords where FARID in (select max(FARID) from FormApprovalRecords where StateCode < 0 and Invalid = 0 and Del = 0 group by FormID,FormsID)) " +
                    //四阶 与我相关的
                    "and CHARINDEX(',' + convert(varchar, " + UID + ") + ',',',' + CanApprovalUID + ',')> 0 and a.Invalid = 0 and a.Del = 0 and b.Invalid = 0 and b.Del = 0 and a.FormID=" + FormID.toInt() + "";

                DataTable _dt2 = MsSQLDbHelper.Query(_sql2).Tables[0];

                if (_dt2.Rows.Count > 0)
                {
                    FontRed = "ws-font-red";
                    layhref = "lay-href=\"/Views/Forms/MicroFormList/View/" + ShortTableName + "/4/" + FormID + "/1/DefaultNumber/GetPendingMyApprovalList/" + DateTime.Now.AddDays(-30).toDateFormat("yyyy-MM-dd") + "/" + DateTime.Now.AddDays(30).toDateFormat("yyyy-MM-dd") + "\"";
                }

                str += "<li class=\"layui-col-xs" + ColNum + "\">";
                str += "<a id=\"aPendingMyApproval\" lay-text=\"" + FormName + "【待我审批】\" class=\"layadmin-backlog-body " + FontRed + "\" " + layhref + ">";
                str += "<h3  class=\"layui-elip\">"+ FormName + "</h3>";
                str += "<p ><cite class=\"" + FontRed + "\">" + _dt2.Rows.Count + "</cite></p>";
                str += "</a>";
                str += "</li>";

            }
            flag = str;

        }
        return flag;
    }


}