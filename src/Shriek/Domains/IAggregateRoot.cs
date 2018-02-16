using Shriek.Storage.Mementos;
using System;

namespace Shriek.Domains
{
    public interface IAggregateRoot<out TKey> : IAggregateRoot
        where TKey : IEquatable<TKey>
    {
        TKey AggregateId { get; }
    }

    public interface IAggregateRoot:IOriginator, IEventProvider
    {
        bool CanCommit { get; }


        int Version { get; }
    }
}