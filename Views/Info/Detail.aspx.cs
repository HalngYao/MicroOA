using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MicroDBHelper;
using MicroPublicHelper;
using MicroAuthHelper;
using MicroUserHelper;

public partial class Views_Info_Detail : System.Web.UI.Page
{
    string ModuleID = MicroPublic.GetFriendlyUrlParm(0); //"27";
    protected void Page_Load(object sender, EventArgs e)
    {
        //检查是否已经登录和页面唯一识别是否一致（ShortTableName）
        MicroAuth.CheckAuth(ModuleID, "InfoList");

        //检查是否有页面浏览权限
        MicroAuth.CheckBrowse(ModuleID);

        txtMID.Value = ModuleID;
    }

    protected string GetDetail()
    {
        string InfoID = MicroPublic.GetFriendlyUrlParm(1);
        int UID = MicroUserInfo.GetUserInfo("UID").toInt();

        Boolean ApprovalPermit = MicroAuth.CheckPermit(ModuleID, "12"),
           EditPermit = MicroAuth.CheckPermit(ModuleID, "3"),  //修改自己
           EditAllPermit = MicroAuth.CheckPermit(ModuleID, "10"), //修改所有
           DelPermit = MicroAuth.CheckPermit(ModuleID, "16"),  //删除自己
           DelAllPermit = MicroAuth.CheckPermit(ModuleID, "18"),  //删除所有
           ViewAllPermit = MicroAuth.CheckPermit(ModuleID, "6");  //查看所有

        string flag = "<div class=\"fly-panel detail-box\"><div class=\"fly-none\">没有相关数据</div></div>";

        string _sql = "select a.*,b.InfoClassName,b.Description,c.DeptName from Information a left join InformationClass b on a.InfoClassID=b.ICID left join Department c on a.DeptID=c.DeptID where PushToInfoPlatform=1 and InfoID=@InfoID and a.Invalid=0 and a.Del=0  ";
        SqlParameter[] _sp = { new SqlParameter("@InfoID", SqlDbType.Int) };

        _sp[0].Value = InfoID.toInt();

        DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

        //得到当前用户角色
        string _sqlUserRoles = "select * from UserRoles where Invalid=0 and Del=0 and UID=" + UID + "";
        DataTable _dtUserRoles = MsSQLDbHelper.Query(_sqlUserRoles).Tables[0];

        //得到当前用户职位
        string _sqlJobTitle = "select * from UserJobTitle where Invalid=0 and Del=0 and UID=" + UID + "";
        DataTable _dtJobTitle = MsSQLDbHelper.Query(_sqlJobTitle).Tables[0];

        //得到当前用户部门
        string _sqlUserDepts = "select * from UserDepts where Invalid=0 and Del=0 and UID=" + UID + "";
        DataTable _dtUserDepts = MsSQLDbHelper.Query(_sqlUserDepts).Tables[0];

        if (_dt.Rows.Count > 0)
        {
            Boolean ViewDetailPermit = false;
            //如果有查看全部权限的则允许查看
            if (ViewAllPermit)
                ViewDetailPermit = true;
            else
            {
                //当没有查看全部权限时，再判断信息是否审批通过，如果通过时
                if (_dt.Rows[0]["InfoState"].toBoolean())
                {
                    //当信息审批通过再判断是不是公开的信息，当信息不开时判断有权限的才可以查看
                    if (_dt.Rows[0]["IsPublic"].toBoolean())
                        ViewDetailPermit = true;
                    else
                    {
                        int intUserRoles = 0,
                            intUserJobTitle = 0,
                            intUserDepts = 0;

                        if (_dtUserRoles.Rows.Count > 0)
                        {
                            string AllowRoles = _dt.Rows[0]["AllowRoles"].toStringTrim();
                            AllowRoles = string.IsNullOrEmpty(AllowRoles) == true ? "0" : AllowRoles;
                            intUserRoles = _dtUserRoles.Select("RID in (" + AllowRoles + ")").Length;
                        }

                        if (_dtJobTitle.Rows.Count > 0)
                        {
                            string AllowJobTitle = _dt.Rows[0]["AllowJobTitle"].toStringTrim();
                            AllowJobTitle = string.IsNullOrEmpty(AllowJobTitle) == true ? "0" : AllowJobTitle;
                            intUserJobTitle = _dtJobTitle.Select("JTID in (" + AllowJobTitle + ")").Length;
                        }

                        if (_dtUserDepts.Rows.Count > 0)
                        {
                            string AllowDepts = _dt.Rows[0]["AllowDepts"].toStringTrim();  //得到文章逗号分隔的AllowDepts 如：1,2,3,4
                            AllowDepts = string.IsNullOrEmpty(AllowDepts) == true ? "0" : AllowDepts;
                            AllowDepts = MicroUserInfo.GetAllDeptsID("AllSubDepts", "DeptID", AllowDepts);  //得到AllowDepts所有子部门的DetpIDs
                            intUserDepts = _dtUserDepts.Select("DeptID in (" + AllowDepts + ")").Length;  //与当前用户的部门表进行对比
                        }

                        //检查当前文章是否为当前用户
                        Boolean CheckCurrUsers = false;
                        if (UID != 0 && UID == _dt.Rows[0]["UID"].toInt())
                            CheckCurrUsers = true;

                        //当允许的角色或允许的职位或允许的用户或是自己发布的其中有一个值为真则插入记录
                        if (intUserRoles > 0 || intUserJobTitle > 0 || intUserDepts > 0 || MicroUserInfo.CheckUID(_dt.Rows[0]["AllowUsers"].toStringTrim()) || CheckCurrUsers)
                            ViewDetailPermit = true;
                    }
                }
                else  //否则还没有经过审批的，则只有拥有审批权限的人才可查看
                {
                    //没有经过审批的，则只拥有审批权限的人才可查看，否则只能查看自己发布的
                    if (ApprovalPermit)
                        ViewDetailPermit = true;
                    else
                    {
                        if (UID == _dt.Rows[0]["UID"].toInt() && UID != 0)
                            ViewDetailPermit = true;
                    }
                }

            }

            if (ViewDetailPermit)
            {
                //为阅读量递增1
                MicroPublic.SetFieldIncrease("Information", "Reading", "InfoID", InfoID);

                for (int i = 0; i < 1; i++)
                {
                    Boolean InfoState = _dt.Rows[i]["InfoState"].toBoolean();
                    string Source = _dt.Rows[i]["Source"].toStringTrim();
                    string DeptName = _dt.Rows[i]["DeptName"].toStringTrim();
                    Source = string.IsNullOrEmpty(Source) ? DeptName : Source;
                    string Secrecy = _dt.Rows[i]["Secrecy"].toStringTrim();

                    flag = "<div class=\"fly-panel detail-box\">";
                    flag += "<h1>" + _dt.Rows[i]["Title"].toStringTrim() + "</h1>";
                    flag += "<div class=\"fly-detail-info\">";
                    flag += "<span class=\"ws-font-gray\" style=\"margin-right:20px;\">" + DateTime.Parse(_dt.Rows[i]["DateCreated"].toStringTrim()).ToString("yyyy-MM-dd HH:mm:ss") + "</span>";
                    flag += "<span class=\"ws-font-gray\" style=\"margin-right:20px;\">来源：" + Source + "</span>";
                    flag += "<span class=\"ws-font-gray\" style=\"margin-right:20px;\">分類：" + _dt.Rows[i]["InfoClassName"].toStringTrim() + "</span>";

                    //不公开时，加上锁头图标
                    if (!_dt.Rows[i]["IsPublic"].toBoolean())
                        flag += "<span style=\"margin-right:20px;\"><i class=\"ws-icon icon-erji-quanxianguanli\" lay-tips=\"機密文書\"></i></span>";

                    //机密等级不为空时显示机密标签
                    if (!string.IsNullOrEmpty(Secrecy))
                        flag += "<span class=\"layui-badge layui-bg-red fly-detail-column\" style=\"margin-right:5px;\" lay-tips=\"機密等級：" + Secrecy + "\">" + Secrecy + "</span>";

                    //还没有审批通过时，打上待审批标签
                    if (!_dt.Rows[i]["InfoState"].toBoolean())
                        flag += "<span class=\"layui-badge layui-bg-red fly-detail-column\" style=\"margin-right:5px;\">待审批</span>";

                    flag += "<span class=\"layui-badge layui-bg-green fly-detail-column\" style=\"margin-right:5px;\">" + _dt.Rows[i]["Nature"].toStringTrim() + "</span>";

                    if (_dt.Rows[i]["PlatformTop"].toBoolean())
                        flag += "<span class=\"layui-badge layui-bg-red\" style=\"margin-right:20px;\">置顶</span>";

                    flag += "<div class=\"fly-list-badge\" style=\"margin-top:-15px;\">";

                    if (!InfoState && ApprovalPermit)
                        flag += "<a href=\"javascript:;\"><span class=\"layui-badge layui-bg-red micro-click\" data-type=\"Approval\" lay-tips=\"审批中的信息只有管理员可见\" micro-stn=\"Info\" micro-id=\"" + _dt.Rows[i]["InfoID"].toStringTrim() + "\">审批</span></a>";

                    //拥有编辑所有权限时显示编辑按钮
                    if (EditAllPermit)
                        flag += "<a href=\"/Views/Forms/SysNonPopForm/Modify/Info/" + ModuleID + "/" + _dt.Rows[i]["InfoID"].toStringTrim() + "\" target=\"_blank\"><span class=\"layui-badge layui-bg-blue\">编辑</span></a>";
                    else
                    {
                        //否则没有编辑所有权限时，如果有编辑单个权限并且紧只能编辑自己发布的信息
                        if (EditPermit && UID == _dt.Rows[i]["UID"].toInt() && UID != 0)
                            flag += "<a href=\"/Views/Forms/SysNonPopForm/Modify/Info/" + ModuleID + "/" + _dt.Rows[i]["InfoID"].toStringTrim() + "\" target=\"_blank\"><span class=\"layui-badge layui-bg-blue\">编辑</span></a>";
                    }

                    if ((DelPermit && UID == _dt.Rows[i]["UID"].toInt() && UID != 0) || DelAllPermit)
                        flag += "<a href=\"javascript:;\"><span class=\"layui-badge layui-bg-black micro-click\" data-type=\"Del\" micro-stn=\"Info\" micro-id=\"" + _dt.Rows[i]["InfoID"].toStringTrim() + "\">删除</span></a>";

                    //拥有编辑所有权限时显示置顶按钮
                    if (EditAllPermit)
                    {
                        if (_dt.Rows[i]["PlatformTop"].toBoolean())
                            flag += "<a href=\"javascript:;\"><span class=\"layui-badge micro-click\" data-type=\"CtrlTop\"  style=\"background-color:#ccc; margin-right:20px;\" micro-type=\"Platform\" micro-stn=\"Info\" micro-id=\"" + _dt.Rows[i]["InfoID"].toStringTrim() + "\" micro-val=\"NoTop\">取消置顶</span></a>";
                        else
                            flag += "<a href=\"javascript:;\"><span class=\"layui-badge layui-bg-red micro-click\" data-type=\"CtrlTop\" style=\"margin-right:20px;\" micro-type=\"Platform\" micro-stn=\"Info\" micro-id=\"" + _dt.Rows[i]["InfoID"].toStringTrim() + "\" micro-val=\"SetTop\">置顶</span></a>";
                    }
                    else
                    {
                        //否则没有编辑所有权限时，如果有编辑单个权限并且紧只能置顶自己发布的信息
                        if (EditPermit && UID == _dt.Rows[i]["UID"].toInt() && UID != 0)
                        {
                            if (_dt.Rows[i]["PlatformTop"].toBoolean())
                                flag += "<a href=\"javascript:;\"><span class=\"layui-badge micro-click\" data-type=\"CtrlTop\"  style=\"background-color:#ccc; margin-right:20px;\" micro-type=\"Platform\" micro-stn=\"Info\" micro-id=\"" + _dt.Rows[i]["InfoID"].toStringTrim() + "\" micro-val=\"NoTop\">取消置顶</span></a>";
                            else
                                flag += "<a href=\"javascript:;\"><span class=\"layui-badge layui-bg-red micro-click\" data-type=\"CtrlTop\" style=\"margin-right:20px;\" micro-type=\"Platform\" micro-stn=\"Info\" micro-id=\"" + _dt.Rows[i]["InfoID"].toStringTrim() + "\" micro-val=\"SetTop\">置顶</span></a>";
                        }
                    }

                    flag += "<span class=\"ws-font-gray ws-font-size14\"> ";
                    flag += "<i class=\"ws-icon icon-yueduliang ws-font-gray\" title=\"阅读\" style=\"padding-top:-20px; font-size:18px; \"></i>" + (_dt.Rows[i]["Reading"].toInt() + 1) + "";
                    flag += "</span>";
                    flag += "</div>";

                    flag += "<hr />";
                    flag += "</div>";
                    flag += "<div class=\"detail-body photos\">";
                    flag += "<div> " + _dt.Rows[i]["Content"].toStringTrim() + " </div>";
                    flag += "</div>";
                    flag += "</div>";
                }
            }
        }

        return flag;

    }
}