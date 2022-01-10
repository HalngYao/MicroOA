<%@ Page Title="" Language="C#" MasterPageFile="~/Resource/MasterPage/Admin.master" AutoEventWireup="true" CodeFile="Control.aspx.cs" Inherits="Views_Set_System_Control" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ClientIDMode="Static">
    <div class="layui-card">
        <div class="layui-card-header">
            控件设置
        </div>

        <div class="layui-card-body layui-form" lay-filter="micro-form">

            <div class="layui-form-item layui-row layui-col-space5">
                <div class="layui-inline layui-col-xs12 layui-col-sm6 layui-col-md6 layui-col-lg6">
                    <label class="layui-form-label">数据字段：</label>
                    <div class="layui-input-block">
                        <select id="selTabColName" name="selTabColName" lay-verify="required" lay-filter="selTabColName" lay-search="">
                            <option value="">请选择</option>
                        </select>
                    </div>
                </div>
            </div>
            <div class="layui-form-item layui-row layui-col-space5">
                <div class="layui-inline">
                    <label class="layui-form-label">是否主键：</label>
                    <div class="layui-input-inline">
                        <input type="checkbox" id="ctlPrimaryKey" name="ctlPrimaryKey" lay-skin="primary" title="设为主键" class="">
                    </div>
                    <div class="layui-form-mid layui-word-aux" style="right: 90px;">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="Bit，是否为主键作为更新时的主键，字段名ctlPrimaryKey与PrimaryKey（每个表的默认ID）字段可二选一，其优先级高于PrimaryKey默认调用ctlPrimaryKey，默认值False。<br/>*特别注意：主键是具有唯一性的并且对应的值也是唯一性的，所以一个表不能够设定两个主键，一但设置错误会导致系统停止运行的所可能，非管理员操作建议使用默认值"></i>
                    </div>
                </div>
                <div class="layui-inline">
                    <label class="layui-form-label">是否禁用：</label>
                    <div class="layui-input-inline">
                        <input type="checkbox" id="Invalid" name="Invalid" lay-skin="primary" title="禁用">
                    </div>
                    <div class="layui-form-mid layui-word-aux" style="right: 120px; ">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="Boolean，禁用，默认值False"></i>
                    </div>
                </div>
            </div>

            <fieldset class="layui-elem-field layui-field-title ">
                <legend style="font-size: 16px;">表单属性/全局属性</legend>
            </fieldset>

            <div class="layui-form-item layui-row layui-col-space5" style="margin-bottom: -5px ">
                <div class="layui-inline">
                    <label class="layui-form-label">表单风格：</label>
                    <div class="layui-input-inline">
                        <select id="ctlFormStyle" name="ctlFormStyle" lay-verify="required" lay-search="">
                            <option value="">请选择</option>
                            <option value="Default" selected="selected">默认风格</option>
                            <option value="Box">方框风格</option>
                        </select>
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-must-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="生成表单的风格。默认风格：像本页面的风格一样； 方框风格：在标题加上方框"></i>
                    </div>
                </div>
            </div>
            <div class="layui-form-item layui-row layui-col-space5" style="margin-bottom: -5px ">
                <div class="layui-inline">
                    <label class="layui-form-label"></label>
                    <div class="layui-input-inline">
                        <input type="checkbox" id="ctlAddDisplayButton" name="ctlAddDisplayButton" lay-skin="primary" title="显示提交按钮" checked="checked" >
                    </div>
                    <div class="layui-input-inline">
                        <input type="checkbox" id="ctlSaveDraftButton" name="ctlSaveDraftButton" lay-skin="primary" title="显示临时保存按钮" checked="checked">
                    </div>
                    <div class="layui-input-inline">
                        <input type="checkbox" id="ctlModifyDisplayButton" name="ctlModifyDisplayButton" lay-skin="primary" title="显示修改按钮" checked="checked">
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="Bit，新增表单情况下显示提交按钮和临时保存按钮，在修改表单的情况下显示修改按钮。默认值 True"></i>
                    </div>
                </div>
            </div>


            <fieldset class="layui-elem-field layui-field-title ">
                <legend style="font-size: 16px;">控件基本属性</legend>
            </fieldset>

            <div class="layui-form-item layui-row layui-col-space5">
                <div class="layui-col-xs12 layui-col-sm6 layui-col-md4 layui-col-lg3">
                    <label class="layui-form-label">控件类型：</label>
                    <div class="layui-input-inline">
                        <select id="ctlTypes" name="ctlTypes" lay-verify="required" lay-filter="ctlTypes">
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
                        <i class="layui-icon layui-icon-about" lay-tips="String，可选值有【Text，Password，Hidden，Select，CheckBox，Radio，Textarea，Upload】当控件类型为CheckBox，Radio，Upload时默认生成Text接收选中的值"></i>
                    </div>
                </div>
                <div class="layui-col-xs12 layui-col-sm6 layui-col-md4 layui-col-lg3">
                    <label class="layui-form-label">控件前缀：</label>
                    <div class="layui-input-inline">
                        <input type="text" id="ctlPrefix" name="ctlPrefix" placeholder="txt|hid|sel|cb|ra" lay-verify="required" autocomplete="off" class="layui-input onlyAlpha" >
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-must-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="String，控件名称的前缀，如UserName->txtUserName。输入的值：txt"></i>
                    </div>
                </div>
                <div class="layui-col-xs12 layui-col-sm6 layui-col-md4 layui-col-lg3">
                    <label class="layui-form-label">控件样式：</label>
                    <div class="layui-input-inline">
                        <input type="text" id="ctlStyle" name="ctlStyle" lay-verify="" autocomplete="off" class="layui-input">
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="String，控件样式，一个完整的 style=&quot;width:300px;&quot;  它不紧紧只是一个样式你还可以这样写：  style=&quot;width:300px;&quot; readonly=&quot;readonly&quot;，即代表控件宽度为300px，并且是只读属性"></i>
                    </div>
                </div>
            </div>
            <div class="layui-form-item layui-row layui-col-space5">
                <div class="layui-col-xs12 layui-col-sm6 layui-col-md4 layui-col-lg3">
                    <label class="layui-form-label">标题：</label>
                    <div class="layui-input-inline">
                        <input type="text" id="ctlTitle" name="ctlTitle" placeholder="控件标题" lay-verify="" autocomplete="off" class="layui-input">
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="String，控件标题，即在label显示的titel，例：空值不显示label，=space 显示一个空的label（即有label但名称是空的）"></i>
                    </div>
                </div>
                <div class="layui-col-xs12 layui-col-sm6 layui-col-md4 layui-col-lg3">
                    <label class="layui-form-label">标题样式：</label>
                    <div class="layui-input-inline">
                        <input type="text" id="ctlTitleStyle" name="ctlTitleStyle" placeholder="style=&quot;width:10px;&quot;" lay-verify="" autocomplete="off" class="layui-input">
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="String，控件标题样式（即label样式），一个完整的 style"></i>
                    </div>
                </div>
                <div class="layui-col-xs12 layui-col-sm6 layui-col-md4 layui-col-lg3">
                    <label class="layui-form-label">占位水印：</label>
                    <div class="layui-input-inline">
                        <input type="text" id="ctlPlaceholder" name="ctlPlaceholder" lay-verify="" autocomplete="off" class="layui-input">
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="String，占位符，即text的水印符，目前只对text,和Textarea生效"></i>
                    </div>
                </div>
            </div>
            <div class="layui-form-item layui-row layui-col-space5">
                <div class="layui-col-xs12 layui-col-sm6 layui-col-md4 layui-col-lg3">
                    <label class="layui-form-label">复选框风格：</label>
                    <div class="layui-input-inline">
                        <select id="ctlCheckboxSkin" name="ctlCheckboxSkin" lay-verify="" lay-filter="ctlCheckboxSkin">
                            <option value="">请选择</option>
                            <option value="">复选框风格</option>
                            <option value="primary">原始风格</option>
                            <option value="switch">开关风格</option>
                        </select>
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="String，复选框风格。当控件类型为CheckBox时，可进行风格的设定，此属性对其它控件无效。可选值：复选框风格、原始风格、开关风格"></i>
                    </div>
                </div>
                <div class="layui-col-xs12 layui-col-sm6 layui-col-md4 layui-col-lg3">
                    <label class="layui-form-label">开关文本：</label>
                    <div class="layui-input-inline">
                        <input type="text" id="ctlSwitchText" name="ctlSwitchText" placeholder="开|关" lay-verify="" autocomplete="off" class="layui-input">
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="String，开关的文本。当Checkbox风格为开关风格时，开关显示的文本，此属性对其它控件无效。如：是|否"></i>
                    </div>
                </div>
            </div>
            <div class="layui-form-item layui-row layui-col-space5">
                <div class="layui-inline">
                    <label class="layui-form-label"></label>
                    <div class="layui-input-inline"  lay-tips="Bit，新增表单情况下显示该控件。默认值 True">
                        <input type="checkbox" id="ctlAddDisplay" name="ctlAddDisplay" lay-skin="primary" title="新增表单时显示该控件" checked="checked" >
                    </div>
                </div>
                <div class="layui-inline">
                    <label class="layui-form-label"></label>
                    <div class="layui-input-inline" lay-tips="Bit，修改表单情况下显示该控件。默认值 True">
                        <input type="checkbox" id="ctlModifyDisplay" name="ctlModifyDisplay" lay-skin="primary" title="修改表单时显示该控件" checked="checked">
                    </div>
                </div>
            </div>
            <div class="layui-form-item layui-row layui-col-space5">
                <div class="layui-inline">
                    <label class="layui-form-label"></label>
                    <div class="layui-input-inline" lay-tips="Bit，查看表单情况下显示该控件（即是否显示该条数据）。默认值 True">
                        <input type="checkbox" id="ctlViewDisplay" name="ctlViewDisplay" lay-skin="primary" title="查看表单时显示该控件" checked="checked">
                    </div>
                </div>
                <div class="layui-inline">
                    <label class="layui-form-label"></label>
                    <div class="layui-input-inline" lay-tips="Bit，查看表单情况下通过Label标签显示数据不用Input等控件。默认值 True">
                        <input type="checkbox" id="ctlViewDisplayLabel" name="ctlViewDisplayLabel" lay-skin="primary" title="查看表单时通过Label显示数据" checked="checked">
                    </div>
                </div>
            </div>
            <div class="layui-form-item layui-row layui-col-space5">
                <div class="layui-inline">
                    <label class="layui-form-label"></label>
                    <div class="layui-input-inline" lay-tips="Bit，控件后显示，通常在申请者提交表单后，需要受理方受理时填写的控件。默认值(D)：True">
                        <input type="checkbox" id="ctlAfterDisplay" name="ctlAfterDisplay" lay-skin="primary" title="控件后处理显示" checked="checked">
                    </div>
                </div>
            </div>
            <div class="layui-form-item layui-row layui-col-space5">
                <div class="layui-inline">
                    <label class="layui-form-label"  lay-tips="String，描述的显示位置，前面(before)：即紧跟在标题后面（像本标签一样）；后面(after)：即显示在HTML控件后面；标签上(label)：在标签上鼠标移入显示，当该值为空时则不显示描述。默认值(D)：after（后面）">描述显示位置</label>
                    <div class="layui-input-inline">
                        <select id="ctlDescriptionDisplayPosition" name="ctlDescriptionDisplayPosition" lay-verify="" lay-filter="ctlDescriptionDisplayPosition">
                            <option value="">请选择</option>
                            <option value="before">控件之前</option>
                            <option value="after">控件之后</option>
                            <option value="label">Label标签上</option>
                        </select>
                    </div>
                    <div class="layui-form-mid layui-word-aux" style="right:-20px;">
                    </div>
                </div>
                <div class="layui-inline">
                    <label class="layui-form-label" lay-tips="Bit，描述的显示方式，鼠标移过提示形式或直接显示。默认是提示形式，默认值：False">描述显示方式</label>
                    <div class="layui-input-inline">
                        <input type="checkbox" id="ctlDescriptionDisplayMode" name="ctlDescriptionDisplayMode" lay-skin="switch" lay-text="直显|提示">
                    </div>
                    <div class="layui-form-mid layui-word-aux" style="right:-20px;">
                    </div>
                </div>
                <div class="layui-inline">
                    <label class="layui-form-label" lay-tips="Bit，是否显示星号。必填项时星号为红色否则灰色。默认为值：True">显示星号</label>
                    <div class="layui-input-inline">
                        <input type="checkbox" id="ctlDisplayAsterisk" name="ctlDisplayAsterisk" checked="checked" lay-skin="switch" lay-text="显示|隐藏">
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                    </div>
                </div>
            </div>
            <div class="layui-form-item layui-row layui-col-space5">
                <div class="layui-col-xs12 layui-col-sm12 layui-col-md12 layui-col-lg6">
                    <label class="layui-form-label">描述内容：</label>
                    <div class="layui-input-block layui-form-text">
                        <textarea id="ctlDescription" name="ctlDescription" autocomplete="off" class="layui-textarea"></textarea>
                    </div>
                </div>
            </div>
            <div class="layui-form-item layui-row layui-col-space5">
                <div class="layui-col-xs12 layui-col-sm12 layui-col-md12 layui-col-lg6">
                    <label class="layui-form-label" lay-tips="String，当描述为直接显示时还需要一个div层存放描述的内容，如有需要可以对这个div层进行的样式的设定。一个完整的style代码"><i class="ws-want-asterisk">&#42;</i>描述层样式：</label>
                    <div class="layui-input-block layui-form-text">
                        <textarea placeholder="String，当描述为直接显示时还需要一个div层存放描述的内容，如有需要可以对这个div层进行的样式的设定。一个完整的style代码" class="layui-textarea" id="ctlDescriptionStyle" name="ctlDescriptionStyle"></textarea>
                    </div>
                </div>
            </div>

            <fieldset class="layui-elem-field layui-field-title">
                <legend style="font-size: 16px;">控件扩展属性</legend>
            </fieldset>

            <div class="layui-form-item layui-row layui-col-space5">
                <div class="layui-col-xs12 layui-col-sm6 layui-col-md4 layui-col-lg3">
                    <label class="layui-form-label">控件值：</label>
                    <div class="layui-input-inline">
                        <input type="text" id="ctlValue" name="ctlValue" lay-verify="" autocomplete="off" class="layui-input">
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="String，控件值，即在生成控件时为控件赋值，当控件类型为CheckBox，Radio或Select时，可根据该值生成该类控件，且该值可从“数据来源”进行取值，但该值不为空时其优先级要高于“数据来源”。该值可以设置多个，使用英文逗号进行分隔即可，如：男,女。<br/>且同时控件类型为CheckBox，Radio或Select还可以通过冒号来区分显示名称和值，其格式： Name:Value 如：男:True,女:False。<br/>除此外该值还可以填入系统内置函数：GetDateTimeNow【返回当前系统时间】、GetClientIP【返回客户端IP】<br/>***注：控件值和控件默认值的关系是，当控件类型为TextBox这一类的，值和默认值随便选填一个都可以，当控件类型为Select\CheckBox\Radio这一类的，则值是用来显示有多少个选项，默认值则是让这类控件默认选中的功能，所以填写时需要注意。控件值与控件默认值的区别是，控件值：在控件生成时生成值，控件默认值：是控件在提交表单时生成需要返回的值"></i>
                    </div>
                </div>
                <div class="layui-col-xs12 layui-col-sm6 layui-col-md4 layui-col-lg3">
                    <label class="layui-form-label">默认值：</label>
                    <div class="layui-input-inline">
                        <input type="text" id="ctlDefaultValue" name="ctlDefaultValue" lay-verify="" autocomplete="off" class="layui-input">
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="String，控件默认值，在提交表单时根据默认值返回实际值。当控件类型为TextBox时默认值直接调用ctlValue【控件值】，且当控件类型为CheckBox，Radio或Select时，默认值即为该类控件默认选中。例：一个性别的Radio两个选项为 男:True,女:False，如果想让“女”默认选中则填入的默认值为False<br/>除此外该值还可以填入系统内置函数：GetDateTimeNow【返回当前系统时间】、GetClientIP【返回客户端IP】<br/>***注：控件值和控件默认值的关系是，当控件类型为TextBox这一类的，值和默认值随便选填一个都可以，当控件类型为Select\CheckBox\Radio这一类的，则值是用来显示有多少个选项，默认值则是让这类控件默认选中的功能，所以填写时需要注意。控件值与控件默认值的区别是，控件值：在控件生成时生成值，控件默认值：是控件在提交表单时生成需要返回的值"></i>
                    </div>
                </div>
                <div class="layui-col-xs12 layui-col-sm6 layui-col-md4 layui-col-lg3">
                    <label class="layui-form-label">额外函数</label>
                    <div class="layui-input-inline">
                        <input type="text" id="ctlExtraFunction" name="ctlExtraFunction" lay-verify="" autocomplete="off" class="layui-input">
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="String，额外函数，这是一个无返回值的函数。通常在提交表单时需要执行的额外操作"></i>
                    </div>
                </div>
                <div class="layui-col-xs12 layui-col-sm6 layui-col-md4 layui-col-lg3 layui-hide">
                    <label class="layui-form-label">接收名称：</label>
                    <div class="layui-input-inline">
                        <input type="text" id="ctlReceiveName" name="ctlReceiveName" lay-verify="" placeholder="暂时没有启用" autocomplete="off" class="layui-input layui-disabled" disabled="disabled">
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="String，接收名称【暂时没有启用20190903】，在提交表单时的变量名称，当为空值时默认由：ctlPrefix+TabColName（前缀+字段名称组成）"></i>
                    </div>
                </div>
            </div>
            <div class="layui-form-item layui-row layui-col-space5">
                <div class="layui-col-xs12 layui-col-sm6 layui-col-md4 layui-col-lg3">
                    <label class="layui-form-label">数据来源：</label>
                    <div class="layui-input-inline">
                        <select id="ctlSourceTable" name="ctlSourceTable" lay-verify="" lay-filter="ctlSourceTable">
                            <option value="">请选择</option>
                        </select>
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="String，当控件类型为CheckBox，Radio，Select时，它的数据来源表。当”默认值“为空时数据来源才生效"></i>
                    </div>
                </div>
                <div class="layui-col-xs12 layui-col-sm6 layui-col-md4 layui-col-lg3">
                    <label class="layui-form-label">显示字段：</label>
                    <div class="layui-input-inline">
                        <select id="ctlTextName" name="ctlTextName" lay-verify="" lay-filter="ctlTextName">
                            <option value="">请选择</option>
                        </select>
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="String，显示的字段名"></i>
                    </div>
                </div>
                <div class="layui-col-xs12 layui-col-sm6 layui-col-md4 layui-col-lg3">
                    <label class="layui-form-label">值字段：</label>
                    <div class="layui-input-inline">
                        <select id="ctlTextValue" name="ctlTextValue" lay-verify="" lay-filter="ctlTextValue">
                            <option value="">请选择</option>
                        </select>
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="String，值的字段"></i>
                    </div>
                </div>
            </div>
 
            <div class="layui-form-item layui-row layui-col-space5">
                <div class="layui-col-xs12 layui-col-sm12 layui-col-md12 layui-col-lg6">
                    <label class="layui-form-label" lay-tips="String，控件扩展JS代码。例如：当控件要输入日期时可以通过这个JS属性进行绑定日期插件等，通常对Text控件进行设定。代码示例：<br/> laydate.render({  <br/> elem: '#test' //或 elem: document.getElementById('test')、elem: lay('#test') 等<br/> });"><i class="ws-want-asterisk">&#42;</i>控件扩展JS</label>
                    <div class="layui-input-block layui-form-text">
                        <textarea placeholder="String，扩展控件JS代码。例如：当控件要输入日期时可以通过这个JS属性进行绑定日期插件等。代码示例：<br/> laydate.render({  <br/> elem: '#test' //或 elem: document.getElementById('test')、elem: lay('#test') 等<br/> });" class="layui-textarea" id="ctlExtendJSCode" name="ctlExtendJSCode"></textarea>
                    </div>
                </div>
            </div>

            <fieldset class="layui-elem-field layui-field-title">
                <legend style="font-size: 16px;">控件内容验证</legend>
            </fieldset>

            <div class="layui-form-item layui-row layui-col-space5">
                <div class="layui-col-xs12 layui-col-sm6 layui-col-md4 layui-col-lg3">
                    <label class="layui-form-label">允许字符长度</label>
                    <div class="layui-input-inline">
                        <input type="text" id="ctlCharLength" name="ctlCharLength" value="0" lay-verify="" autocomplete="off" class="layui-input onlyNumCom">
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="String，字符长度，通常针对text控件进行字符长度限制，该值不为空或0时才生效。例1单个限制：999->允许最大长度为999，例2区间范围限制（逗号分隔）：10,999->允许长度在10~999之间（同时表示该项为必填项）。默认值：0或空即不限制长度"></i>
                    </div>
                </div>
                <div class="layui-col-xs12 layui-col-sm6 layui-col-md4 layui-col-lg3">
                    <label class="layui-form-label">事件过滤：</label>
                    <div class="layui-input-inline">
                        <input type="text" id="ctlFilter" name="ctlFilter" lay-verify="" autocomplete="off" class="layui-input">
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="String，事件过滤器，主要用于事件的精确匹配，跟选择器是比较类似的。其实它并不私属于form模块，它在 layui 的很多基于事件的接口中都会应用到。例：lay-filter=“test”输入的值：test。空值时默认调前缀+用字段名（ctlPrefix+TabColName）"></i>
                    </div>
                </div>
                <div class="layui-col-xs12 layui-col-sm6 layui-col-md4 layui-col-lg3">
                    <label class="layui-form-label">验证1：</label>
                    <div class="layui-input-inline">
                        <input type="text" id="ctlVerify" name="ctlVerify" lay-verify="" placeholder="onlyNum onlyAlpha onlyNumAlpha onlyNumPoint" autocomplete="off" class="layui-input">
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="String，调用CSS样式名称作为验证，多个验证分隔符”空格“。可选值有：<br/>限制只能输入数字：onlyNum，<br/>限制只能输入字母：onlyAlpha，<br/>限制只能输入数字和字母：onlyNumAlpha，<br/>限制只能输入数字和小数点：onlyNumPoint，<br/>限制只能输入数字和逗号：onlyNumCom，<br/>限制只能输入数字和冒号：onlyNumColon""></i>
                    </div>
                </div>
                <div class="layui-col-xs12 layui-col-sm6 layui-col-md4 layui-col-lg3">
                    <label class="layui-form-label">验证2：</label>
                    <div class="layui-input-inline">
                        <input type="text" id="ctlVerify2" name="ctlVerify2" lay-verify="" placeholder="required|phone|email|url|number|date|identity" autocomplete="off" class="layui-input">
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="String，内容验证，多个验证分隔符“|”。可选值有：<br/>必填项：required <br/> 手机号：phone<br/>邮箱：email<br/>网址：url<br/>数字：number <br/> 日期：date <br/> 身份证：identity <br/>自定义名称，为自定义时请在下方文本域填写自定义函数"></i>
                    </div>
                </div>
            </div>
            <div class="layui-form-item layui-row layui-col-space5">
                <div class="layui-col-xs12 layui-col-sm12 layui-col-md12 layui-col-lg6">
                    <label class="layui-form-label" lay-tips="String，这是运行在form.verify({ })代码块内的自定义代码，当验证2包含自定义时，请填写自定义函数。 例子： <br/> Example: function (value, item) {if (value.length <= 0 || value.length > 100) return \'只允许在100字以内。< br/> Only allowed 100 words.\'; }  Example是函数名称"><i class="ws-want-asterisk">&#42;</i>自定函数：</label>
                    <div class="layui-input-block layui-form-text">
                        <textarea placeholder="String，这是运行在form.verify({ })代码块内的自定义代码，当验证2包含自定义时，请填写自定义函数。 例子： <br/> Example: function (value, item) {if (value.length <= 0 || value.length > 100) return \'只允许在100字以内。< br/> Only allowed 100 words.\'; }  Example是函数名称" class="layui-textarea" id="ctlVerifyCustomFunction" name="ctlVerifyCustomFunction"></textarea>
                    </div>
                </div>
            </div>

            <fieldset class="layui-elem-field layui-field-title">
                <legend style="font-size: 16px;">控件排版风格</legend>
            </fieldset>

            <div class="layui-form-item layui-row layui-col-space5">
                <div class="layui-col-xs12 layui-col-sm6 layui-col-md4 layui-col-lg3">
                    <label class="layui-form-label">组别：</label>
                    <div class="layui-input-inline">
                        <input type="text" id="ctlGroup" name="ctlGroup" lay-verify="required" autocomplete="off" class="layui-input onlyNum" value="0">
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="Int，组别数字类型，相同的数字为同一组，把同一个组的控件放在同一个FormItem下。 空值默认值：0"></i>
                    </div>
                </div>
                <div class="layui-col-xs12 layui-col-sm6 layui-col-md4 layui-col-lg3">
                    <label class="layui-form-label">组别描述：</i></label>
                    <div class="layui-input-inline">
                        <input type="text" id="ctlGroupDescription" name="ctlGroupDescription" lay-verify="" autocomplete="off" class="layui-input">
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="String，组别描述，像当页一样用横线分割的文字，空值时不显示分割线。"></i>
                    </div>
                </div>
            </div>
            <div class="layui-form-item layui-row layui-col-space5">
                <div class="layui-col-xs12 layui-col-sm6 layui-col-md4 layui-col-lg3">
                    <label class="layui-form-label">ItemStyle：</label>
                    <div class="layui-input-inline">
                        <input type="text" id="ctlFormItemStyle" name="ctlFormItemStyle" lay-verify="" autocomplete="off" class="layui-input">
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="String，FormItem的样式。一个完整的style代码"></i>
                    </div>
                </div>
                <div class="layui-col-xs12 layui-col-sm6 layui-col-md4 layui-col-lg3">
                    <label class="layui-form-label">InlineClass：</label>
                    <div class="layui-input-inline">
                        <input type="text" id="ctlInlineCss" name="ctlInlineCss" lay-verify="" autocomplete="off" class="layui-input">
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="String，FormItem下的Inline的Css，多个Css空格隔开，配合xs/sm/md/lg实现不同屏幕下的排版显示状态。默认值：layui-inline （layui-inline是一个固定宽度的类可不填）"></i>
                    </div>
                </div>
                <div class="layui-col-xs12 layui-col-sm6 layui-col-md4 layui-col-lg3">
                    <label class="layui-form-label">InlineStyle：</label>
                    <div class="layui-input-inline">
                        <input type="text" id="ctlInlineStyle" name="ctlInlineStyle" lay-verify="" autocomplete="off" class="layui-input">
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="String，FormItem下的Inline的样式。一个完整的style代码"></i>
                    </div>
                </div>
            </div>
            <div class="layui-form-item layui-row layui-col-space5">
                <div class="layui-col-xs12 layui-col-sm6 layui-col-md4 layui-col-lg3">
                    <label class="layui-form-label">ColSpace：</label>
                    <div class="layui-input-inline">
                        <select id="ctlColSpace" name="ctlColSpace" lay-verify="" lay-search="">
                            <option value="">请选择</option>
                            <option value="layui-col-space1">Space1</option>
                            <option value="layui-col-space3">Space3</option>
                            <option value="layui-col-space5">Space5</option>
                            <option value="layui-col-space10">Space10</option>
                            <option value="layui-col-space15">Space15</option>
                            <option value="layui-col-space20">Space20</option>
                            <option value="layui-col-space25">Space25</option>
                            <option value="layui-col-space30">Space30</option>
                        </select>
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="String，列宽数字类型，同一行内的控件元素的间隔，同一行内所设定的数值应该是一样的。可选值【1，3，5，10，15，20，25，30】，默认值 5"></i>
                    </div>
                </div>
                <div class="layui-col-xs12 layui-col-sm6 layui-col-md4 layui-col-lg3">
                    <label class="layui-form-label">Offset：</label>
                    <div class="layui-input-inline">
                        <input type="text" id="ctlOffset" name="ctlOffset" lay-verify="" autocomplete="off" class="layui-input">
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="String，对列追加 类似 layui-col-md-offset* 的预设类，从而让列向右偏移。其中 * 号代表的是偏移占据的列数，md可选值为：lg/md/sm/xs，数字可选值为： 1 - 12。 <br/>如：layui-col-md-offset3，即代表在“中型桌面屏幕”下，让该列向右偏移3个列宽度，多种组合请用空格分开"></i>
                    </div>
                </div>
                <div class="layui-col-xs12 layui-col-sm6 layui-col-md4 layui-col-lg3">
                    <label class="layui-form-label">XS：</label>
                    <div class="layui-input-inline">
                        <select id="ctlXS" name="ctlXS" lay-verify="" lay-search="">
                            <option value="">请选择</option>
                            <option value="layui-col-xs1">XS1</option>
                            <option value="layui-col-xs2">XS2</option>
                            <option value="layui-col-xs3">XS3</option>
                            <option value="layui-col-xs4">XS4</option>
                            <option value="layui-col-xs5">XS5</option>
                            <option value="layui-col-xs6">XS6</option>
                            <option value="layui-col-xs7">XS7</option>
                            <option value="layui-col-xs8">XS8</option>
                            <option value="layui-col-xs9">XS9</option>
                            <option value="layui-col-xs10">XS10</option>
                            <option value="layui-col-xs11">XS11</option>
                            <option value="layui-col-xs12">XS12</option>
                        </select>
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="String，超小屏幕如手机，把一行平均分为12等份，占用12份则显示1列、占用6份则显示2列、占用4份则显示3列，列数=12除以等份的值"></i>
                    </div>
                </div>
                <div class="layui-col-xs12 layui-col-sm6 layui-col-md4 layui-col-lg3">
                    <label class="layui-form-label">SM：</label>
                    <div class="layui-input-inline">
                        <select id="ctlSM" name="ctlSM" lay-verify="" lay-search="">
                            <option value="">请选择</option>
                            <option value="layui-col-sm1">SM1</option>
                            <option value="layui-col-sm2">SM2</option>
                            <option value="layui-col-sm3">SM3</option>
                            <option value="layui-col-sm4">SM4</option>
                            <option value="layui-col-sm5">SM5</option>
                            <option value="layui-col-sm6">SM6</option>
                            <option value="layui-col-sm7">SM7</option>
                            <option value="layui-col-sm8">SM8</option>
                            <option value="layui-col-sm9">SM9</option>
                            <option value="layui-col-sm10">SM10</option>
                            <option value="layui-col-sm11">SM11</option>
                            <option value="layui-col-sm12">SM12</option>
                        </select>
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="String，中午屏幕如平板，把一行平均分为12等份，占用12份则显示1列、占用6份则显示2列、占用4份则显示3列，列数=12除以等份的值。"></i>
                    </div>
                </div>
            </div>
            <div class="layui-form-item layui-row layui-col-space5">
                <div class="layui-col-xs12 layui-col-sm6 layui-col-md4 layui-col-lg3">
                    <label class="layui-form-label">MD：</label>
                    <div class="layui-input-inline">
                        <select id="ctlMD" name="ctlMD" lay-verify="" lay-search="">
                            <option value="">请选择</option>
                            <option value="layui-col-md1">MD1</option>
                            <option value="layui-col-md2">MD2</option>
                            <option value="layui-col-md3">MD3</option>
                            <option value="layui-col-md4">MD4</option>
                            <option value="layui-col-md5">MD5</option>
                            <option value="layui-col-md6">MD6</option>
                            <option value="layui-col-md7">MD7</option>
                            <option value="layui-col-md8">MD8</option>
                            <option value="layui-col-md9">MD9</option>
                            <option value="layui-col-md10">MD10</option>
                            <option value="layui-col-md11">MD11</option>
                            <option value="layui-col-md12">MD12</option>
                        </select>
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="Int，一般屏幕如桌面PC，把一行平均分为12等份，占用12份则显示1列、占用6份则显示2列、占用4份则显示3列，列数=12除以等份的值。"></i>
                    </div>
                </div>
                <div class="layui-col-xs12 layui-col-sm6 layui-col-md4 layui-col-lg3">
                    <label class="layui-form-label">LG：</label>
                    <div class="layui-input-inline">
                        <select id="ctlLG" name="ctlLG" lay-verify="" lay-search="">
                            <option value="">请选择</option>
                            <option value="layui-col-lg1">LG1</option>
                            <option value="layui-col-lg2">LG2</option>
                            <option value="layui-col-lg3">LG3</option>
                            <option value="layui-col-lg4">LG4</option>
                            <option value="layui-col-lg5">LG5</option>
                            <option value="layui-col-lg6">LG6</option>
                            <option value="layui-col-lg7">LG7</option>
                            <option value="layui-col-lg8">LG8</option>
                            <option value="layui-col-lg9">LG9</option>
                            <option value="layui-col-lg10">LG10</option>
                            <option value="layui-col-lg11">LG11</option>
                            <option value="layui-col-lg12">LG12</option>
                        </select>
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="String，超大屏幕如电视机，把一行平均分为12等份，占用12份则显示1列、占用6份则显示2列、占用4份则显示3列，列数=12除以等份的值。"></i>
                    </div>
                </div>
                <div class="layui-col-xs12 layui-col-sm6 layui-col-md4 layui-col-lg3">
                    <label class="layui-form-label">InputInline：</label>
                    <div class="layui-input-inline">
                        <select id="ctlInputInline" name="ctlInputInline" lay-verify="required" lay-filter="ctlTypes">
                            <option value="">请选择</option>
                            <option value="layui-input-inline" selected="selected">Inlike</option>
                            <option value="layui-input-block">Block</option>
                        </select>
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="String，存放Input控件的div，可选值【inline、block】"></i>
                    </div>
                </div>
                <div class="layui-col-xs12 layui-col-sm6 layui-col-md4 layui-col-lg3">
                    <label class="layui-form-label">InputInlineStyle</label>
                    <div class="layui-input-inline">
                        <input type="text" id="ctlInputInlineStyle" name="ctlInputInlineStyle" lay-verify="" autocomplete="off" class="layui-input">
                    </div>
                    <div class="layui-form-mid layui-word-aux">
                        <i class="ws-want-asterisk">&#42;</i>
                        <i class="layui-icon layui-icon-about" lay-tips="String，InputInline样式，一个完整的style代码"></i>
                    </div>
                </div>
            </div>
            <fieldset class="layui-elem-field layui-field-title">
                <legend style="font-size: 16px;">扩展HTML代码</legend>
            </fieldset>

            <div class="layui-form-item layui-row layui-col-space5">
                <div class="layui-col-xs12 layui-col-sm12 layui-col-md12 layui-col-lg6">
                    <label class="layui-form-label" lay-tips="NText，自定义HTML代码，该代码放置layui-card-body内，在所有动态控件生成之后，弥补动态生成的不足"><i class="ws-want-asterisk">&#42;</i>HTML代码：</label>
                    <div class="layui-input-block layui-form-text">
                        <textarea placeholder="NText，自定义HTML代码，在生成表单（新增或修改表单）时用，该代码放置layui-card-body内，在所有动态控件生成之后，弥补动态生成的不足之处。" class="layui-textarea" id="CustomHtmlCode" name="CustomHtmlCode"></textarea>
                    </div>
                </div>
            </div>

            <div class="layui-form-item">
                <div class="layui-input-block" style="margin-top: 10px;">
                    <button type="button" id="btnModify" runat="server" class="layui-btn layui-btn-normal" lay-submit="" lay-filter="btnModify">保存修改</button>
                </div>
            </div>

        </div>

    </div>
    <input id="txtMID" type="hidden" runat="server" />
    <script src="<%: ResolveUrl("~/Views/Set/Js/Control.js")%>"></script>
</asp:Content>

