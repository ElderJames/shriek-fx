using Shriek.EventSourcing;
using System;
using System.Collections.Generic;
using Shriek.Events;
using Shriek.Storage;
using MongoDB.Driver;
using Shriek.Storage.Mementos;

namespace Shriek.EventStorage.MongoDB
{
    public class EventStorageRepository : IEventStorageRepository, IMementoRepository
    {
        private IEventStorageMongoDatabase database;

        private IMongoCollection<StoredEvent> eventStore => database.GetCollection<StoredEvent>("events");
        private IMongoCollection<Memento> mementStore => database.GetCollection<Memento>("mementos");

        public EventStorageRepository(IEventStorageMongoDatabase database)
        {
            this.database = database;
        }

        public IEnumerable<StoredEvent> All(Guid aggregateId)
        {
            return eventStore.Find(e => e.AggregateId == aggregateId).ToEnumerable();
        }

        public void Dispose()
        {
        }

        public Event GetLastEvent(Guid aggregateId)
        {
            return eventStore.Find(e => e.AggregateId == aggregateId).SortByDescending(e => e.Timestamp).FirstOrDefault();
        }

        public Memento GetMemento(Guid aggregateId)
        {
            return mementStore.Find(m => m.aggregateId == aggregateId).SortByDescending(m => m.Version).FirstOrDefault();
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