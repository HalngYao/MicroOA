<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="OvertimeFormList.aspx.cs" Inherits="Views_Forms_HR_OvertimeFormList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script src="<%: ResolveUrl("~/Resource/xmSelect/xmselect.js")%>"></script>
    <link href="<%: ResolveUrl("~/Resource/css/micropopup.css")%>" rel="stylesheet" media="all" />
    <style type="text/css">
        .micro-layer-class2 .layui-layer-btn .layui-layer-btn0 {
        border-color: #1E9FFF !important;
        background-color: #1E9FFF !important;
        color: #fff;}

        /*使用xs表格时把前面的Checkbox缩小*/
        .layui-table-view .layui-form-checkbox[lay-skin=primary] i {
            width: 13px;
            height: 13px;
            margin-top:-5px;
        }

        .layui-form-checkbox[lay-skin=primary] i {
            line-height: 10px;
            font-size: 10px;
        }

    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server" ClientIDMode="Static">
    <div class="layui-card">
        <div id="divHeader" runat="server" class="layui-card-header  layuiadmin-card-header-auto " style="margin-bottom: -20px; border-bottom: 0px;">
            <blockquote id="bqTitle" runat="server" class="layui-elem-quote"><span id="spanTitle" runat="server" class="ws-font-red">加班申请表</span><span id="spanWorkFlow" runat="server" style="position: absolute; right: 20px;"><a herf="javascript:; " style="cursor: pointer;" class="micro-click" micro-mid="4" micro-stn="Overtime" data-type="SetWorkFlow">【自定义流程】</a></span></blockquote>
        </div>
        <div class="layui-card-body layui-form " lay-filter="micro-form">
            <div class="layui-row">
               <%-- 列1 Start--%>
                <%--<div class="layui-col-xs12 layui-col-sm12 layui-col-md12 layui-col-lg12 ">--%>
                <div id="divClo1" runat="server">
                    <div class="layui-form-item layui-row ">
                        <div class="layui-inline layui-hide ">
                            <label class="layui-form-label layui-hide "><i class="ws-want-asterisklayui-hide ">&#42;</i> FormID</label><div class="layui-input-inline layui-hide ">
                                <input type="text" id="hidFormID" name="hidFormID" value="" placeholder="" lay-verify="" autocomplete="off" class="layui-input layui-hide "></div>
                            <div class="layui-form-mid layui-word-aux layui-hide "><i class="layui-icon layui-icon-about" lay-tips="int，表单ID。对应的是Forms表的ID"></i></div>
                        </div>
                    </div>
                    <div class="layui-form-item layui-row ">
                        <div class="layui-inline">
                            <label class="layui-form-label "><i class="ws-want-asterisk">&#42;</i> 表单编号</label><div class="layui-input-inline ">
                                <input type="text" id="txtFormNumber" name="txtFormNumber" value="GetFormNumber" runat="server" placeholder="" lay-verify="txtFormNumberLength" lay-reqtext="表单编号不能为空<br/>This cannot be empty" autocomplete="off" class="layui-input" readonly="&quot;readonly&quot;"></div>
                            <div class="layui-form-mid layui-word-aux "><i class="layui-icon layui-icon-about" lay-tips="表单编号，自动生成"></i>表单编号，自动生成</div>
                        </div>
                        <div class="layui-inline ">
                            <label class="layui-form-label "><i class="ws-want-asterisk">&#42;</i> 表单状态</label><div class="layui-input-inline ">
                                <input type="text" id="txtFormState" name="txtFormState" runat="server" value="填写申请[Fill in]" placeholder="" lay-verify="txtFormStateLength" lay-reqtext="表单状态不能为空<br/>This cannot be empty" autocomplete="off" class="layui-input " readonly="&quot;readonly&quot;"></div>
                            <div class="layui-form-mid layui-word-aux "><i class="layui-icon layui-icon-about" lay-tips="表单状态，自动生成"></i>表单状态，自动生成</div>
                        </div>
                    </div>
                    <div class="layui-form-item layui-row ">
                        <div class="layui-inline layui-hide ">
                            <label class="layui-form-label layui-hide "><i class="ws-want-asterisklayui-hide ">&#42;</i> 状态代码</label><div class="layui-input-inline layui-hide ">
                                <input type="text" id="hidStateCode" name="hidStateCode" value="GetFormStateCode" placeholder="" lay-verify="" autocomplete="off" class="layui-input layui-hide "></div>
                            <div class="layui-form-mid layui-word-aux layui-hide "><i class="layui-icon layui-icon-about" lay-tips="状态代码，只有StateCode<0的情况下才允许修改"></i></div>
                        </div>
                    </div>
                    <fieldset class="layui-elem-field layui-field-title "><legend class="ws-font-green ws-font-size16">加班内容 </legend></fieldset>
                    <div id="divNote" runat="server" visible="false" class="layui-form-item layui-row ">
                        <div class="layui-col-xs12 layui-col-sm12 layui-col-md12 layui-col-lg12 ">
                            <blockquote class="layui-elem-quote" style="border-left-color:#FF5722;"><span id="spanNote" runat="server" class="ws-font-red"></span></blockquote>
                        </div>
                    </div>
                    <div class="layui-form-item layui-row ">
                        <div class="layui-col-xs12 layui-col-sm12 layui-col-md8 layui-col-lg8 ">
                            <div class="layui-input-inline ws-width350">
                                <button type="button" id="btnAddOpenLink" runat="server" class="layui-btn layui-btn-xs micro-click" data-type="btnAddOpenLink">
                                    <i class="layui-icon layui-icon-add-1 layuiadmin-button-btn"></i>添加内容
                                </button>
                                <button type="button" id="btnCopyAddOpenLink" runat="server" class="layui-btn layui-btn-xs layui-btn-warm micro-click" data-type="btnCopyAddOpenLink">
                                    <i class="layui-icon ws-icon icon-fuzhi3 layuiadmin-button-btn"></i>复制添加
                                </button>
                                <button type="button" id="btnModifyOpenLink" runat="server" class="layui-btn layui-btn-normal layui-btn-xs micro-click" data-type="btnModifyOpenLink">
                                    <i class="layui-icon layui-icon-edit layuiadmin-button-btn"></i>修改内容
                                </button>
                                <button type="button" id="btnDel" runat="server" class="layui-btn layui-btn-normal ws-bg-gray6 layui-btn-xs micro-click" data-type="btnDel">
                                    <i class="layui-icon layui-icon-delete layuiadmin-button-btn"></i>删除选中
                                </button>                            
                            </div>
                            <div id="divTips" runat="server" visible="false" class="layui-form-mid layui-word-aux " style="top:-5px; color:red !important;">对不起！不允许添加、修改或删除内容。原因：您的草稿箱还存在未处理的记录，请先处理后再来填写申请。</div>
                        </div>
                    </div>
                    <div class="layui-form-item layui-row ">
                        <div class="layui-col-xs12 layui-col-sm12 layui-col-md12 layui-col-lg12 ">
                            <table id="tabTable" lay-filter="tabTable"></table>
                        </div>
                    </div>

                    <%=HtmlCode %>

                    <div class="layui-form-item layui-hide ws-margin-top10">
                        <div class="layui-input-block micro-btn-div">
                            <button type="button" id="btnSave" class="layui-btn " lay-submit lay-filter="btnSave">立即提交</button>
                            <button type="button" id="btnModify" class="layui-btn " lay-submit lay-filter="btnModify">修改提交</button>
                            <button type="button" id="btnDraft" class="layui-btn layui-btn-normal" lay-submit lay-filter="btnDraft">临时保存</button>
                            <button type="reset" class="layui-btn layui-btn-primary">立即重置</button>
                        </div>
                    </div>

                </div>
                <%-- 列1 End--%>
                <%-- 列2 Start--%>
                <%=divCol2HtmlCode %>

            </div>
        </div>
    </div>

    <input name="txtAction" type="hidden" id="txtAction" runat="server"/>
    <input name="txtMID" type="hidden" id="txtMID" runat="server"/>
    <input name="txtShortTableName" type="hidden" id="txtShortTableName" runat="server"/>
    <input name="txtFormID" type="hidden" id="txtFormID" runat="server"/>
    <input name="txtFormsID" type="hidden" id="txtFormsID" runat="server" value=""/>
    <input id="txtPrimaryKeyName" type="hidden" runat="server" />
    <input id="txtEditTablePermit" type="hidden" runat="server" value="0" />
    <input id="txtRecord" type="hidden" runat="server" value="0" />  <%--检查Table是否有记录--%>

    <script type="text/javascript">
        layui.use(['index', 'jquery', 'form', 'laydate', 'layer', 'colorpicker', 'upload', 'layedit', 'micro'], function () {
            var $ = layui.$,
                form = layui.form,
                laydate = layui.laydate,
                layer = layui.layer,
                colorpicker = layui.colorpicker,
                upload = layui.upload,
                layedit = layui.layedit,
                micro = layui.micro;

            <%=JsXmSelectCode%>
            
        });
    </script>
    <script src="<%=ResolveUrl("~/Views/Forms/HR/Js/OvertimeFormList.js")%>"></script>
</asp:Content>

