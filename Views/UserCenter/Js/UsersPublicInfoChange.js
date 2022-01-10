layui.use(['index', 'jquery', 'form', 'layer', 'laydate', 'micro'], function () {
    var $ = layui.$
        , form = layui.form
		, layer = layui.layer
		, laydate = layui.laydate
        , micro = layui.micro;

	var MID = micro.getUrlPara('mid');
	var UIDS = $("#txtUIDS").val();
	var Type = $("#txtType").val();

	laydate.render({
		elem: '#txtDate'
		, trigger: 'click'
	});

	var mGet = {
		btnModify: function () {
			//得到选中的值
			var checkedData = microXmSelect.getValue("valueStr");
			if (checkedData.length > 0) {

				var Fields = encodeURI(JSON.stringify(checkedData).replace(/\"/g, ""));
				var Parameter = { "action": "modify", "type": Type, "mid": MID, "uids": UIDS, "fields": Fields, "date": $("#txtDate").val() };
				micro.mAjax('text', micro.getRootPath() + '/Views/UserCenter/UsersPublicInfoChange.ashx', Parameter);

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

	var Parameter = { "action": "get", "type": Type, "mid": MID };
	var microXmSelect = xmSelect.render({
		el: '#microXmSelect',
		language: 'zn',
		autoRow: true,
		filterable: true,
		radio: $("#txtType").val() == 'UWorkHourSystem' ? true : false,
		tree: {
			show: true,  //是否显示树状结构
			showFolderIcon: true,  //是否展示三角图标
			showLine: true,  //是否显示虚线
			indent: 20,  //间距
			expandedKeys: true, //默认展开的节点数组[-3], 为true时展开所有节点
			strict: false, //是否遵循严格父子结构
		},
		toolbar: {
			show: true,
			list: ['ALL', 'REVERSE', 'CLEAR']
		},
		filterable: true,
		height: '600px',
		data: eval(micro.getAjaxData('text', micro.getRootPath() + '/App_Handler/GetUserPublicInfoForXmSelect.ashx', Parameter))

	});

    $('.layui-btn').on('click', function () {
        var type = $(this).data('type');
        mGet[type] ? mGet[type].call(this) : '';
    });


});