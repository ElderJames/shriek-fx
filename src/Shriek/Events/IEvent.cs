using System;

namespace Shriek.Events
{
    public interface IEvent<TKey> : IEvent where TKey : IEquatable<TKey>

    {
        TKey AggregateId { get; }

        int Version { get; set; }
    }

    public interface IEvent
    {
    }
}