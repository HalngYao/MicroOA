<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1">
<title>{{d.edit ? '编辑帖子' : '发表新帖'}} - {{lay.base.name}}</title>
{{ include ../common/link }}
</head>
<body>

{{ include ../common/header }}

<div class="layui-container fly-marginTop">
  <div class="fly-panel" pad20 style="padding-top: 5px;">
    {{# if(d.edit && !d.myself){ }}
    <div class="fly-none">并无权限</div>
    {{# } else { }}
    <div class="layui-form layui-form-pane">
      <div class="layui-tab layui-tab-brief" lay-filter="user">
        <ul class="layui-tab-title">
          <li class="layui-this">{{d.edit ? '编辑帖子' : '发表新帖'}}</li>
        </ul>
        <div class="layui-form layui-tab-content" id="LAY_ucm" style="padding: 20px 0;">
          <div class="layui-tab-item layui-show">
            <form action="{{d.edit ? '/jie/edit/' : ''}}" method="post">
              <div class="layui-row layui-col-space15 layui-form-item">
                <div class="layui-col-md3">
                  <label class="layui-form-label">所在专栏</label>
                  <div class="layui-input-block">
                    <select lay-verify="required" name="class" lay-filter="column" {{ d.edit && user.auth < 1 ? 'disabled' : '' }}>
                      <option></option>
                      {{# for(var classes in lay.base.classes){ 
                        if(classes >= 168){ 
                          if(user.auth == 1){ }}
                            <option value="{{ classes }}" {{(d.edit && d.edit['class'] == classes) ? 'selected' : ''}}>{{ lay.base.classes[classes].title }}</option>
                          {{# }
                        } else { }}
                          <option value="{{ classes }}" {{(d.edit && d.edit['class'] == classes) ? 'selected' : ''}}>{{ lay.base.classes[classes].title }}</option>
                        {{# }
                      } }}
                    </select>
                  </div>
                </div>
                <div class="layui-col-md9">
                  <label for="L_title" class="layui-form-label">标题</label>
                  <div class="layui-input-block">
                    {{# if(d.edit){ }}
                    <input type="text" id="L_title" name="title" required lay-verify="required" autocomplete="off" value="{{d.edit.title}}" {{ user.auth >= 1 ? 'required' : 'readonly' }} title="标题不可修改" class="layui-input">
                    <input type="hidden" name="id" value="{{d.edit.id}}">
                    {{# } else { }}
                    <input type="text" id="L_title" name="title" required lay-verify="required" autocomplete="off" class="layui-input">
                    {{# } }}
                  </div>
                </div>
              </div>
              {{# var spe = {}; if(d.edit){
                spe = d.edit.spe || {};
              } }}
              <div class="layui-row layui-col-space15 layui-form-item{{ d.edit ? (d.edit['class'] != '0' ? ' layui-hide' : '') : ' layui-hide' }}" id="LAY_quiz">
                <div class="layui-col-md3">
                  <label class="layui-form-label">所属产品</label>
                  <div class="layui-input-block">
                    <select name="project">
                      <option></option>
                      <option value="layui" {{ spe.project === 'layui' ? 'selected' : '' }}>layui</option>
                      <option value="独立版layer" {{ spe.project === '独立版layer' ? 'selected' : '' }}>独立版layer</option>
                      <option value="独立版layDate" {{ spe.project === '独立版layDate' ? 'selected' : '' }}>独立版layDate</option>
                      <option value="LayIM" {{ spe.project === 'LayIM' ? 'selected' : '' }}>LayIM</option>
                      <option value="Fly社区模板" {{ spe.project === 'Fly社区模板' ? 'selected' : '' }}>Fly社区模板</option>
                    </select>
                  </div>
                </div>
                <div class="layui-col-md3">
                  <label class="layui-form-label" for="L_version">版本号</label>
                  <div class="layui-input-block">
                    <input type="text" id="L_version" value="{{ spe.version || '' }}" name="version" autocomplete="off" class="layui-input">
                  </div>
                </div>
                <div class="layui-col-md6">
                  <label class="layui-form-label" for="L_browser">浏览器</label>
                  <div class="layui-input-block">
                    <input type="text" id="L_browser"  value="{{ spe.browser || '' }}"name="browser" placeholder="浏览器名称及版本，如：IE 11" autocomplete="off" class="layui-input">
                  </div>
                </div>
              </div>
              <div class="layui-form-item layui-form-text">
                <div class="layui-input-block">
                  <textarea id="L_content" name="content" required lay-verify="required" placeholder="详细描述" class="layui-textarea fly-editor" style="height: 260px;">{{= d.edit ? d.edit.content : ''}}</textarea>
                </div>
              </div>
              <div class="layui-form-item">
                
                <div class="layui-inline">
                  <label class="layui-form-label">悬赏飞吻</label>
                  <div class="layui-input-inline" style="width: 190px;">
                    <select name="experience" {{ d.edit ? 'disabled' : '' }}>
                      <option value="20" {{(d.edit && d.edit.experience == 20) ? 'selected' : ''}}>20</option>
                      <option value="30" {{(d.edit && d.edit.experience == 30) ? 'selected' : ''}}>30</option>
                      <option value="50" {{(d.edit && d.edit.experience == 50) ? 'selected' : ''}}>50</option>
                      <option value="60" {{(d.edit && d.edit.experience == 60) ? 'selected' : ''}}>60</option>
                      <option value="80" {{(d.edit && d.edit.experience == 80) ? 'selected' : ''}}>80</option>
                    </select>
                  </div>
                  {{# if(!d.edit){ }}
                  <div class="layui-form-mid layui-word-aux">发表后无法更改飞吻</div>
                  {{# } }}
                </div>
              </div>
              <div class="layui-form-item">
                <label for="L_vercode" class="layui-form-label">人类验证</label>
                <div class="layui-input-inline">
                  <input type="text" id="L_vercode" name="vercode" required lay-verify="required" placeholder="请回答后面的问题" autocomplete="off" class="layui-input">
                </div>
                <div class="layui-form-mid">
                  <span style="color: #c00;">{{d.vercode}}</span>
                </div>
              </div>
              <div class="layui-form-item">
                <button class="layui-btn" lay-filter="*" lay-submit>立即发布</button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
    {{# } }}
  </div>

</div>

{{ include ../common/footer }}

</body>
</html>