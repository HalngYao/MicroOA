layui.use(['index', 'jquery', 'form', 'micro'], function () {
    var $ = layui.$
        , form = layui.form
        , micro = layui.micro;

    var MID = micro.getUrlPara('mid');

    //selTableName bind
    if ($("#selTableName").length > 0) {
        var Fields = { "txt": "TabColName,Title", "val": "TID", "stn": "mTabs", "ob": "TID" };
        micro.getTxtVal("getalltn", "selTableName", "", Fields);
    }

    //selTableName Change
    form.on('select(selTableName)', function (data) {
        var Val = ($(data.elem).find("option:selected").text());
        $("#txtTableName").val(Val.split('-')[0]);
    });

    var mGet = {
        //SourceTabl赋值
        getTxtValSrcTab: function () {
            var Fields = { "txt": "TabColName", "val": "TabColName", "stn": "mTabs", "ob": "ParentID,Sort" };
            micro.getTxtVal('getalltn', 'ctlSourceTable', micro.getRootPath() + '/App_Handler/GetPublicTextValue.ashx', Fields);
        },
        //显示字段
        getTextName: function (TabColName, Selected) {
            var Fields = { "txt": "TabColName", "val": "TabColName", "stn": "mTabs", "tcn": "" + TabColName + "", "ob": "Sort" };
            micro.getTxtVal('get', 'ctlTextName', micro.getRootPath() + '/App_Handler/GetPublicTextValue.ashx', Fields, Selected);
        },
        //值字段
        getTextValue: function (TabColName, Selected) {
            var Fields = { "txt": "TabColName", "val": "TabColName", "stn": "mTabs", "tcn": "" + TabColName + "", "ob": "Sort" };
            micro.getTxtVal('get', 'ctlTextValue', micro.getRootPath() + '/App_Handler/GetPublicTextValue.ashx', Fields, Selected);
        },
        //select值初始化
        setIniSourceTable: function () {
            $('#ctlSourceTable option:first').prop('selected', 'selected');

            $("#ctlTextName").empty();
            $('#ctlTextName').removeAttr('lay-verify');
            $("#ctlTextName").prepend("<option value=''>请选择</option>");

            $("#ctlTextValue").empty();
            $('#ctlTextValue').removeAttr('lay-verify');
            $("#ctlTextValue").prepend("<option value=''>请选择</option>"); 
            form.render();
        },
        //表单控件值初始化
        setIniFormCtl: function () {
            $('#ctlTypes option:first').prop('selected', 'selected');
            $('#ctlTypes').removeAttr('lay-verify');
            $('#txtctlTitle').removeAttr('lay-verify');
            $('#txtctlTitle').val("");
            $('#ctlValue').val("");
            $('#ctlDefaultValue').val("");
            $('#ctlVerify2').val("");
            $('#ctlSourceTable option:first').prop('selected', 'selected');
            $("#ctlTextName").empty();
            $('#ctlTextName').removeAttr('lay-verify');
            $("#ctlTextName").prepend("<option value=''>请选择</option>");
            $("#ctlTextValue").empty();
            $('#ctlTextValue').removeAttr('lay-verify');
            $("#ctlTextValue").prepend("<option value=''>请选择</option>"); 
            form.render();
        }
    };

    //调用mGet函数 ctlSourceTable bind
    mGet.getTxtValSrcTab();

    //Sql数据类型
    form.on('select(selDataType)', function (data) {
        var val = data.value;
        switch (val) {
            case 'bit':
                $('#txtFieldLength').attr('disabled', true).addClass('layui-disabled').removeAttr('lay-verify').val("");
                $('#divFieldLength').hide(500);
                break;
            case 'char':
                $('#txtFieldLength').removeAttr('disabled', true).removeClass('layui-disabled').attr('lay-verify', 'required|number').val("10");
                $('#divFieldLength').show(500);
                break;
            case 'datetime':
                $('#txtFieldLength').attr('disabled', true).addClass('layui-disabled').removeAttr('lay-verify').val("");
                $('#divFieldLength').hide(500);
                break;
            case 'decimal':
                $('#txtFieldLength').removeAttr('disabled', true).removeClass('layui-disabled').attr('lay-verify', 'required').val("18,0");
                $('#divFieldLength').show(500);
                break;
            case 'float':
                $('#txtFieldLength').attr('disabled', true).addClass('layui-disabled').removeAttr('lay-verify').val("");
                $('#divFieldLength').hide(500);
                break;
            case 'int':
                $('#txtFieldLength').attr('disabled', true).addClass('layui-disabled').removeAttr('lay-verify').val("");
                $('#divFieldLength').hide(500);
                break;
            case 'nchar':
                $('#txtFieldLength').removeAttr('disabled', true).removeClass('layui-disabled').attr('lay-verify', 'required|number').val("10");
                $('#divFieldLength').show(500);
                break;
            case 'ntext':
                $('#txtFieldLength').attr('disabled', true).addClass('layui-disabled').removeAttr('lay-verify').val("");
                $('#divFieldLength').hide(500);
                break;
            case 'nvarchar':
                $('#txtFieldLength').removeAttr('disabled', true).removeClass('layui-disabled').attr('lay-verify', 'required|number').val("50");
                $('#divFieldLength').show(500);
                break;
            case 'text':
                $('#txtFieldLength').attr('disabled', true).addClass('layui-disabled').removeAttr('lay-verify').val("");
                $('#divFieldLength').hide(500);
                break;
            case 'varchar':
                $('#txtFieldLength').removeAttr('disabled', true).removeClass('layui-disabled').attr('lay-verify', 'required|number').val("50");
                $('#divFieldLength').show(500);
                break;
        }
    });

    //是否创建HMTL控件
    form.on('checkbox(ckCreateCtrl)', function (data) {
        var flag = data.elem.checked;

        if (flag) {
            $("#divCreateCtrl").show(500);
            $('#ctlTypes').attr('lay-verify', 'required');
            $('#txtctlTitle').attr('lay-verify', 'required');
            $('#txtCreateCtrl').val("True");
        } else {
            $("#divCreateCtrl").hide(500);
            $('#txtCreateCtrl').val("False");
            mGet.setIniFormCtl();

        }
    });      

    //根据控件类型改变一些需要输入的值
    form.on('select(ctlTypes)', function (data) {
        var val = data.value;
        switch (val) {
            case 'Text':
                $('#divDataSource').hide(500);
                mGet.setIniSourceTable();
                break;
            case 'Password':
                $('#divDataSource').hide(500);
                mGet.setIniSourceTable();
                break;
            case 'Hidden':
                $('#divDataSource').hide(500);
                mGet.setIniSourceTable();
                break;
            case 'Select':
                $('#divDataSource').show(500);
                break;
            case 'CheckBox':
                $('#divDataSource').show(500);
                break;
            case 'Radio':
                $('#divDataSource').show(500);
                break;
            case 'Textarea':
                $('#divDataSource').hide(500);
                mGet.setIniSourceTable();
                break;
            case 'ImgUpload':
                $('#divDataSource').hide(500);
                mGet.setIniSourceTable();
                break;
            case 'FileUpload':
                $('#divDataSource').hide(500);
                mGet.setIniSourceTable();
                break;
            case 'Other':
                $('#divDataSource').hide(500);
                mGet.setIniSourceTable();
                break;
        }
    });

    //SourceTable
    form.on('select(ctlSourceTable)', function (data) {
        var v = data.value;
        if (v !== "") {
            $('#ctlTextName').attr('lay-verify', 'required');
            $('#ctlTextValue').attr('lay-verify', 'required');
            mGet.getTextName(v);  //ctlTextName bind
            mGet.getTextValue(v);  //ctlTextValue bind
        } else {
            $("#ctlTextName").empty();
            $("#ctlTextName").prepend("<option value=''>请选择</option>");
            $("#ctlTextValue").empty();
            $("#ctlTextValue").prepend("<option value=''>请选择</option>"); 
            $('#ctlTextName').removeAttr('lay-verify');
            $('#ctlTextValue').removeAttr('lay-verify');
            form.render();
        }
    });

    //提交表单
    form.on('submit(btnAddCol)', function (data) {
        var Fields = JSON.stringify(data.field), CtlTypes = data.field.ctlTypes;
        //控件类型为以下类型时需填写默认值或数据来源
        if (CtlTypes === "Select" || CtlTypes === "CheckBox" || CtlTypes === "Radio") {
            var CtlValue = data.field.ctlValue, CtlSourceTable = data.field.ctlSourceTable;
            if (CtlValue === "" && CtlSourceTable === "") {
                var flag = "当控件类型为：" + CtlTypes + " 时“控件值”和“数据来源”必须填写一个。";
                layer.msg(flag, {
                    icon: 5
                    , anim: 6
                }, function () {
                    //return false;
                });
                return false;
            }
        };
        Fields = Fields.replace(/ctl00\$ContentPlaceHolder1\$/g, "");
        Fields = encodeURI(Fields);
        var Parameter = { "action": "addcol", "mid": MID, "fields": Fields };
        micro.mAjax('text', micro.getRootPath() + '/Views/Set/System/AddMicroDataTableColumn.ashx', Parameter);
    });


});