using Shriek.Storage.Mementos;
using System;

namespace Shriek.Storage
{
    /// <summary>
    /// 事件快照
    /// </summary>
    public interface IEventOriginator
    {
        Memento<TKey> GetMemento<TKey>(TKey aggregateId) where TKey : IEquatable<TKey>;

        void SaveMemento<TKey>(Memento<TKey> memento) where TKey : IEquatable<TKey>;
    }
}