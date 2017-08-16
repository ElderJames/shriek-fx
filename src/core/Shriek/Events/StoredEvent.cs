using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.Events
{
    public class StoredEvent : Event
    {
        public StoredEvent(Event theEvent, string data, string user)
        {
            AggregateId = theEvent.AggregateId;
            Data = data;
            User = user;
        }

        // EF Constructor
        protected StoredEvent() { }

        public string Data { get; private set; }

        public string User { get; private set; }
    }
}