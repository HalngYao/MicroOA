using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using MicroDBHelper;
using MicroDTHelper;
using MicroPublicHelper;
using MicroWorkFlowHelper;
using Newtonsoft.Json.Linq;
using MicroUserHelper;
using MicroXmSelectHelper;
using MicroAuthHelper;
using MicroPrivateHelper;

namespace MicroFormHelper
{
    /// <summary>
    /// 表单操作方法
    /// </summary>
    public class MicroForm
    {

        /// <summary>
        /// 生成Checkbox的Script脚本的Templet插入HTML页面，被Layui DataTable调用转为Switch开关
        /// </summary>
        /// <param name="ShortTableName">传入的表名</param>
        /// <returns>返回数据库字段为bit的script脚本</returns>
        public static string GetLayCheckBoxTpl(string ShortTableName)
        {
            string flag = string.Empty;
            string TableName = MicroPublic.GetTableName(ShortTableName);
            try
            {

                //InJoinSql=1
                string _sql = "select * from MicroTables where JoinTableColumn=1 and Invalid=0 and Del=0 and (TabColName =@TableName or ParentID in (select TID from MicroTables where TabColName =@TableName)) order by ParentID,Sort";
                SqlParameter[] _sp = { new SqlParameter("TableName", SqlDbType.VarChar, 100) };
                _sp[0].Value = TableName;

                DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

                if (_dt != null && _dt.Rows.Count > 0)
                {
                    //查找主键名称
                    DataRow[] _rows = _dt.Select("DataType='int' and PrimaryKey=1", "ParentID,Sort");
                    if (_rows.Length > 0)
                    {
                        //得到主键名称
                        string IDName = _rows[0]["TabColName"].ToString();

                        DataRow[] _rows2 = _dt.Select("DataType='bit'", "ParentID,Sort");
                        foreach (DataRow _dr2 in _rows2)
                        {
                            flag += "<script type=\"text/html\" id=\"sw" + _dr2["TabColName"].ToString() + "\" >";
                            flag += "<input type=\"checkbox\" name=\"" + _dr2["TabColName"].ToString() + "\" value=\"{{ d." + IDName + "}}\" lay-skin=\"switch\" lay-text=\"是|否\" lay-filter=\"" + _dr2["TabColName"].ToString() + "\" {{ d." + _dr2["TabColName"].ToString() + " == 'True' ? 'checked' : '' }} >";
                            flag += "</script>";
                        }

                    }
                }
            }
            catch { }

            return flag;
        }

        /// <summary>
        /// 生成Checkbox的Script脚本的Templet插入HTML页面，被Layui DataTable调用转为Switch开关
        /// </summary>
        /// <param name="ShortTableName"></param>
        /// <param name="ModuleID"></param>
        /// <returns></returns>
        public static string GetLayCheckBoxTpl(string ShortTableName, string ModuleID)
        {
            string flag = string.Empty;
            string TableName = MicroPublic.GetTableName(ShortTableName);
            try
            {
                Boolean EditTablePermit = MicroAuth.CheckPermit(ModuleID, "11"); //编辑表格
                Boolean InvalidPermit = MicroAuth.CheckPermit(ModuleID, "15");  //Invalid
                Boolean DelPermit = MicroAuth.CheckPermit(ModuleID, "16");   //Del
                Boolean DelAllPermit = MicroAuth.CheckPermit(ModuleID, "18");   //DelAll

                //InJoinSql=1
                string _sql = "select * from MicroTables where JoinTableColumn=1 and Invalid=0 and Del=0 and (TabColName =@TableName or ParentID in (select TID from MicroTables where TabColName =@TableName)) order by ParentID,Sort";
                SqlParameter[] _sp = { new SqlParameter("TableName", SqlDbType.VarChar, 100) };
                _sp[0].Value = TableName;

                DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

                if (_dt != null && _dt.Rows.Count > 0)
                {
                    //查找主键名称
                    DataRow[] _rows = _dt.Select("DataType='int' and PrimaryKey=1", "ParentID,Sort");
                    if (_rows.Length > 0)
                    {
                        //得到主键名称
                        string IDName = _rows[0]["TabColName"].ToString();

                        DataRow[] _rows2 = _dt.Select("DataType='bit'", "ParentID,Sort");
                        foreach (DataRow _dr2 in _rows2)
                        {

                            string TabColName = _dr2["TabColName"].ToString();

                            string disabled = string.Empty;

                            if (TabColName == "Invalid")
                            {
                                if (!InvalidPermit)
                                    disabled = "disabled=\"disabled\"";
                            }
                            else if (TabColName == "Del")
                            {
                                if (!DelPermit && !DelAllPermit)
                                    disabled = "disabled=\"disabled\"";
                            }
                            else
                            {
                                if (!EditTablePermit)
                                    disabled = "disabled=\"disabled\"";
                            }


                            flag += "<script type=\"text/html\" id=\"sw" + TabColName + "\" >";
                            flag += "<input type=\"checkbox\" name=\"" + TabColName + "\" value=\"{{ d." + IDName + "}}\" lay-skin=\"switch\" lay-text=\"是|否\" lay-filter=\"" + TabColName + "\" " + disabled + " {{ d." + TabColName + " == 'True' ? 'checked' : '' }} >";
                            flag += "</script>";
                        }

                    }
                }
            }
            catch { }

            return flag;
        }

        /// <summary>
        /// 用于存放各种动态生成的代码，被前端页面调用
        /// </summary>
        public class Code
        {
            /// <summary>
            /// 主要是围绕layui表单框架动态生成的HTML代码即表单，如：div、input、label、span等
            /// </summary>
            //public string HtmlCode = string.Empty;
            public string HtmlCode
            {
                set;
                get;
            }

            /// <summary>
            /// 所有JSFormCode，包含：JsFormVerifyCode、JsFormCtrlChangeCode、JsFormSubmitCode
            /// </summary>
            //public string JsFormCode = string.Empty;
            public string JsFormCode
            {
                set;
                get;
            }

            /// <summary>
            /// 动态生成表单验证的JS代码form.verify 如：验证input输入长度、自定义验证等
            /// </summary>
            //public string JsFormVerifyCode = string.Empty;
            public string JsFormVerifyCode
            {
                set;
                get;
            }

            /// <summary>
            /// 动态生成控件改变时的JS代码form.on 如：Select、Checkbox，Radio在点选时把值写进input里(select不写）
            /// </summary>
            //public string JsFormCtrlChangeCode = string.Empty;
            public string JsFormCtrlChangeCode
            {
                set;
                get;
            }

            /// <summary>
            /// 动态生成控件展代码，如文本框弹出日期代码。该代码必须是一个完整的代码块，放在layui.use下运行的
            /// </summary>
            public string JsFormExtendCode
            {
                set;
                get;
            }

            /// <summary>
            /// 表单流程xmSelect代码
            /// </summary>
            public string JsFormFlowCode
            {
                set;
                get;
            }

            /// <summary>
            /// 动态生成表单提交按钮的JS代码，如：提交到的页面的Ajax代码
            /// </summary>
            //public string JsFormSubmitCode = string.Empty;
            public string JsFormSubmitCode
            {
                set;
                get;
            }
        }

        /// <summary>
        /// 生成表单，具有HMTL结构的控件（含div,input等元素标签）
        /// </summary>
        /// <param name="Action">表单动作/提交动作</param>
        /// <param name="ShortTableName">短表名</param>
        /// <param name="PrimaryKeyValue">在修改表单时用，作为需要修改记录的唯一ID对应的值</param>
        /// <param name="MID">ModuleID</param>
        /// <param name="ShowHeader">显示标题，默认值True</param>
        /// <param name="IsRefresh">提交后刷新页面，默认值True</param>
        /// <returns></returns>
        public static Code GetHtmlCode(string Action, string ShortTableName, string ModuleID, string FormID, string PrimaryKeyValue, string FormType, Boolean IsApprovalForm, Boolean ShowHeader = true, Boolean IsRefresh = true, Boolean ShowButton = false)
        {
            string TableName = string.Empty, htmlCode = string.Empty, jsFormCode = string.Empty, jsFormVerifyCode = string.Empty, jsFormCtrlChangeCode = string.Empty, jsFormExtendCode = string.Empty, jsFormFlowCode = string.Empty, jsFormSubmitCode = string.Empty, ctlFormStyle = string.Empty, FormStyle = string.Empty, ctlDisplay = string.Empty;
            string FormName = string.Empty, DeptID = string.Empty, Description = string.Empty;

            Boolean EditPermit = false;

            //******************表单表信息Start****************
            if (IsApprovalForm)
            {
                DataTable dt = MicroDataTable.GetSingleDataTable("Forms");
                DataRow[] rows = dt.Select("FormID=" + FormID.toInt());
                if (rows.Length > 0)
                {
                    FormName = rows[0]["FormName"].toJsonTrim();
                    DeptID = rows[0]["DeptID"].toJsonTrim();
                    Description = rows[0]["Description"].toJsonTrim();
                }

                //系统编辑权限（暂时应用是否显示编辑流程）
                EditPermit = MicroAuth.CheckPermit(ModuleID, "3");
            }
            //******************表单表信息End****************


            DataTable _dt = null, _dt2 = null, _dt3 = null;

            //根据短表名得到长表名
            TableName = MicroPublic.GetTableName(ShortTableName);
            if (!string.IsNullOrEmpty(TableName))
            {
                switch (Action)
                {
                    case "add":  //在添加表单状态下显示该控件
                        ctlDisplay = " and ctlAddDisplay=1 ";
                        break;
                    case "modify":  //在修改表单状态下显示该控件
                        ctlDisplay = " and ctlModifyDisplay=1 ";
                        break;
                    case "view":   //在查看表单状态下显示该控件
                        ctlDisplay = " and ctlViewDisplay=1 ";
                        break;
                    case "close":  //结案时显示该控件
                        ctlDisplay = " and ctlAfterDisplay=1 ";
                        break;
                }

                //查询得到组别
                string _sql = "select ctlGroup from MicroTables where ParentID in (select TID from MicroTables where TabColName = @TableName) and Del=0 group by ctlGroup order by ctlGroup";
                SqlParameter[] _sp = { new SqlParameter("@TableName", SqlDbType.VarChar, 100) };
                _sp[0].Value = TableName;
                _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

                //再按组别进行查询
                string _sql2 = "select * from MicroTables where (TabColName = @TableName or ParentID in (select TID from MicroTables where TabColName = @TableName)) and Del=0 " + ctlDisplay + " order by Sort";
                SqlParameter[] _sp2 = { new SqlParameter("@TableName", SqlDbType.VarChar, 100) };
                _sp2[0].Value = TableName;
                _dt2 = MsSQLDbHelper.Query(_sql2, _sp2).Tables[0];

                //读取记录值，用于赋值给表单控件
                if ((Action == "add" || Action == "modify" || Action == "view" || Action == "close") && !string.IsNullOrEmpty(PrimaryKeyValue))
                    _dt3 = GetDataTable(TableName, PrimaryKeyValue);

                ctlFormStyle = _dt2.Select("ParentID=0", "ParentID,Sort")[0]["ctlFormStyle"].ToString();  //Global
                FormStyle = ctlFormStyle == "Box" ? "layui-form-pane" : string.Empty;

                var getFormItem = GetFormItem(Action, ModuleID, FormID, PrimaryKeyValue, TableName, _dt, _dt2, _dt3, FormType, IsApprovalForm, ShowButton);

                //string fluidContainer = "layui-fluid"; //layui-fluid=自动伸缩; layui-container=固定长度
                //htmlCode = "<div class=" + fluidContainer + ">";
                htmlCode += "<div class=\"layui-card\">";

                //在表单页面表头是否显示自定义流程按钮
                string CustomFlow = string.Empty;
                //表单类型为审批类型表单时并且拥有编辑权限（权限代码3）时显示
                if (IsApprovalForm && EditPermit)
                    CustomFlow = "<span style=\"position:absolute; right:20px;\"><a herf=\"javascript:; \" style=\"cursor:pointer;\" class=\"micro-click\" micro-mid=\"" + ModuleID + "\" micro-stn=\"" + ShortTableName + "\" data-type=\"GetWorkFlow\">【自定义流程】</a></span>";

                //是否显示头部
                if (ShowHeader)
                {
                    DataRow[] _rows2G = _dt2.Select("ParentID=0", "ParentID,Sort");

                    //不需要审批的表单（如系统表、部门表、用户表等），没有保存在Forms表中无法获取头部说明，暂时以Title作为说明
                    string Title = string.Empty;
                    if (!IsApprovalForm)
                        Title = _rows2G[0]["Title"].toStringTrim();

                    htmlCode += "<div class=\"layui-card-header  layuiadmin-card-header-auto \" style=\"margin-bottom:-20px; border-bottom:0px;\">";

                    //非view模式时显示表单的用途
                    if (Action != "view")
                    {
                        htmlCode += "<blockquote class=\"layui-elem-quote\">";
                        htmlCode += "<span class=\"ws-font-red\">" + Title + FormName + Description + CustomFlow + "</span>";
                        htmlCode += "</blockquote>";
                    }
                    //view模式时显示审批阶段
                    else
                    {
                        //从表单审批记录表获得审批记录作为步骤，在view模式时显示，如：申请->审批->受理->完成
                        if (IsApprovalForm)
                            htmlCode += GetFormWorkFlowStepsHtmlCode(FormID, PrimaryKeyValue);

                    }

                    htmlCode += "</div>";  //layui-card-header
                }

                htmlCode += "<div class=\"layui-card-body layui-form " + FormStyle + "\" lay-filter=\"micro-form\">";
                htmlCode += "<div class=\"layui-row\">";

                //***列1 Start***
                if ((Action == "view" || Action == "close") && IsApprovalForm)
                    htmlCode += "<div class=\"layui-col-xs6 layui-col-sm9 layui-col-md10 layui-col-lg10 ws-scrollbar\" style=\"overflow: auto;\">";
                else
                    htmlCode += "<div class=\"layui-col-xs12 layui-col-sm12 layui-col-md12 layui-col-lg12 \">";

                htmlCode += getFormItem.HtmlCode;

                //*************获取表单审批流程Start***********
                if (IsApprovalForm)
                {
                    var getWorkFlow = MicroWorkFlow.GetWorkFlow(Action, FormID, PrimaryKeyValue, ShortTableName);
                    if (Action != "view" && Action != "close")
                    {
                        jsFormFlowCode = getWorkFlow.JsXmSelectCode;
                        htmlCode += getWorkFlow.HtmlCode;  //显示审批流程
                    }
                }
                //*************获取表单审批流程End***********

                htmlCode += "</div>";  //layui-col-xs6 layui-col-sm3 layui-col-md1 layui-col-lg1
                //***列1 End***

                //***列2 Start*** 日志记录
                if ((Action == "view" || Action == "close") && IsApprovalForm)
                    htmlCode += GetFormApprovalLogsHtmlCode(FormID, PrimaryKeyValue);
                //***列2 End***

                htmlCode += "</div>";  //layui-row
                htmlCode += "</div>";  //layui-card-body

                htmlCode += "</div>"; //layui-card

                //获取js验证代码
                string _jsFormVerifyCode = string.Empty;
                _jsFormVerifyCode = getFormItem.JsFormVerifyCode;
                if (!string.IsNullOrEmpty(_jsFormVerifyCode))
                {
                    jsFormVerifyCode += "form.verify({";
                    jsFormVerifyCode += _jsFormVerifyCode.Substring(0, _jsFormVerifyCode.Length - 1);
                    jsFormVerifyCode += "});";
                }

                //获取Checkbox、Radio点选事件的js代码
                jsFormCtrlChangeCode += getFormItem.JsFormCtrlChangeCode;

                //获取扩展的JS代码
                jsFormExtendCode += getFormItem.JsFormExtendCode;

                //获取提交按钮js代码
                jsFormSubmitCode += GetFormSubmitCode(Action, ShortTableName, ModuleID, FormID, IsApprovalForm, IsRefresh);
            }
            else
                htmlCode = MicroPublic.GetFieldSet("错误提示 Error prompt", MicroPublic.GetMsg("DenyURLError"));


            var Code = new Code
            {
                HtmlCode = htmlCode,
                JsFormVerifyCode = jsFormVerifyCode,
                JsFormCtrlChangeCode = jsFormCtrlChangeCode,
                JsFormFlowCode = jsFormFlowCode,
                JsFormExtendCode = jsFormExtendCode,
                JsFormSubmitCode = jsFormSubmitCode,
                JsFormCode = jsFormVerifyCode + jsFormCtrlChangeCode + jsFormExtendCode + jsFormFlowCode + jsFormSubmitCode
            };

            return Code;
        }

        /// <summary>
        /// 生成FormItem层。按组别（ctlGroup）相同的生成一层
        /// </summary>
        /// <param name="Action">表单动作/提交动作</param>
        /// <param name="ShortTableName">短表名</param>
        /// <returns></returns>
        private static Code GetFormItem(string Action, string ModuleID, string FormID, string FormsID, string TableName, DataTable _dt, DataTable _dt2, DataTable _dt3, string FormType, Boolean IsApprovalForm, Boolean ShowButton)
        {
            string htmlCode = string.Empty, jsFormVerifyCode = string.Empty, jsFormCtrlChangeCode = string.Empty, jsFormExtendCode = string.Empty;
            Boolean ctlAddDisplayButton = false, ctlSaveDraftButton = false, ctlModifyDisplayButton = false;

            DataRow[] _rows = _dt.Select();
            DataRow[] _rows2G = _dt2.Select("ParentID=0", "ParentID,Sort");  //G=Global

            //按组进行循环查询，得到该组的内容
            foreach (DataRow _dr in _rows)
            {
                DataRow[] _rows2 = _dt2.Select("ParentID<>0 and ctlGroup=" + _dr["ctlGroup"].toInt() + " and Invalid=0", "Sort");

                if (_rows2.Count() > 0)
                {
                    string ctlFormItemStyle = string.Empty,  //个别
                    ctlColSpace = "layui-col-space5",  //同一组内该值一样，默认列宽
                    ctlGroupDescription = string.Empty;  //同一组内该值一样

                    //对于全局属性理想状态应该是从表名这一行进行取出相关值，但现在还没有 20191119注，今后必要性再考虑
                    ctlFormItemStyle = _rows2[0]["ctlFormItemStyle"].toStringTrim(); //个别
                    ctlColSpace = _rows2G[0]["ctlColSpace"].toStringTrim();  //Global
                    ctlGroupDescription = _rows2[0]["ctlGroupDescription"].toStringTrim();  //个别
                    ctlAddDisplayButton = _rows2G[0]["ctlAddDisplayButton"].toBoolean();  //Global
                    ctlSaveDraftButton = _rows2G[0]["ctlSaveDraftButton"].toBoolean(); //Global
                    ctlModifyDisplayButton = _rows2G[0]["ctlModifyDisplayButton"].toBoolean(); //Global

                    //显示组别标题块，当组别描述为空时则不显示
                    if (!string.IsNullOrEmpty(ctlGroupDescription))
                    {
                        htmlCode += "<fieldset class=\"layui-elem-field layui-field-title \">";
                        htmlCode += "<legend class=\"ws-font-green ws-font-size16\"> " + ctlGroupDescription + " </legend>";
                        htmlCode += "</fieldset >";
                    }

                    //组Item div
                    htmlCode += "<div class=\"layui-form-item layui-row " + ctlColSpace + "\" " + ctlFormItemStyle + ">";

                    var getFormItemInline = GetFormItemInline(Action, FormID, _dt2, _rows2, _dt3, FormType, IsApprovalForm);
                    htmlCode += getFormItemInline.HtmlCode;
                    jsFormVerifyCode += getFormItemInline.JsFormVerifyCode;
                    jsFormCtrlChangeCode += getFormItemInline.JsFormCtrlChangeCode;
                    jsFormExtendCode += getFormItemInline.JsFormExtendCode;

                    htmlCode += "</div>";
                }
            }

            //自定义代码
            htmlCode += _rows2G[0]["CustomHtmlCode"].ToString(); //带G的代表Global

            //在修改动作下得到记录的所有者UID(即表单申请者UID)，用于判断当前记录是否允许当前用户修改
            //在读取当前要修改的记录前还会判断一下该表是否存在UID字段（避免报错），通常系统表没有UID这个字段
            string OwnerUID = string.Empty;

            //表单提交时按钮是否显示提交按钮
            if (Action == "add")
            {
                if (ctlAddDisplayButton && ctlSaveDraftButton) //显示保存按钮和临时保存按钮
                    htmlCode += GetFormButtonCode(Action, ModuleID, FormID, FormsID, FormType, IsApprovalForm, OwnerUID, ShowButton, "SaveDraft");
                else if (ctlAddDisplayButton) //紧显示保存按钮
                    htmlCode += GetFormButtonCode(Action, ModuleID, FormID, FormsID, FormType, IsApprovalForm, OwnerUID, ShowButton, "Save");
                else if (ctlSaveDraftButton)  //紧显示临时保存按钮
                    htmlCode += GetFormButtonCode(Action, ModuleID, FormID, FormsID, FormType, IsApprovalForm, OwnerUID, ShowButton, "Draft");
            }

            //表单修改时按钮是否显示修改按钮
            if ((Action == "modify" || Action == "close") && ctlModifyDisplayButton)
            {

                //***在修改动作下，得到当前记录的UID Start***
                var getTableAttr = MicroDataTable.GetTableAttr(TableName);  //调用方法返回主键属性
                Boolean isUID = getTableAttr.IsUID;

                if (isUID)
                {
                    if (_dt3 != null && _dt3.Rows.Count > 0)
                        OwnerUID = _dt3.Rows[0]["UID"].ToString();
                }
                //***在修改动作下，得到当前记录的UID End***

                htmlCode += GetFormButtonCode(Action, ModuleID, FormID, FormsID, FormType, IsApprovalForm, OwnerUID, ShowButton);
            }

            var Code = new Code
            {
                HtmlCode = htmlCode,
                JsFormVerifyCode = jsFormVerifyCode,
                JsFormCtrlChangeCode = jsFormCtrlChangeCode,
                JsFormExtendCode = jsFormExtendCode
            };

            return Code;
        }

        /// <summary>
        /// 生成FormItem的行内层
        /// </summary>
        /// <param name="_dt2"></param>
        /// <param name="_rows2"></param>
        /// <returns></returns>
        private static Code GetFormItemInline(string Action, string FormID, DataTable _dt2, DataRow[] _rows2, DataTable _dt3, string FormType, Boolean IsApprovalForm)
        {
            string htmlCode = string.Empty, jsFormVerifyCode = string.Empty, jsFormCtrlChangeCode = string.Empty, jsFormExtendCode = string.Empty;
            string ctlFormStyle = _dt2.Select("ParentID=0", "ParentID,Sort")[0]["ctlFormStyle"].ToString(); //Global

            foreach (DataRow _dr2 in _rows2)
            {
                string ctlPrefix = _dr2["ctlPrefix"].ToString(),
                    TabColName = _dr2["TabColName"].ToString(),
                    DataType = _dr2["DataType"].ToString(),
                    ctlTitle = _dr2["ctlTitle"].ToString(),
                    ctlTitleStyle = _dr2["ctlTitleStyle"].ToString(),
                    ctlDescription = _dr2["ctlDescription"].ToString(),
                    ctlDescriptionDisplayPosition = _dr2["ctlDescriptionDisplayPosition"].ToString(),
                    ctlDescriptionDisplayMode = _dr2["ctlDescriptionDisplayMode"].ToString(),
                    ctlDescriptionStyle = _dr2["ctlDescriptionStyle"].ToString(),
                    ctlVerify2 = _dr2["ctlVerify2"].ToString(),
                    ctlDisplayAsterisk = _dr2["ctlDisplayAsterisk"].ToString(),
                    ctlInlineCss = _dr2["ctlInlineCss"].ToString(),
                    ctlInlineStyle = _dr2["ctlInlineStyle"].ToString(),
                    ctlOffset = _dr2["ctlOffset"].ToString(),
                    ctlXS = _dr2["ctlXS"].ToString(),
                    ctlSM = _dr2["ctlSM"].ToString(),
                    ctlMD = _dr2["ctlMD"].ToString(),
                    ctlLG = _dr2["ctlLG"].ToString(),
                    ctlInputInline = _dr2["ctlInputInline"].ToString(),
                    ctlInputInlineStyle = _dr2["ctlInputInlineStyle"].ToString(),
                    ctlTypes = _dr2["ctlTypes"].ToString(),
                    //ctlFormStyle = _dr2["ctlFormStyle"].ToString(),
                    startPane = string.Empty, //当表单为方框风格时对控件为Checkbox和Radio加上方框层
                    endPane = string.Empty,  //方框结束标记
                    divCss = string.Empty,
                    showAsterisk = string.Empty, //星号
                    cssAsterisk = string.Empty,  //星号红色或灰色
                    _ctlDescription = string.Empty,  //描述直显or提示
                    beforeDescription = string.Empty,  //描述在前面显示，前面时赋值给这个
                    afterDescription = string.Empty,   //描述在后面显示，后面时赋值给这个
                    labelDescription = string.Empty,   //描述在Label标签上显示，时赋值给这个
                    layuiHide = string.Empty;

                Boolean ctlViewDisplayLabel = _dr2["ctlViewDisplayLabel"].toBoolean();  //在Action=View模式下，用div显示内容

                //在Label内容后面显示冒号
                string Colon = string.Empty;
                if (Action == "view" && ctlViewDisplayLabel)
                    Colon = "：";


                //值不为空时在后面加上空格
                ctlInlineCss = string.IsNullOrEmpty(ctlInlineCss) == true ? "layui-inline " : ctlInlineCss + " ";
                ctlOffset = string.IsNullOrEmpty(ctlOffset) == true ? string.Empty : ctlOffset + " ";
                ctlXS = string.IsNullOrEmpty(ctlXS) == true ? string.Empty : ctlXS + " ";
                ctlSM = string.IsNullOrEmpty(ctlSM) == true ? string.Empty : ctlSM + " ";
                ctlMD = string.IsNullOrEmpty(ctlMD) == true ? string.Empty : ctlMD + " ";
                ctlLG = string.IsNullOrEmpty(ctlLG) == true ? string.Empty : ctlLG + " ";

                //为空时显示默认值
                ctlInputInline = string.IsNullOrEmpty(ctlInputInline) == true ? "layui-input-inline " : ctlInputInline + " ";

                //是Checkbox层或者是Radio层时，向层添加divCss，为获取选中的值提供需要的容器名称 ,同时追加方框
                if (ctlTypes == "CheckBox" || ctlTypes == "Radio")
                {
                    divCss = "css" + ctlPrefix + TabColName;

                    //当表单风格为方框风格时为Checkbox和Radio加上方框样式
                    if (ctlFormStyle == "Box")
                    {
                        startPane = "<div style=\"border:1px solid #dddddd !important; width:180px; height:36px; border-radius:2px;\">";
                        endPane = "</div>";
                    }
                }

                //显示星号的样式，必填项时为红色否则灰色
                cssAsterisk = ctlVerify2 == "" ? "ws-want-asterisk" : "ws-must-asterisk";

                //当描述显示位置不为空时显示描述内容
                if (!string.IsNullOrEmpty(ctlDescriptionDisplayPosition) && Action != "view")
                {
                    //直显或鼠标移入提示，默认False 移入提示
                    if (ctlDescriptionDisplayMode == "False")
                        _ctlDescription = "<i class=\"layui-icon layui-icon-about\" lay-tips=\"" + ctlDescription + "\"></i>";
                    else
                        _ctlDescription = ctlDescription;

                    //描述的显示位置，前面、后面或Label上显示，默认after 后面显示
                    if (ctlDescriptionDisplayPosition == "before")
                        beforeDescription = _ctlDescription;  //前面
                    else if (ctlDescriptionDisplayPosition == "after")
                        afterDescription = _ctlDescription;  //后面
                    else if (ctlDescriptionDisplayPosition == "label")
                        labelDescription = "lay-tips=\"" + ctlDescription + "\"";  //label标签上
                }

                //当控件为Hidden时对当前div进行隐藏
                if (ctlTypes == "Hidden")
                    layuiHide = "layui-hide ";

                //星号, ctlDisplayAsterisk == "True"，并且Action!="view" 且 !ctlViewDisplayLabel
                if (ctlDisplayAsterisk == "True" && Action != "view")  //显示星号
                    showAsterisk = "<i class=\"" + cssAsterisk + layuiHide + "\">&#42;</i> ";

                var getFormItemInlineCtrl = GetFormItemInlineCtrl(Action, FormID, _dt2, _dr2["TID"].ToString(), _dt3, FormType, IsApprovalForm);
                //以下属性为空时不显示该层
                if (!string.IsNullOrEmpty(ctlXS) || !string.IsNullOrEmpty(ctlSM) || !string.IsNullOrEmpty(ctlMD) || !string.IsNullOrEmpty(ctlLG))
                    ctlInlineCss = ctlXS + ctlSM + ctlMD + ctlLG;

                // !string.IsNullOrEmpty(layuiHide) || !string.IsNullOrEmpty(ctlInlineCss) || !string.IsNullOrEmpty(ctlInlineStyle))
                htmlCode += "<div class=\"" + ctlInlineCss + layuiHide + "\" " + ctlInlineStyle + ">";

                //控件标题不为空时显示标题label
                if (!string.IsNullOrEmpty(ctlTitle))
                {
                    //显示一个空的label
                    if (ctlTitle == "space")
                        htmlCode += "<label class=\"layui-form-label " + layuiHide + "\" " + ctlTitleStyle + " " + labelDescription + ">" + showAsterisk + beforeDescription + "</label>";
                    else
                        htmlCode += "<label class=\"layui-form-label " + layuiHide + "\" " + ctlTitleStyle + " " + labelDescription + ">" + showAsterisk + _dr2["ctlTitle"].ToString() + beforeDescription + Colon + "</label>";
                }

                htmlCode += "<div class=\"" + ctlInputInline + layuiHide + divCss + "\" " + ctlInputInlineStyle + ">";
                htmlCode += startPane;
                htmlCode += getFormItemInlineCtrl.HtmlCode;
                htmlCode += endPane;

                //数据字段为时间类型且控件类型为input=Date类型时显示在input上日期图标
                if (Action != "view" && DataType == "datetime" && ctlPrefix != "hid" && (ctlTypes == "Date" || ctlTypes == "Time" || ctlTypes == "DateTime" || ctlTypes == "DateWeek" || ctlTypes == "DateWeekTime"))
                    htmlCode += "<i id=\"icondate" + ctlPrefix + TabColName + "\" class=\"layui-icon layui-icon-date " + layuiHide + "\" style=\"position: absolute;top:8px;right: 8px;\"></i>";

                htmlCode += "</div>";

                //当描述显示位置不为空时，并且描述在控件之后显示时，才显示描述层
                if (!string.IsNullOrEmpty(ctlDescriptionDisplayPosition) && !string.IsNullOrEmpty(afterDescription))
                {
                    htmlCode += "<div class=\"layui-form-mid layui-word-aux " + layuiHide + "\" " + ctlDescriptionStyle + ">";
                    htmlCode += afterDescription;
                    htmlCode += "</div>";
                }

                htmlCode += "</div>";

                jsFormVerifyCode += getFormItemInlineCtrl.JsFormVerifyCode;
                jsFormCtrlChangeCode += getFormItemInlineCtrl.JsFormCtrlChangeCode;
                jsFormExtendCode += getFormItemInlineCtrl.JsFormExtendCode;
            }

            var Code = new Code
            {
                HtmlCode = htmlCode,
                JsFormVerifyCode = jsFormVerifyCode,
                JsFormCtrlChangeCode = jsFormCtrlChangeCode,
                JsFormExtendCode = jsFormExtendCode
            };

            return Code;
        }

        /// <summary>
        /// 生成FormItemInline行内控件元素
        /// </summary>
        /// <param name="_dt2"></param>
        /// <param name="TID"></param>
        /// <returns></returns>
        private static Code GetFormItemInlineCtrl(string Action, string FormID, DataTable _dt2, string TID, DataTable _dt3, string FormType, Boolean IsApprovalForm)
        {
            #region
            string htmlCode = string.Empty, jsFormVerifyCode = string.Empty, jsFormCtrlChangeCode = string.Empty, jsFormExtendCode = string.Empty;

            string TabColName = string.Empty, TableName = string.Empty, ShortTableName = string.Empty, PrimaryKey = string.Empty, primaryKeyName = string.Empty, DataType = string.Empty, Invalid = string.Empty, ctlPrimaryKey = string.Empty, ctlFormStyle = string.Empty, ctlTitle = string.Empty, ctlTitleStyle = string.Empty, ctlTypes = string.Empty, ctlPrefix = string.Empty, ctlStyle = string.Empty, ctlCharLength = string.Empty, ctlPlaceholder = string.Empty, ctlReceiveName = string.Empty, ctlDescription = string.Empty, ctlDescriptionDisplayPosition = string.Empty, ctlDescriptionDisplayMode = string.Empty, ctlDescriptionStyle = string.Empty, ctlExtendJSCode = string.Empty, ctlFilter = string.Empty, ctlVerify = string.Empty, ctlVerify2 = string.Empty, xmVerify = string.Empty, ctlDisplayAsterisk = string.Empty, ctlVerifyCustomFunction = string.Empty, ctlValue = string.Empty, ctlDefaultValue = string.Empty, ctlSourceTable = string.Empty, ctlTextName = string.Empty, ctlTextValue = string.Empty, ctlCheckboxSkin = string.Empty, ctlSwitchText = string.Empty, ctlGroup = string.Empty, ctlGroupDescription = string.Empty, ctlFormItemStyle = string.Empty, ctlInlineCss = string.Empty, ctlInlineStyle = string.Empty, ctlColSpace = string.Empty, ctlOffset = string.Empty, ctlXS = string.Empty, ctlSM = string.Empty, ctlMD = string.Empty, ctlLG = string.Empty, ctlInputInline = string.Empty, ctlInputInlineStyle = string.Empty, labText = string.Empty;

            string wsFontRed = string.Empty;

            Boolean tbMainSub = false;
            Boolean ctlViewDisplayLabel = false;


            //在修改表单时给控件赋值，这个值是根据当前需要修改的数据从数据表中读取出来的（Where TableName and PrimaryKeyValue）
            string Value = string.Empty;

            DataRow[] _rows2 = _dt2.Select("ParentID<>0 and TID=" + TID.toInt());
            DataRow[] _rows2G = _dt2.Select("ParentID=0", "ParentID,Sort");

            TableName = _rows2G[0]["TabColName"].ToString(); //Global
            ShortTableName = _rows2G[0]["ShortTableName"].toStringTrim(); //Global
            tbMainSub = _rows2G[0]["tbMainSub"].toBoolean(); //Global

            var GetTableAttr = MicroDataTable.GetTableAttr(TableName);  //获得主键字段名称
            primaryKeyName = GetTableAttr.PrimaryKeyName;  //主键字段名称

            TabColName = _rows2[0]["TabColName"].ToString();
            PrimaryKey = _rows2[0]["PrimaryKey"].ToString();
            //primaryKeyName = _rows2PK[0]["TabColName"].toStringTrim();  //主键名称
            DataType = _rows2[0]["DataType"].toStringTrim();
            Invalid = _rows2[0]["Invalid"].toStringTrim();
            ctlPrimaryKey = _rows2[0]["ctlPrimaryKey"].toStringTrim();
            //ctlFormStyle = _rows2[0]["ctlFormStyle"].ToString();
            ctlFormStyle = _rows2G[0]["ctlFormStyle"].ToString(); //Global
            ctlViewDisplayLabel = _rows2[0]["ctlViewDisplayLabel"].toBoolean();
            ctlTitle = _rows2[0]["ctlTitle"].toStringTrim();
            ctlTitleStyle = _rows2[0]["ctlTitleStyle"].toStringTrim();
            ctlTypes = _rows2[0]["ctlTypes"].toStringTrim();
            ctlPrefix = _rows2[0]["ctlPrefix"].toStringTrim();
            ctlStyle = _rows2[0]["ctlStyle"].toStringTrim();
            ctlCharLength = _rows2[0]["ctlCharLength"].toStringTrim();
            ctlPlaceholder = _rows2[0]["ctlPlaceholder"].toStringTrim();
            ctlReceiveName = _rows2[0]["ctlReceiveName"].toStringTrim();
            ctlDescription = _rows2[0]["ctlDescription"].toStringTrim();
            ctlDescriptionDisplayPosition = _rows2[0]["ctlDescriptionDisplayPosition"].toStringTrim();
            ctlDescriptionDisplayMode = _rows2[0]["ctlDescriptionDisplayMode"].toStringTrim();
            ctlDescriptionStyle = _rows2[0]["ctlDescriptionStyle"].toStringTrim();
            ctlExtendJSCode = _rows2[0]["ctlExtendJSCode"].toStringTrim();
            ctlFilter = _rows2[0]["ctlFilter"].toStringTrim();
            ctlVerify = _rows2[0]["ctlVerify"].toStringTrim();  //自定义验证
            ctlVerify2 = _rows2[0]["ctlVerify2"].toStringTrim();  //layui验证
            xmVerify = ctlVerify2.Contains("required") == true ? "required" : "";  //用于验证XmSelect控件是否为必填
            ctlDisplayAsterisk = _rows2[0]["ctlDisplayAsterisk"].ToString();
            ctlVerifyCustomFunction = _rows2[0]["ctlVerifyCustomFunction"].toStringTrim();
            ctlValue = _rows2[0]["ctlValue"].toStringTrim();
            ctlDefaultValue = _rows2[0]["ctlDefaultValue"].toStringTrim();
            ctlSourceTable = _rows2[0]["ctlSourceTable"].toStringTrim();
            ctlTextName = _rows2[0]["ctlTextName"].toStringTrim();
            ctlTextValue = _rows2[0]["ctlTextValue"].toStringTrim();
            ctlCheckboxSkin = _rows2[0]["ctlCheckboxSkin"].toStringTrim();
            ctlSwitchText = _rows2[0]["ctlSwitchText"].toStringTrim();
            ctlGroup = _rows2[0]["ctlGroup"].toStringTrim();
            ctlGroupDescription = _rows2[0]["ctlGroupDescription"].toStringTrim();
            ctlFormItemStyle = _rows2[0]["ctlFormItemStyle"].toStringTrim();
            ctlInlineCss = _rows2[0]["ctlInlineCss"].toStringTrim();
            ctlInlineStyle = _rows2[0]["ctlInlineStyle"].toStringTrim();
            //ctlColSpace = _rows2[0]["ctlColSpace"].ToString();
            ctlColSpace = _rows2G[0]["ctlColSpace"].toStringTrim(); //Global
            ctlOffset = _rows2[0]["ctlOffset"].toStringTrim();
            ctlXS = _rows2[0]["ctlXS"].toStringTrim();
            ctlSM = _rows2[0]["ctlSM"].toStringTrim();
            ctlMD = _rows2[0]["ctlMD"].toStringTrim();
            ctlLG = _rows2[0]["ctlLG"].toStringTrim();
            ctlInputInline = _rows2[0]["ctlInputInline"].toStringTrim();
            ctlInputInlineStyle = _rows2[0]["ctlInputInlineStyle"].toStringTrim();

            if (string.IsNullOrEmpty(ctlFilter))
                ctlFilter = ctlPrefix + TabColName;

            //Value应用在生成的控件的值 <input type="text" value=Value />
            Value = ctlDefaultValue;
            //控件类型不是以下3种类型时，有控件值的优先于默认值
            if (ctlTypes != "Select" && ctlTypes != "CheckBox" && ctlTypes != "Radio")
                Value = string.IsNullOrEmpty(ctlValue) != true ? MicroPublic.GetBuiltinFunction(Action, ctlValue, "", ShortTableName, TableName, FormID) : Value;  //这里的TableName和FormID配合内置函数GetFormNumber(控件值或默认值)，返回FormNumber， ShortTableName配合GetFormStateCode，返回表单状态
            else
                Value = MicroPublic.GetBuiltinFunction(Action, Value, "", ShortTableName, TableName, FormID);

            if (TabColName != "DateModified")  //DateModified=修改时间，TabColName!= "DateModified"时才读取需要更新记录的值，否则还是获取设定的值
                if (_dt3 != null && _dt3.Rows.Count > 0)
                    Value = _dt3.Rows[0][TabColName].toStringTrim();

            //得到Value值后进行相关格式化，日期格式为datetime并且控件值为GetDate时
            if (DataType == "datetime")
            {
                if (Action == "view")
                {
                    //Value = Value.toDateWeekTime();
                    switch (ctlTypes)
                    {
                        case "Date":
                            Value = Value.toDateFormat(ctlPlaceholder);
                            break;
                        case "Time":
                            Value = Value.toDateFormat(!string.IsNullOrEmpty(ctlPlaceholder) ? ctlPlaceholder : "HH:mm:ss");
                            break;
                        case "DateTime":
                            Value = Value.toDateFormat(!string.IsNullOrEmpty(ctlPlaceholder) ? ctlPlaceholder : "yyyy-MM-dd HH:mm:ss");
                            break;
                        case "DateWeek":
                            Value = Value.toDateWeek();
                            break;
                        case "DateWeekTime":
                            Value = Value.toDateWeekTime();
                            break;
                    }
                }
                else
                {
                    if (ctlValue == "GetDate")
                        Value = Value.toDateFormat(MicroPublic.GetMicroInfo("DateFormat"));
                    else
                    {
                        if (!string.IsNullOrEmpty(ctlPlaceholder))
                            Value = Value.toDateFormat(ctlPlaceholder);
                    }
                }
            }

            if (Action == "add")  //新增记录时
            {
                //*****当字段为排序字段Sort时，获取最大值并+1 返回给控件*****
                //DataRow[] _rows2G = _dt2.Select("ParentID=0", "ParentID,Sort");  //G=Global
                if (TabColName == "Sort")
                {
                    int intCurrentSort = MsSQLDbHelper.GetMaxNumber("Sort", TableName); //返回最大值
                    intCurrentSort = intCurrentSort == 0 ? 1 : intCurrentSort + 1;  //为空值时返回1，否则+1
                    Value = intCurrentSort.ToString();
                }

            }
            else if (Action == "modify" || Action == "view" || Action == "close")
            {
                //表单状态红色显示
                //TabColName == "FormState"说明该表单是审批类型表单，即代表会有StateCode
                if (TabColName == "FormState")
                {
                    int StateCode = 0;
                    if (_dt3 != null && _dt3.Rows.Count > 0)
                        StateCode = _dt3.Rows[0]["StateCode"].toInt();

                    wsFontRed = StateCode == 100 ? "ws-font-green2" : "ws-font-red";
                }

                //如果为修改状态且数据类型为Datetime且ctlPlaceholder不为空时，格式化为ctlPlaceholder的格式
                if (Action == "modify" && DataType == "datetime" && !string.IsNullOrEmpty(ctlPlaceholder))
                    Value = Value.toDateFormat();

                //如果为结案状态且Value为空时，获取控件值，如果控件值为空则尝试获取默认值
                if (Action == "close" && string.IsNullOrEmpty(Value))
                    Value = MicroPublic.GetBuiltinFunction(Action, ctlValue, "", ShortTableName, TableName, FormID);
            }


            //当控件类型为以下几种日期时间类型时它们都是生成Text控件，所以变更为Text，而它们本身的作用是在生成记录时用来格式化日期的格式
            if (ctlTypes == "Date" || ctlTypes == "Time" || ctlTypes == "DateTime" || ctlTypes == "DateWeek" || ctlTypes == "DateWeekTime")
                ctlTypes = "Text";

            #endregion
            switch (ctlTypes)
            {
                //Text【文本框】，Password【密码框】，Hidden【隐藏域】，Select【下拉菜单】，CheckBox【复选框】，Radio【单选】，Textarea【多行文本框】，ImgUpload【图片上传】，FileUpload【文件上传】

                #region Text
                case "Text":

                    //lay-verify赋值
                    string textVerify = string.Empty;
                    if (!string.IsNullOrEmpty(ctlVerify2))
                        textVerify = ctlVerify2 + "|" + ctlPrefix + TabColName + "Length";  //|ctlPrefix+TabColName+Length用于检测input输入长度
                    else
                        textVerify = ctlPrefix + TabColName + "Length";  //|ctlPrefix+TabColName+Length用于检测input输入长度

                    htmlCode = "<input type=\"text\" id=\"" + ctlPrefix + TabColName + "\" name=\"" + ctlPrefix + TabColName + "\" value=\"" + Value + "\" placeholder=\"" + ctlPlaceholder + "\" lay-verify=\"" + textVerify + "\" lay-reqText=\"" + ctlTitle + "不能为空<br/>This cannot be empty\" autocomplete=\"off\" class=\"layui-input " + ctlVerify + "\" " + ctlStyle + ">";

                    //当字符长度不为空时，生成长度验证js代码
                    if (!string.IsNullOrEmpty(ctlCharLength) && ctlCharLength != "0")
                    {
                        if (ctlCharLength.Contains(","))  //验证长度在某范围之间
                        {
                            string[] v = ctlCharLength.Split(',');
                            jsFormVerifyCode = ctlPrefix + TabColName + "Length: [/^.{" + ctlCharLength + "}$/,'" + ctlTitle + "长度只允许" + v[0].toStringTrim() + "到" + v[1].toStringTrim() + "位之间<br/>This length is only allowed between " + v[0].toStringTrim() + " and " + v[1].toStringTrim() + " bits.'],";
                        }
                        else   //验证长度不能超过指定值
                            jsFormVerifyCode = ctlPrefix + TabColName + "Length: function (value, item) { if (value.length > " + ctlCharLength + ") { return '" + ctlTitle + "长度不能超过" + ctlCharLength + "位<br/> This length can not exceed " + ctlCharLength + " bits';}},";
                    }

                    //在查看表单的情况下是以Label形式显示，则以Label显示，否则以控件显示
                    if (Action == "view" && ctlViewDisplayLabel)
                        htmlCode = "<div id=\"div" + ctlPrefix + TabColName + "\" class=\"layui-form-mid layui-word-aux " + wsFontRed + "\">" + Value + "</div>";

                    //控件扩展JS代码
                    if (!string.IsNullOrEmpty(ctlExtendJSCode))
                        jsFormExtendCode += ctlExtendJSCode;

                    //自定义函数，当自定义函数不为空时追加自定义函数
                    if (!string.IsNullOrEmpty(ctlVerifyCustomFunction))
                        jsFormVerifyCode += ctlVerifyCustomFunction + ",";

                    break;
                #endregion

                #region Password
                case "Password":

                    //lay-verify赋值
                    string passVerify = string.Empty;
                    if (!string.IsNullOrEmpty(ctlVerify2))
                        passVerify = ctlVerify2 + "|" + ctlPrefix + TabColName + "Length";  //|ctlPrefix+TabColName+Length用于检测input输入长度
                    else
                        passVerify = ctlPrefix + TabColName + "Length";  //|ctlPrefix+TabColName+Length用于检测input输入长度

                    htmlCode = "<input type=\"password\" id=\"" + ctlPrefix + TabColName + "\" name=\"" + ctlPrefix + TabColName + "\" placeholder=\"" + ctlPlaceholder + "\" lay-verify=\"" + passVerify + "\" lay-reqText=\"" + ctlTitle + "不能为空<br/>This cannot be empty\" autocomplete=\"off\" class=\"layui-input " + ctlVerify + "\" " + ctlStyle + ">";

                    //生成验证js代码
                    if (!string.IsNullOrEmpty(ctlCharLength) && ctlCharLength != "0")
                    {
                        if (ctlCharLength.Contains(","))
                        {
                            string[] v = ctlCharLength.Split(',');
                            jsFormVerifyCode = ctlPrefix + TabColName + "Length: [/^.{" + ctlCharLength + "}$/,'" + ctlTitle + "长度只允许" + v[0].toStringTrim() + "到" + v[1].toStringTrim() + "位之间<br/>This length is only allowed between " + v[0].toStringTrim() + " and " + v[1].toStringTrim() + " bits.'],";
                        }
                        else
                            jsFormVerifyCode = ctlPrefix + TabColName + "Length: function (value, item) { if (value.length > " + ctlCharLength + ") { return '" + ctlTitle + "长度不能超过" + ctlCharLength + "位<br/> This length can not exceed " + ctlCharLength + " bits';}},";
                    }

                    //在查看表单的情况下是以Label形式显示，则以Label显示，否则以控件显示
                    if (Action == "view" && ctlViewDisplayLabel)
                        htmlCode = "<div id=\"div" + ctlPrefix + TabColName + "\" class=\"layui-form-mid layui-word-aux\">" + Value + "</div>";

                    //控件扩展JS代码
                    if (!string.IsNullOrEmpty(ctlExtendJSCode))
                        jsFormExtendCode += ctlExtendJSCode;

                    //自定义函数，当自定义函数不为空时追加自定义函数
                    if (!string.IsNullOrEmpty(ctlVerifyCustomFunction))
                        jsFormVerifyCode += ctlVerifyCustomFunction + ",";

                    break;
                #endregion

                #region Hidden
                case "Hidden":

                    htmlCode = "<input type=\"text\" id=\"" + ctlPrefix + TabColName + "\" name=\"" + ctlPrefix + TabColName + "\" value=\"" + Value + "\" placeholder=\"" + ctlPlaceholder + "\" lay-verify=\"" + ctlVerify2 + "\" autocomplete=\"off\" class=\"layui-input layui-hide " + ctlVerify + "\" " + ctlStyle + ">";

                    //控件扩展JS代码
                    if (!string.IsNullOrEmpty(ctlExtendJSCode))
                        jsFormExtendCode += ctlExtendJSCode;

                    //自定义函数，当自定义函数不为空时追加自定义函数
                    if (!string.IsNullOrEmpty(ctlVerifyCustomFunction))
                        jsFormVerifyCode += ctlVerifyCustomFunction + ",";

                    break;
                #endregion

                #region ColorPicker
                case "ColorPicker":

                    htmlCode = "<div id=\"div" + ctlPrefix + TabColName + "\"></div>";

                    htmlCode += "<input type=\"text\" id=\"" + ctlPrefix + TabColName + "\" name=\"" + ctlPrefix + TabColName + "\" value=\"" + Value + "\" placeholder=\"" + ctlPlaceholder + "\" lay-verify=\"" + ctlVerify2 + "\" autocomplete=\"off\" class=\"layui-input layui-hide " + ctlVerify + "\" " + ctlStyle + ">";

                    //控件扩展JS代码
                    jsFormExtendCode = "  colorpicker.render({elem: '#div" + ctlPrefix + TabColName + "' ,color: '" + Value + "' ,predefine: true ,alpha: true ,done: function(color){ $('#" + ctlPrefix + TabColName + "').val(color); color || this.change(color);} }); ";

                    if (!string.IsNullOrEmpty(ctlExtendJSCode))
                        jsFormExtendCode += ctlExtendJSCode;

                    //自定义函数，当自定义函数不为空时追加自定义函数
                    if (!string.IsNullOrEmpty(ctlVerifyCustomFunction))
                        jsFormVerifyCode += ctlVerifyCustomFunction + ",";

                    break;
                #endregion

                #region Radio
                case "Radio":

                    if (!string.IsNullOrEmpty(ctlValue))   //有控件值时直接从控件值生成控件，没有时从指定表读取记录生成控件
                    {
                        string[] ctlValueArray = ctlValue.Split(',');  //根据控件值生成Radio
                        for (int i = 0; i < ctlValueArray.Length; i++)
                        {
                            //********checked="checked"*********
                            //在新增或修改表单情况下读取Value值与当前控件进行匹配，匹配的情况下让其选中
                            //**注**【在新增表单下该值赋予的是控件默认值ctlDefaultValue，要在修改表单下该值赋予的是该条记录的记录值】
                            string Checked = string.Empty;
                            if (!string.IsNullOrEmpty(Value))
                            {
                                string[] ValueArray = Value.Split(',');
                                for (int j = 0; j < ValueArray.Length; j++)
                                {
                                    if (!string.IsNullOrEmpty(ctlValueArray[i].toStringTrim()) && !string.IsNullOrEmpty(ValueArray[j].toStringTrim()))
                                    {
                                        if ((ctlValueArray[i].toStringTrim().Contains(":") == true ? ctlValueArray[i].toStringTrim().Split(':')[1] : ctlValueArray[i].toStringTrim()) == ValueArray[j].toStringTrim())
                                            Checked = "checked=\"checked\"";
                                    }
                                }
                            }
                            //********checked="checked"*********

                            htmlCode += "<input type=\"radio\" id=\"" + ctlPrefix + "dio" + TabColName + i.ToString() + "\" name=\"" + ctlPrefix + "dio" + TabColName + "\" title=\"" + (ctlValueArray[i].toStringTrim().Contains(":") == true ? ctlValueArray[i].toStringTrim().Split(':')[0] : ctlValueArray[i].toStringTrim()) + "\" value=\"" + (ctlValueArray[i].toStringTrim().Contains(":") == true ? ctlValueArray[i].toStringTrim().Split(':')[1] : ctlValueArray[i].toStringTrim()) + "\" lay-filter=\"" + ctlFilter + "\" " + ctlStyle + " " + Checked + ">";
                        }
                    }
                    else  //没有控件值时从指定表中读取数据生成控件
                    {
                        if (!string.IsNullOrEmpty(ctlSourceTable))
                        {
                            var getTableAttr = MicroDataTable.GetTableAttr(ctlSourceTable);  //获得表的相关属性
                            string OrderBy = getTableAttr.OrderBy;  //得到该表的Order by语句

                            string _sql = "select " + ctlTextName + "," + ctlTextValue + " from " + ctlSourceTable + " where Invalid=0 and Del=0 " + OrderBy + "";
                            DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];


                            if (_dt != null && _dt.Rows.Count > 0)
                            {
                                for (int i = 0; i < _dt.Rows.Count; i++)  //生成Radio
                                {
                                    //********checked="checked"*********
                                    //在新增或修改表单情况下读取Value值与当前控件进行匹配，匹配的情况下让其选中
                                    //**注**【在新增表单下该值赋予的是控件默认值ctlDefaultValue，要在修改表单下该值赋予的是该条记录的记录值】
                                    string Checked = string.Empty;
                                    if (!string.IsNullOrEmpty(Value))
                                    {
                                        string[] ValueArray = Value.Split(',');
                                        for (int j = 0; j < ValueArray.Length; j++)
                                        {
                                            if (!string.IsNullOrEmpty(_dt.Rows[i][ctlTextValue].ToString()) && !string.IsNullOrEmpty(ValueArray[j].toStringTrim()))
                                            {
                                                if (_dt.Rows[i][ctlTextValue].ToString() == ValueArray[j].toStringTrim())
                                                    Checked = "checked=\"checked\"";
                                            }
                                        }
                                    }
                                    //********checked="checked"*********

                                    htmlCode += "<input type=\"radio\" id=\"" + ctlPrefix + "dio" + TabColName + i.ToString() + "\" name=\"" + ctlPrefix + "dio" + TabColName + "\" title=\"" + _dt.Rows[i][ctlTextName].ToString() + "\" value=\"" + _dt.Rows[i][ctlTextValue].ToString() + "\" lay-filter=\"" + ctlFilter + "\" " + ctlStyle + " " + Checked + ">";
                                }
                            }
                        }
                    }

                    //同时生成隐藏文本框，用于存放Checkbox选中的值，便于提交和接收
                    htmlCode += "<input type=\"text\" id=\"" + ctlPrefix + TabColName + "\" name=\"" + ctlPrefix + TabColName + "\" placeholder=\"" + ctlPlaceholder + "\" lay-verify=\"" + ctlVerify2 + "\" lay-reqText=\"" + ctlTitle + "必须选择一个<br/>This item must choose one of them\" autocomplete=\"off\" class=\"layui-input layui-hide " + ctlVerify + "\" value=\"" + Value + "\"  readonly=\"readonly\">";

                    //在查看表单的情况下并且是显示Label形式，则已Label显示
                    if (Action == "view" && ctlViewDisplayLabel)
                        htmlCode = "<div  id=\"div" + ctlPrefix + TabColName + "\" class=\"layui-form-mid layui-word-aux\">" + Value + "</div>";

                    //生成Radio点选操作的js代码，把选中值传入input Text
                    jsFormCtrlChangeCode = "form.on('radio(" + ctlFilter + ")', function (data) {$('#" + ctlPrefix + TabColName + "').val(data.value);});";

                    //控件扩展JS代码
                    //*注：控件为Radio情况下，如果ctlExtendJSCode不为空时把该值赋给jsFormCtrlChangeCode，即采用自定义的代码，因为同一个form.on('radio(Filter)不能同时存在两个
                    if (!string.IsNullOrEmpty(ctlExtendJSCode))
                        jsFormCtrlChangeCode = ctlExtendJSCode;

                    break;

                #endregion

                #region CheckBox
                case "CheckBox":

                    string laySkin = string.Empty, layText = string.Empty;

                    if (!string.IsNullOrEmpty(ctlCheckboxSkin))  //读取skin样式
                        laySkin = " lay-skin=\"" + ctlCheckboxSkin + "\" ";

                    if (ctlCheckboxSkin == "switch")  //Checkbox为开关风格时显示开关风格的文本，否则显示空
                        layText = " lay-text=\"" + ctlSwitchText + "\"";  //显示开关文本

                    if (!string.IsNullOrEmpty(ctlValue))  //有控件值时直接从控件值生成控件，没有时从指定表读取记录生成控件
                    {
                        string[] ctlValueArray = ctlValue.Split(',');  //根据控件值生成CheckBox 
                        for (int i = 0; i < ctlValueArray.Length; i++)
                        {
                            //********checked="checked"*********
                            //在新增或修改表单情况下读取Value值与当前控件进行匹配，匹配的情况下让其选中
                            //**注**【在新增表单下该值赋予的是控件默认值ctlDefaultValue，要在修改表单下该值赋予的是该条记录的记录值】
                            string Checked = string.Empty;
                            try
                            {
                                //读取数据值Value给当前控件进行匹配，匹配的情况下让其选中
                                if (!string.IsNullOrEmpty(Value))
                                {
                                    string[] ValueArray = Value.Split(',');
                                    for (int j = 0; j < ValueArray.Length; j++)
                                    {
                                        if (!string.IsNullOrEmpty(ctlValueArray[i].toStringTrim()) && !string.IsNullOrEmpty(ValueArray[j].toStringTrim()))
                                        {
                                            if ((ctlValueArray[i].toStringTrim().Contains(":") == true ? ctlValueArray[i].toStringTrim().Split(':')[1] : ctlValueArray[i].toStringTrim()) == ValueArray[j].toStringTrim())
                                                Checked = "checked=\"checked\"";
                                        }
                                    }
                                }
                            }
                            catch { }
                            //********checked="checked"*********

                            htmlCode += "<input type=\"checkbox\" " + laySkin + " id=\"" + ctlPrefix + TabColName + i.ToString() + "\" name=\"" + ctlPrefix + TabColName + i.ToString() + "\" title=\"" + (ctlValueArray[i].toStringTrim().Contains(":") == true ? ctlValueArray[i].toStringTrim().Split(':')[0] : ctlValueArray[i].toStringTrim()) + "\" " + layText + " value=\"" + (ctlValueArray[i].toStringTrim().Contains(":") == true ? ctlValueArray[i].toStringTrim().Split(':')[1] : ctlValueArray[i].toStringTrim()) + "\" lay-filter=\"" + ctlFilter + "\" " + ctlStyle + " " + Checked + ">";
                        }
                    }
                    else  //没有控件值时从指定表中读取生成控件
                    {
                        if (!string.IsNullOrEmpty(ctlSourceTable))
                        {
                            var getTableAttr = MicroDataTable.GetTableAttr(ctlSourceTable);  //获得表的相关属性
                            string OrderBy = getTableAttr.OrderBy;  //得到该表的Order by语句

                            string _sql = "select " + ctlTextName + "," + ctlTextValue + " from " + ctlSourceTable + " where Invalid=0 and Del=0 " + OrderBy + "";
                            DataTable _dt = MsSQLDbHelper.Query(_sql).Tables[0];
                            if (_dt != null && _dt.Rows.Count > 0)
                            {
                                for (int i = 0; i < _dt.Rows.Count; i++)  //生成CheckBox
                                {
                                    //********checked="checked"*********
                                    //在新增或修改表单情况下读取Value值与当前控件进行匹配，匹配的情况下让其选中
                                    string Checked = string.Empty;
                                    if (!string.IsNullOrEmpty(Value))
                                    {
                                        string[] ValueArray = Value.Split(',');
                                        for (int j = 0; j < ValueArray.Length; j++)
                                        {
                                            if (!string.IsNullOrEmpty(_dt.Rows[i][ctlTextValue].ToString()) && !string.IsNullOrEmpty(ValueArray[j].toStringTrim()))
                                            {
                                                if (_dt.Rows[i][ctlTextValue].ToString() == ValueArray[j].toStringTrim())
                                                    Checked = "checked=\"checked\"";
                                            }
                                        }
                                    }
                                    //********checked="checked"*********

                                    htmlCode += "<input type=\"checkbox\"" + laySkin + "\" id=\"" + ctlPrefix + TabColName + i.ToString() + "\" name=\"" + ctlPrefix + TabColName + i.ToString() + "\" title=\"" + _dt.Rows[i][ctlTextName].ToString() + "\" " + layText + " value=\"" + _dt.Rows[i][ctlTextValue].ToString() + "\" lay-filter=\"" + ctlFilter + "\" " + ctlStyle + " " + Checked + " >";
                                }
                            }
                        }
                    }

                    //同时生成隐藏文本框，用于存放Checkbox选中的值，便于提交和接收
                    htmlCode += "<input type=\"text\" id=\"" + ctlPrefix + TabColName + "\" name=\"" + ctlPrefix + TabColName + "\" placeholder=\"" + ctlPlaceholder + "\" lay-verify=\"" + ctlVerify2 + "\" lay-reqText=\"" + ctlTitle + "至少选择一个<br/>Choose at least one of these\" autocomplete=\"off\" class=\"layui-input layui-hide " + ctlVerify + "\" value=\"" + Value + "\"  readonly=\"readonly\">";

                    //在查看表单的情况下并且是显示Label形式，则以Label显示
                    if (Action == "view" && ctlViewDisplayLabel)
                        htmlCode = "<div id=\"div" + ctlPrefix + TabColName + "\" class=\"layui-form-mid layui-word-aux\">" + Value + "</div>";

                    //生成Checkbox点选操作的js代码，把选中值传入input Text
                    string ctlCheckboxType = ctlCheckboxSkin == "switch" ? "switch" : "checkbox"; //Checkbox控件默认监听类型为checkbox,Checkbox控件风格为Switch时，监听类型也改为Switch
                    jsFormCtrlChangeCode = "form.on('" + ctlCheckboxType + "(" + ctlFilter + ")', function (data) {var v = $('.css" + ctlPrefix + TabColName + " input[type=checkbox]:checked').map(function (index, elem) {return $(elem).val();}).get().join(','); $('#" + ctlPrefix + TabColName + "').val(v);});";

                    break;
                #endregion

                #region Select 下拉列表
                case "Select":

                    htmlCode = "<select id=\"" + ctlPrefix + TabColName + "\" name=\"" + ctlPrefix + TabColName + "\" lay-verify=\"" + ctlVerify2 + "\" lay-filter=\"" + ctlFilter + "\" lay-reqText=\"" + ctlTitle + "不能为空<br/>This cannot be empty\" " + ctlStyle + " lay-search=\"\" >";
                    htmlCode += "<option value=\"\"></option>";

                    if (!string.IsNullOrEmpty(ctlValue))  //有控件值时直接从控件值生成控件，没有时从指定表读取记录生成控件
                    {
                        string[] ctlValueArray = ctlValue.Split(',');  //根据默认值生成option
                        for (int i = 0; i < ctlValueArray.Length; i++)
                        {
                            //**********selected="selected"***********
                            //在修改表单状态时，匹配值让Select下拉菜单值选中
                            string Selected = string.Empty;  //选中
                            if (!string.IsNullOrEmpty(Value))
                                if ((ctlValueArray[i].toStringTrim().Contains(":") == true ? ctlValueArray[i].toStringTrim().Split(':')[1] : ctlValueArray[i].toStringTrim()) == Value)
                                    Selected = " selected=\"selected\"";
                            //**********selected="selected"***********

                            htmlCode += "<option value=\"" + ctlValueArray[i].ToString().Split(':')[1] + "\" " + Selected + ">" + ctlValueArray[i].ToString().Split(':')[0] + "</option>";
                        }
                    }
                    else  //没有控件值时从指定表中读取生成控件
                    {
                        if (!string.IsNullOrEmpty(ctlSourceTable))
                        {
                            //获取数据来原表的字段和相关属性
                            var getTableAttr = MicroDataTable.GetTableAttr(ctlSourceTable);
                            string PrimaryKeyName = getTableAttr.PrimaryKeyName;  //获得主键字段名称
                            string SortFieldName = getTableAttr.SortFieldName;  //获得排序字段名称
                            string SortFieldNameAscDesc = getTableAttr.SortFieldNameAscDesc;
                            string OrderBy = getTableAttr.OrderBy;  //获得order by 命令字符串
                            Boolean MainSub = getTableAttr.MainSub;  //用于判断是否开启父子项

                            //得到重新构造的表，因为数据表默认是没有MainSub的，要获取MainSub项所以需要重新构造表
                            DataTable _dt = MicroDataTable.GetDataTableForSelect(getTableAttr.MicroDT, ctlSourceTable, PrimaryKeyName, SortFieldName, SortFieldNameAscDesc, OrderBy, MainSub);

                            if (_dt != null && _dt.Rows.Count > 0)
                            {
                                for (int i = 0; i < _dt.Rows.Count; i++)  //生成option
                                {
                                    //**********selected="selected"***********
                                    //在新增或修改表单情况下读取Value值与当前控件进行匹配，匹配的情况下让其选中
                                    //**注**【在新增表单下该值赋予的是控件默认值ctlDefaultValue，要在修改表单下该值赋予的是该条记录的记录值】
                                    string Selected = string.Empty;
                                    if (!string.IsNullOrEmpty(Value))
                                    {
                                        if (_dt.Rows[i][ctlTextValue].ToString() == Value)
                                            Selected = " selected=\"selected\"";
                                    }
                                    //**********selected="selected"***********

                                    //当MainSub为真的时候，显示子项前的符号
                                    string _ctlTextName = string.Empty;
                                    if (MainSub)
                                    {
                                        string _MainSub = string.Empty;
                                        _MainSub = _dt.Rows[i]["MainSub"].ToString() == "Main" ? "" : _dt.Rows[i]["MainSub"].ToString() + " ";
                                        _ctlTextName = _MainSub + _dt.Rows[i][ctlTextName].ToString();
                                    }
                                    else
                                        _ctlTextName = _dt.Rows[i][ctlTextName].ToString();

                                    htmlCode += "<option value=\"" + _dt.Rows[i][ctlTextValue].ToString() + "\" " + Selected + ">" + _ctlTextName + "</option>";
                                }
                            }
                        }
                    }
                    htmlCode += "</select>";


                    //同时生成隐藏文本框，用于在外部读取MicroTable时调用，让选项默认选中
                    htmlCode += "<input type=\"hidden\" id=\"hid" + ctlPrefix + TabColName + "\" name=\"hid" + ctlPrefix + TabColName + "\" value=\"" + Value + "\">";

                    //在查看表单的情况下并且是显示Label形式，则以Label显示
                    if (Action == "view" && ctlViewDisplayLabel)
                        htmlCode = "<div id=\"div" + ctlPrefix + TabColName + "\" class=\"layui-form-mid layui-word-aux\">" + MicroXmSelect.GetCtlDisplayName(ctlSourceTable, ctlTextName, ctlTextValue, FormID, Value) + "</div>";

                    //控件扩展JS代码
                    if (!string.IsNullOrEmpty(ctlExtendJSCode))
                        jsFormExtendCode += ctlExtendJSCode;

                    //自定义函数，当自定义函数不为空时追加自定义函数
                    if (!string.IsNullOrEmpty(ctlVerifyCustomFunction))
                        jsFormVerifyCode += ctlVerifyCustomFunction + ",";

                    if (tbMainSub && TabColName.ToLower() == "parentid")
                    {
                        jsFormCtrlChangeCode = "var mGet = {getLevel: function(val) { var Fields = { \"stn\": \"" + ShortTableName + "\", \"idname\": \"" + primaryKeyName + "\", \"val\": val }; Fields = encodeURI(JSON.stringify(Fields));var Parameter = { \"action\": \"" + Action + "\", \"mid\": MID, \"isapprovalform\":\"" + IsApprovalForm + "\", \"fields\": Fields };var Value = micro.getAjaxData('json', micro.getRootPath() + '/App_Handler/GetParSubLevel.ashx', Parameter, false);$('#hidLevelCode').val(Value['LevelCode']);$('#hidLevel').val(Value['Level']);$('#txtSort').val(Value['Sort']);}}; ";

                        if (Action != "modify")
                            jsFormCtrlChangeCode += "mGet.getLevel(0); ";

                        jsFormCtrlChangeCode += "form.on('select(" + ctlPrefix + TabColName + ")', function (data) {mGet.getLevel(data.value);}); ";
                    }
                    break;

                #endregion

                #region Div 层 
                case "Div":

                    if (Action == "view" && ctlViewDisplayLabel)
                        htmlCode = "<div id=\"div" + ctlPrefix + TabColName + "\" class=\"layui-form-mid layui-word-aux\">" + MicroXmSelect.GetCtlDisplayName(ctlSourceTable, ctlTextName, ctlTextValue, FormID, Value) + "</div>";
                    else
                        htmlCode = "<div id=\"" + ctlPrefix + TabColName + "\"></div>";

                    break;
                #endregion

                #region RadioTreeSelect 单选树
                case "RadioTreeSelect":

                    if (Action == "view" && ctlViewDisplayLabel)
                        htmlCode = "<div id=\"div" + ctlPrefix + TabColName + "\" class=\"layui-form-mid layui-word-aux\">" + MicroXmSelect.GetCtlDisplayName(ctlSourceTable, ctlTextName, ctlTextValue, FormID, Value) + "</div>";
                    else
                    {
                        htmlCode = "<div id=\"div" + TabColName + "\"></div>";

                        jsFormExtendCode = "var Para" + TabColName + " = { \"action\": \"get\", \"mid\": MID, \"formid\": " + FormID + ", \"tn\": \"" + ctlSourceTable + "\", \"txt\": \"" + ctlTextName + "\", \"val\":\"" + ctlTextValue + "\", \"isapprovalform\":\"" + IsApprovalForm + "\", \"defaultvalue\": \"" + Value + "\" };";
                        jsFormExtendCode += "var XmSelect" + TabColName + " = xmSelect.render({ el: '#div" + TabColName + "', name: '" + ctlPrefix + TabColName + "', language: 'zn', autoRow: false, filterable: true, showCount: 10, pageSize: 10, searchTips: 'Keyword', paging: true, height: '200px', radio: true, clickClose: true, layVerify: '" + xmVerify + "', model: {label: {type: 'block', block: {showCount: 10, showIcon: false,}}}, data: eval(micro.getAjaxData('text', micro.getRootPath() + '/App_Handler/GetPublicXmSelect.ashx', Para" + TabColName + "))});";
                    }

                    //控件扩展JS代码
                    if (!string.IsNullOrEmpty(ctlExtendJSCode))
                        jsFormExtendCode += ctlExtendJSCode;

                    break;
                #endregion

                #region CheckBoxTreeSelect 多选树
                case "CheckBoxTreeSelect":

                    if (Action == "view" && ctlViewDisplayLabel)
                        htmlCode = "<div id=\"div" + ctlPrefix + TabColName + "\" class=\"layui-form-mid layui-word-aux\">" + MicroXmSelect.GetCtlDisplayName(ctlSourceTable, ctlTextName, ctlTextValue, FormID, Value) + "</div>";
                    else
                    {
                        htmlCode = "<div id=\"div" + TabColName + "\"></div>";

                        jsFormExtendCode = "var Para" + TabColName + " = { \"action\": \"get\", \"mid\": MID, \"formid\": " + FormID + ", \"tn\": \"" + ctlSourceTable + "\", \"txt\": \"" + ctlTextName + "\", \"val\":\"" + ctlTextValue + "\", \"isapprovalform\":\"" + IsApprovalForm + "\", \"defaultvalue\": \"" + Value + "\" };";
                        jsFormExtendCode += "var XmSelect" + TabColName + " = xmSelect.render({ el: '#div" + TabColName + "', name: '" + ctlPrefix + TabColName + "', language: 'zn', autoRow: false, filterable: true, toolbar: {show: true, list: ['ALL', 'REVERSE', 'CLEAR'] }, showCount: 10, pageSize: 10, searchTips: 'Keyword', paging: true, height: '250px', radio: false, clickClose: false, layVerify: '" + xmVerify + "', max: 800, maxMethod: function (seles, item){layer.msg('最多只能选择800项');} ,tree: { show: true, showFolderIcon: true, showLine: true, indent: 20, expandedKeys: true, strict: false, simple: true, }, model: {label: {type: 'block', block: {showCount: 5, showIcon: true,}}}, data: eval(micro.getAjaxData('text', micro.getRootPath() + '/App_Handler/GetPublicXmSelect.ashx', Para" + TabColName + "))});";
                    }

                    //控件扩展JS代码
                    if (!string.IsNullOrEmpty(ctlExtendJSCode))
                        jsFormExtendCode += ctlExtendJSCode;

                    break;
                #endregion

                #region RadioCascaderSelect 单选树级联
                case "RadioCascaderSelect":

                    if (Action == "view" && ctlViewDisplayLabel)
                        htmlCode = "<div id=\"div" + ctlPrefix + TabColName + "\" class=\"layui-form-mid layui-word-aux\">" + MicroXmSelect.GetCtlDisplayName(ctlSourceTable, ctlTextName, ctlTextValue, FormID, Value) + "</div>";
                    else
                    {
                        htmlCode = "<div id=\"div" + TabColName + "\"></div>";

                        jsFormExtendCode = "var Para" + TabColName + " = { \"action\": \"get\", \"mid\": MID, \"formid\": " + FormID + ", \"tn\": \"" + ctlSourceTable + "\", \"txt\": \"" + ctlTextName + "\", \"val\":\"" + ctlTextValue + "\", \"isapprovalform\":\"" + IsApprovalForm + "\", \"defaultvalue\": \"" + Value + "\" };";
                        jsFormExtendCode += "var XmSelect" + TabColName + " = xmSelect.render({ el: '#div" + TabColName + "', name: '" + ctlPrefix + TabColName + "', language: 'zn', autoRow: false, filterable: true, showCount: 10, pageSize: 10, searchTips: 'Keyword', paging: true, height: 'auto', radio: true, clickClose: true, layVerify: '" + xmVerify + "'," +
                            " model: { icon:'hidden', label: {type: 'block', block: {showCount: 10,showIcon: true,}}}," +
                            " cascader: { show: true, indent: 300, strict: true}, data: eval(micro.getAjaxData('text', micro.getRootPath() + '/App_Handler/GetPublicXmSelect.ashx', Para" + TabColName + "))});";
                        //maxMethod(seles, item){layer.msg('最多只能选择一项');} ,
                    }

                    //控件扩展JS代码
                    if (!string.IsNullOrEmpty(ctlExtendJSCode))
                        jsFormExtendCode += ctlExtendJSCode;

                    break;
                #endregion

                #region CheckBoxCascaderSelect 多选树级联
                case "CheckBoxCascaderSelect":

                    if (Action == "view" && ctlViewDisplayLabel)
                        htmlCode = "<div id=\"div" + ctlPrefix + TabColName + "\" class=\"layui-form-mid layui-word-aux\">" + MicroXmSelect.GetCtlDisplayName(ctlSourceTable, ctlTextName, ctlTextValue, FormID, Value) + "</div>";
                    else
                    {
                        htmlCode = "<div id=\"div" + TabColName + "\"></div>";

                        jsFormExtendCode = "var Para" + TabColName + " = { \"action\": \"get\", \"mid\": MID, \"formid\": " + FormID + ", \"tn\": \"" + ctlSourceTable + "\", \"txt\": \"" + ctlTextName + "\", \"val\":\"" + ctlTextValue + "\", \"isapprovalform\":\"" + IsApprovalForm + "\", \"defaultvalue\": \"" + Value + "\" };";
                        jsFormExtendCode += "var XmSelect" + TabColName + " = xmSelect.render({ el: '#div" + TabColName + "', name: '" + ctlPrefix + TabColName + "', language: 'zn', autoRow: false, filterable: true, pageSize: 10, searchTips: 'Keyword', paging: true, height: '200px', radio: false, clickClose: false, layVerify: '" + xmVerify + "', max: 500, maxMethod: function (seles, item){layer.msg('最多只能选择500项');} , model: { label: {type: 'block', block: {showCount: 3,showIcon: true,}}}, cascader: { show: true, indent: 300, strict: false}, data: eval(micro.getAjaxData('text', micro.getRootPath() + '/App_Handler/GetPublicXmSelect.ashx', Para" + TabColName + "))});";

                    }

                    //控件扩展JS代码
                    if (!string.IsNullOrEmpty(ctlExtendJSCode))
                        jsFormExtendCode += ctlExtendJSCode;

                    break;
                #endregion

                #region RadioCascaderSelectUserByDept 单选树级联按部门列出用户
                case "RadioCascaderSelectUserByDept":

                    if (Action == "view" && ctlViewDisplayLabel)
                        htmlCode = "<div id=\"div" + ctlPrefix + TabColName + "\" class=\"layui-form-mid layui-word-aux\">" + MicroXmSelect.GetCtlDisplayName(ctlSourceTable, ctlTextName, ctlTextValue, FormID, Value) + "</div>";
                    else
                    {
                        htmlCode = "<div id=\"div" + TabColName + "\"></div>";

                        jsFormExtendCode = "var Para" + TabColName + " = { \"action\": \"get\", \"type\": \"DeptUsers\", \"mid\": MID , \"defaultvalue\": $('#xmByUserDefVal').length>0? $('#xmByUserDefVal').val():\"" + Value + "\"};";
                        jsFormExtendCode += "var XmSelect" + TabColName + " = xmSelect.render({ el: '#div" + TabColName + "', name: '" + ctlPrefix + TabColName + "', language: 'zn', autoRow: false, filterable: true, showCount: 10, pageSize: 10, searchTips: 'Keyword', paging: true, height: 'auto', radio: true, clickClose: true, layVerify: '" + xmVerify + "', model: {icon:'hidden', label: {type: 'block', block: {showCount: 10,showIcon: true,}}}, " +
                            "cascader: { show: true, indent: 300, strict: true}, " +
                            "data: eval(micro.getAjaxData('text', micro.getRootPath() + '/App_Handler/GetUserPublicInfoForXmSelect.ashx', Para" + TabColName + "))});";
                    }

                    //控件扩展JS代码
                    if (!string.IsNullOrEmpty(ctlExtendJSCode))
                        jsFormExtendCode += ctlExtendJSCode;

                    break;
                #endregion

                #region CheckBoxCascaderSelectUserByDept 多选树级联按部门列出用户
                case "CheckBoxCascaderSelectUserByDept":

                    if (Action == "view" && ctlViewDisplayLabel)
                        htmlCode = "<div id=\"div" + ctlPrefix + TabColName + "\" class=\"layui-form-mid layui-word-aux\">" + MicroXmSelect.GetCtlDisplayName(ctlSourceTable, ctlTextName, ctlTextValue, FormID, Value) + "</div>";
                    else
                    {
                        htmlCode = "<div id=\"div" + TabColName + "\"></div>";

                        jsFormExtendCode = "var Para" + TabColName + " = { \"action\": \"get\", \"type\": \"DeptUsers\", \"mid\": MID , \"defaultvalue\": $('#xmByUserDefVal').length>0? $('#xmByUserDefVal').val():\"" + Value + "\"};";
                        jsFormExtendCode += "var XmSelect" + TabColName + " = xmSelect.render({ el: '#div" + TabColName + "', name: '" + ctlPrefix + TabColName + "', language: 'zn', autoRow: false, filterable: true, showCount: 10, pageSize: 10, searchTips: 'Keyword', paging: true, height: 'auto', radio: false, clickClose: false, layVerify: '" + xmVerify + "', max: 500, maxMethod: function (seles, item){layer.msg('最多只能选择500个') }, model: {icon:'hidden',label: {type: 'block', block: {showCount: 3,showIcon: true,}}}, " +
                            "cascader: { show: true, indent: 300, strict: true}, " +
                            "data: eval(micro.getAjaxData('text', micro.getRootPath() + '/App_Handler/GetUserPublicInfoForXmSelect.ashx', Para" + TabColName + "))});";
                    }

                    //控件扩展JS代码
                    if (!string.IsNullOrEmpty(ctlExtendJSCode))
                        jsFormExtendCode += ctlExtendJSCode;

                    break;
                #endregion

                #region DragImgUpload 拖拽式图片上传
                case "DragImgUpload":

                    //lay-verify赋值
                    string textDragImgUpload = string.Empty;
                    if (!string.IsNullOrEmpty(ctlVerify2))
                        textDragImgUpload = ctlVerify2 + "|" + ctlPrefix + TabColName + "Length";  //|ctlPrefix+TabColName+Length用于检测input输入长度
                    else
                        textDragImgUpload = ctlPrefix + TabColName + "Length";  //|ctlPrefix+TabColName+Length用于检测input输入长度

                    //Value不为空时显示图片
                    string _wspadding = string.Empty, _layuiHide = "layui-hide", _layuiHide2 = string.Empty;
                    if (!string.IsNullOrEmpty(Value))
                    {
                        _wspadding = " ws-padding5";
                        _layuiHide = "";
                        _layuiHide2 = "layui-hide";
                    }

                    htmlCode = "<div class=\"layui-upload-drag dargUploadImg" + ctlPrefix + TabColName + _wspadding + " \" " + ctlStyle + ">";
                    htmlCode += "<i class=\"layui-icon layui-icon-upload " + _layuiHide2 + "\"></i>";
                    htmlCode += "<p class=\"" + _layuiHide2 + "\">点击上传，或将文件拖拽到此处</p>";
                    htmlCode += "<div class=\"" + _layuiHide + " uploadView" + ctlPrefix + TabColName + "\">";
                    htmlCode += "<img src =\"" + Value + "\" style=\"width:100%; height:100%;\">";
                    htmlCode += "</div>";
                    htmlCode += "</div>";

                    htmlCode += "<input type=\"text\" id=\"" + ctlPrefix + TabColName + "\" name=\"" + ctlPrefix + TabColName + "\" value=\"" + Value + "\" placeholder=\"" + ctlPlaceholder + "\" lay-verify=\"" + textDragImgUpload + "\" lay-reqText=\"" + ctlTitle + "不能为空<br/>This cannot be empty\" autocomplete=\"off\" class=\"layui-input layui-hide css" + ctlPrefix + TabColName + "\" >";

                    //在查看表单的情况下是以Label形式显示，则以Label显示，否则以控件显示
                    //if (Action == "view" && ctlViewDisplayLabel)
                    //{
                    //    //htmlCode = "<div class=\"layui-form-mid layui-word-aux " + wsFontRed + "\">" + Value + "</div>";
                    //    htmlCode = "<div class=\"layui-form-mid layui-word-aux " + wsFontRed + "\"><img src =\"" + Value + "\"></div>";
                    //}

                    if (Action != "view")
                    {
                        ctlExtendJSCode = " upload.render({ elem: '.dargUploadImg" + ctlPrefix + TabColName + "', accept: 'images', acceptMime:'image/*', size: 2048, url: micro.getRootPath() + '/App_Handler/UploadImage.ashx', done: function (src" + ctlPrefix + TabColName + ") { " +
                            "layer.msg('上传成功<br/>Upload successful'); " +
                            "$('.dargUploadImg" + ctlPrefix + TabColName + "').css('padding', '5px').find('p,i').css('display', 'none');" +
                            "$('.uploadView" + ctlPrefix + TabColName + "').removeClass('layui-hide').find('img').attr('src', src" + ctlPrefix + TabColName + ".data.src);" +
                            "$('.css" + ctlPrefix + TabColName + "').val(src" + ctlPrefix + TabColName + ".data.src)} });";
                    }

                    //控件扩展JS代码
                    if (!string.IsNullOrEmpty(ctlExtendJSCode))
                        jsFormExtendCode += ctlExtendJSCode;

                    //自定义函数，当自定义函数不为空时追加自定义函数
                    if (!string.IsNullOrEmpty(ctlVerifyCustomFunction))
                        jsFormVerifyCode += ctlVerifyCustomFunction + ",";

                    break;
                #endregion

                #region MultiDragImgUpload 拖拽式多图片上传
                case "MultiDragImgUpload":

                    htmlCode = "<div class=\"prependMultiUploadImg" + ctlPrefix + TabColName + "\">";

                    //当值不为空时，处理编辑或浏览状态时
                    if (!string.IsNullOrEmpty(Value))
                    {
                        string[] valueArray = Value.Split(',');
                        for (int i = 0; i < valueArray.Length; i++)
                        {
                            htmlCode += "<div class=\"layui-inline\"> <div style = \"height:146px; width:146px;\" ><a href=\"javascript:;\"><img src = \"" + valueArray[i] + "\" style=\"width:100%; height:100%;\" class=\"micro-click2\" data-type=\"ViewImg2\" /></a></div></div>";
                        }
                    }

                    if (Action != "view")
                    {
                        htmlCode += "<div class=\"layui-inline\">";
                        htmlCode += "<div class=\"layui-upload-drag multiUploadImg" + ctlPrefix + TabColName + "\" style=\"height:114px; width:135px; padding:30px 5px 0px 5px;\" >";
                        htmlCode += "<i class=\"layui-icon layui-icon-upload\"></i><p>点击上传<br />或将文件拖拽到此处</p>";
                        htmlCode += "</div> ";
                        htmlCode += "</div>";
                    }

                    htmlCode += "</div>";

                    htmlCode += "<input type=\"text\" id=\"" + ctlPrefix + TabColName + "\" name=\"" + ctlPrefix + TabColName + "\" value=\"" + Value + "\" placeholder=\"" + ctlPlaceholder + "\" lay-reqText=\"" + ctlTitle + "不能为空<br/>This cannot be empty\" autocomplete=\"off\" class=\"layui-input layui-hide css" + ctlPrefix + TabColName + "\" >";


                    if (Action != "view")
                    {
                        ctlExtendJSCode = " upload.render({";
                        ctlExtendJSCode += " elem: '.multiUploadImg" + ctlPrefix + TabColName + "', accept: 'images', acceptMime: 'image/*', size: 2048, url: micro.getRootPath() + '/App_Handler/UploadImage.ashx?dir=UploadImages', multiple: true, done: function (src" + ctlPrefix + TabColName + ") {";
                        ctlExtendJSCode += " var src = src" + ctlPrefix + TabColName + ".data.src;";
                        ctlExtendJSCode += " var divImg = '<div class=\"layui-inline\"> <div style=\"height:146px; width:146px;\" ><a href=\"javascript:;\"><img src=\"' + src + '\" style=\"width:100%; height:100%;\" class=\"micro-click\" data-type=\"ViewImg\" /></a></div></div>';";
                        ctlExtendJSCode += " $('.prependMultiUploadImg" + ctlPrefix + TabColName + "').prepend(divImg);";
                        ctlExtendJSCode += " var txtVal = $('.css" + ctlPrefix + TabColName + "').val(); txtVal = txtVal === '' ? '' : txtVal + ','; $('.css" + ctlPrefix + TabColName + "').val(txtVal + src); }, allDone: function (srcName) { layer.msg('上传成功<br/>Upload successful');";
                        ctlExtendJSCode += " var mGet" + ctlPrefix + TabColName + " = { ViewImg: function () {layer.open({type: 1,title: false,closeBtn: 0,area: ['auto'],skin: 'layui-layer-nobg', shadeClose: true,content: '<img src=\"' + $(this).attr('src') + '\"/>'});}};";
                        ctlExtendJSCode += "  $('.micro-click').on('click', function () {var type = $(this).data('type'); mGet" + ctlPrefix + TabColName + "[type] ? mGet" + ctlPrefix + TabColName + "[type].call(this) : ''; }); } });";
                    }

                    //当值不为空时，处理编辑或浏览状态时
                    if (!string.IsNullOrEmpty(Value))
                        ctlExtendJSCode += " var mGet2" + ctlPrefix + TabColName + " = { ViewImg2: function() { layer.open({ type: 1,title: false,closeBtn: 0,area:['auto'],skin: 'layui-layer-nobg', shadeClose: true,content: '<img src=\"' + $(this).attr('src') + '\"/>'}); }}; $('.micro-click2').on('click', function() { var type = $(this).data('type'); mGet2" + ctlPrefix + TabColName + "[type] ? mGet2" + ctlPrefix + TabColName + "[type].call(this) : ''; });";

                    //控件扩展JS代码
                    if (!string.IsNullOrEmpty(ctlExtendJSCode))
                        jsFormExtendCode += ctlExtendJSCode;

                    //自定义函数，当自定义函数不为空时追加自定义函数
                    if (!string.IsNullOrEmpty(ctlVerifyCustomFunction))
                        jsFormVerifyCode += ctlVerifyCustomFunction + ",";

                    break;
                #endregion

                #region Textarea
                case "Textarea":

                    //lay-verify赋值
                    string textareaVerify = string.Empty;
                    if (!string.IsNullOrEmpty(ctlVerify2))
                        textareaVerify = ctlVerify2 + "|" + ctlPrefix + TabColName + "Length";  //|ctlPrefix+TabColName+Length用于检测input输入长度
                    else
                        textareaVerify = ctlPrefix + TabColName + "Length";  //|ctlPrefix+TabColName+Length用于检测input输入长度

                    htmlCode = "<textarea id=\"" + ctlPrefix + TabColName + "\" name=\"" + ctlPrefix + TabColName + "\" placeholder=\"" + ctlPlaceholder + "\" lay-verify=\"" + textareaVerify + "\" lay-reqText=\"" + ctlTitle + "不能为空<br/>This cannot be empty\" class=\"layui-textarea " + ctlVerify + "\" " + ctlStyle + ">" + Value + "</textarea>";

                    //当字符长度不为空时，生成长度验证js代码
                    if (!string.IsNullOrEmpty(ctlCharLength) && ctlCharLength != "0")
                    {
                        if (ctlCharLength.Contains(","))  //验证长度在某范围之间
                        {
                            string[] v = ctlCharLength.Split(',');
                            jsFormVerifyCode = ctlPrefix + TabColName + "Length: [/^.{" + ctlCharLength + "}$/,'" + ctlTitle + "长度只允许" + v[0].toStringTrim() + "到" + v[1].toStringTrim() + "位之间<br/>This length is only allowed between " + v[0].toStringTrim() + " and " + v[1].toStringTrim() + " bits.'],";
                        }
                        else   //验证长度不能超过指定值
                            jsFormVerifyCode = ctlPrefix + TabColName + "Length: function (value, item) { if (value.length > " + ctlCharLength + ") { return '" + ctlTitle + "长度不能超过" + ctlCharLength + "位<br/> This length can not exceed " + ctlCharLength + " bits';}},";
                    }

                    //在查看表单的情况下并且是显示Label形式，则以Label显示
                    if (Action == "view" && ctlViewDisplayLabel)
                        htmlCode = "<div id=\"div" + ctlPrefix + TabColName + "\" class=\"layui-form-mid layui-word-aux\">" + Value + "</div>";

                    //控件扩展JS代码
                    if (!string.IsNullOrEmpty(ctlExtendJSCode))
                        jsFormExtendCode += ctlExtendJSCode;

                    //自定义函数，当自定义函数不为空时追加自定义函数
                    if (!string.IsNullOrEmpty(ctlVerifyCustomFunction))
                        jsFormVerifyCode += ctlVerifyCustomFunction + ",";

                    break;
                #endregion

                #region LayEdit 富文本编辑器
                case "LayEdit":

                    //lay-verify赋值
                    string textareaVerify2 = string.Empty;
                    if (!string.IsNullOrEmpty(ctlVerify2))
                        textareaVerify2 = ctlVerify2 + "|" + ctlPrefix + TabColName + "Length";  //|ctlPrefix+TabColName+Length用于检测input输入长度
                    else
                        textareaVerify2 = ctlPrefix + TabColName + "Length";  //|ctlPrefix+TabColName+Length用于检测input输入长度

                    //用于创建layedit富文本编辑器
                    htmlCode = "<textarea id=\"" + ctlPrefix + TabColName + "\" name=\"" + ctlPrefix + TabColName + "\" placeholder=\"" + ctlPlaceholder + "\" lay-verify=\"" + textareaVerify2 + "\" class=\"layui-textarea " + ctlVerify + "\" " + ctlStyle + ">" + Value + "</textarea>";

                    //当字符长度(ctlCharLength)不为空时，生成长度验证js代码
                    if (!string.IsNullOrEmpty(ctlCharLength) && ctlCharLength != "0")
                    {
                        if (ctlCharLength.Contains(","))  //验证长度在某范围之间
                        {
                            string[] v = ctlCharLength.Split(',');
                            jsFormVerifyCode = ctlPrefix + TabColName + "Length: [/^.{" + ctlCharLength + "}$/,'" + ctlTitle + "长度只允许" + v[0].toStringTrim() + "到" + v[1].toStringTrim() + "位之间<br/>This length is only allowed between " + v[0].toStringTrim() + " and " + v[1].toStringTrim() + " bits.'],";
                        }
                        else   //验证长度不能超过指定值
                            jsFormVerifyCode = ctlPrefix + TabColName + "Length: function (value, item) { if (value.length > " + ctlCharLength + ") { return '" + ctlTitle + "长度不能超过" + ctlCharLength + "位<br/> This length can not exceed " + ctlCharLength + " bits';}},";
                    }

                    //在查看表单的情况下并且是显示Label形式，则以Label显示
                    if (Action == "view" && ctlViewDisplayLabel)
                        htmlCode = "<div id=\"div" + ctlPrefix + TabColName + "\" class=\"layui-form-mid layui-word-aux\">" + Value + "</div>";

                    //建立富文本编辑器
                    jsFormExtendCode = "layedit.set({ uploadImage: {url: '/App_Handler/FormsUpload.ashx'  , type: 'post' }}); "; //设置立富文本编辑器图标上传路径

                    //建立富文本编辑器 
                    //tool: ['strong' //加粗,'italic' //斜体,'underline' //下划线,'del' //删除线,'|' //分割线,'left' //左对齐,'center' //居中对齐,'right' //右对齐,'link' //超链接,'unlink' //清除链接,'face' //表情,'image' //插入图片,'help' //帮助]
                    jsFormExtendCode += "var layedit" + TabColName + " = layedit.build('" + ctlPrefix + TabColName + "',{tool:['image'],height: 180}); ";

                    //jsFormExtendCode += "var layeditEvent = $('iframe[textarea=" + ctlPrefix + TabColName + "]').contents().find('body');";
                    jsFormExtendCode += "$('#btnSave').on('click',function () {layedit.sync(layedit" + TabColName + ");}); ";

                    //控件扩展JS代码
                    if (!string.IsNullOrEmpty(ctlExtendJSCode))
                        jsFormExtendCode += ctlExtendJSCode;

                    //自定义函数，当自定义函数不为空时追加自定义函数
                    if (!string.IsNullOrEmpty(ctlVerifyCustomFunction))
                        jsFormVerifyCode += ctlVerifyCustomFunction + ",";

                    break;
                #endregion

                #region WangEditor 富文本编辑器
                case "WangEditor":

                    ctlPlaceholder = string.IsNullOrEmpty(ctlPlaceholder) == true ? "请在此处编辑你的内容。" : ctlPlaceholder;

                    //准备div容器，用于创建Editor
                    htmlCode = "<div id=\"div" + ctlPrefix + TabColName + "\"><p>" + ctlPlaceholder + "</p></div>";

                    //lay-verify赋值
                    string textWangEditorVerify = string.Empty;
                    if (!string.IsNullOrEmpty(ctlVerify2))
                        textWangEditorVerify = ctlVerify2 + "|" + ctlPrefix + TabColName + "Length";  //|ctlPrefix+TabColName+Length用于检测input输入长度
                    else
                        textWangEditorVerify = ctlPrefix + TabColName + "Length";  //|ctlPrefix+TabColName+Length用于检测input输入长度

                    htmlCode += "<textarea id=\"" + ctlPrefix + TabColName + "\" name=\"" + ctlPrefix + TabColName + "\" placeholder=\"" + ctlPlaceholder + "\" lay-verify=\"" + textWangEditorVerify + "\" lay-reqText=\"" + ctlTitle + "不能为空<br/>This cannot be empty\" class=\"layui-textarea layui-hide " + ctlVerify + "\" " + ctlStyle + ">" + Value + "</textarea>";

                    if (Action != "view")
                    {
                        jsFormExtendCode = "const E" + ctlPrefix + TabColName + " = window.wangEditor;";  //声明编辑器
                        jsFormExtendCode += "const editor" + ctlPrefix + TabColName + " = new E" + ctlPrefix + TabColName + "('#div" + ctlPrefix + TabColName + "');"; //向容器创建新的对象器
                        jsFormExtendCode += "editor" + ctlPrefix + TabColName + ".config.uploadImgServer = micro.getRootPath() + '/App_Handler/UploadMultiImage.ashx?dir=WEditor';";  //图片上传地址
                        jsFormExtendCode += "editor" + ctlPrefix + TabColName + ".config.uploadImgMaxLength = 10;"; //图片最多允许上传数量
                        jsFormExtendCode += "editor" + ctlPrefix + TabColName + ".config.zIndex = 500;"; //配置 z-index
                        jsFormExtendCode += "editor" + ctlPrefix + TabColName + ".config.onchange = function (newHtml) {$('#" + ctlPrefix + TabColName + "').val(''); $('#" + ctlPrefix + TabColName + "').val(newHtml);};";  //onchange事件把内容插入textarea
                        jsFormExtendCode += "editor" + ctlPrefix + TabColName + ".config.onchangeTimeout = 500;";  //onchange频率
                        jsFormExtendCode += "editor" + ctlPrefix + TabColName + ".config.colors = ['#000000', '#eeece0', '#ffffff', '#1c487f', '#4d80bf', '#c24f4a', '#8baa4a', '#7b5ba1', '#46acc8', '#f9963b', '#009688', '#5FB878', '#393D49', '#1E9FFF', '#FFB800', '#FF5722', '#01AAED', '#2F4056', '#e60021', '#ff0000', '#c71585', '#00babd', '#ff7800', '#fad400'];"; //编辑器字体和背景颜色
                        jsFormExtendCode += "editor" + ctlPrefix + TabColName + ".create();"; //创建编辑器
                        //jsFormExtendCode += "editor" + ctlPrefix + TabColName + ".txt.html('" + HttpContext.Current.Server.HtmlDecode(Value) + "');"; //设置编辑器内容,html字符转义
                        jsFormExtendCode += "editor" + ctlPrefix + TabColName + ".txt.html('" + Value.toEditorTrim() + "');"; //设置编辑器内容
                        jsFormExtendCode += "editor" + ctlPrefix + TabColName + ".config.uploadImgHooks = {customInsert: function (insertImgFn, result) {for (var i = 0; i < result.count; i++) {insertImgFn(result.data[i])}; }};";  //上传图片成功后插入编辑里
                    }

                    if (Action == "view" && ctlViewDisplayLabel)
                        htmlCode = "<div id=\"div" + ctlPrefix + TabColName + "\" class=\"layui-form-mid layui-word-aux\">" + Value + "</div>";

                    //控件扩展JS代码
                    if (!string.IsNullOrEmpty(ctlExtendJSCode))
                        jsFormExtendCode += ctlExtendJSCode;

                    //自定义函数，当自定义函数不为空时追加自定义函数
                    if (!string.IsNullOrEmpty(ctlVerifyCustomFunction))
                        jsFormVerifyCode += ctlVerifyCustomFunction + ",";

                    break;
                    #endregion

            }

            var Code = new Code
            {
                HtmlCode = htmlCode,
                JsFormVerifyCode = jsFormVerifyCode,
                JsFormCtrlChangeCode = jsFormCtrlChangeCode,
                JsFormExtendCode = jsFormExtendCode

            };

            return Code;
        }


        /// <summary>
        /// 生成表单按钮层HTML代码
        /// </summary>
        /// <param name="Action">表单动作/提交动作</param>
        /// <returns></returns>
        private static string GetFormButtonCode(string Action, string ModuleID, string FormID, string FormsID, string FormType, Boolean IsApprovalForm, string OwnerUID, Boolean ShowButton, string BtnType = "Default")
        {

            string Button = string.Empty, layuiHide = string.Empty;
            if (!ShowButton)
                layuiHide = "layui-hide";

            Button += "<div class=\"layui-form-item " + layuiHide + " ws-margin-top10\">";
            Button += "<div class=\"layui-input-block micro-btn-div\">";
            Button += GetFormButton(Action, ModuleID, FormID, FormsID, FormType, IsApprovalForm, OwnerUID, BtnType);
            Button += "</div>";
            Button += "</div>";

            return Button;
        }

        /// <summary>
        ///  生成按钮HTML代码
        /// </summary>
        /// <param name="Action">按钮类型（add=btnSave=立即保存，modify=btnModify=保存修改，reset=btnReset=重置按钮）</param>
        /// <returns></returns>
        public static string GetFormButton(string Action, string ModuleID, string FormID, string FormsID, string FormType, Boolean IsApprovalForm, string OwnerUID, string BtnType = "Default")
        {
            string Button = string.Empty, ButtonType = string.Empty;
            string AddDisabled = string.Empty, AddButtonCssDisabled = string.Empty, ModifyDisabled = string.Empty, ModifyButtonCssDisabled = string.Empty, CloseDisabled = string.Empty, CloseButtonCssDisabled = string.Empty;
            string UID = MicroUserInfo.GetUserInfo("UID");
            string AddNum = "8", ModifyNum = "9";

            if (FormType == "SystemForm")
            {
                AddNum = "2";  //添加权限，系统表单用“2”，申请表单用“8”
                ModifyNum = "3";  //修改权限，系统表单用“3”，申请表单用“9”
            }

            Boolean AddPermit = MicroAuth.CheckPermit(ModuleID, AddNum);  //添加权限，系统表单用“2”，申请表单用“8”
            Boolean ModifyPermit = MicroAuth.CheckPermit(ModuleID, ModifyNum);  //修改权限，系统表单用“3”，申请表单用“9”
            Boolean ModifyAllPermit = MicroAuth.CheckPermit(ModuleID, "10");  //修改所有权限
            Boolean ClosePermit = MicroAuth.CheckPermit(ModuleID, "14");  //结案权限

            //新增记录权限，没有申请（新增）权限，禁用申请按钮
            if (!AddPermit)
            {
                AddDisabled = "disabled=\"disabled\"";
                AddButtonCssDisabled = "layui-btn-disabled";
            }

            //修改记录权限，没有修改所有记录权限时
            if (!ModifyAllPermit)
            {
                //再判断有没有修改自己记录的权限，没有时禁用修改按钮
                if (!ModifyPermit)
                {
                    ModifyDisabled = "disabled=\"disabled\"";
                    ModifyButtonCssDisabled = "layui-btn-disabled";
                }
                else
                {
                    //如果有修改自己的记录的情况下，判断当前记录是否为自己的记录，不是的话则禁止修改
                    if (!string.IsNullOrEmpty(OwnerUID) && OwnerUID != "0")
                        if (OwnerUID != UID)
                        {
                            ModifyDisabled = "disabled=\"disabled\"";
                            ModifyButtonCssDisabled = "layui-btn-disabled";
                        }
                }
            }

            if (Action == "close")
            {
                //结案记录权限，没有结案权限时
                if (!ClosePermit)
                {
                    string iCanApprovalUID = string.Empty;
                    var GetFormApprovalRecordsAttr = MicroWorkFlow.GetFormApprovalRecordsAttr(FormID, FormsID);
                    DataRow[] _rows = GetFormApprovalRecordsAttr.SourceRows;

                    if (_rows.Length > 0)
                        iCanApprovalUID = _rows[_rows.Length - 1]["CanApprovalUID"].toStringTrim();  //_rows.Length - 1 获得能结案者的UID

                    //没有结案权限时再判断自己是不是结案者（即FormApprovalRecords表的结案步骤），没有的话则禁止结案
                    if (!MicroPublic.CheckSplitExists(iCanApprovalUID, UID, ','))
                    {
                        CloseDisabled = "disabled=\"disabled\"";
                        CloseButtonCssDisabled = "layui-btn-disabled";
                    }
                }
            }

            switch (Action)
            {
                case "add":  //保存按钮
                    if (BtnType == "SaveDraft")
                    {
                        Button = "<button type=\"button\" id=\"btnSave\" class=\"layui-btn " + AddButtonCssDisabled + "\" " + AddDisabled + " lay-submit lay-filter=\"btnSave\">立即提交</button>";
                        Button += "<button type=\"button\" id=\"btnDraft\" class=\"layui-btn layui-btn-normal\" lay-submit lay-filter=\"btnDraft\">临时保存</button>";
                    }
                    else if (BtnType == "Save")
                        Button = "<button type=\"button\" id=\"btnSave\" class=\"layui-btn " + AddButtonCssDisabled + "\" " + AddDisabled + " lay-submit lay-filter=\"btnSave\">立即提交</button>";
                    else if (BtnType == "Draft")
                        Button = "<button type=\"button\" id=\"btnDraft\" class=\"layui-btn layui-btn-normal\" lay-submit lay-filter=\"btnDraft\">临时保存</button>";

                    break;

                case "modify":  //编辑按钮
                    Button = "<button type=\"button\" id=\"btnModify\" class=\"layui-btn layui-btn-normal " + ModifyButtonCssDisabled + "\" " + ModifyDisabled + " lay-submit lay-filter=\"btnModify\">保存修改</button>";
                    break;

                case "close":  //结案时显示编辑按钮
                    Button = "<button type=\"button\" id=\"btnClose\" class=\"layui-btn layui-btn-normal " + CloseButtonCssDisabled + "\" " + CloseDisabled + " lay-submit lay-filter=\"btnClose\">保存结案</button>";
                    break;
            }

            //重置按钮
            Button += "<button type=\"reset\" class=\"layui-btn layui-btn-primary\">立即重置</button>";

            return Button;
        }


        /// <summary>
        /// 生成提交表单Submit的JS代码
        /// </summary>
        /// <param name="Action">根据表单的动作类型返回layFilter 如：add=btnSave, modify=btnModify</param>
        /// <param name="ShortTableName">短表名</param>
        /// <param name="MID">ModuleID</param>
        /// <param name="FormID">FormID</param>
        /// <param name="IsApprovalForm">判断表单是否为审批类型的表单</param>
        /// <param name="IsRefresh">是提交后刷新</param>
        /// <returns></returns>
        public static string GetFormSubmitCode(string Action, string ShortTableName, string MID, string FormID, Boolean IsApprovalForm, Boolean IsRefresh)
        {
            string flag = string.Empty, jsFormSubmitCode = string.Empty, layFilter = string.Empty;
            string isCloseParent = string.Empty;

            isCloseParent = IsApprovalForm == true ? "true" : "false";
            isCloseParent = isCloseParent.ToLower();

            switch (Action)
            {
                case "add":
                    layFilter = "btnSave";
                    break;

                case "modify":
                    layFilter = "btnModify";
                    break;

                case "close":
                    layFilter = "btnClose";
                    break;
            }

            jsFormSubmitCode += "form.on('submit(" + layFilter + ")', function(data) {";
            jsFormSubmitCode += "var Fields = JSON.stringify(data.field); ";  //获取提交的字段
            jsFormSubmitCode += "Fields = Fields.replace(/ctl00\\$ContentPlaceHolder1\\$/g, \"\"); ";

            //jsFormSubmitCode += "alert(Fields); ";
            //jsFormSubmitCode += "console.log(data.field);"; //当前容器的全部表单字段，名值对形式：{name: value}
            //jsFormSubmitCode += "return false; ";

            jsFormSubmitCode += "Fields = encodeURI(Fields); ";
            jsFormSubmitCode += "var micro = layui.micro, action=\"" + Action + "\", stn=\"" + ShortTableName + "\" , mid=\"" + MID + "\", formid=\"" + FormID + "\", isapprovalform=\"" + IsApprovalForm + "\";";
            jsFormSubmitCode += "var Parameter = { \"action\": action, \"stn\":stn, \"mid\": mid, \"formid\": formid, \"isapprovalform\": isapprovalform, \"fields\": Fields }; ";
            jsFormSubmitCode += "micro.mAjax('text', '/App_Handler/CtrlMicroForm.ashx', Parameter, true, " + IsRefresh.toStringTrim().ToLower() + ", " + isCloseParent + "); ";
            jsFormSubmitCode += "});";

            flag = jsFormSubmitCode;

            return flag;
        }

        /// <summary>
        /// 传入表名和主键获取数据记录值返回DataTable，通常用于修改表单或查看表单的情况下，在生成表单控件时同时把记录值赋值给控件
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <param name="PrimaryKeyValue">主键值</param>
        /// <returns>返回DataTable</returns>
        public static DataTable GetDataTable(string TableName, string PrimaryKeyValue)
        {
            DataTable _dt = null;

            var getTableAttr = MicroDataTable.GetTableAttr(TableName);  //获得主键字段名称
            string PrimaryKeyName = getTableAttr.PrimaryKeyName;  //主键字段名称
            string PrimaryKeyDataType = getTableAttr.DataType;   //主键字段类型
            string PrimaryKeyFieldLength = getTableAttr.FieldLength;  //主键字段长度（当主键数据类型为string时用到）

            if (!string.IsNullOrEmpty(TableName) && !string.IsNullOrEmpty(PrimaryKeyValue))
            {
                string Invalid = " and Invalid=0 ";
                if (MicroUserInfo.CheckUserRole("Administrators"))
                    Invalid = "";

                PrimaryKeyValue = PrimaryKeyValue.toStringTrim();

                string _sql = "select * from " + TableName + " where " + PrimaryKeyName + "=@" + PrimaryKeyName + " and Del=0 " + Invalid;
                SqlParameter[] _sp = new SqlParameter[1];

                //PrimaryKeyValue（每条记录的唯一ID）,也有可能是字符串（表单编号等）
                switch (PrimaryKeyDataType)
                {

                    case "int":
                        //尝试进行数字转换，成功则已，失败则是字符串
                        _sp[0] = new SqlParameter("@" + PrimaryKeyName, SqlDbType.Int);
                        _sp[0].Value = PrimaryKeyValue.toInt();
                        break;

                    case "varchar":
                        _sp[0] = new SqlParameter("@" + PrimaryKeyName, SqlDbType.VarChar, PrimaryKeyFieldLength.toInt());
                        _sp[0].Value = PrimaryKeyValue;
                        break;
                }

                _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

            }

            return _dt;
        }

        /// <summary>
        /// 提交表单，插入记录
        /// </summary>
        /// <param name="ShortTableName">短表名</param>
        /// <param name="FormFields">表单字段名/表单控件名</param>
        /// <returns></returns>
        public static string SetSubmitForm(string Action, string ShortTableName, string ModuleID, string FormID, Boolean IsApprovalForm, string FormFields)
        {
            string flag = string.Empty, Fields = string.Empty, Values = string.Empty;

            try
            {
                string TableName = MicroPublic.GetTableName(ShortTableName);   //短表名得到长表名（即真实表名）

                List<string> fieldNameList = new List<string>(); //创建List，用于存放表字段名称
                List<string> ctlValueList = new List<string>();  //创建List，用于存放动态json的变量对应的值(即从表单提交过来控件的值)
                List<string> dataTypeList = new List<string>();  //创建List，用于存放字段类型
                List<string> fieldLengthList = new List<string>();  //创建List，用于存放表字段长度，通常只有文本类型才有字段长度
                List<string> ctlExtraFunction = new List<string>();  //创建List，用于存放控件额外函数

                //string jso = "{\"txtTitle0\":\"666\",\"txtTitle\":\"666\",\"txtLogo\":\"logo\",\"txtFoot\":\"foot\"}";  //模拟数据
                dynamic json = JToken.Parse(FormFields) as dynamic;  //json转换

                //查询得到组别
                string _sql = "select ctlGroup from MicroTables where ParentID in (select TID from MicroTables where TabColName = @TableName) and Del=0 group by ctlGroup order by ctlGroup";
                SqlParameter[] _sp = { new SqlParameter("@TableName", SqlDbType.VarChar, 100) };
                _sp[0].Value = TableName;
                DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

                //再按组别进行查询得该组记录
                string _sql2 = "select * from MicroTables where ParentID in (select TID from MicroTables where TabColName = @TableName) and Del=0";
                SqlParameter[] _sp2 = { new SqlParameter("@TableName", SqlDbType.VarChar, 100) };
                _sp2[0].Value = TableName;
                DataTable _dt2 = MsSQLDbHelper.Query(_sql2, _sp2).Tables[0];

                DataRow[] _rows = _dt.Select();

                //按组进行循环查询，得到该组的内容
                foreach (DataRow _dr in _rows)
                {
                    DataRow[] _rows2 = _dt2.Select("ctlGroup=" + _dr["ctlGroup"].toInt() + " and Invalid=0 and PrimaryKey=0 and ctlAddDisplay=1 ", "Sort");

                    if (_rows2.Length > 0)
                    {
                        foreach (DataRow _dr2 in _rows2)
                        {
                            string ctlPrefix = string.Empty,
                            TabColName = string.Empty,
                            ctlName = string.Empty,
                            ctlValue = string.Empty,
                            DataType = string.Empty,
                            FieldName = string.Empty,
                            FieldLength = string.Empty,
                            ExtraFunction = string.Empty;

                            ctlPrefix = _dr2["ctlPrefix"].toStringTrim();  //前缀
                            TabColName = _dr2["TabColName"].toStringTrim();  //数据库字段名
                            DataType = _dr2["DataType"].toStringTrim();  //数据类型
                            FieldName = TabColName;  //写入list用数据库字段名
                            FieldLength = _dr2["FieldLength"].toStringTrim();  //字段长度
                            ExtraFunction = _dr2["ctlExtraFunction"].toStringTrim();  //额外函数

                            ctlName = ctlPrefix + TabColName;  //获取控件的名称, 前缀+字段名组成
                            ctlValue = json[ctlName];  //获取表单提交控件的值

                            //判断数据库字段是否为父ID，如果是父ID且没有值的时候赋予默认值为0
                            if (TabColName == "ParentID")
                                ctlValue = string.IsNullOrEmpty(ctlValue) == true ? "0" : ctlValue;

                            fieldNameList.Add(FieldName);  //获取数据库的字段名称写入List<string>
                            ctlValueList.Add(ctlValue);  //将控件的值写入List<string>
                            dataTypeList.Add(DataType);  //获取该字段在数据库的类型写入List<string>
                            fieldLengthList.Add(FieldLength = string.IsNullOrEmpty(FieldLength) == true ? "0" : FieldLength);  //获取该字段在数据库的长度写入List<string>
                            ctlExtraFunction.Add(ExtraFunction);

                            Fields += TabColName + ",";  //构造sql语句字符串+=（字段部分）
                            Values += "@" + TabColName + ","; //构造sql语句字符串+=（值部分）
                        }
                    }
                }

                //构成字符串后去除最后的逗号
                Fields = string.IsNullOrEmpty(Fields) == true ? string.Empty : Fields.Substring(0, Fields.Length - 1);
                Values = string.IsNullOrEmpty(Values) == true ? string.Empty : Values.Substring(0, Values.Length - 1);

                //构造sql语句字符串
                string _sql3 = "insert into " + TableName + " (" + Fields + ") values (" + Values + ")";

                //构造sql参数
                SqlParameter[] _sp3 = new SqlParameter[fieldNameList.Count];
                for (int i = 0; i < fieldNameList.Count; i++)
                {
                    //调用子函数判断sql数据类型、长度及赋值，返回SqlParameter
                    _sp3[i] = GetSqlParameter(Action, _sp3, dataTypeList[i], i, fieldNameList[i], ctlValueList[i], fieldLengthList[i], ctlExtraFunction[i], ShortTableName, TableName, FormID);
                }

                //执行Sql语句返回受影响行数
                if (MsSQLDbHelper.ExecuteSql(_sql3, _sp3) > 0)
                {
                    //提交表单插入记录成功后，判断表单是否需要审批
                    //判断是否为审批类型表单，如果是要走审批流程
                    if (IsApprovalForm)
                    {
                        ////需要审批时提交审批，把审批节点写进审批记录表（在上面先判断表单是否为审批类型的表单，是审批类型表单时再判断是否启用了审批，即使没有启用审批也有“提交”和“完成”这两步）
                        //写入审批记录表，写入成功返回True
                        if (MicroWorkFlow.SetWorkFlow(ShortTableName, TableName, ModuleID, FormID, "", FormFields))
                            flag = MicroPublic.GetMsg("Save");
                        else
                            flag = MicroPublic.GetMsg("SaveWolrkFlowFailed");
                    }
                    else  //非审批表单，直接提交成功
                        flag = MicroPublic.GetMsg("Save");
                }
                else  //提交失败（插入数据受影响记录数据为0）
                    flag = MicroPublic.GetMsg("SaveFailed");
            }
            catch (Exception e) { flag = MicroPublic.GetMsg("SaveFailedTry") + "<br/> 详细错误：" + e.ToString(); }

            return flag;
        }

        /// <summary>
        /// 提交表单，修改记录
        /// </summary>
        /// <returns></returns>
        public static string SetModifyForm(string Action, string ShortTableName, string ModuleID, string FormID, Boolean IsApprovalForm, string FormFields)
        {
            string flag = string.Empty, FieldsValues = string.Empty, PrimaryKeyValue = string.Empty, ctlDisplay = string.Empty;

            try
            {
                switch (Action)
                {
                    case "modify":  //在修改表单状态下显示该控件
                        ctlDisplay = " and ctlModifyDisplay=1 ";
                        break;
                    case "close":  //结案时显示该控件
                        ctlDisplay = " and ctlAfterDisplay=1 ";
                        break;
                }

                string TableName = MicroPublic.GetTableName(ShortTableName);   //短表名得到长表名（即真实表名）

                List<string> fieldNameList = new List<string>(); //创建List，用于存放表字段名称
                List<string> ctlValueList = new List<string>();  //创建List，用于存放动态json的变量对应的值(即从表单提交过来控件的值)
                List<string> dataTypeList = new List<string>();  //创建List，用于存放字段类型
                List<string> fieldLengthList = new List<string>();  //创建List，用于存放表字段长度，通常只有文本类型才有字段长度
                List<string> ctlExtraFunction = new List<string>();  //创建List，用于存放控件额外函数

                //string json = "{\"txtTitle0\":\"666\",\"txtTitle\":\"666\",\"txtLogo\":\"logo\",\"txtFoot\":\"foot\"}";  //模拟数据
                dynamic json = JToken.Parse(FormFields) as dynamic;  //json转换

                //查询得到组别
                string _sql = "select ctlGroup from MicroTables where ParentID in (select TID from MicroTables where TabColName = @TableName) and Del=0 group by ctlGroup order by ctlGroup";
                SqlParameter[] _sp = { new SqlParameter("@TableName", SqlDbType.VarChar, 100) };
                _sp[0].Value = TableName;
                DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

                //再按组别进行查询得该组记录
                string _sql2 = "select * from MicroTables where ParentID in (select TID from MicroTables where TabColName = @TableName) and Del=0 " + ctlDisplay + "";
                SqlParameter[] _sp2 = { new SqlParameter("@TableName", SqlDbType.VarChar, 100) };
                _sp2[0].Value = TableName;
                DataTable _dt2 = MsSQLDbHelper.Query(_sql2, _sp2).Tables[0];

                DataRow[] _rows = _dt.Select();

                //按组进行循环查询，得到该组的内容
                foreach (DataRow _dr in _rows)
                {
                    DataRow[] _rows2 = _dt2.Select("ctlGroup=" + _dr["ctlGroup"].toInt() + " and Invalid=0 and PrimaryKey=0", "Sort");

                    if (_rows2.Length > 0)
                    {
                        foreach (DataRow _dr2 in _rows2)
                        {
                            string ctlPrefix = string.Empty,
                            TabColName = string.Empty,
                            ctlName = string.Empty,
                            ctlValue = string.Empty,
                            DataType = string.Empty,
                            FieldName = string.Empty,
                            FieldLength = string.Empty,
                            ExtraFunction = string.Empty;

                            ctlPrefix = _dr2["ctlPrefix"].toStringTrim();  //前缀
                            TabColName = _dr2["TabColName"].toStringTrim();  //数据库字段名
                            DataType = _dr2["DataType"].toStringTrim();  //数据类型
                            FieldName = TabColName;  //写入list用数据库字段名
                            FieldLength = _dr2["FieldLength"].toStringTrim();  //字段长度
                            ExtraFunction = _dr2["ctlExtraFunction"].toStringTrim();  //额外函数

                            ctlName = ctlPrefix + TabColName;  //获取控件的名称, 前缀+字段名组成
                            ctlValue = json[ctlName];  //获取动态控件的值

                            fieldNameList.Add(FieldName);  //获取数据库的字段名称写入List<string>
                            ctlValueList.Add(ctlValue);  //将控件的值写入List<string>
                            dataTypeList.Add(DataType);  //获取该字段在数据库的类型写入List<string>
                            fieldLengthList.Add(FieldLength = string.IsNullOrEmpty(FieldLength) == true ? "0" : FieldLength);  //获取该字段在数据库的长度写入List<string>
                            ctlExtraFunction.Add(ExtraFunction);

                            FieldsValues += TabColName + " = @" + TabColName + ","; //构造sql语句字符串+=（字段部分）
                        }
                    }
                }

                //声明var变量取得主键的属性
                var getTableAttr = MicroDataTable.GetTableAttr(TableName);  //调用方法返回主键属性
                string PrimaryKeyName = getTableAttr.PrimaryKeyName;  //赋值，主键字段名称
                string PrimaryKeyDataType = getTableAttr.DataType;   //赋值，主键字段类型
                string PrimaryKeyFieldLength = getTableAttr.FieldLength;  //赋值，主键字段长度（当主键数据类型为string时用到）
                string CtlPrefix = getTableAttr.CtlPrefix; //赋值，控件前缀

                //构成字符串后去除最后的逗号
                FieldsValues = string.IsNullOrEmpty(FieldsValues) == true ? string.Empty : FieldsValues.Substring(0, FieldsValues.Length - 1);

                //构造sql语句字符串
                string _sql3 = "update " + TableName + " set " + FieldsValues + " where " + PrimaryKeyName + " = @" + PrimaryKeyName + "";

                //构造sql参数
                int spCount = fieldNameList.Count + 1;  // +1 用于Where参数
                SqlParameter[] _sp3 = new SqlParameter[spCount];
                for (int i = 0; i < fieldNameList.Count; i++)
                {
                    //调用子函数判断sql数据类型、长度及赋值，返回SqlParameter
                    _sp3[i] = GetSqlParameter(Action, _sp3, dataTypeList[i], i, fieldNameList[i], ctlValueList[i], fieldLengthList[i], ctlExtraFunction[i], ShortTableName, TableName, FormID);
                }

                //构造主键的SqlParameter参数
                PrimaryKeyValue = json[CtlPrefix + PrimaryKeyName]; //取得主键的值
                switch (PrimaryKeyDataType)
                {
                    case "int":
                        _sp3[fieldNameList.Count] = new SqlParameter("@" + PrimaryKeyName, SqlDbType.Int);
                        _sp3[fieldNameList.Count].Value = PrimaryKeyValue.toInt();
                        break;

                    case "varchar":
                        _sp3[fieldNameList.Count] = new SqlParameter("@" + PrimaryKeyName, SqlDbType.VarChar, PrimaryKeyFieldLength.toInt());
                        _sp3[fieldNameList.Count].Value = PrimaryKeyValue.toStringTrim();
                        break;
                }

                //执行Sql语句返回受影响行数
                if (MsSQLDbHelper.ExecuteSql(_sql3, _sp3) > 0)
                {
                    //flag = MicroPublic.GetMsg("Modify");

                    //提交表单插入记录成功后，判断是否为审批表单，是审批表单的话重置审批状态
                    if (Action == "modify")  //在修改情况下重置表单状态
                    {
                        if (IsApprovalForm)
                        {
                            //需要审批时提交审批，把审批节点写进审批记录表（在上面先判断表单是否为审批类型的表单，是审批类型表单时再判断是否启用了审批，即使没有启用审批也有“提交”和“完成”这两步）
                            //写入审批记录表，写入成功返回True
                            if (MicroWorkFlow.SetModifyWorkFlow(ShortTableName, TableName, ModuleID, FormID, PrimaryKeyValue, FormFields))
                                flag = MicroPublic.GetMsg("Save");
                            else
                                flag = MicroPublic.GetMsg("SaveWolrkFlowFailed");
                        }
                        else  //非审批表单，直接提交成功
                            flag = MicroPublic.GetMsg("Save");
                    }
                    else if (Action == "close")  //在结案情况下
                    {
                        //更新表单状态
                        //更新审批记录表状态
                        //写入日志

                        //在结案情况下进行以上操作把结果返回到（flag="True"） 在CtrlMicroForm.ashx页面进行处理
                        flag = "True";

                    }
                }
                else
                    flag = MicroPublic.GetMsg("ModifyFailedTry");

            }
            catch (Exception e) { flag = MicroPublic.GetMsg("ModifyFailedTry") + "<br/> 详细错误：" + e.ToString(); }

            return flag;
        }

        /// <summary>
        /// 返回已构造好的SqlParameter
        /// </summary>
        /// <param name="_sp">SqlParameter</param>
        /// <param name="DataType">DataType</param>
        /// <param name="i">序号</param>
        /// <param name="FieldName">字段名称</param>
        /// <param name="FieldValue">值</param>
        /// <param name="FieldLength">长度</param>
        /// <returns></returns>
        private static SqlParameter GetSqlParameter(string Action, SqlParameter[] _sp, string DataType, int i, string FieldName, string FieldValue, string FieldLength, string ExtraFunction, string ShortTableName, string TableName, string FormID)
        {
            FieldValue = FieldValue.toStringTrim();
            switch (DataType)
            {
                case "bit":
                    _sp[i] = new SqlParameter("@" + FieldName, SqlDbType.Bit);

                    if (string.IsNullOrEmpty(FieldValue))
                        _sp[i].Value = null;
                    else
                        _sp[i].Value = MicroPublic.GetBuiltinFunction(Action, FieldValue, ExtraFunction, ShortTableName, TableName, FormID).toBoolean();

                    break;
                case "char":
                    _sp[i] = new SqlParameter("@" + FieldName, SqlDbType.Char, FieldLength.toInt());

                    if (string.IsNullOrEmpty(FieldValue))
                        _sp[i].Value = null;
                    else
                        _sp[i].Value = MicroPublic.GetBuiltinFunction(Action, FieldValue, ExtraFunction, ShortTableName, TableName, FormID);

                    break;
                case "datetime":
                    _sp[i] = new SqlParameter("@" + FieldName, SqlDbType.DateTime);

                    if (string.IsNullOrEmpty(FieldValue))
                        _sp[i].Value = null;
                    else
                        _sp[i].Value = MicroPublic.GetBuiltinFunction(Action, FieldValue, ExtraFunction, ShortTableName, TableName, FormID).toDateTime("yyyy-MM-dd HH:mm:ss.fff");

                    break;
                case "decimal":
                    _sp[i] = new SqlParameter("@" + FieldName, SqlDbType.Decimal);

                    if (string.IsNullOrEmpty(FieldValue))
                        _sp[i].Value = null;
                    else
                        _sp[i].Value = FieldValue.toDecimal();

                    break;
                case "float":
                    _sp[i] = new SqlParameter("@" + FieldName, SqlDbType.Float);

                    if (string.IsNullOrEmpty(FieldValue))
                        _sp[i].Value = null;
                    else
                        _sp[i].Value = FieldValue.toFloat();

                    break;
                case "int":
                    _sp[i] = new SqlParameter("@" + FieldName, SqlDbType.Int);
                    if (FieldName.ToLower() == "parentid")
                        _sp[i].Value = FieldValue.toInt();
                    else
                    {
                        if (string.IsNullOrEmpty(FieldValue))
                            _sp[i].Value = null;
                        else
                            _sp[i].Value = FieldValue.toInt();
                    }
                    break;
                case "nchar":
                    _sp[i] = new SqlParameter("@" + FieldName, SqlDbType.NChar, FieldLength.toInt());

                    if (string.IsNullOrEmpty(FieldValue))
                        _sp[i].Value = null;
                    else
                        _sp[i].Value = MicroPublic.GetBuiltinFunction(Action, FieldValue, ExtraFunction, ShortTableName, TableName, FormID);

                    break;
                case "ntext":
                    _sp[i] = new SqlParameter("@" + FieldName, SqlDbType.NText);

                    if (string.IsNullOrEmpty(FieldValue))
                        _sp[i].Value = null;
                    else
                        _sp[i].Value = MicroPublic.GetBuiltinFunction(Action, FieldValue, ExtraFunction, ShortTableName, TableName, FormID);   //HttpContext.Current.Server.HtmlEncode(FieldValue)

                    break;
                case "nvarchar":
                    _sp[i] = new SqlParameter("@" + FieldName, SqlDbType.NVarChar, FieldLength.toInt());

                    if (string.IsNullOrEmpty(FieldValue))
                        _sp[i].Value = null;
                    else
                        _sp[i].Value = MicroPublic.GetBuiltinFunction(Action, FieldValue, ExtraFunction, ShortTableName, TableName, FormID);

                    break;
                case "text":
                    _sp[i] = new SqlParameter("@" + FieldName, SqlDbType.Text);

                    if (string.IsNullOrEmpty(FieldValue))
                        _sp[i].Value = null;
                    else
                        _sp[i].Value = MicroPublic.GetBuiltinFunction(Action, FieldValue, ExtraFunction, ShortTableName, TableName, FormID);

                    break;
                case "varchar":
                    _sp[i] = new SqlParameter("@" + FieldName, SqlDbType.VarChar, FieldLength.toInt());

                    if (string.IsNullOrEmpty(FieldValue))
                        _sp[i].Value = null;
                    else
                        _sp[i].Value = MicroPublic.GetBuiltinFunction(Action, FieldValue, ExtraFunction, ShortTableName, TableName, FormID);

                    break;
            }
            return _sp[i];
        }



        /// <summary>
        /// 获取FormNumber
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="FormID"></param>
        /// <returns></returns>
        public static string GetFormNumber(string TableName, string FormID)
        {

            string flag = string.Empty, FormNumber = string.Empty, FormNumberPrefix = string.Empty;
            string Prefix = string.Empty, Format = string.Empty, FormatValue = string.Empty;
            int SerialNumberLength = 0, SerialNumber = 0;

            string _sql = "select * from FormsNumber where Invalid=0 and Del=0 and FormID=@FormID order by DateCreated desc";
            SqlParameter[] _sp = { new SqlParameter("@FormID", SqlDbType.Int) };
            _sp[0].Value = FormID.toInt();
            DataTable _dt = MsSQLDbHelper.Query(_sql, _sp).Tables[0];

            var getFormAttr = MicroForm.GetFormAttr(TableName, FormID);  //TableName本应是ShorTableName但这里没有，又因为FormID不为空以FormID为准
            FormNumberPrefix = getFormAttr.FormNumberPrefix;

            //有记录时
            if (_dt.Rows.Count > 0)
            {
                int FNID = _dt.Rows[0]["FNID"].toInt();

                //表单表Forms中的FormNumberPrefix字段，如果不为空则优先取该值，否则读取FormsNumber表中的Prefix，假如Prefix值也为空，则取表单对应的表名的前两位
                if (string.IsNullOrEmpty(FormNumberPrefix))
                {
                    Prefix = _dt.Rows[0]["Prefix"].toStringTrim();
                    Prefix = string.IsNullOrEmpty(Prefix) == true ? (TableName.Substring(0, 2)).ToUpper() : Prefix.ToUpper();  //Prefix为空时取TableName左边两位
                }
                else
                    Prefix = FormNumberPrefix;

                Format = _dt.Rows[0]["Format"].toStringTrim();
                Format = string.IsNullOrEmpty(Format) == true ? "yyyyMM" : Format;  //Format为空时，默认赋值为yyyyMM

                FormatValue = _dt.Rows[0]["FormatValue"].toStringTrim();
                FormatValue = string.IsNullOrEmpty(FormatValue) == true ? DateTime.Now.ToString(Format) : FormatValue;  //FormatValue，默认赋值DateTime.Now.ToString(Format)

                SerialNumberLength = _dt.Rows[0]["SerialNumberLength"].toInt();  //得到流水号的长度，如：等于0值时，默认赋值4
                SerialNumberLength = SerialNumberLength == 0 ? 4 : SerialNumberLength; //长度=0时，默认赋值4

                SerialNumber = _dt.Rows[0]["SerialNumber"].toInt();
                SerialNumber = SerialNumber == 0 ? 1 : SerialNumber;  //SerialNumber等于0时，赋值为1 ，因为0000不能作为编号

                //变量值和当前时间相等时
                if (DateTime.Now.ToString(Format) == FormatValue)
                {
                    int _SerialNumber = SerialNumber.toStringTrim().Length;  //得到流水号的长度，如：88代表两位

                    //计算流水号补码
                    string Complement = string.Empty;
                    if ((SerialNumberLength - _SerialNumber) > 0)
                    {
                        for (int i = 0; i < (SerialNumberLength - _SerialNumber); i++)
                        {
                            Complement += "0";
                        }
                    }
                    //构造好FormNumber
                    FormNumber = Prefix + FormatValue + Complement + SerialNumber;

                    //*****同时把流水号+1后更新回数据库Start*****
                    string _sql3 = "update FormsNumber set SerialNumber=@SerialNumber where FormID=@FormID and FNID=@FNID";
                    SqlParameter[] _sp3 = { new SqlParameter("@SerialNumber",SqlDbType.Int),
                                            new SqlParameter("@FormID",SqlDbType.Int),
                                            new SqlParameter("@FNID",SqlDbType.Int)
                    };
                    _sp3[0].Value = (SerialNumber + 1);
                    _sp3[1].Value = FormID.toInt();
                    _sp3[2].Value = FNID;

                    MsSQLDbHelper.ExecuteSql(_sql3, _sp3);
                    //*****同时把流水号+1后更新回数据库End*****

                }
                //变量值和当前时间不相等时
                else
                {
                    FormatValue = DateTime.Now.ToString(Format);
                    SerialNumber = 1;
                    int _SerialNumber = SerialNumber.toStringTrim().Length;  //得到流水号的长度，如：88代表两位

                    //计算流水号补码
                    string Complement = string.Empty;
                    if ((SerialNumberLength - _SerialNumber) > 0)
                    {
                        for (int i = 0; i < (SerialNumberLength - _SerialNumber); i++)
                        {
                            Complement += "0";
                        }
                    }

                    //构造好FormNumber
                    FormNumber = Prefix + FormatValue + Complement + SerialNumber;

                    //*****同时把流水号+1后更新回数据库Start*****
                    string _sql4 = "insert into FormsNumber (FormID, Prefix, Format, FormatValue, SerialNumberLength, SerialNumber) values (@FormID, @Prefix, @Format, @FormatValue, @SerialNumberLength, @SerialNumber)";
                    SqlParameter[] _sp4 = { new SqlParameter("@FormID",SqlDbType.Int),
                                            new SqlParameter("@Prefix",SqlDbType.VarChar,50),
                                            new SqlParameter("@Format",SqlDbType.VarChar,50),
                                            new SqlParameter("@FormatValue",SqlDbType.VarChar,50),
                                            new SqlParameter("@SerialNumberLength",SqlDbType.Int),
                                            new SqlParameter("@SerialNumber",SqlDbType.Int),
                    };
                    _sp4[0].Value = FormID.toInt();
                    _sp4[1].Value = Prefix;
                    _sp4[2].Value = Format;
                    _sp4[3].Value = FormatValue;
                    _sp4[4].Value = SerialNumberLength;
                    _sp4[5].Value = SerialNumber + 1;

                    MsSQLDbHelper.ExecuteSql(_sql4, _sp4);
                    //*****同时把流水号+1后更新回数据库End*****
                }

            }
            //没有记录时
            else
            {
                if (!string.IsNullOrEmpty(FormNumberPrefix))
                    Prefix = FormNumberPrefix;
                else
                    Prefix = (TableName.Substring(0, 2)).ToUpper();  //Prefix为空时取TableName左边两位

                Format = "yyyyMM";  //Format为空时，默认赋值为yyyyMM
                FormatValue = DateTime.Now.ToString(Format);  //FormatValue，默认赋值DateTime.Now.ToString(Format)
                SerialNumberLength = 4; //长度=0时，默认赋值4
                SerialNumber = 1;  //赋值为1 ，因为0000不能作为编号

                int _SerialNumber = SerialNumber.toStringTrim().Length;  //得到流水号的长度，如：88代表两位

                //计算流水号补码
                string Complement = string.Empty;
                if ((SerialNumberLength - _SerialNumber) > 0)
                {
                    for (int i = 0; i < (SerialNumberLength - _SerialNumber); i++)
                    {
                        Complement += "0";
                    }
                }

                //构造好FormNumber
                FormNumber = Prefix + FormatValue + Complement + SerialNumber;

                //*****同时把流水号+1后更新回数据库Start*****
                string _sql5 = "insert into FormsNumber (FormID, Prefix, Format, FormatValue, SerialNumberLength, SerialNumber) values (@FormID, @Prefix, @Format, @FormatValue, @SerialNumberLength, @SerialNumber)";
                SqlParameter[] _sp5 = { new SqlParameter("@FormID",SqlDbType.Int),
                                            new SqlParameter("@Prefix",SqlDbType.VarChar,50),
                                            new SqlParameter("@Format",SqlDbType.VarChar,50),
                                            new SqlParameter("@FormatValue",SqlDbType.VarChar,50),
                                            new SqlParameter("@SerialNumberLength",SqlDbType.Int),
                                            new SqlParameter("@SerialNumber",SqlDbType.Int),
                    };
                _sp5[0].Value = FormID.toInt();
                _sp5[1].Value = Prefix;
                _sp5[2].Value = Format;
                _sp5[3].Value = FormatValue;
                _sp5[4].Value = SerialNumberLength;
                _sp5[5].Value = SerialNumber + 1;

                MsSQLDbHelper.ExecuteSql(_sql5, _sp5);
                //*****同时把流水号+1后更新回数据库End*****

            }

            flag = FormNumber;
            return flag;

        }


        /// <summary>
        /// 获取表单属性
        /// </summary>
        public class FormAttr
        {

            public int FormID
            {
                set;
                get;
            }

            public string ShortTableName
            {
                set;
                get;
            }

            /// <summary>
            /// 表单名称
            /// </summary>
            public string FormName
            {
                set;
                get;
            }

            /// <summary>
            /// 表单所属部门ID
            /// </summary>
            public int DeptID
            {
                set;
                get;
            }

            /// <summary>
            /// 表单所属部门名称
            /// </summary>
            public string DeptName
            {
                set;
                get;
            }

            /// <summary>
            /// 表单描述
            /// </summary>
            public string Description
            {
                set;
                get;
            }

            /// <summary>
            /// 表单唯一序列号或ISO号码
            /// </summary>
            public string SerialNumber
            {
                set;
                get;
            }

            /// <summary>
            /// 是否审批表单
            /// </summary>
            public Boolean IsApproval
            {
                set;
                get;
            }

            /// <summary>
            /// 在非审批表单时，判断是否需要受理节点
            /// </summary>
            public Boolean IsAccept
            {
                set;
                get;
            }

            /// <summary>
            /// 审批提示内容，该表单不需要审批或审批流程提示：申请-->科长审批-->部长审批-->IT存档，等
            /// </summary>
            public string ApprovalTips
            {
                set;
                get;
            }

            /// <summary>
            /// 按申请类型分配任务
            /// </summary>
            public Boolean AssignTasksByAppType
            {
                set;
                get;
            }

            /// <summary>
            /// 最后一位审批者通过时，是否自动受理
            /// </summary>
            public Boolean AutoAccept
            {
                set;
                get;
            }

            /// <summary>
            /// 最后一位审批者通过时，是否自动结案
            /// </summary>
            public Boolean AutoClose
            {
                set;
                get;
            }

            /// <summary>
            /// 表单接收窗口，存放的是逗号分隔的UID
            /// </summary>
            public string ReceiveWindow
            {
                set;
                get;
            }

            /// <summary>
            /// 表单编号前缀
            /// </summary>
            public string FormNumberPrefix
            {
                set;
                get;
            }

            /// <summary>
            /// 备注
            /// </summary>
            public string Note
            {
                set;
                get;
            }
        }


        /// <summary>
        /// 获取表单属性，如：表单名称，序列号，描述，是否审批表单，是否有受理节点，是否自动结案等
        /// </summary>
        /// <param name="ShortTableName"></param>
        /// <param name="FormID"></param>
        /// <returns></returns>
        public static FormAttr GetFormAttr(string ShortTableName, string FormID = "")
        {
            string shortTableName = string.Empty, formName = string.Empty, deptName = string.Empty, description = string.Empty, serialNumber = string.Empty, approvalTips = string.Empty, receiveWindow = string.Empty, formNumberPrefix = string.Empty, note = string.Empty;
            int formID = 0, deptID = 0;
            Boolean isApproval = false, isAccept = false, assignTasksByAppType = false, autoAccept = false, autoClose = false;

            DataTable _dt = MicroDataTable.GetDataTable("Forms");
            if (_dt != null)
            {
                DataRow[] _rows = _dt.Select("ShortTableName='" + ShortTableName + "'");

                if (!string.IsNullOrEmpty(FormID))
                    _rows = _dt.Select("FormID=" + FormID);

                if (_rows.Length > 0)
                {
                    formID = _rows[0]["FormID"].toInt();
                    formName = _rows[0]["FormName"].toStringTrim();
                    deptID = _rows[0]["DeptID"].toInt();
                    description = _rows[0]["Description"].toStringTrim();
                    serialNumber = _rows[0]["SerialNumber"].toStringTrim();
                    isApproval = _rows[0]["IsApproval"].toBoolean();
                    isAccept = _rows[0]["IsAccept"].toBoolean();
                    approvalTips = _rows[0]["ApprovalTips"].toStringTrim();
                    assignTasksByAppType = _rows[0]["AssignTasksByAppType"].toBoolean();
                    autoAccept = _rows[0]["AutoAccept"].toBoolean();
                    autoClose = _rows[0]["AutoClose"].toBoolean();
                    receiveWindow = _rows[0]["ReceiveWindow"].toStringTrim();
                    formNumberPrefix = _rows[0]["FormNumberPrefix"].toStringTrim();
                    note = _rows[0]["Note"].toStringTrim();
                }

            }
            var FormAttr = new FormAttr
            {
                FormID = formID,
                ShortTableName = shortTableName,
                FormName = formName,
                DeptID = deptID,
                DeptName = deptName,
                Description = description,
                SerialNumber = serialNumber,
                IsApproval = isApproval,
                IsAccept = isAccept,
                ApprovalTips = approvalTips,
                AssignTasksByAppType = assignTasksByAppType,
                AutoAccept = autoAccept,
                AutoClose = autoClose,
                ReceiveWindow = receiveWindow,
                FormNumberPrefix = formNumberPrefix,
                Note = note
            };

            return FormAttr;

        }


        /// <summary>
        ///  获取表单工作流步骤HtmlCode。从表单审批记录表获得审批记录作为步骤，如：申请->审批->受理->完成
        /// </summary>
        /// <param name="FormID"></param>
        /// <param name="FormsID"></param>
        /// <returns></returns>
        public static string GetFormWorkFlowStepsHtmlCode(string FormID, string FormsID)
        {
            string htmlCode = string.Empty;

            string _sqlStep = "select Sort,NodeName,CanApprovalUID,ApprovalUID,StateCode from FormApprovalRecords where FormID=@FormID and FormsID=@FormsID and Invalid=0 and Del=0 order by Sort";
            SqlParameter[] _spStep = { new SqlParameter("@FormID",SqlDbType.Int),
                                                new SqlParameter("@FormsID",SqlDbType.Int)
                                                };
            _spStep[0].Value = FormID.toInt();
            _spStep[1].Value = FormsID.toInt();
            DataTable _dtStep = MsSQLDbHelper.Query(_sqlStep, _spStep).Tables[0];

            if (_dtStep.Rows.Count > 0)
            {
                string _sqlUsers = "select UID,UserName,ChineseName,EnglishName,AdDisplayName from UserInfo where Invalid=0 and Del=0 ";
                DataTable _dtUsers = MsSQLDbHelper.Query(_sqlUsers).Tables[0];


                //UserName = _dr["UserName"].toJsonTrim();
                //ChineseName = _dr["ChineseName"].toJsonTrim();
                //EnglishName = _dr["EnglishName"].toJsonTrim();
                //AdDisplayName = _dr["AdDisplayName"].toJsonTrim();

                //DisplayName = MicroUserInfo.GetUserInfo(UserName, ChineseName, EnglishName, AdDisplayName);

                htmlCode += "<div class=\"ws-wrap\">";
                htmlCode += "<ul class=\"ws-step-case\">";

                for (int i = 0; i < _dtStep.Rows.Count; i++)
                {

                    int StateCode = _dtStep.Rows[i]["StateCode"].toInt(); //得到审批代码。审批状态：0 = 等待审批[Waiting]、1 = 审批通过[Pass]、-1 = 驳回申请[Return]、-2 = 临时保存[Draft]、11 = 提交申请[Pass]、22 = 等待受理[WaitingForAccept]、23 = 等待对应[WaitingForProcessing]、24 = 等待处理[WaitingForProcessing]、32 = 受理中[Accepting]、33 = 对应中[Processing]、34 = 处理中[Processing]、100 = 完成[Finish]

                    string CanApprovalUID = _dtStep.Rows[i]["CanApprovalUID"].toStringTrim(),
                    ApprovalUID = _dtStep.Rows[i]["ApprovalUID"].toStringTrim();

                    //可审批者
                    string CanApprovalDiaplayNames = string.Empty;
                    if (!string.IsNullOrEmpty(CanApprovalUID))
                    {
                        string UIDs = "0" + "," + CanApprovalUID;
                        DataRow[] _rowsCanApprovalUID = _dtUsers.Select("UID in (" + UIDs + ")");
                        if (_rowsCanApprovalUID.Length > 0)
                        {
                            CanApprovalDiaplayNames = "选定：<br/>";
                            foreach (DataRow _dr in _rowsCanApprovalUID)
                            {
                                string UserName = _dr["UserName"].toJsonTrim(),
                                  ChineseName = _dr["ChineseName"].toJsonTrim(),
                                  EnglishName = _dr["EnglishName"].toJsonTrim(),
                                  AdDisplayName = _dr["AdDisplayName"].toJsonTrim(),
                                  DisplayName = MicroUserInfo.GetUserInfo(UserName, ChineseName, EnglishName, AdDisplayName);

                                CanApprovalDiaplayNames += DisplayName + "<br/>";
                            }
                        }
                    }

                    //实际审批者
                    string ApprovalDiaplayNames = string.Empty;
                    if (!string.IsNullOrEmpty(ApprovalUID))
                    {
                        string UIDs = "0" + "," + ApprovalUID;
                        DataRow[] _rowsCanApprovalUID = _dtUsers.Select("UID in (" + UIDs + ")");
                        if (_rowsCanApprovalUID.Length > 0)
                        {
                            ApprovalDiaplayNames = "操作：<br/>";
                            foreach (DataRow _dr in _rowsCanApprovalUID)
                            {
                                string UserName = _dr["UserName"].toJsonTrim(),
                                  ChineseName = _dr["ChineseName"].toJsonTrim(),
                                  EnglishName = _dr["EnglishName"].toJsonTrim(),
                                  AdDisplayName = _dr["AdDisplayName"].toJsonTrim(),
                                  DisplayName = MicroUserInfo.GetUserInfo(UserName, ChineseName, EnglishName, AdDisplayName);

                                ApprovalDiaplayNames += DisplayName + "<br/>";
                            }
                        }
                    }


                    string wsApprovalColor = string.Empty; //审批颜色

                    if (StateCode == -4 || StateCode == -1 || StateCode == 15)
                        wsApprovalColor = "ws-return"; //审批驳回的颜色“红色”
                    else if (StateCode >= 1)
                        wsApprovalColor = "ws-finish"; //审批通过的颜色“绿色”


                    string b = string.Empty; //b元素，用于形成箭头
                    if (i != 0)
                        b = "<b class=\"b-1\"></b><b class=\"b-2\"></b>";


                    string layTips = "lay-tips=\"" + CanApprovalDiaplayNames + ApprovalDiaplayNames + "\"";

                    if (string.IsNullOrEmpty(CanApprovalDiaplayNames) && string.IsNullOrEmpty(ApprovalDiaplayNames))
                        layTips = "";

                    string Sort = _dtStep.Rows[i]["Sort"].toStringTrim();
                    string NodeName = _dtStep.Rows[i]["NodeName"].toStringTrim();
                    htmlCode += "<li class=\"" + wsApprovalColor + "\" " + layTips + " style=\"width:" + 100 / _dtStep.Rows.Count + "%;\"><span>" + Sort + "." + NodeName + "</span>" + b + "</li>";

                }

                htmlCode += "</ul>";
                htmlCode += "</div>";
            }

            return htmlCode;
        }

        /// <summary>
        /// 获取表单审批日志履历HtmlCode
        /// </summary>
        /// <param name="FormID"></param>
        /// <param name="FormsID"></param>
        /// <returns></returns>
        public static string GetFormApprovalLogsHtmlCode(string FormID, string FormsID)
        {
            string htmlCode = string.Empty;
            //***列2 Start*** 日志记录
            htmlCode += "<div class=\"layui-col-xs6 layui-col-sm3 layui-col-md2 layui-col-lg2\">";
            htmlCode += "<div class=\"ws-padding-left5 ws-margin-bottom10\" style=\"border-left:5px solid #009688; height:30px; line-height:30px;\">日志记录</div>";

            //从表单审批记录表获得审批记录作为步骤，在view模式时显示
            string _sqlLogs = "select * from FormApprovalLogs where FormID=@FormID and FormsID=@FormsID and Invalid=0 and Del=0 order by FALID desc ";
            SqlParameter[] _spLogs = { new SqlParameter("@FormID",SqlDbType.Int),
                                                new SqlParameter("@FormsID",SqlDbType.Int)
                                                };
            _spLogs[0].Value = FormID.toInt();
            _spLogs[1].Value = FormsID.toInt();
            DataTable _dtLogs = MsSQLDbHelper.Query(_sqlLogs, _spLogs).Tables[0];

            if (_dtLogs.Rows.Count > 0)
            {
                //操作履历
                htmlCode += "<ul class=\"layui-timeline \">";

                for (int i = 0; i < _dtLogs.Rows.Count; i++)
                {
                    string divFontColor = string.Empty,
                        wsFontColor = string.Empty;
                    if (i == 0)  //最后一条记录（即当前步骤），用高亮颜色显示
                        wsFontColor = "ws-font-green2";
                    else
                        divFontColor = "ws-font-gray3";


                    int StateCode = _dtLogs.Rows[i]["StateCode"].toInt();
                    if (StateCode == -1 || StateCode == -4 || StateCode == 15)  //状态为驳回时用红色显示，-1 = 驳回，-4 = 申请者撤回，15 = 审批者撤回
                        wsFontColor = "ws-font-red";

                    string Note = _dtLogs.Rows[i]["Note"].toStringTrim();
                    Note = string.IsNullOrEmpty(Note) == true ? "" : "备注：" + Note;

                    htmlCode += "<li class=\"layui-timeline-item\" >" +
                       "<i class=\"layui-icon layui-timeline-axis\"></i>" +
                       "<div class=\"layui-timeline-content layui-text " + divFontColor + "\">" +
                       "<h4 class=\"layui-timeline-title\">" + _dtLogs.Rows[i]["ApprovalTime"].toDateWeekTime() + "</h4>" +
                       "<p>" + _dtLogs.Rows[i]["NodeName"].toStringTrim() + "</p>" +
                       "<p>" + _dtLogs.Rows[i]["ApprovalDisplayName"].toStringTrim() + "<span class=\"ws-padding-left5 " + wsFontColor + "\">" + _dtLogs.Rows[i]["ApprovalState"].toStringTrim() + "</span></p>" +
                       "<p><span class=\"ws-padding-left5 " + wsFontColor + "\">" + Note + "</span></p>" +
                       "</div>" +
                       "</li>";
                }

                htmlCode += "</ul>";
            }

            htmlCode += "</div>";  //layui-col-xs6 layui-col-sm3 layui-col-md2 layui-col-lg2
            //***列2 End***

            return htmlCode;
        }


    }

}