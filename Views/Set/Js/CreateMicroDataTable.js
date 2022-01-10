layui.use(['index', 'jquery', 'form', 'micro'], function () {
    var $ = layui.$
        , form = layui.form
        , micro = layui.micro;

    var MID = micro.getUrlPara('mid');

    //CreateDataTable
    form.on('submit(btnAddTab)', function (data) {
        var Fields = JSON.stringify(data.field);  //获取提交的字段
        Fields = Fields.replace(/ctl00\$ContentPlaceHolder1\$/g, "");
        Fields = encodeURI(Fields);
        var Parameter = { "action": "add", "mid": MID, "fields": Fields };
        micro.mAjax('text', micro.getRootPath() + '/Views/Set/System/CreateMicroDataTable.ashx', Parameter);
    });

});