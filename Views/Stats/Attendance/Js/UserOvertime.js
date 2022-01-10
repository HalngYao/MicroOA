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

            //action, stn, moduleID, dataType, dataURL, queryType, keyword, startDate, endDate
            mGet.GetUserOvertime(action, stn, mid, '', '', 'Search', keyword, startDate, endDate);
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

            //action, stn, moduleID, dataType, dataURL, queryType, keyword, startDate, endDate
            mGet.GetUserOvertime(action, stn, mid, '', '', queryType, keyword, startDate, endDate);
        }
        //*****左侧导航End*****

        //统计用户加班数据
        , GetUserOvertime: function (action, stn, moduleID, dataType, dataURL, queryType, keyword, startDate, endDate) {

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

            table.render({
                elem: '#tabTable'
                , url: '/Views/Stats/Attendance/UserOvertime.ashx' + parms
                , toolbar: true
                //, defaultToolbar: 'default'
                , initSort: { field: 'ColUserDepts', type: 'asc' }
                , totalRow: true
                , cols: [
                    [
                        { type: 'checkbox', fixed: 'left'}
                        , { field: 'UID', title: 'UID', width: 80, align: 'center', fixed: 'left', hide: true }
                        , { field: 'ColUserDepts', title: '部门', width: 180, align: 'left', fixed: 'left', sort: true }
                        , { field: 'DisplayName', title: '姓名', width: 230, align: 'left', fixed: 'left', sort: true }
                        , { field: 'ColUserJobTitle', title: '职务', width: 160, align: 'left', sort: true}
                        //, { field: 'WorkTel', title: '分机', width: 80, align: 'center'}
                        //, { field: 'WorkMobilePhone', title: '手机', width: 120, align:'center'}
                        , { field: 'ColUserWorkHourSystem', title: '工时制', width: 100, align: 'center', sort: true, totalRowText: '合计'}
                        //, { field: 'ShiftName', title: '班次', width: 120, align: 'center', sort: true, totalRowText:'合计'}
                        , { field: 'WorkOvertime', title: '延时加班（H）', width: 140, align: 'center', sort: true, totalRow:true }
                        , { field: 'OffdayOvertime', title: '休日加班（H）', width: 140, align: 'center', sort: true, totalRow: true }
                        , { field: 'StatutoryOvertime', title: '法定加班（H）', width: 140, align: 'center', sort: true, totalRow: true }
                        , { field: 'OvertimeTotal', title: '总加班时间（H）', width: 160, align: 'center', sort: true, totalRow: true }
                        , { field: 'AlreadyDaiXiu', title: '已代休时间（H）', width: 160, align: 'center', sort: true, totalRow: true }
                        , { field: 'ExceptDaiXiu', title: '剔除代休后加班时间（H）', width: 220, align: 'center', sort: true, totalRow: true }
                        , { field: 'WarningValue', title: '预警值（H）', width: 120, align: 'center' }
                        , { field: 'AnnualLeave', title: '剩余年休（H）', width: 140, align: 'center', sort: true, totalRow: true }
                        , { field: 'LeaveTotal', title: '共休假', width: 150, align: 'center', sort: true }
                        
                    ]
                ]
                , page: true
                , limit: 100
                , limits: [10, 20, 30, 50, 100, 200, 300, 500, 1000]
                , even: true
                //, data: ['']//micro.getAjaxData('json', micro.getRootPath() + '/Views/Set/System/MicroDataTableList.ashx', tabPara)["data"]
                , height: 'full-120'
                //, size: 'sm'  //sm
                , text: {
                    none: '暂无相关数据。No relevant data.'
                }
                , done: function (res, curr, count) {
                    $('table tr').each(function () {
                        var otExDXTotal = parseFloat($(this).children().eq(11).text()),
                            warningValue = parseFloat($(this).children().eq(12).text());

                        if (otExDXTotal > warningValue) 
                            $(this).children().toggleClass("ws-bg-warning");
                    });
                }
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

            mGet.GetUserOvertime(action, stn, mid, '', '', queryType, keyword, startDate, endDate);        
        }
    };

    
    //页面初始化默认显示所有用户
    mGet.GetUserOvertime('View', STN, MID, '', '', 'Dept', 'All', StartDate, EndDate);

    $('.layui-btn').on('click', function () {
        var type = $(this).data('type');
        mGet[type] ? mGet[type].call(this) : '';
    });

    $('.micro-click').on('click', function () {
        var type = $(this).data('type');
        mGet[type] ? mGet[type].call(this) : '';
    });

    //触发排序，超出允许加班最大值时标注背景为红色
    table.on('sort(tabTable)', function (obj) {
        $('.layui-table-body table tr').each(function () {

            var otExDXTotal = parseFloat($(this).children().eq(12).text()),
                warningValue = parseFloat($(this).children().eq(13).text());

            if (!isNaN(otExDXTotal) && !isNaN(warningValue) && otExDXTotal > warningValue) {
                $(this).children().toggleClass("ws-bg-warning");
            }
        });
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