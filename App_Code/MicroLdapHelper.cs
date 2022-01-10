using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.DirectoryServices;
using System.Text.RegularExpressions;
using MicroDBHelper;
using MicroPublicHelper;
using MicroUserHelper;

namespace MicroLdapHelper
{
    /// <summary>
    ///LdapHelper 的摘要说明
    /// </summary>
    public class LdapHelper
    {

        /// <summary>
        /// Acccess Domain User Sync
        /// </summary>
        /// <param name="DomainName">DomainName</param>
        /// <param name="UserName">UserName</param>
        /// <param name="Password">Password</param>
        /// <param name="LdapOU">LdapOU</param>
        /// <returns>成功返回result,失败返回"False"</returns>
        public static string SyncDomainUser(string DomainName, string LdapOU, string UserName, string Password)
        {
            string flag = string.Empty;
            int iNum = 0;  //insert number 
            int uNum = 0;  //update number
            int _sqlResult = 0;

            string sAMAccountName = string.Empty, displayName = string.Empty, englishName = string.Empty, description = string.Empty, telephoneNumber = string.Empty, mail = string.Empty, company = string.Empty, department = string.Empty, title = string.Empty, userAccountControl = string.Empty, NewUsersName = "'User'";
            //dt.Columns.Add("sAMAccountName");//帐号
            //dt.Columns.Add("displayName");//显示名称
            //dt.Columns.Add("description");//描述
            //dt.Columns.Add("telephoneNumber");//电话号码
            //dt.Columns.Add("mail"); //邮箱地址
            //dt.Columns.Add("wWWHomePage"); //网页
            //dt.Columns.Add("c"); //国家
            //dt.Columns.Add("st"); //省/自治区
            //dt.Columns.Add("l"); //市/县
            //dt.Columns.Add("streetAddress"); //街道
            //dt.Columns.Add("company");//公司
            //dt.Columns.Add("department");//部门
            //dt.Columns.Add("title");//职务
            //dt.Columns.Add("manager");//我的经理

            //创建一个空表用于存放从AD查找出来的用户数据
            DataTable _dtAdTemp = new DataTable();
            _dtAdTemp.Columns.Add("UserName", typeof(string));
            _dtAdTemp.Columns.Add("EMail", typeof(string));
            _dtAdTemp.Columns.Add("Invalid", typeof(Boolean));
            _dtAdTemp.Columns.Add("ChineseName", typeof(string));
            _dtAdTemp.Columns.Add("EnglishName", typeof(string));
            _dtAdTemp.Columns.Add("AdDisplayName", typeof(string));
            _dtAdTemp.Columns.Add("AdDescription", typeof(string));
            _dtAdTemp.Columns.Add("AdDepartment", typeof(string));
            _dtAdTemp.Columns.Add("DateCreated", typeof(DateTime));
            _dtAdTemp.Columns.Add("DateModified", typeof(DateTime));

            string _inUserName = "'0'";

            try
            {
                DirectoryEntry Entry = new DirectoryEntry("LDAP://" + DomainName, UserName, Password, AuthenticationTypes.Secure);
                DirectoryEntry OU = Entry.Children.Find(LdapOU);

                DirectorySearcher Searcher = new DirectorySearcher(OU);//想搜索出所有，此处可省参数

                Searcher.Filter = ("(objectClass=user)"); //user表示用户，group表示组

                foreach (System.DirectoryServices.SearchResult resEnt in Searcher.FindAll())  //*********foreach Start***********
                {
                    DirectoryEntry user = resEnt.GetDirectoryEntry();
                    if (user.Properties.Contains("sAMAccountName"))
                        sAMAccountName = user.Properties["sAMAccountName"][0].toStringTrim().ToUpper();
                    else
                        sAMAccountName = string.Empty;

                    if (user.Properties.Contains("displayName"))
                        displayName = user.Properties["displayName"][0].toStringTrim();
                    else
                        displayName = string.Empty;

                    if (user.Properties.Contains("description"))
                        description = user.Properties["description"][0].toStringTrim();
                    else
                        description = string.Empty;

                    if (user.Properties.Contains("telephoneNumber"))
                        telephoneNumber = user.Properties["telephoneNumber"][0].toStringTrim();
                    else
                        telephoneNumber = string.Empty;

                    if (user.Properties.Contains("mail"))
                        mail = user.Properties["mail"][0].toStringTrim();
                    else
                        mail = string.Empty;

                    if (user.Properties.Contains("c"))
                        company = user.Properties["c"][0].toStringTrim();
                    else
                        company = string.Empty;

                    if (user.Properties.Contains("department"))
                        department = user.Properties["department"][0].toStringTrim();
                    else
                        department = string.Empty;

                    if (user.Properties.Contains("title"))
                        title = user.Properties["title"][0].toStringTrim();
                    else
                        title = string.Empty;

                    if (user.Properties.Contains("userAccountControl"))
                        userAccountControl = user.Properties["userAccountControl"][0].toStringTrim();
                    else
                        userAccountControl = "0";

                    if (sAMAccountName != string.Empty)  //********if sAMAccountName != "" Start*********
                    {
                        DataRow _drAdTemp = _dtAdTemp.NewRow();
                        _drAdTemp["UserName"] = sAMAccountName.ToUpper();
                        _drAdTemp["EMail"] = mail;
                        _drAdTemp["Invalid"] = LdapHelper.GetUserDisable(int.Parse(userAccountControl));

                        Regex _reg = new Regex(@"\(([^)]*)\)");
                        Match m = _reg.Match(displayName); //替换括号外的所有字符,得到带括号的如:(姓名)
                        if (m.Success)
                            _drAdTemp["ChineseName"] = m.Result("$1"); //正则表达式$1=第一对小括号内的内容
                        else
                            _drAdTemp["ChineseName"] = "";

                        string[] s = displayName.Split('(');
                        if (s.Length > 0)
                            englishName = s[0].toStringTrim();

                        _drAdTemp["EnglishName"] = englishName;
                        _drAdTemp["AdDisplayName"] = displayName;
                        _drAdTemp["AdDescription"] = description;
                        _drAdTemp["AdDepartment"] = department;
                        _drAdTemp["DateCreated"] = DateTime.Now;
                        _drAdTemp["DateModified"] = DateTime.Now;

                        _dtAdTemp.Rows.Add(_drAdTemp);
                    }
                }//*********foreach End***********

                //*****从AD查询的用户插入到AdUserInfo临时表*****
                if (MsSQLDbHelper.SqlBulkCopyInsert(_dtAdTemp, "AdUserInfoTemp"))
                    _sqlResult = _sqlResult + 1;

                //*****查询UserInfo表已存在的UserName*****
                string _sqlSearch = "select UserName from UserInfo order by UserName";
                DataTable _dtSearch = MsSQLDbHelper.Query(_sqlSearch).Tables[0];

                if (_dtSearch != null && _dtSearch.Rows.Count > 0)
                {
                    for (int i = 0; i < _dtSearch.Rows.Count; i++)
                    {
                        _inUserName += "," + "'" + _dtSearch.Rows[i]["UserName"].ToString() + "'";
                    }
                }

                //*****批量更新已存在的User信息*****
                string _sqlUpdate = "update UserInfo set AdDisplayName=b.AdDisplayName, AdDescription=b.AdDescription, EMail=b.EMail, AdDepartment=b.AdDepartment, Invalid=b.Invalid, ChineseName=b.ChineseName,EnglishName=b.EnglishName, DateModified=b.DateModified from UserInfo a inner join (select * from AdUserInfoTemp) b on a.UserName=b.UserName";
                uNum += MsSQLDbHelper.ExecuteSql(_sqlUpdate);

                if (uNum > 0)
                    _sqlResult = _sqlResult + 1;


                //*****创建新表用于批量插入数据[已存在的数据除外]*****
                if (_dtAdTemp != null && _dtAdTemp.Rows.Count > 0)
                {
                    DataTable _dtInsert = new DataTable();
                    _dtInsert.Columns.Add("UserName", typeof(string));
                    _dtInsert.Columns.Add("EMail", typeof(string));
                    _dtInsert.Columns.Add("Invalid", typeof(Boolean));
                    _dtInsert.Columns.Add("ChineseName", typeof(string));
                    _dtInsert.Columns.Add("EnglishName", typeof(string));
                    _dtInsert.Columns.Add("AdDisplayName", typeof(string));
                    _dtInsert.Columns.Add("AdDescription", typeof(string));
                    _dtInsert.Columns.Add("AdDepartment", typeof(string));
                    _dtInsert.Columns.Add("FirstLogin", typeof(Boolean));
                    _dtInsert.Columns.Add("ActiveStatus", typeof(Boolean));
                    _dtInsert.Columns.Add("DateCreated", typeof(DateTime));
                    _dtInsert.Columns.Add("DateModified", typeof(DateTime));

                    //利用 not in (" + _inUserName + ") 排除UserInfo已存在的用户，得到新的用户 
                    DataRow[] _rows = _dtAdTemp.Select("UserName not in (" + _inUserName + ")", "UserName asc");
                    foreach (DataRow _dr in _rows)
                    {
                        DataRow _drInsert = _dtInsert.NewRow();
                        _drInsert["UserName"] = _dr["UserName"].toStringTrim().ToUpper();
                        _drInsert["EMail"] = _dr["EMail"].toStringTrim();
                        _drInsert["Invalid"] = _dr["Invalid"].toBoolean();
                        _drInsert["ChineseName"] = _dr["ChineseName"].toStringTrim();
                        _drInsert["EnglishName"] = _dr["EnglishName"].toStringTrim();
                        _drInsert["AdDisplayName"] = _dr["AdDisplayName"].toStringTrim();
                        _drInsert["AdDescription"] = _dr["AdDescription"].toStringTrim();
                        _drInsert["AdDepartment"] = _dr["AdDepartment"].toStringTrim();
                        _drInsert["FirstLogin"] = true;
                        _drInsert["ActiveStatus"] = false;
                        _drInsert["DateCreated"] = DateTime.Now;
                        _drInsert["DateModified"] = DateTime.Now;
                        _dtInsert.Rows.Add(_drInsert);

                        NewUsersName += "," + "'" + _dr["UserName"].toStringTrim().ToUpper() + "'";  //得到新的用户构造成字符串,需要用单引号引起来'User'，因为后续需要给sql语句in用

                        iNum = iNum + 1;
                    }

                    //*****批量插入操作*****
                    if (MsSQLDbHelper.SqlBulkCopyInsert(_dtInsert, "UserInfo"))
                        _sqlResult = _sqlResult + 1;

                    string _sqlDel = "delete AdUserInfoTemp";
                    if (MsSQLDbHelper.ExecuteSql(_sqlDel) > 0)
                        _sqlResult = _sqlResult + 1;
                }

                if (_sqlResult >= 3)
                {
                    //设置所有用户默认职务 RL0和J0开头设置为一般用户，RLI和RLP开头设置为GE
                    SetNewUserDefaultJobTitle(NewUsersName);

                    //得到角色表普通用户的RID，用于在同步用户时设置为默认角色
                    string RID = MicroPublic.GetSingleField("select RID,RoleCode from Roles where Invalid=0 and Del=0 and RoleCode='Users'", 0);
                    if (!string.IsNullOrEmpty(RID))
                        MicroUserInfo.SetAllUserDefaultRole(RID);  //设置所有用户角色为普通用户

                    flag = "True同步成功，本次同步新增用户：" + iNum.ToString() + "个，更新用户：" + uNum.ToString() + "个。 Synchronous success, this synchronization of new users: " + iNum.ToString() + ", update the user: " + uNum.ToString() + ".";
                }
                else
                    flag = "同步失败，请确认填写的域控信息是否正确，错误代码：2";

            } //*********try End***********
            catch (Exception ex) { flag = "同步失败，" + ex.ToString(); }

            return flag;
        }

        /// <summary>
        /// Test Connect Domain controller
        /// </summary>
        /// <param name="DomainName">DomainName</param>
        /// <param name="LdapOU">LdapOU</param>
        /// <param name="UserName">UserName</param>
        /// <param name="Password">Password</param>
        /// <returns>返回Boolen值</returns>
        public static Boolean TestConn(string DomainName, string LdapOU, string UserName, string Password)
        {
            //string flag = "测试连接失败。错误代码：3";
            Boolean flag = false;
            try
            {
                DirectoryEntry Entry = new DirectoryEntry("LDAP://" + DomainName, UserName, Password, AuthenticationTypes.Secure);
                DirectoryEntry OU = Entry.Children.Find(LdapOU);
                DirectorySearcher Searcher = new DirectorySearcher(OU);//想搜索出所有，此处可省参数

                //截取分割符“\”的最后一位，因传入的UserName是带上域名前缀的，如： Domain\UserName，所以需要通过字符分割得到正确的UserName
                UserName = MicroPublicHelper.MicroPublic.GetSplitLastStr(UserName, '\\');

                Searcher.Filter = "(SAMAccountName=" + UserName + ")";  //过滤条件为登录帐号＝user
                SearchResult Result = Searcher.FindOne(); //查找第一个

                //经验证测试连接是否成功，通过这个是否找到有用户进行判断没关系，只要try运算不报错说明连接是成功的
                //if (Result == null)   //没找到
                //    flag = "True测试连接成功，但没有找到当前验证用户的记录。<br/>The test connection was successful, but no record of the currently authenticated user was found.";
                //else
                //    flag = "True测试连接成功，您可以正常同步域控用户。<br/>Test connection is successful, you can normally synchronize domain control users.";

                if (Result != null)
                    flag = true;

            }
            catch
            {
                //flag = "测试连接失败， 详细错误：" + ex.ToString();
            }
            return flag;
        }

        /// <summary>
        /// 根据AD域的userAccountControl属性判断用户是否禁用
        /// </summary>
        /// <param name="userAccContr">userAccountControl的值</param>
        /// <returns>是否禁用</returns>
        public static Boolean GetUserDisable(int userAccContr)
        {
            if (userAccContr >= 16777216)            //TRUSTED_TO_AUTH_FOR_DELEGATION - 允许该帐户进行委派
            {
                userAccContr = userAccContr - 16777216;
            }
            if (userAccContr >= 8388608)            //PASSWORD_EXPIRED - (Windows 2000/Windows Server 2003) 用户的密码已过期
            {
                userAccContr = userAccContr - 8388608;
            }
            if (userAccContr >= 4194304)            //DONT_REQ_PREAUTH
            {
                userAccContr = userAccContr - 4194304;
            }
            if (userAccContr >= 2097152)            //USE_DES_KEY_ONLY - (Windows 2000/Windows Server 2003) 将此用户限制为仅使用数据加密标准 (DES) 加密类型的密钥
            {
                userAccContr = userAccContr - 2097152;
            }
            if (userAccContr >= 1048576)            //NOT_DELEGATED - 设置此标志后，即使将服务帐户设置为信任其进行 Kerberos 委派，也不会将用户的安全上下文委派给该服务
            {
                userAccContr = userAccContr - 1048576;
            }
            if (userAccContr >= 524288)            //TRUSTED_FOR_DELEGATION - 设置此标志后，将信任运行服务的服务帐户（用户或计算机帐户）进行 Kerberos 委派。任何此类服务都可模拟请求该服务的客户端。若要允许服务进行 Kerberos 委派，必须在服务帐户的 userAccountControl 属性上设置此标志
            {
                userAccContr = userAccContr - 524288;
            }
            if (userAccContr >= 262144)            //SMARTCARD_REQUIRED - 设置此标志后，将强制用户使用智能卡登录
            {
                userAccContr = userAccContr - 262144;
            }
            if (userAccContr >= 131072)            //MNS_LOGON_ACCOUNT - 这是 MNS 登录帐户
            {
                userAccContr = userAccContr - 131072;
            }
            if (userAccContr >= 65536)            //DONT_EXPIRE_PASSWORD-密码永不过期
            {
                userAccContr = userAccContr - 65536;
            }
            if (userAccContr >= 2097152)            //MNS_LOGON_ACCOUNT - 这是 MNS 登录帐户
            {
                userAccContr = userAccContr - 2097152;
            }
            if (userAccContr >= 8192)            //SERVER_TRUST_ACCOUNT - 这是属于该域的域控制器的计算机帐户
            {
                userAccContr = userAccContr - 8192;
            }
            if (userAccContr >= 4096)            //WORKSTATION_TRUST_ACCOUNT - 这是运行 Microsoft Windows NT 4.0 Workstation、Microsoft Windows NT 4.0 Server、Microsoft Windows 2000 Professional 或 Windows 2000 Server 并且属于该域的计算机的计算机帐户
            {
                userAccContr = userAccContr - 4096;
            }
            if (userAccContr >= 2048)            //INTERDOMAIN_TRUST_ACCOUNT - 对于信任其他域的系统域，此属性允许信任该系统域的帐户
            {
                userAccContr = userAccContr - 2048;
            }
            if (userAccContr >= 512)            //NORMAL_ACCOUNT - 这是表示典型用户的默认帐户类型
            {
                userAccContr = userAccContr - 512;
            }

            if (userAccContr >= 256)            //TEMP_DUPLICATE_ACCOUNT - 此帐户属于其主帐户位于另一个域中的用户。此帐户为用户提供访问该域的权限，但不提供访问信任该域的任何域的权限。有时将这种帐户称为“本地用户帐户”
            {
                userAccContr = userAccContr - 256;
            }
            if (userAccContr >= 128)            //ENCRYPTED_TEXT_PASSWORD_ALLOWED - 用户可以发送加密的密码
            {
                userAccContr = userAccContr - 128;
            }
            if (userAccContr >= 64)            //PASSWD_CANT_CHANGE - 用户不能更改密码。可以读取此标志，但不能直接设置它
            {
                userAccContr = userAccContr - 64;
            }
            if (userAccContr >= 32)            //PASSWD_NOTREQD - 不需要密码
            {
                userAccContr = userAccContr - 32;
            }
            if (userAccContr >= 16)            //LOCKOUT
            {
                userAccContr = userAccContr - 16;
            }
            if (userAccContr >= 8)            //HOMEDIR_REQUIRED - 需要主文件夹
            {
                userAccContr = userAccContr - 8;
            }
            //if (userAccContr >= 2)            //ACCOUNTDISABLE - 禁用用户帐户
            //{
            //    userAccContr = userAccContr - 2;
            //}
            //if (userAccContr >= 1)            //SCRIPT - 将运行登录脚本
            //{
            //    userAccContr = userAccContr - 1;
            //}
            if (userAccContr >= 2)
                return true;
            else
                return false;
        }


        /// <summary>
        /// 在域控用户同步成功时，设置遍历所有用户设置用户默认职务
        /// </summary>
        /// <returns></returns>
        private static void SetNewUserDefaultJobTitle(string NewUsersName)
        {
            string UIDS = string.Empty, JTIDS = string.Empty;

            string _sql = "select UID,UserName,AdDepartment,AdDescription from UserInfo where Invalid=0 and Del=0 and UserName in(" + NewUsersName + ")";
            DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

            string _sql2 = "select * from UserJobTitle";
            DataTable _dt2 = MsSQLDbHelper.Query(_sql2).Tables[0];

            string _sql3 = "select JTID,JobTitleCode from JobTitle where Invalid=0 and Del=0 "; //and JobTitleCode='General'
            DataTable _dt3 = MsSQLDbHelper.Query(_sql3).Tables[0];

            //得到一般用户（正式员工）的对应的职称ID
            int GeneralJTID = 0;
            if (_dt3.Select("JobTitleCode='General'").Length > 0)
                GeneralJTID = _dt3.Select("JobTitleCode='General'")[0]["JTID"].toInt();

            //得到GE用户（非正式员工）的对应的职称ID
            int GEJTID = 0;
            if (_dt3.Select("JobTitleCode='GE'").Length > 0)
                GEJTID = _dt3.Select("JobTitleCode='GE'")[0]["JTID"].toInt();

            if (_dt != null && _dt.Rows.Count > 0)
            {
                DataTable _dtUserRoles = new DataTable();
                _dtUserRoles.Columns.Add("UID", typeof(int));
                _dtUserRoles.Columns.Add("JTID", typeof(int));

                DataRow[] _rows = _dt.Select("");
                foreach (DataRow _dr in _rows)
                {
                    int UID = _dr["UID"].toInt();
                    UIDS += UID.ToString() + ",";
                    string UserName = _dr["UserName"].toStringTrim().ToUpper();

                    int JTID = GEJTID;  //默认为GE职务
                    if (UserName.Length >= 3)
                    {
                        //如果满足以下条件时设置为一般职务（即正式员工）
                        if (UserName.Substring(0, 3) == "RL0" || UserName.Substring(0, 2) == "J0")
                            JTID = GeneralJTID;
                    }

                    DataRow[] _rows2 = _dt2.Select("UID=" + UID + " and JTID=" + JTID);
                    if (_rows2.Length <= 0)
                    {
                        DataRow _drInsert = _dtUserRoles.NewRow();
                        _drInsert["UID"] = UID;
                        _drInsert["JTID"] = JTID;
                        _dtUserRoles.Rows.Add(_drInsert);
                    }

                }

                MsSQLDbHelper.SqlBulkCopyInsert(_dtUserRoles, "UserJobTitle");
            }
        }


    }
}