<%@ WebHandler Language="C#" Class="UserBasicInfo" %>

using System;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroPublicHelper;
using MicroUserHelper;
using Newtonsoft.Json.Linq;
using MicroAuthHelper;

public class UserBasicInfo : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    //****************
    //说明
    //根据表名返回表的内容
    //如父项和子项，主要菜单和子菜单等
    //****************

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();
        string flag = MicroPublic.GetMsg("ModifyFailed"),
                    Action = context.Server.UrlDecode(context.Request.Form["action"]),
                    MID = context.Server.UrlDecode(context.Request.Form["mid"]),
                    Fields = context.Server.UrlDecode(context.Request.Form["fields"]);

        if (Action == "modify" && !string.IsNullOrEmpty(Fields))
            flag = ModifyUserInfo(Fields);
     
        context.Response.Write(flag);
    }

    private string ModifyUserInfo(string Fields)
    {
        string flag = string.Empty;
        try
        {

            string ChineseName = string.Empty, EnglishName = string.Empty, EMail = string.Empty, Sex = string.Empty, WorkTel = string.Empty, WorkMobilePhone = string.Empty, UserDepts = string.Empty, IsSiteTips = string.Empty, IsMailTips = string.Empty, IsGlobalTips = string.Empty;
            Boolean  bitIsSiteTips = false, bitIsMailTips = false, bitIsGlobalTips = false;

            int UID = ((MicroUserInfo)HttpContext.Current.Session["UserInfo"]).UID;

            dynamic json = JToken.Parse(Fields) as dynamic;

            ChineseName = json["txtChineseName"];
            if (!string.IsNullOrEmpty(ChineseName))
                ChineseName = ChineseName.toJsonTrim();

            EnglishName = json["txtEnglishName"];
            if (!string.IsNullOrEmpty(EnglishName))
                EnglishName = EnglishName.toJsonTrim();

            EMail = json["txtEMail"];
            if (!string.IsNullOrEmpty(EMail))
                EMail = EMail.toJsonTrim();

            Sex = json["txtSex"];
            if (!string.IsNullOrEmpty(Sex))
                Sex = Sex.toJsonTrim();

            WorkTel = json["txtWorkTel"];
            if (!string.IsNullOrEmpty(WorkTel))
                WorkTel = WorkTel.toJsonTrim();

            WorkMobilePhone = json["txtWorkMobilePhone"];
            if (!string.IsNullOrEmpty(WorkMobilePhone))
                WorkMobilePhone = WorkMobilePhone.toJsonTrim();

            UserDepts = json["txtUserDepts"];
            UserDepts = string.IsNullOrEmpty(UserDepts) == true ? "0" : UserDepts.toJsonTrim();

            IsSiteTips = json["ckIsSiteTips"];
            if (!string.IsNullOrEmpty(IsSiteTips))
                IsSiteTips = IsSiteTips.toJsonTrim();
            bitIsSiteTips = IsSiteTips.toBoolean();

            IsMailTips = json["ckIsMailTips"];
            if (!string.IsNullOrEmpty(IsMailTips))
                IsMailTips = IsMailTips.toJsonTrim();
            bitIsMailTips = IsMailTips.toBoolean();

            IsGlobalTips = json["ckIsGlobalTips"];
            if (!string.IsNullOrEmpty(IsGlobalTips))
                IsGlobalTips = IsGlobalTips.toJsonTrim();
            bitIsGlobalTips = IsGlobalTips.toBoolean();

            string _sql3 = "update UserInfo set ChineseName = @ChineseName, EnglishName = @EnglishName, EMail = @EMail, Sex = @Sex, WorkTel = @WorkTel, WorkMobilePhone = @WorkMobilePhone, IsSiteTips = @IsSiteTips, IsMailTips = @IsMailTips, IsGlobalTips = @IsGlobalTips where UID=@UID";

            SqlParameter[] _sp3 = { new SqlParameter("@ChineseName", SqlDbType.NVarChar,50),
                new SqlParameter("@EnglishName", SqlDbType.NVarChar,50),
                new SqlParameter("@EMail", SqlDbType.VarChar,100),
                new SqlParameter("@Sex", SqlDbType.NVarChar,10),
                new SqlParameter("@WorkTel", SqlDbType.VarChar,30),
                new SqlParameter("@WorkMobilePhone", SqlDbType.VarChar,30),
                new SqlParameter("@IsSiteTips", SqlDbType.Bit),
                new SqlParameter("@IsMailTips", SqlDbType.Bit),
                new SqlParameter("@IsGlobalTips", SqlDbType.Bit),
                new SqlParameter("@UID", SqlDbType.Int),
                };
            _sp3[0].Value = ChineseName;
            _sp3[1].Value = EnglishName;
            _sp3[2].Value = EMail;
            _sp3[3].Value = Sex;
            _sp3[4].Value = WorkTel;
            _sp3[5].Value = WorkMobilePhone;
            _sp3[6].Value = bitIsSiteTips;
            _sp3[7].Value = bitIsMailTips;
            _sp3[8].Value = bitIsGlobalTips;
            _sp3[9].Value = UID;

            //更新基本信息，再更新所属部门
            if (MsSQLDbHelper.ExecuteSql(_sql3, _sp3) > 0)
            {
                ((MicroUserInfo)HttpContext.Current.Session["UserInfo"]).IsGlobalTips = bitIsGlobalTips;

                flag = MicroPublic.GetMsg("Modify");
                //更新所属部门
                string[] Arr = UserDepts.Split(',');
                if (Arr.Length > 0)
                {
                    //先删除数据库里已有而当前未选中的记录
                    string _sqlDel = "delete UserDepts where UID=@UID and DeptID not in (" + UserDepts + ") ";
                    SqlParameter[] _spDel = { new SqlParameter("@UID", SqlDbType.Int) };
                    _spDel[0].Value = UID;
                    MsSQLDbHelper.ExecuteSql(_sqlDel, _spDel);

                    //读取数据库记录
                    string _sql = "select * from UserDepts where UID=@UID ";
                    SqlParameter[] _sp = { new SqlParameter("@UID", SqlDbType.Int) };
                    _sp[0].Value = UID;
                    DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

                    //新建DataTable，用于存放已选中而数据库没有的记录
                    DataTable _dt2 = new DataTable();
                    _dt2.Columns.Add("UID", typeof(int));
                    _dt2.Columns.Add("DeptID", typeof(int));

                    //得到已选中而数据库没有的记录写入DataTable
                    for (int i = 0; i < Arr.Length; i++)
                    {
                        int ID = Arr[i].toInt();
                        DataRow[] _rows = _dt.Select("DeptID=" + ID);
                        if (_rows.Length == 0)
                        {
                            DataRow _dr2 = _dt2.NewRow();
                            _dr2["UID"] = UID;
                            _dr2["DeptID"] = ID;
                            _dt2.Rows.Add(_dr2);
                        }
                    }

                    //把记录写进数据库
                    if (_dt2.Rows.Count > 0)
                        MsSQLDbHelper.SqlBulkCopyInsert(_dt2, "UserDepts");
                }
            }
        }
        catch (Exception ex) { flag = ex.ToString(); }

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