using System;
using System.Collections.Generic;
using System.Linq;
using Shriek.EventSourcing;
using Shriek.Storage.Mementos;

namespace Shriek.Storage
{
    public class InMemoryEventStorageRepository : IMementoRepository, IEventStorageRepository
    {
        private readonly List<Memento> mementoes;
        private readonly List<StoredEvent> events;

        public InMemoryEventStorageRepository()
        {
            mementoes = new List<Memento>();
            events = new List<StoredEvent>();
        }

        public void Dispose()
        {
        }

        public IEnumerable<StoredEvent> GetEvents<TKey>(TKey eventId, int afterVersion = 0) where TKey : IEquatable<TKey>
        {
            var list = this.events.Where(x => x.EventId.Equals(eventId));

            var events = list.Where(e => e.EventId.Equals(eventId) && e.Version >= afterVersion);

            return events;
        }

        public StoredEvent GetLastEvent<TKey>(TKey eventId) where TKey : IEquatable<TKey>
        {
            return events.Where(e => e.EventId.Equals(eventId))
                .OrderBy(e => e.Version).LastOrDefault();
        }

        public Memento GetMemento<TKey>(TKey aggregateId) where TKey : IEquatable<TKey>
        {
            return this.mementoes.Where(x => x.AggregateId == aggregateId.ToString())
                .OrderBy(x => x.Timestamp)
                .LastOrDefault();
        }

        public void SaveMemento(Memento memento)
        {
            mementoes.Add(memento);
        }

        public void Store(StoredEvent theEvent)
        {
            events.Add(theEvent);
        }
    }
}