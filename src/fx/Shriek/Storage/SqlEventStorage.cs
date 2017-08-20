using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Shriek.Domains;
using Shriek.Events;
using Shriek.EventSourcing;
using Shriek.Storage.Mementos;

namespace Shriek.Storage
{
    public class SqlEventStorage : IEventStorage
    {
        private readonly IEventStorageRepository _eventStoreRepository;

        public SqlEventStorage(IEventStorageRepository eventStoreRepository)
        {
            _eventStoreRepository = eventStoreRepository;
        }

        public IEnumerable<Event> GetEvents(Guid aggregateId)
        {
            throw new NotImplementedException();
        }

        public T GetMemento<T>(Guid aggregateId) where T : Memento
        {
            throw new NotImplementedException();
        }

        public void Save<T>(T theEvent) where T : Event
        {
            var serializedData = JsonConvert.SerializeObject(theEvent);

            var storedEvent = new StoredEvent(
                theEvent,
                serializedData,
                ""
                );

            _eventStoreRepository.Store(storedEvent);
        }

        public void SaveAggregateRoot<TAggregateRoot>(TAggregateRoot aggreagate) where TAggregateRoot : IAggregateRoot, IEventProvider
        {
            throw new NotImplementedException();
        }

        public void SaveMemento(Memento memento)
        {
            throw new NotImplementedException();
        }
    }
}