# ShriekFx [![Build status](https://ci.appveyor.com/api/projects/status/mcwi2kqe0daija6c?svg=true)](https://ci.appveyor.com/project/ElderJames/shriekfx) [![Author](https://img.shields.io/badge/author-ElderJames-brightgreen.svg?style=flat-square)](https://yangshunjie.com) [![GitHub license](https://img.shields.io/badge/license-MIT-brightgreen.svg?style=flat-square)](https://github.com/ElderJames/ShriekFx/blob/master/LICENSE)  

A ddd-cqrs framework that would make you shriek! simple,elegant and useful!

一个简单易用的领域驱动框架，宗旨是让小型应用也能用DDD的思想去开发，打破DDD很复杂的谣言！

特性：

1. 领域驱动（DDD）
2. 命令查询职责分离（CQRS）
3. 事件溯源 （ES）
4. 最终一致性

进度：

- [x] C端
  - [x] CommandBus
  - [x] EventBus
  - [x] 内存队列解耦命令和事件的处理
  - [x] 内存事件缓存
  - [ ] 事件存储 + 事件快照（备忘录模式）
    - [x] 内存模式 *(需要在CommandHandler中立刻存储到Real DB)*
    - [x] EF实现
    - [ ] NoSql实现
    - [ ] Redis实现
  - [ ] 消息队列（MQ）
  - [ ] Q端 + Real DB 
    - [ ] EF
    - [ ] Dapper
- [ ] Service层
- [ ] 示例
  - [x] 内存事件仓储示例
  - [x] SQl事件仓储示例
  - [ ] Web示例
