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

public partial class Views_Info_List : System.Web.UI.Page
{
    string ModuleID = "27";   //以“信息管理平台->信息平台” 菜单的ModuleID作为信息平台的权限设置
    public Boolean AdminRole = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        AdminRole = MicroUserInfo.CheckUserRole("Administrators");

        //检查是否已经登录和页面唯一识别是否一致（ShortTableName）
        MicroAuth.CheckAuth(ModuleID, "InfoList");

        //检查是否有页面浏览权限
        MicroAuth.CheckBrowse(ModuleID);

        Boolean ApprovalPermit = MicroAuth.CheckPermit(ModuleID, "12"),
           EditPermit = MicroAuth.CheckPermit(ModuleID, "3"),  //修改自己
           EditAllPermit = MicroAuth.CheckPermit(ModuleID, "10"), //修改所有
           DelPermit = MicroAuth.CheckPermit(ModuleID, "16"),  //删除自己
           DelAllPermit = MicroAuth.CheckPermit(ModuleID, "18"),  //删除所有
           ViewAllPermit = MicroAuth.CheckPermit(ModuleID, "6");  //查看所有

        txtMID.Value = ModuleID;

        string InfoClassID = MicroPublic.GetFriendlyUrlParm(0);
        txtInfoClassID.Value = InfoClassID.toInt().ToString();

        string Keyword = MicroPublic.GetFriendlyUrlParm(1);
        txtAuxKeyword.Value = Keyword.toStringTrim();

        GetInfo(ApprovalPermit, EditPermit, EditAllPermit, DelPermit, DelAllPermit, ViewAllPermit, InfoClassID, Keyword);

    }


    private string GetInfo(Boolean ApprovalPermit, Boolean EditPermit, Boolean EditAllPermit, Boolean DelPermit, Boolean DelAllPermit, Boolean ViewAllPermit, string InfoClassID, string Keyword)
    {
        string flag = "<div class=\"fly-none\">没有相关数据</div>";
        int UID = MicroUserInfo.GetUserInfo("UID").toInt();

        string InfoClassIDStr = string.Empty,
            KeywordQueryStr = string.Empty;

        //如果开启全局搜索并且关键字不为空时设置InfoClassID="0"，否则在分类内搜索
        Boolean IsGlobalSearch = MicroPublic.GetMicroInfo("InfoIsGlobalSearch").toBoolean();
        if (IsGlobalSearch && !string.IsNullOrEmpty(Keyword))
            InfoClassID = "0";

        //当传入的InfoClassID参数不为空时或返回数值不等于0时，追加InfoClassID in()条件进行查询
        if (InfoClassID.toInt() != 0)
        {
            //默认显示当前分类及所有子分类
            InfoClassIDStr = " and InfoClassID in (select ICID from InformationClass where Invalid=0 and Del=0 and LevelCode like (select LevelCode+'%' from InformationClass where ICID=" + InfoClassID.toInt() + ")) ";

            //如果设置为False时则仅显示当前分类
            Boolean InfoDisplayMode = MicroPublic.GetMicroInfo("InfoDisplayMode").toBoolean();
            if (!InfoDisplayMode)
                InfoClassIDStr = " and InfoClassID in(select ICID from InformationClass where Invalid=0 and Del=0 and (ICID=" + InfoClassID.toInt() + " or ParentID=" + InfoClassID.toInt() + "))";
        }

        else if (InfoClassID.toInt() == 0 && string.IsNullOrEmpty(Keyword))  //代表是打开首页
        {
            string InfoClassIDs = MicroPublic.GetMicroInfo("InfoClassForHomePage");  //在页面站点设置上进行，代表允许在首页显示的信息分类
            if (!string.IsNullOrEmpty(InfoClassIDs))
                InfoClassIDStr = " and InfoClassID in(" + InfoClassIDs + ")";
        }

        if (!string.IsNullOrEmpty(Keyword))
        {
            KeywordQueryStr = MicroDTHelper.MicroDataTable.GetKeywordQueryStr("Info");
            Keyword = "%" + Keyword.toStringTrim() + "%";
            //Keyword = Keyword.toStringTrim().Replace('*', '%');
        }

        string Invalid = " and a.Invalid=0 ";
        if (MicroUserInfo.CheckUserRole("Administrators"))
            Invalid = "";

        string _sql = "select a.*,b.InfoClassName,b.Description from Information a left join InformationClass b on a.InfoClassID=b.ICID where a.Del=0 " + Invalid + " and PushToInfoPlatform=1 " + InfoClassIDStr + KeywordQueryStr + " order by DateCreated desc";

        SqlParameter[] _sp = { new SqlParameter("@Keyword", SqlDbType.NVarChar), };
        _sp[0].Value = Keyword;

        DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];
        DataTable TargetDT = _dt.Clone();

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
            string MaxDateCreated = string.Empty;
            string _sqlMaxDateCreated = "select max(DateCreated) as DateCreated from Information where Invalid=0 and Del=0 and PushToInfoPlatform=1 " + InfoClassIDStr;  //加入只在首页显示的板块进行查询
            DataTable _dtMaxDateCreated = MsSQLDbHelper.Query(_sqlMaxDateCreated).Tables[0];

            if (_dtMaxDateCreated.Rows.Count > 0)
                MaxDateCreated = _dtMaxDateCreated.Rows[0]["DateCreated"].toStringTrim();

            //*****根据当前用户可查看权限重新构造DataTable2 Start*****
            DataRow[] _rows = _dt.Select("", "DateCreated desc");

            int _rowsLength = _rows.Length;
            if (_rowsLength > 0)
            {
                foreach (DataRow _dr in _rows)
                {
                    //如果拥有查看所有权限时，能够查看所有
                    if (ViewAllPermit)
                        TargetDT.Rows.Add(_dr.ItemArray);
                    else
                    {
                        //否则没有查看所有权限时，判断是否已经审批过
                        if (_dr["InfoState"].toBoolean())
                        {
                            //没有所有权时，如果是公开的则可查看
                            if (_dr["IsPublic"].toBoolean())
                                TargetDT.Rows.Add(_dr.ItemArray);
                            else
                            {
                                int intUserRoles = 0,
                                    intUserJobTitle = 0,
                                    intUserDepts = 0;

                                //否则不公开的，检查是否拥有不公开时的查看权限，如果有则允许
                                if (_dtUserRoles.Rows.Count > 0)
                                {
                                    string AllowRoles = _dr["AllowRoles"].toStringTrim();  //得到文章逗号分隔的AllowRoles 如：1,2,3,4
                                    AllowRoles = string.IsNullOrEmpty(AllowRoles) == true ? "0" : AllowRoles;
                                    intUserRoles = _dtUserRoles.Select("RID in (" + AllowRoles + ")").Length;  //与当前用户的角色表进行对比
                                }

                                if (_dtJobTitle.Rows.Count > 0)
                                {
                                    string AllowJobTitle = _dr["AllowJobTitle"].toStringTrim();  //得到文章逗号分隔的AllowJobTitle 如：1,2,3,4
                                    AllowJobTitle = string.IsNullOrEmpty(AllowJobTitle) == true ? "0" : AllowJobTitle;
                                    intUserJobTitle = _dtJobTitle.Select("JTID in (" + AllowJobTitle + ")").Length;  //与当前用户的职称表进行对比
                                }

                                if (_dtUserDepts.Rows.Count > 0)
                                {
                                    string AllowDepts = _dr["AllowDepts"].toStringTrim();  //得到文章逗号分隔的AllowDepts 如：1,2,3,4
                                    AllowDepts = string.IsNullOrEmpty(AllowDepts) == true ? "0" : AllowDepts;
                                    AllowDepts = MicroUserInfo.GetAllDeptsID("AllSubDepts", "DeptID", AllowDepts);  //得到AllowDepts所有子部门的DetpIDs
                                    intUserDepts = _dtUserDepts.Select("DeptID in (" + AllowDepts + ")").Length;  //与当前用户的部门表进行对比
                                }


                                //检查当前文章是否为当前用户
                                Boolean CheckCurrUsers = false;
                                if (UID != 0 && UID == _dr["UID"].toInt())
                                    CheckCurrUsers = true;

                                //当允许的角色或允许的职位或允许的用户或是自己发布的其中有一个值为真则插入记录
                                if (intUserRoles > 0 || intUserJobTitle > 0 || intUserDepts > 0 || MicroUserInfo.CheckUID(_dr["AllowUsers"].toStringTrim()) || CheckCurrUsers)
                                    TargetDT.Rows.Add(_dr.ItemArray);
                            }
                        }
                        else  //否则还没有经过审批的，则只有拥有审批权限的人才可查看
                        {
                            //没有经过审批的，则只拥有审批权限的人才可查看，否则只能查看自己发布的
                            if (ApprovalPermit)
                                TargetDT.Rows.Add(_dr.ItemArray);
                            else
                            {
                                if (UID == _dr["UID"].toInt() && UID != 0)
                                    TargetDT.Rows.Add(_dr.ItemArray);
                            }
                        }
                    }
                }
            }
            //*****根据当前用户可查看权限重新构造DataTable2 End*****

            ulTop.InnerHtml = GetTop(TargetDT, ApprovalPermit, EditPermit, EditAllPermit, DelPermit, DelAllPermit, MaxDateCreated);
            //原先直接插入到页面，现在采用js方式加载（20210526）
            //ulList.InnerHtml = GetList(TagetDT, ApprovalPermit, EditPermit, DelPermit, EditAllPermit, MaxDateCreated);
            //dlWeekList.InnerHtml = GetWeekHotList(TagetDT);
        }

        return flag;

    }

    /// <summary>
    /// 获取置顶记录
    /// </summary>
    /// <param name="_dt"></param>
    /// <returns></returns>
    private string GetTop(DataTable _dt, Boolean ApprovalPermit, Boolean EditPermit, Boolean EditAllPermit, Boolean DelPermit, Boolean DelAllPermit, string MaxDateCreated)
    {
        string flag = "<div class=\"fly-none\">没有相关数据</div>";
        DataRow[] _rows = _dt.Select("PlatformTop=1", "PlatformTopSort desc");
        int UID = MicroUserInfo.GetUserInfo("UID").toInt();

        int _rowsLength2 = _rows.Length;
        if (_rowsLength2 > 0)
        {
            string str = string.Empty;
            _rowsLength2 = _rowsLength2 > 5 ? 6 : _rowsLength2;  //取前面5条记录

            for (int i = 0; i < _rowsLength2; i++)
            {
                string NewTips = string.Empty,
                    TitleColor = _rows[i]["TitleColor"].toStringTrim(),
                    DateCreated = _rows[i]["DateCreated"].toDateFormat("yyyy-MM-dd HH:mm:ss"),
                    Secrecy = _rows[i]["Secrecy"].toStringTrim();

                TitleColor = string.IsNullOrEmpty(TitleColor) == true ? "" : TitleColor = "color:" + TitleColor + ";";

                if (!string.IsNullOrEmpty(MaxDateCreated) && DateCreated.toDateTime("yyyy-MM-dd") == MaxDateCreated.toDateTime("yyyy-MM-dd") && MicroPublic.GetMicroInfo("InfoNewTips").toBoolean())
                    NewTips = " <span class=\"layui-badge layui-bg-red\" style=\"border:none; \">新</span>";

                Boolean InfoState = _rows[i]["InfoState"].toBoolean();

                str += "<li>";
                str += "<a href = \"javascript:;\" class=\"fly-avatar\"><img src = \"" + _rows[i]["Picture"].toStringTrim() + "\" ></a>";
                str += "<h2 ><a class=\"layui-badge\" href=\"/Views/Info/List/" + _rows[i]["InfoClassID"].ToString() + "\" >" + _rows[i]["InfoClassName"].toStringTrim() + "</a><a href = \"/Views/Info/Detail/" + ModuleID + "/" + _rows[i]["InfoID"].toStringTrim() + "\" target=\"_blank\" style=\"" + TitleColor + "\" > " + _rows[i]["Title"].toStringTrim() + NewTips + "</a></h2>";
                str += "<div class=\"fly-list-info\">";
                str += "<span>" + _rows[i]["Pseudonym"].toStringTrim() + "</span>";
                str += "<span title=\"" + DateCreated.toDateWeek() + "\">" + DateCreated.toTimeAgo() + "</span>";

                //不公开时，加上锁头图标
                if (!_rows[i]["IsPublic"].toBoolean())
                    str += "<span><i class=\"ws-icon icon-erji-quanxianguanli\" lay-tips=\"機密文書\"></i></span>";

                //机密等级不为空时显示机密标签
                if (!string.IsNullOrEmpty(Secrecy))
                    str += "<span class=\"layui-font-red\" lay-tips=\"機密等級：" + Secrecy + "\">" + Secrecy + "</span>";

                str += "<span class=\"fly-list-nums\">";
                str += "<i class=\"ws-icon icon-yueduliang\" title=\"阅读\"></i>" + _rows[i]["Reading"].toStringTrim() + "";
                str += "</span>";
                str += "</div>";
                str += "<div class=\"fly-list-badge\">";

                //可操作
                if (!InfoState && ApprovalPermit)
                    str += "<a href=\"javascript:;\"><span class=\"layui-badge layui-bg-red micro-click\" data-type=\"Approval\" lay-tips=\"审批中的信息只有管理员可见\" micro-stn=\"Info\" micro-id=\"" + _rows[i]["InfoID"].toStringTrim() + "\">审批</span></a>";

                //拥有编辑所有权限时显示编辑按钮
                if (EditAllPermit)
                    str += "<a href=\"/Views/Forms/SysNonPopForm/Modify/Info/" + ModuleID + "/" + _rows[i]["InfoID"].toStringTrim() + "\" target=\"_blank\"><span class=\"layui-badge layui-bg-blue\">编辑</span></a>";
                else
                {
                    //否则没有编辑所有权限时，如果有编辑单个权限并且紧只能编辑自己发布的信息
                    if (EditPermit && UID == _rows[i]["UID"].toInt() && UID != 0)
                        str += "<a href=\"/Views/Forms/SysNonPopForm/Modify/Info/" + ModuleID + "/" + _rows[i]["InfoID"].toStringTrim() + "\" target=\"_blank\"><span class=\"layui-badge layui-bg-blue\">编辑</span></a>";
                }

                if ((DelPermit && UID == _rows[i]["UID"].toInt() && UID != 0) || DelAllPermit)
                    str += "<a href=\"javascript:;\"><span class=\"layui-badge layui-bg-black micro-click\" data-type=\"Del\" micro-stn=\"Info\" micro-id=\"" + _rows[i]["InfoID"].toStringTrim() + "\">删除</span></a>";

                //拥有编辑所有权限时显示置顶按钮
                if (EditAllPermit)
                    str += "<a href=\"javascript:;\"><span class=\"layui-badge micro-click\" data-type=\"CtrlTop\"  style=\"background-color:#ccc; margin-right:20px;\" micro-type=\"Platform\" micro-stn=\"Info\" micro-id=\"" + _rows[i]["InfoID"].toStringTrim() + "\" micro-val=\"NoTop\">取消置顶</span></a>";
                else
                {
                    //否则没有编辑所有权限时，如果有编辑单个权限并且紧只能置顶自己发布的信息
                    if (EditPermit && UID == _rows[i]["UID"].toInt() && UID != 0)
                        str += "<a href=\"javascript:;\"><span class=\"layui-badge micro-click\" data-type=\"CtrlTop\"  style=\"background-color:#ccc; margin-right:20px;\" micro-type=\"Platform\" micro-stn=\"Info\" micro-id=\"" + _rows[i]["InfoID"].toStringTrim() + "\" micro-val=\"NoTop\">取消置顶</span></a>";
                }

                //***信息平台List右则标签***
                if (!_rows[i]["InfoState"].toBoolean())
                    str += "<span class=\"layui-badge layui-bg-red\">待审批</span>";

                str += "<span class=\"layui-badge layui-bg-green\">" + _rows[i]["Nature"].toStringTrim() + "</span>";
                str += "<span class=\"layui-badge layui-bg-red\">置顶</span>";
                str += "</div>";
                str += "</li>";
            }
            flag = str;

        }
        else
            divTop.Visible = false;

        return flag;
    }

}