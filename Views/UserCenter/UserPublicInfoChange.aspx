<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="UserPublicInfoChange.aspx.cs" Inherits="Views_UserCenter_UserPublicInfoChange" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script src="<%=ResolveUrl("~/Resource/xmSelect/xmselect.js")%>"></script>
    <link href="<%=ResolveUrl("~/Resource/CSS/BackgroundWhite.css")%>" rel="stylesheet" media="all" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server" ClientIDMode="Static">
    <div class="layui-card">
        <div class="layui-card-body">

            <div class="layui-form-item layui-row layui-col-space10">
                <%--<label class="layui-form-label">验证必填项</label>--%>
                <div class="layui-col-xs6 layui-col-sm6 layui-col-md4">
                    <div class="layui-block-inline">
                        <div id="microXmSelect"></div>
                    </div>
                </div>
                <div class="layui-col-xs6 layui-col-sm6 layui-col-md3" id="divDate" runat="server">
                    <label class="layui-form-label">开始日期：</label>
                    <div class="layui-input-inline">
                        <input type="text" name="txtDate" id="txtDate" runat="server" placeholder="生效日期" autocomplete="off" class="layui-input">
                    </div>
                </div>
                <div class="layui-input-inline">
                    <button type="button" id="btnModify" runat="server" class="layui-btn layui-btn-normal" data-type="btnModify">保存修改</button>
                </div>
            </div>
              
        </div>
    </div>
    <input id="txtMID" type="hidden" runat="server" />
    <input id="txtUID" type="hidden" runat="server" />
    <input id="txtType" type="hidden" runat="server" />
    <input id="txtDefaultValue" type="hidden" runat="server" />
    <script src="<%=ResolveUrl("~/Views/UserCenter/Js/UserPublicInfoChange.js")%>"></script>
</asp:Content>

