using Shriek.Exceptions;
using System.Security.Cryptography;
using Shriek.Notifications;
using System.ComponentModel.Design;
using Shriek.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using Shriek.DependencyInjection;
using Shriek.Events;

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

        public CommandBus(IServiceProvider Container, ICommandContext commandContext)
        {
            this.Container = Container;
            this.commandContext = commandContext;
            this.eventBus = Container.GetService<IEventBus>();
        }

        /// <summary>
        /// 发送命令
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="command"></param>
        public void Send<TCommand>(TCommand command) where TCommand : Command
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
                catch (Exception ex)
                {
                    if (ex is DomainException)
                        eventBus.Publish(new DomainNotification(command.GetType().Name, ex.Message));
                    else
                        throw;
                }
            }
            else
            {
                throw new Exceptions.DomainException($"找不到命令{nameof(command)}的处理类，或者IOC未注册。");
            }
        }
    }
}