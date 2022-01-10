layui.use(['index', 'jquery', 'form', 'micro'], function () {
    var $ = layui.$
        , form = layui.form
        , micro = layui.micro;

    var Action = $("#txtAction").val();
    var STN = $("#txtShortTableName").val(); 
    var MID = $("#txtMID").val();
    var FormID = $("#txtFormID").val();
    var PKV = $("#txtPrimaryKeyValue").val();
    
    
    form.on('submit(btnSave)', function (data) {
        var Fields = JSON.stringify(data.field);  //获取提交的字段
        Fields = Fields.replace(/ctl00\$ContentPlaceHolder1\$/g, "");
        Fields = encodeURI(Fields);

        //alert(Fields);
        //return false;

        var Parameter = { "action": Action, "stn": STN, "mid": MID, "formid": FormID, "pkv": PKV, "fields": Fields };
        micro.mAjax('text', micro.getRootPath() + '/Views/Forms/MicroFormWithdrawal.ashx', Parameter, true, true, true);
    });


});