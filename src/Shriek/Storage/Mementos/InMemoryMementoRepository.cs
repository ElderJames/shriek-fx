using Shriek.EventSourcing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shriek.Storage.Mementos
{
    public class InMemoryMementoRepository : IMementoRepository
    {
        private readonly List<Memento> mementoes;

        public InMemoryMementoRepository()
        {
            mementoes = new List<Memento>();
        }

        public Memento GetMemento<TKey>(TKey aggregateId) where TKey : IEquatable<TKey>
        {
            return this.mementoes.Where(x => x.AggregateId == aggregateId.ToString())
                .OrderBy(x => x.Timestamp)
                .LastOrDefault();
        }

        public void SaveMemento(Memento memento)
        {
            mementoes.Add(memento);
        }
    }
}
