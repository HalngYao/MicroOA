<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="MicroPublicFormList.aspx.cs" Inherits="Views_Forms_MicroPublicFormList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="<%: ResolveUrl("~/Resource/CSS/microlayerbtn.css")%>" rel="stylesheet" media="all" />
    <link href="<%: ResolveUrl("~/Resource/CSS/approval.css")%>" rel="stylesheet" media="all" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
    <div class="layui-card">
      <div class="layui-tab layui-tab-brief">
        <ul class="layui-tab-title">
          <li class="layui-this">列表 / List</li>
        </ul>
        <div class="layui-tab-content">
        
          <div class="layui-tab-item layui-show">
<%--            <div class="micro-message-btns" style="margin-bottom: 10px;">
              <button type="button" class="layui-btn layui-btn-primary layui-btn-sm" data-type="btnDel" lay-tips="Delete">删除</button>
              <button type="button" class="layui-btn layui-btn-primary layui-btn-sm" data-type="btnReady" lay-tips="Mark Read">标记已读</button>
              <button type="button" class="layui-btn layui-btn-primary layui-btn-sm" data-type="btnReadyAll" lay-tips="All Read">全部已读</button>
            </div>--%>
            
            <table id="tabTable" lay-filter="tabTable"></table>
          </div>
        </div>
      </div>
    </div>

    <div id="divScript" runat="server"></div>

    <script type="text/html" id="barFormNumber">
      <a href="javascript:;" class="layui-table-link" lay-event="View">{{ d.FormNumber }}</a>
    </script>

    <input id="txtAction" type="hidden" runat="server" />
    <input id="txtType" type="hidden" runat="server" />
    <input id="txtMID" type="hidden" runat="server" />
    <input id="txtLinkAddress" type="hidden" runat="server" value="/Views/Forms/MicroForm" />
    <script src="<%: ResolveUrl("~/Views/Forms/Js/MicroPublicFormList.js")%>"></script>
</asp:Content>

