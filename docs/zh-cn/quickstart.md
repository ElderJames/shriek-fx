# 快速开始 

## 环境准备

1. .NET Core SDK 2.0
2. Visual Studio 2017 15.3+ 或者 Visual Studio Code

## 引入Myget源

目前开发版本已发布到MyGet，从Nuget安装时需要添加MyGet的源地址，或者在解决方案根目录添加`NuGet.config`文件，内容如下：

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
	<add key="Shriek-Fx" value="https://www.myget.org/F/shriek-fx/api/v3/index.json" />
	<add key="Nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
```

