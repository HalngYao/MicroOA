<%@ WebHandler Language="C#" Class="CtrlOvertimeForm" %>

using System;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroDTHelper;
using MicroUserHelper;
using MicroPublicHelper;
using MicroWorkFlowHelper;
using Newtonsoft.Json.Linq;
using MicroAuthHelper;
using MicroHRHelper;
using MicroPrivateHelper;

public class CtrlOvertimeForm : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string strTemp = GetStrTpl();
        string flag = string.Format(strTemp, "False", "0", MicroPublic.GetMsg("SaveURLError"));

        string Action = context.Server.UrlDecode(context.Request.Form["action"]);
        string ShortTableName = context.Server.UrlDecode(context.Request.Form["stn"]);
        string ModuleID = context.Server.UrlDecode(context.Request.Form["mid"]);
        string FormID = context.Server.UrlDecode(context.Request.Form["formid"]);
        string FormsID = context.Server.UrlDecode(context.Request.Form["formsid"]);
        string OvertimeIDs = context.Server.UrlDecode(context.Request.Form["overtimeids"]);
        string IsApprovalForm = context.Server.UrlDecode(context.Request.Form["isapprovalform"]);
        string OvertimeTypeCode = context.Server.UrlDecode(context.Request.Form["overtimetypecode"]);  //平常Normal加班或特殊加班Special

        string FormFields = context.Server.UrlDecode(context.Request.Form["fields"]);  //表单所有字段

        ////FormID = IsApprovalForm.toBoolean() == false ? "0" : FormID;

        //Action = "Modify";
        //ShortTableName = "Overtime";
        //ModuleID = "4";
        //FormID = "5";
        //FormsID = "1585";
        //IsApprovalForm = "False";
        //OvertimeIDs = "1586";
        //OvertimeTypeCode = "Normal";
        //FormFields = "%7B%22hidOvertimeID%22:%221586%22,%22hidParentID%22:%221585%22,%22hidFormID%22:%225%22,%22txtFormNumber%22:%22HRNOT2021040557%22,%22txtFormState%22:%22%E6%92%A4%E5%9B%9E%E7%94%B3%E8%AF%B7%5BWithdrawalApply%5D%22,%22txtApplicant%22:%22Wen%20Li%20(%E6%9D%8E%20%E6%96%87)%22,%22txtPhone%22:%2218565360718%22,%22txtTel%22:%22%22,%22hidStateCode%22:%22-4%22,%22selDeptID%22:%2260%22,%22selOvertimeUID%22:%22279%22,%22raLocationOptions%22:%22GHAC#3%22,%22txtLocation%22:%22GHAC#3%22,%22txtReason%22:%22GHAC#3%203BR%20INN%20FRM%20%E6%B5%8B%E9%87%8F%22,%22radioShiftTypeID%22:%223%22,%22raShiftTypeID%22:%223%22,%22txtOvertimeDate%22:%222021-04-11%22,%22txtStartTime%22:%2210:30%22,%22selOTHour%22:%22%2019%22,%22selOTMin%22:%2230%22,%22cbOvertimeMealID2%22:%223%22,%22cbOvertimeMealID%22:%223%22,%22hidUID%22:%22279%22,%22hidDisplayName%22:%22Wen%20Li%20(%E6%9D%8E%20%E6%96%87)%22,%22hidOvertimeDisplayName%22:%22Wen%20Li%20(%E6%9D%8E%20%E6%96%87)%22,%22hidDateModified%22:%22GetDateTimeNow%22%7D";
        //FormFields = context.Server.UrlDecode(FormFields);


        //context.Response.Write(FormFields);
        //context.Response.End();
        //FormFields = "{\"hidOvertimeID\":\"\",\"hidParentID\":\"\",\"hidFormID\":\"5\",\"txtFormNumber\":\"GetFormNumber\",\"txtFormState\":\"GetFormState\",\"txtApplicant\":\"Admin (管理员)\",\"txtPhone\":\"18565360871\",\"txtTel\":\"101\",\"hidStateCode\":\"GetFormStateCode\",\"selDeptID\":\"85\",\"selOvertimeUID\":\"362\",\"txtLocation\":\"Test888\",\"txtReason\":\"Test888\",\"radioShiftTypeID\":\"6\",\"raShiftTypeID\":\"6\",\"txtOvertimeDate\":\"2021-03-30\",\"txtStartTime\":\"17:00\",\"selOTHour\":\" 2\",\"selOTMin\":\"0\",\"cbOvertimeMealID2\":\"3\",\"cbOvertimeMealID\":\"3\",\"hidUID\":\"1\",\"hidDisplayName\":\"Admin (管理员)\",\"hidOvertimeDisplayName\":\"Admin (管理员)\",\"hidDateModified\":\"GetDateTimeNow\"}";

        ////FormsID = "1";

        //context.Response.Write(FormFields);
        //context.Response.End();

        if (!string.IsNullOrEmpty(Action) && !string.IsNullOrEmpty(ShortTableName) && !string.IsNullOrEmpty(ModuleID) && !string.IsNullOrEmpty(FormID) && !string.IsNullOrEmpty(IsApprovalForm) && !string.IsNullOrEmpty(FormFields) && !string.IsNullOrEmpty(OvertimeTypeCode))
        {
            Action = Action.ToLower();
            OvertimeTypeCode = OvertimeTypeCode.ToLower();

            //context.Response.Write(Tips);
            //context.Response.End();

            //常Normal加班或特殊加班Special，只在新增时用到（因为分正常和特殊两张表单来填写）新增时已固定了所以在修改时不需要变化
            string OvertimeTypeID = "1";
            OvertimeTypeID = OvertimeTypeCode == "special" ? "2" : OvertimeTypeID;

            if (Action == "add")
            {
                //先判断加班者是否超出允加班的最大值，不超才允许继续
                string Tips = MicroHR.CheckUserOvertimeMax(Action, ShortTableName, ModuleID, FormID, IsApprovalForm.toBoolean(), FormFields, OvertimeTypeCode);
                if (Tips.toBoolean())
                {
                    FormsID = FormsID.toInt().ToString();

                    if (string.IsNullOrEmpty(FormsID) || FormsID == "0")
                        flag = SetSubmitForm(Action, ShortTableName, ModuleID, FormID, IsApprovalForm.toBoolean(), FormFields, OvertimeTypeID);
                    else
                        flag = SetAddSubmitFormSub(Action, ShortTableName, ModuleID, FormID, IsApprovalForm.toBoolean(), FormFields, FormsID, OvertimeTypeID);
                }
                else
                    flag = string.Format(strTemp, "False", "0", Tips);
            }

            if (Action == "modify" && OvertimeIDs != "")
            {
                string Tips = MicroHR.CheckModifyUserOvertimeMax(Action, ShortTableName, ModuleID, FormID, IsApprovalForm.toBoolean(), FormFields, OvertimeTypeCode, OvertimeIDs);
                if (Tips.toBoolean())
                    flag = SetModifyForm(Action, ShortTableName, ModuleID, FormID, IsApprovalForm.toBoolean(), FormFields, FormsID, OvertimeIDs);
                else
                    flag = string.Format(strTemp, "False", "0", Tips);
            }


        }

        flag = "{" + flag + "}";
        flag = JToken.Parse(flag).ToString();

        context.Response.Write(flag);
    }

    /// <summary>
    /// 初次插入记录时插入父记录，此时FormsID为空或0
    /// </summary>
    /// <param name="Action"></param>
    /// <param name="ShortTableName"></param>
    /// <param name="ModuleID"></param>
    /// <param name="FormID"></param>
    /// <param name="IsApprovalForm"></param>
    /// <param name="FormFields"></param>
    /// <param name="OvertimeTypeID"></param>
    /// <returns></returns>
    private string SetSubmitForm(string Action, string ShortTableName, string ModuleID, string FormID, Boolean IsApprovalForm, string FormFields, string OvertimeTypeID)
    {
        string strTemp = GetStrTpl();
        string flag = string.Format(strTemp, "False", "0", MicroPublic.GetMsg("SaveURLError"));
        //string flag = MicroPublic.GetMsg("SaveFailed"), Values = string.Empty;

        string TableName = "HROvertime";

        string ParentID = string.Empty; string FormNumber = string.Empty; string FormState = string.Empty; string StateCode = string.Empty; string Applicant = string.Empty; string Phone = string.Empty; string Tel = string.Empty; string DisplayName = string.Empty; string DeptID = string.Empty; string OvertimeDate = string.Empty;

        dynamic json = JToken.Parse(FormFields) as dynamic;

        OvertimeDate = json["txtOvertimeDate"];
        OvertimeDate = OvertimeDate.toStringTrim();

        ParentID = json["hidParentID"];
        ParentID = ParentID.toStringTrim();

        //作为参数进行传输，所以这里不需要赋值
        //FormID = json["hidFormID"];
        //FormID = FormID.toStringTrim();

        FormNumber = json["txtFormNumber"];
        FormNumber = FormNumber.toStringTrim();

        //FormState = json["txtFormState"];
        FormState = MicroWorkFlow.GetApprovalState(-3); //0 = 等待审批[Waiting]，-3 = 填写申请[Fill in]

        //StateCode = json["hidStateCode"];
        StateCode = "-3";

        Applicant = json["txtApplicant"];
        Applicant = Applicant.toStringTrim();

        Phone = json["txtPhone"];
        Phone = Phone.toStringTrim();

        Tel = json["txtTel"];
        Tel = Tel.toStringTrim();

        //UID = json["hidUID"];   //申请者UID
        //UID = UID.toStringTrim();
        int UID = MicroUserInfo.GetUserInfo("UID").toInt();

        DisplayName = json["hidDisplayName"];  //申请者DisplayName
        DisplayName = DisplayName.toStringTrim();

        DeptID = json["selDeptID"];
        DeptID = DeptID.toStringTrim();


        string _sql = "insert into HROvertime (ParentID, FormID, FormNumber, FormState, StateCode, Applicant, Phone, Tel, UID, DisplayName, OvertimeTypeID, DeptID, OvertimeUID, OvertimeDisplayName, LocationOptions, Location, Reason, ShiftTypeID, OvertimeDate, OvertimeTime, StartTime, OTHour, OTMin, EndTime, OvertimeHour, WorkHourSysID, WorkHourSysName, OvertimeTotal, AlreadyDaiXiu, ExceptDaiXiu, WarningHour, WarningTips, OvertimeMealID, UseCar, IP ) values ( @ParentID, @FormID, @FormNumber, @FormState, @StateCode, @Applicant, @Phone, @Tel, @UID, @DisplayName, @OvertimeTypeID, @DeptID, @OvertimeUID, @OvertimeDisplayName, @LocationOptions, @Location, @Reason, @ShiftTypeID, @OvertimeDate, @OvertimeTime, @StartTime, @OTHour, @OTMin, @EndTime, @OvertimeHour, @WorkHourSysID, @WorkHourSysName, @OvertimeTotal, @AlreadyDaiXiu, @ExceptDaiXiu, @WarningHour, @WarningTips, @OvertimeMealID, @UseCar, @IP )";

        SqlParameter[] _sp = { new SqlParameter("@ParentID", SqlDbType.Int),
                                new SqlParameter("@FormID", SqlDbType.Int),
                                new SqlParameter("@FormNumber", SqlDbType.VarChar,200),
                                new SqlParameter("@FormState", SqlDbType.NVarChar,200),
                                new SqlParameter("@StateCode", SqlDbType.Int),
                                new SqlParameter("@Applicant", SqlDbType.NVarChar,200),
                                new SqlParameter("@Phone", SqlDbType.VarChar,30),
                                new SqlParameter("@Tel", SqlDbType.VarChar,30),
                                new SqlParameter("@UID", SqlDbType.Int),
                                new SqlParameter("@DisplayName", SqlDbType.NVarChar,200),
                                new SqlParameter("@OvertimeTypeID", SqlDbType.Int),
                                new SqlParameter("@DeptID", SqlDbType.Int),
                                new SqlParameter("@OvertimeUID", SqlDbType.Int),
                                new SqlParameter("@OvertimeDisplayName", SqlDbType.NVarChar,200),
                                new SqlParameter("@LocationOptions", SqlDbType.NVarChar,1000),
                                new SqlParameter("@Location", SqlDbType.NVarChar,1000),
                                new SqlParameter("@Reason", SqlDbType.NVarChar,2000),
                                new SqlParameter("@ShiftTypeID", SqlDbType.Int),
                                new SqlParameter("@OvertimeDate", SqlDbType.DateTime),
                                new SqlParameter("@OvertimeTime", SqlDbType.VarChar,20),
                                new SqlParameter("@StartTime", SqlDbType.VarChar,5),
                                new SqlParameter("@OTHour", SqlDbType.Decimal),
                                new SqlParameter("@OTMin", SqlDbType.Int),
                                new SqlParameter("@EndTime", SqlDbType.VarChar,5),
                                new SqlParameter("@OvertimeHour", SqlDbType.Decimal),
                                new SqlParameter("@WorkHourSysID", SqlDbType.Int),
                                new SqlParameter("@WorkHourSysName", SqlDbType.NVarChar,2000),
                                new SqlParameter("@OvertimeTotal", SqlDbType.Decimal),
                                new SqlParameter("@AlreadyDaiXiu", SqlDbType.Decimal),
                                new SqlParameter("@ExceptDaiXiu", SqlDbType.Decimal),
                                new SqlParameter("@WarningHour", SqlDbType.Decimal),
                                new SqlParameter("@WarningTips", SqlDbType.NVarChar,2000),
                                new SqlParameter("@OvertimeMealID", SqlDbType.VarChar,200),
                                new SqlParameter("@UseCar", SqlDbType.VarChar,50),
                                new SqlParameter("@IP", SqlDbType.VarChar,50),
                                };

        _sp[0].Value = ParentID.toInt();
        _sp[1].Value = FormID.toInt();
        _sp[2].Value = MicroPublic.GetBuiltinFunction(Action, FormNumber, "", ShortTableName, TableName, FormID);  //FormNumber
        _sp[3].Value = FormState;
        _sp[4].Value = StateCode.toInt(); //StateCode.toInt();  //StateCode 
        _sp[5].Value = Applicant;
        _sp[6].Value = Phone;
        _sp[7].Value = Tel;
        _sp[8].Value = UID;
        _sp[9].Value = DisplayName;
        _sp[10].Value = OvertimeTypeID.toInt();  //OvertimeTypeID 平时或特殊
        _sp[11].Value = DeptID.toInt();
        _sp[12].Value = null;
        _sp[13].Value = "-";
        _sp[14].Value = "-";
        _sp[15].Value = "-";
        _sp[16].Value = "-";
        _sp[17].Value = null;
        _sp[18].Value = OvertimeDate.toDateTime();
        _sp[19].Value = "-";
        _sp[20].Value = "-";
        _sp[21].Value = null;
        _sp[22].Value = null;
        _sp[23].Value = "-";
        _sp[24].Value = null;
        _sp[25].Value = null;
        _sp[26].Value = "-";
        _sp[27].Value = null;
        _sp[28].Value = null;
        _sp[29].Value = null;
        _sp[30].Value = null;
        _sp[31].Value = null;
        _sp[32].Value = null;
        _sp[33].Value = null;
        _sp[34].Value = MicroPublic.GetClientIP();

        if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
        {
            //得到父记录最大的ID作为值返回
            string MaxOvertimeID = SetSubmitFormSub(Action, ShortTableName, ModuleID, FormID, IsApprovalForm, FormFields, OvertimeTypeID);
            if (MaxOvertimeID.toInt() > 0)
                flag = string.Format(strTemp, "True", MaxOvertimeID, MicroPublic.GetMsg("Save"));
        }

        return flag;
    }


    /// <summary>
    /// 初次插入记录时插入子记录，FormsID还是为空或0
    /// </summary>
    /// <param name="Action"></param>
    /// <param name="ShortTableName"></param>
    /// <param name="ModuleID"></param>
    /// <param name="FormID"></param>
    /// <param name="IsApprovalForm"></param>
    /// <param name="FormFields"></param>
    /// <param name="OvertimeTypeID"></param>
    /// <returns></returns>
    private string SetSubmitFormSub(string Action, string ShortTableName, string ModuleID, string FormID, Boolean IsApprovalForm, string FormFields, string OvertimeTypeID)
    {
        string flag = "0";
        string Fields = string.Empty, Values = string.Empty;

        string TableName = "HROvertime";

        string ParentID = string.Empty; string FormNumber = string.Empty; string FormState = string.Empty; string StateCode = string.Empty; string Applicant = string.Empty; string DisplayName = string.Empty; string OvertimeUID = string.Empty; string OvertimeDisplayName = string.Empty; string LocationOptions = string.Empty; string Location = string.Empty; string Reason = string.Empty; string ShiftTypeID = string.Empty; string OvertimeDate = string.Empty; string OvertimeTime = string.Empty; string StartTime = string.Empty; string StartDateTime = string.Empty; string OTHour = string.Empty; string OTMin = string.Empty; string EndTime = string.Empty; string EndDateTime = string.Empty; string OvertimeHour = string.Empty; string WarningTips = string.Empty; string OvertimeMealID = string.Empty, UseCar = string.Empty;
        string MaxOvertimeID = string.Empty;

        DataTable _dtUsers = MicroDataTable.GetDataTable("Use");

        dynamic json = JToken.Parse(FormFields) as dynamic;

        OvertimeDate = json["txtOvertimeDate"];
        OvertimeDate = OvertimeDate.toStringTrim();

        //UID = json["hidUID"];
        //UID = UID.toStringTrim();
        int UID = MicroUserInfo.GetUserInfo("UID").toInt();

        //string StartDate = OvertimeDate.toDateQFirstDay() + " 00:00:00.000",
        //    EndDate = OvertimeDate.toDateQLastDay() + " 23:59:59.998";

        string _sqlOvertime = "select * from HROvertime where OvertimeID=(select max(OvertimeID) from HROvertime where Invalid=0 and Del=0 and ParentID=0)";
        DataTable _dtOvertime = MsSQLDbHelper.Query(_sqlOvertime).Tables[0];

        //得到父记录最大的ID作为值返回
        if (_dtOvertime.Rows.Count > 0)
        {
            MaxOvertimeID = _dtOvertime.Rows[0]["OvertimeID"].toStringTrim();

            ParentID = MaxOvertimeID;  //得到MaxOvertimeID作为ParentID

            //作为参数传递
            //FormID = json["hidFormID"];
            //FormID = FormID.toStringTrim();

            //FormNumber = json["txtFormNumber"];
            FormNumber = _dtOvertime.Rows[0]["FormNumber"].toStringTrim();

            //FormState = json["txtFormState"];
            FormState = _dtOvertime.Rows[0]["FormState"].toStringTrim();

            //StateCode = json["hidStateCode"];
            StateCode = _dtOvertime.Rows[0]["StateCode"].toStringTrim();

        }

        Applicant = json["txtApplicant"];
        Applicant = Applicant.toStringTrim();

        DisplayName = json["hidDisplayName"];
        DisplayName = DisplayName.toStringTrim();

        //DeptID = json["selDeptID"];
        //DeptID = DeptID.toStringTrim();

        OvertimeUID = json["selOvertimeUID"];  //1,2,3  可能存在多位加班的人员，这里插入的是子记录录所以在for循环读取

        LocationOptions = json["raLocationOptions"];
        LocationOptions = LocationOptions.toStringTrim();

        Location = json["txtLocation"];
        Location = Location.toStringTrim();

        Reason = json["txtReason"];
        Reason = Reason.toStringTrim();

        ShiftTypeID = json["raShiftTypeID"];
        ShiftTypeID = ShiftTypeID.toStringTrim();

        //原用来存放时间范围 17:30 - 21:30，由表单提交过来的取值，但现在以持续时间进行计算所以取值由类“ MicroHR.GetOTHour”计算后返回的值
        //OvertimeTime = json["txtOvertimeTime"];
        //OvertimeTime = OvertimeTime.toStringTrim();

        StartTime = json["txtStartTime"];
        StartTime = StartTime.toStringTrim();
        StartDateTime = (OvertimeDate + " " + StartTime).toDateFormat("yyyy-MM-dd HH:mm:ss");

        OTHour = json["selOTHour"];
        OTHour = OTHour.toStringTrim();

        OTMin = json["selOTMin"];
        OTMin = OTMin.toStringTrim();

        OvertimeMealID = json["cbOvertimeMealID"];
        OvertimeMealID = OvertimeMealID.toStringTrim();

        UseCar = json["raUseCar"];
        UseCar = UseCar.toStringTrim();

        //根据班次、加班日期、开始时间、持续小时、分钟、就餐ID计算出：OvertimeTime、EndTime、OvertimeHour
        var getOTHour = MicroHR.GetOTHour(ShiftTypeID, OvertimeDate, StartTime, OTHour, OTMin, OvertimeMealID);
        OvertimeTime = getOTHour.OvertimeTime;
        EndTime = getOTHour.EndTime;
        EndDateTime = (OvertimeDate + " " + EndTime).toDateFormat("yyyy-MM-dd HH:mm:ss");
        OvertimeHour = getOTHour.OvertimeHour;

        if (StartDateTime.toDateTime("yyyy-MM-dd HH:mm:ss") > EndDateTime.toDateTime("yyyy-MM-dd HH:mm:ss"))
            EndDateTime = EndDateTime.toDateTime("yyyy-MM-dd HH:mm:ss").AddDays(1).toDateFormat("yyyy-MM-dd HH:mm:ss");

        string[] ArrOvertimeUID = OvertimeUID.Split(',');

        if (ArrOvertimeUID.Length > 0)
        {
            DataTable _dtInsert = new DataTable();

            _dtInsert.Columns.Add("ParentID", typeof(int));
            _dtInsert.Columns.Add("FormID", typeof(int));
            _dtInsert.Columns.Add("FormNumber", typeof(string));
            _dtInsert.Columns.Add("FormState", typeof(string));
            _dtInsert.Columns.Add("StateCode", typeof(int));
            _dtInsert.Columns.Add("Applicant", typeof(string));
            _dtInsert.Columns.Add("Phone", typeof(string));
            _dtInsert.Columns.Add("Tel", typeof(string));
            _dtInsert.Columns.Add("UID", typeof(int));
            _dtInsert.Columns.Add("DisplayName", typeof(string));
            _dtInsert.Columns.Add("OvertimeTypeID", typeof(int));
            _dtInsert.Columns.Add("DeptID", typeof(int));
            _dtInsert.Columns.Add("OvertimeUID", typeof(int));
            _dtInsert.Columns.Add("OvertimeDisplayName", typeof(string));
            _dtInsert.Columns.Add("LocationOptions", typeof(string));
            _dtInsert.Columns.Add("Location", typeof(string));
            _dtInsert.Columns.Add("Reason", typeof(string));
            _dtInsert.Columns.Add("ShiftTypeID", typeof(int));
            _dtInsert.Columns.Add("OvertimeDate", typeof(DateTime));
            _dtInsert.Columns.Add("OvertimeTime", typeof(string));
            _dtInsert.Columns.Add("StartTime", typeof(string));
            _dtInsert.Columns.Add("StartDateTime", typeof(DateTime));
            _dtInsert.Columns.Add("OTHour", typeof(decimal));
            _dtInsert.Columns.Add("OTMin", typeof(int));
            _dtInsert.Columns.Add("EndTime", typeof(string));
            _dtInsert.Columns.Add("EndDateTime", typeof(DateTime));
            _dtInsert.Columns.Add("OvertimeHour", typeof(decimal));
            _dtInsert.Columns.Add("WorkHourSysID", typeof(int));
            _dtInsert.Columns.Add("WorkHourSysName", typeof(string));
            _dtInsert.Columns.Add("OvertimeTotal", typeof(decimal));
            _dtInsert.Columns.Add("AlreadyDaiXiu", typeof(decimal));
            _dtInsert.Columns.Add("ExceptDaiXiu", typeof(decimal));
            _dtInsert.Columns.Add("WarningHour", typeof(decimal));
            _dtInsert.Columns.Add("WarningTips", typeof(string));
            _dtInsert.Columns.Add("OvertimeMealID", typeof(string));
            _dtInsert.Columns.Add("UseCar", typeof(string));
            _dtInsert.Columns.Add("IP", typeof(string));

            for (int i = 0; i < ArrOvertimeUID.Length; i++)
            {
                string WorkHourSysID = "0"; string WorkHourSysName = "未设置"; string WarningHour = string.Empty; string OvertimeTotal = string.Empty; string OvertimeTotalByUp = string.Empty; string AlreadyDaiXiu = string.Empty; string AlreadyDaiXiuOvertime = string.Empty; string ExceptDaiXiu = string.Empty;

                var getUserWorkHourSystem = MicroHR.GetUserWorkHourSystem(ArrOvertimeUID[i].toInt(), OvertimeDate);
                WorkHourSysID = getUserWorkHourSystem.WorkHourSysID;
                WorkHourSysName = getUserWorkHourSystem.WorkHourSysName;
                WarningHour = getUserWorkHourSystem.WarningValue;

                //加班小时汇总（含休假时间），读取个人数据（HRPersonalRecord表）Hour记录汇总
                var getPersonalRecord = MicroHR.GetPersonalRecord(OvertimeDate, ArrOvertimeUID[i].toInt(), WorkHourSysID.toInt(), 0, "WithdrawalOrMore", StartTime);  //WithdrawalOrMore、ApplyOrMore
                OvertimeTotal = getPersonalRecord.OvertimeTotal;
                OvertimeTotalByUp = getPersonalRecord.OvertimeTotalByUp;
                AlreadyDaiXiu = getPersonalRecord.AlreadyDaiXiu;  //AlreadyDaiXiuByMonth
                AlreadyDaiXiuOvertime = getPersonalRecord.AlreadyDaiXiuOvertime;
                ExceptDaiXiu = getPersonalRecord.ExceptDaiXiu;  //ExceptDaiXiuByMonth

                //读取每个加班者的基础信息
                string UserName = string.Empty, ChineseName = string.Empty, EnglishName = string.Empty, AdDisplayName = string.Empty; string Phone = string.Empty, Tel = string.Empty;
                DataRow[] _rowsUsers = _dtUsers.Select("UID=" + ArrOvertimeUID[i].toInt());
                if (_rowsUsers.Length > 0)
                {
                    UserName = _rowsUsers[0]["UserName"].toStringTrim();
                    ChineseName = _rowsUsers[0]["ChineseName"].toStringTrim();
                    EnglishName = _rowsUsers[0]["EnglishName"].toStringTrim();
                    AdDisplayName = _rowsUsers[0]["AdDisplayName"].toStringTrim();
                    Phone = _rowsUsers[0]["WorkMobilePhone"].toStringTrim();
                    Tel = _rowsUsers[0]["WorkTel"].toStringTrim();
                    OvertimeDisplayName = MicroUserInfo.GetUserInfo(UserName, ChineseName, EnglishName, AdDisplayName);
                }

                string DeptID = MicroUserInfo.GetUserDepts(ArrOvertimeUID[i].ToString(), "DeptID", "LastDept");

                DataRow _drInsert = _dtInsert.NewRow();

                _drInsert["ParentID"] = ParentID.toInt();
                _drInsert["FormID"] = FormID.toInt();
                _drInsert["FormNumber"] = FormNumber;
                _drInsert["FormState"] = FormState;
                _drInsert["StateCode"] = StateCode.toInt();
                _drInsert["Applicant"] = Applicant;
                _drInsert["Phone"] = Phone;
                _drInsert["Tel"] = Tel;
                _drInsert["UID"] = UID;
                _drInsert["DisplayName"] = DisplayName;
                _drInsert["OvertimeTypeID"] = OvertimeTypeID.toInt();  //OvertimeTypeID 平时或特殊
                _drInsert["DeptID"] = DeptID.toInt();
                _drInsert["OvertimeUID"] = ArrOvertimeUID[i].toInt();
                _drInsert["OvertimeDisplayName"] = OvertimeDisplayName;
                _drInsert["LocationOptions"] = LocationOptions;
                _drInsert["Location"] = Location;
                _drInsert["Reason"] = Reason;
                _drInsert["ShiftTypeID"] = ShiftTypeID;
                _drInsert["OvertimeDate"] = OvertimeDate.toDateTime();
                _drInsert["OvertimeTime"] = OvertimeTime;
                _drInsert["StartTime"] = StartTime;
                _drInsert["StartDateTime"] = StartDateTime.toDateTime("yyyy-MM-dd HH:mm:ss");
                _drInsert["OTHour"] = OTHour.toDecimal();
                _drInsert["OTMin"] = OTMin.toInt();
                _drInsert["EndTime"] = EndTime;
                _drInsert["EndDateTime"] = EndDateTime.toDateTime("yyyy-MM-dd HH:mm:ss");
                _drInsert["OvertimeHour"] = OvertimeHour.toDecimal();
                _drInsert["WorkHourSysID"] = WorkHourSysID.toInt();
                _drInsert["WorkHourSysName"] = WorkHourSysName.toStringTrim();
                _drInsert["OvertimeTotal"] = OvertimeTotalByUp.toDecimal() + OvertimeHour.toDecimal(); //已申请过的加班时间(加班日期向上汇总)和当前申请的加班时间
                _drInsert["AlreadyDaiXiu"] = AlreadyDaiXiuOvertime.toDecimal() * 8;  //由休假表取出的LeaveDays需要转为小时所以*8
                _drInsert["ExceptDaiXiu"] = ExceptDaiXiu.toDecimal() + OvertimeHour.toDecimal(); //已申请过的加班时间和当前申请的加班时间;
                _drInsert["WarningHour"] = WarningHour.toDecimal();
                _drInsert["WarningTips"] = WarningTips.toStringTrim();
                _drInsert["OvertimeMealID"] = OvertimeMealID;
                _drInsert["UseCar"] = UseCar;
                _drInsert["IP"] = MicroPublic.GetClientIP();

                _dtInsert.Rows.Add(_drInsert);
            }

            if (MsSQLDbHelper.SqlBulkCopyInsert(_dtInsert, TableName))
            {
                flag = MaxOvertimeID;  //得到父记录最大的ID作为值返回
                MicroPrivate.AddUpdateOvertimeTotal(Action, ShortTableName, ModuleID, FormID, OvertimeUID, OvertimeDate, StartTime, OvertimeHour.toDecimal());
            }
        }


        return flag;
    }


    /// <summary>
    /// 第二次填写内容时带FormsID插入记录
    /// </summary>
    /// <param name="Action"></param>
    /// <param name="ShortTableName"></param>
    /// <param name="ModuleID"></param>
    /// <param name="FormID"></param>
    /// <param name="IsApprovalForm"></param>
    /// <param name="FormFields"></param>
    /// <param name="FormsID"></param>
    /// <param name="OvertimeTypeID"></param>
    /// <returns></returns>
    private string SetAddSubmitFormSub(string Action, string ShortTableName, string ModuleID, string FormID, Boolean IsApprovalForm, string FormFields, string FormsID, string OvertimeTypeID)
    {
        string strTemp = GetStrTpl();
        string flag = string.Format(strTemp, "False", "0", MicroPublic.GetMsg("SaveURLError"));

        string Fields = string.Empty, Values = string.Empty;

        //string _sqlMaxOvertimeID = _dtOvertime.Compute("max(OvertimeID)", "true").ToString();
        string MaxOvertimeID = FormsID;  //更新时FormsID作为MaxOvertimeID 和 ParentID

        string TableName = "HROvertime";

        string ParentID = string.Empty; string FormNumber = string.Empty; string FormState = string.Empty; string StateCode = string.Empty; string Applicant = string.Empty; string DisplayName = string.Empty; string OvertimeUID = string.Empty; string OvertimeDisplayName = string.Empty; string LocationOptions = string.Empty; string Location = string.Empty; string Reason = string.Empty; string ShiftTypeID = string.Empty; string OvertimeDate = string.Empty; string OvertimeTime = string.Empty; string StartTime = string.Empty; string StartDateTime = string.Empty; string OTHour = string.Empty; string OTMin = string.Empty; string EndTime = string.Empty; string EndDateTime = string.Empty; string OvertimeHour = string.Empty; string WarningTips = string.Empty; string OvertimeMealID = string.Empty, UseCar = string.Empty;

        DataTable _dtUsers = MicroDataTable.GetDataTable("Use");

        dynamic json = JToken.Parse(FormFields) as dynamic;

        //ParentID = json["hidParentID"];
        ParentID = MaxOvertimeID;

        //作为参数传递
        //FormID = json["hidFormID"];
        //FormID = FormID.toStringTrim();

        string _sqlOvertime = "select * from HROvertime where Invalid=0 and Del=0 and OvertimeID=@OvertimeID";
        SqlParameter[] _spOvertime = { new SqlParameter("@OvertimeID", SqlDbType.Int) };
        _spOvertime[0].Value = FormsID.toInt();

        DataTable _dtOvertime = MsSQLDbHelper.Query(_sqlOvertime, _spOvertime).Tables[0];

        if (_dtOvertime.Rows.Count > 0)
        {
            //FormNumber = json["txtFormNumber"];
            FormNumber = _dtOvertime.Rows[0]["FormNumber"].toStringTrim();

            //FormState = json["txtFormState"];
            FormState = _dtOvertime.Rows[0]["FormState"].toStringTrim();

            //StateCode = json["hidStateCode"];
            StateCode = _dtOvertime.Rows[0]["StateCode"].toStringTrim();
        }

        Applicant = json["txtApplicant"];
        Applicant = Applicant.toStringTrim();

        int UID = MicroUserInfo.GetUserInfo("UID").toInt();

        DisplayName = json["hidDisplayName"];
        DisplayName = DisplayName.toStringTrim();

        //DeptID = json["selDeptID"];
        //DeptID = DeptID.toStringTrim();

        OvertimeUID = json["selOvertimeUID"];  //1,2,3  可能存在多位加班的人员，这里插入的是子记录录所以在for循环读取

        LocationOptions = json["raLocationOptions"];
        LocationOptions = LocationOptions.toStringTrim();

        Location = json["txtLocation"];
        Location = Location.toStringTrim();

        Reason = json["txtReason"];
        Reason = Reason.toStringTrim();

        ShiftTypeID = json["raShiftTypeID"];
        ShiftTypeID = ShiftTypeID.toStringTrim();

        OvertimeDate = json["txtOvertimeDate"];  //因为这里要用到OvertimeDate所以调整到前面
        OvertimeDate = OvertimeDate.toStringTrim();

        //原用来存放时间范围 17:30 - 21:30，由表单提交过来的取值，但现在以持续时间进行计算所以取值由类计算后返回的值
        //OvertimeTime = json["txtOvertimeTime"];
        //OvertimeTime = OvertimeTime.toStringTrim();

        StartTime = json["txtStartTime"];
        StartTime = StartTime.toStringTrim();
        StartDateTime = (OvertimeDate + " " + StartTime).toDateFormat("yyyy-MM-dd HH:mm:ss");

        OTHour = json["selOTHour"];
        OTHour = OTHour.toStringTrim();

        OTMin = json["selOTMin"];
        OTMin = OTMin.toStringTrim();

        OvertimeMealID = json["cbOvertimeMealID"];
        OvertimeMealID = OvertimeMealID.toStringTrim();

        UseCar = json["raUseCar"];
        UseCar = UseCar.toStringTrim();

        string[] ArrOvertimeUID = OvertimeUID.Split(',');

        var getOTHour = MicroHR.GetOTHour(ShiftTypeID, OvertimeDate, StartTime, OTHour, OTMin, OvertimeMealID);
        OvertimeTime = getOTHour.OvertimeTime;
        EndTime = getOTHour.EndTime;
        EndDateTime = (OvertimeDate + " " + EndTime).toDateFormat("yyyy-MM-dd HH:mm:ss");
        OvertimeHour = getOTHour.OvertimeHour;

        if (StartDateTime.toDateTime("yyyy-MM-dd HH:mm:ss") > EndDateTime.toDateTime("yyyy-MM-dd HH:mm:ss"))
            EndDateTime = EndDateTime.toDateTime("yyyy-MM-dd HH:mm:ss").AddDays(1).toDateFormat("yyyy-MM-dd HH:mm:ss");

        if (ArrOvertimeUID.Length > 0)
        {

            DataTable _dtInsert = new DataTable();

            _dtInsert.Columns.Add("ParentID", typeof(int));
            _dtInsert.Columns.Add("FormID", typeof(int));
            _dtInsert.Columns.Add("FormNumber", typeof(string));
            _dtInsert.Columns.Add("FormState", typeof(string));
            _dtInsert.Columns.Add("StateCode", typeof(int));
            _dtInsert.Columns.Add("Applicant", typeof(string));
            _dtInsert.Columns.Add("Phone", typeof(string));
            _dtInsert.Columns.Add("Tel", typeof(string));
            _dtInsert.Columns.Add("UID", typeof(int));
            _dtInsert.Columns.Add("DisplayName", typeof(string));
            _dtInsert.Columns.Add("OvertimeTypeID", typeof(int));
            _dtInsert.Columns.Add("DeptID", typeof(int));
            _dtInsert.Columns.Add("OvertimeUID", typeof(int));
            _dtInsert.Columns.Add("OvertimeDisplayName", typeof(string));
            _dtInsert.Columns.Add("LocationOptions", typeof(string));
            _dtInsert.Columns.Add("Location", typeof(string));
            _dtInsert.Columns.Add("Reason", typeof(string));
            _dtInsert.Columns.Add("ShiftTypeID", typeof(int));
            _dtInsert.Columns.Add("OvertimeDate", typeof(DateTime));
            _dtInsert.Columns.Add("OvertimeTime", typeof(string));
            _dtInsert.Columns.Add("StartTime", typeof(string));
            _dtInsert.Columns.Add("StartDateTime", typeof(DateTime));
            _dtInsert.Columns.Add("OTHour", typeof(decimal));
            _dtInsert.Columns.Add("OTMin", typeof(int));
            _dtInsert.Columns.Add("EndTime", typeof(string));
            _dtInsert.Columns.Add("EndDateTime", typeof(DateTime));
            _dtInsert.Columns.Add("OvertimeHour", typeof(decimal));
            _dtInsert.Columns.Add("WorkHourSysID", typeof(int));
            _dtInsert.Columns.Add("WorkHourSysName", typeof(string));
            _dtInsert.Columns.Add("OvertimeTotal", typeof(decimal));
            _dtInsert.Columns.Add("AlreadyDaiXiu", typeof(decimal));
            _dtInsert.Columns.Add("ExceptDaiXiu", typeof(decimal));
            _dtInsert.Columns.Add("WarningHour", typeof(decimal));
            _dtInsert.Columns.Add("WarningTips", typeof(string));
            _dtInsert.Columns.Add("OvertimeMealID", typeof(string));
            _dtInsert.Columns.Add("UseCar", typeof(string));
            _dtInsert.Columns.Add("IP", typeof(string));

            for (int i = 0; i < ArrOvertimeUID.Length; i++)
            {
                string WorkHourSysID = "0"; string WorkHourSysName = "未设置"; string WarningHour = string.Empty; string OvertimeTotal = string.Empty; string OvertimeTotalByUp = string.Empty; string AlreadyDaiXiu = string.Empty; string AlreadyDaiXiuOvertime = string.Empty; string ExceptDaiXiu = string.Empty;

                var getUserWorkHourSystem = MicroHR.GetUserWorkHourSystem(ArrOvertimeUID[i].toInt(), OvertimeDate);
                WorkHourSysID = getUserWorkHourSystem.WorkHourSysID;
                WorkHourSysName = getUserWorkHourSystem.WorkHourSysName;
                WarningHour = getUserWorkHourSystem.WarningValue;

                //加班小时汇总（含休假时间），读取个人数据（HRPersonalRecord表）Hour记录汇总
                var getPersonalRecord = MicroHR.GetPersonalRecord(OvertimeDate, ArrOvertimeUID[i].toInt(), WorkHourSysID.toInt(), 0, "WithdrawalOrMore", StartTime);  //WithdrawalOrMore、ApplyOrMore
                OvertimeTotal = getPersonalRecord.OvertimeTotal;
                OvertimeTotalByUp = getPersonalRecord.OvertimeTotalByUp;
                AlreadyDaiXiu = getPersonalRecord.AlreadyDaiXiu;  //AlreadyDaiXiuByMonth
                AlreadyDaiXiuOvertime = getPersonalRecord.AlreadyDaiXiuOvertime;
                ExceptDaiXiu = getPersonalRecord.ExceptDaiXiu;  //ExceptDaiXiuByMonth

                string UserName = string.Empty, ChineseName = string.Empty, EnglishName = string.Empty, AdDisplayName = string.Empty, Phone = string.Empty, Tel = string.Empty;
                DataRow[] _rowsUsers = _dtUsers.Select("UID=" + ArrOvertimeUID[i].toInt());
                if (_rowsUsers.Length > 0)
                {
                    UserName = _rowsUsers[0]["UserName"].toStringTrim();
                    ChineseName = _rowsUsers[0]["ChineseName"].toStringTrim();
                    EnglishName = _rowsUsers[0]["EnglishName"].toStringTrim();
                    AdDisplayName = _rowsUsers[0]["AdDisplayName"].toStringTrim();
                    Phone = _rowsUsers[0]["WorkMobilePhone"].toStringTrim();
                    Tel = _rowsUsers[0]["WorkTel"].toStringTrim();

                    OvertimeDisplayName = MicroUserInfo.GetUserInfo(UserName, ChineseName, EnglishName, AdDisplayName);
                }

                string DeptID = MicroUserInfo.GetUserDepts(ArrOvertimeUID[i].ToString(), "DeptID", "LastDept");

                DataRow _drInsert = _dtInsert.NewRow();

                _drInsert["ParentID"] = ParentID.toInt();
                _drInsert["FormID"] = FormID.toInt();
                _drInsert["FormNumber"] = FormNumber;
                _drInsert["FormState"] = FormState;
                _drInsert["StateCode"] = StateCode.toInt();
                _drInsert["Applicant"] = Applicant;
                _drInsert["Phone"] = Phone;
                _drInsert["Tel"] = Tel;
                _drInsert["UID"] = UID.toInt();
                _drInsert["DisplayName"] = DisplayName;
                _drInsert["OvertimeTypeID"] = OvertimeTypeID.toInt();  //OvertimeTypeID 平时或特殊
                _drInsert["DeptID"] = DeptID.toInt();
                _drInsert["OvertimeUID"] = ArrOvertimeUID[i].toInt();
                _drInsert["OvertimeDisplayName"] = OvertimeDisplayName;
                _drInsert["LocationOptions"] = LocationOptions;
                _drInsert["Location"] = Location;
                _drInsert["Reason"] = Reason;
                _drInsert["ShiftTypeID"] = ShiftTypeID;
                _drInsert["OvertimeDate"] = OvertimeDate.toDateTime();
                _drInsert["OvertimeTime"] = OvertimeTime;
                _drInsert["StartTime"] = StartTime;
                _drInsert["StartDateTime"] = StartDateTime.toDateTime("yyyy-MM-dd HH:mm:ss");
                _drInsert["OTHour"] = OTHour.toDecimal();
                _drInsert["OTMin"] = OTMin.toInt();
                _drInsert["EndTime"] = EndTime;
                _drInsert["EndDateTime"] = EndDateTime.toDateTime("yyyy-MM-dd HH:mm:ss");
                _drInsert["OvertimeHour"] = OvertimeHour.toDecimal();
                _drInsert["WorkHourSysID"] = WorkHourSysID.toInt();
                _drInsert["WorkHourSysName"] = WorkHourSysName.toStringTrim();
                _drInsert["OvertimeTotal"] = OvertimeTotalByUp.toDecimal() + OvertimeHour.toDecimal();  //已申请过的加班时间(根据加班日期向下汇总)和当前申请的加班时间
                _drInsert["AlreadyDaiXiu"] = AlreadyDaiXiuOvertime.toDecimal() * 8;  //由休假表取出的LeaveDays需要转为小时所以*8
                _drInsert["ExceptDaiXiu"] = ExceptDaiXiu.toDecimal() + OvertimeHour.toDecimal(); //已申请过的加班时间和当前申请的加班时间;
                _drInsert["WarningHour"] = WarningHour.toDecimal();
                _drInsert["WarningTips"] = WarningTips.toStringTrim();
                _drInsert["OvertimeMealID"] = OvertimeMealID;
                _drInsert["UseCar"] = UseCar;
                _drInsert["IP"] = MicroPublic.GetClientIP();

                _dtInsert.Rows.Add(_drInsert);
            }
            if (MsSQLDbHelper.SqlBulkCopyInsert(_dtInsert, TableName))
            {
                flag = string.Format(strTemp, "True", MaxOvertimeID, MicroPublic.GetMsg("Save"));
                MicroPrivate.AddUpdateOvertimeTotal(Action, ShortTableName, ModuleID, FormID, OvertimeUID, OvertimeDate, StartTime, OvertimeHour.toDecimal());
            }
        }

        return flag;
    }


    /// <summary>
    /// 修改加班申请表
    /// </summary>
    /// <param name="Action"></param>
    /// <param name="ShortTableName"></param>
    /// <param name="ModuleID"></param>
    /// <param name="FormID"></param>
    /// <param name="IsApprovalForm"></param>
    /// <param name="FormFields"></param>
    /// <param name="FormsID">父记录的ID</param>
    /// <param name="OvertimeIDs">被选中的子记录ID</param>
    /// <returns></returns>
    private string SetModifyForm(string Action, string ShortTableName, string ModuleID, string FormID, Boolean IsApprovalForm, string FormFields, string FormsID, string OvertimeIDs)
    {
        string strTemp = GetStrTpl();
        string flag = string.Format(strTemp, "False", "01", MicroPublic.GetMsg("SaveURLError"));

        string Fields = string.Empty, Values = string.Empty;

        //string _sqlMaxOvertimeID = _dtOvertime.Compute("max(OvertimeID)", "true").ToString();
        string MaxOvertimeID = FormsID;  //更新时FormsID作为MaxOvertimeID 和 ParentID

        string TableName = "HROvertime";

        string ParentID = string.Empty; string FormNumber = string.Empty; string FormState = string.Empty; string StateCode = string.Empty; string Applicant = string.Empty; string DisplayName = string.Empty; string DeptID = string.Empty;
        //string OvertimeUID = string.Empty;
        string OvertimeDisplayName = string.Empty; string LocationOptions = string.Empty; string Location = string.Empty; string Reason = string.Empty; string ShiftTypeID = string.Empty; string OvertimeDate = string.Empty; string OvertimeTime = string.Empty; string StartTime = string.Empty; string StartDateTime = string.Empty; string OTHour = string.Empty; string OTMin = string.Empty; string EndTime = string.Empty; string EndDateTime = string.Empty; string OvertimeHour = string.Empty; string WarningTips = string.Empty; string OvertimeMealID = string.Empty, UseCar = string.Empty;

        dynamic json = JToken.Parse(FormFields) as dynamic;

        OvertimeDate = json["txtOvertimeDate"];
        OvertimeDate = OvertimeDate.toStringTrim();

        LocationOptions = json["raLocationOptions"];
        LocationOptions = LocationOptions.toStringTrim();

        Location = json["txtLocation"];
        Location = Location.toStringTrim();

        Reason = json["txtReason"];
        Reason = Reason.toStringTrim();

        ShiftTypeID = json["raShiftTypeID"];
        ShiftTypeID = ShiftTypeID.toStringTrim();

        StartTime = json["txtStartTime"];
        StartTime = StartTime.toStringTrim();
        StartDateTime = (OvertimeDate + " " + StartTime).toDateFormat("yyyy-MM-dd HH:mm:ss");

        OTHour = json["selOTHour"];
        OTHour = OTHour.toStringTrim();

        OTMin = json["selOTMin"];
        OTMin = OTMin.toStringTrim();

        OvertimeMealID = json["cbOvertimeMealID"];
        OvertimeMealID = OvertimeMealID.toStringTrim();

        UseCar = json["raUseCar"];
        UseCar = UseCar.toStringTrim();

        //根据开始时间、持续时间计算，OvertimeTime、EndTime、OvertimeHour
        var getOTHour = MicroHR.GetOTHour(ShiftTypeID, OvertimeDate, StartTime, OTHour, OTMin, OvertimeMealID);
        OvertimeTime = getOTHour.OvertimeTime;
        EndTime = getOTHour.EndTime;
        EndDateTime = (OvertimeDate + " " + EndTime).toDateFormat("yyyy-MM-dd HH:mm:ss");
        OvertimeHour = getOTHour.OvertimeHour;

        if (StartDateTime.toDateTime("yyyy-MM-dd HH:mm:ss") > EndDateTime.toDateTime("yyyy-MM-dd HH:mm:ss"))
            EndDateTime = EndDateTime.toDateTime("yyyy-MM-dd HH:mm:ss").AddDays(1).toDateFormat("yyyy-MM-dd HH:mm:ss");

        string _sql = "select * from HROvertime where Del=0 and Invalid=0 and OvertimeID in(" + OvertimeIDs + ")";
        DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

        if (_dt.Rows.Count > 0)
        {
            int Result = 0;
            for (int i = 0; i < _dt.Rows.Count; i++)
            {
                string OvertimeID = _dt.Rows[i]["OvertimeID"].ToString(),
                        OvertimeUID = _dt.Rows[i]["OvertimeUID"].ToString(),
                        OldOvertimeDate = _dt.Rows[i]["OvertimeDate"].ToString(),
                        OldStartTime = _dt.Rows[i]["StartTime"].ToString(),
                        OldOvertimeHour = _dt.Rows[i]["OvertimeHour"].ToString(),
                        OldOvertimeTotal = _dt.Rows[i]["OvertimeTotal"].ToString(),
                        OldAlreadyDaiXiu = _dt.Rows[i]["AlreadyDaiXiu"].ToString();

                string WorkHourSysID = "0"; string WorkHourSysName = "未设置"; string WarningHour = string.Empty; string OvertimeTotal = string.Empty; string OvertimeTotalByUp = string.Empty; string AlreadyDaiXiu = string.Empty; string AlreadyDaiXiuOvertime = string.Empty; string ExceptDaiXiu = string.Empty;

                var getUserWorkHourSystem = MicroHR.GetUserWorkHourSystem(OvertimeUID.toInt(), OvertimeDate);
                WorkHourSysID = getUserWorkHourSystem.WorkHourSysID;
                WorkHourSysName = getUserWorkHourSystem.WorkHourSysName;
                WarningHour = getUserWorkHourSystem.WarningValue;

                //加班小时汇总（含休假时间），读取个人数据（HRPersonalRecord表）Hour记录汇总
                var getPersonalRecord = MicroHR.GetPersonalRecord(OvertimeDate, OvertimeUID.toInt(), WorkHourSysID.toInt(), 0, "WithdrawalOrMore", StartTime);  //WithdrawalOrMore、ApplyOrMore
                OvertimeTotal = getPersonalRecord.OvertimeTotal;
                OvertimeTotalByUp = getPersonalRecord.OvertimeTotalByUp;
                AlreadyDaiXiu = getPersonalRecord.AlreadyDaiXiu;
                AlreadyDaiXiuOvertime = getPersonalRecord.AlreadyDaiXiuOvertime;
                ExceptDaiXiu = getPersonalRecord.ExceptDaiXiu;


                decimal decOvertimeTotal = OvertimeTotal.toDecimal();

                string _sql2 = "update " + TableName + " set LocationOptions=@LocationOptions, Location=@Location, Reason=@Reason, ShiftTypeID=@ShiftTypeID, OvertimeDate=@OvertimeDate, OvertimeTime=@OvertimeTime, StartTime=@StartTime, StartDateTime=@StartDateTime, OTHour=@OTHour, OTMin=@OTMin, EndTime=@EndTime, EndDateTime=@EndDateTime, OvertimeHour=@OvertimeHour, WorkHourSysID=@WorkHourSysID, WorkHourSysName=@WorkHourSysName, OvertimeTotal=@OvertimeTotal, AlreadyDaiXiu=@AlreadyDaiXiu, ExceptDaiXiu=@ExceptDaiXiu, WarningHour=@WarningHour, WarningTips=@WarningTips, OvertimeMealID=@OvertimeMealID, UseCar=@UseCar, IP=@IP, DateModified=@DateModified where OvertimeID=@OvertimeID";

                SqlParameter[] _sp2 = {
                                new SqlParameter("@LocationOptions", SqlDbType.NVarChar,1000),
                                new SqlParameter("@Location", SqlDbType.NVarChar,1000),
                                new SqlParameter("@Reason", SqlDbType.NVarChar,2000),
                                new SqlParameter("@ShiftTypeID", SqlDbType.Int),
                                new SqlParameter("@OvertimeDate", SqlDbType.DateTime),
                                new SqlParameter("@OvertimeTime", SqlDbType.VarChar,20),
                                new SqlParameter("@StartTime", SqlDbType.VarChar,5),
                                new SqlParameter("@StartDateTime", SqlDbType.DateTime),
                                new SqlParameter("@OTHour", SqlDbType.Decimal),
                                new SqlParameter("@OTMin", SqlDbType.Int),
                                new SqlParameter("@EndTime", SqlDbType.VarChar,5),
                                new SqlParameter("@EndDateTime", SqlDbType.DateTime),
                                new SqlParameter("@OvertimeHour", SqlDbType.Decimal),
                                new SqlParameter("@WorkHourSysID", SqlDbType.Int),
                                new SqlParameter("@WorkHourSysName", SqlDbType.NVarChar,2000),
                                new SqlParameter("@OvertimeTotal", SqlDbType.Decimal),
                                new SqlParameter("@AlreadyDaiXiu", SqlDbType.Decimal),
                                new SqlParameter("@ExceptDaiXiu", SqlDbType.Decimal),
                                new SqlParameter("@WarningHour", SqlDbType.Decimal),
                                new SqlParameter("@WarningTips", SqlDbType.NVarChar,2000),
                                new SqlParameter("@OvertimeMealID", SqlDbType.VarChar,200),
                                new SqlParameter("@UseCar", SqlDbType.VarChar,50),
                                new SqlParameter("@IP", SqlDbType.VarChar,50),
                                new SqlParameter("@DateModified", SqlDbType.DateTime),
                                new SqlParameter("@OvertimeID", SqlDbType.Int)
                                };

                _sp2[0].Value = LocationOptions;
                _sp2[1].Value = Location;
                _sp2[2].Value = Reason;
                _sp2[3].Value = ShiftTypeID.toInt();
                _sp2[4].Value = OvertimeDate.toDateTime();
                _sp2[5].Value = OvertimeTime;
                _sp2[6].Value = StartTime;
                _sp2[7].Value = StartDateTime.toDateTime("yyyy-MM-dd HH:mm:ss");
                _sp2[8].Value = OTHour.toDecimal();
                _sp2[9].Value = OTMin.toInt();
                _sp2[10].Value = EndTime;
                _sp2[11].Value = EndDateTime.toDateTime("yyyy-MM-dd HH:mm:ss");
                _sp2[12].Value = OvertimeHour.toDecimal();
                _sp2[13].Value = WorkHourSysID.toInt();
                _sp2[14].Value = WorkHourSysName;

                //#1. 如果新旧日期相等时###
                if (OvertimeDate.toDateTime() == OldOvertimeDate.toDateTime())
                {
                    //#1.1 如果新旧开始时间相等时
                    if (StartTime.toDateTime("HH:mm") == OldStartTime.toDateTime("HH:mm"))
                        decOvertimeTotal = OldOvertimeTotal.toDecimal() - OldOvertimeHour.toDecimal() + OvertimeHour.toDecimal();

                    //#1.2 否则如果新时间大于旧时间###
                    else if (StartTime.toDateTime("HH:mm") > OldStartTime.toDateTime("HH:mm"))
                        decOvertimeTotal = OvertimeTotalByUp.toDecimal() - OldOvertimeHour.toDecimal() + OvertimeHour.toDecimal();

                    //#1.3 否则如果新时间小于旧时间###
                    else if (StartTime.toDateTime("HH:mm") < OldStartTime.toDateTime("HH:mm"))
                        decOvertimeTotal = OvertimeTotalByUp.toDecimal() + OvertimeHour.toDecimal();

                }

                //#2. 如果新日期>旧日期###
                else if (OvertimeDate.toDateTime() > OldOvertimeDate.toDateTime())
                    decOvertimeTotal = OvertimeTotalByUp.toDecimal() - OldOvertimeHour.toDecimal() + OvertimeHour.toDecimal();


                //#3. 如果新日期<旧日期###
                else if (OvertimeDate.toDateTime() < OldOvertimeDate.toDateTime())
                    decOvertimeTotal = OvertimeTotalByUp.toDecimal() + OvertimeHour.toDecimal();


                ////剔除代休后加班=新的总加班时间-原来已代休的时间(20210609废除)
                //decExceptDaiXiu = decOvertimeTotal - OldAlreadyDaiXiu.toDecimal();

                _sp2[15].Value = decOvertimeTotal;
                _sp2[16].Value = AlreadyDaiXiuOvertime.toDecimal() * 8; //由休假表取出的LeaveDays需要转为小时所以*8 (20210609更新)，原来保持不变  //OldAlreadyDaiXiu; 
                _sp2[17].Value = ExceptDaiXiu.toDecimal() - OldOvertimeHour.toDecimal() + OvertimeHour.toDecimal(); //(20210609更新) //decExceptDaiXiu;  //decExceptDaiXiu = decOvertimeTotal - OldAlreadyDaiXiu.toDecimal();
                _sp2[18].Value = WarningHour.toDecimal();
                _sp2[19].Value = WarningTips;
                _sp2[20].Value = OvertimeMealID;
                _sp2[21].Value = UseCar;
                _sp2[22].Value = MicroPublic.GetClientIP();
                _sp2[23].Value = DateTime.Now;
                _sp2[24].Value = OvertimeID.toInt();

                if (MsSQLDbHelper.ExecuteSql(_sql2, _sp2) > 0)
                {
                    Result = Result + 1;
                    MicroPrivate.ModifyUpdateOvertimeTotal(Action, ShortTableName, ModuleID, FormID, FormsID, OvertimeID, OvertimeUID, WorkHourSysID, OldOvertimeDate, OvertimeDate, OldStartTime, StartTime, OldOvertimeHour.toDecimal(), OvertimeHour.toDecimal(), AlreadyDaiXiuOvertime.toDecimal() * 8);
                }

            }

            if (Result > 0)
                flag = string.Format(strTemp, "True", MaxOvertimeID, MicroPublic.GetMsg("Save"));

        }

        return flag;

    }

    private string GetStrTpl()
    {
        string strTpl = "\"State\":\"{0}\", \"FormsID\":\"{1}\", \"Msg\":\"{2}\"";
        return strTpl;
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}