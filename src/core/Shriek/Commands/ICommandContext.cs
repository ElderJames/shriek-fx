using System;
using System.Collections.Generic;
using System.Text;
using Shriek.Domains;

namespace Shriek.Commands
{
    public interface ICommandContext
    {
        //
        // 摘要:
        //     上下文集合
        IDictionary<string, object> Items { get; }

        //
        // 摘要:
        //     获取聚合根，仅支持【事件流的内存模式】
        //
        // 参数:
        //   key:
        //     内存模式下的key
        //
        // 类型参数:
        //   TAggregateRoot:
        //     聚合根
        //
        //   TAggregateRootKey:
        //     聚合根标识Id，目前只有Guid,long,string,int四种
        TAggregateRoot GetAggregateRoot<TAggregateRootKey, TAggregateRoot>(TAggregateRootKey key) where TAggregateRoot : class, IAggregateRoot;

        //
        // 摘要:
        //     获取聚合根，仅支持【事件流的内存模式】
        //
        // 参数:
        //   command:
        //     命令Id
        //
        // 类型参数:
        //   TAggregateRoot:
        //     聚合根
        //
        //   TAggregateRootKey:
        //     聚合根标识Id，目前只有Guid,long,string,int四种
        TAggregateRoot GetAggregateRoot<TAggregateRootKey, TAggregateRoot>(IAggregateCommand<TAggregateRootKey> command) where TAggregateRoot : class, IAggregateRoot;

        //
        // 摘要:
        //     获取聚合根，仅支持【仓库模式】
        //
        // 参数:
        //   key:
        //     内存模式下的key
        //
        //   initFromRepository:
        //     如果找不到，则从仓库里面初始化
        //
        // 类型参数:
        //   TAggregateRoot:
        //     聚合根
        //
        //   TAggregateRootKey:
        //     聚合根标识Id，目前只有Guid,long,string,int四种
        TAggregateRoot GetAggregateRoot<TAggregateRootKey, TAggregateRoot>(TAggregateRootKey key, Func<TAggregateRoot> initFromRepository) where TAggregateRoot : class, IAggregateRoot;

        //
        // 摘要:
        //     获取聚合根，仅支持【仓库模式】
        //
        // 参数:
        //   command:
        //     命令Id
        //
        //   initFromRepository:
        //     如果找不到，则从仓库里面初始化
        //
        // 类型参数:
        //   TAggregateRoot:
        //     聚合根
        //
        //   TAggregateRootKey:
        //     聚合根标识Id，目前只有Guid,long,string,int四种
        TAggregateRoot GetAggregateRoot<TAggregateRootKey, TAggregateRoot>(IAggregateCommand<TAggregateRootKey> command, Func<TAggregateRoot> initFromRepository) where TAggregateRoot : class, IAggregateRoot;
    }
}