layui.use(['index', 'jquery', 'form', 'table', 'micro'], function () {
    var $ = layui.$
        , form = layui.form
        , table = layui.table
        , micro = layui.micro;

    //页面初始化
    var MID = $("#txtMID").val();
    var STN = $("#txtSTN").val();
    var FormID = $("#txtFormID").val();
    var WFID = $("#txtWFID").val();
    var EffectiveType = $('#selEffectiveType').val();
    var EffectiveIDStr = $('#hidEffectiveIDStr').val();

    var EditTablePermit = $('#txtEditTablePermit').val();
    var Edit = EditTablePermit === '1' ? 'text' : '';

    var mGet = {
        getFlowNode: function (wfid) {
            var Parameter = { "action": "getflownode", "mid": MID, "wfid": wfid };
            table.render({
                elem: '#tabTable'
                , cols: [[
                    { type: 'checkbox', fixed: 'left' }
                    , { field: 'WFID', width: 100, title: '主键ID', align: 'center', fixed: 'left', hide: true }
                    , { field: 'Sort', width: 100, title: '审批顺序', align: 'center', fixed: 'left', edit: Edit  }
                    , { field: 'FlowName', width: 140, title: '节点名称', align: 'center', fixed: 'left', edit: Edit }
                    , { field: 'FlowCode', width: 180, title: '节点代码<i class=\'layui-icon layui-icon-about\' lay-tips="节点名称有固定的代码请不要随意变动，申请=Apply、受理=Accept、完成=Finish、所有审批节点=Approval"></i>', align: 'center', edit: Edit }
                    , { field: 'ApprovalNameStr', width: 180, title: '主要审批者', align: 'center', templet: '#tplApprovalNameStr' }

                    , { field: 'ApprovalIDStrSort', width: 160, title: '主要选中顺序<i class=\'layui-icon layui-icon-about\' lay-tips="主要审批者默认选中的顺序配合“默认选中审批者”开关使用，按职位的升序或降序（升序：按职位的低到高，降序：则返之，该功能主要用于默认选中。但是无论如何主要审批者和代理审批者如果存在都会被寻找出来只是默认选中谁的问题），默认升序，默认值：False"></i>', align: 'center', templet: '#swApprovalIDStrSort' }

                    , { field: 'ApprovalByNameStr', width: 300, title: '代理审批者<i class=\'layui-icon layui-icon-about\' lay-tips="通常是主要审批者无法匹配时，配合寻找范围工作：如果范围是“自部门”时会根据代理审批者选定的职称由低到高逐级匹配，如果是范围是“跨部门”时则寻找出所有代理审批者"></i>', align: 'center', templet: '#tplApprovalByNameStr' }

                    , { field: 'ApprovalByIDStrSort', width: 160, title: '代理选中顺序<i class=\'layui-icon layui-icon-about\' lay-tips="代理审批者默认选中的顺序配合“默认选中审批者”开关使用，按职位的升序或降序（升序：按职位的低到高，降序：则返之，该功能主要用于默认选中。但是无论如何主要审批者和代理审批者如果存在都会被寻找出来只是默认选中谁的问题），默认升序，默认值：False"></i>', align: 'center', templet: '#swApprovalByIDStrSort' }

                    , { field: 'ApproversSelectedByDefault', width: 180, title: '默认选中审批者<i class=\'layui-icon layui-icon-about\' lay-tips="在默认选中状态下，通常审批者个数只有唯一的一个时才选中"></i>', align: 'center', templet: '#swApproversSelectedByDefault' }

                    , { field: 'IsOptionalApproval', width: 180, title: '显示可选审批者<i class=\'layui-icon layui-icon-about\' lay-tips="通常主要审批者或代理审批者都无法匹配时才显示可选审批者"></i>', align: 'center', templet: '#swIsOptionalApproval' }

                    //寻找审批者方向暂时用不上了，暂时先隐藏
                    //, { field: 'IsVerticalDirection', width: 180, title: '寻找审批者方向<i class=\'layui-icon layui-icon-about\' lay-tips="寻找审批者的方向，横向、纵向，通常审批节点如班长、系长、科长、部长等应该是纵向寻找，而机密委员、IT委员等非正常职称应该是横向寻找。默认纵向，默认值【D】：true，如果是特殊审批则不受此限制"></i>', align: 'center', templet: '#swIsVerticalDirection' }

                    , { field: 'FindRange', width: 180, title: '寻找范围<i class=\'layui-icon layui-icon-about\' lay-tips="查找审批者范围，自部门或跨部门。<br/>自部门：如我所在是IT系，则我的自部门仅仅为IT系，而总务科事业管理部则为我的父部门。<br/>跨部门：即在整个部门级别内跨领域，如我所在是IT系，可以查找到人事系，总务系。默认自部门，默认值【D】：false"></i>', align: 'center', templet: '#swFindRange' }

                    , { field: 'FindGMOffice', width: 180, title: '寻找总经理办<i class=\'layui-icon layui-icon-about\' lay-tips="查找审批者范围追加总经理办公室，如需要总经理审批时请启用此选项。默认值【D】：false"></i>', align: 'center', templet: '#swFindGMOffice' }

                    , { field: 'IsSpecialApproval', width: 180, title: '是否特殊审批<i class=\'layui-icon layui-icon-about\' lay-tips="特殊审批如访问对象部门审批在未填写内容时无确认对象是谁，此时系统根据JobTitle职位职称表中启用“作为审批者”的成员作为后选人，但同时该节点设定的主要审批者和代理审批者也将变为无效"></i>', align: 'center', templet: '#swIsSpecialApproval' }
                    , { field: 'IsConditionApproval', width: 180, title: '启用条件审批', align: 'center', templet: '#swIsConditionApproval' }
                    , { field: 'OperField', width: 180, title: '运算字段', align: 'center', edit: Edit  }
                    , { field: 'Condition', width: 300, title: '运算条件', align: 'center', edit: Edit  }
                    , { field: 'OperValue', width: 300, title: '运算值', align: 'center', edit: Edit }
                    , { field: 'Description', width: 600, title: '节点描述', align: 'left', edit: Edit }
                    , { fixed: 'right', width: 120, title: '操作', toolbar: '#barTools', align: 'center', width: 100 }

                ]]
                , data: micro.getAjaxData('json', micro.getRootPath() + '/Views/Forms/FlowList.ashx', Parameter)["data"]
                , text: { none: 'No Data.没有数据。' }

            });

        }

        , ChangeApproval: function () {

            var microID = $(this).attr("micro-id");
            var microType = $(this).attr("micro-type");
            var microField = $(this).attr("micro-field");
            var microValue = $(this).attr("micro-value");
            var microText = $(this).attr("micro-text");

            layer.open({
                type: 2
                , title: microText
                , content: '/Views/Forms/ChangeApproval/View/' + STN + '/' + MID + '/' + microID + '/' + microType + '/' + microField + '/' + microValue
                , maxmin: true
                , area: ['95%', '90%']
                , btn: ['立即提交', '关闭']
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
        //条件审批设置
        , getTextName: function (TabColName, Selected) {
            var Fields = { "txt": "TabColName,Title", "val": "TabColName", "stn": "mTabs", "tcn": "" + TabColName + "", "ob": "Sort" };
            micro.getTxtVal('get', 'selOperField', micro.getRootPath() + '/App_Handler/GetPublicTextValue.ashx', Fields, Selected);
        }

    };

    //表单名称
    var Parameter0 = { "action": "get", "type": "form", "mid": MID, "DefaultValue": FormID };
    var microXmSelect0 = xmSelect.render({
        el: '#microXmSelect0',
        name: 'selFormID',
        language: 'zn',
        autoRow: false,
        filterable: true,
        showCount: 10,
        pageSize: 10,
        searchTips: 'Keyword',
        paging: true,
        height: 'auto',
        radio: true,
        clickClose: true,
        layVerify: 'required',
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
        data: eval(micro.getAjaxData('text', micro.getRootPath() + '/App_Handler/GetUserPublicInfoForXmSelect.ashx', Parameter0)),
        on: function (data) {
            if (data.arr[0] !== 'undefined' && typeof data.arr[0] !== 'undefined') {
                var stn = data.arr[0]["stn"];
                if (stn !== '' && stn !== null)
                    mGet.getTextName(stn, '');
                else {
                    document.getElementById('selOperField').options.length = 1;
                    form.render();
                }
            } else {
                document.getElementById('selOperField').options.length = 1;
                form.render();
            }
        }
    });


    //生效范围的值
    var Parameter1 = { "action": "get", "type": EffectiveType, "mid": MID, "DefaultValue": EffectiveIDStr };
    var microXmSelect1 = xmSelect.render({
        el: '#microXmSelect1',
        name: 'selEffectiveIDStr',
        language: 'zn',
        autoRow: false,
        filterable: true,
        showCount: 10,
        pageSize: 10,
        searchTips: 'Keyword',
        paging: true,
        tree: {
            show: true,  //是否显示树状结构
            showFolderIcon: true,  //是否展示三角图标
            showLine: true,  //是否显示虚线
            indent: 20,  //间距
            expandedKeys: true, //默认展开的节点数组[-3], 为true时展开所有节点
            strict: false, //是否遵循严格父子结构
            simple: true, //极简模式
        },
        height: 'auto',
        data: eval(micro.getAjaxData('text', micro.getRootPath() + '/App_Handler/GetUserPublicInfoForXmSelect.ashx', Parameter1))

    });

    //生效范围Change事件
    form.on('select(selEffectiveType)', function (data) {

        Parameter1 = { "action": "get", "type": "" + data.value + "", "mid": MID };
        microXmSelect1.update({
            tree: {
                show: data.value == 'Dept' ? true : false,
            },
            data: eval(micro.getAjaxData('text', micro.getRootPath() + '/App_Handler/GetUserPublicInfoForXmSelect.ashx', Parameter1))
        });

        switch (data.value) {
            case 'Dept':
                $('.EffectiveType').text('不指定部门时全公司生效');
                break;
            case 'JTitle':
                $('.EffectiveType').text('不指定职位时全公司生效');
                break;
            case 'Role':
                $('.EffectiveType').text('不指定角色时全公司生效');
                break;
            case 'Use':
                $('.EffectiveType').text('不指定人员时全公司生效');
                break;
        }

    });  

    //主要审批人
    var Parameter2 = { "action": "get", "type": "JTitle", "mid": MID };
    var microXmSelect2 = xmSelect.render({
        el: '#microXmSelect2',
        name: 'selApprovalIDStr',
        language: 'zn',
        autoRow: false,
        filterable: true,
        showCount: 5,
        pageSize: 5,
        tips: '主要审批人--请选择',
        searchTips: 'Keyword',
        paging: true, //分页
        max: 8,
        //radio: true,
        //layVerify:'required',
        height: 'auto',
        data: eval(micro.getAjaxData('text', micro.getRootPath() + '/App_Handler/GetUserPublicInfoForXmSelect.ashx', Parameter2))

    });

    //代理审批人
    var Parameter3 = { "action": "get", "type": "JTitle", "mid": MID };
    var microXmSelect3 = xmSelect.render({
        el: '#microXmSelect3',
        name: 'selApprovalByIDStr',
        language: 'zn',
        autoRow: false,
        filterable: true,
        showCount: 5,
        pageSize: 5,
        tips: '代理审批人--请选择',
        searchTips: 'Keyword',
        paging: true,
        max: 8,
        height: 'auto',
        data: eval(micro.getAjaxData('text', micro.getRootPath() + '/App_Handler/GetUserPublicInfoForXmSelect.ashx', Parameter3))

    });

    //审批人Change事件
    form.on('select(selApprovalType)', function (data) {

        Parameter2 = { "action": "get", "type": "" + data.value + "", "mid": MID };
        microXmSelect2.update({
            cascader: {
                show: data.value == 'DeptUsers' ? true : false,
                indent: 250,
                strict:true
            },
            data: eval(micro.getAjaxData('text', micro.getRootPath() + '/App_Handler/GetUserPublicInfoForXmSelect.ashx', Parameter2))
        });

        Parameter3 = { "action": "get", "type": "" + data.value + "", "mid": MID };
        microXmSelect3.update({
            cascader: {
                show: data.value == 'DeptUsers' ? true : false,
                indent: 250,
                strict: true
            },
            data: eval(micro.getAjaxData('text', micro.getRootPath() + '/App_Handler/GetUserPublicInfoForXmSelect.ashx', Parameter3))
        });

    }); 

    form.on('checkbox(ckIsConditionApproval)', function (data) {
        if (data.elem.checked == true) {
            $('#selOperField').prop('disabled', false);
            $('#selCondition').prop('disabled', false);
            $('#txtOperValue').prop('disabled', false);
            $('#txtOperValue').removeClass('layui-disabled');
            form.render();
        }
        else {
            $('#selOperField').prop('disabled', true);
            $('#selOperField').val('');

            $('#selCondition').prop('disabled', true);
            $('#selCondition').val('');

            $('#txtOperValue').prop('disabled', true);
            $('#txtOperValue').val('');
            $('#txtOperValue').addClass('layui-disabled');
            form.render();
        }

    });  

    //页面初始化
    if (STN !== '')
        mGet.getTextName(STN, '');

    //页面初始化，修改流程时
    if (WFID !== '') {
        mGet.getFlowNode(WFID);
        microXmSelect0.update({ disabled: true });
    }

    //监听单元格编辑
    var Path = micro.getRootPath() + '/Views/Forms/Flow.ashx';

    table.on('edit(tabTable)', function (obj) {
        var data = obj.data, //所在行所有键值
            FieldName = obj.field, //字段名称
            FieldValue = obj.value; //修改后的值
        var Fields = { 'STN': 'WFlow', 'IDName': 'WFID', 'IDValue': data.WFID, 'FieldName': FieldName, 'FieldValue': FieldValue };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modifynote', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);

    });

    //监听Switch
    form.on('switch(ApprovalIDStrSort)', function (obj) {
        var Fields = { 'STN': 'WFlow', 'IDName': 'WFID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modifynote', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });

    form.on('switch(ApprovalByIDStrSort)', function (obj) {
        var Fields = { 'STN': 'WFlow', 'IDName': 'WFID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modifynote', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });

    form.on('switch(IsConditionApproval)', function (obj) {
        var Fields = { 'STN': 'WFlow', 'IDName': 'WFID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modifynote', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });

    form.on('switch(ApproversSelectedByDefault)', function (obj) {
        var Fields = { 'STN': 'WFlow', 'IDName': 'WFID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modifynote', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });

    form.on('switch(IsOptionalApproval)', function (obj) {
        var Fields = { 'STN': 'WFlow', 'IDName': 'WFID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modifynote', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });

    form.on('switch(IsVerticalDirection)', function (obj) {
        var Fields = { 'STN': 'WFlow', 'IDName': 'WFID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modifynote', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });

    form.on('switch(FindRange)', function (obj) {
        var Fields = { 'STN': 'WFlow', 'IDName': 'WFID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modifynote', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });

    form.on('switch(FindGMOffice)', function (obj) {
        var Fields = { 'STN': 'WFlow', 'IDName': 'WFID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modifynote', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });

    form.on('switch(IsSpecialApproval)', function (obj) {
        var Fields = { 'STN': 'WFlow', 'IDName': 'WFID', 'IDValue': this.value, 'FieldName': this.name, 'FieldValue': obj.elem.checked };
        Fields = encodeURI(JSON.stringify(Fields));
        var Parameter = { 'action': 'modifynote', 'mid': MID, 'fields': Fields };
        micro.mAjax('text', Path, Parameter);
    });

    //添加节点
    form.on('submit(btnAddNode)', function (data) {

        var Fields = JSON.stringify(data.field);
        Fields = Fields.replace(/ctl00\$ContentPlaceHolder1\$/g, "");
        Fields = encodeURI(Fields);

        var Approved = data.field.selApprovalIDStr;  //主要审批人
        if (Approved === '') {
            layer.msg('必填项不能为空<br/>Required item cannot be empty', {
                icon: 5
                , anim: 6
            });
            $('#microXmSelect2 xm-select').css("border-color", "rgb(229, 77, 66)");
            return false;
        }
        else {
            //第一次打开页面，新增情况
            if ($("#txtWFID").val() === '') {
                var Parameter = { "action": "add", "mid": MID, "fields": Fields };
                var flag = micro.getAjaxData('text', micro.getRootPath() + '/Views/Forms/Flow.ashx', Parameter);

                if (flag.substring(0, 4) === 'True') {
                    layer.msg('添加成功<br/>Successfully Add', {
                        icon: 1
                        , anim: 5
                    }, function () {
                        var wfid = flag.substring(4, flag.length);
                        $("#txtWFID").val(wfid);
                        mGet.getFlowNode(wfid);
                        microXmSelect0.update({ disabled: true });
                        $('#txtState').val('True');

                    });
                } else {
                    layer.msg(flag, {
                        icon: 5
                        , anim: 6
                    });
                }
            //新增完成，页面回发情况
            } else {
                var Parameter = { "action": "add2", "mid": MID, "fields": Fields };
                var flag = micro.getAjaxData('text', micro.getRootPath() + '/Views/Forms/Flow.ashx', Parameter);
                if (flag.substring(0, 4) === 'True') {
                    layer.msg('添加成功<br/>Successfully Add', {
                        icon: 1
                        , anim: 5
                    }, function () {
                            mGet.getFlowNode($("#txtWFID").val());
                            $('#txtState').val('True');
                    });
                } else {
                    layer.msg(flag, {
                        icon: 5
                        , anim: 6
                    });
                }
            }
        }

    });

    //保存修改
    form.on('submit(btnModify)', function (data) {
        var Fields = JSON.stringify(data.field);
        Fields = Fields.replace(/ctl00\$ContentPlaceHolder1\$/g, "");
        Fields = encodeURI(Fields);

        var wfid = data.field.ctl00$ContentPlaceHolder1$txtWFID;
        if (wfid === '' || wfid === 'undefined' || typeof wfid === 'undefined') {
            $('#btnAddNode').addClass('btnTips');

            layer.msg('请先添加审批节点<br/>Please add approval node', {
                icon: 5
                , anim: 6
                , time: 1000
            }, function () { $('#btnAddNode').removeClass('btnTips'); });
            return false;
        } else {
            var Parameter = { "action": "modify", "mid": MID, "fields": Fields };
            micro.mAjax('text', micro.getRootPath() + '/Views/Forms/Flow.ashx', Parameter);
        }
    });

    //监听行工具事件
    table.on('tool(tabTable)', function (obj) {
        var data = obj.data;
        if (obj.event === 'Del') {
            layer.confirm('确认删除吗？<br/>Are you sure to delete?', function (index) {

                var Parameter = { "action": "del", "mid": MID, "wfid": data.WFID };
                micro.mAjax('text', micro.getRootPath() + '/Views/Forms/Flow.ashx', Parameter);

                obj.del();
                layer.close(index);
            });
        } 
    });

    $('.micro-click').on('click', function () {
        var type = $(this).data('type');
        mGet[type] ? mGet[type].call(this) : '';
    });

});