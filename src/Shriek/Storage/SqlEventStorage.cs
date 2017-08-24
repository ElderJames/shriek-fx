using System.Linq;
using System.Collections.Concurrent;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Shriek.Domains;
using Shriek.Events;
using Shriek.EventSourcing;
using Shriek.Storage.Mementos;

namespace Shriek.Storage
{
    public class SqlEventStorage : IEventStorage
    {
        private readonly IEventStorageRepository eventStoreRepository;
        private readonly IMementoRepository mementoRepository;
        private ConcurrentDictionary<Guid, ConcurrentBag<Event>> _events;

        public SqlEventStorage(IEventStorageRepository eventStoreRepository, IMementoRepository mementoRepository)
        {
            this.eventStoreRepository = eventStoreRepository;
            this.mementoRepository = mementoRepository;

            _events = new ConcurrentDictionary<Guid, ConcurrentBag<Event>>();
        }

        public IEnumerable<Event> GetEvents(Guid aggregateId)
        {
            _events.TryGetValue(aggregateId, out var events);
            if (events == null)
            {
                var storeEvents = eventStoreRepository.All(aggregateId);
                _events[aggregateId] = new ConcurrentBag<Event>();

                foreach (var e in storeEvents)
                {
                    var eventType = Type.GetType(e.EventType);
                    _events[aggregateId].Add(JsonConvert.DeserializeObject(e.Data, eventType) as Event);
                }
            }
            return _events[aggregateId].OrderBy(x => x.Timestamp);
        }

        public Event GetLastEvent(Guid aggregateId)
        {
            return GetEvents(aggregateId).LastOrDefault();
        }

        public void Save<T>(T theEvent) where T : Event
        {
            _events.TryGetValue(theEvent.AggregateId, out var events);
            if (events == null)
                _events[theEvent.AggregateId] = new ConcurrentBag<Event>();

            _events[theEvent.AggregateId].Add(theEvent);

            var serializedData = JsonConvert.SerializeObject(theEvent);

            var storedEvent = new StoredEvent(
                theEvent,
                serializedData,
                ""
                );

            eventStoreRepository.Store(storedEvent);
        }

        public void SaveAggregateRoot<TAggregateRoot>(TAggregateRoot aggregate) where TAggregateRoot : IAggregateRoot, IEventProvider
        {
            var uncommittedChanges = aggregate.GetUncommittedChanges();
            var version = aggregate.Version;

            foreach (var @event in uncommittedChanges)
            {
                version++;
                if (version > 2)
                {
                    if (version % 3 == 0)
                    {
                        var originator = (IOriginator)aggregate;
                        var memento = originator.GetMemento();
                        memento.Version = version;
                        mementoRepository.SaveMemento(memento);
                    }
                }
                @event.Version = version;
                Save(@event);
            }
        }

        public TAggregateRoot Source<TAggregateRoot>(Guid aggregateId) where TAggregateRoot : IAggregateRoot, IEventProvider, new()
        {
            IEnumerable<Event> events;
            Memento memento = null;
            var obj = new TAggregateRoot();

            if (obj is IOriginator)
            {
                //获取该记录的更改快照
                memento = mementoRepository.GetMemento(aggregateId);
            }

            if (memento != null)
            {
                //获取该记录最后一次快照之后的更改，避免加载过多历史更改
                events = GetEvents(aggregateId).Where(x => x.Version >= memento.Version);
                //从快照恢复
                ((IOriginator)obj).SetMemento(memento);
            }
            else
            {
                //获取所有历史更改记录
                events = GetEvents(aggregateId);
            }

            if (memento == null && !events.Any())
                return default(TAggregateRoot);

            //重现历史更改
            obj.LoadsFromHistory(events);
            return obj;
        }
    }
}