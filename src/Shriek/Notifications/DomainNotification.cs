using Shriek.Events;
using System;

namespace Shriek.Notifications
{
    public class DomainNotification<TKey> : IEvent<TKey> where TKey : IEquatable<TKey>
    {
        public Guid DomainNotificationId { get; private set; }
        public string Key { get; private set; }
        public string Value { get; private set; }

        public int Version { get; set; }

        public TKey AggregateId { get; set; }

        public DomainNotification(string key, string value)
        {
            DomainNotificationId = Guid.NewGuid();
            Version = 1;
            Key = key;
            Value = value;
        }
    }
}