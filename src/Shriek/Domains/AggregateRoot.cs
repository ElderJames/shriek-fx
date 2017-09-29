using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shriek.Events;
using Shriek.Storage.Mementos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Shriek.Domains
{
    public abstract class AggregateRoot<TKey> : IAggregateRoot<TKey>, IEventProvider<TKey>, IOriginator<TKey> where TKey : IEquatable<TKey>
    {
        private readonly List<IEvent<TKey>> _changes;

        [Key]
        public int Id { get; protected set; }

        public TKey AggregateId { get; protected set; }

        public int Version { get; protected set; } = -1;
        public int EventVersion { get; protected set; }

        protected AggregateRoot() : this(default(TKey))
        {
        }

        protected AggregateRoot(TKey aggregateId)
        {
            _changes = new List<IEvent<TKey>>();
            AggregateId = aggregateId;
        }

        public override bool Equals(object obj)
        {
            var compareTo = obj as AggregateRoot<TKey>;

            if (ReferenceEquals(this, compareTo)) return true;
            if (ReferenceEquals(null, compareTo)) return false;

            return AggregateId.Equals(compareTo.AggregateId);
        }

        public static bool operator ==(AggregateRoot<TKey> a, AggregateRoot<TKey> b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(AggregateRoot<TKey> a, AggregateRoot<TKey> b)
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

        public void LoadsFromHistory(IEnumerable<IEvent<TKey>> history)
        {
            foreach (var e in history)
            {
                ApplyChange(e, false);
            }
            Version = history.LastOrDefault()?.Version ?? -1;
            EventVersion = Version;
        }

        protected void ApplyChange(IEvent<TKey> @event)
        {
            ApplyChange(@event, true);
        }

        protected void ApplyChange(IEvent<TKey> @event, bool isNew)
        {
            dynamic d = this;
            d.Handle((dynamic)@event);
            if (isNew)
            {
                _changes.Add(@event);
            }
        }

        public IEnumerable<IEvent<TKey>> GetUncommittedChanges()
        {
            return _changes;
        }

        public Memento<TKey> GetMemento()
        {
            return new Memento<TKey>() { AggregateId = AggregateId, Data = JsonConvert.SerializeObject(this), Version = 0 };
        }

        public void SetMemento(Memento<TKey> memento)
        {
            var data = JObject.Parse(memento.Data);
            foreach (var t in data)
            {
                var prop = GetType().GetProperty(t.Key);
                if (prop != null && prop.CanWrite)
                {
                    var value = t.Value.ToObject(prop.PropertyType);
                    prop.SetValue(this, value);
                }
            }
        }

        public bool CanCommit => _changes.Any();
    }
}