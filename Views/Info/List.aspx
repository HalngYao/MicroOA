<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Info.master" AutoEventWireup="true" CodeFile="List.aspx.cs" Inherits="Views_Info_List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
<div class="layui-container">
  <div class="layui-row layui-col-space15">
    <div class="layui-col-md12">
      <div id="divTop" runat="server" class="fly-panel">
        <div class="fly-panel-title fly-filter">
          <a>置顶</a>
        </div>
        <ul id="ulTop" runat="server" class="fly-list">
        </ul>
      </div>

      <div id="divList" runat="server" class="fly-panel" style="margin-bottom: 0;">
        
        <div class="fly-panel-title fly-filter">
          <a>综合</a>
          <span class="fly-filter-right layui-hide-xs layui-hide">
            <a href="" class="layui-this">按最新</a>
            <span class="fly-mid"></span>
            <a href="">按最热</a>
          </span>
        </div>

        <ul id="ulList" runat="server" class="fly-list"></ul>
        <div style="height:30px;"></div>
      </div>
    </div>
  </div>
</div>
<input id="txtMID" type="hidden" runat="server" />
<input id="txtInfoClassID" type="hidden" runat="server" value="0" />
<input id="txtAuxKeyword" type="hidden" runat="server" value="0" />
<script src="<%: ResolveUrl("~/Views/Info/Js/Info.js")%>"></script>
</asp:Content>

