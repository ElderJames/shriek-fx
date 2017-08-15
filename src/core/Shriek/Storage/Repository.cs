using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Shriek.Domains;
using System;
using System.Collections.Generic;
using System.Text;
using Shriek.Events;
using Shriek.Storage.Mementos;
using System.Linq;

namespace Shriek.Storage
{
    public class Repository<T> : IRepository<T> where T : AggregateRoot, new()
    {
        private readonly IEventStorage _eventStorage;
        private static object _lock = new object();

        public Repository(IEventStorage eventStorage)
        {
            _eventStorage = eventStorage;
        }

        public T GetById(Guid id)
        {
            //获取该记录的所有缓存事件
            IEnumerable<Event> events;
            var obj = new T();
            //获取该记录的更改快照
            var memento = _eventStorage.GetMemento<Memento>(id);
            if (memento != null)
            {
                //获取该记录最后一次快照之后的更改，避免加载过多历史更改
                events = _eventStorage.GetEvents(id).Where(x => x.Version >= memento.Version);
                //从快照恢复
                ((IOriginator)obj).SetMemento(memento);
            }
            else
            {
                //获取所有历史更改记录
                events = _eventStorage.GetEvents(id);
            }
            //重现历史更改
            obj.LoadsFromHistory(events);
            return obj;
        }

        public void Save(AggregateRoot aggregate, int expectedVersion)
        {
            if (aggregate.GetUncommittedChanges().Any())
            {
                //在锁内程序执行过程中，会有多次对该聚合根的更改请求
                lock (_lock)
                {
                    //如果不是新增事件
                    if (expectedVersion != -1)
                    {
                        //从历史更改中回滚该聚合根的最后更改状态
                        var item = GetById(aggregate.AggregateId);
                        //如果正要执行的状态与历史中最后一次更改的状态不同，则抛异常，不执行这次更改
                        //（更改命令不会修改version，只有保存更改后聚合根记录的版本才被更新）
                        if (item.Version != expectedVersion)
                        {
                            throw new Exception();
                        }
                    }
                    //保存到事件存储
                    _eventStorage.Save(aggregate);
                }
            }
        }
    }
}