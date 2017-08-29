using Shriek.Storage.Mementos;
using System;

namespace Shriek.Storage
{
    /// <summary>
    /// 事件快照
    /// </summary>
    public interface IEventOriginator
    {
        Memento GetMemento(Guid aggregateId);

        void SaveMemento(Memento memento);
    }
}