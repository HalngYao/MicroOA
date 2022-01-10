layui.use(['index', 'jquery', 'table', 'layer', 'tree', 'micro'], function () {
    var $ = layui.$
        , form = layui.form
        , table = layui.table
        , layer = layui.layer
        , tree = layui.tree
        , micro = layui.micro;

	var MID = $("#txtMID").val();
	var WFID = $("#txtWFID").val();
	var Type = $("#txtType").val();
	var FieldName = $("#txtFieldName").val();
	var DefaultValue = $("#txtDefaultValue").val();

	var mGet = {
		btnModify: function () {
			var checkedData = microXmSelect.getValue("valueStr");
			if (checkedData.length > 0) {
				var Fields = encodeURI(JSON.stringify(checkedData).replace(/\"/g, ""));
				var Parameter = { "action": "modify", "mid": MID, "wfid": WFID, "fieldname": FieldName, "fields": encodeURI(Fields) };
				micro.mAjax('text', micro.getRootPath() + '/Views/Forms/ChangeApproval.ashx', Parameter);
			}
			else {
				layer.msg('至少选择一项<br/>Select at least one', {
					icon: 5
					, anim: 6
				});
				return false
			}
		}
	};

	var Parameter = { "action": "get", "type": Type, "mid": MID, "DefaultValue": encodeURI(DefaultValue) };
	var microXmSelect = xmSelect.render({
		el: '#microXmSelect',
        language: 'zn',
		autoRow: true,
		filterable: true,
		showCount: 10,
		pageSize: 10,
		tips: '主要审批人--请选择',
		searchTips: 'Keyword',
		paging: true, //分页
		max: 8,
		//layVerify:'required',
		toolbar: {
			show: true,
			list: ['ALL', 'REVERSE', 'CLEAR']
		},
		filterable: true,
		height: 'auto',
		cascader: {
			show: Type == 'DeptUsers' ? true : false,
			indent: 250,
			strict: true
		},
		data: eval(micro.getAjaxData('text', micro.getRootPath() + '/App_Handler/GetUserPublicInfoForXmSelect.ashx', Parameter))

	});

    $('.layui-btn').on('click', function () {
        var type = $(this).data('type');
        mGet[type] ? mGet[type].call(this) : '';
    });


});