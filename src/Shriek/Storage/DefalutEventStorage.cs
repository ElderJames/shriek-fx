using Newtonsoft.Json;
using Shriek.Events;
using Shriek.EventSourcing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Shriek.Domains;
using Shriek.Reflection;
using Shriek.Storage.Mementos;

namespace Shriek.Storage
{
    public class DefalutEventStorage : IEventStorage
    {
        private readonly ConcurrentCache<string, ConcurrentBag<Event>> _eventsDict;

        public DefalutEventStorage()
        {
        }

        public DefalutEventStorage(IEventStorageRepository eventStoreRepository, IMementoRepository mementoRepository)
        {
            this.EventStorageRepository = eventStoreRepository;
            this.MementoRepository = mementoRepository;

            _eventsDict = new ConcurrentCache<string, ConcurrentBag<Event>>();
        }

        protected virtual IMementoRepository MementoRepository { get; }

        protected virtual IEventStorageRepository EventStorageRepository { get; }

        public virtual IEnumerable<Event> GetEvents<TKey>(TKey eventId, int afterVersion = 0)
            where TKey : IEquatable<TKey>
        {
            return _eventsDict.GetOrAdd(eventId.ToString(), x =>
            {
                var storeEvents = EventStorageRepository.GetEvents(eventId, afterVersion);
                var eventlist = new ConcurrentBag<Event>();
                foreach (var e in storeEvents)
                {
                    var eventType = Type.GetType(e.EventType);
                    eventlist.Add(JsonConvert.DeserializeObject(e.Data, eventType) as Event);
                }
                return eventlist;
            })
            .Where(e => e.Version >= afterVersion).OrderBy(e => e.Timestamp);
        }

        public virtual IEvent<TKey> GetLastEvent<TKey>(TKey eventId)
            where TKey : IEquatable<TKey>
        {
            return GetEvents(eventId).LastOrDefault() as IEvent<TKey>;
        }

        public virtual void Save(Event theEvent)
        {
            _eventsDict.AddOrUpdate(theEvent.EventId, new ConcurrentBag<Event> { theEvent }, (x, list) =>
            {
                list.Add(theEvent);
                return list;
            });

            var serializedData = JsonConvert.SerializeObject(theEvent);

            var storedEvent = new StoredEvent(
                theEvent,
                serializedData
                );

            EventStorageRepository.Store(storedEvent);
        }

        public void SaveAggregateRoot<TAggregateRoot>(TAggregateRoot aggregate) where TAggregateRoot : IAggregateRoot
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
                        MementoRepository.SaveMemento(memento);
                    }
                }
                @event.Version = version;
                Save(@event);
            }
        }

        public TAggregateRoot Source<TAggregateRoot, TKey>(TKey aggregateId)
            where TAggregateRoot : IAggregateRoot<TKey>
            where TKey : IEquatable<TKey>
        {
            IEnumerable<Event> events = Enumerable.Empty<Event>();
            Memento memento = null;

            var instance = New<TAggregateRoot>.Instance();

            //获取该记录的更改快照
            memento = MementoRepository.GetMemento(aggregateId);

            if (memento != null)
            {
                //获取该记录最后一次快照之后的更改，避免加载过多历史更改
                events = GetEvents(aggregateId, memento.Version);
                //从快照恢复
                ((IOriginator)instance).SetMemento(memento);
            }
            else
            {
                //获取所有历史更改记录
                events = GetEvents(aggregateId);
            }

            if (memento == null && !events.Any())
                return default(TAggregateRoot);

            //重现历史更改
            instance.LoadsFromHistory(events);
            return instance;
        }
    }
}