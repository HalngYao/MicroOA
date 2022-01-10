layui.use(['index', 'jquery', 'table', 'layer', 'micro'], function () {
    var $ = layui.$
        , form = layui.form
        , layer = layui.layer
        , micro = layui.micro;

    var MID = micro.getUrlPara('mid');

    var btn = '<button type="button" id="btnTestSend" class="layui-btn"  lay-submit lay-filter="btnTestSend">测试发送</button>';
    $('.micro-btn-div').append(btn);


    form.on('submit(btnTestSend)', function (data) {
        var Fields = JSON.stringify(data.field);  //获取提交的字段
        Fields = Fields.replace(/ctl00\$ContentPlaceHolder1\$/g, "");
        Fields = encodeURI(Fields);
        var Parameter = { "action": "view", "mid": MID, "fields": Fields };
        micro.mAjax('text', micro.getRootPath() + '/Views/Set/System/WebSiteTestMail.ashx', Parameter);
    });

});