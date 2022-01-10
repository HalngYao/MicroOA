layui.use(['index', 'jquery', 'form', 'table', 'micro'], function () {
    var $ = layui.$
        , form = layui.form
        , table = layui.table
        , micro = layui.micro;

    var Action = $('#txtAction').val();
    var Type = $('#txtType').val();
    var MID = $('#txtMID').val();
    
    //事件
    var mGet = {
        ViewPublicFormList: function (action,type,mid) {
            table.render({
                elem: '#tabTable'
                , url: micro.getRootPath() + '/Views/Forms/MicroPublicFormList.ashx?action=' + action + '&type=' + type + '&mid=' + mid + "&random=" + Math.random()
                , even: true
                , toolbar: true
                , cols: [[
                    { type: 'checkbox', fixed: 'left' }
                    , { field: 'FormName', title: '类型', width: 230, align: 'center', fixed: 'left' }
                    , { field: 'FormNumber', title: '编号', width: 160, align: 'center', fixed: 'left', toolbar: '#barFormNumber' }
                    , { field: 'DisplayName', title: '申请者', width: 200, align: 'center', fixed: 'left' }
                    , { field: 'NodeName', title: '所在阶段', width: 150, align: 'center' }
                    , { field: 'ApprovalState', title: '表单状态', width: 200, align: 'center' }
                    , { field: 'ApprovalDisplayName', title: '审批者', width: 200, align: 'center' }
                    , { field: 'ApprovalTime', title: '审批时间', width: 220, align: 'center' }
                    , { field: 'WorkTel', title: '分机', width: 100, align: 'center' }
                    , { field: 'WorkMobilePhone', title: '手机', width: 160, align: 'center' }
                    , { field: 'EMail', title: '邮箱', width: 330, align: 'center' }
                    , { field: 'AdDepartment', title: '部门/科室', width: 500, align: 'left' }
                    , { field: 'DateCreated', title: '申请时间', width: 220, align: 'center', fixed: 'right' }

                ]]
                , page: true
                , limit: 100
                , limits: [10, 20, 30, 50, 100, 200, 300, 500]
                , data: ''
                , height: 'full-120'
                , text: { none: 'No relevant data.暂无相关数据。' }
            });
       
        },

        layerOpen: function (shortTableName, moduleID, formID, formsID, formNumber) {

            //防止页面回发
            $(window.parent.document).find('#txtState').val('False');
            $('#txtState').val('False');

            var parm = { "action": "View", "stn": shortTableName, "mid": moduleID, "formid": formID, "pkv": formsID }
            layer.open({
                type: 2
                , title: '《' + formNumber + '》申请单的详情 / Details of the application form.'
                , content: micro.getAjaxData('json', micro.getRootPath() + '/App_Handler/GetFormLinkAddress.ashx', parm)["LinkAddress"] + '/View/' + shortTableName + '/' + moduleID + '/' + formID + '/' + formsID
                , area: ['95%', '90%']
                , btn: ['同意【Agree】', '驳回【Return】', '转发【Forward】', '草稿【Draft】', '提交【Submit】', '修改【Modify】', '撤回【Withdrawal】', '打印【Print】', '受理【Accept】', '结案【Case closed】', '删除【Delete】', '关闭【Close】']
                , skin: micro.getAjaxData('json', micro.getRootPath() + '/Views/Forms/MicroFormCheckApproval.ashx', parm)["Class"] 
                , yes: function (index, layero) {  //按钮一 同意
                    layer.open({
                        type: 2
                        , title: '审批表单《' + formNumber + '》'
                        , content: '/Views/Forms/MicroFormApproval/Agree/' + shortTableName + '/' + moduleID + '/' + formID + '/' + formsID
                        , area: ['56%', '62%']
                        , btn: ['保存【Save】', '关闭【Close】']
                        , skin: 'micro-layer-class'
                        , yes: function (index, layero) {  //按钮一
                            var iframeWindow = window['layui-layer-iframe' + index]
                                , submitID = 'btnSave'
                                , submit = layero.find('iframe').contents().find('#' + submitID);

                            $(submit).click();
                        }
                        , cancel: function (index, layero) {  //右上角关闭
                            var State = layero.find('iframe').contents().find('#txtState').val();
                            if (State === 'True') {
                                layer.closeAll('iframe');
                                mGet.layerOpen(shortTableName, moduleID, formID, formsID, formNumber);
                                $('#txtState').val('False');
                                $(window.parent.document).find('#txtState').val('False');
                            }
                        }
                        , end: function () {
                            var State = $('#txtState').val();
                            if (State === 'True') {
                                layer.closeAll('iframe');
                                mGet.layerOpen(shortTableName, moduleID, formID, formsID, formNumber);
                                $('#txtState').val('False');
                                $(window.parent.document).find('#txtState').val('False');
                            }
                        }
                    });
                }
                , btn2: function (index, layero) {//驳回
                    layer.open({
                        type: 2
                        , title: '审批表单《' + formNumber + '》'
                        , content: '/Views/Forms/MicroFormApproval/Return/' + shortTableName + '/' + moduleID + '/' + formID + '/' + formsID
                        , area: ['56%', '60%']
                        , btn: ['保存【Save】', '关闭【Close】']
                        , skin: 'micro-layer-class'
                        , yes: function (index, layero) {  //按钮一
                            var iframeWindow = window['layui-layer-iframe' + index]
                                , submitID = 'btnSave'
                                , submit = layero.find('iframe').contents().find('#' + submitID);

                            $(submit).click();
                        }
                        , cancel: function (index, layero) {  //右上角关闭
                            var State = layero.find('iframe').contents().find('#txtState').val();
                            if (State === 'True') {
                                layer.closeAll('iframe');
                                mGet.layerOpen(shortTableName, moduleID, formID, formsID, formNumber);
                                $('#txtState').val('False');
                                $(window.parent.document).find('#txtState').val('False');
                            }
                        }
                        , end: function () {
                            var State = $('#txtState').val();
                            if (State === 'True') {
                                layer.closeAll('iframe');
                                mGet.layerOpen(shortTableName, moduleID, formID, formsID, formNumber);
                                $('#txtState').val('False');
                                $(window.parent.document).find('#txtState').val('False');
                            }
                        }
                    });
                    return false;
                }
                , btn3: function (index, layero) { //转发

                    layer.msg("转发功能还没有开放。<br/>The forwarding function is not yet open.");
                    var State = layero.find('iframe').contents().find('#txtState').val();
                    if (State == 'True')
                        location.reload();

                    return false;
                }
                , btn4: function (index, layero) { //保存草稿
                    //草稿功能还没开放所以隐藏，只是存放了备用按钮留作今后使用

                    layer.msg("草稿功能还没有开放。<br/>This feature is not yet available.");
                    var State = layero.find('iframe').contents().find('#txtState').val();
                    if (State == 'True')
                        location.reload();

                    return false;
                }
                , btn5: function (index, layero) { //提交
                    ////表单处于可修改状态(表单未提交或草稿时且动作为Add时)
                    //layer.msg("提交功能还没有开放。<br/>This feature is not yet available.");

                    var iframeWindow = window['layui-layer-iframe' + index]
                        , submitID = 'btnSave'
                        , submit = layero.find('iframe').contents().find('#' + submitID);

                    $(submit).click();
                    return false;
                }
                , btn6: function (index, layero) {//修改
                    layer.open({
                        type: 2
                        , title: '修改表单《' + formNumber + '》'
                        , content: micro.getAjaxData('json', micro.getRootPath() + '/App_Handler/GetFormLinkAddress.ashx', parm)["LinkAddress"] + '/Modify/' + shortTableName + '/' + moduleID + '/' + formID + '/' + formsID
                        , area: ['95%', '90%']
                        , btn: ['保存【Save】', '关闭【Close】']
                        , skin: 'micro-layer-class'
                        , yes: function (index, layero) {  //按钮一
                            var iframeWindow = window['layui-layer-iframe' + index]
                                , submitID = 'btnModify'
                                , submit = layero.find('iframe').contents().find('#' + submitID);

                            $(submit).click();
                        }
                        , cancel: function (index, layero) {  //右上角关闭
                            var State = layero.find('iframe').contents().find('#txtState').val();
                            if (State === 'True') {
                                layer.closeAll('iframe');
                                mGet.layerOpen(shortTableName, moduleID, formID, formsID, formNumber);
                                $('#txtState').val('False');
                                $(window.parent.document).find('#txtState').val('False');
                            }
                        }
                        , end: function () {
                            var State = $('#txtState').val();
                            if (State === 'True') {
                                layer.closeAll('iframe');
                                mGet.layerOpen(shortTableName, moduleID, formID, formsID, formNumber);
                                $('#txtState').val('False');
                                $(window.parent.document).find('#txtState').val('False');
                            }
                        }
                    });
                    return false;
                }
                , btn7: function (index, layero) { //撤回
                    //显示撤回的条件是申请者提交了申请，而第一位审批者还没有进行审批
                    //layer.msg("撤回功能还没有开放。<br/>This feature is not yet available.");
                    //var State = layero.find('iframe').contents().find('#txtState').val();
                    //if (State == 'True')
                    //    location.reload();

                    layer.open({
                        type: 2
                        , title: '撤回表单《' + formNumber + '》'
                        , content: '/Views/Forms/MicroFormWithdrawal/withdrawal/' + shortTableName + '/' + moduleID + '/' + formID + '/' + formsID
                        , area: ['56%', '62%']
                        , btn: ['保存【Save】', '关闭【Close】']
                        , skin: 'micro-layer-class'
                        , yes: function (index, layero) {  //按钮一
                            var iframeWindow = window['layui-layer-iframe' + index]
                                , submitID = 'btnSave'
                                , submit = layero.find('iframe').contents().find('#' + submitID);

                            $(submit).click();
                        }
                        , cancel: function (index, layero) {  //右上角关闭
                            var State = layero.find('iframe').contents().find('#txtState').val();
                            if (State === 'True') {
                                layer.closeAll('iframe');
                                mGet.layerOpen(shortTableName, moduleID, formID, formsID, formNumber);
                                $('#txtState').val('False');
                                $(window.parent.document).find('#txtState').val('False');
                            }
                        }
                        , end: function () {
                            var State = $('#txtState').val();
                            if (State === 'True') {
                                layer.closeAll('iframe');
                                mGet.layerOpen(shortTableName, moduleID, formID, formsID, formNumber);
                                $('#txtState').val('False');
                                $(window.parent.document).find('#txtState').val('False');
                            }
                        }
                    });

                    return false;
                }
                , btn8: function (index, layero) { //打印

                    layer.msg("该功能还没有开放。<br/>This feature is not yet available.");
                    var State = layero.find('iframe').contents().find('#txtState').val();
                    if (State == 'True')
                        location.reload();

                    return false;
                }
                , btn9: function (index, layero) { //受理
                    var flag = micro.getAjaxData('text', micro.getRootPath() + '/Views/Forms/MicroFormAccept.ashx', parm);
                    if (flag.substring(0, 4) === 'True') {
                        msg = layer.msg(flag.substring(4, flag.length), {
                            icon: 1
                            , anim: 5
                            , offset: 't'
                        }, function () {
                            layer.closeAll('iframe');
                            mGet.layerOpen(shortTableName, moduleID, formID, formsID, formNumber);
                        });
                    } else {
                        msg = layer.msg(flag, {
                            icon: 5
                            , anim: 6
                            , offset: 't'
                        });
                    }
                    return false;
                }
                , btn10: function (index, layero) { //结案

                    layer.open({
                        type: 2
                        , title: '表单结案《' + formNumber + '》'
                        , content: micro.getAjaxData('json', micro.getRootPath() + '/App_Handler/GetFormLinkAddress.ashx', parm)["LinkAddress"] + '/Close/' + shortTableName + '/' + moduleID + '/' + formID + '/' + formsID
                        , area: ['95%', '90%']
                        , btn: ['保存【Save】', '关闭【Close】']
                        , skin: 'micro-layer-class'
                        , yes: function (index, layero) {  //按钮一
                            var iframeWindow = window['layui-layer-iframe' + index]
                                , submitID = 'btnClose'
                                , submit = layero.find('iframe').contents().find('#' + submitID);

                            $(submit).click();
                        }
                        , cancel: function (index, layero) {  //右上角关闭
                            var State = layero.find('iframe').contents().find('#txtState').val();
                            if (State === 'True') {
                                layer.closeAll('iframe');
                                mGet.layerOpen(shortTableName, moduleID, formID, formsID, formNumber);
                                $('#txtState').val('False');
                                $(window.parent.document).find('#txtState').val('False');
                            }
                        }
                        , end: function () {
                            var State = $('#txtState').val();
                            if (State === 'True') {
                                layer.closeAll('iframe');
                                mGet.layerOpen(shortTableName, moduleID, formID, formsID, formNumber);
                                $('#txtState').val('False');
                                $(window.parent.document).find('#txtState').val('False');
                            }
                        }
                    });

                    return false;
                }
                , btn11: function (index, layero) { //删除

                    layer.prompt({
                        formType: 1
                        , title: '敏感操作，请输入您的Windows登录密码或本系统登录密码'
                    }, function (value, index) {
                        layer.close(index);

                        if (micro.verifyPWD(value) === 'True') {
                            layer.confirm('确定删除吗？', function (index) {
                                var Parameter = { "action": "Del", "type": "MicroForm", "stn": shortTableName, "mid": moduleID, "formid": formID, "pkv": formsID };
                                micro.mAjax('text', micro.getRootPath() + '/App_Handler/CtrlPublicGenService.ashx', Parameter, true, true);
                            });
                        }
                    });

                    return false;
                }
                , cancel: function (index, layero) {  //右上角关闭
                    var State = layero.find('iframe').contents().find('#txtState').val();
                    if (State == 'True')
                        location.reload();
                }
            });
        }

    };

    mGet.ViewPublicFormList(Action, Type, MID);

    //监听工具条
    table.on('tool(tabTable)', function (obj) {
        var data = obj.data;
        if (obj.event === 'View')
            mGet.layerOpen(data.ShortTableName, data.ModuleID, data.FormID, data.FormsID, data.FormNumber);
    });

    $('.micro-message-btns .layui-btn').on('click', function () {
        var type = $(this).data('type');
        mGet[type] ? mGet[type].call(this) : '';
    });
});