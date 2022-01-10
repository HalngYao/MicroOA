using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroUserHelper;
using MicroAuthHelper;
using MicroPublicHelper;

public partial class Views_UserCenter_UserBasicInfo : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            GetUserInfo();

        MicroAuth.CheckLogin();
        txtMaxFileUpload.Value = MicroPublic.GetMicroInfo("MaxFileUpload");
        txtUploadFileType.Value = MicroPublic.GetMicroInfo("UploadFileType");
    }

    private void GetUserInfo()
    {
        string UID = ((MicroUserInfo)Session["UserInfo"]).UID.ToString(); string UserDeptsIDStr = string.Empty;
        txtUID.Value = UID;

        string _sql = "select UserName,EMail,ChineseName,EngLishName,Sex,WorkMobilePhone,WorkTel,Avatar,IsSiteTips,IsMailTips,IsGlobalTips from UserInfo where UID=@UID";
        SqlParameter[] _sp = { new SqlParameter("@UID", SqlDbType.Int) };

        _sp[0].Value = ((MicroUserInfo)Session["UserInfo"]).UID;

        DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

        if (_dt != null && _dt.Rows.Count > 0)
        {
            txtUserName.Value = _dt.Rows[0]["UserName"].toStringTrim();
            txtEMail.Value = _dt.Rows[0]["EMail"].toStringTrim();
            txtChineseName.Value = _dt.Rows[0]["ChineseName"].toStringTrim();
            txtEnglishName.Value = _dt.Rows[0]["EngLishName"].toStringTrim();
            txtSex.Value = _dt.Rows[0]["Sex"].toStringTrim();
            txtWorkMobilePhone.Value = _dt.Rows[0]["WorkMobilePhone"].toStringTrim();
            txtWorkTel.Value = _dt.Rows[0]["WorkTel"].toStringTrim();
            ckIsSiteTips.Checked = _dt.Rows[0]["IsSiteTips"].toBoolean();
            ckIsMailTips.Checked = _dt.Rows[0]["IsMailTips"].toBoolean();
            ckIsGlobalTips.Checked = _dt.Rows[0]["IsGlobalTips"].toBoolean();

            //头像
            string Avatar = _dt.Rows[0]["Avatar"].toStringTrim();
            if (!string.IsNullOrEmpty(Avatar))
                imgAvatar.ImageUrl = Avatar;

            //用户所属部门
            string _sql2 = "select DeptID,UID from UserDepts where UID=@UID";
            SqlParameter[] _sp2 = { new SqlParameter("@UID", SqlDbType.Int) };
            _sp2[0].Value = UID.toInt();

            DataTable _dt2 = MsSQLDbHelper.Query(_sql2, _sp2).Tables[0];
            if (_dt2 != null && _dt2.Rows.Count > 0)
            {
                for (int i = 0; i < _dt2.Rows.Count; i++)
                {
                    UserDeptsIDStr += _dt2.Rows[i]["DeptID"].toStringTrim() + ",";
                }
                UserDeptsIDStr = UserDeptsIDStr.Substring(0, UserDeptsIDStr.Length - 1);
                UserDepts.Value = UserDeptsIDStr;
            }

        }

    }

}