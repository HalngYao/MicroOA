layui.use(['index', 'jquery', 'form', 'table', 'micro'], function () {
    var $ = layui.$
        , form = layui.form
        , table = layui.table
        , micro = layui.micro;

    var MID = $('#txtMID').val();
    var FormID = $('#txtFormID').val();
    var ShortTableName = $('#txtShortTableName').val();
    var PrimaryKeyName = $('#txtPrimaryKeyName').val();

    //生成数据表
    micro.getTableAttributes('View', ShortTableName, PrimaryKeyName, 'UID:Int');

    //事件
    var mGet = {

        //统计未读数量
        getUnreadyTotal: function () {
            var parm = { "action": "UnreadyTotal", "stn": ShortTableName, "mid": MID };
            var flag = micro.getAjaxData('text', micro.getRootPath() + '/Views/UserCenter/Message.ashx', parm, false);
            if (flag > 0) {
                $('#spanNotice').html('<span class="layui-badge">' + flag + '</span>');

                if (flag < 100)
                    $('#spanNotice', window.parent.document).html('<span class="layui-badge">' + flag + '</span>');

            } else {
                $('#spanNotice').html('');

                if (flag < 100)
                    $('#spanNotice', window.parent.document).html('');
            }
        },

        //单条记录点击时设计为已读颜色状态
        setRowColor: function (obj) {
            obj.addClass('ws-msg');
            var index =obj.attr('data-index');
                $('.layui-table-fixed .layui-table-body table tr[data-index="' + index + '"] .ws-icon').removeClass('icon-biaoshilei_weiduxinxi ws-font-red').addClass('icon-yiduxiaoxi ws-font-size20 ws-font-gray2');
        },

        btnReady: function () {
            var checkStatus = table.checkStatus('tabNotice')
                , checkData = checkStatus.data; //得到选中的数据

            if (checkData.length === 0)
                return layer.msg('没有选中任何消息。<br/>メッセージは何も選択されていません。<br/>No messages are selected.');
            else {
                var parm = { "action": "Ready", "stn": ShortTableName, "mid": MID, "fields": encodeURI(JSON.stringify(checkData)) };
                micro.mAjax('text', micro.getRootPath() + '/Views/UserCenter/Message.ashx', parm, false, true);
                mGet.getUnreadyTotal();
            }

        },

        btnUnready: function () {
            var checkStatus = table.checkStatus('tabNotice')
                , checkData = checkStatus.data; //得到选中的数据

            if (checkData.length === 0)
                return layer.msg('没有选中任何消息。<br/>メッセージは何も選択されていません。<br/>No messages are selected.');
            else {
                var parm = { "action": "Unready", "stn": ShortTableName, "mid": MID, "fields": encodeURI(JSON.stringify(checkData)) };
                micro.mAjax('text', micro.getRootPath() + '/Views/UserCenter/Message.ashx', parm, false, true);
                mGet.getUnreadyTotal();
            }

        },

        btnReadyAll: function () {
            layer.confirm('确定吗？<br/>確かですか？<br/>Are you sure?', function (index) {
                var parm = { "action": "ReadyAll", "stn": ShortTableName, "mid": MID };
                micro.mAjax('text', micro.getRootPath() + '/Views/UserCenter/Message.ashx', parm, false, true);
                mGet.getUnreadyTotal();
            });
        },

        btnDelSelected: function () {
            var checkStatus = table.checkStatus('tabNotice')
                , checkData = checkStatus.data; //得到选中的数据

            if (checkData.length === 0)
                return layer.msg('没有选中任何消息。<br/>メッセージは何も選択されていません。<br/>No messages are selected.');

            else {
                layer.prompt({
                    formType: 1
                    , title: '敏感操作，请输入您的Windows登录密码或本系统登录密码'
                }, function (value, index) {
                    layer.close(index);
                    layer.confirm('确定删除吗？<br/>削除しますか？<br/>Are you sure to delete?', function (index) {
                        var parm = { "action": "DelSelected", "stn": ShortTableName, "mid": MID, "v": value, "fields": encodeURI(JSON.stringify(checkData)) };
                        micro.mAjax('text', micro.getRootPath() + '/Views/UserCenter/Message.ashx', parm, false, true);
                        mGet.getUnreadyTotal();
                    });
                });
            }
        },

        btnDelReady: function () {
                layer.prompt({
                    formType: 1
                    , title: '敏感操作，请输入您的Windows登录密码或本系统登录密码'
                }, function (value, index) {
                    layer.close(index);
                    layer.confirm('确定删除吗？<br/>削除しますか？<br/>Are you sure to delete?', function (index) {
                        var parm = { "action": "DelReady", "stn": ShortTableName, "mid": MID, "v": value, "fields": "DelReady" };
                        micro.mAjax('text', micro.getRootPath() + '/Views/UserCenter/Message.ashx', parm, false, true);
                        mGet.getUnreadyTotal();
                    });
                });        
        },

        btnDelAll: function () {
            layer.prompt({
                formType: 1
                , title: '敏感操作，请输入您的Windows登录密码或本系统登录密码'
            }, function (value, index) {
                layer.close(index);
                layer.confirm('确定删除吗？<br/>削除しますか？<br/>Are you sure to delete?', function (index) {
                    var parm = { "action": "DelAll", "stn": ShortTableName, "mid": MID, "v": value, "fields": "DelAll" };
                    micro.mAjax('text', micro.getRootPath() + '/Views/UserCenter/Message.ashx', parm, false, true);
                    mGet.getUnreadyTotal();
                });
            });
        },

        layerOpen: function (shortTableName, noticeID, moduleID, formID, formsID, formNumber) {

            //防止页面回发
            $(window.parent.document).find('#txtState').val('False');
            $('#txtState').val('False');

            //点击通知标记为已读
            var parm = { "action": "Single", "stn": shortTableName, "mid": MID, "noticid": noticeID }
            micro.mAjaxNoMsg('text', '/Views/UserCenter/Message.ashx', parm);

            var parameter = { "action": "View", "stn": shortTableName, "mid": moduleID, "formid": formID, "pkv": formsID }
            layer.open({
                type: 2
                , title: '《' + formNumber + '》申请单的详情 / Details of the application form.'
                , content: micro.getAjaxData('json', micro.getRootPath() + '/App_Handler/GetFormLinkAddress.ashx', parameter)["LinkAddress"] + '/View/' + shortTableName + '/' + moduleID + '/' + formID + '/' + formsID
                , area: ['95%', '90%']
                , btn: ['同意【Agree】', '驳回【Return】', '转发【Forward】', '草稿【Draft】', '提交【Submit】', '修改【Modify】', '撤回【Withdrawal】', '打印【Print】', '受理【Accept】', '结案【Case closed】', '删除【Delete】', '关闭【Close】']
                , skin: micro.getAjaxData('json', micro.getRootPath() + '/Views/Forms/MicroFormCheckApproval.ashx', parameter)["Class"] + ''
                , yes: function (index, layero) {  //同意
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
                                mGet.layerOpen(shortTableName, noticeID, moduleID, formID, formsID, formNumber);
                            }
                        }
                        , end: function () {
                            var State = $('#txtState').val();
                            if (State === 'True') {
                                layer.closeAll('iframe');
                                mGet.layerOpen(shortTableName, noticeID, moduleID, formID, formsID, formNumber);
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
                                mGet.layerOpen(shortTableName, noticeID, moduleID, formID, formsID, formNumber);
                            }
                        }
                        , end: function () {
                            var State = $('#txtState').val();
                            if (State === 'True') {
                                layer.closeAll('iframe');
                                mGet.layerOpen(shortTableName, noticeID, moduleID, formID, formsID, formNumber);
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
                , btn5: function (index, layero) { //按钮四 提交

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
                        , content: micro.getAjaxData('json', micro.getRootPath() + '/App_Handler/GetFormLinkAddress.ashx', parameter)["LinkAddress"] + '/Modify/' + shortTableName + '/' + moduleID + '/' + formID + '/' + formsID
                        , area: ['95%', '90%']
                        , btn: ['修改提交【ModifySubmit】', '关闭【Close】']
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
                                mGet.layerOpen(shortTableName, noticeID, moduleID, formID, formsID, formNumber);
                            }
                        }
                        , end: function () {
                            var State = $('#txtState').val();
                            if (State === 'True') {
                                layer.closeAll('iframe');
                                mGet.layerOpen(shortTableName, noticeID, moduleID, formID, formsID, formNumber);
                            }
                        }
                    });
                    return false;
                }
                , btn7: function (index, layero) { //撤回

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
                                mGet.layerOpen(action, shortTableName, formID, formsID, formNumber);
                                $('#txtState').val('False');
                                $(window.parent.document).find('#txtState').val('False');
                            }
                        }
                        , end: function () {
                            var State = $('#txtState').val();
                            if (State === 'True') {
                                layer.closeAll('iframe');
                                mGet.layerOpen(action, shortTableName, formID, formsID, formNumber);
                                $('#txtState').val('False');
                                $(window.parent.document).find('#txtState').val('False');
                            }
                        }
                    });

                    return false;
                }
                , btn8: function (index, layero) {//打印

                    layer.msg("该功能还没有开放。<br/>This feature is not yet available.");
                    var State = layero.find('iframe').contents().find('#txtState').val();
                    if (State == 'True')
                        location.reload();

                    return false;
                }
                , btn9: function (index, layero) { //受理
                    var flag = micro.getAjaxData('text', micro.getRootPath() + '/Views/Forms/MicroFormAccept.ashx', parameter);
                    if (flag.substring(0, 4) === 'True') {
                        msg = layer.msg(flag.substring(4, flag.length), {
                            icon: 1
                            , anim: 5
                            , offset: 't'
                        }, function () {
                            layer.closeAll('iframe');
                                mGet.layerOpen(shortTableName, noticeID, moduleID, formID, formsID, formNumber);
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
                , btn10: function (index, layero) {//结案

                    layer.open({
                        type: 2
                        , title: '表单结案《' + formNumber + '》'
                        , content: micro.getAjaxData('json', micro.getRootPath() + '/App_Handler/GetFormLinkAddress.ashx', parameter)["LinkAddress"] + '/Close/' + shortTableName + '/' + moduleID + '/' + formID + '/' + formsID
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
                                mGet.layerOpen(shortTableName, noticeID, moduleID, formID, formsID, formNumber);
                                $('#txtState').val('False');
                                $(window.parent.document).find('#txtState').val('False');
                            }
                        }
                        , end: function () {
                            var State = $('#txtState').val();
                            if (State === 'True') {
                                layer.closeAll('iframe');
                                mGet.layerOpen(shortTableName, noticeID, moduleID, formID, formsID, formNumber);
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
                            layer.confirm('确定删除吗？<br/>削除しますか？<br/>Are you sure to delete?', function (index) {
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

    //监听工具条
    table.on('tool(tabNotice)', function (obj) {
        var data = obj.data;
        if (obj.event === 'View') {
            mGet.layerOpen(data.ShortTableName, data.NoticeID, data.ModuleID, data.FormID, data.FormsID, data.FormNumber);
            mGet.setRowColor(obj.tr);
            mGet.getUnreadyTotal();
        }
    });

    $('.micro-message-btns .layui-btn').on('click', function () {
        var type = $(this).data('type');
        mGet[type] ? mGet[type].call(this) : '';
    });
});