layui.use(['index', 'jquery', 'form', 'table', 'layer', 'micro'], function () {
    var  $ = layui.$
        , form = layui.form
        , table = layui.table
        , layer = layui.layer
        , micro = layui.micro;

    var MID = micro.getUrlPara('mid');

    var mGet = {
        //*****左侧导航Start*****
        GetMgr: function () {
            var microSTN = $(this).attr('micro-stn');
            var microText = $(this).attr('micro-text')
            layer.open({
                type: 2
                , title: microText
                , content: '/Views/Forms/SysFormList/View/' + microSTN + '/' + MID
                , maxmin: true
                , area: ['95%', '90%']
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

        , Add: function () {
            var microSTN = $(this).attr('micro-stn');
            var microText = $(this).attr('micro-text')
            layer.open({
                type: 2
                , title: microText
                , content: '/Views/Forms/SysForm/Add/' + microSTN + '/' + MID
                , maxmin: true
                , area: ['95%', '90%']
                , btn: ['立即提交', '关闭']
                , skin: 'micro-layer-class'
                , yes: function (index, layero) {
                    var iframeWindow = window['layui-layer-iframe' + index]
                        , submitID = 'btnSave'
                        , submit = layero.find('iframe').contents().find('#' + submitID);

                    $(submit).click();
                }
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

        , Modify: function () {
            var microText = $(this).attr('micro-text');
            var microSTN = $(this).attr('micro-stn');
            var microID = $(this).attr('micro-id');
            layer.open({
                type: 2
                , title: '修改【' + microText + '】信息'
                , content: '/Views/Forms/SysForm/Modify/' + microSTN + '/' + MID + '/' + microID
                , maxmin: true
                , area: ['95%', '90%']
                , btn: ['保存修改', '关闭']
                , yes: function (index, layero) {
                    var iframeWindow = window['layui-layer-iframe' + index]
                        , submitID = 'btnModify'
                        , submit = layero.find('iframe').contents().find('#' + submitID);

                    $(submit).click();
                }
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

        //左侧菜单点选调用
        , GetForms: function () {
            var microData = $(this).attr('micro-data');
            var microType = $(this).attr('micro-type');
            micro.getTableAttributes('View', 'forms', microData, microType);
        }
        //*****左侧导航End*****

        , btnAddOpenLink: function () {
            layer.open({
                type: 2
                , title: '添加表单'
                , content: '/Views/Forms/FormsForm/Add/Forms/' + MID
                , maxmin: true
                , area: ['95%', '90%']
                , btn: ['立即提交', '关闭']
                , skin: 'micro-layer-class'
                , yes: function (index, layero) {
                    var iframeWindow = window['layui-layer-iframe' + index]
                        , submitID = 'btnSave'
                        , submit = layero.find('iframe').contents().find('#' + submitID);

                    $(submit).click();
                }
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

    //页面初始化默认显示所有记录
    micro.getTableAttributes('View', 'forms');

    $('.layui-btn').on('click', function () {
        var type = $(this).data('type');
        mGet[type] ? mGet[type].call(this) : '';
    });

    $('.micro-click').on('click', function () {
        var type = $(this).data('type');
        mGet[type] ? mGet[type].call(this) : '';
    });

    //提交表单
    form.on('submit(btnSearch)', function (data) {
        var keyword = data.field.txtKeyword;
        if (!micro.isEmpty(keyword)) {
            micro.getTableAttributes('View', 'forms', encodeURI(keyword));
            return false;
        } else
            micro.getTableAttributes('View', 'forms');
    });

    
    //监听工具条
    var viewRefresh = false,  //第一次打开选项卡不刷新
        draftRefresh = false;
    table.on('tool(tabTable)', function (obj) {
        var data = obj.data;

        if (obj.event === 'Add') {
            var linkAddress = data.LinkAddress;
            linkAddress = linkAddress == '' ? '/Views/Forms/MicroForm' : linkAddress;
            layer.open({  //parent.layer.open //在父层弹出
                type: 2
                , title: '填写《' + data.FormName + '》申请/Fill in the application'
                , content: linkAddress + '/Add/' + data.ShortTableName + '/' + MID + '/' + data.FormID
                , area: ['95%', '90%']
                , btn: ['立即提交', '关闭']
                , skin: 'micro-layer-class'
                //, moveOut: true
                , yes: function (index, layero) {
                    var iframeWindow = window['layui-layer-iframe' + index]
                        , submitID = 'btnSave'
                        , submit = layero.find('iframe').contents().find('#' + submitID);

                    $(submit).click();

                }
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
        };

        if (obj.event === 'View') {

            var formID = data.FormID,
                parms = data.ListParms;
   
            if (!micro.isEmpty(parms)) 
                parms = '/1/DefaultNumber/' + parms;

            var url = '/Views/Forms/MicroFormList/View/' + data.ShortTableName + '/' + MID + '/' + formID + parms;
            var topLayui = parent === self ? layui : top.layui;
            topLayui.index.openTabsPage(decodeURIComponent(url), data.FormName + '【申请记录】');

            if (viewRefresh)
                topLayui.admin.events.refresh();

            viewRefresh = true;

        };

        if (obj.event === 'Draft') {

            var url = '/Views/Forms/MicroFormList/Draft/' + data.ShortTableName + '/' + MID + '/' + data.FormID + '/1/DefaultNumber/GetPersonalDraftList';
            var topLayui = parent === self ? layui : top.layui;
            topLayui.index.openTabsPage(decodeURIComponent(url), data.FormName + '【我的草稿箱】');

            if (draftRefresh)
                topLayui.admin.events.refresh();

            draftRefresh = true;

        };

        if (obj.event === 'Modify') {
            layer.open({
                type: 2
                , title: '编辑【' + data.FormName + '】表单'
                , content: '/Views/Forms/SysForm/Modify/Forms/' + MID + '/' + data.FormID + '/' + data.FormID
                , area: ['95%', '90%']
                , btn: ['立即提交', '关闭']
                , skin: 'micro-layer-class'
                , yes: function (index, layero) {
                    var iframeWindow = window['layui-layer-iframe' + index]
                        , submitID = 'btnModify'
                        , submit = layero.find('iframe').contents().find('#' + submitID);

                    $(submit).click();
                }
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
        };

        if (obj.event === 'FormAppType') {
            layer.open({
                type: 2
                , title: '编辑【' + data.FormName + '】表单'
                , content: '/Views/Forms/MicroFormList/View/FormAppType/' + MID
                , area: ['95%', '90%']
                , btn: ['立即提交', '关闭']
                , skin: 'micro-layer-class'
                , yes: function (index, layero) {
                    var iframeWindow = window['layui-layer-iframe' + index]
                        , submitID = 'btnModify'
                        , submit = layero.find('iframe').contents().find('#' + submitID);

                    $(submit).click();
                }
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
        };
    });


});