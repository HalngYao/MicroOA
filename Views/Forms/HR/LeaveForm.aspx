<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="LeaveForm.aspx.cs" Inherits="Views_Forms_HR_LeaveForm" %>
<%@ Register Src="~/App_Ctrl/MicroForm.ascx" TagName="MicroForm" TagPrefix="MicroCtrl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="<%: ResolveUrl("~/Resource/CSS/micropopup.css")%>" rel="stylesheet" media="all" />
    <script src="<%: ResolveUrl("~/Resource/xmSelect/xmselect.js")%>"></script>
    <script src="<%: ResolveUrl("~/Resource/WangEditor/wangEditor.min.js")%>"></script>
    <style type="text/css">
        .laydate-time-show .layui-laydate-content > .layui-laydate-list { padding-bottom: 0px; overflow: hidden;}

        .laydate-time-show .layui-laydate-content > .layui-laydate-list > li {width: 50%; }

        .csshiddenlabel label{ display:none !important;}

        #divShowLeave{ color:#FF5722; }

        #divselLeaveDays:after{ content:" 天，";}
        #divselLeaveHour{ right:40px;}
        #divselLeaveHour:after{ content:" 小时";}

        #divhidRemainingNumber { left:-50px;}
        #divhidRemainingNumber:before {content:" 剩余：";}
        #divhidRemainingNumber:after {content:" 天";}

    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
    <MicroCtrl:MicroForm ID="microForm" runat="server" FormCode="vcode,fcode,scode"/>

    <input id="txtAction" name="txtAction" type="hidden" runat="server"/>
    <input id="txtMID" name="txtMID" type="hidden" runat="server" />
    <input id="txtShortTableName" name="txtShortTableName" type="hidden" runat="server"/>
    <input id="txtFormID" name="txtFormID" type="hidden" runat="server" />
    <input id="txtFormsID" name="txtFormsID" type="hidden" runat="server" value=""/>
    <input id="xmByUserDefVal" name="xmByUserDefVal" type="hidden"  runat="server" value=""/>
    <input id="xmByHolidayTypeDefVal" name="xmByHolidayTypeDefVal" type="hidden"  runat="server" value=""/>
    <input id="xmByLaveDaysDefval" name="xmByLaveDaysDefval" type="hidden"  runat="server" value=""/>
    <input id="xmByLaveHourDefval" name="xmByLaveHourDefval" type="hidden"  runat="server" value=""/>
    <input id="txtMinOvertimeDate" name="txtMinOvertimeDate" type="hidden"  runat="server" value=""/>
    <input id="txtMaxOvertimeDate" name="txtMaxOvertimeDate" type="hidden"  runat="server" value=""/>

    <script src="<%=ResolveUrl("~/Views/Forms/HR/Js/" + txtShortTableName.Value + "Form.js")%>"></script>
</asp:Content>

