using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Shriek.Domains;
using Shriek.Events;
using Shriek.EventSourcing;
using Shriek.Storage;
using Shriek.Storage.Mementos;

namespace Shriek.EventStorage.Redis
{
    public class RedisEventStorage : IEventStorage
    {
        private readonly ICacheService cacheService;
        private readonly IEventStorageRepository eventStorageRepository;
        private readonly IMementoRepository mementoRepository;
        private const string EventCachePrefix = "event_cache_";
        private readonly object locker = new object();

        public RedisEventStorage(ICacheService cacheService, IEventStorageRepository eventStorageRepository, IMementoRepository mementoRepository)
        {
            this.cacheService = cacheService;
            this.eventStorageRepository = eventStorageRepository;
            this.mementoRepository = mementoRepository;
        }

        public IEnumerable<Event> GetEvents(Guid aggregateId, int afterVersion = 0)
        {
            lock (locker)
            {
                var events = cacheService.Get<IEnumerable<Event>>(EventCachePrefix + aggregateId) ?? new Event[0];
                if (!events.Any())
                {
                    var storeEvents = eventStorageRepository.GetEvents(aggregateId, afterVersion);
                    var eventlist = new List<Event>();
                    foreach (var e in storeEvents)
                    {
                        var eventType = Type.GetType(e.MessageType);
                        eventlist.Add(JsonConvert.DeserializeObject(e.Data, eventType) as Event);
                    }

                    if (eventlist.Any())
                    {
                        cacheService.Store(EventCachePrefix + aggregateId, eventlist);
                        events = eventlist;
                    }
                }

                return events.Where(e => e.Version >= afterVersion).OrderBy(e => e.Timestamp);
            }
        }

        public Event GetLastEvent(Guid aggregateId)
        {
            return GetEvents(aggregateId).LastOrDefault();
        }

        public void Save<T>(T @event) where T : Event
        {
            lock (locker)
            {
                var events = cacheService.Get<IEnumerable<Event>>(EventCachePrefix + @event.AggregateId) ?? new Event[0];
                if (!events.Any())
                {
                    events = new[] { @event };
                    cacheService.Store(EventCachePrefix + @event.AggregateId, events);
                }
                else
                {
                    events = events.Concat(new[] { @event });
                    cacheService.Store(EventCachePrefix + @event.AggregateId, events);
                }

                var serializedData = JsonConvert.SerializeObject(@event);

                var storedEvent = new StoredEvent(
                    @event,
                    serializedData,
                    ""
                );

                eventStorageRepository.Store(storedEvent);
            }
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