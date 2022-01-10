<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="WebSite.aspx.cs" Inherits="Views_Set_System_WebSite" %>
<%@ Register Src="~/App_Ctrl/MicroForm.ascx" TagName="MicroForm" TagPrefix="MicroCtrl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script src="<%: ResolveUrl("~/Resource/xmSelect/xmselect.js")%>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
    <MicroCtrl:MicroForm ID="microForm" runat="server" Action="modify" ShortTableName="sys" FormCode="all" FormType="SystemForm" IsApprovalForm="false" ModuleID="15"  PrimaryKeyValue="1" ShowHeader="false" ShowButton="true" />
    <script src="<%: ResolveUrl("~/Views/Set/Js/WebSite.js")%>"></script>
</asp:Content>

