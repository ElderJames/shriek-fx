using System;

namespace Shriek.Commands
{
    /// <summary>
    /// 聚合根命令
    /// </summary>
    /// <typeparam name="TAggregateId">聚合根标识</typeparam>
    public interface IAggregateCommand<out TAggregateId> : ICommand
        where TAggregateId : IEquatable<TAggregateId>
    {
        /// <summary>
        /// 聚合根标认识
        /// </summary>
        TAggregateId AggregateId { get; }
    }
}