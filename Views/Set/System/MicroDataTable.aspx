<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="MicroDataTable.aspx.cs" Inherits="Views_Set_System_MicroDataTable" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="<%: ResolveUrl("~/Resource/css/microlayerbtn.css")%>" rel="stylesheet" media="all" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
    <div class="layui-card">
      <div class="layui-form layui-card-header layuiadmin-card-header-auto">
        <div class="layui-form-item">
          <div class="layui-inline">
            <label class="layui-form-label">Keyword</label>
            <div class="layui-input-block">
              <input type="text" name="txtKeyword" placeholder="请输入" autocomplete="off" class="layui-input">
            </div>
          </div>
          <div class="layui-inline">
            <button class="layui-btn" lay-submit lay-filter="btnSearch">
              <i class="layui-icon layui-icon-search layuiadmin-button-btn"></i>
            </button>
          </div>
          <div class="layui-inline">
              <button type="button" id="btnDel" runat="server" class="layui-btn layui-btn-disabled layui-hide" data-type="btnDel" disabled="disabled">删除</button>
              <button type="button" id="btnAddTab" runat="server" class="layui-btn" data-type="btnAddTab">创建表</button>
              <button type="button" id="btnAddCol" runat="server" class="layui-btn layui-btn-normal" data-type="btnAddCol">添加字段</button>
          </div>
        </div>
      </div>
      
      <div class="layui-card-body">
        <table id="tabTable" lay-filter="tabTable"></table>
      </div>
    </div>

    <script type="text/html" id="swTitleTipsIcon">{{# if(d.ParentID!=0){ }}<input type="checkbox" name="TitleTipsIcon" value="{{d.TID}}" lay-skin="switch" lay-text="是|否" lay-filter="TitleTipsIcon" {{ d.EditTablePermit == 0 ? 'disabled' : '' }} {{ d.TitleTipsIcon == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swAllowNull">{{# if(d.ParentID!=0){ }}<input type="checkbox" name="AllowNull" value="{{d.TID}}" lay-skin="switch" lay-text="是|否" lay-filter="AllowNull" {{ d.EditTablePermit == 0 ? 'disabled' : '' }} {{ d.AllowNull == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swInJoinSql"><input type="checkbox" name="InJoinSql" value="{{d.TID}}" lay-skin="switch" lay-text="是|否" lay-filter="InJoinSql" {{ d.EditTablePermit == 0 ? 'disabled' : '' }} {{ d.InJoinSql == 'True' ? 'checked' : '' }} ></script>

    <script type="text/html" id="swPrimaryKey">{{# if(d.ParentID!=0){ }}<input type="checkbox" name="PrimaryKey" value="{{d.TID}}" lay-skin="switch" lay-text="是|否" lay-filter="PrimaryKey" {{ d.EditTablePermit == 0 ? 'disabled' : '' }} {{ d.PrimaryKey == 'True' ? 'checked' : '' }} >{{# }}}</script>
    <script type="text/html" id="swInvalid"><input type="checkbox" name="Invalid" value="{{d.TID}}" lay-skin="switch" lay-text="是|否" lay-filter="Invalid" {{ d.EditTablePermit == 0 ? 'disabled' : '' }} {{ d.Invalid == 'True' ? 'checked' : '' }} ></script>

    <script type="text/html" id="swDel"><input type="checkbox" name="Del" value="{{d.TID}}" lay-skin="switch" lay-text="是|否" lay-filter="Del" {{ d.EditTablePermit == 0 ? 'disabled' : '' }} {{ d.Del == 'True' ? 'checked' : '' }} ></script>

    <script type="text/html" id="swtbEven">{{# if(d.ParentID==0){ }}<input type="checkbox" name="tbEven" value="{{d.TID}}" lay-skin="switch" lay-text="是|否" lay-filter="tbEven" {{ d.EditTablePermit == 0 ? 'disabled' : '' }} {{ d.tbEven == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swtbPage">{{# if(d.ParentID==0){ }}<input type="checkbox" name="tbPage" value="{{d.TID}}" lay-skin="switch" lay-text="是|否" lay-filter="tbPage" {{ d.EditTablePermit == 0 ? 'disabled' : '' }} {{ d.tbPage == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swtbTotalRow">{{# if(d.ParentID==0){ }}<input type="checkbox" name="tbTotalRow" value="{{d.TID}}" lay-skin="switch" lay-text="是|否" lay-filter="tbTotalRow" {{ d.EditTablePermit == 0 ? 'disabled' : '' }} {{ d.tbTotalRow == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swtbLoading">{{# if(d.ParentID==0){ }}<input type="checkbox" name="tbLoading" value="{{d.TID}}" lay-skin="switch" lay-text="是|否" lay-filter="tbLoading" {{ d.EditTablePermit == 0 ? 'disabled' : '' }} {{ d.tbLoading == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swtbAutoSort">{{# if(d.ParentID==0){ }}<input type="checkbox" name="tbAutoSort" value="{{d.TID}}" lay-skin="switch" lay-text="是|否" lay-filter="tbAutoSort" {{ d.EditTablePermit == 0 ? 'disabled' : '' }} {{ d.tbAutoSort == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swtbMainSub">{{# if(d.ParentID==0){ }}<input type="checkbox" name="tbMainSub" value="{{d.TID}}" lay-skin="switch" lay-text="是|否" lay-filter="tbMainSub" {{ d.EditTablePermit == 0 ? 'disabled' : '' }} {{ d.tbMainSub == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swcolCheckedAll">{{# if(d.ParentID==0){ }}<input type="checkbox" name="colCheckedAll" value="{{d.TID}}" lay-skin="switch" lay-text="是|否" lay-filter="colCheckedAll" {{ d.EditTablePermit == 0 ? 'disabled' : '' }} {{ d.colCheckedAll == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swcolSort">{{# if(d.ParentID!=0){ }}<input type="checkbox" name="colSort" value="{{d.TID}}" lay-skin="switch" lay-text="是|否" lay-filter="colSort" {{ d.EditTablePermit == 0 ? 'disabled' : '' }} {{ d.colSort == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swcolHide">{{# if(d.ParentID!=0){ }}<input type="checkbox" name="colHide" value="{{d.TID}}" lay-skin="switch" lay-text="是|否" lay-filter="colHide" {{ d.EditTablePermit == 0 ? 'disabled' : '' }} {{ d.colHide == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swcolUnReSize">{{# if(d.ParentID!=0){ }}<input type="checkbox" name="colUnReSize" value="{{d.TID}}" lay-skin="switch" lay-text="是|否" lay-filter="colUnReSize" {{ d.EditTablePermit == 0 ? 'disabled' : '' }} {{ d.colUnReSize == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swcolTotalRow">{{# if(d.ParentID!=0){ }}<input type="checkbox" name="colTotalRow" value="{{d.TID}}" lay-skin="switch" lay-text="是|否" lay-filter="colTotalRow" {{ d.EditTablePermit == 0 ? 'disabled' : '' }} {{ d.colTotalRow == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swJoinTableColumn">{{# if(d.ParentID!=0){ }}<input type="checkbox" name="JoinTableColumn" value="{{d.TID}}" lay-skin="switch" lay-text="是|否" lay-filter="JoinTableColumn" {{ d.EditTablePermit == 0 ? 'disabled' : '' }} {{ d.JoinTableColumn == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swctlPrimaryKey">{{# if(d.ParentID!=0){ }}<input type="checkbox" name="ctlPrimaryKey" value="{{d.TID}}" lay-skin="switch" lay-text="是|否" lay-filter="ctlPrimaryKey" {{ d.EditTablePermit == 0 ? 'disabled' : '' }} {{ d.ctlPrimaryKey == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swctlDisplayAsterisk">{{# if(d.ParentID!=0){ }}<input type="checkbox" name="ctlDisplayAsterisk" value="{{d.TID}}" lay-skin="switch" lay-text="是|否" lay-filter="ctlDisplayAsterisk" {{ d.EditTablePermit == 0 ? 'disabled' : '' }} {{ d.ctlDisplayAsterisk == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swctlAddDisplay">{{# if(d.ParentID!=0){ }}<input type="checkbox" name="ctlAddDisplay" value="{{d.TID}}" lay-skin="switch" lay-text="是|否" lay-filter="ctlAddDisplay" {{ d.EditTablePermit == 0 ? 'disabled' : '' }} {{ d.ctlAddDisplay == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swctlAddDisplayButton">{{# if(d.ParentID==0){ }}<input type="checkbox" name="ctlAddDisplayButton" value="{{d.TID}}" lay-skin="switch" lay-text="是|否" lay-filter="ctlAddDisplayButton" {{ d.EditTablePermit == 0 ? 'disabled' : '' }} {{ d.ctlAddDisplayButton == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swctlSaveDraftButton">{{# if(d.ParentID==0){ }}<input type="checkbox" name="ctlSaveDraftButton" value="{{d.TID}}" lay-skin="switch" lay-text="是|否" lay-filter="ctlSaveDraftButton" {{ d.EditTablePermit == 0 ? 'disabled' : '' }} {{ d.ctlSaveDraftButton == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swctlModifyDisplay">{{# if(d.ParentID!=0){ }}<input type="checkbox" name="ctlModifyDisplay" value="{{d.TID}}" lay-skin="switch" lay-text="是|否" lay-filter="ctlModifyDisplay" {{ d.EditTablePermit == 0 ? 'disabled' : '' }} {{ d.ctlModifyDisplay == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swctlModifyDisplayButton">{{# if(d.ParentID==0){ }}<input type="checkbox" name="ctlModifyDisplayButton" value="{{d.TID}}" lay-skin="switch" lay-text="是|否" lay-filter="ctlModifyDisplayButton" {{ d.EditTablePermit == 0 ? 'disabled' : '' }} {{ d.ctlModifyDisplayButton == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swctlViewDisplay">{{# if(d.ParentID!=0){ }}<input type="checkbox" name="ctlViewDisplay" value="{{d.TID}}" lay-skin="switch" lay-text="是|否" lay-filter="ctlViewDisplay" {{ d.EditTablePermit == 0 ? 'disabled' : '' }} {{ d.ctlViewDisplay == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swctlViewDisplayLabel">{{# if(d.ParentID!=0){ }}<input type="checkbox" name="ctlViewDisplayLabel" value="{{d.TID}}" lay-skin="switch" lay-text="是|否" lay-filter="ctlViewDisplayLabel" {{ d.EditTablePermit == 0 ? 'disabled' : '' }} {{ d.ctlViewDisplayLabel == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swctlAfterDisplay">{{# if(d.ParentID!=0){ }}<input type="checkbox" name="ctlAfterDisplay" value="{{d.TID}}" lay-skin="switch" lay-text="是|否" lay-filter="ctlAfterDisplay" {{ d.EditTablePermit == 0 ? 'disabled' : '' }} {{ d.ctlAfterDisplay == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swctlDescriptionDisplayMode"> {{# if(d.ParentID!=0){ }}<input type="checkbox" name="ctlDescriptionDisplayMode" value="{{d.TID}}" lay-skin="switch" lay-text="是|否" lay-filter="ctlDescriptionDisplayMode" {{ d.EditTablePermit == 0 ? 'disabled' : '' }} {{ d.ctlDescriptionDisplayMode == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swqueryAsBaseField">{{# if(d.ParentID!=0){ }}<input type="checkbox" name="queryAsBaseField" value="{{d.TID}}" lay-skin="switch" lay-text="是|否" lay-filter="queryAsBaseField" {{ d.queryAsBaseField == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swqueryAsAdvancedField">{{# if(d.ParentID!=0){ }}<input type="checkbox" name="queryAsAdvancedField" value="{{d.TID}}" lay-skin="switch" lay-text="是|否" lay-filter="queryAsAdvancedField" {{ d.queryAsAdvancedField == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swqueryAsKeywordField">{{# if(d.ParentID!=0){ }}<input type="checkbox" name="queryAsKeywordField" value="{{d.TID}}" lay-skin="switch" lay-text="是|否" lay-filter="queryAsKeywordField" {{ d.queryAsKeywordField == 'True' ? 'checked' : '' }} >{{# }}}</script>


    <input id="txtMID" type="hidden" runat="server" />
    <input id="txtEditTablePermit" type="hidden" runat="server" value="0" />
    <script src="<%=ResolveUrl("~/Views/Set/Js/MicroDataTable.js")%>"></script>

</asp:Content>

