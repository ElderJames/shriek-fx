using System;
using Shriek.Events;
using System.ComponentModel.DataAnnotations;

namespace Shriek.Storage
{
    public sealed class StoredEvent<TKey> : StoredEvent where TKey : IEquatable<TKey>
    {
        public StoredEvent(IEvent<TKey> @event, string data, string user, int id)
            : base(@event.AggregateId.ToString(), data, @event.Version, user, id)
        {
        }
    }

    public class StoredEvent : Event
    {
        public StoredEvent()
        {
        }

        public StoredEvent(string aggregateId, string data, int version, string user, int id)
        {
            AggregateId = aggregateId;
            Data = data;
            User = user;
            Id = id;
            this.Version = version;
        }

        [Key]
        public int Id { get; protected set; }

        public string AggregateId { get; set; }

        public string Data { get; set; }

        public string User { get; set; }
    }
}