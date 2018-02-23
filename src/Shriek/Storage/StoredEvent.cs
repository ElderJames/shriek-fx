using Shriek.Events;
using System;
using System.ComponentModel.DataAnnotations;

namespace Shriek.Storage
{
    public sealed class StoredEvent<TKey> : StoredEvent where TKey : IEquatable<TKey>
    {
        public StoredEvent(IEvent<TKey> @event, string data, string user)
            : base(@event.AggregateId.ToString(), data, @event.Version, user)
        {
        }
    }

    public class StoredEvent : Event
    {
        public StoredEvent()
        {
        }

        public StoredEvent(string eventId, string data, int version, string user)
        {
            this.EventId = eventId;
            this.Data = data;
            this.Version = version;
            this.User = user;
        }

        [Key]
        public int Id { get; protected set; }

        public override string EventId { get; }

        public string Data { get; set; }

        public string User { get; set; }
    }
}