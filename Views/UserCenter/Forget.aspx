<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Login.master" AutoEventWireup="true" CodeFile="Forget.aspx.cs" Inherits="Views_UserCenter_Forget" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
    <div class="layadmin-user-login-main">
        <div class="layadmin-user-login-box layadmin-user-login-header">
            <h2>找回密码</h2>
            <p><%: GetMicroInfo("Title") %></p>
        </div>
        <div class="layadmin-user-login-box layadmin-user-login-body layui-form">
        <div class="layui-form-item">
            <label class="layadmin-user-login-icon ws-icon icon-mail" for="EMail"></label>
            <input type="text" name="txtEMail" id="txtEMail" lay-verify="EMail" placeholder="邮箱地址 / E-Mail" class="layui-input">
        </div>
        <div class="layui-form-item">
            <div class="layui-row">
            <div class="layui-col-xs8">
                <label class="layadmin-user-login-icon layui-icon layui-icon-vercode" for="ValidateCode"></label>
                <input type="text" name="txtValidateCode" id="txtValidateCode" lay-verify="ValidateCode" placeholder="验证码 / ValidateCode" class="layui-input onlyNumAlpha">
            </div>
            <div class="layui-col-xs4">
                <div style="margin-left: 10px;">
                    <img src="<%=ResolveUrl("~/Resource/ValidateCode/ValidateImage.aspx")%>" id="imgCode"  title="看不清换一张 I can't see it change one" style="cursor: pointer; margin-top:1px; width:100px; height:36px;"/>
                </div>
            </div>
            </div>
        </div>
        <div class="layui-form-item">
            <button type="button" class="layui-btn layui-btn-fluid" lay-submit lay-filter="btnSave">找回密码 Forget password</button>
        </div>
        <div class="layui-trans layui-form-item layadmin-user-login-other">
            <asp:HyperLink ID="hlWinLogin" runat="server" lay-tips="Windows domain account login">Windows域账号登录<i class="ws-icon icon-windows1 ws-font-size26 ws-mid"></i></asp:HyperLink>
            <%--<a href="javascript:;" lay-tips="Windows domain account login"><label>Windows域账号登录</label> <i class="ws-icon icon-windows1 ws-font-size26 ws-mid"></i></a>
            <a href="javascript:;"><i class="layui-icon layui-icon-login-wechat"></i></a>
            <a href="javascript:;"><i class="layui-icon layui-icon-login-weibo"></i></a>--%>
          
            <a href="<%=ResolveUrl("~/Views/UserCenter/Login")%>" class="layadmin-user-jump-change layadmin-link layui-hide-xs">登录Login</a>
        </div>
        </div>
    </div>
    <script src='<%=ResolveUrl("~/Views/UserCenter/Js/Forget.js")%>'></script>
</asp:Content>

