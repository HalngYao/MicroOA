layui.use(['index', 'jquery', 'layer', 'laydate', 'micro'], function () {
    var $ = layui.$
        , form = layui.form
        , layer = layui.layer
        , micro = layui.micro;

    var MID = $("#txtMID").val();

    form.on('submit(btnSave)', function (data)
    {
        CheckUserName = { "action": "CheckUser", "fields": data.field.txtUserName };
        var flag = micro.getAjaxData('text', micro.getRootPath() + '/Views/UserCenter/CheckUser.ashx', CheckUserName, true);
        if (flag.substring(0, 4) !== 'True') {
            layer.msg(flag, {
                icon: 5
                , anim: 6
            });
            return false;
        };

        CheckEMail = { "action": "CheckEMail", "fields": data.field.txtEMail };
        var flag = micro.getAjaxData('text', micro.getRootPath() + '/Views/UserCenter/CheckUser.ashx', CheckEMail, true);
        if (flag.substring(0, 4) !== 'True') {
            layer.msg(flag, {
                icon: 5
                , anim: 6
            });
            return false;
        };

        var Fields = JSON.stringify(data.field);
        Fields = Fields.replace(/ctl00\$ContentPlaceHolder1\$/g, "");
        Fields = encodeURI(Fields);
        var Action = "add",STN = "use";
        var Parameter = { "action": Action, "stn": STN, "mid": MID, "fields": Fields };
        micro.mAjax('text', '/App_Handler/CtrlMicroForm.ashx', Parameter, true, false);

    });

});