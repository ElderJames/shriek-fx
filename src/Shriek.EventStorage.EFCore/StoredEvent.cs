using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.Events
{
    public class StoredEvent : Event
    {
        public StoredEvent(Guid aggregateId, string data, string user)
        {
            AggregateId = aggregateId;
            Data = data;
            User = user;
        }

        // EF Constructor
        protected StoredEvent() { }

        public string Data { get; private set; }

        public string User { get; private set; }
    }
}