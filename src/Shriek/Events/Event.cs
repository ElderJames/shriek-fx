using Shriek.Messages;
using System;

namespace Shriek.Events
{
    public class Event : Message, IEvent<Guid>
    {
        public Event()
        {
            this.Timestamp = DateTime.Now;
        }

        public Guid AggregateId { get; set; }

        public int Version { get; set; }

        public DateTime Timestamp { get; set; }
    }
}