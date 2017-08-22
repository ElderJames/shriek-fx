using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.Commands
{
    /// <summary>
    /// 聚合根命令
    /// </summary>
    /// <typeparam name="TAggregateRootId">聚合根标识</typeparam>
    public interface IAggregateCommand<TAggregateRootId> : ICommand
    {
        /// <summary>
        /// 版本号
        /// </summary>
        int Version { get; }

        /// <summary>
        /// 聚合根标认识
        /// </summary>
        TAggregateRootId AggregateId { get; }
    }
}