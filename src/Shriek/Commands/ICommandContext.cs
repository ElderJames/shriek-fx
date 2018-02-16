using Shriek.Domains;
using System;
using System.Collections.Generic;

namespace Shriek.Commands
{
    public interface ICommandContext
    {
        IDictionary<string, object> Items { get; }

        TAggregateRoot GetAggregateRoot<TKey, TAggregateRoot>(TKey key, Func<TAggregateRoot> initFromRepository)
            where TAggregateRoot : IAggregateRoot<TKey>, new()
            where TKey : IEquatable<TKey>;

        TAggregateRoot GetAggregateRoot<TKey, TAggregateRoot>(TKey key)
            where TAggregateRoot : IAggregateRoot<TKey>, new()
            where TKey : IEquatable<TKey>;
    }
}