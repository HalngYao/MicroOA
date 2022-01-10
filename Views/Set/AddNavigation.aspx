<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="AddNavigation.aspx.cs" Inherits="Views_Set_AddNavigation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script src="<%: ResolveUrl("~/Resource/xmSelect/xmselect.js")%>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
    <div class="layui-card">
        <div class="layui-form layui-card-body" style="padding: 15px;">
            <div class="layui-form-item">
                <label class="layui-form-label">名称</label>
                <div class="layui-input-inline">
                    <input type="text" name="txtModuleName" lay-verify="required" autocomplete="off" class="layui-input">
                </div>
            </div>
            <div class="layui-form-item">
                <label class="layui-form-label">上级导航</label>
                <div class="layui-input-inline">
                    <select id="selParentID" name="selParentID" lay-verify="" lay-filter="selParentID">
                        <option value="">--顶级导航--</option>
                    </select>
                </div>
                <div class="layui-form-mid layui-word-aux">
                    当上级名称为空时，新增的导航名称为顶级
                </div>
            </div>
            <div class="layui-form-item">
                <label class="layui-form-label">排序</label>
                <div class="layui-input-inline">
                    <input type="text" name="txtSort" lay-verify="required|number" autocomplete="off" class="layui-input onlyNum">
                </div>
            </div>
            <div class="layui-form-item layui-hide1">
                <label class="layui-form-label">层级</label>
                <div class="layui-input-inline">
                    <input type="text" id="txtLevel"  name="txtLevel" lay-verify="required|number" autocomplete="off" class="layui-input" value="0">
                </div>
                <div class="layui-form-mid layui-word-aux">
                    <i id="tipsLevel" class="layui-icon layui-icon-about"></i>
                    0=父层，1=二层，2=三层...
                </div>
            </div>
            <div class="layui-form-item" pane="">
                <label class="layui-form-label">属性</label>
                <div class="layui-input-block">
                    <input type="checkbox" name="ckShow" lay-skin="primary" checked="checked" title="默认显示">
                    <input type="checkbox" name="ckExpand" lay-skin="primary" checked="checked" title="默认展开">
                    <input type="checkbox" name="ckForm" lay-skin="primary" title="设为表单">
                    <input type="checkbox" name="ckDisable" lay-skin="primary" title="默认禁用">
                </div>
            </div>
            <div class="layui-form-item">
                <label class="layui-form-label">图标</label>
                <div class="layui-input-block">
                    <input type="text" name="txtIcon" lay-verify="" placeholder="请输入" autocomplete="off" class="layui-input">
                </div>

            </div>
            <div class="layui-form-item">
                <label class="layui-form-label">URL</label>
                <div class="layui-input-block">
                    <input type="text" name="txtURL" lay-verify="" placeholder="请输入" autocomplete="off" class="layui-input">
                </div>
            </div>
            <div class="layui-form-item">
                <label class="layui-form-label">描述</label>
                <div class="layui-input-block">
                    <input type="text" name="txtDescription" lay-verify="" placeholder="请输入" autocomplete="off" class="layui-input">
                </div>
            </div>

            <div class="layui-form-item layui-layout-admin layui-hide">
                <div class="layui-input-block">
                    <div class="layui-footer" style="left: 0;">
                        <button type="button" class="layui-btn" id="btnSave" lay-submit="" lay-filter="btnSave">立即提交</button>
                    </div>
                </div>
            </div>

        </div>
    </div>
    <input id="txtMID" type="hidden" runat="server" />
    <script src="<%=ResolveUrl("~/Views/Set/Navigation.js")%>"></script>
</asp:Content>

