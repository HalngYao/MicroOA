<%@ WebHandler Language="C#" Class="CtrlOnDutyFormList" %>

using System;
using System.IO;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroDTHelper;
using MicroPublicHelper;
using MicroWorkFlowHelper;
using MicroAuthHelper;
using MicroUserHelper;
using MicroPrivateHelper;
using Newtonsoft.Json.Linq;

public class CtrlOnDutyFormList : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = MicroPublic.GetMsg("SubmitURLError");

        string Action = context.Server.UrlDecode(context.Request.Form["action"]);
        string ShortTableName = context.Server.UrlDecode(context.Request.Form["stn"]);
        string ModuleID = context.Server.UrlDecode(context.Request.Form["mid"]);
        string FormID = context.Server.UrlDecode(context.Request.Form["formid"]);
        string FormsID = context.Server.UrlDecode(context.Request.Form["formsid"]);
        string IsApprovalForm = context.Server.UrlDecode(context.Request.Form["isapprovalform"]);
        string FilePath = context.Server.UrlDecode(context.Request.Form["filepath"]);
        string Fields = context.Server.UrlDecode(context.Request.Form["fields"]);

        FormID = IsApprovalForm.toBoolean() == false ? "0" : FormID;

        ////测试数据
        //Action = "Add";
        //ShortTableName = "OnDutyForm";
        //ModuleID = "4";
        //FormID = "13";
        //FormsID = "";
        //IsApprovalForm = "True";
        //Fields = "%7B%22hidFormID%22:%22%22,%22txtFormNumber%22:%22GetFormNumber%22,%22txtFormState%22:%22%E5%A1%AB%E5%86%99%E7%94%B3%E8%AF%B7%5BFill%20in%5D%22,%22hidStateCode%22:%22GetFormStateCode%22,%22file%22:%22%22,%22ApprovalNode222%22:%22236%22%7D";
        //Fields = context.Server.UrlDecode(Fields);
        //FilePath = "/Resource/UploadFiles/Forms/HROnDuty/Admin_20210930171158884_OnDuty_20210831.xlsx";

        if (!string.IsNullOrEmpty(Action) && !string.IsNullOrEmpty(ShortTableName) && !string.IsNullOrEmpty(ModuleID) && !string.IsNullOrEmpty(FormID) && !string.IsNullOrEmpty(IsApprovalForm) && !string.IsNullOrEmpty(Fields))
        {
            Action = Action.ToLower();

            if (Action == "add")
            {
                //如果FormsID不为空且FilePatch为空时，代表是手动添加的内容
                if (!string.IsNullOrEmpty(FormsID) && string.IsNullOrEmpty(FilePath))
                    flag = SetSubmitForm(Action, ShortTableName, ModuleID, FormID, FormsID, IsApprovalForm, Fields);

                //否则如果FormsID为空且FilePath不为空时，代表的是通过Excel导入的内容
                else if (string.IsNullOrEmpty(FormsID) && !string.IsNullOrEmpty(FilePath))
                    flag = SetSubmitExcelForm(Action, ShortTableName, ModuleID, FormID, FormsID, IsApprovalForm, FilePath, Fields);

            }

            if (Action == "modify" && !string.IsNullOrEmpty(FilePath) && FormsID.toInt() != 0)
                flag = SetModifyExcelForm(Action, ShortTableName, ModuleID, FormID, FormsID, IsApprovalForm, FilePath, Fields);
        }

        context.Response.Write(flag);
    }

    /// <summary>
    /// 提交表单（通过系统添加功能提交表单，因为现在是通过Excel导入，是否需要开发通过系统添加的功能再考虑 20210909备注）
    /// </summary>
    /// <param name="Action"></param>
    /// <param name="ShortTableName"></param>
    /// <param name="ModuleID"></param>
    /// <param name="FormID"></param>
    /// <param name="FormsID"></param>
    /// <param name="IsApprovalForm"></param>
    /// <param name="FormFields"></param>
    /// <returns></returns>
    private static string SetSubmitForm(string Action, string ShortTableName, string ModuleID, string FormID, string FormsID, string IsApprovalForm, string FormFields)
    {
        string flag = MicroPublic.GetMsg("SubmitURLError"),
                TableName = MicroPublic.GetTableName(ShortTableName);


        return flag;
    }


    private static string SetSubmitExcelForm(string Action, string ShortTableName, string ModuleID, string FormID, string FormsID, string IsApprovalForm, string FilePath, string FormFields)
    {
        string flag = MicroPublic.GetMsg("SubmitURLError"),
                TableName = MicroPublic.GetTableName(ShortTableName);

        try
        {
            dynamic json = JToken.Parse(FormFields) as dynamic;

            string ParentID = string.Empty; string FormNumber = string.Empty; string FormState = string.Empty; string StateCode = string.Empty; string IP = string.Empty; string Applicant = string.Empty; string Phone = string.Empty; string Tel = string.Empty; string UID = string.Empty; string DisplayName = string.Empty; string DeptID = string.Empty; string OvertimeDate = string.Empty;

            ParentID = "0";

            FormNumber = json["txtFormNumber"];
            FormNumber = FormNumber.toStringTrim();
            FormNumber = MicroPublic.GetBuiltinFunction(Action, FormNumber, "", ShortTableName, TableName, FormID);

            StateCode = "0";
            FormState = MicroWorkFlow.GetApprovalState(StateCode.toInt()); //0 = 等待审批[Waiting]，-3 = 填写申请[Fill in]
            IP = MicroPublic.GetClientIP();
            Applicant = MicroUserInfo.GetUserInfo("DisplayName");
            Phone = MicroUserInfo.GetUserInfo("WorkMobilePhone");
            Tel = MicroUserInfo.GetUserInfo("WorkTel");
            UID = MicroUserInfo.GetUserInfo("UID");
            DisplayName = MicroUserInfo.GetUserInfo("DisplayName");
            DeptID = MicroUserInfo.GetUserInfo("LastDeptID");

            //由临时文件路径映射到正式文件路径
            string NewFullFilePath = "/Resource/UploadFiles/Forms/HROnDuty/" + MicroPublic.GetSplitLastStr(FilePath, '/');

            //第一次插入父记录
            string _sql = "insert into " + TableName + " (ParentID, FormID, FormNumber, FormState, StateCode, IP, Applicant, Phone, Tel, UID, DisplayName, DeptID, FilePath) " +
            "values (@ParentID, @FormID, @FormNumber, @FormState, @StateCode, @IP, @Applicant, @Phone, @Tel, @UID, @DisplayName, @DeptID, @FilePath)";

            SqlParameter[] _sp = { new SqlParameter("@ParentID",SqlDbType.Int),
                         new SqlParameter("@FormID",SqlDbType.Int),
                         new SqlParameter("@FormNumber",SqlDbType.VarChar,200),
                         new SqlParameter("@FormState",SqlDbType.NVarChar,200),
                         new SqlParameter("@StateCode",SqlDbType.Int),
                         new SqlParameter("@IP",SqlDbType.VarChar,50),
                         new SqlParameter("@Applicant",SqlDbType.NVarChar,200),
                         new SqlParameter("@Phone",SqlDbType.VarChar,30),
                         new SqlParameter("@Tel",SqlDbType.VarChar,30),
                         new SqlParameter("@UID",SqlDbType.Int),
                         new SqlParameter("@DisplayName",SqlDbType.NVarChar,200),
                         new SqlParameter("@DeptID",SqlDbType.Int),
                         new SqlParameter("@FilePath",SqlDbType.VarChar)
                        };

            _sp[0].Value = ParentID.toInt();
            _sp[1].Value = FormID.toInt();
            _sp[2].Value = FormNumber;
            _sp[3].Value = FormState;
            _sp[4].Value = StateCode.toInt();
            _sp[5].Value = IP;
            _sp[6].Value = Applicant;
            _sp[7].Value = Phone;
            _sp[8].Value = Tel;
            _sp[9].Value = UID.toInt();
            _sp[10].Value = DisplayName;
            _sp[11].Value = DeptID.toInt();
            _sp[12].Value = NewFullFilePath.toStringTrim();


            if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
            {
                //############读取Excel数据写入数据库Start###############
                int MaxDutyID = MsSQLDbHelper.GetMaxNumber("DutyID", TableName);  //取得MaxDutyID作为ParentID

                string FullFilePath = HttpContext.Current.Server.MapPath(FilePath);  //临时文件的Excel路径 

                //读取Excel生成DataTable
                DataTable _dtExcel = MsSQLDbHelper.QueryExcel(FullFilePath).Tables[0];

                string Msg = string.Empty, MsgUserName = string.Empty, MsgChineseName = string.Empty, MsgLocation = string.Empty, MsgDutyDate = string.Empty, MsgShiftName = string.Empty, MsgEmptyColumns = string.Empty;

                //Rows大于1代表从第3行（0起步）开始读取数据，Columns大于3代表从第5列开始取数
                //因为Rows1是标题，Rows2是例子
                //因为Columns前4列是基本数据
                if (_dtExcel != null && _dtExcel.Rows.Count > 1 && _dtExcel.Columns.Count > 3)
                {
                    DataTable _dtUsers = MicroDataTable.GetDataTable("Use");
                    DataTable _dtLocation = MicroDataTable.GetDataTable("CommonLocations");
                    DataTable _dtShiftType = MicroDataTable.GetDataTable("ShiftType");

                    //***********构建表头Start***********
                    //要插入数据的字段
                    DataTable _dtInsert = new DataTable();
                    _dtInsert.Columns.Add("ParentID", typeof(int));
                    _dtInsert.Columns.Add("FormID", typeof(int));
                    _dtInsert.Columns.Add("FormNumber", typeof(string));
                    _dtInsert.Columns.Add("FormState", typeof(string));
                    _dtInsert.Columns.Add("StateCode", typeof(int));  //..
                    _dtInsert.Columns.Add("IP", typeof(string));
                    _dtInsert.Columns.Add("Applicant", typeof(string));
                    _dtInsert.Columns.Add("Phone", typeof(string));
                    _dtInsert.Columns.Add("Tel", typeof(string));
                    _dtInsert.Columns.Add("UID", typeof(int));
                    _dtInsert.Columns.Add("DisplayName", typeof(string));
                    _dtInsert.Columns.Add("DeptID", typeof(int));
                    _dtInsert.Columns.Add("DutyUID", typeof(int));
                    _dtInsert.Columns.Add("DutyUserName", typeof(string));
                    _dtInsert.Columns.Add("DutyDisplayName", typeof(string));
                    _dtInsert.Columns.Add("Location", typeof(string));
                    _dtInsert.Columns.Add("ShiftName", typeof(string));
                    _dtInsert.Columns.Add("ShiftTypeID", typeof(int));
                    _dtInsert.Columns.Add("DutyDate", typeof(DateTime));
                    _dtInsert.Columns.Add("StartDateTime", typeof(DateTime));
                    _dtInsert.Columns.Add("EndDateTime", typeof(DateTime));
                    _dtInsert.Columns.Add("DateCreated", typeof(DateTime));
                    _dtInsert.Columns.Add("DateModified", typeof(DateTime));

                    //***********构建表头End***********

                    //先循环行再循环列，从第3行开始 i=2（第1行是表头，第2行是例子，第3行开始）
                    for (int i = 2; i < _dtExcel.Rows.Count; i++)
                    {
                        string DutyDeptID = string.Empty,
                                DutyUID = string.Empty,
                                DutyUserName = string.Empty,
                                DutyDisplayName = string.Empty,
                                Location = string.Empty;

                        int EmptyColumns = 0;

                        //循环列
                        for (int j = 0; j < _dtExcel.Columns.Count; j++)
                        {
                            string Text = _dtExcel.Rows[0][j].toStringTrim().Split('（')[0].Split('(')[0].toStringTrim(),  //Field字段名称
                                    Value = _dtExcel.Rows[i][j].toStringTrim();


                            //判断UserName是否存在，第2行第1（j=0）个单元格(即A2)
                            if (j == 1)
                            {
                                if (_dtUsers.Select("UserName='" + Value + "'").Length == 0)
                                    MsgUserName += MicroPublic.GetLetters(j) + (i + 1) + "、";
                                else
                                {
                                    DutyUserName = Value;
                                    DutyUID = _dtUsers.Select("UserName='" + DutyUserName + "'")[0]["UID"].toStringTrim();
                                    DutyDisplayName = _dtUsers.Select("UserName='" + DutyUserName + "'")[0]["ChineseName"].toStringTrim();
                                    DutyDeptID = MicroUserInfo.GetUserDepts(DutyUID, "DeptID", "LastDept");
                                }
                            }
                            //判断Location是否存在，第3个单元格的地点是否是数据库存在的常用地点
                            else if (j == 3)
                            {
                                if (_dtLocation.Select("LocationValue='" + Value + "'").Length == 0)
                                    MsgLocation += MicroPublic.GetLetters(j) + i + "、";
                                else
                                    Location = Value;
                            }
                            //判断班次（班次不为空时）是否存在，大于第3个单元格的班次是否是数据库存在的班次
                            else if (j > 3)
                            {
                                string DutyDate = Text; //此时把标题作为日期行为0，列为j

                                //判断标题日期是否合法
                                if (DutyDate.toDateTimeBoolean())
                                {
                                    string ShiftName = Value,
                                    ShiftTypeID = string.Empty;

                                    string StartDateTime = string.Empty,
                                             EndDateTime = string.Empty;

                                    //如果单元格值不为空时判断填入的值是否合法
                                    if (!string.IsNullOrEmpty(Value))
                                    {
                                        //如果不为空同时也不为-号时
                                        if (Value.toStringTrim() != "-" && Value.toStringTrim() != "休")
                                        {
                                            if (_dtShiftType.Select("Alias='" + Value + "'").Length == 0)
                                            {
                                                var getOnDutyFormat = MicroPrivate.GetOnDutyFormat(DutyDate, Value);
                                                if (!getOnDutyFormat.IsCorrect)
                                                    MsgShiftName += MicroPublic.GetLetters(j) + (i + 1) + "、";
                                                else
                                                {
                                                    StartDateTime = getOnDutyFormat.StartDateTime;
                                                    EndDateTime = getOnDutyFormat.EndDateTime;
                                                }
                                            }
                                            else
                                            {
                                                ShiftName = Value;
                                                ShiftTypeID = _dtShiftType.Select("Alias='" + Value + "'")[0]["ShiftTypeID"].toStringTrim();

                                                StartDateTime = DutyDate + " " + _dtShiftType.Select("Alias='" + Value + "'")[0]["StartTime"].toStringTrim();
                                                EndDateTime = DutyDate + " " + _dtShiftType.Select("Alias='" + Value + "'")[0]["EndTime"].toStringTrim();

                                                //如果结束时间<开始时间，则结束时间天数+1
                                                if (EndDateTime.toDateTime("HH:mm") < StartDateTime.toDateTime("HH:mm"))
                                                    EndDateTime = EndDateTime.toDateTime("yyyy-MM-dd HH:mm").AddDays(1).toDateFormat("yyyy-MM-dd HH:mm");
                                            }
                                        }
                                        else
                                            ShiftName = Value;

                                        //ShiftName不为空时开始写数据库
                                        if (!string.IsNullOrEmpty(DutyDeptID) && !string.IsNullOrEmpty(DutyUID) && !string.IsNullOrEmpty(DutyUserName) && !string.IsNullOrEmpty(Location) && !string.IsNullOrEmpty(ShiftName))
                                        {
                                            DataRow _drInsert = _dtInsert.NewRow();
                                            _drInsert["ParentID"] = MaxDutyID;
                                            _drInsert["FormID"] = FormID.toInt();
                                            _drInsert["FormNumber"] = FormNumber;
                                            _drInsert["FormState"] = FormState;
                                            _drInsert["StateCode"] = StateCode.toInt();
                                            _drInsert["IP"] = IP;
                                            _drInsert["Applicant"] = Applicant;
                                            _drInsert["Phone"] = Phone;
                                            _drInsert["Tel"] = Tel;
                                            _drInsert["UID"] = UID;
                                            _drInsert["DisplayName"] = DisplayName;
                                            _drInsert["DeptID"] = DutyDeptID.toInt();
                                            _drInsert["DutyUID"] = DutyUID.toInt();
                                            _drInsert["DutyUserName"] = DutyUserName;
                                            _drInsert["DutyDisplayName"] = DutyDisplayName;
                                            _drInsert["Location"] = Location;
                                            _drInsert["ShiftName"] = ShiftName;
                                            _drInsert["ShiftTypeID"] = ShiftTypeID.toInt();
                                            _drInsert["DutyDate"] = DutyDate.toDateTime();

                                            if (string.IsNullOrEmpty(StartDateTime))
                                                _drInsert["StartDateTime"] = DBNull.Value;
                                            else
                                                _drInsert["StartDateTime"] = StartDateTime;

                                            if (string.IsNullOrEmpty(EndDateTime))
                                                _drInsert["EndDateTime"] = DBNull.Value;
                                            else
                                                _drInsert["EndDateTime"] = EndDateTime;

                                            _drInsert["DateModified"] = DateTime.Now; ;
                                            _dtInsert.Rows.Add(_drInsert);

                                        }
                                    }
                                    //如果为空时让空的列数+1，主要用于判断是否所有的日期班次都为空，如果都为空时禁止提交
                                    else
                                        EmptyColumns = EmptyColumns + 1;
                                }
                                else
                                    MsgDutyDate += MicroPublic.GetLetters(j) + i + "、";

                            }

                        }

                        if ((_dtExcel.Columns.Count - 4) == EmptyColumns)
                            MsgEmptyColumns += "第" + (i + 1) + "行（" + _dtExcel.Rows[i][2].toStringTrim() + "）、";

                    }

                    //如果没有报错时，把DataTable插入到数据库中HROnDutyForm
                    if (string.IsNullOrEmpty(MsgUserName) && string.IsNullOrEmpty(MsgLocation) && string.IsNullOrEmpty(MsgDutyDate) && string.IsNullOrEmpty(MsgShiftName) && string.IsNullOrEmpty(MsgEmptyColumns))
                    {
                        //DataTable insert into HROnDutyForm Successfully
                        if (MsSQLDbHelper.SqlBulkCopyInsert(_dtInsert, "HROnDutyForm"))
                        {
                            if (IsApprovalForm.toBoolean())
                            {
                                if (MicroWorkFlow.SetWorkFlow(ShortTableName, TableName, ModuleID, FormID, MaxDutyID.ToString(), FormFields))
                                    flag = MicroPublic.GetMsg("Submit");
                                else
                                    flag = MicroPublic.GetMsg("SaveWolrkFlowFailed");
                            }

                            //从临时文件移动到正式文件
                            File.Move(FullFilePath, HttpContext.Current.Server.MapPath(NewFullFilePath));

                            //执行私有方法
                            MicroPrivate.UpdatePrivateOnDuty(Action, ShortTableName, ModuleID, FormID, MaxDutyID.ToString());
                        }
                        else
                            flag = "导入失败，写入数据库失败";

                    }
                    else
                    {
                        Msg = "导入失败，详细错误：<br/>";

                        if (!string.IsNullOrEmpty(MsgUserName))
                        {
                            MsgUserName = MsgUserName.Substring(0, MsgUserName.Length - 1);
                            MsgUserName = "电脑ID不存在，单元格：" + MsgUserName + "<br/><br/>";
                        }

                        if (!string.IsNullOrEmpty(MsgLocation))
                        {
                            MsgLocation = MsgLocation.Substring(0, MsgLocation.Length - 1);
                            MsgLocation = "无法匹配地点，单元格：" + MsgLocation + "<br/><br/>";
                        }

                        if (!string.IsNullOrEmpty(MsgDutyDate))
                        {
                            MsgDutyDate = MsgDutyDate.Substring(0, MsgDutyDate.Length - 1);
                            MsgDutyDate = "标题行日期格式不正确，单元格：" + MsgDutyDate + "<br/><div class=\\\"layui-form-mid layui-word-aux \\\">*备注：标题格式 yyyy-MM-dd (星期)</div><br/><br/>";
                        }

                        if (!string.IsNullOrEmpty(MsgShiftName))
                        {
                            MsgShiftName = MsgShiftName.Substring(0, MsgShiftName.Length - 1);
                            MsgShiftName = "班次填写不正确，单元格：" + MsgShiftName + "<br/><br/>";
                        }

                        if (!string.IsNullOrEmpty(MsgEmptyColumns))
                        {
                            MsgEmptyColumns = MsgEmptyColumns.Substring(0, MsgEmptyColumns.Length - 1);
                            MsgEmptyColumns = "不能每天的班次都为空，相关行：" + MsgEmptyColumns + "<br/><br/>";
                        }

                        flag = Msg + MsgUserName + MsgLocation + MsgDutyDate + MsgShiftName + MsgEmptyColumns;
                    }

                }
                else
                    flag = "导入失败，上传的Excel内容格式不正确，请根据正确的模板格式进行导入";

                //############读取Excel数据写入数据库End###############
            }
            else
                flag = "导入失败，写入父记录失败";
        }
        catch (Exception ex) { flag = ex.ToString(); }

        return flag;
    }





    /// <summary>
    /// 修改表单（通过系统修改功能提交表单，因为现在是通过Excel导入，是否需要开发通过系统添加的功能再考虑 20210909备注）
    /// </summary>
    /// <param name="Action"></param>
    /// <param name="ShortTableName"></param>
    /// <param name="ModuleID"></param>
    /// <param name="FormID"></param>
    /// <param name="FormsID"></param>
    /// <param name="IsApprovalForm"></param>
    /// <param name="FormFields"></param>
    /// <returns></returns>
    //private static string SetModifyForm(string Action, string ShortTableName, string ModuleID, string FormID, string FormsID, string IsApprovalForm, string FormFields)
    //{
    //    string flag = MicroPublic.GetMsg("SubmitURLError"),
    //            TableName = MicroPublic.GetTableName(ShortTableName);

    //    return flag;
    //}


    private static string SetModifyExcelForm(string Action, string ShortTableName, string ModuleID, string FormID, string FormsID, string IsApprovalForm, string FilePath, string FormFields)
    {
        string flag = MicroPublic.GetMsg("SubmitURLError"),
                TableName = MicroPublic.GetTableName(ShortTableName);

        try
        {
            dynamic json = JToken.Parse(FormFields) as dynamic;

            string ParentID = string.Empty; string FormNumber = string.Empty; string FormState = string.Empty; string StateCode = string.Empty; string IP = string.Empty; string Applicant = string.Empty; string Phone = string.Empty; string Tel = string.Empty; string UID = string.Empty; string DisplayName = string.Empty; string DeptID = string.Empty; string OvertimeDate = string.Empty;

            ParentID = "0";

            FormNumber = json["txtFormNumber"];
            FormNumber = FormNumber.toStringTrim();
            FormNumber = MicroPublic.GetBuiltinFunction(Action, FormNumber, "", ShortTableName, TableName, FormID);

            StateCode = "0";
            FormState = MicroWorkFlow.GetApprovalState(StateCode.toInt()); //0 = 等待审批[Waiting]，-3 = 填写申请[Fill in]
            IP = MicroPublic.GetClientIP();
            Applicant = MicroUserInfo.GetUserInfo("DisplayName");
            Phone = MicroUserInfo.GetUserInfo("WorkMobilePhone");
            Tel = MicroUserInfo.GetUserInfo("WorkTel");
            UID = MicroUserInfo.GetUserInfo("UID");
            DisplayName = MicroUserInfo.GetUserInfo("DisplayName");
            DeptID = MicroUserInfo.GetUserInfo("LastDeptID");

            //由临时文件路径映射到正式文件路径
            string NewFullFilePath = "/Resource/UploadFiles/Forms/HROnDuty/" + MicroPublic.GetSplitLastStr(FilePath, '/');

            string _sql = "update " + TableName + " set ParentID=@ParentID, FormID=@FormID, FormNumber=@FormNumber, FormState=@FormState, StateCode=@StateCode, IP=@IP, Applicant=@Applicant, Phone=@Phone, Tel=@Tel, UID=@UID, DisplayName=@DisplayName, DeptID=@DeptID, FilePath=@FilePath, DateModified=@DateModified where ParentID=0 and DutyID=@DutyID ";

            SqlParameter[] _sp = { new SqlParameter("@ParentID",SqlDbType.Int),
                         new SqlParameter("@FormID",SqlDbType.Int),
                         new SqlParameter("@FormNumber",SqlDbType.VarChar,200),
                         new SqlParameter("@FormState",SqlDbType.NVarChar,200),
                         new SqlParameter("@StateCode",SqlDbType.Int),
                         new SqlParameter("@IP",SqlDbType.VarChar,50),
                         new SqlParameter("@Applicant",SqlDbType.NVarChar,200),
                         new SqlParameter("@Phone",SqlDbType.VarChar,30),
                         new SqlParameter("@Tel",SqlDbType.VarChar,30),
                         new SqlParameter("@UID",SqlDbType.Int),
                         new SqlParameter("@DisplayName",SqlDbType.NVarChar,200),
                         new SqlParameter("@DeptID",SqlDbType.Int),
                         new SqlParameter("@FilePath",SqlDbType.VarChar),
                         new SqlParameter("@DateModified",SqlDbType.DateTime),
                         new SqlParameter("@DutyID",SqlDbType.Int)
                        };

            _sp[0].Value = ParentID.toInt();
            _sp[1].Value = FormID.toInt();
            _sp[2].Value = FormNumber;
            _sp[3].Value = FormState;
            _sp[4].Value = StateCode.toInt();
            _sp[5].Value = IP;
            _sp[6].Value = Applicant;
            _sp[7].Value = Phone;
            _sp[8].Value = Tel;
            _sp[9].Value = UID.toInt();
            _sp[10].Value = DisplayName;
            _sp[11].Value = DeptID.toInt();
            _sp[12].Value = NewFullFilePath.toStringTrim();
            _sp[13].Value = DateTime.Now;
            _sp[14].Value = FormsID.toInt();


            if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
            {
                //############把原来的子记录设置为无效和删除再重新插入新的Start###########
                string _sql2 = "update " + TableName + " set Invalid=1, Del=1, DateModified=getdate() where Invalid=0 and Del=0 and ParentID=@ParentID";
                SqlParameter[] _sp2 = { new SqlParameter("@ParentID",SqlDbType.Int) };
                _sp2[0].Value = FormsID.toInt();

                MsSQLDbHelper.ExecuteSql(_sql2, _sp2);

                //############把原来的子记录设置为无效和删除再重新插入新的End###########



                //############读取Excel数据写入数据库Start###############
                int MaxDutyID = FormsID.toInt();  //取得DutyID作为ParentID

                string FullFilePath = HttpContext.Current.Server.MapPath(FilePath);  //临时文件的Excel路径

                //读取Excel生成DataTable
                DataTable _dtExcel = MsSQLDbHelper.QueryExcel(FullFilePath).Tables[0];

                string Msg = string.Empty, MsgUserName = string.Empty, MsgChineseName = string.Empty, MsgLocation = string.Empty, MsgDutyDate = string.Empty, MsgShiftName = string.Empty, MsgEmptyColumns = string.Empty;

                //Rows大于1代表从第3行（0起步）开始读取数据，Columns大于3代表从第5列开始取数
                //因为Rows1是标题，Rows2是例子
                //因为Columns前4列是基本数据
                if (_dtExcel != null && _dtExcel.Rows.Count > 1 && _dtExcel.Columns.Count > 3)
                {
                    DataTable _dtUsers = MicroDataTable.GetDataTable("Use");
                    DataTable _dtLocation = MicroDataTable.GetDataTable("CommonLocations");
                    DataTable _dtShiftType = MicroDataTable.GetDataTable("ShiftType");

                    //***********构建表头Start***********
                    //要插入数据的字段
                    DataTable _dtInsert = new DataTable();
                    _dtInsert.Columns.Add("ParentID", typeof(int));
                    _dtInsert.Columns.Add("FormID", typeof(int));
                    _dtInsert.Columns.Add("FormNumber", typeof(string));
                    _dtInsert.Columns.Add("FormState", typeof(string));
                    _dtInsert.Columns.Add("StateCode", typeof(int));  //..
                    _dtInsert.Columns.Add("IP", typeof(string));
                    _dtInsert.Columns.Add("Applicant", typeof(string));
                    _dtInsert.Columns.Add("Phone", typeof(string));
                    _dtInsert.Columns.Add("Tel", typeof(string));
                    _dtInsert.Columns.Add("UID", typeof(int));
                    _dtInsert.Columns.Add("DisplayName", typeof(string));
                    _dtInsert.Columns.Add("DeptID", typeof(int));
                    _dtInsert.Columns.Add("DutyUID", typeof(int));
                    _dtInsert.Columns.Add("DutyUserName", typeof(string));
                    _dtInsert.Columns.Add("DutyDisplayName", typeof(string));
                    _dtInsert.Columns.Add("Location", typeof(string));
                    _dtInsert.Columns.Add("ShiftName", typeof(string));
                    _dtInsert.Columns.Add("ShiftTypeID", typeof(int));
                    _dtInsert.Columns.Add("DutyDate", typeof(DateTime));
                    _dtInsert.Columns.Add("StartDateTime", typeof(DateTime));
                    _dtInsert.Columns.Add("EndDateTime", typeof(DateTime));
                    _dtInsert.Columns.Add("DateCreated", typeof(DateTime));
                    _dtInsert.Columns.Add("DateModified", typeof(DateTime));

                    //***********构建表头End***********

                    //先循环行再循环列，从第3行开始 i=2（第1行是表头，第2行是例子，第3行开始）
                    for (int i = 2; i < _dtExcel.Rows.Count; i++)
                    {
                        string DutyDeptID = string.Empty,
                                DutyUID = string.Empty,
                                DutyUserName = string.Empty,
                                DutyDisplayName = string.Empty,
                                Location = string.Empty;

                        int EmptyColumns = 0;

                        //循环列
                        for (int j = 0; j < _dtExcel.Columns.Count; j++)
                        {
                            string Text = _dtExcel.Rows[0][j].toStringTrim().Split('（')[0].Split('(')[0].toStringTrim(),  //Field字段名称
                                    Value = _dtExcel.Rows[i][j].toStringTrim();


                            //判断UserName是否存在，第2行第1（j=0）个单元格(即A2)
                            if (j == 1)
                            {
                                if (_dtUsers.Select("UserName='" + Value + "'").Length == 0)
                                    MsgUserName += MicroPublic.GetLetters(j) + (i + 1) + "、";
                                else
                                {
                                    DutyUserName = Value;
                                    DutyUID = _dtUsers.Select("UserName='" + DutyUserName + "'")[0]["UID"].toStringTrim();
                                    DutyDisplayName = _dtUsers.Select("UserName='" + DutyUserName + "'")[0]["ChineseName"].toStringTrim();
                                    DutyDeptID = MicroUserInfo.GetUserDepts(DutyUID, "DeptID", "LastDept");
                                }
                            }
                            //判断Location是否存在，第3个单元格的地点是否是数据库存在的常用地点
                            else if (j == 3)
                            {
                                if (_dtLocation.Select("LocationValue='" + Value + "'").Length == 0)
                                    MsgLocation += MicroPublic.GetLetters(j) + i + "、";
                                else
                                    Location = Value;
                            }
                            //判断班次（班次不为空时）是否存在，大于第3个单元格的班次是否是数据库存在的班次
                            else if (j > 3)
                            {
                                string DutyDate = Text; //此时把标题作为日期行为0，列为j

                                //判断标题日期是否合法
                                if (DutyDate.toDateTimeBoolean())
                                {
                                    string ShiftName = Value,
                                    ShiftTypeID = string.Empty;

                                    string StartDateTime = null,
                                             EndDateTime = null;

                                    //如果单元格值不为空时判断填入的值是否合法
                                    if (!string.IsNullOrEmpty(Value))
                                    {
                                        //如果不为空同时也不为-号时
                                        if (Value.toStringTrim() != "-" && Value.toStringTrim() != "休")
                                        {
                                            if (_dtShiftType.Select("Alias='" + Value + "'").Length == 0)
                                            {
                                                var getOnDutyFormat = MicroPrivate.GetOnDutyFormat(DutyDate, Value);
                                                if (!getOnDutyFormat.IsCorrect)
                                                    MsgShiftName += MicroPublic.GetLetters(j) + i + 1 + "、";
                                                else
                                                {
                                                    StartDateTime = getOnDutyFormat.StartDateTime;
                                                    EndDateTime = getOnDutyFormat.EndDateTime;
                                                }
                                            }
                                            else
                                            {
                                                ShiftName = Value;
                                                ShiftTypeID = _dtShiftType.Select("Alias='" + Value + "'")[0]["ShiftTypeID"].toStringTrim();

                                                StartDateTime = DutyDate + " " + _dtShiftType.Select("Alias='" + Value + "'")[0]["StartTime"].toStringTrim();
                                                EndDateTime = DutyDate + " " + _dtShiftType.Select("Alias='" + Value + "'")[0]["EndTime"].toStringTrim();

                                                //如果结束时间<开始时间，则结束时间天数+1
                                                if (EndDateTime.toDateTime("HH:mm") < StartDateTime.toDateTime("HH:mm"))
                                                    EndDateTime = EndDateTime.toDateTime("yyyy-MM-dd HH:mm").AddDays(1).toDateFormat("yyyy-MM-dd HH:mm");
                                            }
                                        }
                                        else
                                            ShiftName = Value;

                                        //ShiftName不为空时开始写数据库
                                        if (!string.IsNullOrEmpty(DutyDeptID) && !string.IsNullOrEmpty(DutyUID) && !string.IsNullOrEmpty(DutyUserName) && !string.IsNullOrEmpty(Location) && !string.IsNullOrEmpty(ShiftName))
                                        {
                                            DataRow _drInsert = _dtInsert.NewRow();
                                            _drInsert["ParentID"] = MaxDutyID;
                                            _drInsert["FormID"] = FormID.toInt();
                                            _drInsert["FormNumber"] = FormNumber;
                                            _drInsert["FormState"] = FormState;
                                            _drInsert["StateCode"] = StateCode.toInt();
                                            _drInsert["IP"] = IP;
                                            _drInsert["Applicant"] = Applicant;
                                            _drInsert["Phone"] = Phone;
                                            _drInsert["Tel"] = Tel;
                                            _drInsert["UID"] = UID;
                                            _drInsert["DisplayName"] = DisplayName;
                                            _drInsert["DeptID"] = DutyDeptID.toInt();
                                            _drInsert["DutyUID"] = DutyUID.toInt();
                                            _drInsert["DutyUserName"] = DutyUserName;
                                            _drInsert["DutyDisplayName"] = DutyDisplayName;
                                            _drInsert["Location"] = Location;
                                            _drInsert["ShiftName"] = ShiftName;
                                            _drInsert["ShiftTypeID"] = ShiftTypeID.toInt();
                                            _drInsert["DutyDate"] = DutyDate.toDateTime();

                                            if (string.IsNullOrEmpty(StartDateTime))
                                                _drInsert["StartDateTime"] = DBNull.Value;
                                            else
                                                _drInsert["StartDateTime"] = StartDateTime;

                                            if (string.IsNullOrEmpty(EndDateTime))
                                                _drInsert["EndDateTime"] = DBNull.Value;
                                            else
                                                _drInsert["EndDateTime"] = EndDateTime;

                                            _drInsert["DateCreated"] = DateTime.Now;
                                            _drInsert["DateModified"] = DateTime.Now; ;
                                            _dtInsert.Rows.Add(_drInsert);

                                        }
                                    }
                                    //如果为空时让空的列数+1，主要用于判断是否所有的日期班次都为空，如果都为空时禁止提交
                                    else
                                        EmptyColumns = EmptyColumns + 1;
                                }
                                else
                                    MsgDutyDate += MicroPublic.GetLetters(j) + i + "、";

                            }

                        }

                        if ((_dtExcel.Columns.Count - 4) == EmptyColumns)
                            MsgEmptyColumns += "第" + (i + 1) + "行（" + _dtExcel.Rows[i][2].toStringTrim() + "）、";

                    }

                    //如果没有报错时，把DataTable插入到数据库中HROnDutyForm
                    if (string.IsNullOrEmpty(MsgUserName) && string.IsNullOrEmpty(MsgLocation) && string.IsNullOrEmpty(MsgDutyDate) && string.IsNullOrEmpty(MsgShiftName) && string.IsNullOrEmpty(MsgEmptyColumns))
                    {
                        //DataTable insert into HROnDutyForm Successfully
                        if (MsSQLDbHelper.SqlBulkCopyInsert(_dtInsert, "HROnDutyForm"))
                        {
                            if (IsApprovalForm.toBoolean())
                            {
                                if (MicroWorkFlow.SetModifyWorkFlow(ShortTableName, TableName, ModuleID, FormID, MaxDutyID.ToString(), FormFields))
                                    flag = MicroPublic.GetMsg("Submit");
                                else
                                    flag = MicroPublic.GetMsg("SaveWolrkFlowFailed");
                            }

                            //从临时文件移动到正式文件
                            File.Move(FullFilePath, HttpContext.Current.Server.MapPath(NewFullFilePath));

                            //执行私有方法
                            MicroPrivate.UpdatePrivateOnDuty(Action, ShortTableName, ModuleID, FormID, MaxDutyID.ToString());
                        }
                        else
                            flag = "导入失败，写入数据库失败";

                    }
                    else
                    {
                        Msg = "导入失败，详细错误：<br/>";

                        if (!string.IsNullOrEmpty(MsgUserName))
                        {
                            MsgUserName = MsgUserName.Substring(0, MsgUserName.Length - 1);
                            MsgUserName = "电脑ID不存在，单元格：" + MsgUserName + "<br/><br/>";
                        }

                        if (!string.IsNullOrEmpty(MsgLocation))
                        {
                            MsgLocation = MsgLocation.Substring(0, MsgLocation.Length - 1);
                            MsgLocation = "无法匹配地点，单元格：" + MsgLocation + "<br/><br/>";
                        }

                        if (!string.IsNullOrEmpty(MsgDutyDate))
                        {
                            MsgDutyDate = MsgDutyDate.Substring(0, MsgDutyDate.Length - 1);
                            MsgDutyDate = "标题行日期格式不正确，单元格：" + MsgDutyDate + "<br/><div class=\\\"layui-form-mid layui-word-aux \\\">*备注：标题格式 yyyy-MM-dd (星期)</div><br/><br/>";
                        }

                        if (!string.IsNullOrEmpty(MsgShiftName))
                        {
                            MsgShiftName = MsgShiftName.Substring(0, MsgShiftName.Length - 1);
                            MsgShiftName = "班次填写不正确，单元格：" + MsgShiftName + "<br/><br/>";
                        }

                        if (!string.IsNullOrEmpty(MsgEmptyColumns))
                        {
                            MsgEmptyColumns = MsgEmptyColumns.Substring(0, MsgEmptyColumns.Length - 1);
                            MsgEmptyColumns = "不能每天的班次都为空，相关行：" + MsgEmptyColumns + "<br/><br/>";
                        }

                        flag = Msg + MsgUserName + MsgLocation + MsgDutyDate + MsgShiftName + MsgEmptyColumns;
                    }

                }
                else
                    flag = "导入失败，上传的Excel内容格式不正确，请根据正确的模板格式进行导入";

                //############读取Excel数据写入数据库End###############
            }
            else
                flag = "导入失败，写入父记录失败";
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