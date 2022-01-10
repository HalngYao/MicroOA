using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroDTHelper;
using MicroUserHelper;
using MicroPublicHelper;
using Newtonsoft.Json.Linq;

namespace MicroHRHelper
{
    public class MicroHR
    {

        public class PersonalRecordAttr
        {

            public string MonthStartDateTime { set; get; }

            public string MonthEndDateTime { set; get; }

            public string QuarterStartDateTime { set; get; }

            public string QuarterEndDateTime { set; get; }

            /// <summary>
            /// 加班总小时，暂时用于提交加班申请时（根据工时制自动统计，标准工时制按月统计，综合工时制按季度统计，没有设置工时制时默认以标准）
            /// </summary>
            public string OvertimeTotal { set; get; }

            /// <summary>
            /// 按日期的向上统计（即比指定日期小的），标准返回Month，综合返回Quarter
            /// </summary>
            public string OvertimeTotalByUp { set; get; }

            /// <summary>
            /// 按日期的向下统计 （即比指定日期大的），标准返回Month，综合返回Quarter
            /// </summary>
            public string OvertimeTotalByDown { set; get; }

            /// <summary>
            /// 按日期的向上统计（即比指定日期小的），月度内
            /// </summary>
            public string OvertimeTotalByUpMonth { set; get; }

            /// <summary>
            /// 按日期的向下统计（即比指定日期大的），月度内
            /// </summary>
            public string OvertimeTotalByDownMonth { set; get; }


            /// <summary>
            /// 按日期的向上统计（即比指定日期小的），季度内
            /// </summary>
            public string OvertimeTotalByUpQuarter { set; get; }

            /// <summary>
            /// 按日期的向下统计（即比指定日期大的），季度内
            /// </summary>
            public string OvertimeTotalByDownQuarter { set; get; }

            /// <summary>
            /// 当日加班小时数（主要用于标准工时制平日和休日加班不能大于3或11小时如果大于则需要填写特殊加班单）
            /// </summary>
            public string OvertimeTotalByDay { set; get; }

            /// <summary>
            /// 月度加班总小时，暂时用于首页个人勤怠数据展示
            /// </summary>
            public string OvertimeTotalByMonth { set; get; }

            /// <summary>
            /// 季度加班总小时，暂时用于首页个人勤怠数据展示
            /// </summary>
            public string OvertimeTotalByQuarter { set; get; }

            /// <summary>
            /// 延时加班
            /// </summary>
            public string WorkOvertime { set; get; }

            /// <summary>
            /// 季度延时加班
            /// </summary>
            public string WorkOvertimeByQuarter { set; get; }

            /// <summary>
            /// 休息日加班
            /// </summary>
            public string OffDayOvertime { set; get; }

            /// <summary>
            /// 季度休息日加班
            /// </summary>
            public string OffDayOvertimeByQuarter { set; get; }

            /// <summary>
            /// 法定日加班
            /// </summary>
            public string Statutory { set; get; }

            /// <summary>
            /// 季度法定日加班
            /// </summary>
            public string StatutoryByQuarter { set; get; }

            /// <summary>
            /// 需代休
            /// </summary>
            public string NeedDaiXiu { set; get; }

            /// <summary>
            /// 已代休（以休假表StartDateTime开始日期和结束日期EndDateTime作为统计），标准 AlreadyDaiXiu=AlreadyDaiXiuByMonth，综合 AlreadyDaiXiu=AlreadyDaiXiuByQuarter
            /// </summary>
            public string AlreadyDaiXiu { set; get; }

            /// <summary>
            /// 本月已代休（以休假表StartDateTime开始日期和结束日期EndDateTime作为统计）
            /// </summary>
            public string AlreadyDaiXiuByMonth { set; get; }

            /// <summary>
            /// 季度已代休（以休假表StartDateTime开始日期和结束日期EndDateTime作为统计）
            /// </summary>
            public string AlreadyDaiXiuByQuarter { set; get; }

            /// <summary>
            /// 已代休（以休假表的加班月份OvertimeDate作为为统计），标准 AlreadyDaiXiuOvertime=AlreadyDaiXiuOvertimeByMonth，综合 AlreadyDaiXiuOvertime=AlreadyDaiXiuOvertimeByQuarter
            /// </summary>
            public string AlreadyDaiXiuOvertime { set; get; }

            /// <summary>
            /// 本月已代休（以休假表的加班月份OvertimeDate作为为统计，即该代休是代休那个月份的加班）
            /// </summary>
            public string AlreadyDaiXiuOvertimeByMonth { set; get; }

            /// <summary>
            /// 季度已代休（以休假表的加班月份OvertimeDate作为为统计，即该代休是代休那个月份的加班）
            /// </summary>
            public string AlreadyDaiXiuOvertimeByQuarter { set; get; }

            /// <summary>
            /// 剔除代休后的加班时间，标准 ExceptDaiXiu=ExceptDaiXiuByMonth，综合 ExceptDaiXiu=ExceptDaiXiuByQuarter
            /// </summary>
            public string ExceptDaiXiu { set; get; }

            /// <summary>
            /// 剔除代休后的月度加班时间（可能和RemainingDaiXiu意思相同），月度剔除代休后的加班时间=月度总加班时间-月度以加班月份为统计条件的代休时间
            /// </summary>
            public string ExceptDaiXiuByMonth { set; get; }

            /// <summary>
            /// 剔除代休后的季度加班时间（可能和RemainingDaiXiu意思相同），季度剔除代休后的加班时间=季度总加班时间-季度以加班月份为统计条件的代休时间
            /// </summary>
            public string ExceptDaiXiuByQuarter { set; get; }

            /// <summary>
            /// 休假汇总，标准 LeaveTotal=LeaveTotalByMonth，综合 LeaveTotal=LeaveTotalByQuarter
            /// </summary>
            public string LeaveTotal { set; get; }

            /// <summary>
            /// 月度休假汇总（含所有假期类型的休假，但目前只汇总了LeaveDays，LeaveHour还没有）
            /// </summary>
            public string LeaveTotalByMonth { set; get; }

            /// <summary>
            /// 季度休假汇总（含所有假期类型的休假，但目前只汇总了LeaveDays，LeaveHour还没有）
            /// </summary>
            public string LeaveTotalByQuarter { set; get; }

            /// <summary>
            /// 剩余可代休（剩余休日加班=月度休日加班-月度已代休(以加班月份为准)）
            /// </summary>
            public string RemainingDaiXiu { set; get; }

            /// <summary>
            /// 季度剩余可代休（剩余休日加班=季度休日加班-季度已代休(以加班季度为准)）
            /// </summary>
            public string RemainingDaiXiuByQuarter { set; get; }

            /// <summary>
            /// 剩余年休假
            /// </summary>
            public string RemainingAnnualLeave { set; get; }

            /// <summary>
            /// 剩余其它假
            /// </summary>
            public string RemainingOtherLeave { set; get; }

            /// <summary>
            /// 自驾
            /// </summary>
            public string SelfDriving { set; get; }

        }


        /// <summary>
        /// 获取用户加班总时间（以加班日期为条件进行计算，标准用户返回该日期所在月份的加班时间汇总，综合用户返回该日期所在季度的加班时间汇总）
        /// </summary>
        /// <param name="AnyDate">任意日期，计算出当该日期的第一天和最后一天</param>
        /// <param name="UserID"></param>
        /// <param name="WorkHourSysID"></param>
        /// <param name="ExceptCurrOvertimeID">当前记录不等于0时，除去当前记录进行计算。通常在修改表单时用到</param>
        /// <param name="DataStateCode">统计数据的类型根据StateCode的状态：Apply填写申请或草稿（and StateCode =-3 or StateCode =-2）、ApplyOrMore填写申请、草稿或以上（and StateCode >=-3 and StateCode <>-1）、Return驳回申请（and StateCode =-1）、Approval等待审批(and StateCode=0)、ApprovalOrMore等待审批或以上(and StateCode>=0)、Finish已审批通过的( and StateCode=100)、All所有（包含临时、驳回、删除、填写申请、等待审批的 StateCode=string.Empty;）</param>
        /// <param name="StartTime">在向上或向下汇总时用到</param>
        /// <returns></returns>
        public static PersonalRecordAttr GetPersonalRecord(string AnyDate, int UserID, int WorkHourSysID, int ExceptCurrOvertimeID = 0, string DataStateCode = "Finish", string StartTime = "")
        {
            //这个方法需要每次都读取数据库记录，因为在申请时一张表中可能有多个相同加班者的记录，如只读一次在汇总时间时每次都以第一次为准得到的数据会不准确

            string monthStartDateTime = string.Empty,
                monthEndDateTime = string.Empty,
                quarterStartDateTime = string.Empty,
                quarterEndDateTime = string.Empty,
                overtimeTotal = "0",
                overtimeTotalByUp = "0",
                overtimeTotalByDown = "0",
                overtimeTotalByUpMonth = "0",
                overtimeTotalByDownMonth = "0",
                overtimeTotalByUpQuarter = "0",
                overtimeTotalByDownQuarter = "0",
                overtimeTotalByDay = "0",
                overtimeTotalByMonth = "0",
                overtimeTotalByQuarter = "0",
                workOvertime = "0",
                workOvertimeByQuarter = "0",
                offDayOvertime = "0",
                offDayOvertimeByQuarter = "0",
                statutory = "0",
                statutoryByQuarter = "0",

                leaveTotal = "0",
                leaveTotalByMonth = "0",
                leaveTotalByQuarter = "0",

                alreadyDaiXiu = "0",
                alreadyDaiXiuByMonth = "0",
                alreadyDaiXiuByQuarter = "0",

                alreadyDaiXiuOvertime = "0",
                alreadyDaiXiuOvertimeByMonth = "0",
                alreadyDaiXiuOvertimeByQuarter = "0",

                exceptDaiXiu = "0",
                exceptDaiXiuByMonth = "0",
                exceptDaiXiuByQuarter = "0",

                remainingDaiXiu = "0",
                remainingDaiXiuByQuarter = "0",
                remainingAnnualLeave = "0",
                remainingOtherLeave = "0",
                selfDriving = "0";  //还没赋值

            string Where = string.Empty, WhereStateCode = " and StateCode=100";

            if (ExceptCurrOvertimeID > 0)
                Where = " and OvertimeID <> " + ExceptCurrOvertimeID;

            //根据数据状态代码进行统计
            if (DataStateCode == "Withdrawal")
                WhereStateCode = " and (StateCode =-4)"; //撤回
            else if (DataStateCode == "WithdrawalOrMore")
                WhereStateCode = " and (StateCode >=-4)"; //撤回或以上
            else if (DataStateCode == "Draft")
                WhereStateCode = " and (StateCode =-3 or StateCode =-2)"; //填写申请或临时保存（统称草稿）
            else if (DataStateCode == "DraftOrMore")
                WhereStateCode = " and (StateCode >=-3)"; //草稿或以上
            else if (DataStateCode == "Return")
                WhereStateCode = " and (StateCode =-1)"; //驳回
            else if (DataStateCode == "ReturnOrMore")
                WhereStateCode = " and (StateCode >=-1)"; //驳回或以上
            else if (DataStateCode == "Apply")
                WhereStateCode = " and (StateCode =-3 or StateCode =-2)"; //填写申请或临时保存（统称草稿）
            else if (DataStateCode == "ApplyOrMore")
                WhereStateCode = " and StateCode >=-3 and StateCode <>-1";
            else if (DataStateCode == "Approval")
                WhereStateCode = " and StateCode=0";  //等待审批
            else if (DataStateCode == "ApprovalOrMore")
                WhereStateCode = " and StateCode>=0"; //等待审批或以上
            else if (DataStateCode == "Finish")
                WhereStateCode = " and StateCode=100";  //已审批通过
            else if (DataStateCode == "All")
                WhereStateCode = string.Empty;  //所有

            monthStartDateTime = AnyDate.toDateMFirstDay() + " 00:00:00.000";
            monthEndDateTime = AnyDate.toDateMLastDay() + " 23:59:59.998";

            quarterStartDateTime = AnyDate.toDateQFirstDay() + " 00:00:00.000";
            quarterEndDateTime = AnyDate.toDateQLastDay() + " 23:59:59.998";

            //统计时间范围sql语句中默认以季度统计，月度统计在_dt.select中追加条件
            string _sql = "select * from HROvertime where Invalid=0 and Del=0 and ParentID<>0 and OvertimeUID=@OvertimeUID and OvertimeDate between @StartDate and @EndDate " + Where + WhereStateCode;
            SqlParameter[] _sp = { new SqlParameter("@OvertimeUID",SqlDbType.Int),
                    new SqlParameter("@StartDate",SqlDbType.VarChar),   //???不用Datetime类型查询，待验证 注：20210316
                     new SqlParameter("@EndDate",SqlDbType.VarChar) };

            _sp[0].Value = UserID;
            _sp[1].Value = quarterStartDateTime;
            _sp[2].Value = quarterEndDateTime;

            DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

            if (_dt != null && _dt.Rows.Count > 0)
            {
                //汇总加班总时间

                //按日期向上汇总（即比指定日期小的），月度内
                string WhereByUpMonth ="OvertimeDate>='" + monthStartDateTime + "' and OvertimeDate<='" + monthEndDateTime + "' and OvertimeDate<'" + AnyDate.toDateFormat() + "'";
                if (!string.IsNullOrEmpty(StartTime))
                    WhereByUpMonth = "OvertimeDate>='" + monthStartDateTime + "' and OvertimeDate<='" + monthEndDateTime + "' and ((OvertimeDate<'" + AnyDate.toDateFormat() + "') or (OvertimeDate='" + AnyDate.toDateFormat() + "' and StartTime<'" + StartTime + "'))";

                overtimeTotalByUpMonth = _dt.Compute("sum(OvertimeHour)", WhereByUpMonth).ToString();
                if (string.IsNullOrEmpty(overtimeTotalByUpMonth))
                    overtimeTotalByUpMonth = "0";

                //按日期向下汇总（即比指定日期大的），月度内
                string WhereByDownMonth = "OvertimeDate>='" + monthStartDateTime + "' and OvertimeDate<='" + monthEndDateTime + "' and OvertimeDate>'" + AnyDate.toDateFormat() + "'";
                if (!string.IsNullOrEmpty(StartTime))
                    WhereByDownMonth =  "OvertimeDate>='" + monthStartDateTime + "' and OvertimeDate<='" + monthEndDateTime + "' and ((OvertimeDate>'" + AnyDate.toDateFormat() + "') or (OvertimeDate='" + AnyDate.toDateFormat() + "' and StartTime>'" + StartTime + "'))";

                overtimeTotalByDownMonth = _dt.Compute("sum(OvertimeHour)", WhereByDownMonth).ToString();
                if (string.IsNullOrEmpty(overtimeTotalByDownMonth))
                    overtimeTotalByDownMonth = "0";

                //按日期向上汇总（即比指定日期小的），季度内
                string WhereByUpQuarter = "OvertimeDate<'" + AnyDate.toDateFormat() + "'";
                if (!string.IsNullOrEmpty(StartTime))
                    WhereByUpQuarter = "((OvertimeDate<'" + AnyDate.toDateFormat() + "') or (OvertimeDate='" + AnyDate.toDateFormat() + "' and StartTime<'" + StartTime + "'))";

                overtimeTotalByUpQuarter = _dt.Compute("sum(OvertimeHour)", WhereByUpQuarter).ToString();
                if (string.IsNullOrEmpty(overtimeTotalByUpQuarter))
                    overtimeTotalByUpQuarter = "0";

                //按日期向下汇总（即比指定日期大的），季度内
                string WhereByDownQuarter = "OvertimeDate>'" + AnyDate.toDateFormat() + "'"; 
                if (!string.IsNullOrEmpty(StartTime))
                    WhereByDownQuarter = "((OvertimeDate>'" + AnyDate.toDateFormat() + "') or (OvertimeDate='" + AnyDate.toDateFormat() + "' and StartTime>'" + StartTime + "'))"; 

                overtimeTotalByDownQuarter = _dt.Compute("sum(OvertimeHour)", WhereByDownQuarter).ToString();
                if (string.IsNullOrEmpty(overtimeTotalByDownQuarter))
                    overtimeTotalByDownQuarter = "0";


                //当天汇总
                overtimeTotalByDay = _dt.Compute("sum(OvertimeHour)", "OvertimeDate='" + AnyDate.toDateFormat() + "'").ToString();
                if (string.IsNullOrEmpty(overtimeTotalByMonth))
                    overtimeTotalByDay = "0";

                //月度汇总
                overtimeTotalByMonth = _dt.Compute("sum(OvertimeHour)", "OvertimeDate>='" + monthStartDateTime + "' and OvertimeDate<='" + monthEndDateTime + "'").ToString();
                if (string.IsNullOrEmpty(overtimeTotalByMonth))
                    overtimeTotalByMonth = "0";

                //季度汇总
                overtimeTotalByQuarter = _dt.Compute("sum(OvertimeHour)", "").ToString();
                if (string.IsNullOrEmpty(overtimeTotalByQuarter))
                    overtimeTotalByQuarter = "0";


                //汇总已代休总时间（在休假表建立后再写统计代码 *注20210212）
                //alreadyDaiXiu=


                //个人本月延时加班小时
                DataTable _dtWorkOvertime = MicroDataTable.GetPersonalAttendance("HROvertime", "", "", "GetWorkOvertime", UserID, monthStartDateTime, monthEndDateTime, "", DataStateCode);
                if (_dtWorkOvertime != null && _dtWorkOvertime.Rows.Count > 0)
                    workOvertime = _dtWorkOvertime.Compute("sum (OvertimeHour)", "ParentID<>0").toStringTrim();

                //个人季度延时加班小时
                DataTable _dtWorkOvertimeByQuarter = MicroDataTable.GetPersonalAttendance("HROvertime", "", "", "GetWorkOvertime", UserID, quarterStartDateTime, quarterEndDateTime, "", DataStateCode);
                if (_dtWorkOvertimeByQuarter != null && _dtWorkOvertimeByQuarter.Rows.Count > 0)
                    workOvertimeByQuarter = _dtWorkOvertimeByQuarter.Compute("sum (OvertimeHour)", "ParentID<>0").toStringTrim();


                //个人本月休息日加班小时
                DataTable _dtOffDayOvertime = MicroDataTable.GetPersonalAttendance("HROvertime", "", "", "GetOffDayOvertime", UserID, monthStartDateTime, monthEndDateTime, "", DataStateCode);
                if (_dtOffDayOvertime != null && _dtOffDayOvertime.Rows.Count > 0)
                    offDayOvertime = _dtOffDayOvertime.Compute("sum (OvertimeHour)", "ParentID<>0").toStringTrim();

                //个人本月休息日加班小时
                DataTable _dtOffDayOvertimeByQuarter = MicroDataTable.GetPersonalAttendance("HROvertime", "", "", "GetOffDayOvertime", UserID, quarterStartDateTime, quarterEndDateTime, "", DataStateCode);
                if (_dtOffDayOvertimeByQuarter != null && _dtOffDayOvertimeByQuarter.Rows.Count > 0)
                    offDayOvertimeByQuarter = _dtOffDayOvertimeByQuarter.Compute("sum (OvertimeHour)", "ParentID<>0").toStringTrim();


                //个人本月法定加班小时
                DataTable _dtStatutory = MicroDataTable.GetPersonalAttendance("HROvertime", "", "", "GetStatutory", UserID, monthStartDateTime, monthEndDateTime, "", DataStateCode);
                if (_dtStatutory != null && _dtStatutory.Rows.Count > 0)
                    statutory = _dtStatutory.Compute("sum (OvertimeHour)", "ParentID<>0").toStringTrim();

                //个人本月法定加班小时
                DataTable _dtStatutoryByQuarter = MicroDataTable.GetPersonalAttendance("HROvertime", "", "", "GetStatutory", UserID, quarterStartDateTime, quarterEndDateTime, "", DataStateCode);
                if (_dtStatutoryByQuarter != null && _dtStatutoryByQuarter.Rows.Count > 0)
                    statutoryByQuarter = _dtStatutoryByQuarter.Compute("sum (OvertimeHour)", "ParentID<>0").toStringTrim();

            }


            //******统计申请休假或已休假的假期Start******
            //统计时间范围sql语句中默认以季度统计，月度统计在_dt.select中追加条件
            //注意这里的比较时间是写反的(开始对结束，结束对开始)StartDateTime<=@EndDateTime and EndDateTime>=@StartDateTime
            string _sql2 = "select * from HRLeave where Invalid=0 and Del=0 and LeaveUID=@LeaveUID " + WhereStateCode;  //and StartDateTime<=@EndDateTime and EndDateTime>=@StartDateTime 
            SqlParameter[] _sp2 = { new SqlParameter("@LeaveUID", SqlDbType.Int) };

            _sp2[0].Value = UserID;
            //_sp2[1].Value = quarterStartDateTime.toDateTime("yyyy-MM-dd HH:mm:ss.fff");
            //_sp2[2].Value = quarterEndDateTime.toDateTime("yyyy-MM-dd HH:mm:ss.fff");

            DataTable _dt2 = MsSQLDbHelper.Query(_sql2, _sp2).Tables[0];

            if (_dt2 != null && _dt2.Rows.Count > 0)
            {
                //月度休假汇总（含所有类别的休假） 注意这里的比较时间是写反的(开始对结束，结束对开始)
                leaveTotalByMonth = _dt2.Compute("sum(LeaveDays)", "StartDateTime<='" + monthEndDateTime + "' and EndDateTime>='" + monthStartDateTime + "'").ToString();
                if (string.IsNullOrEmpty(leaveTotalByMonth))
                    leaveTotalByMonth = "0";

                //季度休假汇总（含所有类别的休假）
                leaveTotalByQuarter = _dt2.Compute("sum(LeaveDays)", "StartDateTime<='" + quarterEndDateTime + "' and EndDateTime>='" + quarterStartDateTime + "'").ToString();
                if (string.IsNullOrEmpty(leaveTotalByQuarter))
                    leaveTotalByQuarter = "0";

                //需代休（还不知道数据来源和谁运算 注：20210316）
                //NeedDaiXiu=

                //月度已代休 HolidayTypeID=2  //注意这里的比较时间是写反的(开始对结束，结束对开始)
                alreadyDaiXiuByMonth = _dt2.Compute("sum(LeaveDays)", "HolidayTypeID=2 and StartDateTime<='" + monthEndDateTime + "' and EndDateTime>='" + monthStartDateTime + "'").ToString();
                if (string.IsNullOrEmpty(alreadyDaiXiuByMonth))
                    alreadyDaiXiuByMonth = "0";

                //季度已代休 HolidayTypeID=2
                alreadyDaiXiuByQuarter = _dt2.Compute("sum(LeaveDays)", "HolidayTypeID=2 and StartDateTime<='" + quarterEndDateTime + "' and EndDateTime>='" + quarterStartDateTime + "'").ToString();
                if (string.IsNullOrEmpty(alreadyDaiXiuByQuarter))
                    alreadyDaiXiuByQuarter = "0";


                //月度已代休 HolidayTypeID=2  以加班月份进行统计
                alreadyDaiXiuOvertimeByMonth = _dt2.Compute("sum(LeaveDays)", "HolidayTypeID=2 and OvertimeDate>='" + monthStartDateTime.toDateFormat("yyyy-MM") + "' and OvertimeDate<='" + monthEndDateTime.toDateFormat("yyyy-MM") + "'").ToString();
                if (string.IsNullOrEmpty(alreadyDaiXiuOvertimeByMonth))
                    alreadyDaiXiuOvertimeByMonth = "0";

                //季度已代休 HolidayTypeID=2 ,以加班季度进行统计
                alreadyDaiXiuOvertimeByQuarter = _dt2.Compute("sum(LeaveDays)", "HolidayTypeID=2 and OvertimeDate>='" + quarterStartDateTime.toDateFormat("yyyy-MM") + "' and OvertimeDate<='" + quarterEndDateTime.toDateFormat("yyyy-MM") + "' ").ToString();
                if (string.IsNullOrEmpty(alreadyDaiXiuOvertimeByQuarter))
                    alreadyDaiXiuOvertimeByQuarter = "0";

            }

            //******统计申请休假或已休假的假期End******


            //******统计可用的假期（来自个人假期表HRPersonalHoliday）Start******
            string _sql3 = "select * from HRPersonalHoliday where Invalid=0 and Del=0 and UID=@UID";
            SqlParameter[] _sp3 = { new SqlParameter("@UID", SqlDbType.Int) };
            _sp3[0].Value = UserID;

            DataTable _dt3 = MsSQLDbHelper.Query(_sql3, _sp3).Tables[0];

            if (_dt3 != null && _dt3.Rows.Count > 0)
            {
                //剩余年休假 HolidayTypeID=1
                remainingAnnualLeave = _dt3.Compute("sum(Days)", "HolidayTypeID=1").ToString();
                if (string.IsNullOrEmpty(remainingAnnualLeave))
                    remainingAnnualLeave = "0";

                //剩余其它假 HolidayTypeID=1
                remainingOtherLeave = _dt3.Compute("sum(Days)", "HolidayTypeID<>1").ToString();
                if (string.IsNullOrEmpty(remainingOtherLeave))
                    remainingOtherLeave = "0";
            }

            //******统计可用的假期（来自个人假期表HRPersonalHoliday）End******


            //月度剔除代休后的加班时间=月度总加班时间-月度以加班月份为统计条件的代休时间
            //exceptDaiXiuByMonth=overtimeTotalByMonth-alreadyDaiXiuOvertimeByMonth
            if (overtimeTotalByMonth.toDecimal() >= 0 && alreadyDaiXiuOvertimeByMonth.toDecimal() >= 0)
            {
                exceptDaiXiuByMonth = (overtimeTotalByMonth.toDecimal() - (alreadyDaiXiuOvertimeByMonth.toDecimal() * 8)).toStringTrim();  //因为已代休的值是天数（从HRLeave表中取），需要转换为时间进行计算

                //剩余可代休（即剩余休日加班）=休日总加班时间-已代休时间(以加班月份为准)，通常用于休假单用于可休选项
                remainingDaiXiu = (offDayOvertime.toDecimal() - (alreadyDaiXiuOvertimeByMonth.toDecimal() * 8)).toStringTrim();
            }

            //季度剔除代休后的加班时间=季度总加班时间-季度以加班月份为统计条件的代休时间
            //exceptDaiXiuByQuarter=overtimeTotalByQuarter-alreadyDaiXiuOvertimeByQuarter
            if (overtimeTotalByQuarter.toDecimal() >= 0 && alreadyDaiXiuOvertimeByQuarter.toDecimal() >= 0)
            {
                exceptDaiXiuByQuarter = (overtimeTotalByQuarter.toDecimal() - (alreadyDaiXiuOvertimeByQuarter.toDecimal() * 8)).toStringTrim();  //因为已代休的值是天数（从HRLeave表中取），需要转换为时间进行计算

                //季度剩余可代休（即剩余休日加班）=休日总加班时间-已代休时间(以加班季度为准)，通常用于休假单用于可休选项
                remainingDaiXiuByQuarter = (offDayOvertimeByQuarter.toDecimal() - (alreadyDaiXiuOvertimeByQuarter.toDecimal() * 8)).toStringTrim();
            }

            //季度剔除代休后的加班时间
            //exceptDaiXiuByQuarter



            //标准工时制，和默认没设置工时制时
            if (WorkHourSysID == 1 || WorkHourSysID == 0 || WorkHourSysID == 3)
            {
                overtimeTotal = overtimeTotalByMonth;
                overtimeTotalByUp = overtimeTotalByUpMonth;
                overtimeTotalByDown = overtimeTotalByDownMonth;
                leaveTotal = leaveTotalByMonth;
                alreadyDaiXiu = alreadyDaiXiuByMonth;
                alreadyDaiXiuOvertime = alreadyDaiXiuOvertimeByMonth;
                exceptDaiXiu = exceptDaiXiuByMonth;
            }

            //综合工时制
            if (WorkHourSysID == 2)
            {
                overtimeTotal = overtimeTotalByQuarter;
                overtimeTotalByUp = overtimeTotalByUpQuarter;
                overtimeTotalByDown = overtimeTotalByDownQuarter;
                leaveTotal = leaveTotalByQuarter;
                alreadyDaiXiu = alreadyDaiXiuByQuarter;
                alreadyDaiXiuOvertime = alreadyDaiXiuOvertimeByQuarter;
                exceptDaiXiu = exceptDaiXiuByQuarter;
            }


            var PersonalRecordAttr = new PersonalRecordAttr
            {
                MonthStartDateTime = monthStartDateTime.toDateFormat(),
                MonthEndDateTime = monthEndDateTime.toDateFormat(),
                QuarterStartDateTime = quarterStartDateTime.toDateFormat(),
                QuarterEndDateTime = quarterEndDateTime.toDateFormat(),
                OvertimeTotal = overtimeTotal,
                OvertimeTotalByUp = overtimeTotalByUp,
                OvertimeTotalByDown = overtimeTotalByDown,
                OvertimeTotalByUpMonth = overtimeTotalByUpMonth,
                OvertimeTotalByDownMonth = overtimeTotalByDownMonth,
                OvertimeTotalByUpQuarter = overtimeTotalByUpQuarter,
                OvertimeTotalByDownQuarter = overtimeTotalByDownQuarter,
                OvertimeTotalByDay = overtimeTotalByDay,
                OvertimeTotalByMonth = overtimeTotalByMonth,
                OvertimeTotalByQuarter = overtimeTotalByQuarter,
                WorkOvertime = workOvertime,
                WorkOvertimeByQuarter = workOvertimeByQuarter,
                OffDayOvertime = offDayOvertime,
                OffDayOvertimeByQuarter = offDayOvertimeByQuarter,
                Statutory = statutory,
                StatutoryByQuarter = statutoryByQuarter,

                LeaveTotal = leaveTotal,
                LeaveTotalByMonth = leaveTotalByMonth,
                LeaveTotalByQuarter = leaveTotalByQuarter,

                AlreadyDaiXiu = alreadyDaiXiu,
                AlreadyDaiXiuByMonth = alreadyDaiXiuByMonth,
                AlreadyDaiXiuByQuarter = alreadyDaiXiuByQuarter,

                AlreadyDaiXiuOvertime = alreadyDaiXiuOvertime,
                AlreadyDaiXiuOvertimeByMonth = alreadyDaiXiuOvertimeByMonth,
                AlreadyDaiXiuOvertimeByQuarter = alreadyDaiXiuOvertimeByQuarter,

                ExceptDaiXiu = exceptDaiXiu,
                ExceptDaiXiuByMonth = exceptDaiXiuByMonth,
                ExceptDaiXiuByQuarter = exceptDaiXiuByQuarter,

                RemainingDaiXiu = remainingDaiXiu,
                RemainingDaiXiuByQuarter = remainingDaiXiuByQuarter,
                RemainingAnnualLeave = remainingAnnualLeave,
                RemainingOtherLeave = remainingOtherLeave,
                SelfDriving = selfDriving
            };

            return PersonalRecordAttr;
        }


        /// <summary>
        /// 加班小时计算用于平时加班，根据班次和选择时长计算出开始时间、结束时间、加班小时的算法
        /// </summary>
        public class OTHourAttr
        {
            //加班小时（表单选择的小时）
            public string OTHour { set; get; }

            //加班分钟（表单选择的分钟）
            public string OTMin { set; get; }

            //开始时间
            public string StartTime { set; get; }

            //开始日期时间
            public string StartDateTime { set; get; }

            //结束时间
            public string EndTime { set; get; }

            //结束日期时间
            public string EndDateTime { set; get; }

            //实际结束时间
            public string ActualEndTime { set; get; }

            //实际结束日期时间
            public string ActualEndDateTime { set; get; }

            //加班时间， StartTime~EndTime
            public string OvertimeTime { set; get; }

            //实际持续时间（即实际加班时长）
            public string DurationTime { set; get; }

            //补时(根据如果需要则补)
            public string AddedTime { set; get; }

            // 就餐时间
            public string MealTime { set; get; }

            //实际加班小时（扣除就餐时间或满7:25时补满8H等)（是对小时和分钟的合并，如：1H和30分=>1.5H）
            public string OvertimeHour { set; get; }

        }

        /// <summary>
        /// 加班小时计算用于平时加班，根据班次和选择时长计算出开始时间、结束时间、加班小时的算法
        /// </summary>
        /// <param name="OvertimeDate"></param>
        /// <param name="OvertimeTime"></param>
        /// <param name="OvertimeMealID"></param>
        /// <param name="ShiftTypeID"></param>
        /// <returns></returns>
        public static OTHourAttr GetOTHour(string ShiftTypeID, string OvertimeDate, string OTStartTime, string OTHour, string OTMin, string OvertimeMealID)
        {
            string startTime = string.Empty, startDateTime = string.Empty, endTime = string.Empty, endDateTime = string.Empty, actualEndTime = string.Empty, actualEndDateTime = string.Empty, overtimeTime = string.Empty, durationTime = string.Empty, addedTime = "0", mealTime = string.Empty, overtimeHour = string.Empty;

            if (!string.IsNullOrEmpty(OvertimeDate) && !string.IsNullOrEmpty(OTStartTime))
            {
                Boolean IsHoliday = CheckIsHoliday(OvertimeDate); //检查是否为周末

                //开始时间
                startTime = OTStartTime;  //HH:mm
                //开始日期时间
                startDateTime = OvertimeDate + " " + OTStartTime;  //日期和时间拼接成 yyyy-MM-dd HH:ss的格式

                //第一次计算结束时间（由开始时间加上加班小时和分钟计算得出）
                //结束时间
                actualEndTime = DateTime.Parse(startDateTime).AddHours(OTHour.toInt()).AddMinutes(OTMin.toInt()).ToString("HH:mm");
                //结束日期时间（由开始日期时间加上加班小时和加班分钟）
                actualEndDateTime = DateTime.Parse(startDateTime).AddHours(OTHour.toInt()).AddMinutes(OTMin.toInt()).ToString("yyyy-MM-dd HH:mm");

                //第一次计算持续时间
                //计算持续时间=otHour+otMin（即加班时间）
                durationTime = (OTHour.toInt() * 60 + OTMin.toInt()).ToString();  //避免出现小数，把时间转为分钟进行计算

                //得到就餐总时间（H），如果是平日则返回30分钟， 如果是周末就餐第一餐45分钟，第二餐30分钟，如吃了两餐则 45+30=75/60，则返回1.25小时
                mealTime = GetOvertimeMealTime(IsHoliday, ShiftTypeID, OTHour, OTMin, OvertimeMealID);

                //这里开始计算出需要补时的时间（根据符合的条件，如周末的2S和3S并且持续时间达到标准加班小时的时候）
                addedTime = GetAddedTime(IsHoliday, ShiftTypeID, OTHour, OTMin);

                //加班小时
                overtimeHour = durationTime.toOTRound();

                //计下班时间，如果有就餐时间则加上就餐时间扣除补时时间作为下班时间
                endTime = DateTime.Parse(actualEndTime).AddMinutes(mealTime.toInt() - addedTime.toInt()).ToString("HH:mm");
                endDateTime = DateTime.Parse(actualEndDateTime).AddMinutes(mealTime.toInt() - addedTime.toInt()).ToString("yyyy-MM-dd HH:mm");

                //加班时间（开始时间~结束时间）
                overtimeTime = startTime + "~" + endTime;

            }

            var OTHourAttr = new OTHourAttr
            {
                OTHour = OTHour,
                OTMin = OTMin,
                StartTime = startTime,
                StartDateTime = startDateTime,
                EndTime = endTime,
                EndDateTime = endDateTime,
                ActualEndTime = actualEndTime,
                ActualEndDateTime = actualEndDateTime,
                OvertimeTime = overtimeTime,
                DurationTime = (durationTime.toDecimal() / 60.toDecimal()).ToString(),
                AddedTime = addedTime,
                MealTime = mealTime,
                OvertimeHour = overtimeHour

            };

            return OTHourAttr;
        }


        /// <summary>
        /// 计算补时
        /// </summary>
        /// <param name="ShiftTypeID"></param>
        /// <param name="OvertimeDate"></param>
        /// <param name="IsHoliday"></param>
        /// <param name="DurationTime"></param>
        /// <param name="OvertimeMealID"></param>
        /// <returns></returns>
        public static string GetAddedTime(Boolean IsHoliday, string ShiftTypeID, string OTHour, string OTMin)
        {
            string flag = "0";

            if (IsHoliday)
            {
                int OTHourMin = OTHour.toInt() * 60 + OTMin.toInt();

                //广州2S持续时间>=7:50则，补时10分钟
                if (ShiftTypeID == "3")
                {
                    int StrandHourMin = 7 * 60 + 50; //7:50
                    if (OTHourMin >= StrandHourMin)
                        flag = "10";
                }

                //广州3S持续时间>=7:30则，补时30分钟
                if (ShiftTypeID == "4")
                {
                    int StrandHourMin = 7 * 60 + 30; //7:30
                    if (OTHourMin >= StrandHourMin)
                        flag = "30";
                }

                //武汉2S持续时间>=7:50则，补时10分钟
                if (ShiftTypeID == "7")
                {
                    int StrandHourMin = 7 * 60 + 55; //7:55
                    if (OTHourMin >= StrandHourMin)
                        flag = "5";
                }

                //武汉3S持续时间>=7:25则，补时35分钟
                if (ShiftTypeID == "8")
                {
                    int StrandHourMin = 7 * 60 + 25; //7:25
                    if (OTHourMin >= StrandHourMin)
                        flag = "35";
                }

                //flag = "3";

            }


            return flag;
        }

        /// <summary>
        /// 用户工时制属性
        /// </summary>
        public class UserWorkHourSystemAttr
        {
            /// <summary>
            /// 工时制ID，1=标准，2=综合，3=不定时
            /// </summary>
            public string WorkHourSysID { set; get; }

            public string WorkHourSysName { set; get; }

            public string WarningValue { set; get; }

            public string MaxValue { set; get; }

        }

        /// <summary>
        /// 获取用户工时制
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static UserWorkHourSystemAttr GetUserWorkHourSystem(int UID, string StartDate, string EndDate = "")
        {
            //工时制ID，1=标准，2=综合，3=不定时
            string workHourSysID = "0",
                workHourSysName = "未设置",
                warningValue = "28",
                maxValue = "36";

            StartDate = StartDate.toDateFormat("yyyy-MM-dd") + " 00:00:00.000";

            string _sql = "select * from HRUserWorkHourSystem where Invalid=0 and Del=0 and UID=@UID order by WorkHourSysDate desc, DateModified desc";
            SqlParameter[] _sp = { new SqlParameter("@UID", SqlDbType.Int) };
            _sp[0].Value = UID;

            DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

            if (_dt.Rows.Count > 0)
            {

                DataRow[] _rows = _dt.Select("", "WorkHourSysDate desc, DateModified desc");

                if (!string.IsNullOrEmpty(StartDate) && string.IsNullOrEmpty(EndDate))  //开始时间不为空，结束时间为空
                    _rows = _dt.Select("WorkHourSysDate<='" + StartDate + "'", "WorkHourSysDate desc, DateModified desc");
                else if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))  //开始和结束时间都不为空
                    _rows = _dt.Select("WorkHourSysDate>='" + StartDate + "' and WorkHourSysDate<='" + EndDate + "'", "WorkHourSysDate desc, DateModified desc");

                if (_rows.Length > 0)
                    workHourSysID = _rows[0]["WorkHourSysID"].toStringTrim();
                else
                    workHourSysID = _dt.Rows[0]["WorkHourSysID"].toStringTrim();  //如果没有记录则返回_dt的数据


                string _sql2 = "select * from HRWorkHourSystem where Invalid=0 and Del=0 and WorkHourSysID=@WorkHourSysID";
                SqlParameter[] _sp2 = { new SqlParameter("@WorkHourSysID", SqlDbType.Int) };

                _sp2[0].Value = workHourSysID.toInt();

                DataTable _dt2 = MsSQLDbHelper.Query(_sql2, _sp2).Tables[0];

                if (_dt2.Rows.Count > 0)
                {
                    workHourSysName = _dt2.Rows[0]["WorkHourSysName"].toStringTrim();
                    warningValue = _dt2.Rows[0]["WarningValue"].toStringTrim();
                    maxValue = _dt2.Rows[0]["MaxValue"].toStringTrim();
                }

            }

            var UserWorkHourSystemAttr = new UserWorkHourSystemAttr
            {
                WorkHourSysID = workHourSysID,
                WorkHourSysName = workHourSysName,
                WarningValue = warningValue,
                MaxValue = maxValue
            };

            return UserWorkHourSystemAttr;
        }


        /// <summary>
        /// 用户默认班次属性
        /// </summary>
        public class UserDefaultShiftTypeAttr
        {
            /// <summary>
            /// 班次ID
            /// </summary>
            public string ShiftTypeID { set; get; }

            /// <summary>
            /// 班次名称
            /// </summary>
            public string ShiftName { set; get; }

            /// <summary>
            /// 班次别名
            /// </summary>
            public string Alias { set; get; }

            /// <summary>
            /// 班次开始时间
            /// </summary>
            public string StartTime { set; get; }

            /// <summary>
            /// 班次结束时间
            /// </summary>
            public string EndTime { set; get; }

            /// <summary>
            /// 班次休息时间
            /// </summary>
            public string RestTime { set; get; }


            /// <summary>
            /// 班次标准上班时间
            /// </summary>
            public string StandardOvertime { set; get; }

        }

        /// <summary>
        /// 获取用户默认班次相关属性（通常是用户没有排班数据时，通过用户所属部门匹配ShiftType表的默认班次记录）
        /// </summary>
        /// <param name="OvertimeDate"></param>
        /// <param name="OvertimeTime"></param>
        /// <param name="OvertimeMealID"></param>
        /// <param name="ShiftTypeID"></param>
        /// <returns></returns>
        public static UserDefaultShiftTypeAttr GetUserDefaultShiftType(string UID)
        {
            string shiftTypeID = string.Empty,
                shiftName = string.Empty,
                alias = string.Empty,
                startTime = string.Empty,
                endTime = string.Empty,
                restTime = string.Empty,
                standardOvertime = string.Empty;

            string ParentDeptsID = MicroUserInfo.GetUserInfo("ParentDeptsID", UID); //返回当前登录用户的所属部门及所有父级部门组成的字符串

            //班次数据
            DataTable _dt = MicroDataTable.GetDataTable("ShiftType");
            DataTable _dtTarget = _dt.Clone();

            DataRow[] _rows = _dt.Select("", "Sort");

            if (_rows.Length > 0)
            {
                foreach (DataRow _dr in _rows)
                {
                    string DeptIDs = _dr["DeptIDs"].toStringTrim();
                    if (MicroPublic.CheckSplitExistss(DeptIDs, ParentDeptsID, ','))
                        _dtTarget.Rows.Add(_dr.ItemArray);
                }
            }

            //获取部门默认班次
            //DataRow[] _rows = _dt.Select("DefaultShift=1 and DeptID in (" + ParentDeptsID + ")");

            if (_dtTarget != null && _dtTarget.Rows.Count > 0)
            {
                _rows = _dtTarget.Select("DefaultShift=1", "Sort");
                if (_rows.Length == 0)
                    _rows = _dtTarget.Select("", "Sort");
            }
            else
            {
                _rows = _dt.Select("DefaultShift=1", "Sort");
                if (_rows.Length == 0)
                    _rows = _dt.Select("", "Sort");
            }

            if (_rows.Length > 0)
            {
                shiftTypeID = _rows[0]["ShiftTypeID"].toStringTrim();
                shiftName = _rows[0]["ShiftName"].toStringTrim();
                alias = _rows[0]["Alias"].toStringTrim();
                startTime = _rows[0]["StartTime"].toStringTrim();
                endTime = _rows[0]["EndTime"].toStringTrim();
                restTime = _rows[0]["RestTime"].toStringTrim();
                standardOvertime = _rows[0]["StandardOvertime"].toStringTrim();
            }


            var UserDefaultShiftTypeAttr = new UserDefaultShiftTypeAttr
            {
                ShiftTypeID = shiftTypeID,
                ShiftName = shiftName,
                Alias = alias,
                StartTime = startTime,
                EndTime = endTime,
                RestTime = restTime,
                StandardOvertime = standardOvertime

            };

            return UserDefaultShiftTypeAttr;

        }


        /// <summary>
        /// 用户排班属性
        /// </summary>
        public class UserOnDutyAttr
        {
            /// <summary>
            /// 班次ID
            /// </summary>
            public string ShiftTypeID { set; get; }

            /// <summary>
            /// 班次名称
            /// </summary>
            public string ShiftName { set; get; }

            /// <summary>
            /// 班次别名
            /// </summary>
            public string Alias { set; get; }

            /// <summary>
            /// 班次开始时间
            /// </summary>
            public string StartTime { set; get; }

            /// <summary>
            /// 班次结束时间
            /// </summary>
            public string EndTime { set; get; }

            /// <summary>
            /// 班次休息时间
            /// </summary>
            public string RestTime { set; get; }


            /// <summary>
            /// 班次标准上班时间
            /// </summary>
            public string StandardOvertime { set; get; }

        }

        /// <summary>
        /// 获取用户班次相关属性，根据传入的UID、日期返回该用户、该日期的班次，（先根据日期的格式依次查询yyyy-MM-dd、yyyy-MM、日期倒序即最近的一次）
        /// </summary>
        /// <param name="OvertimeDate"></param>
        /// <param name="OvertimeTime"></param>
        /// <param name="OvertimeMealID"></param>
        /// <param name="ShiftTypeID"></param>
        /// <returns></returns>
        public static UserOnDutyAttr GetUserOnDuty(string UID, string AnyDate)
        {
            string shiftTypeID = string.Empty,
                shiftName = string.Empty,
                alias = string.Empty,
                startTime = string.Empty,
                endTime = string.Empty,
                restTime = string.Empty,
                standardOvertime = string.Empty;


            string StartDateTime = AnyDate.toDateMFirstDay() + " 00:00:00.000",
                   EndDateTime = AnyDate.toDateMLastDay() + " 23:59:59.998";

            string _sql = "select a.DutyDate,a.DateCreated, b.ShiftTypeID,b.ShiftName,b.Alias,b.StartTime,b.EndTime,b.RestTime,b.StandardOvertime from HROnDutyForm a left join HRShiftType b on a.ShiftTypeID=b.ShiftTypeID where a.Invalid=0 and a.Del=0 and a.StateCode=100 and a.UID=@UID and DutyDate between @StartDateTime and @EndDateTime order by a.DateCreated desc";  //以创建时间倒序排列
            SqlParameter[] _sp = { new SqlParameter("@UID", SqlDbType.Int),
                        new SqlParameter("@StartDateTime", SqlDbType.VarChar,30),
                        new SqlParameter("@EndDateTime", SqlDbType.VarChar,30)
                                };
            _sp[0].Value = UID.toInt();
            _sp[1].Value = StartDateTime;
            _sp[2].Value = EndDateTime;

            DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

            //如果有排班记录
            if (_dt != null && _dt.Rows.Count > 0)
            {
                AnyDate = AnyDate.toDateFormat();
                DataRow[] _rows = _dt.Select("DutyDate='" + AnyDate + "'", "DateCreated desc");   //先按等于AnyDate查询
                if (_rows.Length == 0)
                    _rows = _dt.Select("", "DateCreated desc");  //如果没有记录则按AnyDate所在月份的第一条记录进行查询

                shiftTypeID = _rows[0]["ShiftTypeID"].toStringTrim();
                shiftName = _rows[0]["ShiftName"].toStringTrim();
                alias = _rows[0]["Alias"].toStringTrim();
                startTime = _rows[0]["StartTime"].toStringTrim();
                endTime = _rows[0]["EndTime"].toStringTrim();
                restTime = _rows[0]["RestTime"].toStringTrim();
                standardOvertime = _rows[0]["StandardOvertime"].toStringTrim();
            }
            //如果没有排班信息则用默认班次获取
            else
            {             
                var getUserDefaultShiftType = GetUserDefaultShiftType(UID);

                shiftTypeID = getUserDefaultShiftType.ShiftTypeID;
                shiftName = getUserDefaultShiftType.ShiftName;
                alias = getUserDefaultShiftType.Alias;
                startTime = getUserDefaultShiftType.StartTime;
                endTime = getUserDefaultShiftType.EndTime;
                restTime = getUserDefaultShiftType.RestTime;
                standardOvertime = getUserDefaultShiftType.StandardOvertime;

            }


            var UserOnDutyAttr = new UserOnDutyAttr
            {
                ShiftTypeID = shiftTypeID,
                ShiftName = shiftName,
                Alias = alias,
                StartTime = startTime,
                EndTime = endTime,
                RestTime = restTime,
                StandardOvertime = standardOvertime

            };

            return UserOnDutyAttr;

        }


        /// <summary>
        /// 加班小时计算（第一版测试计算方法），根据选择开始时间和结束时间算出持续时间、实际加班时间等的算法
        /// </summary>
        public class OvertimeHourAttr
        {
            //开始时间
            public string StartTime { set; get; }

            //结束时间
            public string EndTime { set; get; }

            //持续时间
            public string DurationTime { set; get; }

            // 就餐时间
            public string MealTime { set; get; }


            //持续时间减去就餐时间
            public string DurationTimeDiffMealTime { set; get; }

            //持续小时
            public string DurationHour { set; get; }

            //加班小时
            public string OvertimeHour { set; get; }

        }

        /// <summary>
        /// 加班小时计算（第一版测试计算方法），根据选择开始时间和结束时间算出持续时间、实际加班时间等的算法
        /// </summary>
        /// <param name="OvertimeDate"></param>
        /// <param name="OvertimeTime"></param>
        /// <param name="OvertimeMealID"></param>
        /// <param name="ShiftTypeID"></param>
        /// <returns></returns>
        public static OvertimeHourAttr GetOvertimeHour(string OvertimeDate, string OvertimeTime, string OvertimeMealID, string ShiftTypeID)
        {
            string startTime = string.Empty, endTime = string.Empty, durationTime = string.Empty, durationHour = string.Empty, overtimeHour = string.Empty, Hour = string.Empty, Min = string.Empty;
            string durationTimeDiffmealTime = string.Empty; //持续时间减去就餐时间
            string mealTime = string.Empty; //就餐总时间，有多少餐则计算多少餐的总和

            if (!string.IsNullOrEmpty(OvertimeTime))
            {
                startTime = MicroPublic.GetSplitFirstStr(OvertimeTime, '-').toStringTrim();
                endTime = MicroPublic.GetSplitLastStr(OvertimeTime, '-').toStringTrim();

                if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
                {
                    startTime = OvertimeDate + " " + startTime;
                    endTime = OvertimeDate + " " + endTime;
                    //mealTime = GetOvertimeMealTime(OvertimeMealID);  //单位小时

                    //结束时间小于 等于开始时间，则结束日期的天数上自动加1天
                    if (endTime.toDateTime("HH:mm") <= startTime.toDateTime("HH:mm"))
                        endTime = endTime.toDateTime("yyyy-MM-dd HH:mm").AddDays(1).toDateFormat("yyyy-MM-dd HH:mm");

                    //整个申请实际持续时间(d,h,m,s:)
                    var getDateTimeDiff = MicroPublic.GetDateTimeDiff(startTime, endTime);
                    durationTime = getDateTimeDiff.Days + "d " + getDateTimeDiff.Hours + ":" + getDateTimeDiff.Minutes + ":" + getDateTimeDiff.Seconds;

                    //持续小时(H)
                    durationHour = decimal.Round(getDateTimeDiff.TotalHours.toDecimal(), 1).ToString();

                    //加班小时
                    overtimeHour = durationHour;

                    //武汉3S计算加班持续时间大于等于7:25:00时，补时45分钟
                    if (ShiftTypeID.toInt() == 8)
                    {
                        decimal WH3sDurationTime = 25 / 60;  //设置武汉3S持续时间，用于3S申请加班作比较，如果大于等于此值则加45分钟，即0.75小时
                        if (overtimeHour.toDecimal() >= WH3sDurationTime)
                            overtimeHour = (overtimeHour.toDecimal() + ("0.6").toDecimal()).ToString();
                    }

                    //扣除就餐时间后的加班小时（持续时间减去就餐时间）
                    if (!string.IsNullOrEmpty(ShiftTypeID))
                        overtimeHour = (overtimeHour.toDecimal() - mealTime.toDecimal()).ToString();

                    durationTimeDiffmealTime = overtimeHour;
                }

            }

            var OvertimeHourAttr = new OvertimeHourAttr
            {
                StartTime = startTime,
                EndTime = endTime,
                DurationTime = durationTime,
                MealTime = mealTime,
                DurationTimeDiffMealTime = durationTimeDiffmealTime,
                DurationHour = durationHour,
                OvertimeHour = overtimeHour
            };

            return OvertimeHourAttr;

        }



        /// <summary>
        /// 获取总就餐时间（返回分钟）。根据传入的就餐ID逗号分隔字符串，返回每个ID的时间汇总
        /// </summary>
        /// <param name="OvertimeMealID"></param>
        /// <returns></returns>
        public static string GetOvertimeMealTime(Boolean IsHoliday, string ShiftTypeID, string OTHour, string OTMin, string OvertimeMealID)
        {
            string flag = "0";
            string[] ArrOvertimeMealID = OvertimeMealID.Split(',');

            //OvertimeMealID=5代表是无餐（所以要加上不等于5）
            if (!string.IsNullOrEmpty(OvertimeMealID) && OvertimeMealID != "5")
            {
                //读取就餐表所有数据
                DataTable _dt = MicroDataTable.GetDataTable("OvertimeMeal");

                int MealTime = 0;
                for (int i = 0; i < ArrOvertimeMealID.Length; i++)
                {
                    int overtimeMealID = ArrOvertimeMealID[i].toInt();
                    if (_dt.Select("OvertimeMealID=" + overtimeMealID).Length > 0)
                    {
                        string _MealTime = string.Empty;

                        //如果是休日加班，第一餐45分钟，用i=0进行判断
                        if (IsHoliday)
                        {
                            if (i == 0) //第一餐45分钟
                                _MealTime = _dt.Select("OvertimeMealID=" + overtimeMealID)[0]["OvertimeMealTime2"].toStringTrim(); //OvertimeMealTime2=获取周末加班餐的时间，即45分钟
                            else  //第二餐，如果有
                                _MealTime = _dt.Select("OvertimeMealID=" + overtimeMealID)[0]["OvertimeMealTime"].toStringTrim();  //OvertimeMealTime=获取平时加班餐的时间，即30分钟
                        }
                        else  //如果是平日则30分钟
                            _MealTime = _dt.Select("OvertimeMealID=" + overtimeMealID)[0]["OvertimeMealTime"].toStringTrim();

                        if (!string.IsNullOrEmpty(_MealTime))
                            MealTime += _MealTime.toInt();

                    }
                }
                flag = MealTime.ToString();
            }
            else  //否则如果没有选择就餐时，如果加班时间超过该班次的标准时间则主动加上休息时间
            {
                DataTable _dtShiftType = MicroDataTable.GetDataTable("ShiftType");
                DataRow[] _rowsShiftType = _dtShiftType.Select("ShiftTypeID=" + ShiftTypeID.toInt());
                if (_rowsShiftType.Length > 0)
                {
                    int OTHourMin = OTHour.toInt() * 60 + OTMin.toInt();
                    int StandardOvertime = _rowsShiftType[0]["StandardOvertime"].toInt();
                    if (OTHourMin >= StandardOvertime)
                        flag = _rowsShiftType[0]["RestTime"].ToString();
                }

            }

            return flag;
        }



        public class OTStartEndTimeAttr
        {

            /// <summary>
            /// 加班日期
            /// </summary>
            public string OvertimeDate { set; get; }

            //加班开始时间
            public string OTStartTime { set; get; }

            //加班结束时间
            public string OTEndTime { set; get; }
        }

        /// <summary>
        /// 传入加班日期和班次返回加班属性供加班选择时间用（如加班开始日期、加班结束日期等）。同时与日历进行计算如果是周末则返回开始时间是00:00:00，结束时间是23:59:59(即时间选择不受限制)
        /// </summary>
        /// <param name="OvertimeDate"></param>
        /// <param name="ShiftTypeID"></param>
        /// <returns></returns>
        public static OTStartEndTimeAttr GetOTStartEndTime(string OvertimeDate, string ShiftTypeID)
        {
            string overtimeDate = OvertimeDate,
                otStartTime = "00:00:00",
                otEndTime = "23:59:59";

            //先检查加班日期是不是休息日，如果不是再继续
            string _sql = "select * from CalendarDays where Invalid=0 and Del=0 and DaysType=@DaysType and DayDate=@DayDate";
            SqlParameter[] _sp = { new SqlParameter("@DaysType",SqlDbType.VarChar,100),
                                    new SqlParameter("@DayDate",SqlDbType.DateTime)
                                    };

            _sp[0].Value = "Holiday";
            _sp[1].Value = DateTime.Parse(overtimeDate);

            DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

            if (_dt.Rows.Count == 0)
            {

                DataTable _dtShiftType = MicroDataTable.GetDataTable("ShiftType");
                DataRow[] _rowsShiftType = _dtShiftType.Select("ShiftTypeID=" + ShiftTypeID.toInt());
                if (_rowsShiftType.Length > 0)
                {
                    otStartTime = _rowsShiftType[0]["OTStartTime"].toStringTrim();
                    if (string.IsNullOrEmpty(otStartTime))
                        otStartTime = "00:00:00";

                    otEndTime = _rowsShiftType[0]["OTEndTime"].toStringTrim();
                    if (string.IsNullOrEmpty(otEndTime))
                        otEndTime = "23:59:59";

                    otStartTime = DateTime.Parse(otStartTime).ToString("HH:mm:ss");
                    otEndTime = DateTime.Parse(otEndTime).ToString("HH:mm:ss");
                }

            }

            var OTStartEndTimeAttr = new OTStartEndTimeAttr
            {
                OvertimeDate = OvertimeDate,
                OTStartTime = otStartTime,
                OTEndTime = otEndTime

            };

            return OTStartEndTimeAttr;
        }



        /// <summary>
        /// 获取加班小时允许选择的最大值（含平时和周末），根据传入的日期、班次和加班者（OvertimeUID判断加班者的工时制），综合工时制员工不作限制，返回json结果(供Select框用)如：text:1,value:1,..text:n,value:n
        /// </summary>
        /// <param name="FormID"></param>
        /// <param name="OvertimeDate"></param>
        /// <param name="OvertimeUID"></param>
        /// <returns></returns>
        public static string GetOvertimeMax(string FormID, string OvertimeDate, string OvertimeUID = "")
        {
            string MaxValue = "24", Interval = "1";

            Boolean Standard = false;  //假设默认为综合工时制，不限加班小时数

            //又有标准又有综合的时候即时推翻，以标准的为准（即以允许加班小时最低为准）
            int Num = 0;
            if (!string.IsNullOrEmpty(OvertimeUID))
            {
                string[] ArrOvertimeUID = OvertimeUID.Split(',');
                for (int i = 0; i < ArrOvertimeUID.Length; i++)
                {
                    if (Num == 0)
                    {
                        var getUserWorkHourSystem = MicroHR.GetUserWorkHourSystem(ArrOvertimeUID[i].toInt(), OvertimeDate);
                        if ((getUserWorkHourSystem.WorkHourSysID).toInt() == 1)  //等于1，代码为标准工时制
                            Num = Num + 1;
                    }
                }

                if (Num > 0)
                    Standard = true;
            }

            if (Standard)
            {
                //如果返回True, 说明是休息日
                Boolean IsHoliday = CheckIsHoliday(OvertimeDate);

                //读取加班类型表
                var getOvertimeType = GetOvertimeType(FormID);
                Interval = getOvertimeType.Interval;

                //如果是休息日则读取休息日的允许最大值
                if (IsHoliday)
                    MaxValue = getOvertimeType.WeekendMax;

                else  //否则是平日加班允许最大值                 
                    MaxValue = getOvertimeType.WeekdaysMax;

            }

            return GetJsonString(MaxValue, Interval);
        }


        /// <summary>
        /// 传入允许最大值和间隔返回构造好的json格式的数据
        /// </summary>
        /// <param name="MaxValue"></param>
        /// <param name="Interval"></param>
        /// <param name="TextValue">Days或者Hour，由Name:Value,Name2:Value2逗号分隔组成的字符串，该值不为空时从这里取值</param>
        /// <returns></returns>
        private static string GetJsonString(string MaxValue, string Interval, string TextValue = "")
        {
            string flag = string.Empty, strTemp = "\"text\":\"{0}\",\"value\":\"{1}\""; //构造格式字符串  {"text":"显示内容","value":"1"}  
            StringBuilder sb = new StringBuilder();
            try
            {
                if (string.IsNullOrEmpty(TextValue))
                {
                    decimal decMaxValue = MaxValue.toDecimal(),
                        decInterval = Interval.toDecimal();

                    if (decMaxValue > 0 && decInterval > 0)
                    {
                        for (decimal i = 0; i <= decMaxValue; i = i + decInterval)
                        {
                            string str = string.Format(strTemp, i, i);
                            sb.Append("{" + str + "},");
                        }
                    }
                    else
                    {
                        string str = string.Format(strTemp, 0, 0);
                        sb.Append("{" + str + "},");
                    }
                }
                else
                {
                    //arrTextValueByDays=Name:Value,Name2:Value2...
                    if (!string.IsNullOrEmpty(TextValue))  //By Days
                    {
                        string[] arrTextValue = TextValue.Split(',');
                        for (int i = 0; i < arrTextValue.Length; i++)
                        {
                            string[] arrTxtVal = arrTextValue[i].Split(':');
                            string str = string.Format(strTemp, arrTxtVal[0], arrTxtVal[1]);
                            sb.Append("{" + str + "},");
                        }
                    }
                    else
                    {
                        string str = string.Format(strTemp, 0, 0);
                        sb.Append("{" + str + "},");
                    }


                }

                flag = sb.ToString();
                flag = "[" + flag.Substring(0, flag.Length - 1) + "]";
            }
            catch
            { }

            return flag;

        }


        /// <summary>
        /// 获取加班类型相关属性
        /// </summary>
        public class OvertimeTypeAttr
        {
            /// <summary>
            /// 加班类型代码
            /// </summary>
            public string OvertimeTypeCode { set; get; }

            /// <summary>
            /// 平日允许最大加班小时数
            /// </summary>
            public string WeekdaysMax { set; get; }

            /// <summary>
            /// 休日允许最大加班小时数
            /// </summary>
            public string WeekendMax { set; get; }

            public string Interval { set; get; }
        }


        /// <summary>
        /// 获取加班类型相关属性
        /// </summary>
        /// <param name="FormID"></param>
        /// <returns></returns>
        public static OvertimeTypeAttr GetOvertimeType(string FormID)
        {
            string overtimeTypeCode = string.Empty, weekdaysMax = "3", weekendMax = "11", interval = "1";

            //读取班次表
            DataTable _dtOvertimeType = MicroDataTable.GetDataTable("OvertimeType");
            DataRow[] _rowsOvertimeType = _dtOvertimeType.Select("FormID=" + FormID.toInt());


            //不为空值时才取值，否则保持默认
            if (_rowsOvertimeType.Length > 0)
            {
                overtimeTypeCode = _rowsOvertimeType[0]["OvertimeTypeCode"].toStringTrim();
                overtimeTypeCode = string.IsNullOrEmpty(overtimeTypeCode) ? "Normal" : overtimeTypeCode;

                weekdaysMax = _rowsOvertimeType[0]["WeekdaysMax"].toStringTrim();
                weekdaysMax = string.IsNullOrEmpty(weekdaysMax) ? "3" : weekdaysMax;

                weekendMax = _rowsOvertimeType[0]["WeekendMax"].toStringTrim();
                weekendMax = string.IsNullOrEmpty(weekendMax) ? "11" : weekendMax;

                interval = _rowsOvertimeType[0]["Interval"].toStringTrim();
                interval = string.IsNullOrEmpty(interval) ? "1" : interval;
            }


            var OvertimeTypeAttr = new OvertimeTypeAttr
            {
                OvertimeTypeCode = overtimeTypeCode,
                WeekdaysMax = weekdaysMax,
                WeekendMax = weekendMax,
                Interval = interval
            };


            return OvertimeTypeAttr;
        }


        /// <summary>
        /// 检查是否为休息日，如果为休息日则返回True
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        private static Boolean CheckIsHoliday(string Date)
        {
            Boolean flag = false;

            try
            {
                //先检查加班日期是不是休息日，如果不是再继续
                string _sql = "select * from CalendarDays where Invalid=0 and Del=0 and DayDate=@DayDate";
                SqlParameter[] _sp = {
                                    new SqlParameter("@DayDate",SqlDbType.DateTime)
                                    };

                _sp[0].Value = DateTime.Parse(Date);

                //如果返回真说明是休息日
                flag = MsSQLDbHelper.Exists(_sql, _sp);
            }
            catch { }

            return flag;
        }


        /// <summary>
        /// 获取加班开始时间。如果是休息日则获取班次勤务的正常开始时间作为加班开始时间、如果是平日则获取班次勤务的加班开始时间作为加班开始时间
        /// </summary>
        /// <param name="OvertimeDate"></param>
        /// <param name="ShiftTypeID"></param>
        /// <returns></returns>
        public static string GetOTStartTime(string OvertimeDate, string ShiftTypeID)
        {
            string flag = "17:15";

            DataTable _dtShiftType = MicroDataTable.GetDataTable("ShiftType");
            DataRow[] _rowsShiftType = _dtShiftType.Select("ShiftTypeID=" + ShiftTypeID.toInt());
            if (_rowsShiftType.Length > 0)
            {
                //获取开始时间
                string startTime = string.Empty;
                //如果是休息日则获取休息日的加班开始时间，否则获取平时的加班开始时间
                if (CheckIsHoliday(OvertimeDate))
                    startTime = _rowsShiftType[0]["StartTime"].toStringTrim();  //如果是休息日则获取班次勤务的正常开始时间
                else
                    startTime = _rowsShiftType[0]["OTStartTime"].toStringTrim();  //如果是平日则获取班次勤务的加班开始时间

                if (!string.IsNullOrEmpty(startTime))
                    flag = startTime;
            }
            return flag;
        }


        //暂时不需要总加班时间（从加班表读取），已代休（从休假表读取），剔除代休后（两者的差）
        ///// <summary>
        ///// 加班或休假记录统计表。在提交加班申请或休假申请时把记录插入该表便于统计数据汇总
        ///// </summary>
        ///// <param name="RecordType"></param>
        ///// <param name="RecordsID"></param>
        ///// <param name="Hour"></param>
        ///// <param name="RecordDate"></param>
        ///// <param name="RecordUIDs"></param>
        ///// <param name="UID"></param>
        ///// <param name="DisplayName"></param>
        ///// <returns></returns>
        //public static Boolean SetPersonalRecord(string RecordType, string RecordsID, string Hour, string RecordDate, string RecordUIDs, string UID, string DisplayName)
        //{
        //    Boolean flag = false;

        //    string[] ArrRecordUIDs = RecordUIDs.Split(',');

        //    if (ArrRecordUIDs.Length > 0)
        //    {
        //        DataTable _dtInsert = new DataTable();

        //        _dtInsert.Columns.Add("RecordType", typeof(string));  //Overtime or Holiday
        //        _dtInsert.Columns.Add("RecordsID", typeof(int));  //OvertimeID or HolidayID
        //        _dtInsert.Columns.Add("Hour", typeof(decimal)); 
        //        _dtInsert.Columns.Add("RecordDate", typeof(DateTime));
        //        _dtInsert.Columns.Add("RecordUID", typeof(int));  //加班或休假所属人员的UID
        //        _dtInsert.Columns.Add("UID", typeof(int));
        //        _dtInsert.Columns.Add("DisplayName", typeof(string));


        //        for (int i = 0; i < ArrRecordUIDs.Length; i++)
        //        {
        //            DataRow _drInsert = _dtInsert.NewRow();

        //            _drInsert["RecordType"] = RecordType.toStringTrim();
        //            _drInsert["RecordsID"] = RecordsID.toInt();
        //            _drInsert["Hour"] = Hour.toDecimal();
        //            _drInsert["RecordDate"] = RecordDate.toDateTime();
        //            _drInsert["RecordUID"] = ArrRecordUIDs[i].toInt();
        //            _drInsert["UID"] = UID.toInt();
        //            _drInsert["DisplayName"] = DisplayName.toStringTrim();

        //            _dtInsert.Rows.Add(_drInsert);
        //        }

        //    }


        //        return flag;
        //}




        /// <summary>
        /// 获取日历事件属性，如：Title、ClassName、Color
        /// </summary>
        public class CalendarEventAttr
        {
            public string Title { set; get; }
            public string ClassName { set; get; }
            public string Color { set; get; }
        }

        public static CalendarEventAttr GetCalendarEvent(string EventType)
        {
            string title = string.Empty, className = string.Empty, color = string.Empty;
            Boolean CalendarLong = MicroPublic.GetMicroInfo("CalendarLong").toBoolean();

            switch (EventType)
            {
                case "Plan":
                    title = "计划";
                    className = "ws-cal-event-plan";
                    break;
                case "Normal":
                    title = "实际";
                    className = "ws-cal-event-normal";
                    break;
                case "Overtime":
                    title = "加班";
                    className = "ws-cal-event-overtime";
                    break;
                case "BusinessTrip":
                    title = "出差";
                    className = "ws-cal-event-businesstrip";
                    break;
                case "Holiday":
                    title = "休假";
                    className = "ws-cal-event-holiday";
                    break;
                case "Abnormal":
                    title = "异常";
                    className = "ws-cal-event-abnormal";
                    break;
            }

            //if (!CalendarLong)
            //    className = className + "2"; //+2代表宽度变为50%

            var CalendarEventAttr = new CalendarEventAttr
            {
                Title = title,
                ClassName = className,
                Color = color
            };

            return CalendarEventAttr;
        }


        /// <summary>
        /// 获取日历表（CalendarEvents）个人事件 （也许除了排班、加班、休假还会有其它事件需要写在日历上，但暂时用不上20210223）
        /// </summary>
        /// <param name="StartDay"></param>
        /// <param name="EndDay"></param>
        /// <param name="UID"></param>
        /// <returns></returns>
        public static string GetCalendarEvents(string StartDay, string EndDay, int UID)
        {
            string flag = string.Empty;
            StringBuilder sb = new StringBuilder();
            string strTemp = GetStrTpl();

            Boolean CalendarLong = MicroPublic.GetMicroInfo("CalendarLong").toBoolean();

            string StartDateTime = StartDay + " 00:00:00.000",
                EndDateTime = EndDay + " 23:59:59.998";

            //向前和向后加一年的数据（如数据跨月可以出现统计不准确）
            //StartDate = DateTime.Parse(StartDate).AddYears(-1).ToString("yyyy-MM-dd HH:mm:ss:fff");
            //EndDate = DateTime.Parse(EndDate).AddYears(1).ToString("yyyy-MM-dd HH:mm:ss:fff");

            string _sql = "select * from CalendarEvents where Invalid=0 and Del=0 and UID=@UID and StartDateTime>= @StartDateTime and  EndDateTime <= @EndDateTime ";
            SqlParameter[] _sp = {
                new SqlParameter("@UID", SqlDbType.Int),
                new SqlParameter("@StartDateTime", SqlDbType.DateTime),
                new SqlParameter("@EndDateTime", SqlDbType.DateTime),
            };

            _sp[0].Value = UID;
            _sp[1].Value = StartDateTime.toDateFormat("yyyy-MM-dd HH:mm:ss.fff");
            _sp[2].Value = EndDateTime.toDateFormat("yyyy-MM-dd HH:mm:ss.fff");

            DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

            if (_dt.Rows.Count > 0)
            {
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    string Title = _dt.Rows[i]["Title"].toStringTrim();
                    string startDateTime = _dt.Rows[i]["StartDateTime"].toDateFormat("yyyy-MM-ddTHH:mm:ss"),
                     endDateTime = _dt.Rows[i]["EndDateTime"].toDateFormat("yyyy-MM-ddTHH:mm:ss"),
                     color = _dt.Rows[i]["Color"].toStringTrim(),
                     className = _dt.Rows[i]["ClassName"].toStringTrim();

                    //如果日历设置选项为长时间格式时，则在标题显示时间
                    if (CalendarLong)
                        Title = Title + "：" + startDateTime.toDateFormat("HH:mm") + "~" + endDateTime.toDateFormat("HH:mm");

                    string str1 = string.Format(strTemp, Title, startDateTime, endDateTime, color, className);
                    sb.Append("{" + str1 + "},");

                }

                flag = sb.ToString();
            }

            //构造json数据的示例
            //flag =   "{\"title\": \"实际\",\"start\": \"2020-10-28T08:20:00\",\"end\": \"2020-10-28T17:28:00\",\"color\": \"#009688\",\"className\":\"ws-cal-event-normal2\"}," +
            //        "{\"title\": \"实际：08:15~17:45\",\"start\": \"2020-10-29T08:29:00\",\"end\": \"2020-10-29T17:45:00\",\"color\": \"#009688\",\"className\":\"ws-cal-event-normal\"}," +
            //        "{\"title\": \"加班\",\"start\": \"2020-11-02T17:30:00\",\"end\": \"2020-11-02T19:30:00\",\"color\": \"#009688\",\"className\":\"ws-cal-event-overtime2\"}," +
            //        "{\"title\": \"加班：17:30~19:30\",\"start\": \"2020-11-03T17:30:00\",\"end\": \"2020-11-03T19:30:00\",\"color\": \"#009688\",\"className\":\"ws-cal-event-overtime\"},"+
            //        "{\"title\": \"迟到\",\"start\": \"2020-11-05T17:30:00\",\"end\": \"2020-11-05T19:30:00\",\"color\": \"#da0000\",\"className\":\"ws-cal-event-abnormal2\"},"+
            //        "{\"title\": \"早退：08:16~17:00\",\"start\": \"2020-11-06T17:30:00\",\"end\": \"2020-11-06T19:30:00\",\"color\": \"#da0000\",\"className\":\"ws-cal-event-abnormal\"},"+
            //    "{\"title\": \"出差\",\"start\": \"2020-11-20T09:30:00\",\"end\": \"2020-11-27T17:30:00\",\"color\": \"#c71585\",\"className\":\"ws-cal-event-businesstrip\"}," +
            //    "{\"title\": \"休假\",\"start\": \"2020-11-11T00:00:00\",\"end\": \"2020-11-12T23:59:00\",\"color\": \"#c71585\",\"className\":\"ws-cal-event-holiday\"},";

            return flag;

        }


        /// <summary>
        /// 获取个人排班事件
        /// </summary>
        /// <param name="StartDay"></param>
        /// <param name="EndDay"></param>
        /// <returns></returns>
        public static string GetPersonalOnDutyCalendarEvents(string StartDay, string EndDay, int UID)
        {
            string flag = string.Empty;

            StringBuilder sb = new StringBuilder();
            string strTemp = GetStrTpl();

            DateTime StartDate = DateTime.Parse(StartDay.toDateFormat());
            DateTime EndDate = DateTime.Parse(EndDay.toDateFormat());

            //获取计划事件的ClassName和Color
            var getCalendarEvent = GetCalendarEvent("Plan");
            string className = getCalendarEvent.ClassName,
             color = getCalendarEvent.Color;

            //Boolean CalendarLong = MicroPublic.GetMicroInfo("CalendarLong").toBoolean();

            //**********获取日历班次数据Start**********
            //获取用户默认班次，无法匹配排班时用
            var getUserDefaultShiftType = GetUserDefaultShiftType(UID.ToString());


            //*****查询(个人)已提交申请的排班事件Start*****
            string _sql2 = "select * from HROnDutyForm where Invalid=0 and Del=0 and ParentID<>0 and StateCode=100 and DutyUID=@DutyUID and DutyDate between @StartDay and @EndDay ";
            SqlParameter[] _sp2 = {
                new SqlParameter("@DutyUID", SqlDbType.Int),
                new SqlParameter("@StartDay", SqlDbType.VarChar, 50),
            new SqlParameter("@EndDay", SqlDbType.VarChar, 50),
            };

            _sp2[0].Value = UID;
            _sp2[1].Value = StartDay.toDateFormat();
            _sp2[2].Value = EndDay.toDateFormat();

            DataTable _dt2 = MsSQLDbHelper.Query(_sql2, _sp2).Tables[0];
            //*****查询(个人)已提交申请的排班事件End*****

            //*****（全局）得到节假日 日期Start*****
            string _sql4 = "select * from CalendarDays where Invalid=0 and Del=0 and DayDate between @StartDay and @EndDay ";
            SqlParameter[] _sp4 = {
                new SqlParameter("@StartDay", SqlDbType.VarChar, 50),
                new SqlParameter("@EndDay", SqlDbType.VarChar, 50),
            };

            _sp4[0].Value = StartDay.toDateFormat();
            _sp4[1].Value = EndDay.toDateFormat();

            DataTable _dt4 = MsSQLDbHelper.Query(_sql4, _sp4).Tables[0];
            //*****（全局）得到假期日期End*****


            //******读取用户休假记录Start******
            //获取当前时间段所有休假者，在生成审批者时显示休假中（在这里统一读取记录，避免多次循环读取数据库）
            string _sqlUsersLeave = "select LeaveUID,StartDate,EndDate from HRLeave where Invalid=0 and Del=0 and StateCode=100 and LeaveUID=@LeaveUID and StartDate<=@EndDate and EndDate>=@StartDate";
            SqlParameter[] _spUsersLeave = {
                    new SqlParameter("@LeaveUID",SqlDbType.Int),
                    new SqlParameter("@StartDate",SqlDbType.DateTime),
                    new SqlParameter("@EndDate",SqlDbType.DateTime),
                    };
            _spUsersLeave[0].Value = UID;
            _spUsersLeave[1].Value = StartDate;
            _spUsersLeave[2].Value = EndDate;

            DataTable _dtUsersLeave = MsSQLDbHelper.Query(_sqlUsersLeave, _spUsersLeave).Tables[0];
            //******读取用户休假记录End******


            //计算两个日期相差的天数
            int day = EndDate.Subtract(StartDate).Days;

            //根据日历传递过来的开始日期和结束日期计算出天数
            //开始遍历天数
            for (int i = 0; i < day; i++)
            {
                //遍历日期得到当天日期
                string CurrDate = StartDate.AddDays(i).toDateFormat();

                //获取当天个人排班事件
                DataRow[] _rows2 = _dt2.Select("DutyDate= '" + CurrDate + "'", "DateCreated desc");
                //判断是否有个人排班事件，如果有
                if (_rows2.Length > 0)
                {
                    int ShiftTypeID = _rows2[0]["ShiftTypeID"].toInt();

                    string ShiftName = string.Empty, Title = string.Empty, StartTime = string.Empty, EndTime = string.Empty, StartDateTime = string.Empty, EndDateTime = string.Empty;
                    ShiftName = _rows2[0]["ShiftName"].toStringTrim();

                    if (ShiftName != "休" && ShiftName != "-")
                    {
                        Title = ShiftName;
                        StartTime = _rows2[0]["StartDateTime"].toStringTrim();
                        EndTime = _rows2[0]["EndDateTime"].toStringTrim();

                        StartDateTime = CurrDate.toDateFormat("yyyy-MM-dd") + " " + StartTime.toDateFormat("HH:mm");
                        EndDateTime = CurrDate.toDateFormat("yyyy-MM-dd") + " " + EndTime.toDateFormat("HH:mm");

                        //如果结束时间<开始时间，则结束时间设定最大为23:59:59 (仅用于把事件写入日历时间轴上，避免垮天出现日历事件样式block模式)
                        if (EndDateTime.toDateTime("HH:mm") < StartDateTime.toDateTime("HH:mm"))
                            EndDateTime = EndDateTime.toDateFormat("yyyy-MM-dd") + " 23:59:59";

                        //构造得到个人排班事件
                        string str1 = string.Format(strTemp, Title, StartDateTime.toDateFormat("yyyy-MM-ddTHH:mm:ss"), EndDateTime.toDateFormat("yyyy-MM-ddTHH:mm:ss"), color, className);
                        sb.Append("{" + str1 + "},");

                    }
                    //取消班次（即提交了申请后又取消了），则从默认班次获取数据
                    else if (ShiftName == "-")
                    {

                        Title = getUserDefaultShiftType.Alias;
                        StartTime = getUserDefaultShiftType.StartTime;
                        EndTime = getUserDefaultShiftType.EndTime;

                        StartDateTime = CurrDate.toDateFormat("yyyy-MM-dd") + " " + StartTime;
                        EndDateTime = CurrDate.toDateFormat("yyyy-MM-dd") + " " + EndTime;

                        string str1 = string.Format(strTemp, Title, StartDateTime.toDateFormat("yyyy-MM-ddTHH:mm:ss"), EndDateTime.toDateFormat("yyyy-MM-ddTHH:mm:ss"), color, className);
                        sb.Append("{" + str1 + "},");

                    }
                    //如果为休则不显示，需要通过休假申请记录来显示

                }
                else //如果当天没有排班事件、也不是休息日、也没有休假，则显示默认班次
                {
                    //读取当天日历日子数据，判断是否为休息日
                    DataRow[] _rows4 = _dt4.Select("DayDate= '" + CurrDate + "'");

                    //判断今天是否为休息日，如果不是休日时
                    if (_rows4.Length == 0)
                    {
                        //判断是否有休假
                        DataRow[] _drUsersLeave = _dtUsersLeave.Select("'" + CurrDate + "'>=StartDate and '" + CurrDate + "'<=EndDate");

                        //如果没有休假，显示默认班次
                        if (_drUsersLeave.Length == 0)
                        {
                            string Title = string.Empty, StartTime = string.Empty, EndTime = string.Empty, StartDateTime = string.Empty, EndDateTime = string.Empty;

                            Title = getUserDefaultShiftType.Alias;
                            StartTime = getUserDefaultShiftType.StartTime;
                            EndTime = getUserDefaultShiftType.EndTime;

                            StartDateTime = CurrDate.toDateFormat("yyyy-MM-dd") + " " + StartTime;
                            EndDateTime = CurrDate.toDateFormat("yyyy-MM-dd") + " " + EndTime;

                            //如果日历设置选项为长时间格式时，则在标题显示时间
                            //if (CalendarLong)
                            //    Title = Title + "：" + StartTime.toDateFormat("HH:mm") + "~" + EndTime.toDateFormat("HH:mm");

                            string str1 = string.Format(strTemp, Title, StartDateTime.toDateFormat("yyyy-MM-ddTHH:mm:ss"), EndDateTime.toDateFormat("yyyy-MM-ddTHH:mm:ss"), color, className);
                            sb.Append("{" + str1 + "},");

                        }

                    }
                }
            }
            //**********获取日历班次数据End**********

            flag = sb.ToString();
            return flag;
        }

        /// <summary>
        /// 获取加班事件
        /// </summary>
        /// <param name="StartDay"></param>
        /// <param name="EndDay"></param>
        /// <param name="UID"></param>
        /// <returns></returns>
        public static string GetOvertimeCalendarEvents(string StartDay, string EndDay, int UID)
        {
            string flag = string.Empty;
            StringBuilder sb = new StringBuilder();
            string strTemp = GetStrTpl();

            Boolean CalendarLong = MicroPublic.GetMicroInfo("CalendarLong").toBoolean();

            string StartDateTime = StartDay + " 00:00:00.000",
                EndDateTime = EndDay + " 23:59:59.998";

            //向前和向后加一个月的数据（如数据跨月可以出现统计不准确）
            StartDateTime = DateTime.Parse(StartDateTime).AddMonths(-1).ToString("yyyy-MM-dd HH:mm:ss.fff");
            EndDateTime = DateTime.Parse(EndDateTime).AddMonths(1).ToString("yyyy-MM-dd HH:mm:ss.fff");

            string _sql = "select * from HROvertime where Invalid=0 and Del=0 and StateCode=100 and ParentID<>0 and OvertimeUID=@OvertimeUID and OvertimeDate between @StartDateTime and @EndDateTime";
            SqlParameter[] _sp = {
                new SqlParameter("@OvertimeUID", SqlDbType.Int),
                new SqlParameter("@StartDateTime", SqlDbType.DateTime),
            new SqlParameter("@EndDateTime", SqlDbType.DateTime),
            };

            _sp[0].Value = UID;
            _sp[1].Value = StartDateTime.toDateTime("yyyy-MM-dd HH:mm:ss.fff");
            _sp[2].Value = EndDateTime.toDateTime("yyyy-MM-dd HH:mm:ss.fff");

            DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

            if (_dt.Rows.Count > 0)
            {
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    var getCalendarEvent = GetCalendarEvent("Overtime");

                    string Title = getCalendarEvent.Title, //"加班"; //_dt.Rows[i]["Title"].toStringTrim();
                     className = getCalendarEvent.ClassName,
                     color = getCalendarEvent.Color;

                    string OvertimeDate = _dt.Rows[i]["OvertimeDate"].toDateFormat("yyyy-MM-dd"),
                        OvertimeTime = _dt.Rows[i]["OvertimeTime"].toStringTrim(),
                        startTime = _dt.Rows[i]["StartTime"].toDateFormat("HH:mm"),
                        endTime = _dt.Rows[i]["EndTime"].toDateFormat("HH:mm"),
                        OvertimeHour = _dt.Rows[i]["OvertimeHour"].toStringTrim();

                    string startDateTime = OvertimeDate + " " + startTime,
                    endDateTime = OvertimeDate + " " + endTime;

                    //结束时间小于 等于开始时间，则结束日期的天数上自动加1天
                    if (endDateTime.toDateTime("HH:mm") <= startDateTime.toDateTime("HH:mm"))
                        endDateTime = endDateTime.toDateTime("yyyy-MM-dd HH:mm").AddDays(1).toDateFormat("yyyy-MM-dd HH:mm");

                    //如果日历设置选项为长时间格式时，则在标题显示时间
                    if (CalendarLong)
                        Title = Title + " " + OvertimeHour + "H | " + startTime.toDateFormat("HH:mm") + "~" + endTime.toDateFormat("HH:mm");
                    else
                        Title = Title + " " + OvertimeHour + "H";

                    startDateTime = startDateTime.toDateFormat("yyyy-MM-ddTHH:mm:ss");
                    endDateTime = endDateTime.toDateFormat("yyyy-MM-ddTHH:mm:ss");

                    string Str = string.Format(strTemp, Title, startDateTime, endDateTime, color, className);
                    sb.Append("{" + Str + "},");

                }

                flag = sb.ToString();
            }

            return flag;
        }

        /// <summary>
        /// 获取个人休假事件
        /// </summary>
        /// <param name="StartDay"></param>
        /// <param name="EndDay"></param>
        /// <param name="UID"></param>
        /// <returns></returns>
        public static string GetLeaevCalendarEvents(string StartDay, string EndDay, int UID)
        {
            string flag = string.Empty;
            StringBuilder sb = new StringBuilder();
            string strTemp = GetStrTpl();

            Boolean CalendarLong = MicroPublic.GetMicroInfo("CalendarLong").toBoolean();

            string StartDateTime = StartDay + " 00:00:00.000",
                EndDateTime = EndDay + " 23:59:59.998";

            //向前和向后加一个月的数据（如数据跨月可以出现统计不准确）
            StartDateTime = DateTime.Parse(StartDateTime).AddMonths(-1).ToString("yyyy-MM-dd HH:mm:ss.fff");
            EndDateTime = DateTime.Parse(EndDateTime).AddMonths(1).ToString("yyyy-MM-dd HH:mm:ss.fff");

            string _sql = "select a.*,b.HolidayName from HRLeave a left join HRHolidayType b on a.HolidayTypeID=b.HolidayTypeID where a.Invalid=0 and a.Del=0 and a.StateCode=100 and LeaveUID=@LeaveUID and StartDateTime<=@EndDateTime and EndDateTime>=@StartDateTime";
            SqlParameter[] _sp = {
                new SqlParameter("@LeaveUID", SqlDbType.Int),
                new SqlParameter("@StartDateTime", SqlDbType.DateTime),
            new SqlParameter("@EndDateTime", SqlDbType.DateTime),
            };

            _sp[0].Value = UID;
            _sp[1].Value = StartDateTime.toDateTime("yyyy-MM-dd HH:mm:ss.fff");
            _sp[2].Value = EndDateTime.toDateTime("yyyy-MM-dd HH:mm:ss.fff");

            DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

            if (_dt.Rows.Count > 0)
            {
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    var getCalendarEvent = GetCalendarEvent("Holiday");

                    string Title = getCalendarEvent.Title,
                     className = getCalendarEvent.ClassName,
                     color = getCalendarEvent.Color;

                    string startDateTime = _dt.Rows[i]["StartDateTime"].toStringTrim(),
                    endDateTime = _dt.Rows[i]["EndDateTime"].toStringTrim(),
                    leaveDays = _dt.Rows[i]["LeaveDays"].toStringTrim(),
                    leaveHour = _dt.Rows[i]["LeaveHour"].toStringTrim(),
                    holidayName = _dt.Rows[i]["HolidayName"].toStringTrim();

                    leaveDays = leaveDays.toDecimal() > 0 ? leaveDays + "D" : "";

                    if (leaveDays.toDecimal() > 0)
                        leaveHour = leaveHour.toDecimal() > 0 ? "，" + leaveHour + "H" : "";
                    else
                        leaveHour = leaveHour.toDecimal() > 0 ? leaveHour + "H" : ""; //休假天数小于0则不显示逗号

                    //如果日历设置选项为长时间格式时，则在标题显示时间
                    if (CalendarLong)
                        Title = holidayName + leaveDays + leaveHour + " | " + startDateTime.toDateFormat("HH:mm") + "~" + endDateTime.toDateFormat("HH:mm");
                    else
                        Title = holidayName + leaveDays + leaveHour;


                    startDateTime = startDateTime.toDateFormat("yyyy-MM-ddTHH:mm:ss");
                    endDateTime = endDateTime.toDateFormat("yyyy-MM-ddTHH:mm:ss");

                    string Str = string.Format(strTemp, Title, startDateTime, endDateTime, color, className);
                    sb.Append("{" + Str + "},");

                }

                flag = sb.ToString();
            }

            return flag;
        }

        /// <summary>
        /// 获取个人勤怠数据，构成JSON格式字符串（主要首页展示用）
        /// </summary>
        /// <param name="AnyDate"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public static string GetPersonalAttendanceData(string AnyDate, int UID, string DataStateCode = "Finish")
        {
            string flag = string.Empty,
                strTemp = "\"MonthStartDateTime\":\"{0}\",\"MonthEndDateTime\":\"{1}\",\"QuarterStartDateTime\":\"{2}\",\"QuarterEndDateTime\": \"{3}\", \"OvertimeTotal\": \"{4}\", \"OvertimeTotalByMonth\": \"{5}\",\"OvertimeTotalByQuarter\":\"{6}\",\"WorkOvertime\":\"{7}\",\"WorkOvertimeByQuarter\":\"{8}\",\"OffDayOvertime\":\"{9}\",\"OffDayOvertimeByQuarter\":\"{10}\",\"Statutory\":\"{11}\",\"StatutoryByQuarter\":\"{12}\",\"LeaveTotalByMonth\":\"{13}\",\"LeaveTotalByQuarter\":\"{14}\",\"AlreadyDaiXiuByMonth\":\"{15}\",\"AlreadyDaiXiuByQuarter\":\"{16}\",\"AlreadyDaiXiuOvertimeByMonth\":\"{17}\",\"AlreadyDaiXiuOvertimeByQuarter\":\"{18}\",\"ExceptDaiXiuByMonth\":\"{19}\",\"ExceptDaiXiuByQuarter\":\"{20}\",\"RemainingDaiXiu\":\"{21}\",\"RemainingDaiXiuByQuarter\":\"{22}\",\"RemainingAnnualLeave\":\"{23}\",\"RemainingOtherLeave\":\"{24}\",\"SelfDriving\":\"{25}\",\"WorkHourSysID\":\"{26}\",\"WarningValue\":\"{27}\"";
            string StartDateTime = AnyDate.toDateMFirstDay() + " 00:00:00.000",
                EndDateTime = AnyDate.toDateMLastDay() + " 23:59:59.998";

            var getUserWorkHourSystem = GetUserWorkHourSystem(UID, StartDateTime, EndDateTime);
            int WorkHourSysID = (getUserWorkHourSystem.WorkHourSysID).toInt();
            string WarningValue = getUserWorkHourSystem.WarningValue;

            var getPersonalRecord = GetPersonalRecord(AnyDate, UID, WorkHourSysID, 0, DataStateCode);

            string monthStartDateTime = getPersonalRecord.MonthStartDateTime,
                monthEndDateTime = getPersonalRecord.MonthEndDateTime,
                quarterStartDateTime = getPersonalRecord.QuarterStartDateTime,
                quarterEndDateTime = getPersonalRecord.QuarterEndDateTime,
                overtimeTotal = getPersonalRecord.OvertimeTotal,
                overtimeTotalByMonth = getPersonalRecord.OvertimeTotalByMonth,
                overtimeTotalByQuarter = getPersonalRecord.OvertimeTotalByQuarter,
                workOvertime = getPersonalRecord.WorkOvertime,
                workOvertimeByQuarter = getPersonalRecord.WorkOvertimeByQuarter,
                offDayOvertime = getPersonalRecord.OffDayOvertime,
                offDayOvertimeByQuarter = getPersonalRecord.OffDayOvertimeByQuarter,
                statutory = getPersonalRecord.Statutory,
                statutoryByQuarter = getPersonalRecord.StatutoryByQuarter,
                leaveTotalByMonth = getPersonalRecord.LeaveTotalByMonth,
                leaveTotalByQuarter = getPersonalRecord.LeaveTotalByQuarter,
                alreadyDaiXiuByMonth = getPersonalRecord.AlreadyDaiXiuByMonth,
                alreadyDaiXiuByQuarter = getPersonalRecord.AlreadyDaiXiuByQuarter,
                alreadyDaiXiuOvertimeByMonth = getPersonalRecord.AlreadyDaiXiuOvertimeByMonth,
                alreadyDaiXiuOvertimeByQuarter = getPersonalRecord.AlreadyDaiXiuOvertimeByQuarter,
                exceptDaiXiuByMonth = getPersonalRecord.ExceptDaiXiuByMonth,
                exceptDaiXiuByQuarter = getPersonalRecord.ExceptDaiXiuByQuarter,
                remainingDaiXiu = getPersonalRecord.RemainingDaiXiu,
                remainingDaiXiuByQuarter = getPersonalRecord.RemainingDaiXiuByQuarter,
                remainingAnnualLeave = getPersonalRecord.RemainingAnnualLeave,
                remainingOtherLeave = getPersonalRecord.RemainingOtherLeave,
                selfDriving = getPersonalRecord.SelfDriving;

            flag = string.Format(strTemp, monthStartDateTime, monthEndDateTime, quarterStartDateTime, quarterEndDateTime, overtimeTotal, overtimeTotalByMonth, overtimeTotalByQuarter, workOvertime, workOvertimeByQuarter, offDayOvertime, offDayOvertimeByQuarter, statutory, statutoryByQuarter, leaveTotalByMonth, leaveTotalByQuarter, alreadyDaiXiuByMonth, alreadyDaiXiuByQuarter, alreadyDaiXiuOvertimeByMonth, alreadyDaiXiuOvertimeByQuarter, exceptDaiXiuByMonth, exceptDaiXiuByQuarter, remainingDaiXiu, remainingDaiXiuByQuarter, remainingAnnualLeave, remainingOtherLeave, selfDriving, WorkHourSysID, WarningValue);

            flag = "[{" + flag + "}]";
            flag = JToken.Parse(flag).ToString();
            return flag;

        }

        /// <summary>
        /// \"title\": \"{0}\", \"start\": \"{1}\", \"end\": \"{2}\", \"color\": \"{3}\",\"className\":\"{4}\"
        /// </summary>
        /// <returns></returns>
        private static string GetStrTpl()
        {
            string strTpl = "\"title\": \"{0}\", \"start\": \"{1}\", \"end\": \"{2}\", \"color\": \"{3}\",\"className\":\"{4}\"";
            return strTpl;
        }


        /// <summary>
        /// 批量更新用户加班小时汇总（可能发生情况：申请者可能写了很多申请记录，但没有提交在最后提交走审批流程时重新计算一下加班小时汇总）
        /// </summary>
        /// <param name="ParentID"></param>
        public static void SetBulkUpdatePersonalRecord(string ParentID)
        {
            string _sql = "select * from HROvertime where ParentID=@ParentID";
            SqlParameter[] _sp = { new SqlParameter("@ParentID", SqlDbType.Int) };

            _sp[0].Value = ParentID.toInt();

            DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

            if (_dt != null && _dt.Rows.Count > 0)
            {
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    int OvertimeID = _dt.Rows[i]["OvertimeID"].toInt(),
                     OvertimeUID = _dt.Rows[i]["OvertimeUID"].toInt();

                    string OvertimeDate = _dt.Rows[i]["OvertimeDate"].toStringTrim();

                    var getUserWorkHourSystem = MicroHR.GetUserWorkHourSystem(OvertimeUID, OvertimeDate);
                    string WorkHourSysID = getUserWorkHourSystem.WorkHourSysID;

                    var getPersonalRecord = MicroHR.GetPersonalRecord(OvertimeDate, OvertimeUID, WorkHourSysID.toInt(), 0, "ApprovalOrMore");
                    string OvertimeTotal = getPersonalRecord.OvertimeTotal;
                    string AlreadyDaiXiu = getPersonalRecord.AlreadyDaiXiuByMonth;
                    string ExceptDaiXiu = getPersonalRecord.ExceptDaiXiuByMonth;

                    string _sql2 = "update HROvertime set OvertimeTotal=@OvertimeTotal, AlreadyDaiXiu=@AlreadyDaiXiu, ExceptDaiXiu=@ExceptDaiXiu where OvertimeID=@OvertimeID ";
                    SqlParameter[] _sp2 = { new SqlParameter("@OvertimeTotal",SqlDbType.Decimal),
                        new SqlParameter("@AlreadyDaiXiu",SqlDbType.Decimal),
                        new SqlParameter("@ExceptDaiXiu",SqlDbType.Decimal),
                        new SqlParameter("@OvertimeID",SqlDbType.Int),
                    };

                    _sp2[0].Value = OvertimeTotal.toDecimal();
                    _sp2[1].Value = AlreadyDaiXiu.toDecimal();
                    _sp2[2].Value = ExceptDaiXiu.toDecimal();
                    _sp2[3].Value = OvertimeID;

                    MsSQLDbHelper.ExecuteSql(_sql2, _sp2);
                }
            }

        }


        /// <summary>
        /// 获取个人可用假期
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="HolidayTypeID"></param>
        /// <param name="OvertimeDate"></param>
        /// <returns></returns>
        public static string GetPersonalHoliday(string UID, string HolidayTypeID, string OvertimeDate = "", string DataStateCode = "Finish")
        {
            string flag = "0";

            //获取代休假以外的假期（如年休假、事假、婚假等等）
            if (HolidayTypeID.toInt() != 2)
            {
                string _sqlHoliday = "select * from HRPersonalHoliday where Invalid=0 and Del=0 and UID=@UID and HolidayTypeID = @HolidayTypeID";
                SqlParameter[] _spHoliday = { new SqlParameter("@UID",SqlDbType.Int),
                                            new SqlParameter("@HolidayTypeID",SqlDbType.Int)
                                        };
                _spHoliday[0].Value = UID.toInt();
                _spHoliday[1].Value = HolidayTypeID.toInt();

                DataTable _dtHoliday = MsSQLDbHelper.Query(_sqlHoliday, _spHoliday).Tables[0];

                if (_dtHoliday != null && _dtHoliday.Rows.Count > 0)
                {
                    flag = _dtHoliday.Compute("sum(Days)", "").ToString();  //返回的是天数
                }

            }
            else  //否则等于2（即代表代休假），获取代休假(获取休日加班小时数)
            {
                if (!string.IsNullOrEmpty(OvertimeDate))
                {
                    var getUserWorkHourSystem = GetUserWorkHourSystem(UID.toInt(), OvertimeDate);
                    int WorkHourSysID = (getUserWorkHourSystem.WorkHourSysID).toInt();

                    //参数ApprovalOrMore=等待审批或以上
                    var getPersonalRecord = GetPersonalRecord(OvertimeDate, UID.toInt(), WorkHourSysID, 0, DataStateCode);
                    flag = getPersonalRecord.RemainingDaiXiu;  //返回的是休日加班小时数

                    if (WorkHourSysID == 2)
                        flag = getPersonalRecord.RemainingDaiXiuByQuarter;

                }
            }

            return flag;

        }


        public static string GetLeaveMax(string UID, string HolidayTypeID, string OvertimeDate = "", string DataStateCode = "Finish")
        {
            string MaxValue = GetPersonalHoliday(UID, HolidayTypeID, OvertimeDate, DataStateCode),
                    MaxDays = MaxValue,
                    MinDaysUnit = "1",
                    MaxHour = "0",
                    MinHourUnit = "1",
                    HolidayOptionsByDays = string.Empty,
                    HolidayOptionsByHour = string.Empty,
                    Description = string.Empty;

            //假期为代休类型时返回的是休日的加班小时数，需要把小时转为天数所以把值除以8
            if (HolidayTypeID.toInt() == 2)
            {
                MaxDays = (MaxValue.toDecimal() / 8).toDecimalInt32().toStringTrim();
                //MaxHour = MaxValue;  //当为代休假期时MaxHour不需要赋值，因为代休假只能休天数为准
            }

            string Invalid = " and Invalid=0 ";
            if (MicroUserInfo.CheckUserRole("Administrators"))
                Invalid = " ";

            //但无论如何都需要取得休假单位，所以不能放在上面的else上
            string _sql = "select * from HRHolidayType where Del=0 " + Invalid + " and HolidayTypeID=@HolidayTypeID";
            SqlParameter[] _sp = { new SqlParameter("@HolidayTypeID", SqlDbType.Int) };
            _sp[0].Value = HolidayTypeID.toInt();

            DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

            if (_dt != null && _dt.Rows.Count > 0)
            {
                MinDaysUnit = _dt.Rows[0]["MinDaysUnit"].toStringTrim();
                MinHourUnit = _dt.Rows[0]["MinHourUnit"].toStringTrim();


                ///如果休假类型不为年休假或代休假时，同时返回的MaxDays=0 即代表没有记录，则可默认从HRHolidayType表中取出默认值（如事假，病假等，这个是默认设定允许休假上限的数据）
                if (HolidayTypeID.toInt() > 2 && MaxDays.toInt() == 0)
                {
                    MaxDays = _dt.Rows[0]["MaxDays"].toStringTrim();
                    MaxHour = _dt.Rows[0]["MaxHour"].toStringTrim();
                }

                MaxDays = string.IsNullOrEmpty(MaxDays) ? "0" : MaxDays;
                MaxHour = string.IsNullOrEmpty(MaxHour) ? "0" : MaxHour;

                //HolidayOptions由Name:Value,Name2:Value2组成，该值不为空时从这里取值
                HolidayOptionsByDays = _dt.Rows[0]["HolidayOptionsByDays"].toStringTrim();
                HolidayOptionsByHour = _dt.Rows[0]["HolidayOptionsByHour"].toStringTrim();

                Description = _dt.Rows[0]["Description"].toStringTrim();
            }

            if (!string.IsNullOrEmpty(HolidayOptionsByDays))
            {
                string[] arrTextValue = HolidayOptionsByDays.Split(',');
                string[] arrTxtVal = arrTextValue[arrTextValue.Length - 1].Split(':');
                MaxDays = arrTxtVal[1];

            }

            if (!string.IsNullOrEmpty(HolidayOptionsByHour))
            {
                string[] arrTextValue = HolidayOptionsByHour.Split(',');
                string[] arrTxtVal = arrTextValue[arrTextValue.Length - 1].Split(':');
                MaxHour = arrTxtVal[0];

            }

            string flag = "{\"Days\": " + MaxDays + ",\"Hour\":  " + (HolidayTypeID.toInt() == 2 ? MaxValue : MaxHour) + ",\"DaysArr\": " + GetJsonString(MaxDays, MinDaysUnit, HolidayOptionsByDays) + ",\"HourArr\":  " + GetJsonString(MaxHour, MinHourUnit, HolidayOptionsByHour) + ",\"Description\": \"" + Description + "\"}";

            return flag;

        }



        /// <summary>
        /// 在新增时检查用户加班最大值，如果超出允许的最大值则返回提示消息，否则返回true
        /// </summary>
        /// <param name="Action"></param>
        /// <param name="ShortTableName"></param>
        /// <param name="ModuleID"></param>
        /// <param name="FormID"></param>
        /// <param name="IsApprovalForm"></param>
        /// <param name="FormFields"></param>
        /// <param name="OvertimeTypeCode"></param>
        /// <returns></returns>
        public static string CheckUserOvertimeMax(string Action, string ShortTableName, string ModuleID, string FormID, Boolean IsApprovalForm, string FormFields, string OvertimeTypeCode)
        {
            string flag = "保存失败，读取个人加班时长失败",
                UID = MicroUserInfo.GetUserInfo("UID");

            string OvertimeDate = string.Empty, OvertimeUIDs = string.Empty, ShiftTypeID = string.Empty, StartTime = string.Empty, EndTime = string.Empty, OTHour = string.Empty, OTMin = string.Empty, OvertimeMealID = string.Empty, OvertimeHour = string.Empty;

            dynamic json = JToken.Parse(FormFields) as dynamic;

            if (!string.IsNullOrEmpty(OvertimeTypeCode))
                OvertimeTypeCode = OvertimeTypeCode.ToLower();

            OvertimeDate = json["txtOvertimeDate"];
            OvertimeDate = OvertimeDate.toStringTrim();

            OvertimeUIDs = json["selOvertimeUID"];  //1,2,3  可能存在多位加班的人员，这里插入的是子记录录所以在for循环读取

            ShiftTypeID = json["raShiftTypeID"];
            ShiftTypeID = ShiftTypeID.toStringTrim();

            StartTime = json["txtStartTime"];
            StartTime = StartTime.toStringTrim();

            OTHour = json["selOTHour"];
            OTHour = OTHour.toStringTrim();

            OTMin = json["selOTMin"];
            OTMin = OTMin.toStringTrim();

            OvertimeMealID = json["cbOvertimeMealID"];
            OvertimeMealID = OvertimeMealID.toStringTrim();

            //根据班次、加班日期、开始时间、持续小时、分钟、就餐ID计算出：OvertimeTime、EndTime、OvertimeHour
            var getOTHour = GetOTHour(ShiftTypeID, OvertimeDate, StartTime, OTHour, OTMin, OvertimeMealID);
            OvertimeHour = getOTHour.OvertimeHour;
            EndTime = getOTHour.EndTime;

            if (!string.IsNullOrEmpty(OvertimeDate) && !string.IsNullOrEmpty(OvertimeUIDs) && OvertimeHour.toDecimal() > 0)
            {
                //根据FormID得到加班类型（平日加班、特殊班）的最大允许小时数。
                var getOvertimeType = GetOvertimeType(FormID);
                string WeekdaysMax = getOvertimeType.WeekdaysMax,
                    WeekendMax = getOvertimeType.WeekendMax;

                //避免多次读取数据库，在这里打开UserInfo表
                DataTable _dtUsers = MicroDataTable.GetDataTable("Use");

                string Tips = string.Empty, Tips2 = string.Empty, Tips3 = string.Empty, Tips4 = string.Empty;

                //读取记录用于检查时间是否存在冲突
                string quarterStartDateTime = OvertimeDate.toDateQFirstDay() + " 00:00:00.000",
                quarterEndDateTime = OvertimeDate.toDateQLastDay() + " 23:59:59.998";

                string _sql = "select * from HROvertime where Invalid=0 and Del=0 and ParentID<>0 and OvertimeDate between @StartDate and @EndDate and StateCode >=-4 ";
                SqlParameter[] _sp = {
                    new SqlParameter("@StartDate",SqlDbType.VarChar),
                     new SqlParameter("@EndDate",SqlDbType.VarChar)
                };

                _sp[0].Value = quarterStartDateTime;
                _sp[1].Value = quarterEndDateTime;

                DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

                string[] ArrOvertimeUID = OvertimeUIDs.Split(',');
                for (int i = 0; i < ArrOvertimeUID.Length; i++)
                {
                    //获取用户工时制
                    int OvertimeUID = ArrOvertimeUID[i].toInt();

                    var getUserWorkHourSystem = GetUserWorkHourSystem(OvertimeUID, OvertimeDate);
                    string WorkHourSysID = getUserWorkHourSystem.WorkHourSysID,
                           MaxValue = getUserWorkHourSystem.MaxValue;

                    //获取用户DisplayName
                    string OvertimeDisplayName = string.Empty;
                    DataRow[] _rowsUsers = _dtUsers.Select("UID=" + ArrOvertimeUID[i].toInt());
                    if (_rowsUsers.Length > 0)
                    {
                        string UserName = _rowsUsers[0]["UserName"].toStringTrim(),
                        ChineseName = _rowsUsers[0]["ChineseName"].toStringTrim(),
                        EnglishName = _rowsUsers[0]["EnglishName"].toStringTrim(),
                        AdDisplayName = _rowsUsers[0]["AdDisplayName"].toStringTrim();
                        OvertimeDisplayName = MicroUserInfo.GetUserInfo(UserName, ChineseName, EnglishName, AdDisplayName);
                    }

                    //检查同一时间段内是否有其它记录存在
                    //检查开始时间是否包含在已申请的记录中
                    string StartDateTime = (OvertimeDate + " " + StartTime).toDateTime("yyyy-MM-dd HH:mm:ss").AddSeconds(1).toDateFormat("yyyy-MM-dd HH:mm:ss"), //开始时间加1秒
                           EndDateTime = (OvertimeDate + " " + EndTime).toDateTime("yyyy-MM-dd HH:mm:ss").AddSeconds(-1).toDateFormat("yyyy-MM-dd HH:mm:ss"); //开始时间加1秒

                    //如果开始时间大于结束时间（说明是跨天），结束时间天数+1
                    if (StartTime.toDateTime("yyyy-MM-dd HH:mm:ss") > EndTime.toDateTime("yyyy-MM-dd HH:mm:ss"))
                        EndDateTime = EndDateTime.toDateTime("yyyy-MM-dd HH:mm:ss").AddDays(1).toDateFormat("yyyy-MM-dd HH:mm:ss");

                    //*****检查时间是否存在冲突Start*****
                    //注意这里的比较时间是写反的(开始对结束，结束对开始)StartDateTime<=@EndDateTime and EndDateTime>=@StartDateTime
                    DataRow[] _rows6 = _dt.Select("OvertimeUID=" + OvertimeUID + " and OvertimeDate='" + OvertimeDate + "' and StartDateTime<='" + EndDateTime + "' and EndDateTime>='" + StartDateTime + "'");
                    if (_rows6.Length > 0)
                    {
                        string FormNumber = _rows6[0]["FormNumber"].toStringTrim();
                        Tips4 += OvertimeDisplayName + " 的申请时间上存在冲突，<br/>相关表单编号： " + FormNumber + "、";
                    }

                    //*****Old的方法*****
                    //DataRow[] _rows6 = _dt.Select("OvertimeUID=" + OvertimeUID + " and OvertimeDate='" + OvertimeDate + "' and ((StartDateTime<='" + StartDateTime + "' and EndDateTime>='" + StartDateTime + "') or (StartDateTime<='" + StartDateTime + "' and StartDateTime>='" + StartDateTime + "'))");
                    //if (_rows6.Length > 0)
                    //{
                    //    string _UID = _rows6[0]["UID"].toStringTrim(),
                    //        Tips4Applicant = string.Empty;
                    //    if (UID != _UID)
                    //        Tips4Applicant = "【申请者】：" + _rows6[0]["DisplayName"].toStringTrim();

                    //    Tips4 += OvertimeDisplayName + " 在开始时间 " + OvertimeDate + " " + StartTime + " 有冲突 " + Tips4Applicant + "、";
                    //}

                    ////检查结束时间是否包含在已申请的记录中
                    //DataRow[] _rows7 = _dt.Select("OvertimeUID=" + OvertimeUID + " and OvertimeDate='" + OvertimeDate + "' and ((StartDateTime<='" + EndDateTime + "' and EndDateTime>='" + EndDateTime + "') or (EndDateTime<='" + EndDateTime + "' and EndDateTime>='" + EndDateTime + "'))");
                    //if (_rows7.Length > 0)
                    //{
                    //    string _UID = _rows7[0]["UID"].toStringTrim(),
                    //        Tips4Applicant = string.Empty;
                    //    if (UID != _UID)
                    //        Tips4Applicant = "【申请者】：" + _rows7[0]["DisplayName"].toStringTrim();

                    //    Tips4 += OvertimeDisplayName + " 在结束时间 " + OvertimeDate + " " + EndTime + " 有冲突 " + Tips4Applicant + "、";
                    //}
                    //*****Old的方法*****
                    //*****检查时间是否存在冲突End*****


                    var getPersonalRecord = GetPersonalRecord(OvertimeDate, ArrOvertimeUID[i].toInt(), WorkHourSysID.toInt(), 0, "WithdrawalOrMore");  //WithdrawalOrMore、ApplyOrMore
                    string OvertimeTotalByDay = getPersonalRecord.OvertimeTotalByDay,
                        ExceptDaiXiuByMonth = getPersonalRecord.ExceptDaiXiuByMonth,
                        ExceptDaiXiuByQuarter = getPersonalRecord.ExceptDaiXiuByQuarter;


                    //综合工时制
                    if (WorkHourSysID.toInt() == 2)
                    {
                        //当天累计
                        //季度的累计
                        //剔除代休后的季度加班时间+当前申请的加班时间，如果大于综合工时制允许的最大值则返回False
                        if ((ExceptDaiXiuByQuarter.toDecimal() + OvertimeHour.toDecimal()) > MaxValue.toDecimal())
                            Tips2 += OvertimeDisplayName + "、";

                    }
                    else  //否则都当标准工时制
                    {
                        //当天累计
                        //如果是平日加班申请单的情况下
                        //如果是休日
                        if (OvertimeTypeCode == "normal")
                        {
                            if (CheckIsHoliday(OvertimeDate))
                            {
                                //当天的加班累计+当前申请的小时数如果大于休日最大允许小时数WeekendMax
                                if ((OvertimeTotalByDay.toDecimal() + OvertimeHour.toDecimal()) > WeekendMax.toDecimal())
                                    Tips += OvertimeDisplayName + " 当天已申请了 " + OvertimeTotalByDay + " 小时的加班、";
                            }
                            else  //否则非休日（即平日）
                            {
                                //当天的加班累计+当前申请的小时数如果大于平日最大允许小时数WeekdaysMax
                                if ((OvertimeTotalByDay.toDecimal() + OvertimeHour.toDecimal()) > WeekdaysMax.toDecimal())
                                    Tips += OvertimeDisplayName + " 当天已申请了 " + OvertimeTotalByDay + " 小时的加班、";
                            }
                        }

                        //月度累计
                        //剔除代休后的月度加班时间+当前申请的加班时间，如果大于标准工时制允许的最大值则返回False
                        if ((ExceptDaiXiuByMonth.toDecimal() + OvertimeHour.toDecimal()) > MaxValue.toDecimal())
                            Tips2 += OvertimeDisplayName + "、";
                    }

                    //不管是标准还是综合当天的加班累计+当前申请的小时数如果大于24
                    if ((OvertimeTotalByDay.toDecimal() + OvertimeHour.toDecimal()) > 24)
                        Tips3 += OvertimeDisplayName + " 当天已申请过 " + OvertimeTotalByDay + " 小时的加班、";

                }

                if (!string.IsNullOrEmpty(Tips4))
                    flag = "保存失败，时间上存在冲突，在该时间段内已经提交过申请，<br/>详细：" + Tips4.Substring(0, Tips4.Length - 1) + "，请确认。";

                else
                {
                    if (!string.IsNullOrEmpty(Tips3))
                        flag = "保存失败，当日累计加班不允许超过24小时，<br/>详细：" + Tips3.Substring(0, Tips3.Length - 1) + "，请确认。";
                    else
                    {
                        if (!string.IsNullOrEmpty(Tips) && !string.IsNullOrEmpty(Tips2))
                            flag = "保存失败，当日累计加班超出允许加班的最大小时数，<br/>详细：" + Tips.Substring(0, Tips.Length - 1) + " 请确认，<br/>如果还需要继续加班请填写特殊加班申请单。";
                        else if (!string.IsNullOrEmpty(Tips))
                            flag = "保存失败，当日累计加班超出允许加班的最大小时数，<br/>详细：" + Tips.Substring(0, Tips.Length - 1) + " 请确认，<br/>如果还需要继续加班请填写特殊加班申请单。";
                        else if (!string.IsNullOrEmpty(Tips2))
                            flag = "保存失败，原因：" + Tips2.Substring(0, Tips2.Length - 1) + " 的累计加班已超出允许加班的最大小时数，<br/>如果还需要继续加班请先提交代休申请单。";
                        else
                            flag = "True";
                    }
                }
            }

            return flag;
        }


        /// <summary>
        /// 在修改时检查用户加班最大值，如果超出允许的最大值则返回提示消息，否则返回true
        /// </summary>
        /// <param name="Action"></param>
        /// <param name="ShortTableName"></param>
        /// <param name="ModuleID"></param>
        /// <param name="FormID"></param>
        /// <param name="IsApprovalForm"></param>
        /// <param name="FormFields"></param>
        /// <param name="OvertimeTypeCode"></param>
        /// <param name="OvertimeIDs"></param>
        /// <returns></returns>
        public static string CheckModifyUserOvertimeMax(string Action, string ShortTableName, string ModuleID, string FormID, Boolean IsApprovalForm, string FormFields, string OvertimeTypeCode, string OvertimeIDs)
        {
            string flag = "保存失败，读取个人加班时间失败",
                UID = MicroUserInfo.GetUserInfo("UID");

            string OvertimeDate = string.Empty, ShiftTypeID = string.Empty, StartTime = string.Empty, EndTime = string.Empty, OTHour = string.Empty, OTMin = string.Empty, OvertimeMealID = string.Empty, OvertimeHour = string.Empty;
            //string OvertimeUID = string.Empty;

            //用于检测批量修改时，记录中是否出现重复的用户（或者说是2条或以上记录是同一位用户），如果是日期和加班时间段都是一样的然而这种情况（即同一个人同一时间段申请两次加班）是不允许的
            int intRepeat = 0;

            dynamic json = JToken.Parse(FormFields) as dynamic;

            if (!string.IsNullOrEmpty(OvertimeTypeCode))
                OvertimeTypeCode = OvertimeTypeCode.ToLower();

            OvertimeDate = json["txtOvertimeDate"];
            OvertimeDate = OvertimeDate.toStringTrim();

            //OvertimeUID = json["selOvertimeUID"];  //1,2,3  可能存在多位加班的人员，这里插入的是子记录录所以在for循环读取

            ShiftTypeID = json["raShiftTypeID"];
            ShiftTypeID = ShiftTypeID.toStringTrim();

            StartTime = json["txtStartTime"];
            StartTime = StartTime.toStringTrim();

            OTHour = json["selOTHour"];
            OTHour = OTHour.toStringTrim();

            OTMin = json["selOTMin"];
            OTMin = OTMin.toStringTrim();

            OvertimeMealID = json["cbOvertimeMealID"];
            OvertimeMealID = OvertimeMealID.toStringTrim();

            //根据班次、加班日期、开始时间、持续小时、分钟、就餐ID计算出：OvertimeTime、EndTime、OvertimeHour
            var getOTHour = GetOTHour(ShiftTypeID, OvertimeDate, StartTime, OTHour, OTMin, OvertimeMealID);
            EndTime = getOTHour.EndTime;
            OvertimeHour = getOTHour.OvertimeHour;

            if (!string.IsNullOrEmpty(OvertimeDate) && OvertimeHour.toDecimal() > 0 && !string.IsNullOrEmpty(OvertimeIDs))
            {
                //根据FormID得到加班类型（正常加班、特殊班）的最大允许小时数。
                var getOvertimeType = GetOvertimeType(FormID);
                string WeekdaysMax = getOvertimeType.WeekdaysMax,
                    WeekendMax = getOvertimeType.WeekendMax;

                string Tips = string.Empty, Tips2 = string.Empty, Tips3 = string.Empty, Tips4 = string.Empty,

                monthStartDateTime = OvertimeDate.toDateMFirstDay() + " 00:00:00.000",
                monthEndDateTime = OvertimeDate.toDateMLastDay() + " 23:59:59.998",

                quarterStartDateTime = OvertimeDate.toDateQFirstDay() + " 00:00:00.000",
                quarterEndDateTime = OvertimeDate.toDateQLastDay() + " 23:59:59.998";

                //避免多次读取数据库，在这里打开UserInfo表
                DataTable _dtUsers = MicroDataTable.GetDataTable("Use");

                //得到当前日期的季度加班所有记录（所有用户的记录），后面再根据where条件进行筛选
                string _sql = "select * from HROvertime where Invalid=0 and Del=0 and ParentID<>0 and OvertimeDate between @StartDate and @EndDate and StateCode >=-4 ";
                SqlParameter[] _sp = {
                    new SqlParameter("@StartDate",SqlDbType.VarChar),
                     new SqlParameter("@EndDate",SqlDbType.VarChar)
                };

                _sp[0].Value = quarterStartDateTime;
                _sp[1].Value = quarterEndDateTime;

                DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];
                DataTable TargetDT = _dt.Clone();  //克隆一个空表


                //模拟更新OvertimeDate和OvertimeHour，得到目标DataTable，最后再在该目标DataTable进行汇总数据
                //根据传递过来的IDs进行更新，并复制到TargetDT表
                DataRow[] _rows = _dt.Select("OvertimeID in (" + OvertimeIDs + ")");
                if (_rows.Length > 0)
                {
                    foreach (DataRow _dr in _rows)
                    {
                        _dr["OvertimeDate"] = OvertimeDate.toDateTime();
                        _dr["OvertimeHour"] = OvertimeHour.toDecimal();
                        TargetDT.ImportRow(_dr);
                    }
                }

                //同时把除了IDs的记录（不需要更新）也复制到TargetDT表
                DataRow[] _rows2 = _dt.Select(" OvertimeID not in (" + OvertimeIDs + ")");
                if (_rows2.Length > 0)
                {
                    foreach (DataRow _dr2 in _rows2)
                    {
                        TargetDT.ImportRow(_dr2);
                    }
                }

                //根据OvertimeID查询，去除UID（OvertimeUID）重复的记录
                string _OvertimeIDs = "0", _OvertimeUIDs = "0";
                DataRow[] _rows3 = _dt.Select("OvertimeID in (" + OvertimeIDs + ")");
                if (_rows3.Length > 0)
                {
                    foreach (DataRow _dr3 in _rows3)
                    {
                        string _OvertimeID = _dr3["OvertimeID"].ToString(),
                        _OvertimeUID = _dr3["OvertimeUID"].ToString();

                        int Num = 0;
                        string[] ArrOvertimeUIDs = _OvertimeUIDs.Split(',');

                        if (ArrOvertimeUIDs.Length > 0)
                        {
                            for (int i = 0; i < ArrOvertimeUIDs.Length; i++)
                            {
                                if (ArrOvertimeUIDs[i].ToString() == _OvertimeUID)
                                {
                                    Num = Num + 1;
                                    intRepeat = intRepeat + 1;
                                }
                            }
                        }

                        if (Num == 0)
                        {
                            _OvertimeIDs += "," + _OvertimeID;
                            _OvertimeUIDs += "," + _OvertimeUID;
                        }

                    }
                }

                //得到唯一的UIDs（_OvertimeIDs）再重新遍历一次
                DataRow[] _rows4 = _dt.Select("OvertimeID in (" + _OvertimeIDs + ")");

                //批量修改时多条（至少两条或以上）记录是同一位用户
                if (intRepeat == 0)
                {
                    if (_rows4.Length > 0)
                    {
                        //获取休假记录（季度内所有用户记录）
                        //注意这里的比较时间是写反的(开始对结束，结束对开始)
                        string _sql5 = "select * from HRLeave where Invalid=0 and Del=0 and StartDateTime<=@EndDateTime and EndDateTime>=@StartDateTime";
                        SqlParameter[] _sp5 = {
                        new SqlParameter("@StartDateTime",SqlDbType.VarChar),
                        new SqlParameter("@EndDateTime",SqlDbType.VarChar)
                        };
                        _sp5[0].Value = quarterStartDateTime;
                        _sp5[1].Value = quarterEndDateTime;
                        DataTable _dt5 = MsSQLDbHelper.Query(_sql5, _sp5).Tables[0];

                        foreach (DataRow _dr4 in _rows4)
                        {
                            string OvertimeID = _dr4["OvertimeID"].ToString(),
                                OvertimeUID = _dr4["OvertimeUID"].ToString(),
                                OldOvertimeHour = _dr4["OvertimeHour"].ToString();

                            //获取用户工时制
                            var getUserWorkHourSystem = GetUserWorkHourSystem(OvertimeUID.toInt(), OvertimeDate);
                            string WorkHourSysID = getUserWorkHourSystem.WorkHourSysID,
                                   WorkHourSysName = getUserWorkHourSystem.WorkHourSysName,
                                   MaxValue = getUserWorkHourSystem.MaxValue;

                            //获取用户DisplayName
                            string OvertimeDisplayName = string.Empty;
                            DataRow[] _rowsUsers = _dtUsers.Select("UID=" + OvertimeUID.toInt());
                            if (_rowsUsers.Length > 0)
                            {
                                string UserName = _rowsUsers[0]["UserName"].toStringTrim(),
                                 ChineseName = _rowsUsers[0]["ChineseName"].toStringTrim(),
                                 EnglishName = _rowsUsers[0]["EnglishName"].toStringTrim(),
                                 AdDisplayName = _rowsUsers[0]["AdDisplayName"].toStringTrim();
                                OvertimeDisplayName = MicroUserInfo.GetUserInfo(UserName, ChineseName, EnglishName, AdDisplayName);
                            }


                            //检查同一时间段内是否有其它记录存在
                            //检查开始时间是否包含在已申请的记录中
                            string StartDateTime = (OvertimeDate + " " + StartTime).toDateTime("yyyy-MM-dd HH:mm:ss").AddSeconds(1).toDateFormat("yyyy-MM-dd HH:mm:ss"), //开始时间+1秒
                                   EndDateTime = (OvertimeDate + " " + EndTime).toDateTime("yyyy-MM-dd HH:mm:ss").AddSeconds(-1).toDateFormat("yyyy-MM-dd HH:mm:ss"); //结束时间-1秒

                            //如果开始时间大于结束时间（说明是跨天），结束时间天数+1
                            if (StartTime.toDateTime("yyyy-MM-dd HH:mm:ss") > EndTime.toDateTime("yyyy-MM-dd HH:mm:ss"))
                                EndDateTime = EndDateTime.toDateTime("yyyy-MM-dd HH:mm:ss").AddDays(1).toDateFormat("yyyy-MM-dd HH:mm:ss");

                            //*****检查时间是否存在冲突Start*****
                            //注意这里的比较时间是写反的(开始对结束，结束对开始)StartDateTime<=@EndDateTime and EndDateTime>=@StartDateTime
                            DataRow[] _rows6 = _dt.Select("OvertimeUID=" + OvertimeUID + " and OvertimeDate='" + OvertimeDate + "' and OvertimeID<>" + OvertimeID + " and StartDateTime<='" + EndDateTime + "' and EndDateTime>='" + StartDateTime + "'");
                            if (_rows6.Length > 0)
                            {
                                string FormNumber = _rows6[0]["FormNumber"].toStringTrim();
                                Tips4 += OvertimeDisplayName + " 的申请时间存在冲突，相关表单编号： " + FormNumber + "、";
                            }

                            //*****Old的方法*****
                            //DataRow[] _rows6 = _dt.Select("OvertimeUID=" + OvertimeUID + " and OvertimeDate='" + OvertimeDate + "' and OvertimeID<>" + OvertimeID + " and ((StartDateTime<='" + StartDateTime + "' and EndDateTime>='" + StartDateTime + "') or (StartDateTime<='" + StartDateTime + "' and StartDateTime>='" + StartDateTime + "'))");
                            //if (_rows6.Length > 0)
                            //{
                            //    string _UID = _rows6[0]["UID"].toStringTrim(),
                            //    Tips4Applicant = string.Empty;

                            //    if (UID != _UID)
                            //        Tips4Applicant = "【申请者】：" + _rows6[0]["DisplayName"].toStringTrim();

                            //    Tips4 += OvertimeDisplayName + " 在开始时间 " + OvertimeDate + " " + StartTime + " 有冲突 " + Tips4Applicant + "、";
                            //}

                            ////检查结束时间是否包含在已申请的记录中
                            //DataRow[] _rows7 = _dt.Select("OvertimeUID=" + OvertimeUID + " and OvertimeDate='" + OvertimeDate + "' and OvertimeID<>" + OvertimeID + " and ((StartDateTime<='" + EndDateTime + "' and EndDateTime>='" + EndDateTime + "') or (EndDateTime<='" + EndDateTime + "' and EndDateTime>='" + EndDateTime + "'))");
                            //if (_rows7.Length > 0)
                            //{
                            //    string _UID = _rows7[0]["UID"].toStringTrim(),
                            //    Tips4Applicant = string.Empty;

                            //    if (UID != _UID)
                            //        Tips4Applicant = "【申请者】：" + _rows7[0]["DisplayName"].toStringTrim();

                            //    Tips4 += OvertimeDisplayName + " 在结束时间 " + OvertimeDate + " " + EndTime + " 有冲突 " + Tips4Applicant + "、";
                            //}
                            //*****Old的方法*****
                            //*****检查时间是否存在冲突End*****


                            //获取用户加班
                            //当天加班累计（为什么不采用MicroHR.GetPersonalRecord()方法？因为在修改状态下有可能多条记录都属于同一位加班者，这样每条记录都与初次取出的值进行比较会不准确，正确的应该是后一条记录累计前一条记录）
                            string overtimeTotalByDay = TargetDT.Compute("sum(OvertimeHour)", "OvertimeDate='" + OvertimeDate.toDateFormat() + "' and OvertimeUID=" + OvertimeUID + "").ToString(),
                                //月度加班累计
                                overtimeTotalByMonth = TargetDT.Compute("sum(OvertimeHour)", "OvertimeDate>='" + monthStartDateTime + "' and OvertimeDate<='" + monthEndDateTime + "' and OvertimeUID=" + OvertimeUID + "").ToString(),
                                //季度加班累计
                                overtimeTotalByQuarter = TargetDT.Compute("sum(OvertimeHour)", "OvertimeUID=" + OvertimeUID + "").ToString();  //TargetDT本身是按季度查询所以不需要传入季度的开始和结束日期


                            //获取用户已代休
                            //月度已代休 HolidayTypeID=2  以加班月份进行统计
                            string alreadyDaiXiuOvertimeByMonth = _dt5.Compute("sum(LeaveDays)", "HolidayTypeID=2 and LeaveUID=" + OvertimeUID + " and OvertimeDate>='" + monthStartDateTime.toDateFormat("yyyy-MM") + "' and OvertimeDate<='" + monthEndDateTime.toDateFormat("yyyy-MM") + "'").ToString();
                            if (string.IsNullOrEmpty(alreadyDaiXiuOvertimeByMonth))
                                alreadyDaiXiuOvertimeByMonth = "0";

                            //季度已代休 HolidayTypeID=2 ,以加班季度进行统计
                            string alreadyDaiXiuOvertimeByQuarter = _dt5.Compute("sum(LeaveDays)", "HolidayTypeID=2 and LeaveUID=" + OvertimeUID + " and OvertimeDate>='" + quarterStartDateTime.toDateFormat("yyyy-MM") + "' and OvertimeDate<='" + quarterEndDateTime.toDateFormat("yyyy-MM") + "' ").ToString();
                            if (string.IsNullOrEmpty(alreadyDaiXiuOvertimeByQuarter))
                                alreadyDaiXiuOvertimeByQuarter = "0";


                            //综合工时制
                            if (WorkHourSysID.toInt() == 2)
                            {
                                //当天累计
                                //季度的累计
                                //剔除代休后的季度加班时间+当前申请的加班时间(季度加班汇总-季度已代休)，如果大于综合工时制允许的最大值则返回False
                                if ((overtimeTotalByQuarter.toDecimal() - alreadyDaiXiuOvertimeByQuarter.toDecimal() * 8) > MaxValue.toDecimal())
                                    Tips2 += OvertimeDisplayName + "、";

                            }
                            else  //否则都当标准工时制
                            {
                                //当天累计
                                //如果是平日加班申请单的情况下
                                //如果是休日
                                if (OvertimeTypeCode == "normal")
                                {
                                    //判断是否为休日，如果是休日
                                    if (CheckIsHoliday(OvertimeDate))
                                    {
                                        //当天的加班累计+当前申请的小时数如果大于休日最大允许小时数WeekendMax
                                        if (overtimeTotalByDay.toDecimal() > WeekendMax.toDecimal())
                                            Tips += "在修改后 " + OvertimeDisplayName + " " + OvertimeDate + " 当天累计加班达到 " + overtimeTotalByDay + " 小时、";
                                    }
                                    else  //否则非休日（即平日）
                                    {
                                        //当天的加班累计+当前申请的小时数如果大于平日最大允许小时数WeekdaysMax
                                        if (overtimeTotalByDay.toDecimal() > WeekdaysMax.toDecimal())
                                            Tips += "在修改后 " + OvertimeDisplayName + " " + OvertimeDate + " 当天累计加班达到 " + overtimeTotalByDay + " 小时、";
                                    }
                                }

                                //月度累计
                                //剔除代休后的月度加班时间+当前申请的加班时间，如果大于标准工时制允许的最大值则返回False
                                if ((overtimeTotalByMonth.toDecimal() - alreadyDaiXiuOvertimeByMonth.toDecimal() * 8) > MaxValue.toDecimal())
                                    Tips2 += OvertimeDisplayName + "、";
                            }

                            //不管是标准还是综合当天的加班累计+当前申请的小时数如果大于24
                            if (overtimeTotalByDay.toDecimal() > 24)
                                Tips3 += "在修改后 " + OvertimeDisplayName + " " + OvertimeDate + " 当天累计加班达到 " + overtimeTotalByDay + " 小时、";

                        }

                    }

                    if (!string.IsNullOrEmpty(Tips4))
                        flag = "保存失败，时间上存在冲突，在该时间段内已经提交过申请，<br/>详细：" + Tips4.Substring(0, Tips4.Length - 1) + "，请确认。";
                    else
                    {
                        if (!string.IsNullOrEmpty(Tips3))
                            flag = "保存失败，当日累计加班不允许超过24小时，<br/>详细：" + Tips3.Substring(0, Tips3.Length - 1) + "，请确认。";
                        else
                        {
                            if (!string.IsNullOrEmpty(Tips) && !string.IsNullOrEmpty(Tips2))
                                flag = "保存失败，当日累计加班超出允许加班的最大小时数，<br/>详细：" + Tips.Substring(0, Tips.Length - 1) + " 请确认，<br/>如果还需要继续加班请填写特殊加班申请单。";
                            else if (!string.IsNullOrEmpty(Tips))
                                flag = "保存失败，当日累计加班超出允许加班的最大小时数，<br/>详细：" + Tips.Substring(0, Tips.Length - 1) + " 请确认，<br/>如果还需要继续加班请填写特殊加班申请单。";
                            else if (!string.IsNullOrEmpty(Tips2))
                                flag = "保存失败，原因：" + Tips2.Substring(0, Tips2.Length - 1) + " 的累计加班已超出允许加班的最大小时数，<br/>如果还需要继续加班请先提交代休申请单。";
                            else
                                flag = "True";
                        }
                    }
                }
                else
                    flag = "保存失败，批量修改时多条记录属于同一位加班者，<br/>这样会导致同一位加班者在同一时间段内出现多条重复的加班申请记录，<br/>然而这种情况是不允许的，您可以选择单条记录进行修改，<br/>请确认谢谢！";
            }

            return flag;
        }
    }
}