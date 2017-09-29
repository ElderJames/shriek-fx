using System;

namespace Shriek.Domains
{
    public interface IAggregateRoot<out TKey> where TKey : IEquatable<TKey>
    {
        TKey AggregateId { get; }

        int Version { get; }

        bool CanCommit { get; }
    }
}