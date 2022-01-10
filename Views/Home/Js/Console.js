layui.use(['index', 'form', 'carousel', 'element', 'layer', 'micro'], function () {
    var $ = layui.$
        , form = layui.form
        , carousel = layui.carousel
        , element = layui.element
        , layer = layui.layer
        , micro = layui.micro
        , device = layui.device();
        
    var MID = $("#txtMID").val();

    //轮播切换
    $('.layadmin-carousel').each(function () {
        var othis = $(this);
        carousel.render({
            elem: this
            , width: '100%'
            , arrow: 'none'
            , interval: othis.data('interval')
            , autoplay: othis.data('autoplay') === true
            , trigger: (device.ios || device.android) ? 'click' : 'hover'
            , anim: othis.data('anim')
        });
    });

    //进度条
    element.render('progress');

    //自定义函数
    var mGet = {

        //遍历日历视图的天数，如果是假期则背景色填充灰色
        GetCalendarMonthViewDay: function () {
            trList = $("#divCalendar .fc-dayGridMonth-view tbody").children("tr");

            if (trList.length >= 2) {

                var startDate = trList.eq(2).find("td").eq(0).attr('data-date');
                var endDate = trList.eq(trList.length - 1).find("td").eq(6).attr('data-date');

                mGet.GetPersonalAttendanceData(startDate, endDate, encodeURI(calendar.getEventSources()[0].context.viewTitle))

                var Parameter = { "action": "View", "type": "GetCalendarDays", "start": startDate, "end": endDate, "random": Math.random() };
                var calDays = micro.getGetAjaxData('json', '/Views/Forms/Calendar/Calendar.ashx', Parameter, false);

                //console.log(calDays.data);

                if (calDays.count > 0) {

                    $("#divCalendar .fc-dayGridMonth-view tbody").find("tr").each(function () {
                        var tdArr = $(this).children();
                        var Day0 = tdArr.eq(0).attr('data-date');
                        var Day1 = tdArr.eq(1).attr('data-date');
                        var Day2 = tdArr.eq(2).attr('data-date');
                        var Day3 = tdArr.eq(3).attr('data-date');
                        var Day4 = tdArr.eq(4).attr('data-date');
                        var Day5 = tdArr.eq(5).attr('data-date');
                        var Day6 = tdArr.eq(6).attr('data-date');

                        mGet.GetClendarDays(tdArr.eq(0), Day0, calDays);
                        mGet.GetClendarDays(tdArr.eq(1), Day1, calDays);
                        mGet.GetClendarDays(tdArr.eq(2), Day2, calDays);
                        mGet.GetClendarDays(tdArr.eq(3), Day3, calDays);
                        mGet.GetClendarDays(tdArr.eq(4), Day4, calDays);
                        mGet.GetClendarDays(tdArr.eq(5), Day5, calDays);
                        mGet.GetClendarDays(tdArr.eq(6), Day6, calDays);

                    });
                }
            }

        },

        //获取假期日期
        GetClendarDays: function (tdCell, tdDay, calDays) {
            //var Holidays = "2020-11-01,2020-11-07,2020-11-08,2020-11-14,2020-11-15,2020-11-21,2020-12-08,2020-12-09,2020-12-10";

            if (calDays.data.length > 0) {
                //先移除日历所有已有的Class
                tdCell.css('background-color', '#ffffff').removeClass("ws-bg-cal-img-offday");
                tdCell.css('background-color', '#ffffff').removeClass("ws-bg-cal-img-statutory");

                for (var i = 0; i < calDays.data.length; i++) {
                    if (calDays.data[i].DayDate == tdDay) {

                        if (calDays.data[i].DaysType == 'OffDay')
                            tdCell.css('background-color', '#f2f2f2').addClass("ws-bg-cal-img-offday");

                        if (calDays.data[i].DaysType == 'Statutory')
                            tdCell.css('background-color', '#f2f2f2').addClass("ws-bg-cal-img-statutory");
                    }
                }
            }

            //设置今天的日历颜色
            if (micro.getToDay() == tdDay)
                tdCell.css('background-color', '#b4f9af');  //#fff4bb  米黄色
        },

        //日历选择时
        CalendarSelect: function (type, startDate, endDate, val) {
            if (type == 'CtrlCalendarDays' || type == 'CtrlStatutoryDays')
                mGet.CtrlCalendarDays(type, startDate, endDate, val);
        },

        //设置日历假期
        CtrlCalendarDays: function (type, startDate, endDate, val) {
            var Parameter = { "action": "Modify", "type": type, "start": startDate, "end": endDate, "val": val, "random": Math.random() };
            var flag = micro.getGetAjaxData('text', '/Views/Forms/Calendar/Calendar.ashx', Parameter, false);

            if (flag.substring(0, 4) == 'True') {
                mGet.GetCalendarMonthViewDay();
                calendar.refetchEvents();
                layer.msg(flag.substring(4, flag.length));
            }
            else
                layer.msg(flag.substring(4, flag.length));

        },

        //加载个人考勤数据
        GetPersonalAttendanceData: function (startDate, endDate, val) {

            var Parameter = { "action": "View", "type": "GetPersonalAttendanceData", "start": startDate, "end": endDate, "val": val, "random": Math.random() };
            var d = micro.getGetAjaxData('json', '/Views/Forms/Calendar/Calendar.ashx', Parameter, false);
            //console.log(d);

            var MonthStartDateTime = d[0].MonthStartDateTime,
                MonthEndDateTime = d[0].MonthEndDateTime,
                QuarterStartDateTime = d[0].QuarterStartDateTime,
                QuarterEndDateTime = d[0].QuarterEndDateTime,
                //SelfDriving = d[0].SelfDriving,
                WorkHourSysID = d[0].WorkHourSysID,
                WarningValue = d[0].WarningValue,

                OvertimeTotalByMonth = d[0].OvertimeTotalByMonth,
                ExceptDaiXiuByMonth = d[0].ExceptDaiXiuByMonth,
                OvertimeTotalByQuarter = d[0].OvertimeTotalByQuarter,
                ExceptDaiXiuByQuarter = d[0].ExceptDaiXiuByQuarter,
                WorkOvertime = d[0].WorkOvertime,
                WorkOvertimeByQuarter = d[0].WorkOvertimeByQuarter,
                OffDayOvertime = d[0].OffDayOvertime,
                OffDayOvertimeByQuarter = d[0].OffDayOvertimeByQuarter,
                Statutory = d[0].Statutory,
                StatutoryByQuarter = d[0].StatutoryByQuarter,

                //NeedDaiXiu = d[0].NeedDaiXiu,
                AlreadyDaiXiuByMonth = d[0].AlreadyDaiXiuByMonth,
                AlreadyDaiXiuByQuarter = d[0].AlreadyDaiXiuByQuarter,
                AlreadyDaiXiuOvertimeByMonth = d[0].AlreadyDaiXiuOvertimeByMonth,
                AlreadyDaiXiuOvertimeByQuarter = d[0].AlreadyDaiXiuOvertimeByQuarter,
                LeaveTotalByMonth = d[0].LeaveTotalByMonth,
                LeaveTotalByQuarter = d[0].LeaveTotalByQuarter,
                RemainingDaiXiu = d[0].RemainingDaiXiu,
                RemainingDaiXiuByQuarter = d[0].RemainingDaiXiuByQuarter,
                RemainingAnnualLeave = d[0].RemainingAnnualLeave,
                RemainingOtherLeave = d[0].RemainingOtherLeave;

            //***月度、季度共同的***
            if (parseFloat(OvertimeTotalByMonth) > 0)
                $('#aOvertimeTotalByMonth').attr('lay-href', '/Views/Forms/MicroFormList/View/Overtime/4/5/1/DefaultNumber/GetPersonalOvertime/' + encodeURI(MonthStartDateTime) + '/' + encodeURI(MonthEndDateTime));

            if (parseFloat(OvertimeTotalByQuarter) > 0)
                $('#aOvertimeTotalByQuarter').attr('lay-href', '/Views/Forms/MicroFormList/View/Overtime/4/5/1/DefaultNumber/GetPersonalOvertime/' + encodeURI(QuarterStartDateTime) + '/' + encodeURI(QuarterEndDateTime));

            $('#citeOvertimeTotalByMonth').html(OvertimeTotalByMonth);
            $('#citeExceptDaiXiuByMonth').html(ExceptDaiXiuByMonth);
            $('#citeOvertimeTotalByQuarter').html(OvertimeTotalByQuarter);
            $('#citeExceptDaiXiuByQuarter').html(ExceptDaiXiuByQuarter);

            $('#citeRemainingAnnualLeave').html(RemainingAnnualLeave);
            $('#citeRemainingOtherLeave').html(RemainingOtherLeave);

            //***综合工时制***
            if (parseInt(WorkHourSysID) == 2) {

                if (parseFloat(ExceptDaiXiuByQuarter) > parseFloat(WarningValue)) {
                    $('#aExceptDaiXiuByQuarter').addClass('ws-bg-img-warning2');
                    $('#citeExceptDaiXiuByQuarter').addClass('ws-must-asterisk');
                }
                else {
                    $('#aExceptDaiXiuByQuarter').removeClass('ws-bg-img-warning2');
                    $('#citeExceptDaiXiuByQuarter').removeClass('ws-must-asterisk');
                }

                //***标题***
                $('#hWorkOvertime').html('本季度延时 (H)');
                $('#hOffDayOvertime').html('本季度休出 (H)');
                $('#hStatutory').html('本季度法定 (H)');

                //$('#citeNeedDaiXiu').html(NeedDaiXiu);
                $('#hAlreadyDaiXiuOvertime').html('本季度代休 (Day)');
                $('#hLeaveTotal').html('本季度休假 (Day)');
                $('#hRemainingDaiXiu').html('本季度剩余休出 (H)');

                //***内容***
                $('#citeWorkOvertime').html(WorkOvertimeByQuarter);
                $('#citeOffDayOvertime').html(OffDayOvertimeByQuarter);
                $('#citeStatutory').html(StatutoryByQuarter);

                //$('#citeNeedDaiXiu').html(NeedDaiXiu);
                $('#citeAlreadyDaiXiuOvertime').html(AlreadyDaiXiuOvertimeByQuarter);
                $('#citeLeaveTotal').html(LeaveTotalByQuarter);
                $('#citeRemainingDaiXiu').html(RemainingDaiXiuByQuarter);

                if (parseFloat(WorkOvertimeByQuarter) > 0)
                    $('#aWorkOvertime').attr('lay-href', '/Views/Forms/MicroFormList/View/Overtime/4/5/1/DefaultNumber/GetWorkOvertime/' + encodeURI(QuarterStartDateTime) + '/' + encodeURI(QuarterEndDateTime));

                if (parseFloat(OffDayOvertimeByQuarter) > 0)
                    $('#aOffDayOvertime').attr('lay-href', '/Views/Forms/MicroFormList/View/Overtime/4/5/1/DefaultNumber/GetOffDayOvertime/' + encodeURI(QuarterStartDateTime) + '/' + encodeURI(QuarterEndDateTime));

                if (parseFloat(StatutoryByQuarter) > 0)
                    $('#aStatutory').attr('lay-href', '/Views/Forms/MicroFormList/View/Overtime/4/5/1/DefaultNumber/GetStatutory/' + encodeURI(QuarterStartDateTime) + '/' + encodeURI(QuarterEndDateTime));

                //if (parseFloat(NeedDaiXiu) > 0)
                //    $('#aNeedDaiXiu').attr('lay-href', '/Views/Forms/MicroFormList/View/Leave/4/7/1/DefaultNumber/GetNeedDaiXiu/' + encodeURI(QuarterStartDateTime) + '/' + encodeURI(QuarterEndDateTime));

                if (parseFloat(AlreadyDaiXiuOvertimeByQuarter) > 0)
                    $('#aAlreadyDaiXiuOvertime').attr('lay-href', '/Views/Forms/MicroFormList/View/Leave/4/7/1/DefaultNumber/GetAlreadyDaiXiu/' + encodeURI(QuarterStartDateTime) + '/' + encodeURI(QuarterEndDateTime));

                if (parseFloat(LeaveTotalByQuarter) > 0)
                    $('#aLeaveTotal').attr('lay-href', '/Views/Forms/MicroFormList/View/Leave/4/7/1/DefaultNumber/GetLeave/' + encodeURI(QuarterStartDateTime) + '/' + encodeURI(QuarterEndDateTime));
            }
            else {  //***标准工时制***

                if (parseFloat(ExceptDaiXiuByMonth) > parseFloat(WarningValue)) {
                    $('#aExceptDaiXiuByMonth').addClass('ws-bg-img-warning2');  //Warning
                    $('#citeExceptDaiXiuByMonth').addClass('ws-must-asterisk');
                }
                else {
                    $('#aExceptDaiXiuByMonth').removeClass('ws-bg-img-warning2');
                    $('#citeExceptDaiXiuByMonth').removeClass('ws-must-asterisk');
                }

                $('#citeWorkOvertime').html(WorkOvertime);
                $('#citeOffDayOvertime').html(OffDayOvertime);
                $('#citeStatutory').html(Statutory);

                //$('#citeNeedDaiXiu').html(NeedDaiXiu);
                $('#citeAlreadyDaiXiuOvertime').html(AlreadyDaiXiuOvertimeByMonth);
                $('#citeLeaveTotal').html(LeaveTotalByMonth);
                $('#citeRemainingDaiXiu').html(RemainingDaiXiu);

                if (parseFloat(WorkOvertime) > 0)
                    $('#aWorkOvertime').attr('lay-href', '/Views/Forms/MicroFormList/View/Overtime/4/5/1/DefaultNumber/GetWorkOvertime/' + encodeURI(MonthStartDateTime) + '/' + encodeURI(MonthEndDateTime));

                if (parseFloat(OffDayOvertime) > 0)
                    $('#aOffDayOvertime').attr('lay-href', '/Views/Forms/MicroFormList/View/Overtime/4/5/1/DefaultNumber/GetOffDayOvertime/' + encodeURI(MonthStartDateTime) + '/' + encodeURI(MonthEndDateTime));

                if (parseFloat(Statutory) > 0)
                    $('#aStatutory').attr('lay-href', '/Views/Forms/MicroFormList/View/Overtime/4/5/1/DefaultNumber/GetStatutory/' + encodeURI(MonthStartDateTime) + '/' + encodeURI(MonthEndDateTime));

                //if (parseFloat(NeedDaiXiu) > 0)
                //    $('#aNeedDaiXiu').attr('lay-href', '/Views/Forms/MicroFormList/View/Leave/4/7/1/DefaultNumber/GetNeedDaiXiu/' + encodeURI(MonthStartDateTime) + '/' + encodeURI(MonthEndDateTime));

                if (parseFloat(AlreadyDaiXiuOvertimeByMonth) > 0)
                    $('#aAlreadyDaiXiuOvertime').attr('lay-href', '/Views/Forms/MicroFormList/View/Leave/4/7/1/DefaultNumber/GetAlreadyDaiXiu/' + encodeURI(MonthStartDateTime) + '/' + encodeURI(MonthEndDateTime));

                if (parseFloat(LeaveTotalByMonth) > 0)
                    $('#aLeaveTotal').attr('lay-href', '/Views/Forms/MicroFormList/View/Leave/4/7/1/DefaultNumber/GetLeave/' + encodeURI(MonthStartDateTime) + '/' + encodeURI(MonthEndDateTime));
            }

            $('#txtMonthStartDate').val(MonthStartDateTime);
            $('#txtMonthEndDate').val(MonthEndDateTime);
            $('#txtQuarterStartDate').val(QuarterStartDateTime);
            $('#txtQuarterEndDate').val(QuarterEndDateTime);

        }

        //弹窗待我审批
        , GetPendingMyApproval: function () {

            layer.open({
                type: 2
                , title: '待我审批 / <span class="ws-font-size8"> 私の承認待ち</span>'
                , content: '/Views/Home/PendingMyApproval/' + MID
                , maxmin: true
                , area: ['95%', '90%']
            });

        }
    }


    //Loading Calendar
    var calendarEl = document.getElementById('divCalendar');
    var calendar = new FullCalendar.Calendar(calendarEl, {
        customButtons: {
            myCustomButton: {
                text: '自定义按钮',
                click: function () {
                    alert('点击了自定义按钮!');
                }
            }
        },
        headerToolbar: {
            left: 'prevYear prev today next nextYear', //myCustomButton
            center: 'title',
            right: 'dayGridMonth' //,timeGridWeek,timeGridDay,listMonth
        },
        initialView: 'dayGridMonth' //'dayGridMonth' resourceTimelineDay
        , locale: $('#txtLocale').val() // 'zh-cn' 'ja'
        , firstDay: parseInt($('#txtCalendarFirstDay').val()) //一周开始的第一天 0=日期日，1=星期一...
        , fixedWeekCount: false  //一个月视图显示多少周
        , selectable: true  //允许选择时间段
        , businessHours: [ // specify an array instead
            {
                daysOfWeek: [0, 1, 2, 3, 4, 5, 6], // Monday, Tuesday, Wednesday
                startTime: '08:30', // 8am
                endTime: '17:15' // 6pm
            }
        ]
        , height: 'auto'
        , timeZone: 'local'
        , events: {
            url: '/Views/Forms/Calendar/Calendar.ashx?action=View&type=GetCalendarEvents&mid=' + MID + '&random=' + Math.random(),
            cache: false
        }
        , eventTimeFormat: { // like '14:30:00'
            hour: '2-digit',
            minute: '2-digit',
            meridiem: false,
            hour12: false
        }
        , eventColor: '#1E9FFF' //#009688 #5FB878 //所有事件默认颜色
        , eventDrop: function (event, delta, revertFunc) {

            alert(event.title + " was dropped on " + event.id);

            if (!confirm("Are you sure about this change?")) {
                revertFunc();
            }

        }
        , slotLabelFormat: {
            hour: 'numeric', //2-digit
            meridiem: false,
            hour12: false
        }
        ,select: function (info) {
            var startDate = info.startStr, endDate = info.endStr;
            $('.microCtrl input:checkbox').each(function () {
                if ($(this).prop('checked')) {  //$(this).is(':checked') 
                    var type = $(this).attr('micro-type');
                    var val = $(this).attr('value');
                    mGet.CalendarSelect(type, startDate, endDate, val);
                }
            });
        }
    });

    calendar.render();
    mGet.GetCalendarMonthViewDay();

    $('.fc-prevYear-button').click(function () {
        mGet.GetCalendarMonthViewDay();
    });

    $('.fc-prev-button').click(function () {
        mGet.GetCalendarMonthViewDay();
    });

    $('.fc-today-button').click(function () {
        mGet.GetCalendarMonthViewDay();
    });

    $('.fc-next-button').click(function () {
        mGet.GetCalendarMonthViewDay();
    });

    $('.fc-nextYear-button').click(function () {
        mGet.GetCalendarMonthViewDay();
    });

    //CheckBox互斥（载入考勤、设置休日、设置法定）
    form.on('checkbox(ckMicroCheckBox)', function (data) {
        if (data.elem.checked) {
            $('.microCtrl input:checkbox').prop('checked', false);
            $(this).prop('checked', true);
            form.render();
        }
    });


    //监听路由
    layui.admin.on('hash(tab)', function () {
        layui.router().path.join('') || renderDataView(carouselIndex);
    });

    $('.micro-click').on('click', function () {
        var type = $(this).data('type');
        mGet[type] ? mGet[type].call(this) : '';
    });

});


