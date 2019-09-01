using Microsoft.Extensions.DependencyInjection;
using Shriek.Domains;
using Shriek.Events;
using Shriek.Storage;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Shriek.Commands
{
    public class DefaultCommandContext : ICommandContext, ICommandContextSave
    {
        private readonly ConcurrentQueue<IAggregateRoot> aggregates;
        private static readonly object Lock = new object();
        private readonly IEventBus eventBus;
        private readonly IEventStorage eventStorage;

        public DefaultCommandContext(IServiceProvider container)
        {
            eventStorage = container.GetService<IEventStorage>();
            eventBus = container.GetService<IEventBus>();
            aggregates = new ConcurrentQueue<IAggregateRoot>();
        }

        public IDictionary<string, object> Items => new Dictionary<string, object>();

        /// <summary>
        /// 从内存获取聚合，获取不到则使用委托从数据库获取
        /// </summary>
        /// <typeparam name="TAggregateRoot"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="key"></param>
        /// <param name="initFromRepository"></param>
        /// <returns></returns>
        public TAggregateRoot GetAggregateRoot<TKey, TAggregateRoot>(TKey key, Func<TAggregateRoot> initFromRepository)
             where TAggregateRoot : IAggregateRoot<TKey>
             where TKey : IEquatable<TKey>
        {
            var obj = GetById<TKey, TAggregateRoot>(key);
            if (obj == null)
                obj = initFromRepository();

            if (obj != null)
                aggregates.Enqueue(obj);

            return obj;
        }

        public TAggregateRoot GetAggregateRoot<TKey, TAggregateRoot>(TKey key)
            where TAggregateRoot : IAggregateRoot<TKey>
            where TKey : IEquatable<TKey>
        {
            var obj = GetById<TKey, TAggregateRoot>(key);
            if (obj != null)
                aggregates.Enqueue(obj);

            return obj;
        }

        private TAggregateRoot GetById<TKey, TAggregateRoot>(TKey id)
            where TAggregateRoot : IAggregateRoot<TKey>
            where TKey : IEquatable<TKey>
        {
            return eventStorage.Source<TAggregateRoot, TKey>(id);
        }

        public void Save()
        {
            for (var i = 0; i < aggregates.Count; i++)
            {
                if (aggregates.TryDequeue(out var root) && root.CanCommit)
                {
                    SaveAggregateRoot(root);
                }
            }
        }

        public void SaveAggregateRoot<TAggregateRoot>(TAggregateRoot aggregate)
            where TAggregateRoot : IAggregateRoot
        {
            if (aggregate.GetUncommittedChanges().Any())
            {
                //在锁内程序执行过程中，会有多次对该聚合根的更改请求
                lock (Lock)
                {
                    //如果不是新增事件
                    if (aggregate.Version != 0)
                    {
                        var lastEvent = eventStorage.GetLastEvent(((dynamic)aggregate).AggregateId);
                        if (lastEvent != null && lastEvent.Version != aggregate.Version)
                        {
                            throw new Exception("事件库中该聚合的状态版本与当前传入聚合状态版本不同，可能已被更新");
                        }
                    }

                    //保存到事件存储
                    eventStorage.SaveAggregateRoot(aggregate);
                    foreach (var @event in aggregate.GetUncommittedChanges())
                    {
                        eventBus.Publish(@event);
                    }
                }
                aggregate.MarkChangesAsCommitted();
            }
        }
    }
}