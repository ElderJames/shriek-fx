using Shriek.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.Commands
{
    /// <summary>
    /// 命令总线
    /// </summary>
    public class CommandBus : ICommandBus
    {
        //public static Func<IServiceProvider> ContainerAccessor { get; set; }
        //private static IServiceProvider Container => ContainerAccessor();
        private IServiceProvider Container;

        private ICommandContext commandContext;

        public CommandBus(IServiceProvider Container, ICommandContext commandContext)
        {
            this.Container = Container;
            this.commandContext = commandContext;
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
                ((ICommandHandler<TCommand>)handler).Execute(commandContext, command);
            }
            else
            {
                throw new Exceptions.DomainException($"找不到命令{nameof(command)}的处理类，或者IOC未注册。");
            }
        }
    }
}