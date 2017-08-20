using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.Events
{
    public class Event : IEvent<Guid>
    {
        public Event()
        {
            this.EventType = GetType().Name;
            this.Timestamp = DateTime.Now;
        }

        public Guid AggregateId { get; set; }

        public int Version { get; set; }

        public DateTime Timestamp { get; private set; }

        public string EventType { get; private set; }
    }
}