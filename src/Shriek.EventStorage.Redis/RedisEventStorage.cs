using Newtonsoft.Json;
using Shriek.Events;
using Shriek.EventSourcing;
using Shriek.Storage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shriek.EventStorage.Redis
{
    public class RedisEventStorage : DefalutEventStorage
    {
        private readonly ICacheService cacheService;
        private const string EventCachePrefix = "event_cache_";
        private readonly object locker = new object();

        protected override IMementoRepository MementoRepository { get; }

        protected override IEventStorageRepository EventStorageRepository { get; }

        public RedisEventStorage(ICacheService cacheService, IEventStorageRepository eventStorageRepository, IMementoRepository mementoRepository)
        {
            this.cacheService = cacheService;
            this.MementoRepository = mementoRepository;
            this.EventStorageRepository = eventStorageRepository;
        }

        public override IEnumerable<Event> GetEvents<TKey>(TKey eventId, int afterVersion = 0)
        {
            lock (locker)
            {
                var events = cacheService.Get<IEnumerable<Event>>(EventCachePrefix + eventId) ?? Enumerable.Empty<Event>();
                if (!events.Any())
                {
                    var storeEvents = EventStorageRepository.GetEvents(eventId, afterVersion);
                    var eventlist = new List<Event>();
                    foreach (var e in storeEvents)
                    {
                        var eventType = Type.GetType(e.MessageType);
                        eventlist.Add(JsonConvert.DeserializeObject(e.Data, eventType) as Event);
                    }

                    if (eventlist.Any())
                    {
                        cacheService.Store(EventCachePrefix + eventId, eventlist);
                        events = eventlist;
                    }
                }

                return events.Where(e => e.Version >= afterVersion).OrderBy(e => e.Timestamp);
            }
        }

        public override IEvent<TKey> GetLastEvent<TKey>(TKey eventId)
        {
            return GetEvents(eventId).LastOrDefault() as IEvent<TKey>;
        }

        public override void Save(Event @event)
        {
            lock (locker)
            {
                var events = cacheService.Get<IEnumerable<Event>>(EventCachePrefix + @event.EventId) ?? Enumerable.Empty<Event>();
                if (!events.Any())
                {
                    events = new[] { @event };
                    cacheService.Store(EventCachePrefix + @event.EventId, events);
                }
                else
                {
                    events = events.Concat(new[] { @event });
                    cacheService.Store(EventCachePrefix + @event.EventId, events);
                }

                var serializedData = JsonConvert.SerializeObject(@event);

                var storedEvent = new StoredEvent(
                    @event.EventId,
                    serializedData,
                    @event.Version,
                    ""
                );

                EventStorageRepository.Store(storedEvent);
            }
        }
    }
}