<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="MicroFormList.aspx.cs" Inherits="Views_Forms_MicroFormList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="<%: ResolveUrl("~/Resource/css/microlayerbtn.css")%>" rel="stylesheet" media="all" />
    <link href="<%: ResolveUrl("~/Resource/css/approval.css")%>" rel="stylesheet" media="all" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
    <div class="layui-card">
        <div class="layui-form layui-card-header layuiadmin-card-header-auto">
            <div class="layui-form-item">
                <div class="layui-inline" style="margin-right:100px;">
                    <button type="button" id="btnAddOpenLink" runat="server" class="layui-btn" data-type="btnAddOpenLink"><i class="layui-icon layui-icon-add-1 layuiadmin-button-btn"></i>申請</button>
                    <button type="button" id="btnBatchAgree" runat="server" class="layui-btn" data-type="btnBatchApproval" micro-data="BatchAgree" lay-tips="一括承認" visible="false"><i class="layui-icon ws-icon icon-piliangqueren layuiadmin-button-btn"></i>批量同意</button>
                    <button type="button" id="btnBatchReturn" runat="server" class="layui-btn layui-btn-danger" data-type="btnBatchApproval" micro-data="BatchReturn" lay-tips="一括却下" visible="false"><i class="layui-icon ws-icon icon-piliangbufu layuiadmin-button-btn"></i>批量驳回</button>
                </div>
                
                <div class="layui-inline">
                    <%--<label class="layui-form-label">加班日期</label>--%>
                    <div class="layui-input-inline" style="width: 120px;">
                        <select id="selQueryItem" name="selQueryItem" lay-filter="selQueryItem">
                            <%=QueryItem() %>
                        </select>
                    </div>
                    <div class="layui-input-inline" style="width: 120px;">
                        <input type="text" id="txtStartDate" name="txtStartDate" runat="server" placeholder="开始日期" autocomplete="off" class="layui-input" readonly="readonly">
                        <i id="icondatetxtStartDate" class="layui-icon layui-icon-date" style="position: absolute;right: 8px;"></i>
                    </div>
                    <div class="layui-form-mid">~</div>
                    <div class="layui-input-inline" style="width: 120px;">
                        <input type="text" id="txtEndDate" name="txtEndDate" runat="server" placeholder="结束日期" autocomplete="off" class="layui-input" readonly="readonly">
                        <i id="icondatetxtEndDate" class="layui-icon layui-icon-date" style="position: absolute;right: 8px;"></i>
                    </div>

                    <div class="layui-input-inline" style="width:280px;">
                        <input type="text" name="txtKeyword" id="txtKeyword" runat="server" placeholder="Keyword" autocomplete="off" class="layui-input">
                        <i id="iconDescription" runat="server" visible="false" class="layui-icon layui-icon-about" style="right:5px;"></i>
                    </div>
                </div>

                <div class="layui-inline">
                    <button type="button" id="btnSearch" class="layui-btn" data-type="btnSearch">
                        <i class="layui-icon layui-icon-search layuiadmin-button-btn"></i>
                    </button>
                </div>

                <div class="layui-inline layui-hide">
                    <button type="button" id="btnDel" runat="server" class="layui-btn layui-btn-danger" data-type="btnDel">删除</button>
                </div>


            </div>
        </div>

        <div class="layui-card-body">
            <div style="padding-bottom: 10px;">
                <table id="tabTable" lay-filter="tabTable"></table>
            </div>
        </div>

    </div>
    <div id="divScript" runat="server"></div>

    <script type="text/html" id="barFormNumber">
        <a href="javascript:;" class="layui-table-link" lay-event="View">{{ d.FormNumber }}</a>

<%--        {{#  if(d.ParentID==undefined){ }}
            <a href="javascript:;" class="layui-table-link" lay-event="View">{{ d.FormNumber }}</a>
        {{#   } else { }}
        {{#  if(d.ParentID=="0"){ }}
         <a href="javascript:;" class="layui-table-link" lay-event="View">{{ d.FormNumber }}</a>
        {{#   } else { }}
        <span>{{ d.FormNumber }}</span>
        {{#  } }}
        {{#  } }}--%>
    </script>

    <input id="txtAction" type="hidden" runat="server" />
    <input id="txtMID" type="hidden" runat="server" />
    <input id="txtFormID" type="hidden" runat="server" />
    <input id="txtShortTableName" type="hidden" runat="server" />
    <input id="txtPrimaryKeyName" type="hidden" runat="server" />
    <input id="txtPrimaryKeyValue" type="hidden" runat="server" />
    <input id="txtFormNumber" type="hidden" runat="server" />
    <input id="txtLinkAddress" type="hidden" runat="server" value="/Views/Forms/MicroForm" />
    <input id="txtDataURL" type="hidden" runat="server"/>
    <input id="txtDataType" type="hidden" runat="server"/>
<%--    <input id="hidStartDate" type="hidden" runat="server" value="0" />
    <input id="hidEndDate" type="hidden" runat="server" value="0" />--%>
    <script src="<%: ResolveUrl("~/Views/Forms/Js/MicroFormList.js")%>"></script>
</asp:Content>

