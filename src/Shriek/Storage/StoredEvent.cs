using Shriek.Events;
using System;
using System.ComponentModel.DataAnnotations;

namespace Shriek.Storage
{
    public class StoredEvent : Event
    {
        public StoredEvent(Event @event, string data)
         : this(@event.EventId, @event.GetType().AssemblyQualifiedName, data, @event.Version, @event.Creator)
        {
        }

        public StoredEvent(string eventId, string eventType, string data, int version, string creator)
        {
            this.EventId = eventId;
            this.Data = data;
            this.Version = version;
            this.Creator = creator;
            this.EventType = eventType;
        }

        [Key]
        public int Id { get; protected set; }

        public override string EventId { get; }

        public string Data { get; set; }

        public string EventType { get; set; }
    }
}