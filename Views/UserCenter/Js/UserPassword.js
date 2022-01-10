layui.use(['index', 'jquery', 'table', 'layer', 'upload', 'micro'], function () {
    var $ = layui.$
        , form = layui.form
        , table = layui.table
        , layer = layui.layer
        , upload = layui.upload
        , micro = layui.micro;

    var MID = micro.getUrlPara('mid');

    form.verify({
         NewPassword: [
            /^[\S]{8,16}$/
            , '密码必须8到16位，且不能出现空格 <br/> The password must be 8 to 16 digits and no spaces are allowed'
        ]
        , ConfirmPassword: function (value, item) {
            var NewUserPassword = $('#txtNewUserPassword').val();
            if (NewUserPassword != value) {
                return '两次输入的密码不一致。<br/>Two inputted password inconsistencies';
            }
        }

    });  

    //保存修改
    form.on('submit(btnModify)', function (data) {
        var Fields = JSON.stringify(data.field);
        Fields = Fields.replace(/ctl00\$ContentPlaceHolder1\$/g, "");
        var Parameter = { "action": "modify", "mid": MID, "fields": Fields };
        micro.mAjax('text', micro.getRootPath() + '/Views/UserCenter/UserPassword.ashx', Parameter, true, true, false);
    });

});