<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="General.aspx.cs" Inherits="Views_Stats_General"%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
    <div class="layui-row layui-col-space15">
        <%--加班餐--%>
        <div class="layui-col-md12">
            <div class="layui-card">
                <div class="layui-card-header">就餐汇总 / <span class="ws-font-size8">食事汇总</span></div>
                <div class="layui-card-body">
                    <div class="layui-form">
                        <div class="layui-form-item">
                        
                            <div class="layui-inline">
                                <label class="layui-form-label">加班地点</label>
                                <div class="layui-input-inline" style="width: 120px;">
                                    <input type="text" id="txtLocation" name="txtLocation" runat="server" placeholder="加班地点" autocomplete="off" class="layui-input" value="Test">
                                </div>
                                <div class="layui-form-mid">日期</div>
                                <div class="layui-input-inline" style="width: 120px;">
                                    <input type="text" id="txtStartDate" name="txtStartDate" runat="server" placeholder="开始日期" autocomplete="off" class="layui-input" readonly="readonly">
                                    <i id="icondatetxtStartDate" class="layui-icon layui-icon-date" style="position: absolute;right: 8px; line-height: initial; top: 50%; margin-top: -7px;"></i>
                                </div>
                                <div class="layui-form-mid">~</div>
                                <div class="layui-input-inline" style="width: 120px;">
                                    <input type="text" id="txtEndDate" name="txtEndDate" runat="server" placeholder="结束日期" autocomplete="off" class="layui-input" readonly="readonly">
                                    <i id="icondatetxtEndDate" class="layui-icon layui-icon-date" style="position: absolute;right: 8px; line-height: initial; top: 50%; margin-top: -7px;"></i>
                                </div>

                            </div>

                            <div class="layui-inline">
                                <button type="button" id="btnSearch" class="layui-btn" data-type="btnSearch">
                                    <i class="layui-icon layui-icon-search layuiadmin-button-btn"></i>
                                </button>
                            </div>

                        </div>
                    </div>

                    <div class="layui-carousel layadmin-carousel layadmin-backlog" >
                        <div carousel-item >                         
                            <ul class="layui-row layui-col-space10">
                                <li class="layui-col-xs6"><a id="aMeal0" runat="server" lay-text="就餐统计：早餐" class="layadmin-backlog-body" ><h3 lay-tips="早餐 = 加班早餐 + GZ 3S">早餐</h3><p><cite id="citeMeal0"><%=Meal0 %></cite></p></a></li>
                                <li class="layui-col-xs6"><a id="aMeal1" runat="server" lay-text="就餐统计：午餐" class="layadmin-backlog-body" ><h3 lay-tips="午餐 = 加班午餐 + GZ 1S">午餐</h3><p><cite id="citeMeal1"><%=Meal1 %></cite></p></a></li>
                                <li class="layui-col-xs4"><a id="aMeal2" runat="server" lay-text="就餐统计：晚餐" class="layadmin-backlog-body" ><h3 lay-tips="晩餐 = 加班晚餐 + GZ 2S">晚餐</h3><p><cite id="citeMeal2"><%=Meal2 %></cite></p></a></li>
                                <li class="layui-col-xs4"><a id="aMeal3" runat="server" lay-text="就餐统计：宵夜" class="layadmin-backlog-body" ><h3 lay-tips="宵夜 = 加班宵夜 + GZ 2S + GZ 3S">宵夜</h3><p><cite id="citeMeal3"><%=Meal3 %></cite></p></a></li>
                                <li class="layui-col-xs4"><a id="aMeal4" runat="server" lay-text="就餐统计：无" class="layadmin-backlog-body" ><h3 lay-tips="无餐">无</h3><p><cite id="citeMeal4"><%=Meal4 %></cite></p></a></li>
                            </ul>
                        </div>
                    </div>

                </div>
            </div>
        </div>

        <div class="layui-col-md4">
            <div class="layui-card">
                <div class="layui-card-header">加班就餐 / <span class="ws-font-size8">殘業食事</span></div>
                <div class="layui-card-body">

                    <div class="layui-carousel layadmin-carousel layadmin-backlog" >
                        <div carousel-item >                         
                            <ul class="layui-row layui-col-space10">
                                <%=GetOvertimeMeal() %>
                            </ul>
                        </div>
                    </div>

                </div>
            </div>
        </div>

        <div class="layui-col-md8">
            <div class="layui-card">
                <div class="layui-card-header">班次 / <span class="ws-font-size8">排班</span></div>
                <div class="layui-card-body">

                    <div class="layui-carousel layadmin-carousel layadmin-backlog" >
                        <div carousel-item >
                            <ul class="layui-row layui-col-space10">
                                <%=GetOnDuty() %>
                            </ul>
                        </div>
                    </div>

                </div>
            </div>
        </div>

    </div>


    <input name="txtAction" type="hidden" id="txtAction" runat="server"/>
    <input name="txtMID" type="hidden" id="txtMID" runat="server"/>
    <input name="txtShortTableName" type="hidden" id="txtShortTableName" runat="server"/>
    <input name="txtFormID" type="hidden" id="txtFormID" runat="server"/>
    <input name="txtFormsID" type="hidden" id="txtFormsID" runat="server" value=""/>
   
    <script src="<%: ResolveUrl("~/Views/Stats/Js/General.js")%>"></script>
</asp:Content>

