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
        private readonly EventStorageLiteDatabase liteDatabase;

        public EventStorageRepository(EventStorageLiteDatabase liteDatabase)
        {
            this.liteDatabase = liteDatabase;
        }

        public IEnumerable<StoredEvent> GetEvents<TKey>(TKey eventId, int afterVersion = 0) where TKey : IEquatable<TKey>
        {
            return this.liteDatabase.GetCollection<StoredEvent>().Find(e => e.EventId == eventId.ToString() && e.Version >= afterVersion);
        }

        public void Dispose()
        {
            this.liteDatabase.Dispose();
        }

        public StoredEvent GetLastEvent<TKey>(TKey eventId) where TKey : IEquatable<TKey>
        {
            return this.liteDatabase.GetCollection<StoredEvent>().Find(e => e.EventId == eventId.ToString()).OrderBy(e => e.Timestamp).LastOrDefault();
        }

        public Memento GetMemento<TKey>(TKey aggregateId) where TKey : IEquatable<TKey>
        {
            return this.liteDatabase.GetCollection<Memento>().Find(m => m.AggregateId == aggregateId.ToString()).OrderBy(m => m.Version).LastOrDefault();
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