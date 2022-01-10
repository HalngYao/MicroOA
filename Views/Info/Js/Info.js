layui.use(['index', 'jquery', 'form', 'layer', 'flow', 'util', 'micro'], function () {
    var $ = layui.$
        , form = layui.form
        , layer = layui.layer
        , flow = layui.flow
        , util = layui.util
        , micro = layui.micro;

    var MID = micro.getUrlPara('mid');
    $('#txtKeyword').val($('#txtAuxKeyword').val());
    
    var mGet = {

        Approval: function () {
            var microSTN = $(this).attr('micro-stn');
            var microID = $(this).attr('micro-id');

            layer.prompt({
                formType: 1
                , title: '敏感操作，请验证密码'
            }, function (value, index) {
                layer.close(index);

                if (micro.verifyPWD(value) === 'True') {
                    layer.confirm('审批后该信息将全员可见，确定审批吗？', function (index) {
                        var Parameter = { "action": "Modify", "type": "Approval", "stn": microSTN, "mid": MID, "pkv": microID };
                        micro.mAjax('text', micro.getRootPath() + '/App_Handler/CtrlPublicGenService.ashx', Parameter, true, true);
                    });
                }
            });
        }

        , CtrlTop: function () {
            var microType = $(this).attr('micro-type');
            var microSTN = $(this).attr('micro-stn');
            var microID = $(this).attr('micro-id');
            var microVal = $(this).attr('micro-val');
            console.log(MID);

            layer.confirm('确定进行此操作吗？', function (index) {
                var Parameter = { "action": "CtrlTop", "type": microType, "stn": microSTN, "mid": MID, "pkv": microID, "val": microVal };
                micro.mAjax('text', micro.getRootPath() + '/App_Handler/CtrlPublicGenService.ashx', Parameter, true, true);
            });

        }

        , Del: function () {
            var microSTN = $(this).attr('micro-stn');
            var microID = $(this).attr('micro-id');

            layer.prompt({
                formType: 1
                , title: '敏感操作，请验证密码'
            }, function (value, index) {
                layer.close(index);

                if (micro.verifyPWD(value) === 'True') {
                    layer.confirm('确定删除吗？', function (index) {
                        var Parameter = { "action": "Del", "type": "SystemForm", "stn": microSTN, "mid": MID, "pkv": microID };
                        micro.mAjax('text', micro.getRootPath() + '/App_Handler/CtrlPublicGenService.ashx', Parameter, true, true);
                    });
                }
            });
        }

        , Search: function () {

            if ($('#txtKeyword').length > 0) {
                var keyword = $.trim($('#txtKeyword').val()),
                    infoClassID = $.trim($('#txtInfoClassID').val()),
                    url = '/Views/Info/List';

                if (keyword !== "" && keyword !== "undefined")
                    location.replace(url + '/' + infoClassID + '/' + encodeURI(keyword));
                else
                    location.replace(url);

            }
        }
    };


    flow.load({
        elem: '#ulList'
        , done: function (page, next) {
            setTimeout(function () {
                var lis = [];

                $.getJSON('/Views/Info/List.ashx', { action: 'view', infoclassid: $('#txtInfoClassID').val(), page: page, keyword: encodeURI($('#txtAuxKeyword').val()) }, function (res) {
                    layui.each(res.data, function (index, item) {
                        lis.push(util.unescape(item.Item).replace(/&quot;/gi, '"'));
                    });

                    next(lis.join(''), page < res.pagecount);
                });
                micro.setWaterMark();
            }, 500);
        }
    });

    //设置水印
    micro.setWaterMark();

    //禁止视频右键下载
    $('.video01').bind('contextmenu', function () {
        return false;
    })

    //禁止图片右键
    $('img').on('contextmenu', function () { return false; });
    //禁止拖动
    $('img').on('dragstart', function () { return false; });


    //delegate应用于未来的事件
    $('.layui-container').delegate('.micro-click', 'click', function () {
        var type = $(this).data('type');
        mGet[type] ? mGet[type].call(this) : '';
    });

    //delegate应用于未来的事件
    $("blockquote").addClass('ws-elem-quote');


    $(document).keydown(function (e) {
        if (e.keyCode === 13) {
            $('.micro-search').click();
            return false;
        }
    });

    
});