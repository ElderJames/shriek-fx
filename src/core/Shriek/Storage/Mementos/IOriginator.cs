using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.Storage.Mementos
{
    /// <summary>
    /// 备忘录模式接口
    /// </summary>
    public interface IOriginator
    {
        Memento GetMemento();

        void SetMemento(Memento memento);
    }
}