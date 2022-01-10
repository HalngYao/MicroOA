layui.config({
    base: '/layuiadmin/' //静态资源所在路径
}).extend({
    index: 'lib/index' //主入口模块
}).use(['index', 'jquery', 'layer', 'util', 'dropdown', 'micro'], function () {
    $ = layui.$,
        layer = layui.layer,
        util = layui.util,
        dropdown = layui.dropdown,
        micro = layui.micro;

    //高级演示 - 各种组合
    dropdown.render({
        elem: '#citeState'
        , trigger: 'hover'
        , data: [{
            title: '有空'
            , templet: '<span class="layui-badge-dot ws-bg-green"></span> {{d.title}}'
            , id: 100
        }, {
            title: '会议中'
            , templet: '<span class="layui-badge-dot"></span> {{d.title}} '
            , id: 101
        }, {
            title: '外出了'
            , templet: '<span class="layui-badge-dot"></span> {{d.title}} '
            , id: 102
        }, { type: '-' }, {
            title: '自定义'
            , templet: '{{d.title}} '
            , id: 200
        }]

        , click: function (item) {
            var id = item.id,
                title = item.title;

            if (id < 200) {

                if (item.id == 100)
                    $('#spanSetate').addClass('ws-bg-green');
                else
                    $('#spanSetate').removeClass('ws-bg-green');

                $('#citeState').html(title);

                var parm = { "type": "fixed", "statecode": id, "statename": title };
                micro.mAjaxNoMsg('text', '/Views/UserCenter/UserState.ashx', parm);

            }
            else if (id == 200) {

                layer.open({
                    type: 2
                    , title: '自定义状态'
                    , content: '/Views/UserCenter/UserState'
                    , area: ['56%', '62%']
                    , btn: ['保存【Save】', '关闭【Close】']
                    , skin: 'micro-layer-class'
                    , yes: function (index, layero) {  //按钮一
                        var iframeWindow = window['layui-layer-iframe' + index]
                            , submitID = 'btnSave'
                            , submit = layero.find('iframe').contents().find('#' + submitID);

                        $(submit).click();
                    }
                });

            }
            return false;

        }
    });


    //页面重定向
    var url = micro.getUrlPara('url');
    if (url !== 'undefined' && url !== '') {
        var topLayui = parent === self ? layui : top.layui;
        topLayui.index.openTabsPage(decodeURIComponent(url), 'Redirect');
    }
 
    //检查浏览器版本
    micro.checkBrowser($('#hidCheckBrowser').val(), $('#hidUnsupportedBrowserTips').val());

    //全局提示
    var GlobalTips = $('#hidGlobalTips').val();
    if (GlobalTips === 'True') {
        layer.open({
            type: 2
            , title: '提示消息 / Prompt message'
            , content: '/Views/Info/GlobalTips'
            , maxmin: false
            , area: ['36%', '40%']
            , btn2: function (index, layero) {

            }
            , cancel: function (index, layero) {
            }
        });
    }

});