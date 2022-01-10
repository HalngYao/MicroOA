layui.use(['index', 'jquery', 'table', 'form', 'layer', 'laydate', 'micro'], function () {
    var $ = layui.$
        , table = layui.table
        , form = layui.form
        , layer = layui.layer
        , laydate = layui.laydate
        , micro = layui.micro;

    var Action = $('#txtAction').val();
    var MID = micro.getUrlPara('mid');
    var STN = $('#txtShortTableName').val();
    var FormID = $('#txtFormID').val();
    var FormsID = $('#txtFormsID').val();
    var EditTablePermit = $('#txtEditTablePermit').val();
    var Edit = EditTablePermit === '1' ? 'text' : '';

    var mGet = {

        GetTable: function (formID, formsID) {
            table.render({
                elem: '#tabTable'
                , url: '/Views/Forms/HR/OvertimeFormList.ashx?stn=overtime&formid=' + formID + '&QueryType=OvertimeID:int&Keyword=' + formsID + '&Mainsub=Sub&r=' + Math.random()
                //, toolbar: true
                //, defaultToolbar: 'default'
                , even: true
                , cols: [
                    [
                        { type: 'checkbox', fixed: 'left', rowspan: 2 }
                        //, { field: 'MainSub', width: 90, title: 'MainSub', align: 'center', fixed: 'left', rowspan: 2 }
                        , { field: 'OvertimeID', title: 'OvertimeID', width: 80, align: 'center', rowspan: 2, fixed: 'left', hide: true }
                        , { field: 'DeptName', title: '部门', width: 150, align: 'center', edit: Edit, rowspan: 2, fixed: 'left' }
                        , { field: 'OvertimeDisplayName', title: '姓名', width: 180, align: 'center', rowspan: 2, fixed: 'left' }
                        , { field: 'Location', title: '加班地点', width: 160, align: 'center', edit: Edit, rowspan: 2 }
                        , { field: 'Reason', title: '加班理由', width: 200, align: 'center', rowspan: 2 }
                        , { align: 'left', title: '加班安排', colspan: 4 }
                        , { align: 'left', title: '周期内加班情况', colspan: 5 }
                        , { field: 'OvertimeMealName', title: '加班就餐与否', width: 130, align: 'center', edit: Edit, rowspan: 2 }
                        //, { field: 'UseCar', title: '特殊用车', width: 120, align: 'center', edit: Edit, rowspan: 2 }
                    ],
                    [
                        //表格属性tb
                        { field: 'Alias', title: '班次', width: 100, align: 'center', edit: Edit, rowspan: 2 }
                        , { field: 'OvertimeDate', title: '日期', width: 150, align: 'center', edit: Edit }
                        , { field: 'OvertimeTime', title: '时间', width: 150, align: 'center', edit: Edit }
                        , { field: 'OvertimeHour', title: '加班小时（H）', width: 130, align: 'center', edit: Edit }
                        , { field: 'WorkHourSysName', title: '工时制', width: 100, align: 'center', edit: Edit }
                        , { field: 'OvertimeTotal', title: '总加班时间（H）', width: 140, align: 'center', edit: Edit }
                        , { field: 'AlreadyDaiXiu', title: '已代休时间（H）', width: 140, align: 'center', edit: Edit }
                        , { field: 'ExceptDaiXiu', title: '剔除代休后加班时间（H）', width: 200, align: 'center', edit: Edit }
                        , { field: 'WarningHour', title: '预警提醒', width: 110, align: 'center', edit: Edit }  //WarningTips
                    ]
                ]

                //, page: true
                //, limit: 100
                //, limits: [10, 20, 30, 50, 100]
                //, data: ['']//micro.getAjaxData('json', micro.getRootPath() + '/Views/Set/System/MicroDataTableList.ashx', tabPara)["data"]
                //, height: 'full-120'
                , size: 'sm'  //sm
                , text: {
                    none: '暂无相关数据。No relevant data.'
                }
                , done: function (res, curr, count) {
                    $('#txtRecord').val(count);
                    $('table tr').each(function () {
                        var otExDXTotal = parseFloat($(this).children().eq(13).text()),
                            warningValue = parseFloat($(this).children().eq(14).text());

                        if (!isNaN(otExDXTotal) && !isNaN(warningValue) && otExDXTotal > warningValue) {
                            $(this).children().toggleClass("ws-bg-warning");
                        }
                    });
                }
            });
        },

        //添加
        btnAddOpenLink: function () {

            var formsID = $("#txtFormsID").val();

            parent.layer.open({
                type: 2
                , title: '添加加班内容'
                , content: '/Views/Forms/HR/OvertimeForm/Add/' + STN + '/' + MID + '/' + FormID + '/' + formsID
                , maxmin: true
                , area: ['95%', '90%']
                , btn: ['保存', '关闭']
                , skin: 'micro-layer-class2'
                , moveOut: true
                , yes: function (index, layero) {
                    var iframeWindow = window['layui-layer-iframe' + index]
                        , submitID = 'btnSave'
                        , submit = layero.find('iframe').contents().find('#' + submitID);

                    $(submit).click();

                }
                , end: function () {
                    var State = $(window.parent.document).find('#txtState').val(), //$('#txtState').val(),
                        formID = $('#txtFormID').val(),
                        formsID = $('#txtFormsID').val(),
                        gID = $(window.parent.document).find('#txtGID').val();

                    if (State == 'True') {
                        if (formsID == '') {
                            parent.layer.closeAll('iframe');
                            mGet.layerOpen(Action, STN, MID, FormID, gID);
                        }
                        else
                            mGet.GetTable(formID, formsID);

                        $(window.parent.document).find('#txtState').val('False');
                    }
                }
            });
        },

        //复制添加
        btnCopyAddOpenLink: function () {

            var checkStatus = table.checkStatus('tabTable');
            var data = checkStatus.data;  //得到选中用户

            if (data.length == 0) {
                layer.msg('请选中一条记录<br/>レコードを選択してください<br/>Please select a record');
                return false;
            }
            else if (data.length > 1) {
                layer.msg('复制添加只能选中一条记录<br/>レコードを一つだけ選択します<br/>Select only one record');
                return false;
            }
            else {

                var formsID = $("#txtFormsID").val();
                var overtimeID = encodeURI(micro.getStrSplice(data, 'OvertimeID'));
                parent.layer.open({
                    type: 2
                    , title: '复制添加加班内容'
                    , content: '/Views/Forms/HR/OvertimeForm/Add/' + STN + '/' + MID + '/' + FormID + '/' + formsID + '/' + encodeURI(overtimeID)
                    , maxmin: true
                    , area: ['95%', '90%']
                    , btn: ['保存', '关闭']
                    , skin: 'micro-layer-class2'
                    , moveOut: true
                    , yes: function (index, layero) {
                        var iframeWindow = window['layui-layer-iframe' + index]
                            , submitID = 'btnSave'
                            , submit = layero.find('iframe').contents().find('#' + submitID);

                        $(submit).click();

                    }
                    , end: function () {
                        var State = $(window.parent.document).find('#txtState').val(),
                            formID = $('#txtFormID').val(),
                            formsID = $('#txtFormsID').val(),
                            gID = $(window.parent.document).find('#txtGID').val();

                        if (State == 'True') {
                            if (formsID == '') {
                                parent.layer.closeAll('iframe');
                                mGet.layerOpen(Action, STN, MID, FormID, gID);
                            }
                            else
                                mGet.GetTable(formID, formsID);

                            $(window.parent.document).find('#txtState').val('False');
                        }
                    }
                });
            }
        },

        btnModifyOpenLink: function () {

            var checkStatus = table.checkStatus('tabTable');
            var data = checkStatus.data;  //得到选中用户

            if (data.length == 0) {
                layer.msg('没有选中任何记录 <br/>レコードが選択されていません<br/>No record selected');
                return false;
            }
            else if (data.length > 50) {
                layer.msg('每次修改操作不能超过50条记录 <br/>Each modification operation cannot be greater than 50 records');
                return false;
            }
            else {
                var formsID = $("#txtFormsID").val();
                var overtimeID = encodeURI(micro.getStrSplice(data, 'OvertimeID'));
                parent.layer.open({
                    type: 2
                    , title: '修改加班内容'
                    , content: '/Views/Forms/HR/OvertimeForm/Modify/' + STN + '/' + MID + '/' + FormID + '/' + formsID + '/' + encodeURI(overtimeID)
                    , maxmin: true
                    , area: ['95%', '90%']
                    , btn: ['保存', '关闭']
                    , skin: 'micro-layer-class2'
                    , moveOut: true
                    , yes: function (index, layero) {
                        var iframeWindow = window['layui-layer-iframe' + index]
                            , submitID = 'btnModify'
                            , submit = layero.find('iframe').contents().find('#' + submitID);

                        $(submit).click();
                    }
                    , end: function () {
                        var State = $(window.parent.document).find('#txtState').val(),
                            formID = $('#txtFormID').val(),
                            fromsID = $("#txtFormsID").val();
                        if (State == 'True') {
                            //location.replace('/Views/Forms/HR/OvertimeFormList/' + Action + '/' + STN + '/' + MID + '/' + FormID + '/' + fromsID);
                            mGet.GetTable(formID, fromsID);
                            $(window.parent.document).find('#txtState').val('False');
                        }
                    }
                });
            }

        },

        btnDel: function () {

            var checkStatus = table.checkStatus('tabTable');
            var data = checkStatus.data;

            if (data.length == 0) {
                layer.msg('没有选中任何记录 <br/>レコードが選択されていません<br/>No record selected');
                return false;
            }
            else {
                layer.confirm('确认删除吗？<br/>削除を確認しますか？<br/>Are you sure to delete?', {
                    btn: ['是', '否']
                }, function () {
                    var overtimeID = encodeURI(micro.getStrSplice(data, 'OvertimeID')),
                        shortTableName = $("#txtShortTableName").val();
                    var Parameter = { "action": "Dels", "type": "MicroForm", "stn": shortTableName, "mid": MID, "pkv": overtimeID };
                    micro.mAjax('text', micro.getRootPath() + '/App_Handler/CtrlPublicGenService.ashx', Parameter, true, true);
                }, function () {
                    //return false;
                });
            }

        },

        layerOpen: function (action, stn, mid, formID, formsID) {
            parent.layer.open({  //parent.layer.open //在父层弹出
                type: 2
                , title: '填写《加班申请表》申请/Fill in the application'
                , content: '/Views/Forms/HR/OvertimeFormList/' + action + '/' + stn + '/' + mid + '/' + formID + '/' + formsID
                , area: ['95%', '90%']
                , btn: ['立即提交', '关闭']
                , skin: 'micro-layer-class'
                //, moveOut: true
                , yes: function (index, layero) {
                    var submitID = 'btnSave'
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
        },

        SetWorkFlow: function () {

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

    };

    //页面初始加载Table
    mGet.GetTable(FormID, FormsID);

    $('.micro-click').on('click', function () {
        var type = $(this).data('type');
        mGet[type] ? mGet[type].call(this) : '';
    });

    form.on('submit(btnSave)', function (data) {

        var action = $('#txtAction').val();
        var stn = $('#txtShortTableName').val();
        var mid = $('#txtMID').val();
        var formID = $('#txtFormID').val();
        var formsID = $('#txtFormsID').val();
        var isapprovalForm = 'True';
        var record = parseInt($('#txtRecord').val());  //$('#txtRecord').val() //是否有记录

        //请先填写加班申请内容
        if (formsID == "" || formsID == "0" || $.trim(formsID).length == 0 || isNaN(record) || record <= 0) {
            $('#btnAddOpenLink').addClass('btnTips');

            layer.msg('请先填写内容<br/>まず内容を記入してください<br/>Please fill in the content first', {
                icon: 5
                , anim: 6
                , time: 1000
            }, function () { $('#btnAddOpenLink').removeClass('btnTips'); });

            return false;
        } else {
            var Fields = JSON.stringify(data.field); Fields = Fields.replace(/ctl00\$ContentPlaceHolder1\$/g, "");
            Fields = encodeURI(Fields);

            var parameter = { "action": action, "stn": stn, "mid": mid, "formid": formID, "formsid": formsID, "isapprovalform": isapprovalForm, "fields": Fields };

            micro.mAjax('text', '/Views/Forms/HR/CtrlOvertimeFormList.ashx', parameter, true, false, true);
        }
    });


    form.on('submit(btnModify)', function (data) {

        var action = $('#txtAction').val();
        var stn = $('#txtShortTableName').val();
        var mid = $('#txtMID').val();
        var formID = $('#txtFormID').val();
        var formsID = $('#txtFormsID').val();
        var isapprovalForm = "True";
        var record = parseInt($('#txtRecord').val());  //$('#txtRecord').val() //是否有记录

        if (formsID == "" || formsID == "0" || $.trim(formsID).length == 0 || isNaN(record) || record <= 0) {
            $('#btnAddOpenLink').addClass('btnTips');

            layer.msg("请先填写内容<br/>まず内容を記入してください<br/>Please fill in the content first", {
                icon: 5
                , anim: 6
                , time: 1000
            }, function () { $('#btnAddOpenLink').removeClass('btnTips'); });

            return false;
        } else {
            var Fields = JSON.stringify(data.field); Fields = Fields.replace(/ctl00\$ContentPlaceHolder1\$/g, "");
            Fields = encodeURI(Fields);

            var parameter = { "action": action, "stn": stn, "mid": mid, "formid": formID, "formsid": formsID, "isapprovalform": isapprovalForm, "fields": Fields };
            micro.mAjax('text', '/Views/Forms/HR/CtrlOvertimeFormList.ashx', parameter, true, false, true);
        }
    });
});