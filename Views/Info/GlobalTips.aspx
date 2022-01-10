<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GlobalTips.aspx.cs" Inherits="Views_Info_GlobalTips" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script src='<%: ResolveUrl("~/layuiadmin/layui/layui.js")%>'></script>
    <link href="<%: ResolveUrl("~/layuiadmin/layui/css/layui.css")%>" rel="stylesheet" media="all" />
    <link href="<%: ResolveUrl("~/layuiadmin/style/admin.css")%>" rel="stylesheet" media="all" />
    <link href="<%: ResolveUrl("~/Resource/css/micro.css")%>" rel="stylesheet" media="all" />
    <link href="<%: ResolveUrl("~/Resource/css/info.css")%>" rel="stylesheet" media="all" />
    <link href="<%: ResolveUrl("~/Resource/ico/iconfont.css")%>" rel="stylesheet" media="all" />
    <style>
        html { background-color:#ffffff;}
    </style>
</head>
<body style="margin-top:0px;">
    <form id="form1" runat="server">
<div class="layui-container">
  <div class="layui-form layui-row layui-col-space15">
    <div class="layui-col-xs12 layui-col-sm12 layui-col-md12 layui-col-lg12">
        <ul id="ulList" runat="server" class="fly-panel-main fly-list-static">

        </ul>
    </div>
    <div class="layui-form-item" style="position:fixed; width:100%; bottom:0; margin-bottom:0; background-color:#F0F0F0; text-align:right;">
        <div class="layui-input-block">
            <input type="checkbox" name="cbNoTips" lay-skin="primary" lay-filter="cbNoTips" title="不再提示 / Don't prompt again.">
        </div>
    </div>
  </div>
</div>

<script>

    layui.config({
        base: '<%: ResolveUrl("~/layuiadmin/")%>' //静态资源所在路径
    }).extend({
        index: 'lib/index' //主入口模块
        , micro: '{/}/layuiadmin/lib/extend/micro'  //自定义模块
    }).use(['index', 'form', 'layer', 'micro'], function () {
        form = layui.form,
            layer = layui.layer,
            micro = layui.micro;

        form.on('checkbox(cbNoTips)', function (data) {
            console.log(data.elem.checked); //开关是否开启，true或者false

            var Parameter = { "action": "Modify", "type": "GTips", "stn": "Use", "mid": 27, "pkv": "pkv", "val": data.elem.checked };
            var aa = micro.getAjaxData('text', micro.getRootPath() + '/App_Handler/CtrlPublicGenService.ashx', Parameter);
            if (aa !== "")
                layer.msg(aa);

        });

    });

</script>
    </form>
</body>
</html>
