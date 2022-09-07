# Micro-OA简单描述
<b>MicroOA是一款不需要专业的开发知识或开发经验，通过页面交互式即可实现动态搭建表单的微型办公自动化系统。</b>
<br>
在日常工作当中，我们面临着各种各样的表单，在开发系统时，若我们采用每个录入界面设计一个输入表单页面, 
这样有多少个录入界面, 就要设计多少个输入表单页面, 因此需要进行大量的表单设计, 而这些表单往往又是类似的, 
大多应用文本框、列表框、单选按钮、复选框等等表单录入元素, 为了减少重复开发的工作量、提高程序的通用性和工作效率,
因此需要一种动态的、灵活的、安全性高的、快速有效的动态设计方法以方便系统管理和维护。
<br/>
<img src="https://user-images.githubusercontent.com/43397016/148747551-fb7b6a2d-600f-4551-b1cf-cb32c852b415.png" width="600px" />


# 一、主要功能
![图片2](https://user-images.githubusercontent.com/43397016/148900409-83defba1-01d4-4654-b18d-0e6594b948ed.png)


**1.开发工具**
- 开发工具：Visual Studio 2019
- 开发语言：C#(Asp.net)
- 框架：.Net Framework 4.5
- 数据库：Sql Server 2008 R2
- UI： Layui、 Layfly
- 引用控件：WangEditor、 xmSelect、 Fullcalendar
- 引用库： Newtonsoft.Json.dll、 ClosedXML.dll、 DocumentFormat.OpenXml.dll
- 运行环境：推荐使用IIS7.5或以上，也可以使用云虚拟主机，ECS等


**2.运行环境**
- Windows Server 2012或以上安装IIS和.net framework4.5（也可以使用云虚拟主机，ECS等）
- 配置应用程序池为集成模式
- 导入初始数据库（初始数据库路径：源代码根目录\Resource\DB\MicroOA-Initial-database.sql）
--初始数据库导入方法，打开MS SQL Server 2008 R2控制面板，新建空白数据库
--在新建的数据库下，新建查询，把如下所有sql命令复制进去，执行命令
- 修改Web.config文件，第18行<br/>
<add name=“ConnectionName” connectionString=“Server=你的数据库IP地址;Database=你的数据库名称;User ID=你的数据库账号;Password=你的数据库密码" providerName="System.Data.SqlClient"/>


# 二、画面展示
**登录画面**
![Login](https://user-images.githubusercontent.com/43397016/148747156-b8a80529-daf9-4199-a2a4-8556bf14a270.png)

**1.首页**
![HomePage](https://user-images.githubusercontent.com/43397016/148747261-e0fa568c-82ad-4859-98d6-ea11706475fa.png)

**2.表单统计（MicroBI）**
![Sats1](https://user-images.githubusercontent.com/43397016/148747295-36c9cedd-0416-4862-9e3b-ad6befa69258.png)

**3.访问量统计**
![Stats2](https://user-images.githubusercontent.com/43397016/148747356-0c222a6f-022a-4e88-94a9-bc9d9c646327.png)


# 三、相关演示
**1. 功能介绍说明【视频】：**
<br/>
知乎：https://www.zhihu.com/zvideo/1463896474149421056
<br/>
B站：https://www.bilibili.com/video/BV1534y1B7J1?spm_id_from=333.999.0.0
<br/>
相关论文：https://zhuanlan.zhihu.com/p/455489559

**2.环境搭建**
<br/>
开发环境搭建：https://micro-oa.com/Views/Info/Detail/27/9
<br/>
运行环境搭建：https://micro-oa.com/Views/Info/Detail/27/4

**3.演示Demo**
<br/>
演示地址：https://micro-oa.com

**4.仓库地址**
<br/>
GitHub： https://github.com/HalngYao/MicroOA
<br/>
Gitee：  https://gitee.com/shueer/MicroOA

# 四、免责声明
- 任何用户在使用Micro-OA微型办公自动化系统前，请您仔细阅读并透彻理解本声明。您可以选择不使用Micro-OA微型办公自动化系统，若您一旦使用Micro-OA微型办公自动化系统，您的使用行为即被视为对本声明全部内容的认可和接受。
- Micro-OA微型办公自动化系统是一款开源免费的微型办公自动化系统 ，主要用于更便捷地搭建表单、审批流。且Micro-OA微型办公自动化系统并不具备「互联网接入、网络数据存储、通讯传输以及窃取用户隐私」中的任何一项与用户数据等信息相关的动态功能，Micro-OA微型办公自动化系统仅是 UI 组件或素材类或静态方法的本地资源。
- Micro-OA微型办公自动化系统其尊重并保护所有用户的个人隐私权，不窃取任何用户计算机中的信息。更不具备用户数据存储等网络传输功能。
您承诺秉着合法、合理的原则使用Micro-OA微型办公自动化系统，不利用Micro-OA微型办公自动化系统进行任何违法、侵害他人合法利益等恶意的行为，亦不将Micro-OA微型办公自动化系统运用于任何违反我国法律法规的 Web 平台。
- 任何单位或个人因下载使用Micro-OA微型办公自动化系统而产生的任何意外、疏忽、合约毁坏、诽谤、版权或知识产权侵犯及其造成的损失 (包括但不限于直接、间接、附带或衍生的损失等)，本开源项目不承担任何法律责任。
- 用户明确并同意本声明条款列举的全部内容，对使用Micro-OA微型办公自动化系统可能存在的风险和相关后果将完全由用户自行承担，本开源项目不承担任何法律责任。
任何单位或个人在阅读本免责声明后，应在《MIT 开源许可证》所允许的范围内进行合法的发布、传播和使用Micro-OA微型办公自动化系统等行为，若违反本免责声明条款或违反法律法规所造成的法律责任(包括但不限于民事赔偿和刑事责任），由违约者自行承担。
- 如果本声明的任何部分被认为无效或不可执行，其余部分仍具有完全效力。不可执行的部分声明，并不构成我们放弃执行该声明的权利。
- 本开源项目有权随时对本声明条款及附件内容进行单方面的变更，并以消息推送、网页公告等方式予以公布，公布后立即自动生效，无需另行单独通知；若您在本声明内容公告变更后继续使用的，表示您已充分阅读、理解并接受修改后的声明内容。

# 五、打赏支持
开源项目不易，若此项目能得到你的青睐，可以打赏支持作者持续开发与维护，感谢所有支持开源的朋友。
<img src="https://user-images.githubusercontent.com/43397016/148755073-ab97daed-3903-4f08-b4a8-c00a50183503.png" width="800px" />




