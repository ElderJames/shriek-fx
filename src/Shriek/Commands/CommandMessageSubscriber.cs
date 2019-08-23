using Newtonsoft.Json;
using Shriek.Events;
using Shriek.Exceptions;
using Shriek.Messages;
using Shriek.Notifications;
using System;

namespace Shriek.Commands
{
    public class CommandMessageSubscriber<TCommand> : IMessageSubscriber<TCommand> where TCommand : Command
    {
        private readonly IServiceProvider container;
        private readonly ICommandContext commandContext;
        private readonly IEventBus eventBus;

        public CommandMessageSubscriber(IServiceProvider container, ICommandContext context, IEventBus eventBus)
        {
            this.container = container;
            this.commandContext = context;
            this.eventBus = eventBus;
        }

        public void Execute(TCommand command)
        {
            if (container == null) return;

            var handler = container.GetService(typeof(ICommandHandler<TCommand>));

            if (handler != null)
            {
                try
                {
                    ((ICommandHandler<TCommand>)handler).Execute(commandContext, command);
                    ((ICommandContextSave)commandContext).Save();
                }
                catch (DomainException ex)
                {
                    eventBus.Publish(new DomainNotification(ex.Message, JsonConvert.SerializeObject(command)));
                    throw;
                }
            }
            else
            {
                throw new Exception($"找不到命令{nameof(command)}的处理类，或者IOC未注册。");
            }
        }
    }
}