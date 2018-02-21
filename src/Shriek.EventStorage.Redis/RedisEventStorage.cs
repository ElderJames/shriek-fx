using Newtonsoft.Json;
using Shriek.Domains;
using Shriek.Events;
using Shriek.EventSourcing;
using Shriek.Storage;
using Shriek.Storage.Mementos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shriek.EventStorage.Redis
{
    public class RedisEventStorage : AbstractEventStorage, IEventStorage
    {
        private readonly ICacheService cacheService;
        private readonly IEventStorageRepository eventStorageRepository;
        private readonly IMementoRepository mementoRepository;
        private const string EventCachePrefix = "event_cache_";
        private readonly object locker = new object();

        protected override IMementoRepository MementoRepository => mementoRepository;

        public RedisEventStorage(ICacheService cacheService, IEventStorageRepository eventStorageRepository, IMementoRepository mementoRepository)
        {
            this.cacheService = cacheService;
            this.eventStorageRepository = eventStorageRepository;
            this.mementoRepository = mementoRepository;
        }

        public override IEnumerable<Event> GetEvents<TKey>(TKey aggregateId, int afterVersion = 0)
        {
            lock (locker)
            {
                var events = cacheService.Get<IEnumerable<Event>>(EventCachePrefix + aggregateId) ?? Enumerable.Empty<Event>();
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

        public override IEvent<TKey> GetLastEvent<TKey>(TKey aggregateId)
        {
            return GetEvents(aggregateId).LastOrDefault() as IEvent<TKey>;
        }

        public override void Save(Event @event)
        {
            lock (locker)
            {
                var events = cacheService.Get<IEnumerable<Event>>(EventCachePrefix + ((dynamic)@event).AggregateId) ?? Enumerable.Empty<Event>();
                if (!events.Any())
                {
                    events = new[] { @event };
                    cacheService.Store(EventCachePrefix + ((dynamic)@event).AggregateId, events);
                }
                else
                {
                    events = events.Concat(new[] { @event });
                    cacheService.Store(EventCachePrefix + ((dynamic)@event).AggregateId, events);
                }

                var serializedData = JsonConvert.SerializeObject(@event);

                var storedEvent = new StoredEvent(
                    ((dynamic)@event).AggregateId.ToString(),
                    serializedData,
                    @event.Version,
                    ""
                );

                eventStorageRepository.Store(storedEvent);
            }
        }

      
    }
}