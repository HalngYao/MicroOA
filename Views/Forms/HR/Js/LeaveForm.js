layui.use(['index', 'jquery', 'form', 'layer', 'laydate', 'micro'], function () {
    var $ = layui.$
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
    var LeaveUID = $('#xmByUserDefVal').val();
    var HolidayTypeID = $('#xmByHolidayTypeDefVal').val();

    $('#hidFormID').val($('#txtFormID').val());
    $('.ws-overtimedate').addClass('layui-hide');

    if ($('#divselHolidayTypeID').length > 0)
        if ($('#divselHolidayTypeID').text() == '代休假')
            $('.ws-overtimedate').removeClass('layui-hide');
    
    if ($('#divhidStartDateTime').length)
        $('div').removeClass('csshiddenlabel');

    var divselHolidayTypeID = $('#divselHolidayTypeID');
    if (divselHolidayTypeID.length > 0) {
        var HolidayType = divselHolidayTypeID.html().trim();
        if (HolidayType !== '代休假')
            $('#divtxtOvertimeDate').html('Null');

        if (HolidayType !== '年休假' && HolidayType !== '代休假')
            $('#divhidRemainingNumber').addClass('layui-hide');
        else
            $('#divhidRemainingNumber').removeClass('layui-hide');
    }

    //开始时间，结束时间(配合最天事件用)
    var StartTime, EndTime;

    var mGet = {

        //全天事件，把开始时间开结束时间设置为00:00
        setTime: function (isCheck) {
            var txtStartTime = $('#txtStartTime'),
                txtEndTime = $('#txtEndTime');

            if (isCheck) {
                StartTime = txtStartTime.val();
                EndTime = txtEndTime.val();

                txtStartTime.val('00:00');
                txtEndTime.val('00:00');

                txtStartTime.prop('disabled', 'disabled');
                txtStartTime.addClass('layui-disabled');

                txtEndTime.prop('disabled', 'disabled');
                txtEndTime.addClass('layui-disabled');
            }
            else {
                txtStartTime.val(StartTime);
                txtEndTime.val(EndTime);

                txtStartTime.prop('disabled', '');
                txtStartTime.removeClass('layui-disabled');

                txtEndTime.prop('disabled', '');
                txtEndTime.removeClass('layui-disabled');
            }
        },

        //比较两个日期的大小，传入两个日期如果date1>date2则返回true,否则返回false
        checkBothDate: function (date1, date2) {
            var oDate1 = new Date(date1);
            var oDate2 = new Date(date2);
            if (oDate1.getTime() > oDate2.getTime())
                return true;
            else
                return false;

        },

        //加班月份
        getOvertimeDate: function (MinOvertimeDate, MaxOvertimeDate) {

            if ($('#txtOvertimeDate').length > 0) {
                $('#txtOvertimeDate')[0].eventHandler = false;
                laydate.render({
                    elem: '#txtOvertimeDate', type: 'month', format: 'yyyy-MM', 
                    trigger: 'click',
                    min: MinOvertimeDate,
                    max: MaxOvertimeDate, 
                    btns: ['clear', 'confirm'],
                    ready: function (date) { $('.layui-laydate').off('click').on('click', '.laydate-month-list li', function () { $('.layui-laydate').remove(); }); },
                    change: function (value, date, endDate) {
                        $('#txtOvertimeDate').val(value);
                        var uid = XmSelectLeaveUID.getValue('valueStr'),
                            holidayTypeID = XmSelectHolidayTypeID.getValue('valueStr'),
                            overtimeDate = value,
                            holidayTypeName = XmSelectHolidayTypeID.getValue('nameStr');

                        mGet.getPersonalHoliday(uid, holidayTypeID, overtimeDate);
                        mGet.getLeaveTips(uid, holidayTypeID, overtimeDate, holidayTypeName);

                    }
                });
            }
        },

        //获取加班月份允许的选择范围
        getOvertimeDateRange: function (uid) {
            var url = '/Views/Forms/HR/GetLeaveOvertimeDate.ashx';
            $('#txtOvertimeDate').val('');
            $.getJSON(micro.getRootPath() + url, { "action": Action, "mid": MID, "formid": FormID, "formsid": FormsID, "uid": uid }, function (v) { 
               mGet.getOvertimeDate(v.MinOvertimeDate, v.MaxOvertimeDate);
            });
        },

        //调整结束日期，如果开始时间大于结束时间，让结束日期增加一天
        setEndDate: function (startDate, startTime, endDate, endTime) {
            var startDateTime = startDate + ' ' + startTime + ':00',
                endDateTime = endDate + ' ' + endTime + ':00';

            if (mGet.checkBothDate(startDateTime, endDateTime)) {
                var date = new Date(endDate);
                date = date.setDate(date.getDate() + 1);
                date = new Date(date);
                var y = date.getFullYear(),
                    m = date.getMonth() + 1,
                    d = date.getDate();

                m = m < 10 ? '0' + m : m;
                d = d < 10 ? '0' + d : d;

                var ymd = y + '-' + m + '-' + d;
                $('#txtEndDate').val(ymd);
            }
        },

        //开始日期
        getStartDate: function () {
            if ($('#txtStartDate').length > 0) {
                laydate.render({
                    elem: '#txtStartDate'
                    , eventElem: '#icondatetxtStartDate'
                    , trigger: 'click'
                    , btns: ['now', 'confirm']
                    , done: function (value, date, endDate) {
                        var holidayType = XmSelectHolidayTypeID.getValue('valueStr'),
                            allDayEvent = $('#cbAllDayEvent').val(),
                            startDate = value,
                            endDate = value,
                            startTime = $('#txtStartTime').val(),
                            endTime = $('#txtEndTime').val(),
                            leaveDays = $('#selLeaveDays').val(),
                            leaveHour = $('#selLeaveHour').val(),
                            startDateTime = startDate + ' ' + startTime,
                            endDateTime = endDate + ' ' + endTime,
                            uid = XmSelectLeaveUID.getValue('valueStr');

                        $('#txtEndDate').val(endDate);
                        mGet.getEndDate(value);  //改变结束日期的min值
                        mGet.setLeaveEndDateTime(startDate, startTime, leaveDays, leaveHour, uid);
                        mGet.setHidStartEndDateTime(startDate, startTime, endDate, endTime);        
                    }
                });
            }
        },

        //开始时间
        getStartTime: function () {    
            if ($('#txtStartTime').length > 0) {
                laydate.render({
                    elem: '#txtStartTime', type: 'time', format: 'HH:mm'
                    , btns: ['confirm']
                    , trigger: 'click'
                    , done: function (value, date, endDate) {

                        var holidayType = XmSelectHolidayTypeID.getValue('valueStr'),
                            allDayEvent = $('#cbAllDayEvent').val(),
                            startDate = $('#txtStartDate').val(),
                            endDate = $('#txtEndDate').val(),
                            startTime = value,
                            endTime = value,
                            leaveDays = $('#selLeaveDays').val(),
                            leaveHour = $('#selLeaveHour').val(),
                            startDateTime = startDate + ' ' + startTime,
                            endDateTime = endDate + ' ' + endTime,
                            uid = XmSelectLeaveUID.getValue('valueStr');

                        $('#txtEndTime').val(endTime);
                        mGet.setLeaveEndDateTime(startDate, startTime, leaveDays, leaveHour, uid);
                        mGet.setHidStartEndDateTime(startDate, startTime, endDate, endTime);

                    }
                });
            }
        },

        //结束日期
        getEndDate: function (minDate) {

            if ($('#txtEndDate').length > 0) {
                $('#txtEndDate')[0].eventHandler = false;

                var oEndDate;

                laydate.render({
                    elem: '#txtEndDate'
                    , eventElem: '#icondatetxtEndDate'
                    , trigger: 'click'
                    , btns: ['confirm']
                    , min: minDate
                    , ready: function (date) {

                        var y = date.year,
                            m = date.month,
                            d = date.date;

                        m = m < 10 ? '0' + m : m;
                        d = d < 10 ? '0' + d : d;

                        oEndDate = y + '-' + m + '-' + d;
                    }
                    , done: function (value, date, endDate) {

                        var holidayType = XmSelectHolidayTypeID.getValue('valueStr'),
                            allDayEvent = $('#cbAllDayEvent').val(),
                            startDate = $('#txtStartDate').val(),
                            endDate = value,
                            startTime = $('#txtStartTime').val(),
                            endTime = $('#txtEndTime').val(),
                            startDateTime = startDate + ' ' + startTime,
                            endDateTime = endDate + ' ' + endTime;

                        if (mGet.checkBothDate(startDateTime, endDateTime)) {
                            layer.msg('结束时间不能小于开始时间');
                            $('#txtEndDate').val(oEndDate);
                        }

                        mGet.setHidStartEndDateTime(startDate, startTime, endDate, endTime);
                    }
                });
            }
        },

        //结束时间
        getEndTime: function () {    

            if ($('#txtEndTime').length > 0) {
                $('#txtEndTime')[0].eventHandler = false;

                laydate.render({
                    elem: '#txtEndTime', type: 'time', format: 'HH:mm'
                    , btns: ['confirm']
                    , trigger: 'click'
                    , done: function (value, date, endDate) {
                        var holidayType = XmSelectHolidayTypeID.getValue('valueStr'),
                            allDayEvent = $('#cbAllDayEvent').val(),
                            startDate = $('#txtStartDate').val(),
                            startTime = $('#txtStartTime').val(),
                            endDate = $('#txtEndDate').val(),
                            endTime = value,
                            startDateTime = startDate + ' ' + startTime,
                            endDateTime = endDate + ' ' + endTime;

                        mGet.setEndDate(startDate, startTime, endDate, endTime);
                        mGet.setHidStartEndDateTime(startDate, startTime, endDate, endTime);
                    }
                });
            }
        },

        //获取个人假期生成Select
        getPersonalHoliday: function (uid, holidayTypeID, overtimeDate, leaveDaysDefVal, leaveHourDefVal) {

            $.getJSON('/Views/Forms/HR/GetLeaveMax.ashx', { "Date": overtimeDate, "TypeID": holidayTypeID, "Val": uid }, function (data) {

                //休假天数
                var ElemID1 = 'selLeaveDays';
                document.getElementById(ElemID1).options.length = 0; //清空值
                $.each(data.DaysArr, function (index, value) {
                    if (value.value == leaveDaysDefVal)
                        $('#' + ElemID1).append('<option value=" ' + value.value + '" selected="selected">' + value.text + '</option>');
                    else
                        $('#' + ElemID1).append('<option value=" ' + value.value + '">' + value.text + '</option>');
                    form.render();
                });

                //休假小时数
                var ElemID2 = 'selLeaveHour';
                document.getElementById(ElemID2).options.length = 0; //清空值
                $.each(data.HourArr, function (index, value) {
                    if (value.value == leaveHourDefVal)
                        $('#' + ElemID2).append('<option value=" ' + value.value + '" selected="selected">' + value.text + '</option>');
                    else
                        $('#' + ElemID2).append('<option value=" ' + value.value + '">' + value.text + '</option>');
                    form.render();
                });

            });
        },

        //获取个人追加生成提示信息
        getLeaveTips: function (uid, holidayTypeID, overtimeDate, holidayTypeName) {

            var parms = { "Val": uid, "TypeID": holidayTypeID, "Date": encodeURI(overtimeDate), "TypeName": holidayTypeName };
            $.getJSON('/Views/Forms/HR/GetLeaveTips.ashx', parms, function (data) {
                $('#divShowLeave').html(data.Tips);
                $('#hidAvailableNumber').val(data.Days);

                leaveDays = $('#selLeaveDays').val();
                mGet.setRemainingNumber(data.Days, leaveDays);

            });
        },

        //启用或关闭OvertimeDate控件
        setOvertimeDate: function (holidayTypeID) {
            if (holidayTypeID != "2") {
                $('#txtOvertimeDate').addClass('layui-disabled').prop('disabled', 'disabled');
                $('.ws-overtimedate').addClass('layui-hide');
            }
            else {
                $('#txtOvertimeDate').removeClass('layui-disabled').prop('disabled', '');
                $('.ws-overtimedate').removeClass('layui-hide');
            }
        },

        //初次打开页面（新增动作时）根据开始日期、开始时间、LeaveUID(休假时间可为空) 根据User班次计算出开始时间、结束日期、结束时间
        setLeaveStartEndDateTime: function (startDate, startTime, leaveDays, leaveHour, leaveUID) {

            var startDateTime = startDate + ' ' + startTime;
            $.getJSON('/Views/Forms/HR/GetLeaveDateTime.ashx', { "StartDateTime": encodeURI(startDateTime), "LeaveDays": encodeURI(leaveDays), "LeaveHour": encodeURI(leaveHour), "LeaveUID": encodeURI(leaveUID) }, function (data) {
                $('#txtStartTime').val(data[0].StartTime);
                $('#txtEndDate').val(data[0].EndDate);
                $('#txtEndTime').val(data[0].EndTime);
                mGet.setHidStartEndDateTime(startDate, data[0].StartTime, data[0].EndDate, data[0].EndTime);
            });
        },

        //在休假天数或休假小时发生变化时根据开始日期、开始时间、休假时间计算出结束日期
        setLeaveEndDateTime: function (startDate, startTime, leaveDays, leaveHour, leaveUID) {

            var startDateTime = startDate + ' ' + startTime;
            $.getJSON('/Views/Forms/HR/GetLeaveDateTime.ashx', { "StartDateTime": encodeURI(startDateTime), "LeaveDays": encodeURI(leaveDays), "LeaveHour": encodeURI(leaveHour), "LeaveUID": encodeURI(leaveUID) }, function (data) {
                $('#txtEndDate').val(data[0].EndDate);
                $('#txtEndTime').val(data[0].EndTime);
                mGet.setHidStartEndDateTime(startDate, startTime, data[0].EndDate, data[0].EndTime);
            });
        },

        //设置开始日期和结束日期时间隐藏控件的值
        setHidStartEndDateTime: function (startDate, startTime, endDate, endTime) {
            $('#hidStartDateTime').val(startDate + ' ' + startTime);
            $('#hidEndDateTime').val(endDate + ' ' + endTime);
        },

        //计算出剩余数量再设置值到控件
        setRemainingNumber: function (availableNumber, leaveDays) {
            $('#hidRemainingNumber').val(parseFloat(availableNumber) - parseFloat(leaveDays));

            //如果休假类型为非年休假和代休假时不需要计算剩余假期
            if ($('#selHolidayTypeID').length > 0) {
                var holidayTypeID = XmSelectHolidayTypeID.getValue('valueStr');
                if (holidayTypeID != '1' && holidayTypeID != '2') {    
                    $('#hidAvailableNumber').val('0');
                    $('#hidRemainingNumber').val('0');
                }

            }
        }

    };


    //休假姓名
    if ($('#selLeaveUID').length > 0) {

        var ParaLeaveUID = { "action": "get", "type": "DeptUsers", "mid": MID, "defaultvalue": $('#xmByUserDefVal').length > 0 ? $('#xmByUserDefVal').val() : LeaveUID };
        var XmSelectLeaveUID = xmSelect.render({ el: '#selLeaveUID', name: 'selLeaveUID', language: 'zn', autoRow: false, filterable: true, showCount: 10, pageSize: 10, searchTips: 'Keyword', paging: true, height: 'auto', radio: true, clickClose: true, layVerify: 'required', model: { icon: 'hidden', label: { type: 'block', block: { showCount: 10, showIcon: true, } } }, cascader: { show: true, indent: 300, strict: true }, data: eval(micro.getAjaxData('text', micro.getRootPath() + '/App_Handler/GetUserPublicInfoForXmSelect.ashx', ParaLeaveUID)) });

        XmSelectLeaveUID.update({
            on: function (data) {
                $(".layui-laydate").remove();  //关闭日历控件
                document.getElementById('selLeaveDays').options.length = 0;
                var arr = data.arr,
                    uid = $.map(arr, function (n) {
                        return n.value;
                    }).join(),
                    name = $.map(arr, function (n) {
                        return n.name;
                    }).join(),
                    holidayTypeID = XmSelectHolidayTypeID.getValue('valueStr'),
                    holidayTypeName = XmSelectHolidayTypeID.getValue('nameStr'),
                    overtimeDate = $('#txtOvertimeDate').val();

                $('#hidLeaveDisplayName').val(name);
                mGet.getPersonalHoliday(uid, holidayTypeID, overtimeDate);
                mGet.getLeaveTips(uid, holidayTypeID, overtimeDate, holidayTypeName);

                mGet.getOvertimeDateRange(uid);

            },
        })
    }


    //休假类别
    if ($('#selHolidayTypeID').length > 0) {
        
        var ParaHolidayTypeID = { "action": "get", "mid": MID, "formid": FormID, "tn": "HRHolidayType", "txt": "HolidayName", "val": "HolidayTypeID", "isapprovalform": "True", "defaultvalue": HolidayTypeID };
        var XmSelectHolidayTypeID = xmSelect.render({ el: '#selHolidayTypeID', name: 'selHolidayTypeID', language: 'zn', autoRow: false, filterable: true, showCount: 10, pageSize: 10, searchTips: 'Keyword', paging: true, height: 'auto', radio: true, clickClose: true, layVerify: 'required', model: { icon: 'hidden', label: { type: 'block', block: { showCount: 10, showIcon: true, } } }, cascader: { show: true, indent: 300, strict: true }, data: eval(micro.getAjaxData('text', micro.getRootPath() + '/App_Handler/GetPublicXmSelect.ashx', ParaHolidayTypeID)) });

        XmSelectHolidayTypeID.update({
            on: function (data) {
                $('#txtOvertimeDate').val('');  //清空加班月份
                $(".layui-laydate").remove();  //关闭日历控件
                document.getElementById('selLeaveDays').options.length = 0;
                var uid = XmSelectLeaveUID.getValue('valueStr'),
                    arr = data.arr,
                    holidayTypeID = $.map(arr, function (n) {
                        return n.value;
                    }).join(),
                    holidayTypeName = $.map(arr, function (n) {
                        return n.name;
                    }).join(),
                    overtimeDate = $('#txtOvertimeDate').val();

                mGet.getPersonalHoliday(uid, holidayTypeID, overtimeDate);
                mGet.getLeaveTips(uid, holidayTypeID, overtimeDate, holidayTypeName);
                mGet.setOvertimeDate(holidayTypeID);

            },
        })
    }

    mGet.getOvertimeDate($('#txtMinOvertimeDate').val(), $('#txtMaxOvertimeDate').val());
    mGet.getStartDate();
    mGet.getEndDate($('#txtStartDate').val());
    mGet.getStartTime();
    mGet.getEndTime();

    if (Action == "add") {
        var startDate = $('#txtStartDate').val(),
            startTime = $('#txtStartTime').val(),
            leaveDays = $('#selLeaveDays').val(),
            leaveHour = $('#selLeaveHour').val(),
            uid = XmSelectLeaveUID.getValue('valueStr');

        mGet.setLeaveStartEndDateTime(startDate, startTime, leaveDays, leaveHour, uid);
    }

    if (Action == "modify") {
        var overtimeDate = $('#txtOvertimeDate').val(),
            xmByLaveDaysDefval = $('#xmByLaveDaysDefval').val(),
            xmByLaveHourDefval = $('#xmByLaveHourDefval').val(),
            xmSelectHolidayTypeName = XmSelectHolidayTypeID.getValue('nameStr');

        mGet.getPersonalHoliday(LeaveUID, HolidayTypeID, overtimeDate, xmByLaveDaysDefval, xmByLaveHourDefval);
        mGet.getLeaveTips(LeaveUID, HolidayTypeID, overtimeDate, xmSelectHolidayTypeName);
    }

    if ($('#selHolidayTypeID').length > 0)
        mGet.setOvertimeDate(XmSelectHolidayTypeID.getValue('valueStr'));

    //休假天数
    form.on('select(selLeaveDays)', function (data) {

        var startDate = $('#txtStartDate').val(),
            startTime = $('#txtStartTime').val(),
            leaveDays = data.value,
            leaveHour = $('#selLeaveHour').val(),
            availableNumber = $('#hidAvailableNumber').val(),
            uid = XmSelectLeaveUID.getValue('valueStr');

        mGet.setLeaveEndDateTime(startDate, startTime, leaveDays, leaveHour, uid);
        mGet.setRemainingNumber(availableNumber, leaveDays);

    });

    //休假小数
    form.on('select(selLeaveHour)', function (data) {

        var startDate = $('#txtStartDate').val(),
            startTime = $('#txtStartTime').val(),
            leaveDays = $('#selLeaveDays').val(),
            leaveHour = data.value,
            availableNumber = $('#hidAvailableNumber').val(),
            uid = XmSelectLeaveUID.getValue('valueStr');

        mGet.setLeaveEndDateTime(startDate, startTime, leaveDays, leaveHour, uid);
        mGet.setRemainingNumber(availableNumber, leaveDays);

    });

});