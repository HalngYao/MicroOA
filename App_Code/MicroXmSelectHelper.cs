using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroDTHelper;
using MicroUserHelper;

namespace MicroXmSelectHelper
{

    /// <summary>MicroXmSelectHelper
    /// MicroXmSelectHelper 的摘要说明
    /// </summary>
    public class MicroXmSelect
    {

        /// <summary>
        /// 获取适用于XmSelect的数据
        /// </summary>
        /// <param name="TableName">数据来源表名</param>
        /// <param name="DisplayNameField">显示字段</param>
        /// <param name="DisplayValueField">值字段</param>
        /// <param name="DefaultValue">默认值</param>
        /// <returns></returns>
        public static string GetXmSelect(string TableName, string DisplayNameField, string DisplayValueField, string IsApprovalForm, string FormID = "", string DefaultValue = "")
        {
            string flag = string.Empty;
            try
            {
                var getTableAttr = MicroDataTable.GetTableAttr(TableName);
                DataTable MicroDT = getTableAttr.MicroDT; //获取存放在MicroTables的数据，该表是存放每个表的表名和字段
                string PrimaryKeyName = getTableAttr.PrimaryKeyName;  //获得主键字段名称
                string SortFieldName = getTableAttr.SortFieldName;  //获得排序字段名称
                string OrderBy = getTableAttr.OrderBy;  //获得order by 命令字符串
                Boolean MainSub = getTableAttr.MainSub;  //用于判断是否开启父子项
                Boolean IsPage = getTableAttr.IsPage; //是否开启分页

                string _FormID = string.Empty;

                if (IsApprovalForm.toBoolean())
                {
                    if (!string.IsNullOrEmpty(FormID) && MsSQLDbHelper.ColumnExists(TableName, "FormID"))
                        _FormID = " and FormID = " + FormID.toInt();
                }

                string Invalid = " and Invalid=0 ";
                if (MicroUserInfo.CheckUserRole("Administrators"))
                    Invalid = " ";

                string _sql = "select * from " + TableName + " where Del=0 " + Invalid + _FormID + " " + OrderBy;
                DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    if (MainSub)
                        flag = GetMain(_dt, DisplayNameField, DisplayValueField, SortFieldName, DefaultValue);
                    else
                        flag = GetSingle(_dt, DisplayNameField, DisplayValueField, SortFieldName, DefaultValue);
                }
            }
            catch (Exception ex) {
                flag = ex.ToString();
            }

            return flag;

        }

        /// <summary>
        /// 获取适用于XmSelect的数据，适用于具有父子关系的数据表
        /// </summary>
        /// <param name="_dt">数据源</param>
        /// <param name="DisplayNameField">显示字段</param>
        /// <param name="DisplayValueField">值字段</param>
        /// <param name="SortFieldName">排序字段 Field1,Field2</param>
        /// <param name="DefaultValue">默认值</param>
        /// <returns></returns>
        public static string GetMain(DataTable _dt, string DisplayNameField, string DisplayValueField, string SortFieldName, string DefaultValue = "")
        {
            string flag = string.Empty, _str = string.Empty;
            if (_dt != null && _dt.Rows.Count > 0)
            {
                int i = 0;

                //得到顶级部门
                DataRow[] _rows = _dt.Select("ParentID=0", SortFieldName);

                ////有记录时加上中括号[]
                //if (_rows.Length > 0)
                //    _str += "[";

                foreach (DataRow _dr in _rows)
                {

                    i = i + 1;
                    string Val = _dr[DisplayValueField].toStringTrim();
                    string Name = _dr[DisplayNameField].toStringTrim();

                    Boolean Selected = false;
                    if (!string.IsNullOrEmpty(DefaultValue))
                    {
                        string[] Arr = DefaultValue.Split(',');
                        for (int j = 0; j < Arr.Length; j++)
                        {
                            if (Arr[j].toStringTrim().ToLower() == Val.ToLower())
                                Selected = true;
                        }
                    }

                    _str += "{";
                    _str += "name:'" + Name + "', value: " + Val + ", selected: " + Selected.ToString().ToLower() + "";
                    _str += GetSub(_dt, DisplayNameField, DisplayValueField, SortFieldName, Val, DefaultValue); //查询子部门
                    _str += "}";

                    //如果不是最后记录里要加上逗号
                    if (_rows.Length != i)
                        _str += ",";
                }

                ////有记录时加上中括号[]
                //if (_rows.Length > 0)
                //    _str += "]";

            }
            flag = "[" + _str + "]";

            return flag;
        }


        public static string GetSub(DataTable _dt, string DisplayNameField, string DisplayValueField, string SortFieldName, string ParentID, string DefaultValue = "")
        {
            string flag = string.Empty, _str = string.Empty;

            DataRow[] _rows = _dt.Select("ParentID=" + ParentID.toInt(), SortFieldName);

            if (_rows.Length > 0)
            {
                int i = 0;

                _str += ", children: [";

                foreach (DataRow _dr in _rows)
                {
                    i = i + 1;
                    string Val = _dr[DisplayValueField].toStringTrim();
                    string Name = _dr[DisplayNameField].toStringTrim();

                    Boolean Selected = false;
                    if (!string.IsNullOrEmpty(DefaultValue))
                    {
                        string[] Arr = DefaultValue.Split(',');
                        for (int j = 0; j < Arr.Length; j++)
                        {
                            if (Arr[j].toStringTrim().ToLower() == Val.ToLower())
                                Selected = true;
                        }
                    }

                    _str += "{";
                    _str += "name:'" + Name + "', value: " + Val + ", selected: " + Selected.ToString().ToLower() + "";
                    _str += GetSub(_dt, DisplayNameField, DisplayValueField, SortFieldName, Val, DefaultValue);
                    _str += "}";

                    if (_rows.Length != i)
                        _str += ",";

                }
                _str += "]";

            }

            flag = _str;
            return flag;

        }

        /// <summary>
        /// 获取适用于XmSelect的数据，适用于没有父子关系的表
        /// </summary>
        /// <param name="_dt">数据源</param>
        /// <param name="DisplayNameField">显示字段</param>
        /// <param name="DisplayValueField">值字段</param>
        /// <param name="SortFieldName">排序字段 Field1,Field2</param>
        /// <param name="DefaultValue">默认值</param>
        /// <returns></returns>
        public static string GetSingle(DataTable _dt, string DisplayNameField, string DisplayValueField, string SortFieldName, string DefaultValue = "")
        {
            string flag = string.Empty, _str = string.Empty;
            if (_dt != null && _dt.Rows.Count > 0)
            {
                int i = 0;

                //得到顶级部门
                DataRow[] _rows = _dt.Select("", SortFieldName);

                foreach (DataRow _dr in _rows)
                {

                    i = i + 1;
                    string Val = _dr[DisplayValueField].toStringTrim();
                    string Name = _dr[DisplayNameField].toStringTrim();

                    Boolean Selected = false;
                    if (!string.IsNullOrEmpty(DefaultValue))
                    {
                        string[] Arr = DefaultValue.Split(',');
                        for (int j = 0; j < Arr.Length; j++)
                        {
                            if (Arr[j].toStringTrim().ToLower() == Val.ToLower())
                                Selected = true;
                        }
                    }

                    _str += "{";
                    _str += "name:'" + Name + "', value: " + Val + ", selected: " + Selected.ToString().ToLower() + "";
                    _str += "}";

                    //如果不是最后记录里要加上逗号
                    if (_rows.Length != i)
                        _str += ",";
                }

            }
            flag = "[" + _str + "]";

            return flag;
        }



        /// <summary>
        /// 获取显示名组成顿号分隔字符串返回，主要用于申请表单在查看状态下显示数据，在新增和修改状态下它们是以控件形式显示的
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="DisplayNameField"></param>
        /// <param name="DisplayValueField"></param>
        /// <param name="FormID"></param>
        /// <param name="DefaultValue"></param>
        /// <returns></returns>
        public static string GetCtlDisplayName(string TableName, string DisplayNameField, string DisplayValueField, string FormID = "", string DefaultValue = "")
        {
            //本想取名 GetXmSelectDisplayName但想到不紧紧是XmSelect提交后的数据可用，普通Select、CheckBox、Radio也适用，也暂时放在本cs中
            //TableName = "HRLeave";
            string flag = string.Empty;

            if (!string.IsNullOrEmpty(TableName) && !string.IsNullOrEmpty(DisplayNameField) && !string.IsNullOrEmpty(DisplayValueField))
            {
                var getTableAttr = MicroDataTable.GetTableAttr(TableName);
                DataTable MicroDT = getTableAttr.MicroDT; //获取存放在MicroTables的数据，该表是存放每个表的表名和字段
                string PrimaryKeyName = getTableAttr.PrimaryKeyName;  //获得主键字段名称
                string SortFieldName = getTableAttr.SortFieldName;  //获得排序字段名称
                string OrderBy = getTableAttr.OrderBy;  //获得order by 命令字符串
                Boolean MainSub = getTableAttr.MainSub;  //用于判断是否开启父子项
                Boolean IsPage = getTableAttr.IsPage; //是否开启分页
                string Where = string.Empty;

                if (!string.IsNullOrEmpty(FormID) && MsSQLDbHelper.ColumnExists(TableName, "FormID"))
                    Where = " and FormID = " + FormID.toInt();

                string Invalid = " and Invalid=0 ";
                if (MicroUserInfo.CheckUserRole("Administrators"))
                    Invalid = " ";

                string _sql = "select * from " + TableName + " where Del=0 " + Invalid + Where + " " + OrderBy;
                DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    DataRow[] _rows = _dt.Select("", SortFieldName);

                    foreach (DataRow _dr in _rows)
                    {
                        string Val = _dr[DisplayValueField].toStringTrim();
                        string Name = _dr[DisplayNameField].toStringTrim();

                        if (!string.IsNullOrEmpty(DefaultValue))
                        {
                            string[] Arr = DefaultValue.Split(',');
                            for (int j = 0; j < Arr.Length; j++)
                            {
                                if (Arr[j].toStringTrim().ToLower() == Val.ToLower())
                                    flag += Name + "、";
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(flag))
                        flag = flag.Substring(0, flag.Length - 1);

                }
            }
            else
                flag = DefaultValue;

            return flag;

        }

    }


}