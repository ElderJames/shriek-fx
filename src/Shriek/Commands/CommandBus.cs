using System.Threading.Tasks;
using Newtonsoft.Json;
using Shriek.Events;
using Shriek.Exceptions;
using Shriek.Messages;
using Shriek.Notifications;
using System;

namespace Shriek.Commands
{
    /// <summary>
    /// 命令总线
    /// </summary>
    public class CommandBus : ICommandBus
    {
        private IMessagePublisher messageProcessor;

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

            Task.Run(() => messageProcessor.Send(command));
        }
    }
}