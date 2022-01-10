using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroDTHelper;
using MicroPublicHelper;
using MicroUserHelper;
using Newtonsoft.Json.Linq;
using MicroFormHelper;
using MicroNoticeHelper;

namespace MicroWorkFlowHelper
{
    /// <summary>
    /// MicroWorkFlowHelper 的摘要说明
    /// </summary>
    public class MicroWorkFlow
    {

        /// <summary>
        /// 获取流程ID(主要作用是判断当前申请表单应该走什么流程，如按生效范围：指定部门、指定职位、指定角色、指定人员生效，有记录则返回Sort第一条。没有设定生效范围则返回未设置生效范围的以)
        /// </summary>
        /// <param name="Action"></param>
        /// <param name="FormID"></param>
        /// <param name="FormsID"></param>
        /// <param name="ShortTableName"></param>
        /// <returns></returns>
        public static int GetWorkFlowID(DataTable _dtWorkFlow, string Action, string FormID, string FormsID, string ShortTableName)
        {

            int WorkFlowID = 0;

            //通常FormID不为空且FormsID不为空时如果能从FormApprovalRecords表中找到记录，说明该表单是有提交过的（即现在属于修改状态下发生的情况）
            //如果能找到记录且WorkFlowID!=0，则把WorkFlowID返回
            if (!string.IsNullOrEmpty(FormID) && !string.IsNullOrEmpty(FormsID))
            {
                string _sql2 = "select top 1 ParentID from WorkFlow where Invalid=0 and del=0 and WFID in( select WorkFlowID from FormApprovalRecords where Invalid = 0 and del = 0 and FormID = @FormID and FormsID = @FormsID )";
                SqlParameter[] _sp2 = { new SqlParameter("@FormID",SqlDbType.Int),
                                       new SqlParameter("@FormsID",SqlDbType.Int),
                                    };

                _sp2[0].Value = FormID.toInt();
                _sp2[1].Value = FormsID.toInt();

                DataTable _dt2 = MsSQLDbHelper.Query(_sql2, _sp2).Tables[0];

                if (_dt2 != null && _dt2.Rows.Count > 0)
                {
                    int ParentID = _dt2.Rows[0]["ParentID"].toInt();
                    if (ParentID != 0)
                        WorkFlowID = ParentID;
                }
            }

            //如果上述无法找到记录（即WorkFlowID保持为0），且_dtWorkFlow表记录>0时继续执行以下代码
            //通常为新增情况下发生
            if (WorkFlowID == 0 && _dtWorkFlow.Rows.Count > 0)
            {
                //生效范围不为空且设有默认记录时，以默认记录Sort字段的先后顺序取第一条
                DataRow[] _rowsFlow = _dtWorkFlow.Select("ParentID=0 and DefaultFlow=1 and EffectiveType<>'' and EffectiveIDStr<>'' and FormID=" + FormID, "Sort");

                //以默认记录Sort字段的先后顺序取第一条
                if (_rowsFlow.Length > 0)
                    WorkFlowID = _rowsFlow[0]["WFID"].toInt();

                //否则没有默认记录时、按用户、角色、职位、部门的先后顺序进行寻找记录 (_rowsFlow.Length == 0)
                else
                {

                    //按用户、角色、职位、部门的先后顺序进行寻找记录
                    DataRow[] _rowsUseFlow = _dtWorkFlow.Select("ParentID=0 and EffectiveType='Use' and EffectiveIDStr<>'' and FormID=" + FormID, "Sort");
                    DataRow[] _rowsRoleFlow = _dtWorkFlow.Select("ParentID=0 and EffectiveType='Role' and EffectiveIDStr<>'' and FormID=" + FormID, "Sort");
                    DataRow[] _rowsJTitleFlow = _dtWorkFlow.Select("ParentID=0 and EffectiveType='JTitle' and EffectiveIDStr<>'' and FormID=" + FormID, "Sort");
                    DataRow[] _rowsDeptFlow = _dtWorkFlow.Select("ParentID=0 and EffectiveType='Dept' and EffectiveIDStr<>'' and FormID=" + FormID, "Sort");

                    Boolean isTrue = false;

                    //按用户
                    if (_rowsUseFlow.Length > 0 && !isTrue)
                    {
                        foreach (DataRow _drFlow in _rowsUseFlow)
                        {
                            if (!isTrue)
                            {
                                string EffectiveIDStr = _drFlow["EffectiveIDStr"].toStringTrim();
                                isTrue = MicroUserInfo.CheckUID(EffectiveIDStr);

                                //一旦有记录，取出值并中止循环
                                if (isTrue)
                                    WorkFlowID = _drFlow["WFID"].toInt();
                            }
                        }
                    }

                    //按角色
                    if (_rowsRoleFlow.Length > 0 && !isTrue)
                    {
                        foreach (DataRow _drFlow in _rowsRoleFlow)
                        {
                            if (!isTrue)
                            {
                                string EffectiveIDStr = _drFlow["EffectiveIDStr"].toStringTrim();
                                isTrue = MicroUserInfo.CheckUserRoles(EffectiveIDStr);

                                //一旦有记录，取出值并中止循环
                                if (isTrue)
                                    WorkFlowID = _drFlow["WFID"].toInt();
                            }
                        }
                    }

                    //按职位
                    if (_rowsJTitleFlow.Length > 0 && !isTrue)
                    {
                        foreach (DataRow _drFlow in _rowsJTitleFlow)
                        {
                            if (!isTrue)
                            {
                                string EffectiveIDStr = _drFlow["EffectiveIDStr"].toStringTrim();
                                isTrue = MicroUserInfo.CheckUserJobTitle(EffectiveIDStr);

                                //一旦有记录，取出值并中止循环
                                if (isTrue)
                                    WorkFlowID = _drFlow["WFID"].toInt();
                            }
                        }
                    }

                    //按部门
                    if (_rowsDeptFlow.Length > 0 && !isTrue)
                    {
                        foreach (DataRow _drFlow in _rowsDeptFlow)
                        {
                            if (!isTrue)
                            {
                                string EffectiveIDStr = _drFlow["EffectiveIDStr"].toStringTrim();
                                isTrue = MicroUserInfo.CheckUserDepts(EffectiveIDStr);

                                //一旦有记录，取出值并中止循环
                                if (isTrue)
                                    WorkFlowID = _drFlow["WFID"].toInt();
                            }
                        }
                    }

                }

                //如果按上面的EffectiveType<>'' and EffectiveIDStr<>''不等于空的情况下无法匹配记录，则去除这两个条件再进行匹配
                if (WorkFlowID == 0)
                {
                    //否则生效范围为空时，不带生效范围进行查询，同时有设置默认记录时
                    DataRow[] _rowsFlow2 = _dtWorkFlow.Select("ParentID=0 and DefaultFlow=1 and FormID=" + FormID, "Sort");
                    //生效范围为空时，不带生效范围进行查询，且“没有设置”默认记录时
                    if (_rowsFlow2.Length == 0)
                        _rowsFlow2 = _dtWorkFlow.Select("ParentID=0 and FormID=" + FormID, "Sort");

                    if (_rowsFlow2.Length > 0)
                        WorkFlowID = _rowsFlow2[0]["WFID"].toInt();
                }
            }

            return WorkFlowID;
        }

        /// <summary>
        /// 获取表单流程属性(运算条件ParentID=0)，如：IsAccept、WorkFlowID
        /// </summary>
        public class FormWorkFlowAttr
        {
            /// <summary>
            /// 有审批流程
            /// </summary>
            public Boolean IsWorkFlow
            {
                set;
                get;
            }

            /// <summary>
            /// WorkFlow源表数据
            /// </summary>
            public DataTable SourceDT
            {
                set;
                get;
            }

            /// <summary>
            /// 启用受理
            /// </summary>
            public Boolean IsAccept
            {
                set;
                get;
            }

            /// <summary>
            /// WorkFlowID
            /// </summary>
            public string WorkFlowID
            {
                set;
                get;
            }


        }

        /// <summary>
        /// 获取表单流程属性(运算条件ParentID=0)，如：IsAccept、WorkFlowID
        /// </summary>
        /// <param name="FormID"></param>
        /// <returns></returns>
        public static FormWorkFlowAttr GetFormWorkFlowAttr(string FormID, string FormsID = "")
        {
            Boolean isWorkFlow = false, isAccept = false;
            string workFlowID = string.Empty;

            DataTable _dt = MicroDataTable.GetSingleDataTable("WorkFlow");

            int WorkFlowID = GetWorkFlowID(_dt, "", FormID, FormsID, "");

            DataRow[] _rowsFlow = _dt.Select("ParentID=0 and WFID=" + WorkFlowID, "Sort");

            if (_rowsFlow.Length > 0)
            {
                isWorkFlow = true;
                isAccept = _rowsFlow[0]["IsAccept"].toBoolean();
                workFlowID = _rowsFlow[0]["WFID"].toStringTrim();
            }

            var FormWorkFlowAttr = new FormWorkFlowAttr
            {
                IsWorkFlow = isWorkFlow,
                SourceDT = _dt,
                IsAccept = isAccept,
                WorkFlowID = workFlowID
            };

            return FormWorkFlowAttr;

        }

        public class WorkFlowAttr
        {
            /// <summary>
            /// XmSelect控件的html代码
            /// </summary>
            public string HtmlCode
            {
                set;
                get;
            }

            /// <summary>
            /// XmSelect控件的js代码
            /// </summary>
            public string JsXmSelectCode
            {
                set;
                get;
            }

            /// <summary>
            /// XmSelect控件的数据源
            /// </summary>
            public string JsXmSelectData
            {
                set;
                get;
            }

            /// <summary>
            /// 判断表单是否启用审批
            /// </summary>
            public Boolean IsApproval
            {
                set;
                get;
            }

            /// <summary>
            /// 得到需要审批节点ID组成的字符串，用逗号分隔（不含固定步骤的ID，如：第一步“申请”和最后一步“完成”）
            /// </summary>
            public string ApprovalNodeIDs
            {
                set;
                get;
            }
        }


        /// <summary>
        /// 获得表单审批流程，传入表单ID，查询表单流程表生成审批流程显示出审批人员
        /// </summary>
        /// <param name="Action"></param>
        /// <param name="FormID"></param>
        /// <param name="FormsID"></param>
        /// <param name="ShortTableName"></param>
        /// <param name="OperFieldConditionOperValue"></param>
        /// <returns></returns>
        public static WorkFlowAttr GetWorkFlow(string Action, string FormID, string FormsID, string ShortTableName, string OperFieldConditionOperValue = "")
        {
            string flag = string.Empty;
            string htmlCode = string.Empty, jsXmSelectCode = string.Empty, jsXmSelectData = string.Empty, ApprovalTips = string.Empty, approvalNodeIDs = string.Empty;
            Boolean isApproval = false; //由表单表Forms得出where FormID=?;
            //Boolean IsWorkFlow = false; //是否有审批流程
            htmlCode = "<fieldset class=\"layui-elem-field layui-field-title\"><legend class=\"ws-font-green ws-font-size16\">审批信息</legend></fieldset>";

            Action = string.IsNullOrEmpty(Action) == true ? "" : Action.ToLower();

            try
            {
                DataTable _dtForms = MicroDataTable.GetSingleDataTable("Forms");
                DataRow[] _rowsForms = _dtForms.Select("FormID=" + FormID.toInt());

                if (_rowsForms.Length > 0)
                {
                    isApproval = _rowsForms[0]["IsApproval"].toBoolean();
                    ApprovalTips = _rowsForms[0]["ApprovalTips"].toJsonTrim();
                }

                if (isApproval)
                {
                    DataTable _dt = MicroDataTable.GetSingleDataTable("WorkFlow");

                    if (_dt.Rows.Count > 0)
                    {
                        int WorkFlowID = GetWorkFlowID(_dt, Action, FormID, FormsID, ShortTableName);
                        if (WorkFlowID > 0)
                        {
                            var getFlowNode = GetFlowNode(_dt, WorkFlowID.ToString(), Action, FormID, FormsID, OperFieldConditionOperValue);
                            htmlCode += getFlowNode.HtmlCode;
                            jsXmSelectCode = getFlowNode.JsXmSelectCode;
                            jsXmSelectData = getFlowNode.JsXmSelectData;
                            approvalNodeIDs = getFlowNode.ApprovalNodeIDs;
                        }
                        else  //如果没有设定审批流程时提示信息
                            flag = "<input type=\"hidden\" name=\"txtNoSetFlow\" id=\"txtNoSetFlow\" lay-verify=\"required\" lay-reqText=\"该表单还没有设置审批流程，请联系IT进行设置"+WorkFlowID.ToString()+"<br/>The approval process has not been set for this form, please contact it to set it\">";
                    }

                }

                //在表单末尾显示审批流程提示信息，如：申请-》科长审批-》部长审批-》IT存档 or 该表单不需要审批。The form does not require approval.
                htmlCode += "<div class=\"layui-form-item ws-margin-top30\">";
                htmlCode += flag;
                htmlCode += "<blockquote class=\"layui-elem-quote ws-quote-nm\"><span class=\"ws-font-red\">" + ApprovalTips + "</span></blockquote>";
                htmlCode += "</div>";


            }
            catch (Exception ex) { htmlCode = ex.ToString(); }

            var WorkFlowAttr = new WorkFlowAttr
            {
                HtmlCode = htmlCode,
                JsXmSelectCode = jsXmSelectCode,
                JsXmSelectData = jsXmSelectData,
                IsApproval = isApproval,
                ApprovalNodeIDs = approvalNodeIDs
            };

            return WorkFlowAttr;
        }


        /// <summary>
        /// 获得审批流程节点
        /// </summary>
        /// <param name="_dt"></param>
        /// <param name="WorkFlowID">流程名称所属记录WorkFlowID</param>
        /// <param name="Action"></param>
        /// <param name="FormID">表单ID</param>
        /// <param name="FormsID">表单记录ID</param>
        /// <param name="OperFieldConditionOperValue">（操作字段匹配值：操作条件：操作值）用；分号分隔的数组格式 OperField1:Condition1:OperValue1；OperField2:Condition2:OperValue2,...</param>
        /// <returns></returns>
        public static WorkFlowAttr GetFlowNode(DataTable _dt, string WorkFlowID, string Action, string FormID, string FormsID, string OperFieldConditionOperValue = "")
        {
            string htmlCode = string.Empty, jsXmSelectCode = string.Empty, jsXmSelectData = string.Empty, approvalNodeIDs = string.Empty;

            DataRow[] _rows = _dt.Select("Invalid=0 and Del=0 and FixedNode=0 and IsConditionApproval=0 and ParentID=" + WorkFlowID, "Sort");  //FixedNode=0，得到需要审批的节点

            //************条件审批Start************
            if (!string.IsNullOrEmpty(OperFieldConditionOperValue))
            {
                string WorkFlowIDs = "0";
                string[] arrOperFieldConditionOperValue = OperFieldConditionOperValue.Split(';');
                if (arrOperFieldConditionOperValue.Length > 0)
                {
                    for (int i = 0; i < arrOperFieldConditionOperValue.Length; i++)
                    {
                        string OperField = arrOperFieldConditionOperValue[i].Split(':')[0];
                        string Condition = arrOperFieldConditionOperValue[i].Split(':')[1];
                        string OperValue = arrOperFieldConditionOperValue[i].Split(':')[2];

                        Condition = Condition.ToLower();
                        //构造sql语句
                        string __sql = "";
                        switch (Condition)
                        {
                            case "charindex":
                                __sql = "select * from WorkFlow where Invalid=0 and Del=0 and ParentID=" + WorkFlowID + " and IsConditionApproval=1  and OperField ='" + OperField + "' and CHARINDEX(',' + convert(varchar," + OperValue + ") + ',',',' + OperValue + ',')> 0 ";
                                break;
                            case "in":
                                __sql = "select * from WorkFlow where Invalid=0 and Del=0 and ParentID=" + WorkFlowID + " and IsConditionApproval=1  and OperField ='" + OperField + "' and OperValue in ('" + OperValue + "')";
                                break;
                            default:
                                __sql = "select * from WorkFlow where Invalid=0 and Del=0 and ParentID=" + WorkFlowID + " and IsConditionApproval=1  and OperField ='" + OperField + "' and OperValue " + Condition + "'" + OperValue + "'";
                                break;

                        }

                        //执行__sql查询命令
                        if (!string.IsNullOrEmpty(__sql))
                        {
                            DataTable __dt = MsSQLDbHelper.Query(__sql).Tables[0];
                            if (__dt.Rows.Count > 0)
                            {
                                for (int j = 0; j < __dt.Rows.Count; j++)
                                {
                                    WorkFlowIDs += "," + __dt.Rows[j]["WFID"].ToString();
                                }
                            }
                        }
                       
                    }

                    _rows = _dt.Select("FixedNode=0 and IsConditionApproval=0 and ParentID=" + WorkFlowID + " or (WFID in(" + WorkFlowIDs + "))", "Sort");  //FixedNode=0，得到需要审批的节点
                }

            }

            //************条件审批End**************

            if (_rows.Length > 0)
            {
                DataTable _dt2 = null;
                //在修改表单的状态下，从FormApprovalRecords表中读取出CanApprovalUID，让审批节点的审批人员选中（即上次提交表单选中的审批人员）
                if (Action == "modify")
                {
                    string _sql2 = "select * from FormApprovalRecords where Invalid=0 and Del=0 and FormID=@FormID and FormsID=@FormsID";
                    SqlParameter[] _sp2 = {
                                          new SqlParameter("@FormID", SqlDbType.Int),
                                          new SqlParameter("@FormsID" ,SqlDbType.Int)
                                    };

                    _sp2[0].Value = FormID.toInt();
                    _sp2[1].Value = FormsID.toInt();

                    _dt2 = MsSQLDbHelper.Query(_sql2, _sp2).Tables[0];
                }

                //******读取用户休假记录Start******
                //获取当前时间段所有休假者，在生成审批者时显示休假中（在这里统一读取记录，避免多次循环读取数据库）
                string _sqlUsersLeave = "select LeaveUID from HRLeave where Invalid=0 and Del=0 and StateCode=100 and GetDate() between StartDateTime and EndDateTime";
                DataTable _dtUsersLeave = MsSQLDbHelper.Query(_sqlUsersLeave).Tables[0];
                //******读取用户休假记录End******

                //******读取用户状态记录Start******
                string _sqlUsersState = "select UID,StateName from UserState where Invalid=0 and Del=0 and StateCode<>100 and DurationTime >= GetDate() ";
                DataTable _dtUsersState = MsSQLDbHelper.Query(_sqlUsersState).Tables[0];
                //******读取用户状态记录End******

                foreach (DataRow _dr in _rows)
                {
                    string WFID = _dr["WFID"].toStringTrim();
                    WFID = string.IsNullOrEmpty(WFID) == true ? "0" : WFID;

                    string FlowName = _dr["FlowName"].toStringTrim();

                    string ApprovalType = _dr["ApprovalType"].toStringTrim();

                    string ApprovalIDStr = _dr["ApprovalIDStr"].toStringTrim();
                    ApprovalIDStr = string.IsNullOrEmpty(ApprovalIDStr) == true ? "0" : ApprovalIDStr;

                    string ApprovalByIDStr = _dr["ApprovalByIDStr"].toStringTrim();
                    ApprovalByIDStr = string.IsNullOrEmpty(ApprovalByIDStr) == true ? "0" : ApprovalByIDStr;

                    string Description= _dr["Description"].toStringTrim();

                    string CanApprovalUID = "0";

                    Boolean ApprovalIDStrSort = _dr["ApprovalIDStrSort"].toBoolean();  //主要审批者默认选中顺序，升序（由职位低到高）或降序（由职位高到低）

                    Boolean ApprovalByIDStrSort = _dr["ApprovalByIDStrSort"].toBoolean();   //代理审批者默认选中顺序，升序（由职位低到高）或降序（由职位高到低）

                    Boolean ApproversSelectedByDefault = _dr["ApproversSelectedByDefault"].toBoolean();  //填写申请表单时默认选中审批者

                    Boolean IsOptionalApproval = _dr["IsOptionalApproval"].toBoolean();  //是否显示可选审批

                    Boolean IsVerticalDirection = _dr["IsVerticalDirection"].toBoolean();  //寻找审批者的方向，默认纵向

                    Boolean FindRange = _dr["FindRange"].toBoolean();  //寻找审批者的范围，自部门=false、跨部门=true

                    Boolean FindGMOffice = _dr["FindGMOffice"].toBoolean();  //查找审批者范围追加总经理办公室，会在DeptsID加上总经理办公室的ID

                    Boolean IsSpecialApproval = _dr["IsSpecialApproval"].toBoolean();  //是否为特殊审批，特殊审批如访问对象部门审批在未填写内容时无确认对象是谁，此时应该设置显示某些职位作为审批者（如科长、部长，机密委员长、IT委员长等），默认值【D】:false


                    //在修改表单的状态下，从FormApprovalRecords表中读取出CanApprovalUID，让审批节点的审批人员选中（即上次提交表单选中的审批人员）
                    if (Action == "modify")
                    {
                        if (_dt2 != null && _dt2.Rows.Count > 0)
                        {
                            DataRow[] _rows2 = _dt2.Select("WorkFlowID=" + WFID.toInt());
                            if (_rows2.Length > 0)
                                CanApprovalUID = _rows2[0]["CanApprovalUID"].toJsonTrim();
                        }
                    }
                    //返回XmSelect所需的Data数据
                    jsXmSelectData = GetUsers(FlowName, ApprovalType, ApprovalIDStr, ApprovalByIDStr, Action, CanApprovalUID, ApprovalIDStrSort, ApprovalByIDStrSort, ApproversSelectedByDefault, IsOptionalApproval, IsVerticalDirection, FindRange, FindGMOffice, IsSpecialApproval, _dtUsersLeave, _dtUsersState);

                    htmlCode += "<div class=\"layui-form-item layui-row\">";
                    htmlCode += "<div class=\"layui-col-xs12 layui-col-sm12 layui-col-md12 layui-col-lg12\"> ";
                    htmlCode += "<label class=\"layui-form-label\"><i class=\"ws-must-asterisk\">&#42; </i>" + FlowName + "</label>";
                    htmlCode += "<div class=\"layui-input-inline ws-width320\"> ";
                    htmlCode += "<div id=\"XmSelectFlow" + WFID + "\"></div>";
                    htmlCode += "</div>";
                    htmlCode += "<div class=\"layui-form-mid layui-word-aux\">" + Description + "</div>";
                    htmlCode += "</div>";
                    htmlCode += "</div>";

                    jsXmSelectCode += "var microXmSelect" + WFID + " = xmSelect.render({el: '#XmSelectFlow" + WFID + "', name: 'ApprovalNode" + WFID + "', filterable: true, searchTips: 'Search',  radio: true, clickClose: true, layVerify: 'required', height: '300px', template({ item, sels, name, value }){return item.showname  + '<span style=\"color:red;\">' + item.userstate + '</span>'}, data:" + jsXmSelectData + "});";
                }
            }

            var WorkFlowAttr = new WorkFlowAttr
            {
                HtmlCode = htmlCode,
                JsXmSelectCode = jsXmSelectCode,
                JsXmSelectData = jsXmSelectData,
                ApprovalNodeIDs = approvalNodeIDs
            };

            return WorkFlowAttr;
        }

        /// <summary>
        /// 获得审批流程审批人员
        /// </summary>
        /// <param name="FlowName"></param>
        /// <param name="ApprovalType"></param>
        /// <param name="ApprovalIDStr"></param>
        /// <param name="ApprovalByIDStr"></param>
        /// <returns></returns>
        public static string GetUsers(string FlowName, string ApprovalType, string ApprovalIDStr, string ApprovalByIDStr, string Action, string CanApprovalUID, Boolean ApprovalIDStrSort, Boolean ApprovalByIDStrSort, Boolean ApproversSelectedByDefault, Boolean IsOptionalApproval, Boolean IsVerticalDirection, Boolean FindRange, Boolean FindGMOffice, Boolean IsSpecialApproval, DataTable _dtUsersLeave, DataTable _dtUsersState)
        {
            string flag = string.Empty, str = string.Empty, str2 = string.Empty, str3 = string.Empty;
            string UID = MicroUserInfo.GetUserInfo("UID"); //当前登录用户ID
            string DeptsID = MicroUserInfo.GetUserInfo(FindRange ? "ParentSubDeptsID" : "ParentDeptsID"); //FindRange=true时查找所有父部门和子部门ParentSubDeptsID、否则FindRange=false自部门内ParentDeptsID //返回当前登录用户的所属部门及所有父级部门组成的字符串

            string JTIDs = ApprovalIDStr;
            if (!string.IsNullOrEmpty(ApprovalByIDStr))
                JTIDs = ApprovalIDStr + "," + ApprovalByIDStr;  //得到当前职称的ID汇总

            if (FindGMOffice)
            {
                string GMDeptsID = MicroUserInfo.GetGMDepts();  //总经理办公室部门ID
                DeptsID = DeptsID + "," + GMDeptsID;
            }

            //JTitle,Role,DeptUsers
            switch (ApprovalType)
            {
                case "JTitle":

                    //非特殊审批时
                    if (!IsSpecialApproval)
                    {
                        #region 获取主要审批者
                        //***********得到自部或跨部门的主要审批者Start*************
                        string _sql = "select * from UserInfo where Invalid=0 and Del=0 and UID in (select UID from UserJobTitle where Invalid=0 and Del=0 and JTID in (" + ApprovalIDStr + ")) and UID in (select UID from UserDepts where Invalid=0 and Del=0 and DeptID in (" + DeptsID + "))";
                        DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];

                        Boolean isOnlyOne = true;  //用于控制只选中一个
                        string _str = string.Empty,
                            UIDs1 = "0";

                        if (_dt != null && _dt.Rows.Count > 0)
                        {
                            int MainApproverUID = GetMainApproveUID(ApprovalIDStr, FindRange, FindGMOffice, ApprovalIDStrSort, _dtUsersLeave);

                            DataRow[] _rows = _dt.Select("", "EnglishName");
                            foreach (DataRow _dr in _rows)
                            {

                                string DisplayName = string.Empty, UserName = string.Empty, ChineseName = string.Empty, EnglishName = string.Empty, AdDisplayName = string.Empty, UserState = string.Empty;
                                int _UID = _dr["UID"].toInt();
                                UIDs1 += _UID.toJsonTrim() + ",";

                                //判断用户是否休假
                                Boolean isState = false;
                                DataRow[] _drUsersLeave = _dtUsersLeave.Select("LeaveUID=" + _UID);
                                if (_drUsersLeave.Length > 0)
                                {
                                    UserState = "【休假中】";
                                    isState = true;
                                }
                                //否则判断用户状态（会议、外出等）
                                else
                                {
                                    DataRow[] _drUsersState = _dtUsersState.Select("UID=" + _UID);
                                    if (_drUsersState.Length > 0)
                                    {
                                        UserState = "【" + _drUsersState[0]["StateName"].toStringTrim() + "】";
                                        isState = true;
                                    }
                                }

                                UserName = _dr["UserName"].toJsonTrim();
                                ChineseName = _dr["ChineseName"].toJsonTrim();
                                EnglishName = _dr["EnglishName"].toJsonTrim();
                                AdDisplayName = _dr["AdDisplayName"].toJsonTrim();

                                DisplayName = MicroUserInfo.GetUserInfo(UserName, ChineseName, EnglishName, AdDisplayName);

                                string selected = string.Empty;
                                //表单处于添加（申请）动作时并且默认选中审批者为True时，默认选中审批者，并且没有处于休假状态isLeave
                                if (Action == "add" && ApproversSelectedByDefault && isOnlyOne && !isState && MainApproverUID != 0 && MainApproverUID == _UID)
                                {
                                    selected = "selected: true";
                                    isOnlyOne = false;  //只选中第一条记录，后马上设置为False
                                }


                                //***在修改表单的情况下，取得提交表单时选中的审批者，让审批控件选中这些审批者Start***
                                if (Action == "modify")
                                {
                                    string[] uidArray = CanApprovalUID.Split(',');
                                    for (int i = 0; i < uidArray.Length; i++)
                                    {
                                        if (uidArray[i].toInt() == _UID)
                                            selected = "selected: true";
                                    }
                                }
                                //***在修改表单的情况下，取得提交表单时选中的审批者，让审批控件选中这些审批者End***

                                _str += "{name: '" + DisplayName + UserState + "' ,showname: '" + DisplayName + "', userstate:'" + UserState + "' ,value: " + _UID + ", " + selected + "},";
                            }
                            UIDs1 = UIDs1.Substring(0, UIDs1.Length - 1);

                        }
                        else
                            _str += "{name: '还没有审批成员，请联系IT设定' ,showname: '还没有审批成员，请联系IT设定' ,userleave:'' , children:[]},";

                        //string str = "{name: '" + FlowName + " - 主要审批者【自部门】', children: [" + _str + "]},";
                        str = "{name: '主要审批者', children: [" + _str + "]},";
                        //***********得到自部或跨部门的主要审批者End*************
                        #endregion


                        #region 获取代理审批者
                        //*************得到自部或跨部门的代理审批者Start*************
                        string UIDs2 = "0";

                        if (ApprovalByIDStr.Length > 0)
                        {
                            var getByApprovalUser = GetByApprovalUser(Action, ApprovalByIDStr, DeptsID, UIDs1, IsVerticalDirection, FindRange);  //此时传的主要审批者UIDs1进行过滤
                            DataTable _dt2 = getByApprovalUser.DTByApprovalUser;

                            string _str2 = string.Empty;
                            if (_dt2 != null && _dt2.Rows.Count > 0)
                            {

                                int MainApproverUID = GetMainApproveUID(ApprovalByIDStr, FindRange, FindGMOffice, ApprovalByIDStrSort, _dtUsersLeave);

                                DataRow[] _rows2 = _dt2.Select("", "EnglishName");
                                foreach (DataRow _dr2 in _rows2)
                                {
                                    string DisplayName = string.Empty, UserName = string.Empty, ChineseName = string.Empty, EnglishName = string.Empty, AdDisplayName = string.Empty, UserState = string.Empty;
                                    int _UID = _dr2["UID"].toInt();
                                    UIDs2 += _UID.toJsonTrim() + ",";

                                    //判断用户是否休假
                                    Boolean isState = false;
                                    DataRow[] _drUsersLeave = _dtUsersLeave.Select("LeaveUID=" + _UID);
                                    if (_drUsersLeave.Length > 0)
                                    {
                                        UserState = "【休假中】";
                                        isState = true;
                                    }
                                    //否则判断用户状态（会议、外出等）
                                    else
                                    {
                                        DataRow[] _drUsersState = _dtUsersState.Select("UID=" + _UID);
                                        if (_drUsersState.Length > 0)
                                        {
                                            UserState = "【" + _drUsersState[0]["StateName"].toStringTrim() + "】";
                                            isState = true;
                                        }
                                    }

                                    UserName = _dr2["UserName"].toJsonTrim();
                                    ChineseName = _dr2["ChineseName"].toJsonTrim();
                                    EnglishName = _dr2["EnglishName"].toJsonTrim();
                                    AdDisplayName = _dr2["AdDisplayName"].toJsonTrim();

                                    DisplayName = MicroUserInfo.GetUserInfo(UserName, ChineseName, EnglishName, AdDisplayName);

                                    string selected = string.Empty;
                                    //表单处于添加（申请）动作时并且默认选中审批者为True时，默认选中审批者
                                    if (Action == "add" && ApproversSelectedByDefault && isOnlyOne && !isState && MainApproverUID != 0 && MainApproverUID == _UID)
                                    {
                                        selected = "selected: true";
                                        isOnlyOne = false;  //只选中第一条记录，后马上设置为False
                                    }


                                    //***在修改表单的情况下，取得提交表单时选中的审批者，让审批控件选中这些审批者Start***
                                    if (Action == "modify")
                                    {
                                        string[] uidArray = CanApprovalUID.Split(',');
                                        for (int i = 0; i < uidArray.Length; i++)
                                        {
                                            if (uidArray[i].toInt() == _UID)
                                                selected = "selected: true";
                                        }
                                    }
                                    //***在修改表单的情况下，取得提交表单时选中的审批者，让审批控件选中这些审批者End***

                                    _str2 += "{name: '" + DisplayName + UserState + "' ,showname: '" + DisplayName + "', userstate:'" + UserState + "' ,value: " + _UID + ", " + selected + "},";
                                }
                                UIDs2 = UIDs2.Substring(0, UIDs2.Length - 1);

                            }
                            else
                                _str2 += "{name: '还没有审批成员，请联系IT设定' ,showname: '还没有审批成员，请联系IT设定' ,userleave:'', children:[]},";

                            str2 = "{name: '代理审批者', children: [" + _str2 + "]},";
                        }
                        //*************得到自部或跨部门的代理审批者End*************
                        #endregion

                        #region 可选审批者
                        //*************得到他部的可选审批者Start*************
                        string UIDs = UIDs1;
                        if (!string.IsNullOrEmpty(UIDs2))
                            UIDs = UIDs1 + "," + UIDs2;

                        if (IsOptionalApproval)
                            str3 = GetOptionalApprovalUser(Action, CanApprovalUID, UIDs, _dtUsersLeave, _dtUsersState);

                        //*************得到他部的可选审批者End*************
                        #endregion
                    }
                    else
                        str = GetOptionalApprovalUser(Action, CanApprovalUID, "0", _dtUsersLeave, _dtUsersState);


                    flag = "[" + str + str2 + str3 + "]";

                    break;
                case "Role":
                    //按角色审批还没写 20200820
                    break;
                case "DeptUsers":
                    //按部门审批还没写 20200820
                    break;
            }

            return flag;
        }



        /// <summary>
        /// 代理审批者
        /// </summary>
        public class ByApprovalUserAttr
        {
            /// <summary>
            /// 返回代理审批用户的DateTable
            /// </summary>
            public DataTable DTByApprovalUser
            {
                set;
                get;
            }

            /// <summary>
            /// 检查是否有代理审批用户
            /// </summary>
            public Boolean IsCheckByApprovalUser
            {
                set;
                get;
            }

        }

        /// <summary>
        /// 获取代理审批用户(如果主要审批没有匹配成功，则从代理审批者的职称从底到高进行匹配，一旦匹配成功则取出记录终止循环)
        /// </summary>
        /// <returns></returns>
        public static ByApprovalUserAttr GetByApprovalUser(string Action, string ApprovalByIDStr, string DeptsID, string UIDs1, Boolean IsVerticalDirection, Boolean FindRange)
        {
            DataTable dtByApprovalUser = null;
            Boolean isCheckByApprovalUser = false;  //检查是否有代理用户，如果有则返回true

            string _sql2 = "select * from UserInfo where Invalid=0 and Del=0 and UID in (select UID from UserJobTitle where Invalid=0 and Del=0 and JTID in (" + ApprovalByIDStr + ")) and UID in (select UID from UserDepts where Invalid=0 and Del=0 and DeptID in (" + DeptsID + ")) and UID not in (" + UIDs1 + ")";   //and JTID in (" + ApprovalByIDStr + "))
            DataTable _dt2 = MsSQLDbHelper.Query(_sql2).Tables[0];

            if (_dt2 != null && _dt2.Rows.Count > 0)
            {
                dtByApprovalUser = _dt2;
                isCheckByApprovalUser = true;
            }

            var ByApprovalUserAttr = new ByApprovalUserAttr
            {
                DTByApprovalUser = dtByApprovalUser,
                IsCheckByApprovalUser = isCheckByApprovalUser
            };

            return ByApprovalUserAttr;

        }


        /// <summary>
        /// 获取可选审批者，当表单审批节点启用可选审批者时调用此方法。此方法的可选审批者是JobTitle职位职称表“作为审批者”启用的选项
        /// </summary>
        /// <param name="Action"></param>
        /// <param name="CanApprovalUID"></param>
        /// <param name="UIDs"></param>
        /// <returns></returns>
        public static string GetOptionalApprovalUser(string Action, string CanApprovalUID, string UIDs, DataTable _dtUsersLeave, DataTable _dtUsersState)
        {
            string str3 = string.Empty;

            string _sql3 = "select * from UserInfo where Invalid=0 and Del=0 and UID in (select UID from UserJobTitle where Invalid = 0 and Del = 0 and JTID in (select JTID from JobTitle where IsApprover = 1 and Invalid = 0 and Del = 0)) and UID not in (" + UIDs + ")";

            string _str3 = string.Empty;
            string UIDs3 = string.Empty;
            DataTable _dt3 = MsSQLDbHelper.Query(_sql3).Tables[0];
            DataRow[] _rows3 = _dt3.Select("", "EnglishName");
            if (_rows3.Length > 0)
            {
                foreach (DataRow _dr3 in _rows3)
                {
                    string DisplayName = string.Empty, UserName = string.Empty, ChineseName = string.Empty, EnglishName = string.Empty, AdDisplayName = string.Empty, UserState = string.Empty; ;
                    int _UID = _dr3["UID"].toInt();
                    UIDs3 += _UID.toJsonTrim() + ",";

                    //判断用户是否休假
                    DataRow[] _drUsersLeave = _dtUsersLeave.Select("LeaveUID=" + _UID);
                    if (_drUsersLeave.Length > 0)
                        UserState = "【休假中】";
                    //否则判断用户状态（会议、外出等）
                    else
                    {
                        DataRow[] _drUsersState = _dtUsersState.Select("UID=" + _UID);
                        if (_drUsersState.Length > 0)
                            UserState = "【" + _drUsersState[0]["StateName"].toStringTrim() + "】";
                    }

                    UserName = _dr3["UserName"].toJsonTrim();
                    ChineseName = _dr3["ChineseName"].toJsonTrim();
                    EnglishName = _dr3["EnglishName"].toJsonTrim();
                    AdDisplayName = _dr3["AdDisplayName"].toJsonTrim();

                    DisplayName = MicroUserInfo.GetUserInfo(UserName, ChineseName, EnglishName, AdDisplayName);

                    string selected = string.Empty;
                    //***在修改表单的情况下，取得提交表单时选中的审批者，让审批控件选中这些审批者Start***
                    if (Action == "modify")
                    {
                        string[] uidArray = CanApprovalUID.Split(',');
                        for (int i = 0; i < uidArray.Length; i++)
                        {
                            if (uidArray[i].toInt() == _UID)
                                selected = "selected: true";
                        }
                    }
                    //***在修改表单的情况下，取得提交表单时选中的审批者，让审批控件选中这些审批者End***

                    _str3 += "{name: '" + DisplayName + UserState + "' ,showname: '" + DisplayName + "', userstate:'" + UserState + "' ,value: " + _UID + ", " + selected + "},";
                }

                str3 = "{name: '可选审批者', children: [" + _str3 + "]},";
            }

            return str3;
        }


        /// <summary>
        /// 获取主要审批者UID，按班、系、科、部逐级寻找（即在班内、系内、科内、部内逐级上寻找，只有寻找到有记录则停止并返回值）
        /// </summary>
        /// <param name="CanApprovalIDStr"></param>
        /// <param name="BitSort">排序</param>
        /// <returns></returns>
        public static int GetMainApproveUID(string CanApprovalIDStr, Boolean FindRange, Boolean FindGMOffice, Boolean BitSort, DataTable _dtUsersLeave)
        {
            int MainApproverUID = 0;

            //desc=降序，asc=升序（对于Sort数字来说数字越小职位越大，对于职位来说职位越小数字越大，所以要实现从职位低到高的排序，Sort desc即降序）
            string OrderBy = BitSort ? " order by Sort asc" : " order by Sort desc";

            string _sqlJobTitle = "select Sort, JTID from JobTitle where Invalid=0 and Del=0 and JTID in (" + CanApprovalIDStr + ") " + OrderBy + "";
            DataTable _dtJobTitle = MsSQLDbHelper.Query(_sqlJobTitle).Tables[0];

            if (_dtJobTitle != null && _dtJobTitle.Rows.Count > 0)
            {
                Boolean isRecord = true;

                for (int i = 0; i < _dtJobTitle.Rows.Count; i++)
                {
                    if (isRecord)
                    {
                        string JTID = _dtJobTitle.Rows[i]["JTID"].toStringTrim();
                        if (GetMainApproveUID(JTID, FindRange, FindGMOffice, _dtUsersLeave, "SquadDepts") > 0)
                        {
                            MainApproverUID = GetMainApproveUID(JTID, FindRange, FindGMOffice, _dtUsersLeave, "SquadDepts");
                            isRecord = false;
                        }

                        else if (GetMainApproveUID(JTID, FindRange, FindGMOffice, _dtUsersLeave, "SubSectionDepts") > 0)
                        {
                            MainApproverUID = GetMainApproveUID(JTID, FindRange, FindGMOffice, _dtUsersLeave, "SubSectionDepts");
                            isRecord = false;
                        }

                        else if (GetMainApproveUID(JTID, FindRange, FindGMOffice, _dtUsersLeave, "SectionDepts") > 0)
                        {
                            MainApproverUID = GetMainApproveUID(JTID, FindRange, FindGMOffice, _dtUsersLeave, "SectionDepts");
                            isRecord = false;
                        }

                        else if (GetMainApproveUID(JTID, FindRange, FindGMOffice, _dtUsersLeave, "DeptDepts") > 0)
                        {
                            MainApproverUID = GetMainApproveUID(JTID, FindRange, FindGMOffice, _dtUsersLeave, "DeptDepts");
                            isRecord = false;
                        }
                    }
                }
            }

            return MainApproverUID;
        }

        /// <summary>
        /// 获取主要审批者UID （被GetMainApproveUID()逐级方法调用）
        /// </summary>
        /// <param name="CanApprovalIDStr"></param>
        /// <param name="DeptType"></param>
        /// <returns></returns>
        public static int GetMainApproveUID(string JTIDs, Boolean FindRange, Boolean FindGMOffice, DataTable _dtUsersLeave, string DeptType = "DeptDepts")
        {
            int MainApproverUID = 0;
            string DeptsID = MicroUserInfo.GetUserDepts(MicroUserInfo.GetUserInfo("UID"), "DeptID", DeptType, FindRange, FindGMOffice);

            if (FindGMOffice)
            {
                string GMDeptsID = MicroUserInfo.GetGMDepts();  //总经理办公室部门ID
                DeptsID = DeptsID + "," + GMDeptsID;
            }

            string _sqlMainApproverUID = "select UID from UserInfo where Invalid=0 and Del=0 and MainApprover=1 and UID in (select UID from UserJobTitle where Invalid=0 and Del=0 and JTID in (" + JTIDs + ")) and UID in (select UID from UserDepts where Invalid=0 and Del=0 and DeptID in (" + DeptsID + "))";
            DataTable _dtMainApproverUID = MsSQLDbHelper.Query(_sqlMainApproverUID).Tables[0];
            if (_dtMainApproverUID != null && _dtMainApproverUID.Rows.Count > 0)
            {
                Boolean isRecord = false;
                for (int i = 0; i < _dtMainApproverUID.Rows.Count; i++)
                {
                    if (!isRecord)
                    {
                        int _UID = _dtMainApproverUID.Rows[i]["UID"].toInt();
                        //判断用户是否休假

                        DataRow[] _drUsersLeave = _dtUsersLeave.Select("LeaveUID=" + _UID);
                        //==0代表不在休假中取出记录，并设置isRecord=true
                        if (_drUsersLeave.Length == 0)
                        {
                            MainApproverUID = _UID;
                            isRecord = true;
                        }
                    }
                }
            }

            return MainApproverUID;
        }

        /// <summary>
        /// 提交表单时把审批流程节点写入数据库（FormApprovalRecords）
        /// </summary>
        /// <param name="ShortTableName"></param>
        /// <param name="TableName"></param>
        /// <param name="ModuleID"></param>
        /// <param name="FormID"></param>
        /// <param name="FormsID"></param>
        /// <param name="FormFields"></param>
        /// <returns></returns>
        public static Boolean SetWorkFlow(string ShortTableName, string TableName, string ModuleID, string FormID, string FormsID, string FormFields)
        {
            Boolean flag = false;
            
            try
            {
                string FormNumber = string.Empty, ApplicationTypeID = string.Empty, AcceptWindow = string.Empty, UID = string.Empty, Applicant = string.Empty;

                //得到主键名称（主要用于在提交表单时得到该表单最大的ID，作为FormsID存入表单审批记录表中）
                var GetTableAttr = MicroDataTable.GetTableAttr(TableName);
                string PrimaryKeyName = GetTableAttr.PrimaryKeyName;

                //当FormsID为空时，得到主键的最大值，作为FormsID
                if (string.IsNullOrEmpty(FormsID))
                    FormsID = MsSQLDbHelper.GetMaxStr(PrimaryKeyName, TableName);

                //获取表单属性
                var GetFormAttr = MicroForm.GetFormAttr(ShortTableName, FormID);
                Boolean IsApproval = GetFormAttr.IsApproval; //判断表单是否需要审批
                Boolean IsFormAccept = GetFormAttr.IsAccept;
                Boolean AutoAccept = GetFormAttr.AutoAccept;
                Boolean AutoClose = GetFormAttr.AutoClose;
                Boolean AssignTasksByAppType = GetFormAttr.AssignTasksByAppType;   //是否按申请类型分配任务
                string FormName = GetFormAttr.FormName;
                string ReceiveWindow = GetFormAttr.ReceiveWindow;  //默认以表单接收窗口，当启用按申请类型分配时则以申请类型窗口

                //*******读取表单记录得到UID和DisplayName********
                string _sql0 = "select * from " + TableName + " where Invalid=0 and Del=0 and " + PrimaryKeyName + "=" + FormsID.toInt() + "";
                DataTable _dt0 = MsSQLDbHelper.Query(_sql0).Tables[0];

                if (_dt0.Rows.Count > 0)
                {
                    //判断FormNumber字段存在的时候，取出FormNumber的值
                    if (MsSQLDbHelper.ColumnExists(TableName, "FormNumber"))
                        FormNumber = _dt0.Rows[0]["FormNumber"].toStringTrim();

                    //*****判断ApplicationType字段存在的时候，取出AcceptWindow的值Start*****
                    //是否启用按申请类型分配任务（来自申请类型的受理窗口）
                    if (AssignTasksByAppType)
                    {
                        //判断ApplicationTypeID字段是否存在，存在时取出ReceiveWindow的值
                        if (MsSQLDbHelper.ColumnExists(TableName, "ApplicationTypeID"))
                        {
                            ApplicationTypeID = _dt0.Rows[0]["ApplicationTypeID"].toStringTrim();
                            if (!string.IsNullOrEmpty(ApplicationTypeID))
                            {
                                string _sql2 = "select * from FormApplicationType where Invalid=0 and Del=0 and FATID in (" + ApplicationTypeID + ")";
                                DataTable _dt2 = MsSQLDbHelper.Query(_sql2).Tables[0];
                                if (_dt2.Rows.Count > 0)
                                {
                                    for (int i = 0; i < _dt2.Rows.Count; i++)
                                    {
                                        string acceptWindow = _dt2.Rows[i]["AcceptWindow"].toStringTrim();
                                        if (!string.IsNullOrEmpty(acceptWindow))
                                            AcceptWindow += acceptWindow + ',';
                                    }
                                    if (!string.IsNullOrEmpty(AcceptWindow))
                                    {
                                        AcceptWindow = AcceptWindow.Substring(0, AcceptWindow.Length - 1);
                                        AcceptWindow = MicroPublic.GetDistinct(AcceptWindow);  //去除逗号分隔重复的字符串
                                        ReceiveWindow = AcceptWindow; //在启用按申请类型分配任务时（不为空时，否则还是按接收窗口）
                                    }
                                }
                            }
                        }
                    }
                    //*****判断ApplicationType字段存在的时候，取出AcceptWindow的值End*****

                    UID = _dt0.Rows[0]["UID"].toStringTrim();
                    Applicant = _dt0.Rows[0]["DisplayName"].toStringTrim();
                }
                //*******读取表单记录得到UID和DisplayName********


                //当表单是需要审批的表单类型，但该不需要审批时，把FormID赋值为0，读取不用审批的流程（即只有申请和完成两步）
                string _FormID = IsApproval == true ? FormID : "0";

                var getFormWorkFlowAttr = GetFormWorkFlowAttr(_FormID);
                Boolean IsWorkFlow = getFormWorkFlowAttr.IsWorkFlow;

                if (IsWorkFlow)
                {
                    //读取审批流程
                    DataTable _dtWorkFlow = getFormWorkFlowAttr.SourceDT;
                    Boolean IsFlowAccept = getFormWorkFlowAttr.IsAccept;  //受理节点
                    string WorkFlowID = getFormWorkFlowAttr.WorkFlowID;

                    flag = SetApprovalRecords(_dtWorkFlow, IsApproval, ShortTableName, ModuleID, FormName, WorkFlowID, FormID, IsFlowAccept, IsFormAccept, AutoAccept, AutoClose, ReceiveWindow, FormsID, FormNumber, UID, Applicant, FormFields);

                }
            }
            catch (Exception ex) { HttpContext.Current.Response.Write(ex.ToString()); }

            return flag;
        }

        /// <summary>
        /// 把审批流程节点写入数据库（FormApprovalRecords）
        /// </summary>
        /// <param name="_dt"></param>
        /// <param name="WorkFlowID"></param>
        /// <param name="FormID"></param>
        /// <param name="FormsID"></param>
        /// <param name="FormNumber"></param>
        /// <param name="UID"></param>
        /// <param name="Applicant"></param>
        /// <param name="FormFields"></param>
        /// <returns></returns>
        public static Boolean SetApprovalRecords(DataTable _dtWorkFlow, Boolean IsApproval, string ShortTableName, string ModuleID, string FormName, string WorkFlowID, string FormID, Boolean IsFlowAccept, Boolean IsFormAccept, Boolean AutoAccept, Boolean AutoClose, string ReceiveWindow, string FormsID, string FormNumber, string UID, string Applicant, string FormFields)
        {
            Boolean flag = false;
           
            try
            {
                //对提交表单的内容进行序列化转换
                dynamic json = JToken.Parse(FormFields) as dynamic;

                string Where = string.Empty;

                //*****Start*****
                if (IsApproval)
                {
                    if (!IsFlowAccept)
                        Where = " and FlowCode<>'Accept'";
                }
                else
                {
                    if (!IsFormAccept)
                        Where = " and FlowCode<>'Accept'";
                }
                //*****End*****


                DataRow[] _rowsWorkFlow = _dtWorkFlow.Select("IsConditionApproval=0 and ParentID=" + WorkFlowID + Where, "Sort");

                if (_rowsWorkFlow.Length > 0)
                {
                    DataTable _dtInsert = new DataTable();
                    _dtInsert.Columns.Add("Sort", typeof(int));
                    _dtInsert.Columns.Add("ModuleID", typeof(int));
                    _dtInsert.Columns.Add("FormID", typeof(int));
                    _dtInsert.Columns.Add("FormsID", typeof(int));
                    _dtInsert.Columns.Add("FormNumber", typeof(string));
                    _dtInsert.Columns.Add("WorkFlowID", typeof(int));
                    _dtInsert.Columns.Add("NodeName", typeof(string));
                    _dtInsert.Columns.Add("CanApprovalUID", typeof(string));
                    _dtInsert.Columns.Add("ApprovalUID", typeof(int));
                    _dtInsert.Columns.Add("ApprovalDisplayName", typeof(string));
                    _dtInsert.Columns.Add("StateCode", typeof(int));
                    _dtInsert.Columns.Add("ApprovalState", typeof(string));
                    _dtInsert.Columns.Add("ApprovalTime", typeof(DateTime));
                    _dtInsert.Columns.Add("ApprovalIP", typeof(string));
                    _dtInsert.Columns.Add("UID", typeof(int));
                    _dtInsert.Columns.Add("DisplayName", typeof(string));

                    for (int i = 0; i < _rowsWorkFlow.Length; i++)
                    {
                        string WFID = string.Empty, NodeName = string.Empty, CanApprovalUID = string.Empty, ApprovalUID = string.Empty, ApprovalDisplayName = string.Empty, StateCode = string.Empty, ApprovalState = string.Empty;

                        WFID = _rowsWorkFlow[i]["WFID"].toStringTrim();
                        NodeName = _rowsWorkFlow[i]["FlowName"].toJsonTrim();

                        if (IsApproval)
                        {
                            CanApprovalUID = json["ApprovalNode" + WFID];
                            CanApprovalUID = CanApprovalUID.toStringTrim();
                        }

                        //在提交表单成功时
                        //第一步审批其实是“申请”，把申请也作为一个审批流程，并设置状态为审批通过
                        if (i == 0)
                        {
                            ApprovalUID = UID;
                            ApprovalDisplayName = Applicant;
                            CanApprovalUID = UID;

                            //FormApprovalRecords表中的StateCode的值范围应该是 -1、0、1
                            //第一次提交表单时，把StateCode写入FormApprovalRecords表中，代码=1时返回审批通过[Pass]，由于第一次提交表单其实反应的状态应该是=提交申请[Pass]
                            //因为FormApprovalRecords表中的StateCode要用来计算表单状态，所以填入的StateCode为1，但反应的ApprocalState应该是11返回的状态，所以应该是如下
                            StateCode = "1";
                            ApprovalState = GetApprovalState(11);  //审批状态：0 = 等待审批[Waiting]、1 = 审批通过[Pass]、-1 = 驳回申请[Return]、-2 = 临时保存[Draft]、11 = 提交申请[Pass]、22 = 等待受理[WaitingForAccept]、23 = 等待对应[WaitingForProcessing]、24 = 等待处理[WaitingForProcessing]、32 = 受理中[Accepting]、33 = 对应中[Processing]、34 = 处理中[Processing]、100 = 完成[Finish]

                            //把表单第一步，申请作为审批日志插入到审批日志表，主要用于带出表单的履历
                            // 11=提交申请[Pass]、100=完成[Finish]
                            SetApprovalLogs(FormID, FormsID, FormNumber, NodeName, UID, Applicant, 11, "");
                        }
                        else
                        {
                            //初次插入FormApprovalRecords记录，除第一步“提交申请[Pass]”外，其它的在前面加上“等待”构成如：等待机密委员审批、等待IT委员审批、等待科长审批，等等
                            ApprovalState = "等待" + NodeName;
                        }

                        //*********第二步内容插入通知表或发送邮件通知 IsApproval=true时代表第二步为需要审批，=false代表不需要审批直接到受理窗口Start***********
                        if (i == 1)
                        {
                            //***得到FormName Start***
                            string _FormName = string.IsNullOrEmpty(FormName) == true ? "" : FormName;

                            //IsApproval = true为审批表单
                            if (IsApproval)
                            {
                                //提交申请通知审批者 ApplicantToApproval
                                //string Content = "" + Applicant + "，提交了" + _FormName + "申请，编号：" + FormNumber + "，拜托您进行审批。";  //Update 20210111
                                string Content = MicroNotice.GetNoticeContent("ApplicantToApproval", ShortTableName, ModuleID, FormID, FormsID, FormNumber, _FormName, "", "", "", Applicant, ""); 
                                MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, FormsID, FormNumber, "Msg", FormName, Content, false, CanApprovalUID);
                            }
                            //否则非审批表单直接到受理窗口
                            else
                            {
                                //启用受理节点（注意上面i==1代表该步骤是受理节点）
                                if (IsFormAccept)
                                {
                                    //启用自动受理时
                                    if (AutoAccept)
                                    {
                                        //如果启用自动受理时，取得受理窗口的人员UID写入到受理节点（如果没这步则由申请者触发，不友好）
                                        if (!string.IsNullOrEmpty(ReceiveWindow))
                                        {
                                            if (!MicroPublic.CheckSplitExists(ReceiveWindow, ApprovalUID, ','))
                                            {
                                                ApprovalUID = MicroPublic.GetSplitFirstStr(ReceiveWindow, ',');
                                                ApprovalDisplayName = MicroUserInfo.GetUserInfo("DisplayName", ApprovalUID);
                                            }
                                        }

                                        StateCode = "33";
                                        //系统自动接受，通知申请者
                                        //系统自动接受，通知受理者

                                        //更新表单状态
                                        SetFormState(ShortTableName, 33, FormID, FormsID);

                                        //###2受理了通知受理者。例1：xxx提交的《xxx》申请，编号：xxx，受理成功【系统自动受理】请您尽快对应（处理）。
                                        //string Content2 = "" + Applicant + "提交的" + _FormName + "申请，编号：" + FormNumber + "，受理成功【系统自动受理】请您尽快对应（处理）。";  //Update 20210111
                                        string Content2 = MicroNotice.GetNoticeContent("ApplicantAutoAccept", ShortTableName, ModuleID, FormID, FormsID, FormNumber, _FormName, "", "", "", Applicant, "");
                                        MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, FormsID, FormNumber, "Msg", FormName, Content2, false, ReceiveWindow);

                                        //###3受理了通知申请者。例1：您提交的《xxx》申请，编号：xxx，已被受理【系统自动受理】正在为您安排对应（处理），请耐心等待。
                                        //string Content3 = "您提交的" + _FormName + "申请，编号：" + FormNumber + "，已被受理【系统自动受理】正在为您安排对应（处理），请耐心等待。";  //Update 20210111
                                        string Content3 = MicroNotice.GetNoticeContent("AcceptAutoApplicant", ShortTableName, ModuleID, FormID, FormsID, FormNumber, _FormName, "", "", "", "", "");
                                        MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, FormsID, FormNumber, "Msg", FormName, Content3, false, UID);

                                        //插入日志记录
                                        SetApprovalLogs(FormID, FormsID, FormNumber, NodeName, ApprovalUID, Applicant, 33, "【系统自动受理】");

                                        //同时判断在启用自动受理的情况下是否启用自动结案
                                        if (AutoClose)
                                        {
                                            //如果启用自动结案时，取得受理窗口的人员UID写入到结案节点（如果没这步则由申请者触发，不友好）
                                            if (!string.IsNullOrEmpty(ReceiveWindow))
                                            {
                                                if (!MicroPublic.CheckSplitExists(ReceiveWindow, ApprovalUID, ','))
                                                {
                                                    ApprovalUID = MicroPublic.GetSplitFirstStr(ReceiveWindow, ',');
                                                    ApprovalDisplayName = MicroUserInfo.GetUserInfo("DisplayName", ApprovalUID);
                                                }
                                            }

                                            StateCode = "100";
                                            //系统自动结案，通知申请者
                                            //系统自动结案，通知受理者

                                            //更新表单状态
                                            SetFormState(ShortTableName, 100, FormID, FormsID);

                                            //###2结案了通知受理者。例1：xxx提交的《xxx》申请，编号：xxx，审批已通过并且对应（处理）完成【系统自动结案】。
                                            //string Content4 = "" + Applicant + "提交的" + _FormName + "申请，编号：" + FormNumber + "，审批已通过并且对应（处理）完成【系统自动结案】。";  //Update 20210111
                                            string Content4 = MicroNotice.GetNoticeContent("ClosedAutoAccept", ShortTableName, ModuleID, FormID, FormsID, FormNumber, _FormName, "", "", "", Applicant, "");
                                            MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, FormsID, FormNumber, "Msg", FormName, Content4, false, ReceiveWindow);

                                            //###3结案了通知申请者。例1：您提交的《xxx》申请，编号：xxx，已对应（处理）完成【系统自动结案】。
                                            //string Content5 = "您提交的" + _FormName + "申请，编号：" + FormNumber + "，已对应（处理）完成【系统自动结案】。";  //Update 20210111
                                            string Content5 = MicroNotice.GetNoticeContent("ClosedAutoApplicant2", ShortTableName, ModuleID, FormID, FormsID, FormNumber, _FormName, "", "", "", "", "");
                                            MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, FormsID, FormNumber, "Msg", FormName, Content5, false, UID);

                                            //插入日志记录
                                            SetApprovalLogs(FormID, FormsID, FormNumber, NodeName, ApprovalUID, Applicant, 100, "【系统自动结案】");

                                        }
                                    }
                                    else
                                    {
                                        StateCode = "0";
                                        //否则需要手动接受，通知受理者
                                        //申请成功不需要审批的表单但需要受理的情况下，通知受理者进行受理
                                        //更新表单状态
                                        SetFormState(ShortTableName, 23, FormID, FormsID);

                                        //string Content = "" + Applicant + "，提交了" + _FormName + "申请，编号：" + FormNumber + "，拜托您进行对应（处理）。";  //Update 20210111
                                        string Content = MicroNotice.GetNoticeContent("ApplicantToAccept", ShortTableName, ModuleID, FormID, FormsID, FormNumber, _FormName, "", "", "", Applicant, "");
                                        MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, FormsID, FormNumber, "Msg", FormName, Content, false, ReceiveWindow);
                                    }

                                }
                                //否则没有启用受理节点，则该节点变为结案节点（注意上面i==1代表该步骤是结案节点）
                                else
                                {
                                    //启用自动结案时
                                    if (AutoClose)
                                    {
                                        //如果启用自动结案时，取得受理窗口的人员UID写入到结案节点（如果没这步则由申请者触发，不友好）
                                        if (!string.IsNullOrEmpty(ReceiveWindow))
                                        {
                                            if (!MicroPublic.CheckSplitExists(ReceiveWindow, ApprovalUID, ','))
                                            {
                                                ApprovalUID = MicroPublic.GetSplitFirstStr(ReceiveWindow, ',');
                                                ApprovalDisplayName = MicroUserInfo.GetUserInfo("DisplayName", ApprovalUID);
                                            }
                                        }

                                        StateCode = "100";
                                        //系统自动结案，通知申请者
                                        //系统自动结案，通知受理者

                                        //更新表单状态
                                        SetFormState(ShortTableName, 100, FormID, FormsID);

                                        //###2结案了通知受理者。例1：xxx提交的《xxx》申请，编号：xxx，审批已通过并且对应（处理）完成【系统自动结案】。
                                        //string Content4 = "" + Applicant + "提交的" + _FormName + "申请，编号：" + FormNumber + "，审批已通过并且对应（处理）完成【系统自动结案】。";  //Update 20210111
                                        string Content4 = MicroNotice.GetNoticeContent("ClosedAutoAccept", ShortTableName, ModuleID, FormID, FormsID, FormNumber, _FormName, "", "", "", Applicant, "");
                                        MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, FormsID, FormNumber, "Msg", FormName, Content4, false, ReceiveWindow);

                                        //###3结案了通知申请者。例1：您提交的《xxx》申请，编号：xxx，已对应（处理）完成【系统自动结案】。
                                        //string Content5 = "您提交的" + _FormName + "申请，编号：" + FormNumber + "，已对应（处理）完成【系统自动结案】。";  //Update 20210111
                                        string Content5 = MicroNotice.GetNoticeContent("ClosedAutoApplicant2", ShortTableName, ModuleID, FormID, FormsID, FormNumber, _FormName, "", "", "", "", "");
                                        MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, FormsID, FormNumber, "Msg", FormName, Content5, false, UID);

                                        //插入日志记录
                                        SetApprovalLogs(FormID, FormsID, FormNumber, NodeName, ApprovalUID, Applicant, 100, "【系统自动结案】");
                                    }
                                    else
                                    {
                                        StateCode = "0";
                                        //否则需要手动结案，通知受理者
                                        //不需要审批的表单同时在没有启用受理节点的情况下，但需要处理的
                                        //更新表单状态
                                        SetFormState(ShortTableName, 33, FormID, FormsID);

                                        //string Content = "" + Applicant + "，提交了" + _FormName + "申请，编号：" + FormNumber + "，拜托您尽快行对应（处理）。";  //Update 20210111
                                        string Content = MicroNotice.GetNoticeContent("ApplicantToAccept", ShortTableName, ModuleID, FormID, FormsID, FormNumber, _FormName, "", "", "", Applicant, "");

                                        MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, FormsID, FormNumber, "Msg", FormName, Content, false, ReceiveWindow);
                                    }
                                }

                            }  //非审批表单End
                        }
                        //*********第二步内容插入通知表或发送邮件通知 IsApproval=true时代表第二步为需要审批，=false代表不需要审批直接到受理窗口End***********

                        string _CanApprovalUID = CanApprovalUID;
                        // 启用受理时，即审批流多了受理这一步，通常是倒数第二步，所以会有IsAccept && i == _rows.Length - 2
                        if (i == _rowsWorkFlow.Length - 2)
                        {
                            if (IsFlowAccept || IsFormAccept)
                            {
                                _CanApprovalUID = ReceiveWindow;

                                //在受理节点，如果ReceiveWindow为空且_rowsWorkFlow.Length >3 则把最后审批节点的审批者UID作为完成节点的_CanApprovalUID
                                if (string.IsNullOrEmpty(_CanApprovalUID) && _rowsWorkFlow.Length > 3)
                                {
                                    string _WFID = _rowsWorkFlow[_rowsWorkFlow.Length - 3]["WFID"].toStringTrim();
                                    _CanApprovalUID = json["ApprovalNode" + _WFID];
                                }
                            }

                        }

                        // i == _rows.Length - 1 即代表是最后一步（“完成”代表谁有权限结案），在最后一步把 ReceiveWindow作为CanApprovalUID
                        if (i == _rowsWorkFlow.Length - 1)
                        {
                            _CanApprovalUID = ReceiveWindow;

                            if (!IsApproval)
                            {
                                if (AutoAccept && AutoClose)
                                    StateCode = "100";
                            }

                            //在完成节点，如果ReceiveWindow为空且_rowsWorkFlow.Length >2 则把最后审批节点的审批者UID作为完成节点的_CanApprovalUID
                            if (string.IsNullOrEmpty(_CanApprovalUID)  && _rowsWorkFlow.Length >2 )
                            {
                                string _WFID = _rowsWorkFlow[_rowsWorkFlow.Length - 2]["WFID"].toStringTrim();
                                _CanApprovalUID = json["ApprovalNode" + _WFID];
                            }
                        }

                        DataRow _dr = _dtInsert.NewRow();
                        _dr["Sort"] = i + 1;
                        _dr["ModuleID"] = ModuleID.toInt();
                        _dr["FormID"] = FormID.toInt();
                        _dr["FormsID"] = FormsID.toInt();
                        _dr["FormNumber"] = FormNumber;
                        _dr["WorkFlowID"] = WFID.toInt();
                        _dr["NodeName"] = NodeName;
                        _dr["CanApprovalUID"] = _CanApprovalUID;
                        _dr["ApprovalUID"] = ApprovalUID.toInt();
                        _dr["ApprovalDisplayName"] = ApprovalDisplayName;
                        _dr["StateCode"] = StateCode.toInt();
                        _dr["ApprovalState"] = ApprovalState;
                        _dr["ApprovalTime"] = DateTime.Now;
                        _dr["ApprovalIP"] = MicroPublic.GetClientIP();
                        _dr["UID"] = UID.toInt();
                        _dr["DisplayName"] = Applicant;
                        _dtInsert.Rows.Add(_dr);

                    }
                    flag = MsSQLDbHelper.SqlBulkCopyInsert(_dtInsert, "FormApprovalRecords");
                }
            }
            catch (Exception ex) { HttpContext.Current.Response.Write(ex.ToString()); }

            return flag;

        }

        /// <summary>
        /// 修改表单时，修改审批流程审批者（如果有发生修改时发生）
        /// </summary>
        /// <param name="FormID"></param>
        /// <param name="FormsID"></param>
        /// <param name="ShortTableName"></param>
        /// <param name="TableName"></param>
        /// <param name="FormFields"></param>
        /// <returns></returns>
        public static Boolean SetModifyWorkFlow(string ShortTableName, string TableName, string ModuleID, string FormID, string FormsID, string FormFields)
        {
            Boolean flag = false;

            try
            {
                string FormNumber = string.Empty, ApplicationTypeID = string.Empty, AcceptWindow = string.Empty, UID = string.Empty, Applicant = string.Empty;

                //根据表名得到主键名称（主要用于在提交表单时得到该表PrimaryKeyName）
                var GetTableAttr = MicroDataTable.GetTableAttr(TableName);
                string PrimaryKeyName = GetTableAttr.PrimaryKeyName;

                //获取表单属性
                var GetFormAttr = MicroForm.GetFormAttr(ShortTableName, FormID);
                Boolean IsApproval = GetFormAttr.IsApproval;
                Boolean IsFormAccept = GetFormAttr.IsAccept;
                Boolean AutoAccept = GetFormAttr.AutoAccept;
                Boolean AutoClose = GetFormAttr.AutoClose;
                Boolean AssignTasksByAppType = GetFormAttr.AssignTasksByAppType;   //是否按申请类型分配任务
                string FormName = GetFormAttr.FormName;
                string ReceiveWindow = GetFormAttr.ReceiveWindow;  //默认以表单表“Forms”的ReceiveWindow作为结案窗口，如果在申请类型上作了按申请类型受理窗口，此时以受理窗口优先同时谁受理谁结案

                //*******读取表单记录得到UID和DisplayName********
                string _sql0 = "select * from " + TableName + " where Invalid=0 and Del=0 and " + PrimaryKeyName + "=" + FormsID.toInt() + "";
                DataTable _dt0 = MsSQLDbHelper.Query(_sql0).Tables[0];

                if (_dt0.Rows.Count > 0)
                {
                    //判断FormNumber字段存在的时候，取出FormNumber的值
                    if (MsSQLDbHelper.ColumnExists(TableName, "FormNumber"))
                        FormNumber = _dt0.Rows[0]["FormNumber"].toStringTrim();

                    //*****判断ApplicationType字段存在的时候，取出AcceptWindow的值Start*****
                    //是否启用按申请类型分配任务（来自申请类型的受理窗口）
                    if (AssignTasksByAppType)
                    {
                        //判断ApplicationTypeID字段是否存在，存在时取出ReceiveWindow的值
                        if (MsSQLDbHelper.ColumnExists(TableName, "ApplicationTypeID"))
                        {
                            ApplicationTypeID = _dt0.Rows[0]["ApplicationTypeID"].toStringTrim();
                            if (!string.IsNullOrEmpty(ApplicationTypeID))
                            {
                                string _sql2 = "select * from FormApplicationType where Invalid=0 and Del=0 and FATID in (" + ApplicationTypeID + ")";
                                DataTable _dt2 = MsSQLDbHelper.Query(_sql2).Tables[0];
                                if (_dt2.Rows.Count > 0)
                                {
                                    for (int i = 0; i < _dt2.Rows.Count; i++)
                                    {
                                        string acceptWindow = _dt2.Rows[i]["AcceptWindow"].toStringTrim();
                                        if (!string.IsNullOrEmpty(acceptWindow))
                                            AcceptWindow += acceptWindow + ',';
                                    }
                                    if (!string.IsNullOrEmpty(AcceptWindow))
                                    {
                                        AcceptWindow = AcceptWindow.Substring(0, AcceptWindow.Length - 1);
                                        AcceptWindow = MicroPublic.GetDistinct(AcceptWindow);  //去除逗号分隔重复的字符串
                                        ReceiveWindow = AcceptWindow; //在启用按申请类型分配任务时（不为空时，否则还是按接收窗口）
                                    }
                                }
                            }
                        }
                    }
                    //*****判断ApplicationType字段存在的时候，取出AcceptWindow的值End*****

                    UID = _dt0.Rows[0]["UID"].toStringTrim();
                    Applicant = _dt0.Rows[0]["DisplayName"].toStringTrim();
                }
                //*******读取表单记录得到UID和DisplayName********

                //当表单是需要审批类型的表单，但该不需要审批时（即只有申请和完成两步），把FormID赋值为0，读取不用审批的流程
                string _FormID = IsApproval == true ? FormID : "0";

                var getFormWorkFlowAttr = GetFormWorkFlowAttr(_FormID, FormsID);
                Boolean IsWorkFlow = getFormWorkFlowAttr.IsWorkFlow;

                if (IsWorkFlow)
                {
                    //读取审批流程
                    DataTable _dt = getFormWorkFlowAttr.SourceDT;
                    Boolean IsFlowAccept = getFormWorkFlowAttr.IsAccept;
                    string WorkFlowID = getFormWorkFlowAttr.WorkFlowID;

                    flag = SetModifyApprovalRecords(_dt, IsApproval, ShortTableName, ModuleID, FormName, WorkFlowID, FormID, IsFlowAccept, IsFormAccept, AutoAccept, AutoClose, ReceiveWindow, FormsID, FormNumber, UID, Applicant, FormFields);

                }

                //在修改表单时重置审批记录，在审批记录重置成功后更新表单状态(即把表单状态重新设置为等待审批)
                //0 = 等待审批[Waiting]、1 = 审批通过[Pass]、-1 = 驳回申请[Return]、-2 = 临时保存[Draft]、11 = 提交申请[Pass]、22 = 等待受理[WaitingForAccept]、23 = 等待对应[WaitingForProcessing]、24 = 等待处理[WaitingForProcessing]、32 = 受理中[Accepting]、33 = 对应中[Processing]、34 = 处理中[Processing]、100 = 完成[Finish]

                int StateCode = 0;
                if (!IsApproval)
                {
                    //不需要审批时，判断表单表（Forms）是否启用受理属性，启用受理属性时表单状态为23“ 等待对应[WaitingForProcessing]”，否则表单状态属性为33“对应中[Processing]”
                    if (IsFormAccept)
                        StateCode =23;
                    else
                        StateCode = 33;
                }

                if (flag)
                    SetFormState(ShortTableName, StateCode, FormID, FormsID);

            }
            catch (Exception ex) { HttpContext.Current.Response.Write(ex.ToString()); }

            return flag;
        }

        /// <summary>
        /// 修改表单时重置审批记录（FormApprovalRecords）
        /// </summary>
        /// <param name="_dt"></param>
        /// <param name="IsApproval"></param>
        /// <param name="ShortTableName"></param>
        /// <param name="FormName"></param>
        /// <param name="WorkFlowID"></param>
        /// <param name="FormID"></param>
        /// <param name="IsAccept"></param>
        /// <param name="ReceiveWindow"></param>
        /// <param name="FormsID"></param>
        /// <param name="FormNumber"></param>
        /// <param name="UID"></param>
        /// <param name="Applicant"></param>
        /// <param name="FormFields"></param>
        /// <returns></returns>
        public static Boolean SetModifyApprovalRecords(DataTable _dtWorkFlow, Boolean IsApproval, string ShortTableName, string ModuleID, string FormName, string WorkFlowID, string FormID, Boolean IsFlowAccept, Boolean IsFormAccept, Boolean AutoAccept, Boolean AutoClose, string ReceiveWindow, string FormsID, string FormNumber, string UID, string Applicant, string FormFields)
        {
            Boolean flag = false;

            try
            {
                //对提交表单的内容进行序列化转换
                dynamic json = JToken.Parse(FormFields) as dynamic;

                //出现问题时注释本区块内容，恢复备份 注20210126
                //*****Start*****
                string Where = string.Empty;
                if (IsApproval)
                {
                    if (!IsFlowAccept)
                        Where = " and FlowCode<>'Accept'";
                }
                else
                {
                    if (!IsFormAccept)
                        Where = " and FlowCode<>'Accept'";
                }
                //*****End*****

                DataRow[] _rowsWorkFlow = _dtWorkFlow.Select("IsConditionApproval=0 and ParentID=" + WorkFlowID + Where, "Sort");

                int Result = 0;
                if (_rowsWorkFlow.Length > 0)
                {
                    for (int i = 0; i < _rowsWorkFlow.Length; i++)
                    {
                        string WFID = string.Empty, NodeName = string.Empty, CanApprovalUID = string.Empty, ApprovalUID = string.Empty, ApprovalDisplayName = string.Empty, StateCode = string.Empty, ApprovalState = string.Empty;

                        WFID = _rowsWorkFlow[i]["WFID"].toStringTrim();
                        NodeName = _rowsWorkFlow[i]["FlowName"].toJsonTrim();

                        if (IsApproval)
                        {
                            CanApprovalUID = json["ApprovalNode" + WFID];
                            CanApprovalUID = CanApprovalUID.toStringTrim();
                        }

                        //在修改表单成功时和新增提交申请一样
                        //第一步审批其实是“申请”，把申请作为一个审批流程，并设置状态为审批通过
                        if (i == 0)
                        {
                            ApprovalUID = UID;
                            ApprovalDisplayName = Applicant;
                            CanApprovalUID = UID;

                            //FormApprovalRecords表中的StateCode的值范围应该是 -1、0、1
                            //在修改表单时像新增提交申请一样，把StateCode写入FormApprovalRecords表中，代码=1时返回审批通过[Pass]，由于第一次提交表单其实反应的状态应该是=提交申请[Pass]
                            //因为FormApprovalRecords表中的StateCode要用来计算表单状态，所以填入的StateCode为1，但反应的ApprocalState应该是11返回的状态，所以应该是如下
                            StateCode = "1";
                            ApprovalState = "重新" + GetApprovalState(11);  //0 = 等待审批[Waiting]、1 = 审批通过[Pass]、-1 = 驳回申请[Return]、-2 = 临时保存[Draft]、11 = 提交申请[Pass]、22 = 等待受理[WaitingForAccept]、23 = 等待对应[WaitingForProcessing]、24 = 等待处理[WaitingForProcessing]、32 = 受理中[Accepting]、33 = 对应中[Processing]、34 = 处理中[Processing]、100 = 完成[Finish]

                            //把表单第一步，申请作为审批日志插入到审批日志表，主要用于带出表单的履历
                            // 11=提交申请[Pass]、100=完成[Finish]
                            SetApprovalLogs(FormID, FormsID, FormNumber, NodeName, UID, Applicant, 11, ApprovalState);
                        }
                        else
                        {
                            //驳回修改后重新插入FormApprovalRecords记录，除第一步“提交申请[Pass]”外，其它的在前面加上“等待”构成如：等待机密委员审批、等待IT委员审批、等待科长审批，等等
                            ApprovalState = "等待" + NodeName;
                        }

                        //*********第二步内容插入通知表或发送邮件通知 IsApproval=true时代表第二步为需要审批，=false代表不需要审批直接到受理窗口Start***********
                        if (i == 1)
                        {
                            //***得到FormName Start***
                            string _FormName = string.IsNullOrEmpty(FormName) == true ? "" : "《" + FormName + "》";

                            //IsApproval = true时代表第二步为需要审批
                            if (IsApproval)
                            {
                                //string Content = "" + DisplayName + "，重新提交了" + _FormName + "申请，编号：" + FormNumber + "，拜托您进行审批。";  //Update 20210111
                                string Content = MicroNotice.GetNoticeContent("ApplicantReToApproval", ShortTableName, ModuleID, FormID, FormsID, FormNumber, _FormName, "", "", "", Applicant, "");
                                MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, FormsID, FormNumber, "Msg", FormName, Content, false, CanApprovalUID);
                            }
                            //false代表不需要审批直接到受理窗口
                            else
                            {
                                // 20200927注
                                //因为是不需要审批的表单，没有驳回节点（要么自动受理和完成，要么手动受理和完成，不会出现驳回节点所以表单也不能修改），所以这里不需要像新增表单时的审批流程那样
                                //string Content = "" + Applicant + "，提交了" + _FormName + "申请，编号：" + FormNumber + "，拜托您进行对应（处理）。";  //Update 20210111
                                string Content = MicroNotice.GetNoticeContent("ApplicantToAccept", ShortTableName, ModuleID, FormID, FormsID, FormNumber, _FormName, "", "", "", Applicant, "");
                                    MicroNotice.SetNotice(ShortTableName, ModuleID, FormID, FormsID, FormNumber, "Msg", FormName, Content, false, ReceiveWindow);

                                }
                            }
                        //*********第二步内容插入通知表或发送邮件通知 IsApproval=true时代表第二步为需要审批，=false代表不需要审批直接到受理窗口End***********

                        string _CanApprovalUID = CanApprovalUID;

                        // 启用受理时，即审批流多了受理这一步，通常是倒数第二步，所以会有IsAccept && i == _rows.Length - 2
                        if (i == _rowsWorkFlow.Length - 2)
                        {
                            if (IsFlowAccept || IsFormAccept)
                            {
                                _CanApprovalUID = ReceiveWindow;

                                //在受理节点，如果ReceiveWindow为空且_rowsWorkFlow.Length >3 则把最后审批节点的审批者UID作为完成节点的_CanApprovalUID
                                if (string.IsNullOrEmpty(_CanApprovalUID) && _rowsWorkFlow.Length > 3)
                                {
                                    string _WFID = _rowsWorkFlow[_rowsWorkFlow.Length - 3]["WFID"].toStringTrim();
                                    _CanApprovalUID = json["ApprovalNode" + _WFID];
                                }
                            }
                        }

                        // i == _rowsWorkFlow.Length - 1 时
                        //即代表是最后一步（“完成”代表谁有权限结案），在最后一步把 ReceiveWindow作为CanApprovalUID
                        if (i == _rowsWorkFlow.Length - 1)
                        {
                            _CanApprovalUID = ReceiveWindow;

                            if (!IsApproval)
                            {
                                if (AutoAccept && AutoClose)
                                    StateCode = "100";
                            }

                            //在完成节点，如果ReceiveWindow为空且_rowsWorkFlow.Length >2 则把最后审批节点的审批者UID作为完成节点的_CanApprovalUID
                            if (string.IsNullOrEmpty(_CanApprovalUID) && _rowsWorkFlow.Length > 2)
                            {
                                string _WFID = _rowsWorkFlow[_rowsWorkFlow.Length - 2]["WFID"].toStringTrim();
                                _CanApprovalUID = json["ApprovalNode" + _WFID];
                            }
                        }
                        
                        string _sql = "update FormApprovalRecords set CanApprovalUID=@CanApprovalUID, ApprovalUID=@ApprovalUID, ApprovalDisplayName=@ApprovalDisplayName, StateCode=@StateCode, ApprovalState=@ApprovalState, ApprovalTime=@ApprovalTime, ApprovalIP=@ApprovalIP, UID=@UID, DisplayName=@DisplayName, Note=@Note where FormID=@FormID and FormsID=@FormsID and WorkFlowID=@WorkFlowID";

                        SqlParameter[] _sp = {
                                                new SqlParameter("@CanApprovalUID", SqlDbType.VarChar,200),
                                                new SqlParameter("@ApprovalUID", SqlDbType.Int),
                                                new SqlParameter("@ApprovalDisplayName", SqlDbType.NVarChar,200),
                                                new SqlParameter("@StateCode", SqlDbType.Int),
                                                new SqlParameter("@ApprovalState", SqlDbType.NVarChar,200),
                                                new SqlParameter("@ApprovalTime", SqlDbType.DateTime),
                                                new SqlParameter("@ApprovalIP", SqlDbType.VarChar,50),
                                                new SqlParameter("@UID", SqlDbType.Int),
                                                new SqlParameter("@DisplayName", SqlDbType.NVarChar,200),
                                                new SqlParameter("@Note", SqlDbType.NVarChar,4000),
                                                new SqlParameter("@FormID", SqlDbType.Int),
                                                new SqlParameter("@FormsID", SqlDbType.Int),
                                                new SqlParameter("@WorkFlowID", SqlDbType.Int),
                        };

                        _sp[0].Value = _CanApprovalUID;
                        _sp[1].Value = ApprovalUID.toInt();
                        _sp[2].Value = ApprovalDisplayName;
                        _sp[3].Value = StateCode.toInt();
                        _sp[4].Value = ApprovalState;
                        _sp[5].Value = DateTime.Now;
                        _sp[6].Value = MicroPublic.GetClientIP();
                        _sp[7].Value = UID.toInt();
                        _sp[8].Value = Applicant;
                        _sp[9].Value = null;
                        _sp[10].Value = FormID.toInt();
                        _sp[11].Value = FormsID.toInt();
                        _sp[12].Value = WFID.toInt();

                        if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
                            Result = Result + 1;

                    }
                }

                if (Result > 0)
                    flag = true;
            }
            catch (Exception ex) { HttpContext.Current.Response.Write(ex.ToString()); }

            return flag;

        }

        /// <summary>
        /// 更新表单审批记录 （用于在每次审批时更新表单记录状态）
        /// </summary>
        /// <param name="ApprovalUID"></param>
        /// <param name="ApprovalDisplayName"></param>
        /// <param name="StateCode"></param>
        /// <param name="Note"></param>
        /// <param name="FARID"></param>
        /// <returns></returns>
        public static Boolean SetFormApprovalRecords(string ApprovalUID, string ApprovalDisplayName, int StateCode, string Note, int FARID)
        {
            Boolean flag = false;

            string _sql = "update FormApprovalRecords set ApprovalUID=@ApprovalUID, ApprovalDisplayName=@ApprovalDisplayName, StateCode=@StateCode, ApprovalState=@ApprovalState, ApprovalTime=@ApprovalTime, ApprovalIP=@ApprovalIP, Note=@Note, DateModified=@DateModified where FARID=@FARID";

            SqlParameter[] _sp = {
                                    new SqlParameter("@ApprovalUID",SqlDbType.Int),
                                    new SqlParameter("@ApprovalDisplayName",SqlDbType.NVarChar,200),
                                    new SqlParameter("@StateCode",SqlDbType.Int),
                                    new SqlParameter("@ApprovalState",SqlDbType.NVarChar,200),
                                    new SqlParameter("@ApprovalTime",SqlDbType.DateTime),
                                    new SqlParameter("@ApprovalIP",SqlDbType.VarChar,50),
                                    new SqlParameter("@Note",SqlDbType.NVarChar,4000),
                                    new SqlParameter("@DateModified",SqlDbType.DateTime),
                                    new SqlParameter("@FARID",SqlDbType.VarChar)
                                };

            _sp[0].Value = ApprovalUID.toInt();
            _sp[1].Value = ApprovalDisplayName;
            _sp[2].Value = StateCode;
            _sp[3].Value = GetApprovalState(StateCode);
            _sp[4].Value = DateTime.Now;
            _sp[5].Value = MicroUserInfo.GetUserInfo("IP");
            _sp[6].Value = Note;
            _sp[7].Value = DateTime.Now;
            _sp[8].Value = FARID;

            //执行更新审批记录语句
            if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
                flag = true;

            return flag;
        }

        /// <summary>
        /// 插入日志(单条记录)
        /// </summary>
        /// <param name="FormID"></param>
        /// <param name="FormsID"></param>
        /// <param name="FormNumber"></param>
        /// <param name="NodesName"></param>
        /// <param name="ApprovalUID"></param>
        /// <param name="ApprovalDisplayName"></param>
        /// <param name="StateCode"></param>
        /// <param name="ApprovalState"></param>
        /// <param name="UID"></param>
        /// <param name="DisplayName"></param>
        /// <returns></returns>
        public static Boolean SetApprovalLogs(string FormID, string FormsID, string FormNumber, string NodesName, string UID, string DisplayName, int StateCode, string Note)
        {
            Boolean flag = false;

            string[] tmpArray = NodesName.Split(',');

            if (tmpArray.Length > 0)
            {

                DataTable _dtInsert = new DataTable();
                _dtInsert.Columns.Add("FormID", typeof(int));
                _dtInsert.Columns.Add("FormsID", typeof(int));
                _dtInsert.Columns.Add("FormNumber", typeof(string));
                _dtInsert.Columns.Add("NodeName", typeof(string));
                _dtInsert.Columns.Add("ApprovalUID", typeof(int));
                _dtInsert.Columns.Add("ApprovalDisplayName", typeof(string));
                _dtInsert.Columns.Add("StateCode", typeof(int));
                _dtInsert.Columns.Add("ApprovalState", typeof(string));
                _dtInsert.Columns.Add("ApprovalTime", typeof(DateTime));
                _dtInsert.Columns.Add("ApprovalIP", typeof(string));
                _dtInsert.Columns.Add("UID", typeof(int));
                _dtInsert.Columns.Add("DisplayName", typeof(string));
                _dtInsert.Columns.Add("Note", typeof(string));

                for (int i = 0; i < tmpArray.Length; i++)
                {
                    string NodeName = tmpArray[i].toJsonTrim();

                    DataRow _dr = _dtInsert.NewRow();
                    _dr["FormID"] = FormID.toInt();
                    _dr["FormsID"] = FormsID.toInt();
                    _dr["FormNumber"] = FormNumber;
                    _dr["NodeName"] = NodeName;
                    _dr["ApprovalUID"] = UID.toInt();
                    _dr["ApprovalDisplayName"] = DisplayName;
                    _dr["StateCode"] = StateCode;
                    _dr["ApprovalState"] = GetApprovalState(StateCode);
                    _dr["ApprovalTime"] = DateTime.Now;
                    _dr["ApprovalIP"] = MicroPublic.GetClientIP();
                    _dr["UID"] = UID.toInt();
                    _dr["DisplayName"] = MicroUserInfo.GetUserInfo("DisplayName", UID);
                    _dr["Note"] = Note;
                    _dtInsert.Rows.Add(_dr);
                }

                flag = MsSQLDbHelper.SqlBulkCopyInsert(_dtInsert, "FormApprovalLogs");
            }
            return flag;

        }

        /// <summary>
        /// 在审批、受理或对应完成后更新表单状态。传入ShortTableName & StateCode & FormID & FormsID，更新成功返回True，否则返回False
        /// </summary>
        /// <param name="ShortTableName"></param>
        /// <param name="StateCode"></param>
        /// <param name="FormID"></param>
        /// <param name="FormsID"></param>
        /// <returns></returns>
        public static Boolean SetFormState(string ShortTableName, int StateCode, string FormID, string FormsID)
        {
            Boolean flag = false;

            //得到真实表名
            string TableName = MicroPublic.GetTableName(ShortTableName);
            var getTableAttr = MicroDataTable.GetTableAttr(TableName);
            string PrimaryKeyName = getTableAttr.PrimaryKeyName;
            Boolean MainSub = getTableAttr.MainSub;

            if (MainSub)
            {
                string _sql = "update " + TableName + " set StateCode=@StateCode, FormState=@FormState where Invalid=0 and Del=0 and FormID=@FormID and " + PrimaryKeyName + "=@FormsID or (ParentID=@ParentID and Invalid=0 and Del=0)";
                SqlParameter[] _sp = { 
                         new SqlParameter("@StateCode",SqlDbType.Int),
                         new SqlParameter("@FormState",SqlDbType.NVarChar,200),
                         new SqlParameter("@FormID",SqlDbType.Int),
                         new SqlParameter("@FormsID",SqlDbType.Int),
                         new SqlParameter("@ParentID",SqlDbType.Int)
                    };

                _sp[0].Value = StateCode;
                _sp[1].Value = MicroWorkFlow.GetApprovalState(StateCode);
                _sp[2].Value = FormID.toInt();
                _sp[3].Value = FormsID.toInt();
                _sp[4].Value = FormsID.toInt();

                if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
                    flag = true;
            }
            else
            {
                string _sql = "update " + TableName + " set StateCode=@StateCode, FormState=@FormState where Invalid=0 and Del=0 and FormID=@FormID and " + PrimaryKeyName + "=@FormsID ";
                SqlParameter[] _sp = {
                                            new SqlParameter("@StateCode",SqlDbType.Int),
                                            new SqlParameter("@FormState",SqlDbType.NVarChar,200),
                                            new SqlParameter("@FormID",SqlDbType.Int),
                                            new SqlParameter("@FormsID",SqlDbType.Int)
                                            };

                _sp[0].Value = StateCode;
                _sp[1].Value = MicroWorkFlow.GetApprovalState(StateCode);
                _sp[2].Value = FormID.toInt();
                _sp[3].Value = FormsID.toInt();

                if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
                    flag = true;
            }

            return flag;

        }


        /// <summary>
        /// 检查表单状态，通常表单状态小于“0”时才允许修改。审批状态：-40 = 删除[Delete]、-30 = 无效[Invalid]、-4 = 撤回[Withdrawal]、-3 = 填写申请[Fill in]、-2 = 临时保存[Draft]、-1 = 驳回申请[Return]、0 = 等待审批[Waiting]、1 = 审批通过[Pass]、11 = 提交申请[Pass]、15 = 撤回审批[WithdrawalApproval]、18 = 转发[Forward]、22 = 等待受理[WaitingForAccept]、23 = 等待对应[WaitingForProcessing]、24 = 等待处理[WaitingForProcessing]、32 = 受理中[Accepting]、33 = 对应中[Processing]、34 = 处理中[Processing]、100 = 完成[Finish]
        /// </summary>
        /// <param name="ShortTableName"></param>
        /// <param name="FormID"></param>
        /// <param name="FormsID"></param>
        /// <returns></returns>
        public static int GetFormState(string ShortTableName, string FormID, string FormsID)
        {
            //备注与GetFormRecordAttr有重复

            int flag = 0;

            //得到真实表名
            string TableName = MicroPublic.GetTableName(ShortTableName);
            var getTableAttr = MicroDataTable.GetTableAttr(TableName);
            string PrimaryKeyName = getTableAttr.PrimaryKeyName;

            string _sql = "select * from " + TableName + " where Invalid=0 and Del=0 and FormID=@FormID and " + PrimaryKeyName + "=@FormsID ";
            SqlParameter[] _sp = {
                                    new SqlParameter("@FormID",SqlDbType.Int),
                                    new SqlParameter("@FormsID",SqlDbType.Int)
                                    };

            _sp[0].Value = FormID.toInt();
            _sp[1].Value = FormsID.toInt();

            DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];
            if (_dt.Rows.Count > 0)
                flag = _dt.Rows[0]["StateCode"].toInt();

            return flag;

        }

        /// <summary>
        /// 获取表单记录属性，如：状态代码、状态、UID、DisplayName
        /// </summary>
        public class FormRecordAttr
        {

            public string FormNumber 
            {
                set;
                get;
            }

            /// <summary>
            /// 状态代码
            /// </summary>
            public int StateCode
            {
                set;
                get;
            }

            /// <summary>
            /// FormState
            /// </summary>
            public string FormState
            {
                set;
                get;
            }

            /// <summary>
            /// UID
            /// </summary>
            public int UID
            {
                set;
                get;
            }

            /// <summary>
            /// DisplayName
            /// </summary>
            public string DisplayName
            {
                set;
                get;
            }

            /// <summary>
            /// 记录是否存在，存在True否则False
            /// </summary>
            public Boolean IsRecordExists
            {
                set;
                get;
            }

        }

        /// <summary>
        /// 获取表单记录属性，如：状态代码、状态、UID、DisplayName
        /// </summary>
        /// <param name="ShortTableName"></param>
        /// <param name="FormID"></param>
        /// <param name="FormsID"></param>
        /// <returns></returns>
        public static FormRecordAttr GetFormRecordAttr(string ShortTableName, string FormID, string FormsID)
        {
            int stateCode = 0, uID = 0;
            string formNumber = string.Empty, formState = string.Empty, displayName = string.Empty;
            Boolean isRecordExists = false;

            //得到真实表名
            string TableName = MicroPublic.GetTableName(ShortTableName);
            var GetTableAttr = MicroDataTable.GetTableAttr(TableName);
            string PrimaryKeyName = GetTableAttr.PrimaryKeyName;

            string _sql = "select * from " + TableName + " where Invalid=0 and Del=0 and FormID = @FormID and " + PrimaryKeyName + " =@FormsID ";
            SqlParameter[] _sp = {
                                    new SqlParameter("@FormID",SqlDbType.Int),
                                    new SqlParameter("@FormsID",SqlDbType.Int)
                                    };

            _sp[0].Value = FormID.toInt();
            _sp[1].Value = FormsID.toInt();

            DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];
            if (_dt.Rows.Count > 0)
            {
                formNumber = _dt.Rows[0]["FormNumber"].toStringTrim();
                stateCode = _dt.Rows[0]["StateCode"].toInt();
                formState = _dt.Rows[0]["FormState"].toStringTrim();
                uID = _dt.Rows[0]["UID"].toInt();
                displayName = _dt.Rows[0]["DisplayName"].toStringTrim();
                isRecordExists = true;
            }


            var FormRecordAttr = new FormRecordAttr
            {
                FormNumber = formNumber,
                StateCode = stateCode,
                FormState = formState,
                UID = uID,
                DisplayName = displayName,
                IsRecordExists = isRecordExists
            };

            return FormRecordAttr;

        }

        /// <summary>
        /// 传入审批状态代码返回审批状态
        /// </summary>
        /// <param name="StateCode">审批状态：-40 = 删除[Delete]、-30 = 无效[Invalid]、-4 = 撤回[Withdrawal]、-3 = 填写申请[Fill in]、-2 = 临时保存[Draft]、-1 = 驳回申请[Return]、0 = 等待审批[Waiting]、1 = 审批通过[Pass]、11 = 提交申请[Pass]、15 = 撤回审批[WithdrawalApproval]、18 = 转发[Forward]、22 = 等待受理[WaitingForAccept]、23 = 等待对应[WaitingForProcessing]、24 = 等待处理[WaitingForProcessing]、32 = 受理中[Accepting]、33 = 对应中[Processing]、34 = 处理中[Processing]、100 = 完成[Finish]</param>
        /// <returns>审批状态：-40 = 删除[Delete]、-30 = 无效[Invalid]、-4 = 撤回[Withdrawal]、-3 = 填写申请[Fill in]、-2 = 临时保存[Draft]、-1 = 驳回申请[Return]、0 = 等待审批[Waiting]、1 = 审批通过[Pass]、11 = 提交申请[Pass]、15 = 撤回审批[WithdrawalApproval]、18 = 转发[Forward]、22 = 等待受理[WaitingForAccept]、23 = 等待对应[WaitingForProcessing]、24 = 等待处理[WaitingForProcessing]、32 = 受理中[Accepting]、33 = 对应中[Processing]、34 = 处理中[Processing]、100 = 完成[Finish]</returns>
        public static string GetApprovalState(int StateCode)
        {
            string flag = string.Empty;

            switch (StateCode)
            {
                case -40:
                    flag = "删除[Delete]";
                    break;

                case -30:
                    flag = "无效[Invalid]";
                    break;

                case -4:
                    flag = "撤回申请[WithdrawalApply]";  //申请者撤回
                    break;

                case -3:
                    flag = "填写申请[Fill in]";
                    break;

                case -2:
                    flag = "临时保存[Draft]";   //表单状态
                    break;

                case -1:
                    flag = "驳回申请[Return]";  //审批状态、日志状态、表单状态
                    break;

                case 0:
                    flag = "等待审批[Waiting]"; //审批状态、日志状态、表单状态
                    break;

                case 1:
                    flag = "审批通过[Pass]"; //审批状态、日志状态
                    break;

                case 11:
                    flag = "提交申请[Pass]"; //日志状态
                    break;

                case 15:
                    flag = "撤回审批[WithdrawalApproval]"; //审批者、受理者、对应者、结案者进行撤回
                    break;

                case 18:
                    flag = "转发[Forward]"; //审批者、受理者、对应者、结案者进行转发
                    break;

                case 22:
                    flag = "等待受理[WaitingForAccept]";
                    break;

                case 23:
                    flag = "等待对应[WaitingForProcessing]";
                    break;

                case 24:
                    flag = "等待处理[WaitingForProcessing]";
                    break;

                case 32:
                    flag = "受理中[Accepting]";
                    break;

                case 33:
                    flag = "对应中[Processing]";
                    break;

                case 34:
                    flag = "处理中[Processing]";
                    break;

                case 100:
                    flag = "完成[Finish]"; //日志状态、表单状态
                    break;
            }
            return flag;
        }

        /// <summary>
        /// 检查表单需要审批的节点是否审批完成，传入FormID & FormsID，进行条件运算a.CanApprovalUID<>'' and a.ApprovalUID=0 and a.StateCode=0 and b.FixedNode=0判断，如果没有记录代表审批完成
        /// </summary>
        /// <param name="FormID"></param>
        /// <param name="FormsID"></param>
        /// <returns></returns>
        public static Boolean GetIsApprovalFinish(string FormID, string FormsID)
        {
            //注： 该功能包含在FormApprovalRecordsAttr方法中，20200928
            Boolean flag = false;

            string _sql = "select * from FormApprovalRecords a left join WorkFlow b on a.WorkFlowID=b.WFID where a.Invalid=0 and a.Del=0 and a.FormID = @FormID and a.FormsID=@FormsID and a.CanApprovalUID<>'' and a.ApprovalUID=0 and a.StateCode=0 and b.FixedNode=0";
            SqlParameter[] _sp = { new SqlParameter("@FormID", SqlDbType.Int),
                                       new SqlParameter("@FormsID", SqlDbType.Int),
                                 };

            _sp[0].Value = FormID.toInt();
            _sp[1].Value = FormsID.toInt();

            //根据where条件，没有记录时代表审批完成，所以true，时返回true
            //如果有记录返回false
            if (MsSQLDbHelper.Exists(_sql, _sp))
                flag = false;
            //没有记录返回true
            else
                flag = true;


            return flag;

        }

        public class FormApprovalRecordsAttr
        {
            public string FormNumber
            {
                set;
                get;
            }

            //当前节点名称（根据条件获得 CanApprovalUID <> '' and ApprovalUID = 0 and StateCode = 0）
            public string CurrNodeName
            {
                set;
                get;
            }

            /// <summary>
            /// 对需要审批的节点已经审批完成，where FixedNode=1
            /// </summary>
            public Boolean IsApprovalFinish
            {
                set;
                get;
            }

            /// <summary>
            /// WorkFlow源表数据
            /// </summary>
            public DataTable SourceDT
            {
                set;
                get;
            }

            /// <summary>
            /// 源行，FormID = @FormID and a.FormsID=@FormsID
            /// </summary>
            public DataRow[] SourceRows
            {
                set;
                get;
            }

            /// <summary>
            /// 目标行，条件CanApprovalUID <> '' and ApprovalUID = 0 and StateCode = 0 and FixedNode = 0
            /// </summary>
            public DataRow[] TargetRows
            {
                set;
                get;
            }
        }

        /// <summary>
        /// 获取表单审批记录表的属性，如：是否审批完成（指非固定节点），SourceDT、SourceRow、TargetRow
        /// </summary>
        /// <param name="FormID"></param>
        /// <param name="FormsID"></param>
        /// <returns></returns>
        public static FormApprovalRecordsAttr GetFormApprovalRecordsAttr(string FormID, string FormsID)
        {
            string formNumber = string.Empty,
                currNodeName = string.Empty;

            Boolean isApprovalFinish = false;
            DataTable sourceDT = null;
            DataRow[] sourceRows = null;
            DataRow[] targetRows = null;

            string _sql = "select a.*,b.WFID ,b.FlowName ,b.FlowCode ,b.Alias ,b.EffectiveType ,b.EffectiveIDStr ,b.IsConditionApproval ,b.OperField ,b.Condition ,b.OperValue ,b.CustomConditions ,b.ApprovalType ,b.ApprovalIDStr ,b.ApprovalByIDStr ,b.IsSync ,b.Creator ,b.DefaultFlow ,b.FixedNode ,b.Invalid ,b.Del ,b.IsAccept ,b.ApproversSelectedByDefault ,b.ExtraFunction ,b.IsOptionalApproval ,b.IsSpecialApproval ,b.IsVerticalDirection ,b.Description from FormApprovalRecords a left join WorkFlow b on a.WorkFlowID=b.WFID where a.Invalid=0 and a.Del=0 and a.FormID = @FormID and a.FormsID=@FormsID order by a.Sort ";
            SqlParameter[] _sp = { new SqlParameter("@FormID", SqlDbType.Int),
                                       new SqlParameter("@FormsID", SqlDbType.Int)
                                 };

            _sp[0].Value = FormID.toInt();
            _sp[1].Value = FormsID.toInt();

            DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

            if (_dt.Rows.Count > 0)
            {
                formNumber = _dt.Rows[0]["FormNumber"].toStringTrim();
                sourceDT = _dt;
                sourceRows = _dt.Select("", "Sort");

                targetRows = _dt.Select("CanApprovalUID <> '' and ApprovalUID = 0 and StateCode = 0 and FixedNode = 0", "Sort");

                //根据_dt.Select()条件，没有记录时代表审批完成，所以true，时返回true
                //如果有记录返回false
                if (targetRows.Length > 0)
                    isApprovalFinish = false;
                else
                    isApprovalFinish = true;

                //获得当前节点的内容(条件 CanApprovalUID <> '' and ApprovalUID = 0 and StateCode = 0 )
                DataRow[] _rowsCurr = _dt.Select("CanApprovalUID <> '' and ApprovalUID = 0 and StateCode = 0 ", "Sort");
                if (_rowsCurr.Length > 0)
                    currNodeName = _rowsCurr[0]["NodeName"].toStringTrim();

            }

            var FormApprovalRecordsAttr = new FormApprovalRecordsAttr
            {
                FormNumber = formNumber,
                CurrNodeName = currNodeName,
                IsApprovalFinish = isApprovalFinish,
                SourceDT = sourceDT,
                SourceRows = sourceRows,
                TargetRows = targetRows
            };

            return FormApprovalRecordsAttr;

        }

    }
}