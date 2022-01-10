using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Microsoft.AspNet.FriendlyUrls;
using MicroDBHelper;
using MicroUserHelper;
using MicroWorkFlowHelper;
using MicroFormHelper;
using MicroPublicHelper;
using MicroAuthHelper;
using MicroPrivateHelper;
//using System.Diagnostics;


namespace MicroPublicHelper
{

    public class MicroInfo
    {
        public int SID;
        public string Title;
        public string InfoPlatformTitle;
        public string Logo;
        public string WebSiteDoamin;
        public string DateFormat;
        public string TimeFormat;
        public Boolean DisplayDomainAccountLogin;
        public Boolean EnabledVerifyCode;
        public Boolean EnabledAutoLogin;
        public Boolean EnabledRegister;
        public Boolean IsCenterMsgDot;
        public string SafetyDay;

        public int MaxFileUpload;
        public string UploadFileType;
        public string HomePageTitle;
        public string MetaKeyword;
        public string MetaDescription;
        public string Foot;
        public Boolean EnabledLDAPAuth;
        public string DomainName;
        public string DomainPrefix;
        public string LDAP;
        public string LDAPAccount;
        public string LDAPPassword;

        public Boolean IsAccessControl;
        public Boolean DenyIPListAccess;
        public string IPList;
        public string DenyAccessSiteTips;
        public string UnsupportedBrowser;
        public string UnsupportedBrowserTips;
        public string SysMailAddress;
        public string MailNickName;
        public string AuthAccount;
        public string AuthPassword;
        public string SMTPAddress;
        public string Port;

        public string CalendarFirstDay;
        public Boolean CalendarLong;

        public string CalendarPlanColor;
        public string CalendarActualColor;
        public string CalendarOvertimeColor;
        public string CalendarBusinessTripColor;
        public string CalendarHolidayColor;
        public string CalendarAbnormalColor;
        public string CalendarPersonalStateColor;

        public Boolean CalendarPlanEvent;
        public Boolean CalendarActualEvent;
        public Boolean CalendarOvertimeEvent;
        public Boolean CalendarBusinessTripEvent;
        public Boolean CalendarHolidayEvent;
        public Boolean CalendarAbnormalEvent;
        public Boolean CalendarPersonalStateEvent;

        public int InfoLimit;
        public Boolean InfoNewTips;
        public Boolean InfoTimeAgo;
        public Boolean InfoIsGlobalSearch;
        public string InfoClassForHomePage;
        public Boolean InfoDisplayMode;

        public Boolean WaterMarkForOA;
        public Boolean WaterMarkForInfo;
        public string WaterMarkFixedValue;
        public Boolean WaterMarkUserName;
        public Boolean WaterMarkDateTime;
        public string WaterMarkXSpace;
        public string WaterMarkYSpace;
        public string WaterMarkColor;

        //public PerformanceCounter CPUCounter;  //获取CPU计数器
        //public PerformanceCounter RAMCounter;  //获取内存计数器

        /// <summary>
        /// 站点信息
        /// </summary>
        public MicroInfo()
        {
            SID = 0;
            Title = string.Empty;
            InfoPlatformTitle = string.Empty;
            Logo = string.Empty;
            WebSiteDoamin = string.Empty;
            DateFormat = string.Empty;
            TimeFormat = string.Empty;
            DisplayDomainAccountLogin = false;
            EnabledVerifyCode = false;
            EnabledAutoLogin = false;
            EnabledRegister = false;
            IsCenterMsgDot = true;
            SafetyDay = string.Empty;

            MaxFileUpload = 0;
            UploadFileType = string.Empty;
            HomePageTitle = string.Empty;
            MetaKeyword = string.Empty;
            MetaDescription = string.Empty;
            Foot = string.Empty;

            EnabledLDAPAuth = false;
            DomainName = string.Empty;
            DomainPrefix = string.Empty;
            LDAP = string.Empty;
            LDAPAccount = string.Empty;
            LDAPPassword = string.Empty;

            IsAccessControl = false;
            DenyIPListAccess = false;
            IPList = string.Empty;
            DenyAccessSiteTips = string.Empty;
            UnsupportedBrowser = string.Empty;
            UnsupportedBrowserTips = string.Empty;
            SysMailAddress = string.Empty;
            MailNickName = string.Empty;
            AuthAccount = string.Empty;
            AuthPassword = string.Empty;
            SMTPAddress = string.Empty;
            Port = string.Empty;

            CalendarFirstDay = "0";
            CalendarLong = true;

            CalendarPlanColor = "#1E9FFF";
            CalendarActualColor = "#009688";
            CalendarOvertimeColor = "#00ced1";
            CalendarBusinessTripColor = "#c71585";
            CalendarHolidayColor = "#999999";
            CalendarAbnormalColor = "#ff5722";
            CalendarPersonalStateColor = "#ffb800";

            CalendarPlanEvent = true;
            CalendarActualEvent = true;
            CalendarOvertimeEvent = true;
            CalendarBusinessTripEvent = true;
            CalendarHolidayEvent = true;
            CalendarAbnormalEvent = true;
            CalendarPersonalStateEvent = true;

            InfoLimit = 10;
            InfoNewTips = true;
            InfoTimeAgo = true;
            InfoIsGlobalSearch = true;
            InfoClassForHomePage = string.Empty;
            InfoDisplayMode = true;

            WaterMarkForOA = false;
            WaterMarkForInfo = false;
            WaterMarkFixedValue = string.Empty;
            WaterMarkUserName = false;
            WaterMarkDateTime = false;
            WaterMarkXSpace = string.Empty;
            WaterMarkYSpace = string.Empty;
            WaterMarkColor = string.Empty;

            
            //CPUCounter= new PerformanceCounter("Processor", "% Processor Time", "_Total", true);
            //RAMCounter= new PerformanceCounter("Memory", "Available MBytes");

            string _sql = "select top 1 * from SystemInfo order by SID desc";
            DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];
            if (_dt.Rows.Count > 0)
            {
                SID = _dt.Rows[0]["SID"].toInt();  //sLogo = HttpContext.Current.Server.HtmlDecode(_dt.Rows[0]["s_Logo"].ToString());
                Title = _dt.Rows[0]["Title"].toStringTrim();
                InfoPlatformTitle = _dt.Rows[0]["InfoPlatformTitle"].toStringTrim();
                Logo = _dt.Rows[0]["Logo"].toStringTrim();
                WebSiteDoamin = _dt.Rows[0]["WebSiteDoamin"].toStringTrim();
                DateFormat = _dt.Rows[0]["DateFormat"].toStringTrim();
                TimeFormat = _dt.Rows[0]["TimeFormat"].toStringTrim();

                DisplayDomainAccountLogin = _dt.Rows[0]["DisplayDomainAccountLogin"].toBoolean();
                EnabledVerifyCode = _dt.Rows[0]["EnabledVerifyCode"].toBoolean();
                EnabledAutoLogin = _dt.Rows[0]["EnabledAutoLogin"].toBoolean();
                EnabledRegister = _dt.Rows[0]["EnabledRegister"].toBoolean();
                IsCenterMsgDot = _dt.Rows[0]["IsCenterMsgDot"].toBoolean();
                SafetyDay = _dt.Rows[0]["SafetyDay"].toDateFormat("yyyy-MM-dd");

                MaxFileUpload = _dt.Rows[0]["MaxFileUpload"].toInt();
                UploadFileType = _dt.Rows[0]["UploadFileType"].toStringTrim();
                HomePageTitle = _dt.Rows[0]["HomePageTitle"].toStringTrim();
                MetaKeyword = _dt.Rows[0]["MetaKeyword"].toStringTrim();
                MetaDescription = _dt.Rows[0]["MetaDescription"].toStringTrim();
                Foot = _dt.Rows[0]["Foot"].toStringTrim();

                EnabledLDAPAuth = _dt.Rows[0]["EnabledLDAPAuth"].toBoolean();
                DomainName = _dt.Rows[0]["DomainName"].toStringTrim();
                DomainPrefix = _dt.Rows[0]["DomainPrefix"].toStringTrim();
                LDAP = _dt.Rows[0]["LDAP"].toStringTrim();
                LDAPAccount = _dt.Rows[0]["LDAPAccount"].toStringTrim();
                LDAPPassword = _dt.Rows[0]["LDAPPassword"].toStringTrim();

                IsAccessControl = _dt.Rows[0]["IsAccessControl"].toBoolean();
                DenyIPListAccess = _dt.Rows[0]["DenyIPListAccess"].toBoolean();
                IPList = _dt.Rows[0]["IPList"].toStringTrim();
                DenyAccessSiteTips = _dt.Rows[0]["DenyAccessSiteTips"].toStringTrim();
                UnsupportedBrowser = _dt.Rows[0]["UnsupportedBrowser"].toStringTrim();
                UnsupportedBrowserTips = _dt.Rows[0]["UnsupportedBrowserTips"].toStringTrim();
                SysMailAddress = _dt.Rows[0]["SysMailAddress"].toStringTrim();
                MailNickName = _dt.Rows[0]["MailNickName"].toStringTrim();
                AuthAccount = _dt.Rows[0]["AuthAccount"].toStringTrim();
                AuthPassword = _dt.Rows[0]["AuthPassword"].toStringTrim();
                SMTPAddress = _dt.Rows[0]["SMTPAddress"].toStringTrim();
                Port = _dt.Rows[0]["Port"].toStringTrim();

                CalendarFirstDay = _dt.Rows[0]["CalendarFirstDay"].toStringTrim();
                CalendarFirstDay = string.IsNullOrEmpty(CalendarFirstDay) ? "0" : CalendarFirstDay;

                CalendarLong = _dt.Rows[0]["CalendarLong"].toBoolean();


                CalendarPlanColor = _dt.Rows[0]["CalendarPlanColor"].toStringTrim();
                CalendarPlanColor = string.IsNullOrEmpty(CalendarPlanColor) ? "#1e9fff" : CalendarPlanColor;

                CalendarActualColor = _dt.Rows[0]["CalendarActualColor"].toStringTrim();
                CalendarActualColor = string.IsNullOrEmpty(CalendarActualColor) ? "#009688" : CalendarActualColor;

                CalendarOvertimeColor = _dt.Rows[0]["CalendarOvertimeColor"].toStringTrim();
                CalendarOvertimeColor = string.IsNullOrEmpty(CalendarOvertimeColor) ? "#00ced1" : CalendarOvertimeColor;

                CalendarBusinessTripColor = _dt.Rows[0]["CalendarBusinessTripColor"].toStringTrim();
                CalendarBusinessTripColor = string.IsNullOrEmpty(CalendarBusinessTripColor) ? "#c71585" : CalendarBusinessTripColor;

                CalendarHolidayColor = _dt.Rows[0]["CalendarHolidayColor"].toStringTrim();
                CalendarHolidayColor = string.IsNullOrEmpty(CalendarHolidayColor) ? "#999999" : CalendarHolidayColor;

                CalendarAbnormalColor = _dt.Rows[0]["CalendarAbnormalColor"].toStringTrim();
                CalendarAbnormalColor = string.IsNullOrEmpty(CalendarAbnormalColor) ? "#ff5722" : CalendarAbnormalColor;

                CalendarPersonalStateColor = _dt.Rows[0]["CalendarPersonalStateColor"].toStringTrim();
                CalendarPersonalStateColor = string.IsNullOrEmpty(CalendarPersonalStateColor) ? "#ff5722" : CalendarPersonalStateColor;

                CalendarPlanEvent = _dt.Rows[0]["CalendarPlanEvent"].toBoolean();
                CalendarActualEvent = _dt.Rows[0]["CalendarActualEvent"].toBoolean();
                CalendarOvertimeEvent = _dt.Rows[0]["CalendarOvertimeEvent"].toBoolean();
                CalendarBusinessTripEvent = _dt.Rows[0]["CalendarBusinessTripEvent"].toBoolean();
                CalendarHolidayEvent = _dt.Rows[0]["CalendarHolidayEvent"].toBoolean();
                CalendarAbnormalEvent = _dt.Rows[0]["CalendarAbnormalEvent"].toBoolean();
                CalendarPersonalStateEvent = _dt.Rows[0]["CalendarPersonalStateEvent"].toBoolean();

                InfoLimit = _dt.Rows[0]["InfoLimit"].toInt();
                InfoNewTips = _dt.Rows[0]["InfoNewTips"].toBoolean();
                InfoTimeAgo = _dt.Rows[0]["InfoTimeAgo"].toBoolean();
                InfoIsGlobalSearch = _dt.Rows[0]["InfoIsGlobalSearch"].toBoolean();
                InfoClassForHomePage = _dt.Rows[0]["InfoClassForHomePage"].toStringTrim();
                InfoDisplayMode= _dt.Rows[0]["InfoDisplayMode"].toBoolean();

                WaterMarkForOA = _dt.Rows[0]["WaterMarkForOA"].toBoolean();
                WaterMarkForInfo = _dt.Rows[0]["WaterMarkForInfo"].toBoolean();
                WaterMarkFixedValue = _dt.Rows[0]["WaterMarkFixedValue"].toStringTrim();
                WaterMarkUserName = _dt.Rows[0]["WaterMarkUserName"].toBoolean();
                WaterMarkDateTime = _dt.Rows[0]["WaterMarkDateTime"].toBoolean();
                WaterMarkXSpace = _dt.Rows[0]["WaterMarkXSpace"].toInt().toStringTrim();
                WaterMarkYSpace = _dt.Rows[0]["WaterMarkYSpace"].toInt().toStringTrim();
                WaterMarkColor = _dt.Rows[0]["WaterMarkColor"].toStringTrim();

            }
        }
    }

    /// <summary>
    /// 站点公共方法
    /// </summary>
    public class MicroPublic
    {


        /// <summary>
        /// 返回站点信息 Title、InfoPlatformTitle、Logo、WebSiteDoamin、DateFormat、TimeFormat、DisplayDomainAccountLogin、EnabledVerifyCode、EnabledAutoLogin、EnabledRegister、IsCenterMsgDot、MaxFileUpload、UploadFileType、HomePageTitle、MetaKeyword、MetaDescription、Foot、IsAccessControl、DenyIPListAccess、DenyAccessSiteTips、UnsupportedBrowser、UnsupportedBrowserTips、IPList、SysMailAddress、MailNickName、AuthAccount、AuthPassword、SMTPAddress、Port、CalendarFirstDay、CalendarLong、CalendarPlanColour、CalendarActualColour、CalendarOvertimeColour、CalendarBusinessTripColour、CalendarHolidayColour、CalendarAbnormalColour
        /// </summary>
        /// <param name="Type">返回站点信息 Title、InfoPlatformTitle、Logo、WebSiteDoamin、DateFormat、TimeFormat、DisplayDomainAccountLogin、EnabledVerifyCode、EnabledAutoLogin、EnabledRegister、IsCenterMsgDot、MaxFileUpload、UploadFileType、HomePageTitle、MetaKeyword、MetaDescription、Foot、IsAccessControl、DenyIPListAccess、DenyAccessSiteTips、UnsupportedBrowser、UnsupportedBrowserTips、IPList、SysMailAddress、MailNickName、AuthAccount、AuthPassword、SMTPAddress、Port、CalendarFirstDay、CalendarLong、CalendarPlanColour、CalendarActualColour、CalendarOvertimeColour、CalendarBusinessTripColour、CalendarHolidayColour、CalendarAbnormalColour</param>
        /// <returns></returns>
        public static string GetMicroInfo(string Type)
        {
            string flag = string.Empty;
            switch (Type)
            {
                case "Title":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).Title;
                    break;
                case "InfoPlatformTitle":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).InfoPlatformTitle;
                    break;
                case "Logo":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).Logo;
                    break;
                case "WebSiteDoamin":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).WebSiteDoamin;
                    break;
                case "DateFormat":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).DateFormat;
                    break;
                case "TimeFormat":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).TimeFormat;
                    break;
                case "DisplayDomainAccountLogin":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).DisplayDomainAccountLogin.ToString();
                    break;
                case "EnabledVerifyCode":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).EnabledVerifyCode.ToString();
                    break;
                case "EnabledAutoLogin":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).EnabledAutoLogin.ToString();
                    break;
                case "EnabledRegister":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).EnabledRegister.ToString();
                    break;
                case "IsCenterMsgDot":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).IsCenterMsgDot.ToString();
                    break;
                case "SafetyDay":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).SafetyDay;
                    break;

                case "MaxFileUpload":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).MaxFileUpload.ToString();
                    break;
                case "UploadFileType":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).UploadFileType;
                    break;
                case "HomePageTitle":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).HomePageTitle;
                    break;
                case "MetaKeyword":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).MetaKeyword;
                    break;
                case "MetaDescription":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).MetaDescription;
                    break;
                case "Foot":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).Foot;
                    break;

                case "EnabledLDAPAuth":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).EnabledLDAPAuth.ToString();
                    break;
                case "DomainName":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).DomainName;
                    break;
                case "DomainPrefix":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).DomainPrefix;
                    break;
                case "LDAP":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).LDAP;
                    break;
                case "LDAPAccount":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).LDAPAccount;
                    break;
                case "LDAPPassword":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).LDAPPassword;
                    break;


                case "IsAccessControl":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).IsAccessControl.ToString();
                    break;
                case "DenyIPListAccess":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).DenyIPListAccess.ToString();
                    break;
                case "IPList":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).IPList;
                    break;
                case "DenyAccessSiteTips":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).DenyAccessSiteTips;
                    break;
                case "UnsupportedBrowser":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).UnsupportedBrowser;
                    break;
                case "UnsupportedBrowserTips":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).UnsupportedBrowserTips;
                    break;
                case "SysMailAddress":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).SysMailAddress;
                    break;
                case "MailNickName":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).MailNickName;
                    break;
                case "AuthAccount":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).AuthAccount;
                    break;
                case "AuthPassword":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).AuthPassword;
                    break;
                case "SMTPAddress":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).SMTPAddress;
                    break;
                case "Port":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).Port;
                    break;

                case "CalendarFirstDay":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).CalendarFirstDay;
                    break;
                case "CalendarLong":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).CalendarLong.ToString();
                    break;

                case "CalendarPlanColor":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).CalendarPlanColor;
                    break;
                case "CalendarActualColor":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).CalendarActualColor;
                    break;
                case "CalendarOvertimeColor":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).CalendarOvertimeColor;
                    break;
                case "CalendarBusinessTripColor":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).CalendarBusinessTripColor;
                    break;
                case "CalendarHolidayColor":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).CalendarHolidayColor;
                    break;
                case "CalendarAbnormalColor":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).CalendarAbnormalColor;
                    break;
                case "CalendarPersonalStateColor":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).CalendarPersonalStateColor;
                    break;

                case "CalendarPlanEvent":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).CalendarPlanEvent.ToString();
                    break;
                case "CalendarActualEvent":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).CalendarActualEvent.ToString();
                    break;
                case "CalendarOvertimeEvent":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).CalendarOvertimeEvent.ToString();
                    break;
                case "CalendarBusinessTripEvent":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).CalendarBusinessTripEvent.ToString();
                    break;
                case "CalendarHolidayEvent":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).CalendarHolidayEvent.ToString();
                    break;
                case "CalendarAbnormalEvent":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).CalendarAbnormalEvent.ToString();
                    break;
                case "CalendarPersonalStateEvent":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).CalendarPersonalStateEvent.ToString();
                    break;

                case "InfoLimit":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).InfoLimit.ToString();
                    break;
                case "InfoNewTips":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).InfoNewTips.ToString();
                    break;
                case "InfoTimeAgo":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).InfoTimeAgo.ToString();
                    break;
                case "InfoIsGlobalSearch":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).InfoIsGlobalSearch.ToString();
                    break;
                case "InfoClassForHomePage":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).InfoClassForHomePage;
                    break;
                case "InfoDisplayMode":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).InfoDisplayMode.ToString();
                    break;

                case "WaterMarkForOA":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).WaterMarkForOA.ToString();
                    break;
                case "WaterMarkForInfo":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).WaterMarkForInfo.ToString();
                    break;
                case "WaterMarkFixedValue":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).WaterMarkFixedValue.toStringTrim();
                    break;
                case "WaterMarkUserName":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).WaterMarkUserName.ToString();
                    break;
                case "WaterMarkDateTime":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).WaterMarkDateTime.ToString();
                    break;
                case "WaterMarkXSpace":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).WaterMarkXSpace.toStringTrim();
                    break;
                case "WaterMarkYSpace":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).WaterMarkYSpace.toStringTrim();
                    break;
                case "WaterMarkColor":
                    flag = ((MicroInfo)HttpContext.Current.Session["MicroInfo"]).WaterMarkColor.toStringTrim();
                    break;


            }

            return flag;
        }

        /// <summary>
        /// 传入ShortTableName返回长TableName[基于MicroTables]
        /// </summary>
        /// <param name="ShortTableName"></param>
        /// <returns></returns>
        public static string GetTableName(string ShortTableName)
        {
            string flag = string.Empty;
            if (ShortTableName == "mTabs")
                flag = "MicroTables";  //等于mTabs时直接返回MicroTables
            else
            {
                string _sql = "select TabColName from MicroTables where ShortTableName=@ShortTableName";
                SqlParameter[] _sp = { new SqlParameter("@ShortTableName", SqlDbType.VarChar, 100) };
                _sp[0].Value = ShortTableName;
                flag = MicroPublic.GetSingleField(0, _sql, _sp);
            }
            return flag;
        }

        /// <summary>
        /// 获取ModuleID，根据传入的ShortTableName查询Module表返回ModuleID
        /// </summary>
        /// <param name="ShortTableName"></param>
        /// <returns></returns>
        public static string GetModuleID(string ShortTableName)
        {
            string flag = "0";

            if (!string.IsNullOrEmpty(ShortTableName))
            {
                string _sql = "select MID from Modules where ShortTableName=@ShortTableName";
                SqlParameter[] _sp = { new SqlParameter("@ShortTableName", SqlDbType.VarChar, 100) };
                _sp[0].Value = ShortTableName.toStringTrim();

                flag = GetSingleField(0, _sql, _sp);
                flag = string.IsNullOrEmpty(flag) ? "0" : flag;
            }

            return flag;
        }

        /// <summary>
        /// 获取友好的URL参数
        /// </summary>
        /// <param name="Num"></param>
        /// <returns></returns>
        public static string GetFriendlyUrlParm(int Num)
        {
            string flag = string.Empty;
            try
            {
                flag = HttpContext.Current.Request.GetFriendlyUrlSegments()[Num];
            }
            catch
            {
                flag = "";
            }

            return flag;
        }

        /// <summary>
        /// 传入SQL语句,返回第N(从0开始)个字段结果,字符串
        /// </summary>
        /// <param name="SQLString">SQLString</param>
        /// <param name="Num">返回第几个,从0开始</param>
        /// <returns></returns>
        public static string GetSingleField(string SQLString, int Num)
        {
            DataTable _dt = MsSQLDbHelper.Query(SQLString).Tables[0];
            if (_dt != null && _dt.Rows.Count > 0)
                return _dt.Rows[0][Num].ToString();
            else
                return "";
        }

        /// <summary>
        /// 传入SQL语句,返回第N(从0开始)个字段结果,字符串 重载方法
        /// </summary>
        /// <param name="Num">返回第几个,从0开始</param>
        /// <param name="SQLString">SQLString</param>
        /// <param name="cmdParms">SqlParameter</param>
        /// <returns></returns>
        public static string GetSingleField(int Num, string SQLString, params SqlParameter[] cmdParms)
        {
            DataTable _dt = MsSQLDbHelper.Query(SQLString, cmdParms).Tables[0];
            if (_dt != null && _dt.Rows.Count > 0)
                return _dt.Rows[0][Num].ToString();
            else
                return "";
        }

        /// <summary>
        /// 返回主要菜单
        /// </summary>
        /// <returns></returns>
        public static string GetMainMenu()
        {
            string _li = string.Empty;
            Boolean ViewAllPermit = false;

            //导航类型NavType，0=左侧垂直导航，1=顶部横向导航，2=两者，默认值为null
            string _sql = "select * from Modules where Invalid=0 and Del=0 and (NavType=0 or NavType=2 or NavType is null) order by ParentID,Sort";
            DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

            if (_dt != null && _dt.Rows.Count > 0)
            {
                
                DataRow[] _rows2 = _dt.Select("ShortTableName='Mod'", "ParentID,Sort");
                if (_rows2.Length > 0)
                {
                    string _ModuleID = _rows2[0]["MID"].ToString();
                    ViewAllPermit = MicroAuth.CheckPermit(_ModuleID, "6");
                }

                string Show = string.Empty; //只显示需要显示的
                if (!ViewAllPermit)
                    Show = " and Show=1";  //只显示需要显示的

                DataRow[] _rows = _dt.Select("ParentID=0 " + Show, "ParentID,Sort");
                foreach (DataRow _dr in _rows)
                {
                    string Itemed = _dr["Expand"].ToString() == "True" ? "layui-nav-itemed" : "";
                    string URL = _dr["URL"].ToString();
                    string ModuleID = _dr["MID"].ToString();
                    string Parameter = "/" + ModuleID;
                    //Parameter = LayHref.Contains("?") == true ? "&mid=" + ModuleID : "?mid=" + ModuleID;

                    if (MicroAuth.CheckBrowses(ModuleID))
                    {
                        string Href = string.IsNullOrEmpty(URL) == true ? "href=\"javascript:;\"" : "lay-href=\"" + URL + Parameter + "\"";
                        string icon = string.IsNullOrEmpty(_dr["Icon"].ToString()) == true ? "" : "<i class=\"" + _dr["Icon"].ToString() + "\"></i>";

                        _li += "<li class=\"layui-nav-item " + Itemed + "\" >" +
                                    "<a " + Href + " lay-direction=\"2\" >"
                                        + icon +
                                        "<cite lay-tips=\"" + _dr["ModuleName"].ToString() + "\">" + _dr["ModuleName"].toMenuSplit() + "</cite>" +
                                    "</a>";
                        _li += GetSubMenu(ModuleID, _dt, ViewAllPermit);
                        _li += "</li>";
                    }
                }
            }
            return _li;
        }

        /// <summary>
        /// 递归方法显示子模块
        /// </summary>
        /// <param name="_pid">m_ParentID=m_ID</param>
        /// <param name="_dt">DataTable</param>
        /// <returns></returns>
        public static string GetSubMenu(string MID, DataTable _dt, Boolean ViewAllPermit)
        {
            string Show = string.Empty; //只显示需要显示的

            if (!ViewAllPermit)
                Show = " and Show=1";  //只显示需要显示的

            string _li = string.Empty;
            DataRow[] _rows = _dt.Select("ParentID=" + MID + Show, "ParentID,Sort");

            if (_rows.Length > 0)
            {
                _li += "<dl class=\"layui-nav-child\">";
                foreach (DataRow _dr in _rows)
                {
                    string Itemed = _dr["Expand"].ToString() == "True" ? "layui-nav-itemed" : "";
                    string URL = _dr["URL"].ToString();
                    string ModuleID = _dr["MID"].ToString();
                    string Parameter = "/" + ModuleID;
                    //Parameter = LayHref.Contains("?") == true ? "&mid=" + ModuleID : "?mid=" + ModuleID;

                    string Href = string.IsNullOrEmpty(URL) == true ? "href=\"javascript:;\"" : "lay-href=\"" + URL + Parameter + "\"";
                    string icon = string.IsNullOrEmpty(_dr["Icon"].ToString()) == true ? " " : "<i class=\"" + _dr["Icon"].ToString() + "\"></i>";

                    if (MicroAuth.CheckBrowses(ModuleID))
                    {
                        _li += "<dd class=\" " + Itemed + "\" >" +
                                    "<a " + Href + " >"
                                         + icon + _dr["ModuleName"].toMenuSplit() +
                                    "</a>";
                        _li += GetSubMenu(_dr["MID"].ToString(), _dt, ViewAllPermit); //递归
                        _li += "</dd>";
                    }
                }
                _li += "</dl>";
            }
            return _li;
        }



        /// <summary>
        /// 显示顶部导航菜单（顶部菜单是单一的不再含有父子结构，且直接跳转）
        /// </summary>
        /// <param name="MenuType">OA（平台） or Info(信息共享平台)，根据传入的参数在不同平台上调整图标的显示大小</param>
        /// <returns></returns>
        public static string GetTopMenu(string MenuType)
        {
            string _li = string.Empty;
            Boolean ViewAllPermit = false;

            //导航类型NavType，0=左侧垂直导航，1=顶部横向导航，2=两者，默认值为null
            string _sql = "select * from Modules where Invalid=0 and Del=0 and (NavType=1 or NavType=2 or ShortTableName='Mod') order by ParentID,Sort";
            DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

            if (_dt != null && _dt.Rows.Count > 0)
            {
                
                DataRow[] _rows2 = _dt.Select("ShortTableName='Mod'", "ParentID,Sort");
                if (_rows2.Length > 0)
                {
                    string _ModuleID = _rows2[0]["MID"].ToString();
                    ViewAllPermit = MicroAuth.CheckPermit(_ModuleID, "6");
                }

                string Show = string.Empty; //只显示需要显示的
                if (!ViewAllPermit)
                    Show = " and Show=1";  //只显示需要显示的

                DataRow[] _rows = _dt.Select("(ShortTableName<>'Mod' or ShortTableName is null)" + Show, "ParentID,Sort");
                foreach (DataRow _dr in _rows)
                {
                    string ModuleName = _dr["ModuleName"].ToString();
                    string URL = _dr["URL"].ToString();
                    string ModuleID = _dr["MID"].ToString();
                    string layTips = "lay-tips=\"" + ModuleName + "\"";
                    string OATips = string.Empty,
                        InfoTips = string.Empty,
                        iCss = string.Empty; //i元素的css属性

                    if (MenuType == "OA")
                    {
                        OATips = layTips;
                        iCss = " ws-font-size20";
                    }
                    else if (MenuType == "Info")
                        InfoTips = layTips;
                    

                    if (MicroAuth.CheckBrowses(ModuleID))
                    {
                        string Href = string.IsNullOrEmpty(URL) == true ? "href=\"javascript:;\"" : "href=\"" + URL + "\"";
                        string icon = string.IsNullOrEmpty(_dr["Icon"].ToString()) == true ? "" : "<i class=\"layui-icon " + _dr["Icon"].ToString() + iCss + "\" " + InfoTips + " ></i>";

                        _li += "<li class=\"layui-nav-item layui-hide-xs\" " + OATips + " lay-unselect>";
                        _li += "<a " + Href + " target=\"_blank\">";
                        _li += icon; //"<i class=\"ws-icon icon-art2 ws-margin-top2 ws-font-size20\"></i>";
                        _li += "</a>";
                        _li += "</li>";
                    }
                }
                
            }
            return _li;
        }

        /// <summary>
        /// 自定义横向菜单（如信息平台导航菜单）
        /// </summary>
        /// <param name="ShortTableName">需要生成菜单的短表名</param>
        /// <param name="NameField">显示字段名</param>
        /// <param name="PrimaryKeyName">主键名称</param>
        /// <param name="IsUL">是否生成父ul标签</param>
        /// <param name="URL">需要跳转到的URL地址</param>
        /// <returns></returns>
        public static string GetMicroMainMenu(string ShortTableName, string NameField, string PrimaryKeyName, Boolean IsUL = true, string URL = "")
        {
            string _startUL = "<ul class=\"ws-menu\">",
                   _endUL = "</ul>",
                _li = string.Empty;

            string TableName = MicroPublic.GetTableName(ShortTableName);

            string _sql = "select * from " + TableName + " where Invalid=0 and Del=0 order by ParentID,Sort";
            DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

            if (_dt != null && _dt.Rows.Count > 0)
            {
                if (IsUL)
                    _li += _startUL;

                DataRow[] _rows = _dt.Select("ParentID=0 ", "ParentID,Sort");
                foreach (DataRow _dr in _rows)
                {
                    string MenuName = _dr[NameField].toStringTrim(),
                           PKV = _dr[PrimaryKeyName].toStringTrim();

                    _li += "<li>";

                    if (!string.IsNullOrEmpty(URL))
                        _li += "<a href=\"" + URL + PKV + "\">";
                    else
                        _li += "<a href=\"javascript:;\">";

                    _li += MenuName;

                    _li += "</a>";
                    _li += GetMicroSubMenu(_dt, PKV, NameField, PrimaryKeyName, URL);
                    _li += "</li>";
                }

                if (IsUL)
                    _li += _endUL;
            }

            return _li;
        }

        public static string GetMicroSubMenu(DataTable _dt, string PrimaryKeyValue, string NameField, string PrimaryKeyName, string URL = "")
        {

            string _li = string.Empty;
            DataRow[] _rows = _dt.Select("ParentID=" + PrimaryKeyValue, "ParentID,Sort");

            if (_rows.Length > 0)
            {
                _li += "<ul>";
                foreach (DataRow _dr in _rows)
                {
                    string MenuName = _dr[NameField].toStringTrim(),
                           PKV = _dr[PrimaryKeyName].toStringTrim();

                    _li += "<li>";

                    if (!string.IsNullOrEmpty(URL))
                        _li += "<a href=\"" + URL + PKV + "\">";
                    else
                        _li += "<a href=\"javascript:;\">";

                    _li += MenuName;

                    DataRow[] _rows2 = _dt.Select("ParentID=" + PKV, "ParentID,Sort");
                    if (_rows2.Length > 0)
                        _li += "<i class=\"layui-icon layui-icon-right ws-font-size8 ws-menu-more\"></i>";

                    _li += "</a>";

                    _li += GetMicroSubMenu(_dt, PKV, NameField, PrimaryKeyName, URL);

                    _li += "</li>";

                }
                _li += "</ul>";
            }
            return _li;
        }

        /// <summary>
        /// 通用提示消息
        /// </summary>
        /// <param name="MsgType">Case: Save=保存成功、Modify=修改成功、Delete=删除成功、SaveFailed=保存失败、SaveFailedTry=保存失败尝试写数据失败、ModifyFailed=修改失败、SaveUrlParaError=保存失败非法的URL参数、DenyURLError=拒绝访问非法的URL参数、DenyAccess=拒绝访问你没有权限、SubmitFailed=提交失败请检查URL参数、Add=添加成功、AddFailed=添加失败、Set=设置成功、SetFailed=设置失败、Recovery=恢复成功、RecoveryFailed=恢复失败、Upload=上传成功、UploadFailed=上传失败、Update=更新成功、UpdateFailed=更新失败</param>
        /// <returns></returns>
        public static string GetMsg(string MsgType)
        {
            string flag = string.Empty;

            switch (MsgType)
            {
                case "NoPermBrowse":
                    flag = "对不起！您没有权限浏览该页面<br/>Sorry, you don't have permission to browse the page.";
                    break;
                case "Save":
                    flag = "True保存成功<br/>Successfully saved";
                    break;
                case "Modify":
                    flag = "True修改成功<br/>Successfully modified";
                    break;
                case "Del":
                    flag = "True删除成功<br/>successfully deleted";
                    break;
                case "Ctrl":
                    flag = "True操作成功<br/>successfully operation";
                    break;
                case "SaveFailed":
                    flag = "保存失败<br/>Save failed";
                    break;
                case "SaveFailedTry":
                    flag = "保存失败，尝试写数据失败<br/>Save failed, attempt to write data failed";
                    break;
                case "ModifyFailed":
                    flag = "修改失败<br/>Modification failed";
                    break;
                case "ModifyFailedTry":
                    flag = "修改失败，尝试写数据失败<br/>Failed to modify and attempt to save data";
                    break;
                case "SaveURLError":
                    flag = "保存失败，非法的URL参数<br/>Save failed, illegal URL parameter";
                    break;
                case "DelFailed":
                    flag = "删除失败<br/>Deleted failed";
                    break;
                case "DelFailedNoPerm":
                    flag = "对不起，你没有权限删除<br/>Sorry, you do not have permission to delete";
                    break;
                case "DelFailedNoDelState":
                    flag = "删除失败，不在允许删除状态<br/>Failed to delete, no longer allowed to delete state";
                    break;
                case "CtrlFailed":
                    flag = "操作失败<br/>Operation failed";
                    break;
                case "DenyURLError":
                    flag = "拒绝访问，非法的URL参数<br/>Denied access, illegal URL parameter";
                    break;
                case "DenyAccess":
                    flag = "拒绝访问，您没有权限浏览此页面<br/>Denied access, you do not have permission to browse this page";
                    break;
                case "Submit":
                    flag = "True提交成功<br/>Successfully submitted";
                    break;
                case "SubmitFailed":
                    flag = "提交失败，请检查URL参数<br/>The submission failed. Check the URL parameters";
                    break;
                case "SubmitURLError":
                    flag = "提交失败，非法的URL参数<br/>Failed to submit, illegal URL parameterr";
                    break;
                case "Add":
                    flag = "True添加成功<br/>Successfully Add";
                    break;
                case "AddFailed":
                    flag = "添加失败<br/>Add failure";
                    break;
                case "Set":
                    flag = "True设置成功<br/>Successfully set";
                    break;
                case "SetFailed":
                    flag = "设置失败<br/>Set failure";
                    break;
                case "Recovery":
                    flag = "True恢复成功<br/>Successfully recovery";
                    break;
                case "RecoveryFailed":
                    flag = "恢复失败<br/>Recovery failure";
                    break;
                case "Upload":
                    flag = "True上传成功<br/>Successfully upload";
                    break;
                case "UploadFailed":
                    flag = "上传失败<br/>Upload failure";
                    break;
                case "Update":
                    flag = "True更新成功<br/>Successfully update";
                    break;
                case "UpdateFailed":
                    flag = "更新失败<br/>Update failure";
                    break;
                case "SaveWolrkFlow":
                    flag = "True提交审批流程成功<br/>Approval process submitted successfully";
                    break;
                case "SaveWolrkFlowFailed":
                    flag = "审批流程提交失败<br/>Failed to submit approval process";
                    break;
                case "NoApprovalRequired":
                    flag = "该表单不需要审批<br/>The form does not require approval";
                    break;
                case "TipAllNoApprovalRequired":
                    flag = "系统提示：没有任何需要审批的表单<br/>Tip: There are no forms that require approval";
                    break;
                case "TipNoApprovalRequired":
                    flag = "系统提示：该表单不需要审批<br/>Tip: The form does not require approval.";
                    break;
                case "NoPermApproval":
                    flag = "对不起！您没有权限审批该表单<br/>Sorry! You do not have permission to approve the form";
                    break;
                case "TipNoPermApproval":
                    flag = "系统提示：检测到您没有权限审批该表单<br/>Tip: You are detected that you do not have permission to approve the form.";
                    break;
                case "ApprovedFailed":
                    flag = "该表单已审批过或不需要审批<br/>The form has been approved or does not require approval";
                    break;
                case "TipApproved":
                    flag = "系统提示：该表单已审批过或不需要审批。<br/>Tip: The form has been approved or does not require approval.";
                    break;
                case "TipApprovalSubmitted":
                    flag = "系统提示：该表单已提交过请勿重复提交。<br/>Tip: The form has been submitted and must not be submitted repeatedly.";
                    break;
                case "TipApprovalReturn":
                    flag = "系统提示：该表单已被驳回，不能进行审批操作。<br/>Tip: The form has been rejected and cannot be approved.";
                    break;
                case "TipCannotApproval":
                    flag = "系统提示：该表单不在审批阶段，不能进行审批操作。<br/>Tip: This form has been rejected Do not duplicate approval.";
                    break;
                case "Accept":
                    flag = "True受理成功<br/>Successfully acceptance";
                    break;
                case "AcceptFailed":
                    flag = "受理失败<br/>Acceptance failure";
                    break;
                case "NotInClose":
                    flag = "该表单不在结案阶段，不能进行结案操作<br/>The form is not in the closing stage and cannot be closed.";
                    break;
                case "SetFormStateFailed":
                    flag = "更新表单状态失败<br/>The update form status failed.";
                    break;
                case "SetFormApprovalRecordsFailed":
                    flag = "更新表单审批记录失败<br/>The update form approval record failed.";
                    break;
                case "SetApprovalLogsFailed":
                    flag = "更新日志记录失败<br/>The update logging failed.";
                    break;
                case "Closed":
                    flag = "True保存成功，该申请对应完成已关闭<br/>Saved successfully, the request is closed for completion.";
                    break;
                case "ReadWorkFlow":
                    flag = "True流程读取成功<br/>Workflow read successfully.";
                    break;
                case "ReadWorkFlowFailed":
                    flag = "流程读取失败。<br/>Workflow read failed.";
                    break;
                case "Withdrawal":
                    flag = "True撤回成功<br/>Withdrawal successful.";
                    break;
                case "WithdrawalFailed":
                    flag = "撤回失败，该表单当前不在允许撤回阶段，或下一节点的已操作完毕。<br/>Failed to withdraw. The form is not in the stage of withdrawing, or the operation of the next node has been completed.";
                    break;
                case "TipWithdrawal":
                    flag = "系统提示：该表单当前不在允许撤回阶段，不能撤回操作。<br/>Tip: The form is currently not in the stage of allowing withdrawal, and the operation cannot be withdrawn.";
                    break;
                case "PWDWrong":
                    flag = "密码不正确。<br/>The password is incorrect.";
                    break;
                case "StateFailed":
                    flag = "状态设置失败。<br/>Status setting failed.";
                    break;
                case "BatchApproval":
                    flag = "True批量审批成功。<br/>Batch approval successfully.";
                    break;
                case "BatchApprovalFailed":
                    flag = "批量审批失败，在执行批量审批前数据检查失败。<br/>Batch approval failed. Data check failed before batch approval.";
                    break;
                case "BatchOperation":
                    flag = "True批量操作成功。<br/>Batch operation successfully.";
                    break;
                case "BatchOperationFailed":
                    flag = "批量操作失败。<br/>Batch operation failed.";
                    break;
            }

            return flag;
        }

        /// <summary>
        /// 传入内容返回字段集区块
        /// </summary>
        /// <param name="Title">标题</param>
        /// <param name="Content">详细内容</param>
        /// <param name="ColorCss">颜色CSS代码</param>
        /// <returns></returns>
        public static string GetFieldSet(string Title, string Content, string ColorCss = "ws-font-red")
        {
            string flag = string.Empty;

            //flag += "<div class=\"layui-fluid\">";
            flag += "<div class=\"layui-card\">";
            flag += "<div class=\"layui-card-body " + ColorCss + "\">";
            flag += "<fieldset class=\"layui-elem-field\" style=\" background-color:white; \">";
            flag += "<legend class=\"ws-font-size18\">" + Title + "</legend>";
            flag += "<div class=\"layui-field-box\">";
            flag += Content;
            flag += "</div>";
            flag += "</fieldset>";
            flag += "</div>";
            flag += "</div>";
            //flag += "</div>";

            return flag;
        }

        /// <summary>
        /// 为表的指定字段（Filed）的值递增1
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="Field"></param>
        /// <param name="PrimaryKeyName"></param>
        /// <param name="PrimaryKeyValue"></param>
        public static void SetFieldIncrease(string TableName, string Field, string PrimaryKeyName, string PrimaryKeyValue)
        {
            string _sql = " update " + TableName + " set " + Field + " = " + Field + " + 1 where " + PrimaryKeyName + " = @PrimaryKeyValue";
            SqlParameter[] _sp = { new SqlParameter("@PrimaryKeyValue", SqlDbType.Int) };
            _sp[0].Value = PrimaryKeyValue.toInt();

            MsSQLDbHelper.ExecuteSql(_sql, _sp);
        }

        #region 获取客户端IP地址

        /// <summary>  
        /// 获取客户端IP地址  
        /// </summary>  
        /// <returns></returns>  
        public static string GetClientIP()
        {
            //string result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            //if (string.IsNullOrEmpty(result))
            //{
            //    result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            //}
            //if (string.IsNullOrEmpty(result))
            //{
            //    result = HttpContext.Current.Request.UserHostAddress;
            //}
            //if (string.IsNullOrEmpty(result))
            //{
            //    return "0.0.0.0";
            //}
            //return result;

            if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                return HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            //return System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(new char[] { ',' })[0];
            //如果有代理取第一个IP
            else
                return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        }

        ///  <summary>    
        ///  取得客户端真实IP。如果有代理则取第一个非内网地址    
        ///  </summary>    
        public static string GetIPAddress
        {
            get
            {
                var result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!string.IsNullOrEmpty(result))
                {
                    //可能有代理    
                    if (result.IndexOf(".") == -1)        //没有“.”肯定是非IPv4格式    
                        result = null;
                    else
                    {
                        if (result.IndexOf(",") != -1)
                        {
                            //有“,”，估计多个代理。取第一个不是内网的IP。    
                            result = result.Replace("  ", "").Replace("'", "");
                            string[] temparyip = result.Split(",;".ToCharArray());
                            for (int i = 0; i < temparyip.Length; i++)
                            {
                                if (IsIPAddress(temparyip[i])
                                        && temparyip[i].Substring(0, 3) != "10."
                                        && temparyip[i].Substring(0, 7) != "192.168"
                                        && temparyip[i].Substring(0, 7) != "172.16.")
                                {
                                    return temparyip[i];        //找到不是内网的地址    
                                }
                            }
                        }
                        else if (IsIPAddress(result))  //代理即是IP格式    
                            return result;
                        else
                            result = null;        //代理中的内容  非IP，取IP    
                    }

                }

                string IpAddress = (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null && HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != String.Empty) ? HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] : HttpContext.Current.Request.ServerVariables["HTTP_X_REAL_IP"];

                if (string.IsNullOrEmpty(result))
                    result = HttpContext.Current.Request.ServerVariables["HTTP_X_REAL_IP"];

                if (string.IsNullOrEmpty(result))
                    result = HttpContext.Current.Request.UserHostAddress;

                return result;
            }


        }

        /// <summary>
        /// 获取客户端IP地址（无视代理）
        /// </summary>
        /// <returns>若失败则返回回送地址</returns>
        public static string GetHostAddress()
        {
            string userHostAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            if (string.IsNullOrEmpty(userHostAddress))
            {
                if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                    userHostAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString().Split(',')[0].Trim();
            }
            if (string.IsNullOrEmpty(userHostAddress))
            {
                userHostAddress = HttpContext.Current.Request.UserHostAddress;
            }

            //最后判断获取是否成功，并检查IP地址的格式（检查其格式非常重要）
            if (!string.IsNullOrEmpty(userHostAddress) && IsIP(userHostAddress))
            {
                return userHostAddress;
            }
            return "127.0.0.1";
        }

        /// <summary>
        /// 检查IP地址格式
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        ///  <summary>  
        ///  判断是否是IP地址格式  0.0.0.0  
        ///  </summary>  
        ///  <param  name="Str">待判断的IP地址</param>  
        ///  <returns>true  or  false</returns>  
        public static bool IsIPAddress(string Str)
        {
            if (string.IsNullOrEmpty(Str) || Str.Length < 7 || Str.Length > 15) return false;

            const string regFormat = @"^d{1,3}[.]d{1,3}[.]d{1,3}[.]d{1,3}$";

            var regex = new Regex(regFormat, RegexOptions.IgnoreCase);
            return regex.IsMatch(Str);
        }

        /// <summary>
        ///  IP地址转换为Int64
        /// </summary>
        /// <param name="IP"></param>
        /// <returns></returns>
        public static Int64 GetIPNum(string IP)
        {
            Int64 num = 0;
            try
            {
                string[] IPS = new string[4];
                IPS = IP.Split('.');
                string IP0 = IPS[0].toStringTrim(),
                    IP1 = IPS[1].toStringTrim(),
                    IP2 = IPS[2].toStringTrim(),
                    IP3 = IPS[3].toStringTrim();

                num = Convert.ToInt64(IP0) * 256 * 256 * 256 + Convert.ToInt64(IP1) * 256 * 256 + Convert.ToInt64(IP2) * 256 + Convert.ToInt64(IP3) - 1;
            }
            catch { }
            return num;
        }

        /// <summary>
        /// 检查用户IP是否在系统设定的IP列表内，对应SystemInfo表的IPList字段
        /// </summary>
        /// <returns></returns>
        public static Boolean CheckUserIP(string IPList)
        {
            Boolean flag = false;
            int Num = 0;

            string UserIP = GetClientIP();

            //测试数据
            //IPList = "10.248.136.1";
            //UserIP = "10.248.138.1";

            string[] ArrIPList = IPList.Split(Convert.ToChar(10));
            if (ArrIPList.Length > 0)
            {
                if (Num == 0)  //只有Num==0时才进行循环，否则跳出循环
                {
                    for (int i = 0; i < ArrIPList.Length; i++)
                    {
                        string IPS = ArrIPList[i].toStringTrim();
                        string[] ArrIPList2 = IPS.Split('-');
                        if (ArrIPList2.Length > 1)  // 用大于1表示是否存在“-”减号分隔的数组
                        {
                            for (int j = 0; j < ArrIPList2.Length; j++)
                            {
                                string IP21 = ArrIPList2[0].toStringTrim(),  //减号分隔的IP网段，第1位
                                    IP22 = ArrIPList2[1].toStringTrim();   //减号分隔的IP网段，第2位

                                if (GetIPNum(UserIP) >= GetIPNum(IP21) && GetIPNum(UserIP) <= GetIPNum(IP22))
                                    Num = Num + 1;
                            }
                        }
                        else
                        {
                            if (GetIPNum(UserIP) == GetIPNum(IPS))
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
        /// 检查站点访问控制，根据当前用户访问IP与指定IPList进比较实现允许或拒绝访问的功能，需要启用访问控制并且指定IPList不为空时才生效
        /// </summary>
        public static void CheckWebSiteAccessControl()
        {

            string IPList = GetMicroInfo("IPList"), ClientIP = GetClientIP().toStringTrim();
            Boolean IsAccessControl = GetMicroInfo("IsAccessControl").toBoolean(),
                DenyIPListAccess = GetMicroInfo("DenyIPListAccess").toBoolean();

            try
            {
                if (ClientIP != GetServerIP())
                {
                    if (IsAccessControl && !string.IsNullOrEmpty(IPList))
                    {
                        Boolean checkUserIP = CheckUserIP(IPList);

                        if (!DenyIPListAccess)  //!(感叹号)允许IP列表访问的情况下，如果UserIP不在允许列表范围则拒绝访问
                        {
                            if (!checkUserIP)
                                HttpContext.Current.Response.Redirect("~/Views/Message/DenyAccess");
                        }
                        else //否则在拒绝访问的情况下，如果UserIP在拒绝列表范围内则拒绝访问
                        {
                            if (checkUserIP)
                                HttpContext.Current.Response.Redirect("~/Views/Message/DenyAccess");
                        }
                    }
                }
            }
            catch { }
        }


        /// <summary>
        /// 获取服务器端IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetServerIP()
        {
            return HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"];
        }

        #endregion


        /// <summary>
        /// 内置函数传入特定的函数类型返回结果(主要应用于生成控件和提交表单)。如：GetDateTimeNow 返回系统当前时间 、 GetClientIP 返回客户端IP
        /// </summary>
        /// <param name="FunType">函数类型。如：GetDateTimeNow 返回系统当前时间 、 GetClientIP 返回客户端IP、GetUID返回当前登录用户ID、GetUserName返回当前登录用户UserName、GetChineseName返回当前登录用户ChineseName、GetEnglishName返回当前登录用户EnglishName、GetDisplayName返回当前登录用户DisplayName</param>
        /// <returns></returns>
        public static string GetBuiltinFunction(string Action, string FieldValue, string ExtraFunction, string ShortTableName, string TableName, string FormID)
        {
            string flag = string.Empty, _FunType = string.Empty, CustomField = string.Empty, WhereField = string.Empty;
            string[] FieldValueArray = FieldValue.Split('_');
            if (FieldValueArray.Length > 1)
            {
                _FunType = FieldValueArray[0];

                if (_FunType.Length >= 10)
                {
                    if (_FunType.Substring(0, 10) == "GetPrivate")
                    {
                        FieldValue = _FunType;
                        CustomField = FieldValueArray[1];
                        WhereField = FieldValueArray[2];
                    }
                }
            }

            switch (FieldValue)
            {
                case "GetDateTimeNow":
                    flag = DateTime.Now.toDateFormat("yyyy-MM-dd HH:mm:ss");
                    break;
                case "GetClientIP":
                    flag = GetClientIP();
                    break;
                case "GetUID":
                    flag = MicroUserInfo.GetUserInfo("UID");
                    break;
                case "GetUserName":
                    flag = MicroUserInfo.GetUserInfo("UserName");
                    break;
                case "GetChineseName":
                    flag = MicroUserInfo.GetUserInfo("ChineseName");
                    break;
                case "GetEnglishName":
                    flag = MicroUserInfo.GetUserInfo("EnglishName");
                    break;
                case "GetTel":
                    flag = MicroUserInfo.GetUserInfo("WorkTel");
                    break;
                case "GetPhone":
                    flag = MicroUserInfo.GetUserInfo("WorkMobilePhone");
                    break;
                case "GetDisplayName":
                    flag = MicroUserInfo.GetUserInfo("DisplayName");
                    break;
                case "GetMail":
                    flag = MicroUserInfo.GetUserInfo("EMail");
                    break;
                case "GetDeptsID":
                    flag = MicroUserInfo.GetUserInfo("ParentDeptsID");
                    break;
                case "GetDeptID":
                    flag = MicroUserInfo.GetUserInfo("LastDeptID");
                    break;
                case "GetDate":
                    flag = DateTime.Now.toDateFormat("yyyy-MM-dd");
                    break;
                case "GetNextDate":
                    flag = DateTime.Now.AddDays(1).toDateFormat("yyyy-MM-dd");
                    break;
                case "GetTime":
                    string mm = DateTime.Now.toDateFormat("mm");
                    mm = mm.toInt() >= 30 ? "30" : "00";
                    flag = DateTime.Now.toDateFormat("HH") + ":" + mm;
                    break;
                case "GetYearMonth":
                    flag = DateTime.Now.toDateFormat("yyyy-MM");
                    break;
                case "GetFormNumber":
                    lock (flag = MicroForm.GetFormNumber(TableName, FormID))
                    break;

                case "GetFormState":
                    var GetFormAttr = MicroForm.GetFormAttr(ShortTableName, FormID);
                    Boolean IsApproval = GetFormAttr.IsApproval;  //判断表单是否需要审批
                    Boolean IsAccept = GetFormAttr.IsAccept;

                    if (IsApproval)
                        flag = MicroWorkFlow.GetApprovalState(0);  //需要审批时，第一次提交表单设置表单状态为0“等待审批”
                    else
                    {
                        //不需要审批时，判断表单表（Forms）是否启用受理属性，启用受理属性时表单状态为23“ 等待对应[WaitingForProcessing]”，否则表单状态属性为33“对应中[Processing]”
                        if (IsAccept)
                            flag = MicroWorkFlow.GetApprovalState(23);
                        else
                            flag = MicroWorkFlow.GetApprovalState(33);
                    }

                    break;

                case "GetFormStateCode":  //获取表单状态代码，判断表单是否已经进入审批流程（进入审批流程后表单不能修改）
                    var GetFormAttr2 = MicroForm.GetFormAttr(ShortTableName, FormID);
                    Boolean IsApproval2 = GetFormAttr2.IsApproval; //判断表单是否需要审批
                    Boolean IsAccept2 = GetFormAttr2.IsAccept;

                    if (IsApproval2)
                        flag = "0";  //需要审批时，第一次提交表单设置表单状态为0“等待审批”
                    else
                    {
                        //不需要审批时，判断表单表（Forms）是否启用受理属性，启用受理属性时表单状态为23“ 等待对应[WaitingForProcessing]”，否则表单状态属性为33“对应中[Processing]”
                        if (IsAccept2)
                            flag = "23";
                        else
                            flag = "33";
                    }
                    break;

                case "GetPrivateSort":
                    flag = MicroPrivate.GetPrivateSortField(TableName, CustomField, WhereField);

                    break;

                default:
                    flag = FieldValue;
                    break;
            }

            if (!string.IsNullOrEmpty(ExtraFunction))
                MicroPrivate.CtrlVoidExtraFunction(FieldValue, ExtraFunction);


            return flag;
        }

        /// <summary>
        /// 以分隔符截取第一位数
        /// </summary>
        /// <param name="Str">传入字符串</param>
        /// <param name="Symbol">分割符</param>
        /// <returns></returns>
        public static string GetSplitFirstStr(string Str, char Symbol) //以分隔符截取第一位数
        {
            string flag = string.Empty;
            try
            {
                string[] tmpArray = Str.Split(Symbol);
                if (tmpArray.Length > 0)
                {
                    flag = tmpArray[0];
                }
            }
            catch { }

            return flag;
        }

        /// <summary>
        /// 以分隔符截取最后一位数
        /// </summary>
        /// <param name="Str">传入字符串</param>
        /// <param name="Symbol">分割符</param>
        /// <returns></returns>
        public static string GetSplitLastStr(string Str, char Symbol)  //以分隔符截取最后一位数
        {
            string flag = string.Empty;
            try
            {
                int idx = Str.LastIndexOf(Symbol);
                if (idx >= 0)
                {
                    string tmp = Str.Substring(idx + 1); //以分隔符截取最后一位数
                    flag = tmp;
                }
            }
            catch { }

            return flag;
        }

        /// <summary>
        /// 传入字符串、分隔符、返回第几个分隔的值
        /// </summary>
        /// <param name="Str">字符串</param>
        /// <param name="Symbol">分隔符，单引号','</param>
        /// <param name="Num">返回第几个值，由0开始</param>
        /// <returns></returns>
        public static string GetSplitStr(string Str, char Symbol, int Num) //以分隔符截取输入的位数
        {
            string[] tmpArray = Str.Split(Symbol);
            if (tmpArray.Length >= 0)
                return tmpArray[Num].ToString();
            else
                return "";
        }


        /// <summary>
        /// 传入字符串(Source，指定符号分隔的字符串)，要比较的字符(Target，单个字符)，如果出现相同的则返回True，否则False
        /// </summary>
        /// <param name="Source">指定符号分隔的字符串</param>
        /// <param name="Target">单个字符</param>
        /// <param name="Symbol"></param>
        /// <returns></returns>
        public static Boolean CheckSplitExists(string Source, string Target, char Symbol) //以分隔符截取输入的位数
        {
            Boolean flag = false;
            int sum = 0;

            if (!string.IsNullOrEmpty(Source) && !string.IsNullOrEmpty(Target) && Target != "0")
            {
                string[] tmpArray = Source.Split(Symbol);

                    for (int i = 0; i < tmpArray.Length; i++)
                    {
                        if (tmpArray[i].toStringTrim()== Target)
                            sum = sum + 1;
                    }
                
            }

            if (sum > 0)
                flag = true;

            return flag;
        }

        /// <summary>
        /// 判断两个指定符号分隔的字符串，判断源字符串是否包含在目标字符串中，如果存在则返回True
        /// </summary>
        /// <param name="Source">Source指定符号分隔的字符串</param>
        /// <param name="Target">Target指定符号分隔的字符串</param>
        /// <param name="Symbol">指定符号</param>
        /// <returns></returns>
        public static Boolean CheckSplitExistss(string Source, string Target, char Symbol) //以分隔符截取输入的位数
        {
            Boolean flag = false;
            int sum = 0;

            if (!string.IsNullOrEmpty(Source) && !string.IsNullOrEmpty(Target))
            {
                string[] sourceArray = Source.Split(Symbol);
                string[] targetArray = Target.Split(Symbol);

                for (int i = 0; i < sourceArray.Length; i++)
                {
                    for (int j = 0; j < targetArray.Length; j++)
                    {
                        if (sourceArray[i].toStringTrim() == targetArray[j].toStringTrim())
                            sum = sum + 1;
                    }
                }
            }

            if (sum > 0)
                flag = true;

            return flag;
        }


        /// <summary>
        /// 获取两个逗号分隔字符串获相同部分，并返回
        /// </summary>
        /// <param name="SourceStrs"></param>
        /// <param name="TargetStrs"></param>
        /// <param name="Symbol"></param>
        /// <returns></returns>
        public static string GetSourcesOfTargets(string SourceStrs, string TargetStrs, char Symbol = ',')
        {
            string flag = string.Empty;

            if (!string.IsNullOrEmpty(SourceStrs) && !string.IsNullOrEmpty(TargetStrs))
            {
                string[] ArraySourceStrs = SourceStrs.Split(Symbol);
                string[] ArrayTargetStrs = TargetStrs.Split(Symbol);

                for (int i = 0; i < ArraySourceStrs.Length; i++)
                {
                    for (int j = 0; j < ArrayTargetStrs.Length; j++)
                    {
                        if (ArraySourceStrs[i].toStringTrim() == ArrayTargetStrs[j].toStringTrim())
                            flag += ArraySourceStrs[i].toStringTrim() + Symbol;
                    }
                }

                if (!string.IsNullOrEmpty(flag))
                    flag = flag.Substring(0, flag.Length - 1);
            }

            return flag;

        }


        /// <summary>
        /// 把DataTable的某一列转为指定符号分隔的字符串
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="ColumnName"></param>
        /// <param name="Symbol"></param>
        /// <returns></returns>
        public static string GetDataTableColumnSplit(DataTable dataTable, string ColumnName, string Symbol)
        {
            Type dataType = dataTable.Columns[ColumnName].DataType;

            if (dataType.Name == "String")
            {
                string[] idIn = dataTable.AsEnumerable().Select(r => r.Field<string>(ColumnName)).ToArray();
                return string.Join(Symbol, idIn);
            }
            else if (dataType.Name == "Int32")
            {
                int[] idIn = dataTable.AsEnumerable().Select(d => d.Field<int>(ColumnName)).ToArray();
                return string.Join(Symbol, idIn);
            }
            
                return null;
        }


        /// <summary>
        /// 去除逗号分隔重复的字符串
        /// </summary>
        /// <param name="Str"></param>
        /// <returns></returns>
        public static string GetDistinct(string Str, char Symbol = ',')
        {
            string flag = string.Empty;
            if (!string.IsNullOrEmpty(Str))
                flag = string.Join(Symbol.ToString(), Str.Split(Symbol).Distinct());  //去除逗号分隔重复的字符串

            return flag;
        }



        /// <summary>
        /// 用户登录时反馈的信息，单独写在这里是因为UserInfor方法是非static的
        /// </summary>
        /// <param name="LoginStatus"></param>
        /// <param name="HomePage"></param>
        /// <returns></returns>
        public static string GetLoginMsg(int LoginStatus, string HomePage = "")
        {
            string LoginMsg = string.Empty;
            switch (LoginStatus)
            {
                case 0:
                    LoginMsg = "请您先登录。<br/>Please log in first.";
                    break;
                case 1:
                    LoginMsg = "帐号或密码不正确。<br/>Incorrect account or password.";
                    break;
                case 2:
                    LoginMsg = "该帐号没有激活或已被禁用。<br/> The account is not activated or disabled.";
                    break;
                case 3:
                    LoginMsg = "验证码不正确。<br/> Incorrect verification code.";
                    break;
                case 4:
                    LoginMsg = "当前的Windows ID尚未与系统关联，请您使用帐号和密码进行登录。<br/> The Current Windows ID has not been associated with the system. Please use your account and password to login.";
                    break;
                case 5:
                    LoginMsg = "当前Winodws登录方式非域用户验证方式，请您使用帐号和密码进行登录。<br/> The current windows login mode is non domain user authentication mode. Please use your account and password to login.";
                    break;
                case 6:
                    LoginMsg = "当前Windows用户所在的域没有经过认证，请您使用帐号和密码进行登录。<br/> The domain of the Current Windows user is not authenticated. Please log in with your account and password.";
                    break;
                case 7:
                    LoginMsg = "系统没有启用域帐号认证，请您使用帐号和密码进行登录。<br/> The system does not enable domain account authentication. Please log in with your account and password.";
                    break;
                case 888:
                    LoginMsg = "True" + HomePage;
                    break;

            }

            return LoginMsg;
        }

        /// <summary>
        /// 判断是否是数字  2019年9月22日20:54:40  Dennyhui
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumeric(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
        }


        /// <summary>
        /// 传入一个日期返回星期几，星期几的格式： Week=星期几，WW=周几，EN=英文星期
        /// </summary>
        /// <param name="dateTime">日期</param>
        /// <param name="weekFormat">格式： Week=星期几，WW=周几，EN=英文星期</param>
        /// <returns></returns>
        public static string GetWeek(string dateTime, string weekFormat = "")
        {
            string flag = string.Empty;
            weekFormat = string.IsNullOrEmpty(weekFormat) == true ? "Week" : weekFormat;
            weekFormat = weekFormat.ToLower();

            int y = dateTime.toDateFormat("yyyy").toInt();
            int m = dateTime.toDateFormat("MM").toInt();
            int d = dateTime.toDateFormat("dd").toInt();

            int Weeks = GetWeekDay(y, m, d);

            if (weekFormat == "week")
            {
                switch (Weeks)
                {
                    case 1:
                        flag = "星期一";
                        break;
                    case 2:
                        flag = "星期二";
                        break;
                    case 3:
                        flag = "星期三";
                        break;
                    case 4:
                        flag = "星期四";
                        break;
                    case 5:
                        flag = "星期五";
                        break;
                    case 6:
                        flag = "星期六";
                        break;
                    case 7:
                        flag = "星期日";
                        break;
                }
            }
            else if (weekFormat == "ww")
            {
                switch (Weeks)
                {
                    case 1:
                        flag = "周一";
                        break;
                    case 2:
                        flag = "周二";
                        break;
                    case 3:
                        flag = "周三";
                        break;
                    case 4:
                        flag = "周四";
                        break;
                    case 5:
                        flag = "周五";
                        break;
                    case 6:
                        flag = "周六";
                        break;
                    case 7:
                        flag = "周日";
                        break;
                }
            }
            else if (weekFormat == "en")
            {
                switch (Weeks)
                {
                    case 1:
                        flag = "Monday";
                        break;
                    case 2:
                        flag = "Tuesday";
                        break;
                    case 3:
                        flag = "Wednesday";
                        break;
                    case 4:
                        flag = "Thursday";
                        break;
                    case 5:
                        flag = "Friday";
                        break;
                    case 6:
                        flag = "Saturday";
                        break;
                    case 7:
                        flag = "Sunday";
                        break;
                }
            }

            return flag;
        }

        public static int GetWeekDay(int y, int m, int d)
        {
            if (m == 1) { m = 13; y--; }
            if (m == 2) { m = 14; y--; }
            int week = (d + 2 * m + 3 * (m + 1) / 5 + y + y / 4 - y / 100 + y / 400) % 7 + 1;
            return week;
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="ToAddress">收件地址</param>
        /// <param name="CCAddress">CC地址</param>
        /// <param name="Subject">主题</param>
        /// <param name="Body">内容</param>
        /// <returns></returns>
        public static Boolean SendMail(string ToAddress, string CCAddress, string Subject, string Body)
        {
            Boolean flag = false;
            try
            {
                string SysMailAddress, AuthAccount, AuthPassword, SMTPAddress;
                int Port;
                SysMailAddress = GetMicroInfo("SysMailAddress");
                AuthAccount = GetMicroInfo("AuthAccount");
                AuthPassword = GetMicroInfo("AuthPassword");
                SMTPAddress = GetMicroInfo("SMTPAddress");
                Port = GetMicroInfo("Port").toInt();

                ////测试例子
                //ToAddress = "shugeer@163.com";
                //SysMailAddress = "892806509@qq.com";
                //AuthAccount = "892806509@qq.com";
                //AuthPassword = "xqbbtvmnavrdbfgf";   //xqbbtvmnavrdbfgf
                //SMTPAddress = "smtp.qq.com";
                //Port = 587;

                //设定发件服务器信息
                SmtpClient smtpClient = new SmtpClient(); //设置SMTP服务器,和端口
                smtpClient.Host = SMTPAddress;

                //System.Net.Mail 不支持Implicit SSL
                if (Port == 465 || Port == 587)
                    smtpClient.EnableSsl = true;

                smtpClient.Port = Port;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Credentials = new NetworkCredential(AuthAccount, AuthPassword); //验证帐号和密码

                MailMessage mailMessage = new MailMessage();

                string[] toAddressArray = ToAddress.Split(';'); //拆分收件方地址集合
                foreach (string to in toAddressArray)
                {
                    if (to != null && to != "")
                        mailMessage.To.Add(to.ToString());
                }

                if (CCAddress != "")
                {
                    string[] _ccAddressArry = CCAddress.Split(';'); //拆分CC地址集合
                    foreach (string _cc in _ccAddressArry)
                    {
                        if (_cc != null && _cc != "")
                            mailMessage.CC.Add(_cc.ToString());
                    }
                }

                mailMessage.From = new MailAddress(SysMailAddress); //发件者
                mailMessage.Subject = Subject;  //主题
                mailMessage.Body = Body;  //正文内容
                mailMessage.IsBodyHtml = true;  //邮件格式
                mailMessage.BodyEncoding = System.Text.Encoding.UTF8;   //编码

                smtpClient.Send(mailMessage);
                flag = true;
            }
            catch { flag = false; }

            return flag;
        }


        /// <summary>
        /// 根据传入的自定义日期获取年月日（如果传入的时间为空则以当前日期作为计算）
        /// </summary>
        /// <param name="Type">CurrDay=今天yyyy-MM-dd、CurrYearFirstDay=本年的第一天、CurrYearLastDay=本年的最后一天、CurrMonthFirstDay=当前日期月份的第一天、CurrMonthLastDay=当前日期月份的最后一天、CurrWeekFirstDay=当前日期所在星期的第一天（以星期一为第一天）、CurrWeekLastDay=当前日期所在星期的最后一天（以星期日为最后一天）</param>
        /// <param name="CustomDate">自定义日期，格式：yyyy-MM-dd。自定义日期不为空时则以该日期进行计算</param>
        /// <returns></returns>
        public static string GetYearMonthDay(string Type, string CustomDate = "")
        {
            string flag = string.Empty;
            string _date = DateTime.Now.ToString("yyyy-MM-dd");
            DateTime DateTimeNow = DateTime.Now;

            if (!string.IsNullOrEmpty(CustomDate))
            {
                _date = CustomDate;
                DateTimeNow = DateTime.Parse(CustomDate);
            }

            switch (Type)
            {
                case "CurrDay":
                    flag = DateTimeNow.ToString("yyyy-MM-dd");
                    break;
                case "CurrYearFirstDay":
                    flag = DateTimeNow.ToString("yyyy") + "-01-01";
                    break;
                case "CurrYearLastDay":
                    flag = DateTimeNow.ToString("yyyy") + "-12-31";
                    break;
                case "CurrMonthFirstDay":
                    DateTime CurrFirstDay = DateTime.Parse(_date);
                    flag = (CurrFirstDay.AddDays(1 - CurrFirstDay.Day)).ToString("yyyy-MM-dd");
                    break;
                case "CurrMonthLastDay":
                    DateTime CurrLastDay = DateTime.Parse(_date);
                    flag = (CurrLastDay.AddDays(1 - CurrLastDay.Day)).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
                    break;

                case "CurrWeekFirstDay":
                    //星期一为第一天  
                    int CurrWeekFirstDay = Convert.ToInt32(DateTimeNow.DayOfWeek);
                    //因为是以星期一为第一天，所以要判断weeknow等于0时，要向前推6天。  
                    CurrWeekFirstDay = (CurrWeekFirstDay == 0 ? (7 - 1) : (CurrWeekFirstDay - 1));
                    int CurrWeekFirstDaydiff = (-1) * CurrWeekFirstDay;
                    //本周第一天  
                    flag = DateTimeNow.AddDays(CurrWeekFirstDaydiff).ToString("yyyy-MM-dd");
                    break;

                case "CurrWeekLastDay":
                    //星期天为最后一天  
                    int CurrWeekLastDay = Convert.ToInt32(DateTimeNow.DayOfWeek);
                    CurrWeekLastDay = (CurrWeekLastDay == 0 ? 7 : CurrWeekLastDay);
                    int CurrWeekLastDaydiff = (7 - CurrWeekLastDay);
                    //本周最后一天  
                    flag = DateTimeNow.AddDays(CurrWeekLastDaydiff).ToString("yyyy-MM-dd") + " 23:59:59";
                    break;

                case "QFirstDay":  //日期所在季度的第一天
                    flag = DateTimeNow.toDateTime().AddMonths(0 - ((DateTimeNow.toDateTime().Month - 1) % 3)).AddDays(1 - DateTimeNow.toDateTime().Day).toDateFormat();
                    break;

                case "QLastDay":  //日期所在季度的最后一天
                    flag = DateTimeNow.toDateTime().AddMonths(0 - ((DateTimeNow.toDateTime().Month - 1) % 3)).AddDays(1 - DateTimeNow.toDateTime().Day).toDateFormat();
                    break;

                default:
                    flag = "2020-01-01";
                    break;
            }

            return flag;

        }

        public class DateTimeDiffAttr
        {
            /// <summary>
            /// 天与天对比，相差天数
            /// </summary>
            public string Days { set; get; }

            /// <summary>
            /// 时于时对比，相差小时数
            /// </summary>
            public string Hours { set; get; }

            /// <summary>
            /// 分与分对比，相差分钟数
            /// </summary>
            public string Minutes { set; get; }

            /// <summary>
            /// 秒与秒对比，相关秒数
            /// </summary>
            public string Seconds { set; get; }


            /// <summary>
            /// 总时间相差天数
            /// </summary>
            public string TotalDays { set; get; }

            /// <summary>
            /// 总时间相差小时数
            /// </summary>
            public string TotalHours { set; get; }

            /// <summary>
            /// 总时间相差分钟数
            /// </summary>
            public string TotalMinutes { set; get; }

            /// <summary>
            /// 总时间相差秒数
            /// </summary>
            public string TotalSeconds { set; get; }

            /// <summary>
            /// 两时间差的时分秒,格式 HH:mm:ss
            /// </summary>
            public string HHmmss { set; get; }


        }


        /// <summary>
        /// 比较两个日期时间的差
        /// </summary>
        /// <param name="DateTime1"></param>
        /// <param name="DateTime2"></param>
        /// <returns></returns>
        public static DateTimeDiffAttr GetDateTimeDiff(string StartTime, string EndTime)
        {
            string flag = string.Empty,
               days = string.Empty,  //天与天对比，相差天数
               hours = string.Empty,  //时于时对比，相差小时数
               minutes = string.Empty,  //分与分对比，相差分钟数
               seconds = string.Empty,  //秒与秒对比，相关秒数
               totalDays = string.Empty,  //总时间相差天数
               totalHours = string.Empty,  //总时间相差小时数
               totalMinutes = string.Empty,  //总时间相差分钟数
               totalSeconds = string.Empty,  //总时间相差秒数
               hhmmss = string.Empty;

            DateTime t1 = StartTime.toDateTime("yyyy-MM-dd HH:mm:ss"),
                t2 = EndTime.toDateTime("yyyy-MM-dd HH:mm:ss");

            TimeSpan ts = t2 - t1;

            days = ts.Days.ToString();
            hours = ts.Hours.ToString();
            minutes = ts.Minutes.ToString();
            seconds = ts.Seconds.ToString();

            totalDays = ts.TotalDays.ToString();
            totalHours = ts.TotalHours.ToString();
            totalMinutes = ts.TotalMinutes.ToString();
            totalSeconds = ts.TotalSeconds.ToString();

            hhmmss = hours + ":" + minutes + ":" + seconds;


            var DateTimeDiffAttr = new DateTimeDiffAttr
            {
                Days = days,
                Hours = hours,
                Minutes = minutes,
                Seconds = seconds,

                TotalDays = totalDays,
                TotalHours = totalHours,
                TotalMinutes = totalMinutes,
                TotalSeconds = totalSeconds,
                HHmmss = hhmmss
            };


            return DateTimeDiffAttr;
        }


        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="Title">日志标题，默认值空</param>
        /// <param name="LogAction">产生日志动作，默认值空</param>
        /// <param name="LogType">日志类型，默认值空</param>
        public static void SetSysLog(string Title = "Automatic", string LogAction = "View", string LogType = "User")
        {
            try
            {
                string _sql = "insert into SystemLog (Title, LogAction, LogType, URL, IP, BrowserType, BrowserName, BrowserVersion, Platform, OS, OSVersion, UserAgent, UID) values (@Title, @LogAction, @LogType, @URL, @IP, @BrowserType, @BrowserName, @BrowserVersion, @Platform, @OS, @OSVersion, @UserAgent, @UID)";
                SqlParameter[] _sp = { new SqlParameter("@Title",SqlDbType.NVarChar,4000),
                             new SqlParameter("@LogAction",SqlDbType.NVarChar,4000),
                             new SqlParameter("@LogType",SqlDbType.NVarChar,4000),
                             new SqlParameter("@URL",SqlDbType.VarChar,4000),
                             new SqlParameter("@IP",SqlDbType.VarChar,100),
                             new SqlParameter("@BrowserType",SqlDbType.VarChar,2000),
                             new SqlParameter("@BrowserName",SqlDbType.VarChar,2000),
                             new SqlParameter("@BrowserVersion",SqlDbType.VarChar,2000),
                             new SqlParameter("@Platform",SqlDbType.VarChar,2000),
                             new SqlParameter("@OS",SqlDbType.VarChar,2000),
                             new SqlParameter("@OSVersion",SqlDbType.VarChar,2000),
                             new SqlParameter("@UserAgent",SqlDbType.VarChar,2000),
                             new SqlParameter("@UID",SqlDbType.Int)
                            };


                _sp[0].Value = Title;
                _sp[1].Value = LogAction;
                _sp[2].Value = LogType;
                _sp[3].Value = HttpContext.Current.Request.Url.ToString();
                _sp[4].Value = GetClientIP();
                _sp[5].Value = GetBrowser("BrowserNameVersion");
                _sp[6].Value = GetBrowser("BrowserName");
                _sp[7].Value = GetBrowser("Version");
                _sp[8].Value = GetBrowser("Platform");
                _sp[9].Value = GetOS();
                _sp[10].Value = null;
                _sp[11].Value = HttpContext.Current.Request.UserAgent;
                _sp[12].Value = MicroUserInfo.GetUserInfo("UID").toInt();

                MsSQLDbHelper.ExecuteSql(_sql, _sp);
            }
            catch { }
        }

        /// <summary>
        /// 获取浏览器版本信息
        /// </summary>
        /// <param name="InfoType">信息类型：浏览器名称和版本号=BrowserNameVersion，名称=BrowserName，版本=Version，操作平台=Platform</param>
        /// <returns></returns>
        public static string GetBrowser(string InfoType)
        {
            string flag = string.Empty;

            HttpBrowserCapabilities Browser = HttpContext.Current.Request.Browser;

            switch (InfoType)
            {
                case "BrowserNameVersion":
                    flag = Browser.Type;
                    break;
                case "BrowserName":
                    flag = Browser.Browser;
                    break;
                case "Version":
                    flag = Browser.Version;
                    break;
                case "Platform":
                    flag = Browser.Platform;
                    break;
            }

            return flag;
        }


        /// <summary>
        /// 获取用户操作系统名称
        /// </summary>
        /// <returns></returns>
        public static string GetOS()
        {
            string userAgent = HttpContext.Current.Request.UserAgent;

            string osVersion = "unknown";

            if (userAgent.Contains("NT 10.0"))
            {
                osVersion = "Windows 10";
            }
            else if (userAgent.Contains("NT 6.3"))
            {
                osVersion = "Windows 8.1";
            }
            else if (userAgent.Contains("NT 6.2"))
            {
                osVersion = "Windows 8";
            }
            else if (userAgent.Contains("NT 6.1"))
            {
                osVersion = "Windows 7";
            }
            else if (userAgent.Contains("NT 6.0"))
            {
                osVersion = "Windows Vista/Server 2008";
            }
            else if (userAgent.Contains("NT 5.2"))
            {
                if (userAgent.Contains("64"))
                    osVersion = "Windows XP";
                else
                    osVersion = "Windows Server 2003";
            }
            else if (userAgent.Contains("NT 5.1"))
            {
                osVersion = "Windows XP";
            }
            else if (userAgent.Contains("NT 5"))
            {
                osVersion = "Windows 2000";
            }
            else if (userAgent.Contains("NT 4"))
            {
                osVersion = "Windows NT4";
            }
            else if (userAgent.Contains("Me"))
            {
                osVersion = "Windows Me";
            }
            else if (userAgent.Contains("98"))
            {
                osVersion = "Windows 98";
            }
            else if (userAgent.Contains("95"))
            {
                osVersion = "Windows 95";
            }
            else if (userAgent.Contains("Mac"))
            {
                osVersion = "Mac";
            }
            else if (userAgent.Contains("Unix"))
            {
                osVersion = "UNIX";
            }
            else if (userAgent.Contains("Linux"))
            {
                osVersion = "Linux";
            }
            else if (userAgent.Contains("SunOS"))
            {
                osVersion = "SunOS";
            }
            else
            {
                osVersion = System.Web.HttpContext.Current.Request.Browser.Platform;
            }
            return osVersion;
        }

        /// <summary>
        /// 检查浏览器是否为禁止的类型，如果是则返回True （获取当前用户的浏览器版本是否包含为传入的字符进行判断）
        /// </summary>
        /// <param name="BrowserTypes">禁止类型多个可由逗号分隔</param>
        /// <returns></returns>
        public static Boolean CheckBrowser(string BrowserTypes= "InternetExplorer")
        {
            Boolean flag = false;       
            int Num = 0;

            BrowserTypes = GetMicroInfo("UnsupportedBrowser");

            string CurrBrowser = GetBrowser("BrowserNameVersion");

            if (!string.IsNullOrEmpty(BrowserTypes) && !string.IsNullOrEmpty(CurrBrowser))
            {
                string[] ArrBrowserTypes = BrowserTypes.Split(',');
                if (ArrBrowserTypes.Length > 0)
                {                 
                    for (int i = 0; i < ArrBrowserTypes.Length; i++)
                    {
                        string BrowserType = ArrBrowserTypes[i].ToLower();

                        if (CurrBrowser.ToLower().IndexOf(BrowserType) > -1)
                            Num = Num + 1;
                    }
                }

                if (Num > 0)
                    flag = true;
            }


            return flag;

        }


        /// <summary>
        /// 传入一个数组返回一个随机数
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static string GetRandom(string[] Arr)
        {
            // string[] Arr = { "aa", "bb", "bb", "88" };

            Random ran = new Random();

            int i = ran.Next(Arr.Length);

            return Arr[i];

        }

        /// <summary>
        /// 从两个数的范围之间内取随机数
        /// </summary>
        /// <param name="StartNum"></param>
        /// <param name="EndNum"></param>
        /// <returns></returns>
        public static int GetRangeRandom(int StartNum, int EndNum)
        {
            // string[] Arr = { "aa", "bb", "bb", "88" };

            Random ran = new Random();

            int i = ran.Next(StartNum, EndNum);

            return i;

        }


        /// <summary>
        /// 传入数字返回英文字母1=A,2=B,27=AA,28=AB，注：最大支持两位直到702=ZZ
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static string GetLetters(int i)
        {
            //i = i - 1;
            string alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            if (i < 0 || i >= 26 + 26 * 26) return "?";

            if (i < 26) return alpha[i].ToString();

            return alpha[i / 26 - 1].ToString() + alpha[i % 26].ToString();
        }

    }


}



public static class MicroStaticPublic
{
    /// <summary>
    /// 字符串替换，返回String实例，替换JSON数据类型的特殊字符
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string toJsonTrim(this object Obj)
    {
        string flag = string.Empty;
        if (Obj != null)
        {
            if (!string.IsNullOrEmpty(Obj.ToString()))
                flag = Obj.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\b", "\\b").Replace("\f", "\\f").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t").Replace(Convert.ToChar(220).ToString(), "").Replace(Convert.ToChar(13).ToString(), "").Replace("<br/>", "").Replace("<br>", "").Replace("<p>", "").Replace("</p>", "");  //.Replace("/", "\\/") //第3
        }
        return flag;
    }

    public static string toEditorTrim(this object Obj)
    {
        string flag = string.Empty;
        if (Obj != null)
        {
            if (!string.IsNullOrEmpty(Obj.ToString()))
                flag = Obj.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\b", "\\b").Replace("\f", "\\f").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t").Replace(Convert.ToChar(220).ToString(), "").Replace(Convert.ToChar(13).ToString(), "").Replace("<", "\\<").Replace(">", "\\>").Replace("'", "\\'");  //.Replace("/", "\\/") //第3
        }
        return flag;
    }

    /// <summary>
    /// 字符串转换，返回Boolean实例，为“on”或“True”时返回true，否则返回false
    /// </summary>
    /// <param name="Str"></param>
    /// <returns></returns>
    public static Boolean toBoolean(this string Str)
    {
        Boolean flag = false;
        if (!string.IsNullOrEmpty(Str))
        {
            switch (Str.Trim())
            {
                case "on":
                    flag = true;
                    break;
                case "True":
                    flag = true;
                    break;
                case "true":
                    flag = true;
                    break;
                case "1":
                    flag = true;
                    break;
                case "是":
                    flag = true;
                    break;
                default:
                    flag = false;
                    break;
            }
        }
        return flag;
    }

    public static Boolean toBoolean(this object Obj)
    {
        Boolean flag = false;
        if (!string.IsNullOrEmpty(Obj.toStringTrim()))
        {
            switch (Obj.toStringTrim())
            {
                case "on":
                    flag = true;
                    break;
                case "True":
                    flag = true;
                    break;
                case "true":
                    flag = true;
                    break;
                case "0":
                    flag = true;
                    break;
                case "否":
                    flag = true;
                    break;
                default:
                    flag = false;
                    break;
            }
        }
        return flag;
    }

    /// <summary>
    /// 验证字符串是否为日期或时间或日期时间格，如果是则返回True否则返回False
    /// </summary>
    /// <param name="Str"></param>
    /// <returns></returns>
    public static Boolean toDateTimeBoolean(this string Str)
    {
        Boolean flag = false;
        if (!string.IsNullOrEmpty(Str))
        {
            try
            {
                DateTime.Parse(Str);
                flag = true;
            }
            catch { flag = false; }
        }
        return flag;
    }


    /// <summary>
    /// 字符串Trim，返回String实例，Null时返回Empty
    /// </summary>
    /// <param name="Str"></param>
    /// <returns></returns>
    public static string toStringTrim(this string Str)
    {
        string flag = string.Empty;
        if (!string.IsNullOrEmpty(Str))
            flag = Str.Trim();

        return flag;
    }

    public static string toStringTrim(this object Str)
    {
        string flag = string.Empty;
        if (!string.IsNullOrEmpty(Str.ToString()))
            flag = Str.ToString().Trim();

        return flag;
    }

    /// <summary>
    /// 字符串类型转换，返回Int实例，转换失败返回0
    /// </summary>
    /// <param name="Str"></param>
    /// <returns></returns>
    public static int toInt(this string Str)
    {
        int flag = 0;
        try
        {
            if (!string.IsNullOrEmpty(Str))
                flag = int.Parse(Str.Trim());
        }
        catch { }

        return flag;
    }

    public static int toInt(this object Obj)
    {
        int flag = 0;
        try
        {
            if (!string.IsNullOrEmpty(Obj.ToString()))
                flag = int.Parse(Obj.ToString().Trim());
        }
        catch { }

        return flag;

    }

    /// <summary>
    /// 字符串类型转换，返回decimal实例，转换失败返回0
    /// </summary>
    /// <param name="Str"></param>
    /// <returns></returns>
    public static decimal toDecimal(this string Str)
    {
        decimal flag = 0;
        try
        {
            if (!string.IsNullOrEmpty(Str))
                flag = decimal.Parse(Str.Trim());
        }
        catch { }

        return flag;
    }

    public static decimal toDecimal(this object Obj)
    {
        decimal flag = 0;
        try
        {
            if (!string.IsNullOrEmpty(Obj.ToString()))
                flag = decimal.Parse(Obj.ToString().Trim());
        }
        catch { }

        return flag;

    }

    /// <summary>
    /// 字符串类型转换，返回decimal实例，转换失败返回0
    /// </summary>
    /// <param name="Str"></param>
    /// <returns></returns>
    public static decimal toDecimalInt32(this string Str)
    {
        decimal flag = 0;
        try
        {
            if (!string.IsNullOrEmpty(Str))
                flag = decimal.ToInt32(Str.Trim().toDecimal());
        }
        catch { }

        return flag;
    }

    public static decimal toDecimalInt32(this object Obj)
    {
        decimal flag = 0;
        try
        {
            if (!string.IsNullOrEmpty(Obj.ToString()))
                flag = decimal.ToInt32(Obj.ToString().Trim().toDecimal());
        }
        catch { }

        return flag;

    }

    /// <summary>
    /// 字符串类型转换，返回float实例，转换失败返回0
    /// </summary>
    /// <param name="Str"></param>
    /// <returns></returns>
    public static float toFloat(this string Str)
    {
        float flag = 0;
        try
        {
            if (!string.IsNullOrEmpty(Str))
                flag = float.Parse(Str.Trim());
        }
        catch { }

        return flag;
    }

    public static float toFloat(this object Obj)
    {
        float flag = 0;
        try
        {
            if (!string.IsNullOrEmpty(Obj.ToString()))
                flag = float.Parse(Obj.ToString().Trim());
        }
        catch { }

        return flag;

    }

    /// <summary>
    /// 移除最后一位
    /// </summary>
    /// <param name="Str"></param>
    /// <returns></returns>
    public static string toRemoveLastBit(this string Str)
    {
        string flag = string.Empty;
        if (!string.IsNullOrEmpty(Str))
        {
            flag = Str.Trim();
            flag = flag.Substring(0, flag.Length - 1);
        }
        return flag;
    }

    /// <summary>
    /// 移除最后一位
    /// </summary>
    /// <param name="Obj"></param>
    /// <returns></returns>
    public static string toRemoveLastBit(this object Obj)
    {
        string flag = string.Empty;
        if (!string.IsNullOrEmpty(Obj.ToString()))
        {
            flag = Obj.ToString().Trim();
            flag = flag.Substring(0, flag.Length - 1);
        }
        return flag;

    }

    /// <summary>
    /// 返回格式化后的日期字符串类型
    /// </summary>
    /// <param name="Str"></param>
    /// <param name="Format"></param>
    /// <returns></returns>
    public static string toDateFormat(this string Str, string Format = "")
    {
        string flag = null;
        if (string.IsNullOrEmpty(Format))
        {
            string DateFormat = MicroPublic.GetMicroInfo("DateFormat");  //返回系统设定的日期格式
            if (!string.IsNullOrEmpty(DateFormat))
                Format = DateFormat;
            else
                Format = "yyyy-MM-dd";
        }

        try
        {
            if (!string.IsNullOrEmpty(Str))
                flag = DateTime.Parse(Str).ToString(Format);
        }
        catch { }

        return flag;
    }

    public static string toDateFormat(this object Obj, string Format = "")
    {
        string flag = "2020-01-01";
        if (string.IsNullOrEmpty(Format))
        {
            string DateFormat = MicroPublic.GetMicroInfo("DateFormat");  //返回系统设定的日期格式
            if (!string.IsNullOrEmpty(DateFormat))
                Format = DateFormat;
            else
                Format = "yyyy-MM-dd";
        }

        try
        {
            if (!string.IsNullOrEmpty(Obj.ToString().Trim()))
                flag = DateTime.Parse(Obj.ToString().Trim()).ToString(Format);
            else
                flag = null;
        }
        catch { }

        return flag;
    }

    /// <summary>
    ///  返回格式化后的日期Datetime类型
    /// </summary>
    /// <param name="Str"></param>
    /// <param name="Format"></param>
    /// <returns></returns>
    public static DateTime toDateTime(this string Str, string Format = "")
    {
        DateTime flag = DateTime.Now;
        if (string.IsNullOrEmpty(Format))  //如果Format为空时
        {
            string DateFormat = MicroPublic.GetMicroInfo("DateFormat");  //返回系统设定的日期格式
            if (!string.IsNullOrEmpty(DateFormat))
                Format = DateFormat;
            else
                Format = "yyyy-MM-dd";
        }

        try
        {
            if (!string.IsNullOrEmpty(Str))
                flag = DateTime.Parse(DateTime.Parse(Str).ToString(Format));
        }
        catch { }

        return flag;
    }

    public static DateTime toDateTime(this object Obj, string Format = "")
    {
        DateTime flag = DateTime.Now;
        if (string.IsNullOrEmpty(Format))
        {
            string DateFormat = MicroPublic.GetMicroInfo("DateFormat");  //返回系统设定的日期格式
            if (!string.IsNullOrEmpty(DateFormat))
                Format = DateFormat;
            else
                Format = "yyyy-MM-dd";
        }

        try
        {
            if (!string.IsNullOrEmpty(Obj.ToString().Trim()))
                flag = DateTime.Parse(DateTime.Parse(Obj.ToString().Trim()).ToString(Format));
        }
        catch { }

        return flag;
    }


    public static string toMenuSplit(this string Str, char Symbol = '/')
    {
        string flag = string.Empty;
        if (!string.IsNullOrEmpty(Str))
        {
            string[] strArray = Str.Split(Symbol);
            if (strArray.Length > 1)
                flag = strArray[0].toStringTrim() + " " + Symbol + "  <cite class=\"ws-font-size8\">" + strArray[1].toStringTrim() + "</cite>";
            else
                flag = Str.Trim();
        }

        return flag;
    }

    public static string toMenuSplit(this object Str, char Symbol = '/')
    {
        string flag = string.Empty;
        if (!string.IsNullOrEmpty(Str.toStringTrim()))
        {
            string[] strArray = Str.toStringTrim().Split(Symbol);
            if (strArray.Length > 1)
                flag = strArray[0].toStringTrim() + " " + Symbol + " <cite class=\"ws-font-size8\">" + strArray[1].toStringTrim() + "</cite>";
            else
                flag = Str.toStringTrim();
        }

        return flag;
    }


    /// <summary>
    /// 传入时间返回yyyy-MM-dd (周几) HH:mm
    /// </summary>
    /// <param name="Str"></param>
    /// <param name="weekFormat">星期格式</param>
    /// <param name="timeFormat">时间格式 传递过来的格式通常会是yyyy-MM-dd HH:mm:ss，因为是时间部分格式化所以只取时间部分的 HH:mm:ss</param>
    /// <returns></returns>
    public static string toDateWeekTime(this string Str, string weekFormat = "", string timeFormat="")
    {
        weekFormat = string.IsNullOrEmpty(weekFormat) ? "ww" : weekFormat;
        timeFormat = string.IsNullOrEmpty(timeFormat) ? "HH:mm" : timeFormat;

        //传递过来的格式通常会是yyyy-MM-dd HH:mm:ss，因为是时间部分格式化所以只取时间部分的 HH:mm:ss
        if (timeFormat.Split(' ').Length > 1)
            timeFormat = timeFormat.Split(' ')[1].toStringTrim();

        string flag = "2020-01-01";

        try
        {
            if (!string.IsNullOrEmpty(Str))
            {
                string date = Str.toDateFormat();  //yyyy-MM-dd
                string week = MicroPublic.GetWeek(Str, weekFormat);
                string time = DateTime.Parse(Str).ToString(timeFormat);

                flag = date + " (" + week + ") " + time;
            }
            else
                flag = null;
        }
        catch { }

        return flag;
    }


    /// <summary>
    /// 传入时间返回yyyy-MM-dd (周几) HH:mm
    /// </summary>
    /// <param name="Obj"></param>
    /// <param name="weekFormat"></param>
    /// <returns></returns>
    public static string toDateWeekTime(this Object Obj, string weekFormat = "")
    {
        weekFormat = string.IsNullOrEmpty(weekFormat) ? "ww" : weekFormat;

        string flag = "2020-01-01";

        try
        {
            if (!string.IsNullOrEmpty(Obj.toStringTrim()))
            {
                string date = Obj.toDateFormat();  //yyyy-MM-dd
                string week = MicroPublic.GetWeek(Obj.ToString(), weekFormat);
                string time = DateTime.Parse(Obj.ToString()).ToString("HH:mm");

                flag = date + " (" + week + ") " + time;
            }
            else
                flag = null;
        }
        catch { }

        return flag;
    }


    /// <summary>
    /// 传入时间返回yyyy-MM-dd (周几)
    /// </summary>
    /// <param name="Str"></param>
    /// <param name="weekFormat"></param>
    /// <returns></returns>
    public static string toDateWeek(this string Str, string weekFormat = "")
    {
        weekFormat = string.IsNullOrEmpty(weekFormat) ? "ww" : weekFormat;

        string flag = "2020-01-01";

        try
        {
            if (!string.IsNullOrEmpty(Str))
            {
                string date = Str.toDateFormat();  //yyyy-MM-dd
                string week = MicroPublic.GetWeek(Str, weekFormat);

                flag = date + " (" + week + ")";
            }
            else
                flag = null;
        }
        catch { }

        return flag;
    }


    /// <summary>
    /// 传入时间返回yyyy-MM-dd (周几) 
    /// </summary>
    /// <param name="Obj"></param>
    /// <param name="weekFormat"></param>
    /// <returns></returns>
    public static string toDateWeek(this Object Obj, string weekFormat = "")
    {
        weekFormat = string.IsNullOrEmpty(weekFormat) ? "ww" : weekFormat;

        string flag = "2020-01-01";

        try
        {
            if (!string.IsNullOrEmpty(Obj.toStringTrim()))
            {
                string date = Obj.toDateFormat();  //yyyy-MM-dd
                string week = MicroPublic.GetWeek(Obj.ToString(), weekFormat);

                flag = date + " (" + week + ")";
            }
            else
                flag = null;
        }
        catch { }

        return flag;
    }

    /// <summary>
    /// 返回日期所在星期的第一天
    /// </summary>
    /// <param name="Str"></param>
    /// <param name="Format"></param>
    /// <returns></returns>
    public static string toDateWFirstDay(this string Str, string Format = "")
    {   
        string flag = null;
        if (string.IsNullOrEmpty(Format))
        {
            string DateFormat = MicroPublic.GetMicroInfo("DateFormat");  //返回系统设定的日期格式
            if (!string.IsNullOrEmpty(DateFormat))
                Format = DateFormat;
            else
                Format = "yyyy-MM-dd";
        }

        try
        {
            if (!string.IsNullOrEmpty(Str))
            {
                int weeknow = Convert.ToInt32(Str.toDateTime().DayOfWeek);
                int daydiff = (-1) * weeknow + 1;
                //本周第一天
                flag=Str.toDateTime().AddDays(daydiff).ToString(Format);
            }
        }
        catch { }

        return flag;
    }

    /// <summary>
    /// 返回所在星期的第一天
    /// </summary>
    /// <param name="Obj"></param>
    /// <param name="Format"></param>
    /// <returns></returns>
    public static string toDateWFirstDay(this object Obj, string Format = "")
    {
        string flag = null;
        if (string.IsNullOrEmpty(Format))
        {
            string DateFormat = MicroPublic.GetMicroInfo("DateFormat");  //返回系统设定的日期格式
            if (!string.IsNullOrEmpty(DateFormat))
                Format = DateFormat;
            else
                Format = "yyyy-MM-dd";
        }

        try
        {
            if (!string.IsNullOrEmpty(Obj.ToString().Trim()))
            {
                int weeknow = Convert.ToInt32(Obj.toDateTime().DayOfWeek);
                int daydiff = (-1) * weeknow + 1;
                //本周第一天
                flag = Obj.toDateTime().AddDays(daydiff).ToString(Format);
            }
        }
        catch { }

        return flag;
    }

    /// <summary>
    /// 返回日期所在星期的最后一天
    /// </summary>
    /// <param name="Str"></param>
    /// <param name="Format"></param>
    /// <returns></returns>
    public static string toDateWLastDay(this string Str, string Format = "")
    {
        string flag = null;
        if (string.IsNullOrEmpty(Format))
        {
            string DateFormat = MicroPublic.GetMicroInfo("DateFormat");  //返回系统设定的日期格式
            if (!string.IsNullOrEmpty(DateFormat))
                Format = DateFormat;
            else
                Format = "yyyy-MM-dd";
        }

        try
        {
            if (!string.IsNullOrEmpty(Str))
            {
                int weeknow = Convert.ToInt32(Str.toDateTime().DayOfWeek);
                int dayadd = 7 - weeknow;
                //本周最后一天
                flag = Str.toDateTime().AddDays(dayadd).ToString(Format);
            }
        }
        catch { }

        return flag;
    }

    /// <summary>
    /// 返回日期所在星期的最后一天
    /// </summary>
    /// <param name="Obj"></param>
    /// <param name="Format"></param>
    /// <returns></returns>
    public static string toDateWLastDay(this object Obj, string Format = "")
    {
        string flag = null;
        if (string.IsNullOrEmpty(Format))
        {
            string DateFormat = MicroPublic.GetMicroInfo("DateFormat");  //返回系统设定的日期格式
            if (!string.IsNullOrEmpty(DateFormat))
                Format = DateFormat;
            else
                Format = "yyyy-MM-dd";
        }

        try
        {
            if (!string.IsNullOrEmpty(Obj.ToString().Trim()))
            {
                int weeknow = Convert.ToInt32(Obj.toDateTime().DayOfWeek);
                int dayadd = 7 - weeknow;
                //本周最后一天
                flag = Obj.toDateTime().AddDays(dayadd).ToString(Format);
            }
        }
        catch { }

        return flag;
    }

    /// <summary>
    /// 返回日期所在月份的第一天
    /// </summary>
    /// <param name="Str"></param>
    /// <param name="Format"></param>
    /// <returns></returns>
    public static string toDateMFirstDay(this string Str, string Format = "")
    {
        string flag = null;
        if (string.IsNullOrEmpty(Format))
        {
            string DateFormat = MicroPublic.GetMicroInfo("DateFormat");  //返回系统设定的日期格式
            if (!string.IsNullOrEmpty(DateFormat))
                Format = DateFormat;
            else
                Format = "yyyy-MM-dd";
        }

        try
        {
            if (!string.IsNullOrEmpty(Str))
                flag = (Str.toDateTime().Year.ToString() + "-" + Str.toDateTime().Month.ToString() + "-1").toDateFormat(Format);
        }
        catch { }

        return flag;
    }

    /// <summary>
    /// 返回日期所在月份的第一天
    /// </summary>
    /// <param name="Obj"></param>
    /// <param name="Format"></param>
    /// <returns></returns>
    public static string toDateMFirstDay(this object Obj, string Format = "")
    {
        string flag = null;
        if (string.IsNullOrEmpty(Format))
        {
            string DateFormat = MicroPublic.GetMicroInfo("DateFormat");  //返回系统设定的日期格式
            if (!string.IsNullOrEmpty(DateFormat))
                Format = DateFormat;
            else
                Format = "yyyy-MM-dd";
        }

        try
        {
            if (!string.IsNullOrEmpty(Obj.ToString().Trim()))
                flag = (Obj.toDateTime().Year.ToString() + "-" + Obj.toDateTime().Month.ToString() + "-1").toDateFormat(Format);
        }
        catch { }

        return flag;
    }


    /// <summary>
    /// 返回日期所在月份的最后一天
    /// </summary>
    /// <param name="Str"></param>
    /// <param name="Format"></param>
    /// <returns></returns>
    public static string toDateMLastDay(this string Str, string Format = "")
    {
        string flag = null;
        if (string.IsNullOrEmpty(Format))
        {
            string DateFormat = MicroPublic.GetMicroInfo("DateFormat");  //返回系统设定的日期格式
            if (!string.IsNullOrEmpty(DateFormat))
                Format = DateFormat;
            else
                Format = "yyyy-MM-dd";
        }

        try
        {
            if (!string.IsNullOrEmpty(Str))
                flag = DateTime.Parse(Str.toDateTime().Year.ToString() + "-" + Str.toDateTime().Month.ToString() + "-1").AddMonths(1).AddDays(-1).toDateFormat(Format);
        }
        catch { }

        return flag;
    }

    /// <summary>
    /// 返回日期所在月份的最后一天
    /// </summary>
    /// <param name="Obj"></param>
    /// <param name="Format"></param>
    /// <returns></returns>
    public static string toDateMLastDay(this object Obj, string Format = "")
    {
        string flag = null;
        if (string.IsNullOrEmpty(Format))
        {
            string DateFormat = MicroPublic.GetMicroInfo("DateFormat");  //返回系统设定的日期格式
            if (!string.IsNullOrEmpty(DateFormat))
                Format = DateFormat;
            else
                Format = "yyyy-MM-dd";
        }

        try
        {
            if (!string.IsNullOrEmpty(Obj.ToString().Trim()))
                flag = DateTime.Parse((Obj.toDateTime().Year.ToString() + "-" + Obj.toDateTime().Month.ToString() + "-1")).AddMonths(1).AddDays(-1).toDateFormat(Format);
        }
        catch { }

        return flag;
    }

    /// <summary>
    /// 返回日期所在季度的第一天
    /// </summary>
    /// <param name="Str"></param>
    /// <param name="Format"></param>
    /// <returns></returns>
    public static string toDateQFirstDay(this string Str, string Format = "")
    {
        string flag = null;
        if (string.IsNullOrEmpty(Format))
        {
            string DateFormat = MicroPublic.GetMicroInfo("DateFormat");  //返回系统设定的日期格式
            if (!string.IsNullOrEmpty(DateFormat))
                Format = DateFormat;
            else
                Format = "yyyy-MM-dd";
        }

        try
        {
            if (!string.IsNullOrEmpty(Str))
                flag = Str.toDateTime(Format).AddMonths(0 - ((Str.toDateTime(Format).Month - 1) % 3)).AddDays(1 - Str.toDateTime(Format).Day).toDateFormat(Format);
        }
        catch { }

        return flag;
    }

    /// <summary>
    /// 返回日期所在季度的第一天
    /// </summary>
    /// <param name="Obj"></param>
    /// <param name="Format"></param>
    /// <returns></returns>
    public static string toDateQFirstDay(this object Obj, string Format = "")
    {
        string flag = null;
        if (string.IsNullOrEmpty(Format))
        {
            string DateFormat = MicroPublic.GetMicroInfo("DateFormat");  //返回系统设定的日期格式
            if (!string.IsNullOrEmpty(DateFormat))
                Format = DateFormat;
            else
                Format = "yyyy-MM-dd";
        }

        try
        {
            if (!string.IsNullOrEmpty(Obj.ToString().Trim()))
                flag = Obj.toDateTime(Format).AddMonths(0 - ((Obj.toDateTime(Format).Month - 1) % 3)).AddDays(1 - Obj.toDateTime(Format).Day).toDateFormat(Format);
        }
        catch { }

        return flag;
    }

    /// <summary>
    /// 返回日期所在季度的最后一天
    /// </summary>
    /// <param name="Str"></param>
    /// <param name="Format"></param>
    /// <returns></returns>
    public static string toDateQLastDay(this string Str, string Format = "")
    {
        string flag = null;
        if (string.IsNullOrEmpty(Format))
        {
            string DateFormat = MicroPublic.GetMicroInfo("DateFormat");  //返回系统设定的日期格式
            if (!string.IsNullOrEmpty(DateFormat))
                Format = DateFormat;
            else
                Format = "yyyy-MM-dd";
        }

        try
        {
            if (!string.IsNullOrEmpty(Str))
                flag = DateTime.Parse(Str.toDateTime(Format).AddMonths(3 - ((Str.toDateTime(Format).Month - 1) % 3)).ToString("yyyy-MM-01")).AddDays(-1).toDateFormat(Format);
        }
        catch { }

        return flag;
    }

    /// <summary>
    /// 返回日期所在季度的最后一天
    /// </summary>
    /// <param name="Obj"></param>
    /// <param name="Format"></param>
    /// <returns></returns>
    public static string toDateQLastDay(this object Obj, string Format = "")
    {
        string flag = null;
        if (string.IsNullOrEmpty(Format))
        {
            string DateFormat = MicroPublic.GetMicroInfo("DateFormat");  //返回系统设定的日期格式
            if (!string.IsNullOrEmpty(DateFormat))
                Format = DateFormat;
            else
                Format = "yyyy-MM-dd";
        }

        try
        {
            if (!string.IsNullOrEmpty(Obj.ToString().Trim()))
                flag = DateTime.Parse(Obj.toDateTime(Format).AddMonths(3 - ((Obj.toDateTime(Format).Month - 1) % 3)).ToString("yyyy-MM-01")).AddDays(-1).toDateFormat(Format);
        }
        catch { }

        return flag;
    }

    /// <summary>
    /// 传入一个日期或时间（字符串类型），返回该日期的开始时间 yyyy-MM-dd 00:00:00.000
    /// </summary>
    /// <param name="Date"></param>
    /// <returns></returns>
    public static string todayStartTime(this string Date)
    {
        string flag = Date.toDateFormat("yyyy-MM-dd")+" 00:00:00.000";
        return flag;
    }

    /// <summary>
    /// 传入一个日期或时间（对象类型），返回该日期的开始时间 yyyy-MM-dd 00:00:00.000
    /// </summary>
    /// <param name="Date"></param>
    /// <returns></returns>
    public static string todayStartTime(this object Date)
    {
        string flag = Date.toDateFormat("yyyy-MM-dd") + " 00:00:00.000";
        return flag;
    }

    /// <summary>
    /// 传入一个日期或时间（字符串类型），返回该日期的结束时间 yyyy-MM-dd 23:59:59.998
    /// </summary>
    /// <param name="Date"></param>
    /// <returns></returns>
    public static string todayEndTime(this string Date)
    {
        string flag = Date.toDateFormat("yyyy-MM-dd") + " 23:59:59.998";
        return flag;
    }

    /// <summary>
    /// 传入一个日期或时间（对象类型），返回该日期的结束时间 yyyy-MM-dd 23:59:59.998
    /// </summary>
    /// <param name="Date"></param>
    /// <returns></returns>
    public static string todayEndTime(this object Date)
    {
        string flag = Date.toDateFormat("yyyy-MM-dd") + " 23:59:59.998";
        return flag;
    }

    /// <summary>
    /// 【HR】根据输入的数值返回0或0.5，主要用于加班小时计算。如果>=0并且<30则返回0，否则如果>=30则返回0.5
    /// </summary>
    /// <param name="Obj"></param>
    /// <returns></returns>
    public static decimal toOTAddedTime(this object Obj)
    {
        decimal flag = 0;

        try
        {
            if (!string.IsNullOrEmpty(Obj.toStringTrim()))
            {
                flag = Obj.toStringTrim().toDecimal();
                if (flag >= 0 && flag < 30)
                    flag = 0;
                else if (flag >= 30)
                    flag = "0.5".toDecimal();
            }

        }
        catch { }


        return flag;
    }


    /// <summary>
    /// 【HR】根据输入的数值返回0或0.5，主要用于加班小时计算。如果>=0并且<30则返回0，否则如果>=30则返回0.5
    /// </summary>
    /// <param name="Str"></param>
    /// <returns></returns>
    public static decimal toOTAddedTime(this string Str)
    {
        decimal flag = 0;

        try
        {
            if (!string.IsNullOrEmpty(Str.toStringTrim()))
            {
                flag = Str.toStringTrim().toDecimal();
                if (flag >= 0 && flag < 30)
                    flag = 0;
                else if (flag >= 30)
                    flag = "0.5".toDecimal();
            }

        }
        catch { }


        return flag;
    }

    /// <summary>
    /// 【HR】根据传入的Decimal值进行用“.”分割，小数位小于0.5则小数位返回0，否则小数位返回0.5
    /// </summary>
    /// <param name="Obj"></param>
    /// <returns></returns>
    public static string toOTRound(this string Str)
    {
        string flag = string.Empty;

        try
        {
            if (!string.IsNullOrEmpty(Str))
            {
                flag = Str.toStringTrim();
                string s1 = (Str.toInt() / 60).ToString();
                string s2 = (Str.toInt() % 60).ToString();

                if (s2.toInt() < 30)
                    s2 = ".0";
                else
                    s2 = ".5";

                flag = s1 + s2;
            }

        }
        catch { }


        return flag;
    }

    /// <summary>
    /// 传入日期与当前时间作比较返回多久前
    /// </summary>
    /// <param name="Str"></param>
    /// <returns></returns>
    public static string toTimeAgo(this string Str)
    {
        string flag = string.Empty,
            StartDateTime = Str,
            EndDateTime = DateTime.Now.toDateFormat("yyyy-MM-dd HH:mm:ss");

        var getDateTimeDiff = MicroPublic.GetDateTimeDiff(StartDateTime, EndDateTime);

        decimal DecTotalDays = getDateTimeDiff.TotalDays.toDecimal(),
            DecTotalHours = getDateTimeDiff.TotalHours.toDecimal(),
            DecTotalMinutes = getDateTimeDiff.TotalMinutes.toDecimal(),
            DecTotalSeconds = getDateTimeDiff.TotalSeconds.toDecimal();

        //if (StartDateTime.toDateTime() == EndDateTime.toDateTime())
        //    flag = "今天";

        //else if (StartDateTime.toDateTime().AddDays(1) == EndDateTime.toDateTime())
        //    flag = "昨天";

        //else
        //{
        //if (DecTotalDays >= 30 && DecTotalDays < 90)
        //    flag = (DecTotalDays / 30).toDecimalInt32() + "个月前";

        //else if (DecTotalDays >= 7 && DecTotalDays < 30)
        //    flag = (DecTotalDays / 7).toDecimalInt32() + "周前";

        if (MicroPublic.GetMicroInfo("InfoTimeAgo").toBoolean())
        {
            if (DecTotalDays >= 1 && DecTotalDays < 8)
                flag = (DecTotalDays / 1).toDecimalInt32() + "天前";

            else if (DecTotalHours >= 1 && DecTotalHours < 24)
                flag = (DecTotalHours / 1).toDecimalInt32() + "小时前";

            else if (DecTotalMinutes > 3 && DecTotalMinutes < 60)
                flag = (DecTotalMinutes / 1).toDecimalInt32() + "分钟前";

            else if (DecTotalMinutes <= 3)
                flag = "刚刚";
            else
                flag = StartDateTime.toDateFormat();
        }
        else
            flag = StartDateTime.toDateFormat();
        //}

        return flag;

    }

}

