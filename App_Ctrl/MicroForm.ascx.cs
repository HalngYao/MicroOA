using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MicroPublicHelper;
using MicroFormHelper;

public partial class App_Ctrl_MicroForm : System.Web.UI.UserControl
{

    /// <summary>
    /// 表单动作 add/modify
    /// </summary>
    private string action = "add";
    public string Action
    {
        set { action = value; }
        get { return action; }
    }

    private string shortTableName = "sys";
    public string ShortTableName
    {
        set { shortTableName = value; }
        get { return shortTableName; }
    }

    /// <summary>
    /// 模块ID
    /// </summary>
    private string moduleID = "1";
    public string ModuleID
    {
        set { moduleID = value; }
        get { return moduleID; }
    }

    /// <summary>
    /// FormID
    /// </summary>
    private string formID = "1";
    public string FormID
    {
        set { formID = value; }
        get { return formID; }
    }

    /// <summary>
    /// 主键值，通常是在修改（即Action=Modify）的情况下需要传入的参数
    /// </summary>
    private string primaryKeyValue = "0";
    public string PrimaryKeyValue
    {
        set { primaryKeyValue = value; }
        get { return primaryKeyValue; }
    }

    /// <summary>
    /// 返回表单的JS代码，可选值有all、vcode、ccode、scode
    /// </summary>
    private string formCode = "all";
    public string FormCode
    {
        set { formCode = value; }
        get { return formCode; }
    }


    /// <summary>
    /// 是否为审批表单，是否为审批表单主是表现为是否显示审批流程。一些系统表如：部门，用户等表是不需要审批的，此时传入false值即可。默认值：true
    /// </summary>
    private Boolean isApprovalForm = true;
    public Boolean IsApprovalForm
    {
        set { isApprovalForm = value; }
        get { return isApprovalForm; }
    }

    /// <summary>
    /// 是否显示头部Header，默认显示，值True
    /// </summary>
    private Boolean showHeader = true;
    public Boolean ShowHeader
    {
        set { showHeader = value; }
        get { return showHeader; }
    }

    /// <summary>
    /// 提交后刷新页面
    /// </summary>
    private Boolean isRefresh = true;
    public Boolean IsRefresh
    {
        set { isRefresh = value; }
        get { return isRefresh; }
    }

    public Boolean showButton = false;
    public Boolean ShowButton
    {
        set { showButton = value; }
        get { return showButton; }
    }

    /// <summary>
    /// 表单类型（可选：MicroForm(用户填写申请用)、SystemForm（管理员修改系统设置用）），用于判断表单的权限，MicroForm用申请权限=8、修改权限=9，SystemForm用申请权限=2、修改权限=3，默认值：MicroForm
    /// </summary>
    public string formType = "MicroForm";
    public string FormType
    {
        set { formType = value; }
        get { return formType; }
    }

    /// <summary>
    ///  动态生成表单
    /// </summary>
    protected string GetHtmlCode = string.Empty;

    /// <summary>
    /// 动态生成用于验证表单的JS代码
    /// </summary>
    protected string GetJsCode = string.Empty;


    protected void Page_Load(object sender, EventArgs e)
    {
        string flag = string.Empty;

        try
        {
            if (!string.IsNullOrEmpty(action) && !string.IsNullOrEmpty(shortTableName))
            {
                var Code = MicroForm.GetHtmlCode(action.ToLower(), shortTableName, moduleID, formID, primaryKeyValue, formType, isApprovalForm, showHeader, isRefresh, showButton);

                //动态生成Html控件代码
                GetHtmlCode = Code.HtmlCode;

                //根据传入不同的参数返回对应的JS代码
                if (formCode.ToLower() == "all")
                    GetJsCode = Code.JsFormCode;  //all等于以下所有的JS代码
                else
                {
                    string[] formCodeArr = formCode.Split(',');
                    for (int i = 0; i < formCodeArr.Length; i++)
                    {
                        switch (formCodeArr[i].toStringTrim().ToLower())
                        {
                            case "vcode":
                                GetJsCode += Code.JsFormVerifyCode;  //返回表单验证JS代码，如不允许为空值检测等
                                break;
                            case "ccode":
                                GetJsCode += Code.JsFormCtrlChangeCode;  //返回控件Change事件JS代码，如Select Change事件，CheckBox Click事件等
                                break;
                            case "ecode":
                                GetJsCode += Code.JsFormExtendCode;  //返回控件的扩展代码，如文本框弹出日期代码。该代码必须是一个完整的代码块，放在layui.use下运行的
                                break;
                            case "fcode":
                                GetJsCode += Code.JsFormFlowCode;  //返回表单流程选择审批者控件的xmSelect代码
                                break;
                            case "scode":
                                GetJsCode += Code.JsFormSubmitCode; //返回表单提交按钮JS代码submit
                                break;
                        }
                    }
                }
            }
            else
                GetHtmlCode = MicroPublic.GetFieldSet("错误提示 Error prompt", MicroPublic.GetMsg("DenyURLError"));

        }
        catch (Exception ex) { GetHtmlCode = ex.ToString(); }

    }
}