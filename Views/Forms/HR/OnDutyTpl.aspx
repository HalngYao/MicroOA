<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="OnDutyTpl.aspx.cs" Inherits="Views_Forms_HR_OnDutyTpl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script src="<%: ResolveUrl("~/Resource/xmSelect/xmselect.js")%>"></script>
    <link href="<%: ResolveUrl("~/Resource/css/micropopup.css")%>" rel="stylesheet" media="all" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
    <div class="layui-card">
        <div id="divHeader" runat="server" class="layui-card-header  layuiadmin-card-header-auto " style="margin-bottom: -20px; border-bottom: 0px;">
            <blockquote id="bqTitle" runat="server" class="layui-elem-quote"><span id="spanTitle" runat="server" class="ws-font-red">模板下载</span></blockquote>
        </div>
        <div class="layui-card-body layui-form " lay-filter="micro-form" id="microform" runat="server">
            <div class="layui-row">
                <div class="layui-form-item layui-row "> 
                    <div class="layui-col-lg6 layui-col-md6 layui-col-sm12 layui-col-xs12">
                        <label class="layui-form-label"><i class="ws-must-asterisk">&#42;</i> 部门</label>      
                        <div class="layui-input-block">
                            <div id="microXmSelect"></div>
                            <input id="UserDepts" type="hidden" runat="server" />
                        </div>
                    </div>
                </div>
                <div class="layui-form-item layui-row ">
                    <label class="layui-form-label"><i class="ws-asterisk">&#42;</i>日期</label>
                    <div class="layui-input-inline" style="width: 120px;">
                        <input type="text" id="txtStartDate" name="txtStartDate" runat="server" placeholder="开始日期" autocomplete="off" class="layui-input" readonly="readonly" lay-verify="required">
                        <i id="icondatetxtStartDate" class="layui-icon layui-icon-date" style="position: absolute;right: 8px; line-height: initial; top: 50%; margin-top: -7px;"></i>
                    </div>
                    <div class="layui-form-mid">~</div>
                    <div class="layui-input-inline" style="width: 120px;">
                        <input type="text" id="txtEndDate" name="txtEndDate" runat="server" placeholder="结束日期" autocomplete="off" class="layui-input" readonly="readonly" lay-verify="required">
                        <i id="icondatetxtEndDate" class="layui-icon layui-icon-date" style="position: absolute;right: 8px; line-height: initial; top: 50%; margin-top: -7px;"></i>
                    </div>
                    <div class="layui-form-mid layui-word-aux ">需要导入的排班日期范围（通常以周为单位）</div>
                </div>
                <div class="layui-form-item layui-row "> 
                    <div class="layui-col-lg6 layui-col-md6 layui-col-sm12 layui-col-xs12">
                        <label class="layui-form-label"><i class="ws-want-asterisk">&#42;</i> 对象</label>      
                        <div class="layui-input-block">
                            <input type="checkbox" name="cbObject" title="仅正式员工" lay-skin="primary" checked>
                        </div>
                    </div>
                </div>
                <div class="layui-form-item layui-row ">
                    <div class="layui-inline">
                        <label class="layui-form-label"></label>
                        
                        <button type="button" id="btnDownload" class="layui-btn layui-btn-normal" lay-filter="btnDownload" lay-submit>
                            <i class="layui-icon layui-icon-download-circle layuiadmin-button-btn"></i>下载</button>

                    </div>
                </div>

            </div>
        </div>
    </div>

    <input name="txtMID" type="hidden" id="txtMID" runat="server"/>
    <input name="txtShortTableName" type="hidden" id="txtShortTableName" runat="server"/>

    <script src="<%=ResolveUrl("~/Views/Forms/HR/Js/OnDutyTpl.js")%>"></script>
</asp:Content>

