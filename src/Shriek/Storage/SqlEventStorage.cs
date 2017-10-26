using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ConcurrentDictionary<string, ConcurrentBag<Event>> _eventsDict;

        public SqlEventStorage(IEventStorageRepository eventStoreRepository, IMementoRepository mementoRepository)
        {
            this.eventStoreRepository = eventStoreRepository;
            this.mementoRepository = mementoRepository;

            _eventsDict = new ConcurrentDictionary<string, ConcurrentBag<Event>>();
        }

        public IEnumerable<Event> GetEvents<TKey>(TKey aggregateId, int afterVersion = 0) where TKey : IEquatable<TKey>
        {
            return _eventsDict.GetOrAdd(aggregateId.ToString(), x =>
            {
                var storeEvents = eventStoreRepository.GetEvents(aggregateId, afterVersion);
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

        public IEvent<TKey> GetLastEvent<TKey>(TKey aggregateId) where TKey : IEquatable<TKey>
        {
            return GetEvents(aggregateId).LastOrDefault() as IEvent<TKey>;
        }

        public void Save(Event theEvent)
        {
            Func<string, ConcurrentBag<Event>, ConcurrentBag<Event>> func = (x, list) =>
            {
                list.Add(theEvent);
                return list;
            };

            _eventsDict.AddOrUpdate(((dynamic)theEvent).AggregateId.ToString(), new ConcurrentBag<Event> { theEvent }, func);

            var serializedData = JsonConvert.SerializeObject(theEvent);

            var storedEvent = new StoredEvent(
                ((dynamic)theEvent).AggregateId.ToString(),
                serializedData,
                theEvent.Version,
                ""
                );

            eventStoreRepository.Store(storedEvent);
        }

        public void SaveAggregateRoot<TAggregateRoot>(TAggregateRoot aggregate) where TAggregateRoot : AggregateRoot
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

        public TAggregateRoot Source<TAggregateRoot, TKey>(TKey aggregateId)
            where TAggregateRoot : AggregateRoot, new()
            where TKey : IEquatable<TKey>
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