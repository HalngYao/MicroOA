<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MicroForm.ascx.cs" Inherits="App_Ctrl_MicroForm" %>

    <%--
        该控件用法说明：
 
         URL参数顺序
        控件关联参数：
       0. Action=add/modify/view ，默认值：add
       1. ShortTableName=sys  //通常直接写定在调用的控件上，默认值：sys
       2. ModuleID=模块ID，默认值：1
       3. FormID=表单ID，默认值：1    配合IsApprovalForm使用，只有IsApprovalForm=true情况下FormID才生效
          FormID的作用，通常是审批类型的表单（即IsApprovalForm=true），用于获取表单【Forms表】的表单名称，所属部门、表单描述等信息
          因为表单通常分两种，一种是审批类型的表单、另一种是系统表单，如添加菜单、添加部门等，这两种表单都是调用该控件所以要进行区分
       4. PrimaryKeyValue=主键值，通常是在修改（即Action=modify）的情况下需要传入的参数
       5. FormCode=all/vcode/ccode/scode //通常直接写定在调用的控件上，如果单独传入两种或以上可以用逗号分割，如：vcode,ccode,ecode,scode 默认值：all
       6. IsApprovalForm 判断是否为审批类型的表单，配合FormID使用，如第3点所说
       7. ShowHeader在页面显示头部
       8. IsRefresh 提交后刷新页面



    --%>

    <%-- 动态生成Html控件 --%>
    <%= GetHtmlCode %>

    <%-- 动态生成表单验证、SelectChange事件、提交表单等JS代码 --%>
    <script type="text/javascript">
        layui.use(['index', 'jquery', 'form', 'laydate', 'layer', 'colorpicker', 'upload', 'layedit', 'micro'], function () {
            var $ = layui.$,
                form = layui.form,
                laydate = layui.laydate,
                layer = layui.layer,
                colorpicker = layui.colorpicker,
                upload = layui.upload,
                layedit = layui.layedit,
                micro = layui.micro;

            var MID = $("#txtMID").val();
            var FormID = $('#txtFormID').val();

            var microGet = {

                GetWorkFlow: function () {

                    var microSTN = $(this).attr('micro-stn');
                    var microMID = $(this).attr('micro-mid');
                    var microText = '自定义流程/Custom WorkFlow';

                    layer.open({
                        type: 2
                        , title: microText
                        , content: '/Views/Forms/WorkFlow/View/' + microSTN + '/' + microMID + '/' + FormID
                        , maxmin: true
                        , area: ['100%', '100%']
                        , scrollbar: false
                        , yes: function (index, layero) {
                            var State = layero.find('iframe').contents().find('#txtState').val();
                            if (State == 'True')
                                location.reload();
                        }
                        , cancel: function (index, layero) {
                            var State = layero.find('iframe').contents().find('#txtState').val();
                            if (State == 'True')
                                location.reload();
                        }
                    });
                }
            }

            $('.micro-click').on('click', function () {
                var type = $(this).data('type');
                microGet[type] ? microGet[type].call(this) : '';
            });

             <%= GetJsCode%>
        });
    </script>