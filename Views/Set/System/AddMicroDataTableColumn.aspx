<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="AddMicroDataTableColumn.aspx.cs" Inherits="Views_Set_System_AddMicroDataTableColumn" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="<%: ResolveUrl("~/Resource/css/micropopup.css")%>" rel="stylesheet" media="all" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
    <div class="layui-card">
        <div class="layui-form layui-card-body" style="padding: 15px;">

           <div class="layui-form-item">
                <label class="layui-form-label">数据表</label>
                <div class="layui-input-inline">
                    <select id="selTableName" name="selTableName" lay-verify="required" lay-filter="selTableName" lay-search="">
                        <option value="">请选择</option>
                    </select>
                    <input type="text" id="txtTableName" name="txtTableName" lay-verify="required" placeholder="" autocomplete="off" class="layui-input layui-hide">
                </div>
            </div>
            <div class="layui-form-item">
                <div class="layui-inline">
                    <label class="layui-form-label">数据类型</label>
                    <div class="layui-input-inline">
                        <select id="selDataType" name="selDataType" lay-verify="required" lay-filter="selDataType">
                            <option value="">请选择</option>
                            <option value="bit">bit</option>
                            <option value="char">char</option>
                            <option value="date">date</option>
                            <option value="time">time</option>
                            <option value="datetime">datetime</option>
                            <option value="decimal">decimal(18, 0)</option>
                            <option value="float">float</option>
                            <option value="int">int</option>
                            <option value="nchar">nchar(10)</option>
                            <option value="ntext">ntext</option>
                            <option value="nvarchar">nvarchar(50)</option>
                            <option value="text">text</option>
                            <option value="varchar">varchar(50)</option>
                        </select>
                    </div>
                    <div class="layui-form-mid layui-word-aux"> 
                        <i class="ws-must-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-tips" lay-tips="数据库的字段类型"></i>
                    </div>
                </div>
                <div class="layui-inline">
                    <label class="layui-form-label">允许空值</label>
                    <div class="layui-input-inline">
                        <input type="checkbox" id="ckAllowNull" name="ckAllowNull" lay-skin="switch" lay-text="是|否" lay-filter="ckAllowNull" title="允许空值" checked="checked">
                    </div>
                </div>
                <%--<div class="layui-form-mid layui-word-aux">括号值为默认长度，可填写字段长度改变</div>--%>
            </div>
            <div class="layui-form-item">
                <div class="layui-inline">
                    <label class="layui-form-label">字段名称</label>
                    <div class="layui-input-inline">
                        <input type="text" id="txtTabColName" name="txtTabColName" lay-verify="required" placeholder="请输入名称" autocomplete="off" class="layui-input onlyNumAlpha">
                    </div>
                    <div class="layui-form-mid layui-word-aux"> 
                        <i class="ws-must-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-tips" lay-tips="字段名称是唯一的不能重复"></i>
                    </div>
                </div>
                <div class="layui-inline">
                    <label class="layui-form-label">字段默认值</label>
                    <div class="layui-input-inline">
                        <input type="text" id="txtDefaultValue" name="txtDefaultValue" lay-verify="" placeholder="请输入" autocomplete="off" class="layui-input">
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-tips" lay-tips="数据库字段的默认值，当数据类型为Bit且需要填写默认值时，应该填入0或1"></i>
                    </div>
                </div>
                <div class="layui-inline" id="divFieldLength" style="display:none;">
                    <label class="layui-form-label">字段长度</label>
                    <div class="layui-input-inline">
                        <input type="text" id="txtFieldLength" name="txtFieldLength" lay-verify="required" placeholder="请输入数字" autocomplete="off" class="layui-input onlyNumCom">
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-must-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-tips" lay-tips="通常只有字符串类型和decimal才需设定长度"></i></div>
                </div>
            </div>
            <div class="layui-form-item">
                <div class="layui-inline">
                    <label class="layui-form-label">描述</label>
                    <div class="layui-input-inline layui-form-text">
                        <textarea  id="txtDescription" name="txtDescription" placeholder="请输入描述" class="layui-textarea"></textarea>
                    </div>
                </div>
            </div>
            <div class="layui-form-item">
                <div class="layui-inline">
                    <label class="layui-form-label">创建表单控件</label>
                    <div class="layui-input-inline">
                        <input type="checkbox" id="ckCreateCtrl" name="ckCreateCtrl" lay-skin="primary" lay-filter="ckCreateCtrl" title="是">
                        <input type="text" id="txtCreateCtrl" name="txtCreateCtrl" placeholder="" value="False" autocomplete="off" class="layui-input layui-hide">
                    </div>
                </div>
            </div>
            <div id="divCreateCtrl" style="display:none;">
                <div class="layui-form-item">
                    <div class="layui-inline">
                        <label class="layui-form-label">控件类型</label>
                        <div class="layui-input-inline">
                            <select id="ctlTypes" name="ctlTypes" lay-filter="ctlTypes">
                                <option value="">请选择</option>
                                <option value="Text">TextBox【文本框】</option>
                                <option value="Date">Date【日期选择框 yyyy-MM-dd】</option>
                                <option value="Time">Time【时间选择框 HH:mm:ss】</option>
                                <option value="DateTime">DateTime【日期时间选择框 yyyy-MM-dd HH:mm:ss】</option>
                                <option value="DateWeek">DateWeek【日期星期 yyyy-MM-dd(Week)】</option>
                                <option value="DateWeekTime">DateWeekTime【日期星期时间 yyyy-MM-dd(Week)HH:mm:ss】</option>
                                <option value="Password">Password【密码框】</option>
                                <option value="Hidden">Hidden 【隐藏域】</option>
                                <option value="ColorPicker">ColorPicker【颜色选择器】</option>
                                <option value="Radio">Radio【单选按钮】</option>
                                <option value="CheckBox">CheckBox【复选框】</option>
                                <option value="Select">Select【下拉列表】</option>
                                <option value="Div">Div【Div层】</option>
                                <option value="RadioTreeSelect">RadioTreeSelect【单选树】</option>
                                <option value="CheckBoxTreeSelect">CheckBoxTreeSelect【多选树】</option>
                                <option value="RadioCascaderSelect">RadioCascaderSelect【单选树级联】</option>
                                <option value="CheckBoxCascaderSelect">CheckBoxCascaderSelect【多选树级联】</option>
                                <option value="RadioCascaderSelectUserByDept">RadioCascaderSelectUserByDept 【单选树级联部门用户】</option>
                                <option value="CheckBoxCascaderSelectUserByDept">CheckBoxCascaderSelectUserByDept 【多选树级联部门用户】</option>
                                <option value="DragImgUpload">DragImgUpload【单图上转】</option>
                                <option value="MultiDragImgUpload">MultiDragImgUpload【多图上传】</option>
                                <option value="FileUpload">FileUpload【上传文件】</option>
                                <option value="Textarea">Textarea【文本域】</option>
                                <option value="LayEdit">LayEdit【富文本编辑器】</option>
                                <option value="WangEditor">WangEditor【富文本编辑器】</option>
                                <option value="Other">Other【其它】</option>
                            </select>
                        </div>
                        <div class="layui-form-mid layui-word-aux">
                            <i class="ws-must-asterisk">&#42;</i>
                            <i class="layui-icon layui-icon-tips" lay-tips="HTML控件"></i>
                        </div>
                    </div>
                    <div class="layui-inline">
                        <label class="layui-form-label">控件名称</label>
                        <div class="layui-input-inline">
                            <input type="text" id="txtctlTitle" name="txtctlTitle" placeholder="请输入控件名称" autocomplete="off" class="layui-input">
                        </div>
                        <div class="layui-form-mid layui-word-aux">
                            <i class="ws-must-asterisk">&#42;</i>
                            <i class="layui-icon layui-icon-tips" lay-tips="通常只有字符串类型和decimal才需设定长度"></i>
                        </div>
                    </div>
                </div>
                <div class="layui-form-item">
                    <div class="layui-inline">
                        <label class="layui-form-label">控件值</label>
                        <div class="layui-input-inline">
                            <input type="text" id="ctlValue" name="ctlValue" lay-verify="" placeholder="CtrlValue" autocomplete="off" class="layui-input">
                        </div>
                        <div class="layui-form-mid layui-word-aux">
                            <i class="ws-want-asterisk">&#42;</i>
                            <i class="layui-icon layui-icon-tips" lay-tips="String，控件值，即在生成控件时为控件赋值，当控件类型为CheckBox，Radio或Select时，可根据该值生成该类控件，且该值可从“数据来源”进行取值，但该值不为空时其优先级要高于“数据来源”。该值可以设置多个，使用英文逗号进行分隔即可，如：男,女。<br/>且同时控件类型为CheckBox，Radio或Select还可以通过冒号来区分显示名称和值，其格式： Name:Value 如：男:True,女:False。<br/>除此外该值还可以填入系统内置函数：GetDateTimeNow【返回当前系统时间】、GetClientIP【返回客户端IP】<br/>***注：控件值和控件默认值的关系是，当控件类型为TextBox这一类的，值和默认值随便选填一个都可以，当控件类型为Select\CheckBox\Radio这一类的，则值是用来显示有多少个选项，默认值则是让这类控件默认选中的功能，所以填写时需要注意。控件值与控件默认值的区别是，控件值：在控件生成时生成值，控件默认值：是控件在提交表单时生成需要返回的值"></i>
                        </div>
                    </div>
                    <div class="layui-inline">
                        <label class="layui-form-label">控件默认值</label>
                        <div class="layui-input-inline">
                            <input type="text" id="ctlDefaultValue" name="ctlDefaultValue" lay-verify="" placeholder="DefaultValue" autocomplete="off" class="layui-input">
                        </div>
                        <div class="layui-form-mid layui-word-aux">
                            <i class="ws-want-asterisk">&#42;</i>
                            <i class="layui-icon layui-icon-tips" lay-tips="String，控件默认值，在提交表单时根据默认值返回实际值。当控件类型为TextBox时默认值直接调用ctlValue【控件值】，且当控件类型为CheckBox，Radio或Select时，默认值即为该类控件默认选中。例：一个性别的Radio两个选项为 男:True,女:False，如果想让“女”默认选中则填入的默认值为False<br/>除此外该值还可以填入系统内置函数：GetDateTimeNow【返回当前系统时间】、GetClientIP【返回客户端IP】<br/>***注：控件值和控件默认值的关系是，当控件类型为TextBox这一类的，值和默认值随便选填一个都可以，当控件类型为Select\CheckBox\Radio这一类的，则值是用来显示有多少个选项，默认值则是让这类控件默认选中的功能，所以填写时需要注意。控件值与控件默认值的区别是，控件值：在控件生成时生成值，控件默认值：是控件在提交表单时生成需要返回的值"></i>
                        </div>
                    </div>
                    <div class="layui-inline">
                        <label class="layui-form-label">内容验证</label>
                        <div class="layui-input-inline">
                            <input type="text" id="ctlVerify2" name="ctlVerify2" lay-verify="" placeholder="required|phone|number" autocomplete="off" class="layui-input">
                        </div>
                        <div class="layui-form-mid layui-word-aux">
                            <i class="ws-want-asterisk">&#42;</i>
                            <i class="layui-icon layui-icon-tips" lay-tips="String，内容验证，多个验证分隔符“|”。可选值有：<br/>必填项：required <br/> 手机号：phone<br/>邮箱：email<br/>网址：url<br/>数字：number <br/> 日期：date <br/> 身份证：identity <br/>填入上述任意验证时则代表该控件不能为空值，则变为必填项"></i>
                        </div>
                    </div>
                </div>
                <div class="layui-form-item layui-row" id="divDataSource" style="display:none;">
                    <div class="layui-inline">
                        <label class="layui-form-label">数据来源</label>
                        <div class="layui-input-inline">
                            <select id="ctlSourceTable" name="ctlSourceTable" lay-verify="" lay-filter="ctlSourceTable">
                                <option value="">请选择</option>
                            </select>
                        </div>
                        <div class="layui-form-mid layui-word-aux">
                            <i class="ws-want-asterisk">&#42;</i>
                            <i class="layui-icon layui-icon-tips" lay-tips="String，当控件类型为CheckBox，Radio，Select时，它的数据来源表。当“控件默认值”为空时数据来源才生效"></i>
                        </div>
                    </div>
                    <div class="layui-inline">
                        <label class="layui-form-label">显示字段</label>
                        <div class="layui-input-inline">
                            <select id="ctlTextName" name="ctlTextName" lay-verify="" lay-filter="ctlTextName">
                                <option value="">请选择</option>
                            </select>
                        </div>
                        <div class="layui-form-mid layui-word-aux">
                            <i class="ws-want-asterisk">&#42;</i>
                            <i class="layui-icon layui-icon-tips" lay-tips="String，显示的字段名"></i>
                        </div>
                    </div>
                    <div class="layui-inline">
                        <label class="layui-form-label">值字段</label>
                        <div class="layui-input-inline">
                            <select id="ctlTextValue" name="ctlTextValue" lay-verify="" lay-filter="ctlTextValue">
                                <option value="">请选择</option>
                            </select>
                        </div>
                        <div class="layui-form-mid layui-word-aux">
                            <i class="ws-want-asterisk">&#42;</i>
                            <i class="layui-icon layui-icon-tips" lay-tips="String，值的字段"></i>
                        </div>
                    </div>
                </div>
            </div>
            <div class="layui-form-item" style="padding: 10px;">
                <div class="layui-form-mid layui-word-aux">
                    1. char是定长的，不足空格满，超出截断。<br />
                    2. varchar(n)，可变长度，非Unicode字符数据。n的取值范围为1至8000。<br />
                    3. nvarchar(n),包含n个字节的可变长度Unicode字符数据。n的取值范围为1到4000。无论字母或文字都占两个字节
                </div>
            </div>
            <div class="layui-form-item layui-hide1">
                <input type="button" id="btnAddCol" runat="server" class="layui-btn layui-hide" lay-submit lay-filter="btnAddCol"  value="立即保存">
            </div>

        </div>
    </div>
    <input id="txtMID" type="hidden" runat="server" />
    <script src="<%=ResolveUrl("~/Views/Set/Js/AddMicroDataTableColumn.js")%>"></script>
</asp:Content>

