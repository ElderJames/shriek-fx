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
        Memento GetMemento(Guid aggregateId);

        void SaveMemento(Memento memento);
    }
}