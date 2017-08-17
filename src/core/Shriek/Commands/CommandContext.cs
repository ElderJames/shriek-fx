using JetBrains.Annotations;
using System.Xml.Linq;
using Shriek.Events;
using Shriek.Domains;
using System.Runtime.InteropServices.ComTypes;
using Shriek.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using Shriek.DependencyInjection;
using Shriek.Storage.Mementos;
using System.Linq;

namespace Shriek.Commands
{
    public class CommandContext : ICommandContext
    {
        private IServiceProvider Container;
        private Queue<AggregateRoot> aggregates;
        private static object _lock = new object();
        private IEventStorage eventStorage;

        public CommandContext(IServiceProvider Container)
        {
            this.Container = Container;
            eventStorage = Container.GetService<IEventStorage>();
        }

        public IDictionary<string, object> Items => new Dictionary<string, object>();

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

        public void SaveAggregateRoot<TAggregateRoot>(TAggregateRoot aggregate) where TAggregateRoot : AggregateRoot, IAggregateRoot, new()
        {
            if (aggregate.GetUncommittedChanges().Any())
            {
                //在锁内程序执行过程中，会有多次对该聚合根的更改请求
                lock (_lock)
                {
                    //如果不是新增事件
                    if (aggregate.Version != -1)
                    {
                        //从历史更改中回滚该聚合根的最后更改状态
                        var item = GetById<TAggregateRoot>(aggregate.AggregateId);
                        //如果正要执行的状态与历史中最后一次更改的状态不同，则抛异常，不执行这次更改
                        //（更改命令不会修改version，只有保存更改后聚合根记录的版本才被更新）
                        if (item.Version != aggregate.Version)
                        {
                            throw new Exception("与已保存的版本相同，无需更新");
                        }
                    }
                    //保存到事件存储
                    eventStorage.SaveAggregateRoot(aggregate);
                }
            }
        }
    }
}