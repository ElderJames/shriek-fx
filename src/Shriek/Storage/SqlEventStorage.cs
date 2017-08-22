using System.Reflection;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Shriek.Domains;
using Shriek.Events;
using Shriek.EventSourcing;

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
            var storeEvents = _eventStoreRepository.All(aggregateId);
            foreach (var e in storeEvents)
            {
                var eventType = Type.GetType(e.EventType);
                yield return JsonConvert.DeserializeObject(e.Data, eventType) as Event;
            }
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

        public void SaveAggregateRoot<TAggregateRoot>(TAggregateRoot aggregate) where TAggregateRoot : IAggregateRoot, IEventProvider
        {
            var uncommittedChanges = aggregate.GetUncommittedChanges();
            var version = aggregate.Version;

            foreach (var @event in uncommittedChanges)
            {
                version++;
                //if (version > 2)
                //{
                //    if (version % 3 == 0)
                //    {
                //        var originator = (IOriginator)aggregate;
                //        var memento = originator.GetMemento();
                //        memento.Version = version;
                //        SaveMemento(memento);
                //    }
                //}
                @event.Version = version;
                Save(@event);
            }
        }
    }
}