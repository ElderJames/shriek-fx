using Shriek.Domains;
using Shriek.Events;
using System;
using System.Collections.Generic;

namespace Shriek.Storage
{
    public interface IEventStorage
    {
        IEnumerable<Event> GetEvents(Guid aggregateId);

        void SaveAggregateRoot<TAggregateRoot>(TAggregateRoot aggregate) where TAggregateRoot : IAggregateRoot, IEventProvider;

        /// <summary>
        /// 事件溯源，获取聚合根
        /// </summary>
        /// <typeparam name="TAggregateRoot">聚合根类型</typeparam>
        /// <param name="aggregateId">唯一标识</param>
        TAggregateRoot Source<TAggregateRoot>(Guid aggregateId) where TAggregateRoot : IAggregateRoot, IEventProvider, new();

        Event GetLastEvent(Guid aggregateId);

        void Save<T>(T @event) where T : Event;
    }
}