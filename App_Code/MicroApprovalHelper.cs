using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroDTHelper;
using MicroFormHelper;
using MicroWorkFlowHelper;
using MicroAuthHelper;
using MicroUserHelper;
using MicroPublicHelper;
using MicroNoticeHelper;
using Newtonsoft.Json.Linq;

namespace MicroApprovalHelper
{
    public class MicroApproval
    {

        public class ApprovalAttr
        {
            /// <summary>
            /// ClassName显示相关按钮
            /// </summary>
            public string ClassName { set; get; }

            /// <summary>
            /// 是否在允许撤回的阶段，True则允许
            /// </summary>
            public Boolean Withdrawal { set; get; }

            /// <summary>
            /// 是否允许转发，True则允许
            /// </summary>
            public Boolean Forward { set; get; }


            public int FormState { set; get; }

        }


        /// <summary>
        /// 获取表单状态返回Class、Withdrawal、Forward等结果（用于显示表单相关按钮、是否允许撤回、是否允许转发）
        /// </summary>
        /// <param name="Action"></param>
        /// <param name="ShortTableName"></param>
        /// <param name="ModuleID"></param>
        /// <param name="FormID"></param>
        /// <param name="FormsID"></param>
        /// <returns></returns>
        public static ApprovalAttr GetApproval(string Action, string ShortTableName, string ModuleID, string FormID, string FormsID)
        {

            Boolean withdrawal = false, //撤回，True允许撤回
                forward = false;  //转发，True允许转发

            string className = "micro-layer-class4", //默认只显示打印和关闭，审批、修改、受理和结案隐藏
                FormNumber = string.Empty,
                UID = MicroUserInfo.GetUserInfo("UID"),
                DisplayName = MicroUserInfo.GetUserInfo("DisplayName"),
                ApprovalState = MicroWorkFlow.GetApprovalState(1),
                IP = MicroUserInfo.GetUserInfo("IP");

            var GetFormAttr = MicroForm.GetFormAttr(ShortTableName, FormID);

            Boolean IsApproval = GetFormAttr.IsApproval;
            Boolean AutoAccept = GetFormAttr.AutoAccept;
            Boolean AutoClose = GetFormAttr.AutoClose;

            // 检查表单状态，通常表单状态小于“0”时才允许修改。
            // 0 = 等待审批[Waiting]、1 = 审批通过[Pass]、-1 = 驳回申请[Return]、-2 = 临时保存[Draft]、11 = 提交申请[Pass]、22 = 等待受理[WaitingForAccept]、23 = 等待对应[WaitingForProcessing]、24 = 等待处理[WaitingForProcessing]、32 = 受理中[Accepting]、33 = 对应中[Processing]、34 = 处理中[Processing]、100 = 完成[Finish]
            int formState = MicroWorkFlow.GetFormState(ShortTableName, FormID, FormsID);

            //表单状态>=0时才判断是否需要审批，因为表单状态<0时代表表单还没有提交所以不需要审批
            if (formState >= 0)
            {
                var GetFormApprovalRecordsAttr = MicroWorkFlow.GetFormApprovalRecordsAttr(FormID, FormsID);
                DataTable _dt = GetFormApprovalRecordsAttr.SourceDT;

                if (_dt != null && _dt.Rows.Count > 0)
                {
                    //表单需要审批时（来自表单属性）
                    if (IsApproval)
                    {
                        //创建者的UID，来自FormApprovalRecords的UID
                        string uid = _dt.Rows[0]["UID"].toStringTrim();
                        //0. 审批者，撤回审批
                        if (formState == 15)
                        {
                            DataRow[] _rows = _dt.Select("CanApprovalUID<>'' and ApprovalUID=" + UID + " and StateCode=15", "Sort");

                            if (_rows.Length > 0)
                            {
                                string FlowCode = _rows[0]["FlowCode"].toStringTrim();

                                if (FlowCode == "Approval")
                                {
                                    className = "micro-layer-class5-2";

                                    //有全局审批权限时（系统默认根据CanApprovalUID判断是否有审批权限，这里的权限是指赋予系统权限的审批权限，该权限凌驾于CanApprovalUID之上，代表你不是该表单的审批者也有权限审批）
                                    if (MicroAuth.CheckPermit(ModuleID, "12"))
                                        className = "micro-layer-class5-2";
                                }
                                else if (FlowCode == "Accept")
                                {
                                    className = "micro-layer-class6-2";

                                    //有全局审批权限时（系统默认根据CanApprovalUID判断是否有审批权限，这里的权限是指赋予系统权限的审批权限，该权限凌驾于CanApprovalUID之上，代表你不是该表单的审批者也有权限审批）
                                    if (MicroAuth.CheckPermit(ModuleID, "12"))
                                        className = "micro-layer-class6-2";
                                }
                            }
                            else  //让申请者在任意阶段有撤回功能，显示撤回按钮
                            {
                                //如果你是申请者，并且表单是加班申请或像休假申请，显示撤回按钮
                                if (uid.toInt() == UID.toInt() && uid.toInt() != 0)
                                {
                                    //如果是加班或休假申请表时
                                    string shortTableName = ShortTableName.ToLower();
                                    if (shortTableName == "overtime" || shortTableName == "leave")
                                    {
                                        className = "micro-layer-class4-2";
                                        withdrawal = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            DataRow[] _rows = _dt.Select("CanApprovalUID<>'' and ApprovalUID=0 and StateCode=0 and FixedNode=0", "Sort");

                            //如果有审批记录需要审批时(即处于审批状态时)
                            if (_rows.Length > 0)
                            {
                                //得到当前审批节点的可审批者
                                string iCanApprovalUID = _rows[0]["CanApprovalUID"].toJsonTrim();
                                Boolean CurrApprovalUID = MicroPublic.CheckSplitExists(iCanApprovalUID, UID, ',');  //获取当前登录用户是否有审批权限，判断是否有权限审批（CanApprovalUID与当前登录用户UID比较）

                                //有全局审批权限时（系统默认根据CanApprovalUID判断是否有审批权限，这里的权限是指赋予系统权限的审批权限，该权限凌驾于CanApprovalUID之上，代表你不是该表单的审批者也有权限审批）
                                if (MicroAuth.CheckPermit(ModuleID, "12"))
                                    className = "micro-layer-class5";  //显示审批和撤回按钮
                                else
                                {
                                    //判断当前登录是否为上一审批者，如果是则显示撤回按钮
                                    //如果上一审批节点是你，当前审批节点也是你（则显示审批和撤回按钮）
                                    if (CheckPreNodeUID(FormID, FormsID) && CurrApprovalUID)
                                        className = "micro-layer-class5";  //表单处理审批状态（表单不能修改），按钮只显示审批、打印和关闭，修改、受理和结案隐藏

                                    //否则如果上一审批节点者是你，但当前审批节点不是你（则显示撤回按钮）
                                    else if (CheckPreNodeUID(FormID, FormsID) && !CurrApprovalUID)
                                    {
                                        className = "micro-layer-class8";
                                        withdrawal = true;
                                    }

                                    //否则上一节点不是你，但当前节点是你（则显示审批）
                                    else if (!CheckPreNodeUID(FormID, FormsID) && CurrApprovalUID)
                                        className = "micro-layer-class5-2";

                                    //否则你是申请者，显示撤回按钮
                                    else if (uid.toInt() == UID.toInt() && uid.toInt() != 0)
                                    {
                                        //如果是加班或休假申请表时
                                        string shortTableName = ShortTableName.ToLower();
                                        if (shortTableName == "overtime" || shortTableName == "leave")
                                        {
                                            className = "micro-layer-class4-2";
                                            withdrawal = true;
                                        }
                                    }

                                    //否则以上条件都不符合则保持默认
                                }

                            }
                            else //否则没有需要审批的记录时
                            {
                                //没有审批记录需要审批时，出现以下情况：
                                //1.处于等待受理状态
                                if (formState > 20 && formState < 30)
                                {

                                    DataRow[] _rows2 = _dt.Select("CanApprovalUID<>'' and ApprovalUID=0 and StateCode=0", "Sort");
                                    if (_rows2.Length > 0)
                                    {
                                        string iCanApprovalUID = _rows2[0]["CanApprovalUID"].toJsonTrim();  //或者采用_rows2.Length-2
                                        Boolean CurrApprovalUID = MicroPublic.CheckSplitExists(iCanApprovalUID, UID, ','); //获取当前登录用户是否有审批权限，判断是否有权限审批（CanApprovalUID与当前登录用户UID比较）

                                        //有系统设置的受理权限时
                                        if (MicroAuth.CheckPermit(ModuleID, "13"))
                                            className = "micro-layer-class6";
                                        else
                                        {
                                            //上一节点是你，当前节点也是你，有可能你需要撤回或只需要受理（所以要显示撤回和受理）
                                            if (CheckPreNodeUID(FormID, FormsID) && CurrApprovalUID)
                                                className = "micro-layer-class6";
                                            //否则上一节点是你，但当前节点不是你（只显示撤回）
                                            else if (CheckPreNodeUID(FormID, FormsID) && !CurrApprovalUID)
                                            {
                                                className = "micro-layer-class8";
                                                withdrawal = true;
                                            }
                                            //否则上一节点不是你，但当前节点是你（只显示受理）
                                            else if (!CheckPreNodeUID(FormID, FormsID) && CurrApprovalUID)
                                            {
                                                if (uid.toInt() != 0)
                                                {
                                                    if (uid.toInt() == UID.toInt())
                                                    {
                                                        //如果是加班或休假申请表时
                                                        string shortTableName = ShortTableName.ToLower();
                                                        if (shortTableName == "overtime" || shortTableName == "leave")
                                                        {
                                                            className = "micro-layer-class6";  //受理（含撤回按钮）
                                                            withdrawal = true;
                                                        }
                                                        else
                                                            className = "micro-layer-class6-2"; //受理（不含撤回按钮）
                                                    }
                                                    else
                                                        className = "micro-layer-class6-2";  //受理（不含撤回按钮）

                                                }
                                            }
                                            //否则以上条件都不符合则保持默认
                                        }
                                    }
                                }

                                //2.处于结案状态
                                else if (formState > 30 && formState < 40)
                                {
                                    DataRow[] _rows2 = _dt.Select("CanApprovalUID<>'' and ApprovalUID=0 and StateCode=0", "Sort");
                                    if (_rows2.Length > 0)
                                    {
                                        string iCanApprovalUID = _rows2[0]["CanApprovalUID"].toJsonTrim();  //或者采用_rows2.Length-2
                                        Boolean CurrApprovalUID = MicroPublic.CheckSplitExists(iCanApprovalUID, UID, ','); //获取当前登录用户是否有审批权限，判断是否有权限审批（CanApprovalUID与当前登录用户UID比较）
                                                                                                                           //有结案权限时
                                        if (MicroAuth.CheckPermit(ModuleID, "14"))
                                        {
                                            className = "micro-layer-class7";
                                            withdrawal = true;
                                        }
                                        else
                                        {
                                            //没有结案权限时，判断是否为结案者（FormApprovalRecords表里该表单记录的结案步骤）
                                            //上一节点是你，当前节点也是你，有可能你需要撤回或只需要结案（所以要显示撤回和结案）
                                            if (CheckPreNodeUID(FormID, FormsID) && CurrApprovalUID)
                                            {
                                                className = "micro-layer-class7";
                                                withdrawal = true;
                                            }
                                            //否则上一节点是你，但当前节点不是你（只显示撤回）
                                            else if (CheckPreNodeUID(FormID, FormsID) && !CurrApprovalUID)
                                            {
                                                className = "micro-layer-class8";
                                                withdrawal = true;
                                            }
                                            //否则上一节点不是你，但当前节点是你（只显示结案）
                                            else if (!CheckPreNodeUID(FormID, FormsID) && CurrApprovalUID)
                                                className = "micro-layer-class7-2";

                                            //否则以上条件都不符合则保持默认
                                        }
                                    }
                                }
                                //3.处理完成阶段
                                else if (formState == 100)
                                {
                                    //否则你是申请者，显示撤回按钮
                                    if (uid.toInt() == UID.toInt() && uid.toInt() != 0)
                                    {
                                        //如果是加班或休假申请表时
                                        string shortTableName = ShortTableName.ToLower();
                                        if (shortTableName == "overtime" || shortTableName == "leave")
                                        {
                                            className = "micro-layer-class4-2";
                                            withdrawal = true;
                                        }
                                    }
                                }
                            }
                        }

                    }
                    else  //否则非审批表单时
                    {
                        //表单属性不需要审批时，出现以下情况：
                        string uid = _dt.Rows[0]["UID"].toStringTrim();
                        if (formState == 15)
                        {
                            DataRow[] _rows = _dt.Select("CanApprovalUID<>'' and ApprovalUID=" + UID + " and StateCode=15", "Sort");

                            if (_rows.Length > 0)
                            {
                                string FlowCode = _rows[0]["FlowCode"].toStringTrim();

                                if (FlowCode == "Accept")
                                {
                                    className = "micro-layer-class6-2";

                                    //有全局审批权限时（系统默认根据CanApprovalUID判断是否有审批权限，这里的权限是指赋予系统权限的审批权限，该权限凌驾于CanApprovalUID之上，代表你不是该表单的审批者也有权限审批）
                                    if (MicroAuth.CheckPermit(ModuleID, "12"))
                                        className = "micro-layer-class6-2";
                                }
                            }
                        }

                        //1.处于受理状态
                        if (formState > 20 && formState < 30)
                        {
                            DataRow[] _rows2 = _dt.Select("CanApprovalUID<>'' and ApprovalUID=0 and StateCode=0", "Sort");
                            if (_rows2.Length > 0)
                            {
                                string iCanApprovalUID = _rows2[0]["CanApprovalUID"].toJsonTrim();  //或者采用_rows2.Length-2
                                Boolean CurrApprovalUID = MicroPublic.CheckSplitExists(iCanApprovalUID, UID, ','); //获取当前登录用户是否有审批权限，判断是否有权限审批（CanApprovalUID与当前登录用户UID比较）

                                //有系统设置的受理权限时
                                if (MicroAuth.CheckPermit(ModuleID, "13"))
                                    className = "micro-layer-class6";
                                else
                                {
                                    //上一节点是你，当前节点也是你，有可能你需要撤回或只需要受理（所以要显示撤回和受理）
                                    if (CheckPreNodeUID(FormID, FormsID) && CurrApprovalUID)
                                        className = "micro-layer-class6";
                                    //否则上一节点是你，但当前节点不是你（只显示撤回）
                                    else if (CheckPreNodeUID(FormID, FormsID) && !CurrApprovalUID)
                                    {
                                        className = "micro-layer-class8";
                                        withdrawal = true;
                                    }
                                    //否则上一节点不是你，但当前节点是你（只显示受理隐藏撤回）
                                    else if (!CheckPreNodeUID(FormID, FormsID) && CurrApprovalUID)
                                        className = "micro-layer-class6-2";

                                    //否则以上条件都不符合则保持默认
                                }
                            }
                        }

                        //2.处于结案状态
                        else if (formState > 30 && formState < 40)
                        {
                            DataRow[] _rows2 = _dt.Select("CanApprovalUID<>'' and ApprovalUID=0 and StateCode=0", "Sort");
                            if (_rows2.Length > 0)
                            {
                                string iCanApprovalUID = _rows2[0]["CanApprovalUID"].toJsonTrim();  //或者采用_rows2.Length-2
                                Boolean CurrApprovalUID = MicroPublic.CheckSplitExists(iCanApprovalUID, UID, ','); //获取当前登录用户是否有审批权限，判断是否有权限审批（CanApprovalUID与当前登录用户UID比较）

                                //有结案权限时
                                if (MicroAuth.CheckPermit(ModuleID, "14"))
                                {
                                    className = "micro-layer-class7";
                                    withdrawal = true;
                                }
                                else
                                {
                                    //没有结案权限时，判断是否为结案者（FormApprovalRecords表里该表单记录的结案步骤）
                                    //上一节点是你，当前节点也是你，有可能你需要撤回或只需要结案（所以要显示撤回和结案）
                                    if (CheckPreNodeUID(FormID, FormsID) && CurrApprovalUID)
                                    {
                                        className = "micro-layer-class7";
                                        withdrawal = true;
                                    }
                                    //否则上一节点是你，但当前节点不是你（只显示撤回）
                                    else if (CheckPreNodeUID(FormID, FormsID) && !CurrApprovalUID)
                                    {
                                        className = "micro-layer-class8";
                                        withdrawal = true;
                                    }
                                    //否则上一节点不是你，但当前节点是你（只显示结案）
                                    else if (!CheckPreNodeUID(FormID, FormsID) && CurrApprovalUID)
                                        className = "micro-layer-class7-2";

                                    //否则以上条件都不符合则保持默认
                                }
                            }
                        }
                    }
                }
            }
            else  //FormState<0表单状态被驳回或保存草稿时（表单处于可修改状态），只显示修改、打印和关闭，审批、受理和结案隐藏
            {
                //具有修改所有权限
                if (MicroAuth.CheckPermit(ModuleID, "10"))
                {
                    if (formState == -1 || formState == -4)  //被驳回或被撤回（按钮显示修改）
                        className = "micro-layer-class3";
                    else  //填写申请或草稿（按钮显示提交）
                        className = "micro-layer-class3-2";
                }
                else
                {
                    //判断是否有修改自己记录的权限
                    if (MicroAuth.CheckPermit(ModuleID, "9"))
                    {
                        var GetFormRecordAttr = MicroWorkFlow.GetFormRecordAttr(ShortTableName, FormID, FormsID);
                        if (UID != "0" && UID.toInt() == GetFormRecordAttr.UID)
                        {
                            if (formState == -1 || formState == -4)  //被驳回或被撤回
                                className = "micro-layer-class3";
                            else  //填写申请或草稿
                                className = "micro-layer-class3-2";
                        }
                        //否则显示默认
                    }
                    //否则显示默认

                }

            }


            var ApprovalAttr = new ApprovalAttr
            {
                ClassName = className,
                Withdrawal = withdrawal,
                Forward = forward,
                FormState = formState
            };

            return ApprovalAttr;
        }



        /// <summary>
        /// 获得表单当前已审批节点的最大记录，作为当前未审批节点的上一节点（用于判断当前节点审批完时，而下一节点没有审批此时可撤回）
        /// </summary>
        /// <param name="FormID"></param>
        /// <param name="FormsID"></param>
        /// <returns></returns>
        private static Boolean CheckPreNodeUID(string FormID, string FormsID)
        {
            Boolean flag = false;

            try
            {
                string _sql = "select FARID from FormApprovalRecords where Invalid=0 and Del=0 and FormID=@FormID and FormsID=@FormsID " +
                    //得到当前表单已审批的节点的最大值
                    "and FARID = (select max(FARID) from FormApprovalRecords where Invalid=0 and Del=0 and FormID=@FormID and FormsID=@FormsID and CanApprovalUID<>'' and ApprovalUID<>0) " +
                    //同时判断审批者是否为当前登录用户
                    "and ApprovalUID=@ApprovalUID";

                SqlParameter[] _sp = {
                        new SqlParameter("@FormID",SqlDbType.Int),
                        new SqlParameter("@FormsID",SqlDbType.Int),
                        new SqlParameter("@ApprovalUID",SqlDbType.Int)
                        };

                _sp[0].Value = FormID.toInt();
                _sp[1].Value = FormsID.toInt();
                _sp[2].Value = MicroUserInfo.GetUserInfo("UID").toInt();

                flag = MsSQLDbHelper.Exists(_sql, _sp);
            }
            catch { }

            return flag;

        }



        /// <summary>
        /// 同意申请
        /// </summary>
        /// <param name="FormID"></param>
        /// <param name="FormsID"></param>
        /// <param name="Fields"></param>
        /// <returns></returns>
        public static string SetAgreeForm(string ModuleID, string ShortTableName, string FormID, string FormsID, string Fields, int FormStateCode)
        {
            //审批状态：-40 = 删除[Delete]、-30 = 无效[Invalid]、-4 = 撤回[Withdrawal]、-3 = 填写申请[Fill in]、-2 = 临时保存[Draft]、-1 = 驳回申请[Return]、0 = 等待审批[Waiting]、1 = 审批通过[Pass]、11 = 提交申请[Pass]、15 = 撤回审批[WithdrawalApproval]、18 = 转发[Forward]、22 = 等待受理[WaitingForAccept]、23 = 等待对应[WaitingForProcessing]、24 = 等待处理[WaitingForProcessing]、32 = 受理中[Accepting]、33 = 对应中[Processing]、34 = 处理中[Processing]、100 = 完成[Finish]
            int ApprovalStateCode = 1;

            string flag = MicroPublic.GetMsg("SaveFailed"),
                    FormNumber = string.Empty,
                    UID = MicroUserInfo.GetUserInfo("UID"),
                    DisplayName = MicroUserInfo.GetUserInfo("DisplayName"),
                    ApprovalState = MicroWorkFlow.GetApprovalState(ApprovalStateCode),
                    IP = MicroUserInfo.GetUserInfo("IP");

            var GetFormAttr = MicroForm.GetFormAttr(ShortTableName, FormID);
            Boolean IsApproval = GetFormAttr.IsApproval;
            Boolean AutoClose = GetFormAttr.AutoClose;
            string FormName = GetFormAttr.FormName;

            try
            {
                //表单属性不需要审批时
                if (!IsApproval)
                    flag = MicroPublic.GetMsg("NoApprovalRequired"); //"该表单不需要审批。<br/>The form does not require approval.";

                else  //否则需要审批时
                {
                    //***得到提交表单的页面控件的值 Start***
                    dynamic json = JToken.Parse(Fields) as dynamic;
                    string Note = json["txtNote"];  //审批备注
                    Note = Note.toJsonTrim();
                    string ckAllApproval = json["ckAllApproval"];  //是否一起审批
                    Boolean AllApproval = ckAllApproval.toBoolean(); //转为Boolean
                    //***得到提交表单的页面控件的值 End***


                    var GetFormApprovalRecordsAttr = MicroWorkFlow.GetFormApprovalRecordsAttr(FormID, FormsID);
                    DataTable _dt = GetFormApprovalRecordsAttr.SourceDT;

                    //*****得到申请者和受理者信息Start*****
                    //申请者信息
                    string Applicant = string.Empty;
                    string ApplicantID = string.Empty;
                    //受理者信息
                    string CanReceiveID = string.Empty;
                    string ReceiveID = string.Empty;

                    //当FormApprovalRecords表有记录时，以Sort排序的第一条记录作为申请者信息，以最后一条记录作为受理者申请
                    //（备注一下作提醒：不管理表单“IsApproval”是否有审批流，都是需要审批的，只是=false时只有申请和完成两步）
                    if (_dt.Rows.Count > 0)
                    {
                        //第一条记录（审批通过是通知申请者）
                        Applicant = _dt.Rows[0]["DisplayName"].toJsonTrim();
                        ApplicantID = _dt.Rows[0]["UID"].toJsonTrim();

                        //最后一条记录（_dt.Rows.Count - 1）（审批完成时通知受理者或结案者）
                        CanReceiveID = _dt.Rows[_dt.Rows.Count - 1]["CanApprovalUID"].toStringTrim();
                        ReceiveID = _dt.Rows[_dt.Rows.Count - 1]["ApprovalUID"].toStringTrim();
                    }
                    //*****得到申请者和受理者信息End*****

                    //得到需要审批而又未审批的记录（即审批步骤，如：科长审批，部长审批）
                    DataRow[] _rows = _dt.Select("CanApprovalUID<>'' and ApprovalUID=0 and StateCode=0 and FixedNode=0", "Sort");

                    if (FormStateCode == 15)
                    {
                        //全局审批权限（系统默认根据CanApprovalUID判断是否有审批权限，这里的权限是指赋予系统权限的审批权限，该权限凌驾于CanApprovalUID之上，代表你不是该表单的审批者也有权限审批）
                        if (MicroAuth.CheckPermit(ModuleID, "12"))
                            _rows = _dt.Select("CanApprovalUID<>'' and StateCode=15 and FixedNode=0", "Sort");

                        else
                            _rows = _dt.Select("CanApprovalUID<>'' and ApprovalUID=" + UID + " and StateCode=15 and FixedNode=0", "Sort");
                    }

                    if (_rows.Length > 0)
                    {
                        //根据当前登录用户判断第一条需要审批的步骤是否有审批权限（CanApprovalUID与当前登录用户UID比较）
                        Boolean Perm = false;
                        //判断是否有连续审批
                        Boolean isContinuous = AllApproval;
                        //有连续审批时把审批节点插入sb，没有的话插入第一个
                        StringBuilder sbFARID = new StringBuilder();
                        StringBuilder sbNodeNames = new StringBuilder();

                        //全局审批权限（系统默认根据CanApprovalUID判断是否有审批权限，这里的权限是指赋予系统权限的审批权限，该权限凌驾于CanApprovalUID之上，代表你不是该表单的审批者也有权限审批）
                        Boolean ApprovalPermit = MicroAuth.CheckPermit(ModuleID, "12");

                        for (int i = 0; i < _rows.Length; i++)
                        {
                            //根据当前登录用户判断第一步需要审批的步骤是否有审批权限
                            if (i == 0)
                            {
                                string iCanApprovalUID = _rows[i]["CanApprovalUID"].toJsonTrim();

                                //有全局审批权限时
                                if (ApprovalPermit)
                                {
                                    //得到FormNumber，用于插入FormApprovalLogs表
                                    FormNumber = _rows[i]["FormNumber"].toStringTrim();

                                    //把第一条记录插入sb
                                    sbFARID.Append(_rows[i]["FARID"].toStringTrim() + ",");
                                    sbNodeNames.Append(_rows[i]["NodeName"].toStringTrim() + ",");
                                    Perm = true;  //赋予审批权限
                                }
                                else
                                {
                                    //没有全局审批权限时，判断能审批的CanApprovalUID是否包含自己的(当前登录用户)UID，如果有可以审批
                                    if (MicroPublic.CheckSplitExists(iCanApprovalUID, UID, ','))
                                    {
                                        //得到FormNumber，用于插入FormApprovalLogs表
                                        FormNumber = _rows[i]["FormNumber"].toStringTrim();

                                        //把第一条记录插入sb
                                        sbFARID.Append(_rows[i]["FARID"].toStringTrim() + ",");
                                        sbNodeNames.Append(_rows[i]["NodeName"].toStringTrim() + ",");
                                        Perm = true;  //赋予审批权限
                                    }
                                    else
                                        Perm = false;
                                }
                            }

                            //*****得到连续审批的节点名称Start*****
                            if (isContinuous)
                            {
                                int j = i + 1;
                                if (j < _rows.Length)
                                {
                                    string jCanApprovalUID = _rows[j]["CanApprovalUID"].toJsonTrim();

                                    //每次都和第一条进行比较，判断是否为连续审批
                                    if (MicroPublic.CheckSplitExists(_rows[0]["CanApprovalUID"].toJsonTrim(), UID, ',') && MicroPublic.CheckSplitExists(jCanApprovalUID, UID, ','))
                                    {
                                        sbFARID.Append(_rows[j]["FARID"].toStringTrim() + ",");
                                        sbNodeNames.Append(_rows[j]["NodeName"].toStringTrim() + ",");
                                    }
                                    //只要有一次不是连续审批的话马上中断
                                    else
                                        isContinuous = false;
                                }
                            }
                            //*****得到连续审批的节点名称End*****

                        }

                        //当前登录用户有审批权限时，更新审批记录
                        if (Perm)
                        {
                            string FARIDs = sbFARID.ToString();
                            FARIDs = FARIDs.Substring(0, FARIDs.Length - 1);
                            int FARIDsSum = FARIDs.Split(',').Length;

                            string NodeNames = sbNodeNames.ToString();
                            NodeNames = NodeNames.Substring(0, NodeNames.Length - 1);

                            if (FARIDsSum > 0)
                            {
                                //更新审批记录
                                string _sql2 = "update FormApprovalRecords set ApprovalUID=@ApprovalUID, ApprovalDisplayName=@ApprovalDisplayName, StateCode=@StateCode, ApprovalState=@ApprovalState, ApprovalTime=@ApprovalTime, ApprovalIP=@ApprovalIP,  Note=@Note where FARID in (" + FARIDs + ")";

                                SqlParameter[] _sp2 = {
                                        new SqlParameter("@ApprovalUID",SqlDbType.Int),
                                        new SqlParameter("@ApprovalDisplayName",SqlDbType.NVarChar,200),
                                        new SqlParameter("@StateCode",SqlDbType.Int),
                                        new SqlParameter("@ApprovalState",SqlDbType.NVarChar,200),
                                        new SqlParameter("@ApprovalTime",SqlDbType.DateTime),
                                        new SqlParameter("@ApprovalIP",SqlDbType.VarChar,50),
                                        new SqlParameter("@Note",SqlDbType.NVarChar,4000),
                                };

                                _sp2[0].Value = UID.toInt();
                                _sp2[1].Value = DisplayName;
                                _sp2[2].Value = ApprovalStateCode;
                                _sp2[3].Value = ApprovalState;
                                _sp2[4].Value = DateTime.Now;
                                _sp2[5].Value = IP;
                                _sp2[6].Value = Note;

                                //执行更新审批记录语句
                                if (MsSQLDbHelper.ExecuteSql(_sql2, _sp2) > 0)
                                {
                                    flag = MicroPublic.GetMsg("Save");

                                    //审批结果更新表成功时，插入logs记录
                                    MicroWorkFlow.SetApprovalLogs(FormID, FormsID, FormNumber, NodeNames, UID, DisplayName, ApprovalStateCode, Note);

                                    //###1站点通知，审批通过通知申请者。例1：您提交的《xxx》申请，编号：xxx已由科长审批通过，例2：您提交的《xxx》申请，编号：xxx已由科长审批、部长审批通过
                                    //string Content = "您提交的" + FormName + "申请，编号：" + FormNumber + "，已由" + NodeNames + DisplayName + "审批通过";  //Update 20210112
                                    string Content = MicroNotice.GetNoticeContent("ApprovalToApplicant", ShortTableName, ModuleID, FormID, FormsID, FormNumber, FormName, NodeNames, "", DisplayName, Applicant, Note);
                                    MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, FormsID, FormNumber, "Msg", FormName, Content, false, ApplicantID);

                                    //继续判断是否为最后一步（完成）,若是则会有两种情况
                                    //1.若是则改变表单状态为【等待对应】
                                    //2.若是且表单是自动完成审批则自动补上最后一步操作记录
                                    //否则不改变

                                    //用于判断审批是否完成
                                    //检查表单所有审批是否完成，传入FormID & FormsID，进行条件运算 a.CanApprovalUID<>'' and a.ApprovalUID=0 and a.StateCode=0 and b.FixedNode=0判断，如果没有记录代表审批完成
                                    Boolean IsApprovalFinish = MicroWorkFlow.GetIsApprovalFinish(FormID, FormsID);

                                    //用于判断最后一位审批通过时，是否自动受理，是否自动结案
                                    var getFormAttr = MicroForm.GetFormAttr(ShortTableName, FormID);
                                    Boolean autoAccept = getFormAttr.AutoAccept;
                                    Boolean autoClose = getFormAttr.AutoClose;

                                    //如果开启自动完成审批，并且已是最后一位审批者审批通过时，1.更新表单状态，2.更新FormApprovalRecords状态（最后一步，即“完成”），3.更新日志状态
                                    //所有需要审批的节点已经完成时
                                    if (IsApprovalFinish)
                                    {
                                        //StateCode 0 = 等待审批[Waiting]、1 = 审批通过[Pass]、-1 = 驳回申请[Return]、-2 = 临时保存[Draft]、11 = 提交申请[Pass]、22 = 等待受理[WaitingForAccept]、23 = 等待对应[WaitingForProcessing]、24 = 等待处理[WaitingForProcessing]、32 = 受理中[Accepting]、33 = 对应中[Processing]、34 = 处理中[Processing]、100 = 完成[Finish]

                                        //判断下该表单的流程是否有受理步骤
                                        var getFormWorkFlowAttr = MicroWorkFlow.GetFormWorkFlowAttr(FormID);
                                        Boolean IsAccept = getFormWorkFlowAttr.IsAccept;

                                        //*****判断是否有自动受理Start*****
                                        //如果有受理步骤的情况下
                                        if (IsAccept)
                                        {
                                            //如果开启自动受理
                                            if (autoAccept)
                                            {
                                                //1.更新FormApprovalRecords状态（倒数第二步，即“受理”）
                                                DataRow[] _rows3 = _dt.Select("", "Sort");
                                                if (_rows3.Length > 0)
                                                {
                                                    int FARID3 = _rows3[_rows3.Length - 2]["FARID"].toInt();
                                                    string NodeName3 = _rows3[_rows3.Length - 2]["NodeName"].toJsonTrim(),
                                                           CanApprovalUID3 = _rows3[_rows3.Length - 2]["CanApprovalUID"].toJsonTrim(),
                                                           UID3 = UID,
                                                           DisplayName3 = DisplayName;

                                                    if (!string.IsNullOrEmpty(CanApprovalUID3))
                                                    {
                                                        if (!MicroPublic.CheckSplitExists(CanApprovalUID3, UID, ','))
                                                        {
                                                            UID3 = MicroPublic.GetSplitFirstStr(CanApprovalUID3, ',');
                                                            DisplayName3 = MicroUserInfo.GetUserInfo("DisplayName", UID3);
                                                        }
                                                    }

                                                    //2.更新日志状态（在FormApprovalRecords状态更新成功时)
                                                    if (MicroWorkFlow.SetFormApprovalRecords(UID3, DisplayName3, 33, Note, FARID3))
                                                    {
                                                        //3.更新表单状态，设置表单状态为“对应中[Processing]”状态
                                                        if (!MicroWorkFlow.SetFormState(ShortTableName, 33, FormID, FormsID))
                                                            flag = "更新表单状态失败。错误代码：101 <br/>The update of the form status failed.";

                                                        //###2受理了通知受理者（或处理者）。例1：xxx提交的《xxx》申请，编号：xxx，受理成功【系统自动受理】请您尽快对应（处理）。
                                                        //string Content2 = "" + Applicant + "提交的" + FormName + "申请，编号：" + FormNumber + "，受理成功【系统自动受理】请您尽快对应（处理）。";  //Update 20210112
                                                        string Content2 = MicroNotice.GetNoticeContent("AcceptAutoProcess", ShortTableName, ModuleID, FormID, FormsID, FormNumber, FormName, NodeNames, "", DisplayName, Applicant, Note);
                                                        MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, FormsID, FormNumber, "Msg", FormName, Content2, false, CanReceiveID);

                                                        //###3受理了通知申请者。例1：您提交的《xxx》申请，编号：xxx，已被受理【系统自动受理】正在为您安排对应（处理），请耐心等待。
                                                        //string Content3 = "您提交的" + FormName + "申请，编号：" + FormNumber + "，已被受理【系统自动受理】正在为您安排对应（处理），请耐心等待。"; //Update 20210112
                                                        string Content3 = MicroNotice.GetNoticeContent("AcceptAutoApplicant", ShortTableName, ModuleID, FormID, FormsID, FormNumber, FormName, NodeNames, "", DisplayName, Applicant, Note);
                                                        MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, FormsID, FormNumber, "Msg", FormName, Content3, false, ApplicantID);

                                                        if (!MicroWorkFlow.SetApprovalLogs(FormID, FormsID, FormNumber, NodeName3, UID3, DisplayName3, 33, Note + "【系统自动受理】"))
                                                            flag = "更新日志状态“自动受理”步骤失败。错误代码：102<br/>The auto accept step of updating the log status failed.";


                                                        //*****在自动受理成功后判断是否是自动结案Start*****
                                                        //如果开启自动结案
                                                        if (autoClose)
                                                        {
                                                            //1.更新FormApprovalRecords状态（最后一步，即“完成”）
                                                            DataRow[] _rows4 = _dt.Select("", "Sort");
                                                            if (_rows4.Length > 0)
                                                            {
                                                                int FARID4 = _rows4[_rows4.Length - 1]["FARID"].toInt();
                                                                string NodeName4 = _rows4[_rows4.Length - 1]["NodeName"].toJsonTrim(),
                                                                    CanApprovalUID4 = _rows4[_rows4.Length - 1]["CanApprovalUID"].toJsonTrim(),
                                                                    UID4 = UID,
                                                                    DisplayName4 = DisplayName;

                                                                if (!string.IsNullOrEmpty(CanApprovalUID4))
                                                                {
                                                                    if (!MicroPublic.CheckSplitExists(CanApprovalUID4, UID, ','))
                                                                    {
                                                                        UID4 = MicroPublic.GetSplitFirstStr(CanApprovalUID4, ',');
                                                                        DisplayName4 = MicroUserInfo.GetUserInfo("DisplayName", UID4);
                                                                    }
                                                                }

                                                                //2.更新日志状态（最后一步，即“完成”在FormApprovalRecords状态更新成功时，再次更新日志状态)
                                                                if (MicroWorkFlow.SetFormApprovalRecords(UID4, DisplayName4, 100, Note, FARID4))
                                                                {
                                                                    //StateCode 0 = 等待审批[Waiting]、1 = 审批通过[Pass]、-1 = 驳回申请[Return]、-2 = 临时保存[Draft]、11 = 提交申请[Pass]、22 = 等待受理[WaitingForAccept]、23 = 等待对应[WaitingForProcessing]、24 = 等待处理[WaitingForProcessing]、32 = 受理中[Accepting]、33 = 对应中[Processing]、34 = 处理中[Processing]、100 = 完成[Finish]
                                                                    //###1更新表单状态，设置表单状态为100“完成[Finish]”状态
                                                                    if (!MicroWorkFlow.SetFormState(ShortTableName, 100, FormID, FormsID))
                                                                        flag = "更新表单状态失败。错误代码：103 <br/>The update of the form status failed.";

                                                                    //###2结案了通知受理者。例1：xxx提交的《xxx》申请，编号：xxx，审批已通过并且对应（处理）完成【系统自动结案】。
                                                                    //string Content4 = "" + Applicant + "提交的" + FormName + "申请，编号：" + FormNumber + "，审批已通过并且对应（处理）完成【系统自动结案】。";  //Update 20210112
                                                                    string Content4 = MicroNotice.GetNoticeContent("ClosedAutoAccept", ShortTableName, ModuleID, FormID, FormsID, FormNumber, FormName, NodeNames, "", DisplayName, Applicant, Note);
                                                                    MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, FormsID, FormNumber, "Msg", FormName, Content4, false, CanReceiveID);

                                                                    //###3结案了通知申请者。例1：您提交的《xxx》申请，编号：xxx，已对应（处理）完成【系统自动结案】。
                                                                    //string Content5 = "您提交的" + FormName + "申请，编号：" + FormNumber + "，已对应（处理）完成【系统自动结案】。";  //Update 20210112
                                                                    string Content5 = MicroNotice.GetNoticeContent("ClosedAutoApplicant2", ShortTableName, ModuleID, FormID, FormsID, FormNumber, FormName, NodeNames, "", DisplayName, Applicant, Note);
                                                                    MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, FormsID, FormNumber, "Msg", FormName, Content5, false, ApplicantID);

                                                                    if (!MicroWorkFlow.SetApprovalLogs(FormID, FormsID, FormNumber, NodeName4, UID4, DisplayName4, 100, Note + "【系统自动结案】"))
                                                                        flag = "更新日志状态“自动结案”步骤失败。错误代码：104<br/>The auto close step of updating the log status failed.";

                                                                }
                                                                else
                                                                    flag = "更新审批状态“自动结案”步骤失败。错误代码：105<br/>The auto close step of updating the approval status failed.";
                                                            }
                                                        }
                                                        //autoClose else.非自动完成结案审批时，更新表单状态（FormApprovalRecords状态和日志状态已在上面更新）
                                                        else
                                                        {
                                                            if (!MicroWorkFlow.SetFormState(ShortTableName, 33, FormID, FormsID))
                                                                flag = "更新表单状态失败。错误代码：106 <br/>The update of the form status failed.";

                                                            //###4没有需要审批的，所有审批已完成，通知受理者
                                                            //站点通知，通知受理者。例1：xxx提交的《xxx》申请，编号：xxx，已由xxx审批通过，，拜托您进行对应（处理）。 例2：xxx提交的《xxx》申请，编号：xxx已由xx审批、xx审批通过，拜托您进行对应（处理）。
                                                            //string Content6 = "" + Applicant + "提交的" + FormName + "申请，编号：" + FormNumber + "，已由" + NodeNames + DisplayName + "审批通过，拜托您进行对应（处理）。";  //Update 20210112
                                                            string Content6 = MicroNotice.GetNoticeContent("ApprovalToProcess", ShortTableName, ModuleID, FormID, FormsID, FormNumber, FormName, NodeNames, "", DisplayName, Applicant, Note);
                                                            MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, FormsID, FormNumber, "Msg", FormName, Content6, false, CanReceiveID);

                                                        }
                                                        //*****在自动受理成功后判断是否是自动结案End*****
                                                    }
                                                    else
                                                        flag = "更新审批状态“自动受理”步骤失败。错误代码：107<br/>The auto accept step of updating the approval status failed.";
                                                }
                                            }
                                            //autoAccept else.非自动完成受理时，更新表单状态（FormApprovalRecords状态和日志状态已在上面更新）
                                            else
                                            {
                                                //非自动受理更新表单状态为22“等待受理[WaitingForAccept]”
                                                if (!MicroWorkFlow.SetFormState(ShortTableName, 22, FormID, FormsID))
                                                    flag = "更新表单状态失败。错误代码：108 <br/>The update of the form status failed.";

                                                //###4审批通过了，没有再需要审批的（由最后审批者触发，即所有审批已完成，因为这个是在所有审批已完成的代码块内），通知受理者
                                                //站点通知，通知受理者。例1：xxx提交的《xxx》申请，编号：xxx，已由xxx审批通过，拜托您进行对应。 例2：xxx提交的《xxx》申请，编号：xxx已由xx审批、xx审批通过，拜托您进行对应（处理）。
                                                //string Content2 = "" + Applicant + "提交的" + FormName + "申请，编号：" + FormNumber + "，已由" + NodeNames + DisplayName + "审批通过，拜托您进行对应（处理）。";  //Update 20210112
                                                string Content2 = MicroNotice.GetNoticeContent("ApprovalToProcess", ShortTableName, ModuleID, FormID, FormsID, FormNumber, FormName, NodeNames, "", DisplayName, Applicant, Note);
                                                MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, FormsID, FormNumber, "Msg", FormName, Content2, false, CanReceiveID);

                                            }
                                        }
                                        else  //*****否则没有启用受理步骤情况下Else，因为Close步骤是必须的*****
                                        {
                                            //*****判断是否有自动结案Start*****
                                            //如果开启自动结案
                                            if (autoClose)
                                            {
                                                //1.更新FormApprovalRecords状态（最后一步，即“完成”）
                                                DataRow[] _rowsLast = _dt.Select("", "Sort");
                                                if (_rowsLast.Length > 0)
                                                {
                                                    int FARID5 = _rowsLast[_rowsLast.Length - 1]["FARID"].toInt();
                                                    string NodeName5 = _rowsLast[_rowsLast.Length - 1]["NodeName"].toJsonTrim(),
                                                           CanApprovalUID5 = _rowsLast[_rowsLast.Length - 1]["CanApprovalUID"].toJsonTrim(),
                                                           UID5 = UID,
                                                           DisplayName5 = DisplayName;

                                                    if (!string.IsNullOrEmpty(CanApprovalUID5))
                                                    {
                                                        if (!MicroPublic.CheckSplitExists(CanApprovalUID5, UID, ','))
                                                        {
                                                            UID5 = MicroPublic.GetSplitFirstStr(CanApprovalUID5, ',');
                                                            DisplayName5 = MicroUserInfo.GetUserInfo("DisplayName", UID5);
                                                        }
                                                    }

                                                    //2.更新日志状态（最后一步，即“完成”在FormApprovalRecords状态更新成功时，再次更新日志状态)
                                                    if (MicroWorkFlow.SetFormApprovalRecords(UID5, DisplayName5, 100, Note, FARID5))
                                                    {
                                                        //StateCode 0 = 等待审批[Waiting]、1 = 审批通过[Pass]、-1 = 驳回申请[Return]、-2 = 临时保存[Draft]、11 = 提交申请[Pass]、22 = 等待受理[WaitingForAccept]、23 = 等待对应[WaitingForProcessing]、24 = 等待处理[WaitingForProcessing]、32 = 受理中[Accepting]、33 = 对应中[Processing]、34 = 处理中[Processing]、100 = 完成[Finish]
                                                        //3.更新表单状态，设置表单状态为100“完成[Finish]”状态
                                                        if (!MicroWorkFlow.SetFormState(ShortTableName, 100, FormID, FormsID))
                                                            flag = "更新表单状态失败。错误代码：109 <br/>The update of the form status failed.";

                                                        //###2结案了通知受理者。例1：xxx提交的《xxx》申请，编号：xxx，审批已通过并且对应（处理）完成【系统自动结案】。
                                                        //string Content2 = "" + Applicant + "提交的" + FormName + "申请，编号：" + FormNumber + "，审批已通过并且对应（处理）完成【系统自动结案】。";  //Update 20210112
                                                        string Content2 = MicroNotice.GetNoticeContent("ClosedAutoAccept", ShortTableName, ModuleID, FormID, FormsID, FormNumber, FormName, NodeNames, "", DisplayName, Applicant, Note);
                                                        MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, FormsID, FormNumber, "Msg", FormName, Content2, false, CanReceiveID);

                                                        //###3结案了通知申请者。例1：您提交的《xxx》申请，编号：xxx，已对应（处理）完成【系统自动结案】。
                                                        //string Content3 = "您提交的" + FormName + "申请，编号：" + FormNumber + "，已对应（处理）完成【系统自动结案】。";  //Update 20210112
                                                        string Content3 = MicroNotice.GetNoticeContent("ClosedAutoApplicant2", ShortTableName, ModuleID, FormID, FormsID, FormNumber, FormName, NodeNames, "", DisplayName, Applicant, Note);
                                                        MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, FormsID, FormNumber, "Msg", FormName, Content3, false, ApplicantID);

                                                        if (!MicroWorkFlow.SetApprovalLogs(FormID, FormsID, FormNumber, NodeName5, UID5, DisplayName5, 100, Note + "【系统自动结案】"))
                                                            flag = "更新日志状态“自动结案”步骤失败。错误代码：110<br/>The auto close step of updating the log status failed.";
                                                    }
                                                    else
                                                        flag = "更新审批状态“自动结案”步骤失败。错误代码：111<br/>The auto close step of updating the approval status failed.";
                                                }
                                            }
                                            //autoClose else.非自动完成结案审批时，更新表单状态（FormApprovalRecords状态和日志状态已在上面更新）
                                            else
                                            {
                                                //非自动受理更新表单状态为33“对应中[Processing]”
                                                if (!MicroWorkFlow.SetFormState(ShortTableName, 33, FormID, FormsID))
                                                    flag = "更新表单状态失败。错误代码：112 <br/>The update of the form status failed.";

                                                //###4没有需要审批的，所有审批已完成，通知受理者
                                                //站点通知，通知受理者。例1：xxx提交的《xxx》申请，编号：xxx，已由xxx审批通过，拜托您进行对应（处理）。 例2：xxx提交的《xxx》申请，编号：xxx已由xx审批、xx审批通过，拜托您进行对应（处理）。
                                                //string Content2 = "" + Applicant + "提交的" + FormName + "申请，编号：" + FormNumber + "，已由" + NodeNames + DisplayName + "审批通过，拜托您进行对应（处理）。";  //Update 20210112
                                                string Content2 = MicroNotice.GetNoticeContent("ApprovalToProcess", ShortTableName, ModuleID, FormID, FormsID, FormNumber, FormName, NodeNames, "", DisplayName, Applicant, Note);
                                                MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, FormsID, FormNumber, "Msg", FormName, Content2, false, CanReceiveID);
                                            }
                                            //*****判断是否有自动审批End*****
                                        }
                                        //*****判断是否有自动受理End*****
                                    }
                                    //如果还没有审批完成继续提示等待审批（FormApprovalRecords状态和日志状态已在上面更新）
                                    else
                                    {
                                        if (!MicroWorkFlow.SetFormState(ShortTableName, 0, FormID, FormsID))
                                            flag = "更新表单状态失败。错误代码：113 <br/>The update of the form status failed.";

                                        //*****下一位审批者的信息Start*****
                                        string NextCanReceiveID = string.Empty;

                                        var GetFormApprovalRecordsAttr2 = MicroWorkFlow.GetFormApprovalRecordsAttr(FormID, FormsID);
                                        DataTable _dt4 = GetFormApprovalRecordsAttr2.SourceDT;

                                        //*****下一位审批者的信息End*****

                                        //得到下一位需要审批而又未审批的记录（即审批步骤，如：科长审批，部长审批）
                                        DataRow[] _rows4 = _dt4.Select("CanApprovalUID<>'' and ApprovalUID=0 and StateCode=0 and FixedNode=0", "Sort");
                                        if (_rows4.Length > 0)
                                        {
                                            NextCanReceiveID = _rows4[0]["CanApprovalUID"].toStringTrim(); //得到下一位审批者的信息

                                            //###5还没有审批完成，继续通知下一位审批者。
                                            //例1：xxx提交的《xxx》申请，编号：xxx，已由xxx审批通过，拜托您进行审批。 例2：xxx提交的《xxx》申请，表单编号：xxx，已由xxx审批、xxx审批通过，拜托您进行审批。
                                            //string Content2 = "" + Applicant + "提交的" + FormName + "申请，编号：" + FormNumber + "，已由 " + NodeNames + DisplayName + " 审批通过，拜托您进行审批。";  //Update 20210112
                                            string Content2 = MicroNotice.GetNoticeContent("ApprovalToApproval", ShortTableName, ModuleID, FormID, FormsID, FormNumber, FormName, NodeNames, "", DisplayName, Applicant, Note);
                                            MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, FormsID, FormNumber, "Msg", FormName, Content2, false, NextCanReceiveID);
                                        }
                                    }
                                }//审批同意更新FormApprovalRecords状态
                                else
                                    flag = "更新审批状态失败。错误代码：114<br/>The update approval status failed.";
                            }
                        }
                        else
                            flag = MicroPublic.GetMsg("NoPermApproval"); //"您没有权限审批该表单。<br/>You do not have permission to approve the form.";

                    } //没有审批节点时
                    else
                        flag = MicroPublic.GetMsg("ApprovedFailed"); //"该表单已审批过或不需要审批。<br/>The form has been approved or does not require approval.";

                } //表单属性需要审批时结束

            }
            catch (Exception ex)
            {
                flag = ex.ToString();
            }
            return flag;

        }



        /// <summary>
        /// 驳回申请
        /// </summary>
        /// <param name="FormID"></param>
        /// <param name="ShortTableName"></param>
        /// <param name="FormsID"></param>
        /// <param name="Fields"></param>
        /// <returns></returns>
        public static string SetReturnForm(string ModuleID, string ShortTableName, string FormID, string FormsID, string Fields)
        {
            //审批状态：-40 = 删除[Delete]、-30 = 无效[Invalid]、-4 = 撤回[Withdrawal]、-3 = 填写申请[Fill in]、-2 = 临时保存[Draft]、-1 = 驳回申请[Return]、0 = 等待审批[Waiting]、1 = 审批通过[Pass]、11 = 提交申请[Pass]、15 = 撤回审批[WithdrawalApproval]、18 = 转发[Forward]、22 = 等待受理[WaitingForAccept]、23 = 等待对应[WaitingForProcessing]、24 = 等待处理[WaitingForProcessing]、32 = 受理中[Accepting]、33 = 对应中[Processing]、34 = 处理中[Processing]、100 = 完成[Finish]
            int ApprovalStateCode = -1;

            string flag = MicroPublic.GetMsg("SaveFailed"),
                    FARID = string.Empty,
                    FormNumber = string.Empty,
                    NodeName = string.Empty,
                    UID = MicroUserInfo.GetUserInfo("UID"),
                    DisplayName = MicroUserInfo.GetUserInfo("DisplayName"),
                    ApprovalState = MicroWorkFlow.GetApprovalState(ApprovalStateCode),
                    IP = MicroUserInfo.GetUserInfo("IP");

            var GetFormAttr = MicroForm.GetFormAttr(ShortTableName, FormID);
            Boolean IsApproval = GetFormAttr.IsApproval;
            Boolean AutoClose = GetFormAttr.AutoClose;
            string FormName = GetFormAttr.FormName;

            //表单属性不需要审批时
            if (!IsApproval)
                flag = MicroPublic.GetMsg("NoApprovalRequired"); //"该表单不需要审批。<br/>The form does not require approval.";

            else
            {
                dynamic json = JToken.Parse(Fields) as dynamic;
                string Note = json["txtNote"];  //审批备注
                Note = Note.toJsonTrim();

                var GetFormApprovalRecordsAttr = MicroWorkFlow.GetFormApprovalRecordsAttr(FormID, FormsID);
                DataTable _dt = GetFormApprovalRecordsAttr.SourceDT;

                //*****得到申请者和受理者信息Start*****
                //申请者信息
                string Applicant = string.Empty;
                string ApplicantID = string.Empty;
                //受理者信息
                //string CanReceiveID = string.Empty;
                //string ReceiveID = string.Empty;

                //当FormApprovalRecords表有记录时，以Sort排序的第一条记录作为申请者信息，以最后一条记录作为受理者申请
                //（备注一下作提醒：不管理表单“IsApproval”是否有审批流，都是需要审批的，只是=false时只有申请和完成两步）
                if (_dt.Rows.Count > 0)
                {
                    //第一条记录 (用于驳回时通知申请者)
                    Applicant = _dt.Rows[0]["DisplayName"].toJsonTrim();
                    ApplicantID = _dt.Rows[0]["UID"].toJsonTrim();

                    //最后一条记录（_dt.Rows.Count - 1）
                    //CanReceiveID = _dt.Rows[_dt.Rows.Count - 1]["CanApprovalUID"].toStringTrim();
                    //ReceiveID = _dt.Rows[_dt.Rows.Count - 1]["ApprovalUID"].toStringTrim();
                }
                //*****得到申请者和受理者信息End*****


                DataRow[] _rows = _dt.Select("CanApprovalUID<>'' and ApprovalUID=0 and StateCode=0 and FixedNode=0", "Sort");

                int FormStateCode = MicroWorkFlow.GetFormState(ShortTableName, FormID, FormsID);
                if (FormStateCode == 15)
                {
                    //全局审批权限（系统默认根据CanApprovalUID判断是否有审批权限，这里的权限是指赋予系统权限的审批权限，该权限凌驾于CanApprovalUID之上，代表你不是该表单的审批者也有权限审批）
                    if (MicroAuth.CheckPermit(ModuleID, "12"))
                        _rows = _dt.Select("CanApprovalUID<>'' and StateCode=15 and FixedNode=0", "Sort");

                    else
                        _rows = _dt.Select("CanApprovalUID<>'' and ApprovalUID=" + UID + " and StateCode=15 and FixedNode=0", "Sort");
                }


                //如果有审批记录需要审批时
                if (_rows.Length > 0)
                {
                    //判断是否有权限审批（CanApprovalUID与当前登录用户UID比较）
                    Boolean Perm = false;
                    string iCanApprovalUID = _rows[0]["CanApprovalUID"].toJsonTrim();

                    //有全局的审批权限时
                    if (MicroAuth.CheckPermit(ModuleID, "12"))
                    {
                        //得到FARID，用于更新FormApprovalLogs表
                        FARID = _rows[0]["FARID"].toStringTrim();
                        //得到FormNumber，用于更新FormApprovalLogs表
                        FormNumber = _rows[0]["FormNumber"].toStringTrim();
                        //NodeName，用于更新FormApprovalLogs表
                        NodeName = _rows[0]["NodeName"].toStringTrim();
                        Perm = true;  //赋予审批权限
                    }
                    else
                    {
                        //没有设置的审批权限时，判断能审批的UID是否包含自己的(当前登录用户)UID，如果有可以审批
                        if (MicroPublic.CheckSplitExists(iCanApprovalUID, UID, ','))
                        {
                            //得到FARID，用于更新FormApprovalLogs表
                            FARID = _rows[0]["FARID"].toStringTrim();
                            //得到FormNumber，用于更新FormApprovalLogs表
                            FormNumber = _rows[0]["FormNumber"].toStringTrim();
                            //NodeName，用于更新FormApprovalLogs表
                            NodeName = _rows[0]["NodeName"].toStringTrim();
                            Perm = true;  //赋予审批权限
                        }
                        else
                            Perm = false;
                    }


                    //有审批权限时，更新审批记录
                    if (Perm)
                    {
                        //把审批记录表的记录StateCode为非“0”的记录设置为-1
                        if (MicroWorkFlow.SetFormApprovalRecords(UID, DisplayName, ApprovalStateCode, Note, FARID.toInt()))
                        {
                            flag = MicroPublic.GetMsg("Save");

                            //审批结果更新表成功时，插入logs记录
                            MicroWorkFlow.SetApprovalLogs(FormID, FormsID, FormNumber, NodeName, UID, DisplayName, ApprovalStateCode, Note);

                            if (!MicroWorkFlow.SetFormState(ShortTableName, ApprovalStateCode, FormID, FormsID))
                                flag = "更新表单状态失败。错误代码：115 <br/>The update of the form status failed.";

                            //审批驳回通知申请者。您提交的《xxx》申请，编号：xxx，已被NodeName驳回，原因：Note
                            //string Content6 = "您提交的" + FormName + "申请，编号：" + FormNumber + "，已被" + NodeName + DisplayName + "驳回，原因：" + Note + "";  //Update 20210112
                            string Content6 = MicroNotice.GetNoticeContent("RejectedToApplicant", ShortTableName, ModuleID, FormID, FormsID, FormNumber, FormName, "", NodeName, DisplayName, Applicant, Note);
                            MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, FormsID, FormNumber, "Msg", FormName, Content6, false, ApplicantID);
                        }
                        else
                            flag = "更新审批状态失败。错误代码：116<br/>The update approval status failed.";
                    }
                    else
                        flag = MicroPublic.GetMsg("NoPermApproval"); //"您没有权限审批该表单。<br/>You do not have permission to approve the form.";

                } //有审批节点时
                else
                    flag = MicroPublic.GetMsg("ApprovedFailed"); //"该表单已审批过或不需要审批。<br/>The form has been approved or does not require approval.";
            }
            return flag;
        }



        /// <summary>
        /// 获取上一审批节点的属性（上一审批节点这是相对当前节点且未审批而言的）
        /// </summary>
        public class PreNodeAttr
        {

            public string FARID
            {
                set;
                get;
            }

            public string FormNumber
            {
                set;
                get;
            }

            public string NodeName
            {
                set;
                get;
            }


            /// <summary>
            /// 可审批者UID
            /// </summary>
            public string CanApprovalUID
            {
                set;
                get;
            }

            /// <summary>
            /// 审批者UID
            /// </summary>
            public string ApprovalUID
            {
                set;
                get;
            }

            /// <summary>
            /// 审批者DisplayName
            /// </summary>
            public string ApprovalDisplayName
            {
                set;
                get;
            }

            /// <summary>
            /// 审批代码
            /// </summary>
            public string StateCode
            {
                set;
                get;
            }

            /// <summary>
            /// 审批状态
            /// </summary>
            public string ApprovalState
            {
                set;
                get;
            }

            /// <summary>
            /// 创建者UID(通常是申请者提交时创建)
            /// </summary>
            public string UID
            {
                set;
                get;
            }

            /// <summary>
            /// 创建者DisplayName(通常是申请者提交时创建)
            /// </summary>
            public string DisplayName
            {
                set;
                get;
            }
        }

        /// <summary>
        /// 获取上一审批节点的属性（上一审批节点这是相对当前节点且未审批而言的，比如说当前节点是部长审批（处于等待审批状态），则它的上一审批节点是科长审批）
        /// </summary>
        /// <param name="FormID"></param>
        /// <param name="FormsID"></param>
        /// <returns></returns>
        public static PreNodeAttr GetPreNodeAttr(string FormID, string FormsID)
        {
            string farid = string.Empty, formNumber = string.Empty, nodeName = string.Empty, canApprovalUID = string.Empty, approvalUID = string.Empty, approvalDisplayName = string.Empty, stateCode = string.Empty, approvalState = string.Empty, uid = string.Empty, displayName = string.Empty;

            string _sql = "select * from FormApprovalRecords where Invalid=0 and Del=0 and FormID=@FormID and FormsID=@FormsID " +
                        //得到当前表单已审批的节点的最大值
                        "and FARID = (select max(FARID) from FormApprovalRecords where Invalid=0 and Del=0 and FormID=@FormID and FormsID=@FormsID and CanApprovalUID<>'' and ApprovalUID<>0) ";
            ////同时判断审批者是否为当前登录用户
            //"and CanApprovalUID<>'' and ApprovalUID=@ApprovalUID";

            SqlParameter[] _sp = {
                        new SqlParameter("@FormID",SqlDbType.Int),
                        new SqlParameter("@FormsID",SqlDbType.Int)
                        };

            _sp[0].Value = FormID.toInt();
            _sp[1].Value = FormsID.toInt();


            DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

            if (_dt != null && _dt.Rows.Count > 0)
            {
                farid = _dt.Rows[0]["FARID"].toStringTrim();
                formNumber = _dt.Rows[0]["FormNumber"].toStringTrim();
                nodeName = _dt.Rows[0]["NodeName"].toStringTrim();
                canApprovalUID = _dt.Rows[0]["CanApprovalUID"].toStringTrim();
                approvalUID = _dt.Rows[0]["ApprovalUID"].toStringTrim();
                approvalDisplayName = _dt.Rows[0]["ApprovalDisplayName"].toStringTrim();
                stateCode = _dt.Rows[0]["StateCode"].toStringTrim();
                approvalState = _dt.Rows[0]["ApprovalState"].toStringTrim();
                uid = _dt.Rows[0]["UID"].toStringTrim();
                displayName = _dt.Rows[0]["DisplayName"].toStringTrim();
            }


            var PreNodeAttr = new PreNodeAttr
            {
                FARID = farid,
                FormNumber = formNumber,
                NodeName = nodeName,
                CanApprovalUID = canApprovalUID,
                ApprovalUID = approvalUID,
                ApprovalDisplayName = approvalDisplayName,
                StateCode = stateCode,
                ApprovalState = approvalState,
                UID = uid,
                DisplayName = displayName
            };

            return PreNodeAttr;
        }


        /// <summary>
        /// 获取当前审批节点的属性（这是相对当前节点且未审批而言的）
        /// </summary>
        public class CurrNodeAttr
        {

            public string FARID
            {
                set;
                get;
            }

            public string FormNumber
            {
                set;
                get;
            }

            public string NodeName
            {
                set;
                get;
            }

            /// <summary>
            /// 可审批者UID
            /// </summary>
            public string CanApprovalUID
            {
                set;
                get;
            }

            /// <summary>
            /// 审批者UID
            /// </summary>
            public string ApprovalUID
            {
                set;
                get;
            }

            /// <summary>
            /// 审批者DisplayName
            /// </summary>
            public string ApprovalDisplayName
            {
                set;
                get;
            }

            /// <summary>
            /// 审批代码
            /// </summary>
            public string StateCode
            {
                set;
                get;
            }

            /// <summary>
            /// 审批状态
            /// </summary>
            public string ApprovalState
            {
                set;
                get;
            }

            /// <summary>
            /// 创建者UID(通常是申请者提交时创建)
            /// </summary>
            public string UID
            {
                set;
                get;
            }

            /// <summary>
            /// 创建者DisplayName(通常是申请者提交时创建)
            /// </summary>
            public string DisplayName
            {
                set;
                get;
            }
        }

        /// <summary>
        /// 获取当前审批节点的属性（这是相对当前节点且未审批而言的）
        /// </summary>
        /// <param name="FormID"></param>
        /// <param name="FormsID"></param>
        /// <returns></returns>
        public static CurrNodeAttr GetCurrNodeAttr(string FormID, string FormsID)
        {
            string farid = string.Empty, formNumber = string.Empty, nodeName = string.Empty, canApprovalUID = string.Empty, approvalUID = string.Empty, approvalDisplayName = string.Empty, stateCode = string.Empty, approvalState = string.Empty, uid = string.Empty, displayName = string.Empty;

            var GetFormApprovalRecordsAttr = MicroWorkFlow.GetFormApprovalRecordsAttr(FormID, FormsID);
            DataTable _dt = GetFormApprovalRecordsAttr.SourceDT;
            DataRow[] _rows = _dt.Select("CanApprovalUID<>'' and ApprovalUID=0 and StateCode=0", "Sort");  //考虑 and FixedNode=0


            if (_rows.Length > 0)
            {
                farid = _rows[0]["FARID"].toStringTrim();
                formNumber = _dt.Rows[0]["FormNumber"].toStringTrim();
                nodeName = _dt.Rows[0]["NodeName"].toStringTrim();
                canApprovalUID = _rows[0]["CanApprovalUID"].toStringTrim();
                approvalUID = _rows[0]["ApprovalUID"].toStringTrim();
                approvalDisplayName = _rows[0]["ApprovalDisplayName"].toStringTrim();
                stateCode = _rows[0]["StateCode"].toStringTrim();
                approvalState = _rows[0]["ApprovalState"].toStringTrim();
                uid = _rows[0]["UID"].toStringTrim();
                displayName = _rows[0]["DisplayName"].toStringTrim();
            }


            var CurrNodeAttr = new CurrNodeAttr
            {
                FARID = farid,
                FormNumber = formNumber,
                NodeName = nodeName,
                CanApprovalUID = canApprovalUID,
                ApprovalUID = approvalUID,
                ApprovalDisplayName = approvalDisplayName,
                StateCode = stateCode,
                ApprovalState = approvalState,
                UID = uid,
                DisplayName = displayName

            };

            return CurrNodeAttr;
        }


        /// <summary>
        /// 检查撤回情况，主要用于申请者撤回时（审批者撤回只回到上一状态所以不应用），同一个表同一个人不允许同时撤回多张单，撤回后必须处理完后才允许撤回另一张单
        /// </summary>
        /// <param name="Action"></param>
        /// <param name="ShortTableName"></param>
        /// <param name="ModuleID"></param>
        /// <param name="FormID"></param>
        /// <param name="FormsID"></param>
        /// <returns></returns>
        public static string CheckWithdrawal(string Action, string ShortTableName, string ModuleID, string FormID, string FormsID)
        {
            string flag = "True",
                  TableName = MicroPublic.GetTableName(ShortTableName),
                  DisplayNameField = "DisplayName",
                  FormNumbers = string.Empty,
                  DisplayNames = string.Empty;

            var getTableAttr = MicroDataTable.GetTableAttr(TableName);  //调用方法返回主键属性
            string PrimaryKeyName = getTableAttr.PrimaryKeyName;  //赋值，主键字段名称

            string _sql = " select * from " + TableName + " where Invalid=0 and Del=0 and StateCode=-4 and FormID=@FormID and UID in (select UID from " + TableName + " where Invalid=0 and Del=0 and  " + PrimaryKeyName + "=@FormsID)";

            if (ShortTableName == "Overtime")
            {
                _sql = "select * from " + TableName + " where Invalid=0 and Del=0 and StateCode=-4 and FormID=@FormID and OvertimeUID in (select OvertimeUID from " + TableName + " where Invalid=0 and Del=0 and ParentID=@FormsID)";

                DisplayNameField = "OvertimeDisplayName";
            }

            else if (ShortTableName == "Leave")
            {
                _sql = "select * from " + TableName + " where Invalid=0 and Del=0 and StateCode=-4 and FormID=@FormID and LeaveUID in (select LeaveUID from " + TableName + " where Invalid=0 and Del=0 and " + PrimaryKeyName + "=@FormsID)";

                DisplayNameField = "LeaveDisplayName";
            }

            SqlParameter[] _sp = { new SqlParameter("@FormID", SqlDbType.Int),
                                   new SqlParameter("@FormsID", SqlDbType.Int),};

            _sp[0].Value = FormID.toInt();
            _sp[1].Value = FormsID.toInt();

            DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

            if (_dt != null && _dt.Rows.Count > 0)
            {
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    string DisplayName = string.Empty;
                    if (MsSQLDbHelper.ColumnExists(TableName, "DisplayName"))
                        DisplayName = " 申请者：" + _dt.Rows[i]["DisplayName"].toStringTrim();

                    FormNumbers += _dt.Rows[i]["FormNumber"].toStringTrim() + DisplayName + "、";
                    DisplayNames += _dt.Rows[i][DisplayNameField].toStringTrim() + "、";
                }

                FormNumbers = MicroPublic.GetDistinct(FormNumbers, '、');
                FormNumbers = FormNumbers.Substring(0, FormNumbers.Length - 1);

                DisplayNames = MicroPublic.GetDistinct(DisplayNames, '、');
                DisplayNames = DisplayNames.Substring(0, DisplayNames.Length - 1);

                flag = "撤回失败：" + DisplayNames + " 已经有撤回的申请还没有处理完毕，<br/>相关单号：" + FormNumbers + "，<br/>处理完毕后才能继续执行撤回，谢谢！";
            }

            return flag;

        }

    }
}