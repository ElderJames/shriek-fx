namespace Shriek.Commands
{
    /// <summary>
    /// 命令总线
    /// </summary>
    public interface ICommandBus
    {
        /// <summary>
        /// 发送命令
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="command">命令</param>
        void Send<TCommand>(TCommand command) where TCommand : Command;
    }
}