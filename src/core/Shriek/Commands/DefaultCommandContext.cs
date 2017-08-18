using Shriek.Events;
using Shriek.Domains;
using Shriek.Storage;
using System;
using System.Collections.Generic;
using Shriek.Storage.Mementos;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Shriek.Commands
{
    public class DefaultCommandContext : ICommandContext
    {
        private IServiceProvider Container;
        private Queue<AggregateRoot> aggregates = null;
        private static object _lock = new object();
        private IEventStorage eventStorage;

        public DefaultCommandContext(IServiceProvider Container)
        {
            this.Container = Container;
            eventStorage = Container.GetService<IEventStorage>();
            aggregates = new Queue<AggregateRoot>();
        }

        public IDictionary<string, object> Items => new Dictionary<string, object>();

        /// <summary>
        /// 从内存获取聚合，获取不到则使用委托从数据库获取
        /// </summary>
        /// <typeparam name="TAggregateRoot"></typeparam>
        /// <param name="key"></param>
        /// <param name="initFromRepository"></param>
        /// <returns></returns>
        TAggregateRoot ICommandContext.GetAggregateRoot<TAggregateRoot>(Guid key, Func<TAggregateRoot> initFromRepository)
        {
            var obj = GetById<TAggregateRoot>(key);
            if (obj == null)
                obj = initFromRepository();

            if (obj != null)
                aggregates.Enqueue(obj);

            return obj;
        }

        private TAggregateRoot GetById<TAggregateRoot>(Guid Id) where TAggregateRoot : AggregateRoot, new()
        {
            //获取该记录的所有缓存事件
            IEnumerable<Event> events;
            var obj = new TAggregateRoot();
            //获取该记录的更改快照
            var memento = eventStorage.GetMemento<Memento>(Id);
            if (memento != null)
            {
                //获取该记录最后一次快照之后的更改，避免加载过多历史更改
                events = eventStorage.GetEvents(Id).Where(x => x.Version >= memento.Version);
                //从快照恢复
                ((IOriginator)obj).SetMemento(memento);
            }
            else
            {
                //获取所有历史更改记录
                events = eventStorage.GetEvents(Id);
            }

            if (!events.Any())
                return null;

            //重现历史更改
            obj.LoadsFromHistory(events);
            return obj;
        }

        public void Save()
        {
            for (var i = 0; i < aggregates.Count; i++)
            {
                var root = aggregates.Dequeue();
                SaveAggregateRoot(root);
            }
        }

        public void SaveAggregateRoot<TAggregateRoot>(TAggregateRoot aggregate) where TAggregateRoot : AggregateRoot, IAggregateRoot
        {
            if (aggregate.GetUncommittedChanges().Any())
            {
                //在锁内程序执行过程中，会有多次对该聚合根的更改请求
                lock (_lock)
                {
                    //如果不是新增事件
                    if (aggregate.Version != -1)
                    {
                        var lastestEvent = eventStorage.GetEvents(aggregate.AggregateId).OrderBy(x => x.Version).LastOrDefault();
                        if (lastestEvent != null && lastestEvent.Version != aggregate.Version)
                        {
                            throw new Exception("事件库中该聚合的状态版本与当前传入聚合状态版本不同，可能已被更新");
                        }
                    }
                    //{
                    //    //从历史更改中回滚该聚合根的最后更改状态
                    //    var item = GetById<TAggregateRoot>(aggregate.AggregateId);
                    //    //如果正要执行的状态与历史中最后一次更改的状态不同，则抛异常，不执行这次更改
                    //    //（更改命令不会修改version，只有保存更改后聚合根记录的版本才被更新）
                    //    if (item.Version != aggregate.Version)
                    //    {
                    //        throw new Exception("与已保存的版本相同，无需更新");
                    //    }
                    //}
                    //保存到事件存储
                    eventStorage.SaveAggregateRoot(aggregate);
                }
            }
        }
    }
}