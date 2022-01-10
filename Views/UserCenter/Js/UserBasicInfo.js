layui.use(['index', 'jquery', 'table', 'layer', 'upload', 'micro'], function () {
    var $ = layui.$
        , form = layui.form
        , table = layui.table
        , layer = layui.layer
        , upload = layui.upload
        , micro = layui.micro;

    var MID = micro.getUrlPara('mid');
    var UID = $("#txtUID").val();
    var UserDepts = $("#UserDepts").val();

    var Parameter = { "action": "get", "type": "Depts", "mid": MID, "DefaultValue": UserDepts };
    var microXmSelect = xmSelect.render({
        el: '#microXmSelect',
        name: 'txtUserDepts',
        language: 'zn',
        autoRow: true,
        filterable: true,
        disabled:true,
        tree: {
            show: true,  //是否显示树状结构
            showFolderIcon: true,  //是否展示三角图标
            showLine: true,  //是否显示虚线
            indent: 20,  //间距
            expandedKeys: true, //默认展开的节点数组[-3], 为true时展开所有节点
            strict: false, //是否遵循严格父子结构
        },
        toolbar: {
            show: true,
            list: ['ALL', 'REVERSE', 'CLEAR']
        },
        filterable: true,
        height: '300px',
        data: eval(micro.getAjaxData('text', micro.getRootPath() + '/App_Handler/GetUserPublicInfoForXmSelect.ashx', Parameter))

    });

    //头像上传
    var uploadInst = upload.render({
        elem: '#btnUpload'
        , url: micro.getRootPath() + '/Views/UserCenter/UploadAvatar.ashx'
        , size: parseInt($("#txtMaxFileUpload").val())  //大小限制
        , exts: $("#txtUploadFileType").val()  //类型限制
        , before: function (obj) {
            //预读本地文件示例，不支持ie8
            obj.preview(function (index, file, result) {
                $('#imgAvatar').attr('src', result); //图片链接（base64）
            });
        }
        , done: function (res) {
            return layer.msg(res.msg);
        }
        , error: function () {
            //演示失败状态，并实现重传
            var demoText = $('#demoText');
            demoText.html('<span style="color: #FF5722;">上传失败。 Upload failure.</span> <a class="layui-btn layui-btn-mini demo-reload">重试</a>');
            demoText.find('.demo-reload').on('click', function () {
                uploadInst.upload();
            });
        }
    });

    //RadioButton Change事件，把选中的值存放在TextBox里
    form.on('radio(radioSex)', function (data) { $('#txtSex').val(data.value); });

    //获取当前值选中RadioButton
    if ($("#txtSex").val() == "男") {
        $("#radioSex0").prop("checked", "checked");
        form.render();
    }
    if ($("#txtSex").val() == "女") {
        $("#radioSex1").prop("checked", "checked");
        form.render();
    }

    //保存修改
    form.on('submit(btnModify)', function (data) {
        var Fields = JSON.stringify(data.field);
        Fields = Fields.replace(/ctl00\$ContentPlaceHolder1\$/g, "");
        Fields = encodeURI(Fields);
        var Parameter = { "action": "modify", "mid": MID, "fields": Fields };
        micro.mAjax('text', micro.getRootPath() + '/Views/UserCenter/UserBasicInfo.ashx', Parameter);
    });

});