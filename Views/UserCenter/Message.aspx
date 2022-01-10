<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="Message.aspx.cs" Inherits="Views_UserCenter_Message" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="<%: ResolveUrl("~/Resource/ico/iconfont.css")%>" rel="stylesheet" media="all" />
    <link href="<%: ResolveUrl("~/Resource/CSS/approval.css")%>" rel="stylesheet" media="all" />

    <style type="text/css">
        /*弹出层按钮行内背景灰色*/
        .layui-layer-btn {
            border-top: solid 1px #eeeeee;
            background-color: #F8F8F8;
        }
        .ws-msg,.ws-msg a {
         color: #c2c2c2 !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
    <div class="layui-card">
      <div class="layui-tab layui-tab-brief">
        <ul class="layui-tab-title">
          <li class="layui-this">
              全部消息 
              <span id="spanNotice"><%= Notice %></span>
          </li>
        </ul>
        <div class="layui-tab-content">
        
          <div class="layui-tab-item layui-show">
            <div class="micro-message-btns" style="margin-bottom: 10px;">
              <button type="button" class="layui-btn layui-btn-primary layui-btn-sm" data-type="btnReady" lay-tips="标记消息为已读状态<br/>Mark message as read">标记已读</button>
              <button type="button" class="layui-btn layui-btn-primary layui-btn-sm" data-type="btnUnready" lay-tips="标记消息为未读状态<br/>Mark message unread">标记未读</button>
              <button type="button" class="layui-btn layui-btn-primary layui-btn-sm" data-type="btnReadyAll" lay-tips="标记全部消息为已读状态<br/>Mark all messages as read">全部已读</button>

              <button type="button" class="layui-btn layui-btn-primary layui-btn-sm" data-type="btnDelSelected" lay-tips="删除选中的消息<br/>Delete selected message">删除选中</button>
              <button type="button" class="layui-btn layui-btn-primary layui-btn-sm" data-type="btnDelReady" lay-tips="删除已阅读的消息<br/>Delete read messages">删除已读</button>
              <button type="button" class="layui-btn layui-btn-primary layui-btn-sm" data-type="btnDelAll" lay-tips="删除所有消息<br/>Delete all messages">删除所有</button>
            </div>
            
            <table id="tabNotice" lay-filter="tabNotice"></table>
          </div>

        </div>
      </div>
    </div>

    <div id="divScript" runat="server"></div>

    <script type="text/html" id="tplIsRead">
      {{# if(d.IsRead=='True'){ }}  <%-- 已读--%>
        <span class="icon ws-icon icon-yiduxiaoxi ws-font-size20 ws-font-gray2"></span>
      {{# }else{ }}  <%-- 未读--%>
        <span class="icon ws-icon icon-biaoshilei_weiduxinxi ws-font-size18 ws-font-red"></span>
      {{# }}}
    </script>

    <script type="text/html" id="barFormNumber">
      {{# if(d.IsRead=='True'){ }}  <%-- 已读--%>
        <a href="javascript:;" class="layui-table-link ws-font-gray2" lay-event="View">{{ d.FormNumber }}</a>
      {{# }else{ }}  <%-- 未读--%>
        <a href="javascript:;" class="layui-table-link" lay-event="View">{{ d.FormNumber }}</a>
      {{# }}}
    </script>

    <script type="text/html" id="tplTitle">
      {{# if(d.IsRead=='True'){ }}  <%-- 已读--%>
        <span class="ws-font-gray2">{{ d.Title }}</span>
      {{# }else{ }}  <%-- 未读--%>
        <span>{{ d.Title }}</span>
      {{# }}}
    </script>

    <script type="text/html" id="barContent">
      {{# if(d.IsRead=='True'){ }}  <%-- 已读--%>
        <a href="javascript:;" class="layui-table-link ws-font-gray2" lay-event="View" lay-tips="{{ d.Content }}">{{ d.Content.replace(/&lt;br&gt;/g, " ") }}</a>
      {{# }else{ }}  <%-- 未读--%>
        <a href="javascript:;" class="layui-table-link" lay-event="View" lay-tips="{{ d.Content }}">{{ d.Content.replace(/&lt;br&gt;/g, " ") }}</a>
      {{# }}}
    </script>

    <script type="text/html" id="tplDateCreated">
      {{# if(d.IsRead=='True'){ }}  <%-- 已读--%>
        <span class="ws-font-gray2">{{ d.DateCreated }}</span>
      {{# }else{ }}  <%-- 未读--%>
        <span>{{ d.DateCreated }}</span>
      {{# }}}
    </script>

    <input id="txtMID" type="hidden" runat="server" value="26" />
    <input id="txtFormID" type="hidden" runat="server" />
    <input id="txtShortTableName" type="hidden" runat="server" value="Notice" />
    <input id="txtPrimaryKeyName" type="hidden" runat="server" />
    <script src="<%: ResolveUrl("~/Views/UserCenter/Js/Message.js")%>"></script>
</asp:Content>

