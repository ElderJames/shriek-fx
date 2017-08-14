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
        public void Execute(CreateConfigItemCommand command)
        {
            var root = ConfigItemAggregateRoot.Register(command);
        }
    }
}