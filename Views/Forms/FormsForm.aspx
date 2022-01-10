<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="FormsForm.aspx.cs" Inherits="Views_Forms_FormsForm" %>
<%@ Register Src="~/App_Ctrl/MicroForm.ascx" TagName="MicroForm" TagPrefix="MicroCtrl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script src="<%: ResolveUrl("~/Resource/xmSelect/xmselect.js")%>"></script>
    <link href="<%: ResolveUrl("~/Resource/css/micropopup.css")%>" rel="stylesheet" media="all" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
    <MicroCtrl:MicroForm ID="microForm" runat="server"  Action="add" ShortTableName="Forms" FormCode="all" FormType="SystemForm" IsApprovalForm="false" ShowHeader="True" />
    <input id="txtMID" type="hidden" runat="server" />
    <script src="<%: ResolveUrl("~/Views/Forms/Js/FormsForm.js")%>"></script>
</asp:Content>

