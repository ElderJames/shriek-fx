using System;

namespace Shriek.Storage.Mementos
{
    /// <summary>
    /// 备忘录模式接口
    /// </summary>
    public interface IOriginator<TKey> where TKey : IEquatable<TKey>
    {
        Memento<TKey> GetMemento();

        void SetMemento(Memento<TKey> memento);
    }
}