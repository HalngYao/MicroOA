
{{# 
var rows = d.rows;
}}

<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1">
<title>{{= rows.title}} - {{lay.base.name}}</title>
<meta name="description" content="{{= d.description }}">

{{ include ../common/link }}
</head>
<body>

{{ include ../common/header }}
<div class="layui-hide-xs">
  {{ include ../common/column }}
</div>

{{# var myself = rows.uid === user.id; }}

<div class="layui-container">
  <div class="layui-row layui-col-space15">
    <div class="layui-col-md8 content detail">
      <div class="fly-panel detail-box">
        <h1>{{=rows.title}}</h1>
        <div class="fly-detail-info">
          {{# if(rows.status == -1){ }}
            <span class="layui-badge">审核中</span>
          {{# } }}
          <span class="layui-badge layui-bg-green fly-detail-column">
            {{ lay.base.classes[rows['class']] ? lay.base.classes[rows['class']].title : '提问' }}
          </span>
          {{# if(rows.accept === -1){ }}
            <span class="layui-badge" style="background-color: #999;">未结</span>
          {{# } else if(rows.accept >= 0 ){ }}
            <span class="layui-badge" style="background-color: #5FB878;">已结</span>
          {{# } }}

          {{ rows.stick > 0 ? '<span class="layui-badge layui-bg-black">置顶</span>' :'' }}
          {{ rows.status == 1 ? '<span class="layui-badge layui-bg-red">精帖</span>' : ''}}
          
          <div class="fly-admin-box" data-id="{{rows.id}}">
            {{# if(user.auth == 1){ }}
            <span class="layui-btn layui-btn-xs jie-admin" type="del">删除</span>
            {{# } }}
            
            {{# if(user.auth == 1){ }}
              {{# if(rows.stick > 0){ }}
                <span class="layui-btn layui-btn-xs jie-admin" type="set" field="stick" rank="0" style="background-color:#ccc;">取消置顶</span>
              {{# } else { }}
                <span class="layui-btn layui-btn-xs jie-admin" type="set" field="stick" rank="1">置顶</span> 
              {{# };
              if(rows.status == 1){ }}
                <span class="layui-btn layui-btn-xs jie-admin" type="set" field="status" rank="0" style="background-color:#ccc;">取消加精</span>
              {{# } else { }}
                <span class="layui-btn layui-btn-xs jie-admin" type="set" field="status" rank="1">加精</span> 
              {{# }; 
            } }}
          </div>
          <span class="fly-list-nums"> 
            <a href="#comment"><i class="iconfont" title="回答">&#xe60c;</i> {{rows.comment}}</a>
            <i class="iconfont" title="人气">&#xe60b;</i> {{rows.hits}}
          </span>
        </div>
        <div class="detail-about">
          <a class="fly-avatar" href="/u/{{168*rows.uid}}/">
            <img src="{{rows.user.avatar}}" alt="{{rows.user.username}}">
          </a>
          <div class="fly-detail-user">
            <a href="/u/{{168*rows.uid}}/" class="fly-link">
              <cite>{{rows.user.username}}</cite>
              {{# if(rows.user.approve){ }}
                <i class="iconfont icon-renzheng" title="认证信息：{{ rows.user.approve }}"></i>
              {{# } }}
              {{# if(rows.user.rmb){ }}
                {{ lay.util.vipBadge(rows.user.rmb) }}
              {{# } }}
            </a>
            <span>{{lay.time(rows.time, true)}}</span>
          </div>
          <div class="detail-hits" id="LAY_jieAdmin" data-id="{{rows.id}}">
            <span style="padding-right: 10px; color: #FF7200">悬赏：{{rows.experience}}飞吻</span>  
            {{# if((user.username && myself && rows.accept == -1) || user.auth == 1){ }}
              <span class="layui-btn layui-btn-xs jie-admin" type="edit"><a href="/jie/edit/{{rows.id}}">编辑此贴</a></span>
            {{# } }} 
          </div>
        </div>
        <div class="detail-body photos">
          {{# if(rows['class'] == '0' && rows.spe){ }}
          <table class="layui-table">
            <tbody>
              <tr>
                <td>版本：{{ rows.spe.project || '' }} {{ rows.spe.version || '' }}</td>
                <td>浏览器：{{ rows.spe.browser || '' }}</td>
              </tr>
            </tbody>
          </table>
          {{# } }}
          {{ d.content(rows.content) }} 
        </div>
      </div>

      {{# var jieda = rows.jieda; }}
      
      <div class="fly-panel detail-box" id="flyReply">
        <fieldset class="layui-elem-field layui-field-title" style="text-align: center;">
          <legend>回帖</legend>
        </fieldset>

        <ul class="jieda" id="jieda">
        {{# jieda.forEach(function(item, index){ 
          var myda = item.user.username === user.username;
        }}
          <li data-id="{{item.id}}" {{item.id == rows.accept ? 'class="jieda-daan"' : '' }}>
            <a name="item-{{item.time}}"></a>
            <div class="detail-about detail-about-reply">
              <a class="fly-avatar" href="/u/{{168*item.user.id}}/">
                <img src="{{item.user.avatar}}" alt="{{item.user.username}}">
              </a>
              <div class="fly-detail-user">
                <a href="/u/{{168*item.user.id}}/" class="fly-link">
                  <cite>{{item.user.username}}</cite>
                  {{# if(item.user.approve){ }}
                    <i class="iconfont icon-renzheng" title="认证信息：{{ item.user.approve }}"></i>
                  {{# } }}
                  {{# if(item.user.rmb){ }}
                    {{ lay.util.vipBadge(item.user.rmb) }}
                  {{# } }}                 
                </a>

                {{# if(item.user.username === rows.username){ }}
                <span>(楼主)</span>
                {{# } else if(item.user.auth == 1) { }}
                <span style="color:#5FB878">(管理员)</span>
                {{# } else if(item.user.auth == 2) { }}
                <span style="color:#FF9E3F">（社区之光）</span>
                {{# } else if(item.user.auth == -1) { }}
                <span style="color:#999">（该号已被封）</span>
                {{# } }}
              </div>

              <div class="detail-hits">
                <span>{{lay.time(item.time, true)}}</span>
              </div>

              {{# if(item.id == rows.accept){ }}
              <i class="iconfont icon-caina" title="最佳答案"></i>
              {{# } }}
            </div>
            <div class="detail-body jieda-body photos">
              {{ d.content(item.content) }}
            </div>
            <div class="jieda-reply">
            <span class="jieda-zan {{d.session['zan'+item.id] ? 'zanok' : ''}}" type="zan">
              <i class="iconfont icon-zan"></i>
              <em>{{item.praise}}</em>
            </span>
            <span type="reply">
              <i class="iconfont icon-svgmoban53"></i>
              回复
            </span>
            {{# if(user.auth == 1 || user.auth == 2 || (user.username && myself && !myda)){ }}
              <div class="jieda-admin">
              {{# if(user.auth == 1 || (user.auth == 2 && item.accept != 1)){ }}
                <span type="edit">
                  编辑
                </span>
                <span type="del">
                  删除
                </span>
                {{# if(rows.accept == -1){ }}  
                <span class="jieda-accept" type="accept">
                  采纳
                </span>
                {{# } }}
              {{# } else if(rows.accept == -1 && !myda){ }}
                <span class="jieda-accept" type="accept">
                  采纳
                </span>
              {{# } }}
              </div>
            {{# } }}
            </div>
          </li>
        {{# }); if(jieda.length === 0){ }}
          <li class="fly-none">消灭零回复</li>
        {{# } }}
        </ul>
        
        <div style="text-align: center">
          {{ d.laypage }}
        </div>
        
        <div class="layui-form layui-form-pane">
          <form action="/jie/reply/" method="post">
            <div class="layui-form-item layui-form-text">
              <a name="comment"></a>
              <div class="layui-input-block">
                <textarea id="L_content" name="content" required lay-verify="required" placeholder="请输入内容"  class="layui-textarea fly-editor" style="height: 150px;"></textarea>
              </div>
            </div>
            <div class="layui-form-item">
              <input type="hidden" name="jid" value="{{rows.id}}">
              <input type="hidden" name="daPages" value="{{rows.jieda.pages}}">
              <button class="layui-btn" lay-filter="*" lay-submit>提交回复</button>
            </div>
          </form>
        </div>
      </div>
    </div>
    <div class="layui-col-md4">
      {{ include ../common/list-hot }}

      {{ include ../ad/detail }}

      {{ include ../ad/ours }}

      <div class="fly-panel" style="padding: 20px 0; text-align: center;">
        <img src="//cdn.layui.com/upload/2017_8/168_1501894831075_19619.jpg" style="max-width: 100%;" alt="layui">
        <p style="position: relative; color: #666;">微信扫码关注 layui 公众号</p>
      </div>

    </div>
  </div>
</div>

{{ include ../common/footer }}

</body>
</html>