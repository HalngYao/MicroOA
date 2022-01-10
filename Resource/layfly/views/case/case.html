{{# 

var user = d.session.user || {};
var cases = d.rows || [];
var year = new Date().getFullYear();
}}

<!DOCTYPE html>
<html style="background-color: #e2e2e2;">
<head>
<meta charset="utf-8">
<meta name="keywords" content="{{ lay.base.keywords }}">
<meta name="description" content="{{ lay.base.description }}">
<meta name="renderer" content="webkit">
<meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1">
<meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1">
<title>发现 Layui {{ d.year || year }} 年度最佳案例</title>
{{ include ../common/link }}
<style>
.header{border-bottom: 1px solid #404553; border-right: 1px solid #404553;}
</style>
</head>
<body class="fly-full">

{{ include ../common/header }}

<div class="fly-case-header">
  <p class="fly-case-year">{{ d.year || year }}</p>
  <a href="/case/{{ year }}/">
    <img class="fly-case-banner" src="{{d.res}}images/fly/case.png" alt="发现 Layui 年度最佳案例">
  </a>
  <div class="fly-case-btn">
    <a href="javascript:;" class="layui-btn layui-btn-big fly-case-active" data-type="push">提交案例</a>
    {{# if(user.id){ }}
    <a href="/case/u/{{168*user.id}}" class="layui-btn layui-btn-primary layui-btn-big">我的案例</a>
    {{# } else { }}
    <a href="/user/login/" class="layui-btn layui-btn-primary layui-btn-big">我的案例</a>
    {{# } }}
    
    {{# if(d.year === '2016'){ }}
    <a href="http://fly.layui.com/jie/5077.html" target="_blank" style="padding: 0 15px; text-decoration: underline">参赛规则</a>
    {{# } else { }}
    <a href="http://fly.layui.com/jie/11996/" target="_blank" style="padding: 0 15px; text-decoration: underline">案例要求</a>
    {{# } }}
  </div>
</div>

<div class="fly-main" style="overflow: hidden;">

  <div class="fly-tab-border fly-case-tab">
    <span>
      <a href="/case/2017/" {{ d.year == 2017 ? 'class="tab-this"' : '' }}>2017年度</a>
      <a href="/case/2016/" {{ d.year == 2016 ? 'class="tab-this"' : '' }}>2016年度</a>
    </span>
  </div>
  {{# if(!d.params.uid){ }}
  <div class="layui-tab layui-tab-brief">
    <ul class="layui-tab-title">
      <li {{ d.pageType.indexOf('case/'+ d.year +'/top') === -1 ? 'class="layui-this"' : '' }}><a href="/case/{{d.year}}/">按提交时间</a></li>
      <li {{ d.pageType.indexOf('case/'+ d.year +'/top') !== -1 ? 'class="layui-this"' : '' }}><a href="/case/{{d.year}}/top/">按点赞排行</a></li>
    </ul>
  </div>
  {{# } }}

  <ul class="fly-case-list">
  {{# cases.forEach(function(item){ 
    var praise_user = JSON.parse(item.praise_user||'{}');
  }}
    <li data-id="{{item.id}}">
      <a class="fly-case-img" href="{{item.link}}" target="_blank" rel="nofollow">
        <img src="{{item.cover}}" alt="{{item.title}}">
        <cite class="layui-btn layui-btn-primary layui-btn-small">去围观</cite>
      </a>
      <h2><a href="{{item.link}}" target="_blank" rel="nofollow">{{item.title}}</a></h2>
      <p class="fly-case-desc">{{item.desc}}</p>
      <div class="fly-case-info">
        <a href="/u/{{168*item.uid}}" class="fly-case-user" target="_blank"><img src="{{item.user.avatar}}"></a>
        <p class="layui-elip" style="font-size: 12px;"><span style="color: #666;">{{item.user.username}}</span> {{lay.time(item.create_time, true)}}</p>
        <p>获得<a class="fly-case-nums fly-case-active" href="javascript:;" data-type="showPraise" style=" padding:0 5px; color: #01AAED;">{{item.praise||0}}</a>个赞</p>
        <button class="layui-btn {{# if(!praise_user[user.id]){ }}layui-btn-primary{{# } }} fly-case-active" data-type="praise">{{ praise_user[user.id] ? '已赞' : '点赞' }}</button>
      </div>
    </li>
  {{# }); }}
  </ul>
  
  {{# if(cases.length === 0){ }}
  <blockquote class="layui-elem-quote layui-quote-nm">{{d.year ? '暂无'+ d.year +'年度 的案例' : '您还未发布案例'}}</blockquote>
  {{# } }}

  <div style="text-align: center;">
    {{ d.laypage }}
  </div>

</div>

{{ include ../common/footer }}

</body>
</html>