using Shriek.Storage.Mementos;
using Shriek.Events;
using Shriek.Utils;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Shriek.Domains
{
    public abstract class AggregateRoot : IEventProvider, IAggregateRoot, IOriginator
    {
        private readonly List<Event> _changes;

        public Guid AggregateId { get; protected set; }

        public int Version { get; protected set; } = -1;
        public int EventVersion { get; protected set; }

        public AggregateRoot(Guid aggregateId)
        {
            _changes = new List<Event>();
            AggregateId = aggregateId;
        }

        public override bool Equals(object obj)
        {
            var compareTo = obj as AggregateRoot;

            if (ReferenceEquals(this, compareTo)) return true;
            if (ReferenceEquals(null, compareTo)) return false;

            return AggregateId.Equals(compareTo.AggregateId);
        }

        public static bool operator ==(AggregateRoot a, AggregateRoot b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(AggregateRoot a, AggregateRoot b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (GetType().GetHashCode() * 907) + AggregateId.GetHashCode();
        }

        public override string ToString()
        {
            return GetType().Name + " [Id=" + AggregateId.ToString() + "]";
        }

        public void MarkChangesAsCommitted()
        {
            _changes.Clear();
        }

        public void LoadsFromHistory(IEnumerable<Event> history)
        {
            foreach (var e in history)
            {
                ApplyChange(e, false);
            }
            Version = history.Last().Version;
            EventVersion = Version;
        }

        protected void ApplyChange(Event @event)
        {
            ApplyChange(@event, true);
        }

        protected void ApplyChange(Event @event, bool isNew)
        {
            dynamic d = this;
            d.Handle((dynamic)@event);
            if (isNew)
            {
                _changes.Add(@event);
            }
        }

        public IEnumerable<Event> GetUncommittedChanges()
        {
            return _changes;
        }

        public Memento GetMemento()
        {
            var dict = this.ToMap();
            return new Memento() { Id = AggregateId, Mapper = dict, Version = 0 };
        }

        public void SetMemento(Memento memento)
        {
            memento.Mapper.ToObject(this.GetType());
        }

        public bool CanCommit => _changes.Any();
    }
}