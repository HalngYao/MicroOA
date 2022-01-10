<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Views_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title><%: GetMicroInfo("Title") %></title>
    <meta name="description" content="<%: MicroPublicHelper.MicroPublic.GetMicroInfo("MetaDescription") %>" />
    <meta name="keywords" content="<%: MicroPublicHelper.MicroPublic.GetMicroInfo("MetaKeyword") %>" />
    <meta name="renderer" content="webkit">
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1">
    <meta name="viewport" content="width=device-width, initial-scale=1.0, minimum-scale=1.0, maximum-scale=1.0, user-scalable=0">
    <link rel="stylesheet" href="<%: ResolveUrl("~/layuiadmin/layui/css/layui.css")%>" media="all">
    <link rel="stylesheet" href="<%: ResolveUrl("~/layuiadmin/style/admin.css")%>" media="all">
    <link rel="stylesheet" href="<%: ResolveUrl("~/Resource/css/micro.css")%>" media="all" />
    <link rel="stylesheet" href="<%: ResolveUrl("~/Resource/ico/iconfont.css")%>" media="all" />
  <style>
      #citeState:after {
      content:"|";
      margin-left:3px;
      color:#cccccc;
      }

    .layui-layer-btn {
        border-top: solid 1px #eeeeee;
        background-color: #F8F8F8;
        }
  </style>
</head>
<body>
    <form id="form1" runat="server">
      <div id="LAY_app">
        <div class="layui-layout layui-layout-admin">
          <div class="layui-header">
            <!-- 头部区域 -->
            <ul class="layui-nav layui-layout-left">
              <li class="layui-nav-item layadmin-flexible" lay-tips="侧边伸缩" lay-unselect>
                <a href="javascript:;" layadmin-event="flexible">
                  <i class="layui-icon layui-icon-shrink-right ws-font-size20" id="LAY_app_flexible"></i>
                </a>
              </li>
<%--              <li class="layui-nav-item layui-hide-xs" lay-tips="主页/ホームページ" lay-unselect>
                <a lay-href="/Views/Home/Console/2" title="主页">
                  <i class="ws-icon icon-shouye ws-font-size24"></i>
                </a>
              </li>--%>
              <li class="layui-nav-item layui-hide-xs" lay-tips="OA系统" lay-unselect>
                <a lay-href="<%: ResolveUrl("~/Views/Home/Console/2")%>">
                  <i class="layui-icon ws-icon icon-OA ws-font-size20"></i><%--OA--%>
                </a>
              </li>
              <li class="layui-nav-item layui-hide-xs" lay-tips="信息平台" lay-unselect>
                <a href="/Views/Info/List" target="_blank">
                  <i class="layui-icon ws-icon icon-art2 ws-font-size20"></i><%--信息平台--%>
                </a>
              </li>
              <%=TopMenu() %>
              <li class="layui-nav-item layui-hide-xs" lay-tips="刷新" lay-unselect>
                <a href="javascript:;" layadmin-event="refresh">
                  <i class="layui-icon layui-icon-refresh-3 ws-font-size20"></i><%--刷新--%>
                </a>
              </li>
                <%--搜索--%>
              <%--<li class="layui-nav-item layui-hide-xs" lay-unselect>
                <input type="text" placeholder="搜索..." autocomplete="off" class="layui-input layui-input-search" layadmin-event="serach" lay-action="template/search.html?keywords="> 
              </li>--%>
            </ul>
            <ul class="layui-nav layui-layout-right" lay-filter="layadmin-layout-right">
          
              <li class="layui-nav-item" lay-tips="消息中心" lay-unselect>
                <a lay-href="<%: ResolveUrl("~/Views/UserCenter/Message")%>" layadmin-event="message" lay-text="消息中心">
                  <i class="layui-icon layui-icon-notice"></i>    
                  <!-- 如果有新消息，则显示小圆点 -->
                  <%--<span class="layui-badge-dot"></span>--%>
                  <span id="spanNotice"><%= Notice %></span>
                </a>
              </li>
              <li style="left:12px;" class="layui-nav-item layui-hide-xs" lay-tips="配色方案" lay-unselect>
                <a href="javascript:;" layadmin-event="theme">
                  <i class="layui-icon layui-icon-theme"></i>
                </a>
              </li>
<%--              <li class="layui-nav-item layui-hide-xs" lay-unselect>
                <a href="javascript:;" layadmin-event="note">
                  <i class="layui-icon layui-icon-note"></i>
                </a>
              </li>--%>
              <li class="layui-nav-item layui-hide-xs" lay-tips="全屏" lay-unselect>
                <a href="javascript:;" layadmin-event="fullscreen">
                  <i class="layui-icon layui-icon-screen-full"></i>
                </a>
              </li>
              <li class="layui-nav-item">
                <a href="javascript:;">
                    <asp:Image ID="imgAvatar" runat="server" CssClass="layui-nav-img" ImageUrl="~/Resource/UploadFiles/Images/Avatar/default.jpg" />
                    <span id="spanSetate" class="layui-badge-dot ws-bg-green" style="position:absolute; left:50px; top:27px;" runat="server"></span>
                    <cite id="citeState" runat="server">有空</cite>
                    <cite><%=GetDisplayName()%></cite>
                </a>                
                <dl class="layui-nav-child">
                    <dd><a lay-href="<%: ResolveUrl("~/Views/UserCenter/UserBasicInfo/16")%>" title="Basic Info"><i class="ws-icon icon-shezhi1 ws-font-size20" style="top: 4px; margin-right:5px;"></i>基本设置</a></dd>
                    <dd><a lay-href="<%: ResolveUrl("~/Views/UserCenter/UserPassword/17")%>" title="Change Password"><i class="ws-icon icon-xiugaimima ws-font-size20" style="top: 4px; margin-right:5px;"></i>修改密码</a></dd>

                    <hr>
                    <dd layadmin-event="logout1"><asp:LinkButton ID="lbLogOff" runat="server" OnClick="lbLogOff_Click"><i class="ws-icon icon-tuichu5 ws-font-size18" style="top: 4px; margin-right:5px;"></i>退出</asp:LinkButton></dd>
                </dl>

              </li>
          
              <li class="layui-nav-item layui-hide-xs" lay-tips="About" lay-unselect>
                <a href="javascript:;" layadmin-event="about"><i class="layui-icon layui-icon-more-vertical"></i></a>
              </li>
              <li class="layui-nav-item layui-show-xs-inline-block layui-hide-sm" lay-tips="More" lay-unselect>
                <a href="javascript:;" layadmin-event="more"><i class="layui-icon layui-icon-more-vertical"></i></a>
              </li>
            </ul>
          </div>
      
          <!-- 侧边菜单 -->
          <div class="layui-side layui-side-menu">
            <div class="layui-side-scroll">
               <%-- home/console.html--%>
              <div class="layui-logo" lay-href="<%: ResolveUrl("~/Views/Home/Console/2")%>">
                  <span><img src="<%: GetMicroInfo("Logo") %>" style="width:100px; height:34px; background-color:#ffffff; border-radius:4px;"></span>
              </div>        
              <ul class="layui-nav layui-nav-tree" lay-shrink="all" id="LAY-system-side-menu" lay-filter="layadmin-system-side-menu">
                  <%= LeftMenu() %>
              </ul>
            </div>
          </div>

          <!-- 页面标签 -->
          <div class="layadmin-pagetabs" id="LAY_app_tabs">
            <div class="layui-icon layadmin-tabs-control layui-icon-prev" layadmin-event="leftPage"></div>
            <div class="layui-icon layadmin-tabs-control layui-icon-next" layadmin-event="rightPage"></div>
            <div class="layui-icon layadmin-tabs-control layui-icon-down">
              <ul class="layui-nav layadmin-tabs-select" lay-filter="layadmin-pagetabs-nav">
                <li class="layui-nav-item" lay-unselect style="text-align:left;">
                  <a href="javascript:;"></a>
                  <dl class="layui-nav-child layui-anim-fadein">
                    <dd layadmin-event="closeThisTabs"><a href="javascript:;">关闭当前标签页</a></dd>
                    <dd layadmin-event="closeOtherTabs"><a href="javascript:;">关闭其它标签页</a></dd>
                    <dd layadmin-event="closeAllTabs"><a href="javascript:;">关闭全部标签页</a></dd>
                  </dl>
                </li>
              </ul>
            </div>
            <div class="layui-tab" lay-unauto lay-allowClose="true" lay-filter="layadmin-layout-tabs">
              <ul class="layui-tab-title" id="LAY_app_tabsheader">
                <li lay-id="<%: ResolveUrl("~/Views/Home/Console/2")%>" lay-attr="<%: ResolveUrl("~/Views/Home/Console/2")%>" class="layui-this"><i class="layui-icon layui-icon-home"></i></li>
              </ul>
            </div>
          </div>
          
          <!-- 主体内容 -->
          <div class="layui-body" id="LAY_app_body">
            <div class="layadmin-tabsbody-item layui-show">
              <iframe src="<%: ResolveUrl("~/Views/Home/Console/2")%>" frameborder="0" class="layadmin-iframe"></iframe>
            </div>
          </div>
      
          <!-- 辅助元素，一般用于移动设备下遮罩 -->
          <div class="layadmin-body-shade" layadmin-event="shade"></div>
        </div>
      </div>
    <input id="hidGlobalTips" type="hidden" runat="server" />
    <input id="hidCheckBrowser" type="hidden" runat="server"/>
    <input id="hidUnsupportedBrowserTips" type="hidden" runat="server"/>

    <script src='<%: ResolveUrl("~/layuiadmin/layui/layui.js")%>'></script>
    <script src='<%: ResolveUrl("~/Views/Js/Default.js?ver="+ DateTime.Now.ToFileTime()+"")%>'></script>
    </form>
</body>
</html>
