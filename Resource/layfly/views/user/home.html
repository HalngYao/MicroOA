{{# 
var info = d.info || {}, jie = d.jie, da = d.da;
}}

<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8"/>
<meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1">
<title>{{info.username}}的主页 - {{lay.base.name}}</title>
{{ include ../common/link }}
</head>
<body class="fly-full">

{{ include ../common/header }}

<div class="fly-home fly-panel" style="background-image: url();">
  <img src="{{info.avatar}}" alt="{{info.username}}">
  {{# if(info.approve){ }}
    <i class="iconfont icon-renzheng" title="Fly社区认证"></i>
  {{# } }}
  <h1>
    {{info.username}} 
    <i class="iconfont 
    {{# if(info.sex == 0){ }}
      icon-nan
    {{# } else if(info.sex == 1){ }}
      icon-nv
    {{# } }}
    "></i> 

    {{# if(info.rmb) { }}
      <i class="layui-badge fly-badge-vip">VIP{{ lay.util.vip(info.rmb) }}</i>
    {{# } }}
    {{# if(info.auth == 1){ }}
      <span style="color:#c00;">（管理员）</span>
    {{# } else if(info.auth == 2) { }}
      <span style="color:#5FB878;">（社区之光）</span>
    {{# } else if(info.auth == -1) { }}
      <span>（该号已被封）</span>
    {{# } }}
  </h1>

  {{# if(info.approve){ }}
    <p style="padding: 10px 0; color: #5FB878;">认证信息：{{ info.approve }}</p>
  {{# } }}

  <p class="fly-home-info">
    <i class="iconfont icon-kiss" title="飞吻"></i><span style="color: #FF7200;">{{info.experience}}飞吻</span>
    <i class="iconfont icon-shijian"></i><span>{{lay.time(info.joinTime, 1)}} 加入</span>
    <i class="iconfont icon-chengshi"></i><span>来自{{= info.city||'中国 某城'}}</span>
  </p>

  <p class="fly-home-sign">（{{= info.sign||'这个人懒得留下签名' }}）</p>

  {{# if(user.id !== info.id && d.res === 'http://local.res.layui.com/'){ 
    var userinfo = JSON.stringify({
      uid: 168*info.id
      ,avatar: info.avatar
      ,type: 'friend'
      ,username: info.username
    });
  }}
  <div class="fly-sns" data-user="{{ encodeURIComponent(userinfo) }}">
    <a href="javascript:;" class="layui-btn layui-btn-primary fly-imActive" data-type="addFriend">加为好友</a>
    <a href="javascript:;" class="layui-btn layui-btn-normal fly-imActive" data-type="chat">发起会话</a>
  </div>
  {{# } }}

</div>

<div class="layui-container">
  <div class="layui-row layui-col-space15">
    <div class="layui-col-md6 fly-home-jie">
      <div class="fly-panel">
        <h3 class="fly-panel-title">{{info.username}} 最近的提问</h3>
        <ul class="jie-row">
        {{# jie.forEach(function(item, index){ }}
          <li>
            {{ item.status == 1 ? '<span class="fly-jing layui-hide-xs">精</span>' : ''}}
            <a href="/jie/{{item.id}}/" class="jie-title">{{= item.title}}</a>
            <i>{{lay.time(item.time, 1)}}</i>
            <em class="layui-hide-xs">{{item.hits}}阅/{{item.comment}}答</em>
          </li>
        {{# }); }}
        
        {{# if(jie.length === 0){ }}
        <div class="fly-none" style="min-height: 50px; padding:30px 0; height:auto;"><i style="font-size:14px;">没有发表任何求解</i></div>
        {{# } }}
        </ul>
      </div>
    </div>
    
    <div class="layui-col-md6 fly-home-da">
      <div class="fly-panel">
        <h3 class="fly-panel-title">{{info.username}} 最近的回答</h3>
        <ul class="home-jieda">
        {{# da.forEach(function(item, index){ }}
        <li>
          <p>
          <span>{{lay.time(item.time)}}</span>
          在<a href="/jie/{{item.jid}}/#item-{{item.time}}" target="_blank">{{= item.title||'求解'}}</a>中回答：
          </p>
          <div class="home-dacontent">{{d.content(item.content)}}</div>
        </li>
        {{# }); }}
        
        {{# if(da.length === 0){ }}
        <div class="fly-none" style="min-height: 50px; padding:30px 0; height:auto;"><span>没有回答任何问题</span></div>
        {{# }; }}
        </ul>
      </div>
    </div>
  </div>
</div>

{{ include ../common/footer }}

</body>
</html>