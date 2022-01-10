using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroDTHelper;
using MicroUserHelper;
using MicroFormHelper;
using MicroHRHelper;
using MicroPublicHelper;
using Newtonsoft.Json.Linq;

namespace MicroPrivateHelper
{

    /// <summary>
    /// 站点私有方法，主要是针对性的私有方法不进行共用
    /// </summary>
    public class MicroPrivate
    {

        /// <summary>
        /// 操作额外函数
        /// </summary>
        /// <param name="ExtraFunction"></param>
        /// <returns></returns>
        public static void CtrlVoidExtraFunction(string FieldValue, string ExtraFunction)
        {
            switch (ExtraFunction)
            {
                case "SetUsersGlobalTips":
                    if (FieldValue.toBoolean())
                        SetPrivateUsersGlobalTips();
                    break;
            }
        }


        /// <summary>
        /// 针对信息表【Information】传入表名和需要返回的排序的字段名，返回当前排序字段的值+1 (置顶或取消置顶排序用)
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <param name="SortField">表中的排序字段名</param>
        /// <returns></returns>
        public static string GetPrivateSortField(string TableName, string SortField, string WhereField)
        {
            string flag = string.Empty, CurrYearMonth = DateTime.Now.ToString("yyyyMM");

            try
            {
                string _sql = "select " + SortField + " from " + TableName + " where Invalid=0 and Del=0 and " + WhereField + "=0 and " + SortField + " like '" + CurrYearMonth + "%'";

                flag = MicroPublic.GetSingleField(_sql, 0);
                if (string.IsNullOrEmpty(flag))
                    flag = CurrYearMonth + "0001";
                else
                    flag = (flag.toInt() + 1).ToString();
            }
            catch { }

            return flag;
        }

        /// <summary>
        /// 在发布信息打开了全局提示时，设置所有用户全局提示为True
        /// </summary>
        private static void SetPrivateUsersGlobalTips()
        {
            string _sql = "update UserInfo set IsGlobalTips=1 where Invalid=0 and Del=0";

            MsSQLDbHelper.ExecuteSql(_sql);

        }


        /// <summary>
        /// 在提交表单之前执行私有方法。这个方法是放在CtrlMicroForm.ashx提交之前（提交表单或修改表单都执行），返回flag前4位等于 True时触发，如休假申请提交前需要判断是否重复
        /// </summary>
        /// <param name="Action"></param>
        /// <param name="ShortTableName"></param>
        /// <param name="ModuleID"></param>
        /// <param name="FormID"></param>
        /// <param name="IsApprovalForm"></param>
        /// <param name="FormFields"></param>
        /// <param name="FormsID"></param>
        /// <returns></returns>
        public static string SubmitFormBeforeExecutePrivate(string Action, string ShortTableName, string ModuleID, string FormID, Boolean IsApprovalForm, string FormFields, string FormsID = "")
        {
            string flag = "保存失败，执行前置方法失败。";

            if (!string.IsNullOrEmpty(Action) && !string.IsNullOrEmpty(ShortTableName))
            {
                Action = Action.ToLower();
                ShortTableName = ShortTableName.ToLower();

                if ((Action == "add" || Action == "modify") && ShortTableName == "leave")
                    flag = CheckRepeatLeave(Action, ShortTableName, ModuleID, FormID, IsApprovalForm, FormFields, FormsID);
                else
                    flag = MicroPublic.GetMsg("Save");
            }

            return flag;
        }


        /// <summary>
        /// 提交表单成功后执行私有方法。这个方法是放在CtrlMicroForm.ashx提交成功后（提交表单或修改表单都执行），返回flag前4位等于 True时触发，如休假申请（年休假）提交后需扣除假期
        /// </summary>
        /// <param name="Action"></param>
        /// <param name="ShortTableName"></param>
        /// <param name="ModuleID"></param>
        /// <param name="FormID"></param>
        /// <param name="IsApprovalForm"></param>
        /// <param name="FormFields"></param>
        /// <param name="FormsID"></param>
        /// <returns></returns>
        public static Boolean SubmitFormAfterExecutePrivate(string Action, string ShortTableName, string ModuleID, string FormID, Boolean IsApprovalForm, string FormFields, string FormsID = "")
        {
            Boolean flag = false;

            if (!string.IsNullOrEmpty(Action) && !string.IsNullOrEmpty(ShortTableName))
            {
                Action = Action.ToLower();
                ShortTableName = ShortTableName.ToLower();

                if ((Action == "add" || Action == "modify" || Action == "return" || Action == "batchreturn" || Action == "withdrawal") && ShortTableName == "leave")
                    flag = UpdatePersonalHoliday(Action, ShortTableName, ModuleID, FormID, IsApprovalForm, FormFields, FormsID);
            }

            return flag;
        }


        /// <summary>
        /// 更新个人假期数据
        /// </summary>
        /// <param name="Action"></param>
        /// <param name="ShortTableName"></param>
        /// <param name="ModuleID"></param>
        /// <param name="FormID"></param>
        /// <param name="IsApprovalForm"></param>
        /// <param name="Fields"></param>
        /// <returns></returns>
        private static Boolean UpdatePersonalHoliday(string Action, string ShortTableName, string ModuleID, string FormID, Boolean IsApprovalForm, string FormFields, string FormsID)
        {
            Boolean flag = false;
            try
            {
                string TableName = MicroPublic.GetTableName(ShortTableName);

                if (Action == "add")
                    FormsID = MsSQLDbHelper.GetMaxStr("LeaveID", TableName);
                else
                {
                    //如果FormsID为空时尝试从表单字段中获取
                    if (string.IsNullOrEmpty(FormsID) && !string.IsNullOrEmpty(FormFields))
                    {
                        dynamic json = JToken.Parse(FormFields) as dynamic;
                        FormsID = json["txtLeaveID"];
                    }
                }

                //FormsID不等于零，说明有记录时
                if (FormsID.toInt() != 0)
                {
                    //根据FormsID得到该记录
                    string _sql = "select * from " + TableName + " where Invalid=0 and Del=0 and LeaveID=@LeaveID";
                    SqlParameter[] _sp = { new SqlParameter("@LeaveID", SqlDbType.Int) };
                    _sp[0].Value = FormsID.toInt();

                    DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        int LeaveUID = _dt.Rows[0]["LeaveUID"].toInt(),  //个人休假UID 
                            HolidayTypeID = _dt.Rows[0]["HolidayTypeID"].toInt();  //休假类型ID

                        decimal LeaveDays = _dt.Rows[0]["LeaveDays"].toDecimal(),  //休假天数
                            LeaveHour = _dt.Rows[0]["LeaveHour"].toDecimal();  //休假小时数（除事假外暂时用不到）

                        //不为代休假时
                        if (HolidayTypeID != 2)
                        {
                            //操作运算符
                            string Operator = string.Empty;

                            //在新增休假记录或修改休假记录提交表单时，从个人假期表中扣除假期数据
                            if (Action == "add" || Action == "modify")
                                Operator = "-";

                            //驳回申请或撤回时，还原被扣除的假期数据 （撤回逻辑还没有 注：20210316）
                            if (Action == "return" || Action == "batchreturn" || Action == "withdrawal")
                                Operator = "+";

                            //当运算符不为空，并且休假天数据大于0时，执行更新操作
                            if (!string.IsNullOrEmpty(Operator) && LeaveDays.toDecimal() > 0)
                            {
                                string _sql2 = "update HRPersonalHoliday set Days=Days " + Operator + LeaveDays.toDecimal() + " , DateModified=@DateModified where PersonalHolidayID =(select top 1 PersonalHolidayID from HRPersonalHoliday where Invalid=0 and Del=0 and HolidayTypeID=@HolidayTypeID and UID=@UID order by HolidayOfDate desc)";

                                SqlParameter[] _sp2 = { new SqlParameter("@DateModified",SqlDbType.DateTime),
                                            new SqlParameter("@HolidayTypeID",SqlDbType.Int),
                                            new SqlParameter("@UID",SqlDbType.Int) };

                                _sp2[0].Value = DateTime.Now;
                                _sp2[1].Value = HolidayTypeID;
                                _sp2[2].Value = LeaveUID;

                                if (MsSQLDbHelper.ExecuteSql(_sql2, _sp2) > 0)
                                    flag = true;
                            }

                        }
                    }
                }
            }
            catch { }

            return flag;

        }



        /// <summary>
        /// 在第二次插入加班数据时按加班日期的先后顺序更新数据（更新比当前日期大的数据）
        /// </summary>
        /// <param name="Action"></param>
        /// <param name="ShortTableName"></param>
        /// <param name="ModuleID"></param>
        /// <param name="FormID"></param>
        /// <param name="OvertimeUIDs"></param>
        /// <param name="OvertimeDate"></param>
        /// <param name="StartTime"></param>
        /// <param name="OvertimeHour"></param>
        /// <returns></returns>
        public static Boolean AddUpdateOvertimeTotal(string Action, string ShortTableName, string ModuleID, string FormID, string OvertimeUIDs, string OvertimeDate, string StartTime, decimal OvertimeHour)
        {
            Boolean flag = false;
            int Num = 0;

            string TableName = MicroPublic.GetTableName(ShortTableName);
            string[] ArrOvertimeUIDs = OvertimeUIDs.Split(',');

            if (ArrOvertimeUIDs.Length > 0)
            {
                for (int i = 0; i < ArrOvertimeUIDs.Length; i++)
                {
                    int OvertimeUID = ArrOvertimeUIDs[i].toInt();

                    //默认月度
                    string StartDateTime = OvertimeDate.toDateMFirstDay() + " 00:00:00.000",
                        EndDateTime = OvertimeDate.toDateMLastDay() + " 23:59:59.998";

                    var getUserWorkHourSystem = MicroHR.GetUserWorkHourSystem(OvertimeUID, OvertimeDate);
                    string WorkHourSysID = getUserWorkHourSystem.WorkHourSysID;

                    //综合工时制时为季度
                    if (WorkHourSysID == "2")
                    {
                        StartDateTime = OvertimeDate.toDateQFirstDay() + " 00:00:00.000";
                        EndDateTime = OvertimeDate.toDateQLastDay() + " 23:59:59.998";
                    }

                    //更新大于当前日期的记录 OvertimeDate>@OvertimeDate
                    string _sql = " update " + TableName + " set OvertimeTotal = OvertimeTotal + " + OvertimeHour + ", ExceptDaiXiu = ExceptDaiXiu + " + OvertimeHour + " where Invalid=0 and Del=0 " +
                           " and OvertimeID in (select OvertimeID from " + TableName + " where Invalid=0 and Del=0 and ParentID<>0 and OvertimeDate>@OvertimeDate and OvertimeDate between @StartDateTime and @EndDateTime and OvertimeUID=@OvertimeUID)";
                    SqlParameter[] _sp = {
                        new SqlParameter("@OvertimeDate",SqlDbType.VarChar,50),
                        new SqlParameter("@StartDateTime",SqlDbType.VarChar,50),
                        new SqlParameter("@EndDateTime",SqlDbType.VarChar,50),
                        new SqlParameter("@OvertimeUID",SqlDbType.Int)
                         };

                    _sp[0].Value = OvertimeDate;
                    _sp[1].Value = StartDateTime;
                    _sp[2].Value = EndDateTime;
                    _sp[3].Value = OvertimeUID;

                    if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
                        Num = Num + 1;
                }

                //更新等于当前日期并且StartTime大于当前StartTime的(可以用or关键字和上面语句合并)
                string _sql2 = " update " + TableName + " set OvertimeTotal=OvertimeTotal + " + OvertimeHour + ", ExceptDaiXiu = ExceptDaiXiu + " + OvertimeHour + "  where Invalid=0 and Del=0" +
                    " and ParentID<>0 and OvertimeDate = @OvertimeDate and StartTime>@StartTime and OvertimeUID in(" + OvertimeUIDs + ")";
                SqlParameter[] _sp2 = {
                               new SqlParameter("@OvertimeDate",SqlDbType.VarChar,50),
                               new SqlParameter("@StartTime",SqlDbType.VarChar,50),
                             };

                _sp2[0].Value = OvertimeDate;
                _sp2[1].Value = StartTime;

                if (MsSQLDbHelper.ExecuteSql(_sql2, _sp2) > 0)
                    Num = Num + 1;

                if (Num > 0)
                    flag = true;
            }

            return flag;

        }


        /// <summary>
        /// 在修改加班数据时按加班日期的先后顺序更新数据（更新比当前日期大的数据）
        /// </summary>
        /// <param name="Action"></param>
        /// <param name="ShortTableName"></param>
        /// <param name="ModuleID"></param>
        /// <param name="FormID"></param>
        /// <param name="FormsID"></param>
        /// <param name="OvertimeID"></param>
        /// <param name="OvertimeUID"></param>
        /// <param name="WorkHourSysID"></param>
        /// <param name="OldOvertimeDate"></param>
        /// <param name="NewOvertimeDate"></param>
        /// <param name="OldStartTime"></param>
        /// <param name="NewStartTime"></param>
        /// <param name="OldOvertimeHour"></param>
        /// <param name="NewOvertimeHour"></param>
        /// <param name="AlreadyDaiXiuOvertime"></param>
        /// <returns></returns>
        public static Boolean ModifyUpdateOvertimeTotal(string Action, string ShortTableName, string ModuleID, string FormID, string FormsID, string OvertimeID, string OvertimeUID, string WorkHourSysID, string OldOvertimeDate, string NewOvertimeDate, string OldStartTime, string NewStartTime, decimal OldOvertimeHour, decimal NewOvertimeHour, decimal AlreadyDaiXiuOvertime)
        {
            Boolean flag = false;
            int Num = 0;

            string TableName = MicroPublic.GetTableName(ShortTableName);

            string OvertimeDate = OldOvertimeDate,
                StartDate = OvertimeDate.toDateMFirstDay(),
                EndDate = OvertimeDate.toDateMLastDay();

            if (WorkHourSysID == "2")
            {
                StartDate = OvertimeDate.toDateQFirstDay();
                EndDate = OvertimeDate.toDateQLastDay();
            }

            //#1. 如果新旧日期相等时
            if (NewOvertimeDate.toDateTime() == OldOvertimeDate.toDateTime())
            {
                //#1.1 如果新旧开始时间相等时
                if (NewStartTime.toDateTime("HH:mm") == OldStartTime.toDateTime("HH:mm"))
                {
                    //if(NewVal>OldVal)
                    //>Date的数据 OldTotal-OldVal+NewVal

                    //if(NewVal<OldVal)
                    //>Date的数据 OldTotal-OldVal+NewVal

                    string _sql = " update " + TableName + " set " +
                        " OvertimeTotal = OvertimeTotal - " + OldOvertimeHour + "+" + NewOvertimeHour + ", " +
                        " AlreadyDaiXiu = " + AlreadyDaiXiuOvertime + "," +   //原来没有更新AlreadyDaiXiu这条值 20210609新增                                                            
                        " ExceptDaiXiu = (OvertimeTotal - " + OldOvertimeHour + " + " + NewOvertimeHour + ") - " + AlreadyDaiXiuOvertime +    //Old " ExceptDaiXiu = ExceptDaiXiu - " + OldOvertimeHour + "+" + NewOvertimeHour +
                        " where Invalid=0 and Del=0 " +
                        " and OvertimeID in (select OvertimeID from " + TableName + " where Invalid=0 and Del=0 " +
                        " and OvertimeUID=@OvertimeUID and OvertimeID <> @OvertimeID" +
                        " and OvertimeDate between @StartDate and @EndDate " +
                        " and ((OvertimeDate>@OldOvertimeDate) or (OvertimeDate=@OldOvertimeDate and StartTime>@OldStartTime))" +
                        " )";

                    SqlParameter[] _sp = {
                        new SqlParameter("@OvertimeUID",SqlDbType.Int),
                        new SqlParameter("@OvertimeID",SqlDbType.Int),
                        new SqlParameter("@StartDate",SqlDbType.VarChar,50),
                        new SqlParameter("@EndDate",SqlDbType.VarChar,50),
                        new SqlParameter("@OldOvertimeDate",SqlDbType.VarChar,50),
                        new SqlParameter("@OldStartTime",SqlDbType.VarChar,50),
                         };

                    _sp[0].Value = OvertimeUID;
                    _sp[1].Value = OvertimeID;
                    _sp[2].Value = StartDate;
                    _sp[3].Value = EndDate;
                    _sp[4].Value = OldOvertimeDate;
                    _sp[5].Value = OldStartTime;

                    if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
                        Num = Num + 1;


                }

                //#1.2 否则如果新时间大于旧时间
                else if (NewStartTime.toDateTime("HH:mm") > OldStartTime.toDateTime("HH:mm"))
                {
                    //if(NewVal==OldVal)
                    //NTime~OTime 之间的数据-OldVal
                    //>NTime的数据 不用变

                    //if(NewVal>OldVal)
                    //NTime~OTime 之间的数据-OldVal
                    //>NTime的数据 OldTotal+(NewVal-OldVal)

                    //if(NewVal<OldVal)
                    //NTime~OTime 之间的数据-OldVal
                    //>NTime的数据 OldTotal-(OldVal-NewVal)


                    //#以上3种情况相同点：NTime~OTime 之间的数据-OldVal
                    string _sql = " update " + TableName + " set" +
                        " OvertimeTotal = OvertimeTotal - " + OldOvertimeHour + "," +
                        " AlreadyDaiXiu = " + AlreadyDaiXiuOvertime + "," +   //原来没有更新AlreadyDaiXiu这条值 20210609新增   
                        " ExceptDaiXiu = (OvertimeTotal - " + OldOvertimeHour + ") - " + AlreadyDaiXiuOvertime + //" ExceptDaiXiu = ExceptDaiXiu - " + OldOvertimeHour + 
                        " where Invalid=0 and Del=0 " +
                        " and OvertimeID in (select OvertimeID from " + TableName + " where Invalid=0 and Del=0 " +
                        " and OvertimeUID = @OvertimeUID and OvertimeID <> @OvertimeID" +
                        " and OvertimeDate = @OldOvertimeDate " +  //(只改变当天内且时间在新旧范围之间的，大于当天的保持不变)
                        " and StartTime between @OldStartTime and @NewStartTime" +  //小的日期要在前面
                        " )";

                    SqlParameter[] _sp = {
                        new SqlParameter("@OvertimeUID",SqlDbType.Int),
                        new SqlParameter("@OvertimeID",SqlDbType.Int),
                        new SqlParameter("@OldOvertimeDate",SqlDbType.VarChar,50),
                        new SqlParameter("@OldStartTime",SqlDbType.VarChar,50),
                        new SqlParameter("@NewStartTime",SqlDbType.VarChar,50),
                         };

                    _sp[0].Value = OvertimeUID;
                    _sp[1].Value = OvertimeID;
                    _sp[2].Value = OldOvertimeDate;
                    _sp[3].Value = OldStartTime;
                    _sp[4].Value = NewStartTime;

                    if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
                        Num = Num + 1;


                    //#if(NewVal>OldVal)
                    //>NTime的数据 OldTotal+(NewVal-OldVal)
                    if (NewOvertimeHour > OldOvertimeHour)
                    {
                        string _sql2 = " update " + TableName + " set" +
                            " OvertimeTotal = OvertimeTotal + (" + NewOvertimeHour + "-" + OldOvertimeHour + ")," +
                            " AlreadyDaiXiu = " + AlreadyDaiXiuOvertime + "," +   //原来没有更新AlreadyDaiXiu这条值 20210609新增  
                            " ExceptDaiXiu = (OvertimeTotal + (" + NewOvertimeHour + "-" + OldOvertimeHour + ")) - " + AlreadyDaiXiuOvertime +  //" ExceptDaiXiu = ExceptDaiXiu + (" + NewOvertimeHour + "-" + OldOvertimeHour + ")" +
                            " where Invalid=0 and Del=0 " +
                            " and OvertimeID in (select OvertimeID from " + TableName + " where Invalid=0 and Del=0 " +
                            " and OvertimeUID=@OvertimeUID and OvertimeID <> @OvertimeID" +
                            " and OvertimeDate between @StartDate and @EndDate " +
                            " and ((OvertimeDate>@OldOvertimeDate) or (OvertimeDate=@OldOvertimeDate and StartTime>@NewStartTime))" +
                            " )";

                        SqlParameter[] _sp2 = {
                        new SqlParameter("@OvertimeUID",SqlDbType.Int),
                        new SqlParameter("@OvertimeID",SqlDbType.Int),
                        new SqlParameter("@StartDate",SqlDbType.VarChar,50),
                        new SqlParameter("@EndDate",SqlDbType.VarChar,50),
                        new SqlParameter("@OldOvertimeDate",SqlDbType.VarChar,50),
                        new SqlParameter("@NewStartTime",SqlDbType.VarChar,50),
                         };

                        _sp2[0].Value = OvertimeUID;
                        _sp2[1].Value = OvertimeID;
                        _sp2[2].Value = StartDate;
                        _sp2[3].Value = EndDate;
                        _sp2[4].Value = OldOvertimeDate;
                        _sp2[5].Value = NewStartTime;

                        if (MsSQLDbHelper.ExecuteSql(_sql2, _sp2) > 0)
                            Num = Num + 1;

                    }


                    //#if(NewVal<OldVal)
                    //>NTime的数据 OldTotal-(OldVal-NewVal)
                    if (NewOvertimeHour < OldOvertimeHour)
                    {
                        string _sql2 = " update " + TableName + " set" +
                            " OvertimeTotal = OvertimeTotal - (" + OldOvertimeHour + "-" + NewOvertimeHour + ")," +
                            " AlreadyDaiXiu = " + AlreadyDaiXiuOvertime + "," +   //原来没有更新AlreadyDaiXiu这条值 20210609新增 
                            " ExceptDaiXiu = (OvertimeTotal - (" + OldOvertimeHour + "-" + NewOvertimeHour + ")) - " + AlreadyDaiXiuOvertime +  //" ExceptDaiXiu = ExceptDaiXiu - (" + OldOvertimeHour + "-" + NewOvertimeHour + ")" +
                            " where Invalid=0 and Del=0 " +
                            " and OvertimeID in (select OvertimeID from " + TableName + " where Invalid=0 and Del=0 " +
                            " and OvertimeUID=@OvertimeUID and OvertimeID <> @OvertimeID" +
                            " and OvertimeDate between @StartDate and @EndDate " +
                            " and ((OvertimeDate>@OldOvertimeDate) or (OvertimeDate=@OldOvertimeDate and StartTime>@NewStartTime))" +
                            " )";

                        SqlParameter[] _sp2 = {
                        new SqlParameter("@OvertimeUID",SqlDbType.Int),
                        new SqlParameter("@OvertimeID",SqlDbType.Int),
                        new SqlParameter("@StartDate",SqlDbType.VarChar,50),
                        new SqlParameter("@EndDate",SqlDbType.VarChar,50),
                        new SqlParameter("@OldOvertimeDate",SqlDbType.VarChar,50),
                        new SqlParameter("@NewStartTime",SqlDbType.VarChar,50),
                         };

                        _sp2[0].Value = OvertimeUID;
                        _sp2[1].Value = OvertimeID;
                        _sp2[2].Value = StartDate;
                        _sp2[3].Value = EndDate;
                        _sp2[4].Value = OldOvertimeDate;
                        _sp2[5].Value = NewStartTime;

                        if (MsSQLDbHelper.ExecuteSql(_sql2, _sp2) > 0)
                            Num = Num + 1;

                    }
                }

                //#1.3 否则如果新时间小于旧时间
                else if (NewStartTime.toDateTime("HH:mm") < OldStartTime.toDateTime("HH:mm"))
                {
                    //if(NewVal==OldVal)
                    //NTime~OTime 之间的数据+NewVal
                    //>ODate的数据 不用变

                    //if(NewVal>OldVal)
                    //NTime~OTime 之间的数据+NewVal
                    //>OTime的数据 OldTotal+(NewVal-OldVal)

                    //if(NewVal<OldVal)
                    //NTime~OTime 之间的数据+NewVal
                    //>OTime的数据 OldTotal-(OldVal-NewVal)


                    //#以上3种情况相同点：NTime~OTime 之间的数据+NewVal
                    string _sql = " update " + TableName + " set" +
                        " OvertimeTotal = OvertimeTotal + " + NewOvertimeHour + "," +
                        " AlreadyDaiXiu = " + AlreadyDaiXiuOvertime + "," +   //原来没有更新AlreadyDaiXiu这条值 20210609新增 
                        " ExceptDaiXiu = (OvertimeTotal + " + NewOvertimeHour + ") - " + AlreadyDaiXiuOvertime +  //" ExceptDaiXiu = ExceptDaiXiu + " + NewOvertimeHour + "" +
                        " where Invalid=0 and Del=0 " +
                        " and OvertimeID in (select OvertimeID from " + TableName + " where Invalid=0 and Del=0 " +
                        " and OvertimeUID = @OvertimeUID and OvertimeID <> @OvertimeID" + // 
                        " and OvertimeDate = @OldOvertimeDate " +  //(只改变当天内且时间在新旧范围之间的，大于当天的保持不变)
                        " and StartTime between @NewStartTime and @OldStartTime" +
                        " )";

                    SqlParameter[] _sp = {
                        new SqlParameter("@OvertimeUID",SqlDbType.Int),
                        new SqlParameter("@OvertimeID",SqlDbType.Int),
                        new SqlParameter("@OldOvertimeDate",SqlDbType.VarChar,50),
                        new SqlParameter("@NewStartTime",SqlDbType.VarChar,50),
                        new SqlParameter("@OldStartTime",SqlDbType.VarChar,50),
                         };

                    _sp[0].Value = OvertimeUID;
                    _sp[1].Value = OvertimeID;
                    _sp[2].Value = OldOvertimeDate;
                    _sp[3].Value = NewStartTime;
                    _sp[4].Value = OldStartTime;

                    if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
                        Num = Num + 1;


                    //#if(NewVal>OldVal)
                    //>OTime的数据 OldTotal+(NewVal-OldVal)
                    if (NewOvertimeHour > OldOvertimeHour)
                    {
                        string _sql2 = " update " + TableName + " set" +
                            " OvertimeTotal = OvertimeTotal + (" + NewOvertimeHour + "-" + OldOvertimeHour + ")," +
                            " AlreadyDaiXiu = " + AlreadyDaiXiuOvertime + "," +   //原来没有更新AlreadyDaiXiu这条值 20210609新增 
                            " ExceptDaiXiu = (OvertimeTotal + (" + NewOvertimeHour + "-" + OldOvertimeHour + ")) - " + AlreadyDaiXiuOvertime +   //" ExceptDaiXiu = ExceptDaiXiu + (" + NewOvertimeHour + "-" + OldOvertimeHour + ")" +
                            " where Invalid=0 and Del=0 " +
                            " and OvertimeID in (select OvertimeID from " + TableName + " where Invalid=0 and Del=0 " +
                            " and OvertimeUID=@OvertimeUID and OvertimeID <> @OvertimeID" +
                            " and OvertimeDate between @StartDate and @EndDate " +
                            " and ((OvertimeDate>@OldOvertimeDate) or (OvertimeDate=@OldOvertimeDate and StartTime>@OldStartTime))" +
                            " )";

                        SqlParameter[] _sp2 = {
                        new SqlParameter("@OvertimeUID",SqlDbType.Int),
                        new SqlParameter("@OvertimeID",SqlDbType.Int),
                        new SqlParameter("@StartDate",SqlDbType.VarChar,50),
                        new SqlParameter("@EndDate",SqlDbType.VarChar,50),
                        new SqlParameter("@OldOvertimeDate",SqlDbType.VarChar,50),
                        new SqlParameter("@OldStartTime",SqlDbType.VarChar,50),
                         };

                        _sp2[0].Value = OvertimeUID;
                        _sp2[1].Value = OvertimeID;
                        _sp2[2].Value = StartDate;
                        _sp2[3].Value = EndDate;
                        _sp2[4].Value = OldOvertimeDate;
                        _sp2[5].Value = OldStartTime;

                        if (MsSQLDbHelper.ExecuteSql(_sql2, _sp2) > 0)
                            Num = Num + 1;

                    }

                    //#if(NewVal<OldVal)
                    //>OTime的数据 OldTotal-(OldVal-NewVal)
                    if (NewOvertimeHour < OldOvertimeHour)
                    {
                        string _sql2 = " update " + TableName + " set" +
                            " OvertimeTotal = OvertimeTotal - (" + OldOvertimeHour + "-" + NewOvertimeHour + ")," +
                            " AlreadyDaiXiu = " + AlreadyDaiXiuOvertime + "," +   //原来没有更新AlreadyDaiXiu这条值 20210609新增 
                            " ExceptDaiXiu = (OvertimeTotal - (" + OldOvertimeHour + "-" + NewOvertimeHour + ")) - " + AlreadyDaiXiuOvertime +  //" ExceptDaiXiu = ExceptDaiXiu - (" + OldOvertimeHour + "-" + NewOvertimeHour + ")" +
                            " where Invalid=0 and Del=0 " +
                            " and OvertimeID in (select OvertimeID from " + TableName + " where Invalid=0 and Del=0 " +
                            " and OvertimeUID=@OvertimeUID and OvertimeID <> @OvertimeID" +
                            " and OvertimeDate between @StartDate and @EndDate " +
                            " and ((OvertimeDate>@OldOvertimeDate) or (OvertimeDate=@OldOvertimeDate and StartTime>@OldStartTime))" +
                            " )";

                        SqlParameter[] _sp2 = {
                        new SqlParameter("@OvertimeUID",SqlDbType.Int),
                        new SqlParameter("@OvertimeID",SqlDbType.Int),
                        new SqlParameter("@StartDate",SqlDbType.VarChar,50),
                        new SqlParameter("@EndDate",SqlDbType.VarChar,50),
                        new SqlParameter("@OldOvertimeDate",SqlDbType.VarChar,50),
                        new SqlParameter("@OldStartTime",SqlDbType.VarChar,50),
                         };

                        _sp2[0].Value = OvertimeUID;
                        _sp2[1].Value = OvertimeID;
                        _sp2[2].Value = StartDate;
                        _sp2[3].Value = EndDate;
                        _sp2[4].Value = OldOvertimeDate;
                        _sp2[5].Value = OldStartTime;

                        if (MsSQLDbHelper.ExecuteSql(_sql2, _sp2) > 0)
                            Num = Num + 1;

                    }

                }

            }

            //#2. 如果新日期>旧日期
            else if (NewOvertimeDate.toDateTime() > OldOvertimeDate.toDateTime())
            {

                //if(NewVal==OldVal)
                //NDate~ODate 之间的数据-OldVal
                //>NDate的数据 不用变

                //if(NewVal>OldVal)
                //NDate~ODate 之间的数据-OldVal
                //>NDate的数据 OldTotal+(NewVal-OldVal)

                //if(NewVal<OldVal)
                //NDate~ODate 之间的数据-OldVal
                //>NDate的数据 OldTotal-(OldVal-NewVal)


                //#以上3种情况相同点：NDate~ODate 之间的数据-OldVal
                string _sql = " update " + TableName + " set" +
                    " OvertimeTotal = OvertimeTotal - " + OldOvertimeHour + "," +
                    " AlreadyDaiXiu = " + AlreadyDaiXiuOvertime + "," +   //原来没有更新AlreadyDaiXiu这条值 20210609新增 
                    " ExceptDaiXiu = OvertimeTotal - " + OldOvertimeHour + " - " + AlreadyDaiXiuOvertime +  //" ExceptDaiXiu = ExceptDaiXiu - " + OldOvertimeHour + "" +
                    " where Invalid=0 and Del=0 " +
                    " and OvertimeID in (select OvertimeID from " + TableName + " where Invalid=0 and Del=0 " +
                    " and OvertimeUID = @OvertimeUID and OvertimeID <> @OvertimeID" +
                    " and OvertimeDate between @OldOvertimeDate and @NewOvertimeDate " +
                    " and ((OvertimeDate>@OldOvertimeDate) or (OvertimeDate=@OldOvertimeDate and StartTime>@OldStartTime))" +
                    " and ((OvertimeDate<@NewOvertimeDate) or (OvertimeDate=@NewOvertimeDate and StartTime<@NewStartTime))" +
                    " )";

                SqlParameter[] _sp = {
                        new SqlParameter("@OvertimeUID",SqlDbType.Int),
                        new SqlParameter("@OvertimeID",SqlDbType.Int),
                        new SqlParameter("@NewOvertimeDate",SqlDbType.VarChar,50),
                        new SqlParameter("@OldOvertimeDate",SqlDbType.VarChar,50),
                        new SqlParameter("@NewStartTime",SqlDbType.VarChar,50),
                        new SqlParameter("@OldStartTime",SqlDbType.VarChar,50),

                         };

                _sp[0].Value = OvertimeUID;
                _sp[1].Value = OvertimeID;
                _sp[2].Value = NewOvertimeDate;
                _sp[3].Value = OldOvertimeDate;
                _sp[4].Value = NewStartTime;
                _sp[5].Value = OldStartTime;

                if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
                    Num = Num + 1;


                //#if(NewVal>OldVal)
                //>NDate的数据 OldTotal+(NewVal-OldVal)
                if (NewOvertimeHour > OldOvertimeHour)
                {
                    string _sql2 = " update " + TableName + " set" +
                        " OvertimeTotal = OvertimeTotal + (" + NewOvertimeHour + "-" + OldOvertimeHour + ")," +
                        " AlreadyDaiXiu = " + AlreadyDaiXiuOvertime + "," +   //原来没有更新AlreadyDaiXiu这条值 20210609新增 
                        " ExceptDaiXiu = (OvertimeTotal + (" + NewOvertimeHour + "-" + OldOvertimeHour + ")) - " + AlreadyDaiXiuOvertime +  //" ExceptDaiXiu = ExceptDaiXiu + (" + NewOvertimeHour + "-" + OldOvertimeHour + ")" +
                        " where Invalid=0 and Del=0 " +
                        " and OvertimeID in (select OvertimeID from " + TableName + " where Invalid=0 and Del=0 " +
                        " and OvertimeUID=@OvertimeUID and OvertimeID <> @OvertimeID" +
                        " and OvertimeDate between @StartDate and @EndDate " +
                        " and ((OvertimeDate>@NewOvertimeDate) or (OvertimeDate=@NewOvertimeDate and StartTime>@NewStartTime))" +
                        " )";

                    SqlParameter[] _sp2 = {
                        new SqlParameter("@OvertimeUID",SqlDbType.Int),
                        new SqlParameter("@OvertimeID",SqlDbType.Int),
                        new SqlParameter("@StartDate",SqlDbType.VarChar,50),
                        new SqlParameter("@EndDate",SqlDbType.VarChar,50),
                        new SqlParameter("@NewOvertimeDate",SqlDbType.VarChar,50),
                        new SqlParameter("@NewStartTime",SqlDbType.VarChar,50),
                         };

                    _sp2[0].Value = OvertimeUID;
                    _sp2[1].Value = OvertimeID;
                    _sp2[2].Value = StartDate;
                    _sp2[3].Value = EndDate;
                    _sp2[4].Value = NewOvertimeDate;
                    _sp2[5].Value = NewStartTime;

                    if (MsSQLDbHelper.ExecuteSql(_sql2, _sp2) > 0)
                        Num = Num + 1;

                }

                //#if(NewVal<OldVal)
                //>NDate的数据 OldTotal-(OldVal-NewVal)
                if (NewOvertimeHour < OldOvertimeHour)
                {
                    string _sql2 = " update " + TableName + " set" +
                        " OvertimeTotal = OvertimeTotal - (" + OldOvertimeHour + "-" + NewOvertimeHour + ")," +
                        " AlreadyDaiXiu = " + AlreadyDaiXiuOvertime + "," +   //原来没有更新AlreadyDaiXiu这条值 20210609新增 
                        " ExceptDaiXiu = (OvertimeTotal - (" + OldOvertimeHour + "-" + NewOvertimeHour + ")) - " + AlreadyDaiXiuOvertime +  //" ExceptDaiXiu = ExceptDaiXiu - (" + OldOvertimeHour + "-" + NewOvertimeHour + ")" +
                        " where Invalid=0 and Del=0 " +
                        " and OvertimeID in (select OvertimeID from " + TableName + " where Invalid=0 and Del=0 " +
                        " and OvertimeUID=@OvertimeUID and OvertimeID <> @OvertimeID" +
                        " and OvertimeDate between @StartDate and @EndDate " +
                        " and ((OvertimeDate>@NewOvertimeDate) or (OvertimeDate=@NewOvertimeDate and StartTime>@NewStartTime))" +
                        " )";

                    SqlParameter[] _sp2 = {
                        new SqlParameter("@OvertimeUID",SqlDbType.Int),
                        new SqlParameter("@OvertimeID",SqlDbType.Int),
                        new SqlParameter("@StartDate",SqlDbType.VarChar,50),
                        new SqlParameter("@EndDate",SqlDbType.VarChar,50),
                        new SqlParameter("@NewOvertimeDate",SqlDbType.VarChar,50),
                        new SqlParameter("@NewStartTime",SqlDbType.VarChar,50),
                         };

                    _sp2[0].Value = OvertimeUID;
                    _sp2[1].Value = OvertimeID;
                    _sp2[2].Value = StartDate;
                    _sp2[3].Value = EndDate;
                    _sp2[4].Value = NewOvertimeDate;
                    _sp2[5].Value = NewStartTime;

                    if (MsSQLDbHelper.ExecuteSql(_sql2, _sp2) > 0)
                        Num = Num + 1;

                }



            }

            //#3. 如果新日期<旧日期
            else if (NewOvertimeDate.toDateTime() < OldOvertimeDate.toDateTime())
            {
                //if(NewVal==OldVal)
                //NDate~ODate 之间的数据+NewVal
                //>ODate的数据 不用变

                //if(NewVal>OldVal)
                //NDate~ODate 之间的数据+NewVal
                //>ODate的数据 OldTotal+(NewVal-OldVal)

                //if(NewVal<OldVal)
                //NDate~ODate 之间的数据+NewVal
                //>ODate的数据 OldTotal-(OldVal-NewVal)


                //#以上3种情况相同点：NDate~ODate 之间的数据+NewVal
                string _sql = " update " + TableName + " set" +
                    " OvertimeTotal = OvertimeTotal + " + NewOvertimeHour + "," +
                    " AlreadyDaiXiu = " + AlreadyDaiXiuOvertime + "," +   //原来没有更新AlreadyDaiXiu这条值 20210609新增 
                    " ExceptDaiXiu = OvertimeTotal + " + NewOvertimeHour + " - " + AlreadyDaiXiuOvertime +  //" ExceptDaiXiu = ExceptDaiXiu + " + NewOvertimeHour + "" +
                    " where Invalid=0 and Del=0 " +
                    " and OvertimeID in (select OvertimeID from " + TableName + " where Invalid=0 and Del=0 " +
                    " and OvertimeUID = @OvertimeUID and OvertimeID <> @OvertimeID" +
                    " and OvertimeDate between @NewOvertimeDate and @OldOvertimeDate " +
                    " and ((OvertimeDate<@OldOvertimeDate) or (OvertimeDate=@OldOvertimeDate and StartTime<@OldStartTime))" +
                    " and ((OvertimeDate>@NewOvertimeDate) or (OvertimeDate=@NewOvertimeDate and StartTime>@NewStartTime))" +
                    " )";

                SqlParameter[] _sp = {
                        new SqlParameter("@OvertimeUID",SqlDbType.Int),
                        new SqlParameter("@OvertimeID",SqlDbType.Int),
                        new SqlParameter("@NewOvertimeDate",SqlDbType.VarChar,50),
                        new SqlParameter("@OldOvertimeDate",SqlDbType.VarChar,50),
                        new SqlParameter("@NewStartTime",SqlDbType.VarChar,50),
                        new SqlParameter("@OldStartTime",SqlDbType.VarChar,50),

                         };

                _sp[0].Value = OvertimeUID;
                _sp[1].Value = OvertimeID;
                _sp[2].Value = NewOvertimeDate;
                _sp[3].Value = OldOvertimeDate;
                _sp[4].Value = NewStartTime;
                _sp[5].Value = OldStartTime;

                if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
                    Num = Num + 1;


                //#if(NewVal>OldVal)
                //>ODate的数据 OldTotal+(NewVal-OldVal)
                if (NewOvertimeHour > OldOvertimeHour)
                {
                    string _sql2 = " update " + TableName + " set" +
                        " OvertimeTotal = OvertimeTotal + (" + NewOvertimeHour + "-" + OldOvertimeHour + ")," +
                        " AlreadyDaiXiu = " + AlreadyDaiXiuOvertime + "," +   //原来没有更新AlreadyDaiXiu这条值 20210609新增 
                        " ExceptDaiXiu = (OvertimeTotal + (" + NewOvertimeHour + "-" + OldOvertimeHour + ")) - " + AlreadyDaiXiuOvertime +  //" ExceptDaiXiu = ExceptDaiXiu + (" + NewOvertimeHour + "-" + OldOvertimeHour + ")" +
                        " where Invalid=0 and Del=0 " +
                        " and OvertimeID in (select OvertimeID from " + TableName + " where Invalid=0 and Del=0 " +
                        " and OvertimeUID=@OvertimeUID and OvertimeID <> @OvertimeID" +
                        " and OvertimeDate between @StartDate and @EndDate " +
                        " and ((OvertimeDate>@OldOvertimeDate) or (OvertimeDate=@OldOvertimeDate and StartTime>@OldStartTime))" +
                        " )";

                    SqlParameter[] _sp2 = {
                        new SqlParameter("@OvertimeUID",SqlDbType.Int),
                        new SqlParameter("@OvertimeID",SqlDbType.Int),
                        new SqlParameter("@StartDate",SqlDbType.VarChar,50),
                        new SqlParameter("@EndDate",SqlDbType.VarChar,50),
                        new SqlParameter("@OldOvertimeDate",SqlDbType.VarChar,50),
                        new SqlParameter("@OldStartTime",SqlDbType.VarChar,50),
                         };

                    _sp2[0].Value = OvertimeUID;
                    _sp2[1].Value = OvertimeID;
                    _sp2[2].Value = StartDate;
                    _sp2[3].Value = EndDate;
                    _sp2[4].Value = OldOvertimeDate;
                    _sp2[5].Value = OldStartTime;

                    if (MsSQLDbHelper.ExecuteSql(_sql2, _sp2) > 0)
                        Num = Num + 1;

                }

                //#if(NewVal<OldVal)
                //>ODate的数据 OldTotal-(OldVal-NewVal)
                if (NewOvertimeHour < OldOvertimeHour)
                {
                    string _sql2 = " update " + TableName + " set" +
                        " OvertimeTotal = OvertimeTotal - (" + OldOvertimeHour + "-" + NewOvertimeHour + ")," +
                        " AlreadyDaiXiu = " + AlreadyDaiXiuOvertime + "," +   //原来没有更新AlreadyDaiXiu这条值 20210609新增 
                        " ExceptDaiXiu = (OvertimeTotal - (" + OldOvertimeHour + "-" + NewOvertimeHour + ")) - " + AlreadyDaiXiuOvertime + //" ExceptDaiXiu = ExceptDaiXiu - (" + OldOvertimeHour + "-" + NewOvertimeHour + ")" +
                        " where Invalid=0 and Del=0 " +
                        " and OvertimeID in (select OvertimeID from " + TableName + " where Invalid=0 and Del=0 " +
                        " and OvertimeUID=@OvertimeUID and OvertimeID <> @OvertimeID" +
                        " and OvertimeDate between @StartDate and @EndDate " +
                        " and ((OvertimeDate>@OldOvertimeDate) or (OvertimeDate=@OldOvertimeDate and StartTime>@OldStartTime))" +
                        " )";

                    SqlParameter[] _sp2 = {
                        new SqlParameter("@OvertimeUID",SqlDbType.Int),
                        new SqlParameter("@OvertimeID",SqlDbType.Int),
                        new SqlParameter("@StartDate",SqlDbType.VarChar,50),
                        new SqlParameter("@EndDate",SqlDbType.VarChar,50),
                        new SqlParameter("@OldOvertimeDate",SqlDbType.VarChar,50),
                        new SqlParameter("@OldStartTime",SqlDbType.VarChar,50),
                         };

                    _sp2[0].Value = OvertimeUID;
                    _sp2[1].Value = OvertimeID;
                    _sp2[2].Value = StartDate;
                    _sp2[3].Value = EndDate;
                    _sp2[4].Value = OldOvertimeDate;
                    _sp2[5].Value = OldStartTime;

                    if (MsSQLDbHelper.ExecuteSql(_sql2, _sp2) > 0)
                        Num = Num + 1;

                }

            }

            if (Num > 0)
                flag = true;

            return flag;

        }


        /// <summary>
        /// 删除加班申请时，更新OvertimeTotal的值（该值在删除动作或修改动作时才更新，而撤回时不需要，有可能是撤回后不做任何操作又重新提交了）
        /// </summary>
        /// <param name="Action"></param>
        /// <param name="ShortTableName"></param>
        /// <param name="ModuleID"></param>
        /// <param name="FormID"></param>
        /// <param name="FormsID"></param>
        /// <param name="DelType">删除类型：父记录或子记录 父记录=Parent：字段ParentID=@FormsID，子记录=Sub：字段OvertimeID=FormsID </param>
        /// <returns></returns>
        public static Boolean DelUpdateOvertimeTotal(string Action, string ShortTableName, string ModuleID, string FormID, string FormsID, string DelType = "Sub")
        {
            Boolean flag = false;
            int Num = 0;

            string TableName = MicroPublic.GetTableName(ShortTableName),
                Field = "OvertimeID";

            if (DelType == "Parent")
                Field = "ParentID";

            string _sql = "select OvertimeID,OvertimeUID,OvertimeHour,OvertimeDate,StartTime from " + TableName + " where ParentID<>0 and " + Field + " in (" + FormsID + ") order by OvertimeDate,StartTime";  //确保日期由最小的开始
            DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

            if (_dt != null && _dt.Rows.Count > 0)
            {
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    int OvertimeID = _dt.Rows[i]["OvertimeID"].toInt();
                    int OvertimeUID = _dt.Rows[i]["OvertimeUID"].toInt();
                    decimal OldOvertimeHour = _dt.Rows[i]["OvertimeHour"].toDecimal();
                    string OvertimeDate = _dt.Rows[i]["OvertimeDate"].toStringTrim();
                    string StartTime = _dt.Rows[i]["StartTime"].toStringTrim();

                    string StartDateTime = OvertimeDate.toDateMFirstDay() + " 00:00:00.000",
                        EndDateTime = OvertimeDate.toDateMLastDay() + " 23:59:59.998";

                    var getUserWorkHourSystem = MicroHR.GetUserWorkHourSystem(OvertimeUID, OvertimeDate);
                    string WorkHourSysID = getUserWorkHourSystem.WorkHourSysID;

                    if (WorkHourSysID == "2")
                    {
                        StartDateTime = OvertimeDate.toDateQFirstDay() + " 00:00:00.000";
                        EndDateTime = OvertimeDate.toDateQLastDay() + " 23:59:59.998";
                    }

                    //更新加班日期或时间比被删除这条记录的日期或时间大的记录
                    string _sql2 = " update " + TableName + " set OvertimeTotal = OvertimeTotal - " + OldOvertimeHour + ", ExceptDaiXiu = ExceptDaiXiu - " + OldOvertimeHour + " where ParentID<>0 " +
                           " and OvertimeID in (select OvertimeID from " + TableName + " where ParentID<>0 and OvertimeDate between @StartDateTime and @EndDateTime and OvertimeUID=@OvertimeUID and OvertimeID <> @OvertimeID" +
                           " and ((OvertimeDate>@OvertimeDate) or (OvertimeDate=@OvertimeDate and StartTime>@StartTime)) )";

                    SqlParameter[] _sp2 = {
                        new SqlParameter("@OvertimeDate",SqlDbType.VarChar,50),
                        new SqlParameter("@StartDateTime",SqlDbType.VarChar,50),
                        new SqlParameter("@EndDateTime",SqlDbType.VarChar,50),
                        new SqlParameter("@OvertimeUID",SqlDbType.Int),
                        new SqlParameter("@OvertimeID",SqlDbType.Int),
                        new SqlParameter("@StartTime",SqlDbType.VarChar,50),
                         };

                    _sp2[0].Value = OvertimeDate;
                    _sp2[1].Value = StartDateTime;
                    _sp2[2].Value = EndDateTime;
                    _sp2[3].Value = OvertimeUID;
                    _sp2[4].Value = OvertimeID;
                    _sp2[5].Value = StartTime;

                    if (MsSQLDbHelper.ExecuteSql(_sql2, _sp2) > 0)
                        Num = Num + 1;
                }

                if (Num > 0)
                    flag = true;
            }

            return flag;

        }


        /// <summary>
        /// 在提交表单前检查休假申请时间段是否存在重复申请
        /// </summary>
        /// <param name="Action"></param>
        /// <param name="ShortTableName"></param>
        /// <param name="ModuleID"></param>
        /// <param name="FormID"></param>
        /// <param name="IsApprovalForm"></param>
        /// <param name="FormFields"></param>
        /// <param name="FormsID"></param>
        /// <returns></returns>
        private static string CheckRepeatLeave(string Action, string ShortTableName, string ModuleID, string FormID, Boolean IsApprovalForm, string FormFields, string FormsID = "")
        {
            string flag = MicroPublic.GetMsg("Submit"),
                LeaveUID = string.Empty,
                StartDateTime = string.Empty,
                EndDateTime = string.Empty;

            dynamic json = JToken.Parse(FormFields) as dynamic;

            LeaveUID = json["selLeaveUID"];
            StartDateTime = json["hidStartDateTime"];
            EndDateTime = json["hidEndDateTime"];

            if (!string.IsNullOrEmpty(LeaveUID) && !string.IsNullOrEmpty(StartDateTime) && !string.IsNullOrEmpty(EndDateTime))
            {
                StartDateTime = StartDateTime.toDateTime("yyyy-MM-dd HH:mm:ss").AddSeconds(1).toDateFormat("yyyy-MM-dd HH:mm:ss"); //开始时间+1秒
                EndDateTime = EndDateTime.toDateTime("yyyy-MM-dd HH:mm:ss").AddSeconds(-1).toDateFormat("yyyy-MM-dd HH:mm:ss"); //结束时间-1秒

                //注意这里的比较时间是写反的(开始对结束，结束对开始)StartDateTime<=@EndDateTime and EndDateTime>=@StartDateTime
                string _sql = "select * from HRLeave where Invalid=0 and Del=0 and LeaveUID=@LeaveUID and StartDateTime<=@EndDateTime and EndDateTime>=@StartDateTime and LeaveID<>@FormsID";
                SqlParameter[] _sp = { new SqlParameter("@LeaveUID",SqlDbType.Int),
                    new SqlParameter("@StartDateTime",SqlDbType.DateTime),
                    new SqlParameter("@EndDateTime",SqlDbType.DateTime),
                    new SqlParameter("@FormsID",SqlDbType.Int),
                };

                _sp[0].Value = LeaveUID.toInt();
                _sp[1].Value = StartDateTime.toDateTime("yyyy-MM-dd HH:mm:ss");
                _sp[2].Value = EndDateTime.toDateTime("yyyy-MM-dd HH:mm:ss");
                _sp[3].Value = FormsID.toInt();

                DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

                //_dt.Rows.Count>0说明有记录
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    string LeaveDisplayName = _dt.Rows[0]["LeaveDisplayName"].toStringTrim(),
                        FormNumber = _dt.Rows[0]["FormNumber"].toStringTrim();

                    flag = "提交失败，" + LeaveDisplayName + " 在该时间段内已经提交过休假申请，<br/>相关表单编号：" + FormNumber + "，请确认。";
                }

            }

            return flag;
        }


        /// <summary>
        /// 删除代休假事前检查（删除记录前先判断记录删除后的加班总时间是否会超出最大允许时间，如果超出则返回提示，否则返回True+提示，默认允许删除）
        /// </summary>
        /// <param name="Action"></param>
        /// <param name="ShortTableName"></param>
        /// <param name="ModuleID"></param>
        /// <param name="FormID"></param>
        /// <param name="IsApprovalForm"></param>
        /// <param name="FormFields"></param>
        /// <param name="FormsID"></param>
        /// <returns></returns>
        public static string CheckLeaveDel(string Action, string ShortTableName, string ModuleID, string FormID, string FormsID)
        {
            string flag = MicroPublic.GetMsg("Del");

            string _sql = "select LeaveUID, LeaveDays, OvertimeDate from HRLeave where Invalid=0 and Del=0 and HolidayTypeID=2 and LeaveID=@FormsID";
            SqlParameter[] _sp = { new SqlParameter("@FormsID", SqlDbType.Int) };
            _sp[0].Value = FormsID.toInt();

            DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

            if (_dt != null && _dt.Rows.Count > 0)
            {
                int LeaveUID = _dt.Rows[0]["LeaveUID"].toInt();
                decimal LeaveDays = _dt.Rows[0]["LeaveDays"].toDecimal();  //代休天数
                string OvertimeDate = _dt.Rows[0]["OvertimeDate"].toStringTrim();  //加班月份

                if (LeaveUID != 0 && LeaveDays != 0 && !string.IsNullOrEmpty(OvertimeDate))
                {
                    //获取用户工时制
                    var getUserWorkHourSystem = MicroHR.GetUserWorkHourSystem(LeaveUID, OvertimeDate);
                    string WorkHourSysID = getUserWorkHourSystem.WorkHourSysID,
                     MaxValue = getUserWorkHourSystem.MaxValue;  //最大允许加班值

                    //加班小时汇总（含休假时间），读取个人数据（HRPersonalRecord表）Hour记录汇总
                    //把休假的加班朋份OvertimeDate作为统计加班的日期
                    var getPersonalRecord = MicroHR.GetPersonalRecord(OvertimeDate, LeaveUID.toInt(), WorkHourSysID.toInt(), 0, "WithdrawalOrMore");  //WithdrawalOrMore、ApplyOrMore
                    string ExceptDaiXiu = getPersonalRecord.ExceptDaiXiu;  //剔除代休后的总加班时间

                    //剔除代休后的加班小时+即将要被删除的加班小时数>允许加班的最大小时数则不允许删除
                    if (MaxValue.toDecimal() != 0 && ((ExceptDaiXiu.toDecimal() + LeaveDays * 8) > MaxValue.toDecimal()))
                        flag = "删除失败，删除后加班时间将超出“" + MaxValue + "”小时，请确认。";

                }
                else
                    flag = "删除失败，检证数据时发生错误的值。";

            }

            return flag;

        }


        /// <summary>
        /// 导入排班内容时更新父行的DutyConetnt列，逻列出日期：班次：姓名1、姓名2、姓名3等等这样的格式
        /// </summary>
        /// <param name="Action"></param>
        /// <param name="ShortTableName"></param>
        /// <param name="ModuleID"></param>
        /// <param name="FormID"></param>
        /// <param name="FormsID"></param>
        /// <returns></returns>
        public static Boolean UpdatePrivateOnDuty(string Action, string ShortTableName, string ModuleID, string FormID, string FormsID)
        {
            Boolean flag = false;

            string Str = string.Empty,
                TableName = MicroPublic.GetTableName(ShortTableName);

            //根据唯一的日期查询记录
            string _sql = "select Distinct DutyDate from " + TableName + " where Invalid=0 and Del=0 and ParentID=@ParentID order by DutyDate";
            SqlParameter[] _sp = { new SqlParameter("@ParentID", SqlDbType.Int) };
            _sp[0].Value = FormsID.toInt();

            DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

            if (_dt != null && _dt.Rows.Count > 0)
            {
                //按每条明细查询
                string _sql2 = "select * from " + TableName + " where Invalid=0 and Del=0 and ParentID=@ParentID order by DutyDate";
                SqlParameter[] _sp2 = { new SqlParameter("@ParentID", SqlDbType.Int) };
                _sp2[0].Value = FormsID.toInt();

                DataTable _dt2 = MsSQLDbHelper.Query(_sql2, _sp2).Tables[0];

                //班次表
                DataTable _dt3 = MicroDataTable.GetDataTable("ShiftType");

                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    string DutyDate = _dt.Rows[i]["DutyDate"].toDateFormat("yyyy-MM-dd");
                    Str += "【" + DutyDate + "（" + MicroPublic.GetWeek(DutyDate, "WW") + "）】";

                    if (_dt3 != null && _dt3.Rows.Count > 0)
                    {
                        for (int j = 0; j < _dt3.Rows.Count; j++)
                        {
                            int ShiftTypeID = _dt3.Rows[j]["ShiftTypeID"].toInt();

                            string DutyDisplayName = string.Empty,
                                Alias = _dt3.Rows[j]["Alias"].toStringTrim();

                            DataRow[] _rows2 = _dt2.Select("DutyDate='" + DutyDate + "' and ShiftTypeID=" + ShiftTypeID + "");
                            if (_rows2.Length > 0)
                            {
                                Str += Alias;
                                Str += "（" + _rows2.Length + "人）：";

                                foreach (DataRow _dr2 in _rows2)
                                {
                                    DutyDisplayName += _dr2["DutyDisplayName"].toStringTrim() + "、";
                                }

                                if (!string.IsNullOrEmpty(DutyDisplayName))
                                    DutyDisplayName = DutyDisplayName.Substring(0, DutyDisplayName.Length - 1) + "；<br/>";

                                Str += DutyDisplayName;
                            }

                        }
                    }

                    //为特殊次时
                    DataRow[] _rows21 = _dt2.Select("DutyDate='" + DutyDate + "' and ShiftTypeID=0 and ShiftName <> '-' and ShiftName <> '休'");
                    if (_rows21.Length > 0)
                    {

                        Str += "特殊班次";
                        Str += "（" + _rows21.Length + "人）：";
                        string DutyDisplayName = string.Empty;

                        foreach (DataRow _dr21 in _rows21)
                        {
                            DutyDisplayName += _dr21["DutyDisplayName"].toStringTrim() + "、";
                        }

                        if (!string.IsNullOrEmpty(DutyDisplayName))
                            DutyDisplayName = DutyDisplayName.Substring(0, DutyDisplayName.Length - 1) + "；<br/>";

                        Str += DutyDisplayName;
                    }

                    //为休息时
                    DataRow[] _rows23 = _dt2.Select("DutyDate='" + DutyDate + "' and ShiftTypeID=0 and ShiftName = '休'");
                    if (_rows23.Length > 0)
                    {

                        Str += "休";
                        Str += "（" + _rows23.Length + "人）：";
                        string DutyDisplayName = string.Empty;

                        foreach (DataRow _dr23 in _rows23)
                        {
                            DutyDisplayName += _dr23["DutyDisplayName"].toStringTrim() + "、";
                        }

                        if (!string.IsNullOrEmpty(DutyDisplayName))
                            DutyDisplayName = DutyDisplayName.Substring(0, DutyDisplayName.Length - 1) + "；<br/>";

                        Str += DutyDisplayName;
                    }

                    //为取消班次时
                    DataRow[] _rows22 = _dt2.Select("DutyDate='" + DutyDate + "' and ShiftTypeID=0 and ShiftName = '-'");
                    if (_rows22.Length > 0)
                    {

                        Str += "取消班次";
                        Str += "（" + _rows22.Length + "人）：";
                        string DutyDisplayName = string.Empty;

                        foreach (DataRow _dr22 in _rows22)
                        {
                            DutyDisplayName += _dr22["DutyDisplayName"].toStringTrim() + "、";
                        }

                        if (!string.IsNullOrEmpty(DutyDisplayName))
                            DutyDisplayName = DutyDisplayName.Substring(0, DutyDisplayName.Length - 1) + "；<br/>";

                        Str += DutyDisplayName;
                    }


                }


                if (!string.IsNullOrEmpty(Str))
                {
                    string _sql5 = " update " + TableName + " set Location=@Location, DutyContent=@DutyContent where ParentID=0 and DutyID=@DutyID";
                    SqlParameter[] _sp5 = { new SqlParameter("@Location",SqlDbType.NVarChar,1000),
                                            new SqlParameter("@DutyContent",SqlDbType.Text),
                                            new SqlParameter("@DutyID",SqlDbType.Int),
                                    };

                    _sp5[0].Value = "-";
                    _sp5[1].Value = Str.toStringTrim();
                    _sp5[2].Value = FormsID.toInt();

                    if (MsSQLDbHelper.ExecuteSql(_sql5, _sp5) > 0)
                        flag = true;

                }
            }


            return flag;

        }



        public class OnDutyFormatAttr
        {
            /// <summary>
            /// 用于判断特殊班次格式是否正确，（特：13:00~17:00）
            /// </summary>
            public Boolean IsCorrect { set; get; }


            /// <summary>
            /// 开始日期时间
            /// </summary>
            public string StartDateTime { set; get; }


            /// <summary>
            /// 结束日期格式
            /// </summary>
            public string EndDateTime { set; get; }
        }


        /// <summary>
        /// 当班次为特殊班次时检查填入的特殊班次的格式是否正确
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static OnDutyFormatAttr GetOnDutyFormat(string DutyDate, string Value)
        {
            Boolean isCorrect = false;

            string startDateTime = string.Empty,
                endDateTime = string.Empty;

            if (Value.Contains("特"))
            {
                string[] ValueArr = Value.Split('特');

                if (ValueArr[1].Split('-').Length > 1)
                {
                    if (ValueArr[1].Split('-')[0].toStringTrim().toDateTimeBoolean() && ValueArr[1].Split('-')[1].toStringTrim().toDateTimeBoolean())
                    {
                        isCorrect = true;
                        startDateTime = ValueArr[1].Split('-')[0].toStringTrim();
                        endDateTime = ValueArr[1].Split('-')[1].toStringTrim();
                    }
                }
                else if (ValueArr[1].Split('~').Length > 1)
                {
                    if (ValueArr[1].Split('-')[0].toStringTrim().toDateTimeBoolean() && ValueArr[1].Split('-')[1].toStringTrim().toDateTimeBoolean())
                    {
                        isCorrect = true;
                        startDateTime = ValueArr[1].Split('~')[0].toStringTrim();
                        endDateTime = ValueArr[1].Split('~')[1].toStringTrim();
                    }
                }

            }

            if (!string.IsNullOrEmpty(startDateTime) && !string.IsNullOrEmpty(endDateTime))
            {
                startDateTime = DutyDate + " " + startDateTime;
                endDateTime = DutyDate + " " + endDateTime;

                //如果结束时间<开始时间，则结束时间天数+1
                if (endDateTime.toDateTime("HH:mm") <= startDateTime.toDateTime("HH:mm"))
                    endDateTime = endDateTime.toDateTime("yyyy-MM-dd HH:mm").AddDays(1).toDateFormat("yyyy-MM-dd HH:mm");

            }


            var OnDutyFormatAttr = new OnDutyFormatAttr
            {
                IsCorrect = isCorrect,
                StartDateTime = startDateTime,
                EndDateTime = endDateTime

            };

            return OnDutyFormatAttr;
        }


    }

}

