using System;
using System.Collections.Generic;
using System.Linq;
using Shriek.Events;
using Shriek.Domains;
using Shriek.Storage.Mementos;

namespace Shriek.Storage
{
    public class InMemoryEventStorage : IEventStorage, IEventOriginator
    {
        private List<Event> _events;
        private List<Memento> _mementoes;

        public InMemoryEventStorage()
        {
            _events = new List<Event>();
            _mementoes = new List<Memento>();
        }

        public IEnumerable<Event> GetEvents(Guid aggregateId)
        {
            var events = _events.Where(p => p.AggregateId == aggregateId);

            return events;
        }

        public Event GetLastEvent(Guid aggregateId)
        {
            return _events.Where(p => p.AggregateId == aggregateId)
                .OrderBy(x => x.Version).LastOrDefault();
        }

        public void SaveAggregateRoot<TAggregateRoot>(TAggregateRoot aggregate) where TAggregateRoot : IEventProvider, IAggregateRoot
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

        public T GetMemento<T>(Guid aggregateId) where T : Memento
        {
            var memento = _mementoes.Where(m => m.Id == aggregateId).OrderBy(m => m.Version).LastOrDefault();
            if (memento != null)
            {
                return (T)memento;
            }
            return null;
        }

        public void SaveMemento(Memento memento)
        {
            _mementoes.Add(memento);
        }

        public void Save<T>(T @event) where T : Event
        {
            _events.Add(@event);
        }

        public TAggregateRoot Source<TAggregateRoot>(Guid aggregateId) where TAggregateRoot : IAggregateRoot, IEventProvider, new()
        {
            //获取该记录的所有缓存事件
            IEnumerable<Event> events;
            Memento memento = null;
            var obj = new TAggregateRoot();

            if (obj is IOriginator)
            {
                //获取该记录的更改快照
                memento = GetMemento<Memento>(aggregateId);
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