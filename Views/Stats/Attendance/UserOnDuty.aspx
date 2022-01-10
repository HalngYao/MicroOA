<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="UserOnDuty.aspx.cs" Inherits="Views_Stats_Attendance_UserOnDuty" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link rel="stylesheet" href="<%: ResolveUrl("~/Resource/css/inleftnav.css")%>" media="all" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
    <div class="layui-card">
        <div class="layui-form layui-card-header layuiadmin-card-header-auto">
            <div class="layui-row">
                <%--left--%>
                <div class="layui-col-xs6 layui-col-sm6 layui-col-md2 layui-col-lg2">
                    排班统计
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

    <script src="<%: ResolveUrl("~/Views/Stats/Attendance/Js/UserOnDuty.js")%>"></script>
</asp:Content>

