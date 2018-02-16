using Shriek.Domains;
using Shriek.Events;
using Shriek.Storage.Mementos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shriek.Storage
{
    public class InMemoryEventStorage : IEventStorage, IEventOriginator
    {
        private readonly List<Event> events;
        private readonly List<Memento> mementoes;

        public InMemoryEventStorage()
        {
            events = new List<Event>();
            mementoes = new List<Memento>();
        }

        public IEnumerable<Event> GetEvents<TKey>(TKey aggregateId, int afterVersion = 0) where TKey : IEquatable<TKey>
        {
            var list = this.events.Select(x => x as Event<TKey>).Where(x => x.AggregateId.Equals(aggregateId));

            var events = list.Where(e => e.AggregateId.Equals(aggregateId) && e.Version >= afterVersion);

            return events;
        }

        public IEvent<TKey> GetLastEvent<TKey>(TKey aggregateId) where TKey : IEquatable<TKey>
        {
            var list = events.Select(x => x as IEvent<TKey>);

            return list.Where(e => e.AggregateId.Equals(aggregateId))
                .OrderBy(e => e.Version).LastOrDefault();
        }

        public void SaveAggregateRoot<TAggregateRoot>(TAggregateRoot aggregate)
            where TAggregateRoot : IAggregateRoot
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
                        SaveMemento(memento);
                    }
                }
                @event.Version = version;
                Save(@event);
            }
        }

        public void SaveMemento(Memento memento)
        {
            mementoes.Add(memento);
        }

        public void Save(Event @event)
        {
            events.Add(@event);
        }

        public Memento GetMemento<TKey>(TKey aggregateId) where TKey : IEquatable<TKey>
        {
            return this.mementoes.Where(x => x.AggregateId == aggregateId.ToString())
                .OrderBy(x => x.Timestamp)
                .LastOrDefault();
        }

        public TAggregateRoot Source<TAggregateRoot, TKey>(TKey aggregateId)
            where TAggregateRoot : IAggregateRoot<TKey>, new()
            where TKey : IEquatable<TKey>
        {
            //获取该记录的所有缓存事件
            IEnumerable<Event> events;
            Memento memento = null;
            var obj = new TAggregateRoot();

            //获取该记录的更改快照
            memento = GetMemento(aggregateId);

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