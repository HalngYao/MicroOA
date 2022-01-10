layui.use(['index', 'jquery', 'form', 'layer', 'micro'], function () {
    var $ = layui.$
        , form = layui.form
        , layer = layui.layer
        , micro = layui.micro;

    //刷新验证码
    $("#imgCode").click(function () {
        $("#imgCode").attr('src', micro.getRootPath() + '/Resource/ValidateCode/ValidateImage?' + Math.random());
    });

    //检查浏览器版本
    micro.checkBrowser($('#hidCheckBrowser').val(), $('#hidUnsupportedBrowserTips').val());

    //登录
    form.on('submit(btnLogin)', function (obj) {
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
        };

        if ($('#txtValidateCode').length > 0) {
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
        }

        var Fields = { "action": "login", "fields": encodeURI(JSON.stringify(field)) };

        $.ajax({
            async: true,
            type: 'post',
            dataType: 'text',
            url: micro.getRootPath() + '/Views/UserCenter/CheckLogin.ashx',
            data: Fields,
            beforeSend: function () {
                layer.load(2);
            },
            success: function (data) {
                flag = data;
                if (flag.substring(0, 4) === 'True') {
                    //url参数不为空时跳转
                    var url = micro.getUrlPara("url");
                    if (url !== "" && url !== "undefined")
                        location.replace(url);
                    else {
                        url = micro.getRootPath() + '/Views/Default';
                        //个人主页不为空时跳转
                        var homePage = decodeURIComponent(flag.substring(4, flag.length));
                        if (homePage !== "" && homePage !== "undefined" && /^http(s*):\/\//.test(homePage))
                            location.replace(homePage);
                        else //跳转到默认主页
                            location.replace(url);
                    }
                }
                else {
                    layer.msg(flag, {
                        icon: 5,
                        anim: 6
                        , offset: 't'
                        , skin: 'ws-margin-top10'
                    }, function () {
                        layer.close(layer.load(2));
                        return false;
                    });
                    return false;
                }
                layer.close(layer.load(2));
            },
            error: function () {
                flag = "error,出错了";
                layer.close(layer.load(2));
            }
        });

        return false;
    });

    //Win登录
    $("#hlWinLogin").click(function () {
        var field = { "cbAutoLogin": $("#cbAutoLogin").prop('checked') == true ? "on" : "" };
        var Fields = { "action": "winlogin", "fields": encodeURI(JSON.stringify(field)) };

        $.ajax({
            async: true,
            type: 'post',
            dataType: 'text',
            url: micro.getRootPath() + '/Views/UserCenter/CheckLogin.ashx',
            data: Fields,
            beforeSend: function () {
                layer.load(2);
            },
            success: function (data) {
                flag = data;

                if (flag.substring(0, 4) === 'True') {
                    //url参数不为空时跳转
                    var url = micro.getUrlPara("url");
                    if (url !== "" && url !== "undefined")
                        location.replace(url);
                    else {
                        url = micro.getRootPath() + '/Views/Default';
                        //个人主页不为空时跳转
                        var homePage = decodeURIComponent(flag.substring(4, flag.length));
                        if (homePage !== "" && homePage !== "undefined" && /^http(s*):\/\//.test(homePage))
                            location.replace(homePage);
                        else //跳转到默认主页
                            location.replace(url);
                    }
                }
                else {
                    layer.msg(flag, {
                        icon: 5,
                        anim: 6
                        , offset: 't'
                        , skin: 'ws-margin-top10'
                    }, function () {
                        layer.close(layer.load(2));
                        return false;
                    });
                    return false;
                }
                layer.close(layer.load(2));
            },
            error: function () {
                flag = "error,出错了";
                layer.close(layer.load(2));
            }
        });

        return false;
    });

    $(document).keydown(function (e) {
        if (e.keyCode === 13) {
            $(".layui-btn").click();
        }
    });

});