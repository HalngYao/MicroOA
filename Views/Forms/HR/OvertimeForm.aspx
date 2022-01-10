<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="OvertimeForm.aspx.cs" Inherits="Views_Forms_HR_OvertimeForm" %>

<%@ Register Src="~/App_Ctrl/MicroForm.ascx" TagName="MicroForm" TagPrefix="MicroCtrl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="<%: ResolveUrl("~/Resource/CSS/micropopup.css")%>" rel="stylesheet" media="all" />
    <script src="<%: ResolveUrl("~/Resource/xmSelect/xmselect.js")%>"></script>
    <script src="<%: ResolveUrl("~/Resource/WangEditor/wangEditor.min.js")%>"></script>
    <style type="text/css">
        .layui-laydate-content > .layui-laydate-list {
            padding-bottom: 0px;
            overflow: hidden;
        }

        .layui-laydate-content > .layui-laydate-list > li {
            width: 50%;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server" ClientIDMode="Static">
    <MicroCtrl:MicroForm ID="microForm" runat="server" FormCode="vcode,ecode" IsApprovalForm="false" ShowHeader="false" />

    <input name="txtAction" type="hidden" id="txtAction" runat="server"/>
    <input name="txtMID" type="hidden" id="txtMID" runat="server"/>
    <input name="txtShortTableName" type="hidden" id="txtShortTableName" runat="server"/>
    <input name="txtFormID" type="hidden" id="txtFormID" runat="server"/>
    <input name="txtFormsID" type="hidden" id="txtFormsID" runat="server" value=""/>
    <input name="txtOvertimeIDs" type="hidden" id="txtOvertimeIDs" runat="server" value=""/>

    <%--xmByUserDefVal隐藏域用于给xmSelect控件赋予默认值，当修改加班申请表时根据传递过来的OvertimeID而读取出OvertimeUID组成逗号分隔字符串（而不是直接传递UID），
    所以xmSelect会先判断xmByUserDefVal元素是否存在，如果存在则取该值，如果不存在则取修改表单时的value，详细内容请看MicroFormHelper.cs的控件CheckBoxCascaderSelectUserByDept--%>
    <input name="xmByUserDefVal" type="hidden" id="xmByUserDefVal" runat="server" value=""/>
    <input name="txtOTHour" type="hidden" id="txtOTHour" runat="server" value="0"/>
    <input name="txtOTMin" type="hidden" id="txtOTMin" runat="server" value="0"/>
    <input name="txtEditTablePermit" type="hidden" id="txtEditTablePermit" runat="server" value="0" />
    <input name="txtOvertimeTypeCode" type="hidden" id="txtOvertimeTypeCode" runat="server" value="Normal" />

    <script src="<%=ResolveUrl("~/Views/Forms/HR/Js/OvertimeForm.js")%>"></script>
</asp:Content>

