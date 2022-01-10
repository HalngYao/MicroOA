layui.use(['index', 'jquery', 'form', 'table', 'micro'], function () {
    var $ = layui.$
        , form = layui.form
        , table = layui.table
        , micro = layui.micro;

    var MID = $("#txtMID").val();

    //*****监听checkbox全选事件*****
    form.on('checkbox(cbAll)', function (obj) {
        $("input[type=checkbox]").each(function () {
            if (obj.elem.checked) {
                $(this).prop("checked", true);
            } else {
                if (!$(this).prop("disabled"))
                    $(this).prop("checked", false);
            }
        });
        form.render();
    });

    //*****监听checkbox全选事件*****
    form.on('checkbox(cbModuleCheckAll)', function (obj) {
        $("#" + obj.elem.value + " input[type=checkbox]").each(function () {
            if (obj.elem.checked) {
                $(this).prop("checked", true);
            } else {
                if (!$(this).prop("disabled"))
                    $(this).prop("checked", false);
            }

        });
        form.render();
    });

    //保存提交
    form.on('submit(btnSave)', function (data) {
        var Fields = JSON.stringify(data.field); 
        Fields = Fields.replace(/ctl00\$ContentPlaceHolder1\$/g, "");
        Fields = encodeURI(Fields);
        var Parameter = { "action": "modify", "mid": MID, "type": "save", "fields": Fields };
        micro.mAjax('text', micro.getRootPath() + '/Views/UserCenter/ModulePerms.ashx', Parameter);
    });

    //设置默认
    form.on('submit(btnSetDefault)', function (data) {
        var Fields = JSON.stringify(data.field);  //获取提交的字段
        Fields = Fields.replace(/ctl00\$ContentPlaceHolder1\$/g, "");
        Fields = encodeURI(Fields);
        var Parameter = { "action": "modify", "mid": MID, "type": "default", "fields": Fields };
        micro.mAjax('text', micro.getRootPath() + '/Views/UserCenter/ModulePerms.ashx', Parameter);
    });

    //恢复默认
    form.on('submit(btnRecovery)', function (data) {
        var Fields = JSON.stringify(data.field);  //获取提交的字段
        Fields = Fields.replace(/ctl00\$ContentPlaceHolder1\$/g, "");
        Fields = encodeURI(Fields);
        var Parameter = { "action": "modify", "mid": MID, "type": "recovery", "fields": Fields };
        micro.mAjax('text', micro.getRootPath() + '/Views/UserCenter/ModulePerms.ashx', Parameter);
    });

    var mGet = {

        btnOpenLink: function () {
            var microText = $(this).attr('micro-text')
            layer.open({
                type: 2
                , title: microText
                //, content: '/Views/UserCenter/Perms/' + MID
                , content: '/Views/Forms/SysFormList/View/Perm/' + MID
                , maxmin: true
                , area: ['100%', '100%']
                , scrollbar: false
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

    $('.layui-btn').on('click', function () {
        var type = $(this).data('type');
        mGet[type] ? mGet[type].call(this) : '';
    });

});