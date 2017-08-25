using System;
using System.Collections.Generic;
using Shriek.Events;
using Shriek.EventSourcing;
using Shriek.Storage;
using Shriek.Storage.Mementos;

namespace Shriek.EventStorage.LiteDB
{
    public class EventStorageRepository : IEventStorageRepository, IMementoRepository
    {
        public IEnumerable<StoredEvent> All(Guid aggregateId)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Event GetLastEvent(Guid aggregateId)
        {
            throw new NotImplementedException();
        }

        public Memento GetMemento(Guid aggregateId)
        {
            throw new NotImplementedException();
        }

        public void SaveMemento(Memento memento)
        {
            throw new NotImplementedException();
        }

        public void Store(StoredEvent theEvent)
        {
            throw new NotImplementedException();
        }
    }
}