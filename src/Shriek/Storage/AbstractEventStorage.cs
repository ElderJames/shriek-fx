using System;
using System.Collections.Generic;
using System.Linq;
using Shriek.Domains;
using Shriek.Events;
using Shriek.EventSourcing;
using Shriek.Storage.Mementos;

namespace Shriek.Storage
{
    public abstract class AbstractEventStorage : IEventStorage
    {
        protected abstract IMementoRepository MementoRepository { get; }

        public abstract IEnumerable<Event> GetEvents<TKey>(TKey aggregateId, int afterVersion = 0)
            where TKey : IEquatable<TKey>;

        public abstract IEvent<TKey> GetLastEvent<TKey>(TKey aggregateId) where TKey : IEquatable<TKey>;

        public abstract void Save(Event @event);

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
          where TAggregateRoot : IAggregateRoot<TKey>, new()
          where TKey : IEquatable<TKey>
        {
            IEnumerable<Event> events = Enumerable.Empty<Event>();
            Memento memento = null;
            var obj = new TAggregateRoot();

            //获取该记录的更改快照
            memento = MementoRepository.GetMemento(aggregateId);

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
