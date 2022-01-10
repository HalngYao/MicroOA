<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="UserPassword.aspx.cs" Inherits="Views_UserCenter_UserPassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server" ClientIDMode="Static">
    <div class="layui-card">

        <div class="layui-card-header ">
            修改密码
        </div>

        <div class="layui-card-body layui-form">
            <div class="layui-row">
                <div id="divUserPassword" runat="server" class="layui-form-item layui-row ">
                    <div class="layui-inline ">
                        <label class="layui-form-label "><i class="ws-must-asterisk">*</i>当前密码</label>
                        <div class="layui-input-inline">
                            <input type="password" id="txtUserPassword" name="txtUserPassword" runat="server" value="" placeholder="" lay-verify="required" lay-reqtext="当前密码不能为空<br/>This cannot be empty" autocomplete="off" class="layui-input">
                        </div>
                        <div class="layui-form-mid layui-word-aux"></div>
                    </div>
                </div>
                <div class="layui-form-item layui-row ">
                    <div class="layui-inline ">
                        <label class="layui-form-label"><i class="ws-must-asterisk">*</i> 新密码</label>
                        <div class="layui-input-inline ">
                            <input type="password" id="txtNewUserPassword" name="txtNewUserPassword" value="" placeholder="" lay-verify="required|NewPassword" lay-reqtext="新密码不能为空<br/>This cannot be empty" autocomplete="off" class="layui-input" >
                        </div>
                        <div class="layui-form-mid layui-word-aux">密码8~16位</div>
                    </div>
                </div>
                <div class="layui-form-item layui-row ">
                    <div class="layui-inline ">
                        <label class="layui-form-label "><i class="ws-must-asterisk">*</i> 确认新密码</label>
                        <div class="layui-input-inline ">
                            <input type="password" id="txtConfirmPassword" name="txtConfirmPassword" value="" placeholder="" lay-verify="required|ConfirmPassword" lay-reqtext="确认新密码不能为空<br/>This cannot be empty" autocomplete="off" class="layui-input ">
                        </div>
                        <div class="layui-form-mid layui-word-aux">密码8~16位</div>
                    </div>
                </div>
                <div class="layui-form-item layui-row ">
                    <div class="layui-inline ">
                        <div class="layui-form-mid layui-word-aux">备注：修改密码只能修改本系统登录密码，并不能修改Windows登录密码。
                            <br/>Tips: Changing the password can only change the login password of this system, but not the Windows login password.</div>
                    </div>
                </div>
                <div class="layui-form-item ws-margin-top10">
                    <div class="layui-input-block">
                        <button type="button" id="btnModify" class="layui-btn layui-btn-normal" lay-submit lay-filter="btnModify">修改</button>
                        <%--<button type="reset" class="layui-btn layui-btn-primary">立即重置</button>--%>
                    </div>
                </div>
            </div>
        </div>

    </div>
    <input id="txtMID" type="hidden" runat="server" />
    <script src="<%=ResolveUrl("~/Views/UserCenter/Js/UserPassword.js?version=1")%>"></script>
</asp:Content>

