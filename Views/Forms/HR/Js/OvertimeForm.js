layui.use(['index', 'jquery', 'table', 'form', 'layer', 'laydate', 'micro'], function () {
    var $ = layui.$
        , table = layui.table
        , form = layui.form
        , layer = layui.layer
        , laydate = layui.laydate
        , micro = layui.micro;

    var Action = $("#txtAction").val();
    var MID = $("#txtMID").val();
    var STN = $("#txtShortTableName").val();
    var FormID = $("#txtFormID").val();
    var FormsID = $("#txtFormsID").val();
    var EditTablePermit = $('#txtEditTablePermit').val();
    var Edit = EditTablePermit === '1' ? 'text' : '';

    var mGet = {
        mAjax: function (DataType, URL, Parameter, isLoad, isRefresh, isCloseParent) {
            var flag = 'False', msg = "";
            isLoad = isLoad || true;  //isLoad = typeof isLoad === 'undefined' ? true : isLoad;
            isRefresh = isRefresh || false;
            isCloseParent = isCloseParent || false;

            $.ajax({
                async: true,
                type: 'post',
                dataType: DataType,
                url: URL,
                data: Parameter,
                beforeSend: function () {
                    if (isLoad)
                        layer.load(2);

                    //防止重新提交表单
                    if ($("#btnSave").length > 0)
                        $("#btnSave").prop('disabled', true);
                },
                success: function (v) {
                    //加载层
                    if (isLoad)
                        layer.close(layer.load(2));

                    if (v.State === 'True') {
                        $(window.parent.document).find('#txtState').val('True'); 
                        $(window.parent.document).find('#txtGID').val(v.FormsID); 
                        flag = v.Msg;
                        msg = layer.msg(flag.substring(4, flag.length), {
                            icon: 1
                            , anim: 5
                            , offset: 't'
                            , skin: 'ws-margin-top10'
                        }, function () {  
                            if (isCloseParent) {
                                //当你在iframe页面关闭自身时
                                var index = parent.layer.getFrameIndex(window.name); //先得到当前iframe层的索引
                                parent.layer.close(index); //再执行关闭 
                            }

                            if (isRefresh) //层销毁是否刷新页面
                                location.reload();
                        });
                    } else {

                        //如果保存失败解锁防止重新提交表单
                        if ($("#btnSave").length > 0)
                            $("#btnSave").prop('disabled', false);

                        msg = layer.msg(v.Msg, {
                            icon: 5
                            , anim: 6
                            , offset: 't'
                        }, function () {
                            //return false;
                        });
                    }
                    return msg;
                },
                error: function () {
                    if (isLoad)
                        layer.close(layer.load(2));

                    //如果表单提交失败解锁防止重新提交表单
                    if ($("#btnSave").length > 0)
                        $("#btnSave").prop('disabled', false);

                    flag = "error,出错了";
                    msg = layer.msg(flag, {
                        icon: 5
                        , anim: 6
                        , offset: 't'
                    }, function () {
                        //return false;
                    });
                    return msg;
                }
            });
        },

        //加载加班开始时间
        OvertimeStartTime: function (overtimeDate, shiftTypeID, startTime, endTime) {
            startTime = startTime || '00:00:00';
            endTime = endTime || '23:59:59';

            var parameter = { "overtimedate": overtimeDate, "shifttypeid": shiftTypeID };
            $.ajax({
                async: false,
                type: 'post',
                dataType: 'text',
                url: '/Views/Forms/HR/GetOTStartTime.ashx',
                data: parameter,
                beforeSend: function () {

                },
                success: function (d) {
                    var oldValue = $('#txtStartTime').val();
                    //关闭上次时间渲染
                    $('#txtStartTime')[0].eventHandler = false;
                    laydate.render({
                        elem: '#txtStartTime'
                        , btns: ['clear', 'confirm']
                        , type: 'time'
                        //, range: true
                        , trigger: 'click'
                        , format: 'HH:mm'
                        , value: Action.toLowerCase() == 'modify' ? oldValue : d
                        , min: startTime
                        , max: endTime
                        //只保留00和30 ss
                        , ready: function (date) {
                            var dom = $(".laydate-time-list").children("li");
                            for (var i = 0; i < dom.length; i++) {
                                if (i == 2 || i == 5) {
                                    $(dom[i]).remove();
                                }
                                else if (i == 1 || i == 4) {
                                    var li = $(dom[i]).children("ol").children("li")
                                    for (var j = 0; j < li.length; j++) {
                                        if ($(li[j]).text() != 00 && $(li[j]).text() != 5 && $(li[j]).text() != 15 && $(li[j]).text() != 30 && $(li[j]).text() != 40 && $(li[j]).text() != 45 && $(li[j]).text() != 55) {
                                            $(li[j]).remove();
                                        }
                                    }
                                }
                            }

                        }
                    });
                },
                error: function () {
                    alert("error,出错了")
                }
            });
        },

        //加载加班小时/持续时间
        OvertimeHour: function (formID, overtimeDate, shiftTypeID, overtimeUID) {
            overtimeUID = overtimeUID || $('#xmByUserDefVal').val();
            var elemID = 'selOTHour',
                url = '/Views/Forms/HR/GetOvertimeMax.ashx',
                defauleVal = $('#txtOTHour').val();

            micro.getSelectOption(elemID, url, formID, overtimeDate, shiftTypeID, overtimeUID, defauleVal);
        }
    };


    //修改状态下设置一些已知值
    if (Action.toLowerCase() == "add" || Action.toLowerCase() == "modify") {
        //修改状态下默认加载开始时间
        overtimeDate = $('#txtOvertimeDate').val();
        shiftTypeID = $('#raShiftTypeID').val();
        mGet.OvertimeStartTime(overtimeDate, shiftTypeID, '', '');  //修改状态下不加载开始时间

        if ($('#raLocationOptions').val() == 'Other') {
            $('#txtLocation').removeClass('layui-disabled');
            $('#txtLocation').prop("readonly", false);
        }
    }

    //加班地点选择(radio)
    form.on('radio(raLocationOptions)', function (data) {
        var v = $('.cssraLocationOptions input[type=radio]:checked').map(function (index, elem) { return $(elem).val(); }).get().join(',');
        $('#raLocationOptions').val(v);
        if (v != 'Other') {
            $('#txtLocation').val(v);
            $('#txtLocation').addClass('layui-disabled');
            $('#txtLocation').prop("readonly", true);
            $('#txtLocation').prop('placeholder', '请选择加班地点');
        }
        else {
            $('#txtLocation').val('');
            $('#txtLocation').removeClass('layui-disabled');
            $('#txtLocation').prop('readonly', false);
            $('#txtLocation').prop('placeholder', '选择其它时请填写加班地点');
            $('#txtLocation').attr('lay-reqtext', '选择其它时请填写加班地点<br/>Please fill in overtime location when selecting other');
        }
    });

    //加班地点选择(select)
    form.on('select(raLocationOptions)', function (data) {
        var v = data.value;
        $('#raLocationOptions').val(v);
        if (v != 'Other') {
            $('#txtLocation').val(v);
            $('#txtLocation').addClass('layui-disabled');
            $('#txtLocation').prop("readonly", true);
            $('#txtLocation').prop('placeholder', '请选择加班地点');
        }
        else {
            $('#txtLocation').val('');
            $('#txtLocation').removeClass('layui-disabled');
            $('#txtLocation').prop('readonly', false);
            $('#txtLocation').prop('placeholder', '选择其它时请填写加班地点');
            $('#txtLocation').attr('lay-reqtext', '选择其它时请填写加班地点<br/>Please fill in overtime location when selecting other');
        }
    });

    //选择班次
    form.on('radio(raShiftTypeID)', function (data) {

        var formID = $("#txtFormID").val(),
            shiftTypeID = data.value,
            overtimeDate = $('#txtOvertimeDate').val();

        $('#raShiftTypeID').val(shiftTypeID);
        //清空已有时间
        if (overtimeDate != '' && shiftTypeID != '') {
            //先清空持续时间
            $('#selOTHour').val('0');
            $('#selOTMin').val('0');
            //加载开始时间
            mGet.OvertimeStartTime(overtimeDate, shiftTypeID, '', '');
            //加载持续时间
            mGet.OvertimeHour(formID, overtimeDate, shiftTypeID);
        }
    });

    //加班日期
    laydate.render({
        elem: '#txtOvertimeDate',
        eventElem: '#icondatetxtOvertimeDate',
        trigger: 'click'
        //, value: new Date()
        , done: function (value, date, endDate) {
            var formID = $("#txtFormID").val(),
                shiftTypeID = $('#raShiftTypeID').val(),
                overtimeDate = $('#txtOvertimeDate').val();

            if (overtimeDate != '' && shiftTypeID != '') {
                //先清空持续时间
                $('#selOTHour').val('0');
                $('#selOTMin').val('0');
                //加载开始时间
                mGet.OvertimeStartTime(value, shiftTypeID, '', '');
                //加载持续时间
                mGet.OvertimeHour(formID, value, shiftTypeID);
            }
        }
    });

    //监听开始时间
    $('#txtStartTime').on('click', function () {

        var shiftTypeID = $('#raShiftTypeID').val(),
            overtimeDate = $('#txtOvertimeDate').val();

        if (shiftTypeID == '') {
            layer.msg('请先选择班次', {
                offset: 't'
                , time: 1000
            });
        } else if (overtimeDate == '') {
            layer.msg('请选择加班日期', {
                offset: 't'
                , time: 1000
            });
        }

    });

    //加班小时监听
    form.on('select(selOTHour)', function (data) {
        var shiftTypeID = $('#raShiftTypeID').val(),
            overtimeDate = $('#txtOvertimeDate').val();

        if (shiftTypeID == '') {
            layer.msg('请先选择班次', {
                offset: 't'
                , time: 1000
            });
            $('#selOTHour').val('0');
            $('#selOTMin').val('0');
            form.render();
            return false;
        } else if (overtimeDate == '') {
            layer.msg('请先选择加班日期', {
                offset: 't'
                , time: 1000
            });
            $('#selOTHour').val('0');
            $('#selOTMin').val('0');
            form.render();
            return false;
        }
    });

    //加班分钟监听
    form.on('select(selOTMin)', function (data) {
        var shiftTypeID = $('#raShiftTypeID').val(),
            overtimeDate = $('#txtOvertimeDate').val();

        if (shiftTypeID == '') {
            layer.msg('请先选择班次', {
                offset: 't'
                , time: 1000
            });
            $('#selOTHour').val('0');
            $('#selOTMin').val('0');
            form.render();
            return false;
        } else if (overtimeDate == '') {
            layer.msg('请先选择加班日期', {
                offset: 't'
                , time: 1000
            });
            $('#selOTHour').val('0');
            $('#selOTMin').val('0');
            form.render();
            return false;
        }
    });

    //就餐选择
    form.on('checkbox(cbOvertimeMealID)', function (data) {
        var v = $('.csscbOvertimeMealID input[type=checkbox]:checked').map(function (index, elem) { return $(elem).val(); }).get().join(',');
        $('#cbOvertimeMealID').val(v);
    });


    //是否用车
    form.on('radio(raUseCar)', function (data) {
        $('#raUseCar').val(data.value);
    });

    //页面打开默认加载加班小时数
    mGet.OvertimeHour(FormID, $('#txtOvertimeDate').val(), '2');

    form.on('submit(btnSave)', function (data) {
        var action = $("#txtAction").val();
        var stn = $("#txtShortTableName").val();
        var mid = $("#txtMID").val();
        var formid = $("#txtFormID").val();
        var formsID = $("#txtFormsID").val();
        var isapprovalform = "False";

        var selOTHour = $('#selOTHour').val();
        var selOTMin = $('#selOTMin').val();

        var overtimeTypeCode = $('#txtOvertimeTypeCode').val();

        if (parseInt(selOTHour) === 0 && parseInt(selOTMin) === 0) {
            layer.msg("请选择加班小时数<br/>Please select overtime hours", {
                icon: 5
                , anim: 6
                //, offset: 't'
            }, function () {
                return false;
            });
            return false;
        }

        var Fields = JSON.stringify(data.field); Fields = Fields.replace(/ctl00\$ContentPlaceHolder1\$/g, "");
        Fields = encodeURI(Fields);

        var parameter = { "action": action, "stn": stn, "mid": mid, "formid": formid, "formsid": formsID, "isapprovalform": isapprovalform, "fields": Fields, "overtimetypecode": overtimeTypeCode };
        mGet.mAjax('json', '/Views/Forms/HR/CtrlOvertimeForm.ashx', parameter, true, true, true);

    });

    form.on('submit(btnModify)', function (data) {
        var action = $("#txtAction").val();
        var stn = $("#txtShortTableName").val();
        var mid = $("#txtMID").val();
        var formid = $("#txtFormID").val();
        var formsID = $("#txtFormsID").val();
        var overtimeIDs = $("#txtOvertimeIDs").val();
        var isapprovalform = "False";

        var selOTHour = $('#selOTHour').val();
        var selOTMin = $('#selOTMin').val();

        var overtimeTypeCode = $('#txtOvertimeTypeCode').val();

        if (parseInt(selOTHour) === 0 && parseInt(selOTMin) === 0) {
            layer.msg("请选择加班小时数<br/>Please select overtime hours", {
                icon: 5
                , anim: 6
                //, offset: 't'
            }, function () {
                return false;
            });
            return false;
        }

        var Fields = JSON.stringify(data.field); Fields = Fields.replace(/ctl00\$ContentPlaceHolder1\$/g, "");
        Fields = encodeURI(Fields);

        var parameter = { "action": action, "stn": stn, "mid": mid, "formid": formid, "formsid": formsID, "overtimeids": overtimeIDs, "isapprovalform": isapprovalform, "fields": Fields, "overtimetypecode": overtimeTypeCode };
        mGet.mAjax('json', '/Views/Forms/HR/CtrlOvertimeForm.ashx', parameter, true, true, true);

    });


});