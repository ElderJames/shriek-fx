using System;
using Shriek.Events;
using System.ComponentModel.DataAnnotations;

namespace Shriek.Storage
{
    public class StoredEvent<TKey> : Event<TKey> where TKey : IEquatable<TKey>
    {
        public StoredEvent(Event<TKey> @event, string data, string user)
        {
            AggregateId = @event.AggregateId;
            Data = data;
            User = user;
            Version = @event.Version;
        }

        // EF Constructor
        public StoredEvent() { }

        [Key]
        public int Id { get; protected set; }

        public string Data { get; set; }

        public string User { get; set; }
    }
}