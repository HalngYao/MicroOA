<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="CreateMicroDataTable.aspx.cs" Inherits="Views_Set_System_CreateMicroDataTable" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="<%: ResolveUrl("~/Resource/css/micropopup.css")%>" rel="stylesheet" media="all" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
    <div class="layui-card">
        <div class="layui-form layui-card-body" style="padding: 15px;">

            <div class="layui-form-item">
                <label class="layui-form-label">表命名</label>
                <div class="layui-input-inline">
                    <input type="text" name="txtTableName" lay-verify="required" placeholder="如：ITGeneralForm" autocomplete="off" class="layui-input onlyNumAlpha">
                </div>
                <div class="layui-input-inline" style="width:100px;">
                    <input type="text" name="txtShortTableName" lay-verify="required" placeholder="短表名" autocomplete="off" class="layui-input onlyNumAlpha">
                </div>
                <div class="layui-form-mid layui-word-aux">在创建表的同时默认添加Sort【排序】、Invalid【无效】、Del【删除】字段</div>
            </div>
            <div class="layui-form-item">
                <label class="layui-form-label">标题</label>
                <div class="layui-input-inline">
                    <input type="text" name="txtTitle" lay-verify="required" placeholder="如：IT通用表单" autocomplete="off" class="layui-input">
                </div>
                <div class="layui-form-mid layui-word-aux"></div>
            </div>


            <div class="layui-form-item">
                <label class="layui-form-label">属性</label>
                <div class="layui-input-inline" style="width:100px;">
                    <input type="checkbox" id="cktbMainSub" name="cktbMainSub" lay-skin="primary" lay-filter="cktbMainSub" title="启用级联关系">
                </div>
                <div class="layui-form-mid layui-word-aux"> &nbsp;
                    <i class="layui-icon layui-icon-tips" lay-tips="通常用于具有级联关系的表，如【部门表】有部门和子部门、如【菜单表】有父菜单和子菜单等。当启用级联关系时，在创建表的同时默认添加ParentID【父ID】、Level【级别】、LevelCode【级别代码】字段"></i>
                </div>

                <div class="layui-input-inline" style="width:80px; padding-left:15px;">
                    <input type="checkbox" id="ckApprovalForm" name="ckApprovalForm" lay-skin="primary" lay-filter="ckApprovalForm" title="审批表单">
                </div>
                <div class="layui-form-mid layui-word-aux"> &nbsp;
                    <i class="layui-icon layui-icon-tips" lay-tips="在创建新表时创建： FormID【表单ID】、FormNumber【表单号码】、FormState【表单状态】、StateCode【状态代码】、IP【IP地址】"></i>
                </div>

                <div class="layui-input-inline" style="width:80px; padding-left:15px;">
                    <input type="checkbox" id="ckBasicInfo" name="ckBasicInfo" lay-skin="primary" lay-filter="ckBasicInfo" title="基本信息">
                </div>
                <div class="layui-form-mid layui-word-aux"> &nbsp;
                    <i class="layui-icon layui-icon-tips" lay-tips="在创建新表时创建： Applicant【申请者】、Phone【手机】、Tel【分机】、UID【用户ID】（隐藏的）、DisplayName【显示名】（隐藏的）"></i>
                </div>

                <div class="layui-input-inline" style="width:80px; padding-left:15px;">
                    <input type="checkbox" id="ckDept" name="ckDept" lay-skin="primary" lay-filter="ckDept" title="创建部门">
                </div>
                <div class="layui-form-mid layui-word-aux"> &nbsp;
                    <i class="layui-icon layui-icon-tips" lay-tips="在创建新表时创建： DeptID【部门】"></i>
                </div>

                <div class="layui-input-inline" style="width:70px; padding-left:15px;">
                    <input type="checkbox" id="ckPCInfo" name="ckPCInfo" lay-skin="primary" lay-filter="ckPCInfo" title="PC相关">
                </div>
                <div class="layui-form-mid layui-word-aux"> &nbsp;
                    <i class="layui-icon layui-icon-tips" lay-tips="在创建新表时创建： PCLoginID【电脑登录ID】、PCName【电脑名称】、Mail【邮箱地址】"></i>
                </div>

                <div class="layui-input-inline" style="width:80px; padding-left:15px;">
                    <input type="checkbox" id="ckTitleContent" name="ckTitleContent" lay-skin="primary" lay-filter="ckTitleContent" title="标题内容">
                </div>
                <div class="layui-form-mid layui-word-aux"> &nbsp;
                    <i class="layui-icon layui-icon-tips" lay-tips="在创建新表时创建： Title【标题】、Content【内容】"></i>
                </div>

                <div class="layui-input-inline" style="width:80px; padding-left:15px;">
                    <input type="checkbox" id="ckApplicationType" name="ckApplicationType" lay-skin="primary" lay-filter="ckApplicationType" title="申请类型">
                </div>
                <div class="layui-form-mid layui-word-aux"> &nbsp;
                    <i class="layui-icon layui-icon-tips" lay-tips="在创建新表时创建： ApplicationTypeID【申请类型】、ApplicationDate【申请日期】"></i>
                </div>

                <div class="layui-input-inline" style="width:80px; padding-left:15px;">
                    <input type="checkbox" id="ckHopeDate" name="ckHopeDate" lay-skin="primary" lay-filter="ckHopeDate" title="希望对应日">
                </div>
                <div class="layui-form-mid layui-word-aux"> &nbsp;
                    <i class="layui-icon layui-icon-tips" lay-tips="在创建新表时创建：HopeDate【希望对应日】"></i>
                </div>

            </div>


            <div class="layui-form-item">
                <label class="layui-form-label">主键</label>
                <div class="layui-input-inline">
                    <input type="text" name="txtPrimaryKeyName" lay-verify="required" placeholder="主键名" autocomplete="off" class="layui-input onlyNumAlpha">
                </div>
                <div class="layui-form-mid layui-word-aux">主键默认数据类型int，标识1自动增长1</div>
            </div>
            <div class="layui-form-item">
                <label class="layui-form-label">描述</label>
                <div class="layui-input-inline">
                    <textarea name="txtDescription" placeholder="请输入内容"  lay-verify="required" class="layui-textarea"></textarea>
                </div>
                <div class="layui-form-mid layui-word-aux">请描述表的作用</div>
            </div>
            <div class="layui-form-item">
                <label class="layui-form-label"></label>
                <div class="layui-input-inline">
                     <input type="button" id="btnAddTab" runat="server" class="layui-btn layui-hide" lay-filter="btnAddTab" lay-submit value="立即保存" >
                </div>
            </div>

        </div>
    </div>
    <input id="txtMID" type="hidden" runat="server" />
    <script src="<%=ResolveUrl("~/Views/Set/Js/CreateMicroDataTable.js")%>"></script>
</asp:Content>

