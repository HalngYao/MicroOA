<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Login.master" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Views_UserCenter_Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style type="text/css">
      body .ws-text-border-red {border:solid 1px red !important;  }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
    <div class="layadmin-user-login-main">
        <div class="layadmin-user-login-box layadmin-user-login-header">
            <img src="<%: GetMicroInfo("Logo") %>" alt="" style="width:270px; height:110px; border-radius:6px;">
            <p><%: GetMicroInfo("Title") %></p>
        </div>
        <div class="layadmin-user-login-box layadmin-user-login-body layui-form">
            <div class="layui-form-item">
                <label class="layadmin-user-login-icon layui-icon layui-icon-username" for="txtUserName"></label>
                <input type="text" name="txtUserName" id="txtUserName" lay-verify="UserName" placeholder="用户名" micro-tips="请输入用户名 <br/> Please enter a user name" class="layui-input onlyNumAlpha" value="admin">
            </div>
            <div class="layui-form-item">
                <label class="layadmin-user-login-icon layui-icon layui-icon-password" for="txtUserPassword"></label>
                <input type="password" name="txtUserPassword" id="txtUserPassword" lay-verify="UserPassword" placeholder="密码" micro-tips="请输入密码 <br/> Please enter your password" class="layui-input" value="admin">
            </div>
            <div id="divValidateCode" runat="server" class="layui-form-item">
                <div class="layui-row">
                    <div class="layui-col-xs8">
                        <label class="layadmin-user-login-icon layui-icon layui-icon-vercode" for="txtValidateCode"></label>
                        <input type="text" name="txtValidateCode" id="txtValidateCode" lay-verify="ValidateCode" placeholder="验证码 / ValidateCode" micro-tips="请输入验证码 <br/> Please input verification code" class="layui-input onlyNumAlpha">
                    </div>
                    <div class="layui-col-xs4">
                        <div style="margin-left: 10px;">
                            <img src="<%=ResolveUrl("~/Resource/ValidateCode/ValidateImage")%>" id="imgCode"  title="看不清换一张 I can't see it change one" style="cursor: pointer; margin-top:1px; width:100px; height:36px;"/>
                        </div>
                    </div>
                </div>
            </div>
            <div class="layui-form-item" style="margin-bottom: 20px;">
                <div id="divAutoLogin" runat="server" class="layui-inline" lay-tips="Remember login">
                    <input type="checkbox" id="cbAutoLogin" name="cbAutoLogin" lay-skin="primary" title="下次自动登录">
                </div>
                <a href="<%: ResolveUrl("~/Views/UserCenter/Forget")%>" class="layadmin-user-jump-change layadmin-link layui-hide" style="margin-top: 7px;" lay-tips="Forget password?">忘记密码？</a>
            </div>
            <div class="layui-form-item">
                <button type="button" class="layui-btn layui-btn-fluid" lay-submit="" lay-filter="btnLogin">登 录 Login</button>
            </div>
            <div class="layui-trans layui-form-item layadmin-user-login-other">
                <asp:HyperLink ID="hlWinLogin" runat="server" lay-tips="以当前Windows ID一键登录">Windows ID一键登录<i class="ws-icon icon-windows1 ws-font-size26 ws-mid"></i></asp:HyperLink>
                <%--
                <a href="javascript:;"><i class="layui-icon layui-icon-login-wechat"></i></a>
                <a href="javascript:;"><i class="layui-icon layui-icon-login-weibo"></i></a>      
                --%>
                <asp:HyperLink ID="hlRegister" runat="server" CssClass="layadmin-user-jump-change layadmin-link layui-hide-xs" lay-tips="New user registration" NavigateUrl="~/Views/UserCenter/Register">新用户注册</asp:HyperLink>
            </div>
        </div>
    </div>
    <input id="hidCheckBrowser" type="hidden" runat="server"/>
    <input id="hidUnsupportedBrowserTips" type="hidden" runat="server"/>
    <script src='<%: ResolveUrl("~/Views/UserCenter/Js/Login.js")%>'></script>


</asp:Content>

