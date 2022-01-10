<%@ WebHandler Language="C#" Class="CreateMicroDataTable" %>

using System;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using MicroPublicHelper;
using MicroDBHelper;
using Newtonsoft.Json.Linq;


public class CreateMicroDataTable : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        string flag = "提交失败，请检查URL参数<br/>The submission failed. Check the URL parameters",
                        Action = context.Server.UrlDecode(context.Request.Form["action"]),
                        MID = context.Server.UrlDecode(context.Request.Form["mid"]),
                        Fields = context.Server.UrlDecode(context.Request.Form["fields"]);

        if (!string.IsNullOrEmpty(Action) && !string.IsNullOrEmpty(MID) && !string.IsNullOrEmpty(Fields))
        {
            if (Action.Trim().ToLower() == "add")
                flag = CreateDataTable(MID.Trim(), Fields.Trim());
        }

        context.Response.Write(flag);

    }

    /// <summary>
    /// 创建数据表
    /// </summary>
    /// <param name="mid"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    private string CreateDataTable(string MID, string Fields)
    {
        string flag = "创建失败<br/>Failed to create", TableName = string.Empty, ShortTableName = string.Empty, ApprovalForm = string.Empty, BasicInfo = string.Empty, Dept = string.Empty, PCInfo = string.Empty, TitleContent = string.Empty, ApplicationType = string.Empty, HopeDate = string.Empty, Title = string.Empty, tbDone = string.Empty, tbMainSub = string.Empty, PrimaryKeyName = string.Empty, Description = string.Empty;
        Boolean bitMainSub = false;

        string createApprovalForm = string.Empty, createBasicInfo = string.Empty, createDept = string.Empty, createPCInfo = string.Empty, createTitleContent = string.Empty, createApplicationType = string.Empty, createHopeDate = string.Empty;

        try
        {
            dynamic json = JToken.Parse(Fields) as dynamic;
            TableName = json["txtTableName"];
            ShortTableName = json["txtShortTableName"];

            ApprovalForm = json["ckApprovalForm"];   //创建： FormID int NULL, FormNumber varchar (200) NULL, FormState nvarchar (200) NULL, StateCode int NULL,
            BasicInfo = json["ckBasicInfo"];  //创建： Applicant nvarchar (200) NULL, Phone varchar (30) NULL, Tel varchar (200) NULL, UID int NULL, DisplayName nvarchar (200) NULL,
            Dept = json["ckDept"];   //创建： DeptID int NULL,
            PCInfo = json["ckPCInfo"]; //创建：PCLoginID varchar (200) NULL, PCName varchar (100) NULL, Mail varchar (200) NULL,

            TitleContent = json["ckTitleContent"];  //创建： Title nvarchar (500) NULL, Content ntext NULL,
            ApplicationType = json["ckApplicationType"];  //创建： ApplicationType varchar (100) NULL, ApplicationDate datetime NULL,
            HopeDate = json["ckHopeDate"];  //创建： ApplicationDate datetime NULL,

            Title = json["txtTitle"];
            tbMainSub = json["cktbMainSub"];
            bitMainSub = tbMainSub.toBoolean();
            PrimaryKeyName = json["txtPrimaryKeyName"];
            Description = json["txtDescription"];


            if (ApprovalForm.toBoolean())
                createApprovalForm = "FormID int NULL, FormNumber varchar (200) NULL, FormState nvarchar (200) NULL, StateCode int NULL, IP varchar (50) NULL,";

            if (BasicInfo.toBoolean())
                createBasicInfo = "Applicant nvarchar (200) NULL, Phone varchar (30) NULL, Tel varchar (30) NULL, UID int NULL, DisplayName nvarchar (200) NULL,";

            if (Dept.toBoolean())
                createDept = "DeptID int NULL,";

            if (PCInfo.toBoolean())
                createPCInfo = "PCLoginID varchar (200) NULL, PCName varchar (100) NULL, Mail varchar (200) NULL,";

            if (TitleContent.toBoolean())
                createTitleContent = "Title nvarchar (500) NULL, Content ntext NULL,";

            if (ApplicationType.toBoolean())
                createApplicationType = "ApplicationTypeID int NULL, ApplicationDate datetime NULL,";

            if (HopeDate.toBoolean())
                createHopeDate = "HopeDate datetime NULL,";


            //开启父子项时把父项加粗代码写入[tbDone]中
            if (bitMainSub)
                tbDone = "$('table tr').each(function () {  var s = $(this).children().eq(1).text(); if (s === 'Main')  $(this).toggleClass('ws-datatable-tr');  });";
            else
                tbDone = null;

            if (!string.IsNullOrEmpty(TableName) && !string.IsNullOrEmpty(ShortTableName) && !string.IsNullOrEmpty(PrimaryKeyName))
            {
                if (!MsSQLDbHelper.TableExists(TableName))  //检查表是否存在
                {
                    if (!MsSQLDbHelper.MicroTablesFieldExists("ShortTableName", ShortTableName))  //检查短表名是否存在，不存在时向下执行
                    {
                        //执行创建表命令（同时默认创建Sort,Invalid,Del字段）
                        string _sql = "create table " + TableName + " (" + PrimaryKeyName + " int IDENTITY (1,1) NOT NULL PRIMARY KEY, Sort int NULL, " + createApprovalForm + createBasicInfo + createDept + createPCInfo + createTitleContent + createApplicationType + createHopeDate + " SaveDraft bit NULL Default 0, DateCreated datetime NULL Default getdate(), DateModified datetime NULL Default getdate(), Invalid bit NULL Default 0, Del bit Null Default 0)";

                        //当开启父子关系时（同时创建ParentID,Level,LevelCode字段）
                        if (bitMainSub)
                            _sql = "create table " + TableName + " (" + PrimaryKeyName + " int IDENTITY (1,1) NOT NULL PRIMARY KEY, Sort int NULL, ParentID int NULL, Level int NULL, LevelCode varchar (200) NULL, " + createApprovalForm + createBasicInfo + createDept + createPCInfo + createTitleContent + createApplicationType + createHopeDate + " SaveDraft bit NULL Default 0, DateCreated datetime NULL Default getdate(), DateModified datetime NULL Default getdate(), Invalid bit NULL Default 0, Del bit NULL Default 0)";

                        MsSQLDbHelper.ExecuteSql(_sql);

                        int intSort = MicroPublic.GetSingleField("select MAX(Sort) as Sort from MicroTables where ParentID=0", 0).toInt();

                        //将表名插入源数据表
                        string _sql2 = "insert into MicroTables " +
                        "(Sort,ParentID,TabColName,ShortTableName,Title,Description,tbElem,tbURL,tbToolbar,tbDefaultToolbar,tbTitle,tbText,tbPage,tbLimit,tbLimits,tbDone,tbMainSub,colType,ctlFormStyle) values " +
                       "(@Sort,@ParentID,@TabColName,@ShortTableName,@Title,@Description,@tbElem,@tbURL,@tbToolbar,@tbDefaultToolbar,@tbTitle,@tbText,@tbPage,@tbLimit,@tbLimits,@tbDone,@tbMainSub,@colType,@ctlFormStyle)";
                        SqlParameter[] _sp2 = {
                            new SqlParameter("@Sort",SqlDbType.Int),
                            new SqlParameter("@ParentID",SqlDbType.Int),
                            new SqlParameter("@TabColName", SqlDbType.VarChar,100),
                            new SqlParameter("@ShortTableName", SqlDbType.VarChar,100),
                            new SqlParameter("@Title", SqlDbType.VarChar,100),
                            new SqlParameter("@Description", SqlDbType.NVarChar,4000),
                            new SqlParameter("@tbElem", SqlDbType.VarChar,100),
                            new SqlParameter("@tbURL", SqlDbType.VarChar,500),
                            new SqlParameter("@tbToolbar", SqlDbType.VarChar,1000),
                            new SqlParameter("@tbDefaultToolbar", SqlDbType.VarChar,200),
                            new SqlParameter("@tbTitle", SqlDbType.VarChar,200),
                            new SqlParameter("@tbText", SqlDbType.VarChar,200),
                            new SqlParameter("@tbPage", SqlDbType.Bit),
                            new SqlParameter("@tbLimit", SqlDbType.Int),
                            new SqlParameter("@tbLimits", SqlDbType.VarChar,200),
                            new SqlParameter("@tbDone", SqlDbType.VarChar,5000),
                            new SqlParameter("@tbMainSub", SqlDbType.Bit),
                            new SqlParameter("@colType", SqlDbType.VarChar,20),
                            new SqlParameter("@ctlFormStyle", SqlDbType.VarChar,50)
                        };
                        _sp2[0].Value = intSort + 1;
                        _sp2[1].Value = 0;
                        _sp2[2].Value = TableName.toStringTrim();
                        _sp2[3].Value = ShortTableName.toStringTrim();
                        _sp2[4].Value = Title.toStringTrim();
                        _sp2[5].Value = Description.toStringTrim();
                        _sp2[6].Value = "#tabTable";
                        _sp2[7].Value = "/App_Handler/GetPublicTableList.ashx";
                        _sp2[8].Value = "true";
                        _sp2[9].Value = "['filter', 'print', 'exports']";
                        _sp2[10].Value = Title.toStringTrim();
                        _sp2[11].Value = "No Data. 没有数据。";
                        _sp2[12].Value = true;
                        _sp2[13].Value = 10;
                        _sp2[14].Value = "[10,20,30,40,50,60,70,80,90]";
                        _sp2[15].Value = tbDone;
                        _sp2[16].Value = bitMainSub;
                        _sp2[17].Value = "checkbox";
                        _sp2[18].Value = "Default";


                        //将主键插入全局数据表，并且ParentID与表名TID关联
                        if (MsSQLDbHelper.ExecuteSql(_sql2, _sp2) > 0)
                        {
                            int intParentID = MsSQLDbHelper.GetMaxNumber("TID", "MicroTables");

                            //要插入数据的字段
                            DataTable _dtInsert = new DataTable();
                            _dtInsert.Columns.Add("Sort", typeof(int));
                            _dtInsert.Columns.Add("ParentID", typeof(int));
                            _dtInsert.Columns.Add("TabColName", typeof(string));
                            _dtInsert.Columns.Add("Title", typeof(string));
                            _dtInsert.Columns.Add("DataType", typeof(string));
                            _dtInsert.Columns.Add("FieldLength", typeof(string));  //..
                            _dtInsert.Columns.Add("DefaultValue", typeof(string));
                            _dtInsert.Columns.Add("AllowNull", typeof(Boolean));
                            _dtInsert.Columns.Add("PrimaryKey", typeof(Boolean));
                            _dtInsert.Columns.Add("Description", typeof(string));
                            _dtInsert.Columns.Add("colCustomSort", typeof(int));
                            _dtInsert.Columns.Add("colTitle", typeof(string));
                            _dtInsert.Columns.Add("colCustomField", typeof(string));
                            _dtInsert.Columns.Add("colAlign", typeof(string));
                            _dtInsert.Columns.Add("colEdit", typeof(string));
                            _dtInsert.Columns.Add("colTemplet", typeof(string));
                            _dtInsert.Columns.Add("ctlAddDisplay", typeof(Boolean));
                            _dtInsert.Columns.Add("ctlModifyDisplay", typeof(Boolean));
                            _dtInsert.Columns.Add("ctlViewDisplay", typeof(Boolean));
                            _dtInsert.Columns.Add("ctlTitle", typeof(string));
                            _dtInsert.Columns.Add("ctlTypes", typeof(string));
                            _dtInsert.Columns.Add("ctlPrefix", typeof(string));
                            _dtInsert.Columns.Add("ctlStyle", typeof(string));
                            _dtInsert.Columns.Add("ctlDescription", typeof(string));
                            _dtInsert.Columns.Add("ctlSourceTable", typeof(string));
                            _dtInsert.Columns.Add("ctlTextName", typeof(string));
                            _dtInsert.Columns.Add("ctlTextValue", typeof(string));
                            _dtInsert.Columns.Add("ctlFilter", typeof(string));
                            _dtInsert.Columns.Add("ctlVerify", typeof(string));
                            _dtInsert.Columns.Add("ctlVerify2", typeof(string));
                            _dtInsert.Columns.Add("ctlValue", typeof(string));//...
                            _dtInsert.Columns.Add("ctlDefaultValue", typeof(string));
                            _dtInsert.Columns.Add("ctlCharLength", typeof(string));  //...
                            _dtInsert.Columns.Add("ctlExtendJSCode", typeof(string));  //...
                            _dtInsert.Columns.Add("ctlGroup", typeof(int));
                            _dtInsert.Columns.Add("ctlInputInline", typeof(string));
                            _dtInsert.Columns.Add("JoinTableColumn", typeof(Boolean));


                            //主键
                            DataRow _drInsert = _dtInsert.NewRow();
                            _drInsert["Sort"] = 1;
                            _drInsert["ParentID"] = intParentID;
                            _drInsert["TabColName"] = PrimaryKeyName;
                            _drInsert["Title"] = "主键ID";
                            _drInsert["DataType"] = "int";
                            _drInsert["FieldLength"] = null;  //...
                            _drInsert["DefaultValue"] = null;
                            _drInsert["AllowNull"] = false;
                            _drInsert["PrimaryKey"] = true;
                            _drInsert["Description"] = "主键，字段类型int，默认标识1，自动递增1";
                            _drInsert["colCustomSort"] = 1;
                            _drInsert["colTitle"] = "主键ID";
                            _drInsert["colCustomField"] = null;
                            _drInsert["colAlign"] = "center";
                            _drInsert["colEdit"] = null;
                            _drInsert["colTemplet"] = null;
                            _drInsert["ctlAddDisplay"] = false;
                            _drInsert["ctlModifyDisplay"] = true;
                            _drInsert["ctlViewDisplay"] = true;
                            _drInsert["ctlTitle"] = PrimaryKeyName;
                            _drInsert["ctlTypes"] = "Hidden";
                            _drInsert["ctlPrefix"] = "hid";
                            _drInsert["ctlStyle"] = null;
                            _drInsert["ctlDescription"] = "主键，字段类型int，默认标识1，自动递增1";
                            _drInsert["ctlSourceTable"] = null;
                            _drInsert["ctlTextName"] = null;
                            _drInsert["ctlTextValue"] = null;
                            _drInsert["ctlFilter"] = null;
                            _drInsert["ctlVerify"] = null;
                            _drInsert["ctlVerify2"] = null;
                            _drInsert["ctlValue"] = null; //..
                            _drInsert["ctlDefaultValue"] = null;
                            _drInsert["ctlCharLength"] = null; //..
                            _drInsert["ctlExtendJSCode"] = null; //..
                            _drInsert["ctlGroup"] = 1;
                            _drInsert["ctlInputInline"] = "layui-input-inline";
                            _drInsert["JoinTableColumn"] = true;
                            _dtInsert.Rows.Add(_drInsert);

                            //排序
                            DataRow _drInsert2 = _dtInsert.NewRow();
                            _drInsert2["Sort"] = 2;
                            _drInsert2["ParentID"] = intParentID;
                            _drInsert2["TabColName"] = "Sort";
                            _drInsert2["Title"] = "排序";
                            _drInsert2["DataType"] = "int";
                            _drInsert2["FieldLength"] = null;  //...
                            _drInsert2["DefaultValue"] = null;
                            _drInsert2["AllowNull"] = true;
                            _drInsert2["PrimaryKey"] = false;
                            _drInsert2["Description"] = "排序";
                            _drInsert2["colCustomSort"] = 2;
                            _drInsert2["colTitle"] = "排序";
                            _drInsert2["colCustomField"] = null;
                            _drInsert2["colAlign"] = "center";
                            _drInsert2["colEdit"] = "text";
                            _drInsert2["colTemplet"] = null;
                            _drInsert2["ctlAddDisplay"] = true;
                            _drInsert2["ctlModifyDisplay"] = true;
                            _drInsert2["ctlViewDisplay"] = true;
                            _drInsert2["ctlTitle"] = "排序";
                            _drInsert2["ctlTypes"] = "Text";
                            _drInsert2["ctlPrefix"] = "txt";
                            _drInsert2["ctlStyle"] = null;
                            _drInsert2["ctlDescription"] = "数字类型，数字越小排序越靠前";
                            _drInsert2["ctlSourceTable"] = null;
                            _drInsert2["ctlTextName"] = null;
                            _drInsert2["ctlTextValue"] = null;
                            _drInsert2["ctlFilter"] = null;
                            _drInsert2["ctlVerify"] = "onlyNum";
                            _drInsert2["ctlVerify2"] = null;
                            _drInsert2["ctlValue"] = null;//..
                            _drInsert2["ctlDefaultValue"] = null;
                            _drInsert2["ctlCharLength"] = null; //..
                            _drInsert2["ctlExtendJSCode"] = null; //..
                            _drInsert2["ctlGroup"] = 2;
                            _drInsert2["ctlInputInline"] = "layui-input-inline";
                            _drInsert2["JoinTableColumn"] = true;
                            _dtInsert.Rows.Add(_drInsert2);

                            if (bitMainSub)
                            {
                                //上级
                                DataRow _drInsert3 = _dtInsert.NewRow();
                                _drInsert3["Sort"] = 3;
                                _drInsert3["ParentID"] = intParentID;
                                _drInsert3["TabColName"] = "ParentID";
                                _drInsert3["Title"] = "上级";
                                _drInsert3["DataType"] = "int";
                                _drInsert3["FieldLength"] = null;  //...
                                _drInsert3["DefaultValue"] = null;
                                _drInsert3["AllowNull"] = true;
                                _drInsert3["PrimaryKey"] = false;
                                _drInsert3["Description"] = "如上级为空时，则该级别为顶级";
                                _drInsert3["colCustomSort"] = 3;
                                _drInsert3["colTitle"] = "上级";
                                _drInsert3["colCustomField"] = null;
                                _drInsert3["colAlign"] = "center";
                                _drInsert3["colEdit"] = null;
                                _drInsert3["colTemplet"] = null;
                                _drInsert3["ctlAddDisplay"] = true;
                                _drInsert3["ctlModifyDisplay"] = true;
                                _drInsert3["ctlViewDisplay"] = true;
                                _drInsert3["ctlTitle"] = "上级";
                                _drInsert3["ctlTypes"] = "Select";
                                _drInsert3["ctlPrefix"] = "sel";
                                _drInsert3["ctlStyle"] = null;
                                _drInsert3["ctlDescription"] = "如上级为空时，则该级别为顶级";
                                _drInsert3["ctlSourceTable"] = null;
                                _drInsert3["ctlTextName"] = null;
                                _drInsert3["ctlTextValue"] =DBNull.Value;
                                _drInsert3["ctlFilter"] = null;
                                _drInsert3["ctlVerify"] = null;
                                _drInsert3["ctlVerify2"] = null;
                                _drInsert3["ctlValue"] = null;//..
                                _drInsert3["ctlDefaultValue"] = null;
                                _drInsert3["ctlCharLength"] = null; //..
                                _drInsert3["ctlExtendJSCode"] = null; //..
                                _drInsert3["ctlGroup"] = 3;
                                _drInsert3["ctlInputInline"] = "layui-input-inline";
                                _drInsert3["JoinTableColumn"] = true;
                                _dtInsert.Rows.Add(_drInsert3);

                                //层级数值
                                DataRow _drInsert993 = _dtInsert.NewRow();
                                _drInsert993["Sort"] = 993;
                                _drInsert993["ParentID"] = intParentID;
                                _drInsert993["TabColName"] = "Level";
                                _drInsert993["Title"] = "层级数值";
                                _drInsert993["DataType"] = "int";
                                _drInsert993["FieldLength"] = null;  //...
                                _drInsert993["DefaultValue"] = null;
                                _drInsert993["AllowNull"] = true;
                                _drInsert993["PrimaryKey"] = false;
                                _drInsert993["Description"] = "层级数值，通常父节点为1（同层节点也为1），下一子节点为2，再一下子节点为3，以此类推";
                                _drInsert993["colCustomSort"] = 993;
                                _drInsert993["colTitle"] = "层级数值";
                                _drInsert993["colCustomField"] = null;
                                _drInsert993["colAlign"] = "center";
                                _drInsert993["colEdit"] = "text";
                                _drInsert993["colTemplet"] = null;
                                _drInsert993["ctlAddDisplay"] = true;
                                _drInsert993["ctlModifyDisplay"] = true;
                                _drInsert993["ctlViewDisplay"] = true;
                                _drInsert993["ctlTitle"] = "层级数值";
                                _drInsert993["ctlTypes"] = "Hidden";
                                _drInsert993["ctlPrefix"] = "hid";
                                _drInsert993["ctlStyle"] = null;
                                _drInsert993["ctlDescription"] = "层级数值，通常父节点为1（同层节点也为1），下一子节点为2，再一下子节点为3，以此类推";
                                _drInsert993["ctlSourceTable"] = null;
                                _drInsert993["ctlTextName"] = null;
                                _drInsert993["ctlTextValue"] = null;
                                _drInsert993["ctlFilter"] = null;
                                _drInsert993["ctlVerify"] = "onlyNum";
                                _drInsert993["ctlVerify2"] = null;
                                _drInsert993["ctlValue"] = null;//..
                                _drInsert993["ctlDefaultValue"] = null;
                                _drInsert993["ctlCharLength"] = null; //..
                                _drInsert993["ctlExtendJSCode"] = null; //..
                                _drInsert993["ctlGroup"] = 993;
                                _drInsert993["ctlInputInline"] = "layui-input-inline";
                                _drInsert993["JoinTableColumn"] = true;
                                _dtInsert.Rows.Add(_drInsert993);

                                //层级编号
                                DataRow _drInsert994 = _dtInsert.NewRow();
                                _drInsert994["Sort"] = 994;
                                _drInsert994["ParentID"] = intParentID;
                                _drInsert994["TabColName"] = "LevelCode";
                                _drInsert994["Title"] = "层级编号";
                                _drInsert994["DataType"] = "varchar";
                                _drInsert994["FieldLength"] = "200";  //...
                                _drInsert994["DefaultValue"] = null;
                                _drInsert994["AllowNull"] = true;
                                _drInsert994["PrimaryKey"] = false;
                                _drInsert994["Description"] = "层级编号，方面通过 ‘code%’进行查询属于该级的所有下级[lv1=01,lv2=0101]";
                                _drInsert994["colCustomSort"] = 994;
                                _drInsert994["colTitle"] = "层级编号";
                                _drInsert994["colCustomField"] = null;
                                _drInsert994["colAlign"] = "center";
                                _drInsert994["colEdit"] = "text";
                                _drInsert994["colTemplet"] = null;
                                _drInsert994["ctlAddDisplay"] = true;
                                _drInsert994["ctlModifyDisplay"] = true;
                                _drInsert994["ctlViewDisplay"] = true;
                                _drInsert994["ctlTitle"] = "层级数值";
                                _drInsert994["ctlTypes"] = "Hidden";
                                _drInsert994["ctlPrefix"] = "hid";
                                _drInsert994["ctlStyle"] = null;
                                _drInsert994["ctlDescription"] = "层级编号，方面通过 ‘code%’进行查询属于该级的所有下级[lv1=01,lv2=0101]";
                                _drInsert994["ctlSourceTable"] = null;
                                _drInsert994["ctlTextName"] = null;
                                _drInsert994["ctlTextValue"] = null;
                                _drInsert994["ctlFilter"] = null;
                                _drInsert994["ctlVerify"] = null;
                                _drInsert994["ctlVerify2"] = null;
                                _drInsert994["ctlValue"] = null;//..
                                _drInsert994["ctlDefaultValue"] = null;
                                _drInsert994["ctlCharLength"] = "200"; //..
                                _drInsert994["ctlExtendJSCode"] = null; //..
                                _drInsert994["ctlGroup"] = 994;
                                _drInsert994["ctlInputInline"] = "layui-input-inline";
                                _drInsert994["JoinTableColumn"] = true;
                                _dtInsert.Rows.Add(_drInsert994);
                            }

                            //表单属性
                            if (ApprovalForm.toBoolean())
                            {
                                //FormID
                                DataRow _drInsert4 = _dtInsert.NewRow();
                                _drInsert4["Sort"] = 4;
                                _drInsert4["ParentID"] = intParentID;
                                _drInsert4["TabColName"] = "FormID";
                                _drInsert4["Title"] = "FormID";
                                _drInsert4["DataType"] = "int";
                                _drInsert4["FieldLength"] = null;  //...
                                _drInsert4["DefaultValue"] = null;
                                _drInsert4["AllowNull"] = true;
                                _drInsert4["PrimaryKey"] = false;
                                _drInsert4["Description"] = "int，表单ID。对应的是Forms表的ID";
                                _drInsert4["colCustomSort"] = 4;
                                _drInsert4["colTitle"] = "FormID";
                                _drInsert4["colCustomField"] = null;
                                _drInsert4["colAlign"] = "center";
                                _drInsert4["colEdit"] = null;
                                _drInsert4["colTemplet"] = null;
                                _drInsert4["ctlAddDisplay"] = true;
                                _drInsert4["ctlModifyDisplay"] = true;
                                _drInsert4["ctlViewDisplay"] = true;
                                _drInsert4["ctlTitle"] = "FormID";
                                _drInsert4["ctlTypes"] = "Hidden";
                                _drInsert4["ctlPrefix"] = "hid";
                                _drInsert4["ctlStyle"] = null;
                                _drInsert4["ctlDescription"] = "int，表单ID。对应的是Forms表的ID";
                                _drInsert4["ctlSourceTable"] = null;
                                _drInsert4["ctlTextName"] = null;
                                _drInsert4["ctlTextValue"] = null;
                                _drInsert4["ctlFilter"] = null;
                                _drInsert4["ctlVerify"] = null;
                                _drInsert4["ctlVerify2"] = null;
                                _drInsert4["ctlValue"] = null; //..
                                _drInsert4["ctlDefaultValue"] = null;
                                _drInsert4["ctlCharLength"] = null; //..
                                _drInsert4["ctlExtendJSCode"] = "$('#hidFormID').val($('#txtFormID').val());"; //..
                                _drInsert4["ctlGroup"] = 4;
                                _drInsert4["ctlInputInline"] = "layui-input-inline";
                                _drInsert4["JoinTableColumn"] = true;
                                _dtInsert.Rows.Add(_drInsert4);

                                //FormNumber
                                DataRow _drInsert5 = _dtInsert.NewRow();
                                _drInsert5["Sort"] = 5;
                                _drInsert5["ParentID"] = intParentID;
                                _drInsert5["TabColName"] = "FormNumber";
                                _drInsert5["Title"] = "表单编号";
                                _drInsert5["DataType"] = "varchar";
                                _drInsert5["FieldLength"] = "200";  //...
                                _drInsert5["DefaultValue"] = null;
                                _drInsert5["AllowNull"] = true;
                                _drInsert5["PrimaryKey"] = false;
                                _drInsert5["Description"] = "string，表单编号。某一张表的流水编号。如“IT故障维修表”的编号：IT001、IT002";
                                _drInsert5["colCustomSort"] = 5;
                                _drInsert5["colTitle"] = "表单编号";
                                _drInsert5["colCustomField"] = null;
                                _drInsert5["colAlign"] = "center";
                                _drInsert5["colEdit"] = null;
                                _drInsert5["colTemplet"] = null;
                                _drInsert5["ctlAddDisplay"] = true;
                                _drInsert5["ctlModifyDisplay"] = true;
                                _drInsert5["ctlViewDisplay"] = true;
                                _drInsert5["ctlTitle"] = "表单编号";
                                _drInsert5["ctlTypes"] = "Text";
                                _drInsert5["ctlPrefix"] = "txt";
                                _drInsert5["ctlStyle"] = "readonly=&quot;readonly&quot;";
                                _drInsert5["ctlDescription"] = "表单编号，自动生成";
                                _drInsert5["ctlSourceTable"] = null;
                                _drInsert5["ctlTextName"] = null;
                                _drInsert5["ctlTextValue"] = null;
                                _drInsert5["ctlFilter"] = null;
                                _drInsert5["ctlVerify"] = null;
                                _drInsert5["ctlVerify2"] = null;
                                _drInsert5["ctlValue"] = null; //..
                                _drInsert5["ctlDefaultValue"] = "GetFormNumber";
                                _drInsert5["ctlCharLength"] = "200"; //..
                                _drInsert5["ctlExtendJSCode"] = null; //..
                                _drInsert5["ctlGroup"] = 5;
                                _drInsert5["ctlInputInline"] = "layui-input-inline";
                                _drInsert5["JoinTableColumn"] = true;
                                _dtInsert.Rows.Add(_drInsert5);

                                //FormState
                                DataRow _drInsert6 = _dtInsert.NewRow();
                                _drInsert6["Sort"] = 6;
                                _drInsert6["ParentID"] = intParentID;
                                _drInsert6["TabColName"] = "FormState";
                                _drInsert6["Title"] = "表单状态";
                                _drInsert6["DataType"] = "nvarchar";
                                _drInsert6["FieldLength"] = "200";  //...
                                _drInsert6["DefaultValue"] = null;
                                _drInsert6["AllowNull"] = true;
                                _drInsert6["PrimaryKey"] = false;
                                _drInsert6["Description"] = "表单状态、审批状态：0 = 等待审批[Waiting]、1 = 审批通过[Pass]、-1 = 驳回申请[Return]、-2 = 临时保存[Draft]、11 = 提交申请[Pass]、22 = 等待受理[WaitingForAccept]、23 = 等待对应[WaitingForProcessing]、24 = 等待处理[WaitingForProcessing]、32 = 受理中[Accepting]、33 = 对应中[Processing]、34 = 处理中[Processing]、100 = 完成[Finish]";
                                _drInsert6["colCustomSort"] = 6;
                                _drInsert6["colTitle"] = "表单状态";
                                _drInsert6["colCustomField"] = null;
                                _drInsert6["colAlign"] = "center";
                                _drInsert6["colEdit"] = null;
                                _drInsert6["colTemplet"] = null;
                                _drInsert6["ctlAddDisplay"] = true;
                                _drInsert6["ctlModifyDisplay"] = true;
                                _drInsert6["ctlViewDisplay"] = true;
                                _drInsert6["ctlTitle"] = "表单状态";
                                _drInsert6["ctlTypes"] = "Text";
                                _drInsert6["ctlPrefix"] = "txt";
                                _drInsert6["ctlStyle"] = "readonly=&quot;readonly&quot;";
                                _drInsert6["ctlDescription"] = "表单状态，自动生成";
                                _drInsert6["ctlSourceTable"] = null;
                                _drInsert6["ctlTextName"] = null;
                                _drInsert6["ctlTextValue"] = null;
                                _drInsert6["ctlFilter"] = null;
                                _drInsert6["ctlVerify"] = null;
                                _drInsert6["ctlVerify2"] = null;
                                _drInsert6["ctlValue"] = "GetFormState"; //..
                                _drInsert6["ctlDefaultValue"] = null;
                                _drInsert6["ctlCharLength"] = "200"; //..
                                _drInsert6["ctlExtendJSCode"] = null; //..
                                _drInsert6["ctlGroup"] = 6;
                                _drInsert6["ctlInputInline"] = "layui-input-inline";
                                _drInsert6["JoinTableColumn"] = true;
                                _dtInsert.Rows.Add(_drInsert6);

                                //状态代码
                                DataRow _drInsert7 = _dtInsert.NewRow();
                                _drInsert7["Sort"] = 7;
                                _drInsert7["ParentID"] = intParentID;
                                _drInsert7["TabColName"] = "StateCode";
                                _drInsert7["Title"] = "状态代码";
                                _drInsert7["DataType"] = "int";
                                _drInsert7["FieldLength"] = null;  //...
                                _drInsert7["DefaultValue"] = null;
                                _drInsert7["AllowNull"] = true;
                                _drInsert7["PrimaryKey"] = false;
                                _drInsert7["Description"] = "状态代码，只有StateCode<0的情况下才允许修改";
                                _drInsert7["colCustomSort"] = 7;
                                _drInsert7["colTitle"] = "状态代码";
                                _drInsert7["colCustomField"] = null;
                                _drInsert7["colAlign"] = "center";
                                _drInsert7["colEdit"] = null;
                                _drInsert7["colTemplet"] = null;
                                _drInsert7["ctlAddDisplay"] = true;
                                _drInsert7["ctlModifyDisplay"] = true;
                                _drInsert7["ctlViewDisplay"] = true;
                                _drInsert7["ctlTitle"] = "状态代码";
                                _drInsert7["ctlTypes"] = "Hidden";
                                _drInsert7["ctlPrefix"] = "hid";
                                _drInsert7["ctlStyle"] = null;
                                _drInsert7["ctlDescription"] = "状态代码，只有StateCode<0的情况下才允许修改";
                                _drInsert7["ctlSourceTable"] = null;
                                _drInsert7["ctlTextName"] = null;
                                _drInsert7["ctlTextValue"] = null;
                                _drInsert7["ctlFilter"] = null;
                                _drInsert7["ctlVerify"] = null;
                                _drInsert7["ctlVerify2"] = null;
                                _drInsert7["ctlValue"] = null;//..
                                _drInsert7["ctlDefaultValue"] = "GetFormStateCode";
                                _drInsert7["ctlCharLength"] = null; //..
                                _drInsert7["ctlExtendJSCode"] = null; //..
                                _drInsert7["ctlGroup"] = 7;
                                _drInsert7["ctlInputInline"] = "layui-input-inline";
                                _drInsert7["JoinTableColumn"] = true;
                                _dtInsert.Rows.Add(_drInsert7);

                                //IP
                                DataRow _drInsert992 = _dtInsert.NewRow();
                                _drInsert992["Sort"] = 992;
                                _drInsert992["ParentID"] = intParentID;
                                _drInsert992["TabColName"] = "IP";
                                _drInsert992["Title"] = "IP";
                                _drInsert992["DataType"] = "varchar";
                                _drInsert992["FieldLength"] = "50";  //...
                                _drInsert992["DefaultValue"] = null;
                                _drInsert992["AllowNull"] = true;
                                _drInsert992["PrimaryKey"] = false;
                                _drInsert992["Description"] = "IP";
                                _drInsert992["colCustomSort"] = 992;
                                _drInsert992["colTitle"] = "IP";
                                _drInsert992["colCustomField"] = null;
                                _drInsert992["colAlign"] = "center";
                                _drInsert992["colEdit"] = "text";
                                _drInsert992["colTemplet"] = null;
                                _drInsert992["ctlAddDisplay"] = true;
                                _drInsert992["ctlModifyDisplay"] = true;
                                _drInsert992["ctlViewDisplay"] = true;
                                _drInsert992["ctlTitle"] = "IP";
                                _drInsert992["ctlTypes"] = "Hidden";
                                _drInsert992["ctlPrefix"] = "hid";
                                _drInsert992["ctlStyle"] = null;
                                _drInsert992["ctlDescription"] = "IP";
                                _drInsert992["ctlSourceTable"] = null;
                                _drInsert992["ctlTextName"] = null;
                                _drInsert992["ctlTextValue"] = null;
                                _drInsert992["ctlFilter"] = null;
                                _drInsert992["ctlVerify"] = null;
                                _drInsert992["ctlVerify2"] = null;
                                _drInsert992["ctlValue"] = "GetClientIP";//..
                                _drInsert992["ctlDefaultValue"] = null;
                                _drInsert992["ctlCharLength"] = "50"; //..
                                _drInsert992["ctlExtendJSCode"] = null; //..
                                _drInsert992["ctlGroup"] = 992;
                                _drInsert992["ctlInputInline"] = "layui-input-inline";
                                _drInsert992["JoinTableColumn"] = true;
                                _dtInsert.Rows.Add(_drInsert992);

                            }


                            //基本信息
                            if (BasicInfo.toBoolean())
                            {
                                //申请者
                                DataRow _drInsert20 = _dtInsert.NewRow();
                                _drInsert20["Sort"] = 20;
                                _drInsert20["ParentID"] = intParentID;
                                _drInsert20["TabColName"] = "Applicant";
                                _drInsert20["Title"] = "申请者";
                                _drInsert20["DataType"] = "nvarchar";
                                _drInsert20["FieldLength"] = "200";  //...
                                _drInsert20["DefaultValue"] = null;
                                _drInsert20["AllowNull"] = true;
                                _drInsert20["PrimaryKey"] = false;
                                _drInsert20["Description"] = "申请者";
                                _drInsert20["colCustomSort"] = 20;
                                _drInsert20["colTitle"] = "申请者";
                                _drInsert20["colCustomField"] = null;
                                _drInsert20["colAlign"] = "center";
                                _drInsert20["colEdit"] = "text";
                                _drInsert20["colTemplet"] = null;
                                _drInsert20["ctlAddDisplay"] = true;
                                _drInsert20["ctlModifyDisplay"] = true;
                                _drInsert20["ctlViewDisplay"] = true;
                                _drInsert20["ctlTitle"] = "申请者";
                                _drInsert20["ctlTypes"] = "Text";
                                _drInsert20["ctlPrefix"] = "txt";
                                _drInsert20["ctlStyle"] = null;
                                _drInsert20["ctlDescription"] = "申请者";
                                _drInsert20["ctlSourceTable"] = null;
                                _drInsert20["ctlTextName"] = null;
                                _drInsert20["ctlTextValue"] = null;
                                _drInsert20["ctlFilter"] = null;
                                _drInsert20["ctlVerify"] = null;
                                _drInsert20["ctlVerify2"] = "required";
                                _drInsert20["ctlValue"] = "GetDisplayName"; //..
                                _drInsert20["ctlDefaultValue"] = null;
                                _drInsert20["ctlCharLength"] = "200"; //..
                                _drInsert20["ctlExtendJSCode"] = null; //..
                                _drInsert20["ctlGroup"] = 20;
                                _drInsert20["ctlInputInline"] = "layui-input-inline";
                                _drInsert20["JoinTableColumn"] = true;
                                _dtInsert.Rows.Add(_drInsert20);

                                //Phone
                                DataRow _drInsert21 = _dtInsert.NewRow();
                                _drInsert21["Sort"] = 21;
                                _drInsert21["ParentID"] = intParentID;
                                _drInsert21["TabColName"] = "Phone";
                                _drInsert21["Title"] = "手机";
                                _drInsert21["DataType"] = "varchar";
                                _drInsert21["FieldLength"] = "30";  //...
                                _drInsert21["DefaultValue"] = null;
                                _drInsert21["AllowNull"] = true;
                                _drInsert21["PrimaryKey"] = false;
                                _drInsert21["Description"] = "工作手机号码";
                                _drInsert21["colCustomSort"] = 21;
                                _drInsert21["colTitle"] = "手机";
                                _drInsert21["colCustomField"] = null;
                                _drInsert21["colAlign"] = "center";
                                _drInsert21["colEdit"] = "text";
                                _drInsert21["colTemplet"] = null;
                                _drInsert21["ctlAddDisplay"] = true;
                                _drInsert21["ctlModifyDisplay"] = true;
                                _drInsert21["ctlViewDisplay"] = true;
                                _drInsert21["ctlTitle"] = "手机";
                                _drInsert21["ctlTypes"] = "Text";
                                _drInsert21["ctlPrefix"] = "txt";
                                _drInsert21["ctlStyle"] = null;
                                _drInsert21["ctlDescription"] = "请填写工作手机号码，如没有请填写私人号码";
                                _drInsert21["ctlSourceTable"] = null;
                                _drInsert21["ctlTextName"] = null;
                                _drInsert21["ctlTextValue"] = null;
                                _drInsert21["ctlFilter"] = null;
                                _drInsert21["ctlVerify"] = null;
                                _drInsert21["ctlVerify2"] = null;
                                _drInsert21["ctlValue"] = "GetPhone"; //..
                                _drInsert21["ctlDefaultValue"] = null;
                                _drInsert21["ctlCharLength"] = "30"; //..
                                _drInsert21["ctlExtendJSCode"] = null; //..
                                _drInsert21["ctlGroup"] = 20;
                                _drInsert21["ctlInputInline"] = "layui-input-inline";
                                _drInsert21["JoinTableColumn"] = true;
                                _dtInsert.Rows.Add(_drInsert21);

                                //Tel
                                DataRow _drInsert22 = _dtInsert.NewRow();
                                _drInsert22["Sort"] = 22;
                                _drInsert22["ParentID"] = intParentID;
                                _drInsert22["TabColName"] = "Tel";
                                _drInsert22["Title"] = "分机";
                                _drInsert22["DataType"] = "varchar";
                                _drInsert22["FieldLength"] = "30";  //...
                                _drInsert22["DefaultValue"] = null;
                                _drInsert22["AllowNull"] = true;
                                _drInsert22["PrimaryKey"] = false;
                                _drInsert22["Description"] = "分机号码";
                                _drInsert22["colCustomSort"] = 22;
                                _drInsert22["colTitle"] = "分机";
                                _drInsert22["colCustomField"] = null;
                                _drInsert22["colAlign"] = "center";
                                _drInsert22["colEdit"] = "text";
                                _drInsert22["colTemplet"] = null;
                                _drInsert22["ctlAddDisplay"] = true;
                                _drInsert22["ctlModifyDisplay"] = true;
                                _drInsert22["ctlViewDisplay"] = true;
                                _drInsert22["ctlTitle"] = "分机";
                                _drInsert22["ctlTypes"] = "Text";
                                _drInsert22["ctlPrefix"] = "txt";
                                _drInsert22["ctlStyle"] = null;
                                _drInsert22["ctlDescription"] = "分机号码，没有主留空";
                                _drInsert22["ctlSourceTable"] = null;
                                _drInsert22["ctlTextName"] = null;
                                _drInsert22["ctlTextValue"] = null;
                                _drInsert22["ctlFilter"] = null;
                                _drInsert22["ctlVerify"] = null;
                                _drInsert22["ctlVerify2"] = null;
                                _drInsert22["ctlValue"] = "GetTel"; //..
                                _drInsert22["ctlDefaultValue"] = null;
                                _drInsert22["ctlCharLength"] = "30"; //..
                                _drInsert22["ctlExtendJSCode"] = null; //..
                                _drInsert22["ctlGroup"] = 20;
                                _drInsert22["ctlInputInline"] = "layui-input-inline";
                                _drInsert22["JoinTableColumn"] = true;
                                _dtInsert.Rows.Add(_drInsert22);

                                //UID
                                DataRow _drInsert25 = _dtInsert.NewRow();
                                _drInsert25["Sort"] = 25;
                                _drInsert25["ParentID"] = intParentID;
                                _drInsert25["TabColName"] = "UID";
                                _drInsert25["Title"] = "UID";
                                _drInsert25["DataType"] = "int";
                                _drInsert25["FieldLength"] = null;  //...
                                _drInsert25["DefaultValue"] = null;
                                _drInsert25["AllowNull"] = true;
                                _drInsert25["PrimaryKey"] = false;
                                _drInsert25["Description"] = "用户ID";
                                _drInsert25["colCustomSort"] = 25;
                                _drInsert25["colTitle"] = "UID";
                                _drInsert25["colCustomField"] = null;
                                _drInsert25["colAlign"] = "center";
                                _drInsert25["colEdit"] = null;
                                _drInsert25["colTemplet"] = null;
                                _drInsert25["ctlAddDisplay"] = true;
                                _drInsert25["ctlModifyDisplay"] = true;
                                _drInsert25["ctlViewDisplay"] = true;
                                _drInsert25["ctlTitle"] = "UID";
                                _drInsert25["ctlTypes"] = "Hidden";
                                _drInsert25["ctlPrefix"] = "hid";
                                _drInsert25["ctlStyle"] = null;
                                _drInsert25["ctlDescription"] = "用户ID";
                                _drInsert25["ctlSourceTable"] = null;
                                _drInsert25["ctlTextName"] = null;
                                _drInsert25["ctlTextValue"] = null;
                                _drInsert25["ctlFilter"] = null;
                                _drInsert25["ctlVerify"] = null;
                                _drInsert25["ctlVerify2"] = "required";
                                _drInsert25["ctlValue"] = "GetUID";//..
                                _drInsert25["ctlDefaultValue"] = null;
                                _drInsert25["ctlCharLength"] = null; //..
                                _drInsert25["ctlExtendJSCode"] = null; //..
                                _drInsert25["ctlGroup"] = 25;
                                _drInsert25["ctlInputInline"] = "layui-input-inline";
                                _drInsert25["JoinTableColumn"] = true;
                                _dtInsert.Rows.Add(_drInsert25);

                                //DisplayName
                                DataRow _drInsert26 = _dtInsert.NewRow();
                                _drInsert26["Sort"] = 26;
                                _drInsert26["ParentID"] = intParentID;
                                _drInsert26["TabColName"] = "DisplayName";
                                _drInsert26["Title"] = "用户名";
                                _drInsert26["DataType"] = "nvarchar";
                                _drInsert26["FieldLength"] = "200";  //...
                                _drInsert26["DefaultValue"] = null;
                                _drInsert26["AllowNull"] = true;
                                _drInsert26["PrimaryKey"] = false;
                                _drInsert26["Description"] = "显示名";
                                _drInsert26["colCustomSort"] = 26;
                                _drInsert26["colTitle"] = "用户名";
                                _drInsert26["colCustomField"] = null;
                                _drInsert26["colAlign"] = "center";
                                _drInsert26["colEdit"] = null;
                                _drInsert26["colTemplet"] = null;
                                _drInsert26["ctlAddDisplay"] = true;
                                _drInsert26["ctlModifyDisplay"] = true;
                                _drInsert26["ctlViewDisplay"] = true;
                                _drInsert26["ctlTitle"] = "用户名";
                                _drInsert26["ctlTypes"] = "Hidden";
                                _drInsert26["ctlPrefix"] = "hid";
                                _drInsert26["ctlStyle"] = null;
                                _drInsert26["ctlDescription"] = "显示名";
                                _drInsert26["ctlSourceTable"] = null;
                                _drInsert26["ctlTextName"] = null;
                                _drInsert26["ctlTextValue"] = null;
                                _drInsert26["ctlFilter"] = null;
                                _drInsert26["ctlVerify"] = null;
                                _drInsert26["ctlVerify2"] = "required";
                                _drInsert26["ctlValue"] = "GetDisplayName"; //..
                                _drInsert26["ctlDefaultValue"] = null;
                                _drInsert26["ctlCharLength"] = "200"; //..
                                _drInsert26["ctlExtendJSCode"] = null; //..
                                _drInsert26["ctlGroup"] = 26;
                                _drInsert26["ctlInputInline"] = "layui-input-inline";
                                _drInsert26["JoinTableColumn"] = true;
                                _dtInsert.Rows.Add(_drInsert26);

                            }

                            if (Dept.toBoolean())
                            {
                                //部门
                                DataRow _drInsert24 = _dtInsert.NewRow();
                                _drInsert24["Sort"] = 24;
                                _drInsert24["ParentID"] = intParentID;
                                _drInsert24["TabColName"] = "DeptID";
                                _drInsert24["Title"] = "部门";
                                _drInsert24["DataType"] = "int";
                                _drInsert24["FieldLength"] = null;  //...
                                _drInsert24["DefaultValue"] = null;
                                _drInsert24["AllowNull"] = true;
                                _drInsert24["PrimaryKey"] = false;
                                _drInsert24["Description"] = "部门";
                                _drInsert24["colCustomSort"] = 24;
                                _drInsert24["colTitle"] = "部门";
                                _drInsert24["colCustomField"] = "DeptName";
                                _drInsert24["colAlign"] = "center";
                                _drInsert24["colEdit"] = null;
                                _drInsert24["colTemplet"] = null;
                                _drInsert24["ctlAddDisplay"] = true;
                                _drInsert24["ctlModifyDisplay"] = true;
                                _drInsert24["ctlViewDisplay"] = true;
                                _drInsert24["ctlTitle"] = "部门";
                                _drInsert24["ctlTypes"] = "Select";
                                _drInsert24["ctlPrefix"] = "sek";
                                _drInsert24["ctlStyle"] = null;
                                _drInsert24["ctlDescription"] = "部门";
                                _drInsert24["ctlSourceTable"] = "Department";
                                _drInsert24["ctlTextName"] = "DeptName";
                                _drInsert24["ctlTextValue"] = "DeptID";
                                _drInsert24["ctlFilter"] = "selDeptID";
                                _drInsert24["ctlVerify"] = null;
                                _drInsert24["ctlVerify2"] = "required";
                                _drInsert24["ctlValue"] = null;//..
                                _drInsert24["ctlDefaultValue"] = "GetDeptID";
                                _drInsert24["ctlCharLength"] = null; //..
                                _drInsert24["ctlExtendJSCode"] = null; //..
                                _drInsert24["ctlGroup"] = 24;
                                _drInsert24["ctlInputInline"] = "layui-input-inline";
                                _drInsert24["JoinTableColumn"] = true;
                                _dtInsert.Rows.Add(_drInsert24);
                            }

                            if (PCInfo.toBoolean())
                            {
                                //电脑登录ID
                                DataRow _drInsert27 = _dtInsert.NewRow();
                                _drInsert27["Sort"] = 27;
                                _drInsert27["ParentID"] = intParentID;
                                _drInsert27["TabColName"] = "PCLoginID";
                                _drInsert27["Title"] = "电脑登录ID";
                                _drInsert27["DataType"] = "varchar";
                                _drInsert27["FieldLength"] = "200";  //...
                                _drInsert27["DefaultValue"] = null;
                                _drInsert27["AllowNull"] = true;
                                _drInsert27["PrimaryKey"] = false;
                                _drInsert27["Description"] = "电脑登录ID";
                                _drInsert27["colCustomSort"] = 27;
                                _drInsert27["colTitle"] = "电脑登录ID";
                                _drInsert27["colCustomField"] = null;
                                _drInsert27["colAlign"] = "center";
                                _drInsert27["colEdit"] = "text";
                                _drInsert27["colTemplet"] = null;
                                _drInsert27["ctlAddDisplay"] = true;
                                _drInsert27["ctlModifyDisplay"] = true;
                                _drInsert27["ctlViewDisplay"] = true;
                                _drInsert27["ctlTitle"] = "电脑登录ID";
                                _drInsert27["ctlTypes"] = "Text";
                                _drInsert27["ctlPrefix"] = "txt";
                                _drInsert27["ctlStyle"] = null;
                                _drInsert27["ctlDescription"] = "电脑登录ID";
                                _drInsert27["ctlSourceTable"] = null;
                                _drInsert27["ctlTextName"] = null;
                                _drInsert27["ctlTextValue"] = null;
                                _drInsert27["ctlFilter"] = null;
                                _drInsert27["ctlVerify"] = null;
                                _drInsert27["ctlVerify2"] = null;
                                _drInsert27["ctlValue"] = "GetUserName"; //..
                                _drInsert27["ctlDefaultValue"] = null;
                                _drInsert27["ctlCharLength"] = "200"; //..
                                _drInsert27["ctlExtendJSCode"] = null; //..
                                _drInsert27["ctlGroup"] = 27;
                                _drInsert27["ctlInputInline"] = "layui-input-inline";
                                _drInsert27["JoinTableColumn"] = true;
                                _dtInsert.Rows.Add(_drInsert27);

                                //电脑名
                                DataRow _drInsert28 = _dtInsert.NewRow();
                                _drInsert28["Sort"] = 28;
                                _drInsert28["ParentID"] = intParentID;
                                _drInsert28["TabColName"] = "PCName";
                                _drInsert28["Title"] = "电脑名";
                                _drInsert28["DataType"] = "varchar";
                                _drInsert28["FieldLength"] = "100";  //...
                                _drInsert28["DefaultValue"] = null;
                                _drInsert28["AllowNull"] = true;
                                _drInsert28["PrimaryKey"] = false;
                                _drInsert28["Description"] = "电脑名";
                                _drInsert28["colCustomSort"] = 28;
                                _drInsert28["colTitle"] = "电脑名";
                                _drInsert28["colCustomField"] = null;
                                _drInsert28["colAlign"] = "center";
                                _drInsert28["colEdit"] = "text";
                                _drInsert28["colTemplet"] = null;
                                _drInsert28["ctlAddDisplay"] = true;
                                _drInsert28["ctlModifyDisplay"] = true;
                                _drInsert28["ctlViewDisplay"] = true;
                                _drInsert28["ctlTitle"] = "电脑名";
                                _drInsert28["ctlTypes"] = "Text";
                                _drInsert28["ctlPrefix"] = "txt";
                                _drInsert28["ctlStyle"] = null;
                                _drInsert28["ctlDescription"] = "电脑名";
                                _drInsert28["ctlSourceTable"] = null;
                                _drInsert28["ctlTextName"] = null;
                                _drInsert28["ctlTextValue"] = null;
                                _drInsert28["ctlFilter"] = null;
                                _drInsert28["ctlVerify"] = null;
                                _drInsert28["ctlVerify2"] = null;
                                _drInsert28["ctlValue"] = null; //..
                                _drInsert28["ctlDefaultValue"] = null;
                                _drInsert28["ctlCharLength"] = "100"; //..
                                _drInsert28["ctlExtendJSCode"] = null; //..
                                _drInsert28["ctlGroup"] = 28;
                                _drInsert28["ctlInputInline"] = "layui-input-inline";
                                _drInsert28["JoinTableColumn"] = true;
                                _dtInsert.Rows.Add(_drInsert28);

                                //Mail
                                DataRow _drInsert29 = _dtInsert.NewRow();
                                _drInsert29["Sort"] = 29;
                                _drInsert29["ParentID"] = intParentID;
                                _drInsert29["TabColName"] = "Mail";
                                _drInsert29["Title"] = "邮箱";
                                _drInsert29["DataType"] = "varchar";
                                _drInsert29["FieldLength"] = "200";  //...
                                _drInsert29["DefaultValue"] = null;
                                _drInsert29["AllowNull"] = true;
                                _drInsert29["PrimaryKey"] = false;
                                _drInsert29["Description"] = "邮箱";
                                _drInsert29["colCustomSort"] = 29;
                                _drInsert29["colTitle"] = "邮箱";
                                _drInsert29["colCustomField"] = null;
                                _drInsert29["colAlign"] = "center";
                                _drInsert29["colEdit"] = "text";
                                _drInsert29["colTemplet"] = null;
                                _drInsert29["ctlAddDisplay"] = true;
                                _drInsert29["ctlModifyDisplay"] = true;
                                _drInsert29["ctlViewDisplay"] = true;
                                _drInsert29["ctlTitle"] = "邮箱";
                                _drInsert29["ctlTypes"] = "Text";
                                _drInsert29["ctlPrefix"] = "txt";
                                _drInsert29["ctlStyle"] = null;
                                _drInsert29["ctlDescription"] = "邮箱";
                                _drInsert29["ctlSourceTable"] = null;
                                _drInsert29["ctlTextName"] = null;
                                _drInsert29["ctlTextValue"] = null;
                                _drInsert29["ctlFilter"] = null;
                                _drInsert29["ctlVerify"] = null;
                                _drInsert29["ctlVerify2"] = null;
                                _drInsert29["ctlValue"] = "GetMail"; //..
                                _drInsert29["ctlDefaultValue"] = null;
                                _drInsert29["ctlCharLength"] = "200"; //..
                                _drInsert29["ctlExtendJSCode"] = null; //..
                                _drInsert29["ctlGroup"] = 29;
                                _drInsert29["ctlInputInline"] = "layui-input-inline";
                                _drInsert29["JoinTableColumn"] = true;
                                _dtInsert.Rows.Add(_drInsert29);
                            }

                            //申请类型
                            if (ApplicationType.toBoolean())
                            {
                                //申请类型
                                DataRow _drInsert30 = _dtInsert.NewRow();
                                _drInsert30["Sort"] = 30;
                                _drInsert30["ParentID"] = intParentID;
                                _drInsert30["TabColName"] = "ApplicationTypeID";
                                _drInsert30["Title"] = "申请类型";
                                _drInsert30["DataType"] = "int";
                                _drInsert30["FieldLength"] = null;  //...
                                _drInsert30["DefaultValue"] = null;
                                _drInsert30["AllowNull"] = true;
                                _drInsert30["PrimaryKey"] = false;
                                _drInsert30["Description"] = "申请类型";
                                _drInsert30["colCustomSort"] = 30;
                                _drInsert30["colTitle"] = "申请类型";
                                _drInsert30["colCustomField"] = null;
                                _drInsert30["colAlign"] = "center";
                                _drInsert30["colEdit"] = null;
                                _drInsert30["colTemplet"] = null;
                                _drInsert30["ctlAddDisplay"] = true;
                                _drInsert30["ctlModifyDisplay"] = true;
                                _drInsert30["ctlViewDisplay"] = true;
                                _drInsert30["ctlTitle"] = "申请类型";
                                _drInsert30["ctlTypes"] = "RadioCascaderSelect";
                                _drInsert30["ctlPrefix"] = "sel";
                                _drInsert30["ctlStyle"] = null;
                                _drInsert30["ctlDescription"] = "申请类型";
                                _drInsert30["ctlSourceTable"] = null;
                                _drInsert30["ctlTextName"] = null;
                                _drInsert30["ctlTextValue"] = null;
                                _drInsert30["ctlFilter"] = null;
                                _drInsert30["ctlVerify"] = null;
                                _drInsert30["ctlVerify2"] = "required";
                                _drInsert30["ctlValue"] = null; //..
                                _drInsert30["ctlDefaultValue"] = null;
                                _drInsert30["ctlCharLength"] = null; //..
                                _drInsert30["ctlExtendJSCode"] = null; //..
                                _drInsert30["ctlGroup"] = 30;
                                _drInsert30["ctlInputInline"] = "layui-input-block";
                                _drInsert30["JoinTableColumn"] = true;
                                _dtInsert.Rows.Add(_drInsert30);

                                //依赖日期(放在内容下 序号为48)
                                DataRow _drInsert48 = _dtInsert.NewRow();
                                _drInsert48["Sort"] = 48;
                                _drInsert48["ParentID"] = intParentID;
                                _drInsert48["TabColName"] = "ApplicationDate";
                                _drInsert48["Title"] = "依赖日期";
                                _drInsert48["DataType"] = "datetime";
                                _drInsert48["FieldLength"] = null;  //...
                                _drInsert48["DefaultValue"] = null;
                                _drInsert48["AllowNull"] = true;
                                _drInsert48["PrimaryKey"] = false;
                                _drInsert48["Description"] = "依赖日期、申请日期";
                                _drInsert48["colCustomSort"] = 48;
                                _drInsert48["colTitle"] = "依赖日期";
                                _drInsert48["colCustomField"] = null;
                                _drInsert48["colAlign"] = "center";
                                _drInsert48["colEdit"] = "text";
                                _drInsert48["colTemplet"] = null;
                                _drInsert48["ctlAddDisplay"] = true;
                                _drInsert48["ctlModifyDisplay"] = true;
                                _drInsert48["ctlViewDisplay"] = true;
                                _drInsert48["ctlTitle"] = "依赖日期";
                                _drInsert48["ctlTypes"] = "Text";
                                _drInsert48["ctlPrefix"] = "txt";
                                _drInsert48["ctlStyle"] = "readonly=&quot;readonly&quot;";
                                _drInsert48["ctlDescription"] = "依赖日期、申请日期";
                                _drInsert48["ctlSourceTable"] = null;
                                _drInsert48["ctlTextName"] = null;
                                _drInsert48["ctlTextValue"] = null;
                                _drInsert48["ctlFilter"] = null;
                                _drInsert48["ctlVerify"] = null;
                                _drInsert48["ctlVerify2"] = null;
                                _drInsert48["ctlValue"] = "GetDate";//..
                                _drInsert48["ctlDefaultValue"] = null;
                                _drInsert48["ctlCharLength"] = "100"; //..
                                _drInsert48["ctlExtendJSCode"] = "laydate.render({ elem: '#txtApplicationDate', trigger: 'click' });"; //..
                                _drInsert48["ctlGroup"] = 49;
                                _drInsert48["ctlInputInline"] = "layui-input-inline";
                                _drInsert48["JoinTableColumn"] = true;
                                _dtInsert.Rows.Add(_drInsert48);

                            }

                            //标题内容
                            if (TitleContent.toBoolean())
                            {
                                //Title
                                DataRow _drInsert40 = _dtInsert.NewRow();
                                _drInsert40["Sort"] = 40;
                                _drInsert40["ParentID"] = intParentID;
                                _drInsert40["TabColName"] = "Title";
                                _drInsert40["Title"] = "标题";
                                _drInsert40["DataType"] = "nvarchar";
                                _drInsert40["FieldLength"] = "500";  //...
                                _drInsert40["DefaultValue"] = null;
                                _drInsert40["AllowNull"] = true;
                                _drInsert40["PrimaryKey"] = false;
                                _drInsert40["Description"] = "标题";
                                _drInsert40["colCustomSort"] = 40;
                                _drInsert40["colTitle"] = "标题";
                                _drInsert40["colCustomField"] = null;
                                _drInsert40["colAlign"] = "center";
                                _drInsert40["colEdit"] = "text";
                                _drInsert40["colTemplet"] = null;
                                _drInsert40["ctlAddDisplay"] = true;
                                _drInsert40["ctlModifyDisplay"] = true;
                                _drInsert40["ctlViewDisplay"] = true;
                                _drInsert40["ctlTitle"] = "标题";
                                _drInsert40["ctlTypes"] = "Text";
                                _drInsert40["ctlPrefix"] = "txt";
                                _drInsert40["ctlStyle"] = null;
                                _drInsert40["ctlDescription"] = "标题";
                                _drInsert40["ctlSourceTable"] = null;
                                _drInsert40["ctlTextName"] = null;
                                _drInsert40["ctlTextValue"] = null;
                                _drInsert40["ctlFilter"] = null;
                                _drInsert40["ctlVerify"] = null;
                                _drInsert40["ctlVerify2"] = "required";
                                _drInsert40["ctlValue"] = null; //..
                                _drInsert40["ctlDefaultValue"] = null;
                                _drInsert40["ctlCharLength"] = "500"; //..
                                _drInsert40["ctlExtendJSCode"] = null; //..
                                _drInsert40["ctlGroup"] = 40;
                                _drInsert40["ctlInputInline"] = "layui-input-block";
                                _drInsert40["JoinTableColumn"] = true;
                                _dtInsert.Rows.Add(_drInsert40);

                                //Content
                                DataRow _drInsert41 = _dtInsert.NewRow();
                                _drInsert41["Sort"] = 41;
                                _drInsert41["ParentID"] = intParentID;
                                _drInsert41["TabColName"] = "Content";
                                _drInsert41["Title"] = "内容";
                                _drInsert41["DataType"] = "ntext";
                                _drInsert41["FieldLength"] = null;  //...
                                _drInsert41["DefaultValue"] = null;
                                _drInsert41["AllowNull"] = true;
                                _drInsert41["PrimaryKey"] = false;
                                _drInsert41["Description"] = "内容";
                                _drInsert41["colCustomSort"] = 30;
                                _drInsert41["colTitle"] = "内容";
                                _drInsert41["colCustomField"] = null;
                                _drInsert41["colAlign"] = "left";
                                _drInsert41["colEdit"] = "text";
                                _drInsert41["colTemplet"] = null;
                                _drInsert41["ctlAddDisplay"] = true;
                                _drInsert41["ctlModifyDisplay"] = true;
                                _drInsert41["ctlViewDisplay"] = true;
                                _drInsert41["ctlTitle"] = "内容";
                                _drInsert41["ctlTypes"] = "Textarea";
                                _drInsert41["ctlPrefix"] = "txt";
                                _drInsert41["ctlStyle"] = null;
                                _drInsert41["ctlDescription"] = "内容";
                                _drInsert41["ctlSourceTable"] = null;
                                _drInsert41["ctlTextName"] = null;
                                _drInsert41["ctlTextValue"] = null;
                                _drInsert41["ctlFilter"] = null;
                                _drInsert41["ctlVerify"] = null;
                                _drInsert41["ctlVerify2"] = "required";
                                _drInsert41["ctlValue"] = null; //..
                                _drInsert41["ctlDefaultValue"] = null;
                                _drInsert41["ctlCharLength"] = null; //..
                                _drInsert41["ctlExtendJSCode"] = null; //..
                                _drInsert41["ctlGroup"] = 41;
                                _drInsert41["ctlInputInline"] = "layui-input-block";
                                _drInsert41["JoinTableColumn"] = true;
                                _dtInsert.Rows.Add(_drInsert41);
                            }

                            //希望对应日 （和依赖日期是一组，所组号一样为49）
                            if (HopeDate.toBoolean())
                            {
                                //希望对应日
                                DataRow _drInsert49 = _dtInsert.NewRow();
                                _drInsert49["Sort"] = 49;
                                _drInsert49["ParentID"] = intParentID;
                                _drInsert49["TabColName"] = "HopeDate";
                                _drInsert49["Title"] = "希望对应日";
                                _drInsert49["DataType"] = "datetime";
                                _drInsert49["FieldLength"] = null;  //...
                                _drInsert49["DefaultValue"] = null;
                                _drInsert49["AllowNull"] = true;
                                _drInsert49["PrimaryKey"] = false;
                                _drInsert49["Description"] = "希望对应日";
                                _drInsert49["colCustomSort"] = 49;
                                _drInsert49["colTitle"] = "希望对应日";
                                _drInsert49["colCustomField"] = null;
                                _drInsert49["colAlign"] = "center";
                                _drInsert49["colEdit"] = "text";
                                _drInsert49["colTemplet"] = null;
                                _drInsert49["ctlAddDisplay"] = true;
                                _drInsert49["ctlModifyDisplay"] = true;
                                _drInsert49["ctlViewDisplay"] = true;
                                _drInsert49["ctlTitle"] = "希望对应日";
                                _drInsert49["ctlTypes"] = "Text";
                                _drInsert49["ctlPrefix"] = "txt";
                                _drInsert49["ctlStyle"] = "readonly=&quot;readonly&quot;";
                                _drInsert49["ctlDescription"] = "希望对应日";
                                _drInsert49["ctlSourceTable"] = null;
                                _drInsert49["ctlTextName"] = null;
                                _drInsert49["ctlTextValue"] = null;
                                _drInsert49["ctlFilter"] = null;
                                _drInsert49["ctlVerify"] = null;
                                _drInsert49["ctlVerify2"] = null;
                                _drInsert49["ctlValue"] = "GetDate";//..
                                _drInsert49["ctlDefaultValue"] = null;
                                _drInsert49["ctlCharLength"] = "100"; //..
                                _drInsert49["ctlExtendJSCode"] = "laydate.render({ elem: '#txtHopeDate', trigger: 'click' });"; //..
                                _drInsert49["ctlGroup"] = 49;
                                _drInsert49["ctlInputInline"] = "layui-input-inline";
                                _drInsert49["JoinTableColumn"] = true;
                                _dtInsert.Rows.Add(_drInsert49);

                            }

                            //临时保存，在新增记录时需要临时保存时，把该值设置为True
                            DataRow _drInsert995 = _dtInsert.NewRow();
                            _drInsert995["Sort"] = 995;
                            _drInsert995["ParentID"] = intParentID;
                            _drInsert995["TabColName"] = "SaveDraft";
                            _drInsert995["Title"] = "临时保存";
                            _drInsert995["DataType"] = "bit";
                            _drInsert995["FieldLength"] = null;  //...
                            _drInsert995["DefaultValue"] = false;
                            _drInsert995["AllowNull"] = true;
                            _drInsert995["PrimaryKey"] = false;
                            _drInsert995["Description"] = "Boolean，临时保存/保存草稿，默认值False";
                            _drInsert995["colCustomSort"] = 995;
                            _drInsert995["colTitle"] = "临时保存";
                            _drInsert995["colCustomField"] = null;
                            _drInsert995["colAlign"] = "center";
                            _drInsert995["colEdit"] = null;
                            _drInsert995["colTemplet"] = "#swSaveDraft";
                            _drInsert995["ctlAddDisplay"] = false;
                            _drInsert995["ctlModifyDisplay"] = false;
                            _drInsert995["ctlViewDisplay"] = false;
                            _drInsert995["ctlTitle"] = "临时保存";
                            _drInsert995["ctlTypes"] = null;
                            _drInsert995["ctlPrefix"] = null;
                            _drInsert995["ctlStyle"] = null;
                            _drInsert995["ctlDescription"] = "Boolean，临时保存/保存草稿，默认值False";
                            _drInsert995["ctlSourceTable"] = null;
                            _drInsert995["ctlTextName"] = null;
                            _drInsert995["ctlTextValue"] = null;
                            _drInsert995["ctlFilter"] = null;
                            _drInsert995["ctlVerify"] = null;
                            _drInsert995["ctlVerify2"] = null;
                            _drInsert995["ctlValue"] = null;//..
                            _drInsert995["ctlDefaultValue"] = null;
                            _drInsert995["ctlCharLength"] = null; //..
                            _drInsert995["ctlExtendJSCode"] = null; //..
                            _drInsert995["ctlGroup"] = 995;
                            _drInsert995["ctlInputInline"] = "layui-input-inline";
                            _drInsert995["JoinTableColumn"] = false;
                            _dtInsert.Rows.Add(_drInsert995);

                            //创建日期
                            DataRow _drInsert996 = _dtInsert.NewRow();
                            _drInsert996["Sort"] = 996;
                            _drInsert996["ParentID"] = intParentID;
                            _drInsert996["TabColName"] = "DateCreated";
                            _drInsert996["Title"] = "创建日期";
                            _drInsert996["DataType"] = "datetime";
                            _drInsert996["FieldLength"] = null;  //...
                            _drInsert996["DefaultValue"] = "getdate()";
                            _drInsert996["AllowNull"] = true;
                            _drInsert996["PrimaryKey"] = false;
                            _drInsert996["Description"] = "DateTime，创建日期，D默认值：getdate()当前系统时间";
                            _drInsert996["colCustomSort"] = 996;
                            _drInsert996["colTitle"] = "创建日期";
                            _drInsert996["colCustomField"] = null;
                            _drInsert996["colAlign"] = "center";
                            _drInsert996["colEdit"] = null;
                            _drInsert996["colTemplet"] = null;
                            _drInsert996["ctlAddDisplay"] = false;
                            _drInsert996["ctlModifyDisplay"] = false;
                            _drInsert996["ctlViewDisplay"] = false;
                            _drInsert996["ctlTitle"] = "创建日期";
                            _drInsert996["ctlTypes"] = null;
                            _drInsert996["ctlPrefix"] = null;
                            _drInsert996["ctlStyle"] = null;
                            _drInsert996["ctlDescription"] = "DateTime，创建日期，默认值：当前系统时间";
                            _drInsert996["ctlSourceTable"] = null;
                            _drInsert996["ctlTextName"] = null;
                            _drInsert996["ctlTextValue"] = null;
                            _drInsert996["ctlFilter"] = null;
                            _drInsert996["ctlVerify"] = null;
                            _drInsert996["ctlVerify2"] = null;
                            _drInsert996["ctlValue"] = null;//..
                            _drInsert996["ctlDefaultValue"] = "GetDateTimeNow";
                            _drInsert996["ctlCharLength"] = null; //..
                            _drInsert996["ctlExtendJSCode"] = null; //..
                            _drInsert996["ctlGroup"] = 996;
                            _drInsert996["ctlInputInline"] = "layui-input-inline";
                            _drInsert996["JoinTableColumn"] = true;
                            _dtInsert.Rows.Add(_drInsert996);

                            //修改日期
                            DataRow _drInsert997 = _dtInsert.NewRow();
                            _drInsert997["Sort"] = 997;
                            _drInsert997["ParentID"] = intParentID;
                            _drInsert997["TabColName"] = "DateModified";
                            _drInsert997["Title"] = "修改日期";
                            _drInsert997["DataType"] = "datetime";
                            _drInsert997["FieldLength"] = null;  //...
                            _drInsert997["DefaultValue"] = "GetDateTimeNow";
                            _drInsert997["AllowNull"] = true;
                            _drInsert997["PrimaryKey"] = false;
                            _drInsert997["Description"] = "DateTime，修改日期，D默认值：getdate()当前系统时间，M默认值：GetDateTimeNow";
                            _drInsert997["colCustomSort"] = 997;
                            _drInsert997["colTitle"] = "修改日期";
                            _drInsert997["colCustomField"] = null;
                            _drInsert997["colAlign"] = "center";
                            _drInsert997["colEdit"] = null;
                            _drInsert997["colTemplet"] = null;
                            _drInsert997["ctlAddDisplay"] = false;
                            _drInsert997["ctlModifyDisplay"] = true;
                            _drInsert997["ctlViewDisplay"] = false;
                            _drInsert997["ctlTitle"] = "修改日期";
                            _drInsert997["ctlTypes"] = "Hidden";
                            _drInsert997["ctlPrefix"] = null;
                            _drInsert997["ctlStyle"] = null;
                            _drInsert997["ctlDescription"] = "DateTime，修改日期，D默认值：getdate()当前系统时间，M默认值：GetDateTimeNow";
                            _drInsert997["ctlSourceTable"] = null;
                            _drInsert997["ctlTextName"] = null;
                            _drInsert997["ctlTextValue"] = null;
                            _drInsert997["ctlFilter"] = null;
                            _drInsert997["ctlVerify"] = null;
                            _drInsert997["ctlVerify2"] = null;
                            _drInsert997["ctlValue"] = null;//..
                            _drInsert997["ctlDefaultValue"] = "GetDateTimeNow";
                            _drInsert997["ctlCharLength"] = null; //..
                            _drInsert997["ctlExtendJSCode"] = null; //..
                            _drInsert997["ctlGroup"] = 997;
                            _drInsert997["ctlInputInline"] = "layui-input-inline";
                            _drInsert997["JoinTableColumn"] = true;
                            _dtInsert.Rows.Add(_drInsert997);

                            //无效
                            DataRow _drInsert998 = _dtInsert.NewRow();
                            _drInsert998["Sort"] = 998;
                            _drInsert998["ParentID"] = intParentID;
                            _drInsert998["TabColName"] = "Invalid";
                            _drInsert998["Title"] = "无效";
                            _drInsert998["DataType"] = "bit";
                            _drInsert998["FieldLength"] = null;  //...
                            _drInsert998["DefaultValue"] = false;
                            _drInsert998["AllowNull"] = true;
                            _drInsert998["PrimaryKey"] = false;
                            _drInsert998["Description"] = "Boolean，无效，默认值False，无效时管理员还可见，其它角色不可见";
                            _drInsert998["colCustomSort"] = 998;
                            _drInsert998["colTitle"] = "无效";
                            _drInsert998["colCustomField"] = null;
                            _drInsert998["colAlign"] = "center";
                            _drInsert998["colEdit"] = null;
                            _drInsert998["colTemplet"] = "#swInvalid";
                            _drInsert998["ctlAddDisplay"] = false;
                            _drInsert998["ctlModifyDisplay"] = false;
                            _drInsert998["ctlViewDisplay"] = false;
                            _drInsert998["ctlTitle"] = "无效";
                            _drInsert998["ctlTypes"] = null;
                            _drInsert998["ctlPrefix"] = null;
                            _drInsert998["ctlStyle"] = null;
                            _drInsert998["ctlDescription"] = "无效";
                            _drInsert998["ctlSourceTable"] = null;
                            _drInsert998["ctlTextName"] = null;
                            _drInsert998["ctlTextValue"] = null;
                            _drInsert998["ctlFilter"] = null;
                            _drInsert998["ctlVerify"] = null;
                            _drInsert998["ctlVerify2"] = null;
                            _drInsert998["ctlValue"] = null;//..
                            _drInsert998["ctlDefaultValue"] = null;
                            _drInsert998["ctlCharLength"] = null; //..
                            _drInsert998["ctlExtendJSCode"] = null; //..
                            _drInsert998["ctlGroup"] = 998;
                            _drInsert998["ctlInputInline"] = "layui-input-inline";
                            _drInsert998["JoinTableColumn"] = true;
                            _dtInsert.Rows.Add(_drInsert998);

                            //删除
                            DataRow _drInsert999 = _dtInsert.NewRow();
                            _drInsert999["Sort"] = 999;
                            _drInsert999["ParentID"] = intParentID;
                            _drInsert999["TabColName"] = "Del";
                            _drInsert999["Title"] = "删除";
                            _drInsert999["DataType"] = "bit";
                            _drInsert999["FieldLength"] = null;  //...
                            _drInsert999["DefaultValue"] = false;
                            _drInsert999["AllowNull"] = true;
                            _drInsert999["PrimaryKey"] = false;
                            _drInsert999["Description"] = "Boolean，删除，默认值False，为了保护数据的安全性，这里的删除非真的删除只是在查询结果对所有角色不再显示";
                            _drInsert999["colCustomSort"] = 999;
                            _drInsert999["colTitle"] = "删除";
                            _drInsert999["colCustomField"] = null;
                            _drInsert999["colAlign"] = "center";
                            _drInsert999["colEdit"] = null;
                            _drInsert999["colTemplet"] = "#swDel";
                            _drInsert999["ctlAddDisplay"] = false;
                            _drInsert999["ctlModifyDisplay"] = false;
                            _drInsert999["ctlViewDisplay"] = false;
                            _drInsert999["ctlTitle"] = "删除";
                            _drInsert999["ctlTypes"] = null;
                            _drInsert999["ctlPrefix"] = null;
                            _drInsert999["ctlStyle"] = null;
                            _drInsert999["ctlDescription"] = "删除";
                            _drInsert999["ctlSourceTable"] = null;
                            _drInsert999["ctlTextName"] = null;
                            _drInsert999["ctlTextValue"] = null;
                            _drInsert999["ctlFilter"] = null;
                            _drInsert999["ctlVerify"] = null;
                            _drInsert999["ctlVerify2"] = null;
                            _drInsert999["ctlValue"] = null;//..
                            _drInsert999["ctlDefaultValue"] = null;
                            _drInsert999["ctlCharLength"] = null; //..
                            _drInsert999["ctlExtendJSCode"] = null; //..
                            _drInsert999["ctlGroup"] = 999;
                            _drInsert999["ctlInputInline"] = "layui-input-inline";
                            _drInsert999["JoinTableColumn"] = true;
                            _dtInsert.Rows.Add(_drInsert999);

                            if (MsSQLDbHelper.SqlBulkCopyInsert(_dtInsert, "MicroTables"))
                                flag = "True创建表成功<br/>Successful table creation";
                        }

                    }
                    else
                        flag = ShortTableName + "短表名已存在，请输入其它的短表名<br/>Short form name already exists. Please enter another short form name.";

                }
                else
                    flag = TableName + "表已存在，请输入其它的表名<br/>The table already exists. Please enter another table name.";
            }
        }
        catch (Exception e) { flag = e.toStringTrim(); }
        finally { }

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