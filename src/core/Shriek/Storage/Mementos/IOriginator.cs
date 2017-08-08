using Shriek.Domains;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.Storage.Mementos
{
    public interface IOriginator
    {
        Memento GetMemento();

        void SetMemento(Memento memento);
    }
}