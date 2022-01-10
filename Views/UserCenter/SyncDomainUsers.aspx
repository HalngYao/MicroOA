<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="SyncDomainUsers.aspx.cs" Inherits="Views_UserCenter_SyncDomainUsers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="<%=ResolveUrl("~/Resource/CSS/BackgroundWhite.css")%>" rel="stylesheet" media="all" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
    <div class="layui-card">
        <div class="layui-card-body">
            <div class="layui-tab-item layui-show layui-form layui-form-pane">

                <div class="layui-form-item layui-row">
                    <div class="layui-col-xs12 layui-col-sm6 layui-col-md6 layui-col-lg6">
                        <label class="layui-form-label">帐号</label>
                        <div class="layui-input-block">
                            <input type="text" name="txtUserName" id="txtUserName" lay-verify="required" placeholder="Domain\UserName" autocomplete="off" class="layui-input" value="">
                        </div>
                    </div>
                </div>
                <div class="layui-form-item layui-row">
                    <div class="layui-col-xs12 layui-col-sm6 layui-col-md6 layui-col-lg6">
                    <label class="layui-form-label">密码</label>
                        <div class="layui-input-block">
                            <input type="password" name="txtPassword" id="txtPassword" lay-verify="required" placeholder="" autocomplete="off" class="layui-input" value="">
                        </div>
                    </div>
                </div>
                <div class="layui-form-item layui-row">
                    <div class="layui-col-xs12 layui-col-sm6 layui-col-md6 layui-col-lg6">
                    <label class="layui-form-label">域名</label>
                        <div class="layui-input-block">
                            <input type="text" name="txtDomainName" id="txtDomainName" lay-verify="required" placeholder="完整的域名，如：domain.com" autocomplete="off" class="layui-input " value="micro-oa.com">
                        </div>
                    </div>
                </div>
                <div class="layui-form-item layui-row">
                    <div class="layui-col-xs12 layui-col-sm6 layui-col-md6 layui-col-lg6">
                    <label class="layui-form-label">LDAP OU</label>
                        <div class="layui-input-block">
                            <input type="text" name="txtLdapOU" id="txtLdapOU" lay-verify="required" placeholder="LDAP路径，如：OU=Account,OU=Global" autocomplete="off" class="layui-input" value="">
                        </div>
                    </div>
                </div>
                <div class="layui-form-item layui-row">
                    <div class="layui-col-xs12 layui-col-sm6 layui-col-md6 layui-col-lg6">
                        <button type="button" class="layui-btn layui-btn-normal" lay-submit lay-filter="btnTest" id="btnTest" runat="server">测试连接</button>
                        <button type="button" class="layui-btn" lay-submit lay-filter="btnSave" id="btnSave" runat="server">立即同步</button>
                    </div>
                </div>
                <div class="layui-form-item" style=" margin-top:50px; color:Gray;">  
                    <blockquote class="layui-elem-quote">同步域控用户只同步域控的以下属性：<br>
                        用户ID：sAMAccountName，姓名：ChineseName，显示名称：DisplayName，描述：Description，邮箱地址：E-mail Address，部门：Department
                    </blockquote>
                </div>
            </div>
        </div>

    </div>
    <div id="divScript" runat="server"></div>
    <input id="txtMID" type="hidden" runat="server" />
    <script src="<%=ResolveUrl("~/Views/UserCenter/Js/SyncDomainUsers.js")%>"></script>
</asp:Content>

