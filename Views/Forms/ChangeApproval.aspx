<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="ChangeApproval.aspx.cs" Inherits="Views_Forms_ChangeApproval" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script src="<%=ResolveUrl("~/Resource/xmSelect/xmselect.js")%>"></script>
    <link href="<%=ResolveUrl("~/Resource/CSS/BackgroundWhite.css")%>" rel="stylesheet" media="all" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
    <div class="layui-card">
        <div class="layui-card-body">

            <div class="layui-form-item">
                <div class="layui-input-inline" style="width:60%">
                    <div id="microXmSelect"></div>
                </div>
                <button type="button" id="btnModify" runat="server" class="layui-btn layui-btn-normal" data-type="btnModify">保存修改</button>
            </div>
              
        </div>
    </div>
    <input id="txtMID" type="hidden" runat="server" />
    <input id="txtWFID" type="hidden" runat="server" />
    <input id="txtType" type="hidden" runat="server" />
    <input id="txtFieldName" type="hidden" runat="server" />
    <input id="txtDefaultValue" type="hidden" runat="server" />
    <script src="<%=ResolveUrl("~/Views/Forms/Js/ChangeApproval.js")%>"></script>
</asp:Content>

