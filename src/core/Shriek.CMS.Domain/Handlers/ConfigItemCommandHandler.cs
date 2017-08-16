using Shriek.ConfigCenter.Domain.Repositories;
using Shriek.Storage;
using Shriek.ConfigCenter.Domain.Aggregates;
using Shriek.ConfigCenter.Domain.Commands;
using Shriek.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.ConfigCenter.Domain.Handlers
{
    public class ConfigItemCommandHandler : ICommandHandler<CreateConfigItemCommand>
    {
        //private IRepository<ConfigItemAggregateRoot> repository;

        public ConfigItemCommandHandler()
        {
            //this.repository = repository;
        }

        public void Execute(ICommandContext context, CreateConfigItemCommand command)
        {
            var root = context.GetAggregateRoot(command.AggregateId, () => ConfigItemAggregateRoot.Register(command));

            // repository.Save(root, root.Version);
        }
    }
}