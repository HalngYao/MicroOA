<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="Console.aspx.cs" Inherits="Views_home_Console" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link rel="stylesheet" href="<%: ResolveUrl("~/Resource/fullcalendar/lib/main.css")%>" media="all">
    <script src="<%: ResolveUrl("~/Resource/fullcalendar/lib/main.js")%>"></script>
    <script src="<%: ResolveUrl("~/Resource/fullcalendar/lib/locales-all.js")%>"></script>
    <script src="<%: ResolveUrl("~/Scripts/jquery-3.4.1.js")%>"></script>
    <style type="text/css">

        /*计划*/
        .ws-cal-event-plan,.ws-cal-event-plan:hover {
        font-size:12px;
        width: 98%;
        border:none;
        top:1px !important;
        left:1px !important;
        }
        .ws-cal-event-plan .fc-daygrid-event-dot{ border-color:<%:CalendarPlanColor%> !important;}

        /*计划 Short*/
        .ws-cal-event-plan2,.ws-cal-normal-plan2:hover {
        text-align:center;
        background-color:<%:CalendarPlanColor%>  !important;
        font-size:12px;
        width: 50%;
        border:none;
        color:#ffffff;
        }
        .ws-cal-event-plan2 .fc-daygrid-event-dot{ display:none;}

        /*实际（正常）*/
        .ws-cal-event-normal,.ws-cal-event-normal:hover {
        text-align:center;
        background-color:<%:CalendarActualColor%> !important;
        font-size:12px;
        width: 98%;
        border:none;
        color:#ffffff;
        }
        .ws-cal-event-normal .fc-daygrid-event-dot{ display:none;}

        /*实际（正常） Short*/
        .ws-cal-event-normal2,.ws-cal-normal2:hover {
        text-align:center;
        background-color:<%:CalendarActualColor%> !important;
        font-size:12px;
        width: 50%;
        border:none;
        color:#ffffff;
        }
        .ws-cal-event-normal2 .fc-daygrid-event-dot{ display:none;}

        /*加班*/
        .ws-cal-event-overtime,.ws-cal-event-overtime:hover {
        text-align:center;
        background-color:<%:CalendarOvertimeColor%>  !important;
        font-size:12px;
        width: 98%;
        border:none;
        color:#ffffff;
        }
        .ws-cal-event-overtime .fc-daygrid-event-dot{ display:none;}

        .ws-cal-event-overtime2,.ws-cal-event-overtime2:hover {
        text-align:center;
        background-color:<%:CalendarOvertimeColor%>  !important;
        font-size:12px;
        width: 50%;
        border:none;
        color:#ffffff;
        }
        .ws-cal-event-overtime2 .fc-daygrid-event-dot{ display:none;}

        /*出差*/
        .ws-cal-event-businesstrip,.ws-cal-event-businesstrip:hover {
        text-align:center;
        background-color:<%:CalendarBusinessTripColor%> !important;
        font-size:12px;
        width: 98%;
        border:none;
        color:#ffffff;
        }
        .ws-cal-event-businesstrip .fc-daygrid-event-dot{ display:none;}

        .ws-cal-event-businesstrip2,.ws-cal-event-businesstrip2:hover {
        text-align:center;
        background-color:<%:CalendarBusinessTripColor%> !important;
        font-size:12px;
        width: 50%;
        border:none;
        color:#ffffff;
        }
        .ws-cal-event-businesstrip2 .fc-daygrid-event-dot{ display:none;}

        /*休假*/
        .ws-cal-event-holiday,.ws-cal-event-holiday:hover {
        text-align:center;
        background-color:<%:CalendarHolidayColor%> !important;
        font-size:12px;
        width: 98%;
        border:none;
        color:#ffffff;
        }
        .ws-cal-event-holiday .fc-daygrid-event-dot{ display:none;}

        .ws-cal-event-holiday2,.ws-cal-event-holiday2:hover {
        text-align:center;
        background-color:<%:CalendarHolidayColor%> !important;
        font-size:12px;
        width: 50%;
        border:none;
        color:#ffffff;
        }
        .ws-cal-event-holiday2 .fc-daygrid-event-dot{ display:none;}
    
        /*异常 （迟到、早退）  #e60021*/
        .ws-cal-event-abnormal,.ws-cal-event-abnormal:hover {
        text-align:center;
        background-color:<%:CalendarAbnormalColor%> !important; 
        font-size:12px;
        width: 98%;
        border:none;
        color:#ffffff;
        }
        .ws-cal-event-abnormal .fc-daygrid-event-dot{ display:none;}

        .ws-cal-event-abnormal2,.ws-cal-event-abnormal2:hover {
        text-align:center;
        background-color:<%:CalendarAbnormalColor%> !important;
        font-size:12px;
        width: 50%;
        border:none;
        color:#ffffff;
        }
        .ws-cal-cal-event-abnormal2 .fc-daygrid-event-dot{ display:none;}

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

        /*日历图例颜色*/
        /*蓝色 计划*/
        .ws-cal-legend-plan{ background-color: <%:CalendarPlanColor%> !important;}
        /*绿色 实际*/
        .ws-cal-legend-actual{ background-color: <%:CalendarActualColor%> !important;}
        /*浅蓝色 加班*/
        .ws-cal-legend-overtime{ background-color: <%:CalendarOvertimeColor%> !important;}
        /*紫色 出差*/
        .ws-cal-legend-businesstrip { background-color: <%:CalendarBusinessTripColor%> !important; }
        /*灰色 休假*/
        .ws-cal-legend-holiday { background-color: <%:CalendarHolidayColor%> !important;}
        /*红色 异常*/
        .ws-cal-legend-abnormal { background-color: <%:CalendarAbnormalColor%> !important;}

    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">

    <div class="layui-row layui-col-space15">
      <%--md8 Start--%>
      <div class="layui-col-md8"> 
        <div class="layui-row layui-col-space15">
          <div class="layui-col-md6">
            <div class="layui-card">
              <div class="layui-card-header layui-elip">全局</div>
              <div class="layui-card-body">

                <div class="layui-carousel layadmin-carousel layadmin-backlog">
                  <div carousel-item>

                    <ul class="layui-row layui-col-space10">
                      <li class="layui-col-xs12" style="height:99%; text-align:center; vertical-align:bottom;">
                        <a class="layadmin-backlog-body" style="height:100%;">
                          <h3 class="layui-elip">安全运行（Day）</h3>
                          <p style="margin-top:50px;"><cite style="font-size:900%;"><%: DayQty %></cite></p>
                        </a>
                      </li>
<%--                      <li class="layui-col-xs6">
                        <a href="javascript:;" class="layadmin-backlog-body">
                          <h3>待定</h3>
                          <p><cite><%: WeekQty %></cite></p>
                        </a>
                      </li>
                      <li class="layui-col-xs6">
                        <a href="javascript:;" class="layadmin-backlog-body">
                          <h3>待定</h3>
                          <p><cite><%: MonthQty %></cite></p>
                        </a>
                      </li>
                      <li class="layui-col-xs6">
                        <a href="javascript:;" class="layadmin-backlog-body">
                          <h3>RunTime</h3>
                          <p><cite><%: RunTime %></cite></p>
                        </a>
                      </li>--%>
                    </ul>

                  </div>
                </div>
              </div>
            </div>
          </div>

          <%--任务--%>
          <div class="layui-col-md6">
            <div class="layui-card">
              <div class="layui-card-header layui-elip">任务</div>
              <div class="layui-card-body">

                <div class="layui-carousel layadmin-carousel layadmin-backlog">
                  <div carousel-item>

                    <ul class="layui-row layui-col-space10">
                      <li class="layui-col-xs6">
                        <a id="aPendingMyApproval" runat="server" lay-text="待我审批【任务】" class="layadmin-backlog-body ws-font-red micro-click">
                          <h3 class="layui-elip">待我审批</h3>
                          <p class="layui-elip"><cite class="ws-font-red"><%: PendingMyApproval %></cite></p>
                        </a>
                      </li>
                      <li class="layui-col-xs6">
                        <a name="aPendingMyAccept" id="aPendingMyAccept" runat="server" lay-text="待我处理【任务】" class="layadmin-backlog-body ws-font-orange">
                          <h3 class="layui-elip">待我处理</h3>
                          <p class="layui-elip"><cite class="ws-font-orange"><%: PendingMyAccept %></cite></p>
                        </a>
                      </li>
                      <li class="layui-col-xs6">
                        <a id="aAccepting" runat="server" lay-text="处理中【任务】" class="layadmin-backlog-body ws-font-blue">
                          <h3 class="layui-elip">处理中</h3>
                          <p class="layui-elip"><cite class="ws-font-blue"><%: Accepting %></cite></p>
                        </a>
                      </li>
                      <li class="layui-col-xs6">
                        <a id="aFinish" runat="server" lay-text="完成【任务】" class="layadmin-backlog-body ws-font-green">
                          <h3 class="layui-elip">完成</h3>
                          <p class="layui-elip"><cite><%: Finish %></cite></p>
                        </a>
                      </li>
                    </ul>

                  </div>
                </div>
              </div>
            </div>
          </div>

          <%--日历--%>
          <div class="layui-col-md12">
            <div class="layui-card layui-form">
              <div class="layui-card-header">
                    日历
                    <div class="layui-inline" style=" position:absolute; left:150px; ">
                        <div id="divPlanLegend" runat="server" visible="false" class="ws-circle ws-cal-legend-plan"></div><span id="spanPlanLegend"  runat="server" visible="false" lay-tips="计划排班事件">班次</span>
                        <div id="divActualLegend" runat="server" visible="false" class="ws-circle ws-cal-legend-actual"></div><span id="spanActualLegend" runat="server" visible="false" lay-tips="考勤打卡事件">考勤</span>
                        <div id="divOvertimeLegend" runat="server" visible="false" class="ws-circle ws-cal-legend-overtime"></div><span id="spanOvertimeLegend" runat="server" visible="false" lay-tips="加班事件">加班</span>
                        <div id="divBusinessTripLegend" runat="server" visible="false" class="ws-circle ws-cal-legend-businesstrip"></div><span id="spanBusinessTripLegend" runat="server" visible="false" lay-tips="出差事件">出差</span>
                        <div id="divHolidayLegend" runat="server" visible="false" class="ws-circle ws-cal-legend-holiday"></div><span id="spanHolidayLegend" runat="server" visible="false" lay-tips="休假事件">休假</span>
                        <div id="divAbnormalLegend" runat="server" visible="false" class="ws-circle ws-cal-legend-abnormal"></div><span id="spanAbnormalLegend" runat="server" visible="false" lay-tips="异常事件">异常</span>
                    </div>
                    <div class="layui-inline microCtrl" style=" position:absolute; right:5px; ">
                        <input type="checkbox" id="ckLoadKQ" name="ckLoadKQ" lay-skin="primary"  micro-type="GetLoadKQ" lay-filter="ckMicroCheckBox" title="载入考勤" disabled="disabled" runat="server" visible="false">
                        <input type="checkbox" id="ckSetCalendarDays" name="ckSetCalendarDays" lay-skin="primary" micro-type="CtrlCalendarDays" lay-filter="ckMicroCheckBox"  title="设置休日" runat="server" visible="false">
                        <input type="checkbox" id="ckSetStatutoryDays" name="ckSetStatutoryDays" lay-skin="primary" micro-type="CtrlStatutoryDays" lay-filter="ckMicroCheckBox"  title="设置法定" runat="server" visible="false"> 
                    </div>
              </div>
              <div class="layui-card-body">
                <div id="divCalendar"></div>            
              </div>
            </div>
          </div>
           
        </div>
      </div>
      
      <%--md4 Start--%>
      <div class="layui-col-md4">

            <%--我的--%>
            <div class="layui-card">
                <div class="layui-card-header layui-elip">我的</div>
                <div class="layui-card-body">
                    <div class="layui-carousel layadmin-carousel layadmin-backlog">
                        <div carousel-item>
                            <ul class="layui-row layui-col-space10">
                                <li class="layui-col-xs4">
                                <a class="layadmin-backlog-body">
                                    <h3 class="layui-elip">完成 / 申请</h3>
                                    <p class="layui-elip">
                                        <cite id="aMyApplyFinish" runat="server" lay-text="我的申请完成"><%: MyApplyFinish %></cite>
                                        <cite>/</cite>
                                        <cite id="aMyApply" runat="server" lay-text="我的申请"><%: MyApply %></cite>
                                    </p>
                                </a>
                                </li>
                                <li class="layui-col-xs4">
                                <a id="aMyApplyWaitApproval" runat="server" lay-text="等待审批【我的申请】" class="layadmin-backlog-body">
                                    <h3 class="layui-elip">等待审批</h3>
                                    <p class="layui-elip"><cite><%: MyApplyWaitApproval %></cite></p>
                                </a>
                                </li>
                                <li class="layui-col-xs4">
                                <a id="aMyApplyWaitAccept" runat="server" lay-text="等待处理【我的申请】" class="layadmin-backlog-body">
                                    <h3 class="layui-elip">等待处理</h3>
                                    <p class="layui-elip"><cite><%: MyApplyWaitAccept %></cite></p>
                                </a>
                                </li>
                                <li class="layui-col-xs4">
                                <a id="aMyApplyAccepting" runat="server" lay-text="处理中【我的申请】" class="layadmin-backlog-body">
                                    <h3 class="layui-elip">处理中</h3>
                                    <p class="layui-elip"><cite><%: MyApplyAccepting %></cite></p>
                                </a>
                                </li>
                                <li class="layui-col-xs4">
                                <a id="aMyApplyReject" runat="server" lay-text="驳回【我的申请】" class="layadmin-backlog-body">
                                    <h3 class="layui-elip">驳回</h3>
                                    <p class="layui-elip"><cite><%: MyApplyReject %></cite></p>
                                </a>
                                </li>
                                <li class="layui-col-xs4">
                                <a id="aMyApplyWithdrawal" runat="server" lay-text="撤回【我的申请】" class="layadmin-backlog-body">
                                    <h3 class="layui-elip">撤回</h3>
                                    <p class="layui-elip"><cite><%: MyApplyWithdrawal %></cite></p>
                                </a>
                                </li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>

            <%--个人考勤数据--%>
            <div class="layui-card">
              <div class="layui-card-header layui-elip">加班数据（个人）</span></div>
              <div class="layui-card-body">

                <div class="layui-carousel layadmin-carousel layadmin-backlog">
                  <div carousel-item>
                    <ul class="layui-row layui-col-space10">
                        <li class="layui-col-xs3">
                            <a id="aOvertimeTotalByMonth" lay-text="当月累计加班明细" class="layadmin-backlog-body">
                                <h3 id="hOvertimeTotalByMonth" class="layui-elip" lay-tips="当月累计加班总时间">当月累计 (H)</h3>
                                <p class="layui-elip"><cite id="citeOvertimeTotalByMonth">0</cite></p>
                            </a>
                        </li>
                        <li class="layui-col-xs3">
                            <a id="aExceptDaiXiuByMonth" lay-text="当月累计加班明细" class="layadmin-backlog-body">
                                <h3 id="hExceptDaiXiuByMonth" class="layui-elip" lay-tips="当月剔除代休后累计加班总时间">当月剔除 (H)</h3>
                                <p class="layui-elip"><cite id="citeExceptDaiXiuByMonth">0</cite></p>
                            </a>
                        </li>
                        <li class="layui-col-xs3">
                            <a id="aOvertimeTotalByQuarter" lay-text="季度累计加班明细" class="layadmin-backlog-body">
                                <h3 id="hOvertimeTotalByQuarter" class="layui-elip" lay-tips="本季度累计加班总时间">季度累计 (H)</h3>
                                <p class="layui-elip"><cite id="citeOvertimeTotalByQuarter">0</cite></p>
                            </a>
                        </li>
                        <li class="layui-col-xs3">
                            <a id="aExceptDaiXiuByQuarter" lay-text="季度累计加班明细" class="layadmin-backlog-body">
                                <h3 id="hExceptDaiXiuByQuarter" class="layui-elip" lay-tips="本季度剔除代休后累计加班总时间">季度剔除 (H)</h3>
                                <p class="layui-elip"><cite id="citeExceptDaiXiuByQuarter">0</cite></p>
                            </a>
                        </li>
                        <li class="layui-col-xs4">
                            <a id="aWorkOvertime" lay-text="当月延时加班明细" class="layadmin-backlog-body">
                                <h3 id="hWorkOvertime" class="layui-elip" lay-tips="当月延时加班总时间">当月延时 (H)</h3>
                                <p class="layui-elip"><cite id="citeWorkOvertime">0</cite></p>
                            </a>
                        </li>
                        <li class="layui-col-xs4">
                            <a id="aOffDayOvertime" lay-text="当月休出加班明细" class="layadmin-backlog-body">
                                <h3 id="hOffDayOvertime" class="layui-elip" lay-tips="当月休息日出勤总时间">当月休出 (H)</h3>
                                <p class="layui-elip"><cite id="citeOffDayOvertime">0</cite></p>
                            </a>
                        </li>
                        <li class="layui-col-xs4">
                            <a id="aStatutory" lay-text="当月法定加班明细" class="layadmin-backlog-body">
                                <h3 id="hStatutory" class="layui-elip" lay-tips="当月法定加班总时间">当月法定 (H)</h3>
                                <p class="layui-elip"><cite id="citeStatutory">0</cite></p>
                            </a>
                        </li>

                    </ul>
                  </div>
                </div>

              </div>
            </div>

            <%--个人假期数据--%>
            <div class="layui-card">
              <div class="layui-card-header layui-elip">假期数据（个人）</div>
              <div class="layui-card-body">

                <div class="layui-carousel layadmin-carousel layadmin-backlog">
                  <div carousel-item>
                    <ul class="layui-row layui-col-space10">
                        <li class="layui-col-xs6">
                            <a id="aAlreadyDaiXiuOvertime" lay-text="已代休 (Day) " class="layadmin-backlog-body">
                                <h3 id="hAlreadyDaiXiuOvertime" class="layui-elip" lay-tips="已提交代休申请且获得承认的">当月代休 (Day)</h3>
                                <p><cite id="citeAlreadyDaiXiuOvertime">0</cite></p>
                            </a>
                        </li>
                        <li class="layui-col-xs6">
                            <a id="aLeaveTotal" lay-text="休假" class="layadmin-backlog-body">
                                <h3 id="hLeaveTotal" class="layui-elip" lay-tips="如年休、代休等">当月休假 (Day)</h3>
                                <p><cite id="citeLeaveTotal">0</cite></p>
                            </a>
                        </li>
                        <li class="layui-col-xs4">
                            <a id="aRemainingDaiXiu" lay-text="剩余休出 (H)" class="layadmin-backlog-body">
                                <h3 id="hRemainingDaiXiu" class="layui-elip" lay-tips="剔除代休后剩余的休息日出勤总时间">当月剩余休出 (H)</h3>
                                <p><cite id="citeRemainingDaiXiu">0</cite></p>
                            </a>
                        </li>
                        <li class="layui-col-xs4">
                            <a id="aRemainingAnnualLeave" lay-text="剩余年休 (Day)" class="layadmin-backlog-body">
                                <h3 id="hRemainingAnnualLeave" class="layui-elip">剩余年休 (Day)</h3>
                                <p><cite id="citeRemainingAnnualLeave">0</cite></p>
                            </a>
                        </li>
                        <li class="layui-col-xs4">
                            <a id="aRemainingOtherLeave" lay-text="剩余其它假期 (Day)" class="layadmin-backlog-body">
                                <h3 id="hRemainingOtherLeave" class="layui-elip">剩余其它假期 (Day)</h3>
                                <p><cite id="citeRemainingOtherLeave">0</cite></p>
                            </a>
                        </li>
                    </ul>
                  </div>
                </div>

              </div>
            </div>
      
      </div>
      
    </div>
    <input id="txtMID" type="hidden" runat="server" />
    <input id="txtLocale" type="hidden" runat="server" value="zh-cn" />
    <input id="txtAdminRole" type="hidden" runat="server" value="false" />
    <input id="txtCalendarFirstDay" type="hidden" runat="server" value="0" />
    <input id="txtMonthStartDate" type="hidden" runat="server" value="0" />
    <input id="txtMonthEndDate" type="hidden" runat="server" value="0" />
    <input id="txtQuarterStartDate" type="hidden" runat="server" value="0" />
    <input id="txtQuarterEndDate" type="hidden" runat="server" value="0" />
   
    <script src="<%: ResolveUrl("~/Views/Home/Js/Console.js")%>"></script>
</asp:Content>

