using Shriek.Messages;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Threading;
using Shriek.Exceptions;
using Shriek.Notifications;
using System;
using Shriek.Events;
using System.Collections.Concurrent;
using System.Linq;

namespace Shriek.Commands
{
    /// <summary>
    /// 命令总线
    /// </summary>
    public class CommandBus : ICommandBus
    {
        private IServiceProvider Container;

        private ICommandContext commandContext;

        private IEventBus eventBus;

        private IMessagePublisher messageProcessor;

        private IDomainNotificationHandler<DomainNotification> notification;

        public CommandBus(IServiceProvider Container, ICommandContext commandContext, IEventBus eventBus, IMessagePublisher messageProcessor, IDomainNotificationHandler<DomainNotification> notification)
        {
            this.Container = Container;
            this.commandContext = commandContext;
            this.eventBus = eventBus;
            this.messageProcessor = messageProcessor;
            messageProcessor.Subscriber(h => Handle((dynamic)h));
            this.notification = notification;
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

        private void Handle<TCommand>(TCommand command) where TCommand : Command
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