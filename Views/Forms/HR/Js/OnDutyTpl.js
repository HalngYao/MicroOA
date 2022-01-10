layui.use(['index', 'jquery', 'table', 'form', 'layer', 'laydate', 'upload', 'micro'], function () {
    var $ = layui.$
        , form = layui.form
        , layer = layui.layer
        , laydate = layui.laydate
        , micro = layui.micro;

    var Action = $("#txtAction").val();
    var MID = micro.getUrlPara('mid');
    var STN = $("#txtShortTableName").val();
    var UserDepts = $("#UserDepts").val();

    var Parameter = { "action": "get", "type": "Depts", "mid": MID, "DefaultValue": UserDepts };
    var microXmSelect = xmSelect.render({
        el: '#microXmSelect',
        name: 'txtUserDepts',
        language: 'zn',
        autoRow: true,
        filterable: true,
        layVerify: 'required',
        //disabled:true,
        tree: {
            show: true,  //是否显示树状结构
            showFolderIcon: true,  //是否展示三角图标
            showLine: true,  //是否显示虚线
            indent: 20,  //间距
            expandedKeys: true, //默认展开的节点数组[-3], 为true时展开所有节点
            strict: true, //是否遵循严格父子结构
            simple: true, //是否开启极简模式
        },
        toolbar: {
            show: true,
            list: ['ALL', 'REVERSE', 'CLEAR']
        },
        model: {
            label: {
                type: 'block',
                block: {
                    //最大显示数量, 0:不限制
                    showCount: 3,
                    //是否显示删除图标
                    showIcon: true,
                }
            }
        },
        filterable: true,
        height: '300px',
        data: eval(micro.getAjaxData('text', micro.getRootPath() + '/App_Handler/GetUserPublicInfoForXmSelect.ashx', Parameter)),
        on: function (data) {
            var deptIDs = $.map(data.arr, function (n) {
                return n.value;
            }).join();
            
            $('#UserDepts').val(deptIDs);
        }

    });


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
        }

    };

    mGet.getStartDate();
    mGet.getEndDate($('#txtStartDate').val());

    form.on('submit(btnDownload)', function (data) {

        var deptIDs = microXmSelect.getValue('valueStr');
        var startDate = $("#txtStartDate").val();
        var endDate = $("#txtEndDate").val();
        var userObj = data.field.cbObject;

        //console.log(userObj);
        //console.log(microXmSelect.getTreeValue());

        layer.load();
        window.location.href = '/Views/Forms/HR/OnDutyTpl.ashx?DeptIDs=' + deptIDs + '&StartDate=' + startDate + '&EndDate=' + endDate + '&UserObj=' + userObj;
        setTimeout(function () { layer.closeAll('loading') }, 3000);

    });

});