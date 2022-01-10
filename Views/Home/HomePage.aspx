<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="HomePage.aspx.cs" Inherits="Views_Home_HomePage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
<div class="layui-row layui-col-space15">
      
      <div class="layui-col-sm6 layui-col-md3">
        <div class="layui-card">
          <div class="layui-card-header">
            日申请量
            <span id="spanDay" class="layui-badge layui-bg-green layuiadmin-badge ws-day" micro-data="Qty">日</span>
          </div>
          <div class="layui-card-body layuiadmin-card-list">
            <p id="pDayQty" class="layuiadmin-big-font">0</p>
            <p>
              较昨日增长 
              <span id="spanDiffDayQty" class="layuiadmin-span-color">0</span>
            </p>
          </div>
        </div>
      </div>
      <div class="layui-col-sm6 layui-col-md3">
        <div class="layui-card">
          <div class="layui-card-header">
            周申请量
            <span id="spanWeek" class="layui-badge layui-bg-blue layuiadmin-badge">周</span>
          </div>
          <div class="layui-card-body layuiadmin-card-list">
            <p id="pWeekQty" class="layuiadmin-big-font">0</p>
            <p>
              较上周增长 
              <span id="spanDiffWeekQty" class="layuiadmin-span-color">0</span>
            </p>
          </div>
        </div>
      </div>
      <div class="layui-col-sm6 layui-col-md3">
        <div class="layui-card">
          <div class="layui-card-header">
            月申请量
            <span id="spanMonth" class="layui-badge layui-bg-orange layuiadmin-badge">月</span>
          </div>
          <div class="layui-card-body layuiadmin-card-list">

            <p id="pMonthQty" class="layuiadmin-big-font">0</p>
            <p>
              较上月增长
              <span id="spanDiffMonthQty" class="layuiadmin-span-color">0</span>
            </p>
          </div>
        </div>
      </div>
      <div class="layui-col-sm6 layui-col-md3">
        <div class="layui-card">
          <div class="layui-card-header">
            年申请量
            <span id="spanYear" class="layui-badge layui-bg-cyan layuiadmin-badge">年</span>
          </div>
          <div class="layui-card-body layuiadmin-card-list">

            <p id="pYearQty" class="layuiadmin-big-font">0</p>
            <p>
              较去年增长 
              <span id="spanDiffYearQty" class="layuiadmin-span-color">0</span>
            </p>
          </div>
        </div>
      </div>   
      <div class="layui-col-sm12">
        <div class="layui-card">
          <div class="layui-card-header">
            申请量
            <div class="layui-btn-group layuiadmin-btn-group">
              <a id="aDay" href="javascript:;" class="layui-btn layui-btn-primary layui-btn-xs ws-day">日</a>
              <a id="aWeek" href="javascript:;" class="layui-btn layui-btn-primary layui-btn-xs">周</a>
              <a id="aMonth" href="javascript:;" class="layui-btn layui-btn-primary layui-btn-xs">月</a>
              <a id="aYear" href="javascript:;" class="layui-btn layui-btn-primary layui-btn-xs">年</a>
            </div>
          </div>
          <div class="layui-card-body" style="height:500px !important;" >
            <div class="layui-row">
              <div class="layui-col-sm8">
                  <div class="layui-carousel layadmin-carousel layadmin-dataview" data-anim="fade" lay-filter="LAY-index-dataview" style="height:500px !important;" >
                    <div carousel-item id="LAY-index-dataview1" style="height:500px !important;" >
                      <div style="height:500px !important;" ><i class="layui-icon layui-icon-loading1 layadmin-loading"></i></div>
                    </div>
                  </div>
              </div>
              <div class="layui-col-sm4">
                  <div class="layui-carousel layadmin-carousel layadmin-dataview" data-anim="fade" lay-filter="LAY-index-2dataview" style="height:500px !important;" >
                    <div carousel-item id="LAY-index-2dataview1" style="height:500px !important;" >
                      <div style="height:500px !important;" ><i class="layui-icon layui-icon-loading1 layadmin-loading"></i></div>
                    </div>
                  </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="layui-col-sm12">
        <div class="layui-card">
          <div class="layui-card-header">
            访问量【当前在线人数：<%=Application["OnlineUser"].ToString() %>】
            <div class="layui-btn-group layuiadmin-btn-group">
              <a id="aDay2" href="javascript:;" class="layui-btn layui-btn-primary layui-btn-xs ws-day">日</a>
              <a id="aWeek2" href="javascript:;" class="layui-btn layui-btn-primary layui-btn-xs">周</a>
              <a id="aMonth2" href="javascript:;" class="layui-btn layui-btn-primary layui-btn-xs">月</a>
              <a id="aYear2" href="javascript:;" class="layui-btn layui-btn-primary layui-btn-xs">年</a>
            </div>
          </div>
          <div class="layui-card-body" style="height:400px !important;" >
            <div class="layui-row">
              <div class="layui-col-sm12">
                  <div class="layui-carousel layadmin-carousel layadmin-dataview" data-anim="fade" lay-filter="LAY-index-3dataview" style="height:400px !important;" >
                    <div carousel-item id="LAY-index-3dataview1" style="height:400px !important;" >
                      <div style="height:400px !important;" ><i class="layui-icon layui-icon-loading1 layadmin-loading"></i></div>
                    </div>
                  </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="layui-col-sm8">
        <div class="layui-card">
          <div class="layui-card-header">
            浏览器
            <div class="layui-btn-group layuiadmin-btn-group">
            </div>
          </div>
          <div class="layui-card-body" style="height:400px !important;" >
            <div class="layui-row">
              <div class="layui-col-sm12">
                  <div class="layui-carousel layadmin-carousel layadmin-dataview" data-anim="fade" lay-filter="LAY-index-4dataview" style="height:400px !important;" >
                    <div carousel-item id="LAY-index-4dataview1" style="height:400px !important;" >
                      <div style="height:400px !important;" ><i class="layui-icon layui-icon-loading1 layadmin-loading"></i></div>
                    </div>
                  </div>
              </div>
            </div>
          </div>
        </div>
      </div>
     
      <div class="layui-col-sm4">
        <div class="layui-card">
          <div class="layui-card-header">
            服务器
            <div class="layui-btn-group layuiadmin-btn-group">
            </div>
          </div>
          <div class="layui-card-body" style="height:400px !important;" >
            <div class="layui-row">
                <%--CPU、内存使用率--%>
              <div id="divSysInfo" class="layui-col-sm12">
                  <%=MicroBIHelper.MicroBI.GetSysInfo() %>
              </div>

            </div>
          </div>
        </div>
      </div>

    </div>
    <script src="<%: ResolveUrl("~/Views/Home/Js/HomePage.js")%>"></script>
</asp:Content>

