<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Login.master" AutoEventWireup="true" CodeFile="Register.aspx.cs" Inherits="Views_UserCenter_Register" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
    <div class="layadmin-user-login-main">
        <div class="layadmin-user-login-box layadmin-user-login-header">
            <h2>用户注册</h2>
            <p><%: GetMicroInfo("Title") %></p>
        </div>
        <div class="layadmin-user-login-box layadmin-user-login-body layui-form">
        <div class="layui-form-item">
            <label class="layadmin-user-login-icon layui-icon layui-icon-username" for="UserName"></label>
            <input type="text" name="txtUserName" id="txtUserName" lay-verify="UserName" placeholder="用户名 / UserID" micro-tips="用户名不能为空。<br/> User name cannot be empty." class="layui-input onlyNumAlpha">
        </div>
        <div class="layui-form-item">
            <label class="layadmin-user-login-icon layui-icon layui-icon-password" for="UserPassword"></label>
            <input type="password" name="txtUserPassword" id="txtUserPassword" lay-verify="UserPassword" placeholder="密码8~16位 / Password 8~16 bits" micro-tips="密码必须8到16位，且不能出现空格 <br/> The password must be 8 to 16 digits and no spaces are allowed" class="layui-input">
        </div>
        <div class="layui-form-item">
            <label class="layadmin-user-login-icon layui-icon layui-icon-password" for="ConfirmPassword"></label>
            <input type="password" name="txtConfirmPassword" id="txtConfirmPassword" lay-verify="ConfirmPassword" placeholder="确认密码 / ConfirmPassword" micro-tips="两次输入的密码不一致。<br/>Two inputted password inconsistencies" class="layui-input">
        </div>
        <div class="layui-form-item">
            <label class="layadmin-user-login-icon ws-icon icon-mail" for="EMail"></label>
            <input type="text" name="txtEMail" id="txtEMail" lay-verify="EMail" placeholder="邮箱地址 / E-Mail" micro-tips="邮箱地址不能为空。<br/> Email address cannot be empty" class="layui-input">
        </div>
        <div id="divValidateCode" runat="server" class="layui-form-item">
            <div class="layui-row">
            <div class="layui-col-xs8">
                <label class="layadmin-user-login-icon layui-icon layui-icon-vercode" for="ValidateCode"></label>
                <input type="text" name="txtValidateCode" id="txtValidateCode" lay-verify="ValidateCode" placeholder="验证码 / ValidateCode" micro-tips="请输入验证码 <br/> Please input verification code" class="layui-input onlyNumAlpha">
            </div>
            <div class="layui-col-xs4">
                <div style="margin-left: 10px;">
                    <img src="<%=ResolveUrl("~/Resource/ValidateCode/ValidateImage")%>" id="imgCode"  title="看不清换一张 I can't see it change one" style="cursor: pointer; margin-top:1px; width:100px; height:36px;"/>
                </div>
            </div>
            </div>
        </div>
        <div class="layui-form-item" lay-tips="Agree to user agreement">
            <input type="checkbox" name="agreement" lay-skin="primary" title="同意用户协议" checked>
        </div>
        <div class="layui-form-item">
            <button type="button" class="layui-btn layui-btn-fluid" lay-submit lay-filter="btnSave">注 册 Register</button>
        </div>
        <div class="layui-trans layui-form-item layadmin-user-login-other">
            <%--<asp:HyperLink ID="hlDomainAccountRegister" runat="server" lay-tips="Windows domain account registration">Windows域账号注册<i class="ws-icon icon-windows1 ws-font-size26 ws-mid"></i></asp:HyperLink>--%>
            <%-- <a href="javascript:;" lay-tips="Windows domain account registration"><label>Windows域账号注册</label> <i class="ws-icon icon-windows1 ws-font-size26 ws-mid"></i></a>
            <a href="javascript:;"><i class="layui-icon layui-icon-login-wechat"></i></a>
            <a href="javascript:;"><i class="layui-icon layui-icon-login-weibo"></i></a>--%>
          
            <a href="<%=ResolveUrl("~/Views/UserCenter/Login")%>" class="layadmin-user-jump-change layadmin-link layui-hide-xs" lay-tips="Log in with your existing account">用已有帐号登入</a>
        </div>
        </div>
    </div>
    <script src='<%=ResolveUrl("~/Views/UserCenter/Js/Register.js")%>'></script>
</asp:Content>

