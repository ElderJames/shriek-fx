using Shriek.Messages;
using System;

namespace Shriek.Events
{
    public class Event<TKey> : Event, IEvent<TKey> where TKey : IEquatable<TKey>
    {
        public Event()
        {
            this.Timestamp = DateTime.Now;
        }

        public virtual TKey AggregateId { get; set; }

        public override string EventId => AggregateId.ToString();
    }

    public abstract class Event : Message, IEvent
    {
        public abstract string EventId { get; }
        public int Version { get; internal set; }
    }
}