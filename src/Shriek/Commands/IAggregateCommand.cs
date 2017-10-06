namespace Shriek.Commands
{
    /// <summary>
    /// 聚合根命令
    /// </summary>
    /// <typeparam name="TKey">聚合根标识</typeparam>
    public interface IAggregateCommand<out TKey> : ICommand
    {
        /// <summary>
        /// 版本号
        /// </summary>
        int Version { get; }

        /// <summary>
        /// 聚合根标认识
        /// </summary>
        TKey AggregateId { get; }
    }
}