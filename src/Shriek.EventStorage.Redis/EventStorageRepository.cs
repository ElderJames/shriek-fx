using System;
using System.Collections.Generic;
using System.Linq;
using Shriek.EventSourcing;
using Shriek.Storage;
using Shriek.Storage.Mementos;

namespace Shriek.EventStorage.Redis
{
    public class EventStorageRepository : IEventStorageRepository, IMementoRepository
    {
        private readonly ICacheService cacheService;
        private const string eventStorePrefix = "event_store_";
        private const string mementoStorePrefix = "memento_store_";

        public EventStorageRepository(ICacheService cacheService)
        {
            this.cacheService = cacheService;
        }

        public IEnumerable<StoredEvent> GetEvents<TKey>(TKey aggregateId, int afterVersion = 0)
            where TKey : IEquatable<TKey>
        {
            return cacheService.Get<IEnumerable<StoredEvent>>(eventStorePrefix + aggregateId)?.Where(x => x.Version >= afterVersion) ?? new StoredEvent[0];
        }

        public void Dispose()
        {
        }

        public StoredEvent GetLastEvent<TKey>(TKey aggregateId)
            where TKey : IEquatable<TKey>
        {
            return cacheService.Get<IEnumerable<StoredEvent>>(eventStorePrefix + aggregateId)?.OrderByDescending(e => e.Timestamp).FirstOrDefault();
        }

        public void Store(StoredEvent theEvent)
        {
            var events = cacheService.Get<IEnumerable<StoredEvent>>(eventStorePrefix + theEvent.AggregateId) ?? new StoredEvent[0];

            cacheService.Store(theEvent.AggregateId.ToString(), events.Concat(new[] { theEvent }));
        }

        public Memento GetMemento<TKey>(TKey aggregateId)
            where TKey : IEquatable<TKey>
        {
            return cacheService.Get<IEnumerable<Memento>>(mementoStorePrefix + aggregateId)?.OrderByDescending(e => e.Version).FirstOrDefault();
        }

        public void SaveMemento(Memento memento)
        {
            var mementos = cacheService.Get<IEnumerable<Memento>>(mementoStorePrefix + memento.AggregateId) ?? new Memento[0];

            cacheService.Store(memento.AggregateId.ToString(), mementos.Concat(new[] { memento }));
        }
    }
}