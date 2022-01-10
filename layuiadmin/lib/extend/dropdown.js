/* layui_dropdown v2.0.0 by Microanswer,doc:http://test.microanswer.cn/dropdown.html */
layui.define(["jquery","laytpl"],function(i){"use strict";function e(i,t){"string"==typeof i&&(i=s(i)),this.$dom=i,this.option=s.extend({downid:String(Math.random()).split(".")[1],filter:i.attr("lay-filter")},o,t),20<this.option.gap&&(this.option.gap=20),this.init()}var s=layui.jquery||layui.$,n=layui.laytpl,d=s(window.document.body),a="a",h={},r=window.MICROANSWER_DROPDOWAN||"dropdown",l="<div tabindex='0' class='layui-anim layui-anim-upbit dropdown-root' "+r+"-id='{{d.downid}}' style='z-index: {{d.zIndex}}'><div class='dropdown-pointer'></div><div class='dropdown-content' style='margin: {{d.gap}}px {{d.gap}}px;background-color: {{d.backgroundColor}};min-width: {{d.minWidth}}px;min-height: {{d.minHeight}}px;max-height: {{d.maxHeight}}px;overflow: auto;'>",p="</div></div>",c=l+"{{# layui.each(d.menus, function(index, item){ }}{{# if ('hr' === item) { }}<hr>{{# } else if (item.header) { }}{{# if (item.withLine) { }}<fieldset class=\"layui-elem-field layui-field-title menu-header\" style=\"margin-left:0;margin-bottom: 0;margin-right: 0\"><legend>{{item.header}}</legend></fieldset>{{# } else { }}<div class='menu-header' style='text-align: {{item.align||'left'}}'>{{item.header}}</div>{{# } }}{{# } else { }}<div class='menu-item'><a href='javascript:;' lay-event='{{item.event}}'>{{# if (item.layIcon){ }}<i class='layui-icon {{item.layIcon}}'></i>&nbsp;{{# } }}<span>{{item.txt}}</span></a></div>{{# } }}{{# }); }}"+p,o={showBy:"click",align:"left",minWidth:10,minHeight:10,maxHeight:300,zIndex:102,gap:8,onHide:function(i,t){},onShow:function(i,t){},scrollBehavior:"follow",backgroundColor:"#FFF"};function t(i,n){s(i||"[lay-"+r+"]").each(function(){var i=s(this),t=new Function("return "+(i.attr("lay-"+r)||"{}"))();i.removeAttr("lay-"+r);var o=i.data(r)||new e(i,s.extend({},t,n||{}));i.data(r,o)})}e.prototype.init=function(){var t=this;if(t.option.menus&&0<t.option.menus.length)n(c).render(t.option,function(i){t.$down=s(i),t.$dom.after(t.$down),t.initSize(),t.initEvent()});else if(t.option.template){var i;i=-1===t.option.template.indexOf("#")?"#"+t.option.template:t.option.template;var o=s.extend(s.extend({},t.option),t.option.data||{});n(l+s(i).html()+p).render(o,function(i){t.$down=s(i),t.$dom.after(t.$down),t.option.success&&t.option.success(t.$down),t.initSize(),t.initEvent()})}else layui.hint().error("下拉框目前即没配置菜单项，也没配置下拉模板。[#"+(t.$dom.attr("id")||"")+",filter="+t.option.filter+"]")},e.prototype.initSize=function(){this.$down.find(".dropdown-pointer").css("width",2*this.option.gap),this.$down.find(".dropdown-pointer").css("height",2*this.option.gap)},e.prototype.initPosition=function(){var i,t,o,n,e=this.$dom.offset(),s=this.$dom.outerHeight(),d=this.$dom.outerWidth(),a=e.left,h=e.top-window.pageYOffset,r=this.$down.outerHeight(),l=this.$down.outerWidth();o="right"===this.option.align?(i=a+d-l+this.option.gap,-Math.min(l-2*this.option.gap,d)/2):"center"===this.option.align?(i=a+(d-l)/2,(l-2*this.option.gap)/2):(i=a-this.option.gap,Math.min(l-2*this.option.gap,d)/2),t=s+h;var p=this.$down.find(".dropdown-pointer");n=-this.option.gap,0<o?(p.css("left",o),p.css("right","unset")):(p.css("left","unset"),p.css("right",-1*o)),t+r>=window.innerHeight?(t=h-r,n=r-this.option.gap,p.css("top",n).addClass("bottom")):p.css("top",n).removeClass("bottom"),i+l>=window.innerWidth&&(i=window.innerWidth-l+this.option.gap),this.$down.css("left",i),this.$down.css("top",t)},e.prototype.show=function(){var o,i;this.initPosition(),this.$down.addClass("layui-show"),this.opened=!0,o=this,i=h[a]||[],s.each(i,function(i,t){t(o)}),this.option.onShow&&this.option.onShow(this.$dom,this.$down)},e.prototype.hide=function(){this.fcd=!1,this.$down.removeClass("layui-show"),this.opened=!1,this.option.onHide&&this.option.onHide(this.$dom,this.$down)},e.prototype.hideWhenCan=function(){this.mouseInCompoent||this.fcd||this.hide()},e.prototype.toggle=function(){this.opened?this.hide():this.show()},e.prototype._onScroll=function(){var i=this;"follow"===this.option.scrollBehavior?setTimeout(function(){i.initPosition()},1):this.hide()},e.prototype.initEvent=function(){var i,t,o,n=this;t=function(i){i!==n&&n.hide()},(o=h[i=a]||[]).push(t),h[i]=o,n.$dom.mouseenter(function(){n.mouseInCompoent=!0,"hover"===n.option.showBy&&(n.fcd=!0,n.$down.focus(),n.show())}),n.$dom.mouseleave(function(){n.mouseInCompoent=!1}),n.$down.mouseenter(function(){n.mouseInCompoent=!0,n.$down.focus()}),n.$down.mouseleave(function(){n.mouseInCompoent=!1}),"click"===n.option.showBy&&n.$dom.on("click",function(){n.fcd=!0,n.toggle()}),d.on("click",function(){n.mouseInCompoent||(n.fcd=!1,n.hideWhenCan())}),s(window).on("scroll",function(){n._onScroll()}),n.$dom.parents().on("scroll",function(){n._onScroll()}),s(window).on("resize",function(){n.initPosition()}),n.$dom.on("blur",function(){n.fcd=!1,n.hideWhenCan()}),n.$down.on("blur",function(){n.fcd=!1,n.hideWhenCan()}),n.option.menus&&s("["+r+"-id='"+n.option.downid+"']").on("click","a",function(){var i=(s(this).attr("lay-event")||"").trim();i?(layui.event.call(this,r,r+"("+n.option.filter+")",i),n.hide()):layui.hint().error("菜单条目["+this.outerHTML+"]未设置event。")})},t(),i(r,{suite:t,onFilter:function(i,t){layui.onevent(r,r+"("+i+")",function(i){t&&t(i)})},version:"2.0.0"})});