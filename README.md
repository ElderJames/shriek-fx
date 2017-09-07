# ShriekFx:zap: ![](https://img.shields.io/badge/.NET%20Core-2.0.0-brightgreen.svg?style=flat-square) [![Build status](https://ci.appveyor.com/api/projects/status/mcwi2kqe0daija6c?svg=true)](https://ci.appveyor.com/project/ElderJames/shriekfx) [![Author](https://img.shields.io/badge/author-ElderJames-brightgreen.svg?style=flat-square)](https://yangshunjie.com) [![GitHub license](https://img.shields.io/badge/license-MIT-brightgreen.svg?style=flat-square)](https://github.com/ElderJames/ShriekFx/blob/master/LICENSE)  

A ddd-cqrs framework for **.NET Core 2.0**  that would make you shriek! For it's simple,elegant and useful!

一个使用 **.NET Core 2.0** 开发的简单易用的领域驱动框架，宗旨是让小型应用也能用DDD的思想去开发，打破DDD很复杂的谣言！

### 特性：

1. 领域驱动（DDD）
2. 命令查询职责分离（CQRS）
3. 事件回溯 （ES）
4. 最终一致性 （Eventually Consistent）

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
    - [x] 服务端 
  - [ ] RPC
  - [ ] GraphSQL
- 定时任务
  - [ ] Hangfire   
- 示例
  - [x] 内存事件仓储示例
  - [x] EFCore事件仓储示例
  - [x] NoSQL事件仓储示例
  - [ ] Web示例
