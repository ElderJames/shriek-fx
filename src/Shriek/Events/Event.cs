using System;
using System.Collections.Generic;
using System.Text;
using Shriek.Messages;

namespace Shriek.Events
{
    public class Event : Message, IEvent<Guid>
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