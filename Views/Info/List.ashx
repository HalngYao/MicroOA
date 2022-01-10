<%@ WebHandler Language="C#" Class="List" %>

using System;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MicroDBHelper;
using MicroUserHelper;
using Newtonsoft.Json.Linq;
using MicroAuthHelper;
using MicroPublicHelper;


public class List : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        string flag = string.Empty,
                Action = context.Server.UrlDecode(context.Request.QueryString["action"]),
                ModuleID = "27",   //以“信息管理平台->信息平台” 菜单的ModuleID作为信息平台的权限设置
                InfoClassID = context.Server.UrlDecode(context.Request.QueryString["infoclassid"]),
                Page = context.Server.UrlDecode(context.Request.QueryString["page"]),
                Limit = MicroPublic.GetMicroInfo("InfoLimit"),
                Keyword = context.Server.UrlDecode(context.Request.QueryString["keyword"]);

        Limit = Limit.toInt() == 0 ? "10" : Limit;
        //InfoClassID = "4";

        Boolean ApprovalPermit = MicroAuth.CheckPermit(ModuleID, "12"),
           EditPermit = MicroAuth.CheckPermit(ModuleID, "3"),  //修改自己
           EditAllPermit = MicroAuth.CheckPermit(ModuleID, "10"), //修改所有
           DelPermit = MicroAuth.CheckPermit(ModuleID, "16"),  //删除自己
           DelAllPermit = MicroAuth.CheckPermit(ModuleID, "18"),  //删除所有
           ViewAllPermit = MicroAuth.CheckPermit(ModuleID, "6");  //查看所有

        flag = GetInfo(ModuleID, ApprovalPermit, EditPermit, EditAllPermit, DelPermit, DelAllPermit, ViewAllPermit, InfoClassID, Page, Limit, Keyword);

        context.Response.Write(JToken.Parse(flag));
    }


    private string GetInfo(string ModuleID, Boolean ApprovalPermit, Boolean EditPermit, Boolean EditAllPermit, Boolean DelPermit, Boolean DelAllPermit, Boolean ViewAllPermit, string InfoClassID = "0", string Page = "1", string Limit = "10", string Keyword = "")
    {
        string flag = "{\"code\": 0,\"msg\": \"\",\"count\": 0,\"pagecount\":0,\"data\":  [{Item:\"" + HttpContext.Current.Server.HtmlEncode("<li>没有相关数据</li>") + "\"}] }";

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
            InfoClassIDStr = " and InfoClassID in(select ICID from InformationClass where Invalid=0 and Del=0 and LevelCode like (select LevelCode+'%' from InformationClass where ICID=" + InfoClassID.toInt() + ")) ";

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
        DataTable TagetDT = _dt.Clone();

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
            string _sqlMaxDateCreated = "select max(DateCreated) as DateCreated from Information where Invalid=0 and Del=0 and PushToInfoPlatform=1 ";  //+ InfoClassIDStr; //加入以当板块为条件的查询
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
                        TagetDT.Rows.Add(_dr.ItemArray);
                    else
                    {
                        //否则没有查看所有权限时，判断是否已经审批过
                        if (_dr["InfoState"].toBoolean())
                        {
                            //没有所有权时，如果是公开的则可查看
                            if (_dr["IsPublic"].toBoolean())
                                TagetDT.Rows.Add(_dr.ItemArray);
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
                                    TagetDT.Rows.Add(_dr.ItemArray);
                            }
                        }
                        else  //否则还没有经过审批的，则只有拥有审批权限的人才可查看
                        {
                            //没有经过审批的，则只拥有审批权限的人才可查看，否则只能查看自己发布的
                            if (ApprovalPermit)
                                TagetDT.Rows.Add(_dr.ItemArray);
                            else
                            {
                                if (UID == _dr["UID"].toInt() && UID != 0)
                                    TagetDT.Rows.Add(_dr.ItemArray);
                            }
                        }
                    }
                }
            }
            //*****根据当前用户可查看权限重新构造DataTable2 End*****

            flag = GetList(ModuleID, TagetDT, ApprovalPermit, EditPermit, EditAllPermit, DelPermit, DelAllPermit, MaxDateCreated, Page, Limit);
        }

        return flag;

    }



    /// <summary>
    /// 获取正文list
    /// </summary>
    /// <param name="_dt"></param>
    /// <returns></returns>
    private string GetList(string ModuleID, DataTable _dt, Boolean ApprovalPermit, Boolean EditPermit, Boolean EditAllPermit, Boolean DelPermit, Boolean DelAllPermit, string MaxDateCreated, string Page = "1", string Limit = "10")
    {
        string flag = "{\"code\": 0,\"msg\": \"\",\"count\": 0,\"pagecount\":0,\"data\":  [{Item:\"" + HttpContext.Current.Server.HtmlEncode("<li>没有相关数据</li>") + "\"}] }";

        int UID = MicroUserInfo.GetUserInfo("UID").toInt();

        //*****正文list要除去已经置顶的记录Start*****
        DataRow[] _rows = _dt.Select("PlatformTop=1", "PlatformTopSort desc");
        string TopInfoID = "0";

        int _rowsLength = _rows.Length;
        if (_rowsLength > 0)
        {
            _rowsLength = _rowsLength > 5 ? 6 : _rowsLength;  //取前面5条记录
            for (int i = 0; i < _rowsLength; i++)
            {
                TopInfoID += _rows[i]["InfoID"].toStringTrim() + ",";
            }
            TopInfoID = TopInfoID.Substring(0, TopInfoID.Length - 1);
        }
        //*****正文list要除去已经置顶的记录End*****

        DataRow[] _rows2 = _dt.Select("InfoID not in (" + TopInfoID + ")", "DateCreated desc");

        if (_rows2.Length > 0)
        {
            int IntPage = Page.toInt(),
                IntLimit = Limit.toInt();
            //DataRow转DataTable
            //TargetDT = SourceDT.AsEnumerable().Skip((IntPage - 1) * IntLimit).Take(IntLimit).CopyToDataTable<DataRow>();
            DataTable TargetDT = _rows2.CopyToDataTable().DefaultView.ToTable().AsEnumerable().Skip((IntPage - 1) * IntLimit).Take(IntLimit).CopyToDataTable<DataRow>();

            string strTemp = "\"Item\":\"{0}\"";

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < TargetDT.Rows.Count; i++)
            {
                string str = string.Empty,
                        NewTips = string.Empty,
                        TitleColor = TargetDT.Rows[i]["TitleColor"].toStringTrim(),
                        DateCreated = TargetDT.Rows[i]["DateCreated"].toDateFormat("yyyy-MM-dd HH:mm:ss"),
                        Secrecy = TargetDT.Rows[i]["Secrecy"].toStringTrim();

                TitleColor = string.IsNullOrEmpty(TitleColor) == true ? "" : TitleColor = "color:" + TitleColor + ";";

                if (!string.IsNullOrEmpty(MaxDateCreated) && DateCreated.toDateTime("yyyy-MM-dd") == MaxDateCreated.toDateTime("yyyy-MM-dd") && MicroPublic.GetMicroInfo("InfoNewTips").toBoolean())
                    NewTips = " <span class=\"layui-badge layui-bg-red\" style=\"border:none; \">新</span>";

                Boolean InfoState = TargetDT.Rows[i]["InfoState"].toBoolean();
                str += "<li>";
                str += "<a href = \"javascript:;\" class=\"fly-avatar\"><img src = \"" + TargetDT.Rows[i]["Picture"].toStringTrim() + "\" ></a>";
                str += "<h2 ><a class=\"layui-badge\" href=\"/Views/Info/List/" + TargetDT.Rows[i]["InfoClassID"].ToString() + "\">" + TargetDT.Rows[i]["InfoClassName"].toStringTrim() + "</a><a href = \"/Views/Info/Detail/" + ModuleID + "/" + TargetDT.Rows[i]["InfoID"].toStringTrim() + "\" target=\"_blank\" style=\"" + TitleColor + "\"  > " + TargetDT.Rows[i]["Title"].toStringTrim() + NewTips + "</a></h2>";
                str += "<div class=\"fly-list-info\">";
                str += "<span>" + TargetDT.Rows[i]["Pseudonym"].toStringTrim() + "</span>";
                str += "<span title=\"" + DateCreated.toDateWeek() + "\">" + DateCreated.toTimeAgo() + "</span>";

                //不公开时，加上锁头图标
                if (!TargetDT.Rows[i]["IsPublic"].toBoolean())
                    str += "<span><i class=\"ws-icon icon-erji-quanxianguanli\" lay-tips=\"機密文書\"></i></span>";

                //机密等级不为空时显示机密标签
                if (!string.IsNullOrEmpty(Secrecy))
                    str += "<span class=\"layui-font-red\" lay-tips=\"機密等級：" + Secrecy + "\">" + Secrecy + "</span>";

                str += "<span class=\"fly-list-nums\">";
                str += "<i class=\"ws-icon icon-yueduliang\" title=\"阅读\"></i>" + TargetDT.Rows[i]["Reading"].toStringTrim() + "";
                str += "</span>";
                str += "</div>";
                str += "<div class=\"fly-list-badge\">";

                if (!InfoState && ApprovalPermit)
                    str += "<a href=\"javascript:;\"><span class=\"layui-badge layui-bg-red micro-click\" data-type=\"Approval\" lay-tips=\"审批中的信息只有管理员可见\" micro-stn=\"Info\" micro-id=\"" + TargetDT.Rows[i]["InfoID"].toStringTrim() + "\">审批</span></a>";

                //拥有编辑所有权限时显示编辑按钮
                if (EditAllPermit)
                    str += "<a href=\"/Views/Forms/SysNonPopForm/Modify/Info/" + ModuleID + "/" + TargetDT.Rows[i]["InfoID"].toStringTrim() + "\" target=\"_blank\" class=\"ws-font-white\"><span class=\"layui-badge layui-bg-blue\">编辑</span></a>";
                else
                {
                    //否则没有编辑所有权限时，如果有编辑单个权限并且紧只能编辑自己发布的信息
                    if (EditPermit && UID == TargetDT.Rows[i]["UID"].toInt() && UID != 0)
                        str += "<a href=\"/Views/Forms/SysNonPopForm/Modify/Info/" + ModuleID + "/" + TargetDT.Rows[i]["InfoID"].toStringTrim() + "\" target=\"_blank\" class=\"ws-font-white\"><span class=\"layui-badge layui-bg-blue\">编辑</span></a>";
                }

                if ((DelPermit && UID == TargetDT.Rows[i]["UID"].toInt() && UID != 0) || DelAllPermit)
                    str += "<a href=\"javascript:;\"><span class=\"layui-badge layui-bg-black micro-click\" data-type=\"Del\" micro-stn=\"Info\" micro-id=\"" + TargetDT.Rows[i]["InfoID"].toStringTrim() + "\">删除</span></a>";

                //拥有编辑所有权限时显示置顶按钮
                if (EditAllPermit)
                    str += "<a href=\"javascript:;\"><span class=\"layui-badge layui-bg-red micro-click\" data-type=\"CtrlTop\" style=\"margin-right:20px;\" micro-type=\"Platform\" micro-stn=\"Info\" micro-id=\"" + TargetDT.Rows[i]["InfoID"].toStringTrim() + "\" micro-val=\"SetTop\">置顶</span></a>";
                else
                {
                    //否则没有编辑所有权限时，如果有编辑单个权限并且紧只能置顶自己发布的信息
                    if (EditPermit && UID == TargetDT.Rows[i]["UID"].toInt() && UID != 0)
                        str += "<a href=\"javascript:;\"><span class=\"layui-badge layui-bg-red micro-click\" data-type=\"CtrlTop\" style=\"margin-right:20px;\" micro-type=\"Platform\" micro-stn=\"Info\" micro-id=\"" + TargetDT.Rows[i]["InfoID"].toStringTrim() + "\" micro-val=\"SetTop\">置顶</span></a>";
                }

                if (!TargetDT.Rows[i]["InfoState"].toBoolean())
                    str += "<span class=\"layui-badge layui-bg-red\">待审批</span>";

                str += "<span class=\"layui-badge layui-bg-green\">" + TargetDT.Rows[i]["Nature"].toStringTrim() + "</span>";
                str += "</div>";
                str += "</li>";

                string Str = string.Format(strTemp, HttpContext.Current.Server.HtmlEncode(str));
                sb.Append("{" + Str + "},");
            }
            string json = sb.ToString();

            //计算页数取整
            int PageCount = _rows2.Length / IntLimit;

            //计算余数，如果>0则+1
            if (_rows2.Length % IntLimit > 0)
                PageCount = PageCount + 1;

            flag = json.Substring(0, json.Length - 1);
            flag = "{\"code\": 0,\"msg\": \"\",\"count\": 0, \"pagecount\": " + PageCount + ",\"data\":  [" + flag + "] }";
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