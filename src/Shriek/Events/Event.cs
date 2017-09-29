using Shriek.Messages;
using System;

namespace Shriek.Events
{
    public class Event<TKey> : Message, IEvent<TKey> where TKey : IEquatable<TKey>
    {
        public Event()
        {
            this.Timestamp = DateTime.Now;
        }

        public TKey AggregateId { get; set; }

        public int Version { get; set; }

        public DateTime Timestamp { get; set; }
    }
}