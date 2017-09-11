using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Shriek.Commands;
using Shriek.Events;
using Shriek.Exceptions;
using Shriek.Notifications;

namespace Shriek.Messages
{
    public class CommandMessageSubscriber<TCommand> : IMessageSubscriber<TCommand> where TCommand : Command
    {
        private IServiceProvider Container;
        private ICommandContext commandContext;
        private IEventBus eventBus;

        public CommandMessageSubscriber(IServiceProvider container, ICommandContext context, IEventBus eventBus)
        {
            this.Container = container;
            this.commandContext = context;
            this.eventBus = eventBus;
        }

        public void Execute(TCommand command)
        {
            if (Container == null) return;

            var handler = Container.GetService(typeof(ICommandHandler<TCommand>));

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
                }
            }
            else
            {
                throw new Exception($"找不到命令{nameof(command)}的处理类，或者IOC未注册。");
            }
        }
    }
}