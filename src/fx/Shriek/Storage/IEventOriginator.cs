using System;
using System.Collections.Generic;
using System.Text;
using Shriek.Storage.Mementos;

namespace Shriek.Storage
{
    /// <summary>
    /// 事件快照
    /// </summary>
    public interface IEventOriginator
    {
        T GetMemento<T>(Guid aggregateId) where T : Memento;

        void SaveMemento(Memento memento);
    }
}