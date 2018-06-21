using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shriek.Events;
using Shriek.Storage.Mementos;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Shriek.Domains
{
    public abstract class AggregateRoot<TKey> : IAggregateRoot<TKey>
        where TKey : IEquatable<TKey>
    {
        [Key]
        public int Id { get; private set; }

        public int Version { get; private set; } = -1;

        private ConcurrentBag<Event> _changes;

        private ConcurrentBag<Event> Changes => _changes ?? (_changes = new ConcurrentBag<Event>());

        public bool CanCommit => this.Changes.Any();

        public TKey AggregateId { get; protected set; }

        private AggregateRoot()
        {
        }

        protected AggregateRoot(TKey aggregateId)
        {
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
            return GetType().Name + " [Id=" + AggregateId + "]";
        }

        public void LoadsFromHistory(IEnumerable<Event> history)
        {
            foreach (var e in history)
            {
                ApplyChange(e, false);
            }
            Version = history.LastOrDefault()?.Version ?? -1;
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
                this.Changes.Add(@event);
            }
        }

        public Memento GetMemento()
        {
            return new Memento() { AggregateId = AggregateId.ToString(), Data = JsonConvert.SerializeObject(this), Version = 0 };
        }

        public IEnumerable<Event> GetUncommittedChanges()
        {
            return Changes;
        }

        public void MarkChangesAsCommitted()
        {
            while (!Changes.IsEmpty)
            {
                Changes.TryTake(out var _);
            }
        }

        public void SetMemento(Memento memento)
        {
            var data = JObject.Parse(memento.Data);
            foreach (var t in data)
            {
                var prop = GetType().GetProperty(t.Key);
                if (prop == null || !prop.CanWrite)
                    continue;

                var value = t.Value.ToObject(prop.PropertyType);
                prop.SetValue(this, value);
            }
        }
    }
}