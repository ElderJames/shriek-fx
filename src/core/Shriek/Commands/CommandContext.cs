using Shriek.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.Commands
{
    public class CommandContext : ICommandContext
    {
        public IDictionary<string, object> Items => new Dictionary<string, object>();

        TAggregateRoot ICommandContext.GetAggregateRoot<TAggregateRootKey, TAggregateRoot>(TAggregateRootKey key)
        {
            throw new NotImplementedException();
        }

        TAggregateRoot ICommandContext.GetAggregateRoot<TAggregateRootKey, TAggregateRoot>(IAggregateCommand<TAggregateRootKey> command)
        {
            throw new NotImplementedException();
        }

        TAggregateRoot ICommandContext.GetAggregateRoot<TAggregateRootKey, TAggregateRoot>(TAggregateRootKey key, Func<TAggregateRoot> initFromRepository)
        {
            throw new NotImplementedException();
        }

        TAggregateRoot ICommandContext.GetAggregateRoot<TAggregateRootKey, TAggregateRoot>(IAggregateCommand<TAggregateRootKey> command, Func<TAggregateRoot> initFromRepository)
        {
            throw new NotImplementedException();
        }
    }
}