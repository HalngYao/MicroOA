layui.use(['index', 'jquery', 'laydate', 'micro'], function () {
    var $ = layui.$
        , form = layui.form
        , laydate = layui.laydate
        , micro = layui.micro;

    var MID = micro.getUrlPara('mid');

    laydate.render({
        elem: '#txtDate'
        , btns: ['now', 'confirm']
        , trigger: 'click'
    });

    laydate.render({
        elem: '#txtTime'
        , btns: ['now', 'confirm']
        , trigger: 'click'
        , type: 'time'
        , format: 'HH:mm'
        , ready: function (date) {
            var dom = $(".laydate-time-list").children("li");
            for (var i = 0; i < dom.length; i++) {
                if (i == 2 || i == 5)
                    $(dom[i]).remove();
            }

        }
    });


    form.verify({
        txtCustomStateName: [
            /^[\S]{0,5}$/
            , '自定义状态不能超过5个字符 <br/>カスタムステータスは5文字を超えてはいけません <br/>Custom state cannot exceed 5 characters'
        ]
    });


    form.on('select(selStateName)', function (data) {
        var val = data.value;

        if (val == 200) {
            $('#divCustomStateName').show(100);
            $('#txtCustomStateName').attr('lay-verify', 'required');
        } else {
            $('#divCustomStateName').hide(100);
            $('#txtCustomStateName').removeAttr('lay-verify', 'required');
        }
    });

    form.on('select(selDurationTime)', function (data) {
        var val = data.value;

        if (val == 'Custom') {
            $("#divCustomDateTime").removeClass('layui-hide');
            $("#divCustomDateTime").show(100);
        }
        else
            $("#divCustomDateTime").hide(100);

    });

    //保存
    form.on('submit(btnSave)', function (data) {
        var selStateName = data.field.selStateName,
            txtCustomStateName = data.field.txtCustomStateName,
            selDurationTime = data.field.selDurationTime,
            txtDate = data.field.ctl00$ContentPlaceHolder1$txtDate,
            txtTime = data.field.ctl00$ContentPlaceHolder1$txtTime;

        var stateCode = selStateName, stateName;

        if (stateCode == 200)
            stateName = txtCustomStateName;
        else if (stateCode == 100)
            stateName = "有空";
        else if (stateCode == 101)
            stateName = "会议中";
        else if (stateCode == 102)
            stateName = "外出了";

        var parm = { "type": "custom", "statecode": stateCode, "statename": stateName, "durationtime": selDurationTime, "customdurationtime": txtDate + ' ' + txtTime };
        micro.getAjaxData('text', '/Views/UserCenter/UserState.ashx', parm, true);

        $('#citeState', window.parent.document).html(stateName);

        if (stateCode == 100)
            $('#spanSetate', window.parent.document).addClass('ws-bg-green');
        else
            $('#spanSetate', window.parent.document).removeClass('ws-bg-green');

        parent.layer.closeAll('iframe');

    });

});