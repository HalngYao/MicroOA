<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="ModulePerms.aspx.cs" Inherits="Views_UserCenter_ModulePerms" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
    <div class="layui-card">
        <div class="layui-card-header">
            模块权限赋予<%--<i class="layui-icon layui-icon-about" lay-tips="模块权限赋予"></i>--%>
            <div class="layui-inline" style="right:5px; float:right;">
                <button type="button" id="btnOpenLink" runat="server" class="layui-btn layui-btn-sm layui-btn-normal" micro-text="权限种类管理" data-type="btnOpenLink">权限种类管理</button>
            </div>
        </div>

        <div class="layui-card-body layui-form" lay-filter="micro-form">

            <%= GetModulePerm() %>

            <div class="layui-form-item">
                <div class="layui-input-block" style="margin-top: 10px;">
                    <button type="button" id="btnSave" runat="server" class="layui-btn" lay-submit="" lay-filter="btnSave">立即提交</button>
                    <button type="button" id="btnSetDefault" runat="server" class="layui-btn layui-btn-normal" lay-submit="" lay-filter="btnSetDefault">设为默认</button>
                    <button type="button" id="btnRecovery" runat="server" class="layui-btn layui-btn-warm" lay-submit="" lay-filter="btnRecovery">恢复默认</button>
                </div>
            </div>

        </div>

    </div>
    <input id="txtAction" type="hidden" runat="server" />
    <input id="txtShortTableName" type="hidden" runat="server" />
    <input id="txtMID" type="hidden" runat="server" />
    <script src="<%=ResolveUrl("~/Views/UserCenter/Js/ModulePerms.js")%>"></script>
</asp:Content>

