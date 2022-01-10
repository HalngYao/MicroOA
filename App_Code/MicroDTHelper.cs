using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using MicroDBHelper;
using MicroPublicHelper;
using MicroUserHelper;
using MicroAuthHelper;
using ClosedXML.Excel;

/// <summary>
/// DataTable数据表
/// </summary>
namespace MicroDTHelper
{

    public class MicroDataTable
    {

        #region region获取应用于LayuiDataTable的JSON格式的数据

        /// <summary>
        /// 传入短表名调用GetTableAttr获取表的属性，获取生成应用于LayuiDataTable数据格式(JSON)的数据。【该方法是GetMainSubLayuiDataTableList和GetSingleLayuiDataTableList方法的集合，适用于所有的表(有父子关系的和没有父子关系的)，有父子关系的经过ParentID,Sort排序，同时开启分页的话根据分页设定的数量进行输出】
        /// </summary>
        /// <param name="ShortTableName">短表名</param>
        /// <param name="Page">页码。默认缺省值：IntPage = 1</param>
        /// <param name="Limit">每页显示数量。默认缺省值：IntLimit = 10</param>
        /// <param name="GetPage">获取分页数据，默认缺省值True ***注：为True值的情况下且还要依赖于表设定的参数IsPage的值来判断是否获取分面数据，但有时候你希望获得所有的数据，则该值传入False即可***</param>
        /// <param name="QueryType">查询类型配合Keyword使用。接受内容  “Field:DataType(字段:数据类型)”或“All”。当QueryType=Field:DataType格式时， Field=需要查询的字段用于接拼到sql语句中，DataType查询的数据字段类型如：int,varchar,nvarchar等，当QueryType=All时，直接查询表中所有的文本字段。默认缺省值：QueryType = "All"</param>
        /// <param name="Keyword">关键字</param>
        /// <param name="GetMainOrSub">只针对有父子关系的表，获取只Main数据或只Sub数据或MainSub全部数据</param>
        /// <returns>返回构造好应用于LayuiDataTable的json字符串"{\"code\": 0, \"msg\": \"\", \"count\": " + SourceCount + ", \"data\":  " + strTpl + " }"</returns>
        public static string GetLayuiDataTableList(string ShortTableName, string Page = "1", string Limit = "10", Boolean GetPage = true, string QueryType = "All", string Keyword = "", string GetMainOrSub = "MainSub", DataTable CustomDataTable = null, string CustomSortAscDesc = "")
        {
            string flag = "False", TableName = string.Empty;

            try
            {
                //获取长表名
                TableName = MicroPublic.GetTableName(ShortTableName);

                if (!string.IsNullOrEmpty(TableName))
                {
                    //获取表的字段和相关属性
                    var getTableAttr = GetTableAttr(TableName);
                    DataTable MicroDT = getTableAttr.MicroDT; //获取存放在MicroTables的数据，该表是存放每个表的表名和字段
                    string PrimaryKeyName = getTableAttr.PrimaryKeyName;  //获得主键字段名称
                    string SortFieldName = getTableAttr.SortFieldName;  //获得排序字段名称
                    string AscDesc = " " + getTableAttr.AscDesc;
                    string SortFieldNameAscDesc = getTableAttr.SortFieldNameAscDesc;  //获得排序字段名称

                    if (!string.IsNullOrEmpty(CustomSortAscDesc))
                        SortFieldNameAscDesc = CustomSortAscDesc;

                    string OrderBy = getTableAttr.OrderBy;  //获得order by 命令字符串
                    Boolean MainSub = getTableAttr.MainSub;  //用于判断是否开启父子项
                    Boolean IsPage = getTableAttr.IsPage; //是否开启分页
                    string QueryField = getTableAttr.QueryField;

                    //构造dt.Selece()的排序语句，与sql语句排序不同不需要order by，只需要 Field1 asc or desc 
                    SortFieldName = string.IsNullOrEmpty(SortFieldName) == true ? "" : SortFieldName + AscDesc;

                    IsPage = GetPage == true ? IsPage : false;  //GetPage等于True时返回IsPage的值，否则设置IsPage值为False(=False即代表返回所有不分页的记录)

                    //如果开启了父子关系的项，则从GetMainSubLayuiDataTableList方法获取记录，否则从GetSingleLayuiDataTableList()方法获取
                    if (MainSub)
                        flag = GetMainSubLayuiDataTableList(MicroDT, TableName, PrimaryKeyName, SortFieldName, SortFieldNameAscDesc, OrderBy, IsPage, Page.toInt(), Limit.toInt(), QueryType, QueryField, Keyword, GetMainOrSub, CustomDataTable);
                    else
                        flag = GetSingleLayuiDataTableList(MicroDT, TableName, SortFieldName, OrderBy, SortFieldNameAscDesc, IsPage, Page.toInt(), Limit.toInt(), QueryType, QueryField, Keyword, CustomDataTable);
                }
            }
            catch (Exception ex) { flag = ex.ToString(); }

            return flag;
        }

        /// <summary>
        /// 调用GetMainSubDataTableArrt方法，生成应用于LayuiDataTable数据格式(JSON)的数据【紧适用于“含有”父子关系的数据表】
        /// </summary>
        /// <param name="MicroDT">MicroDT</param>
        /// <param name="TableName">表名</param>
        /// <param name="PrimaryKeyName">表的主键名称</param>
        /// <param name="SortFieldName">排序字段，如Field1,Field2</param>
        /// <param name="OrderBy">order by 语句，order by Field1,Field2</param>
        /// <param name="IsPage">开启分页。默认缺省值：True</param>
        /// <param name="IntPage">页码。默认缺省值：IntPage = 1</param>
        /// <param name="IntLimit">每页显示数量。默认缺省值：IntLimit = 10</param>
        /// <returns>返回构造好应用于LayuiDataTable的json字符串"{\"code\": 0, \"msg\": \"\", \"count\": " + SourceCount + ", \"data\":  " + strTpl + " }"</returns>
        private static string GetMainSubLayuiDataTableList(DataTable MicroDT, string TableName, string PrimaryKeyName, string SortFieldName, string SortFieldNameAscDesc, string OrderBy, Boolean IsPage = true, int IntPage = 1, int IntLimit = 10, string QueryType = "All", string QueryField = "", string Keyword = "", string GetMainOrSub = "MainSub", DataTable CustomDataTable = null)
        {
            string flag = "False", strTpl = string.Empty;
            DataSet _ds = new DataSet();
            DataTable _dt = new DataTable();

            //得到重新构造好的表及相关信息
            var getMainSubDataTableArrt = GetMainSubDataTableArrt(MicroDT, TableName, PrimaryKeyName, SortFieldName, SortFieldNameAscDesc, OrderBy, IsPage, IntPage, IntLimit, QueryType, QueryField, Keyword, GetMainOrSub, CustomDataTable);

            if (IsPage)
                _dt = getMainSubDataTableArrt.TargetDT;  //开启分页时返回分页的记录
            else
                _dt = getMainSubDataTableArrt.SourceDT;  //不开启分页时返回所有记录

            //无论是否开启分页都要获取总记录数
            int SourceCount = getMainSubDataTableArrt.SourceCount;  //获得总记录数，作为分页用

            if (_dt != null && _dt.Rows.Count > 0)
            {
                //*******************Start**************************
                //先循环得到需要夸表查询的表存放到DataSet中。（避免在循环记录每次都需要读取一次数据库）
                DataRow[] microRows = MicroDT.Select("ParentID<>0 and colCustomField<>'' and ctlSourceTable<>'' and ctlTextName<>'' and ctlTextValue<>''", "Sort");
                foreach (DataRow microDR in microRows)
                {
                    string SourceTable = microDR["ctlSourceTable"].toJsonTrim();  //得到数据来源表的名称
                    if (TableName.ToLower() != SourceTable.ToLower())  //数据来源表不等于表本身时才执行
                    {
                        DataTable _dtSourceTable = new DataTable();
                        _dtSourceTable = GetSingleDataTable(SourceTable);  //传入表名返回DataTable
                        _dtSourceTable.TableName = SourceTable;  //为DataTable命名

                        //判断DataSet中DataTable是否存在，不存在时添加到DataSet中
                        if (!_ds.Tables.Contains(_dtSourceTable.TableName))
                            _ds.Tables.Add(_dtSourceTable.Copy());  //把查询得到的表内容Copy到DataSet中
                    }
                }
                //*********************End************************

                DataRow[] _rows = _dt.Select("");  //这里不需要再排序SortFieldName。因为这个_dt是父子关系的，经过上一级的方法得到新构造的父子系统的数据表

                foreach (DataRow _dr in _rows)
                {
                    strTpl += "{";
                    strTpl += "\"MainSub\":\"" + _dr["MainSub"].toJsonTrim() + "\",";
                    DataRow[] MicroRows = MicroDT.Select("ParentID<>0", "Sort");

                    foreach (DataRow MicroDR in MicroRows) //与类同 //for (int i = 0; i < _dt.Rows.Count; i++)
                    {
                        string strText = MicroDR["TabColName"].toJsonTrim(), strValue = _dr["" + MicroDR["TabColName"].toJsonTrim() + ""].toJsonTrim();

                        //记录为日期时间时把值格式化为控件类型的样式
                        string ctlTypes = MicroDR["ctlTypes"].toJsonTrim(),
                               ctlPlaceholder = MicroDR["ctlPlaceholder"].toJsonTrim();

                        switch (ctlTypes)
                        {
                            case "Date":
                                strValue = strValue.toDateFormat(ctlPlaceholder);
                                break;
                            case "Time":
                                strValue = strValue.toDateFormat(!string.IsNullOrEmpty(ctlPlaceholder) ? ctlPlaceholder : "HH:mm:ss");
                                break;
                            case "DateTime":
                                strValue = strValue.toDateFormat(!string.IsNullOrEmpty(ctlPlaceholder) ? ctlPlaceholder : "yyyy-MM-dd HH:mm:ss");
                                break;
                            case "DateWeek":
                                strValue = strValue.toDateWeek();
                                break;
                            case "DateWeekTime":
                                strValue = strValue.toDateWeekTime("ww", ctlPlaceholder);
                                break;
                        }

                        //构造字符串
                        strTpl += "\"" + strText + "\":\"" + strValue + "\",";  //如："DeptID":"21"

                        //**********************************Start****************************************
                        //从DataSet中读取出DataTable，通过_dt.Select()条件查询得到需要显示的字段内容
                        string ctlSourceTable = string.Empty, colCustomField = string.Empty, ctlTextName = string.Empty, ctlTextValue = string.Empty;
                        colCustomField = MicroDR["colCustomField"].toJsonTrim(); //自定义字段
                        ctlSourceTable = MicroDR["ctlSourceTable"].toJsonTrim();  //数据来源表
                        ctlTextName = MicroDR["ctlTextName"].toJsonTrim();  //显示字段
                        ctlTextValue = MicroDR["ctlTextValue"].toJsonTrim();  //值字段

                        if (!string.IsNullOrEmpty(colCustomField) && !string.IsNullOrEmpty(ctlSourceTable) && !string.IsNullOrEmpty(ctlTextName) && !string.IsNullOrEmpty(ctlTextValue))
                        {
                            if (TableName.ToLower() != ctlSourceTable.ToLower())   //数据来源表的名称不等于本DataTable本身名称时，如部门表的上级部门，数据据来源表是部门表本身
                            {
                                if (_ds != null)
                                {
                                    if (!string.IsNullOrEmpty(strValue))
                                    {
                                        DataTable _dtSourceTable = _ds.Tables[ctlSourceTable];
                                        DataRow[] _rowsSourceTable = _dtSourceTable.Select(ctlTextValue + " in (" + strValue + ")");
                                        //DataRow[] _rowsSourceTable = _dtSourceTable.Select(ctlTextValue + " in (15,421)");
                                        //通过自定义字段colCustomField，多生成一条数据记录（即DeptID字段和DeptName字段同时生成），在生成Layui数据表时通过表头控制需要显示的数据
                                        if (_rowsSourceTable.Count() > 0)
                                        {
                                            string colCustomFields = string.Empty;
                                            foreach (DataRow _drCustom in _rowsSourceTable)
                                            {
                                                colCustomFields += _drCustom[colCustomField].toJsonTrim() + "、";
                                            }
                                            colCustomFields = colCustomFields.Substring(0, colCustomFields.Length - 1);
                                            strTpl += "\"" + colCustomField + "\":\"" + colCustomFields + "\",";  //如："DeptName":"总务科、财务科"
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //以主键=ParentID，筛选出显示名称
                                if (strValue.toInt() != 0)  //如ID=0时会报错，所以ID不等于0时执行
                                {
                                    DataRow[] _rows2 = _dt.Select(ctlTextValue + "=" + strValue);
                                    if (_rows2.Count() > 0)
                                        strTpl += "\"" + strText + "\":\"" + _rows2[0][ctlTextName].toJsonTrim() + "\",";
                                }
                            }

                        }
                        //*************************************End*************************************
                    }

                    strTpl = strTpl.Substring(0, strTpl.Length - 1);
                    strTpl += "},";
                }

                strTpl = strTpl.Substring(0, strTpl.Length - 1);

            }

            flag = "{\"code\": 0, \"msg\": \"\", \"count\": " + SourceCount + ", \"data\": [" + strTpl + "] }";

            return flag;
        }

        /// <summary>
        /// 调用GetSingleDataTableAttr方法，生成应用于LayuiDataTable数据格式(JSON)的数据【适用于“没有”父子关系的数据表】
        /// </summary>
        /// <param name="MicroDT">MicroDT</param>
        /// <param name="TableName">表名</param>
        /// <param name="SortFieldName">排序字段，如Field1,Field2</param>
        /// <param name="OrderBy">order by 语句，order by Field1,Field2</param>
        /// <param name="IsPage">开启分页</param>
        /// <param name="IntPage">页码。默认缺省值：IntPage = 1</param>
        /// <param name="IntLimit">每页显示数量。默认缺省值：IntLimit = 10</param>
        /// <returns>返回构造好应用于LayuiDataTable的json字符串"{\"code\": 0, \"msg\": \"\", \"count\": " + SourceCount + ", \"data\":  " + strTpl + " }"</returns>
        private static string GetSingleLayuiDataTableList(DataTable MicroDT, string TableName, string SortFieldName, string OrderBy, string SortFieldNameAscDesc, Boolean IsPage, int IntPage = 1, int IntLimit = 10, string QueryType = "All", string QueryField = "", string Keyword = "", DataTable CustomDataTable = null)
        {
            string flag = "False", strTpl = string.Empty;

            try
            {
                DataSet _ds = new DataSet();
                DataTable _dt = new DataTable();

                //根据表名返回表的相关属性，如数据表、记录总数量
                var getSingleDataTableAttr = GetSingleDataTableAttr(TableName, OrderBy, IsPage, IntPage, IntLimit, QueryType, QueryField, Keyword, CustomDataTable);

                if (IsPage)
                    _dt = getSingleDataTableAttr.TargetDT; //开启分页时返回分页的记录
                else
                    _dt = getSingleDataTableAttr.SourceDT; //不开启分页时返回所有记录

                //无论是否开启分页都要获取总记录数
                int SourceCount = getSingleDataTableAttr.SourceCount; //获得总记录数，作为分页用

                if (_dt != null && _dt.Rows.Count > 0)
                {
                    //*******************跨表查询Start**************************
                    //先循环得到需要跨表查询的表存放到DataSet中。（避免在循环记录每次都需要读取一次数据库）
                    DataRow[] microRows = MicroDT.Select("colCustomField<>'' and ctlSourceTable<>'' and ctlTextName<>'' and ctlTextValue<>''", "Sort");
                    foreach (DataRow microDR in microRows)
                    {
                        string SourceTable = microDR["ctlSourceTable"].toJsonTrim();  //得到数据来源表的名称
                        if (TableName.ToLower() != SourceTable.ToLower()) //数据来源表不等于表本身时才执行
                        {
                            DataTable _dtSourceTable = new DataTable();
                            _dtSourceTable = GetSingleDataTable(SourceTable);  //传入表名返回DataTable
                            _dtSourceTable.TableName = SourceTable;  //为DataTable命名

                            //判断DataSet中DataTable是否存在，不存在时添加到DataSet中
                            if (!_ds.Tables.Contains(_dtSourceTable.TableName))
                                _ds.Tables.Add(_dtSourceTable.Copy());  //把查询得到的表内容Copy到DataSet中
                        }
                    }
                    //*********************跨表查询End************************

                    DataRow[] _rows = _dt.Select("", SortFieldNameAscDesc);

                    foreach (DataRow _dr in _rows)
                    {
                        strTpl += "{";
                        DataRow[] MicroRows = MicroDT.Select("ParentID<>0", "Sort");

                        foreach (DataRow MicroDR in MicroRows) //与类同 //for (int i = 0; i < _dt.Rows.Count; i++)
                        {
                            string strText = MicroDR["TabColName"].toJsonTrim(), strValue = _dr["" + MicroDR["TabColName"].toJsonTrim() + ""].toJsonTrim();

                            //记录为日期时间时把值格式化为控件类型的样式
                            string ctlTypes = MicroDR["ctlTypes"].toJsonTrim();
                            switch (ctlTypes)
                            {
                                case "Date":
                                    strValue = strValue.toDateFormat();
                                    break;
                                case "Time":
                                    strValue = strValue.toDateFormat("HH:mm:ss");
                                    break;
                                case "DateTime":
                                    strValue = strValue.toDateFormat("yyyy-MM-dd HH:mm:ss");
                                    break;
                                case "DateWeek":
                                    strValue = strValue.toDateWeek();
                                    break;
                                case "DateWeekTime":
                                    strValue = strValue.toDateWeekTime();
                                    break;
                            }

                            //构造字符串
                            strTpl += "\"" + strText + "\":\"" + strValue + "\",";  //如："DeptID":"21"

                            //**********************************跨表查询Start****************************************
                            //从DataSet中读取出DataTable，通过_dt.Select()条件查询得到需要显示的字段内容
                            string ctlSourceTable = string.Empty, colCustomField = string.Empty, ctlTextName = string.Empty, ctlTextValue = string.Empty;
                            colCustomField = MicroDR["colCustomField"].toJsonTrim(); //自定义字段
                            ctlSourceTable = MicroDR["ctlSourceTable"].toJsonTrim();  //数据来源表
                            ctlTextName = MicroDR["ctlTextName"].toJsonTrim();  //显示字段
                            ctlTextValue = MicroDR["ctlTextValue"].toJsonTrim();  //值字段

                            if (!string.IsNullOrEmpty(colCustomField) && !string.IsNullOrEmpty(ctlSourceTable) && !string.IsNullOrEmpty(ctlTextName) && !string.IsNullOrEmpty(ctlTextValue))
                            {
                                if (TableName.ToLower() != ctlSourceTable.ToLower())  //数据来源表的名称不等于本DataTable本身名称时，如部门表的上级部门，数据据来源表是部门表本身
                                {
                                    if (_ds != null)
                                    {
                                        if (!string.IsNullOrEmpty(strValue))
                                        {
                                            DataTable _dtSourceTable = _ds.Tables[ctlSourceTable];
                                            DataRow[] _rowsSourceTable = _dtSourceTable.Select(ctlTextValue + " in (" + strValue + ")");
                                            //DataRow[] _rowsSourceTable = _dtSourceTable.Select(ctlTextValue + " in (15,421)");
                                            //通过自定义字段colCustomField，多生成一条数据记录（即DeptID字段和DeptName字段同时生成），在生成Layui数据表时通过表头控制需要显示的数据
                                            if (_rowsSourceTable.Count() > 0)
                                            {
                                                string colCustomFields = string.Empty;
                                                foreach (DataRow _drCustom in _rowsSourceTable)
                                                {
                                                    colCustomFields += _drCustom[colCustomField].toJsonTrim() + "、";
                                                }
                                                colCustomFields = colCustomFields.Substring(0, colCustomFields.Length - 1);
                                                strTpl += "\"" + colCustomField + "\":\"" + colCustomFields + "\",";  //如："DeptName":"总务科、财务科"
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //以主键=ParentID，筛选出显示名称
                                    if (strValue.toInt() != 0)  //如ID=0时会报错，所以ID不等于0时执行
                                    {
                                        DataRow[] _rows2 = _dt.Select(ctlTextValue + "=" + strValue);
                                        if (_rows2.Count() > 0)
                                            strTpl += "\"" + strText + "\":\"" + _rows2[0][ctlTextName].toJsonTrim() + "\",";
                                    }
                                }
                            }
                            //*************************************跨表查询End*************************************
                        }

                        strTpl = strTpl.Substring(0, strTpl.Length - 1);
                        strTpl += "},";
                    }

                    strTpl = strTpl.Substring(0, strTpl.Length - 1);

                }

                flag = "{\"code\": 0, \"msg\": \"\", \"count\": " + SourceCount + ", \"data\":  [" + strTpl + "] }";
            }
            catch (Exception ex) { flag = ex.ToString(); }

            return flag;

        }

        #endregion



        #region region获取SourceDT、TargetDT和数量、相关属性等

        /// <summary>
        /// set;get; DataTableAttr
        /// </summary>
        public class DataTableAttr
        {
            /// <summary>
            /// set;get;返回MicroDT，含有该表的表名和所有字段
            /// </summary>
            public DataTable MicroDT
            {
                set;
                get;
            }

            /// <summary>
            /// set;get;主键名称
            /// </summary>
            public string PrimaryKeyName
            {
                set;
                get;
            }

            /// <summary>
            /// set;get;主键的控件前缀
            /// </summary>
            public string CtlPrefix
            {
                set;
                get;
            }

            /// <summary>
            /// set;get;主键的数据类型
            /// </summary>
            public string DataType
            {
                set;
                get;
            }

            /// <summary>
            /// set;get;主键的字段长度
            /// </summary>
            public string FieldLength
            {
                set;
                get;
            }

            /// <summary>
            /// set;get;该表的排序字段名称，两个或以上由逗号进行分隔，如： Field1,Field2
            /// </summary>
            public string SortFieldName
            {
                set;
                get;
            }

            /// <summary>
            /// set;get;排序的方式 asc升序，desc降序
            /// </summary>
            public string AscDesc
            {
                set;
                get;
            }

            /// <summary>
            /// set;get;返回order by语句，如order by Field1,Field2
            /// </summary>
            public string OrderBy
            {
                set;
                get;
            }

            /// <summary>
            /// set;get;返回MainSub值，该值用于判断该表是否含有父子关系
            /// </summary>
            public Boolean MainSub
            {
                set;
                get;
            }

            /// <summary>
            /// set;get;是否开启分页
            /// </summary>
            public Boolean IsPage
            {
                set;
                get;
            }

            /// <summary>
            /// set;get;返回源表SourceDataTable
            /// </summary>
            public DataTable SourceDT
            {
                set;
                get;
            }

            /// <summary>
            /// set;get;返回SourceDataTable总记录数
            /// </summary>
            public int SourceCount
            {
                set;
                get;
            }

            /// <summary>
            /// set;get;返回被处理过的DataTable【被处理过的表，如：加入了分页条件，按页码和每页显示的数量进行查询的记录】
            /// </summary>
            public DataTable TargetDT
            {
                set;
                get;
            }

            /// <summary>
            /// set;get;返回TargetDataTable总记录数
            /// </summary>
            public int TargetCount
            {
                set;
                get;
            }

        }

        /// <summary>
        /// 传入短表名调用GetTableAttr获取表的属性，返回MicroDT、SourceDT、TargetDT、记录数量和该表的相关属性，如：主键名称、主键前缀、主键数据类型、排序字段名称、OrderBy等。【该方法是GetMainSubDataTableArrt和GetSingleDataTableAttr方法的集合适用于所有的表，有父子关系的和没有父子关系的，有父子关系的经过ParentID,Sort排序，同时开启分页的话根据分页设定的数量进行输出】
        /// </summary>
        /// <param name="ShortTableName">短表名</param>
        /// <param name="IsShortTableName">默认缺省值：True, 通过该值判断是否为短表名，是短表名的则需要转换为长表名，否则不需要</param>
        /// <param name="GetPage">获取分页数据，默认缺省值False(即返回所有数据) *注：如果为True值的情况下并且还要依赖于表设定的参数IsPage的值来判断是否获取分面数据，但有时候你希望获得所有的数据，则不传入该值或传入False即可</param>
        /// <param name="IntPage">页码。默认缺省值：IntPage = 1</param>
        /// <param name="IntLimit">每页显示数量。默认缺省值：IntLimit = 10</param>
        /// <returns></returns>
        public static DataTableAttr GetDataTableAttr(string ShortTableName, Boolean IsShortTableName = true, Boolean GetPage = false, int IntPage = 1, int IntLimit = 10)
        {
            string TableName = string.Empty, PrimaryKeyName = string.Empty, CtlPrefix = string.Empty, DataType = string.Empty, FieldLength = string.Empty, SortFieldName = string.Empty, AscDesc = string.Empty, SortFieldNameAscDesc = string.Empty, OrderBy = string.Empty;

            DataTable MicroDT = null, SourceDT = null, TargetDT = null;

            Boolean MainSub = false, IsPage = false;

            int SourceCount = 0, TargetCount = 0;

            //如果是短表名时则获取长表名，否则直接使用
            if (IsShortTableName)
                TableName = MicroPublic.GetTableName(ShortTableName);
            else
                TableName = ShortTableName;

            if (!string.IsNullOrEmpty(TableName))
            {
                //获取表的字段和相关属性
                var getTableAttr = MicroDataTable.GetTableAttr(TableName);
                MicroDT = getTableAttr.MicroDT; //获取存放在MicroTables的数据，该表是存放每个表的表名和字段
                PrimaryKeyName = getTableAttr.PrimaryKeyName;  //获得主键字段名称
                CtlPrefix = getTableAttr.CtlPrefix; //主键前缀
                DataType = getTableAttr.DataType; //主键数据类型
                FieldLength = getTableAttr.FieldLength; //主键字段长度
                SortFieldName = getTableAttr.SortFieldName;  //获得排序字段名称
                AscDesc = getTableAttr.AscDesc; //表记录的排序方式
                SortFieldNameAscDesc = getTableAttr.SortFieldNameAscDesc;
                OrderBy = getTableAttr.OrderBy;  //获得order by 命令字符串
                MainSub = getTableAttr.MainSub;  //用于判断是否开启父子项
                IsPage = getTableAttr.IsPage; //是否开启分页

                IsPage = GetPage == true ? IsPage : false;  //GetPage等于True时返回IsPage的值，否则设置IsPage值为False(=False即代表返回所有不分页的记录)

                if (MainSub)
                {
                    var getMainSubDataTableArrt = GetMainSubDataTableArrt(MicroDT, TableName, PrimaryKeyName, SortFieldName, SortFieldNameAscDesc, OrderBy, IsPage, IntPage, IntLimit);
                    SourceDT = getMainSubDataTableArrt.SourceDT;
                    SourceCount = getMainSubDataTableArrt.SourceCount;
                    TargetDT = getMainSubDataTableArrt.TargetDT;
                    TargetCount = getMainSubDataTableArrt.TargetCount;
                }
                else
                {
                    var getSingleDataTableAttr = GetSingleDataTableAttr(TableName, OrderBy, IsPage, IntPage, IntLimit);
                    SourceDT = getSingleDataTableAttr.SourceDT;
                    SourceCount = getSingleDataTableAttr.SourceCount;
                    TargetDT = getSingleDataTableAttr.TargetDT;
                    TargetCount = getSingleDataTableAttr.TargetCount;
                }
            }


            var DataTableAttr = new DataTableAttr
            {
                MicroDT = MicroDT,
                PrimaryKeyName = PrimaryKeyName,
                CtlPrefix = CtlPrefix,
                DataType = DataType,
                FieldLength = FieldLength,
                SortFieldName = SortFieldName,
                AscDesc = AscDesc,
                OrderBy = OrderBy,
                MainSub = MainSub,
                IsPage = IsPage,
                SourceDT = SourceDT,
                SourceCount = SourceCount,
                TargetDT = TargetDT,
                TargetCount = TargetCount
            };

            return DataTableAttr;

        }


        /// <summary>
        /// set;get;MainSubDataTableAttr
        /// </summary>
        private class MainSubDataTableAttr
        {
            /// <summary>
            /// set;get;返回源表SourceDT，未分页的DT
            /// </summary>
            public DataTable SourceDT
            {
                set;
                get;
            }

            /// <summary>
            /// set;get;返回SourceDT总记录数
            /// </summary>
            public int SourceCount
            {
                set;
                get;
            }

            /// <summary>
            /// set;get;返回TargetDT,被处理过的DataTable(根据设定的分页进行分页)
            /// </summary>
            public DataTable TargetDT
            {
                set;
                get;
            }

            /// <summary>
            /// set;get;返回TargetDT总记录数
            /// </summary>
            public int TargetCount
            {
                set;
                get;
            }
        }

        /// <summary>
        /// 传入表名调用GetMainDataTable方法，返回经过排序的Source表、Taget表和表的记录数(经过排序或分页，是否分页依赖IsPage参数进行判断)【紧适用于“含有”父子关系的表】
        /// </summary>
        /// <param name="MicroDT">MicroDT</param>
        /// <param name="TableName">表名</param>
        /// <param name="PrimaryKeyName">表的主键名称</param>
        /// <param name="SortFieldName">排序字段，如Field1,Field2</param>
        /// <param name="OrderBy">order by 语句，order by Field1,Field2</param>
        /// <param name="IsPage">是否启用分面</param>
        /// <param name="IntPage">页码。默认缺省值：IntPage = 1</param>
        /// <param name="IntLimit">每页显示数量。默认缺省值：IntLimit = 10</param>
        /// <returns>返回经过排序的Source表、Taget表和表的记录数(经过排序或分页，是否分页依赖IsPage参数进行判断)</returns>
        /// 
        private static MainSubDataTableAttr GetMainSubDataTableArrt(DataTable MicroDT, string TableName, string PrimaryKeyName, string SortFieldName, string SortFieldNameAscDesc, string OrderBy, Boolean IsPage, int IntPage = 1, int IntLimit = 10, string QueryType = "All", string QueryField = "", string Keyword = "", string GetMainOrSub = "MainSub", DataTable CustomDataTable = null)
        {
            DataTable SourceDT = null,
                TargetDT = null;
            int SourceCount = 0,
                TargetCount = 0;

            //调用GetMainDataTable方法返回经过排序的记录且重新生成的数据表，该GetMainDataTable方法会自动递归所有子项的记录
            SourceDT = GetMainDataTable(MicroDT, TableName, PrimaryKeyName, SortFieldName, SortFieldNameAscDesc, OrderBy, QueryType, QueryField, Keyword, GetMainOrSub, CustomDataTable);
            if (SourceDT != null && SourceDT.Rows.Count > 0)
                SourceCount = SourceDT.Rows.Count;

            //开启分页的情况下返回分页数据
            //使用AsEnumerable().Skip("跳过N条记录").Take("显示N条记录")
            if (SourceDT != null && SourceDT.Rows.Count > 0 && IsPage)
            {
                TargetDT = SourceDT.AsEnumerable().Skip((IntPage - 1) * IntLimit).Take(IntLimit).CopyToDataTable<DataRow>();
                TargetCount = TargetDT.Rows.Count;
            }

            var MainSubDataTableAttr = new MainSubDataTableAttr
            {
                SourceDT = SourceDT,
                SourceCount = SourceCount,
                TargetDT = TargetDT,
                TargetCount = TargetCount
            };

            return MainSubDataTableAttr;

        }


        /// <summary>
        /// set;get;返回DataTable或相关属性，如：数据表、总记录数【适用于“没有”父子关系的数据表】
        /// </summary>
        private class SingleDataTableAttr
        {
            /// <summary>
            /// set;get;返回源表SourceDT，未分页的DT
            /// </summary>
            public DataTable SourceDT
            {
                set;
                get;
            }

            /// <summary>
            /// set;get;返回SourceDT总记录数
            /// </summary>
            public int SourceCount
            {
                set;
                get;
            }

            /// <summary>
            /// set;get;返回TargetDT,被处理过的DataTable(根据设定的分页进行分页)
            /// </summary>
            public DataTable TargetDT
            {
                set;
                get;
            }

            /// <summary>
            /// set;get;返回TargetDT总记录数
            /// </summary>
            public int TargetCount
            {
                set;
                get;
            }
        }

        /// <summary>
        /// 传入表名调用GetSingleDataTable方法，返回Source表、Target表和表的记录数(Source表经过排序但未分页，Taget经过排序且已分页，是否分页还要依赖IsPage参数进行判断)【适用于“没有”父子关系的表】
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <param name="PrimaryKeyName">表的主键名称</param>
        /// <param name="OrderBy">order by 语句，order by Field1,Field2。默认缺省值：OrderBy = ""</param>
        /// <param name="IsPage"> 是否开启分页。默认缺省值：IsPage = fals</param>
        /// <param name="IntPage">页码。默认缺省值：IntPage = 1</param>
        /// <param name="IntLimit">每页显示数量。默认缺省值：IntLimit = 10</param>
        /// <returns>回Source表、Target表和表的记录数(Source表经过排序但未分页，Taget经过排序且已分页，是否分页还要依赖IsPage参数进行判断)</returns>
        private static SingleDataTableAttr GetSingleDataTableAttr(string TableName, string OrderBy = "", Boolean IsPage = false, int IntPage = 1, int IntLimit = 10, string QueryType = "All", string QueryField = "", string Keyword = "", DataTable CustomDataTable = null)
        {
            DataTable SourceDT = null,
                TargetDT = null;
            int SourceCount = 0,
                TargetCount = 0;

            SourceDT = GetSingleDataTable(TableName, OrderBy, QueryType, QueryField, Keyword, CustomDataTable);
            if (SourceDT != null && SourceDT.Rows.Count > 0)
                SourceCount = SourceDT.Rows.Count;

            //***开启分页时***
            if (SourceDT != null && SourceDT.Rows.Count > 0 && IsPage)
            {
                TargetDT = SourceDT.AsEnumerable().Skip((IntPage - 1) * IntLimit).Take(IntLimit).CopyToDataTable<DataRow>();
                TargetCount = TargetDT.Rows.Count;
            }

            var SingleDataTableAttr = new SingleDataTableAttr
            {
                SourceDT = SourceDT,
                SourceCount = SourceCount,
                TargetDT = TargetDT,
                TargetCount = TargetCount
            };

            return SingleDataTableAttr;

        }


        /// <summary>
        /// 根据传入的表名返回Source表(经过排序但未分页)和Target表(经过排序且已经分页，是否分页依赖IsPage参数进行判断)和表的记录数。【适用于“没有”父子关系的表】 重载方法+1 
        /// </summary>
        /// <param name="TableName">长表名</param>
        /// <param name="PrimaryKeyName">主键名称</param>
        /// <param name="OrderBy">order by语句</param>
        /// <param name="IsPage">开启分页</param>
        /// <param name="IntPage">页码</param>
        /// <param name="IntLimit">每页显示数量</param>
        /// <returns></returns>
        //private static SingleDataTableAttr GetSingleDataTableAttr(string TableName, string PrimaryKeyName, string OrderBy, Boolean IsPage, int IntPage = 1, int IntLimit = 10)
        //{
        //    DataTable SourceDT = null,
        //        TargetDT = null;
        //    int SourceCount = 0,
        //        TargetCount = 0;

        //    //***默认不开启分页时***
        //    string _sql = "select * from " + TableName + " where Invalid=0 and Del=0 " + OrderBy;

        //    SourceDT = MsSQLDbHelper.Query(_sql).Tables[0];
        //    SourceCount = SourceDT.Rows.Count;

        //    //***开启分页时***
        //    if (IsPage)
        //    {
        //        //***Sql例子*** //_sql = "select top " + Limit.toInt() + " * from UserInfo where uid not in (select top ((" + Page.toInt() + "-1)*" + Limit.toInt() + ") UID from UserInfo where Del=0 order by UID) and Del=0 order by UID";

        //        _sql = "select top " + IntLimit + " * from " + TableName + " where Invalid=0 and Del=0 and " + PrimaryKeyName + " not in (select top ((" + IntPage + "-1) * " + IntLimit + ") " + PrimaryKeyName + " from " + TableName + " where Del=0 " + OrderBy + ") " + OrderBy + "";

        //        TargetDT = MsSQLDbHelper.Query(_sql).Tables[0];


        //    }

        //    var SingleDataTableAttr = new SingleDataTableAttr
        //    {
        //        SourceDT = SourceDT,
        //        SourceCount = SourceCount,
        //        TargetDT = TargetDT,
        //        TargetCount = TargetCount
        //    };

        //    return SingleDataTableAttr;

        //}
        #endregion



        #region region获取DataTable
        /// <summary>
        /// 传入表名或短表名，调用GetTableAttr方法获取表的属性，返回所有且经过排序的记录的DataTable【该方法是GetMainDataTable方法和GetSingleDataTable方法的集合，它紧返回DataTable不含其它属性，适用于所有表(“含有(调用GetMainDataTable)”或“不含有(调用GetSingleDataTable)”父子关系的表)】
        /// </summary>
        /// <param name="ShortTableName">可以传入短表名或长表名，通过IsShortTableName值判断</param>
        /// <param name="IsShortTableName">是否为短表名，“是”则查找出长表名，默认值：True</param>
        /// <returns></returns>
        public static DataTable GetDataTable(string ShortTableName, Boolean IsShortTableName = true, DataTable CustomDataTable = null)
        {
            DataTable MicroDT = null, _dt = null;
            string TableName = string.Empty, PrimaryKeyName = string.Empty, SortFieldName = string.Empty, SortFieldNameAscDesc = string.Empty, OrderBy = string.Empty;
            Boolean MainSub = false;

            //获取长表名
            if (IsShortTableName)
                TableName = MicroPublic.GetTableName(ShortTableName);
            else
                TableName = ShortTableName;

            //获取表的字段和相关属性
            var getTableAttr = MicroDataTable.GetTableAttr(TableName);
            MicroDT = getTableAttr.MicroDT; //获取存放在MicroTables的数据，该表是存放每个表的表名和字段
            PrimaryKeyName = getTableAttr.PrimaryKeyName;  //获得主键字段名称
            SortFieldName = getTableAttr.SortFieldName;  //获得排序字段名称
            SortFieldNameAscDesc = getTableAttr.SortFieldNameAscDesc;
            OrderBy = getTableAttr.OrderBy;  //获得order by 命令字符串
            MainSub = getTableAttr.MainSub;

            if (MainSub)
                _dt = GetMainDataTable(MicroDT, TableName, PrimaryKeyName, SortFieldName, SortFieldNameAscDesc, OrderBy, "All", "", "", "MainSub", CustomDataTable);
            else
                _dt = GetSingleDataTable(TableName, OrderBy);

            return _dt;
        }


        /// <summary>
        /// 传入表名，返回所有记录DataTable【紧返回DataTable不含其它属性】
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <param name="OrderBy">order by 语句，order by Field1,Field2。默认缺省值：OrderBy = ""</param>
        /// <returns>返回DataTable， 执行的语句是："select * from " + TableName + " where Invalid=0 and Del=0 " + OrderBy;</returns>
        public static DataTable GetSingleDataTable(string TableName, string OrderBy = "", string QueryType = "All", string QueryField = "", string Keyword = "", DataTable CustomDataTable = null)
        {
            DataTable SourceDT = null;
            string _sql = string.Empty;
            QueryType = string.IsNullOrEmpty(QueryType) == true ? "All" : QueryType;

            string Invalid = " and Invalid=0 ";
            if (MicroUserInfo.CheckUserRole("Administrators"))
                Invalid = "";

            string StateCode = string.Empty;
            if (MsSQLDbHelper.ColumnExists(TableName, "StateCode"))
                StateCode = " and StateCode <> -3";  //不显示草稿，草稿另外在草稿箱显示

            //如果自定义数据表不为空时则SourceDT = CustomDataTable;
            if (CustomDataTable != null)
                SourceDT = CustomDataTable;
            else
            {
                if (QueryType.ToLower() == "all")
                {
                    if (string.IsNullOrEmpty(QueryField) && string.IsNullOrEmpty(Keyword))
                    {
                        _sql = "select * from " + TableName + " where Del=0 " + Invalid + StateCode + OrderBy;
                        SourceDT = MsSQLDbHelper.Query(_sql).Tables[0];
                    }
                    else
                    {
                        _sql = "select * from " + TableName + " where Del=0 " + Invalid + StateCode + " and ( " + QueryField + " ) " + OrderBy;
                        SqlParameter[] _sp = { new SqlParameter("@Keyword", SqlDbType.VarChar) };
                        _sp[0].Value = "%" + Keyword.toJsonTrim() + "%";
                        SourceDT = MsSQLDbHelper.Query(_sql, _sp).Tables[0];
                    }
                }
                else
                {
                    //QueryType=Field:DataType;
                    string Where = string.Empty,
                        Field = string.Empty,
                        DataType = string.Empty;

                    if (QueryType.Split(':').Length > 1)
                    {
                        Field = QueryType.Split(':')[0];
                        DataType = QueryType.Split(':')[1];

                        if (DataType.ToLower() == "int")
                            Where = " and " + Field + "=" + Keyword.toInt();
                        else
                            Where = " and " + Field + "='" + Keyword.toStringTrim() + "'";

                        _sql = "select * from " + TableName + " where Del=0 " + Invalid + StateCode + Where + OrderBy;

                        SourceDT = MsSQLDbHelper.Query(_sql).Tables[0];

                    }
                }
            }
            return SourceDT;
        }


        /// <summary>
        /// 传入表名返回经过父子关排序并且重新构造的数据表(加入列名MainSub，ParentID=0时列MainSub值为Main，ParentID非0时：中间记录MainSub值为├、最后记录MainSub值为└)【紧适用于“含有”父子关系的数据表】
        /// </summary>
        /// <param name="MicroDT">MicroDT</param>
        /// <param name="TableName">表名</param>
        /// <param name="PrimaryKeyName">表的主键名称</param>
        /// <param name="SortFieldName">排序字段，如Field1,Field2</param>
        /// <param name="OrderBy">order by 语句，order by Field1,Field2</param>
        /// <returns>返回已经过父子关系梳理且重新构造好的数据表，加入列名MainSub，ParentID=0时列MainSub值为Main，ParentID非0时：中间记录MainSub值为├、最后记录MainSub值为└</returns> 
        private static DataTable GetMainDataTable(DataTable MicroDT, string TableName, string PrimaryKeyName, string SortFieldName, string SortFieldNameAscDesc, string OrderBy, string QueryType = "All", string QueryField = "", string Keyword = "", string GetMainOrSub = "MainSub", DataTable CustomDataTable = null)
        {
            DataTable SourceDT = null, //用于存放所有记录的原表还没有经过父子关系梳理的，这里紧用于中间临时调用，该方法最终返回的是Taget表
                TargetDT = null; //用于存放所有记录已经过父子关系梳理的表

            string _sql = string.Empty;
            QueryType = string.IsNullOrEmpty(QueryType) == true ? "All" : QueryType;

            string Invalid = " and Invalid=0 ";
            if (MicroUserInfo.CheckUserRole("Administrators"))
                Invalid = "";

            string StateCode = string.Empty;
            if (MsSQLDbHelper.ColumnExists(TableName, "StateCode"))
                StateCode = " and StateCode <> -3";  //不显示草稿，草稿另外要草稿箱显示

            //如果自定义数据表不为空时则SourceDT = CustomDataTable;
            if (CustomDataTable != null)
                SourceDT = CustomDataTable;
            else
            {

                if (QueryType.ToLower() == "all")
                {
                    if (string.IsNullOrEmpty(QueryField) && string.IsNullOrEmpty(Keyword))
                    {
                        _sql = "select * from " + TableName + " where Del=0 " + Invalid + StateCode + OrderBy;
                        SourceDT = MsSQLDbHelper.Query(_sql).Tables[0];
                    }
                    else
                    {
                        _sql = "select * from " + TableName + " where Del=0 " + Invalid + StateCode + " and ( " + QueryField + " ) " + OrderBy;
                        SqlParameter[] _sp = { new SqlParameter("@Keyword", SqlDbType.VarChar) };
                        _sp[0].Value = "%" + Keyword.toJsonTrim() + "%";
                        SourceDT = MsSQLDbHelper.Query(_sql, _sp).Tables[0];
                    }
                }
                else
                {
                    //QueryType=Field:DataType;
                    string Field = QueryType.Split(':')[0];
                    string DataType = QueryType.Split(':')[1];

                    _sql = "select * from " + TableName + " where Del=0 " + Invalid + StateCode + " and " + Field + " = @Keyword " + OrderBy;

                    SqlParameter[] _sp = { new SqlParameter("@Keyword", DataType.ToLower() == "int" ? SqlDbType.Int : SqlDbType.VarChar) };

                    if (DataType.ToLower() == "int")
                        _sp[0].Value = Keyword.toInt();
                    else
                        _sp[0].Value = Keyword.toStringTrim();

                    SourceDT = MsSQLDbHelper.Query(_sql, _sp).Tables[0];
                }
            }

            if (SourceDT != null && SourceDT.Rows.Count > 0)
            {
                TargetDT = SourceDT.Clone();  //克隆源表结构
                TargetDT.Columns.Add("MainSub", typeof(string));  //追加一列MainSub

                DataRow[] _SourceRows = SourceDT.Select("ParentID=0", SortFieldNameAscDesc);

                foreach (DataRow _SourceDr in _SourceRows)
                {
                    DataRow _TargerDr = TargetDT.NewRow();

                    DataRow[] MicroRows = MicroDT.Select("ParentID<>0", "Sort");
                    foreach (DataRow MicroDR in MicroRows)   //得到该表的字段，并且读取出_dt表的行记录赋值给到_dt2表行
                    {
                        _TargerDr["" + MicroDR["TabColName"].toJsonTrim() + ""] = _SourceDr["" + MicroDR["TabColName"].toJsonTrim() + ""];  //这里因为是对象所以不用加.ToString()之类的
                    }

                    if (GetMainOrSub == "MainSub" || GetMainOrSub == "Main")
                    {
                        _TargerDr["MainSub"] = GetMainOrSub == "Main" ? "-" : "Main";  //把Main作为值插入到MainSub列中
                        TargetDT.Rows.Add(_TargerDr);  //把行插入到_dt2表中
                    }

                    if (GetMainOrSub == "MainSub" || GetMainOrSub == "Sub")
                        GetSubDataTable(MicroDT, SourceDT, TargetDT, PrimaryKeyName, _SourceDr[PrimaryKeyName].toJsonTrim(), SortFieldName, SortFieldNameAscDesc, QueryType, QueryField, Keyword, GetMainOrSub, CustomDataTable);
                }
            }

            return TargetDT;
        }


        /// <summary>
        /// 传入表名返回重新构造好的数据表，它的父级方法是GetMainDataTable【紧适用于“含有”父子关系的数据表】
        /// </summary>
        /// <param name="MicroDT">MicroDT</param>
        /// <param name="SourceDT">源数据表临时使用，用于生成Taget表</param>
        /// <param name="TargetDT">重新构造的表</param>
        /// <param name="PrimaryKeyName">主键名称</param>
        /// <param name="PrimaryKeyValue">主键值，用于"ParentID=" + PrimaryKeyValue</param>
        /// <param name="SortFieldName">排序字段，如Field1,Field2</param>
        /// <returns>返回已经过父子关系梳理且重新构造好的数据表，加入列名MainSub，ParentID=0时列MainSub值为Main，ParentID非0时：中间记录MainSub值为├、最后记录MainSub值为└</returns>
        private static DataTable GetSubDataTable(DataTable MicroDT, DataTable SourceDT, DataTable TargetDT, string PrimaryKeyName, string PrimaryKeyValue, string SortFieldName, string SortFieldNameAscDesc, string QueryType = "All", string QueryField = "", string Keyword = "", string GetMainOrSub = "MainSub", DataTable CustomDataTable = null)
        {
            DataRow[] _rows = null;

            QueryType = string.IsNullOrEmpty(QueryType) == true ? "All" : QueryType;
            string Invalid = " and Invalid=0 ";

            if (MicroUserInfo.CheckUserRole("Administrators"))
                Invalid = "";

            //如果自定义数据表不为空时则SourceDT = CustomDataTable;
            if (CustomDataTable != null)
                _rows = SourceDT.Select("Del=0 " + Invalid + " and ParentID=" + PrimaryKeyValue, SortFieldNameAscDesc);
            else
            {
                if (QueryType.ToLower() == "all")
                {
                    if (string.IsNullOrEmpty(QueryField) && string.IsNullOrEmpty(Keyword))
                    {
                        _rows = SourceDT.Select("Del=0 " + Invalid + " and ParentID=" + PrimaryKeyValue, SortFieldNameAscDesc);
                    }
                    else
                    {
                        string _QueryField = QueryField.Replace("@Keyword", "'%" + Keyword + "%'");
                        _QueryField = " and (" + _QueryField + ")";
                        _rows = SourceDT.Select("Del=0 " + Invalid + " and ParentID=" + PrimaryKeyValue + _QueryField, SortFieldNameAscDesc);
                    }
                }
                else
                {
                    //QueryType=Field:DataType;
                    string Where = string.Empty,
                        Field = string.Empty,
                        DataType = string.Empty;

                    if (QueryType.Split(':').Length > 1)
                    {
                        Field = QueryType.Split(':')[0];
                        DataType = QueryType.Split(':')[1];

                        if (DataType.ToLower() == "int")
                            Where = " and " + Field + "=" + Keyword.toInt();
                        else
                            Where = " and " + Field + "='" + Keyword.toStringTrim() + "'";
                    }

                    _rows = SourceDT.Select("Del=0 " + Invalid + " and ParentID=" + PrimaryKeyValue + Where, SortFieldNameAscDesc);

                }
            }

            int count = 0;
            if (_rows.Length > 0)
            {
                foreach (DataRow _SourceDr in _rows)
                {
                    DataRow _TarterDr = TargetDT.NewRow();
                    count++;

                    string MainSub = string.Empty;
                    for (int i = 1; i < _SourceDr["Level"].ToString().toInt(); i++)
                    {
                        MainSub += "&nbsp;&nbsp;&nbsp;&nbsp;";
                    }

                    if (GetMainOrSub != "Sub")
                    {
                        if (count == _rows.Length)
                            MainSub += "└";
                        else
                            MainSub += "├";
                    }
                    else
                        MainSub += "-";


                    DataRow[] MicroRows = MicroDT.Select("ParentID<>0", "Sort");
                    foreach (DataRow MicroDR in MicroRows)
                    {
                        _TarterDr["" + MicroDR["TabColName"].toJsonTrim() + ""] = _SourceDr["" + MicroDR["TabColName"].toJsonTrim() + ""];  //这里最后因为是对象所以不用加.ToString()之类的;
                    }

                    _TarterDr["MainSub"] = MainSub;
                    TargetDT.Rows.Add(_TarterDr);

                    GetSubDataTable(MicroDT, SourceDT, TargetDT, PrimaryKeyName, _SourceDr[PrimaryKeyName].toJsonTrim(), SortFieldName, SortFieldNameAscDesc, QueryType, QueryField, Keyword);
                }
            }

            return TargetDT;
        }
        #endregion




        #region region获取应用于Select控件的DataTable
        /// <summary>
        /// 调用GetDataTable方法，生成应用于Select控件的数据表，有父子关系的数据经过了处理，子项在前面加入了特殊符号
        /// </summary>
        /// <param name="ShortTableName">短表名</param>
        /// <param name="IsShortTableName">判断是短表名或长表名</param>
        /// <returns></returns>
        public static DataTable GetDataTableForSelect(string ShortTableName, Boolean IsShortTableName)
        {
            DataTable _dt = null;
            _dt = GetDataTable(ShortTableName, IsShortTableName);
            return _dt;
        }


        /// <summary>
        /// 生成应用于Select控件的数据表，有父子关系的数据经过了处理，子项在前面加入了特殊符号。 重载方法+1
        /// </summary>
        /// <param name="MicroDT"></param>
        /// <param name="TableName"></param>
        /// <param name="PrimaryKeyName"></param>
        /// <param name="SortFieldName"></param>
        /// <param name="OrderBy"></param>
        /// <param name="MainSub"></param>
        /// <returns></returns>
        public static DataTable GetDataTableForSelect(DataTable MicroDT, string TableName, string PrimaryKeyName, string SortFieldName, string SortFieldNameAscDesc, string OrderBy, Boolean MainSub)
        {
            DataTable _dt = null;

            try
            {

                //如果开启了父子关系的项，则从GetMainSubDataTableList方法获取记录，否则从GetDataTableList获取
                if (MainSub)
                    _dt = GetMainDataTable(MicroDT, TableName, PrimaryKeyName, SortFieldName, SortFieldNameAscDesc, OrderBy);
                else
                    _dt = GetSingleDataTable(TableName, OrderBy);

            }
            catch { }

            return _dt;
        }
        #endregion




        /// <summary>
        /// set;get;获取表的相关属性，主键字段名称、数据类型、字段长度、排序【order by语句】
        /// </summary>
        public class TableAttr
        {
            /// <summary>
            /// set;get;返回MicroDT，含有该表的表名和所有字段
            /// </summary>
            public DataTable MicroDT
            {
                set;
                get;
            }

            /// <summary>
            /// set;get;主键名称
            /// </summary>
            public string PrimaryKeyName
            {
                set;
                get;
            }

            /// <summary>
            /// set;get;主键的控件前缀
            /// </summary>
            public string CtlPrefix
            {
                set;
                get;
            }

            /// <summary>
            /// set;get;主键的数据类型
            /// </summary>
            public string DataType
            {
                set;
                get;
            }

            /// <summary>
            /// set;get;主键的字段长度
            /// </summary>
            public string FieldLength
            {
                set;
                get;
            }

            /// <summary>
            /// set;get;该表的排序字段名称，两个或以上由逗号进行分隔，如： Field1,Field2
            /// </summary>
            public string SortFieldName
            {
                set;
                get;
            }

            /// <summary>
            /// set;get;排序的方式 asc升序，desc降序
            /// </summary>
            public string AscDesc
            {
                set;
                get;
            }

            /// <summary>
            /// set;get;加上AscDesc的排序方式，如以第一个字段的升序，第二个字段的降序，如：Field1 asc,Field2 desc
            /// </summary>
            public string SortFieldNameAscDesc
            {
                set;
                get;
            }

            /// <summary>
            /// set;get;返回order by语句，如order by Field1,Field2
            /// </summary>
            public string OrderBy
            {
                set;
                get;
            }

            /// <summary>
            /// set;get;返回MainSub值，该值用于判断该表是否含有父子关系
            /// </summary>
            public Boolean MainSub
            {
                set;
                get;
            }

            /// <summary>
            /// set;get;是否开启分页
            /// </summary>
            public Boolean IsPage
            {
                set;
                get;
            }

            /// <summary>
            /// set;get;构造查询字段
            /// </summary>
            public string QueryField
            {
                set;
                get;
            }

            /// <summary>
            /// 检查表是否有UID，通常用于与当前登录UID作比较，确认记录是否为当用户的，是否允许修改
            /// </summary>
            public Boolean IsUID
            {
                set;
                get;
            }
        }


        /// <summary>
        /// 传入表名返回表的主键名称及其相关属性如：数据类型、字段长度、排序【order by语句】，其中主键名称优先返回自定义主键名称【ctlPrimaryKey】，没有则返回默认主键名称【PrimaryKey】
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public static TableAttr GetTableAttr(string TableName)
        {
            string primaryKeyName = string.Empty, dataType = string.Empty, fieldLength = string.Empty, ctlPrefix = string.Empty, sortFieldName = string.Empty, ascDesc = string.Empty, sortFieldNameAscDesc = string.Empty, orderBy = string.Empty, queryField = string.Empty;
            Boolean mainSub = false, isPage = false, isUID = false;

            string Invalid = " and Invalid=0 ";
            if (MicroUserInfo.CheckUserRole("Administrators"))
                Invalid = "";

            string _sql = "select * from MicroTables where Del=0 " + Invalid + " and (ParentID in (select TID from MicroTables where TabColName = @TabColName) or TabColName = @TabColName)  order by ParentID,Sort ";
            SqlParameter[] _sp = { new SqlParameter("@TabColName", SqlDbType.VarChar, 100) };
            _sp[0].Value = TableName;

            DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];
            DataRow[] _rows = _dt.Select("ctlPrimaryKey=1");  //以自定义主键优先作为查询条件

            //没有自定义主键【ctlPrimaryKey】，则以默认主键【PrimaryKey】为查询条件
            if (_rows.Count() == 0)
                _rows = _dt.Select("PrimaryKey=1");

            if (_rows.Length > 0)
            {
                primaryKeyName = _rows[0]["TabColName"].toStringTrim();
                dataType = _rows[0]["DataType"].toStringTrim();
                fieldLength = _rows[0]["FieldLength"].toStringTrim();
                ctlPrefix = _rows[0]["ctlPrefix"].toStringTrim();
            }

            //得到排序方式 asc Or desc 、是否父子关系、是否分页
            DataRow[] _rows2 = _dt.Select("TabColName='" + TableName + "'");

            if (_rows2.Length > 0)
            {
                ascDesc = _rows2[0]["AscDesc"].toStringTrim(); //以ParentID=0 作为父级排序
                //ascDesc = string.IsNullOrEmpty(ascDesc) == true ? "asc" : ascDesc;  //空值时默认以asc方式排序
                mainSub = _rows2[0]["tbMainSub"].toStringTrim() == "True" ? true : false;  //是否为父子关系
                isPage = _rows2[0]["tbPage"].toStringTrim() == "True" ? true : false;  //是否开启分页
            }


            //获取自定义需要排序的字段，构造order by语句
            DataRow[] _rows3 = _dt.Select("IntSort is not null and IntSort > 0", "IntSort");
            if (_rows3.Count() > 0)
            {
                foreach (DataRow _dr3 in _rows3)
                {
                    sortFieldName += _dr3["TabColName"].toStringTrim() + ",";

                    //父级排序为空时,采用子排序
                    if (string.IsNullOrEmpty(ascDesc))
                    {
                        string _ascDesc = string.Empty;
                        _ascDesc = " " + _dr3["AscDesc"].toStringTrim();
                        sortFieldNameAscDesc += _dr3["TabColName"].toStringTrim() + _ascDesc + ",";
                    }
                }

                if (!string.IsNullOrEmpty(sortFieldName))
                    sortFieldName = sortFieldName.Substring(0, sortFieldName.Length - 1);  //得到自定义排序的字段

                if (!string.IsNullOrEmpty(sortFieldNameAscDesc))
                    sortFieldNameAscDesc = sortFieldNameAscDesc.Substring(0, sortFieldNameAscDesc.Length - 1);  //得到加入了Asc or Desc自定义排序的字段

                if (!string.IsNullOrEmpty(ascDesc))
                    orderBy = " order by " + sortFieldName + " " + ascDesc; //构造order by语句
                else
                    orderBy = " order by " + sortFieldNameAscDesc; //构造order by语句
            }
            //当没有自定义排序字段时检查是否有Sort字段，有的话构造order by语句
            else
            {
                DataRow[] _rows4 = _dt.Select("TabColName='Sort'");
                if (_rows4.Count() > 0)
                {
                    sortFieldName = "Sort";
                    orderBy = " order by " + sortFieldName + " " + ascDesc;
                }
                //否则没有Sort字段的话把表的主键作为排序字段
                else
                {
                    sortFieldName = primaryKeyName;
                    if (!string.IsNullOrEmpty(sortFieldName))
                        orderBy = " order by " + sortFieldName + " " + ascDesc;
                }
            }

            //以关键字提交查询记录时。构造查询语句。如：UserName like @Keyword or ChineseName like @Keyword
            //以数据库字段为文本类型时的字段
            DataRow[] _rows5 = _dt.Select("DataType='char' or DataType='nchar' or DataType='varchar' or DataType='nvarchar' or DataType='text' or DataType='ntext'");
            if (_rows5.Count() > 0)
            {
                foreach (DataRow _dr5 in _rows5)
                {
                    queryField += _dr5["TabColName"].toStringTrim() + " like @Keyword or ";
                }
                queryField = queryField.Substring(0, queryField.Length - 3);
            }

            //检查是否有UID这个字段
            DataRow[] _rows6 = _dt.Select("TabColName='UID'");
            if (_rows6.Count() > 0)
                isUID = true;

            var TableAttr = new TableAttr
            {
                MicroDT = _dt,
                PrimaryKeyName = primaryKeyName,
                DataType = dataType,
                FieldLength = fieldLength,
                CtlPrefix = ctlPrefix,
                SortFieldName = sortFieldName,
                AscDesc = ascDesc,
                SortFieldNameAscDesc = sortFieldNameAscDesc,
                OrderBy = orderBy,
                MainSub = mainSub,
                IsPage = isPage,
                QueryField = queryField,
                IsUID = isUID
            };

            return TableAttr;
        }


        /// <summary>
        /// 获取加班申请list，主要用在MicroFormlist的平常加班申请和特殊加班申请上（因为要区分平常和特殊所以无法使用默认的MicroFormlist）
        /// </summary>
        /// <param name="ShortTableName"></param>
        /// <param name="ModuleID"></param>
        /// <param name="FormID"></param>
        /// <param name="QueryField"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="Keyword"></param>
        /// <param name="FormFields"></param>
        /// <returns></returns>
        public static DataTable GetOvertimeList(string ShortTableName, string ModuleID, string FormID = "", string QueryField = "", string StartDate = "", string EndDate = "", string Keyword = "", string FormFields = "")
        {
            //获取长表名
            string TableName = MicroPublic.GetTableName(ShortTableName),
                QueryFieldsStr = string.Empty,    //传递过来的关键字可以由 字段1:值1,字段2:值2,Keyword:KeywordValue这样组成，此时检查如果字段是存在的话由组成sql语句：and 字段1=值1 加入sql主语句进行查询
                KeywordQueryStr = string.Empty;
            DataTable _dt = null;

            QueryField = string.IsNullOrEmpty(QueryField) ? "OvertimeDate" : QueryField;

            StartDate = string.IsNullOrEmpty(StartDate) ? DateTime.Now.toDateFormat() : StartDate;
            StartDate = StartDate.todayStartTime();
            EndDate = string.IsNullOrEmpty(EndDate) ? DateTime.Now.toDateFormat() : EndDate;
            EndDate = EndDate.todayEndTime();

            if (FormID.toInt() > 0 && TableName.ToLower() == "hrovertime")
            {
                var getTableAttr = MicroDataTable.GetTableAttr(TableName);
                string OrderBy = getTableAttr.OrderBy;  //获得order by 命令字符串

                string UID = MicroUserInfo.GetUserInfo("UID"),
                    UIDs = MicroUserInfo.GetDeptUIDs();

                if (MicroAuth.CheckPermit(ModuleID, "6")) //查看所有       
                    UIDs = MicroUserInfo.GetDeptUIDs("All");
                else if (MicroAuth.CheckPermit(ModuleID, "19"))  //查看自部门及自部门所有的父部门及所有父部门包含的子部门      
                    UIDs = MicroUserInfo.GetDeptUIDs("ParentSubDepts");
                else if (MicroAuth.CheckPermit(ModuleID, "20"))  //查看科内，科内及科内所有子部门      
                    UIDs = MicroUserInfo.GetDeptUIDs("SectionDepts");
                else if (MicroAuth.CheckPermit(ModuleID, "17"))  //查看自部门含子部门      
                    UIDs = MicroUserInfo.GetDeptUIDs("SubDeptsID");

                //如果关键字不为空时，构造条件参与查询
                if (!string.IsNullOrEmpty(Keyword))
                {
                    KeywordQueryStr = GetKeywordQueryStr(ShortTableName);
                    string[] ArrKeyword = Keyword.Split(',');

                    if (ArrKeyword.Length > 1)
                    {
                        string NewKeyword = string.Empty;  //用于取得Keyword（Keyword:Value）的值

                        for (int i = 0; i < ArrKeyword.Length; i++)
                        {
                            string Fields = ArrKeyword[i].toStringTrim();
                            string[] ArrFields = Fields.Split(':');
                            if (ArrFields.Length > 1)
                            {
                                string Field = ArrFields[0].toStringTrim(),
                                    Value = ArrFields[1].toStringTrim();
                                if (Field.ToLower() != "keyword")
                                {
                                    if (MsSQLDbHelper.ColumnExists(TableName, Field))
                                        QueryFieldsStr += " and " + Field + " = '" + Value + "'";
                                }
                                else
                                    NewKeyword += Value;
                            }
                        }

                        if (!string.IsNullOrEmpty(NewKeyword))
                            Keyword = NewKeyword;
                    }

                    Keyword = "%" + Keyword.toStringTrim() + "%";
                    //Keyword = Keyword.toStringTrim().Replace('*', '%');
                }

                string _sql = "" +
                    //一阶StateCode>=0且加班者OvertimeUID
                    "select * from HROvertime where Invalid=0 and Del=0 and ((ParentID<>0 and " + QueryField + " between @StartDate and @EndDate " + QueryFieldsStr + ") or ParentID=0) and StateCode>=@StateCode and FormID = @FormID " +
                    "and (" +
                    "(OvertimeUID in(" + UIDs + "))" +
                    //二阶或者，根据加班者OvertimeUID得到ParentID，再根据OvertimeID=ParentID，即得到父记录
                    "or (OvertimeID in(select ParentID from HROvertime where Invalid=0 and Del=0 and ((ParentID<>0 and " + QueryField + " between @StartDate and @EndDate " + QueryFieldsStr + ") or ParentID=0) and StateCode>=@StateCode and FormID = @FormID and OvertimeUID in(" + UIDs + "))) " +
                    //三阶或者申请者UID，自己申请的
                    "or (UID=@UID and Invalid=0 and Del=0 and " + QueryField + " between @StartDate and @EndDate and StateCode>=@StateCode and FormID = @FormID " + QueryFieldsStr + ")" +
                    ") " + KeywordQueryStr +
                    "" + OrderBy + "";  

                SqlParameter[] _sp = {
                                    new SqlParameter("@StateCode",SqlDbType.Int),
                                    new SqlParameter("@FormID",SqlDbType.Int),
                                    new SqlParameter("@UID",SqlDbType.Int),
                                    new SqlParameter("@StartDate",SqlDbType.VarChar),
                                    new SqlParameter("@EndDate",SqlDbType.VarChar),
                                    new SqlParameter("@Keyword",SqlDbType.VarChar),
                                    };

                _sp[0].Value = 0;
                _sp[1].Value = FormID.toInt();
                _sp[2].Value = UID.toInt();
                _sp[3].Value = StartDate;
                _sp[4].Value = EndDate;
                _sp[5].Value = Keyword.toStringTrim();

                _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];
            }

            return _dt;
        }

        /// <summary>
        /// 获取个人草稿数据表，传入短表名获取草稿记录（即未提交临时保存的记录，基础条件为UID=自己的UID）
        /// </summary>
        /// <param name="ShortTableName"></param>
        /// <param name="ModuleID"></param>
        /// <param name="FormID"></param>
        /// <returns></returns>
        public static DataTable GetPersonalDraft(string ShortTableName, string ModuleID, string FormID = "")
        {
            //获取长表名
            string TableName = MicroPublic.GetTableName(ShortTableName);
            DataTable _dt = null;

            if (FormID.toInt() > 0 && TableName.ToLower() == "hrovertime")
            {
                var getTableAttr = MicroDataTable.GetTableAttr(TableName);
                string OrderBy = getTableAttr.OrderBy;  //获得order by 命令字符串

                string _sql = "select * from " + TableName + " where Invalid=0 and Del=0 and UID=@UID and StateCode>=@StateCode and StateCode<=@StateCode2 and OvertimeTypeID=(select OvertimeTypeID from HROvertimeType where Invalid=0 and Del=0 and FormID=@FormID) " + OrderBy + "";
                SqlParameter[] _sp = {
                                    new SqlParameter("@UID",SqlDbType.Int),
                                    new SqlParameter("@StateCode",SqlDbType.Int),
                                    new SqlParameter("@StateCode2",SqlDbType.Int),
                                    new SqlParameter("@FormID",SqlDbType.Int)
                                    };

                _sp[0].Value = MicroUserInfo.GetUserInfo("UID").toInt();
                _sp[1].Value = -4;
                _sp[2].Value = -1;
                _sp[3].Value = FormID.toInt();

                _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];
            }
            else
            {
                string _sql = "select * from " + TableName + " where Invalid=0 and Del=0 and UID=@UID and StateCode=@StateCode";
                SqlParameter[] _sp = {
                                    new SqlParameter("@UID",SqlDbType.Int),
                                    new SqlParameter("@StateCode",SqlDbType.Int)
                                    };

                _sp[0].Value = MicroUserInfo.GetUserInfo("UID").toInt();
                _sp[1].Value = -3;

                _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];
            }

            return _dt;
        }

        /// <summary>
        /// 获取休假申请list，主要用在MicroFormlist（因为要区分只能查看自己所以无法使用默认的MicroFormlist）
        /// </summary>
        /// <param name="ShortTableName"></param>
        /// <param name="ModuleID"></param>
        /// <param name="FormID"></param>
        /// <param name="QueryField"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="Keyword"></param>
        /// <param name="FormFields"></param>
        /// <returns></returns>
        public static DataTable GetLeaveList(string ShortTableName, string ModuleID, string FormID = "", string QueryField = "", string StartDate = "", string EndDate = "", string Keyword = "", string FormFields = "")
        {
            //获取长表名
            string TableName = MicroPublic.GetTableName(ShortTableName),
                KeywordQueryStr = string.Empty;

            DataTable _dt = null;

            if (TableName.ToLower() == "hrleave")
            {
                QueryField = string.IsNullOrEmpty(QueryField) ? "StartDateTime" : QueryField;

                if (!string.IsNullOrEmpty(Keyword))
                {
                    KeywordQueryStr = GetKeywordQueryStr(ShortTableName);
                    Keyword = "%" + Keyword.toStringTrim() + "%";
                    //Keyword = Keyword.toStringTrim().Replace('*', '%');
                }

                StartDate = string.IsNullOrEmpty(StartDate) ? DateTime.Now.toDateFormat() : StartDate;
                StartDate = StartDate.todayStartTime();
                EndDate = string.IsNullOrEmpty(EndDate) ? DateTime.Now.toDateFormat() : EndDate;
                EndDate = EndDate.todayEndTime();

                var getTableAttr = GetTableAttr(TableName);
                string OrderBy = getTableAttr.OrderBy;  //获得order by 命令字符串

                string UID = MicroUserInfo.GetUserInfo("UID"),
                    UIDs = MicroUserInfo.GetDeptUIDs();

                if (MicroAuth.CheckPermit(ModuleID, "6")) //查看所有       
                    UIDs = MicroUserInfo.GetDeptUIDs("All");
                else if (MicroAuth.CheckPermit(ModuleID, "19"))  //查看自部门及自部门所有的父部门及所有父部门包含的子部门      
                    UIDs = MicroUserInfo.GetDeptUIDs("ParentSubDepts");
                else if (MicroAuth.CheckPermit(ModuleID, "20"))  //查看科内，科内及科内所有子部门      
                    UIDs = MicroUserInfo.GetDeptUIDs("SectionDepts");
                else if (MicroAuth.CheckPermit(ModuleID, "17"))  //查看自部门含子部门      
                    UIDs = MicroUserInfo.GetDeptUIDs("SubDeptsID");

                string _sql = "" +
                    //一阶
                    "select * from " + TableName + " where Invalid=0 and Del=0 and FormID=@FormID and (LeaveUID in(" + UIDs + ") " +
                    //二阶或者 自己申请的 申请者UID
                    " or UID=@UID) " +
                    " and " + QueryField + " between @StartDate and @EndDate " + KeywordQueryStr +
                    " " + OrderBy + "";   //

                SqlParameter[] _sp = {
                                    new SqlParameter("@FormID", SqlDbType.Int),
                                    new SqlParameter("@UID", SqlDbType.Int),
                                    new SqlParameter("@StartDate",SqlDbType.VarChar),
                                    new SqlParameter("@EndDate",SqlDbType.VarChar),
                                    new SqlParameter("@Keyword",SqlDbType.VarChar),
                                };

                _sp[0].Value = FormID.toInt();
                _sp[1].Value = UID.toInt();
                _sp[2].Value = StartDate;
                _sp[3].Value = EndDate;
                _sp[4].Value = Keyword.toStringTrim();

                _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

            }

            return _dt;
        }


        /// <summary>
        /// 获取排班申请list，主要用在MicroFormlist
        /// </summary>
        /// <param name="ShortTableName"></param>
        /// <param name="ModuleID"></param>
        /// <param name="FormID"></param>
        /// <param name="QueryField"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="Keyword"></param>
        /// <param name="FormFields"></param>
        /// <returns></returns>
        public static DataTable GetOnDutyFormList(string ShortTableName, string ModuleID, string FormID = "", string QueryField = "", string StartDate = "", string EndDate = "", string Keyword = "", string FormFields = "")
        {
            //获取长表名
            string TableName = MicroPublic.GetTableName(ShortTableName),
                QueryFieldsStr = string.Empty,    //传递过来的关键字可以由 字段1:值1,字段2:值2,Keyword:KeywordValue这样组成，此时检查如果字段是存在的话由组成sql语句：and 字段1=值1 加入sql主语句进行查询
                KeywordQueryStr = string.Empty;
            DataTable _dt = null;

            QueryField = string.IsNullOrEmpty(QueryField) ? "DateCreated" : QueryField;

            StartDate = string.IsNullOrEmpty(StartDate) ? DateTime.Now.toDateFormat() : StartDate;
            StartDate = StartDate.todayStartTime();
            EndDate = string.IsNullOrEmpty(EndDate) ? DateTime.Now.toDateFormat() : EndDate;
            EndDate = EndDate.todayEndTime();

            if (FormID.toInt() > 0 && TableName.ToLower() == "hrondutyform")
            {
                var getTableAttr = MicroDataTable.GetTableAttr(TableName);
                string OrderBy = getTableAttr.OrderBy;  //获得order by 命令字符串

                string UID = MicroUserInfo.GetUserInfo("UID"),
                    UIDs = MicroUserInfo.GetDeptUIDs();

                if (MicroAuth.CheckPermit(ModuleID, "6")) //查看所有       
                    UIDs = MicroUserInfo.GetDeptUIDs("All");
                else if (MicroAuth.CheckPermit(ModuleID, "19"))  //查看自部门及自部门所有的父部门及所有父部门包含的子部门      
                    UIDs = MicroUserInfo.GetDeptUIDs("ParentSubDepts");
                else if (MicroAuth.CheckPermit(ModuleID, "20"))  //查看科内，科内及科内所有子部门      
                    UIDs = MicroUserInfo.GetDeptUIDs("SectionDepts");
                else if (MicroAuth.CheckPermit(ModuleID, "17"))  //查看自部门含子部门      
                    UIDs = MicroUserInfo.GetDeptUIDs("SubDeptsID");

                //如果关键字不为空时，构造条件参与查询
                if (!string.IsNullOrEmpty(Keyword))
                {
                    KeywordQueryStr = GetKeywordQueryStr(ShortTableName);
                    string[] ArrKeyword = Keyword.Split(',');

                    if (ArrKeyword.Length > 1)
                    {
                        string NewKeyword = string.Empty;  //用于取得Keyword（Keyword:Value）的值

                        for (int i = 0; i < ArrKeyword.Length; i++)
                        {
                            string Fields = ArrKeyword[i].toStringTrim();
                            string[] ArrFields = Fields.Split(':');
                            if (ArrFields.Length > 1)
                            {
                                string Field = ArrFields[0].toStringTrim(),
                                    Value = ArrFields[1].toStringTrim();
                                if (Field.ToLower() != "keyword")
                                {
                                    if (MsSQLDbHelper.ColumnExists(TableName, Field))
                                        QueryFieldsStr += " and " + Field + " = '" + Value + "'";
                                }
                                else
                                    NewKeyword += Value;
                            }
                        }

                        if (!string.IsNullOrEmpty(NewKeyword))
                            Keyword = NewKeyword;
                    }

                    Keyword = "%" + Keyword.toStringTrim() + "%";
                    //Keyword = Keyword.toStringTrim().Replace('*', '%');
                }


                string _sql = "" +
                    //一阶StateCode>=0且加班者OvertimeUID
                    "select * from HROnDutyForm where Invalid=0 and Del=0 and ((ParentID<>0 and " + QueryField + " between @StartDate and @EndDate " + QueryFieldsStr + ") or ParentID=0) and StateCode>=@StateCode and FormID = @FormID " +
                    "and (" +
                    "(DutyUID in(" + UIDs + "))" +
                    //二阶或者，根据加班者OvertimeUID得到ParentID，再根据OvertimeID=ParentID，即得到父记录
                    "or (DutyID in(select ParentID from HROnDutyForm where Invalid=0 and Del=0 and ((ParentID<>0 and " + QueryField + " between @StartDate and @EndDate " + QueryFieldsStr + ") or ParentID=0) and StateCode>=@StateCode and FormID = @FormID and DutyUID in(" + UIDs + "))) " +
                    //三阶或者申请者UID
                    "or (UID=@UID and Invalid=0 and Del=0 and " + QueryField + " between @StartDate and @EndDate and StateCode>=@StateCode and FormID = @FormID " + QueryFieldsStr + ")" +
                    ") " + KeywordQueryStr +
                    "" + OrderBy + "";  //自己申请的

                SqlParameter[] _sp = {
                                    new SqlParameter("@StateCode",SqlDbType.Int),
                                    new SqlParameter("@FormID",SqlDbType.Int),
                                    new SqlParameter("@UID",SqlDbType.Int),
                                    new SqlParameter("@StartDate",SqlDbType.VarChar),
                                    new SqlParameter("@EndDate",SqlDbType.VarChar),
                                    new SqlParameter("@Keyword",SqlDbType.VarChar),
                                    };

                _sp[0].Value = -4;
                _sp[1].Value = FormID.toInt();
                _sp[2].Value = UID.toInt();
                _sp[3].Value = StartDate;
                _sp[4].Value = EndDate;
                _sp[5].Value = Keyword.toStringTrim();

                _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

            }

            return _dt;
        }


        /// <summary>
        /// 获取排班申请list，主要用在就餐统计上
        /// </summary>
        /// <param name="ShortTableName"></param>
        /// <param name="ModuleID"></param>
        /// <param name="FormID"></param>
        /// <param name="QueryField"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="Keyword"></param>
        /// <param name="FormFields"></param>
        /// <returns></returns>
        public static DataTable GetOnDutyFormListByMeal(string ShortTableName, string ModuleID, string FormID = "", string QueryField = "", string StartDate = "", string EndDate = "", string Keyword = "", string FormFields = "")
        {
            //获取长表名
            string TableName = MicroPublic.GetTableName(ShortTableName),
                QueryFieldsStr = string.Empty,    //传递过来的关键字可以由 字段1:值1,字段2:值2,Keyword:KeywordValue这样组成，此时检查如果字段是存在的话由组成sql语句：and 字段1=值1 加入sql主语句进行查询
                KeywordQueryStr = string.Empty;
            DataTable _dt = null;

            QueryField = string.IsNullOrEmpty(QueryField) ? "DateCreated" : QueryField;

            StartDate = string.IsNullOrEmpty(StartDate) ? DateTime.Now.toDateFormat() : StartDate;
            StartDate = StartDate.todayStartTime();
            EndDate = string.IsNullOrEmpty(EndDate) ? DateTime.Now.toDateFormat() : EndDate;
            EndDate = EndDate.todayEndTime();

            if (FormID.toInt() > 0 && TableName.ToLower() == "hrondutyform")
            {
                var getTableAttr = MicroDataTable.GetTableAttr(TableName);
                string OrderBy = getTableAttr.OrderBy;  //获得order by 命令字符串

                string UID = MicroUserInfo.GetUserInfo("UID"),
                    UIDs = MicroUserInfo.GetDeptUIDs();

                if (MicroAuth.CheckPermit(ModuleID, "6")) //查看所有       
                    UIDs = MicroUserInfo.GetDeptUIDs("All");
                else if (MicroAuth.CheckPermit(ModuleID, "19"))  //查看自部门及自部门所有的父部门及所有父部门包含的子部门      
                    UIDs = MicroUserInfo.GetDeptUIDs("ParentSubDepts");
                else if (MicroAuth.CheckPermit(ModuleID, "20"))  //查看科内，科内及科内所有子部门      
                    UIDs = MicroUserInfo.GetDeptUIDs("SectionDepts");
                else if (MicroAuth.CheckPermit(ModuleID, "17"))  //查看自部门含子部门      
                    UIDs = MicroUserInfo.GetDeptUIDs("SubDeptsID");

                //如果关键字不为空时，构造条件参与查询
                if (!string.IsNullOrEmpty(Keyword))
                {
                    KeywordQueryStr = GetKeywordQueryStr(ShortTableName);
                    string[] ArrKeyword = Keyword.Split(',');

                    if (ArrKeyword.Length > 1)
                    {
                        string NewKeyword = string.Empty;  //用于取得Keyword（Keyword:Value）的值

                        for (int i = 0; i < ArrKeyword.Length; i++)
                        {
                            string Fields = ArrKeyword[i].toStringTrim();
                            string[] ArrFields = Fields.Split(':');
                            if (ArrFields.Length > 1)
                            {
                                string Field = ArrFields[0].toStringTrim(),
                                    Value = ArrFields[1].toStringTrim();
                                if (Field.ToLower() != "keyword")
                                {
                                    if (MsSQLDbHelper.ColumnExists(TableName, Field))
                                        QueryFieldsStr += " and " + Field + " = '" + Value + "'";
                                }
                                else
                                    NewKeyword += Value;
                            }
                        }

                        if (!string.IsNullOrEmpty(NewKeyword))
                            Keyword = NewKeyword;
                    }

                    Keyword = "%" + Keyword.toStringTrim() + "%";
                    //Keyword = Keyword.toStringTrim().Replace('*', '%');
                }

                //###Backup by 20211124  大概需要16秒的时间，改为下面6秒左右###
                //string _sql = "" +
                //    //一阶StateCode>=0且加班者OvertimeUID
                //    "select DutyID, Sort, ParentID, Level, LevelCode, FormID, FormNumber, FormState, StateCode, IP, Applicant, Phone, Tel, UID, DisplayName, DeptID, SaveDraft, DateCreated, DateModified, Invalid, Del, DutyUID, DutyUserName, DutyDisplayName, Location, DutyDate, ShiftName, ShiftTypeID, FilePath, ('【'+FORMAT(DutyDate, 'yyyy-MM-dd') +'】'+ShiftName+'：'+DutyDisplayName) as DutyContent, StartDateTime, EndDateTime" +
                //    " from HROnDutyForm where Invalid=0 and Del=0 " +

                //    //同一个人同一天存在多条申请记录时，只取时间最大的那一条
                //    " and DutyID in(select DutyID from HROnDutyForm a where a.DateCreated in (select max(b.DateCreated) from HROnDutyForm b where b.DutyUID=a.DutyUID and a.DutyDate=b.DutyDate and b." + QueryField + " between @StartDate and @EndDate)" +
                //    " or a.DutyID in ( select ParentID from HROnDutyForm a where a.DateCreated in (select max(b.DateCreated) from HROnDutyForm b where b.DutyUID=a.DutyUID and a.DutyDate=b.DutyDate and b." + QueryField + " between @StartDate and @EndDate) ) )" +

                //    "and ((ParentID<>0 and " + QueryField + " between @StartDate and @EndDate " + QueryFieldsStr + ") or ParentID=0) and StateCode>=@StateCode and FormID = @FormID " +
                //    "and (" +
                //    "(DutyUID in(" + UIDs + "))" +
                //    //二阶或者，根据加班者OvertimeUID得到ParentID，再根据OvertimeID=ParentID，即得到父记录
                //    "or (DutyID in(select ParentID from HROnDutyForm where Invalid=0 and Del=0 and ((ParentID<>0 and " + QueryField + " between @StartDate and @EndDate " + QueryFieldsStr + ") or ParentID=0) and StateCode>=@StateCode and FormID = @FormID and DutyUID in(" + UIDs + "))) " +
                //    //三阶或者申请者UID
                //    "or (UID=@UID and Invalid=0 and Del=0 and " + QueryField + " between @StartDate and @EndDate and StateCode>=@StateCode and FormID = @FormID " + QueryFieldsStr + ")" +
                //    ") " + KeywordQueryStr +
                //    "" + OrderBy + "";  //自己申请的


            string _sql = "" +
                    //一阶StateCode>=0且加班者OvertimeUID
                    "select DutyID, Sort, ParentID, Level, LevelCode, FormID, FormNumber, FormState, StateCode, IP, Applicant, Phone, Tel, UID, DisplayName, DeptID, SaveDraft, DateCreated, DateModified, Invalid, Del, DutyUID, DutyUserName, DutyDisplayName, Location, DutyDate, ShiftName, ShiftTypeID, FilePath, ('【'+FORMAT(DutyDate, 'yyyy-MM-dd') +'】'+ShiftName+'：'+DutyDisplayName) as DutyContent, StartDateTime, EndDateTime" +
                    " from HROnDutyForm where Invalid=0 and Del=0 " +

                    //同一个人同一天存在多条申请记录时，只取时间最大的那一条记录
                    " and (" +
                    " DutyID in(" +
                    "select DutyID from HROnDutyForm a where a.DateCreated in (select max(b.DateCreated) from HROnDutyForm b where b.DutyUID=a.DutyUID and a.DutyDate=b.DutyDate and b." + QueryField + " between @StartDate and @EndDate) ) " +
                    " or DutyID in (" +
                    "select ParentID from HROnDutyForm a where a.DateCreated in (select max(b.DateCreated) from HROnDutyForm b where b.DutyUID=a.DutyUID and a.DutyDate=b.DutyDate and b." + QueryField + " between @StartDate and @EndDate) )" + 
                    ")" + //and (

                    "and ((ParentID<>0 and " + QueryField + " between @StartDate and @EndDate " + QueryFieldsStr + ") or ParentID=0) and StateCode>=@StateCode and FormID = @FormID " +
                    "and (" +
                    "DutyUID in(" + UIDs + ")" +
                    //二阶或者，根据加班者OvertimeUID得到ParentID，再根据OvertimeID=ParentID，即得到父记录
                    "or DutyID in(select ParentID from HROnDutyForm where Invalid=0 and Del=0 and ((ParentID<>0 and " + QueryField + " between @StartDate and @EndDate " + QueryFieldsStr + ") or ParentID=0) and StateCode>=@StateCode and FormID = @FormID and DutyUID in(" + UIDs + ")) " +
                    //三阶或者申请者UID，自己申请的
                    "or (UID=@UID and Invalid=0 and Del=0 and " + QueryField + " between @StartDate and @EndDate and StateCode>=@StateCode and FormID = @FormID " + QueryFieldsStr + ")" +
                    ") " +
                    "" + KeywordQueryStr +
                    "" + OrderBy + ""; 


                SqlParameter[] _sp = {
                                    new SqlParameter("@StateCode",SqlDbType.Int),
                                    new SqlParameter("@FormID",SqlDbType.Int),
                                    new SqlParameter("@UID",SqlDbType.Int),
                                    new SqlParameter("@StartDate",SqlDbType.VarChar),
                                    new SqlParameter("@EndDate",SqlDbType.VarChar),
                                    new SqlParameter("@Keyword",SqlDbType.VarChar),
                                    };

                _sp[0].Value = -4;
                _sp[1].Value = FormID.toInt();
                _sp[2].Value = UID.toInt();
                _sp[3].Value = StartDate;
                _sp[4].Value = EndDate;
                _sp[5].Value = Keyword.toStringTrim();

                _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

            }

            return _dt;
        }


        /// <summary>
        /// 默认获取表单记录
        /// </summary>
        /// <param name="ShortTableName"></param>
        /// <param name="ModuleID"></param>
        /// <param name="FormID"></param>
        /// <param name="QueryField"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="Keyword"></param>
        /// <param name="FormFields"></param>
        /// <returns></returns>
        public static DataTable GetGeneralFormList(string ShortTableName, string ModuleID, string FormID = "", string QueryField = "", string StartDate = "", string EndDate = "", string Keyword = "", string FormFields = "")
        {
            //获取长表名
            string TableName = MicroPublic.GetTableName(ShortTableName),
                KeywordQueryStr = string.Empty;

            DataTable _dt = null;

            QueryField = string.IsNullOrEmpty(QueryField) ? "DateCreated" : QueryField;

            if (!string.IsNullOrEmpty(Keyword))
            {
                KeywordQueryStr = GetKeywordQueryStr(ShortTableName);
                Keyword = "%" + Keyword.toStringTrim() + "%";
                //Keyword = Keyword.toStringTrim().Replace('*', '%');
            }

            StartDate = string.IsNullOrEmpty(StartDate) ? DateTime.Now.toDateFormat() : StartDate;
            StartDate = StartDate.todayStartTime();
            EndDate = string.IsNullOrEmpty(EndDate) ? DateTime.Now.toDateFormat() : EndDate;
            EndDate = EndDate.todayEndTime();

            var getTableAttr = GetTableAttr(TableName);
            string OrderBy = getTableAttr.OrderBy;  //获得order by 命令字符串


            string _sql = "" +
                "select * from " + TableName + " where Invalid=0 and Del=0 " +
                " and " + QueryField + " between @StartDate and @EndDate " + KeywordQueryStr +
                " " + OrderBy + "";   //

            SqlParameter[] _sp = {  new SqlParameter("@StartDate",SqlDbType.VarChar),
                                    new SqlParameter("@EndDate",SqlDbType.VarChar),
                                    new SqlParameter("@Keyword",SqlDbType.VarChar),
                                };

            _sp[0].Value = StartDate;
            _sp[1].Value = EndDate;
            _sp[2].Value = Keyword.toStringTrim();

            _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

            return _dt;
        }


        /// <summary>
        /// 获取待我审批的记录
        /// </summary>
        /// <param name="ShortTableName"></param>
        /// <param name="ModuleID"></param>
        /// <param name="FormID"></param>
        /// <param name="QueryField"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="Keyword"></param>
        /// <param name="FormFields"></param>
        /// <returns></returns>
        public static DataTable GetPendingMyApprovalList(string ShortTableName, string ModuleID, string FormID = "", string QueryField = "", string StartDate = "", string EndDate = "", string Keyword = "", string FormFields = "")
        {
            //获取长表名
            string TableName = MicroPublic.GetTableName(ShortTableName),
                KeywordQueryStr = string.Empty,
                UID = MicroUserInfo.GetUserInfo("UID");

            DataTable _dt = null;

            QueryField = string.IsNullOrEmpty(QueryField) ? "DateCreated" : QueryField;

            if (!string.IsNullOrEmpty(Keyword))
            {
                KeywordQueryStr = GetKeywordQueryStr(ShortTableName);
                Keyword = "%" + Keyword.toStringTrim() + "%";
                //Keyword = Keyword.toStringTrim().Replace('*', '%');
            }

            StartDate = string.IsNullOrEmpty(StartDate) ? DateTime.Now.toDateFormat() : StartDate;
            StartDate = StartDate.todayStartTime();
            EndDate = string.IsNullOrEmpty(EndDate) ? DateTime.Now.toDateFormat() : EndDate;
            EndDate = EndDate.todayEndTime();

            var getTableAttr = GetTableAttr(TableName);
            string OrderBy = getTableAttr.OrderBy,  //获得order by 命令字符串
                 PrimaryKeyName = getTableAttr.PrimaryKeyName; //获得主键字段名称
            Boolean MainSub = getTableAttr.MainSub;

            string _sqlMainSub = string.Empty;

            //如果开启了父子关系的表，需要把父记录也显示出来
            //***因为采用了FormNumber作为查询，所以不需要ID & ParentID这种查询方式， 注：20211118***
            //if (MainSub)
            //    _sqlMainSub = " or ParentID in( select " + PrimaryKeyName + " from " + TableName + " where  " + PrimaryKeyName + " in (" +
            //            //一阶 关联相关表
            //            "select a.FormsID as FormsID from FormApprovalRecords a left join Forms b on a.FormID=b.FormID left join UserInfo c on a.UID=c.UID " +
            //            //二阶 得到在审批阶段最小的需要审批的记录
            //            "where FARID in (select min(FARID) from FormApprovalRecords where WorkFlowID in (select WFID from WorkFlow where Invalid=0 and Del=0 and FlowCode='Approval') and Invalid=0 and Del=0 and StateCode=0 group by FormID,FormsID) " +
            //            //三阶 去除在任意审批阶段被驳回的记录 （注：申请、受理、结案不算审批阶段）
            //            "and FormNumber not in(select FormNumber from FormApprovalRecords where FARID in (select max(FARID) from FormApprovalRecords where StateCode < 0 and Invalid = 0 and Del = 0 group by FormID,FormsID)) " +
            //            //四阶 与我相关的
            //            "and CHARINDEX(',' + convert(varchar, " + UID + ") + ',',',' + CanApprovalUID + ',')> 0 and a.Invalid = 0 and a.Del = 0 and a.FormID=" + FormID.toInt() + ")) ";


            //string _sql2 = " and (" + PrimaryKeyName + " in (" + 
            //       //一阶 关联相关表
            //      "select a.FormsID as FormsID from FormApprovalRecords a left join Forms b on a.FormID=b.FormID left join UserInfo c on a.UID=c.UID " +
            //      //二阶 得到在审批阶段最小的需要审批的记录
            //      "where FARID in (select min(FARID) from FormApprovalRecords where WorkFlowID in (select WFID from WorkFlow where Invalid=0 and Del=0 and FlowCode='Approval') and Invalid=0 and Del=0 and StateCode=0 group by FormID,FormsID) " +
            //      //三阶 去除在任意审批阶段被驳回的记录 （注：申请、受理、结案不算审批阶段）
            //      "and FormNumber not in(select FormNumber from FormApprovalRecords where FARID in (select max(FARID) from FormApprovalRecords where StateCode < 0 and Invalid = 0 and Del = 0 group by FormID,FormsID)) " +
            //      //四阶 与我相关的
            //      "and CHARINDEX(',' + convert(varchar, " + UID + ") + ',',',' + CanApprovalUID + ',')> 0 and a.Invalid = 0 and a.Del = 0 and a.FormID=" + FormID.toInt() + ") " + _sqlMainSub + ") ";

            string _sql2 = " and FormNumber in (" +
                  //一阶 关联相关表
                  "select a.FormNumber as FormNumber from FormApprovalRecords a left join Forms b on a.FormID=b.FormID left join UserInfo c on a.UID=c.UID " +
                  //二阶 得到在审批阶段最小的需要审批的记录
                  "where FARID in (select min(FARID) from FormApprovalRecords where WorkFlowID in (select WFID from WorkFlow where Invalid=0 and Del=0 and FlowCode='Approval') and Invalid=0 and Del=0 and StateCode=0 group by FormID,FormsID) " +
                  //三阶 去除在任意审批阶段被驳回的记录 （注：申请、受理、结案不算审批阶段）
                  "and FormNumber not in(select FormNumber from FormApprovalRecords where FARID in (select max(FARID) from FormApprovalRecords where StateCode < 0 and Invalid = 0 and Del = 0 group by FormID,FormsID)) " +
                  //四阶 与我相关的
                  "and CHARINDEX(',' + convert(varchar, " + UID + ") + ',',',' + CanApprovalUID + ',')> 0 and a.Invalid = 0 and a.Del = 0 and a.FormID=" + FormID.toInt() + ")";

            string _sql = "" +
                " select * from " + TableName + " where Invalid=0 and Del=0  and StateCode>=0 and StateCode<100 " +
                 _sql2 +
                " and " + QueryField + " between @StartDate and @EndDate " + KeywordQueryStr +
                " " + OrderBy + "";  //

            //HttpContext.Current.Response.Write(_sql);
            //HttpContext.Current.Response.End();

            SqlParameter[] _sp = {  new SqlParameter("@StartDate",SqlDbType.VarChar),
                                    new SqlParameter("@EndDate",SqlDbType.VarChar),
                                    new SqlParameter("@Keyword",SqlDbType.VarChar),
                                };

            _sp[0].Value = StartDate;
            _sp[1].Value = EndDate;
            _sp[2].Value = Keyword.toStringTrim();

            _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

            return _dt;
        }

        /// <summary>
        /// 获取个人加班数据(主要用于首页明细)，返回DataTable
        /// </summary>
        /// <param name="ShortTableName"></param>
        /// <param name="ModuleID"></param>
        /// <param name="FormID"></param>
        /// <param name="DataType"></param>
        /// <param name="UID"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="Keyword"></param>
        /// <param name="DataStateCode">统计数据的类型根据StateCode的状态：Apply填写申请或草稿（and StateCode =-3 or StateCode =-2）、ApplyOrMore填写申请、草稿或以上（and StateCode >=-3 and StateCode <>-1）、Return驳回申请（and StateCode =-1）、Approval等待审批(and StateCode=0)、ApprovalOrMore等待审批或以上(and StateCode>=0)、Finish已审批通过的( and StateCode=100)、All所有（包含临时、驳回、删除、填写申请、等待审批的 StateCode=string.Empty;）</param>
        /// <returns></returns>
        public static DataTable GetPersonalAttendance(string ShortTableName, string ModuleID, string FormID, string DataType, int UID, string StartDate, string EndDate, string Keyword, string DataStateCode = "Finish")
        {
            //获取长表名
            //string TableName = MicroPublic.GetTableName(ShortTableName);  //指定了长表名，所以不需要
            string StartDateTime = StartDate.toDateFormat() + " 00:00:00.000",
             EndDateTime = EndDate.toDateFormat() + " 23:59:59.998";
            string _sql = string.Empty,
                KeywordQueryStr = string.Empty,
            WhereStateCode = " and StateCode=100";

            //根据数据状态代码进行统计
            if (DataStateCode == "Apply")
                WhereStateCode = " and (StateCode =-3 or StateCode =-2)"; //申请或草稿
            else if (DataStateCode == "ApplyOrMore")
                WhereStateCode = " and StateCode >=-3 and StateCode <>-1";  //申请或以上，但不等于驳回
            else if (DataStateCode == "Return")
                WhereStateCode = " and StateCode=-1";  //驳回
            else if (DataStateCode == "Approval")
                WhereStateCode = " and StateCode=0";  //等待审批
            else if (DataStateCode == "ApprovalOrMore")
                WhereStateCode = " and StateCode>=0"; //等待审批或以上
            else if (DataStateCode == "Finish")
                WhereStateCode = " and StateCode=100";  //已审批通过
            else if (DataStateCode == "All")
                WhereStateCode = string.Empty;  //所有


            //如果关键字不为空时，构造条件参与查询
            if (!string.IsNullOrEmpty(Keyword))
            {
                KeywordQueryStr = GetKeywordQueryStr(ShortTableName);
                Keyword = "%" + Keyword.toStringTrim() + "%";
                //Keyword = Keyword.toStringTrim().Replace('*', '%');
            }


            DataType = DataType.ToLower();
            switch (DataType)
            {
                //个人加班数据
                //个人月度延时加班，or OvertimeID in 是通过他们的子记录得到父记录
                case "getworkovertime":
                    _sql = "select * from HROvertime where Invalid=0 and Del=0 and ParentID<>0 and OvertimeUID = @UID and OvertimeDate between @StartDateTime and @EndDateTime " + WhereStateCode + " and OvertimeDate not in (select DayDate from CalendarDays where Invalid = 0 and Del = 0 and (DaysType = 'OffDay' or DaysType = 'Statutory') and DayDate between @StartDateTime and @EndDateTime) or OvertimeID in (select ParentID from HROvertime where Invalid=0 and Del=0 and ParentID<>0 and OvertimeUID = @UID and OvertimeDate between @StartDateTime and @EndDateTime " + WhereStateCode + " and OvertimeDate not in (select DayDate from CalendarDays where Invalid = 0 and Del = 0 and (DaysType = 'OffDay' or DaysType = 'Statutory') and DayDate between @StartDateTime and @EndDateTime))" + KeywordQueryStr;
                    break;

                //个人月度休息日加班，or OvertimeID in 是通过他们的子记录得到父记录
                case "getoffdayovertime":
                    _sql = "select * from HROvertime where Invalid=0 and Del=0 and ParentID<>0 and OvertimeUID = @UID and OvertimeDate between @StartDateTime and @EndDateTime " + WhereStateCode + " and OvertimeDate in (select DayDate from CalendarDays where Invalid = 0 and Del = 0 and DaysType = 'OffDay' and DayDate between @StartDateTime and @EndDateTime) or OvertimeID in (select ParentID from HROvertime where Invalid=0 and Del=0 and ParentID<>0 and OvertimeUID = @UID and OvertimeDate between @StartDateTime and @EndDateTime " + WhereStateCode + " and OvertimeDate in (select DayDate from CalendarDays where Invalid = 0 and Del = 0 and DaysType = 'OffDay' and DayDate between @StartDateTime and @EndDateTime))" + KeywordQueryStr;
                    break;

                //个人月度法定加班，or OvertimeID in 是通过他们的子记录得到父记录
                case "getstatutory":
                    _sql = "select * from HROvertime where Invalid=0 and Del=0 and ParentID<>0 and OvertimeUID = @UID and OvertimeDate between @StartDateTime and @EndDateTime " + WhereStateCode + " and OvertimeDate in (select DayDate from CalendarDays where Invalid = 0 and Del = 0 and DaysType = 'Statutory' and DayDate between @StartDateTime and @EndDateTime) or OvertimeID in (select ParentID from HROvertime where Invalid=0 and Del=0 and ParentID<>0 and OvertimeUID = @UID and OvertimeDate between @StartDateTime and @EndDateTime " + WhereStateCode + " and OvertimeDate in (select DayDate from CalendarDays where Invalid = 0 and Del = 0 and DaysType = 'Statutory' and DayDate between @StartDateTime and @EndDateTime))" + KeywordQueryStr;
                    break;

                //个人休假数据
                //个人需代休
                case "getneeddaixiu":
                    //开始你的逻辑
                    break;

                //个人已代休，带有休假类型条件 and HolidayTypeID=2
                case "getalreadydaixiu":
                    //原以休假开始时间和结束时间进行统计，现改为以加班月份进行统计（20210608）
                    // _sql = "select * from HRLeave where Invalid=0 and Del=0 and LeaveUID = @UID and HolidayTypeID=2 and StartDateTime<=@EndDateTime and EndDateTime>=@StartDateTime" + WhereStateCode + 
                    _sql = "select * from HRLeave where Invalid=0 and Del=0 and LeaveUID = @UID and HolidayTypeID=2 and OvertimeDate between '" + StartDateTime.toDateFormat("yyyy-MM") + "' and '" + EndDateTime.toDateFormat("yyyy-MM") + "' " + WhereStateCode + KeywordQueryStr;
                    break;

                //个人所有休假（首页明细用），没有带休假类型条件HolidayTypeID
                case "getleave":
                    _sql = "select * from HRLeave where Invalid=0 and Del=0 and LeaveUID = @UID and StartDateTime<=@EndDateTime and EndDateTime>=@StartDateTime" + WhereStateCode + KeywordQueryStr;
                    break;

                //默认根据时间范围统计月度或季度（根据传入的日期范围），or OvertimeID in 是通过他们的子记录得到父记录
                default:
                    _sql = "select * from HROvertime where Invalid=0 and Del=0 and ParentID<>0 and OvertimeUID=@UID and OvertimeDate between @StartDateTime and @EndDateTime " + WhereStateCode +
                " or OvertimeID in (select ParentID from HROvertime where Invalid=0 and Del=0 and ParentID<>0 and OvertimeUID=@UID and OvertimeDate between @StartDateTime and @EndDateTime " + WhereStateCode + ")" + KeywordQueryStr;
                    break;

            }


            SqlParameter[] _sp = { new SqlParameter("@UID",SqlDbType.Int),
                     new SqlParameter("@StartDateTime",SqlDbType.DateTime),
                     new SqlParameter("@EndDateTime",SqlDbType.DateTime),
                     new SqlParameter("@Keyword",SqlDbType.VarChar),
             };

            _sp[0].Value = UID;
            _sp[1].Value = StartDateTime.toDateTime("yyyy-MM-dd HH:mm:ss");
            _sp[2].Value = EndDateTime.toDateTime("yyyy-MM-dd HH:mm:ss");
            _sp[3].Value = Keyword.toStringTrim();

            DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

            return _dt;
        }



        /// <summary>
        /// 获取所用户属性返回数据表（如用户部门、用户职称、用户角色、用户工时制、用户排班表）
        /// </summary>
        /// <param name="DataTableType">UserDepts（用户部门）、UserJobTitle（用户职称）、UserRoles（用户角色）、HRUserWorkHourSystem（用户工时制）、UserOnDuty（用户排班表）</param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static DataTable GetAllUserAttrDataTable(string DataTableType, string StartDate = "", string EndDate = "", string Where = "")
        {
            DataTable _dt = null;
            string _sql = string.Empty, StartDateTime = string.Empty, EndDateTime = string.Empty;

            //如果开始时间和结束时间为空时，默认取当前日期的第一天和最后一天
            if (string.IsNullOrEmpty(StartDate) && string.IsNullOrEmpty(EndDate))
            {
                StartDate = DateTime.Now.ToString().toDateMFirstDay();
                EndDate = DateTime.Now.ToString().toDateMLastDay();
            }
            else
            {
                StartDateTime = StartDate.toDateFormat() + " 00:00:00.000";
                EndDateTime = EndDate.toDateFormat() + " 23:59:59.998";
            }

            switch (DataTableType)
            {
                case "UserDepts":
                    _sql = "select a.UID,b.* from UserDepts a left join Department b on a.DeptID=b.DeptID where a.Invalid=0 and a.Del=0 and b.Invalid=0 and b.Del=0";
                    break;
                case "UserJobTitle":
                    _sql = "select a.UID,b.* from UserJobTitle a left join JobTitle b on a.JTID=b.JTID where a.Invalid=0 and a.Del=0 and b.Invalid=0 and b.Del=0";
                    break;
                case "UserRoles":
                    _sql = "select a.UID,b.* from UserRoles a left join Roles b on a.RID=b.RID where a.Invalid=0 and a.Del=0 and b.Invalid=0 and b.Del=0";
                    break;
                case "UserWorkHourSystem":
                    _sql = "select a.UID,a.WorkHourSysDate,a.DateModified,b.WorkHourSysName,b.WarningValue,b.MaxValue from HRUserWorkHourSystem a left join HRWorkHourSystem b on a.WorkHourSysID=b.WorkHourSysID where a.Invalid=0 and a.Del=0 and b.Invalid=0 and b.Del=0 " +
                        //"and WorkHourSysDate between '" + StartDate + "' and '" + EndDate + "'" +
                        " order by WorkHourSysDate desc, a.DateModified desc";
                    break;
                case "UserOnDuty":  //新的排班表
                    _sql = "select * from HROnDutyForm a where Invalid=0 and Del=0 and StateCode=100 and a.DateCreated in (select max(b.DateCreated) from HROnDutyForm b where b.DutyUID=a.DutyUID and a.DutyDate=b.DutyDate) " +
                      "and DutyDate between '" + StartDate + "' and '" + EndDate + "'" +
                      " order by DutyDate desc";
                    break;
                case "UserOvertime":
                    _sql = "select * from HROvertime where Invalid=0 and Del=0 and ParentID<>0 and OvertimeDate between '" + StartDateTime + "' and '" + EndDateTime + "' " + Where;
                    break;
                //用户休假记录
                case "UserLeave":
                    _sql = "select a.*, b.HolidayName from HRLeave a left join HRHolidayType b on a.HolidayTypeID=b.HolidayTypeID where a.Invalid=0 and a.Del=0 and a.StartDateTime<='" + EndDateTime + "' and a.EndDateTime>='" + StartDateTime + "' " + Where;
                    break;
                //用户剩余所有假期
                case "UserRemainingLeave":
                    _sql = "select * from HRPersonalHoliday where Invalid=0 and Del=0 " + Where;
                    break;
            }

            if (!string.IsNullOrEmpty(_sql))
                _dt = MsSQLDbHelper.Query(_sql).Tables[0];

            return _dt;

        }

        /// <summary>
        /// 获取自定义用户属性返回数据表（如用户部门、用户职称、用户角色），CustomUID为空时默认为当前登录用户
        /// </summary>
        /// <param name="DataTableType">UserDepts、UserRoles、UserJobTitle</param>
        /// <param name="CustomUID">CustomUID为空时默认为当前登录用户</param>
        /// <returns></returns>
        public static DataTable GetCustomUserAttrDataTable(string DataTableType, string CustomUID = "")
        {
            DataTable _dt = null;

            if (string.IsNullOrEmpty(CustomUID))
                CustomUID = MicroUserInfo.GetUserInfo("UID");

            switch (DataTableType)
            {
                case "UserDepts":
                    //得到当前用户部门
                    string _sqlUserDepts = "select * from UserDepts where Invalid=0 and Del=0 and UID=" + CustomUID + "";
                    _dt = MsSQLDbHelper.Query(_sqlUserDepts).Tables[0];
                    break;

                case "UserRoles":
                    //得到当前用户角色
                    string _sqlUserRoles = "select * from UserRoles where Invalid=0 and Del=0 and UID=" + CustomUID + "";
                    _dt = MsSQLDbHelper.Query(_sqlUserRoles).Tables[0];
                    break;

                case "UserJobTitle":
                    //得到当前用户职位
                    string _sqlJobTitle = "select * from UserJobTitle where Invalid=0 and Del=0 and UID=" + CustomUID + "";
                    _dt = MsSQLDbHelper.Query(_sqlJobTitle).Tables[0];
                    break;
            }

            return _dt;
        }

        /// <summary>
        /// 获取指定时间范围内日历日子数据
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static DataTable GetCalendarDays(string StartDate, string EndDate)
        {
            DataTable _dt = null;
            string _sql = string.Empty, StartDateTime = string.Empty, EndDateTime = string.Empty;

            StartDateTime = StartDate.toDateFormat() + " 00:00:00.000";
            EndDateTime = EndDate.toDateFormat() + " 23:59:59.998";

            _sql = "select * from CalendarDays where Invalid = 0 and Del = 0 and DayDate between @StartDateTime and @EndDateTime";
            SqlParameter[] _sp = { new SqlParameter("@StartDateTime",SqlDbType.VarChar,50),
                                    new SqlParameter("@EndDateTime",SqlDbType.VarChar,50),
                                };
            _sp[0].Value = StartDateTime;
            _sp[1].Value = EndDateTime;

            _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

            return _dt;

        }


        /// <summary>
        /// 获取关键字查询字符串，把MicroTabls表中设定为queryAsKeywordField=true的字段构造成sql查询语句返回（通常在MicroFormList页面中用到）
        /// </summary>
        /// <param name="ShortTableName"></param>
        /// <returns></returns>
        public static string GetKeywordQueryStr(string ShortTableName)
        {
            string flag = string.Empty,
                QueryItem = string.Empty;

            if (!string.IsNullOrEmpty(ShortTableName))
            {
                ShortTableName = ShortTableName.ToLower();

                //如果是ShortTableName == "overtime"则返回，否则返回
                if (ShortTableName == "overtime")
                {
                    flag = " and (" +
                          " (DeptID in (select DeptID from Department where Invalid = 0 and del = 0 and LevelCode like(select top 1 LevelCode from Department where Invalid=0 and Del=0 and (DeptName like @Keyword or AdDepartment like @Keyword or Description like @Keyword)) + '%'))" +
                          " or (ShiftTypeID in (select top 1 ShiftTypeID from HRShiftType where Invalid = 0 and Del = 0 and(ShiftName like @Keyword or Alias like @Keyword)))" +
                          //带出ShiftTypeID的父记录
                          " or (OvertimeID in (select ParentID from HROvertime where Invalid=0 and Del=0 and ShiftTypeID in (select top 1 ShiftTypeID from HRShiftType where Invalid=0 and Del=0 and (ShiftName like @keyword or Alias like @keyword))))" +
                          " or (CHARINDEX(',' + convert(varchar, (select top 1 OvertimeMealID from HROvertimeMeal where Invalid=0 and Del=0 and OvertimeMealName like @keyword) ) + ',',',' + OvertimeMealID + ',')> 0)" +
                          //带出OvertimeMealID的父记录
                          " or (OvertimeID in (select ParentID from HROvertime where Invalid=0 and Del=0 and (CHARINDEX(',' + convert(varchar, (select top 1 OvertimeMealID from HROvertimeMeal where Invalid=0 and Del=0 and OvertimeMealName like @keyword) ) + ',',',' + OvertimeMealID + ',')> 0)))" +
                          " or (FormState like @Keyword)" +  //父记录与子记录一样，所以不需要单独再查询父记录
                          " or (FormNumber like @Keyword)" +
                          " or (OvertimeDisplayName like @Keyword)" +
                          " or (Location like @Keyword)" +
                          " or (Reason like @Keyword)" +
                          " or (WorkHourSysName like @Keyword)" +
                          " or (Applicant like @Keyword)" +
                          " or (Phone like @keyword)" +
                          " or (Tel like @keyword)" +
                          //带出上面几个的父记录
                          " or (OvertimeID in (select ParentID from HROvertime where Invalid=0 and Del=0 and (OvertimeDisplayName like @keyword or Location like @keyword or Reason like @keyword or WorkHourSysName like @keyword)))" +
                          " ) ";
                }
                else
                {
                    string TableName = MicroPublic.GetTableName(ShortTableName);
                    //获取表的字段和相关属性
                    var getTableAttr = GetTableAttr(TableName);

                    Boolean MainSub = getTableAttr.MainSub;

                        string _sql = "select TID, TabColName, Title, DataType, ctlTextValue, ctlSourceTable from MicroTables where Invalid=0 and Del=0 and queryAsKeywordField=1 and ParentID = (select TID from MicroTables where ShortTableName=@ShortTableName) order by Sort";

                    SqlParameter[] _sp = { new SqlParameter("@ShortTableName", SqlDbType.VarChar, 100) };
                    _sp[0].Value = ShortTableName.toStringTrim();

                    DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < _dt.Rows.Count; i++)
                        {
                            string or = string.Empty,
                                TabColName = _dt.Rows[i]["TabColName"].toStringTrim(),
                                Title = _dt.Rows[i]["Title"].toStringTrim(),
                                DataType = _dt.Rows[i]["DataType"].toStringTrim(),
                                ctlSourceTable = _dt.Rows[i]["ctlSourceTable"].toStringTrim(),
                                ctlTextValue = _dt.Rows[i]["ctlTextValue"].toStringTrim();

                            or = i == 0 ? or : " or ";

                            if (!string.IsNullOrEmpty(ctlSourceTable) && !string.IsNullOrEmpty(ctlTextValue))
                            {
                                string SubQueryItem = GetSubKeywordQueryStr(ctlSourceTable, TabColName, ctlTextValue, DataType);
                                if (!string.IsNullOrEmpty(SubQueryItem))
                                    QueryItem += or + SubQueryItem;
                            }
                            else
                                QueryItem += or + "(" + TabColName + " like @Keyword)";
                        }

                        string WhereParentID = string.Empty;

                        if (MainSub && MsSQLDbHelper.ColumnExists(TableName, "ParentID"))
                            WhereParentID = " or (ParentID=0) " ;

                        flag = " and (" + QueryItem + WhereParentID + ")";
                    }
                }
            }

            return flag;

        }


        public static string GetSubKeywordQueryStr(string ctlSourceTable, string TabColName, string ctlTextValue, string DataType)
        {
            string flag = string.Empty,
                QueryItem = string.Empty;

            string _sql = "select TID, ParentID, TabColName, Title, DataType, colCustomField, ctlSourceTable, tbMainSub, queryAsKeywordField from MicroTables where Invalid = 0 and Del = 0 and(ParentID = (select TID from MicroTables where TabColName = @ctlSourceTable) or TID = (select TID from MicroTables where TabColName = @ctlSourceTable)) order by TID";

            SqlParameter[] _sp = { new SqlParameter("@ctlSourceTable", SqlDbType.VarChar, 100) };
            _sp[0].Value = ctlSourceTable.toStringTrim();

            DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

            if (_dt != null && _dt.Rows.Count > 0)
            {
                DataRow[] _rows = _dt.Select("ParentID<>0 and queryAsKeywordField=1");
                if (_rows.Length > 0)
                {
                    int i = 0;
                    foreach (DataRow _dr in _rows)
                    {
                        string or = string.Empty,
                            _TabColName = _dr["TabColName"].toStringTrim();

                        or = i == 0 ? or : " or ";
                        QueryItem += or + _TabColName + " like @Keyword";

                        i = i + 1;
                    }

                    Boolean MainSub = _dt.Select("ParentID=0")[0]["tbMainSub"].toBoolean();

                    if (DataType == "int")
                    {
                        if (MainSub)
                            QueryItem = "(" + TabColName + " in (select " + ctlTextValue + " from " + ctlSourceTable + " where Invalid = 0 and Del = 0 and LevelCode like(select top 1 LevelCode from " + ctlSourceTable + " where Invalid=0 and Del=0 and (" + QueryItem + ")) + '%'))";
                        else
                            QueryItem = "(" + TabColName + " in (select " + ctlTextValue + " from " + ctlSourceTable + " where Invalid = 0 and Del = 0  and(" + QueryItem + ") ))";

                        flag = QueryItem;
                    }

                }
            }

            return flag;

        }


        /// <summary>
        /// DataTable导出数据到Excel
        /// </summary>
        /// <param name="SourceDT">DataTabte</param>
        /// <param name="FileName">文件名</param>
        public static void ExportExcel(DataTable SourceDT, string FileName = "FileName", string SheetName = "Sheet1")
        {
            FileName = string.IsNullOrEmpty(FileName) ? "FileName" : FileName;

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(SourceDT, SheetName);

                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.Charset = "utf-8";
                HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + FileName + ".xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
                    HttpContext.Current.Response.Flush();
                    HttpContext.Current.Response.End();
                }
            }

        }

        /// <summary>
        /// 获取OnDuty的DataTableList（此方法有两个功能：1.检查填写的Excel格式是否正确。 2. 读取Excel生成LayuiDataTable用于预览）
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static string GetOnDutyDataTableList(string FileName, string FilePath)
        {
            string flag = string.Empty;

            string strTpl = "\"code\":{0},\"msg\":\"{1}\",\"filepath\":\"{2}\",\"title\":\"{3}\",\"cols\":\"{4}\" ,\"data\":{5}";
            int Code = 0;

            string Msg = string.Empty, MsgUserName = string.Empty, MsgChineseName = string.Empty, MsgLocation = string.Empty, MsgDutyDate = string.Empty, MsgShiftName = string.Empty, MsgEmptyColumns = string.Empty;
            string Locations = string.Empty, ShiftTypes = string.Empty;
            string dataTpl = "\"{0}\":\"{1}\",";

            //获取文件真实路径
            string fullFilePath = HttpContext.Current.Server.MapPath(FilePath);

            DataTable _dtExcel = MsSQLDbHelper.QueryExcel(fullFilePath).Tables[0];

            string Cols = string.Empty, Data = string.Empty;

            //Rows大于1代表从第3行（0起步）开始读取数据，Columns大于3代表从第5列开始取数
            //因为Rows1是标题，Rows2是例子
            //因为Columns前4列是基本数据
            if (_dtExcel != null && _dtExcel.Rows.Count > 1 && _dtExcel.Columns.Count > 3)
            {
                DataTable _dtUsers = MicroDataTable.GetDataTable("Use");
                DataTable _dtLocation = MicroDataTable.GetDataTable("CommonLocations");
                DataTable _dtShiftType = MicroDataTable.GetDataTable("ShiftType");

                //获取所有常用地点
                if (_dtLocation.Rows.Count > 0)
                {
                    DataRow[] _LocationRows = _dtLocation.Select("", "Sort");
                    foreach (DataRow _drLocation in _LocationRows)
                    {
                        Locations += _drLocation["LocationValue"].toStringTrim() + "、";
                    }

                    if (!string.IsNullOrEmpty(Locations))
                        Locations = Locations.Substring(0, Locations.Length - 1);
                }

                //获取所有班次名称
                if (_dtShiftType.Rows.Count > 0)
                {
                    DataRow[] _dtShiftTypeRows = _dtShiftType.Select("", "Sort");
                    foreach (DataRow _drShiftType in _dtShiftTypeRows)
                    {
                        ShiftTypes += _drShiftType["Alias"].toStringTrim() + "、";
                    }

                    if (!string.IsNullOrEmpty(ShiftTypes))
                        ShiftTypes = ShiftTypes.Substring(0, ShiftTypes.Length - 1);
                }


                //*****（全局）得到节假日 日期Start*****
                string _sql4 = "select * from CalendarDays where Invalid=0 and Del=0 and DayDate between @StartDay and @EndDay ";
                    SqlParameter[] _sp4 = {
                    new SqlParameter("@StartDay", SqlDbType.VarChar, 50),
                    new SqlParameter("@EndDay", SqlDbType.VarChar, 50),
                };

                _sp4[0].Value = DateTime.Now.AddYears(-1).toDateFormat();  //因为无法确定日期所以取当时间的前一年和后一年
                _sp4[1].Value = DateTime.Now.AddYears(1).toDateFormat();

                DataTable _dt4 = MsSQLDbHelper.Query(_sql4, _sp4).Tables[0];
                //*****（全局）得到假期日期End*****

                //***********构建表头Start***********
                //注意这里是获取表头，所以读取的是列的数据
                for (int i = 0; i < _dtExcel.Columns.Count; i++)
                {
                    string CurrDate = _dtExcel.Rows[0][i].toStringTrim().Split('（')[0].Split('(')[0].toStringTrim(),  //英文或中文括号
                        Field = CurrDate,
                        OffDay = string.Empty;

                    string Week = string.Empty,
                           FixedLeft = string.Empty,
                           hide = string.Empty;

                    if (i > 3)
                    {
                        Week = MicroPublic.GetWeek(CurrDate, "WW");

                        DataRow[] _rows4 = _dt4.Select("DayDate= '" + CurrDate + "'");
                        if (_rows4.Length > 0)
                        {
                            string DaysType = _rows4[0]["DaysType"].ToString();
                            if (DaysType == "OffDay")
                                OffDay = " 休";
                            else if (DaysType == "Statutory")
                                OffDay = " 法";
                        }
                        else
                        {
                            if (Week == "周六" || Week == "周日")
                                OffDay = " 班";
                        }

                        Week = "（" + Week + OffDay + "）";
                        if (CurrDate.toStringTrim().toDateTimeBoolean())
                            MsgDutyDate += MicroPublic.GetLetters(i) + 1 + "、";

                    }
                    else
                        FixedLeft = ", fixed: 'left'";

                    //表头默认宽度
                    int width = 180;

                    //第1、2列宽度
                    if (i == 1 || i == 2)
                        width = 120;

                    if (i == 1)
                        hide = ", hide:true";

                    Cols += "{field: '" + Field + "', title: '" + Field + Week + "', width:" + width + ", align:'center', sort:true " + FixedLeft + hide + " },";
                }
                //构造表头字符串
                Cols = "[" + Cols.Substring(0, Cols.Length - 1) + "]";
                //***********构建表头End***********


                //先循环行再循环列，从第3行开始 i=2（第1行是表头，第2行是例子，第3行开始）
                for (int i = 2; i < _dtExcel.Rows.Count; i++)
                {
                    string _Data = string.Empty;
                    int EmptyColumns = 0;

                    Data += "{";

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
                        }
                        //判断Location是否存在，第3个单元格的地点是否是数据库存在的常用地点
                        else if (j == 3)
                        {
                            if (!string.IsNullOrEmpty(Value))
                            {
                                if (_dtLocation.Select("LocationValue='" + Value + "'").Length == 0)
                                {
                                    if (Value.Split(':').Length > 1)
                                    {
                                        if (Value.Split(':')[0] != "其它" && Value.Split(':')[0] != "其他")
                                            MsgLocation += MicroPublic.GetLetters(j) + i + "、";
                                    }

                                    else if (Value.Split('：').Length > 1)
                                    {
                                        if (Value.Split(':')[0] != "其它" && Value.Split('：')[0] != "其他")
                                            MsgLocation += MicroPublic.GetLetters(j) + i + "、";
                                    }
                                    else
                                        MsgLocation += MicroPublic.GetLetters(j) + i + "、";
                                }
                            }
                            else
                                MsgLocation += MicroPublic.GetLetters(j) + i + "、";

                        }
                        //判断班次（班次不为空时）是否存在，大于第3个单元格的班次是否是数据库存在的班次
                        else if (j > 3)
                        {
                            //如果单元格值不为空时，再判断不为“-”时判断是否为特殊班次，检查填入的值是否合法
                            if (!string.IsNullOrEmpty(Value))
                            {
                                if (Value.toStringTrim() != "-" && Value.toStringTrim() != "休")
                                {
                                    //如果值不为-号时，出现两种情况，有可能是正常班次，有可能是特殊班次，特殊班次时判断格式是否正确（特13:00~17:00）
                                    //如果不是正常班次时，判断是否为特殊班次
                                    if (_dtShiftType.Select("Alias='" + Value + "'").Length == 0)
                                    {
                                        var getOnDutyFormat = MicroPrivateHelper.MicroPrivate.GetOnDutyFormat(Text, Value);
                                        if (!getOnDutyFormat.IsCorrect)
                                            MsgShiftName += MicroPublic.GetLetters(j) + (i + 1) + "、";
                                    }
                                }
                            }
                            //如果为空时让空的列数+1，主要用于判断是否所有的日期班次都为空，如果都为空时禁止提交
                            else
                                EmptyColumns = EmptyColumns + 1;
                        }

                        _Data += string.Format(dataTpl, Text, Value);

                    }

                    _Data = _Data.Substring(0, _Data.Length - 1);
                    Data += _Data;

                    Data += "},";

                    if ((_dtExcel.Columns.Count - 4) == EmptyColumns)
                        MsgEmptyColumns += "第" + (i + 1) + "行（" + _dtExcel.Rows[i][2].toStringTrim() + "）、";

                }

                //如果没有报错时（所有报错消息都为空时构造json数据）
                if (string.IsNullOrEmpty(MsgUserName) && string.IsNullOrEmpty(MsgLocation) && string.IsNullOrEmpty(MsgDutyDate) && string.IsNullOrEmpty(MsgShiftName) && string.IsNullOrEmpty(MsgEmptyColumns))
                {
                    Code = 100;
                    Msg = "True导入成功！";
                    Data = "[" + Data.Substring(0, Data.Length - 1) + "]";
                }
                else
                {
                    Msg = "<b>导入失败，详细错误：</b><br/>";
                    Data = "[]";

                    if (!string.IsNullOrEmpty(MsgUserName))
                    {
                        MsgUserName = MsgUserName.Substring(0, MsgUserName.Length - 1);
                        MsgUserName = "电脑ID不存在，单元格：" + MsgUserName + "<br/><br/>";
                    }

                    if (!string.IsNullOrEmpty(MsgLocation))
                    {
                        MsgLocation = MsgLocation.Substring(0, MsgLocation.Length - 1);
                        MsgLocation = "无法匹配地点，单元格：" + MsgLocation + "<br/><div class=\\\"layui-form-mid layui-word-aux \\\">*备注：参考可选地点：" + Locations + " <br/>如果是其它地点请按 “其它：+ 地点” 的格式输入，如-> 其它：广州xxx公司 </div><br/><br/>";
                    }

                    if (!string.IsNullOrEmpty(MsgDutyDate))
                    {
                        MsgDutyDate = MsgDutyDate.Substring(0, MsgDutyDate.Length - 1);
                        MsgDutyDate = "标题行日期格式不正确，单元格：" + MsgDutyDate + "<br/><div class=\\\"layui-form-mid layui-word-aux \\\">*备注：标题格式 yyyy-MM-dd (星期)</div><br/><br/>";
                    }

                    if (!string.IsNullOrEmpty(MsgShiftName))
                    {
                        MsgShiftName = MsgShiftName.Substring(0, MsgShiftName.Length - 1);
                        MsgShiftName = "班次填写不正确，单元格：" + MsgShiftName + "<br/><div class=\\\"layui-form-mid layui-word-aux \\\">*备注：参考可选班次：" + ShiftTypes + "<br/>如果是特殊班次请按 “特开始时间~结束时间” 的格式输入，如-> 特11:00~20:00 <br/>如果是休息请填入 “休”，但仍需写休假申请 </div><br/><br/>";
                        //<br/>如果想取消某一天申请过的班次请填入英文横杠 “-”
                    }

                    if (!string.IsNullOrEmpty(MsgEmptyColumns))
                    {
                        MsgEmptyColumns = MsgEmptyColumns.Substring(0, MsgEmptyColumns.Length - 1);
                        MsgEmptyColumns = "不能每天的班次都为空，相关行：" + MsgEmptyColumns + "<br/><br/>";
                    }

                }

                flag = "{" + string.Format(strTpl, Code, Msg + MsgUserName + MsgLocation + MsgDutyDate + MsgShiftName + MsgEmptyColumns, FilePath, FileName, Cols, Data) + "}";

            }
            else
                flag = "{" + string.Format(strTpl, Code, "导入失败，上传的Excel内容格式不正确，请根据正确的模板格式进行导入", "", "", "", "[]") + "}";


            return flag;
        }

    }

}