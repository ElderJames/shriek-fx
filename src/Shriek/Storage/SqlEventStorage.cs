using Newtonsoft.Json;
using Shriek.Domains;
using Shriek.Events;
using Shriek.EventSourcing;
using Shriek.Storage.Mementos;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Shriek.Storage
{
    public class SqlEventStorage : IEventStorage
    {
        private readonly IEventStorageRepository _eventStoreRepository;
        private readonly IMementoRepository _mementoRepository;
        private readonly ConcurrentDictionary<Guid, ConcurrentBag<Event>> _eventsDict;

        public SqlEventStorage(IEventStorageRepository eventStoreRepository, IMementoRepository mementoRepository)
        {
            this._eventStoreRepository = eventStoreRepository;
            this._mementoRepository = mementoRepository;

            _eventsDict = new ConcurrentDictionary<Guid, ConcurrentBag<Event>>();
        }

        public IEnumerable<Event> GetEvents(Guid aggregateId, int afterVersion = 0)
        {
            return _eventsDict.GetOrAdd(aggregateId, x =>
            {
                var storeEvents = _eventStoreRepository.GetEvents(aggregateId, afterVersion);
                var eventlist = new ConcurrentBag<Event>();
                foreach (var e in storeEvents)
                {
                    var eventType = Type.GetType(e.MessageType);
                    eventlist.Add(JsonConvert.DeserializeObject(e.Data, eventType) as Event);
                }
                return eventlist;
            })
            .Where(e => e.Version >= afterVersion).OrderBy(e => e.Timestamp);
        }

        public Event GetLastEvent(Guid aggregateId)
        {
            return GetEvents(aggregateId).LastOrDefault();
        }

        public void Save<T>(T theEvent) where T : Event
        {
            _eventsDict.AddOrUpdate(theEvent.AggregateId, new ConcurrentBag<Event> { theEvent }, (key, list) =>
            {
                list.Add(theEvent);
                return list;
            });

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
                if (version > 2)
                {
                    if (version % 3 == 0)
                    {
                        var originator = (IOriginator)aggregate;
                        var memento = originator.GetMemento();
                        memento.Version = version;
                        _mementoRepository.SaveMemento(memento);
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
                memento = _mementoRepository.GetMemento(aggregateId);
            }

            if (memento != null)
            {
                //获取该记录最后一次快照之后的更改，避免加载过多历史更改
                events = GetEvents(aggregateId, memento.Version);
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