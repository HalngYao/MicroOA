layui.use(['index', 'form', 'layer', 'jquery', 'micro'], function () {
    var form = layui.form
        , $ = layui.jquery
        , layer = layui.layer
        , micro = layui.micro;

    var MID = $("#txtMID").val();

    //监听测试按钮
    form.on('submit(btnTest)', function (data) {
        var Fields = JSON.stringify(data.field);
        Fields = Fields.replace(/ctl00\$ContentPlaceHolder1\$/g, "");
        Fields = encodeURI(Fields);
        var Parameter = { "action": "test", "mid": MID, "fields": Fields };
        micro.mAjax('text', micro.getRootPath() + '/Views/UserCenter/SyncDomainUsers.ashx', Parameter, true);
    });


    //监听同步按钮
    form.on('submit(btnSave)', function (data) {
        var Fields = JSON.stringify(data.field);
        Fields = Fields.replace(/ctl00\$ContentPlaceHolder1\$/g, "");
        Fields = encodeURI(Fields);
        var Parameter = { "action": "sync", "mid": MID, "fields": Fields };
        micro.mAjax('text', micro.getRootPath() + '/Views/UserCenter/SyncDomainUsers.ashx', Parameter, true);
    });

  

});