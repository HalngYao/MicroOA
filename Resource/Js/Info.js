//*****获取根路径Start*****
function getRootPath() {
    var curPageUrl = window.document.location.href;
    var rootPath = "";
    rootPath = curPageUrl.split("//")[1].split("/")[0];
    rootPath = rootPath.slice(0, 9);
    if (rootPath === "localhost") {
        rootPath = curPageUrl.split("//")[0] + "//" + curPageUrl.split("//")[1].split("/")[0];
    } else {
        rootPath = curPageUrl.split("//")[0] + "//" + curPageUrl.split("//")[1].split("/")[0];
    }
    return rootPath;
}

////判断url是否在iframe打开
//if (window.frames.length == parent.frames.length) {
//    //alert('不在iframe中' + window.document.location.href);
//    window.location.replace(getRootPath() + '/Views/Default?url=' + encodeURIComponent(window.document.location.href));
//    //window.location.replace(getRootPath() + '/Views/Default?url=' + window.document.location.href);
//}

layui.config({
    base: getRootPath() + '/layuiadmin/' //静态资源所在路径
    , version: true
}).extend({
    index: 'lib/index' //主入口模块
    //, micro: '{/}/layuiadmin/lib/extend/micro'  //自定义模块
    //, dropdown: '{/}/layuiadmin/lib/extend/dropdown'  //自定义模块;
});