using System;
using System.ComponentModel.DataAnnotations;
using Shriek.Events;

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

        public StoredEvent(string aggregateId, string data, int version, string user)
        {
            AggregateId = aggregateId;
            Data = data;
            User = user;
            this.Version = version;
        }

        [Key]
        public int Id { get; protected set; }

        public string AggregateId { get; set; }

        public string Data { get; set; }

        public string User { get; set; }
    }
}