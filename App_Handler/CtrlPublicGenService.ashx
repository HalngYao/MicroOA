<%@ WebHandler Language="C#" Class="CtrlPublicGenService" %>

using System;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroDTHelper;
using MicroUserHelper;
using MicroPublicHelper;
using MicroAuthHelper;
using MicroWorkFlowHelper;
using MicroApprovalHelper;
using MicroPrivateHelper;

public class CtrlPublicGenService : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = MicroPublic.GetMsg("CtrlFailed"),
                Action = context.Server.UrlDecode(context.Request.Form["action"]),
                ShortTableName = context.Server.UrlDecode(context.Request.Form["stn"]),
                ModuleID = context.Server.UrlDecode(context.Request.Form["mid"]),
                Type = context.Server.UrlDecode(context.Request.Form["type"]),
                FormID = context.Server.UrlDecode(context.Request.Form["formid"]),
                PrimaryKeyValue = context.Server.UrlDecode(context.Request.Form["pkv"]),
                Value = context.Server.UrlDecode(context.Request.Form["val"]);

        ////测试数据
        //Action = "CtrlTop";
        //Type = "Platform";
        //ShortTableName = "Info";
        //ModuleID = "27";
        //PrimaryKeyValue = "1670";
        //Value = "SetTop";

        if (!string.IsNullOrEmpty(Action) && !string.IsNullOrEmpty(ShortTableName) && !string.IsNullOrEmpty(ModuleID) && !string.IsNullOrEmpty(Type) && !string.IsNullOrEmpty(PrimaryKeyValue))
        {

            //string DelNum = "4";  //系统表单用4，MicroForm用16
            //if (Type == "MicroForm")
            //    DelNum = "16";

            Boolean ApprovalPermit = MicroAuth.CheckPermit(ModuleID, "12"),
            EditPermit = MicroAuth.CheckPermit(ModuleID, "3"),  //修改自己
            EditAllPermit = MicroAuth.CheckPermit(ModuleID, "10");  //修改所有

            //DelPermit = MicroAuth.CheckPermit(ModuleID, DelNum);

            if (Action == "Modify" && Type == "Approval" && ApprovalPermit)
                flag = CtrlInfoState(ShortTableName, PrimaryKeyValue);

            if (Action == "CtrlTop" && !string.IsNullOrEmpty(Value) && (EditPermit || EditAllPermit))
                flag = CtrlPublicTop(Type, ShortTableName, PrimaryKeyValue, Value);

            if (Action == "Del")
                flag = CtrlPublicDel(ShortTableName, ModuleID, Type, FormID, PrimaryKeyValue);

            if (Action == "Dels")
                flag = CtrlPublicDels(ShortTableName, ModuleID, Type, FormID, PrimaryKeyValue);

            if (Action == "Modify" && Type == "GTips" && !string.IsNullOrEmpty(Value))
                flag = CtrlGlobalTips(Value);

        }

        context.Response.Write(flag);
    }

    /// <summary>
    /// 设置信息状态为True，一般用户才能看见
    /// </summary>
    /// <param name="ShortTableName"></param>
    /// <param name="PrimaryKeyValue"></param>
    /// <returns></returns>
    private string CtrlInfoState(string ShortTableName, string PrimaryKeyValue)
    {
        string flag = MicroPublic.GetMsg("CtrlFailed");
        string TableName = MicroPublic.GetTableName(ShortTableName);

        var getTableAttr = MicroDataTable.GetTableAttr(TableName);  //调用方法返回主键属性
        string PrimaryKeyName = getTableAttr.PrimaryKeyName;  //赋值，主键字段名称

        string _sql = "update " + TableName + " set InfoState=1 where " + PrimaryKeyName + "=@PrimaryKeyValue";
        SqlParameter[] _sp = { new SqlParameter("@PrimaryKeyValue", SqlDbType.Int) };

        _sp[0].Value = PrimaryKeyValue.toInt();

        if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
            flag = MicroPublic.GetMsg("Ctrl");

        return flag;
    }


    /// <summary>
    /// 设置Information置顶
    /// </summary>
    /// <param name="Type">HomePage or Platform</param>
    /// <param name="ShortTableName"></param>
    /// <param name="PrimaryKeyValue"></param>
    /// <param name="CtrlValue"></param>
    /// <returns></returns>
    private string CtrlPublicTop(string Type, string ShortTableName, string PrimaryKeyValue, string CtrlValue)
    {
        string flag = MicroPublic.GetMsg("CtrlFailed"),
        PushField = string.Empty, TopField = string.Empty, TopSortField = string.Empty;
        Boolean TopValue = CtrlValue == "SetTop" ? true : false;
        //string TopSortValue = DateTime.Now.ToString("yyyyMMdd") + "0001";

        string TableName = MicroPublic.GetTableName(ShortTableName);

        var getTableAttr = MicroDataTable.GetTableAttr(TableName);  //调用方法返回主键属性
        string PrimaryKeyName = getTableAttr.PrimaryKeyName;  //赋值，主键字段名称

        if (Type == "HomePage")
        {
            PushField = "PushToHomePage";
            TopField = "HomePageTop";
            TopSortField = "HomePageTopSort";
        }

        if (Type == "Platform")
        {
            PushField = "PushToInfoPlatform";
            TopField = "PlatformTop";
            TopSortField = "PlatformTopSort";
        }

        //置顶
        if (TopValue && !string.IsNullOrEmpty(PushField))
        {
            string _sql = "select max(" + TopSortField + ") from " + TableName + " where Invalid=0 and Del=0 and " + PushField + "=1 ";
            int TopSortValue = (MicroPublic.GetSingleField(_sql, 0).toInt()) + 1;

            string _sql2 = "update " + TableName + " set " + TopField + "=1, " + TopSortField + "=@TopSortValue where Invalid=0 and Del=0 and " + PrimaryKeyName + "=@PrimaryKeyValue";
            SqlParameter[] _sp2 = { new SqlParameter("@TopSortValue", SqlDbType.VarChar,500),
                                    new SqlParameter("@PrimaryKeyValue", SqlDbType.Int)
                                    };

            _sp2[0].Value = TopSortValue.ToString();
            _sp2[1].Value = PrimaryKeyValue;

            if (MsSQLDbHelper.ExecuteSql(_sql2, _sp2) > 0)
                flag = MicroPublic.GetMsg("Ctrl");


        }
        else  //取消置顶
        {
            string _sql3 = "update " + TableName + " set " + TopField + "=0 where Invalid=0 and Del=0 and " + PrimaryKeyName + "=@PrimaryKeyValue";
            SqlParameter[] _sp3 = {
                                    new SqlParameter("@PrimaryKeyValue", SqlDbType.Int)
                                    };

            _sp3[0].Value = PrimaryKeyValue;

            if (MsSQLDbHelper.ExecuteSql(_sql3, _sp3) > 0)
                flag = MicroPublic.GetMsg("Ctrl");

        }

        return flag;
    }


    /// <summary>
    /// 公共删除方法传入单个ID的删除（如果被删除的记录所在表属性父子关系的表，则也会带条件删除 and ParentID=@PrimaryKeyValue)
    /// </summary>
    /// <param name="ShortTableName"></param>
    /// <param name="PrimaryKeyValue"></param>
    /// <returns></returns>
    private string CtrlPublicDel(string ShortTableName, string ModuleID, string Type, string FormID, string PrimaryKeyValue)
    {
        string flag = MicroPublic.GetMsg("DelFailed");
        string TableName = MicroPublic.GetTableName(ShortTableName),
                UID = MicroUserInfo.GetUserInfo("UID"),
                DisplayName = MicroUserInfo.GetUserInfo("DisplayName");

        int StateCode = -40;
        string StateName = MicroWorkFlow.GetApprovalState(StateCode);

        Boolean IsPerm = true;

        // DelPermit = MicroAuth.CheckPermit(ModuleID, DelNum);

        //调用删除方法时，Type应该是SystemForm 或 MicroForm，是MicroForm时判断表单是StateCode是否处于允许删除状态。
        //有父子关系的表需要把父记录和子记录一起删除

        var getTableAttr = MicroDataTable.GetTableAttr(TableName);  //调用方法返回主键属性
        string PrimaryKeyName = getTableAttr.PrimaryKeyName;  //赋值，主键字段名称
        Boolean MainSub = getTableAttr.MainSub;

        //系统表单时
        string _sql = "update " + TableName + " set Del=1 where Del=0 and " + PrimaryKeyName + "=@PrimaryKeyValue";

        if (MainSub)
            _sql = "update " + TableName + " set Del=1 where Del=0 and " + PrimaryKeyName + "=@PrimaryKeyValue or (Del=0 and ParentID=@PrimaryKeyValue)";


        //审批表单，需要判断是否在允许删除状态 StateCode<0
        if (Type == "MicroForm")
        {
            //默认紧能删除自己
            string Where = " and UID=" + UID;

            //如果拥有删除所有权限时，则把条件UID设置为空
            if (MicroAuth.CheckPermit(ModuleID, "18"))
                Where = string.Empty;

            _sql = "update " + TableName + " set Del=1, StateCode=" + StateCode + ", FormState='" + StateName + "', DateModified=GETDATE(), UID=" + UID + ", DisplayName='" + DisplayName + "'  where  Del=0 and " + PrimaryKeyName + "=@PrimaryKeyValue and StateCode<0 " + Where;

            if (MainSub)
                _sql = "update " + TableName + " set Del=1, StateCode=" + StateCode + ", FormState='" + StateName + "', DateModified=GETDATE(), UID=" + UID + ", DisplayName='" + DisplayName + "' where Del=0 and " + PrimaryKeyName + "=@PrimaryKeyValue and StateCode<0  " + Where + " or (Del=0 and ParentID=@PrimaryKeyValue " + Where + " )";
        }

        SqlParameter[] _sp = { new SqlParameter("@PrimaryKeyValue", SqlDbType.Int) };
        _sp[0].Value = PrimaryKeyValue.toInt();

        //有删除自己或删除所有权限时才执行删除
        if (MicroAuth.CheckPermit(ModuleID, "16") || MicroAuth.CheckPermit(ModuleID, "18"))
        {
            //在删除休假时（代休）进行事前检查
            string flag2 = MicroPrivate.CheckLeaveDel("", ShortTableName, ModuleID, FormID, PrimaryKeyValue);
            //如果删除的是休假记录，则检查代休删除后是否超出允许的最大值
            if (ShortTableName == "Leave")
                IsPerm = flag2.Substring(0, 4).toBoolean();

            if (IsPerm)
            {
                if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
                {
                    flag = MicroPublic.GetMsg("Del");

                    if (Type == "MicroForm")
                    {
                        var getPreNodeAttr = MicroApproval.GetPreNodeAttr(FormID, PrimaryKeyValue);
                        string PreFormNumber = getPreNodeAttr.FormNumber,
                                PreNodeName = getPreNodeAttr.NodeName,
                                PreDisplayName = getPreNodeAttr.DisplayName;

                        //更新日志
                        MicroWorkFlow.SetApprovalLogs(FormID, PrimaryKeyValue, PreFormNumber, PreNodeName, UID, PreDisplayName, StateCode, "");

                        //如果删除的记录为加班申请表时更新数据
                        if (ShortTableName == "Overtime")
                            MicroPrivate.DelUpdateOvertimeTotal("", ShortTableName, ModuleID, FormID, PrimaryKeyValue, "Parent");

                        //同时删除 FormApprovalRecords
                        string _sql2 = "update FormApprovalRecords set Del=1, StateCode=" + StateCode + ",ApprovalState='" + StateName + "', DateModified=GETDATE(), UID=" + UID + ", DisplayName='" + DisplayName + "' where Del=0 and FormID=@FormID and FormsID=@FormsID ";

                        SqlParameter[] _sp2 = {
                            new SqlParameter("@FormID", SqlDbType.Int),
                            new SqlParameter("@FormsID", SqlDbType.Int) };

                        _sp2[0].Value = FormID.toInt();
                        _sp2[1].Value = PrimaryKeyValue.toInt();

                        MsSQLDbHelper.ExecuteSql(_sql2, _sp2);

                    }

                }
                else
                    flag = MicroPublic.GetMsg("DelFailedNoDelState");  //不在允许删除状态
            }
            else
                flag = flag2;

        }
        else  //否则提示没有权限删除
            flag = MicroPublic.GetMsg("DelFailedNoPerm");


        return flag;

    }


    /// <summary>
    /// 公共删除方批量的删除，删除单条记录（传入逗号分隔的ID，这个方法不能带这样的条件and ParentID=@PrimaryKeyValue，因为传入的ID不一定是父记录的ID）
    /// </summary>
    /// <param name="ShortTableName"></param>
    /// <param name="ModuleID"></param>
    /// <param name="Type"></param>
    /// <param name="PrimaryKeyValue"></param>
    /// <returns></returns>
    private string CtrlPublicDels(string ShortTableName, string ModuleID, string Type, string FormID, string PrimaryKeyValue)
    {
        string flag = MicroPublic.GetMsg("DelFailed");
        string TableName = MicroPublic.GetTableName(ShortTableName),
                UID = MicroUserInfo.GetUserInfo("UID"),
                DisplayName = MicroUserInfo.GetUserInfo("DisplayName");

        int StateCode = -40;
        string StateName = MicroWorkFlow.GetApprovalState(StateCode);
        // DelPermit = MicroAuth.CheckPermit(ModuleID, DelNum);

        //调用删除方法时，Type应该是SystemForm 或 MicroForm，是MicroForm时判断表单是StateCode是否处于允许删除状态。
        //有父子关系的表需要把父记录和子记录一起删除（采用上面的不带s方法 CtrlPublicDel）

        var getTableAttr = MicroDataTable.GetTableAttr(TableName);  //调用方法返回主键属性
        Boolean MainSub = getTableAttr.MainSub;
        string PrimaryKeyName = getTableAttr.PrimaryKeyName;  //赋值，主键字段名称

        //系统表单时
        string _sql = "update " + TableName + " set Del=1 where Del=0 and " + PrimaryKeyName + " in (" + PrimaryKeyValue + ")";

        //审批表单，需要判断是否在允许删除状态 StateCode<0
        if (Type == "MicroForm")
        {
            //默认紧能删除自己
            string Where = " and UID=" + UID;

            //如果拥有删除所有权限时，则把条件UID设置为空
            if (MicroAuth.CheckPermit(ModuleID, "18"))
                Where = string.Empty;

            _sql = "update " + TableName + " set Del=1, StateCode=" + StateCode + ", FormState='" + StateName + "', DateModified=GETDATE(), UID=" + UID + ", DisplayName='" + DisplayName + "' where Del=0 and " + PrimaryKeyName + " in (" + PrimaryKeyValue + ") and StateCode<0 " + Where;

        }

        //有删除自己或删除所有权限时才执行删除
        if (MicroAuth.CheckPermit(ModuleID, "16") || MicroAuth.CheckPermit(ModuleID, "18"))
        {
            if (MsSQLDbHelper.ExecuteSql(_sql) > 0)
            {
                flag = MicroPublic.GetMsg("Del");
                if (Type == "MicroForm")
                {
                    var getPreNodeAttr = MicroApproval.GetPreNodeAttr(FormID, PrimaryKeyValue);
                    string PreFormNumber = getPreNodeAttr.FormNumber,
                            PreNodeName = getPreNodeAttr.NodeName,
                            PreApprovalDisplayName = getPreNodeAttr.ApprovalDisplayName;

                    MicroWorkFlow.SetApprovalLogs(FormID, PrimaryKeyValue, PreFormNumber, PreNodeName, UID, PreApprovalDisplayName, StateCode, "");

                    //如果删除的记录为加班申请表时更新数据
                    if (ShortTableName == "Overtime")
                        MicroPrivate.DelUpdateOvertimeTotal("", ShortTableName, ModuleID, FormID, PrimaryKeyValue);

                    //非Overtime表单才删除审批记录，因为Overtime有父记录，此时调用该方法只是删除子记录，可能还存在其它子记录，所以Overtime不能删除审批记录，而其它表单都是单条记录
                    else
                    {
                        //同时删除 FormApprovalRecords
                        string _sql2 = "update FormApprovalRecords set Del=1, StateCode=" + StateCode + ", ApprovalState='" + StateName + "', DateModified=GETDATE(), UID=" + UID + ", DisplayName='" + DisplayName + "' where Del=0 and FormID=@FormID and FormsID in (@FormsID) ";

                        SqlParameter[] _sp2 = {
                            new SqlParameter("@FormID", SqlDbType.Int),
                            new SqlParameter("@FormsID", SqlDbType.Int) };

                        _sp2[0].Value = FormID.toInt();
                        _sp2[1].Value = PrimaryKeyValue.toInt();

                        MsSQLDbHelper.ExecuteSql(_sql2, _sp2);
                    }

                }
            }
            else
                flag = MicroPublic.GetMsg("DelFailedNoDelState");  //不在允许删除状态

        }
        else  //否则提示没有权限删除
            flag = MicroPublic.GetMsg("DelFailedNoPerm");


        return flag;

    }

    /// <summary>
    /// 全局提示设置
    /// </summary>
    /// <param name="Result"></param>
    /// <returns></returns>
    private string CtrlGlobalTips(string GTips)
    {
        string flag = MicroPublic.GetMsg("SetFailed");
        Boolean GlobalTips = GTips.toBoolean() == true ? false : true;

        int UID = MicroUserInfo.GetUserInfo("UID").toInt();

        string _sql = "update UserInfo set IsGlobalTips=@IsGlobalTips where UID=@UID";
        SqlParameter[] _sp = { new SqlParameter("@IsGlobalTips",SqlDbType.Bit),
                                new SqlParameter("@UID",SqlDbType.Int)
                                };

        _sp[0].Value = GlobalTips;
        _sp[1].Value = UID;

        if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
        {
            flag = MicroPublic.GetMsg("Set");
            flag = flag.Substring(4, flag.Length - 4);

            ((MicroUserInfo)HttpContext.Current.Session["UserInfo"]).IsGlobalTips = GlobalTips;
        }

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