<%@ WebHandler Language="C#" Class="AddMicroDataTableColumn" %>

using System;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using MicroPublicHelper;
using MicroDBHelper;
using Newtonsoft.Json.Linq;

public class AddMicroDataTableColumn : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        string flag = "提交失败，请检查URL参数<br/>The submission failed. Check the URL parameters",
                        Action = HttpUtility.UrlEncode(context.Request.Form["action"]),
                        MID = context.Server.UrlDecode(context.Request.Form["mid"]),
                        Fields = context.Server.UrlDecode(context.Request.Form["fields"]);

        if (!string.IsNullOrEmpty(Action) && !string.IsNullOrEmpty(MID) && !string.IsNullOrEmpty(Fields))
        {
            if (Action.Trim().ToLower() == "addcol")
                flag = AddColumn(MID.Trim(), Fields.Trim());
        }

        context.Response.Write(flag);

    }

    /// <summary>
    /// 向表添加列
    /// </summary>
    /// <param name="mid"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    protected string AddColumn(string MID, string Fields)
    {
        string flag = "添加失败<br/>Add failure", TableName = string.Empty, ParentID = string.Empty, TabColName = string.Empty, Title = string.Empty, DataType = string.Empty, _DataType = string.Empty, FieldLength = string.Empty, AllowNull = string.Empty, DefaultValue = string.Empty, _DefaultValue=string.Empty, Description = string.Empty, colAlign = "center", colEdit = string.Empty, ctlTitle = string.Empty, ctlTypes = string.Empty, ctlPrefix = string.Empty, ctlStyle = string.Empty, ctlCheckboxSkin = string.Empty, ctlDescription = string.Empty, ctlValue=string.Empty, ctlDefaultValue = string.Empty, ctlCharLength = string.Empty, ctlSourceTable = string.Empty, ctlTextName = string.Empty, ctlTextValue = string.Empty, ctlExtendJSCode = string.Empty, ctlFilter = string.Empty, ctlVerify = string.Empty, ctlVerify2 = string.Empty, ctlInputInline = "layui-input-inline";

        //用于生成Layui数据表时显示自定义字段，如用户所属部门保存的是用户ID，想要显示的是部门名称而该值正是这个要显示的字段
        //该值与“ctlSourceTable”“ctlTextName”“ctlTextValue”相配合使用，这三个值不为空时该字段默认赋值为“ctlTextName”
        string colCustomField = string.Empty;

        try
        {
            dynamic json = JToken.Parse(Fields) as dynamic;

            ParentID = json["selTableName"];
            TableName = json["txtTableName"]; TableName = TableName.toStringTrim();
            TabColName = json["txtTabColName"];TabColName = TabColName.toStringTrim();
            //Title = TabColName;
            DataType = json["selDataType"];  //DataType =  nvarchar(200) 
            _DataType = DataType; // nvarchar 
            FieldLength = json["txtFieldLength"]; FieldLength = FieldLength.toStringTrim();

            DefaultValue = json["txtDefaultValue"]; DefaultValue = DefaultValue.toStringTrim();
            _DefaultValue = DefaultValue;
            if (!string.IsNullOrEmpty(DefaultValue))
                DefaultValue = " default " + DefaultValue;

            AllowNull = json["ckAllowNull"];  AllowNull = AllowNull == "on" ? " null " : " not null ";
            Description = json["txtDescription"]; Description = Description.toStringTrim();
            ctlTitle = json["txtctlTitle"]; ctlTitle = ctlTitle.toStringTrim();
            ctlTypes = json["ctlTypes"]; ctlTypes = ctlTypes.toStringTrim();
            //ctlPrefix = string.Empty;
            //ctlStyle = string.Empty;
            //ctlCheckboxSkin = string.Empty;
            ctlDescription = Description;
            ctlValue = json["ctlValue"]; ctlValue = ctlValue.toStringTrim();
            ctlDefaultValue = json["ctlDefaultValue"]; ctlDefaultValue = ctlDefaultValue.toStringTrim();
            //ctlCharLength = FieldLength;
            ctlSourceTable = json["ctlSourceTable"]; ctlSourceTable = ctlSourceTable.toStringTrim();
            ctlTextName = json["ctlTextName"]; ctlTextName = ctlTextName.toStringTrim();
            ctlTextValue = json["ctlTextValue"]; ctlTextValue = ctlTextValue.toStringTrim();
            //ctlExtendJSCode = string.Empty;
            //ctlFilter = string.Empty;
            //ctlVerify = string.Empty;
            ctlVerify2 = json["ctlVerify2"]; ctlVerify2 = ctlVerify2.toStringTrim();
            //ctlGroup = string.Empty;
            //ctlInputInline = string.Empty;

            if (!string.IsNullOrEmpty(ctlSourceTable) && !string.IsNullOrEmpty(ctlTextName) && !string.IsNullOrEmpty(ctlTextValue))
                colCustomField = ctlTextName;

            Title = string.IsNullOrEmpty(ctlTitle) == true ? TabColName : ctlTitle;

            if (!string.IsNullOrEmpty(TableName) && !string.IsNullOrEmpty(DataType) && !string.IsNullOrEmpty(TabColName))
            {
                //根据SQL数据类型，动态得到创建SQL字段命令的所需参数
                switch (DataType)
                {
                    case "bit":
                        DataType = " bit ";
                        FieldLength = string.Empty;
                        if (_DefaultValue == "0")
                            _DefaultValue = "False";
                        else if (_DefaultValue == "1")
                            _DefaultValue = "True";
                        break;
                    case "char":
                        DataType = " char(" + FieldLength + ") ";
                        break;
                    case "date":  
                        DataType = " datetime ";
                        FieldLength = string.Empty;
                        break;
                    case "time":
                        DataType = " datetime ";
                        FieldLength = string.Empty;
                        break;
                    case "datetime":
                        DataType = " datetime ";
                        FieldLength = string.Empty;
                        break;
                    case "decimal":
                        DataType = " decimal(" + FieldLength + ") ";
                        break;
                    case "float":
                        DataType = " float ";
                        FieldLength = string.Empty;
                        break;
                    case "int":
                        DataType = " int ";
                        FieldLength = string.Empty;
                        break;
                    case "nchar":
                        DataType = " nchar(" + FieldLength + ") ";
                        break;
                    case "ntext":
                        DataType = " ntext ";
                        FieldLength = string.Empty;
                        break;
                    case "nvarchar":
                        DataType = " nvarchar(" + FieldLength + ") ";
                        break;
                    case "text":
                        DataType = " text ";
                        FieldLength = string.Empty;
                        break;
                    case "varchar":
                        DataType = " varchar(" + FieldLength + ") ";
                        break;
                }

                ctlCharLength = FieldLength;

                if (!MsSQLDbHelper.ColumnExists(TableName, TabColName))  //确认某表的某列是否存在，不存在的时候
                {
                    string _sql = "alter table " + TableName + " add " + TabColName + DataType + AllowNull + DefaultValue;  //向表添加列
                    MsSQLDbHelper.ExecuteSql(_sql);

                    //获取当前个数加上1，作为排序Sort的值
                    string _sql3 = "  select COUNT(*) from MicroTables where ParentID in (select TID from MicroTables where TabColName=@TabColName) and Sort < 500";
                    SqlParameter[] _sp3 = { new SqlParameter("@TabColName", SqlDbType.VarChar, 100) };
                    _sp3[0].Value = TableName;
                    string Count = MicroPublic.GetSingleField(0, _sql3, _sp3);
                    int Sort = Count.toInt() + 1;
                    int ctlGroup = Sort;

                    switch (ctlTypes)
                    {
                        case "Text":
                            colEdit = "text";
                            ctlPrefix = "txt";
                            if (DataType == "int")
                                ctlVerify = "onlyNum";
                            break;
                        case "Date":  //日期 yyyy-MM-dd
                            colEdit = "text";
                            ctlPrefix = "txt";
                            ctlTypes = "Text";
                            ctlStyle = " readonly=&quot;readonly&quot; ";
                            ctlExtendJSCode = "laydate.render({ elem: '#" + ctlPrefix + TabColName + "', trigger:'click'});";
                            break;
                        case "Time":  //时间 HH:mm
                            colEdit = "text";
                            ctlPrefix = "txt";
                            ctlTypes = "Text";
                            ctlStyle = " readonly=&quot;readonly&quot; ";
                            ctlExtendJSCode = "laydate.render({ elem: '#" + ctlPrefix + TabColName + "', type:'time', trigger:'click', format:'HH:mm' });";
                            break;
                        case "DateTime": //日期时间 yyyy-MM-dd HH:mm:ss
                            colEdit = "text";
                            ctlPrefix = "txt";
                            ctlTypes = "Text";
                            ctlStyle = " readonly=&quot;readonly&quot; ";
                            ctlExtendJSCode = "laydate.render({ elem: '#" + ctlPrefix + TabColName + "', type:'datetime', trigger:'click'});";
                            break;
                        case "DateWeek":  //日期星期 yyyy-MM-dd(Week)
                            colEdit = "text";
                            ctlPrefix = "txt";
                            ctlTypes = "Text";
                            ctlStyle = " readonly=&quot;readonly&quot; ";
                            ctlExtendJSCode = "laydate.render({ elem: '#" + ctlPrefix + TabColName + "', trigger:'click' });";
                            break;
                        case "DateWeekTime": //日期星期时间 yyyy-MM-dd(Week)HH:mm:ss
                            colEdit = "text";
                            ctlPrefix = "txt";
                            ctlTypes = "Text";
                            ctlStyle = " readonly=&quot;readonly&quot; ";
                            ctlExtendJSCode = "laydate.render({ elem: '#" + ctlPrefix + TabColName + "', trigger:'click', type:'datetime'});";
                            break;
                        case "Password":
                            ctlPrefix = "txt";
                            break;
                        case "Hidden":
                            ctlPrefix = "hid";
                            break;
                        case "Radio":
                            ctlPrefix = "ra";
                            ctlFilter = ctlPrefix + TabColName;
                            break;
                        case "CheckBox":
                            ctlPrefix = "cb";
                            ctlCheckboxSkin = "primary";
                            ctlFilter = ctlPrefix + TabColName;
                            break;
                        case "Select":
                            ctlPrefix = "sel";
                            ctlFilter = ctlPrefix + TabColName;
                            break;
                        case "Div":
                            ctlPrefix = "div";
                            break;
                        case "RadioTreeSelect":
                            ctlPrefix = "sel";
                            break;
                        case "CheckBoxTreeSelect":
                            ctlPrefix = "sel";
                            break;
                        case "RadioCascaderSelect":
                            ctlPrefix = "sel";
                            break;
                        case "CheckBoxCascaderSelect":
                            ctlPrefix = "sel";
                            break;
                        case "RadioCascaderSelectUserByDept":
                            ctlPrefix = "sel";
                            break;
                        case "CheckBoxCascaderSelectUserByDept":
                            ctlPrefix = "sel";
                            break;
                        case "Textarea":
                            ctlPrefix = "txt";
                            break;
                        case "LayEdit":
                            ctlPrefix = "txt";
                            break;
                        case "ImgUpload":
                            ctlPrefix = "img";
                            break;
                        case "FileUpload":
                            ctlPrefix = "file";
                            break;
                    }

                    string _sql2 = "insert into MicroTables (Sort, ParentID, TabColName, Title, DataType, FieldLength, DefaultValue, AllowNull, Description, colAlign, colEdit, ctlTitle, ctlTypes, ctlPrefix, ctlStyle, ctlCheckboxSkin, ctlDescription, ctlValue, ctlDefaultValue, ctlCharLength, ctlSourceTable, ctlTextName, ctlTextValue, ctlExtendJSCode, ctlFilter, ctlVerify, ctlVerify2, ctlGroup, ctlInputInline, colCustomSort, colTitle, colCustomField) values (@Sort, @ParentID, @TabColName, @Title, @DataType, @FieldLength, @DefaultValue, @AllowNull, @Description, @colAlign, @colEdit, @ctlTitle, @ctlTypes, @ctlPrefix, @ctlStyle, @ctlCheckboxSkin, @ctlDescription, @ctlValue, @ctlDefaultValue, @ctlCharLength, @ctlSourceTable, @ctlTextName, @ctlTextValue, @ctlExtendJSCode, @ctlFilter, @ctlVerify, @ctlVerify2, @ctlGroup, @ctlInputInline, @colCustomSort, @colTitle, @colCustomField)";

                    SqlParameter[] _sp2 = {
                                 new SqlParameter("@Sort", SqlDbType.Int),
                                 new SqlParameter("@ParentID", SqlDbType.Int),
                                 new SqlParameter("@TabColName", SqlDbType.VarChar,100),
                                 new SqlParameter("@Title", SqlDbType.VarChar,100),
                                 new SqlParameter("@DataType", SqlDbType.VarChar,100),
                                 new SqlParameter("@FieldLength", SqlDbType.VarChar,100),
                                 new SqlParameter("@DefaultValue", SqlDbType.VarChar,20),
                                 new SqlParameter("@AllowNull", SqlDbType.Bit),
                                 new SqlParameter("@Description", SqlDbType.NVarChar,4000),
                                 new SqlParameter("@colAlign", SqlDbType.VarChar,20),
                                 new SqlParameter("@colEdit", SqlDbType.VarChar,20),
                                 new SqlParameter("@ctlTitle", SqlDbType.VarChar,50),
                                 new SqlParameter("@ctlTypes", SqlDbType.VarChar,200),
                                 new SqlParameter("@ctlPrefix", SqlDbType.VarChar,30),
                                 new SqlParameter("@ctlStyle", SqlDbType.VarChar,500),
                                 new SqlParameter("@ctlCheckboxSkin", SqlDbType.VarChar,100),
                                 new SqlParameter("@ctlDescription", SqlDbType.NVarChar,4000),
                                 new SqlParameter("@ctlValue", SqlDbType.VarChar,8000),
                                 new SqlParameter("@ctlDefaultValue", SqlDbType.VarChar,8000),
                                 new SqlParameter("@ctlCharLength", SqlDbType.VarChar,30),
                                 new SqlParameter("@ctlSourceTable", SqlDbType.VarChar,100),
                                 new SqlParameter("@ctlTextName", SqlDbType.VarChar,100),
                                 new SqlParameter("@ctlTextValue", SqlDbType.VarChar,100),
                                 new SqlParameter("@ctlExtendJSCode", SqlDbType.VarChar,8000),
                                 new SqlParameter("@ctlFilter", SqlDbType.VarChar,100),
                                 new SqlParameter("@ctlVerify", SqlDbType.VarChar,100),
                                 new SqlParameter("@ctlVerify2", SqlDbType.VarChar,100),
                                 new SqlParameter("@ctlGroup", SqlDbType.Int),
                                 new SqlParameter("@ctlInputInline", SqlDbType.VarChar,100),
                                 new SqlParameter("@colCustomSort", SqlDbType.Int),
                                 new SqlParameter("@colTitle", SqlDbType.VarChar,100),
                                 new SqlParameter("@colCustomField", SqlDbType.VarChar,100)
                        };

                    _sp2[0].Value = Sort;
                    _sp2[1].Value = ParentID.toInt();
                    _sp2[2].Value = TabColName;
                    _sp2[3].Value = Title;
                    _sp2[4].Value = _DataType;
                    _sp2[5].Value = FieldLength;
                    _sp2[6].Value = _DefaultValue;
                    _sp2[7].Value = AllowNull.toStringTrim() == "null" ? true : false;
                    _sp2[8].Value = Description;
                    _sp2[9].Value = colAlign;
                    _sp2[10].Value = colEdit;
                    _sp2[11].Value = ctlTitle;
                    _sp2[12].Value = ctlTypes;
                    _sp2[13].Value = ctlPrefix;
                    _sp2[14].Value = ctlStyle;
                    _sp2[15].Value = ctlCheckboxSkin;
                    _sp2[16].Value = ctlDescription;
                    _sp2[17].Value = ctlValue;
                    _sp2[18].Value = ctlDefaultValue;
                    _sp2[19].Value = ctlCharLength;
                    _sp2[20].Value = ctlSourceTable;
                    _sp2[21].Value = ctlTextName;
                    _sp2[22].Value = ctlTextValue;
                    _sp2[23].Value = ctlExtendJSCode;
                    _sp2[24].Value = ctlFilter;
                    _sp2[25].Value = ctlVerify;
                    _sp2[26].Value = ctlVerify2;
                    _sp2[27].Value = ctlGroup;
                    _sp2[28].Value = ctlInputInline;
                    _sp2[29].Value = Sort;
                    _sp2[30].Value = Title;
                    _sp2[31].Value = colCustomField;

                    MsSQLDbHelper.ExecuteSql(_sql2, _sp2);

                    flag = MicroPublic.GetMsg("Save");
                }
                else
                    flag = "表" + TableName + "的列名" + TabColName + "已存在，请输入其它的列名<br/>The table already exists. Please enter another table name.";
            }
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