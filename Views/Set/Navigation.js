layui.use(['index', 'jquery', 'form', 'table', 'micro'], function () {
    var $ = layui.$
        , form = layui.form
        , table = layui.table
        , micro = layui.micro;

    var MID = $("#txtMID").val();
    var ShortTableName = $('#txtShortTableName').val();
    var PrimaryKeyName = $('#txtPrimaryKeyName').val();

    //生成数据表
    micro.getTableAttributes('View', 'mod');

    //事件
    var mGet = {
        btnDel: function () {
            var checkStatus = table.checkStatus('tabTable')
                , checkData = checkStatus.data; //得到选中的数据

            if (checkData.length === 0) {
                return layer.msg('请选择数据');
            }

            layer.prompt({
                formType: 1
                , title: '敏感操作，请验证口令'
            }, function (value, index) {
                layer.close(index);

                layer.confirm('确定删除吗？', function (index) {

                    //执行 Ajax 后重载
                    /*
                    admin.req({
                      url: 'xxx'
                      //,……
                    });
                    */
                    table.reload('tabTable');
                    layer.msg('已删除');
                });
            });
        }

        , btnAddOpenLink: function () {
            layer.open({
                type: 2
                , title: '添加菜单'
                , content: '/Views/Set/NavigationForm/Add/Mod/' + MID
                , maxmin: true
                , area: ['95%', '90%']
                , btn: ['立即提交', '关闭']
                , yes: function (index, layero) {
                    var iframeWindow = window['layui-layer-iframe' + index]
                        , submitID = 'btnSave'
                        , submit = layero.find('iframe').contents().find('#' + submitID);

                    $(submit).click();
                }
                , btn2: function (index, layero) {
                    var State = layero.find('iframe').contents().find('#txtState').val();
                    if (State == "True")
                        location.reload();
                }
                , cancel: function (index, layero) {
                    var State = layero.find('iframe').contents().find('#txtState').val();
                    if (State == "True")
                        location.reload();
                }
            });
        }

        , getTxtVal: function () {
            var Fields = { "txt": "ModuleName", "val": "MID", "lv": "True", "stn": "mod", "ob": "Sort" };
            micro.getTxtVal("get", "selParentID", "/App_Handler/GetParSubTextValue.ashx", Fields);
        }
    };

    //控件存在时，对控件进行赋值
    if ($("#selParentID").length > 0)
        mGet.getTxtVal();


    form.on('select(selParentID)', function (data) {
        var val = data.value, Level = $(data.elem).find("option:selected").attr("level");
        if (val !== "")
            $("#txtLevel").val(Level);
        else
            $("#txtLevel").val(0);

    });

    //提交表单
    form.on('submit(btnAdd)', function (data) {
        var Fields = JSON.stringify(data.field);  //获取提交的字段
        Fields = Fields.replace(/ctl00\$ContentPlaceHolder1\$/g, "");
        Fields = encodeURI(Fields);
        var Parameter = { "action": "add", "mid": MID, "fields": Fields };
        micro.mAjax('text', '/Views/Set/Navigation.ashx', Parameter);
        mGet.getTxtVal();  //调用mGet函数selParentID bind
    });

    micro.Tips("值：数字【0=父层，1=二层，2=三层...】。表示属于第几层，主要用于显示前缀", "#tipsLevel", 2);

    $('.layui-btn').on('click', function () {
        var type = $(this).data('type');
        mGet[type] ? mGet[type].call(this) : '';
    });

    //监听工具条
    table.on('tool(tabTable)', function (obj) {
        var data = obj.data;

        if (obj.event === 'Modify') {
            layer.open({
                type: 2
                , title: '编辑内容'
                , content: '/Views/Forms/SysForm/Modify/' + ShortTableName + '/' + MID + '/' + eval(PrimaryKeyName)
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
        }

    });

});