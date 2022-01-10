<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="SysNonPopForm.aspx.cs" Inherits="Views_Forms_SysNonPopForm" %>
<%@ Register Src="~/App_Ctrl/MicroForm.ascx" TagName="MicroForm" TagPrefix="MicroCtrl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script src="<%: ResolveUrl("~/Resource/xmSelect/xmselect.js")%>"></script>
    <script src="<%: ResolveUrl("~/Resource/WangEditor/wangEditor.min.js")%>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
    <%--该表单：不是由页面点击弹窗出来的表单，直接在选项卡上显示的--%>

    <MicroCtrl:MicroForm ID="microForm" runat="server" FormCode="all" FormType="SystemForm" IsApprovalForm="false" ShowHeader="True" ShowButton="true" />
    <input id="txtMID" type="hidden" runat="server" />
    <script src="<%: ResolveUrl("~/Views/Forms/Js/SysNonPopForm.js")%>"></script>
</asp:Content>

