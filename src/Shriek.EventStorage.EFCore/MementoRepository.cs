using Shriek.EventSourcing;
using Shriek.Storage.Mementos;
using System;

namespace Shriek.EventStorage.EFCore
{
    public class MementoRepository : IMementoRepository
    {
        public Memento GetMemento(Guid aggregateId)
        {
            throw new NotImplementedException();
        }

        public void SaveMemento(Memento memento)
        {
            throw new NotImplementedException();
        }
    }
}