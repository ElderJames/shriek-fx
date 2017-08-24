using Shriek.EventSourcing;
using System;
using System.Collections.Generic;
using System.Text;
using Shriek.Storage.Mementos;
using System.Linq;

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