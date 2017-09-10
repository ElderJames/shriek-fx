# ShriekFx:zap: [![](https://img.shields.io/badge/.NET%20Core-2.0.0-brightgreen.svg?style=flat-square)](https://www.microsoft.com/net/download/core) [![Build Status](https://travis-ci.org/ElderJames/shriek-fx.svg?branch=master)](https://travis-ci.org/ElderJames/shriek-fx) [![Build status](https://ci.appveyor.com/api/projects/status/mcwi2kqe0daija6c?svg=true)](https://ci.appveyor.com/project/ElderJames/shriekfx) [![Author](https://img.shields.io/badge/author-ElderJames-brightgreen.svg?style=flat-square)](https://yangshunjie.com) [![GitHub license](https://img.shields.io/badge/license-MIT-brightgreen.svg?style=flat-square)](https://github.com/ElderJames/ShriekFx/blob/master/LICENSE)  

A ddd-cqrs framework for **.NET Core 2.0**  that would make you shriek! For it's simple,elegant and useful!

一个使用 **.NET Core 2.0** 开发的简单易用的领域驱动框架，宗旨是让小型应用也能用DDD的思想去开发，打破DDD很复杂的谣言！

---

### 特性：

1. 领域驱动（DDD）
2. 命令查询职责分离（CQRS）
3. 事件回溯 （ES）
4. 最终一致性 （Eventually Consistent）
5. Server/Client 动态代理 (提供接口自动实现客户端和服务端)

### 开发环境

1. [Visual Studio 15.3](https://www.visualstudio.com/zh-hans/thank-you-downloading-visual-studio/?sku=Community&rel=15)
2. .NET Core 2.0 SDK [ [x64](https://download.microsoft.com/download/0/F/D/0FD852A4-7EA1-4E2A-983A-0484AC19B92C/dotnet-sdk-2.0.0-win-x64.exe) | [x86](https://download.microsoft.com/download/0/F/D/0FD852A4-7EA1-4E2A-983A-0484AC19B92C/dotnet-sdk-2.0.0-win-x86.exe) ]

---

### 任务列表（更新中）：

- C端
  - [x] 命令总线 CommandBus
  - [x] 事件总线 EventBus
  - [x] 进程内异步队列
  - [x] 内存事件缓存
  - 事件存储 + 聚合快照（备忘录模式）
    - [x] 内存模式 *(聚合修改后立刻持久化)*
    - [x] EF Core实现
    - NoSQL实现
		- [x] LiteDB
		- [x] Cosmos DB （MongoDB API）
    - [ ] Redis
  - [ ] 消息队列（MQ）
  - [ ] Saga
- Q端 + Real DB 
  - [x] EF Core
  - [ ] Dapper
  - [ ] 仓储扫描注册
  - [ ] 查询基类
- Service层
  - WebApi 接口动态代理
    - [x] 客户端
    - 服务端 
      - [x] Http / MVC
      - [ ] TCP  / RPC
  - [ ] GraphSQL
- 定时任务
  - [ ] Hangfire   
- 示例
  - [x] 内存事件仓储
  - [x] EFCore事件仓储
  - [x] NoSQL事件仓储
  - [x] WebApi代理
  - [ ] CQRS 整体示例
