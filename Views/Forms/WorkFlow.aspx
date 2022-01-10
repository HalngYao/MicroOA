<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="WorkFlow.aspx.cs" Inherits="Views_Forms_WorkFlow" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script src="<%: ResolveUrl("~/Resource/xmSelect/xmselect.js")%>"></script>
    <link href="<%: ResolveUrl("~/Resource/css/micropopup.css")%>" rel="stylesheet" media="all" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
    <div class="layui-card">
        <%--<div class="layui-card-header ">
            流程设置 WorkFlow Setting
        </div>--%>

        <div class="layui-card-body layui-form">
            <div class="layui-row">
                <div class="layui-form-item">
                    <div class="layui-inline ">
                        <div class="layui-input-inline ">
                             <div id="microXmSelect"></div>
                        </div>
                        <div class="layui-form-mid layui-word-aux">审批流程设置</div>
                    </div>
                </div>
            </div>
            <div class="layui-row" style="background-color:#F0F0F0; height:60px; line-height:60px; margin-bottom:10px;">
                <div class="layui-form-item" style="margin-left:10px;">
                    <button type="button" id="btnAddOpenLink" runat="server" class="layui-btn layui-btn-sm micro-click" data-type="btnAddOpenLink"><i class="layui-icon layui-icon-add-1 layuiadmin-button-btn"></i>添加流程</button>
                </div>
            </div>
            <div class="layui-row">
                <div class="layui-form-item">
                    <table id="tabTable" lay-filter="tabTable"></table>
                </div>
            </div>
            <%--<input id="txtFormID" type="hidden" runat="server" />--%>
        </div>

    </div>
    <input id="txtSTN" type="hidden" runat="server" />
    <input id="txtMID" type="hidden" runat="server" />
    <input id="txtFormID" type="hidden" runat="server" />
    <script type="text/html" id="swDefaultFlow">
        <input type="checkbox" name="DefaultFlow" value="{{d.WFID}}" lay-skin="primary" lay-filter="DefaultFlow" {{d.DefaultFlow == 'True' ? 'checked' : ''}}>
    </script>
    <script type="text/html" id="swIsAccept">
        <input type="checkbox" name="IsAccept" value="{{d.WFID}}" lay-skin="switch" lay-text="是|否" lay-filter="IsAccept" {{ d.IsAccept == 'True' ? 'checked' : '' }}>
    </script>
    <script type="text/html" id="swIsSync">
        <input type="checkbox" name="IsSync" value="{{d.WFID}}" lay-skin="switch" lay-text="是|否" lay-filter="IsSync" {{ d.IsSync == 'True' ? 'checked' : '' }}>
    </script>
    <script type="text/html" id="barTools">
        <a id="linkModify" runat="server" class="layui-btn layui-btn-normal layui-btn-xs" lay-event="Modify">修改</a>
        <a id="linkDel" runat="server" class="layui-btn layui-btn-danger layui-btn-xs" lay-event="Del">删除</a>
    </script>
    <script src="<%: ResolveUrl("~/Views/Forms/Js/WorkFlow.js")%>"></script>
</asp:Content>

