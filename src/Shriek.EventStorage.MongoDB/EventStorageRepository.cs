using MongoDB.Driver;
using Shriek.EventSourcing;
using Shriek.Storage;
using Shriek.Storage.Mementos;
using System;
using System.Collections.Generic;

namespace Shriek.EventStorage.MongoDB
{
    public class EventStorageRepository : IEventStorageRepository, IMementoRepository
    {
        private IMongoDatabase database;

        private IMongoCollection<StoredEvent> eventStore => database.GetCollection<StoredEvent>("events");
        private IMongoCollection<Memento> mementStore => database.GetCollection<Memento>("mementos");

        public EventStorageRepository(MongoDatabase database)
        {
            this.database = database.Database;
        }

        public IEnumerable<StoredEvent> GetEvents(Guid aggregateId, int afterVersion = 0)
        {
            return eventStore.Find(e => e.AggregateId == aggregateId && e.Version >= afterVersion).ToEnumerable();
        }

        public void Dispose()
        {
        }

        public StoredEvent GetLastEvent(Guid aggregateId)
        {
            return eventStore.Find(e => e.AggregateId == aggregateId).SortByDescending(e => e.Timestamp).FirstOrDefault();
        }

        public Memento GetMemento(Guid aggregateId)
        {
            return mementStore.Find(m => m.AggregateId == aggregateId).SortByDescending(m => m.Version).FirstOrDefault();
        }

        public void SaveMemento(Memento memento)
        {
            mementStore.InsertOne(memento);
        }

        public void Store(StoredEvent theEvent)
        {
            eventStore.InsertOne(theEvent);
        }
    }
}