using Shriek.Messages;
using System;

namespace Shriek.Events
{
    public class Event : Message, IEvent<Guid>
    {
        public Event()
        {
            this.EventType = GetType().FullName;
            this.Timestamp = DateTime.Now;
        }

        public Guid AggregateId { get; set; }

        public int Version { get; set; }

        public DateTime Timestamp { get; private set; }

        public string EventType { get; protected set; }
    }
}