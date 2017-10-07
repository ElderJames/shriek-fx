using Shriek.EventSourcing;
using Shriek.Storage;
using Shriek.Storage.Mementos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shriek.EventStorage.EFCore
{
    public class EventStorageSQLRepository : IEventStorageRepository, IMementoRepository
    {
        private EventStorageSQLContext context;

        public EventStorageSQLRepository(EventStorageSQLContext context)
        {
            this.context = context;
        }

        public IEnumerable<StoredEvent> GetEvents(Guid aggregateId, int afterVersion = 0)
        {
            return context.Set<StoredEvent>().Where(e => e.AggregateId == aggregateId && e.Version >= afterVersion);
        }

        public void Dispose()
        {
            context.Dispose();
        }

        public StoredEvent GetLastEvent(Guid aggregateId)
        {
            return context.Set<StoredEvent>().Where(e => e.AggregateId == aggregateId).OrderBy(e => e.Timestamp).LastOrDefault();
        }

        public void Store(StoredEvent theEvent)
        {
            context.Set<StoredEvent>().Add(theEvent);
            context.SaveChanges();
        }

        public Memento GetMemento(Guid aggregateId)
        {
            return context.Set<Memento>().Where(m => m.AggregateId == aggregateId).OrderBy(m => m.Version).LastOrDefault();
        }

        public void SaveMemento(Memento memento)
        {
            context.Set<Memento>().Add(memento);
            context.SaveChanges();
        }
    }
}