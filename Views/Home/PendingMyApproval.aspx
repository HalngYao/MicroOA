<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="PendingMyApproval.aspx.cs" Inherits="Views_home_PendingMyApproval" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link rel="stylesheet" href="<%: ResolveUrl("~/Resource/fullcalendar/lib/main.css")%>" media="all">
    <script src="<%: ResolveUrl("~/Resource/fullcalendar/lib/main.js")%>"></script>
    <script src="<%: ResolveUrl("~/Resource/fullcalendar/lib/locales-all.js")%>"></script>
    <script src="<%: ResolveUrl("~/Scripts/jquery-3.4.1.js")%>"></script>

    <style type="text/css">

        .fc-event-title { font-weight:normal !important;}

        .fc-event-time { display:none;}

        .ws-circle {
            width: 10px;
            height: 10px;
            border-radius: 50%; 
            display: inline-block;
            margin-right: 5px;
        }

        .ws-circle+span {
            margin-right:10px;
            }

        .divCalendar a:hover{
            background-color:none;
        }

    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">

    <div class="layui-row layui-col-space15">
      <%--md8 Start--%>
      <div class="layui-col-md12"> 
        <div class="layui-row layui-col-space15">

            <%=GetPendingMyApproval() %>
            
        </div>
      </div>
           
    </div>
    <input id="txtMID" type="hidden" runat="server" />

    <script src="<%: ResolveUrl("~/Views/Home/Js/PendingMyApproval.js")%>"></script>
</asp:Content>

