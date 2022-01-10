<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="CodeTools.aspx.cs" Inherits="Views_Tools_CodeTools" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%--    <link href="<%: ResolveUrl("~/resource/elementui/lib/theme-chalk/index.css")%>" rel="stylesheet" media="all" />
    <script src='<%: ResolveUrl("~/Resource/ElementUI/vue.js")%>'></script>
    <script src='<%: ResolveUrl("~/Resource/ElementUI/lib/index.js")%>'></script>--%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
    <div class="layui-card">
        <div class="layui-form layui-card-header layuiadmin-card-header-auto">
            <div class="layui-form-item">
                <label class="layui-form-label">数据表</label>
                <div class="layui-input-block">
                    <div class="layui-inline">
                        <select id="selTableName" name="selTableName" lay-verify="required" lay-filter="selTableName">
                            <option value="">请选择</option>
                        </select>
                        <input type="text" id="txtTableName" name="txtTableName" runat="server" lay-verify="required" placeholder="" autocomplete="off" class="layui-input layui-hide">
                    </div>



<%--                    <div id="app" class="layui-inline">
                        <div class="block">
                            <el-cascader 
                             v-model="value"
                                :rules="[{ required: true, message: '请输入电话号码', trigger: 'blur' }]"
                            :options="options"
                            :props="{ expandTrigger: 'hover' }"
                            @change="handleChange" :show-all-levels="false" filterable clearable ></el-cascader>
                        </div>
                    </div>--%>

                    <div class="layui-inline">
                        <button type="button" id="btnInsert" runat="server" class="layui-btn" data-type="btnInsert" lay-submit>GetInsert</button>
                    </div>
                    <div class="layui-inline">
                        <button type="button" id="btnUpdate" runat="server" class="layui-btn" data-type="btnUpdate" lay-submit>GetUpdate</button>
                    </div>
                    <div class="layui-inline">
                        <button type="button" id="btnField" runat="server" class="layui-btn" data-type="btnField" lay-submit>GetFields</button>
                    </div>

                </div>
            </div>
<%--            <div class="layui-form-item">
                <button type="button" class="layui-btn layui-btn-fluid" lay-submit lay-filter="btnSave">注 册 Register</button>
            </div>--%>

        </div>

        <div class="layui-card-body">
            <div id="divContent" runat="server" style="padding-bottom: 10px;">
                <br />
                <br />
            </div>
        </div>
    </div>
    <input id="txtMID" type="hidden" runat="server" />
   <script>
       layui.use(['index', 'form', 'micro'], function () {
           var $ = layui.$
               , form = layui.form
               , micro = layui.micro;

           var MID = micro.getUrlPara('mid');

           var Fields = { "txt": "TabColName", "val": "TID", "stn": "mTabs", "ob": "TID" };
           micro.getTxtVal("getalltn", "selTableName", "", Fields);

           form.on('select(selTableName)', function (data) {
               $("#txtTableName").val(($(data.elem).find("option:selected").text()));
           });

           //事件
           var active = {
               btnInsert: function () {
                   var Fields = { "TableName": $("#txtTableName").val() };
                   Fields = encodeURI(JSON.stringify(Fields));
                   var Parameter = { "action": "insert", "mid": MID, "fields": Fields };
                   var flag = micro.getAjaxData('text', '/Views/Tools/CodeTools.ashx', Parameter);
                   $("#divContent").html(flag);
                   //$('.el-input__inner').focus();
               }
               , btnUpdate: function () {
                   var Fields = { "TableName": $("#txtTableName").val() };
                   Fields = encodeURI(JSON.stringify(Fields));
                   var Parameter = { "action": "update", "mid": MID, "fields": Fields };
                   var flag = micro.getAjaxData('text', '/Views/Tools/CodeTools.ashx', Parameter);
                   $("#divContent").html(flag);
               }
               , btnField: function () {
                   var Fields = { "TableName": $("#txtTableName").val() };
                   Fields = encodeURI(JSON.stringify(Fields));
                   var Parameter = { "action": "field", "mid": MID, "fields": Fields };
                   var flag = micro.getAjaxData('text', '/Views/Tools/CodeTools.ashx', Parameter);
                   $("#divContent").html(flag);
               }
           };

           $('.layui-btn').on('click', function () {
               var type = $(this).data('type');
               active[type] ? active[type].call(this) : '';
           });

        });

   </script>
</asp:Content>

