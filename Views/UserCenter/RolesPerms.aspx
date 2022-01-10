<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="RolesPerms.aspx.cs" Inherits="Views_UserCenter_RolesPerms" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server" ClientIDMode="Static">
    <link rel="stylesheet" href="<%: ResolveUrl("~/Resource/css/inleftnav.css")%>" media="all" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
    <div class="layui-card">
        <div class="layui-form layui-card-header layuiadmin-card-header-auto1">
            角色和权限管理
        </div>

        <div class="layui-card-body">
            <div class="layui-row">
                <%--left--%>
                <div class="layui-col-xs6 layui-col-sm6 layui-col-md2 layui-col-lg2 ws-scrollbar" style="overflow:auto !important;height:730px !important;">
                    <ul class="layui-nav layui-nav-tree layui-inline">
                        <%=GetLeftNav() %>
                    </ul>
                </div>
               <%--right--%>
                <div class="layui-col-xs6 layui-col-sm6 layui-col-md10 layui-col-lg10" style="padding-left:10px;">
                    <div class="layui-form layui-card-header layuiadmin-card-header-auto1">
                        【<span id="spanTitle">超级管理员</span>】所拥有的权限
                        <div class="layui-inline" style="right:5px; float:right;">
                            <button type="button" id="btnSave" runat="server" class="layui-btn layui-btn-sm" micro-pagename="Role" micro-id="1" data-type="btnSave">立即保存</button>
                            <button type="button" id="btnOpenLink" runat="server" class="layui-btn layui-btn-sm layui-btn-normal" micro-text="模块权限赋予" data-type="btnOpenLink">模块权限赋予</button>
                        </div>
                    </div>
                    <div class="ws-scrollbar" id="divShowPerms" style="padding-left:20px; overflow:auto !important; padding-bottom:10px; height:730px; "></div>
                </div>

            </div>
        </div>

    </div>
    <div id="divScript" runat="server"></div>
    <input id="txtAction" type="hidden" runat="server" />
    <input id="txtShortTableName" type="hidden" runat="server" />
    <input id="txtMID" type="hidden" runat="server" />
    <input id="txtEdit" type="hidden" runat="server" value="False" />
    <script src="<%: ResolveUrl("~/Views/UserCenter/Js/RolesPerms.js")%>"></script>
</asp:Content>

