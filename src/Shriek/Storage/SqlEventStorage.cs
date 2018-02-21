using Newtonsoft.Json;
using Shriek.Events;
using Shriek.EventSourcing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Shriek.Storage
{
    public class SqlEventStorage : AbstractEventStorage, IEventStorage
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

        protected override IMementoRepository MementoRepository => mementoRepository;

        public override IEnumerable<Event> GetEvents<TKey>(TKey aggregateId, int afterVersion = 0)
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

        public override IEvent<TKey> GetLastEvent<TKey>(TKey aggregateId)
        {
            return GetEvents(aggregateId).LastOrDefault() as IEvent<TKey>;
        }

        public override void Save(Event theEvent)
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
    }
}