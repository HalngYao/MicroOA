layui.use(['index', 'jquery', 'layer', 'micro'], function () {
    var $ = layui.$
        , form = layui.form
        , layer = layui.layer
        , micro = layui.micro;


    //刷新验证码
    $("#imgCode").click(function () {
        $("#imgCode").attr('src', micro.getRootPath() + '/Resource/ValidateCode/ValidateImage?' + Math.random());
    });

    //提交
    form.on('submit(btnSave)', function (obj) {
        var field = obj.field;
        var oForm = obj.form;

        if (field.txtUserName.length <= 0) {
            $(oForm.txtUserName).focus();
            $(oForm.txtUserName).addClass('layui-form-danger');
            var flag = $(oForm.txtUserName).attr('micro-tips');
            layer.msg(flag, {
                icon: 5
                , anim: 6
                , offset: 't'
                , skin: 'ws-margin-top10'
            });
            return false;
        };

        if (field.txtUserPassword.length <= 0) {
            $(oForm.txtUserPassword).focus();
            $(oForm.txtUserPassword).addClass('layui-form-danger');
            var flag = $(oForm.txtUserPassword).attr('micro-tips');
            layer.msg(flag, {
                icon: 5
                , anim: 6
                , offset: 't'
                , skin: 'ws-margin-top10'
            });
            return false;
        } else if (!(/^[\S]{8,16}$/).test(field.txtUserPassword)) {
            $(oForm.txtUserPassword).focus();
            $(oForm.txtUserPassword).addClass('layui-form-danger');
            var flag = $(oForm.txtUserPassword).attr('micro-tips');
            layer.msg(flag, {
                icon: 5
                , anim: 6
                , offset: 't'
                , skin: 'ws-margin-top10'
            });
            return false;

        };

        if (field.txtUserPassword !=field.txtConfirmPassword) {
            $(oForm.txtConfirmPassword).focus();
            $(oForm.txtConfirmPassword).addClass('layui-form-danger');
            var flag = $(oForm.txtConfirmPassword).attr('micro-tips');
            layer.msg(flag, {
                icon: 5
                , anim: 6
                , offset: 't'
                , skin: 'ws-margin-top10'
            });
            return false;
        };

        if (field.txtEMail.length <= 0) {
            $(oForm.txtEMail).focus();
            $(oForm.txtEMail).addClass('layui-form-danger');
            var flag = $(oForm.txtEMail).attr('micro-tips');
            layer.msg(flag, {
                icon: 5
                , anim: 6
                , offset: 't'
                , skin: 'ws-margin-top10'
            });
            return false;
        } else if (!(/^([a-zA-Z]|[0-9])(\w|\-)+@[a-zA-Z0-9\-]+\.([a-zA-Z]{2,4})$/).test(field.txtEMail)) {
            $(oForm.txtEMail).focus();
            $(oForm.txtEMail).addClass('layui-form-danger');
            var flag = '邮箱地址格式不正确。<br/> The email address format is incorrect';
            layer.msg(flag, {
                icon: 5
                , anim: 6
                , offset: 't'
                , skin: 'ws-margin-top10'
            });
            return false;

        };

        if (field.txtValidateCode.length <= 0) {
            $(oForm.txtValidateCode).focus();
            $(oForm.txtValidateCode).addClass('layui-form-danger');
            var flag = $(oForm.txtValidateCode).attr('micro-tips');
            layer.msg(flag, {
                icon: 5
                , anim: 6
                , offset: 't'
                , skin: 'ws-margin-top10'
            });
            return false;
        }

        //是否同意用户协议
        if (!field.agreement) {
            return layer.msg('你必须同意用户协议才能注册 <br/>You must agree to the user agreement to register',{
                icon: 5,
                anim: 6
                , offset: 't'
                , skin: 'ws-margin-top10'
            }, function () {
                return false;
            });
        }

        var Fields = { "fields": encodeURI(JSON.stringify(field)) };
        var flag = micro.getAjaxData('text', micro.getRootPath() + '/Views/UserCenter/Register.ashx', Fields, true);

        if (flag.substring(0, 4) === 'True') {
            layer.msg(flag.substring(4, flag.length), {
                icon: 1
                , offset: 't'
                , skin: 'ws-margin-top10'
            }, function () {
                location.replace(micro.getRootPath() + '/Views/UserCenter/Login');
            });
        } else {
            layer.msg(flag, {
                icon: 5,
                anim: 6
                , offset: 't'
                , skin: 'ws-margin-top10'
            }, function () {
                return false;
            });
        }

        return false;
    });

    $(document).keydown(function (e) {
        if (e.keyCode === 13) {
            $(".layui-btn").click();
        }
    });
});