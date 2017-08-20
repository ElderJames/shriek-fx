using System;
using System.Collections.Generic;
using System.Text;
using Shriek.Domains;

namespace Shriek.Commands
{
    public interface ICommandContext
    {
        IDictionary<string, object> Items { get; }

        TAggregateRoot GetAggregateRoot<TAggregateRoot>(Guid key, Func<TAggregateRoot> initFromRepository) where TAggregateRoot : AggregateRoot, new();

        TAggregateRoot GetAggregateRoot<TAggregateRoot>(Guid key) where TAggregateRoot : AggregateRoot, new();
    }
}