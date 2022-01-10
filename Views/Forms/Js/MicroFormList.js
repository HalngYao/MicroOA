layui.use(['index', 'jquery', 'form', 'table', 'laydate', 'micro'], function () {
    var $ = layui.$
        , form = layui.form
        , table = layui.table
        , laydate = layui.laydate
        , micro = layui.micro;

    var Action = $('#txtAction').val();
    var MID = $('#txtMID').val();
    var FormID = $('#txtFormID').val();
    var ShortTableName = $('#txtShortTableName').val();
    var PrimaryKeyName = $('#txtPrimaryKeyName').val();
    var PrimaryKeyValue = $('#txtPrimaryKeyValue').val();
    var FormNumber = $('#txtFormNumber').val();
    var QueryType = $('#selQueryItem').val();
    var DataURL = $('#txtDataURL').val();  //指定数据表的URL
    var DataType = $('#txtDataType').val();  //指定数据类型
    var StartDate = $('#txtStartDate').val();  //指定数据类型
    var EndDate = $('#txtEndDate').val();  //指定数据类型
    var Keyword = $('#txtKeyword').val();

    //生成数据表
    micro.getTableAttributes(Action, ShortTableName, Keyword, QueryType, MID, FormID, DataURL, DataType, StartDate, EndDate);

    //事件
    var mGet = {

        //开始日期
        getStartDate: function () {

            laydate.render({
                elem: '#txtStartDate',
                eventElem: '#icondatetxtStartDate',
                trigger: 'click'
                , min: '2020-01-01'
                , done: function (value, date, endDate) {
                    mGet.getEndDate(value);
                }
            });

        },

        //结束日期
        getEndDate: function (minDate) {
            $('#txtEndDate')[0].eventHandler = false;
            oldEndDate = $('#txtEndDate').val();
            currDate = micro.checkBothDate(minDate, oldEndDate) ? minDate : oldEndDate;

            laydate.render({
                elem: '#txtEndDate',
                eventElem: '#icondatetxtEndDate',
                trigger: 'click'
                , min: minDate
                , value: currDate
                , btns: ['clear', 'confirm']
            });

        },

        //查询
        btnSearch: function () {

            var queryType = $('#selQueryItem').val(),
                startDate = $('#txtStartDate').val(),
                endDate = $('#txtEndDate').val(),
                keyword = $('#txtKeyword').val();

            micro.getTableAttributes(Action, ShortTableName, keyword, queryType, MID, FormID, DataURL, DataType, startDate, endDate);

        },

        //删除
        btnDel: function () {
            var checkStatus = table.checkStatus('tabTable')
                , checkData = checkStatus.data; //得到选中的数据

            if (checkData.length === 0) {
                return layer.msg('请选择数据');
            }

            layer.prompt({
                formType: 1
                , title: '敏感操作，请验证口令'
            }, function (value, index) {
                layer.close(index);

                layer.confirm('确定删除吗？', function (index) {

                    //执行 Ajax 后重载
                    /*
                    admin.req({
                      url: 'xxx'
                      //,……
                    });
                    */
                    table.reload('tabTable');
                    layer.msg('已删除');
                });
            });
        }

        , btnAddOpenLink: function () {
            var linkAddress = $('#txtLinkAddress').val();
            linkAddress = linkAddress == '' ? '/Views/Forms/MicroForm' : linkAddress;
            layer.open({
                type: 2
                , title: '添加'
                , content: linkAddress + '/add/' + ShortTableName + '/' + MID + '/' + FormID
                , maxmin: true
                , area: ['95%', '90%']
                , btn: ['立即提交', '关闭']
                , skin: 'micro-layer-class'
                , yes: function (index, layero) {
                    var iframeWindow = window['layui-layer-iframe' + index]
                        , submitID = 'btnSave'
                        , submit = layero.find('iframe').contents().find('#' + submitID);

                    $(submit).click();
                }
                , btn2: function (index, layero) {
                    var State = layero.find('iframe').contents().find('#txtState').val();
                    var GID = $(window.parent.document).find('#txtGID').val();
                    if (State == "True")
                        location.reload();
                }
                , cancel: function (index, layero) {
                    var State = layero.find('iframe').contents().find('#txtState').val();
                    var GID = $(window.parent.document).find('#txtGID').val();
                    if (State == "True")
                        location.reload();
                }
                , end: function () {
                    var State = $('#txtState').val();
                    var GID = $(window.parent.document).find('#txtGID').val();
                    if (State === 'True') {
                        if (GID == "")
                            location.reload();

                        $('#txtState').val('False');
                        $(window.parent.document).find('#txtState').val('False');
                    }
                }
            });
        },

        layerOpen: function (action, shortTableName, formID, formsID, formNumber) {
            var linkAddress = $('#txtLinkAddress').val();
            // /Views/Forms/MicroForm   //默认
            // /Views/Forms/HR/OvertimeFormList   //自定义
            var parm = { "action": "" + action + "", "stn": shortTableName, "mid": MID, "formid": formID, "pkv": formsID };
            layer.open({
                type: 2
                , title: '《' + formNumber + '》申请单的详情 / Details of the application form.'
                , content: linkAddress + '/' + action + '/' + shortTableName + '/' + MID + '/' + formID + '/' + formsID
                , area: ['95%', '90%']
                , btn: ['同意【Agree】', '驳回【Return】', '转发【Forward】', '草稿【Draft】', '提交【Submit】', '修改【Modify】', '撤回【Withdrawal】', '打印【Print】', '受理【Accept】', '结案【Case closed】', '删除【Delete】', '关闭【Close】']
                , skin: micro.getAjaxData('json', micro.getRootPath() + '/Views/Forms/MicroFormCheckApproval.ashx', parm)["Class"]  //'micro-layer-class2'
                , yes: function (index, layero) {  //按钮一 同意
                    layer.open({
                        type: 2
                        , title: '审批表单《' + formNumber + '》'
                        , content: '/Views/Forms/MicroFormApproval/Agree/' + shortTableName + '/' + MID + '/' + formID + '/' + formsID
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
                }
                , btn2: function (index, layero) {//驳回
                    layer.open({
                        type: 2
                        , title: '审批表单《' + formNumber + '》'
                        , content: '/Views/Forms/MicroFormApproval/Return/' + shortTableName + '/' + MID + '/' + formID + '/' + formsID
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
                    //表单处于可修改状态(表单被驳回且动作为Modify时)
                    layer.open({
                        type: 2
                        , title: '修改表单《' + formNumber + '》'
                        , content: linkAddress + '/Modify/' + shortTableName + '/' + MID + '/' + formID + '/' + formsID
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
                , btn7: function (index, layero) { //撤回
                    layer.open({
                        type: 2
                        , title: '撤回表单《' + formNumber + '》'
                        , content: '/Views/Forms/MicroFormWithdrawal/Withdrawal/' + shortTableName + '/' + MID + '/' + formID + '/' + formsID
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
                , btn8: function (index, layero) { //打印

                    layer.msg("打印功能还没有开放。<br/>This feature is not yet available.");
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
                            mGet.layerOpen(action, shortTableName, formID, formsID, formNumber);
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
                        , content: linkAddress + '/Close/' + shortTableName + '/' + MID + '/' + formID + '/' + formsID
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
                , btn11: function (index, layero) { //删除
                    layer.prompt({
                        formType: 1
                        , title: '敏感操作，请输入您的Windows登录密码或本系统登录密码'
                    }, function (value, index) {
                        layer.close(index);

                        if (micro.verifyPWD(value) === 'True') {
                            layer.confirm('确定删除吗？<br/>削除しますか？<br/>Are you sure to delete?', function (index) {
                                var Parameter = { "action": "Del", "type": "MicroForm", "stn": shortTableName, "mid": MID, "formid": formID, "pkv": formsID };
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
                , end: function () {
                    var State = $('#txtState').val();
                    if (State === 'True') {
                        location.reload();
                        $('#txtState').val('False');
                        $(window.parent.document).find('#txtState').val('False');
                    }
                }

            });
        }

        //批量审批
        , btnBatchApproval: function () { //获取选中数据
            var checkStatus = table.checkStatus('tabTable')
                , data = checkStatus.data;

            //layer.alert(JSON.stringify(data));
            if (data.length == 0)
                layer.msg('请选中需要审批的记录<br/>承認が必要な記録を選択してください<br/>Please select the record to be approved');
            else {

                var action = $(this).attr('micro-data');

                if (action == 'BatchAgree' || action == 'BatchReturn') {
                    var formsIDs = micro.getStrSplice(data, ShortTableName.toLowerCase() == 'overtime' ? 'ParentID' : PrimaryKeyName, ',', false),
                        Parameter = { "action": action, "stn": ShortTableName, "mid": MID, "formid": FormID, "formsids": formsIDs },
                        flag = micro.getAjaxData('text', '/Views/Forms/BeforeBatchApprovalCheck.ashx', Parameter, false);

                    if (flag.substring(0, 4) === 'True') {

                        layer.open({
                            type: 2
                            , title: '批量审批'
                            , content: '/Views/Forms/MicroFormApproval/' + action + '/' + ShortTableName + '/' + MID + '/' + FormID
                            , area: ['95%', '90%']
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
                                    location.reload();
                                }
                            }
                            , end: function () {
                                var State = $('#txtState').val();
                                if (State === 'True') {
                                    layer.closeAll('iframe');
                                    location.reload();
                                }
                            }
                        });

                    } else {

                        layer.msg(flag.substring(4, flag.length), {
                            icon: 1
                            , anim: 5
                            , offset: 't'
                            , skin: 'ws-margin-top10'
                        }, function () {

                        });
                    }
                }
            }
        }
    };

    mGet.getStartDate();
    mGet.getEndDate($('#txtStartDate').val());

    //监听工具条
    table.on('tool(tabTable)', function (obj) {
        var data = obj.data;
        if (obj.event === 'View') {
            //alert(data.ParentID);
            var shortTableName = $('#txtShortTableName').val();
            var stateCode = parseInt(data.StateCode),
                action = 'View';

            action = stateCode == -3 ? 'Add' : 'View';

            var formsID = eval('data.' + PrimaryKeyName);
            if (data.ParentID != undefined && data.ParentID != 0)
                formsID = data.ParentID;  //如果是子记录时把ParentID作为FormsID

            mGet.layerOpen(action, shortTableName, data.FormID, formsID, data.FormNumber);
            
        }
    });


    if (ShortTableName.toLowerCase() == 'overtime') {

        //相同表单号同时选中
        table.on('checkbox(tabTable)', function (obj) {
            if (obj.type == 'one') {
                var v = obj.data.FormNumber,
                    d = table.cache.tabTable;

                $('.layui-table-fixed .layui-table-body table tr').each(function () {
                    var v2 = $(this).children().eq(2).text();

                    if (obj.checked) {
                        if (v.trim() == v2.trim()) {
                            var index = $(this).attr('data-index');
                            d[index]["LAY_CHECKED"] = true;
                            $('.layui-table-fixed .layui-table-body table tr[data-index="' + index + '"] input[type="checkbox"]').prop('checked', true);
                            $('.layui-table-fixed .layui-table-body table tr[data-index="' + index + '"] input[type="checkbox"]').next('.layui-form-checkbox').addClass('layui-form-checked');
                        }
                    } else {
                        if (v.trim() == v2.trim()) {
                            var index = $(this).attr('data-index');
                            d[index]["LAY_CHECKED"] = false;
                            $('.layui-table-fixed .layui-table-body table tr[data-index="' + index + '"] input[type="checkbox"]').prop('checked', false);
                            $('.layui-table-fixed .layui-table-body table tr[data-index="' + index + '"] input[type="checkbox"]').next('.layui-form-checkbox').removeClass('layui-form-checked');
                        }
                    }

                });
            }

        });

        //触发排序，超出允许加班最大值时标注背景为红色
        table.on('sort(tabTable)', function (obj) {
            $('.layui-table-body table tr').each(function () {

                var otExDXTotal = parseFloat($(this).children().eq(18).text()),
                    warningValue = parseFloat($(this).children().eq(19).text());

                if (!isNaN(otExDXTotal) && !isNaN(warningValue) && otExDXTotal > warningValue) {
                    $(this).children().toggleClass("ws-bg-warning");
                }
            });
        });

    }


    $('.layui-btn').on('click', function () {
        var type = $(this).data('type');
        mGet[type] ? mGet[type].call(this) : '';
    });

    //外部链接进入
    if (PrimaryKeyValue !== '' && FormNumber !== '' && FormNumber !=='DefaultNumber') {
        mGet.layerOpen('View', ShortTableName, FormID, PrimaryKeyValue, FormNumber);
    };
});