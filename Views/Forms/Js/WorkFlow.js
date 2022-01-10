layui.use(['index', 'jquery', 'form', 'table', 'micro'], function () {
    var $ = layui.$
        , form = layui.form
        , table = layui.table
        , micro = layui.micro;

    //页面初始化
    var STN = $("#txtSTN").val();
    var MID = $("#txtMID").val();
    var FormID = $("#txtFormID").val();
    
    var mGet = {
        getWorkFlow: function (mid) {
            table.render({
                elem: '#tabTable'
                , cols: [[
                    { field: 'DefaultFlow', width: 80, title: '默认', align: 'center', templet: '#swDefaultFlow', fixed: 'left' }
                    , { field: 'WFID', width: 100, title: '主键ID', align: 'center', hide:true }
                    , { field: 'Sort', width: 100, title: '排序', align: 'center', edit:'text' }
                    , { field: 'FlowName', title: '流程名称', align: 'center', edit: 'text' }
                    , { field: 'EffectiveNameStr', title: '生效范围', align: 'center' }
                    , { field: 'IsAccept', width: 100, title: '启用受理', align: 'center', templet: '#swIsAccept' }
                    , { field: 'IsSync', width: 100, title: '同步审批', align: 'center', templet: '#swIsSync' }
                    , { field: 'Creator', width: 200, title: '创建人', align: 'center' }
                    , { field: 'DateCreated', width: 200, title: '创建时间', align: 'center' }
                    , { fixed: 'right', title: '操作', toolbar: '#barTools', width: 150, align: 'center', fixed: 'right' }

                ]]
                , page: true
                , limit: 10
                , limits: [10, 20, 30, 50, 100]
                , data: micro.getAjaxData('json', micro.getRootPath() + '/Views/Forms/WorkFlowList.ashx', { 'action':'getworkflow','mid':mid})["data"]
                , text: { none: 'No Data.没有数据。' }

            });
        }

        , btnAddOpenLink: function () {
            var stn = $("#txtSTN").val();
            stn = stn === '' ? 'micro' : stn;
            var formName = microXmSelect.getValue('name');
            var microText = '添加【' + formName + '】的审批流程';
            layer.open({
                type: 2
                , title: microText
                , content: '/Views/Forms/Flow/Add/' + stn + '/' + MID + '/' + $("#txtFormID").val()
                , maxmin: true
                , area: ['100%', '100%']
                , scrollbar: false
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

    };

    mGet.getWorkFlow(FormID = FormID == '' ? 1 : FormID);

    var Parameter = { "action": "get", "type": "form", "mid": MID, "DefaultValue": FormID };
    var microXmSelect = xmSelect.render({
        el: '#microXmSelect',
        name: 'selFormID',
        language: 'zn',
        autoRow: true,
        filterable: true,
        showCount: 10,
        pageSize: 10,
        searchTips: 'Keyword',
        paging: true,
        height: 'auto',
        radio: true,
        clickClose: true,
        model: {
            label: {
                type: 'block',
                block: {
                    //最大显示数量, 0:不限制
                    showCount: 10,
                    //是否显示删除图标
                    showIcon: false,
                }
            }
        },
        data: eval(micro.getAjaxData('text', micro.getRootPath() + '/App_Handler/GetUserPublicInfoForXmSelect.ashx', Parameter)),
        on: function (data) {
            if (data.arr[0] !== 'undefined' && typeof data.arr[0] !== 'undefined') {
                var v = data.arr[0]["value"];  //value=FormID
                if (v !== "" && v !== null)
                    mGet.getWorkFlow(v);

                $("#txtFormID").val(v);
                $("#txtSTN").val(data.arr[0]["stn"]);
            }
        }

    });

    var Path = micro.getRootPath() + '/Views/Forms/WorkFlow.ashx';

    table.on('edit(tabTable)', function (obj) {
        var data = obj.data, //所在行所有键值
            FieldName = obj.field, //字段名称
            FieldValue = obj.value; //修改后的值
        var Fields = { 'IDValue': data.WFID, 'FieldName': FieldName, 'FieldValue': FieldValue };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter, true, false ,false);

    });

    form.on('checkbox(DefaultFlow)', function (obj) {
        var Fields = { 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked};
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter, true, false, false);
    });

    form.on('switch(IsAccept)', function (obj) {
        var Fields = { 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter, true, false, false);
    });

    form.on('switch(IsSync)', function (obj) {
        var Fields = { 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked};
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modify', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter, true, false, false);
    });

    //监听行工具事件
    table.on('tool(tabTable)', function (obj) {
        var data = obj.data;
        if (obj.event === 'Del') {
            layer.confirm('确认删除吗？<br/>Are you sure to delete?', function (index) {

                var Parameter = { "action": "del", "mid": MID, "wfid": data.WFID };
                micro.mAjax('text', micro.getRootPath() + '/Views/Forms/WorkFlow.ashx', Parameter, true, false, false);

                obj.del();
                layer.close(index);
            });
        } else if (obj.event === 'Modify') {
            var formName = microXmSelect.getValue('name');
            layer.open({
                type: 2
                , title: '修改【' + formName + '】的审批流程'
                , content: '/Views/Forms/Flow/Modify/' + STN + '/' + MID + '/' + data.FormID + '/' + data.WFID
                , maxmin: true
                , area: ['100%', '100%']
                , scrollbar: false
                , btn: ['保存修改', '关闭']
                , yes: function (index, layero) {
                    var iframeWindow = window['layui-layer-iframe' + index]
                        , submitID = 'btnModify'
                        , submit = layero.find('iframe').contents().find('#' + submitID);

                    $(submit).click();
                }
                , btn2: function (index, layero) {
                    var State = layero.find('iframe').contents().find('#txtState').val();
                    if (State == "True")
                        location.reload();
                }
                , cancel: function (index, layero) {
                    var State = layero.find('iframe').contents().find('#txtState').val();
                    if (State == "True")
                        location.reload();
                }
            });
        }
    });

    $('.micro-click').on('click', function () {
        var type = $(this).data('type');
        mGet[type] ? mGet[type].call(this) : '';
    });

});