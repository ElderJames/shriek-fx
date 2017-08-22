using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.Events
{
    public class StoredEvent : Event
    {
        public StoredEvent(Event @event, string data, string user)
        {
            AggregateId = @event.AggregateId;
            EventType = @event.GetType().AssemblyQualifiedName;
            Data = data;
            User = user;
            Version = @event.Version;
        }

        // EF Constructor
        protected StoredEvent() { }

        [Key]
        public int Id { get; set; }

        public string Data { get; private set; }

        public string User { get; private set; }
    }
}