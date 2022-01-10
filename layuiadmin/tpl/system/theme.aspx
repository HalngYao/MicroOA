<%@ Page Language="C#" AutoEventWireup="true" CodeFile="theme.aspx.cs" Inherits="layuiadmin_tpl_system_theme" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <script type="text/html" template lay-done="layui.data.theme();">
              {{# 
                var local = layui.data(layui.setter.tableName)
                ,theme = local.theme || {}
                ,themeColorIndex =  parseInt((theme && theme.color) ? theme.color.index : 0) || 0;
              }}

              <div class="layui-card-header">
                配色方案 / <cite class="ws-font-size8">色を合わせる方案</cite>
              </div>
              <div class="layui-card-body layadmin-setTheme">
                <ul class="layadmin-setTheme-color">
                  {{# layui.each(layui.setter.theme.color, function(index, item){ }}
                    <li layadmin-event="setTheme" data-index="{{ index }}" data-alias="{{ item.alias }}" 
                    {{ index === themeColorIndex ? 'class="layui-this"' : '' }} title="{{ item.alias }}">
                      <div class="layadmin-setTheme-header" style="background-color: {{ item.header }};"></div>
                      <div class="layadmin-setTheme-side" style="background-color: {{ item.main }};">
                        <div class="layadmin-setTheme-logo" style="background-color: {{ item.logo }};"></div>
                      </div>
                    </li>
                  {{# }); }}
                </ul>
              </div>
            </script>

            <script>
                layui.data.theme = function () {
                    layui.use('form', function () {
                        var form = layui.form
                            , admin = layui.admin;

                        //监听隐藏开关
                        form.on('switch(system-theme-sideicon)', function () {
                            admin.theme({
                                hideSideIcon: this.checked
                            })
                        });
                    });
                };
            </script>
        </div>
    </form>
</body>
</html>
