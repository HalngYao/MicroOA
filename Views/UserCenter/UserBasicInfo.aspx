<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="UserBasicInfo.aspx.cs" Inherits="Views_UserCenter_UserBasicInfo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script src="<%=ResolveUrl("~/Resource/xmSelect/xmselect.js")%>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server" ClientIDMode="Static">
    <div class="layui-card">

        <div class="layui-card-header ">
            设置我的资料
        </div>
        
        <div class="layui-card-body">
            <div class="layui-row">

                <div class="layui-col-xs12 layui-col-sm12 layui-col-md4 layui-col-lg4" style="padding-left: 10px; overflow: auto !important; min-height: 730px !important; border-right:1px dashed #F0F0F0;">
                    <div class="layui-form-item"> 
                        <div class=" layui-upload ws-height400 layui-bg-gray layui-border-box ws-min-height200 ws-padding40 ws-align-center" style="width:95%;" >   
                            <button type="button" class="layui-btn" id="btnUpload">
                            <i class="layui-icon">&#xe67c;</i>上传头像</button>
                            <p style="font-size:12px; margin-bottom:30px;"></p>
                            <div class="layui-upload-list" style=" width:168px; height:168px; margin:0 auto; ">
                                <asp:Image ID="imgAvatar" runat="server" CssClass="layui-upload-img layui-nav-img" ImageUrl="~/Resource/UploadFiles/Images/Avatar/default.jpg" Width="100%" Height="100%" />
                            <p id="demoText"></p>
                            </div>
                            <p style="font-size:12px; margin-top:40px;">温馨提示：最大不能超过500KB，建议尺寸168*168，支持jpg、png、gif格式</p>
                        </div>
                    </div>
                </div>
                <%--==========================--%>
                <div class="layui-form layui-col-xs12 layui-col-sm12 layui-col-md8 layui-col-lg8" style="padding-left: 20px; padding-bottom: 10px; min-height: 730px;" >
                    <div class="layui-form-item layui-row ">
                        <div class="layui-inline ">
                            <label class="layui-form-label " lay-tips="UserID">UserID</label>
                            <div class="layui-input-inline" style="width:280px;">
                                <input type="text" id="txtUserName" name="txtUserName" value="" placeholder="" lay-verify="required|txtUserNameLength" lay-reqtext="用户名不能为空<br/>This cannot be empty" autocomplete="off" class="layui-input layui-disabled" runat="server" readonly="readonly">
                            </div>
                            <div class="layui-form-mid layui-word-aux">UserID不可修改用于系统登录</div>
                        </div>
                    </div>
                    <div class="layui-form-item layui-row ">
                        <div class="layui-inline ">
                            <label class="layui-form-label " lay-tips="E-Mail Address">邮箱</label>
                            <div class="layui-input-inline " style="width:280px;">
                                <input type="text" id="txtEMail" name="txtEMail" value="" placeholder="" lay-verify="email|txtEMailLength" lay-reqtext="邮箱地址不能为空<br/>This cannot be empty" autocomplete="off" class="layui-input layui-disabled" runat="server" readonly="readonly">
                            </div>
                            <div class="layui-form-mid layui-word-aux">邮箱地址不可修改</div>
                        </div>
                    </div>
                    <div class="layui-form-item layui-row ">
                        <div class="layui-inline ">
                            <label class="layui-form-label " lay-tips="Chinease Name">名字</label>
                            <div class="layui-input-inline " style="width:280px;">
                                <input type="text" id="txtChineseName" name="txtChineseName" value="" placeholder="" lay-verify="txtChineseNameLength" lay-reqtext="中文名字不能为空<br/>This cannot be empty" autocomplete="off" class="layui-input " runat="server">
                            </div>
                        </div>
                        <div class="layui-inline ">
                            <div class="layui-input-inline cssraSex">
                                <input type="radio" id="radioSex0" name="radioSex" title="男" value="男" lay-filter="radioSex"><input type="radio" id="radioSex1" name="radioSex" title="女" value="女" lay-filter="radioSex">
                                <input type="text" id="txtSex" name="txtSex" placeholder="" lay-verify="" lay-reqtext="性别必须选择一个<br/>This item must choose one of them" autocomplete="off" class="layui-input layui-hide txtSex" value="" runat="server"></div>
                            <div class="layui-form-mid layui-word-aux"></div>
                        </div>
                    </div>
                    <div class="layui-form-item layui-row ">
                        <div class="layui-inline ">
                            <label class="layui-form-label " lay-tips="English Name">英文名</label>
                            <div class="layui-input-inline " style="width:280px;">
                                <input type="text" id="txtEnglishName" name="txtEnglishName" value="" placeholder="" lay-verify="txtEnglishNameLength" lay-reqtext="英文名字不能为空<br/>This cannot be empty" autocomplete="off" class="layui-input " runat="server">
                            </div>
                        </div>
                    </div>
                    <div class="layui-form-item layui-row ">
                        <div class="layui-inline ">
                            <label class="layui-form-label "  lay-tips="Work mobile phone">手机</label>
                            <div class="layui-input-inline " style="width:280px;">
                                <input type="text" id="txtWorkMobilePhone" name="txtWorkMobilePhone" value="" placeholder="" lay-verify="txtWorkMobilePhoneLength" lay-reqtext="工作手机不能为空<br/>This cannot be empty" autocomplete="off" class="layui-input " runat="server">
                            </div>
                            <label class="layui-form-label " style="width:60px!important;" lay-tips="Telephone">分机</label>
                            <div class="layui-input-inline " style="width:80px;">
                                <input type="text" id="txtWorkTel" name="txtWorkTel" value="" placeholder="" lay-verify="txtWorkTelLength" lay-reqtext="分机不能为空<br/>This cannot be empty" autocomplete="off" class="layui-input " runat="server">
                            </div>
                        </div>
                    </div>
                    <div class="layui-form-item layui-row ">
                        <div class="layui-inline ">
                            <label class="layui-form-label " lay-tips="Department">部门</label>
                            <div class="layui-input-inline " style="width:280px;">
                                <div id="microXmSelect"></div>
                                <input id="UserDepts" type="hidden" runat="server" />
                            </div>
                        </div>
                    </div>
                    <div class="layui-form-item layui-row ">
                        <div class="layui-inline ">
                            <label class="layui-form-label " lay-tips="Receive global message">通知</label>
                            <div class="layui-input-block">
                                <input type="checkbox" id="ckIsSiteTips" name="ckIsSiteTips" runat="server" title="站点" lay-filter="ckIsSiteTips">
                                <input type="checkbox" id="ckIsMailTips" name="ckIsMailTips" runat="server" title="邮件" lay-filter="ckIsMailTips">
                                <input type="checkbox" id="ckIsGlobalTips" name="ckIsGlobalTips" runat="server" title="全局消息" lay-filter="ckIsGlobalTips" >
                            </div>
                            <div class="layui-form-mid layui-word-aux" style="left:120px;">注：全局消息状态关闭时，若有新的消息需要全局推送将由管理员自动开启</div>
                        </div> 
                    </div>
                    <div class="layui-form-item ws-margin-top10">
                        <div class="layui-input-block">
                            <button type="button" id="btnModify" class="layui-btn layui-btn-normal" lay-submit lay-filter="btnModify">修改</button>
                            <%--<button type="reset" class="layui-btn layui-btn-primary">立即重置</button></div>--%>
                    </div>
                </div>
                <%--=====================--%>
            </div>
        </div>

    </div>

    </div>
    <input id="txtMID" type="hidden" runat="server" />
    <input id="txtUID" type="hidden" runat="server" />
    <input id="txtMaxFileUpload" type="hidden" runat="server" />
    <input id="txtUploadFileType" type="hidden" runat="server" />
    <script src="<%=ResolveUrl("~/Views/UserCenter/Js/UserBasicInfo.js?version=1")%>"></script>
</asp:Content>

