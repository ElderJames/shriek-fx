using Shriek.Messages;

namespace Shriek.Commands
{
    /// <summary>
    /// 命令总线
    /// </summary>
    public class CommandBus : ICommandBus
    {
        private readonly IMessagePublisher messageProcessor;

        public CommandBus(IMessagePublisher messageProcessor)
        {
            this.messageProcessor = messageProcessor;
        }

        /// <summary>
        /// 发送命令
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="command"></param>
        public void Send<TCommand>(TCommand command) where TCommand : Command
        {
            if (command == null)
                return;

            messageProcessor.Send(command);
        }
    }
}