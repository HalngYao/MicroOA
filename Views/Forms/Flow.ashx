<%@ WebHandler Language="C#" Class="Flow" %>

using System;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroPublicHelper;
using MicroUserHelper;
using MicroDTHelper;
using Newtonsoft.Json.Linq;
using MicroAuthHelper;

public class Flow : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        MicroAuth.CheckLogin();

        string flag = MicroPublic.GetMsg("ModifyFailed"),
                    Action = context.Server.UrlDecode(context.Request.Form["action"]),
                    MID = context.Server.UrlDecode(context.Request.Form["mid"]),
                    WFID = context.Server.UrlDecode(context.Request.Form["wfid"]),
                    Fields = context.Server.UrlDecode(context.Request.Form["fields"]);

        Action = Action.ToLower();

        if (Action == "add" && !string.IsNullOrEmpty(Fields))
            flag = AddFlowNode(Fields);

        if (Action == "add2" && !string.IsNullOrEmpty(Fields))
            flag = AddFlowNode2(Fields);

        if (Action == "modify" && !string.IsNullOrEmpty(Fields))
            flag = ModifyFlowName(Fields);

        if (Action == "del" && !string.IsNullOrEmpty(WFID))
            flag = DelFlowNode(WFID);

        if (Action == "modifynote" && !string.IsNullOrEmpty(Fields))
            flag = ModifyNote(Fields);


        context.Response.Write(flag);
    }


    /// <summary>
    /// 添加流程节点，第一次时添加时同时添加流程名称和流程节点
    /// </summary>
    /// <param name="Fields"></param>
    /// <returns></returns>
    private string AddFlowNode(string Fields)
    {
        string flag = MicroPublic.GetMsg("AddFailed");
        try
        {
            string FormID = string.Empty; string FlowName = string.Empty; string IsSync = string.Empty;  string IsAccept = string.Empty; string FlowCode = string.Empty; string EffectiveType = string.Empty; string EffectiveIDStr = string.Empty; string NoteName = string.Empty; string IsConditionApproval = string.Empty; string ApprovalType = string.Empty; string ApprovalIDStr = string.Empty; string ApprovalByIDStr = string.Empty; string OperField = string.Empty; string Condition = string.Empty; string OperValue = string.Empty; string Creator = string.Empty;
            int intSort = 0;
            int intFormID = 0;
            int intParentID = 0;

            Boolean bitIsConditionApproval = false;
            Boolean bitIsSync = false;
            Boolean bitIsAccept = false;
            Boolean bitDefaultFlow = false;

            dynamic json = JToken.Parse(Fields) as dynamic;

            FormID = json["selFormID"];  //表单ID
            intFormID = FormID.toInt();

            //Get DataTable
            DataTable _dt = MicroDataTable.GetDataTable("WFlow");
            int intMaxSort = _dt.Compute("max(Sort)", "ParentID=0 and FormID=" + intFormID).toInt();

            bitDefaultFlow = intMaxSort == 0 ? true : false; //把第一条流程作为默认流程
            intSort = intMaxSort + 1;

            intSort = intSort > 88888 ? 88 : intSort;  //避免Sort超出固定节点的值

            FlowName = json["txtFlowName"];  //流程名称
            FlowName = FlowName.toStringTrim();

            IsSync = json["ckIsSync"]; //同步审批
            bitIsSync = IsSync.toBoolean();

            IsAccept = json["ckIsAccept"]; //启用受理
            bitIsAccept = IsAccept.toBoolean();

            FlowCode = json["txtFlowCode"];  //流程代码，暂时未用到此字段
            FlowCode = FlowCode.toStringTrim();

            EffectiveType = json["selEffectiveType"];  //生效范围、生效类型
            EffectiveType = EffectiveType.toStringTrim();

            EffectiveIDStr = json["selEffectiveIDStr"];   //生效范围的值
            EffectiveIDStr = EffectiveIDStr.toStringTrim();

            NoteName = json["txtNoteName"];  //审批名称
            NoteName = NoteName.toStringTrim();

            IsConditionApproval = json["ckIsConditionApproval"];   //是否启用条件审批
            bitIsConditionApproval = IsConditionApproval.toBoolean();

            ApprovalType = json["selApprovalType"]; //审批类型，按职位、角色、人员
            ApprovalType = ApprovalType.toStringTrim();

            ApprovalIDStr = json["selApprovalIDStr"];  //主要审批人
            ApprovalIDStr = ApprovalIDStr.toStringTrim();

            ApprovalByIDStr = json["selApprovalByIDStr"];  //代理审批人
            ApprovalByIDStr = ApprovalByIDStr.toStringTrim();

            OperField = json["selOperField"];   //条件审核，运算字段
            OperField = OperField.toStringTrim();

            Condition = json["selCondition"];   //条件审核，运算条件
            Condition = Condition.toStringTrim();

            OperValue = json["txtOperValue"];   //条件审核，运算值
            OperValue = OperValue.toStringTrim();

            Creator = ((MicroUserInfo)HttpContext.Current.Session["UserInfo"]).DisplayName;

            if (!string.IsNullOrEmpty(FormID) && !string.IsNullOrEmpty(FlowName))
            {

                //添加流程名称
                string _sql = "insert into WorkFlow (Sort, FormID, ParentID, FlowName, IsSync, FlowCode, EffectiveType, EffectiveIDStr, Creator, DefaultFlow, IsAccept) values ( @Sort, @FormID, @ParentID, @FlowName, @IsSync, @FlowCode, @EffectiveType, @EffectiveIDStr,  @Creator, @DefaultFlow, @IsAccept)";

                SqlParameter[] _sp = { new SqlParameter("@Sort", SqlDbType.Int),
                        new SqlParameter("@FormID", SqlDbType.Int),
                        new SqlParameter("@ParentID", SqlDbType.Int),
                        new SqlParameter("@FlowName", SqlDbType.NVarChar,100),
                        new SqlParameter("@IsSync", SqlDbType.Bit),
                        new SqlParameter("@FlowCode", SqlDbType.VarChar,50),
                        new SqlParameter("@EffectiveType", SqlDbType.VarChar,100),
                        new SqlParameter("@EffectiveIDStr", SqlDbType.VarChar,200),
                        new SqlParameter("@Creator", SqlDbType.NVarChar,50),
                        new SqlParameter("@DefaultFlow", SqlDbType.Bit),
                        new SqlParameter("@IsAccept", SqlDbType.Bit),
                        };

                _sp[0].Value = intSort;
                _sp[1].Value = intFormID;
                _sp[2].Value = intParentID;
                _sp[3].Value = FlowName;
                _sp[4].Value = bitIsSync;
                _sp[5].Value = FlowCode;
                _sp[6].Value = EffectiveType;
                _sp[7].Value = EffectiveIDStr;
                _sp[8].Value = Creator;
                _sp[9].Value = bitDefaultFlow;
                _sp[10].Value = bitIsAccept;

                //流程名称添加成功后再添加流程节点
                if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
                {
                    //Get DataTable
                    DataTable _dt2 = MicroDataTable.GetDataTable("WFlow");
                    int intWFID = _dt2.Compute("max(WFID)", "ParentID=0").toInt();  //得到以"ParentID=0"为条件的最大WFID的值作为流程节点的ParentID
                    intMaxSort = _dt2.Compute("max(Sort)", "ParentID=" + intWFID + " and Sort < 88888").toInt();  //获取流程节点最大的Sort，若当前没有流程节点时返回0
                    intSort = intMaxSort + 1;

                    DataTable _dtInsert = new DataTable();
                    _dtInsert.Columns.Add("Sort", typeof(int));
                    _dtInsert.Columns.Add("FormID", typeof(int));
                    _dtInsert.Columns.Add("ParentID", typeof(int));
                    _dtInsert.Columns.Add("FlowName", typeof(string));
                    _dtInsert.Columns.Add("FlowCode", typeof(string));
                    _dtInsert.Columns.Add("IsConditionApproval", typeof(Boolean));
                    _dtInsert.Columns.Add("ApprovalType", typeof(string));
                    _dtInsert.Columns.Add("ApprovalIDStr", typeof(string));
                    _dtInsert.Columns.Add("ApprovalByIDStr", typeof(string));
                    _dtInsert.Columns.Add("OperField", typeof(string));
                    _dtInsert.Columns.Add("Condition", typeof(string));
                    _dtInsert.Columns.Add("OperValue", typeof(string));
                    _dtInsert.Columns.Add("Creator", typeof(string));
                    _dtInsert.Columns.Add("FixedNode", typeof(Boolean));
                    _dtInsert.Columns.Add("IsAccept", typeof(Boolean));

                    //申请，固定的流程节点
                    DataRow _drInsert = _dtInsert.NewRow();
                    _drInsert["Sort"] = 0;
                    _drInsert["FormID"] = intFormID;
                    _drInsert["ParentID"] = intWFID;
                    _drInsert["FlowName"] = "申请";
                    _drInsert["FlowCode"] = "Apply";
                    _drInsert["IsConditionApproval"] = false;
                    _drInsert["ApprovalType"] = "";
                    _drInsert["ApprovalIDStr"] = "";
                    _drInsert["ApprovalByIDStr"] = "";
                    _drInsert["OperField"] = null;
                    _drInsert["Condition"] = null;
                    _drInsert["OperValue"] = null;
                    _drInsert["Creator"] = Creator;
                    _drInsert["FixedNode"] = true;
                    _drInsert["IsAccept"] = bitIsAccept;

                    _dtInsert.Rows.Add(_drInsert);

                    //由提交页面传递过来的信息
                    DataRow _drInsert2 = _dtInsert.NewRow();
                    _drInsert2["Sort"] = intSort;
                    _drInsert2["FormID"] = intFormID;
                    _drInsert2["ParentID"] = intWFID;
                    _drInsert2["FlowName"] = NoteName;
                    _drInsert2["FlowCode"] = "Approval";
                    _drInsert2["IsConditionApproval"] = bitIsConditionApproval;
                    _drInsert2["ApprovalType"] = ApprovalType;
                    _drInsert2["ApprovalIDStr"] = ApprovalIDStr;
                    _drInsert2["ApprovalByIDStr"] = ApprovalByIDStr;
                    _drInsert2["OperField"] = OperField;
                    _drInsert2["Condition"] = Condition;
                    _drInsert2["OperValue"] = OperValue;
                    _drInsert2["Creator"] = Creator;
                    _drInsert2["FixedNode"] = false;
                    _drInsert2["IsAccept"] = bitIsAccept;

                    _dtInsert.Rows.Add(_drInsert2);

                    //受理，固定的流程节点
                    if (bitIsAccept)
                    {
                        DataRow _drInsert3 = _dtInsert.NewRow();
                        _drInsert3["Sort"] = 888887;
                        _drInsert3["FormID"] = intFormID;
                        _drInsert3["ParentID"] = intWFID;
                        _drInsert3["FlowName"] = "受理";
                        _drInsert3["FlowCode"] = "Accept";
                        _drInsert3["IsConditionApproval"] = false;
                        _drInsert3["ApprovalType"] = "";
                        _drInsert3["ApprovalIDStr"] = "";
                        _drInsert3["ApprovalByIDStr"] = "";
                        _drInsert3["OperField"] = null;
                        _drInsert3["Condition"] = null;
                        _drInsert3["OperValue"] = null;
                        _drInsert3["Creator"] = Creator;
                        _drInsert3["FixedNode"] = true;
                        _drInsert3["IsAccept"] = bitIsAccept;

                        _dtInsert.Rows.Add(_drInsert3);
                    }

                    //完成，固定的流程节点
                    DataRow _drInsert4 = _dtInsert.NewRow();
                    _drInsert4["Sort"] = 888888;
                    _drInsert4["FormID"] = intFormID;
                    _drInsert4["ParentID"] = intWFID;
                    _drInsert4["FlowName"] = "完成";
                    _drInsert4["FlowCode"] = "Finish";
                    _drInsert4["IsConditionApproval"] = false;
                    _drInsert4["ApprovalType"] = "";
                    _drInsert4["ApprovalIDStr"] = "";
                    _drInsert4["ApprovalByIDStr"] = "";
                    _drInsert4["OperField"] = null;
                    _drInsert4["Condition"] = null;
                    _drInsert4["OperValue"] = null;
                    _drInsert4["Creator"] = Creator;
                    _drInsert4["FixedNode"] = true;
                    _drInsert4["IsAccept"] = bitIsAccept;

                    _dtInsert.Rows.Add(_drInsert4);

                    if (MsSQLDbHelper.SqlBulkCopyInsert(_dtInsert, "WorkFlow"))
                        flag = "True" + intWFID;
                }
            }
        }
        catch (Exception ex) { flag = ex.toStringTrim(); }

        return flag;
    }


    /// <summary>
    /// 添加流程节点，第二次添加流程节点时更新流程名称和插入流程节点
    /// </summary>
    /// <param name="Fields"></param>
    /// <returns></returns>
    private string AddFlowNode2(string Fields)
    {
        string flag = MicroPublic.GetMsg("AddFailed");
        try
        {
            string WFID = string.Empty; string FormID = string.Empty; string FlowName = string.Empty; string IsSync = string.Empty; string IsAccept = string.Empty; string FlowCode = string.Empty; string EffectiveType = string.Empty; string EffectiveIDStr = string.Empty; string NoteName = string.Empty; string IsConditionApproval = string.Empty; string ApprovalType = string.Empty; string ApprovalIDStr = string.Empty; string ApprovalByIDStr = string.Empty; string OperField = string.Empty; string Condition = string.Empty; string OperValue = string.Empty;  string Creator = string.Empty;

            int intWFID = 0;
            int intSort = 0;
            int intFormID = 0;
            int intParentID = 0;

            Boolean bitIsConditionApproval = false;
            Boolean bitIsSync = false;
            Boolean bitIsAccept = false;
            Boolean bitDefaultFlow = false;

            dynamic json = JToken.Parse(Fields) as dynamic;

            WFID = json["txtWFID"];
            intWFID = WFID.toInt();

            FormID = json["selFormID"];
            intFormID = FormID.toInt();

            //Get DataTable
            DataTable _dt = MicroDataTable.GetDataTable("WFlow");
            int intMaxSort = _dt.Compute("max(Sort)", "ParentID=0 and FormID=" + intFormID  + " and Sort < 88888").toInt();

            bitDefaultFlow = intMaxSort == 0 ? true : false; //把第一条流程作为默认流程

            //intSort = intMaxSort + 1;
            //intSort = intSort > 88888 ? 888 : intSort;

            FlowName = json["txtFlowName"];
            FlowName = FlowName.toStringTrim();

            IsSync = json["ckIsSync"];
            bitIsSync = IsSync.toBoolean();

            IsAccept = json["ckIsAccept"]; //启用受理
            bitIsAccept = IsAccept.toBoolean();
           
            FlowCode = json["txtFlowCode"];
            FlowCode = FlowCode.toStringTrim();

            EffectiveType = json["selEffectiveType"];
            EffectiveType = EffectiveType.toStringTrim();

            EffectiveIDStr = json["selEffectiveIDStr"];
            EffectiveIDStr = EffectiveIDStr.toStringTrim();

            NoteName = json["txtNoteName"];  //审批名称
            NoteName = NoteName.toStringTrim();

            IsConditionApproval = json["ckIsConditionApproval"];   //是否启用条件审批
            bitIsConditionApproval = IsConditionApproval.toBoolean();

            ApprovalType = json["selApprovalType"];
            ApprovalType = ApprovalType.toStringTrim();

            ApprovalIDStr = json["selApprovalIDStr"];
            ApprovalIDStr = ApprovalIDStr.toStringTrim();

            ApprovalByIDStr = json["selApprovalByIDStr"];
            ApprovalByIDStr = ApprovalByIDStr.toStringTrim();

            OperField = json["selOperField"];   //条件审核，运算字段
            OperField = OperField.toStringTrim();

            Condition = json["selCondition"];   //条件审核，运算条件
            Condition = Condition.toStringTrim();

            OperValue = json["txtOperValue"];   //条件审核，运算值
            OperValue = OperValue.toStringTrim();

            Creator = ((MicroUserInfo)HttpContext.Current.Session["UserInfo"]).DisplayName;

            if (!string.IsNullOrEmpty(WFID) && !string.IsNullOrEmpty(FlowName))
            {
                //更新流程名称
                string _sql = "update WorkFlow set FlowName=@FlowName, IsSync=@IsSync, IsAccept=@IsAccept, EffectiveType=@EffectiveType, EffectiveIDStr=@EffectiveIDStr, DateModified=@DateModified where WFID=@WFID";

                SqlParameter[] _sp = {
                        new SqlParameter("@FlowName", SqlDbType.NVarChar,100),
                        new SqlParameter("@IsSync", SqlDbType.Bit),
                        new SqlParameter("@IsAccept", SqlDbType.Bit),
                        new SqlParameter("@EffectiveType", SqlDbType.VarChar,100),
                        new SqlParameter("@EffectiveIDStr", SqlDbType.VarChar,200),
                        new SqlParameter("@DateModified", SqlDbType.DateTime),
                        new SqlParameter("@WFID", SqlDbType.Int),
                        };

                _sp[0].Value = FlowName;
                _sp[1].Value = bitIsSync;
                _sp[2].Value = bitIsAccept;
                _sp[3].Value = EffectiveType;
                _sp[4].Value = EffectiveIDStr;
                _sp[5].Value = DateTime.Now;
                _sp[6].Value = intWFID;

                //流程名称更新成功后再添加流程节点
                if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
                {
                    //Get DataTable
                    DataTable _dt2 = MicroDataTable.GetDataTable("WFlow");
                    intParentID = intWFID;
                    intMaxSort = _dt2.Compute("max(Sort)", "ParentID=" + intParentID + " and Sort < 88888").toInt();  //获取流程节点最大的Sort，若当前没有流程节点时返回0
                    intSort = intMaxSort + 1;
                    intSort = intSort > 88888 ? 88 : intSort;

                    string _sql2 = "insert into WorkFlow (Sort, FormID, ParentID, FlowName, FlowCode, IsConditionApproval, ApprovalType, ApprovalIDStr, ApprovalByIDStr, OperField, Condition, OperValue, Creator, IsAccept) values ( @Sort, @FormID, @ParentID, @FlowName, @FlowCode, @IsConditionApproval, @ApprovalType, @ApprovalIDStr, @ApprovalByIDStr, @OperField, @Condition, @OperValue, @Creator, @IsAccept)";

                    SqlParameter[] _sp2 = { new SqlParameter("@Sort", SqlDbType.Int),
                    new SqlParameter("@FormID", SqlDbType.Int),
                    new SqlParameter("@ParentID", SqlDbType.Int),
                    new SqlParameter("@FlowName", SqlDbType.NVarChar,100),
                    new SqlParameter("@FlowCode", SqlDbType.VarChar,50),
                    new SqlParameter("@IsConditionApproval", SqlDbType.Bit),
                    new SqlParameter("@ApprovalType", SqlDbType.VarChar,100),
                    new SqlParameter("@ApprovalIDStr", SqlDbType.VarChar,200),
                    new SqlParameter("@ApprovalByIDStr", SqlDbType.VarChar,200),
                    new SqlParameter("@OperField", SqlDbType.VarChar,100),
                    new SqlParameter("@Condition", SqlDbType.VarChar,10),
                    new SqlParameter("@OperValue", SqlDbType.VarChar,100),
                    new SqlParameter("@Creator", SqlDbType.NVarChar,50),
                    new SqlParameter("@IsAccept", SqlDbType.Bit)
                    };
                    _sp2[0].Value = intSort;
                    _sp2[1].Value = intFormID;
                    _sp2[2].Value = intParentID;
                    _sp2[3].Value = NoteName;
                    _sp2[4].Value = "Approval";
                    _sp2[5].Value = bitIsConditionApproval;
                    _sp2[6].Value = ApprovalType;
                    _sp2[7].Value = ApprovalIDStr;
                    _sp2[8].Value = ApprovalByIDStr;
                    _sp2[9].Value = OperField;
                    _sp2[10].Value = Condition;
                    _sp2[11].Value = OperValue;
                    _sp2[12].Value = Creator;
                    _sp2[13].Value = bitIsAccept;

                    if (MsSQLDbHelper.ExecuteSql(_sql2, _sp2) > 0)
                        flag = MicroPublic.GetMsg("Add");
                }
            }
        }
        catch (Exception ex) { flag = ex.ToString(); }

        return flag;
    }

    /// <summary>
    /// 修改流程名称，不涉及流程节点
    /// </summary>
    /// <param name="Fields"></param>
    /// <returns></returns>
    private string ModifyFlowName(string Fields)
    {
        string flag = MicroPublic.GetMsg("ModifyFailed");
        try
        {
            string WFID = string.Empty; string FormID = string.Empty; string FlowName = string.Empty; string FlowCode = string.Empty; string EffectiveType = string.Empty; string EffectiveIDStr = string.Empty; string ApprovalType = string.Empty; string ApprovalIDStr = string.Empty; string ApprovalByIDStr = string.Empty; string IsSync = string.Empty; string IsAccept = string.Empty; string Creator = string.Empty;

            int intWFID = 0;
            int intFormID = 0;

            Boolean bitIsSync = false;
            Boolean bitIsAccept = false;

            dynamic json = JToken.Parse(Fields) as dynamic;

            WFID = json["txtWFID"];
            intWFID = WFID.toInt();

            FormID = json["selFormID"];
            intFormID = FormID.toInt();

            FlowName = json["txtFlowName"];
            FlowName = FlowName.toStringTrim();

            FlowCode = json["txtFlowCode"];
            FlowCode = FlowCode.toStringTrim();

            EffectiveType = json["selEffectiveType"];
            EffectiveType = EffectiveType.toStringTrim();

            EffectiveIDStr = json["selEffectiveIDStr"];
            EffectiveIDStr = EffectiveIDStr.toStringTrim();

            ApprovalType = json["selApprovalType"];
            ApprovalType = ApprovalType.toStringTrim();

            ApprovalIDStr = json["selApprovalIDStr"];
            ApprovalIDStr = ApprovalIDStr.toStringTrim();

            ApprovalByIDStr = json["selApprovalByIDStr"];
            ApprovalByIDStr = ApprovalByIDStr.toStringTrim();

            IsAccept = json["ckIsAccept"]; //启用受理
            bitIsAccept = IsAccept.toBoolean();

            IsSync = json["ckIsSync"];
            bitIsSync = IsSync.toBoolean();

            Creator = ((MicroUserInfo)HttpContext.Current.Session["UserInfo"]).DisplayName;

            if (!string.IsNullOrEmpty(WFID) && !string.IsNullOrEmpty(FlowName))
            {
                //紧更新流程名称，不涉及流程节点
                string _sql = "update WorkFlow set FlowName=@FlowName, IsAccept=@IsAccept, IsSync=@IsSync, EffectiveType=@EffectiveType, EffectiveIDStr=@EffectiveIDStr, DateModified=@DateModified where WFID=@WFID";

                SqlParameter[] _sp = {
                        new SqlParameter("@FlowName", SqlDbType.NVarChar,100),
                        new SqlParameter("@IsAccept", SqlDbType.Bit),
                        new SqlParameter("@IsSync", SqlDbType.Bit),
                        new SqlParameter("@EffectiveType", SqlDbType.VarChar,100),
                        new SqlParameter("@EffectiveIDStr", SqlDbType.VarChar,200),
                        new SqlParameter("@DateModified", SqlDbType.DateTime),
                        new SqlParameter("@WFID", SqlDbType.Int),
                        };

                _sp[0].Value = FlowName;
                _sp[1].Value = bitIsAccept;
                _sp[2].Value = bitIsSync;
                _sp[3].Value = EffectiveType;
                _sp[4].Value = EffectiveIDStr;
                _sp[5].Value = DateTime.Now;
                _sp[6].Value = intWFID;

                if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
                    flag = MicroPublic.GetMsg("Modify");
            }
        }
        catch (Exception ex) { flag = ex.toStringTrim(); }

        return flag;
    }


    /// <summary>
    /// 删除流程节点
    /// </summary>
    /// <param name="WFID"></param>
    /// <returns></returns>
    private string DelFlowNode(string WFID)
    {
        string flag = MicroPublic.GetMsg("DelFailed");

        string _sql = "update WorkFlow set Del=@Del where WFID=@WFID";
        SqlParameter[] _sp = { new SqlParameter("@Del",SqlDbType.Bit),
                            new SqlParameter("@WFID",SqlDbType.Int)
                            };
        _sp[0].Value = true;
        _sp[1].Value = WFID.toInt();

        if (MsSQLDbHelper.ExecuteSql(_sql,_sp) > 0)
            flag = MicroPublic.GetMsg("Del");

        return flag;
    }

    /// <summary>
    /// 修改节点，通过编辑单元格修改
    /// </summary>
    /// <param name="Fields"></param>
    /// <returns></returns>
    private static string ModifyNote(string Fields)
    {

        string flag = MicroPublic.GetMsg("SaveFailed"), ShortTableName = string.Empty, IDName = string.Empty, IDValue = string.Empty, FieldName = string.Empty, FieldValue = string.Empty, _sql = string.Empty;

        try
        {
            dynamic json = JToken.Parse(Fields) as dynamic;
            ShortTableName = json["STN"];
            IDName = json["IDName"];
            IDValue = json["IDValue"];
            FieldName = json["FieldName"];
            FieldValue = json["FieldValue"];
            FieldValue = FieldValue.toStringTrim();

            string TableName = MicroPublic.GetTableName(ShortTableName);

            _sql = "update " + TableName + " set " + FieldName + "=@" + FieldName + " , DateModified = @DateModified where " + IDName + "=@" + IDName + " ";

            SqlParameter[] _sp = new SqlParameter[3];
            switch (FieldName)
            {
                case "Sort":
                    _sp[0] = new SqlParameter("@" + FieldName, SqlDbType.Int);
                    _sp[0].Value = FieldValue.toInt();
                    break;
                case "FlowName":
                    _sp[0] = new SqlParameter("@" + FieldName, SqlDbType.NVarChar, 100);
                    _sp[0].Value = FieldValue;
                    break;
                case "FlowCode":
                    _sp[0] = new SqlParameter("@" + FieldName, SqlDbType.NVarChar, 100);
                    _sp[0].Value = FieldValue;
                    break;
                case "ApprovalIDStrSort":
                    _sp[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "ApprovalByIDStrSort":
                    _sp[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "ApproversSelectedByDefault":
                    _sp[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "IsOptionalApproval":
                    _sp[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "IsVerticalDirection":
                    _sp[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "FindRange":
                    _sp[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "FindGMOffice":
                    _sp[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "IsSpecialApproval":
                    _sp[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "IsConditionApproval":
                    _sp[0] = new SqlParameter("@" + FieldName, SqlDbType.Bit);
                    _sp[0].Value = FieldValue.ToLower() == "true" ? true : false;
                    break;
                case "OperField":
                    _sp[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 100);
                    _sp[0].Value = FieldValue;
                    break;
                case "Condition":
                    _sp[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 10);
                    _sp[0].Value = FieldValue;
                    break;
                case "OperValue":
                    _sp[0] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, 100);
                    _sp[0].Value = FieldValue;
                    break;
                case "Description":
                    _sp[0] = new SqlParameter("@" + FieldName, SqlDbType.NVarChar, 4000);
                    _sp[0].Value = FieldValue;
                    break;
            }

            _sp[1] = new SqlParameter("@DateModified", SqlDbType.DateTime);
            _sp[1].Value = DateTime.Now;

            _sp[2] = new SqlParameter("@" + IDName, SqlDbType.Int);
            _sp[2].Value = IDValue.toInt();

            if (MsSQLDbHelper.ExecuteSql(_sql, _sp) > 0)
                flag = MicroPublic.GetMsg("Save");
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