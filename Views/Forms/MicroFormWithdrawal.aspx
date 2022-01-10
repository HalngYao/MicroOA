<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="MicroFormWithdrawal.aspx.cs" Inherits="Views_Forms_MicroFormWithdrawal" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="<%: ResolveUrl("~/Resource/CSS/micropopup.css")%>" rel="stylesheet" media="all" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">

    <%=GetHtmlCode %>

    <input id="txtAction" type="hidden" runat="server" />
    <input id="txtShortTableName" type="hidden" runat="server" />
    <input id="txtMID" type="hidden" runat="server" />
    <input id="txtFormID" type="hidden" runat="server" />  
    <input id="txtPrimaryKeyValue" type="hidden" runat="server" />

    <script src="<%: ResolveUrl("~/Views/Forms/Js/MicroFormWithdrawal.js")%>"></script>
</asp:Content>

