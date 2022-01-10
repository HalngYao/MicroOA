<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Info.master" AutoEventWireup="true" CodeFile="Detail.aspx.cs" Inherits="Views_Info_Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
<div class="layui-container">
  <div class="layui-row layui-col-space15">
    <div class="layui-col-md12 content detail">
       <%= GetDetail() %>
    </div>
  </div>
</div>
<input id="txtMID" type="hidden" runat="server" />
<input id="txtInfoClassID" type="hidden" runat="server" value="0" />
<script src="<%: ResolveUrl("~/Views/Info/Js/Info.js")%>"></script>
</asp:Content>

