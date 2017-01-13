# You-Get-Tool
You-Get小工具
=======
##使用帮助  [简单演示](http://www.bilibili.com/video/av7997075/)
* 下载 `You-Get` [@You-Get Releases](https://github.com/soimort/you-get/releases)
* 下载 `VLC` [@videolan.org](http://www.videolan.org/)
* 下载 `exe`()>_<|||) [@GitHub](https://github.com/xiaocsl/You-Get-Tool/releases) 并将此 exe 放入 You-Get 根目录
* 首次打开需点击 `写入注册表` 按钮
* 安装 `Tampermonkey` 并添加脚本
* JS 代码很简单,根据自己需求添加待匹配网站
* 推荐设置 VLC `仅允许运行一个实例`和`播放并退出`

##起因

最开始是因为每次下载视频时都要手动输入各种参数,控制台操作复制粘贴也不方便.

所以想用 C# 拖了个 GUI 直接调用 `cmd.exe` 参数配置不用管,还可以根据域名来自动选择是否使用代理.

没几行代码的事,立刻准备动手.刚才 VS 里新建项目突然想起来 You-Get 能直接调用播放器,干脆写成浏览器打开视频网址后直接用 VLC 播放算了,不光不用忍受 Flash 这渣渣,~~(还能去广告)~~

##思路
* C# 主要就通过调用 CMD 来执行 You-Get 
* 在注册表里添加一个 URI Scheme ,注册表结构如下:
```javascript
HKEY_CLASSES_ROOT
   ygtxxx
      URL Protocol = ""
      shell
         open
            command
               (Default) = "{Path}\YouGet.exe" "%1"
```
* 通过 Tampermonkey 脚本,来匹配在线播放链接,并发出请求.

##You-Get + VLC 的各种小问题
* 优酷视频分割太多
* 爱奇艺不显示时间
* 部分网站缓冲速度太慢
* 等等.........


##Tampermonkey 

* 只包含了几个自己常用的网站,可自行添加,只要 You-Get 支持即可.
* 实际上就一行代码,前面的转换成 You-Get 支持的微博视频地址.

```javascript
// ==UserScript==
// @name         You-Get Tool
// @namespace    http://tampermonkey.net/
// @version      0.1
// @description  try to take over the world!
// @author       You
// @include      http://www.iqiyi.com/v_*
// @include      http://v.youku.com/v_show/id_*
// @include      http://video.sina.com.cn/*
// @include      http://weibo.com/tv/v/*
// @include      http://tv.sohu.com/*html*
// @include      http://www.tudou.com/*html*
// @grant        none
// @require      https://cdn.staticfile.org/jquery/3.1.1/jquery.min.js
// ==/UserScript==

(function() {
    'use strict';
    var openUrl = window.location.toString();
    //取Url参数
    function getUrlParam(name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
        var r = window.location.search.substr(1).match(reg);
        if (r != null) return unescape(r[2]); return null;
    }
    //微博视频需转换Url格式
    if(openUrl.indexOf("/weibo.com/tv/v") != -1){
        var tempFid = getUrlParam("fid");
        if(tempFid == null){
            return;
        }
        openUrl = "http://video.weibo.com/show?fid=" + tempFid;
    }
    //移除页面中的flash元素
    $("#flash,#player,#myflashBox,#playerRoom").remove();
    //请求应用程序
    window.location.href = "ygtxxx:sendmsg?url=" + openUrl;
    //关闭页面
    setTimeout("window.close()",100);
})();
```

##PS 

所有的算下来没几行代码,但是效果很给力,各种感谢  [@You-Get](https://you-get.org/) 

很少用 C# ,大部分代码都是搜出来的.各种感谢 [@Google](https://www.google.com/) 

代码很烂,各种感谢 [@吃瓜群众](#) 

其实我最好奇的是 OS X 平台上为什么没人写一个类似的呢.随便想了下就有好多解决方案.

对 MacBook 来说,收益更大.也希望这个小软件给大家提了个小思路.

最后,估计不会进行任何维护,代码量实在太少了,任何人瞥一眼就可以自己写一份了.
>>>>>>> f
