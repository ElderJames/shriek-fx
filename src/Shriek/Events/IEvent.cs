using System;

namespace Shriek.Events
{
    public interface IEvent<out TKey> : IEvent where TKey : IEquatable<TKey>

    {
        TKey AggregateId { get; }
    }

    public interface IEvent
    {
        int Version { get; }

        string Creator { get; }
    }
}