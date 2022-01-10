//*****获取根路径Start*****
function getRootPath() {
    var curPageUrl = window.document.location.href;
    var rootPath = "";
    rootPath = curPageUrl.split("//")[1].split("/")[0];
    rootPath = rootPath.slice(0, 9);
    if (rootPath === "localhost") {
        //rootPath = curPageUrl.split("//")[0] + "//" + curPageUrl.split("//")[1].split("/")[0] + "/" + curPageUrl.split("//")[1].split("/")[1];
        rootPath = curPageUrl.split("//")[0] + "//" + curPageUrl.split("//")[1].split("/")[0];
    } else {
        rootPath = curPageUrl.split("//")[0] + "//" + curPageUrl.split("//")[1].split("/")[0];
    }
    return rootPath;
}

layui.config({
    base: getRootPath() + '/layuiadmin/' //静态资源所在路径
    //, version: true
}).extend({
    index: 'lib/index' //主入口模块
    //, micro: '{/}/layuiadmin/lib/extend/micro'  //自定义模块
});
//}).use('index');
