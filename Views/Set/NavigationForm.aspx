<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="NavigationForm.aspx.cs" Inherits="Views_Set_NavigationForm" %>
<%@ Register Src="~/App_Ctrl/MicroForm.ascx" TagName="MicroForm" TagPrefix="MicroCtrl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="<%: ResolveUrl("~/Resource/CSS/micropopup.css")%>" rel="stylesheet" media="all" />
    <script src="<%: ResolveUrl("~/Resource/xmSelect/xmselect.js")%>"></script>
    <style>
        /*隐藏页面的保存按钮，用弹窗的按钮进行操作*/
        .ws-hide-btn{display:none;} 
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
    <MicroCtrl:MicroForm ID="microForm" runat="server"  Action="add" ShortTableName="Mod" FormCode="all" FormType="SystemForm" IsApprovalForm="false" ShowHeader="False" />
    <input id="txtMID" type="hidden" runat="server" />
    <script src="<%: ResolveUrl("~/Views/Set/NavigationForm.js")%>"></script>
</asp:Content>

