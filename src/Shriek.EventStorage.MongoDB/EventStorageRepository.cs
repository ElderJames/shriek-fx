using MongoDB.Driver;
using Shriek.Events;
using Shriek.EventSourcing;
using Shriek.Storage;
using Shriek.Storage.Mementos;
using System;
using System.Collections.Generic;

namespace Shriek.EventStorage.MongoDB
{
    public class EventStorageRepository : IEventStorageRepository, IMementoRepository
    {
        private readonly IMongoDatabase database;

        private IMongoCollection<StoredEvent> EventStore => database.GetCollection<StoredEvent>("events");
        private IMongoCollection<Memento> MementStore => database.GetCollection<Memento>("mementos");

        public EventStorageRepository(MongoDatabase database)
        {
            this.database = database.Database;
        }

        public IEnumerable<StoredEvent> GetEvents<TKey>(TKey aggregateId, int afterVersion = 0)
            where TKey : IEquatable<TKey>
        {
            return EventStore.Find(e => e.AggregateId == aggregateId.ToString() && e.Version >= afterVersion).ToEnumerable();
        }

        public void Dispose()
        {
        }

        public Event GetLastEvent<TKey>(TKey aggregateId)
            where TKey : IEquatable<TKey>
        {
            return EventStore.Find(e => e.AggregateId == aggregateId.ToString()).SortByDescending(e => e.Timestamp).FirstOrDefault();
        }

        public Memento GetMemento<TKey>(TKey aggregateId)
            where TKey : IEquatable<TKey>
        {
            return MementStore.Find(m => m.AggregateId == aggregateId.ToString()).SortByDescending(m => m.Version).FirstOrDefault();
        }

        public void SaveMemento(Memento memento)
        {
            MementStore.InsertOne(memento);
        }

        public void Store(StoredEvent theEvent)
        {
            EventStore.InsertOne(theEvent);
        }
    }
}