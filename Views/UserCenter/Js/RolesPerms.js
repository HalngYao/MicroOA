layui.use(['index', 'jquery', 'layer', 'tree', 'micro'], function () {
    var $ = layui.$
        , layer = layui.layer
        , tree = layui.tree
        , micro = layui.micro;

    var Action = $('#txtAction').val();
    var STN = $('#txtShortTableName').val(); 
    var MID = micro.getUrlPara('mid');

    var mGet = {
        //*****左侧导航Start*****
        GetMgr: function () {
            var microSTN = $(this).attr('micro-stn');
            var microText = $(this).attr('micro-text');
            
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
            var microText = $(this).attr('micro-text');
            var microSTN = $(this).attr('micro-stn');
            
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

        , GetTreeData: function (Type,ID) {
            var Parameter = { "action": "get" + Type + "Perms", "mid": MID, "id": ID };  
            var Data = micro.getAjaxData('text', micro.getRootPath() + '/Views/UserCenter/' + Type + 'Perms.ashx', Parameter, false);
            return Data;
        }

        , GetTree: function (Type, ID) {
            var _edit = $('#txtEdit').val();
            //渲染
            tree.render({
                elem: '#divShowPerms'  //绑定元素
                , showCheckbox: true
                , edit: [_edit == 'True' ? 'update' : '']
                , data: eval(mGet.GetTreeData(Type, ID))
                , id: Type + 'PermsID'  //RolePermsID  JobTitlePermsID
                , operate: function (obj) {
                    var type = obj.type; //得到操作类型：add、edit、del
                    var data = obj.data; //得到当前节点的数据

                    if (type === 'update') { //修改节点
                        if (data.field !== '') {
                            Fields = encodeURI(JSON.stringify(data));
                            var Parameter = { "action": "modifyalias", "mid": MID, "fields": Fields };
                            micro.mAjax('text', micro.getRootPath() + '/Views/UserCenter/' + Type + 'Perms.ashx', Parameter, true, false);
                        } else {
                            layer.msg('模块名称不允许修改，只允许修改权限名称<br/>Module name cannot be modified, only permission name can be modified', {
                                icon: 5
                                , anim: 6
                            });
                        }
                    }
                }
            });
        }

        //左侧菜单点选调用
        , GetPerms: function () {
            var microText = $(this).attr('micro-text');
            var microPageName = $(this).attr('micro-pagename');  //Role  JobTitle
            var microID = $(this).attr('micro-id');  //ID

            $('#spanTitle').html(microText); //改变标题名称
            $('#btnSave').attr('micro-pagename', microPageName);  //给按钮特定属性赋值，在click事件
            $('#btnSave').attr('micro-id', microID);  //给按钮特定属性赋值，在click事件

            mGet.GetTree(microPageName, microID);
        }

        //保存修改
        , btnSave: function () {
            var microPageName = $(this).attr('micro-pagename');  //Role  JobTitle
            var microID = $(this).attr('micro-id');  //ID
            var checkedData = tree.getChecked(microPageName + 'PermsID'); //获取选中节点的数据  Role JobTitle

            if (JSON.stringify(checkedData).length === 2) {
                layer.msg('至少选择一个权限<br/>Select at least one permission', {
                    icon: 5
                    , anim: 6
                });
                return false
            }

            Fields = encodeURI(JSON.stringify(checkedData));
            var Parameter = { "action": "modify", "mid": MID, "id": microID, "fields": Fields };
            micro.mAjax('text', micro.getRootPath() + '/Views/UserCenter/' + microPageName + 'Perms.ashx', Parameter, true, false, false, true);

        }    

        //模块权限赋予
        , btnOpenLink: function () {
            var microText = $(this).attr('micro-text');
            layer.open({
                type: 2
                , title: microText
                , content: '/Views/UserCenter/ModulePerms/' + Action + '/' + STN + '/' + MID
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

    //页面初始化默认值
    mGet.GetTree('Role', 1);

    $('.layui-btn').on('click', function () {
        var type = $(this).data('type');
        mGet[type] ? mGet[type].call(this) : '';
    });

    $('.micro-click').on('click', function () {
        var type = $(this).data('type');
        mGet[type] ? mGet[type].call(this) : '';
    });

});