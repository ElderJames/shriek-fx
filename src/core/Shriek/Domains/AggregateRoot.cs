using Shriek.Events;
using Shriek.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Shriek.Domains
{
    public abstract class AggregateRoot<TAggregateKey> : IEventProvider<TAggregateKey>, IAggregateRoot
    {
        private readonly List<Event<TAggregateKey>> _changes;

        public TAggregateKey AggregateId { get; protected set; }

        public int Version { get; protected set; }
        public int EventVersion { get; protected set; }

        public AggregateRoot(TAggregateKey aggregateId)
        {
            _changes = new List<Event<TAggregateKey>>();
            AggregateId = aggregateId;
        }

        public override bool Equals(object obj)
        {
            var compareTo = obj as AggregateRoot<TAggregateKey>;

            if (ReferenceEquals(this, compareTo)) return true;
            if (ReferenceEquals(null, compareTo)) return false;

            return AggregateId.Equals(compareTo.AggregateId);
        }

        public static bool operator ==(AggregateRoot<TAggregateKey> a, AggregateRoot<TAggregateKey> b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(AggregateRoot<TAggregateKey> a, AggregateRoot<TAggregateKey> b)
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

        public void LoadsFromHistory(IEnumerable<Event<TAggregateKey>> history)
        {
            foreach (var e in history)
            {
                ApplyChange(e, false);
            }
            Version = history.Last().Version;
            EventVersion = Version;
        }

        protected void ApplyChange(Event<TAggregateKey> @event)
        {
            ApplyChange(@event, true);
        }

        protected void ApplyChange(Event<TAggregateKey> @event, bool isNew)
        {
            dynamic d = this;
            d.Handle((dynamic)@event);
            if (isNew)
            {
                _changes.Add(@event);
            }
        }

        public IEnumerable<Event<TAggregateKey>> GetUncommittedChanges()
        {
            return _changes;
        }

        //public abstract Memento GetMemento();

        //public abstract void SetMemento(Memento memento);

        public bool CanCommit => _changes.Any();
    }
}