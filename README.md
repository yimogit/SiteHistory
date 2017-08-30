## 那年那站那样	
> 伴随着时间，记录着网站的历史.       
> 记录下网站现在的样子，待那年今日.         
> 那一年，那个网站，是那个样子.     


## 技术栈		
1. [`.NET Core`](https://www.microsoft.com/net/core#windowscmd):.NET Core 是.NET Framework的新一代版本，具有跨平台 (Windows、Mac OSX、Linux) 能力的应用程序开发框架 (Application Framework)。
2. [`Selenium`](https://github.com/SeleniumHQ/selenium):一个用于Web应用程序测试的工具。Selenium测试直接运行在浏览器中，就像真正的用户在操作一样。结合[phantomjs](http://phantomjs.org/)等驱动可以实现页面自动化。
3. [`Github`](https://github.com/):一个面向开源及私有软件项目的托管平台，因为只支持git 作为唯一的版本库格式进行托管，故名GitHub,<span style="color:white;">又名GayHub</span>
4. [`Travis CI`](https://travis-ci.org):采用yaml格式配置，简洁清新的开源持续集成构建项目。
> 我将其用来打包vue的纯工具站点[`metools`](https://github.com/yimogit/metools),以及.net core程序([`SiteHistory`](https://github.com/yimogit/SiteHistory))
> 啥，还不会？戳这里→→[使用travis-ci自动部署github上的项目](http://www.cnblogs.com/morang/p/7228488.html)

## 程序运行		
0. 安装[phantomjs](http://phantomjs.org/)，并设置环境变量(Travis CI环境提供PhantomJS预装)
1. [安装.net core2.0 SDK](https://www.microsoft.com/net/core#windowscmd)
2. 执行命令：`dotnet run 参数1[名称] 参数2[网页链接] 参数3[图片格式] 参数4[等待时间] 参数5[保存目录]`
	1. `dotnet run baidu https://www.baidu.com/`
		> 保存[https://www.baidu.com]页面的截图名称为[baidu.jpg]
	2. `dotnet run baidu https://www.baidu.com/ png`
		>指定图片类型为`png`
	3. `dotnet run baidu https://www.baidu.com/ jpg 20`
		> 加载完毕后等待20s后截图(图片加载或网站速度过慢)
	4. `dotnet run baidu https://www.baidu.com/ jpg 10 download-test`
		> 下载的图片保存到download-test文件夹下
	4. `dotnet run baidu https://www.baidu.com/ jpg 10 download-test "alert('233')"`
		> 加载完毕后执行一段js
## `.travis.yml`配置说明

```
# 语言为scharp,系统为ubuntu14.04(代号trusty),.netcore 版本2.0
# 使用测试框架即可使用phantomjs
language: csharp
dist: trusty
mono: none
dotnet: 2.0.0
sudo: required

## 若无需要的组件，可以在此install节点安装
# install:
#  - export DOTNET_INSTALL_DIR="$PWD/.dotnetsdk"
#  - curl -sSL https://raw.githubusercontent.com/dotnet/cli/rel/2.0.0/scripts/obtain/dotnet-install.sh | bash /dev/stdin --version "$CLI_VERSION" --install-dir "$DOTNET_INSTALL_DIR"
#  - export PATH="$DOTNET_INSTALL_DIR:$PATH"

# 检查dotnet信息，还原依赖包,运行任务
script:
  - dotnet --info
  - dotnet --version
  - dotnet restore
  - dotnet run ip http://1212.ip138.com/ic.asp png 
  - dotnet run acfun http://www.acfun.cn/ jpg 20
  - dotnet run bilibili https://www.bilibili.com jpg 20
  - dotnet run youtube https://www.youtube.com jpg 20
  - dotnet run google https://www.google.com
  
# 将截图提交到 ${P_BRANCH} 设定的分支中(gh-pages) 
after_script:
  - cd download
  - git init
  - git config user.name "${U_NAME}"
  - git config user.email "${U_EMAIL}"
  - git add .
  - git commit -m "add imgs"
  - git remote add orginimgs "https://${GH_TOKEN}@${GH_REF}"
  - export abc='date +%Y%m%d'
  - echo "year:$($abc)"
  - git push --force --quiet orginimgs master:${P_BRANCH}
  - git push --force --quiet orginimgs master:${P_BRANCH}_$($abc)

branches:
  only:
    - master

```

> 亲测搭配[travis-ci](https://travis-ci.org/)食用最佳，Fork之后，前往travis-ci配置即可 [参阅文章:使用travis-ci自动部署github上的项目](http://www.cnblogs.com/morang/p/7228488.html)     
> 欢迎Fork，Star，欢迎分享值得记录的网站。