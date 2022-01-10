<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="UserOvertime.aspx.cs" Inherits="Views_Stats_Attendance_UserOvertime" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server" ClientIDMode="Static">
    <link rel="stylesheet" href="<%: ResolveUrl("~/Resource/css/inleftnav.css")%>" media="all" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
    <div class="layui-card">
        <div class="layui-form layui-card-header layuiadmin-card-header-auto">
            <div class="layui-row">
                <%--left--%>
                <div class="layui-col-xs6 layui-col-sm6 layui-col-md2 layui-col-lg2">
                    加班统计
                </div>
                <div class="layui-col-xs6 layui-col-sm6 layui-col-md10 layui-col-lg10">
                    <div class="layui-form-item">
                        <div class="layui-inline">
                            <label class="layui-form-label " lay-tips="日期范围/DateRange">日期范围</label>
                            <div class="layui-input-inline" style="width:210px;">
                                <input type="text" id="txtDateRange" name="txtDateRange" runat="server" placeholder="日期范围/DateRange" autocomplete="off" class="layui-input" readonly="readonly"><i class="layui-icon layui-icon-date"></i> 
                            </div>
                            <div class="layui-input-inline" style="left:20px;">
                                <input type="text" id="txtKeyword" name="txtKeyword" runat="server" placeholder="关键字/Keyword" autocomplete="off" class="layui-input">
                            </div>
                        </div>
                        <div class="layui-inline ws-margin-left10">
                            <button type="button" class="layui-btn" lay-submit lay-filter="btnSearch" data-type="btnSearch">
                                <i class="layui-icon layui-icon-search layuiadmin-button-btn"></i>
                            </button>
                        </div>
                        <%--<div class="layui-inline">
                            <button type="button" id="btnDel" runat="server" class="layui-btn layui-btn-danger" data-type="btnDel">删除</button>
                        </div>
                        <div class="layui-inline">
                            <button type="button" id="btnAddOpenLink" runat="server" class="layui-btn" data-type="btnAddOpenLink"><i class="layui-icon layui-icon-add-1 layuiadmin-button-btn"></i>添加用户</button>
                        </div>
                        <div class="layui-inline micro-btn">
                            <button type="button" id="btnMore" runat="server" class="layui-btn layui-btn-normal" lay-filter="btnMore"
                                       lay-dropdown="{menus: [{txt: '同步域控用户', event:'SyncDomainUsers'}, {txt: '同步域控用户所属部门', event:'SyncDomainUsersDept'}, {txt: '设置所有用户默认职务', event:'SetAllUserDefaultJobTitle'}, {txt: '设置所有用户默认角色', event:'SetAllUserDefaultRole'}, {txt: '批量修改用户所属部门', event:'ModifyUsersDept'}, {txt: '批量修改用户职位职称', event:'ModifyUsersJobTitle'}, {txt: '批量修改用户所属角色', event:'ModifyUsersRole'}], zIndex: 8888, gap:2}">
                                <span>更多操作</span>
                                <i class="layui-icon layui-icon-triangle-d"></i>
                            </button>
                        </div>--%>
                    </div>
                </div>
            </div>
        </div>

        <div class="layui-card-body">
            <div class="layui-row">
                <%--left--%>
                <div class="layui-col-xs6 layui-col-sm6 layui-col-md2 layui-col-lg2 ws-scrollbar" style="overflow: auto !important;height: 730px !important;">
                    <ul class="layui-nav layui-nav-tree layui-inline">
                        <%=GetLeftNav() %>
                    </ul>
                </div>
                <div class="layui-col-xs6 layui-col-sm6 layui-col-md10 layui-col-lg10" style="padding-left:10px;">
                    <div style="padding-bottom: 10px;">
                        <table id="tabTable" lay-filter="tabTable"></table>
                    </div>
                </div>

            </div>
        </div>

    </div>
    <div id="divScript" runat="server"></div>

    <input name="txtAction" type="hidden" id="txtAction" runat="server"/>
    <input name="txtMID" type="hidden" id="txtMID" runat="server"/>
    <input name="txtShortTableName" type="hidden" id="txtShortTableName" runat="server"/>
    <input name="txtFormID" type="hidden" id="txtFormID" runat="server"/>
    <input name="txtFormsID" type="hidden" id="txtFormsID" runat="server" value=""/>

    <script type="text/html" id="barColUserDepts">
        {{#  if(typeof d.ColUserDepts === 'undefined'){ }}
            <a href="javascript:;" id="link1ChangeDept" runat="server" class="layui-table-link" lay-event="ChangeDept">Null</a>
        {{#  } else { }}
            <a href="javascript:;" id="link2ChangeDept" runat="server" class="layui-table-link" lay-event="ChangeDept">{{d.ColUserDepts = d.ColUserDepts == "" ? "Null" : d.ColUserDepts}}</a>
        {{#  } }}
    </script>
    <script type="text/html" id="barColUserJobTitle">
        {{#  if(typeof d.ColUserJobTitle === 'undefined'){ }}
            <a href="javascript:;" id="link1ChangeJobTitle" runat="server" class="layui-table-link" lay-event="ChangeJobTitle">Null</a>
        {{#  } else { }}
            <a href="javascript:;" id="link2ChangeJobTitle" runat="server" class="layui-table-link" lay-event="ChangeJobTitle">{{d.ColUserJobTitle = d.ColUserJobTitle == "" ? "Null" : d.ColUserJobTitle}}</a>
        {{#  } }}
    </script>
    <script type="text/html" id="barColUserRoles">
        {{#  if(typeof d.ColUserRoles === 'undefined'){ }}
            <a href="javascript:;" id="link1ChangeRoles" runat="server" class="layui-table-link" lay-event="ChangeRoles">Null</a>
        {{#  } else { }}
            <a href="javascript:;" id="link2ChangeRoles" runat="server" class="layui-table-link" lay-event="ChangeRoles">{{d.ColUserRoles = d.ColUserRoles == "" ? "Null" : d.ColUserRoles}}</a>
        {{#  } }}
    </script>

    <script src="<%: ResolveUrl("~/Views/Stats/Attendance/Js/UserOvertime.js")%>"></script>
</asp:Content>

