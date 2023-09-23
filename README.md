![Static Badge](https://img.shields.io/badge/Stand_with-China-F61C1C) 
![Static Badge](https://img.shields.io/badge/Stand_with-USA-red)
![Static Badge](https://img.shields.io/badge/Stand_with-Ukraine-ffd700) 
![Static Badge](https://img.shields.io/badge/Stand_with-Russia-blue) 
![Static Badge](https://img.shields.io/badge/Stand_with-Peace-white) 


# ClassScheduler 

一个可用于在桌面显示课表（可用于希沃白板）的工具

## 关于双模式

本项目支持作为独立程序启动, 并同时适配 KitX, 可以作为 KitX 插件导入 KitX Dashboard

### 独立程序启动

`ClassScheduler.Runner` 项目作为容器, 加载并启动 `ClassScheduler.WPF` 项目

```shell
# 获取源码
git clone git@github.com:StardustSeemsInk/ClassScheduler.git

# 进入项目目录
cd 'ClassScheduler/ClassScheduler.Runner'

# 运行程序
dotnet run
```

### 如何为 KitX 打包

> [!WARNING]  
> KitX Project 正在开发新的 KitX Plugin Studio 用于插件制作及打包等, 以下方式不建议继续使用

```shell
# 获取源码
git clone git@github.com:StardustSeemsInk/ClassScheduler.git

# 进入项目目录
cd 'ClassScheduler/ClassScheduler.WPF'

# 使用 Release Profile 构建
dotnet build -c Release

# 进入生成目录
cd bin/Release/net7.0-windows/

# 使用 kxpmaker 生成 kxp 插件包
# 当前目录下会出现 net7.0-windows.kxp 文件
# 需要提前安装 kxpmaker, 使用 kxpmaker --help 命令查看更多用法
kxpmaker -s .
```

# About KitX Project

官网：https://kitx.apps.catrol.cn/

Github：https://github.com/Crequency/KitX/
