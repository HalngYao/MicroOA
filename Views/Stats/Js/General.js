layui.use(['index', 'form', 'carousel', 'laydate', 'layer', 'micro'], function () {
    var $ = layui.$
        , form = layui.form
        , carousel = layui.carousel
        , laydate = layui.laydate
        , layer = layui.layer
        , device = layui.device()
        , micro = layui.micro;

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

    //自定义函数
    var mGet = {
        //开始日期
        getStartDate: function () {

            laydate.render({
                elem: '#txtStartDate',
                eventElem: '#icondatetxtStartDate',
                trigger: 'click'
                , min: '2020-01-01'
                , btns: ['now', 'confirm']
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
                , btns: ['now', 'confirm']
            });
        },

        //就餐统计
        getOvertimeMeal: function (location, startDate, endDate) {
            var keywordStr = '';
            var parms = { "action": "View", "type": location, "startdate": startDate, "enddate": endDate };
            var d = micro.getGetAjaxData('json', '/Views/Stats/General.ashx', parms, false);

            if (location.trim() != '')
                keywordStr = 'Location_' + location + ',Keyword_';

            //就餐汇总（加班+排班）
            for (var i = 0; i < Object.keys(d.totalData).length; i++) {
                var totalCount = d.totalData[i].TotalCount;
                $('#citeMeal' + i).html(totalCount);
            }

            //加班就餐数据
            for (var i = 0; i < Object.keys(d.mealData).length; i++) {
                var mealCount = d.mealData[i].MealCount;

                if (mealCount > 0)
                    $('#aMeal2' + i).attr('lay-href', '/Views/Forms/MicroFormList/View/Overtime/4/5/1/DefaultNumber/GetOvertimeList/' + decodeURI(startDate) + '/' + decodeURI(endDate) + '/' + decodeURI(keywordStr + d.mealData[i].OvertimeMealName) +'/OvertimeDate');
                else
                    $('#aMeal2' + i).removeAttr('lay-href');

                $('#citeMeal2' + i).html(mealCount);
            }

            //排班数据
            for (var i = 0; i < Object.keys(d.dutyData).length; i++) {
                var dutyCount = d.dutyData[i].DutyCount;

                if (dutyCount > 0)
                    $('#aShiftName3' + i).attr('lay-href', '/Views/Forms/MicroFormList/View/OnDutyForm/4/13/1/DefaultNumber/GetOnDutyFormListByMeal/' + decodeURI(startDate) + '/' + decodeURI(endDate) + '/' + decodeURI(keywordStr + d.dutyData[i].ShiftName) + '/DutyDate');
                else
                    $('#aShiftName3' + i).removeAttr('lay-href');

                $('#citeShiftName3' + i).html(dutyCount);
            }

        },

        //查询
        btnSearch: function () {

            var location = $('#txtLocation').val(),
                startDate = $('#txtStartDate').val(),
                endDate = $('#txtEndDate').val();

            mGet.getOvertimeMeal(location, startDate, endDate);
        }
    }

    mGet.getStartDate();
    mGet.getEndDate($('#txtStartDate').val());

    $('.layui-btn').on('click', function () {
        var type = $(this).data('type');
        mGet[type] ? mGet[type].call(this) : '';
    });

});


