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
        private readonly EventStorageLiteDatabase _liteDatabase;

        public EventStorageRepository(EventStorageLiteDatabase liteDatabase)
        {
            this._liteDatabase = liteDatabase;
        }

        public IEnumerable<StoredEvent> GetEvents(Guid aggregateId, int afterVersion = 0)
        {
            return this._liteDatabase.GetCollection<StoredEvent>().Find(e => e.AggregateId == aggregateId && e.Version >= afterVersion);
        }

        public void Dispose()
        {
            this._liteDatabase.Dispose();
        }

        public StoredEvent GetLastEvent(Guid aggregateId)
        {
            return this._liteDatabase.GetCollection<StoredEvent>().Find(e => e.AggregateId == aggregateId).OrderBy(e => e.Timestamp).LastOrDefault();
        }

        public Memento GetMemento(Guid aggregateId)
        {
            return this._liteDatabase.GetCollection<Memento>().Find(m => m.AggregateId == aggregateId).OrderBy(m => m.Version).LastOrDefault();
        }

        public void SaveMemento(Memento memento)
        {
            this._liteDatabase.GetCollection<Memento>().Insert(memento);
        }

        public void Store(StoredEvent theEvent)
        {
            this._liteDatabase.GetCollection<StoredEvent>().Insert(theEvent);
        }
    }
}