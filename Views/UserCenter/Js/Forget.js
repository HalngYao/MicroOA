layui.use(['index', 'jquery', 'layer', 'micro'], function () {
    var $ = layui.$
        , form = layui.form
        , layer = layui.layer
        , micro = layui.micro;

    form.verify({
         EMail: function (value, item) {
            if (value.length <= 0) {
                return '邮箱地址不能为空。<br/> Email address cannot be empty';
            }
            var reg = /^([a-zA-Z]|[0-9])(\w|\-)+@[a-zA-Z0-9\-]+\.([a-zA-Z]{2,4})$/;
            if (!reg.test(value))
                return '邮箱地址格式不正确。<br/> The email address format is incorrect';

        }
        , ValidateCode: function (value, item) {
            if (value.length < 1) {
                return '请输入验证码 <br/> Please input verification code';
            }
        }
    });  

    //刷新验证码
    $("#imgCode").click(function () {
        $("#imgCode").attr('src', micro.getRootPath() + '/Resource/ValidateCode/ValidateImage.aspx?' + Math.random());
    });

    //提交
    form.on('submit(btnSave)', function (obj) {
        var field = obj.field;

        //是否同意用户协议
        if (!field.agreement) {
            return layer.msg('该功能还没有开放。<br/>The function is not yet open.');
        }

        ////请求接口
        //admin.req({
        //    url: layui.setter.base + 'json/user/reg.js' //实际使用请改成服务端真实接口
        //    , data: field
        //    , done: function (res) {
        //        layer.msg('注册成功', {
        //            offset: '15px'
        //            , icon: 1
        //            , time: 1000
        //        }, function () {
        //            location.hash = '/user/login'; //跳转到登入页
        //        });
        //    }
        //});

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

});