layui.use(['index', 'jquery', 'form', 'layer', 'micro'], function () {
    var $ = layui.$
        , form = layui.form
        , layer = layui.layer
        , micro = layui.micro;

    var MID = $('#txtMID').val();

    var mGet = {
        getLevel: function (val) {
            var Fields = { "stn": "Dept", "idname": "DeptID", "val": val };
            Fields = encodeURI(JSON.stringify(Fields));  
            var Parameter = { "action": "get", "mid": MID, "fields": Fields };
            var Value = micro.getAjaxData('json', micro.getRootPath() + '/App_Handler/GetParSubLevel.ashx', Parameter, false);
            $('#txtLevelCode').val(Value['LevelCode']);
            $('#txtLevel').val(Value['Level']);
            $('#txtSort').val(Value['Sort']);
        }
    };

    mGet.getLevel(0);

    form.on('select(selParentID)', function (data) {
        mGet.getLevel(data.value);
    });
});