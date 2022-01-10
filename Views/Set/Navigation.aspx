<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="Navigation.aspx.cs" Inherits="Views_Set_Navigation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="<%: ResolveUrl("~/Resource/css/microlayerbtn.css")%>" rel="stylesheet" media="all" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
    <div class="layui-card">
        <div class="layui-form layui-card-header layuiadmin-card-header-auto">
            <div class="layui-form-item">
                <div class="layui-inline">
                    <div class="layui-input-inline">
                        <input type="text" name="txtKeyword" placeholder="Keyword" autocomplete="off" class="layui-input">
                    </div>
                </div>
                <div class="layui-inline">
                    <button class="layui-btn" lay-submit lay-filter="btnSearch">
                        <i class="layui-icon layui-icon-search layuiadmin-button-btn"></i>
                    </button>
                </div>
                <div class="layui-inline">
                    <button type="button" id="btnDel" runat="server" class="layui-btn layui-btn-danger" data-type="btnDel">删除</button>
                </div>
                <div class="layui-inline">
                    <button type="button" id="btnAddOpenLink" runat="server" class="layui-btn" data-type="btnAddOpenLink"><i class="layui-icon layui-icon-add-1 layuiadmin-button-btn"></i>添加导航</button>
                </div>
            </div>
        </div>

        <div class="layui-card-body">
            <div style="padding-bottom: 10px;">
                <table id="tabTable" lay-filter="tabTable"></table>
            </div>
        </div>

    </div>
    <div id="divScript" runat="server"></div>
    <input id="txtMID" type="hidden" runat="server" />
    <input id="txtShortTableName" type="hidden" runat="server" />
    <input id="txtPrimaryKeyName" type="hidden" runat="server" />

    <script type="text/html" id="microBar">
        <%-- <a id="linkAdd" runat="server" class="layui-btn layui-btn-xs" lay-event="Add" lay-tips="填写申请"><i class="layui-icon layui-icon-add-1"></i>申请</a>
        <a id="linkView" runat="server" class="layui-btn layui-btn-xs layui-btn-normal" lay-event="View" lay-tips="查看申请记录"><i class="layui-icon layui-icon-list"></i>记录</a>--%>
        <a id="linkModify" runat="server" class="layui-btn layui-btn-xs layui-btn-warm" lay-event="Modify" lay-tips="编辑表单"><i class="layui-icon layui-icon-edit"></i>编辑</a>
    </script>
    <script src="<%: ResolveUrl("~/Views/Set/Navigation.js")%>"></script>
</asp:Content>

