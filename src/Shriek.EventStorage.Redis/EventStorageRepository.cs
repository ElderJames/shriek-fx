using System;
using System.Collections.Generic;
using System.Linq;
using Shriek.Events;
using Shriek.EventSourcing;
using Shriek.Storage;
using Shriek.Storage.Mementos;

namespace Shriek.EventStorage.Redis
{
    public class EventStorageRepository : IEventStorageRepository, IMementoRepository
    {
        private readonly ICacheService cacheService;

        public EventStorageRepository(ICacheService cacheService)
        {
            this.cacheService = cacheService;
        }

        public IEnumerable<StoredEvent> GetEvents(Guid aggregateId, int afterVersion = 0)
        {
            return cacheService.Get<IEnumerable<StoredEvent>>(aggregateId.ToString()).Where(x => x.Version >= afterVersion);
        }

        public void Dispose()
        {
        }

        public Event GetLastEvent(Guid aggregateId)
        {
            return cacheService.Get<IEnumerable<StoredEvent>>(aggregateId.ToString()).OrderByDescending(e => e.Timestamp).FirstOrDefault();
        }

        public Memento GetMemento(Guid aggregateId)
        {
            return cacheService.Get<IEnumerable<Memento>>(aggregateId.ToString()).OrderByDescending(e => e.Version).FirstOrDefault();
        }

        public void SaveMemento(Memento memento)
        {
            var mementos = cacheService.Get<IEnumerable<Memento>>(memento.AggregateId.ToString());

            cacheService.Store(memento.AggregateId.ToString(), mementos.Concat(new[] { memento }));
        }

        public void Store(StoredEvent theEvent)
        {
            var events = cacheService.Get<IEnumerable<StoredEvent>>(theEvent.AggregateId.ToString());

            cacheService.Store(theEvent.AggregateId.ToString(), events.Concat(new[] { theEvent }));
        }
    }
}