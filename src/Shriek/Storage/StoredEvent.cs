using Shriek.Events;
using System;
using System.ComponentModel.DataAnnotations;

namespace Shriek.Storage
{
    public sealed class StoredEvent<TKey> : StoredEvent where TKey : IEquatable<TKey>
    {
        public StoredEvent(IEvent<TKey> @event, string data, string messageType, DateTime timestampe, int version, string user)
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
            this.AggregateId = aggregateId;
            this.Data = data;
            this.Version = version;
            this.User = user;
        }

        [Key]
        public int Id { get; protected set; }

        public string AggregateId { get; set; }

        public string Data { get; set; }

        public string User { get; set; }
    }
}