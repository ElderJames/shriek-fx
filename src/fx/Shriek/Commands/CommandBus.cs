using System.Threading.Tasks;
using System.Threading;
using Shriek.Exceptions;
using Shriek.Notifications;
using System;
using Shriek.DependencyInjection;
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

        private ConcurrentQueue<Command> commandQueue;
        private Task queueTask;

        public CommandBus(IServiceProvider Container, ICommandContext commandContext, IEventBus eventBus)
        {
            this.Container = Container;
            this.commandContext = commandContext;
            this.eventBus = eventBus;

            Subscriber();
        }

        /// <summary>
        /// 发送命令
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="command"></param>
        public void Send<TCommand>(TCommand command) where TCommand : Command
        {
            if (command == null) return;
            commandQueue.Enqueue(command);
            //Handle(command);
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
                    eventBus.Publish(new DomainNotification(command.GetType().Name, ex.Message));
                }
            }
            else
            {
                throw new Exception($"找不到命令{nameof(command)}的处理类，或者IOC未注册。");
            }
        }

        public void Subscriber()
        {
            commandQueue = new ConcurrentQueue<Command>();
            queueTask = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);

                    while (commandQueue.Any())
                    {
                        try
                        {
                            if (commandQueue.TryPeek(out Command command))
                            {
                                Handle((dynamic)command);
                                commandQueue.TryDequeue(out command);
                            }
                        }
                        catch
                        {
                            break;
                        }
                    }
                }
            });
        }
    }
}