layui.use(['index', 'jquery', 'form', 'table', 'layer', 'laydate', 'micro'], function () {
    var $ = layui.$
        , form = layui.form
        , table = layui.table
        , layer = layui.layer
        , laydate = layui.laydate
        , micro = layui.micro;

    var Action = $('#txtAction').val();
    var MID = micro.getUrlPara('mid');
    var STN = $('#txtShortTableName').val();
    var FormID = $('#txtFormID').val();
    var FormsID = $('#txtFormsID').val();
    var DateRange = $('#txtDateRange').val();
    var StartDate = DateRange.split('~')[0].trim();
    var EndDate = DateRange.split('~')[1].trim();

    laydate.render({
        elem: '#txtDateRange'
        , range: '~'
        , btns: ['confirm']
        , trigger: 'click'
        //, showBottom: false
        , done: function (value, date, endDate) {
            var action = $('#txtAction').val();
            var mid = micro.getUrlPara('mid');
            var stn = $('#txtShortTableName').val();
            var keyword = $('#txtKeyword').val();
            var startDate = value.split('~')[0].trim();
            var endDate = value.split('~')[1].trim();

            mGet.GetData(action, stn, mid, '', '', 'Search', keyword, startDate, endDate);
        }
    });

    var mGet = {
        //*****左侧导航Start*****
        //管理
        GetMgr: function () {
            var microSTN = $(this).attr('micro-stn');
            var microText = $(this).attr('micro-text');
          
            layer.open({
                type: 2
                , title: microText
                , content: '/Views/Forms/SysFormList/View/' + microSTN + '/' + MID
                , maxmin: true
                , area: ['95%', '90%']
                , btn2: function (index, layero) {
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

        , Add: function () {
            var microText = $(this).attr('micro-text');
            var microSTN = $(this).attr('micro-stn');

            layer.open({
                type: 2
                , title: microText
                , content: '/Views/Forms/SysForm/Add/' + microSTN + '/' + MID
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

        , Modify: function () {
            var microText = $(this).attr('micro-text');
            var microSTN = $(this).attr('micro-stn');
            var microID = $(this).attr('micro-id');

            layer.open({
                type: 2
                , title: '修改【' + microText + '】信息'
                , content: '/Views/Forms/SysForm/Modify/' + microSTN + '/' + MID + '/' + microID
                , maxmin: true
                , area: ['95%', '90%']
                , btn: ['保存修改', '关闭']
                , yes: function (index, layero) {
                    var iframeWindow = window['layui-layer-iframe' + index]
                        , submitID = 'btnModify'
                        , submit = layero.find('iframe').contents().find('#' + submitID);

                    $(submit).click();
                }
                , btn2: function (index, layero) {
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

        //左侧菜单点选调用
        , GetUsers: function () {
            var microType = $(this).attr('micro-type');
            var microID = $(this).attr('micro-id');

            var action = $('#txtAction').val();
            var mid = micro.getUrlPara('mid');
            var stn = $('#txtShortTableName').val();
            var queryType = microType;
            var keyword = microID;
            var dateRange = $('#txtDateRange').val();
            var startDate = dateRange.split('~')[0].trim();
            var endDate = dateRange.split('~')[1].trim();

            mGet.GetData(action, stn, mid, '', '', queryType, keyword, startDate, endDate);
        }
        //*****左侧导航End*****

        //统计用户加班数据
        , GetData: function (action, stn, moduleID, dataType, dataURL, queryType, keyword, startDate, endDate) {

            var parms = '',
                action = action || 'View',
                moduleID = moduleID || $("#txtMID").val(),
                dataType = dataType || '',
                dataURL = dataURL || '',
                queryType = queryType || '',
                keyword = keyword || '',
                startDate = startDate || StartDate,
                endDate = endDate || EndDate;

            if (typeof (stn) !== "undefined" && stn !== "" && stn !== null)
                parms = '?Action=' + action + '&STN=' + stn + '&ModuleID=' + moduleID + '&DataType=' + dataType + '&QueryType=' + queryType + '&Keyword=' + keyword + '&StartDate=' + startDate + '&EndDate=' + endDate;

            var url = '/Views/Stats/Attendance/UserOnDuty.ashx';

            $.ajax({
                async: true,
                type: 'post',
                dataType: 'json',
                url: url,
                data: parms,
                cache: false,
                beforeSend: function () {
                    layer.load(2);
                },
                success: function (v) {
                    layer.close(layer.load(2));

                    if (v.msg.substring(0, 4) === 'True')
                        mGet.GetTable(v);
                    else {
                        layer.msg('数据读取失败<br/>データの読み込みに失敗しました<br/>Data reading failed', {
                            icon: 5
                            , anim: 6
                        });
                        mGet.GetTable();
                    }
                },
                error: function () {

                    layer.close(layer.load(2));
                    layer.msg('数据读取失败<br/>データの読み込みに失敗しました<br/>Data reading failed', {
                        icon: 5
                        , anim: 6
                    });

                }
            });

        },

        GetTable: function (v) {
            v = v || eval('({\"code\": 0,\"msg\": \"数据读取失败<br/>データの読み込みに失敗しました<br/>Data reading failed\",\"count\":0,\"cols\":  [],\"data\":  [] })');

            table.render({
                elem: '#tabTable'
                , id: 'tabTable'
                , toolbar: true
                , initSort: { field: 'ColUserDepts', type: 'asc' }
                //, url: v //micro.getRootPath() + tbURL + parms //异步数据接口,get方式
                , data: v.data
                , text: { none: '暂无相关数据' }
                , even: true
                , height:'full-120'
                , cellMinWidth: 60
                , cols: [eval(v.cols)]
                , page: true
                , limit: 100
                , limits: [100,200,300,400,500,600,700,800,900,1000]
            });
        },

        btnSearch: function () {
            var action = $('#txtAction').val();
            var mid = micro.getUrlPara('mid');
            var stn = $('#txtShortTableName').val();
            var queryType = 'Search';
            var keyword = $('#txtKeyword').val();
            var dateRange = $('#txtDateRange').val();
            var startDate = dateRange.split('~')[0].trim();
            var endDate = dateRange.split('~')[1].trim();

            mGet.GetData(action, stn, mid, '', '', queryType, keyword, startDate, endDate);        
        }
    };

    
    //页面初始化默认显示所有用户
    mGet.GetData('View', STN, MID, '', '', 'Dept', 'All', StartDate, EndDate);

    $('.layui-btn').on('click', function () {
        var type = $(this).data('type');
        mGet[type] ? mGet[type].call(this) : '';
    });

    $('.micro-click').on('click', function () {
        var type = $(this).data('type');
        mGet[type] ? mGet[type].call(this) : '';
    });

    //提交表单
    form.on('submit(btnSearch)', function (data) {
        var Keyword = data.field.txtKeyword;
        if (Keyword !== '') {
            mGet.btnSearch();
            return false;
        }
    });


});