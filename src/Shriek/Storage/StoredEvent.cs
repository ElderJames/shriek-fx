using Shriek.Events;
using System.ComponentModel.DataAnnotations;

namespace Shriek.Storage
{
    public class StoredEvent : Event
    {
        public StoredEvent(Event @event, string data, string user)
        {
            AggregateId = @event.AggregateId;
            Data = data;
            User = user;
            Version = @event.Version;
        }

        public StoredEvent()
        {
        }

        [Key]
        public int Id { get; protected set; }

        public string Data { get; set; }

        public string User { get; set; }
    }
}