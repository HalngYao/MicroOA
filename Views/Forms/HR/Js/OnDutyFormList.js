layui.use(['index', 'jquery', 'table', 'form', 'layer', 'laydate', 'upload', 'micro'], function () {
    var $ = layui.$
        , table = layui.table
        , form = layui.form
        , layer = layui.layer
        , laydate = layui.laydate
        , upload = layui.upload
        , micro = layui.micro;

    var Action = $("#txtAction").val();
    var MID = micro.getUrlPara('mid');
    var STN = $("#txtShortTableName").val();
    var FormID = $("#txtFormID").val();
    var FormsID = $("#txtFormsID").val();
    var EditTablePermit = $('#txtEditTablePermit').val();
    var Edit = EditTablePermit === '1' ? 'text' : '';

    var mGet = {

        GetData: function (action, stn, mid, formID, formsID, v) {

            if (action.toLowerCase() == 'add')
                mGet.GetTable();
            else {
                var parameter = { "action": action, "stn": stn, "mid": mid, "formid": formID, "formsid": formsID };
                var url = '/Views/Forms/HR/OnDutyFormList.ashx';
                $.ajax({
                    async: true,
                    type: 'post',
                    dataType: 'json',
                    url: url,
                    data: parameter,
                    cache: false,
                    beforeSend: function () {
                        layer.load(2);
                    },
                    success: function (v) {

                        //console.log(JSON.stringify(v));

                        layer.close(layer.load(2));

                        if (v.msg.substring(0, 4) === 'True') {
                            $('.ws-msg').html('');
                            mGet.GetTable(v);

                        } else {
                            layer.msg('数据读取失败<br/>データの読み込みに失敗しました<br/>Data reading failed', {
                                icon: 5
                                , anim: 6
                            });

                            $('.ws-msg').html(v.msg);
                            mGet.GetTable();
                        }
                    },
                    error: function () {

                        layer.close(layer.load(2));
                        layer.msg('数据读取失败<br/>データの読み込みに失敗しました<br/>Data reading failed', {
                            icon: 5
                            , anim: 6
                        });

                    }
                });

            }

        },

        GetTable: function (v) {
            v = v || eval('(' + $('#txtTabParms').val() + ')');

            table.render({
                elem: '#tabTable'
                , id: 'tabTable'
                //, url: micro.getRootPath() + tbURL + parms //异步数据接口,get方式
                , data: v.data
                //, toolbar: v.data[0].tbToolbar == 'True' ? true : (v.data[0].tbToolbar == 'False' ? false : v.data[0].tbToolbar)  //值为bit和string类型
                //, defaultToolbar: v.data[0].tbDefaultToolbar !== "" ? eval("(" + v.data[0].tbDefaultToolbar + ")") : "" //v.data[0].tbDefaultToolbar
                //, title: v.data[0].tbTitle
                , text: { none: '暂无相关数据' }
                //, initSort: v.data[0].tbInitSort !== "" ? eval("(" + v.data[0].tbInitSort + ")") : ""
                //, skin: v.data[0].tbSkin
                , even: true
                , size: 'sm'
                //, width: parseInt(v.data[0].tbWidth)
                , height: v.code == 100 ? 'full-280' : ''
                , cellMinWidth: 60
                , cols: [eval(v.cols)]
                , page: true
                , limit: 1000
                , limits: [1000]
                //, totalRow: v.data[0].tbTotalRow === 'True' ? true : false
                //, loading: v.data[0].tbLoading === 'True' ? true : false
                //, autoSort: v.data[0].tbAutoSort === 'True' ? true : false
                //, done: function () {
                //    eval(v.data[0].tbDone);
                //    eval(v.data[0].Tips);
                //    eval(v.data[0].CtrlTableContJS);  //生成监听编辑单元格和监听switch change事件代码
                //}
                , done: function (res, curr, count) {
                    $('#txtRecord').val(count);
                    $('#txtFilePath').val(v.filepath);
                    //console.log(count);
                    //console.log(v.filepath);
                    //console.log('CURR:' + curr);
                    //console.log('RES:' + JSON.stringify(res));
                }
            });
        },

        //模板下载
        btnTemplate: function () {

            parent.layer.open({
                type: 2
                , title: '模板下载'
                , content: '/Views/Forms/HR/OnDutyTpl'
                , maxmin: true
                , area: ['95%', '90%']
                , btn: ['下载', '关闭']
                //, skin: 'micro-layer-class2'
                , moveOut: true
                , yes: function (index, layero) {
                    var iframeWindow = window['layui-layer-iframe' + index]
                        , submitID = 'btnDownload'
                        , submit = layero.find('iframe').contents().find('#' + submitID);

                    $(submit).click();

                }
            });
        },

        SetWorkFlow: function () {

            var microSTN = $(this).attr('micro-stn');
            var microMID = $(this).attr('micro-mid');
            var microText = '自定义流程/Custom WorkFlow';

            layer.open({
                type: 2
                , title: microText
                , content: '/Views/Forms/WorkFlow/View/' + microSTN + '/' + microMID + '/' + FormID
                , maxmin: true
                , area: ['100%', '100%']
                , scrollbar: false
                , yes: function (index, layero) {
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

    //页面初始加载Table
    mGet.GetData(Action, STN, MID, FormID, FormsID, '');

    //指定允许上传的文件类型
    upload.render({
        elem: '#btnUpload'
        , url: '/Views/Forms/HR/OnDutyUpload.ashx' //此处配置你自己的上传接口即可
        , accept: 'file' //普通文件
        , acceptMime: 'application/vnd.ms-excel,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'//'image/jpg, image/png'
        , exts: 'xls|xlsx'
        , before: function (obj) { 
            layer.load(); //上传loading
        }
        , done: function (v) {
           
            layer.closeAll('loading');
            if (v.msg.substring(0, 4) === 'True') {
                layer.msg('导入成功，如需保存请点击“立即提交”');
                $('.ws-msg').html('');
                mGet.GetTable(v);

            } else {
                layer.msg('导入失败<br/>インポート失敗<br/>Import failed',{
                    icon: 5
                    , anim: 6
                });

                $('.ws-msg').html(v.msg);
                mGet.GetTable();
            }
        }
        , error: function (index, upload) {
            layer.closeAll('loading');
            layer.msg('导入失败，上传Excel数据失败');
            mGet.GetTable();
        }
    });


    $('.micro-click').on('click', function () {
        var type = $(this).data('type');
        mGet[type] ? mGet[type].call(this) : '';
    });

    form.on('submit(btnSave)', function (data) {

        var action = $("#txtAction").val();
        var stn = $("#txtShortTableName").val();
        var mid = $("#txtMID").val();
        var formID = $("#txtFormID").val();
        var formsID = $('#txtFormsID').val();
        var isapprovalForm = "True";
        var record = parseInt($('#txtRecord').val());  //$('#txtRecord').val() //是否有记录
        var filePath = $('#txtFilePath').val();  //通过Excel时返回Excel路径

        //请先添加或导入内容
        //formsID == "" || formsID == "0" || $.trim(formsID).length == 0 || 
        if (isNaN(record) || record <= 0 || (formsID == "" && filePath == "")) {
            $('#btnUpload').addClass('btnTips');

            layer.msg('请先导入内容<br/>まず内容を導入してください<br/>Please import the content first', {
                icon: 5
                , anim: 6
                , time: 1000
            }, function () { $('#btnUpload').removeClass('btnTips'); });

            return false;

        } else {

            var Fields = JSON.stringify(data.field); Fields = Fields.replace(/ctl00\$ContentPlaceHolder1\$/g, "");
            Fields = encodeURI(Fields);

            console.log(Fields);

            var parameter = { "action": action, "stn": stn, "mid": mid, "formid": formID, "formsid": formsID, "isapprovalform": isapprovalForm, "filepath": filePath, "fields": Fields };
            micro.mAjax('text', '/Views/Forms/HR/CtrlOnDutyFormList.ashx', parameter, true, false, true);
        }
    });


    form.on('submit(btnModify)', function (data) {

        var action = $("#txtAction").val();
        var stn = $("#txtShortTableName").val();
        var mid = $("#txtMID").val();
        var formID = $("#txtFormID").val();
        var formsID = $('#txtFormsID').val();
        var isapprovalForm = "True";
        var record = parseInt($('#txtRecord').val());  //$('#txtRecord').val() //是否有记录
        var filePath = $('#txtFilePath').val();  //通过Excel时返回Excel路径

        //请先添加或导入内容
        //formsID == "" || formsID == "0" || $.trim(formsID).length == 0 || 
        if (isNaN(record) || record <= 0 || formsID == "" || filePath == "") {
            $('#btnUpload').addClass('btnTips');

            layer.msg('请先导入内容<br/>まず内容を導入してください<br/>Please import the content first', {
                icon: 5
                , anim: 6
                , time: 1000
            }, function () { $('#btnUpload').removeClass('btnTips'); });

            return false;

        } else {

            var Fields = JSON.stringify(data.field); Fields = Fields.replace(/ctl00\$ContentPlaceHolder1\$/g, "");
            Fields = encodeURI(Fields);

            console.log(Fields);

            var parameter = { "action": action, "stn": stn, "mid": mid, "formid": formID, "formsid": formsID, "isapprovalform": isapprovalForm, "filepath": filePath, "fields": Fields };
            micro.mAjax('text', '/Views/Forms/HR/CtrlOnDutyFormList.ashx', parameter, true, false, true);
        }
    });
});