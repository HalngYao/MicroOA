layui.use(['index', 'jquery', 'form', 'table', 'layer', 'micro', 'dropdown'], function () {
    var $ = layui.$
        , form = layui.form
        , table = layui.table
        , layer = layui.layer
        , micro = layui.micro
        , dropdown = layui.dropdown;

    var MID = micro.getUrlPara('mid');

    //dropdown
    dropdown.render({
        elem: '#btnMore'
        , data: [{
            title: '同步域控用户'
            , id: 100
        }, {
            title: '同步域控用户所属部门'
            , id: 101
        }, {
            title: '设置所有用户默认职务'
            , id: 102
        }, {
            title: '设置所有用户默认角色'
            , id: 103
        }, {
            title: '批量修改用户所属部门'
            , id: 104
        }, {
            title: '批量修改用户职位职称'
            , id: 105
        }, {
            title: '批量修改用户所属角色'
            , id: 106
        }, {
            title: '批量修改用户工时制'
            , id: 107
        }]
        , click: function (obj) {
            var id = obj.id;

            if (id === 100)
                mGet.SyncDomainUsers();
            else if (id === 101)
                mGet.SyncDomainUsersDept();
            else if (id === 102)
                mGet.SetUsersProp('设置所有用户默认职务', 'SetAUJTitle');
            else if (id === 103)
                mGet.SetUsersProp('设置所有用户默认角色', 'SetAURole');
            else if (id === 104)
                mGet.ModifyUsersProp('批量修改用户所属部门', 'Udepts');
            else if (id === 105)
                mGet.ModifyUsersProp('批量修改用户职位职称', 'UJTitle');
            else if (id === 106)
                mGet.ModifyUsersProp('批量修改用户所属角色', 'URoles');
            else if (id === 107)
                mGet.ModifyUsersProp('批量修改用户所属角色', 'UWorkHourSystem');

        }
    });

    var mGet = {
        //*****左侧导航Start*****
        //管理
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

        //左侧菜单点选调用
        , GetUsers: function () {
            var microType = $(this).attr('micro-type');
            var microID = $(this).attr('micro-id');
            mGet.getTableAttributes('use', microType, microID);
        }
        //*****左侧导航End*****

        , btnAddOpenLink: function () {
            layer.open({
                type: 2
                , title: '添加用户'
                , content: '/Views/UserCenter/UsersForm/Add/Use/' + MID
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

        //同步域控用户
        , SyncDomainUsers: function () {
            layer.open({
                type: 2
                , title: '同步域控用户'
                , content: '/Views/UserCenter/SyncDomainUsers/' + MID
                , maxmin: true
                , area: ['95%', '90%']
                //, btn: ['保存', '取消']
                , yes: function (index, layero) {
                    var iframeWindow = window['layui-layer-iframe' + index]
                        , submitID = 'btnSave'
                        , submit = layero.find('iframe').contents().find('#' + submitID);

                    $(submit).click();
                }
            });
        }

        //同步用户所属部门
        , SyncDomainUsersDept: function () {
            layer.confirm('更新用户所属部门：是根据域控用户的部门属性栏的值更新到本系统中，<br/>对于本系统的某些用户可能已经被设置了多个所属部门，则这些设置会被删除，<br/>您确定要更新吗？',
                { title: '警告信息' },
                function (index) {
                    var Parameter = { "action": "userdepts", "mid": MID };
                    micro.mAjax('text', micro.getRootPath() + '/Views/UserCenter/SyncDomainUsers.ashx', Parameter);
                });
        }

        //设置所有用户默认职务、角色
        , SetUsersProp: function (Title, Type) {
            layer.open({
                type: 2
                , title: Title
                , content: '/Views/UserCenter/UsersPublicInfoChange/Modify/' + Type + '/' + MID
                , area: ['95%', '90%']
                , btn2: function (index, layero) {
                    //var State = layero.find('iframe').contents().find('#txtState').val();
                    //if (State == 'True')
                    //    location.reload();
                }
                , cancel: function (index, layero) {
                    //var State = layero.find('iframe').contents().find('#txtState').val();
                    //if (State == 'True')
                    //    location.reload();
                }
            });
        }

        //批量修改用户属性,如：所属部门、职位职称、所属角色
        , ModifyUsersProp: function (Title, Type) {
            var checkStatus = table.checkStatus('tabTable');
            var data = checkStatus.data;  //得到选中用户

            if (data.length == 0) {
                layer.msg('没有选中任何用户 <br/>No user selected');
                return false;
            }
            else if (data.length > 50) {
                layer.msg('批量修改每次操作不能大于50位用户 <br/>Batch modification cannot exceed 50 users per operation');
                return false;
            }
            else {
                var UID = encodeURI(micro.getStrSplice(data, 'UID'));
                layer.open({
                    type: 2
                    , title: Title
                    , content: '/Views/UserCenter/UsersPublicInfoChange/Modify/' + Type + '/' + MID + '/' + UID
                    , area: ['95%', '90%']
                    , btn2: function (index, layero) {
                        //var State = layero.find('iframe').contents().find('#txtState').val();
                        //if (State == 'True')
                        //    location.reload();
                    }
                    , cancel: function (index, layero) {
                        //var State = layero.find('iframe').contents().find('#txtState').val();
                        //if (State == 'True')
                        //    location.reload();
                    }
                });
            }
        }

        //获取表格的属性，然后创建layui数据表
        , getTableAttributes: function (STN, Type, TypeID = 1, Keyword = '') {
            var UrlPara = '';
            if (typeof (STN) !== 'undefined' && STN !== '' && STN !== null)
                UrlPara = '?stn=' + STN + '&mid=' + MID + '&type=' + Type + '&typeid=' + TypeID + '&Keyword=' + Keyword;

            URL = micro.getRootPath() + '/App_Handler/GetTableAttributes.ashx' + UrlPara;

            $.ajax({
                async: true,
                type: 'post',
                dataType: 'json',
                url: URL,
                data: '',
                beforeSend: function () {
                    //return flag;
                },
                success: function (v) {
                    //console.log(v.data[0].Cols);
                    table.render({
                        elem: v.data[0].tbElem
                        , url: micro.getRootPath() + v.data[0].tbURL + UrlPara //异步数据接口,get方式
                        , data: ''
                        , toolbar: v.data[0].tbToolbar == 'True' ? true : (v.data[0].tbToolbar == 'False' ? false : v.data[0].tbToolbar)  //值为bit和string类型
                        , defaultToolbar: v.data[0].tbDefaultToolbar !== '' ? eval('(' + v.data[0].tbDefaultToolbar + ')') : '' //v.data[0].tbDefaultToolbar
                        , title: v.data[0].tbTitle
                        , text: { none: v.data[0].tbText }
                        , initSort: v.data[0].tbInitSort !== '' ? eval('(' + v.data[0].tbInitSort + ')') : ''
                        , skin: v.data[0].tbSkin
                        , even: v.data[0].tbEven === 'True' ? true : false
                        , size: v.data[0].tbSize
                        , width: parseInt(v.data[0].tbWidth)
                        , height: v.data[0].tbHeight !== '0' ? 'full-' + v.data[0].tbHeight : ''
                        , cellMinWidth: parseInt(v.data[0].tbCellMinWidth)
                        , cols: [eval(v.data[0].Cols)]
                        , page: v.data[0].tbPage === 'True' ? true : false
                        , limit: parseInt(v.data[0].tbLimit)
                        , limits: v.data[0].tbLimits !== '' ? eval('(' + v.data[0].tbLimits + ')') : ''
                        , totalRow: v.data[0].tbTotalRow === 'True' ? true : false
                        , loading: v.data[0].tbLoading === 'True' ? true : false
                        , autoSort: v.data[0].tbAutoSort === 'True' ? true : false
                        , done: function () {
                            eval(v.data[0].tbDone);
                            eval(v.data[0].Tips);
                            eval(v.data[0].CtrlTableContJS);  //生成监听编辑单元格和监听switch change事件代码
                        }
                    });
                },
                error: function () {
                    flag = 'error,出错了';
                    //return flag;
                }
            });
        }
    };

    //页面初始化默认显示所有用户
    mGet.getTableAttributes('use', 'all', 1);

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
        var Keyword = data.field.txtKeyword;
        if (Keyword !== '') {
            mGet.getTableAttributes('use', 'Search', 1, encodeURI(Keyword));
            return false;
        }
    });

    //监听工具条
    table.on('tool(tabTable)', function (obj) {
        var data = obj.data;

        if (obj.event === 'ChangeDept') {
            layer.open({
                type: 2
                , title: '修改【' + data.ChineseName + '】所属部门'
                , content: '/Views/UserCenter/UserPublicInfoChange/Modify/UDepts/' + MID + '/' + data.UID
                , area: ['95%', '90%']
            });
        }
        if (obj.event === 'ChangeJobTitle') {
            layer.open({
                type: 2
                , title: '修改【' + data.ChineseName + '】所属职位职称'
                , content: '/Views/UserCenter/UserPublicInfoChange/Modify/UJTitle/' + MID + '/' + data.UID
                , area: ['95%', '90%']
            });
        }
        if (obj.event === 'ChangeRoles') {
            layer.open({
                type: 2
                , title: '修改【' + data.ChineseName + '】所属角色'
                , content: '/Views/UserCenter/UserPublicInfoChange/Modify/URoles/' + MID + '/' + data.UID
                , area: ['95%', '90%']
            });
        }
        if (obj.event === 'ChangeWorkHourSystem') {
            layer.open({
                type: 2
                , title: '修改【' + data.ChineseName + '】所属工时制'
                , content: '/Views/UserCenter/UserPublicInfoChange/Modify/UWorkHourSystem/' + MID + '/' + data.UID
                , area: ['95%', '90%']
            });
        }
    });

});