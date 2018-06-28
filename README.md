# ShriekFx:zap: 
[![](https://img.shields.io/badge/.NET%20Core-2.0.0-brightgreen.svg?style=flat-square)](https://www.microsoft.com/net/download/core) 
[![Build status](https://ci.appveyor.com/api/projects/status/mcwi2kqe0daija6c?svg=true)](https://ci.appveyor.com/project/ElderJames/shriekfx)
[![MyGet Pre Release](https://img.shields.io/myget/shriek-fx/vpre/Shriek.svg?style=flat-square&label=myget)](https://www.myget.org/feed/Packages/shriek-fx)
[![Author](https://img.shields.io/badge/author-ElderJames-brightgreen.svg?style=flat-square)](https://yangshunjie.com)
[![GitHub license](https://img.shields.io/badge/license-MIT-brightgreen.svg?style=flat-square)](https://github.com/ElderJames/ShriekFx/blob/master/LICENSE)  

shriek-fx 是一个基于 **.NET Core 2.0** 开发的简单易用的快速开发框架，遵循领域驱动设计规范约束，并结合CQRS架构提供实现事件驱动、事件回溯、响应式等特性的基础设施。内部调用对用户几乎无感知也无需自己实现，开箱即用。目标是协助小型应用使用DDD的思维去开发，最终让开发者告别对领域驱动设计的复杂认识，并且享受到正真意义的面向对象设计模式来带的美感。

除此之外，还包含为了增强核心框架功能和迎合通用业务系统快速开发需求的众多实用的、面向微服务的拓展组件。

PS. 领域驱动设计是一种软件系统设计方法理论，而本框架则提供了规范约束，是能够让这种设计理论真正落地实现的开发工具套件（SDK）。

本框架参考自《领域驱动设计》原著、《实现领域驱动设计》和ENode。

### 特性：

1. 领域驱动设计（DDD）
2. 命令查询职责分离（CQRS）
3. 事件驱动架构 (EDA)
4. 事件回溯 （ES）
5. 最终一致性 （Eventually Consistent）
6. [契约即服务](https://ehttps://shriek-projects.github.io/shriek-fx) (通过定义的接口自动生成客户端和服务端实现，支持Http和Socket)
7. 框架中每个组件都有基础实现，最简单时只需一个核心类库就能跑起来
8. 遵循端口与适配器模式，框架组件适配多种第三方组件实现，可从单体架构到面向服务架构按需扩展

### 设计规范

1. 尽量使用.NET Standard和官方提供的类库，第三方类库设计成组件利用DI来按需组合。


---

### 文档

- [中文](https://shriek-projects.github.io/shriek-fx)

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

[认证授权系统 ShriekFX.Auth](https://github.com/Shriek-Projects/shriek-auth) 开发中

[内容管理系统 ShriekFX.CMS](https://github.com/Shriek-Projects/shriek-cms) 开发中

其他示例在Samples目录下

### 任务列表（会不断调整）：

- C端
  - [x] 命令总线 CommandBus
  - [x] 事件总线 EventBus
  - [x] 进程内异步队列
  - [x] 内存事件缓存
  - [x] 接口实现自动注册
  - [ ] Actor 响应式架构
  - [ ] Saga 流程管理
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
- Q端 + Real DB ORM
  - [x] EF Core
  - Dapper
    - [x] 接口标注Sql特性动态创建仓储
    - [ ] Linq 扩展
    - [ ] 特性指定事务范围
  - [x] [SmartSql](https://github.com/Ahoo-Wang/SmartSql)扩展（实现Xml Sql模板）
  - [ ] TiDB
  - [ ] 查询基类
- 应用服务层
  - 接口即服务
	- [x] Http / MVC
	- [x] TCP  / RPC
	- [ ] Dotnetty
  - [ ] GraphQL
- UI层
  - [ ] 权限管理
  - [ ] OAuth 2.0
  - [ ] ASP.NET Core 扩展
	- [ ] WebApi JS客户端生成
	- [ ] Swagger
  - [ ] 动态表单
  - [ ] Angular
  - [ ] Vue (Vuetify)
- 定时任务
  - [ ] Hangfire
- 基础设施
  - Aop 拦截器
	- [ ] [AspectCore](https://github.com/dotnetcore/AspectCore-Framework)
  - 跟踪监控
	- [ ] [SkyWalking](https://github.com/OpenSkywalking/skywalking-netcore)
    - [ ] [Butterfly](https://github.com/ButterflyAPM)
  - 日志
	- [ ] NLog
	- [ ] Log4net
	- [ ] Exceptionless
	- [ ] Kafka + ELK
  - [ ] 序列化器
  - [ ] 服务定位器
  - [ ] 加密
  - [ ] 爬虫
- 单元测试
  - [ ] CQRS
  - [ ] ServiceProxy
- 示例 （Samples）
  - [x] 内存事件仓储
  - [x] EFCore事件仓储
  - [x] NoSQL事件仓储
  - [x] InfluxDB事件仓储
  - [x] Redis事件仓储
  - [x] RabbitMQ总线
  - [x] WebApi代理
  - [ ] CQRS 整体示例
