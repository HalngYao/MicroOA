<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="Flow.aspx.cs" Inherits="Views_Forms_Flow" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script src="<%: ResolveUrl("~/Resource/xmSelect/xmselect.js")%>"></script>
    <link href="<%: ResolveUrl("~/Resource/css/micropopup.css")%>" rel="stylesheet" media="all" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
    <div class="layui-card">
        <%--<div class="layui-card-header ">
            流程设置 WorkFlow Setting
        </div>--%>

        <div class="layui-card-body layui-form">
            <div class="layui-row">
                <div class="layui-form-item">
                    <div class="layui-inline ">
                        <label class="layui-form-label ">表单</label>
                        <div class="layui-input-inline ">
                             <div id="microXmSelect0"></div>
                        </div>
                        <div class="layui-form-mid layui-word-aux"></div>
                    </div>
                </div>
                <div class="layui-form-item">
                    <div class="layui-inline ">
                        <label class="layui-form-label ">流程名称</label>
                        <div class="layui-input-inline">
                            <input type="text" id="txtFlowName" name="txtFlowName" value="FlowName1" placeholder="" lay-verify="required" lay-reqtext="流程名称不能为空<br/>This cannot be empty" autocomplete="off" class="layui-input" runat="server">
                        </div>
                    </div>
                </div>
                <div class="layui-form-item">
                    <div class="layui-inline ">
                        <label class="layui-form-label" lay-tips="优先级排序，人员->角色->职位->部门">生效范围</label>
                        <div class="layui-input-inline ">
                            <select id="selEffectiveType" name="selEffectiveType" lay-filter="selEffectiveType" runat="server">                             
                                <option value="Use">指定人员生效</option>                               
                                <option value="Role">指定角色生效</option>                              
                                <option value="JTitle">指定职位生效</option>
                                <option value="Dept">指定部门生效</option>
                            </select>
                        </div>
                    </div>
                    <div class="layui-inline ">
                        <div class="layui-input-inline " style="width:280px;">
                            <div id="microXmSelect1"></div>
                            <input id="hidEffectiveIDStr" type="hidden" runat="server"/>
                        </div>
                        <div class="layui-form-mid layui-word-aux EffectiveType">不指定部门时全公司生效</div>
                    </div>
                </div>
                <div class="layui-form-item">
                    <div class="layui-inline ">
                        <label class="layui-form-label ">流程属性</label>
                        <div class="layui-input-inline" style="width:120px; left:15px;">
                            <input type="checkbox" id="ckIsAccept" name="ckIsAccept" lay-filter="ckIsAccept" title="启用受理" runat="server"> 
                        </div>
                        <div class="layui-form-mid layui-word-aux">
                            <i class="layui-icon layui-icon-about" lay-tips="启用受理：代表该表单是否需要受理（如：故障申请、派车申请等等，申请、审批通过后还需要进行受理、处理和对应的。 如请假申请，不需要受理，审批完成即完成）启用受理在这里紧在第一次添加流程时生效"></i> 
                        </div>
                        <div class="layui-input-inline" style="width:120px; left:15px;">
                            <input type="checkbox" id="ckIsSync" name="ckIsSync" lay-filter="ckIsSync" title="同步审批" runat="server"> 
                        </div>
                        <div class="layui-form-mid layui-word-aux">
                            <i class="layui-icon layui-icon-about" lay-tips="同步审批：即流程有多个节点需要审批时，审批流不需要按先后顺序进行，反之则需要按先后顺序进行"></i> 
                        </div>
                    </div>
                </div>
                <fieldset class="layui-elem-field layui-field-title" style="margin-top: 20px;">
                  <legend style="font-size:14px;">审批节点</legend>
                </fieldset>
                <div class="layui-form-item">
                    <div class="layui-inline ">
                        <label class="layui-form-label">节点名称</label>
                        <div class="layui-input-inline ">
                            <input type="text" id="txtNoteName" name="txtNoteName"  placeholder="节点名称" lay-reqtext="节点名称不能为空<br/>This cannot be empty" autocomplete="off" class="layui-input" runat="server">
                        </div>
                        <div class="layui-input-inline " style="width:100px; left:15px;">
                            <input type="checkbox" id="ckIsConditionApproval" name="ckIsConditionApproval" lay-skin="primary" lay-filter="ckIsConditionApproval" title="启用条件审批" runat="server"> 
                        </div>
                       <%-- <div class="layui-form-mid layui-word-aux"> 
                            <i class="ws-must-asterisk">&#42;</i>
                            <i class="layui-icon layui-icon-tips" lay-tips="节点名称。如：科长审批、部长审批等"></i>
                        </div>--%>
                    </div>
                </div>
                <div class="layui-form-item">
                    <div class="layui-inline ">
                        <label class="layui-form-label">审批类型</label>
                        <div class="layui-input-inline ">
                            <select name="selApprovalType" lay-filter="selApprovalType">
                                <option value="JTitle">按职位</option>
                                <option value="Role" disabled="disabled">按角色（还没开发）</option>
                                <option value="DeptUsers" disabled="disabled">按人员（还没开发）</option>
                            </select>
                        </div>
                    </div>
                    <div class="layui-inline ">
                        <div class="layui-input-inline" style="width:280px;">
                            <div id="microXmSelect2"></div> <%--主要审批人--%>
                        </div>
                    </div>
                    <div class="layui-inline ">
                        <div class="layui-input-inline " style="width:280px;">
                            <div id="microXmSelect3"></div> <%--代理审批人--%>
                        </div>
                    </div>
                    <div class="layui-inline">
                        <button type="button" id="btnAddNode" runat="server" class="layui-btn layui-btn-sm layui-btn-normal" lay-submit lay-filter="btnAddNode"><i class="layui-icon layui-icon-add-1 layuiadmin-button-btn"></i>添加审批节点</button>
                        <button type="button" id="btnModify" runat="server" class="layui-btn layui-btn-sm layui-hide" lay-submit lay-filter="btnModify">保存修改</button>
                    </div>
                </div>
                <div class="layui-form-item">
                    <div class="layui-inline ">
                        <label class="layui-form-label">条件设置</label>
                        <div class="layui-input-inline ">
                            <select id="selOperField" name="selOperField" lay-verify="" lay-filter="selOperField" disabled="disabled">
                                <option value="">运算字段</option>
                            </select>
                        </div>
                    </div>
                    <div class="layui-inline ">
                        <div class="layui-input-inline" style="width:280px;">
                            <select id="selCondition" name="selCondition" lay-filter="selCondition" disabled="disabled">
                                <option value="">条件</option>
                                <option value=">">大于</option>
                                <option value="=">等于</option>
                                <option value="<">小于</option>
                                <option value=">=">大于等于</option>
                                <option value="<=">小于等于</option>
                                <option value="<>">不等于</option>
                                <option value="like">包含</option>
                                <option value="not like">不包含</option>
                            </select>
                        </div>
                    </div>
                    <div class="layui-inline ">
                        <div class="layui-input-inline " style="width:280px;">
                            <input type="text" id="txtOperValue" name="txtOperValue"  placeholder="值" lay-reqtext="值不能为空<br/>This cannot be empty" autocomplete="off" class="layui-input layui-disabled" runat="server" disabled="disabled">
                        </div>
                    </div>
                </div>
                <div class="layui-form-item">
                    <table id="tabTable" lay-filter="tabTable"></table>
                </div>
            </div>
            <input id="txtWFID" name="txtWFID" type="hidden" runat="server" />
            <%--需提交值放在Form标签内--%>
        </div>

    </div>
    <input id="txtSTN" type="hidden" runat="server" />
    <input id="txtMID" type="hidden" runat="server" />
    <input id="txtFormID" type="hidden" runat="server"/>
    <input id="txtEditTablePermit" type="hidden" runat="server" value="0" />

    <script type="text/html" id="tplApprovalNameStr">
      <a href="javascript:;" class="layui-table-link micro-click" micro-id="{{d.WFID}}" micro-type="{{d.ApprovalType}}" micro-field="ApprovalIDStr" micro-value="{{d.ApprovalIDStr}}" micro-text="修改主要审批人" data-type="ChangeApproval">{{ d.ApprovalNameStr }}</a>
    </script>

    <script type="text/html" id="swApprovalIDStrSort">{{# if(d.ParentID!=0){ }}<input type="checkbox" name="ApprovalIDStrSort" value="{{d.WFID}}" lay-skin="switch" lay-text="降序|升序" lay-filter="ApprovalIDStrSort" {{ d.ApprovalIDStrSort == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="tplApprovalByNameStr">
      <a href="javascript:;" class="layui-table-link micro-click" micro-id="{{d.WFID}}" micro-type="{{d.ApprovalType}}" micro-field="ApprovalByIDStr" micro-value="{{d.ApprovalByIDStr}}"  micro-text="修改代理审批人" data-type="ChangeApproval">{{ d.ApprovalByNameStr = d.ApprovalByNameStr == "" ? "Null" : d.ApprovalByNameStr}}</a>
    </script>

    <script type="text/html" id="swApprovalByIDStrSort">{{# if(d.ParentID!=0){ }}<input type="checkbox" name="ApprovalByIDStrSort" value="{{d.WFID}}" lay-skin="switch" lay-text="降序|升序" lay-filter="ApprovalByIDStrSort" {{ d.ApprovalByIDStrSort == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swApproversSelectedByDefault">{{# if(d.ParentID!=0){ }}<input type="checkbox" name="ApproversSelectedByDefault" value="{{d.WFID}}" lay-skin="switch" lay-text="是|否" lay-filter="ApproversSelectedByDefault" {{ d.ApproversSelectedByDefault == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swIsOptionalApproval">{{# if(d.ParentID!=0){ }}<input type="checkbox" name="IsOptionalApproval" value="{{d.WFID}}" lay-skin="switch" lay-text="是|否" lay-filter="IsOptionalApproval" {{ d.IsOptionalApproval == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swIsVerticalDirection">{{# if(d.ParentID!=0){ }}<input type="checkbox" name="IsVerticalDirection" value="{{d.WFID}}" lay-skin="switch" lay-text="纵向|横向" lay-filter="IsVerticalDirection" {{ d.IsVerticalDirection == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swFindRange">{{# if(d.ParentID!=0){ }}<input type="checkbox" name="FindRange" value="{{d.WFID}}" lay-skin="switch" lay-text="跨部门|自部门" lay-filter="FindRange" {{ d.FindRange == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swFindGMOffice">{{# if(d.ParentID!=0){ }}<input type="checkbox" name="FindGMOffice" value="{{d.WFID}}" lay-skin="switch" lay-text="是|否" lay-filter="FindGMOffice" {{ d.FindGMOffice == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swIsSpecialApproval">{{# if(d.ParentID!=0){ }}<input type="checkbox" name="IsSpecialApproval" value="{{d.WFID}}" lay-skin="switch" lay-text="是|否" lay-filter="IsSpecialApproval" {{ d.IsSpecialApproval == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="swIsConditionApproval">{{# if(d.ParentID!=0){ }}<input type="checkbox" name="IsConditionApproval" value="{{d.WFID}}" lay-skin="switch" lay-text="是|否" lay-filter="IsConditionApproval" {{ d.IsConditionApproval == 'True' ? 'checked' : '' }} >{{# }}}</script>

    <script type="text/html" id="barTools">
        <a id="linkDel" runat="server" class="layui-btn layui-btn-danger layui-btn-xs" lay-event="Del">删除</a>
    </script>
    <script src="<%: ResolveUrl("~/Views/Forms/Js/Flow.js")%>"></script>
</asp:Content>

