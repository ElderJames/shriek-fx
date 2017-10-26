using System;
using Shriek.Messages;

namespace Shriek.Events
{
    public class Event<TKey> : Event, IEvent<TKey> where TKey : IEquatable<TKey>
    {
        public Event()
        {
            this.Timestamp = DateTime.Now;
        }

        public virtual TKey AggregateId { get; set; }
    }

    public class Event : Message, IEvent
    {
        public int Version { get; set; }
    }
}