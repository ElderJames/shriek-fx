# ShriekFx:zap: 
[![](https://img.shields.io/badge/.NET%20Core-2.0.0-brightgreen.svg?style=flat-square)](https://www.microsoft.com/net/download/core) 
[![Build status](https://ci.appveyor.com/api/projects/status/mcwi2kqe0daija6c?svg=true)](https://ci.appveyor.com/project/ElderJames/shriekfx)
[![MyGet Pre Release](https://img.shields.io/myget/shriek-fx/vpre/Shriek.svg?style=flat-square&label=myget)](https://www.myget.org/feed/Packages/shriek-fx)
[![Author](https://img.shields.io/badge/author-ElderJames-brightgreen.svg?style=flat-square)](https://yangshunjie.com)
[![GitHub license](https://img.shields.io/badge/license-MIT-brightgreen.svg?style=flat-square)](https://github.com/ElderJames/ShriekFx/blob/master/LICENSE)  

一个使用 **.NET Core 2.0** 开发的简单易用的领域驱动设计分层框架（DDD+CQRS），宗旨是让小型应用也能用DDD的思想去开发，使开发者告别对领域驱动设计的复杂认识。

### 特性：

1. 领域驱动设计（DDD）
2. 命令查询职责分离（CQRS）
3. 事件驱动架构 (EDA)
4. 事件回溯 （ES）
5. 最终一致性 （Eventually Consistent）
6. [契约即服务](https://elderjames.github.io/shriek-fx/#/zh-cn/service-intro) (通过定义的接口自动获得客户端和服务端实现)
7. 框架中每个组件都有基础实现，最简单时只需一个核心类库就能跑起来
8. 遵循端口与适配器模式，框架组件适配多种第三方组件实现，可从单体架构到面向服务架构按需扩展

---

### [文档](https://yangshunjie.com/shriek-fx)

### 安装Nuget包

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

### 开发环境

1. [Visual Studio 15.3+](https://www.visualstudio.com/zh-hans/thank-you-downloading-visual-studio/?sku=Community&rel=15)
2. [.NET Core SDK 2.0+](https://github.com/dotnet/core/blob/master/release-notes/download-archive.md)

---

### 示例项目

[ShriekFX.CMS](https://github.com/ElderJames/ShriekFx.CMS) 开发中

其他示例在Samples目录下

### 任务列表（会不断调整）：

- C端
  - [x] 命令总线 CommandBus
  - [x] 事件总线 EventBus
  - [x] 进程内异步队列
  - [x] 内存事件缓存
  - [x] 接口实现自动注册
  - 事件存储 + 聚合快照（备忘录模式）
	- [x] 内存模式 *(聚合修改后立刻持久化)*
	- [x] EF Core实现
	- NoSQL实现
		- [x] LiteDB
		- [x] Cosmos DB （MongoDB API）
	- [x] InfluxDB (时序数据库)
	- [x] Redis
  - Bus / 消息队列（MQ）
	- [x] RabbitMQ
  - [ ] Actor 响应式架构
  - [ ] Saga 流程管理
- Q端 + Real DB 适配器
  - [x] EF Core
  - [ ] Dapper
  - [ ] TiDB
  - [ ] 查询基类
- 应用服务层
  - 接口即服务
	- [x] Http / MVC
	- [x] TCP  / RPC 
  - [ ] GraphQL
- UI层
  - [ ] 权限管理
  - [ ] OAuth 2.0
  - [ ] ASP.NET Core 扩展
  - [ ] Angular
  - [ ] Vue (Vuetify)
- 定时任务
  - [ ] Hangfire
- 基础设施
  - Aop 拦截器
	- [ ] [AspectCore](https://github.com/dotnetcore/AspectCore-Framework)
  - 跟踪监控
    - [ ] [Butterfly](https://github.com/ButterflyAPM)
  - 日志
	- [ ] NLog
	- [ ] Log4net
	- [ ] Exceptionless
  - [ ] 序列化器
  - [ ] 服务定位器
  - [ ] 加密
  - [ ] 爬虫
- 示例 （Samples）
  - [x] 内存事件仓储
  - [x] EFCore事件仓储
  - [x] NoSQL事件仓储
  - [x] InfluxDB事件仓储
  - [x] Redis事件仓储 
  - [x] RabbitMQ总线
  - [x] WebApi代理
  - [ ] CQRS 整体示例
