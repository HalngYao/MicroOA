<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="Forms.aspx.cs" Inherits="Views_Forms_Forms" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style type="text/css">    
        .layui-nav{position:relative;padding:0 20px;background-color:#fff;color:#000!important;border-radius:2px;font-size:0;box-sizing:border-box;}  /*定义nav背景颜色为白色，字体颜色黑色*/
        .layui-nav .layui-nav-item a{display:block;padding:0 20px;color:#000 !important;color:rgba(0,0,0,.7)!important;transition:all .3s;-webkit-transition:all .3s;}
        .layui-nav .layui-nav-more{content:'';width:0;height:0;border-style:solid dashed dashed;border-color:#fff transparent transparent !important;overflow:hidden;cursor:pointer;transition:all .2s;-webkit-transition:all .2s;position:absolute;top:50%;right:8px;margin-top:-4px;border-width:6px;border-top-color:rgba(0,0,0,.5)!important;}  /*三角符号生成，位置和颜色*/
        .layui-nav .layui-nav-mored,.layui-nav-itemed>a .layui-nav-more{margin-top:-9px;border-style:dashed dashed solid;border-color:transparent transparent rgba(0,0,0,.5)!important;}
        .layui-nav-tree .layui-nav-bar{width:5px;height:0;background-color:#009688; display:none;}
        .layui-nav-tree .layui-nav-child dd.layui-this,.layui-nav-tree .layui-nav-child dd.layui-this a,.layui-nav-tree .layui-this,.layui-nav-tree .layui-this>a,.layui-nav-tree .layui-this>a:hover{background-color:#FFFFFF!important;color:#009688!important;}  /*选中时的样式*/
        .layui-nav-itemed>a{border-left:5px solid #009688!important; font-weight:bold;}
        .layui-nav-item>a, layui-nav-item>a {border-left:5px solid #009688!important; background-color:#f2f2f2!important; font-weight:bold;}
        .layui-nav-tree .layui-nav-child,.layui-nav-tree .layui-nav-child a:hover{background:0 0;color:#009688!important;} /*鼠标移过背景颜色和字体颜色*/
        .layui-nav-itemed>.layui-nav-child{display:block;padding:0;background-color:rgba(255,255,255,.3)!important;} /*子菜单背景颜色*/
        .layui-nav-child a { display:inline!important;}
        .layui-nav-child dd span{opacity:0;}  /*默认隐藏*/
        .layui-nav-child dd:hover span{opacity:1;display:inline!important; right:3px; height:40px;line-height:40px;position:absolute; cursor:pointer;}  /*鼠标悬停显示*/
        .layui-nav-item >a +span {opacity:0; display:inline!important; color:#009688; position:absolute; right:-34px; top:0px;cursor:pointer;}
        .layui-nav-item:hover >a +span {opacity:1; display:inline!important; color:#009688; position:absolute; right:-34px; top:0px;cursor:pointer;}  /*鼠标悬停显示*/
        /*更多操作的下拉按钮三角箭头取消继承父样式*/
        .micro-btn .layui-icon {all:unset; }
        
        .layui-layer-btn {
            border-top: solid 1px #eeeeee;
            background-color: #F8F8F8;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
    <div class="layui-card">
        <div class="layui-form layui-card-header layuiadmin-card-header-auto">
            <div class="layui-row">
                <%--left--%>
                <div class="layui-col-xs6 layui-col-sm6 layui-col-md2 layui-col-lg2">
                    表单列表
                </div>
                <div class="layui-col-xs6 layui-col-sm6 layui-col-md10 layui-col-lg10">
                    <div class="layui-form-item">
                        <div class="layui-inline">
                            <div class="layui-input-inline" style="left:20px;">
                                <input type="text" name="txtKeyword" placeholder="关键字/Keyword" autocomplete="off" class="layui-input">
                            </div>
                        </div>
                        <div class="layui-inline">
                            <button type="button" class="layui-btn" lay-submit lay-filter="btnSearch">
                                <i class="layui-icon layui-icon-search layuiadmin-button-btn"></i>
                            </button>
                        </div>
                        <div class="layui-inline">
                            <button type="button" id="btnDel" runat="server" class="layui-btn layui-btn-danger" data-type="btnDel">删除</button>
                        </div>
                        <div class="layui-inline">
                            <button type="button" id="btnAddOpenLink" runat="server" class="layui-btn" data-type="btnAddOpenLink"><i class="layui-icon layui-icon-add-1 layuiadmin-button-btn"></i>添加表单</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="layui-card-body">
            <div class="layui-row">
                <%--left--%>
                <div class="layui-col-xs2 layui-col-sm2 layui-col-md2 layui-col-lg2 ws-scrollbar" style="overflow: auto !important;height: 730px !important;">
                    <ul class="layui-nav layui-nav-tree layui-inline">
                        <%=GetLeftNav() %>
                    </ul>
                </div>
                <div class="layui-col-xs10 layui-col-sm10 layui-col-md10 layui-col-lg10" style="padding-left:10px;">
                    <div style="padding-bottom: 10px;">
                        <table id="tabTable" lay-filter="tabTable"></table>
                    </div>
                </div>

            </div>
        </div>

    </div>
    <div id="divScript" runat="server"></div>
    <input id="txtMID" type="hidden" runat="server" />
    <script type="text/html" id="microBar">
        <a id="linkAdd" runat="server" class="layui-btn layui-btn-xs" lay-event="Add" lay-tips="【{{ d.FormName }}】 填写申请"><i class="layui-icon layui-icon-add-1"></i>申请</a>
        <a id="linkView" runat="server" class="layui-btn layui-btn-xs layui-btn-normal" lay-event="View" lay-tips="【{{ d.FormName }}】 查看申请记录"><i class="layui-icon layui-icon-list"></i>记录</a>
        {{#  if(d.FormID == "5" || d.FormID == "6"){ }}
            <a id="linkDraft" runat="server" class="layui-btn layui-btn-xs layui-btn-warm" lay-event="Draft" lay-tips="草稿箱"><i class="ws-icon icon-cunrucaogaoxiang"></i>草稿</a>
        {{#  } else { }}
            <a id="linkDraft2" runat="server" class="layui-btn layui-btn-xs layui-btn-warm layui-btn-disabled" lay-tips="草稿箱暂不提供"><i class="ws-icon icon-cunrucaogaoxiang"></i>草稿</a>
        {{#  } }}
        
        <a id="linkModify" runat="server" class="layui-btn layui-btn-xs layui-btn-normal" lay-event="Modify" lay-tips="编辑表单"><i class="layui-icon layui-icon-edit"></i>修改</a>
    </script>

    <script src="<%: ResolveUrl("~/Views/Forms/Js/Forms.js")%>"></script>

</asp:Content>

