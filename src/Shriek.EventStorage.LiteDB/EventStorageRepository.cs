using Shriek.Events;
using Shriek.EventSourcing;
using Shriek.Storage;
using Shriek.Storage.Mementos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shriek.EventStorage.LiteDB
{
    public class EventStorageRepository : IEventStorageRepository, IMementoRepository
    {
        private EventStorageLiteDatabase liteDatabase;

        public EventStorageRepository(EventStorageLiteDatabase liteDatabase)
        {
            this.liteDatabase = liteDatabase;
        }

        public IEnumerable<StoredEvent> GetEvents(Guid aggregateId, int afterVersion = 0)
        {
            return this.liteDatabase.GetCollection<StoredEvent>().Find(e => e.AggregateId == aggregateId && e.Version >= afterVersion);
        }

        public void Dispose()
        {
            this.liteDatabase.Dispose();
        }

        public Event GetLastEvent(Guid aggregateId)
        {
            return this.liteDatabase.GetCollection<StoredEvent>().Find(e => e.AggregateId == aggregateId).OrderBy(e => e.Timestamp).LastOrDefault();
        }

        public Memento GetMemento(Guid aggregateId)
        {
            return this.liteDatabase.GetCollection<Memento>().Find(m => m.aggregateId == aggregateId).OrderBy(m => m.Version).LastOrDefault();
        }

        public void SaveMemento(Memento memento)
        {
            this.liteDatabase.GetCollection<Memento>().Insert(memento);
        }

        public void Store(StoredEvent theEvent)
        {
            this.liteDatabase.GetCollection<StoredEvent>().Insert(theEvent);
        }
    }
}