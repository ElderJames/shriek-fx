using Shriek.Events;
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

        public IEnumerable<StoredEvent> All(Guid aggregateId)
        {
            return context.Set<StoredEvent>().Where(e => e.AggregateId == aggregateId);
        }

        public void Dispose()
        {
            context.Dispose();
        }

        public Event GetLastEvent(Guid aggregateId)
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
            return context.Set<Memento>().Where(m => m.aggregateId == aggregateId).OrderBy(m => m.Version).LastOrDefault();
        }

        public void SaveMemento(Memento memento)
        {
            context.Set<Memento>().Add(memento);
            context.SaveChanges();
        }
    }
}