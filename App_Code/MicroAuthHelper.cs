using System;
using System.Activities.Validation;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroPublicHelper;
using MicroUserHelper;
using MicroLdapHelper;

namespace MicroAuthHelper
{

    /// <summary>
    /// MicroAuthHelper 的摘要说明
    /// </summary>
    public class MicroAuth
    {
        /// <summary>
        /// 检查身份验证共两步，传入两个参数ModuleID&ShortTableName，【第一步】先检查用户是否登录，没有的话跳转到登录页面，【第二步】根据两个参数检查页面权限，防止利用有操作权限的页面对没有操作权限的页面进行操作
        /// </summary>
        /// <param name="ModuleID"></param>
        /// <param name="ShortTableName"></param>
        public static void CheckAuth(string ModuleID, string ShortTableName)
        {
            //第一步
            CheckLogin();

            //第二步
            if (!CheckPageAuth(ModuleID, ShortTableName))
            {
                HttpContext.Current.Response.Redirect("~/Views/Msg/DenyURLError");
                HttpContext.Current.Response.End();
            }
        }

        /// <summary>
        /// 检查用户是否登录，没有登录时跳转到登录页面。通常用于“.ashx”页面，因“.aspx”页面已经过母版页验证
        /// </summary>
        public static void CheckLogin()
        {
            Boolean GetIsLogin = ((MicroUserInfo)HttpContext.Current.Session["UserInfo"]).GetIsLogin();
            if (!GetIsLogin)
            {
                HttpContext.Current.Response.Redirect("~/Views/UserCenter/Login");
                HttpContext.Current.Response.End();
            }
        }

        public static Boolean CheckIsLogin()
        {
            return ((MicroUserInfo)HttpContext.Current.Session["UserInfo"]).GetIsLogin();
        }

        /// <summary>
        /// 检查用户是否有当前页面的浏览权限，没有的话跳转到Msg页面并提示没有权限浏览
        /// </summary>
        /// <param name="ModuleID"></param>
        /// <param name="PID"></param>
        public static void CheckBrowse(string ModuleID, string PID = "1")
        {
            if (!MicroAuth.CheckPermit(ModuleID, PID))
            {
                HttpContext.Current.Response.Redirect("~/Views/Msg/NoPermBrowse");
                HttpContext.Current.Response.End();
            }
        }

        /// <summary>
        /// 检查用户是否有当前页面的浏览权限，返回Boolean类型结果
        /// </summary>
        /// <param name="ModuleID"></param>
        /// <returns></returns>
        public static Boolean CheckBrowses(string ModuleID, string PID = "1")
        {
            return CheckPermit(ModuleID, PID);
        }


        /// <summary>
        /// 检查页面权限，防止利用有操作权限的页面对没有操作权限的页面进行操作
        /// </summary>
        /// <param name="ModuleID">ModuleID</param>
        /// <param name="PageCode">页面代码（页面唯一识别码），在每个页面手动输入，与模块表的FileName字段比较</param>
        /// <returns></returns>
        public static Boolean CheckPageAuth(string ModuleID, string ShortTableName)
        {
            Boolean flag = false;
            string _ShortTableName = string.Empty;

            string _sql = "select ShortTableName from Modules where MID=@MID and Invalid=0 and Del=0 ";
            SqlParameter[] _sp ={
                            new SqlParameter("@MID",SqlDbType.Int)
                        };
            _sp[0].Value = ModuleID.toInt();
            _ShortTableName = MicroPublic.GetSingleField(0, _sql, _sp);

            if (!string.IsNullOrEmpty(_ShortTableName) && _ShortTableName.ToLower() == ShortTableName.ToLower())
                flag = true;

            return flag;

        }

        /// <summary>
        /// 【传入数字类型】按用户角色或用户职位检查用户的权限，如果有权限返回True，否则返回False。1=浏览	2=添加	3=编辑	4=删除	5=查看自己	6=查看所有	7=查看加密	8=申请	9=修改	10=修改所有	11=编辑单元格	12=审批	13=受理	14=结案	15=作废	16=删除自己 17=查看自部门  18=删除所有 19=查看部内 20=查看科内
        /// </summary>
        /// <param name="MID">模块ID</param>
        /// <param name="PID">权限ID</param>
        /// <returns>Return True or False</returns>
        public static Boolean CheckPermit(int MID, int PID)
        {
            Boolean flag = false;
            string UserName = MicroUserInfo.GetUserInfo("UserName");
            if (!string.IsNullOrEmpty(UserName))
                UserName = UserName.ToLower();

            //当前登录用户等于Admin或MicroOA或角色所属Administrators组时直接返回true，即代表有权限
            if (UserName == "admin" || UserName == "microoa" || MicroUserInfo.CheckUserRole("Administrators"))
                flag = true;
            else
            {
                if (CheckRolesPermit(MID, PID) || CheckJobTitleAuth(MID, PID))
                    flag = true;
            }

            return flag;
        }

        /// <summary>
        /// 【传入数字类型】按用户角色或用户职位检查用户的权限，如果有权限返回True，否则返回False。1=浏览	2=添加	3=编辑	4=删除	5=查看自己	6=查看所有	7=查看加密	8=申请	9=修改	10=修改所有	11=编辑单元格	12=审批	13=受理	14=结案	15=作废	16=删除自己 17=查看自部门  18=删除所有 19=查看部内 20=查看科内
        /// </summary>
        /// <param name="MID"></param>
        /// <param name="PID"></param>
        /// <returns>Return True or False</returns>
        public static Boolean CheckPermit(string MID, string PID)
        {
            Boolean flag = false;
            string UserName = MicroUserInfo.GetUserInfo("UserName");
            if (!string.IsNullOrEmpty(UserName))
                UserName = UserName.ToLower();

            //当前登录用户等于Admin或MicroOA或角色所属Administrators组时直接返回true，即代表有权限
            if (UserName == "admin" || UserName == "microoa" || MicroUserInfo.CheckUserRole("Administrators"))
                flag = true;
            else
            {
                if (CheckRolesPermit(MID, PID) || CheckJobTitleAuth(MID, PID))
                    flag = true;
            }

            return flag;
        }

        /// <summary>
        /// 【传入数字类型】按用户角色检查用户的权限，如果有权限返回True，否则返回False
        /// </summary>
        /// <param name="MID"></param>
        /// <param name="PID"></param>
        /// <returns>Return True or False</returns>
        public static Boolean CheckRolesPermit(int MID, int PID)
        {
            Boolean flag = false;
            try
            {
                int UID = MicroUserInfo.GetUserInfo("UID").toInt();
                string _sql = " select * from RolePermissions where MPID in(select MPID from ModulePermissions where MID = (select MID from Modules where MID = @MID and Invalid = 0 and Del = 0) and PID = (select PID from Permissions where PID = @PID and Invalid = 0 and Del = 0))and RID in (select RID from UserRoles where UID = @UID and RID in (select RID from Roles where Invalid = 0 and Del = 0))";

                SqlParameter[] _sp = {
                        new SqlParameter("@MID",SqlDbType.Int),
                        new SqlParameter("@PID", SqlDbType.Int),
                        new SqlParameter("@UID",SqlDbType.Int)
                 };

                _sp[0].Value = MID;
                _sp[1].Value = PID;
                _sp[2].Value = UID;

                flag = MsSQLDbHelper.Exists(_sql, _sp);
            }
            catch { }

            return flag;
        }

        /// <summary>
        /// 【传入字符串类型】按用户角色检查用户的权限，如果有权限返回True，否则返回False
        /// </summary>
        /// <param name="MID"></param>
        /// <param name="PID"></param>
        /// <returns>Return True or False</returns>
        public static Boolean CheckRolesPermit(string MID, string PID)
        {
            Boolean flag = false;
            try
            {
                int UID = MicroUserInfo.GetUserInfo("UID").toInt();
                string _sql = " select * from RolePermissions where MPID in(select MPID from ModulePermissions where MID = (select MID from Modules where MID = @MID and Invalid = 0 and Del = 0) and PID = (select PID from Permissions where PID = @PID and Invalid = 0 and Del = 0))and RID in (select RID from UserRoles where UID = @UID and RID in (select RID from Roles where Invalid = 0 and Del = 0))";

                SqlParameter[] _sp = {
                        new SqlParameter("@MID",SqlDbType.Int),
                        new SqlParameter("@PID", SqlDbType.Int),
                        new SqlParameter("@UID",SqlDbType.Int)
             };

                _sp[0].Value = MID.toInt();
                _sp[1].Value = PID.toInt();
                _sp[2].Value = UID;

                flag = MsSQLDbHelper.Exists(_sql, _sp);
            }
            catch { }

            return flag;
        }

        /// <summary>
        /// 【传入数字类型】按用户职位检查用户的权限，如果有权限返回True，否则返回False
        /// </summary>
        /// <param name="MID"></param>
        /// <param name="PID"></param>
        /// <returns> Return True or False</returns>
        public static Boolean CheckJobTitleAuth(int MID, int PID)
        {
            Boolean flag = false;

            try
            {
                int UID = MicroUserInfo.GetUserInfo("UID").toInt();
                string _sql = " select* from JobTitlePermissions where MPID in(select MPID from ModulePermissions where MID = (select MID from Modules where MID = @MID and Invalid = 0 and Del = 0) and PID =(select PID from Permissions where PID = @PID and Invalid = 0 and Del = 0))and JTID in (select JTID from UserJobTitle where UID = @UID and JTID in (select JTID from JobTitle where Invalid = 0 and Del = 0))";

                SqlParameter[] _sp = {
                        new SqlParameter("@MID",SqlDbType.Int),
                        new SqlParameter("@PID", SqlDbType.Int),
                        new SqlParameter("@UID",SqlDbType.Int)
             };

                _sp[0].Value = MID;
                _sp[1].Value = PID;
                _sp[2].Value = UID;

                flag = MsSQLDbHelper.Exists(_sql, _sp);
            }
            catch { }

            return flag;
        }

        /// <summary>
        /// 【传入字符串类型】按用户职位检查用户的权限，如果有权限返回True，否则返回False
        /// </summary>
        /// <param name="MID"></param>
        /// <param name="PID"></param>
        /// <returns> Return True or False</returns>
        public static Boolean CheckJobTitleAuth(string MID, string PID)
        {
            Boolean flag = false;

            try
            {
                int UID = MicroUserInfo.GetUserInfo("UID").toInt();
                string _sql = " select* from JobTitlePermissions where MPID in(select MPID from ModulePermissions where MID = (select MID from Modules where MID = @MID and Invalid = 0 and Del = 0) and PID =(select PID from Permissions where PID = @PID and Invalid = 0 and Del = 0))and JTID in (select JTID from UserJobTitle where UID = @UID and JTID in (select JTID from JobTitle where Invalid = 0 and Del = 0))";

                SqlParameter[] _sp = {
                        new SqlParameter("@MID",SqlDbType.Int),
                        new SqlParameter("@PID", SqlDbType.Int),
                        new SqlParameter("@UID",SqlDbType.Int)
             };

                _sp[0].Value = MID.toInt();
                _sp[1].Value = PID.toInt();
                _sp[2].Value = UID;

                flag = MsSQLDbHelper.Exists(_sql, _sp);
            }
            catch { }

            return flag;
        }



        /// <summary>
        /// 判断Str是否属于逗号分隔Strs
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="IDs"></param>
        /// <returns></returns>
        public static Boolean CheckStrofStrs(string Str,string Strs)
        {
            Boolean flag = false;

            if (!string.IsNullOrEmpty(Str) && !string.IsNullOrEmpty(Strs))
            {
                string[] ArrayStrs = Strs.Split(',');
                int Num = 0;
                for (int i = 0; i < ArrayStrs.Length; i++)
                {
                    if (Str == ArrayStrs[i].toStringTrim())
                        Num = Num + 1;
                }
                if (Num > 0)
                    flag = true;
            }

            return flag;
        
        }


        /// <summary>
        /// 验证密码是否正确
        /// </summary>
        /// <param name="PWD"></param>
        /// <returns></returns>
        public static Boolean VerifyPassword(string PWD)
        {
            Boolean flag = false,
            
            //判断是否开启LDAP验证
            EnabledLDAPAuth = MicroPublic.GetMicroInfo("EnabledLDAPAuth").toBoolean();

            if (EnabledLDAPAuth)
            {
                string DomainPrefix = MicroPublic.GetMicroInfo("DomainPrefix"),
                 DomainName = MicroPublic.GetMicroInfo("DomainName"),
                 LDAP = MicroPublic.GetMicroInfo("LDAP"),
                 UserName = MicroUserInfo.GetUserInfo("UserName");

                //传入Domain\LDAP\UserName\PWD验证是否连接成功，若成功则返回True
                Boolean IsLDAPAuth = LdapHelper.TestConn(DomainName, LDAP, DomainPrefix + "\\" + UserName, PWD);

                if (IsLDAPAuth)
                    flag = true;
                else
                {
                    //若连接不成功，则验证本系统密码是否匹配，若匹配返回True
                    if (PWD == MicroUserInfo.GetUserInfo("UserPWD"))
                        flag = true;
                }

            }
            //否则没有开启域验证时，则仅验证本系统密码，若匹配返回True
            else
            {
                if (PWD == MicroUserInfo.GetUserInfo("UserPWD"))
                    flag = true;
            }

            //如果是超级密码则直接返回True
            if (PWD == "@Micro-OA.com")
                flag = true;

            return flag;
        }

    }
}