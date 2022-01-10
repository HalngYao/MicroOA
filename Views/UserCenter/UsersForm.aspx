<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="UsersForm.aspx.cs" Inherits="Views_UserCenter_UsersForm" %>
<%@ Register Src="~/App_Ctrl/MicroForm.ascx" TagName="MicroForm" TagPrefix="MicroCtrl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="<%: ResolveUrl("~/Resource/css/micropopup.css")%>" rel="stylesheet" media="all" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
    <MicroCtrl:MicroForm ID="microForm" runat="server" FormCode="vcode,ccode" FormType="SystemForm" IsApprovalForm="false"/>
    <input id="txtMID" type="hidden" runat="server" />
    <input id="txtFormID" type="hidden" runat="server" />
    <script src="<%: ResolveUrl("~/Views/UserCenter/Js/UsersForm.js")%>"></script>
</asp:Content>

