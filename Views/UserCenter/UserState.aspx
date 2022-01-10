<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="UserState.aspx.cs" Inherits="Views_UserCenter_UserState" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="<%: ResolveUrl("~/Resource/CSS/micropopup.css")%>" rel="stylesheet" media="all" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server" ClientIDMode="Static">
    <div class="layui-card">
        <div class="layui-card-body layui-form">
            <div class="layui-row">
                <div class="layui-form-item">
                    <div class="layui-inline ">
                        <label class="layui-form-label ">状态</label>
                        <div class="layui-input-inline ws-width100" >
                            <select name="selStateName" lay-verify="required" lay-filter="selStateName">
                                <option value="100">有空</option>
                                <option value="101">会议中</option>
                                <option value="102">外出了</option>
                                <option value="200" selected="selected">自定义</option>
                            </select>
                        </div>
                        <div id="divCustomStateName" class="layui-input-inline ws-width100">
                            <input type="text" id="txtCustomStateName" name="txtCustomStateName" placeholder="如：忙碌" lay-verify="required|txtCustomStateName" lay-reqtext="自定义时状态不能为空<br/>This cannot be empty" autocomplete="off" class="layui-input">
                        </div>
                        <div class="layui-form-mid layui-word-aux"></div>
                    </div>
                </div>
                <div class="layui-form-item">
                    <div class="layui-inline ">
                        <label class="layui-form-label">重置状态</label>
                        <div class="layui-input-inline ws-width100" >
                            <select name="selDurationTime" lay-verify="required"  lay-filter="selDurationTime">
                                <option value="30">30分钟</option>
                                <option value="1">1小时</option>
                                <option value="2">2小时</option>
                                <option value="3">3小时</option>
                                <option value="4">4小时</option>
                                <option value="ToDay" selected="selected">今天</option>
                                <option value="Custom">自定义</option>
                            </select>
                        </div>
                        <div class="layui-form-mid layui-word-aux">在此时间之后重置状态</div>
                    </div>
                </div>
                <div id="divCustomDateTime" class="layui-form-item layui-hide">
                    <div class="layui-inline ">
                        <label class="layui-form-label">自定义时间</label>
                        <div class="layui-input-inline ws-width100 ">
                            <input type="text" id="txtDate" name="txtDate" runat="server" placeholder="yyyy-MM-dd"  autocomplete="off" class="layui-input" readonly="readonly">
                        </div>
                        <div class="layui-input-inline ws-width80 ">
                            <input type="text" id="txtTime" name="txtTime" runat="server" placeholder="HH:mm" autocomplete="off" class="layui-input" readonly="readonly">
                        </div>
                        <div class="layui-form-mid layui-word-aux">请选择自定义时间</div>
                    </div>
                </div>


                <div class="layui-form-item layui-row ">
                    <div class="layui-inline ">
                        <div class="layui-form-mid layui-word-aux"></div>
                    </div>
                </div>
                <div class="layui-form-item layui-hide ws-margin-top10">
                    <div class="layui-input-block">
                        <button type="button" id="btnSave" class="layui-btn" lay-submit lay-filter="btnSave">保存</button>
                        <%--<button type="reset" class="layui-btn layui-btn-primary">立即重置</button>--%>
                    </div>
                </div>
            </div>
        </div>

    </div>
    <input id="txtMID" type="hidden" runat="server" />
    <script src="<%=ResolveUrl("~/Views/UserCenter/Js/UserState.js")%>"></script>
</asp:Content>

