using Shriek.Events;
using System;

namespace Shriek.Notifications
{
    public class DomainNotification : Event
    {
        public override string EventId => DomainNotificationId.ToString();
        public Guid DomainNotificationId { get; }
        public string Key { get; }
        public string Value { get; }

        public DomainNotification(string key, string value)
        {
            DomainNotificationId = Guid.NewGuid();
            Version = 1;
            Key = key;
            Value = value;
        }
    }
}