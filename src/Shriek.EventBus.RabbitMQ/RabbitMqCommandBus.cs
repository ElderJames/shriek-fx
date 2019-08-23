using Shriek.Commands;
using System;
using System.Runtime.CompilerServices;

namespace Shriek.Messages.RabbitMQ
{
    public class RabbitMqCommandBus : ICommandBus, IDisposable
    {
        private readonly IMessagePublisher messagePublisher;

        public RabbitMqCommandBus(IMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        public void Dispose()
        {
            messagePublisher.Dispose();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Send<TCommand>(TCommand command) where TCommand : Command
        {
            if (command == null)
                return;

            messagePublisher.Send(command);
        }
    }
}