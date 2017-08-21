using Shriek.Storage;
using Shriek.Console.Aggregates;
using Shriek.Console.Commands;
using Shriek.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.Console.Handlers
{
    public class ConfigItemCommandHandler : ICommandHandler<CreateConfigItemCommand>,
        ICommandHandler<ChangeConfigItemCommand>
    {
        public ConfigItemCommandHandler()
        {
        }

        public void Execute(ICommandContext context, CreateConfigItemCommand command)
        {
            var root = context.GetAggregateRoot(command.AggregateId, () => ConfigItemAggregateRoot.Register(command));
        }

        public void Execute(ICommandContext context, ChangeConfigItemCommand command)
        {
            var root = context.GetAggregateRoot<ConfigItemAggregateRoot>(command.AggregateId);
            root.Change(command);
        }
    }
}